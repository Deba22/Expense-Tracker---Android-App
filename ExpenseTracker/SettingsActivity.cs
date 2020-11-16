using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ExpenseTracker.Helper;
using Firebase.Firestore;
using FR.Ganfra.Materialspinner;
using MyMomsCollection.Helpers;
using Plugin.Connectivity;
using static Android.Views.View;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace ExpenseTracker
{
    [Activity(Label = "SettingsActivity", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : AppCompatActivity, IOnClickListener, IOnSuccessListener, IOnFailureListener
    {
        Spinner currencyspinner;
        ISharedPreferences prefs;
        List<CurrencySymbols> lstCurrencySymbols;
        ArrayAdapter<CurrencySymbols> adapter;
        FirebaseFirestore database;
        string CurrencySymbolSelected, CurrencyTextSelected;
        RadioButton radio_Submission_Date, radio_Expense_Date;
        RadioGroup radiogrp;
        LinearLayout privacy, appintro;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);
            this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            this.Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#204060"));
           
            var toolbar = FindViewById<Toolbar>(Resource.Id.tbsettings);
            SetSupportActionBar(toolbar);
           
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.ParseColor("#04040C"), PorterDuff.Mode.SrcAtop);
            toolbar.SetNavigationOnClickListener(this);

            currencyspinner = FindViewById<Spinner>(Resource.Id.currencyspinner);
            currencyspinner.ItemSelected -= Currencyspinner_ItemSelected;
            currencyspinner.ItemSelected += Currencyspinner_ItemSelected;

            appintro = FindViewById<LinearLayout>(Resource.Id.appintro);
            appintro.Click += Appintro_Click;
            privacy = FindViewById<LinearLayout>(Resource.Id.privacy);
            privacy.Click += Privacy_Click;
            radio_Submission_Date = FindViewById<RadioButton>(Resource.Id.radio_Submission_Date);
            radio_Expense_Date = FindViewById<RadioButton>(Resource.Id.radio_Expense_Date);
            radio_Submission_Date.Click += Radio_Submission_Date_Click;
            radio_Expense_Date.Click += Radio_Expense_Date_Click;
            radiogrp = FindViewById<RadioGroup>(Resource.Id.radiogrp);
            database = AppDataHelper.GetDatabase();
          
            FetchCurrencySymbolsTable();
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            var mString = prefs.GetString("OrderBySelected", "");
            radiogrp.ClearCheck();
            if (mString== "DateDay")
            {
                radio_Expense_Date.Checked = true;
                radio_Submission_Date.Checked = false;
            }
            else
            {
                radio_Expense_Date.Checked = false;
                radio_Submission_Date.Checked = true;
            }
        }

        private void Appintro_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SliderIntroActivity)));
        }

        private void Privacy_Click(object sender, EventArgs e)
        {
            var intent =new Intent(Android.Content.Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse("https://expense-tracker-app-a176f.web.app/"));
            StartActivity(intent);
        }

        private void Radio_Expense_Date_Click(object sender, EventArgs e)
        {
            AppDataHelper.editor.PutString("OrderBySelected", "DateDay");
            AppDataHelper.editor.Apply();
            radiogrp.ClearCheck();
            radio_Expense_Date.Checked = true;
            radio_Submission_Date.Checked = false;
        }

        private void Radio_Submission_Date_Click(object sender, EventArgs e)
        {
            AppDataHelper.editor.PutString("OrderBySelected", "CreatedDate");
            AppDataHelper.editor.Apply();
            radiogrp.ClearCheck();
            radio_Expense_Date.Checked = false;
            radio_Submission_Date.Checked = true;
        }

        private void FetchCurrencySymbolsTable()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
            CollectionReference CurrencySymbolsRef = database.Collection("CurrencySymbolsTable");
            Query query = CurrencySymbolsRef.OrderBy("Currency", Query.Direction.Ascending);
            query.Get().AddOnSuccessListener(this).AddOnFailureListener(this);

        }

        public void OnClick(View v)
        {
            this.Finish();
        }
        public void SetupCurrencySpinner()
        {
         
            adapter = new ArrayAdapter<CurrencySymbols>(this, Resource.Layout.spinner_row, lstCurrencySymbols);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            currencyspinner.Adapter = adapter;
            

            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            var mString = prefs.GetString("CurrencyTextSelected", "");

            var val = lstCurrencySymbols.Where(x => x.Currency == mString).FirstOrDefault();
            int spinnerPosition = adapter.GetPosition(val);
            currencyspinner.SetSelection(spinnerPosition);
            // currencyspinner.OnItemSelectedListener = AdapterView.IOnItemSelectedListener;
        }

        private void Currencyspinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position != -1)
            {
                
                CurrencySymbolSelected = lstCurrencySymbols[e.Position].UnicodeValue;
                CurrencyTextSelected = lstCurrencySymbols[e.Position].Currency;
                AppDataHelper.editor.PutString("CurrencySymbolSelected", CurrencySymbolSelected);
                AppDataHelper.editor.PutString("CurrencyTextSelected", CurrencyTextSelected);
                AppDataHelper.editor.Apply();
            }
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            lstCurrencySymbols = new List<CurrencySymbols>();
            var snapshot = (QuerySnapshot)result;
            if (!snapshot.IsEmpty)
            {
                var documents = snapshot.Documents;
                foreach (DocumentSnapshot item in documents)
                {
                    string myStr = Regex.Unescape(item.Get("UnicodeValue").ToString());
                    lstCurrencySymbols.Add(new CurrencySymbols { Country = item.Get("Country").ToString(), Currency = item.Get("Currency").ToString(), UnicodeValue = myStr });
                   // lstitems.Add(item.Get("ExpenseItemName").ToString());
                }
            }
            if (this != null)
            {
                SetupCurrencySpinner();
            }
            else
            {
               // itemspinner.FloatingLabelText = "Failed to load";

            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, e.Message, ToastLength.Long).Show();
        }
    }
}