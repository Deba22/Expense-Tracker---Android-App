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
    public class ExpenseItemParentViewHolder : ParentViewHolder
    {
        public TextView _textView, _textViewAmount;
        public ImageButton _imageButton;
        public ImageView _categoryicon;
        public ExpenseItemParentViewHolder(View itemView):base(itemView)
        {
            _textView = itemView.FindViewById<TextView>(Resource.Id.parentitemname);
            _textViewAmount = itemView.FindViewById<TextView>(Resource.Id.parentitemamount);
            _imageButton = itemView.FindViewById<ImageButton>(Resource.Id.expandarrow);
            _categoryicon = itemView.FindViewById<ImageView>(Resource.Id.categoryicon);
        }
    }
}