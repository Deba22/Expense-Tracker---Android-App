using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views.Animations;
using Android.Support.V4.Widget;

using static Android.Support.V7.Widget.SearchView;
using static Android.Support.V7.Widget.Toolbar;
using static Android.Views.View;
using Android.Support.V7.Widget;
using ExpenseTracker.Helper;
using System.Collections.Generic;
using Java.Sql;
using System;
using System.Globalization;
using Android.Graphics;
using static Android.App.DatePickerDialog;
using ExpenseTracker.Fragments;
using Firebase.Firestore;
using MyMomsCollection.Helpers;
using Java.Nio.Channels;
using XamDroid.ExpandableRecyclerView;
using Android.Gms.Tasks;
using System.Linq;
using static ExpenseTracker.Helper.Ver_ExpRecycleViewAdapter;
using static Android.Support.Design.Widget.NavigationView;
using Android.Support.V4.View;
using Firebase.Auth;
using Android.Content;
using System.Net;
using Xamarin.Facebook.Login;
using Android.Preferences;
using Android.Content.PM;
using Android.Text;
using Android.Text.Style;
using Android.Support.Design.Behavior;
using Plugin.Connectivity;
using Xamarin.Essentials;
using ShowcaseView.Interfaces;
using Square.Picasso;

namespace ExpenseTracker
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, IOnQueryTextListener, IOnClickListener, DrawerLayout.IDrawerListener,  IOnDateSetListener,IOnSuccessListener, IOnFocusChangeListener, IEventListener, IAdapterCallback, IVerAdapterCallback, IOnFailureListener, IOnNavigationItemSelectedListener, OnViewInflateListener
    {
        //,ISearchViewListener, IOnQueryTextListener,IOnMenuItemClickListener
        BottomAppBar bottomAppBar;
        FloatingActionButton fab;
        TextView statsText, homeText, Datetxt, totalexpenseVALUE, navheader_username;
        ISharedPreferences prefs;
        ImageView calendarpickericon, ProfilePic;
        //Drawable[] homeDrawable, statsDrawable;
        //EditText Searchtext;
        //ImageView SearchButton, Menubtn;
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        NestedScrollView nestedscroolview;
        private RecyclerView recyclerView, exppandablerecyclerView;
        private Hor_RecycleViewAdapter adapter;
        private RecyclerView.LayoutManager LayoutManager;
        private List<Data> lstData = new List<Data>();
        List<string> TotalExpenseValue;
        DatePickerDialog picker;
        string YearSelected;
        string MonthSelected;
        string CurrentUserUid, CurrentUserDisplayName, CurrentUserEmail, CurrentUserPhoto;
       // public static String[] MONTHS = {"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"};
        public static String[] MONTHS = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        FirebaseAuth firebaseAuth;
        Add_Edit_Expense_Fragment add_Edit_Expense_Fragment;
        FirebaseFirestore database;
        ExpenseItemParent expenseitem;
        List<IParentObject> parentObjects;
        Ver_ExpRecycleViewAdapter expenseadapter;
        List<Object> GlobalchildList;
        List<ExpenseItemChild> childListsearch;
        ImageView nodataicon;
        CurrencyModal_Fragment currencyModal_Fragment;
        TextView versionno;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
           
            SetContentView(Resource.Layout.activity_main);

            CurrentUserUid = Intent.GetStringExtra("CurrentUserUid");
            CurrentUserDisplayName = Intent.GetStringExtra("CurrentUserDisplayName");
            //CurrentUserEmail = Intent.GetStringExtra("CurrentUserEmail");
            CurrentUserPhoto = Intent.GetStringExtra("CurrentUserPhoto");
            // var ButtonClickStatus = Intent.GetStringExtra("ButtonClickStatus");
            


            database = AppDataHelper.GetDatabase();
            firebaseAuth = AppDataHelper.GetFirebaseAuth();
            this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            this.Window.SetStatusBarColor(Color.ParseColor("#204060"));

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
           
            SetSupportActionBar(toolbar);
            toolbar.SetNavigationOnClickListener(this);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
           // SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.baseline_notes_24);
            nodataicon= FindViewById<ImageView>(Resource.Id.nodataicon);
            homeText = FindViewById<TextView>(Resource.Id.homeText);
            statsText = FindViewById<TextView>(Resource.Id.statsText); 
            calendarpickericon = FindViewById<ImageView>(Resource.Id.calendarpickericon);
            calendarpickericon.SetColorFilter(Color.ParseColor("#04040C"));
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerLayout.AddDrawerListener(this);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            View headerView = navigationView.GetHeaderView(0);
            navheader_username = headerView.FindViewById<TextView>(Resource.Id.navheader_username);
            navheader_username.Text=CurrentUserDisplayName;
            ProfilePic= headerView.FindViewById<ImageView>(Resource.Id.ProfilePic);
            versionno=FindViewById<TextView>(Resource.Id.versionno);
            versionno.Text = "v " + Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;

            //Android.Net.Uri myUri = Android.Net.Uri.Parse(CurrentUserPhoto);
            //if (CrossConnectivity.Current.IsConnected)
            //{
            //    var imageBitmap = GetImageBitmapFromUrl(CurrentUserPhoto);
            //    ProfilePic.SetImageBitmap(imageBitmap);
            //}
            Picasso.Get().Load(CurrentUserPhoto).Placeholder(Resource.Drawable.icon).Into(ProfilePic);
            // ProfilePic.SetImageURI(myUri);
            //ProfilePic.SetImageURI(CurrentUserPhoto);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            nestedscroolview = FindViewById<NestedScrollView>(Resource.Id.nestedscroolview);
            MarginItemDecoration_Hor marginItemDecoration_Hor = new MarginItemDecoration_Hor(15, true, true);
            recyclerView.AddItemDecoration(marginItemDecoration_Hor);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            AppDataHelper.editor = prefs.Edit();
          
            var mString = prefs.GetString("CurrencySymbolSelected", "");
            if(mString=="")
            {
                AppDataHelper.editor.PutString("CurrencySymbolSelected", GetString(Resource.String.Rs));
                AppDataHelper.editor.PutString("CurrencyTextSelected", GetString(Resource.String.RsCurrency));
                AppDataHelper.editor.Apply();
            }

            var mStringOrderBySelected = prefs.GetString("OrderBySelected", "");
            if (mStringOrderBySelected == "")
            {
                AppDataHelper.editor.PutString("OrderBySelected", "DateDay");
                AppDataHelper.editor.Apply();
            }
            //else
            //{

            //}
            totalexpenseVALUE = FindViewById<TextView>(Resource.Id.totalexpenseVALUE);
            totalexpenseVALUE.Text = prefs.GetString("CurrencySymbolSelected", "") + "0";

            Datetxt = FindViewById<TextView>(Resource.Id.Datetxt);
            Datetxt.Text = DateTime.Now.DayOfWeek+", "+ DateTime.Today.Day+" "+DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture)+" "+DateTime.Today.Year;
            YearSelected = DateTime.Today.Year.ToString();
            MonthSelected = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);

            homeText.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.baseline_home_24), null, null);
            statsText.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.outline_insert_chart_24), null, null);

            homeText.Click += HomeText_Click;
            statsText.Click += StatsText_Click;
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += Fab_Click;
            if (VersionTracking.IsFirstLaunchEver)
            {
                currencyModal_Fragment = new CurrencyModal_Fragment();
                var trans = SupportFragmentManager.BeginTransaction();
                currencyModal_Fragment.Show(trans, "Currency");
            }
           

            calendarpickericon.Click += Calendartxt_Click;
            TotalExpenseValue = new List<string>();
            parentObjects = new List<IParentObject>();
            SetupVerticalRecyclerView();
            DisplayAndBindMonthRecyclerView();

            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month - 1;
            int year = DateTime.Now.Year;
            picker = new DatePickerDialog(this);
            picker.UpdateDate(year, month, day);
          
            bottomAppBar = FindViewById<BottomAppBar>(Resource.Id.bar);

            childListsearch = new List<ExpenseItemChild>();
            //FetchAndListenExpensesTable();
            recyclerView.SetOnClickListener(this);
        }

        public void ShowHelpScreen()
        {
            ShowCaseView showcase = new ShowCaseView.Builder()
                .Context(this)
                .FocusOn(fab)
                .CloseOnTouch(true)
                .CustomView(Resource.Layout.CustomShowcaseView, this)
                .BackgroundColor(Android.Graphics.Color.ParseColor("#F2204060"))
                .FocusBorderColor(Color.White)
                .FocusBorderSize(10)
                //.TitleGravity((int)GravityFlags.CenterHorizontal)
                .FocusCircleRadiusFactor(0.8)
                .Build();

            showcase.Show();
        }

        public void FetchExpensesTableForMonthYear()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
            CollectionReference monthyearRef = database.Collection("ExpenseTable");
            Query query = monthyearRef.WhereEqualTo("DateYear", YearSelected).WhereEqualTo("DateMonth", MonthSelected).WhereEqualTo("UserUid", CurrentUserUid).OrderBy(prefs.GetString("OrderBySelected", ""), Query.Direction.Descending);
            query.Get().AddOnSuccessListener(this).AddOnFailureListener(this);
            
        }

        public void DisplayAndBindMonthRecyclerView()
        {
            InitData();
           
            recyclerView.HasFixedSize = true;

            recyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));
            int index = lstData.FindIndex(a => a.txtMonth == MonthSelected.Substring(0, 3).ToUpper());
            adapter = new Hor_RecycleViewAdapter(this,lstData, index);
            recyclerView.SetAdapter(adapter);
           
            recyclerView.GetLayoutManager().ScrollToPosition(index);
            FetchExpensesTableForMonthYear();


        }


        private void SetupVerticalRecyclerView()
        {
            
            exppandablerecyclerView = FindViewById<RecyclerView>(Resource.Id.exppandablerecyclerView);
            exppandablerecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            //MarginItemDecoration_Hor marginItemDecoration_Ver = new MarginItemDecoration_Hor(8, true, true);
           // exppandablerecyclerView.AddItemDecoration(marginItemDecoration_Ver);
            expenseadapter = new Ver_ExpRecycleViewAdapter(this, parentObjects);
            expenseadapter.CustomParentAnimationViewId = Resource.Id.expandarrow;
            expenseadapter.SetParentClickableViewAnimationDefaultDuration();
            expenseadapter.ParentAndIconExpandOnClick = true;
      
            exppandablerecyclerView.SetAdapter(expenseadapter);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            if (TotalExpenseValue.Count > 0)
            {
                //SpannableString styledString = new SpannableString("9-10th STD");
                //styledString.SetSpan(new SuperscriptSpan(), 4, 6, 0);
                //totalexpenseVALUE.te((char[])styledString,0,4);
                var totalexpense = CalculateTotalExpense();
                //string x = Html.FromHtml("< sup >< small > 2 </ small ></ sup >").ToString();// + totalexpense;GetString(Resource.String.Rs)
                totalexpenseVALUE.Text = prefs.GetString("CurrencySymbolSelected", "") +  totalexpense;
            }
            else
            {
                totalexpenseVALUE.Text = prefs.GetString("CurrencySymbolSelected", "") + "0";
            }
        }

      
        private void Calendartxt_Click(object sender, EventArgs e)
        {
           
            picker.Show();
            picker.SetOnDateSetListener(this);
        }

        private void InitData()
        {
            lstData = new List<Data>();
            lstData.Add(new Data() { txtYear= YearSelected, txtMonth = "JAN" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "FEB" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "MAR" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "APR" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "MAY" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "JUN" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "JUL" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "AUG" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "SEP" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "OCT" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "NOV" });
            lstData.Add(new Data() { txtYear = YearSelected, txtMonth = "DEC" });
        }

       
       
        private void StatsText_Click(object sender, System.EventArgs e)
        {
            //searchView.ShowSearch(true);
            homeText.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.outline_home_24), null, null);
            statsText.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.baseline_insert_chart_24), null, null);

            Intent intent = new Intent(this, typeof(StatsActivity));
            intent.PutExtra("CurrentUserUid", firebaseAuth.CurrentUser.Uid.ToString());
            intent.PutExtra("SelectedYear", YearSelected);
            intent.PutExtra("SelectedMonth", MonthSelected);
            intent.PutExtra("CurrentUserDisplayName", CurrentUserDisplayName);
            
            StartActivity(intent);

        }

        private void HomeText_Click(object sender, System.EventArgs e)
        {
            homeText.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.baseline_home_24), null, null);
            statsText.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.outline_insert_chart_24), null, null);
            //FetchExpensesTableForMonthYear();
            nestedscroolview.SmoothScrollTo(0,0);
        }

        private void Fab_Click(object sender, System.EventArgs e)
        {
            Vibrator vibrator = (Vibrator)this.GetSystemService(VibratorService);
            vibrator.Vibrate(30);
            Bundle bundle = new Bundle();
            bundle.PutString("CurrentUserUid", CurrentUserUid);
            add_Edit_Expense_Fragment = new Add_Edit_Expense_Fragment();
            add_Edit_Expense_Fragment.Arguments = bundle;
            var trans = SupportFragmentManager.BeginTransaction();
            add_Edit_Expense_Fragment.Show(trans,"Add Expense");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

       
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
       
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            Android.Support.V7.Widget.SearchView searchView =(Android.Support.V7.Widget.SearchView) menu.GetItem(0).ActionView;
            searchView.SetOnQueryTextListener(this);
            searchView.SetOnQueryTextFocusChangeListener(this);
            return true;
        }

        public bool OnQueryTextChange(string newText)
        {
            //Toast.MakeText(this, "text changed", ToastLength.Long).Show();
            if (!string.IsNullOrWhiteSpace(newText))
            {
                childListsearch = GlobalchildList.ConvertAll(x => (ExpenseItemChild)x);
                childListsearch = (from expense in childListsearch
                                   where expense.ExpenseItemName.ToLower().Contains(newText.ToLower()) ||
                                    expense.ExpenseItemDescription.ToLower().Contains(newText.ToLower())
                                   select expense).ToList();

                //childListsearch = (from expense in childListsearch
                //                   where ((expense.ExpenseItemName.ToLower().Contains(newText.ToLower())) ||
                //                    (expense.ExpenseItemDescription.ToLower().Contains(newText.ToLower())))
                //                   select expense).ToList();

              
                parentObjects = new List<IParentObject>();
                foreach (var searchitem in childListsearch)
                {
                    // TotalExpenseValue.Add(docitem.Get("Amount").ToString());
                    var childList = new List<Object>();
                    childList.Add(new ExpenseItemChild(searchitem.ExpenseItemDescription, searchitem.ExpenseId, searchitem.ExpenseItemDate, searchitem.ExpenseItemName, searchitem.ExpenseItemAmount));

                    expenseitem = new ExpenseItemParent()
                    {

                        ExpenseItem = searchitem.ExpenseItemName,
                        ExpenseItemAmount = searchitem.ExpenseItemAmount

                    };
                    expenseitem.ChildObjectList = childList;

                    parentObjects.Add(expenseitem);

                }

                SetupVerticalRecyclerView();
            }
            else
            {
                FetchExpensesTableForMonthYear();
            }
                  return true;
        }

        public bool OnQueryTextSubmit(string newText)
        {
            //Toast.MakeText(this, "submitt", ToastLength.Long).Show();
            childListsearch = GlobalchildList.ConvertAll(x => (ExpenseItemChild)x);


            childListsearch = (from expense in childListsearch
                               where expense.ExpenseItemName.ToLower().Contains(newText.ToLower()) ||
                                expense.ExpenseItemDescription.ToLower().Contains(newText.ToLower())
                               select expense).ToList();

          
            parentObjects = new List<IParentObject>();
            foreach (var searchitem in childListsearch)
            {
                // TotalExpenseValue.Add(docitem.Get("Amount").ToString());
                var childList = new List<Object>();
                childList.Add(new ExpenseItemChild(searchitem.ExpenseItemDescription, searchitem.ExpenseId, searchitem.ExpenseItemDate, searchitem.ExpenseItemName, searchitem.ExpenseItemAmount));

                expenseitem = new ExpenseItemParent()
                {

                    ExpenseItem = searchitem.ExpenseItemName,
                    ExpenseItemAmount = searchitem.ExpenseItemAmount

                };
                expenseitem.ChildObjectList = childList;

                parentObjects.Add(expenseitem);

            }

            SetupVerticalRecyclerView();
            return true;
        }

        public static List<ExpenseItemChild> ConvertList(List<object> list)
        {
            List<ExpenseItemChild> castedList = (List<ExpenseItemChild>)list.Select(x => (ExpenseItemChild)x);
            return castedList;
        }
        //public static List<ExpenseItemChild> ConvertList(List<object> value, Type type)
        //{
        //    var containedType = type.GenericTypeArguments.First();
        //    return value.Select(item => Convert.ChangeType(item, containedType)).ToList();
        //}
        public void OnClick(View v)
        {
           // Toast.MakeText(this, "hamburger", ToastLength.Long).Show();
            drawerLayout.OpenDrawer(GravityCompat.Start);
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {

            MonthSelected = MONTHS[month];
            YearSelected = year.ToString();
            DisplayAndBindMonthRecyclerView();
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
           if(hasFocus)
            {
                bottomAppBar.Visibility = ViewStates.Gone;
                fab.Visibility = ViewStates.Gone;
            }
            else
            {
                bottomAppBar.Visibility = ViewStates.Visible;
                fab.Visibility = ViewStates.Visible;
            }
        }

        //public void OnFailure(Java.Lang.Object result)
        //{

        //}

        public void OnSuccess(Java.Lang.Object result)
        {
            TotalExpenseValue = new List<string>();
            var snapshot = (QuerySnapshot)result;
            parentObjects = new List<IParentObject>();
            GlobalchildList = new List<object>();
            if (!snapshot.IsEmpty)
            {
                var documents = snapshot.Documents;
                foreach (DocumentSnapshot docitem in documents)
                {
                    TotalExpenseValue.Add(docitem.Get("Amount").ToString());
                    expenseitem = new ExpenseItemParent();
                   var childList = new List<Object>();
                    childList.Add(new ExpenseItemChild(docitem.Get("Description").ToString(),docitem.Id.ToString(),docitem.Get("Date").ToString(),docitem.Get("ItemName").ToString(),docitem.Get("Amount").ToString()));
                 
                    expenseitem = new ExpenseItemParent()
                    {
                      
                        ExpenseItem = docitem.Get("ItemName").ToString(),
                        ExpenseItemAmount=docitem.Get("Amount").ToString()

                    };
                    expenseitem.ChildObjectList = childList;
                    GlobalchildList.Add(expenseitem.ChildObjectList[0]);
                    parentObjects.Add(expenseitem);

                }
                nodataicon.Visibility = ViewStates.Invisible;
                SetupVerticalRecyclerView();
                //if(expenseadapter!=null)
                //{
                //    expenseadapter.NotifyDataSetChanged();
                //}
            }
            else
            {
                nodataicon.Visibility = ViewStates.Visible;
                parentObjects = new List<IParentObject>();
                SetupVerticalRecyclerView();
            }

        }

        //void FetchAndListenExpensesTable()
        //{
            
        //   CollectionReference monthyearRef = database.Collection("ExpenseTable");
        //    Query query = monthyearRef.WhereEqualTo("DateYear", YearSelected).WhereEqualTo("DateMonth", MonthSelected).WhereEqualTo("UserUid", CurrentUserUid).OrderBy("DateDay", Query.Direction.Descending);
        //    query.Get().AddOnSuccessListener(this).AddOnFailureListener(this);
        //}

        public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        {
            TotalExpenseValue = new List<string>();
            var snapshot = (QuerySnapshot)obj;
            parentObjects = new List<IParentObject>();
            GlobalchildList = new List<object>();
            if (!snapshot.IsEmpty)
            {
                var documents = snapshot.Documents;
                foreach (DocumentSnapshot docitem in documents)
                {
                    TotalExpenseValue.Add(docitem.Get("Amount").ToString());
                    expenseitem = new ExpenseItemParent();
                   var childList = new List<Object>();
                    childList.Add(new ExpenseItemChild(docitem.Get("Description").ToString(), docitem.Id.ToString(), docitem.Get("Date").ToString(), docitem.Get("ItemName").ToString(), docitem.Get("Amount").ToString()));
                   // GlobalchildList.Add(childList);
                    expenseitem = new ExpenseItemParent()
                    {
                       
                        ExpenseItem = docitem.Get("ItemName").ToString(),
                        ExpenseItemAmount = docitem.Get("Amount").ToString()

                    };
                    expenseitem.ChildObjectList = childList;
                    GlobalchildList.Add(expenseitem.ChildObjectList[0]);
                    parentObjects.Add(expenseitem);

                }
              //  GlobalchildList = expenseitem.ChildObjectList;
                SetupVerticalRecyclerView();
                //if(expenseadapter!=null)
                //{
                //    expenseadapter.NotifyDataSetChanged();
                //}
            }

        }

        public void onMethodCallback(string Month,string Year)
        {
            // bottomAppBar = FindViewById<BottomAppBar>(Resource.Id.bar);
            bottomAppBar.TranslationY = 0.0f;
            fab.TranslationY = -84.0f;
            MonthSelected = Month;
            YearSelected = Year;
            FetchExpensesTableForMonthYear();
        }

      public double  CalculateTotalExpense()
        {
            var sum = 0.0;
            if (TotalExpenseValue.Count > 0)
            {

                sum= TotalExpenseValue.Sum(x => Convert.ToDouble(x));
                return sum;
            }
            else
            {
                return sum;
            }

        }

        public void onEditMethodCallback(String expenseID, string ExpenseItemDescription, string ExpenseItemDate, string ExpenseItemAmount, string ExpenseItemName)
        {
            Bundle bundle = new Bundle();
            bundle.PutString("expenseID", expenseID);
            bundle.PutString("ExpenseItemDescription", ExpenseItemDescription);
            bundle.PutString("ExpenseItemDate", ExpenseItemDate);
            bundle.PutString("ExpenseItemAmount", ExpenseItemAmount);
            bundle.PutString("ExpenseItemName", ExpenseItemName);
            // set Fragmentclass Arguments
            Add_Edit_Expense_Fragment fragobj = new Add_Edit_Expense_Fragment();
            fragobj.Arguments=bundle;

            var trans = SupportFragmentManager.BeginTransaction();
            fragobj.Show(trans, "Edit Expense");
            bottomAppBar.TranslationY = 0.0f;
            fab.TranslationY = -84.0f;
        }

        public void onDeleteMethodCallback(string expenseID)
        {
             if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
           // AlertDialog alert = dialog.Create();
            alert.SetTitle("Delete");
            alert.SetMessage("Are you sure you wan't to delete this?");
           // alert.SetIcon(Resource.Drawable.alert);
            alert.SetPositiveButton("YES", (c, ev) =>
            {
                DocumentReference docref = database.Collection("ExpenseTable").Document(expenseID);
                docref.Delete();
                bottomAppBar.TranslationY = 0.0f;
                fab.TranslationY = -84.0f;
                FetchExpensesTableForMonthYear();
            });
            alert.SetNegativeButton("CANCEL", (c, ev) => { });
            Dialog dialog = alert.Create();
            dialog.Show();


        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, e.Message, ToastLength.Long).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {

                case Resource.Id.nav_Logout:
                    {
                       
                        firebaseAuth.SignOut();
                         prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                        var mString = prefs.GetString("WhichButtonClick", "");
                        if (mString == "facebook")
                        {
                            LoginManager.Instance.LogOut();
                        }
                        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                        break;
                    }
                case Resource.Id.nav_settings:
                    {
                        Intent intent = new Intent(this, typeof(SettingsActivity));
                        StartActivity(intent);
                        break;
                    }
                case Resource.Id.nav_FeedBack:
                    {
                        var intent = new Intent(Android.Content.Intent.ActionView);
                        intent.SetData(Android.Net.Uri.Parse("https://expensetracker-feedback.web.app/"));
                        StartActivity(intent);
                        //var email = new Intent(Android.Content.Intent.ActionSend);
                        //email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { "marbles.apps@gmail.com" });
                        //email.PutExtra(Android.Content.Intent.ExtraSubject, "Feedback about "+ GetString(Resource.String.app_name));
                        //email.SetType("message/rfc822");
                        //StartActivity(email);
                        break;
                    }
                case Resource.Id.nav_rating:
                    {
                        var appPackageName = this.PackageName;
                        try
                        {
                            StartActivity(new Intent(Intent.ActionView,Android.Net.Uri.Parse("market://details?id=" + appPackageName)));
                        }
                        catch (Android.Content.ActivityNotFoundException anfe)
                        {
                            StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName)));
                        }
                        break;
                    }
            }
            //close navigation drawer
            drawerLayout.CloseDrawer(GravityCompat.Start);
            return false;
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
                // Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
               this.FinishAffinity();
            });
            alert.SetNegativeButton("CANCEL", (c, ev) => { });
            Dialog dialog = alert.Create();
            dialog.Show();
        }
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return imageBitmap;
        }
        protected override void OnResume()
        {
            base.OnResume();
            homeText.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.baseline_home_24), null, null);
            statsText.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.outline_insert_chart_24), null, null);
            // SetupVerticalRecyclerView();
            FetchExpensesTableForMonthYear();
        }

        public void OnDrawerClosed(View drawerView)
        {
            
        }

        public void OnDrawerOpened(View drawerView)
        {
            //if (CrossConnectivity.Current.IsConnected)
            //{
            //    var imageBitmap = GetImageBitmapFromUrl(CurrentUserPhoto);
            //    ProfilePic.SetImageBitmap(imageBitmap);
            //}
            Picasso.Get().Load(CurrentUserPhoto).Placeholder(Resource.Drawable.icon).Into(ProfilePic);
        }

        public void OnDrawerSlide(View drawerView, float slideOffset)
        {
         
        }

        public void OnDrawerStateChanged(int newState)
        {
           
        }

        public void OnViewInflated(View view)
        {
            //throw new NotImplementedException();
        }
    }
}