using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ExpenseTracker.Helper
{
    public class SampleSlide : Android.Support.V4.App.Fragment
    {
        private static string ARG_LAYOUT_RES_ID = "layoutResId";
        private int layoutResId;

        public static SampleSlide NewInstance(int layoutResId)
        {
            SampleSlide sampleSlide = new SampleSlide();
            Bundle args = new Bundle();
            args.PutInt(ARG_LAYOUT_RES_ID, layoutResId);
            sampleSlide.Arguments = args;
            return sampleSlide;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Arguments != null && Arguments.ContainsKey(ARG_LAYOUT_RES_ID))
            {
                layoutResId = Arguments.GetInt(ARG_LAYOUT_RES_ID);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(layoutResId, container, false);
            //var btnFindout = view.FindViewById<Button>(Resource.Id.btnFindout);
            //btnFindout.Click -= BtnFindout_Click;
            //btnFindout.Click += BtnFindout_Click;
            return view;
        }

        //private void BtnFindout_Click(object sender, EventArgs e)
        //{
        //    var uri = Android.Net.Uri.Parse("https://www.who.int/emergencies/diseases/novel-coronavirus-2019");
        //    var intent = new Intent(Intent.ActionView, uri);
        //    StartActivity(intent);
        //}
    }
}