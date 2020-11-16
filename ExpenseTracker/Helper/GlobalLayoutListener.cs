using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace ExpenseTracker.Helper
{
    internal class GlobalLayoutListener : Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private static InputMethodManager _inputManager;

        private static void ObtainInputManager()
        {
            _inputManager = (InputMethodManager)TinyIoCContainer.Current.Resolve<Activity>()
                .GetSystemService(Context.InputMethodService);
        }

        public void OnGlobalLayout()
        {
            if (_inputManager.Handle == IntPtr.Zero)
            {
                ObtainInputManager();
            }
            //Keyboard service events
        }
    }
}