using Android.Views;

namespace ExpenseTracker.Helper
{
    internal interface IItemClickListener
    {
        void OnClick(View v, int adapterPosition);
    }
}