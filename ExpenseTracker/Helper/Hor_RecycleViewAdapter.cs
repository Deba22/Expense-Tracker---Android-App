using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ExpenseTracker.Interface;
namespace ExpenseTracker.Helper
{
    
    class Hor_RecycleViewAdapter : RecyclerView.Adapter, IItemClickListener
    {
        Context context;
        private List<Data> lstData = new List<Data>();
        private int lastselecteditem = 0;
        private int currentselectedItem = 0;
        private IAdapterCallback mAdapterCallback;
        public static String[] MONTHS = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public Hor_RecycleViewAdapter(Context appcontext,List<Data> lstData,int CurrentselectedItem)
        {
            this.lstData = lstData;
            currentselectedItem = CurrentselectedItem;
            context = appcontext;
            this.mAdapterCallback = ((IAdapterCallback)appcontext);
        }

        public override int ItemCount => lstData.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder viewHolder = holder as RecyclerViewHolder;
            viewHolder.txtYear.Text= lstData[position].txtYear;
            viewHolder.txtMonth.Text = lstData[position].txtMonth;
            if(currentselectedItem == position)
            {
                viewHolder.container.SetBackgroundResource(Resource.Drawable.curved_back_selector);
                viewHolder.txtYear.SetTextColor(Color.ParseColor("#ffffff"));
               // viewHolder.txtMonth.SetTextColor(Color.ParseColor("#ffffff"));
            }
            else
            {
                viewHolder.container.SetBackgroundResource(Resource.Drawable.curved_back);
                viewHolder.txtYear.SetTextColor(Color.ParseColor("#04040C"));
                //viewHolder.txtMonth.SetTextColor(Color.ParseColor("#04040C"));
            }
            viewHolder.SetItemClickListener(this);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View itemview = inflater.Inflate(Resource.Layout.item_horrizontal, parent, false);
            return new RecyclerViewHolder(itemview);
        }
       

        public void OnClick(View v, int adapterPosition)
        {
            lastselecteditem = currentselectedItem;
             currentselectedItem = adapterPosition;

            Vibrator vibrator = (Vibrator)context.GetSystemService(Context.VibratorService);
            vibrator.Vibrate(30);
            this.NotifyItemChanged(lastselecteditem);
            this.NotifyItemChanged(currentselectedItem);
          
            mAdapterCallback.onMethodCallback(MONTHS[adapterPosition],lstData[adapterPosition].txtYear);
        }
       
    }

    class RecyclerViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        public TextView txtYear { get; set; }
        public TextView txtMonth { get; set; }
        public LinearLayout container{ get; set; }

    private IItemClickListener itemClickListener;
        public RecyclerViewHolder(View itemView) : base(itemView)
        {
            txtYear = itemView.FindViewById<TextView>(Resource.Id.txtYear);
            txtMonth = itemView.FindViewById<TextView>(Resource.Id.txtMonth);
            container = itemView.FindViewById<LinearLayout>(Resource.Id.container);

            itemView.SetOnClickListener(this);
        }
        public void SetItemClickListener(IItemClickListener itemClickListener)
        {
            this.itemClickListener = itemClickListener;
        }

        public void OnClick(View v)
        {
            itemClickListener.OnClick(v, AdapterPosition);
        }

       
    }
    public  interface IAdapterCallback
    {
        void onMethodCallback(String Month,String Year);
    }
}