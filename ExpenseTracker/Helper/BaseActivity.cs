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
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : Activity, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private int contentDiff;
        private int rootHeight;

        public void OnGlobalLayout()
        {
            View contentView = getw().findViewById(Window.ID_ANDROID_CONTENT);
            if (rootHeight != mDrawerLayout.getRootView().getHeight())
            {
                rootHeight = mDrawerLayout.getRootView().getHeight();
                contentDiff = rootHeight - contentView.getHeight();
                return;
            }
            int newContentDiff = rootHeight - contentView.getHeight();
            if (contentDiff != newContentDiff)
            {
                if (contentDiff < newContentDiff)
                {
                    onShowKeyboard(newContentDiff - contentDiff);
                }
                else
                {
                    onHideKeyboard();
                }
                contentDiff = newContentDiff;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        
    }
}