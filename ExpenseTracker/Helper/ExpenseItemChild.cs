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
    public  class ExpenseItemChild
    {
        public string ExpenseItemDescription { get; set; }
        public string ExpenseId { get; set; }

        public string ExpenseItemDate { get; set; }
        public string ExpenseItemName { get; set; }
        public string ExpenseItemAmount { get; set; }
        public ExpenseItemChild(string xpenseItemDescription, string expenseId,string expenseItemDate,string expenseItemName,string expenseItemAmount)
        {
            ExpenseItemDescription = xpenseItemDescription;
            ExpenseId = expenseId;
            ExpenseItemDate = expenseItemDate;
            ExpenseItemName = expenseItemName;
            ExpenseItemAmount = expenseItemAmount;
        }

    }
}