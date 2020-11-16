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
  public  class PieChartData
    {
        public string ExpenseCategory { get; set; }
        public string ExpenseDescription { get; set; }
        public string ExpenseDate { get; set; }
        public string Amount { get; set; }
    }

    public class PieChartDataValue
    {
        public string ExpenseCategory { get; set; }
        public double Total { get; set; }
    }
}