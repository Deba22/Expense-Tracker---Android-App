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
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

namespace MyMomsCollection.Helpers
{
   public static class AppDataHelper
    {
        public static ISharedPreferencesEditor editor;
        public static FirebaseFirestore GetDatabase()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseFirestore database;
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("expense-tracker-app-a176f")
                    .SetApplicationId("expense-tracker-app-a176f")
                    .SetApiKey("AIzaSyD0HaWxjzluyG97Dcxf9C5d9MUHWmQaYgQ")
                    .SetDatabaseUrl("https://expense-tracker-app-a176f.firebaseio.com")
                    .SetStorageBucket("expense-tracker-app-a176f.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, options);
                database = FirebaseFirestore.GetInstance(app);
                FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder().SetTimestampsInSnapshotsEnabled(true).Build();
                database.FirestoreSettings = settings;
            }
            else
            {
                database = FirebaseFirestore.GetInstance(app);
                FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder().SetTimestampsInSnapshotsEnabled(true).Build();
                database.FirestoreSettings = settings;
            }
            return database;
       
        }


        public static FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseFirestore database;
            FirebaseAuth mAuth;
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("expense-tracker-app-a176f")
                    .SetApplicationId("expense-tracker-app-a176f")
                    .SetApiKey("AIzaSyD0HaWxjzluyG97Dcxf9C5d9MUHWmQaYgQ")
                    .SetDatabaseUrl("https://expense-tracker-app-a176f.firebaseio.com")
                    .SetStorageBucket("expense-tracker-app-a176f.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, options);
                database = FirebaseFirestore.GetInstance(app);
                FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder().SetTimestampsInSnapshotsEnabled(true).Build();
                database.FirestoreSettings = settings;
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                database = FirebaseFirestore.GetInstance(app);
                FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder().SetTimestampsInSnapshotsEnabled(true).Build();
                database.FirestoreSettings = settings;
                mAuth = FirebaseAuth.Instance;
            }
            return mAuth;

        }

        public static string GetHighResPhoto(IList<string> providerId, string photoURL)
        {
            var result = "";
            if (providerId.Contains("facebook.com"))
            {
                result = photoURL + "?type=large";

            }
            else
            {
                result = photoURL.Replace("s96-c", "s400-c");
            }
            return result;
        }

        //public static void ShowProgressSpinner(Context context)
        //{
        //    ProgressBar progress = new ProgressBar(context);
        //    //progress.tit("Loading");
        //    //progress.SetMessage("Please wait...");
        //    //progress.SetCancelable(false); // disable dismiss by tapping outside of the dialog
        //    progress.Visibility = ViewStates.Visible;
        //}

        //public static void HideProgressSpinner(Context context)
        //{
        //    ProgressBar progress = new ProgressBar(context);
        //    //progress.tit("Loading");
        //    //progress.SetMessage("Please wait...");
        //    //progress.SetCancelable(false); // disable dismiss by tapping outside of the dialog
        //    progress.Visibility = ViewStates.Gone;
        //}
    }
}
