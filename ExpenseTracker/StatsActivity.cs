using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Print;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using ExpenseTracker.Helper;
using Firebase.Firestore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microcharts.Droid;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using MyMomsCollection.Helpers;
using Plugin.Connectivity;
using static Android.Views.View;
using Color = iTextSharp.text.Color;
using Toolbar = Android.Support.V7.Widget.Toolbar;


namespace ExpenseTracker
{
    [Activity(Label = "StatsActivity", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class StatsActivity : AppCompatActivity, IOnClickListener, IOnSuccessListener, IOnFailureListener,IPermissionListener//, IAxisValueFormatter
    {
        public static string filename = "Expense Report.pdf";
        List<string> TotalExpenseJan;List<string> TotalExpenseFeb; List<string> TotalExpenseMar; List<string> TotalExpenseApr; List<string> TotalExpenseMay; List<string> TotalExpenseJun;
        List<string> TotalExpenseJul; List<string> TotalExpenseAug; List<string> TotalExpenseSep; List<string> TotalExpenseOct; List<string> TotalExpenseNov; List<string> TotalExpenseDec;
        ChartView chartview;
        ImageView showpiechart, showbarchart;
        TextView statstype, statsdata;
        FirebaseFirestore database;
        string CurrentUserUid, SelectedYear, SelectedMonth, CurrentUserDisplayName;
        List<BarChartData> lstbarChartData;
        ProgressBar progressBarStats;
        Button generatePDF;
        CoordinatorLayout rootView;
        MikePhil.Charting.Charts.BarChart barchartStats;
        MikePhil.Charting.Charts.PieChart piechartStats;
       List<string> labelnames;
        bool IsPieChartSeleted;
        ISharedPreferences prefs;
        List<string> lstExpenseCategories;
        List<PieChartData> lstPieChartData;
        List<PieChartDataValue> lstPieChartDataValue;
        TextView analysistype;
        ArrayAdapter<string> adapter;
        public static String[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        Spinner monthspinner;
        RelativeLayout monthcontainer;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_stats);
            this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            this.Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#204060"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarstats);
            SetSupportActionBar(toolbar);
            toolbar.SetNavigationOnClickListener(this);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            toolbar.NavigationIcon.SetColorFilter(Android.Graphics.Color.ParseColor("#04040C"),PorterDuff.Mode.SrcAtop);
            statstype = FindViewById<TextView>(Resource.Id.statstype);
            statsdata = FindViewById<TextView>(Resource.Id.statsdata);
            generatePDF = FindViewById<Button>(Resource.Id.generatePDF);
            showbarchart = FindViewById<ImageView>(Resource.Id.showbarchart);
            showbarchart.Click += Showbarchart_Click;
            showpiechart = FindViewById<ImageView>(Resource.Id.showpiechart);
            showpiechart.Click += Showpiechart_Click;
            analysistype = FindViewById<TextView>(Resource.Id.analysistype);
            analysistype.Text = "Category-wise Expenses";
            //chartview = FindViewById<ChartView>(Resource.Id.chartview);
            monthcontainer= FindViewById<RelativeLayout>(Resource.Id.monthcontainer);
            progressBarStats = FindViewById<ProgressBar>(Resource.Id.progressBarStats);
            rootView = FindViewById<CoordinatorLayout>(Resource.Id.rootView);
            monthspinner = FindViewById<Spinner>(Resource.Id.monthspinner);
            CurrentUserUid = Intent.GetStringExtra("CurrentUserUid");
            SelectedYear = Intent.GetStringExtra("SelectedYear");
            SelectedMonth = Intent.GetStringExtra("SelectedMonth");
            CurrentUserDisplayName = Intent.GetStringExtra("CurrentUserDisplayName");
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            //statstype.Text = "Total expense for the year"+"("+ SelectedYear+")";
            piechartStats = FindViewById<MikePhil.Charting.Charts.PieChart>(Resource.Id.piechartStats);
            piechartStats.SetNoDataText("");

            barchartStats = FindViewById<MikePhil.Charting.Charts.BarChart>(Resource.Id.barchartStats);
            barchartStats.SetNoDataText("");
            barchartStats.Description.Enabled = false;
            barchartStats.AxisRight.SetDrawLabels(false);
            database = AppDataHelper.GetDatabase();
            IsPieChartSeleted = true;
            SetupMonthSpinner();
            //FetchExpensesTableForMonth();

            // await Common.WriteFileToStorageAsync(this, "PlayfairDisplay-Regular.ttf");
            await Common.WriteFileToStorageAsync(this, "nunitosans.ttf");
            
            Dexter.WithActivity(this).WithPermission(Android.Manifest.Permission.WriteExternalStorage).WithListener(this).Check();
            generatePDF.Click -= GeneratePDF_Click;
            generatePDF.Click += GeneratePDF_Click;
           
        }
        public void SetupMonthSpinner()
        {

            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, months);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            monthspinner.Adapter = adapter;
            monthspinner.ItemSelected += Monthspinner_ItemSelected;
            if (SelectedMonth != null)
            {
                int spinnerPosition = adapter.GetPosition(SelectedMonth.Substring(0, 3));
                monthspinner.SetSelection(spinnerPosition);
            }
        }

        private void Monthspinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position != -1)
            {
                var x = e.Parent;
                var y =(TextView)e.View;
                y.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
                SelectedMonth = MainActivity.MONTHS[e.Position];
                monthspinner.Visibility = ViewStates.Visible;
                barchartStats.Visibility = ViewStates.Invisible;
                piechartStats.Visibility = ViewStates.Invisible;
                statstype.Text = "";
                statsdata.Visibility = ViewStates.Invisible;
                analysistype.Text = "Category-wise Expenses";
                FetchExpensesTableForMonth();
            }
        }

        private void Showpiechart_Click(object sender, EventArgs e)
        {
           
            FetchExpensesTableForMonth();
            monthcontainer.Visibility = ViewStates.Visible;
            monthspinner.Visibility = ViewStates.Visible;
            barchartStats.Visibility = ViewStates.Invisible;
            piechartStats.Visibility = ViewStates.Invisible;
            statstype.Text = "";
            statsdata.Visibility = ViewStates.Invisible;
            showpiechart.SetBackgroundResource(Resource.Drawable.curved_pieselected);
            showpiechart.SetImageResource(Resource.Drawable.pie_chartselectedicon);
            showpiechart.SetColorFilter(Android.Graphics.Color.ParseColor("#ffffff"));
            showbarchart.SetBackgroundResource(Resource.Drawable.curved_barnotselected);
            showbarchart.SetImageResource(Resource.Drawable.bar_chartnotselectedicon);
            showbarchart.SetColorFilter(Android.Graphics.Color.ParseColor("#204060"));
            IsPieChartSeleted = true;
            analysistype.Text = "Category-wise Expenses";
        }

        private void Showbarchart_Click(object sender, EventArgs e)
        {
            
            FetchExpensesTableForYear();
            monthcontainer.Visibility = ViewStates.Invisible;
            monthspinner.Visibility = ViewStates.Invisible;
            piechartStats.Visibility = ViewStates.Invisible;
            barchartStats.Visibility = ViewStates.Invisible;
            statstype.Text = "";
            statsdata.Visibility = ViewStates.Invisible;
            showbarchart.SetBackgroundResource(Resource.Drawable.curved_barselected);
            showbarchart.SetImageResource(Resource.Drawable.bar_chartnotselectedicon);
            showbarchart.SetColorFilter(Android.Graphics.Color.ParseColor("#ffffff"));
            showpiechart.SetBackgroundResource(Resource.Drawable.curved_pienotselected);
            showpiechart.SetImageResource(Resource.Drawable.pie_chartselectedicon);
            showpiechart.SetColorFilter(Android.Graphics.Color.ParseColor("#204060"));
            IsPieChartSeleted = false;
            analysistype.Text = "Month-wise Expenses";
        }

        //private void Showgraph_Click(object sender, EventArgs e)
        //{
        //    PopupMenu popupmenu = new PopupMenu(this, showgraph);
        //    popupmenu.MenuItemClick += Popupmenu_MenuItemClick;
        //    popupmenu.Menu.Add(Menu.None, 1, 1, "Expenses for the Month");
        //    popupmenu.Menu.Add(Menu.None, 2, 2, "Expenses for the Year");
        //    popupmenu.Show();
        //}

        private void FetchExpensesTableForYear()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                generatePDF.Visibility = ViewStates.Invisible;
                progressBarStats.Visibility = ViewStates.Invisible;
                Toast.MakeText(this, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
            progressBarStats.Visibility = ViewStates.Visible;
            CollectionReference monthyearRef = database.Collection("ExpenseTable");
            Query query = monthyearRef.WhereEqualTo("DateYear", SelectedYear).WhereEqualTo("UserUid", CurrentUserUid);
            query.Get().AddOnSuccessListener(this).AddOnFailureListener(this);

        }

        

        //private void Popupmenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        //{
        //    string stattype = e.Item.TitleFormatted.ToString();
        //   // statstype.Text = stattype;
        //    if (stattype == "Expenses for the Month")
        //    {
        //        FetchExpensesTableForMonth();
        //        barchartStats.Visibility = ViewStates.Invisible;
            
        //        IsPieChartSeleted = true;
        //    }
        //    else
        //    {
        //        FetchExpensesTableForYear();
        //        piechartStats.Visibility = ViewStates.Invisible;
               
              
        //        IsPieChartSeleted = false;
        //    }
        //}

        private void FetchExpensesTableForMonth()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                generatePDF.Visibility = ViewStates.Invisible;
                progressBarStats.Visibility = ViewStates.Invisible;
                Toast.MakeText(this, "Please check your internet connection", ToastLength.Long).Show();
                return;
            }
            progressBarStats.Visibility = ViewStates.Visible;
            CollectionReference monthyearRef = database.Collection("ExpenseTable");
            Query query = monthyearRef.WhereEqualTo("DateYear", SelectedYear).WhereEqualTo("DateMonth", SelectedMonth).WhereEqualTo("UserUid", CurrentUserUid);
            query.Get().AddOnSuccessListener(this).AddOnFailureListener(this);
        }

        
        public void OnClick(View v)
        {
            this.Finish();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            if (IsPieChartSeleted)
            {
               
                lstExpenseCategories = new List<string>();
                lstPieChartData = new List<PieChartData>();
                lstPieChartDataValue = new List<PieChartDataValue>();

                var snapshot = (QuerySnapshot)result;

                if (!snapshot.IsEmpty)
                {
                    var documents = snapshot.Documents;
                    foreach (DocumentSnapshot docitem in documents)
                    {
                        var itemName = docitem.Get("ItemName").ToString();
                        if(!lstExpenseCategories.Contains(itemName))
                        {
                            lstExpenseCategories.Add(itemName);
                        }
                        lstPieChartData.Add(new PieChartData { ExpenseCategory = docitem.Get("ItemName").ToString(), ExpenseDescription= docitem.Get("Description").ToString(), ExpenseDate= docitem.Get("Date").ToString(), Amount = docitem.Get("Amount").ToString() });
                    }

                    foreach (var item in lstExpenseCategories)
                    {
                        var ExpenseCategoryItem = item;
                        lstPieChartDataValue.Add(new PieChartDataValue { ExpenseCategory = ExpenseCategoryItem, Total = lstPieChartData.Where(x => x.ExpenseCategory == ExpenseCategoryItem).Select(x => x.Amount).Sum(x => Convert.ToDouble(x))});

                    }
                    DrawPieChart();
                }
                else
                {
                    generatePDF.Visibility = ViewStates.Invisible;
                    progressBarStats.Visibility = ViewStates.Invisible;
                    statsdata.Visibility = ViewStates.Visible;
                    statstype.Text = "Total expense for the month" + "(" + SelectedMonth.Substring(0, 3) + ")" + " = " + prefs.GetString("CurrencySymbolSelected", "") + lstPieChartDataValue.Sum(x => Convert.ToDouble(x.Total));
                }
            }
            else
            {
                TotalExpenseJan = new List<string>();
                TotalExpenseFeb = new List<string>();
                TotalExpenseMar = new List<string>();
                TotalExpenseApr = new List<string>();
                TotalExpenseMay = new List<string>();
                TotalExpenseJun = new List<string>();
                TotalExpenseJul = new List<string>();
                TotalExpenseAug = new List<string>();
                TotalExpenseSep = new List<string>();
                TotalExpenseOct = new List<string>();
                TotalExpenseNov = new List<string>();
                TotalExpenseDec = new List<string>();
                lstbarChartData = new List<BarChartData>();
                var snapshot = (QuerySnapshot)result;

                if (!snapshot.IsEmpty)
                {
                    var documents = snapshot.Documents;
                    foreach (DocumentSnapshot docitem in documents)
                    {
                        var month = docitem.Get("DateMonth").ToString();
                        switch (month)
                        {
                            case "January":
                                TotalExpenseJan.Add(docitem.Get("Amount").ToString());
                                break;
                            case "February":
                                TotalExpenseFeb.Add(docitem.Get("Amount").ToString());
                                break;
                            case "March":
                                TotalExpenseMar.Add(docitem.Get("Amount").ToString());
                                break;
                            case "April":
                                TotalExpenseApr.Add(docitem.Get("Amount").ToString());
                                break;
                            case "May":
                                TotalExpenseMay.Add(docitem.Get("Amount").ToString());
                                break;
                            case "June":
                                TotalExpenseJun.Add(docitem.Get("Amount").ToString());
                                break;
                            case "July":
                                TotalExpenseJul.Add(docitem.Get("Amount").ToString());
                                break;
                            case "August":
                                TotalExpenseAug.Add(docitem.Get("Amount").ToString());
                                break;
                            case "September":
                                TotalExpenseSep.Add(docitem.Get("Amount").ToString());
                                break;
                            case "October":
                                TotalExpenseOct.Add(docitem.Get("Amount").ToString());
                                break;
                            case "November":
                                TotalExpenseNov.Add(docitem.Get("Amount").ToString());
                                break;
                            case "December":
                                TotalExpenseDec.Add(docitem.Get("Amount").ToString());
                                break;

                        }

                    }
                    lstbarChartData.Add(new BarChartData { Month = "Jan", Total = TotalExpenseJan.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Feb", Total = TotalExpenseFeb.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Mar", Total = TotalExpenseMar.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Apr", Total = TotalExpenseApr.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "May", Total = TotalExpenseMay.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Jun", Total = TotalExpenseJun.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Jul", Total = TotalExpenseJul.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Aug", Total = TotalExpenseAug.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Sep", Total = TotalExpenseSep.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Oct", Total = TotalExpenseOct.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Nov", Total = TotalExpenseNov.Sum(x => Convert.ToDouble(x)) });
                    lstbarChartData.Add(new BarChartData { Month = "Dec", Total = TotalExpenseDec.Sum(x => Convert.ToDouble(x)) });
                    DrawBarChart();
                    generatePDF.Visibility = ViewStates.Invisible;
                }
                else
                {
                    generatePDF.Visibility = ViewStates.Invisible;
                    progressBarStats.Visibility = ViewStates.Invisible;
                    statsdata.Visibility = ViewStates.Visible;
                    statstype.Text = "Total expense for the year" + "(" + SelectedYear + ")" + " = " + prefs.GetString("CurrencySymbolSelected", "") + lstbarChartData.Sum(x => Convert.ToDouble(x.Total));
                }
            }
        }

        private void DrawPieChart()
        {
            List<PieEntry> datalist = new List<PieEntry>();
            int[] colors = new int[14];
            for (int i = 0; i < lstPieChartDataValue.Count; i++)
            {
                string expensecategory = lstPieChartDataValue[i].ExpenseCategory;
                double total = lstPieChartDataValue[i].Total;
                datalist.Add(new PieEntry((float)total, expensecategory));

                switch (expensecategory)
                {
                    case "Food and Dining":
                        colors[i] = Android.Graphics.Color.ParseColor("#FFA500");
                        break;
                    case "Shopping":
                        colors[i] = Android.Graphics.Color.ParseColor("#40C4FF");
                        break;
                    case "Travelling":
                        colors[i] = Android.Graphics.Color.ParseColor("#00BFA5");
                        break;
                    case "Entertainment":
                        colors[i] = Android.Graphics.Color.ParseColor("#e49ef0");
                        break;
                    case "Medical":
                        colors[i] = Android.Graphics.Color.ParseColor("#FF0000");
                        break;
                    case "Personal Care":
                        colors[i] = Android.Graphics.Color.ParseColor("#0EDBDB");
                        break;
                    case "Education":
                        colors[i] = Android.Graphics.Color.ParseColor("#1b49f2");
                        break;
                    case "Bills and Utilities":
                        colors[i] = Android.Graphics.Color.ParseColor("#006600");
                        break;
                    case "Banking":
                        colors[i] = Android.Graphics.Color.ParseColor("#FFAB91");
                        break;
                    case "Rent":
                        colors[i] = Android.Graphics.Color.ParseColor("#9E9D24");
                        break;
                    case "Taxes":
                        colors[i] = Android.Graphics.Color.ParseColor("#DB32B1");
                        break;
                    case "Insurance":
                        colors[i] = Android.Graphics.Color.ParseColor("#AA00FF");
                        break;
                    case "Gifts and Donations":
                        colors[i] = Android.Graphics.Color.ParseColor("#8699E3");
                        break;
                    case "Other":
                        colors[i] = Android.Graphics.Color.ParseColor("#695e5e");
                        break;
                }
            }
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            MikePhil.Charting.Data.PieDataSet pieDataSet = new MikePhil.Charting.Data.PieDataSet(datalist, "");
            pieDataSet.YValuePosition = PieDataSet.ValuePosition.OutsideSlice;
            //pieDataSet.ValueLinePart1OffsetPercentage=100f; /** When valuePosition is OutsideSlice, indicates offset as percentage out of the slice size */
            pieDataSet.ValueLinePart1Length=0.4f; /** When valuePosition is OutsideSlice, indicates length of first half of the line */
            pieDataSet.ValueLinePart2Length=0.4f;
            pieDataSet.SliceSpace = 0.5f;
            pieDataSet.SelectionShift = 5f;
            Java.Util.Random rnd = new Java.Util.Random();
            //int[] colors = new int[lstExpenseCategories.Count];
            //for (int i = 0; i < lstExpenseCategories.Count; i++)
            //{
            //    Android.Graphics.Color randomColor = Android.Graphics.Color.Rgb(rnd.NextInt(256), rnd.NextInt(256), rnd.NextInt(256));
            //    colors[i] = randomColor;
            //}


            pieDataSet.SetColors(colors);
            // pieDataSet.SetColors(PieChartColors.piecharcolors.Take(lstExpenseCategories.Count).ToArray());
            // pieDataSet.Colors = ColorTemplate.MaterialColors.Select(c => new Java.Lang.Integer(c)).ToList();
            pieDataSet.ValueTextSize = 10f;
            pieDataSet.ValueTextColor = Android.Graphics.Color.Black;
            piechartStats.Description.Enabled = false;
            piechartStats.CenterText = "";

            Legend l = piechartStats.Legend;
            l.VerticalAlignment = Legend.LegendVerticalAlignment.Bottom;
            l.HorizontalAlignment = Legend.LegendHorizontalAlignment.Left;
            l.Orientation = Legend.LegendOrientation.Horizontal;
            l.WordWrapEnabled = true;
            l.SetDrawInside(false);
            l.Enabled = true;
            piechartStats.SetExtraOffsets(0f, 2f, 0f, 2f);
            piechartStats.SetDrawEntryLabels(false);

            PieData pieData = new PieData(pieDataSet);
            piechartStats.Data = pieData;
            
            piechartStats.AnimateXY(1000,1000);
            piechartStats.Invalidate();
            progressBarStats.Visibility = ViewStates.Invisible;
            piechartStats.Visibility = ViewStates.Visible;
            statsdata.Visibility = ViewStates.Invisible;
            generatePDF.Visibility = ViewStates.Visible;
            statstype.Text = "Total expense for the month" + "(" + SelectedMonth.Substring(0, 3) + ")" + " = " + prefs.GetString("CurrencySymbolSelected", "") + lstPieChartDataValue.Sum(x => Convert.ToDouble(x.Total));

        }

        private void DrawBarChart()
        {
            List<BarEntry> datalist = new List<BarEntry>();
            labelnames = new List<string>();
            for (int i=0;i< lstbarChartData.Count; i++)
            {
                string month = lstbarChartData[i].Month;
                double total = lstbarChartData[i].Total;
                datalist.Add(new BarEntry(i, (float)total));
                labelnames.Add(month);
            }
           var SelectedMonthindex=labelnames.IndexOf(SelectedMonth.Substring(0, 3));
            int[] colors=new int[12];
            for (int i=0;i< labelnames.Count;i++)
            {
                if(i== SelectedMonthindex)
                {
                    colors[i] = Android.Graphics.Color.ParseColor("#efbe5d");
                }
                else
                {
                    colors[i] = Android.Graphics.Color.ParseColor("#204060");
                }
            }


            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            MikePhil.Charting.Data.BarDataSet barDataSet = new MikePhil.Charting.Data.BarDataSet(datalist, "Months");
            barDataSet.SetColors(colors);
            barDataSet.ValueTextSize = 8.5f;
            BarData barData = new BarData(barDataSet);
            barData.HighlightEnabled = false;
            barchartStats.Data = barData;

           // YAxis yAxis = barchartStats.AxisLeft;
           // yAxis.ValueFormatter = new IndexAxisValueFormatter(labelnames);
            XAxis xAxis = barchartStats.XAxis;
            xAxis.ValueFormatter = new IndexAxisValueFormatter(labelnames);

            xAxis.Position = XAxis.XAxisPosition.Top;
            xAxis.YOffset = -4f;
            xAxis.SetDrawAxisLine(false);
            xAxis.SetDrawGridLines(false);
            xAxis.Granularity = 1f;
            xAxis.SetLabelCount(lstbarChartData.Count, false);
            xAxis.LabelRotationAngle = 270f;
            barchartStats.AnimateY(1000);
          
            barchartStats.Invalidate();

            progressBarStats.Visibility = ViewStates.Invisible;
            barchartStats.Visibility = ViewStates.Visible;
            statsdata.Visibility = ViewStates.Invisible;
            statstype.Text = "Total expense for the year" + "(" + SelectedYear + ")"+" = "+ prefs.GetString("CurrencySymbolSelected", "") + lstbarChartData.Sum(x => Convert.ToDouble(x.Total));
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            progressBarStats.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, e.Message, ToastLength.Long).Show();
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            
            var snackbar = Snackbar.Make(rootView, "You must accept this permission as its required to generate reports.", Snackbar.LengthLong)
                 .SetAction("SETTINGS", V => CallSettings());
            snackbar.SetActionTextColor(Android.Graphics.Color.ParseColor("#efbe5d"));
            snackbar.Show();
        }

        private void CallSettings()
        {
            Intent intent = new Intent(Settings.ActionApplicationDetailsSettings);
            Android.Net.Uri uri = Android.Net.Uri.FromParts("package", this.PackageName, null);
           // intent.AddFlags(ActivityFlags.NewTask);
            intent.SetData(uri);
            StartActivity(intent);
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {

            generatePDF.Click -= GeneratePDF_Click;
            generatePDF.Click += GeneratePDF_Click;
        }

        private void GeneratePDF_Click(object sender, EventArgs e)
        {
            if (this.CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) == Permission.Granted)
            {
                CreatePDFFile(Common.GetAppPath(this) + filename);
            }
            else
            {
                var snackbar = Snackbar.Make(rootView, "You must accept this permission as its required to generate reports.", Snackbar.LengthLong)
                .SetAction("SETTINGS", V => CallSettings());
                snackbar.SetActionTextColor(Android.Graphics.Color.ParseColor("#efbe5d"));
                snackbar.Show();
            }
        }

        private void CreatePDFFile(string p)
        {
            if (new Java.IO.File(p).Exists())
                new Java.IO.File(p).Delete();
            try
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, new FileStream(p, FileMode.Create));
                document.Open();
                document.SetPageSize(PageSize.A4);
                document.AddCreationDate();
                //document.AddAuthor("DebWeb");
                //document.AddCreator("Deb");

                Color colorAccesnt=new Color(0, 32, 64, 96);
                float fontSize = 24.0f, valueFontSize = 26.0f;
                BaseFont fontName = BaseFont.CreateFont(Common.GetFilePath(this, "nunitosans.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                //BaseFont fontName = FontFactory.GetFont(Common.GetFilePath(this, "PlayfairDisplay-Regular.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12);
                //Font f1 = FontFactory.GetFont(Common.GetFilePath(this, "PlayfairDisplay-Regular.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12);
                Font titleFont = new Font(fontName, 33.0f, Font.NORMAL, colorAccesnt);
                Font expenseNameFont = new Font(fontName, fontSize, Font.NORMAL, Color.BLACK);
                Font expenseValueFont = new Font(fontName, valueFontSize, Font.NORMAL, colorAccesnt);

                AddNewItem(document, "REPORT", Element.ALIGN_CENTER, titleFont);

                AddNewItem(document, "Report for the month/year: "+ SelectedMonth+" "+SelectedYear, Element.ALIGN_LEFT, expenseNameFont);
                AddNewItem(document, "Report date: " + DateTime.UtcNow.ToString(), Element.ALIGN_LEFT, expenseNameFont);
                AddNewItem(document, "Report generated by: " + CurrentUserDisplayName, Element.ALIGN_LEFT, expenseNameFont);
                AddLineSeparator(document);

                AddNewItem(document, "Expense Details", Element.ALIGN_CENTER, titleFont);
                foreach (var item in lstPieChartData)
                {
                    AddNewItemWithLeftAndRight(document, item.ExpenseCategory, prefs.GetString("CurrencySymbolSelected", "")+item.Amount, expenseValueFont, expenseValueFont);
                    AddNewItem(document, item.ExpenseDescription, Element.ALIGN_LEFT, expenseNameFont);
                    string[] datevalue = item.ExpenseDate.Split(",");
                    AddNewItem(document, datevalue[0] + " " + datevalue[1], Element.ALIGN_LEFT, expenseNameFont);
                    AddLineSeparator(document);
                }

                AddLineSpace(document);
                AddLineSpace(document);

                AddNewItemWithLeftAndRight(document, "Total Expense", prefs.GetString("CurrencySymbolSelected", "")+ lstPieChartDataValue.Sum(x => Convert.ToDouble(x.Total)).ToString(), titleFont, titleFont);

                document.Close();

                PrintPDF();
            }
            catch(FileNotFoundException e)
            {
                Log.Debug("",e.Message);
            }
            catch(DocumentException e)
            {
                Log.Debug("", e.Message);
            }
            catch(IOException e)
            {
                Log.Debug("", e.Message);
            }
        }

        private void PrintPDF()
        {
            PrintManager printManager = (PrintManager)GetSystemService(Context.PrintService);
            try
            {
                PrintPDFAdapter adapter = new PrintPDFAdapter(this, Common.GetAppPath(this)+filename);
                printManager.Print("Document", adapter, new PrintAttributes.Builder().Build());
            }
            catch(Exception e)
            {
                Log.Error("",e.Message);
            }
        }

        private void AddNewItemWithLeftAndRight(Document document, string lefttext, string righttext, Font leftfont, Font rightfont)
        {
            Chunk chunkleft = new Chunk(lefttext, leftfont);
            Chunk chunkright = new Chunk(righttext, rightfont);
            Paragraph p = new Paragraph(chunkleft);
            p.Add(new Chunk(new VerticalPositionMark()));
            p.Add(chunkright);
            document.Add(p);
        }

        private void AddLineSeparator(Document document)
        {
            LineSeparator lineSeparator = new LineSeparator();
            lineSeparator.LineColor = new Color(0, 0, 0, 68);
            AddLineSpace(document);
            document.Add(new Chunk(lineSeparator));
            AddLineSpace(document);
        }

        private void AddLineSpace(Document document)
        {
            document.Add(new Paragraph(""));
        }

        private void AddNewItem(Document document, string text, int align, Font font)
        {
            Chunk chunk = new Chunk(text, font);
            Paragraph p = new Paragraph(chunk);
            p.Alignment = align;
            document.Add(p);
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
            p1.ContinuePermissionRequest();
        }
        public override void OnBackPressed()
        {
            this.Finish();
        }
        protected override void OnResume()
        {
            base.OnResume();

            if (this.CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage)== Permission.Granted)
            {
                generatePDF.Click -= GeneratePDF_Click;
                generatePDF.Click += GeneratePDF_Click;
               
            }
        }

        //public string GetFormattedValue(float value, AxisBase axis)
        //{
         
        //    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
        //    return prefs.GetString("CurrencySymbolSelected", "") + value;
        //}
    }
}