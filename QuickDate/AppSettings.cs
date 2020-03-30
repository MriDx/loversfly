//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PixelPhoto 15/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================
//For the accuracy of the icon and logo, please use this website " http://nsimage.brosteins.com " and add images according to size in folders " mipmap " 

using Android.Graphics;

namespace QuickDate
{
    public static class AppSettings
    {
        //Main Settings >>>>>
        //*********************************************************
        public static string TripleDesAppServiceProvider = "IbxeixrTQTIbjj2d58d1xYtxtePYeG20TiE0gbSZMHTD0Y5OUimTBb1v262TFUFOgzaWRmZ9Fcf+Y1AoLopa9l5ww8L4aQZFVZBqzIV/zu0n0FdZdSKIYtW+50SO8Dyp1eq/jL9DHxbgKFNGxoppkVOXSsewgnT0p2k8v1kz1CcN21w9D/r2c00axmdWnCEqKdpZBM3swjTr43t8FXeLN+i1+RyA/niFI5fwTltID7VEzfil/npnNTRnOf4dcl5iNBr7k2cBklCm7ArxCdsWxkovbt+jBaqRtNWBMd7CbJg=";
        
        public static string Version = "1.5";
        public static string ApplicationName = "LoversFly";

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#a33596";
        public static string StartColor = MainColor;
        public static string EndColor = "#63245c";
        public static Color TitleTextColor = Color.Black;
        public static Color TitleTextColorDark = Color.White;

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language ar_AE

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "0eeb44be-0ee2-422c-99b7-d338c59c5906"; 

        //********************************************************* 

        //Add Animation Image User
        //*********************************************************
        public static bool EnableAddAnimationImageUser = false;
         
        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file or AndroidManifest.xml
        //Facebook >> ../values/analytic.xml  
        //Google >> ../Properties/AndroidManifest.xml .. line 42
        //*********************************************************
        public static bool ShowFacebookLogin = false;
        public static bool ShowGoogleLogin = false; //#New
        public static bool ShowWoWonderLogin = false; //#New
         
        public static string ClientId = "716215768781-1riglii0rihhc9gmp53qad69tt8o2e03.apps.googleusercontent.com";

        public static string AppNameWoWonder = "WoWonder";//#New

        //AdMob >> Please add the code ads in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdMobBanner = true;
        public static bool ShowAdMobInterstitial = false;
        public static bool ShowAdMobRewardVideo = false;
        public static bool ShowAdMobNative = true;

        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/6657648824";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/7559666953";
        public static string AdAdMobNativeKey = "ca-app-pub-5135691635931982/2342769069";
       
        //Three times after entering the ad is displayed
        public static int ShowAdMobInterstitialCount = 3;
        public static int ShowAdMobRewardedVideoCount = 3;

        //########################### 

        //Last_Messages Page >>
        ///********************************************************* 
        public static bool RunSoundControl = true;
        public static int RefreshChatActivitiesSeconds = 6000; // 6 Seconds
        public static int MessageRequestSpeed = 3000; // 3 Seconds
                  
        //Set Theme Tab
        //*********************************************************
        public static bool SetTabColoredTheme = false;
        public static bool SetTabDarkTheme = false;

        public static string TabColoredColor = MainColor;
        public static bool SetTabIsTitledWithText = false;

        //Bypass Web Errors  
        //*********************************************************
        public static bool TurnTrustFailureOnWebException = true;
        public static bool TurnSecurityProtocolType3072On = true;

        //Show custom error reporting page
        public static bool RenderPriorityFastPostLoad = true;

        //New Version 
        //*********************************************************

        //Trending 
        //*********************************************************
        public static bool ShowTrending = true; 
         
        public static bool ShowFilterBasic = true;
        public static bool ShowFilterLooks = true;
        public static bool ShowFilterBackground = true;
        public static bool ShowFilterLifestyle = true;
        public static bool ShowFilterMore = true;
         
        //true = Show Users 2 Column and use CardView
        //false = Show Users 3 Column  and image Circle 
        public static bool ShowUsersAsCards = true;
         
        //*********************************************************

        //Premium system
        public static bool PremiumSystemEnabled = true;

        //Phone Validation system
        public static bool ValidationEnabled = true;
        public static bool CompressImage = false;
        public static int AvatarSize = 60;  
        public static int ImageSize = 200;    

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = false; 
         
        public static bool ShowWalkTroutPage = true;

        public static bool EnableAppFree = false;

        //Payment System (ShowPaymentCardPage >> Paypal & Stripe ) (ShowLocalBankPage >> Local Bank ) 
        //*********************************************************
        public static bool ShowPaypal = false;
        public static bool ShowCreditCard = false;
        public static bool ShowBankTransfer = true;

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="com.android.vending.BILLING" />
        /// </summary>
        public static bool ShowInAppBilling = true; //#New 
        //*********************************************************

        //Settings Page >>  
        //********************************************************* 
        public static bool ShowSettingsAccount = true; //#New
        public static bool ShowSettingsSocialLinks = true;//#New
        public static bool ShowSettingsPassword = true;//#New
        public static bool ShowSettingsBlockedUsers = true;//#New
        public static bool ShowSettingsDeleteAccount = true;//#New
        public static bool ShowSettingsTwoFactor = false; //#New >> Next Version 
        public static bool ShowSettingsManageSessions = true; //#New
        public static bool ShowSettingsWithdrawals = true; //#New
        public static bool ShowSettingsMyAffiliates = true;//#New 
         
        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="android.permission.READ_CONTACTS" />
        /// <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" />
        /// <uses-permission android:name="android.permission.SEND_SMS" />
        /// </summary>
        public static bool InvitationSystem = false;
         

    }
} 