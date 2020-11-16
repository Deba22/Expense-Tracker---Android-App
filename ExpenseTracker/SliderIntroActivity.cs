using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExpenseTracker.Helper;
using Firebase.Auth;
using MyMomsCollection.Helpers;

namespace ExpenseTracker
{
    [Activity(Label = "SliderIntroActivity", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SliderIntroActivity : AppIntro.AppIntro
    {
        FirebaseAuth firebaseAuth;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddSlide(SampleSlide.NewInstance(Resource.Layout.Intro1));
            AddSlide(SampleSlide.NewInstance(Resource.Layout.Intro2));
            AddSlide(SampleSlide.NewInstance(Resource.Layout.Intro3));
            SetFlowAnimation();
            SetColorSkipButton(Color.ParseColor("#ffffff"));
            SetColorDoneText(Color.ParseColor("#ffffff"));
            SetNextArrowColor(Color.ParseColor("#204060"));
            SetIndicatorColor(Color.ParseColor("#204060"), Color.ParseColor("#bdd3e9"));
            firebaseAuth = AppDataHelper.GetFirebaseAuth();
        }



        public override void OnDonePressed()
        {
            if (firebaseAuth.CurrentUser != null)
            {
                Finish();
            }
            else
            {

                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                Finish();
            }
        }
        public override void OnSkipPressed()
        {
            if (firebaseAuth.CurrentUser != null)
            {
                Finish();
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                Finish();
            }
           
        }
        public override void OnSlideChanged()
        {


        }
    }
}