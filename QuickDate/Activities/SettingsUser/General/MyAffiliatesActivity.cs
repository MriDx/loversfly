using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.Share;
using Plugin.Share.Abstractions;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MyAffiliatesActivity : AppCompatActivity 
    {
        #region Variables Basic

        private ImageView ImageUser;
        private TextView TxtLink , TxtMyAffiliates;
        private Button BtnShare; 

        #endregion
         
        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
               
                Methods.App.FullScreenApp(this);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                // Create your application here
                SetContentView(Resource.Layout.MyAffiliatesLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                AdsGoogle.Ad_AdMobNative(this);
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
                AddOrRemoveEvent(true);
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
                AddOrRemoveEvent(false);
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

        #endregion
         
        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                ImageUser = FindViewById<ImageView>(Resource.Id.ImageUser);
                TxtLink = FindViewById<TextView>(Resource.Id.linkText);
                TxtMyAffiliates = FindViewById<TextView>(Resource.Id.myAffiliatesText);
                BtnShare = FindViewById<Button>(Resource.Id.cont);
                 
                TxtLink.Text = Client.WebsiteUrl + "register?ref=" + UserDetails.Username;

                var option = ListUtils.SettingsSiteList?.FirstOrDefault();
                if (option != null)
                {
                    if (!string.IsNullOrEmpty(option.AmountPercentRef) && option.AmountPercentRef != "0")
                    {
                        TxtMyAffiliates.Text = GetString(Resource.String.Lbl_EarnUpTo) + "%" + option.AmountPercentRef + " " + GetString(Resource.String.Lbl_forEachUserYourReferToUs) + " !";
                    }
                    else if (!string.IsNullOrEmpty(option.AmountRef) && option.AmountRef != "0")  
                    { 
                        TxtMyAffiliates.Text = GetString(Resource.String.Lbl_EarnUpTo) + " $" + option.AmountRef + " " + GetString(Resource.String.Lbl_forEachUserYourReferToUs) + " !";
                    }
                    else
                    {
                        TxtMyAffiliates.Text = GetString(Resource.String.Lbl_EarnUpTo) + " $" + " " + GetString(Resource.String.Lbl_forEachUserYourReferToUs) + " !";
                    }
                }
                else
                {
                    TxtMyAffiliates.Text = GetString(Resource.String.Lbl_EarnUpTo) + " $" + " " + GetString(Resource.String.Lbl_forEachUserYourReferToUs) + " !";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetText(Resource.String.Lbl_MyAffiliates);
                    toolbar.SetTitleTextColor(AppSettings.SetTabDarkTheme ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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
         
        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    BtnShare.Click += BtnShareOnClick;
                    TxtLink.LongClick += TxtLinkOnLongClick;
                }
                else
                {
                    BtnShare.Click -= BtnShareOnClick;
                    TxtLink.LongClick -= TxtLinkOnLongClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Copy link
        private void TxtLinkOnLongClick(object sender, View.LongClickEventArgs e)
        {
            try
            {
                Methods.CopyToClipboard(this, TxtLink.Text);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Share
        private async void BtnShareOnClick(object sender, EventArgs e)
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = UserDetails.Username,
                    Text = "",
                    Url = TxtLink.Text
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion 
         
    }
}