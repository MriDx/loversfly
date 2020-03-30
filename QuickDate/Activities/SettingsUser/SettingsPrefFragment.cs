using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Activities.SettingsUser.General;
using QuickDate.Activities.SettingsUser.Support;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Requests;
using Exception = System.Exception;

namespace QuickDate.Activities.SettingsUser
{
    public class SettingsPrefFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private Preference MyAccountPref, PasswordPref, SocialLinksPref, BlockedUsersPref, StoragePref, HelpPref, AboutPref, DeleteAccountPref, LogoutPref, TwoFactorPref, ManageSessionsPref, WithdrawalsPref, MyAffiliatesPref, NightMode;
        private SwitchPreferenceCompat ChatOnlinePref, PSearchEnginesPref, PRandomUsersPref, PFindMatchPagePref; 
        private readonly Activity ActivityContext;
        private string ChatOnline, PSearchEngines, PRandomUsers, PFindMatchPage;
        private string TypeDialog;

        #endregion

        #region General

        public SettingsPrefFragment(Activity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // create ContextThemeWrapper from the original Activity Context with the custom theme
                Context contextThemeWrapper = AppSettings.SetTabDarkTheme ? new ContextThemeWrapper(ActivityContext, Resource.Style.SettingsThemeDark) : new ContextThemeWrapper(ActivityContext, Resource.Style.SettingsTheme);

                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = base.OnCreateView(localInflater, container, savedInstanceState);

                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            try
            {
                AddPreferencesFromResource(Resource.Xml.SettingsPrefs);

                InitComponent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override PreferenceScreen PreferenceScreen
        {
            get => base.PreferenceScreen;
            set
            {
                base.PreferenceScreen = value;
                if (PreferenceScreen != null)
                {
                    var count = PreferenceScreen.PreferenceCount;
                    for (var i = 0; i < count; i++)
                    {
                        PreferenceScreen.GetPreference(i).IconSpaceReserved = false;
                    }
                }
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                //Add OnChange event to Preferences
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();
                PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);

                //Close OnChange event to Preferences
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                MainSettings.SharedData = PreferenceManager.SharedPreferences;
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                MyAccountPref = FindPreference("myAccount_key");
                PasswordPref = FindPreference("editPassword_key");
                SocialLinksPref = FindPreference("socialLinks_key");
                BlockedUsersPref = FindPreference("blocked_key");
                StoragePref = FindPreference("Storage_key");
                HelpPref = FindPreference("help_key");
                AboutPref = FindPreference("about_key");
                DeleteAccountPref = FindPreference("deleteAccount_key");
                LogoutPref = FindPreference("logout_key");
                TwoFactorPref = FindPreference("Twofactor_key");
                ManageSessionsPref = FindPreference("ManageSessions_key");
                WithdrawalsPref = FindPreference("Withdrawals_key");
                MyAffiliatesPref = FindPreference("MyAffiliates_key");
                NightMode = FindPreference("Night_Mode_key");

                ChatOnlinePref = (SwitchPreferenceCompat)FindPreference("chatOnline_key");
                PSearchEnginesPref = (SwitchPreferenceCompat)FindPreference("searchEngines_key");
                PRandomUsersPref = (SwitchPreferenceCompat)FindPreference("randomUsers_key");
                PFindMatchPagePref = (SwitchPreferenceCompat)FindPreference("findMatchPage_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "Night_Mode_key");

                //Delete Preference
                //============== SecurityAccount_key ===================
                var mCategoryAccount = (PreferenceCategory)FindPreference("CategoryGeneral_key");
                if (!AppSettings.ShowSettingsAccount)
                    mCategoryAccount.RemovePreference(MyAccountPref);

                if (!AppSettings.ShowSettingsSocialLinks)
                    mCategoryAccount.RemovePreference(SocialLinksPref);

                if (!AppSettings.ShowSettingsBlockedUsers)
                    mCategoryAccount.RemovePreference(BlockedUsersPref);

                if (!AppSettings.ShowSettingsWithdrawals)
                    mCategoryAccount.RemovePreference(WithdrawalsPref);

                if (!AppSettings.ShowSettingsMyAffiliates)
                    mCategoryAccount.RemovePreference(MyAffiliatesPref);

                //============== SecurityAccount_key ===================
                var mCategorySecurity = (PreferenceCategory)FindPreference("SecurityAccount_key");
                if (!AppSettings.ShowSettingsPassword)
                    mCategorySecurity.RemovePreference(PasswordPref);
                 
                if (!AppSettings.ShowSettingsTwoFactor)
                    mCategorySecurity.RemovePreference(TwoFactorPref);

                if (!AppSettings.ShowSettingsManageSessions)
                    mCategorySecurity.RemovePreference(ManageSessionsPref);

                //============== CategorySupport_key ===================
                var mCategorySupport = (PreferenceCategory)FindPreference("CategorySupport_key");
              
                if (!AppSettings.ShowSettingsDeleteAccount)
                    mCategorySupport.RemovePreference(DeleteAccountPref);


                //Update Preferences data on Load
                OnSharedPreferenceChanged(MainSettings.SharedData, "chatOnline_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "searchEngines_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "randomUsers_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "findMatchPage_key");
                 
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                if (dataUser != null)
                {
                    ChatOnlinePref.Checked = dataUser.Online == "1"; 
                    PFindMatchPagePref.Checked = dataUser.PrivacyShowProfileMatchProfiles == "1"; 
                    PRandomUsersPref.Checked = dataUser.PrivacyShowProfileRandomUsers == "1";
                    PSearchEnginesPref.Checked = dataUser.PrivacyShowProfileOnGoogle == "1";
                }

                ChatOnlinePref.IconSpaceReserved = false;
                PSearchEnginesPref.IconSpaceReserved = false;
                PRandomUsersPref.IconSpaceReserved = false;
                PFindMatchPagePref.IconSpaceReserved = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    MyAccountPref.PreferenceClick += MyAccountPrefOnPreferenceClick;
                    PasswordPref.PreferenceClick += PasswordPrefOnPreferenceClick;
                    SocialLinksPref.PreferenceClick += SocialLinksPrefOnPreferenceClick;
                    BlockedUsersPref.PreferenceClick += BlockedUsersPrefOnPreferenceClick;
                    StoragePref.PreferenceClick += StoragePrefOnPreferenceClick;
                    HelpPref.PreferenceClick += HelpPrefOnPreferenceClick;
                    AboutPref.PreferenceClick += AboutPrefOnPreferenceClick;
                    DeleteAccountPref.PreferenceClick += DeleteAccountPrefOnPreferenceClick;
                    LogoutPref.PreferenceClick += LogoutPrefOnPreferenceClick;
                    ChatOnlinePref.PreferenceChange += ChatOnlinePrefOnPreferenceChange;
                    PSearchEnginesPref.PreferenceChange += PSearchEnginesPrefOnPreferenceChange;
                    PRandomUsersPref.PreferenceChange += PRandomUsersPrefOnPreferenceChange;
                    PFindMatchPagePref.PreferenceChange += PFindMatchPagePrefOnPreferenceChange;
                    ManageSessionsPref.PreferenceClick += ManageSessionsPrefOnPreferenceClick;
                    TwoFactorPref.PreferenceClick += TwoFactorPrefOnPreferenceClick;
                    WithdrawalsPref.PreferenceClick += WithdrawalsPrefOnPreferenceClick;
                    MyAffiliatesPref.PreferenceClick += MyAffiliatesPrefOnPreferenceClick;
                }
                else
                {
                    MyAccountPref.PreferenceClick -= MyAccountPrefOnPreferenceClick;
                    PasswordPref.PreferenceClick -= PasswordPrefOnPreferenceClick;
                    SocialLinksPref.PreferenceClick -= SocialLinksPrefOnPreferenceClick;
                    BlockedUsersPref.PreferenceClick -= BlockedUsersPrefOnPreferenceClick;
                    StoragePref.PreferenceClick -= StoragePrefOnPreferenceClick;
                    HelpPref.PreferenceClick -= HelpPrefOnPreferenceClick;
                    AboutPref.PreferenceClick -= AboutPrefOnPreferenceClick;
                    DeleteAccountPref.PreferenceClick -= DeleteAccountPrefOnPreferenceClick;
                    LogoutPref.PreferenceClick -= LogoutPrefOnPreferenceClick;
                    ChatOnlinePref.PreferenceChange -= ChatOnlinePrefOnPreferenceChange;
                    PSearchEnginesPref.PreferenceChange -= PSearchEnginesPrefOnPreferenceChange;
                    PRandomUsersPref.PreferenceChange -= PRandomUsersPrefOnPreferenceChange;
                    PFindMatchPagePref.PreferenceChange -= PFindMatchPagePrefOnPreferenceChange;
                    ManageSessionsPref.PreferenceClick -= ManageSessionsPrefOnPreferenceClick;
                    TwoFactorPref.PreferenceClick -= TwoFactorPrefOnPreferenceClick;
                    WithdrawalsPref.PreferenceClick -= WithdrawalsPrefOnPreferenceClick;
                    MyAffiliatesPref.PreferenceClick -= MyAffiliatesPrefOnPreferenceClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Event Privacy

        //Privacy >> Show my profile in find match page
        private void PFindMatchPagePrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    var etp = (SwitchPreferenceCompat)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
                    {
                        dataUser.PrivacyShowProfileMatchProfiles = "1";
                        PFindMatchPage = "1";
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileMatchProfiles = "0";
                        PFindMatchPage = "0";
                    }

                    dataUser.PrivacyShowProfileMatchProfiles = PFindMatchPage;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);
                    database.Dispose();

                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_match_profiles", PFindMatchPage}
                    };
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        //Privacy >> Show my profile in random users
        private void PRandomUsersPrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    var etp = (SwitchPreferenceCompat)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
                    {
                        dataUser.PrivacyShowProfileRandomUsers = "1";
                        PRandomUsers = "1";
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileRandomUsers = "0";
                        PRandomUsers = "0";
                    }

                    dataUser.PrivacyShowProfileRandomUsers = PRandomUsers;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);
                    database.Dispose();

                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_random_users", PRandomUsers}
                    };
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Privacy >> Show my profile on search engines (google)
        private void PSearchEnginesPrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    var etp = (SwitchPreferenceCompat)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
                    {
                        dataUser.PrivacyShowProfileOnGoogle = "1";
                        PSearchEngines = "1";
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileOnGoogle = "0";
                        PSearchEngines = "0";
                    }

                    dataUser.PrivacyShowProfileOnGoogle = PSearchEngines;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);
                    database.Dispose();

                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_on_google", PSearchEngines}
                    };
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UpdateProfileAsync(dataPrivacy) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Event Support

        //Logout
        private void LogoutPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                TypeDialog = "logout";
                
                var dialog = new MaterialDialog.Builder(Activity).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(GetText(Resource.String.Lbl_Are_you_logout));
                dialog.PositiveText(GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //DeleteAccount
        private void DeleteAccountPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(DeleteAccountActivity))); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //About
        private void AboutPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(AboutAppActivity));
                ActivityContext.StartActivity(intent); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Help
        private void HelpPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/contact");
                intent.PutExtra("Type", GetText(Resource.String.Lbl_Help));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Event General

        //MyAffiliates
        private void MyAffiliatesPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(MyAffiliatesActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Withdrawals
        private void WithdrawalsPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(WithdrawalsActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Manage Sessions
        private void ManageSessionsPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(ManageSessionsActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Two-Factor Authentication
        private void TwoFactorPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(TwoFactorAuthActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //BlockedUsers
        private void BlockedUsersPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(BlockedUsersActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //SocialLinks
        private void SocialLinksPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(SocialLinksActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Change Password
        private void PasswordPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(PasswordActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //MyAccount
        private void MyAccountPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(MyAccountActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        //Clear Cache >> Media
        private void StoragePrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var dialog = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light); 
                dialog.Title(GetText(Resource.String.Lbl_Warning));
                dialog.Content(GetText(Resource.String.Lbl_TheFilesWillBeDeleted));
                dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_FilesAreNowDeleted), ToastLength.Long).Show();

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var dirPath = ActivityContext.CacheDir;
                            dirPath.Delete();

                            Methods.Path.DeleteAll_MyFolderDisk();
                            Methods.Path.Chack_MyFolder();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    });
                });
                dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }
         
        //ChatOnline
        private void ChatOnlinePrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    var etp = (SwitchPreferenceCompat)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
                    {
                        dataUser.Online = "1";
                        ChatOnline = "1";
                    }
                    else
                    {
                        dataUser.Online = "0";
                        ChatOnline = "0";
                    }

                    dataUser.Online = ChatOnline;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);
                    database.Dispose();

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Chat.SwitchOnlineAsync(ChatOnline) });
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }

             
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
                if (key.Equals("chatOnline_key"))
                {
                    var getValue = MainSettings.SharedData.GetBoolean("chatOnline_key", true);
                    ChatOnlinePref.Checked = getValue;
                }
                else if (key.Equals("searchEngines_key"))
                {
                    var getValue = MainSettings.SharedData.GetBoolean("searchEngines_key", true);
                    PSearchEnginesPref.Checked = getValue;
                }
                else if (key.Equals("randomUsers_key"))
                {
                    var getValue = MainSettings.SharedData.GetBoolean("randomUsers_key", true);
                    PRandomUsersPref.Checked = getValue;
                }
                else if (key.Equals("findMatchPage_key"))
                {
                    var getValue = MainSettings.SharedData.GetBoolean("findMatchPage_key", true);
                    PFindMatchPagePref.Checked = getValue;
                }
                else if (key.Equals("Night_Mode_key"))
                {
                    // Set summary to be the user-description for the selected value
                    Preference etp = FindPreference("Night_Mode_key");
                      
                    string getValue = MainSettings.SharedData.GetString("Night_Mode_key", string.Empty);
                    etp.Summary = getValue; 
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #region MaterialDialog

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (TypeDialog == "logout")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_You_will_be_logged), ToastLength.Long).Show();
                        ApiRequest.RunLogout = false;
                        ApiRequest.Logout(ActivityContext);
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else
                {
                    if (p1 == DialogAction.Positive)
                    {
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
 
        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();

                string getValue = MainSettings.SharedData.GetString("Night_Mode_key", string.Empty);

                if (text == GetString(Resource.String.Lbl_Light) && getValue != MainSettings.LightMode)
                {
                    //Set Light Mode   
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Light);

                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                    AppSettings.SetTabDarkTheme = false;
                    MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.LightMode).Commit();

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                        ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    }

                    Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    ActivityContext.StartActivity(intent);
                    ActivityContext.FinishAffinity(); 
                }
                else if (text == GetString(Resource.String.Lbl_Dark) && getValue != MainSettings.DarkMode)
                {
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Dark);

                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                    AppSettings.SetTabDarkTheme = true;
                    MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.DarkMode).Commit();

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                        ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    }

                    Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    ActivityContext.StartActivity(intent);
                    ActivityContext.FinishAffinity();
                }
                else if (text == GetString(Resource.String.Lbl_SetByBattery) && getValue != MainSettings.DefaultMode)
                { 
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_SetByBattery); 
                    MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.DefaultMode).Commit();

                    if ((int)Build.VERSION.SdkInt >= 29)
                    {
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;

                        var currentNightMode = Resources.Configuration.UiMode & UiMode.NightMask;
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
                    }
                    else
                    {
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightAuto;

                        var currentNightMode = Resources.Configuration.UiMode & UiMode.NightMask;
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

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                            ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        }

                        Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        ActivityContext.StartActivity(intent);
                        ActivityContext.FinishAffinity();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            try
            {
                if (preference.Key == "Night_Mode_key")
                {
                    TypeDialog = "NightMode";

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                    dialogList.Title(Resource.String.Lbl_Night_Mode);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_Light));
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Dark));

                    if ((int)Build.VERSION.SdkInt >= 29)
                        arrayAdapter.Add(GetText(Resource.String.Lbl_SetByBattery));

                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }

                return base.OnPreferenceTreeClick(preference);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return base.OnPreferenceTreeClick(preference);
            } 
        }
    }
}