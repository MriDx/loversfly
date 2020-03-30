using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Java.Lang;
using QuickDate.Activities.Default;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.SQLite;
using QuickDateClient;
using Exception = System.Exception;

namespace QuickDate.Activities
{
    [Activity(MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/SplashScreenTheme", NoHistory = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreenActivity : AppCompatActivity
    {
        #region Variables Basic

        private SqLiteDatabase DbDatabase;

        #endregion
         
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                DbDatabase = new SqLiteDatabase();
                DbDatabase.CheckTablesStatus();

                new Handler(Looper.MainLooper).Post(new Runnable(FirstRunExcite)); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void FirstRunExcite()
        {
            try
            {
                DbDatabase = new SqLiteDatabase();
                DbDatabase.CheckTablesStatus();

                if (!string.IsNullOrEmpty(AppSettings.Lang))
                {
                    LangController.SetApplicationLang(this, AppSettings.Lang);
                }
                else
                {
                    UserDetails.LangName = Resources.Configuration.Locale.Language.ToLower();
                    LangController.SetApplicationLang(this, UserDetails.LangName);
                }

                DbDatabase.GetSettings();
                 
                var result = DbDatabase.Get_data_Login_Credentials();
                if (result != null)
                {
                    Current.AccessToken = result.AccessToken; 
                    switch (result.Status)
                    {
                        case "Active":
                        case "Pending":
                            StartActivity(new Intent(this, typeof(HomeActivity)));
                            break;
                        default:
                            StartActivity(new Intent(this, typeof(FirstActivity)));
                            break;
                    }
                }
                else
                {
                    StartActivity(new Intent(this, typeof(FirstActivity)));
                }

                DbDatabase.Dispose();

                if (AppSettings.ShowAdMobBanner || AppSettings.ShowAdMobInterstitial || AppSettings.ShowAdMobRewardVideo || AppSettings.ShowAdMobNative)
                    MobileAds.Initialize(this, GetString(Resource.String.admob_app_id));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}