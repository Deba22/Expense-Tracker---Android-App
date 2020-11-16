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
  public  class ItemCreator
    {
        static ItemCreator _itemCreator;
         Context context;
        List<ExpenseItemParent> _expenseItemParent;

        public ItemCreator(Context context)
        {
            //_expenseItemParent = new List<ExpenseItemParent>();
            //for(int i=1;i<=14;i++)
            //{
            //    var item = new ExpenseItemParent()
            //    {
            //        ExpenseItem = $"caller #{i}"
            //    };
            //_expenseItemParent.Add(item);
            //}

        }
        public static ItemCreator Get(Context context)
        {
            if(_itemCreator == null)
            {
                _itemCreator = new ItemCreator(context);
            }
            return _itemCreator;
        }

        public List<ExpenseItemParent> GetAll
        {
            get
            {
                return _expenseItemParent;
            }
          private  set
            {

            }
        }
    }
}