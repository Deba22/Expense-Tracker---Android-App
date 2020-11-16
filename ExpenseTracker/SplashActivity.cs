using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using MyMomsCollection.Helpers;
using Xamarin.Essentials;

namespace ExpenseTracker
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        FirebaseAuth firebaseAuth;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.activity_Splash);
                firebaseAuth = AppDataHelper.GetFirebaseAuth();

            }
            catch (Exception ex)
            {
            }
        }
        // Launches the startup task
        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                Task startupWork = new Task(() => { SimulateStartup(); });
                startupWork.Start();
            }
            catch (Exception ex)
            {

            }

        }
        async void SimulateStartup()
        {
            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
            await Task.Delay(500); // Simulate a bit of startup work.
            Log.Debug(TAG, "Startup work is finished - starting MainActivity.");
            //  StartActivity(new Intent(Application.Context, typeof(MainActivity)));

            if (VersionTracking.IsFirstLaunchEver && firebaseAuth.CurrentUser == null)
            {
                StartActivity(new Intent(Application.Context, typeof(SliderIntroActivity)));
            }
            else
            {
                if (firebaseAuth.CurrentUser != null)
                {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.PutExtra("CurrentUserUid", firebaseAuth.CurrentUser.Uid.ToString());
                    intent.PutExtra("CurrentUserDisplayName", firebaseAuth.CurrentUser.DisplayName.ToString());
                    //intent.PutExtra("CurrentUserEmail", firebaseAuth.CurrentUser.Email.ToString());
                    string highresphoto = AppDataHelper.GetHighResPhoto(firebaseAuth.CurrentUser.Providers, firebaseAuth.CurrentUser.PhotoUrl.ToString());
                    intent.PutExtra("CurrentUserPhoto", highresphoto);
                    this.StartActivity(intent);

                }
                else
                {
                    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                }
            }
               
               
        }
    }
}
