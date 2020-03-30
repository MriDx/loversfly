using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Gigamole.Navigationtabbar.Ntb;
using Com.Theartofdev.Edmodo.Cropper;
using Java.IO;
using Newtonsoft.Json;
using Q.Rorbin.Badgeview;
using QuickDate.Activities.Chat;
using QuickDate.Activities.Chat.Service;
using QuickDate.Activities.Tabbes.Fragment;
using QuickDate.Activities.UserProfile;
using QuickDate.ButtomSheets;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.OneSignal;
using QuickDate.PaymentGoogle;
using QuickDate.SQLite;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Xamarin.PayPal.Android;
using Console = System.Console;
using Uri = Android.Net.Uri;
using Exception = System.Exception;
using Permission = Android.Content.PM.Permission;

namespace QuickDate.Activities.Tabbes
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class HomeActivity : AppCompatActivity, ServiceResultReceiver.IReceiver 
    {
        #region Variables Basic

        public CardMachFragment CardFragment;
        public TrendingFragment TrendingFragment;
        public NotificationsFragment NotificationsFragment;
        public ProfileFragment ProfileFragment;
        public NavigationTabBar NavigationTabBar;
        public FragmentBottomNavigationView FragmentBottomNavigator;
        
        private string  TypeAvatar = "";
        public static int CountNotificationsStatic, CountMessagesStatic;
        private static HomeActivity Instance;
        public TracksCounter TracksCounter;
        private PowerManager.WakeLock Wl;
        private ServiceResultReceiver Receiver;
        private readonly Handler ExitHandler = new Handler();
        private bool RecentlyBackPressed;

        public InitPayPalPayment InitPayPalPayment;
        public InitInAppBillingPayment BillingPayment; 

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                AddFlagsWakeLock();

                // Create your application here
                SetContentView(Resource.Layout.TabbedMainLayout);

                Instance = this;

                TracksCounter = new TracksCounter(this);

                CardFragment = new CardMachFragment();
                TrendingFragment = new TrendingFragment();
                NotificationsFragment = new NotificationsFragment();
                ProfileFragment = new ProfileFragment();

                if (AppSettings.ShowPaypal)
                    InitPayPalPayment = new InitPayPalPayment(this);

                if (AppSettings.ShowInAppBilling)
                    BillingPayment = new InitInAppBillingPayment(this);

                if (AppSettings.EnableAppFree)
                {
                    AppSettings.PremiumSystemEnabled = false;
                }
                 
                //Get Value
                SetupBottomNavigationView();
                 
                GetMyInfoData();
                 
                string type = Intent.GetStringExtra("TypeNotification") ?? "Don't have type";
                if (!string.IsNullOrEmpty(type) && type != "Don't have type")
                {
                    //var result = DbDatabase.Get_data_Login_Credentials();

                    var intent = new Intent(this, typeof(UserProfileActivity));

                    if (type == "got_new_match")
                    {
                        intent.PutExtra("EventPage", "HideButton");
                    }
                    else if (type == "like")
                    {
                        intent.PutExtra("EventPage", "likeAndClose");
                    }
                    else
                    {
                        intent.PutExtra("EventPage", "Close");
                    }

                    intent.PutExtra("DataType", "OneSignal");
                    intent.PutExtra("ItemUser", JsonConvert.SerializeObject(OneSignalNotification.UserData));
                    StartActivity(intent);
                }

                SetService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            {
                base.OnConfigurationChanged(newConfig);

                var currentNightMode = newConfig.UiMode & UiMode.NightMask;
                switch (currentNightMode)
                {
                    case UiMode.NightNo:
                        // Night mode is not active, we're using the light theme
                        AppSettings.SetTabDarkTheme = false;
                        break;
                    case UiMode.NightYes:
                        // Night mode is active, we're using dark theme
                        AppSettings.SetTabDarkTheme = true;
                        break;
                }

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                if (AppSettings.SetTabColoredTheme)
                {
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Cards)).Color = Color.ParseColor(AppSettings.TabColoredColor);

                    if (AppSettings.ShowTrending)
                        FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Trending)).Color = Color.ParseColor(AppSettings.TabColoredColor);

                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Notifications)).Color = Color.ParseColor(AppSettings.TabColoredColor);
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_messages)).Color = Color.ParseColor(AppSettings.TabColoredColor);
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Profile)).Color = Color.ParseColor(AppSettings.TabColoredColor);

                    NavigationTabBar.BgColor = Color.ParseColor(AppSettings.MainColor);
                    NavigationTabBar.ActiveColor = Color.White;
                    NavigationTabBar.InactiveColor = Color.White;
                }
                else if (AppSettings.SetTabDarkTheme)
                {
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Cards)).Color = Color.ParseColor(AppSettings.MainColor);

                    if (AppSettings.ShowTrending)
                        FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Trending)).Color = Color.ParseColor(AppSettings.MainColor);

                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Notifications)).Color = Color.ParseColor(AppSettings.MainColor);
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_messages)).Color = Color.ParseColor(AppSettings.MainColor);
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Profile)).Color = Color.ParseColor(AppSettings.MainColor);

                    NavigationTabBar.BgColor = Color.ParseColor("#000000");
                    NavigationTabBar.ActiveColor = Color.White;
                    NavigationTabBar.InactiveColor = Color.White;
                }
                else
                {
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Cards)).Color = Color.ParseColor("#ffffff");

                    if (AppSettings.ShowTrending)
                        FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Trending)).Color = Color.ParseColor("#ffffff");

                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Notifications)).Color = Color.ParseColor("#ffffff");
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_messages)).Color = Color.ParseColor("#ffffff");
                    FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Profile)).Color = Color.ParseColor("#ffffff");

                    NavigationTabBar.BgColor = Color.White;
                    NavigationTabBar.ActiveColor = Color.ParseColor(AppSettings.MainColor);
                    NavigationTabBar.InactiveColor = Color.ParseColor("#bfbfbf");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                TracksCounter?.RewardedVideoAd?.OnResume(this);
                SetWakeLock();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                TracksCounter?.RewardedVideoAd?.OnPause(this);
                OffWakeLock();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            { 
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static HomeActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                if (AppSettings.ShowPaypal)
                    InitPayPalPayment?.StopPayPalService();

                if (AppSettings.ShowInAppBilling)
                    BillingPayment?.DisconnectInAppBilling();

                ProfileFragment?.Time?.SetStopTimer();

                OffWakeLock();
                 
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region Functions

        public void SetToolBar(Android.Support.V7.Widget.Toolbar toolbar, string title, bool showIconBack = true)
        {
            try
            {
                if (toolbar != null)
                {
                    if (!string.IsNullOrEmpty(title))
                        toolbar.Title = title;

                    toolbar.SetTitleTextColor(AppSettings.SetTabDarkTheme ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(showIconBack);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                     
                    toolbar.SetBackgroundResource(AppSettings.SetTabDarkTheme ? Resource.Drawable.linear_gradient_drawable_Dark : Resource.Drawable.linear_gradient_drawable); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetupBottomNavigationView()
        {
            try
            {
                NavigationTabBar = FindViewById<NavigationTabBar>(Resource.Id.ntb_horizontal);
                FragmentBottomNavigator = new FragmentBottomNavigationView(this);

                CardFragment = new CardMachFragment();
                
                TrendingFragment = new TrendingFragment();
                NotificationsFragment = new NotificationsFragment();
                ProfileFragment = new ProfileFragment();

                FragmentBottomNavigator.FragmentListTab0.Add(CardFragment);

                if (AppSettings.ShowTrending)
                    FragmentBottomNavigator.FragmentListTab1.Add(TrendingFragment);

                FragmentBottomNavigator.FragmentListTab2.Add(NotificationsFragment);
                FragmentBottomNavigator.FragmentListTab4.Add(ProfileFragment);

                FragmentBottomNavigator.SetupNavigation(NavigationTabBar);
                NavigationTabBar.SetModelIndex(0, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        //Open Chat 
        public void ShowChat()
        {
            try
            {
                //Convert to fragment
                Intent intent = new Intent(this, typeof(LastChatActivity));
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ShowMessagesBox(UserInfoObject dataUser)
        {
            try
            {
                Intent intent = new Intent(this, typeof(MessagesBoxActivity));
                intent.PutExtra("UserId", dataUser.Id.ToString());
                intent.PutExtra("TypeChat", "LastChat");
                intent.PutExtra("UserItem", JsonConvert.SerializeObject(dataUser));

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartActivity(intent);
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                        StartActivity(intent);

                    }
                    else
                        new PermissionsController(this).RequestPermission(100);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        public void OpenDialogGallery(string typeAvatar = "")
        {
            try
            {
                TypeAvatar = typeAvatar;
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Methods.Path.Chack_MyFolder();

                    //Open Image 
                    var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        //request Code 108
                        new PermissionsController(this).RequestPermission(108);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenAddPhotoFragment()
        {
            try
            {
                var addPhoto = new AddPhotoBottomDialogFragment();
                addPhoto.Show(SupportFragmentManager, "addPhoto");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (AppSettings.ShowInAppBilling)
                    BillingPayment?.Handler?.HandleActivityResult(requestCode, resultCode, data);

                if (requestCode == 108 || requestCode == CropImage.CropImageActivityRequestCode)
                {
                    if (Methods.CheckConnectivity())
                    {
                        var result = CropImage.GetActivityResult(data);
                        if (result.IsSuccessful)
                        {
                            var resultPathImage = result.Uri.Path;
                            if (!string.IsNullOrEmpty(resultPathImage))
                            {
                                if (TypeAvatar == "Avatar")
                                {
                                    GlideImageLoader.LoadImage(this,resultPathImage, ProfileFragment?.ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.UpdateAvatarApi(this, resultPathImage) });
                                }
                                else
                                {
                                    //sent api  
                                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UploadMediaFileUserAsync(resultPathImage) });
                                }
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 1050) //1050
                {
                    //Get Location And Get Data Api
                    TrendingFragment?.CheckAndGetLocation();
                }
                else if (requestCode == InitPayPalPayment?.PayPalDataRequestCode)
                {
                    switch (resultCode)
                    {
                        case Result.Ok:
                            var confirmObj = data.GetParcelableExtra(PaymentActivity.ExtraResultConfirmation);
                            PaymentConfirmation configuration = Android.Runtime.Extensions.JavaCast<PaymentConfirmation>(confirmObj);
                            if (configuration != null)
                            {
                                //string createTime = configuration.ProofOfPayment.CreateTime;
                                //string intent = configuration.ProofOfPayment.Intent;
                                //string paymentId = configuration.ProofOfPayment.PaymentId;
                                //string state = configuration.ProofOfPayment.State;
                                //string transactionId = configuration.ProofOfPayment.TransactionId;

                                if (InitPayPalPayment?.PayType != "membership")
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        (int apiStatus, var respond) = await RequestsAsync.Auth.SetCreditAsync(InitPayPalPayment?.Credits, InitPayPalPayment?.Price, "PayPal").ConfigureAwait(false);
                                        if (apiStatus == 200)
                                        {
                                            if (respond is SetCreditObject result)
                                            {
                                                RunOnUiThread(() =>
                                                {
                                                    try
                                                    {
                                                        var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                                                        if (dataUser != null)
                                                        {
                                                            dataUser.Balance = result.Balance;

                                                            var sqlEntity = new SqLiteDatabase();
                                                            sqlEntity.InsertOrUpdate_DataMyInfo(dataUser);
                                                            sqlEntity.Dispose();
                                                        }

                                                        if (ProfileFragment.WalletNumber != null)
                                                            ProfileFragment.WalletNumber.Text = result.Balance;

                                                        Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine(e);
                                                    }
                                                });
                                            }
                                        }
                                        else Methods.DisplayReportResult(this, respond);

                                    }
                                    else
                                    {
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                    }
                                }
                                else
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        (int apiStatus, var respond) = await RequestsAsync.Auth.SetProAsync(InitPayPalPayment?.Id, InitPayPalPayment?.Price, "PayPal").ConfigureAwait(false);
                                        if (apiStatus == 200)
                                        {
                                            RunOnUiThread(() =>
                                            {
                                                try
                                                {
                                                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                                                    if (dataUser != null)
                                                    {
                                                        dataUser.VerifiedFinal = true;
                                                        dataUser.IsPro = "1";

                                                        var sqlEntity = new SqLiteDatabase();
                                                        sqlEntity.InsertOrUpdate_DataMyInfo(dataUser);
                                                        sqlEntity.Dispose();
                                                    }

                                                    Toast.MakeText(this, GetText(Resource.String.Lbl_Done),
                                                        ToastLength.Long).Show();
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            });
                                        }
                                        else Methods.DisplayReportResult(this, respond);
                                    }
                                    else
                                    {
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                    }
                                }
                            }

                            break;
                        case Result.Canceled:
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Canceled), ToastLength.Long).Show();
                            break;
                    } 
                }
                else if (requestCode == PaymentActivity.ResultExtrasInvalid)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Invalid), ToastLength.Long).Show();
                }
                else if (requestCode == 1001 && resultCode == Result.Ok && AppSettings.ShowInAppBilling)
                {
                    if (BillingPayment?.PayType != "membership")
                    {
                        if (Methods.CheckConnectivity())
                        {
                            (int apiStatus, var respond) = await RequestsAsync.Auth.SetCreditAsync(BillingPayment?.Credits, BillingPayment?.Price, "Google InApp").ConfigureAwait(false);
                            if (apiStatus == 200)
                            {
                                if (respond is SetCreditObject result)
                                {
                                    RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                                            if (dataUser != null)
                                            {
                                                dataUser.Balance = result.Balance;

                                                var sqlEntity = new SqLiteDatabase();
                                                sqlEntity.InsertOrUpdate_DataMyInfo(dataUser);
                                                sqlEntity.Dispose();
                                            }

                                            if (ProfileFragment.WalletNumber != null)
                                                ProfileFragment.WalletNumber.Text = result.Balance.Replace(".00", "");

                                            Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    });
                                }
                            }
                            else Methods.DisplayReportResult(this, respond);
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        if (Methods.CheckConnectivity())
                        {
                            (int apiStatus, var respond) = await RequestsAsync.Auth.SetProAsync(BillingPayment?.Id, BillingPayment?.Price, "Google InApp").ConfigureAwait(false);
                            if (apiStatus == 200)
                            {
                                RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                                        if (dataUser != null)
                                        {
                                            dataUser.VerifiedFinal = true;
                                            dataUser.IsPro = "1";

                                            var sqlEntity = new SqLiteDatabase();
                                            sqlEntity.InsertOrUpdate_DataMyInfo(dataUser);
                                            sqlEntity.Dispose();
                                        }

                                        Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                });
                            }
                            else Methods.DisplayReportResult(this, respond);
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 105)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Get Location And Get Data Api
                        TrendingFragment?.CheckAndGetLocation();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        OpenDialogGallery(TypeAvatar);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if(requestCode == 110)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if(requestCode == 100)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        ShowMessagesBox(DialogController.DataUser);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Back Pressed 

        public override void OnBackPressed()
        {
            try
            {
                if (FragmentBottomNavigator.GetCountFragment() > 0)
                {
                    FragmentBottomNavigator.BackStackClickFragment();
                }
                else
                {
                    if (RecentlyBackPressed)
                    {
                        ExitHandler.RemoveCallbacks(() => { RecentlyBackPressed = false; });
                        RecentlyBackPressed = false;
                        MoveTaskToBack(true);
                    }
                    else
                    {
                        RecentlyBackPressed = true;
                        Toast.MakeText(this, GetString(Resource.String.press_again_exit), ToastLength.Long).Show();
                        ExitHandler.PostDelayed(() => { RecentlyBackPressed = false; }, 2000L);
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Timer
         
        public async void GetNotifications()
        {
            try
            {
                if (FragmentBottomNavigator.Models != null)
                {
                    var (countNotifications, countMessages) = await ApiRequest.GetCountNotifications(this).ConfigureAwait(false);
                    NavigationTabBar.Model tabNotifications = FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Notifications));
                    NavigationTabBar.Model tabMessages = FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_messages));
                    if (tabNotifications != null && countNotifications != 0 && countNotifications != CountNotificationsStatic)
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                CountNotificationsStatic = countNotifications;
                                tabNotifications.BadgeTitle = countNotifications.ToString();
                                tabNotifications.UpdateBadgeTitle(countNotifications.ToString());
                                tabNotifications.ShowBadge();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                    }

                    if (tabMessages != null && countMessages != 0 && countMessages != CountMessagesStatic)
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                CountMessagesStatic = countMessages;
                                tabMessages.BadgeTitle = countMessages.ToString();
                                tabMessages.UpdateBadgeTitle(countMessages.ToString());
                                tabMessages.ShowBadge();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                    } 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }
         
        #endregion

        private void GetMyInfoData()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                 
                var settingsData = dbDatabase.GetSettings();
                if (settingsData == null) 
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetSettings_Api(this) });

                var myInfoData = dbDatabase.GetDataMyInfo();
                if (myInfoData == null)
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetInfoData(this, UserDetails.UserId.ToString()) });
                 
                var listFavorite = dbDatabase.GetDataFavorite(); 
                if (ListUtils.FavoriteUserList.Count == 0 && listFavorite?.Count > 0)
                    ListUtils.FavoriteUserList = listFavorite;
                  
                var listStickers = dbDatabase.GetStickersList();
                if (ListUtils.StickersList.Count == 0 && listStickers?.Count > 0)
                    ListUtils.StickersList = listStickers;
                else
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetStickers(this) });

                var listGifts = dbDatabase.GetGiftsList();
                if (ListUtils.GiftsList.Count == 0 && listGifts?.Count > 0)
                    ListUtils.GiftsList = listGifts;
                else
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetGifts(this) });

                dbDatabase.Dispose(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ShowOrHideBadgeView(int countMessages = 0, bool show = false)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        if (show)
                        {
                            if (NotificationsFragment?.IconFriend != null)
                            {
                                int gravity = (int)(GravityFlags.End | GravityFlags.Bottom);
                                QBadgeView badge = new QBadgeView(this);
                                badge.BindTarget(NotificationsFragment.IconFriend);
                                badge.SetBadgeNumber(countMessages);
                                badge.SetBadgeGravity(gravity);
                                badge.SetBadgeBackgroundColor(Color.ParseColor(AppSettings.MainColor));
                                badge.SetGravityOffset(0, true);

                                NotificationsFragment.IconFriend.Visibility = ViewStates.Visible;
                            }
                        }
                        else
                        {
                            if (NotificationsFragment?.IconFriend != null)
                            {
                                new QBadgeView(this).BindTarget(NotificationsFragment.IconFriend).Hide(true);
                                NotificationsFragment.IconFriend.Visibility = ViewStates.Gone;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #region WakeLock System

        public void AddFlagsWakeLock()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.WakeLock) == Permission.Granted)
                    {
                        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        //request Code 110
                        new PermissionsController(this).RequestPermission(110);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetWakeLock()
        {
            try
            {
                if (Wl == null)
                {
                    PowerManager pm = (PowerManager)GetSystemService(PowerService);
                    Wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                    Wl.Acquire();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetOnWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)GetSystemService(PowerService);
                Wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                Wl.Acquire();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetOffWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)GetSystemService(PowerService);
                Wl = pm.NewWakeLock(WakeLockFlags.ProximityScreenOff, "My Tag");
                Wl.Acquire();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OffWakeLock()
        {
            try
            {
                // ..screen will stay on during this section..
                Wl?.Release();
                Wl = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Service Chat

        public void SetService(bool run = true)
        {
            try
            {
                if (run)
                {
                    try
                    {
                        Receiver = new ServiceResultReceiver(new Handler());
                        Receiver.SetReceiver(this);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    var intent = new Intent(this, typeof(ScheduledApiService));
                    intent.PutExtra("receiverTag", Receiver);
                    StartService(intent);
                }
                else
                {
                    var intentService = new Intent(this, typeof(ScheduledApiService));
                    StopService(intentService);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnReceiveResult(int resultCode, Bundle resultData)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<GetConversationListObject>(resultData.GetString("Json"));
                if (result != null)
                {
                    LastChatActivity.GetInstance()?.LoadDataJsonLastChat(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

    }
}