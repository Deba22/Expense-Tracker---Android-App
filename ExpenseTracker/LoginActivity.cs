using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Util;
using MyMomsCollection.Helpers;
using Plugin.Connectivity;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

namespace ExpenseTracker
{
  
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : AppCompatActivity, IOnSuccessListener,IOnFailureListener,IFacebookCallback
    {
        
        Button googlebutton ,facebookbutton;
        LoginButton fb_dummybuttton;
        GoogleSignInOptions gso;
        GoogleApiClient googleApiClient;
        FirebaseAuth firebaseAuth;
        ICallbackManager callbackManager;
        bool usingFirebase;
        ISharedPreferences prefs;
       
        ProgressBar progressBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_login);
         
            this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            this.Window.SetStatusBarColor(Color.ParseColor("#204060"));
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            googlebutton = FindViewById<Button>(Resource.Id.googlebutton);
            googlebutton.Click += Googlebutton_Click;
            facebookbutton = FindViewById<Button>(Resource.Id.facebookbutton);
            facebookbutton.Click += Facebookbutton_Click;
            fb_dummybuttton = FindViewById<LoginButton>(Resource.Id.fb_dummybuttton);
            fb_dummybuttton.SetReadPermissions(new List<string> { "public_profile", "email" });
            callbackManager = CallbackManagerFactory.Create();
            fb_dummybuttton.RegisterCallback(callbackManager, this);


            gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn).RequestIdToken("787402240794-prndui4ntngl1je2lglu46lev1esogsi.apps.googleusercontent.com")
                .RequestEmail().Build();
            googleApiClient = new GoogleApiClient.Builder(this).AddApi(Auth.GOOGLE_SIGN_IN_API, gso).Build();
            googleApiClient.Connect();
            firebaseAuth = AppDataHelper.GetFirebaseAuth();
             prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            AppDataHelper.editor = prefs.Edit();
        }

        private void Googlebutton_Click(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
            if (firebaseAuth.CurrentUser == null)
            {


                AppDataHelper.editor.PutString("WhichButtonClick", "google");
                AppDataHelper.editor.Apply();

                var intent = Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);
                googleApiClient.ClearDefaultAccountAndReconnect();
                StartActivityForResult(intent, 1);
            }
        }

        private void Facebookbutton_Click(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
            fb_dummybuttton.PerformClick();

            AppDataHelper.editor.PutString("WhichButtonClick", "facebook");
            AppDataHelper.editor.Apply();
        }

       

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            
          
            
            base.OnActivityResult(requestCode, resultCode, data);
            if(requestCode==1)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if(result.IsSuccess)
                {
                    progressBar.Visibility = ViewStates.Visible;
                    GoogleSignInAccount account = result.SignInAccount;
                    LoginWithFirebase(account);
                }
            }
            else
            {
                progressBar.Visibility = ViewStates.Visible;
                callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
            }
        }

        private void LoginWithFirebase(GoogleSignInAccount account)
        {
            usingFirebase = true;
            var credentials = GoogleAuthProvider.GetCredential(account.IdToken, null);
            firebaseAuth.SignInWithCredential(credentials).AddOnSuccessListener(this).AddOnFailureListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
          

            if (!usingFirebase)
            {
                usingFirebase = true;
                LoginResult loginResult = result as LoginResult;
                var credentials = FacebookAuthProvider.GetCredential(loginResult.AccessToken.Token);
                firebaseAuth.SignInWithCredential(credentials).AddOnSuccessListener(this).AddOnFailureListener(this);
            }
            else
            {
                usingFirebase = false;
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("CurrentUserUid", firebaseAuth.CurrentUser.Uid.ToString());
                intent.PutExtra("CurrentUserDisplayName", firebaseAuth.CurrentUser.DisplayName.ToString());
                //intent.PutExtra("CurrentUserEmail", firebaseAuth.CurrentUser.Email.ToString());
                string highresphoto= AppDataHelper.GetHighResPhoto(firebaseAuth.CurrentUser.Providers, firebaseAuth.CurrentUser.PhotoUrl.ToString());
                intent.PutExtra("CurrentUserPhoto", highresphoto);
              
                this.StartActivity(intent);
            }
          
            progressBar.Visibility = ViewStates.Invisible;
        }

       

        public void OnFailure(Java.Lang.Exception e)
        {
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, e.Message, ToastLength.Long).Show();
        }
        
        public override void OnBackPressed()
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            // AlertDialog alert = dialog.Create();
            alert.SetTitle("Exit");
            alert.SetMessage("Are you sure you wan't to exit the app?");
            // alert.SetIcon(Resource.Drawable.alert);
            alert.SetPositiveButton("YES", (c, ev) =>
            {
                this.FinishAffinity();

            });
            alert.SetNegativeButton("CANCEL", (c, ev) => { });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        public void OnCancel()
        {
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, "Facebook login cancelled", ToastLength.Long).Show();
        }

        public void OnError(FacebookException error)
        {
         
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, error.Message, ToastLength.Long).Show();
        }
    }
}