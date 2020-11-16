using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExpenseTracker.ViewHolder;
using XamDroid.ExpandableRecyclerView;

namespace ExpenseTracker.Helper
{
    public class Ver_ExpRecycleViewAdapter : ExpandableRecyclerAdapter<ExpenseItemParentViewHolder, ExpenseItemChildViewHolder>, IParentItemClickListener
    {

        LayoutInflater _inflater;
        Context _context;
        private IVerAdapterCallback mVerAdapterCallback;
        List<IParentObject> _itemList;
        //ExpenseItemChild selectedchildObject;
        // private IItemClickListener itemClickListener;
        //ExpenseItemChild expenseItemChild;
        public Ver_ExpRecycleViewAdapter(Context context, List<IParentObject>itemList):base(context,itemList)
        {
            _inflater = LayoutInflater.From(context);
            _context = context;
            _itemList = itemList;
            this.mVerAdapterCallback = ((IVerAdapterCallback)context);
        }
        public override void OnBindChildViewHolder(ExpenseItemChildViewHolder ExpenseItemChild, int position, object childObject)
        {
           var  expenseItemChild = (ExpenseItemChild)childObject;
           // selectedchildObject= (ExpenseItemChild)childObject;
            ExpenseItemChild.childdescname.Text = expenseItemChild.ExpenseItemDescription;

            string[] datevalue = expenseItemChild.ExpenseItemDate.Split(",");
            ExpenseItemChild.childdate.Text = datevalue[0]+ datevalue[1];
            // ExpenseItemChild._editicon.Click += _editicon_Click;
            // ExpenseItemChild._deleteicon.Click += _deleteicon_Click;
            // ExpenseItemChild._deleteicon.SetOnClickListener(this);
            ExpenseItemChild._deleteicon.Click += (o, e) =>
            {
                ExpenseItemChild expense = (ExpenseItemChild)childObject;
                mVerAdapterCallback.onDeleteMethodCallback(expense.ExpenseId);
            };
            ExpenseItemChild._editicon.Click += (o, e) =>
            {
                ExpenseItemChild expense = (ExpenseItemChild)childObject;
               mVerAdapterCallback.onEditMethodCallback(expense.ExpenseId, expense.ExpenseItemDescription, expense.ExpenseItemDate, expense.ExpenseItemAmount, expense.ExpenseItemName);

            };
          
            // ExpenseItemChild._editicon.SetOnClickListener(this);
            // ExpenseItemChild.ItemView.SetOnClickListener(this);
        }

        //private void _deleteicon_Click(object sender, EventArgs e)
        //{
        //   // mVerAdapterCallback.onDeleteMethodCallback(MONTHS[adapterPosition], lstData[adapterPosition].txtYear);
        //}

        //private void _editicon_Click(object sender, EventArgs e)
        //{
        //  //  mVerAdapterCallback.onEditMethodCallback(MONTHS[adapterPosition], lstData[adapterPosition].txtYear);
        //}



        public override void OnBindParentViewHolder(ExpenseItemParentViewHolder parentViewHolder, int position, object parentObject)
        {
            var expenseItemParent= (ExpenseItemParent)parentObject;
            parentViewHolder._textView.Text = expenseItemParent.ExpenseItem;
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            parentViewHolder._textViewAmount.Text = prefs.GetString("CurrencySymbolSelected", "") + expenseItemParent.ExpenseItemAmount;

            switch(expenseItemParent.ExpenseItem)
            {
                case "Food and Dining":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.foodanddiningicon);
                    break;
                case "Shopping":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.shoppingicon);
                    break;
                case "Travelling":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.travellingicon);
                    break;
                case "Entertainment":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.entertainmenticon);
                    break;
                case "Medical":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.medicalicon);
                    break;
                case "Personal Care":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.personalcareicon);
                    break;
                case "Education":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.educationicon);
                    break;
                case "Bills and Utilities":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.billsandutilitiesicon);
                    break;
                case "Banking":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.bankingicon);
                    break;
                case "Rent":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.renticon);
                    break;
                case "Taxes":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.taxesicon);
                    break;
                case "Insurance":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.insuranceicon);
                    break;
                case "Gifts and Donations":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.giftsdonationsicon);
                    break;
                case "Other":
                    parentViewHolder._categoryicon.SetImageResource(Resource.Drawable.othericon);
                    break;
            }
            //parentViewHolder.SetMainItemClickToExpand();


        }

        public override ExpenseItemChildViewHolder OnCreateChildViewHolder(ViewGroup childViewGroup)
        {
            var view = _inflater.Inflate(Resource.Layout.item_vertical_child, childViewGroup, false);
            return new ExpenseItemChildViewHolder(view);
        }

        public override ExpenseItemParentViewHolder OnCreateParentViewHolder(ViewGroup parentViewGroup)
        {
            var view = _inflater.Inflate(Resource.Layout.item_vertical_parent, parentViewGroup, false);
            return new ExpenseItemParentViewHolder(view);
        }

        //public void OnClick(View v)
        //{

         



        //    if (v.GetType().Name == "AppCompatImageButton")
        //    {
        //        mVerAdapterCallback.onEditMethodCallback(selectedchildObject.ExpenseId, selectedchildObject.ExpenseItemDescription, selectedchildObject.ExpenseItemDate, selectedchildObject.ExpenseItemAmount, selectedchildObject.ExpenseItemName);
        //    }
        //    else
        //    {
        //        mVerAdapterCallback.onDeleteMethodCallback(selectedchildObject.ExpenseId);
        //    }


        //}

        public interface IVerAdapterCallback
        {
            void onEditMethodCallback(String expenseID, string ExpenseItemName, string ExpenseItemDescription, string ExpenseItemDate, string ExpenseItemAmount);

            void onDeleteMethodCallback(String expenseID);
        }
    }
}