using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using FR.Ganfra.Materialspinner;
using Java.Util;
using MyMomsCollection.Helpers;
using Plugin.Connectivity;
using static Android.App.DatePickerDialog;
using static Android.Views.View;

namespace ExpenseTracker.Fragments
{
    public class Add_Edit_Expense_Fragment :Android.Support.V4.App.DialogFragment, IOnDateSetListener, IOnSuccessListener, IOnFailureListener
    {
        TextInputLayout desciptiontext, amounttext;
        TextView headertext;
        EditText datefield;
        MaterialSpinner itemspinner;
        ImageView closebutton;
        Button saveexpensebutton;
        List<string> lstitems;
        ArrayAdapter<string> adapter;
        string ítemSelected;
        DatePickerDialog picker;
        public static String[] MONTHS = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        string MonthSelected;
        string YearSelected;
        string DaySelected;
        FirebaseFirestore database;
        string FragTagName;
        string expenseID, ExpenseItemDescription, ExpenseItemDate, ExpenseItemAmount, ExpenseItemName;
        string CurrentUserUid;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Add_Edit_Expense, container, false);
            headertext= view.FindViewById<TextView>(Resource.Id.headertext);
            desciptiontext = view.FindViewById<TextInputLayout>(Resource.Id.desciptiontext);
            amounttext = view.FindViewById<TextInputLayout>(Resource.Id.amounttext);
            datefield = view.FindViewById<EditText>(Resource.Id.datefield);
            itemspinner = view.FindViewById<MaterialSpinner>(Resource.Id.itemspinner);
            saveexpensebutton=view.FindViewById<Button>(Resource.Id.saveexpensebutton);
            closebutton = view.FindViewById<ImageView>(Resource.Id.closebutton);
            closebutton.SetColorFilter(Color.ParseColor("#04040C"));
            closebutton.Click += Closebutton_Click;
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            amounttext.Hint="Amount"+"(" + prefs.GetString("CurrencySymbolSelected", "") + ")";
            database = AppDataHelper.GetDatabase();
            FragTagName = Tag;
            FetchExpenseItemsTable();
            if (FragTagName=="Add Expense")
            {
                headertext.Text = FragTagName;
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month - 1;
                int year = DateTime.Now.Year;

                picker = new DatePickerDialog(Activity);
                picker.UpdateDate(year, month, day);
                CurrentUserUid = Arguments.GetString("CurrentUserUid");


            }
            else
            {
                headertext.Text = FragTagName;
                expenseID = Arguments.GetString("expenseID");
                ExpenseItemDescription = Arguments.GetString("ExpenseItemDescription");
                ExpenseItemDate = Arguments.GetString("ExpenseItemDate");
                ExpenseItemAmount = Arguments.GetString("ExpenseItemAmount");
                ExpenseItemName = Arguments.GetString("ExpenseItemName");

                desciptiontext.EditText.Text = ExpenseItemDescription;
                amounttext.EditText.Text = ExpenseItemAmount;
                datefield.Text = ExpenseItemDate;

                string[] val = ExpenseItemDate.Split(',');

                int day = Convert.ToInt32(val[0].Trim());
                int month = Array.IndexOf(MONTHS, val[1].Trim()); 
                int year = Convert.ToInt32(val[2].Trim());
                picker = new DatePickerDialog(Activity);
                picker.UpdateDate(year, month, day);

                MonthSelected = val[1].Trim();
                YearSelected = val[2].Trim();
                DaySelected = val[0].Trim();
            }

         

            datefield.Click += Datefield_Click;
            saveexpensebutton.Click += Saveexpensebutton_Click;
          
            return view;
        }
        void FetchExpenseItemsTable()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(Activity, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
            CollectionReference ExpenseItemsRef = database.Collection("ExpenseItemsTable");
            Query query = ExpenseItemsRef.OrderBy("ExpenseItemName", Query.Direction.Ascending);
            query.Get().AddOnSuccessListener(this).AddOnFailureListener(this);

        }

        public void OnSuccess(Java.Lang.Object result)
        {
            lstitems = new List<string>();
            var snapshot = (QuerySnapshot)result;
            if (!snapshot.IsEmpty)
            {
                var documents = snapshot.Documents;
                foreach (DocumentSnapshot item in documents)
                {
                    lstitems.Add(item.Get("ExpenseItemName").ToString());
                }
                lstitems.Remove("Other");
                lstitems.Add("Other");
            }
            if (Context != null)
            {
                SetupitemsSpinner();
            }
            else
            {
                itemspinner.FloatingLabelText = "Failed to load";

            }
        }

        public void SetupitemsSpinner()
        {
           
            adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerDropDownItem, lstitems);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            itemspinner.Adapter = adapter;
           
            itemspinner.ItemSelected += Itemspinner_ItemSelected;
            if (ExpenseItemName != null && FragTagName == "Edit Expense")
            {
                int spinnerPosition = adapter.GetPosition(ExpenseItemName);
                itemspinner.SetSelection(spinnerPosition+1);
            }
        }

        private void Itemspinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position==-1)
            {
                ítemSelected = "";
            }
            else
            {
                ítemSelected = lstitems[e.Position];
            }
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            MonthSelected = MONTHS[month];
            YearSelected = year.ToString();
            DaySelected = dayOfMonth.ToString();
            datefield.Text = DaySelected + ", " + MonthSelected + ", " + YearSelected;
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void Saveexpensebutton_Click(object sender, EventArgs e)
        {
             if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(Activity, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
           if (string.IsNullOrEmpty(ítemSelected))
            {
                Toast.MakeText(Activity, "Add an item", ToastLength.Long).Show();
            }
            else if (string.IsNullOrWhiteSpace(desciptiontext.EditText.Text))
            {
                Toast.MakeText(Activity, "Add a description", ToastLength.Long).Show();
            }
            else if (string.IsNullOrWhiteSpace(amounttext.EditText.Text))
            {
                Toast.MakeText(Activity, "Add an amount", ToastLength.Long).Show();
            }
            else if (string.IsNullOrEmpty(datefield.Text))
            {
                Toast.MakeText(Activity, "Add a date", ToastLength.Long).Show();
            }
          else
            {
                if (FragTagName == "Add Expense")
                {
                    HashMap map = new HashMap();
                    map.Put("ItemName", ítemSelected);
                    map.Put("Description", desciptiontext.EditText.Text);
                    map.Put("Amount", amounttext.EditText.Text);
                    map.Put("Date", datefield.Text);
                    map.Put("DateDay", Convert.ToInt32(DaySelected));
                    map.Put("DateMonth", MonthSelected);
                    map.Put("DateYear", YearSelected);
                    map.Put("CreatedDate", DateTime.UtcNow.ToString());
                  
                    map.Put("UserUid", CurrentUserUid); 
                     DocumentReference docRef = database.Collection("ExpenseTable").Document();
                    docRef.Set(map);
                  
                }
                else
                {
                    DocumentReference docRef = database.Collection("ExpenseTable").Document(expenseID);
                    docRef.Update("ItemName", ítemSelected);
                    docRef.Update("Description", desciptiontext.EditText.Text);
                    docRef.Update("Amount", amounttext.EditText.Text);
                    docRef.Update("Date", datefield.Text);
                    docRef.Update("DateDay", Convert.ToInt32(DaySelected));
                    docRef.Update("DateMonth", MonthSelected);
                    docRef.Update("DateYear", YearSelected);
                     docRef.Update("CreatedDate", DateTime.UtcNow.ToString());
                   // docRef.Update("CreatedDate", FieldValue.ServerTimestamp());
                }
                Vibrator vibrator = (Vibrator)Activity.GetSystemService(Context.VibratorService);
                vibrator.Vibrate(30);
                ((MainActivity)Activity).DisplayAndBindMonthRecyclerView();
                this.Dismiss();
            }

        }

        private void Datefield_Click(object sender, EventArgs e)
        {
            picker.Show();
            picker.SetOnDateSetListener(this);
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
        }
    }
}