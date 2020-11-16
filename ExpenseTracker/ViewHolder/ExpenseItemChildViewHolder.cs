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
using XamDroid.ExpandableRecyclerView;

namespace ExpenseTracker.ViewHolder
{
   public class ExpenseItemChildViewHolder:ChildViewHolder
    {
        public TextView childdescname, childdate;
        //public TextView ExpenseId;
        public ImageButton _editicon;
         public ImageView _deleteicon  ;
        public ExpenseItemChildViewHolder(View itemView):base(itemView)
        {
            childdescname = itemView.FindViewById<TextView>(Resource.Id.childdescname);
            childdate = itemView.FindViewById<TextView>(Resource.Id.childdate);
            // ExpenseId = itemView.FindViewById<TextView>(Resource.Id.ExpenseIdHidden);
            _deleteicon = itemView.FindViewById<ImageView>(Resource.Id.deleteicon);
            _editicon = itemView.FindViewById<ImageButton>(Resource.Id.editicon);
        }
    }
}