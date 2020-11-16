using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using MyMomsCollection.Helpers;

namespace ExpenseTracker.Fragments
{
    public class CurrencyModal_Fragment : Android.Support.V4.App.DialogFragment,IDialogInterfaceOnDismissListener
    {
        ImageView closebutton;
        TextView mainsetting;
        TextView rupeebtn, dollarbtn, poundbtn, eurobtn, yenbtn;
        ISharedPreferences prefs;
        Button savecurrencybutton;
        string currencySelected;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CurrencyModal, container, false);

            mainsetting = view.FindViewById<TextView>(Resource.Id.mainsetting);
            mainsetting.PaintFlags = PaintFlags.UnderlineText;
            mainsetting.Click += Mainsetting_Click;

            rupeebtn = view.FindViewById<TextView>(Resource.Id.rupeebtn);
            dollarbtn = view.FindViewById<TextView>(Resource.Id.dollarbtn);
            poundbtn = view.FindViewById<TextView>(Resource.Id.poundbtn);
            eurobtn = view.FindViewById<TextView>(Resource.Id.eurobtn);
            yenbtn = view.FindViewById<TextView>(Resource.Id.yenbtn);
            rupeebtn.Text = GetString(Resource.String.Rs);
            dollarbtn.Text = GetString(Resource.String.Dollar);
            poundbtn.Text = GetString(Resource.String.Pound);
            eurobtn.Text = GetString(Resource.String.Euro);
            yenbtn.Text = GetString(Resource.String.Yen);

            rupeebtn.Click += Rupeebtn_Click;
            dollarbtn.Click += Dollarbtn_Click;
            poundbtn.Click += Poundbtn_Click;
            eurobtn.Click += Eurobtn_Click;
            yenbtn.Click += Yenbtn_Click;

             closebutton = view.FindViewById<ImageView>(Resource.Id.closebutton);
            closebutton.Click += Closebutton_Click;
            savecurrencybutton = view.FindViewById<Button>(Resource.Id.savecurrencybutton);
            savecurrencybutton.Click += Savecurrencybutton_Click;
            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            AppDataHelper.editor = prefs.Edit();
            currencySelected = GetString(Resource.String.RsCurrency);
            return view;
        }

        private void Savecurrencybutton_Click(object sender, EventArgs e)
        {
            if(currencySelected== GetString(Resource.String.YenCurrency))
            {
                AppDataHelper.editor.PutString("CurrencySymbolSelected", GetString(Resource.String.Yen));
                AppDataHelper.editor.PutString("CurrencyTextSelected", GetString(Resource.String.YenCurrency));
                AppDataHelper.editor.Apply();
            }
            else if (currencySelected == GetString(Resource.String.EuroCurrency))
            {
                AppDataHelper.editor.PutString("CurrencySymbolSelected", GetString(Resource.String.Euro));
                AppDataHelper.editor.PutString("CurrencyTextSelected", GetString(Resource.String.EuroCurrency));
                AppDataHelper.editor.Apply();
            }
            else if (currencySelected == GetString(Resource.String.PoundCurrency))
            {

                AppDataHelper.editor.PutString("CurrencySymbolSelected", GetString(Resource.String.Pound));
                AppDataHelper.editor.PutString("CurrencyTextSelected", GetString(Resource.String.PoundCurrency));
                AppDataHelper.editor.Apply();
            }
            else if (currencySelected == GetString(Resource.String.DollarCurrency))
            {
                AppDataHelper.editor.PutString("CurrencySymbolSelected", GetString(Resource.String.Dollar));
                AppDataHelper.editor.PutString("CurrencyTextSelected", GetString(Resource.String.DollarCurrency));
                AppDataHelper.editor.Apply();
            }
            else
            {
                AppDataHelper.editor.PutString("CurrencySymbolSelected", GetString(Resource.String.Rs));
                AppDataHelper.editor.PutString("CurrencyTextSelected", GetString(Resource.String.RsCurrency));
                AppDataHelper.editor.Apply();
            }
            this.Dismiss();
        }

        private void Yenbtn_Click(object sender, EventArgs e)
        {
            currencySelected = GetString(Resource.String.YenCurrency);
            rupeebtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            dollarbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            poundbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            eurobtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            yenbtn.SetTextColor(Android.Graphics.Color.ParseColor("#ffffff"));

            rupeebtn.SetBackgroundResource(Resource.Drawable.curved_pienotselected);
            dollarbtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            poundbtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            eurobtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            yenbtn.SetBackgroundResource(Resource.Drawable.curved_barselected);

           
         
        }

        private void Eurobtn_Click(object sender, EventArgs e)
        {
            currencySelected = GetString(Resource.String.EuroCurrency);
            rupeebtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            dollarbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            poundbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            eurobtn.SetTextColor(Android.Graphics.Color.ParseColor("#ffffff"));
            yenbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));

            rupeebtn.SetBackgroundResource(Resource.Drawable.curved_pienotselected);
            dollarbtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            poundbtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            eurobtn.SetBackgroundResource(Resource.Drawable.rect_selected);
            yenbtn.SetBackgroundResource(Resource.Drawable.curved_barnotselected);

           
           
        }

        private void Poundbtn_Click(object sender, EventArgs e)
        {
            currencySelected = GetString(Resource.String.PoundCurrency);
            rupeebtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            dollarbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            poundbtn.SetTextColor(Android.Graphics.Color.ParseColor("#ffffff"));
            eurobtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            yenbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));

            rupeebtn.SetBackgroundResource(Resource.Drawable.curved_pienotselected);
            dollarbtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            poundbtn.SetBackgroundResource(Resource.Drawable.rect_selected);
            eurobtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            yenbtn.SetBackgroundResource(Resource.Drawable.curved_barnotselected);

           
        }

        private void Dollarbtn_Click(object sender, EventArgs e)
        {
            currencySelected = GetString(Resource.String.DollarCurrency);
            rupeebtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            dollarbtn.SetTextColor(Android.Graphics.Color.ParseColor("#ffffff"));
            poundbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            eurobtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            yenbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));

            rupeebtn.SetBackgroundResource(Resource.Drawable.curved_pienotselected);
            dollarbtn.SetBackgroundResource(Resource.Drawable.rect_selected);
            poundbtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            eurobtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            yenbtn.SetBackgroundResource(Resource.Drawable.curved_barnotselected);

           
            
        }

        private void Rupeebtn_Click(object sender, EventArgs e)
        {
            currencySelected = GetString(Resource.String.RsCurrency);
            rupeebtn.SetTextColor(Android.Graphics.Color.ParseColor("#ffffff"));
            dollarbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            poundbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            eurobtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));
            yenbtn.SetTextColor(Android.Graphics.Color.ParseColor("#204060"));

            rupeebtn.SetBackgroundResource(Resource.Drawable.curved_pieselected);
            dollarbtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            poundbtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            eurobtn.SetBackgroundResource(Resource.Drawable.rect_notselected);
            yenbtn.SetBackgroundResource(Resource.Drawable.curved_barnotselected);

            AppDataHelper.editor.PutString("CurrencySymbolSelected", GetString(Resource.String.Rs));
            AppDataHelper.editor.PutString("CurrencyTextSelected", GetString(Resource.String.RsCurrency));
            AppDataHelper.editor.Apply();
            
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void Mainsetting_Click(object sender, EventArgs e)
        {
            this.Dismiss();
            Intent intent = new Intent(Activity, typeof(SettingsActivity));
            StartActivity(intent);
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            ((MainActivity)Activity).ShowHelpScreen();
            ((MainActivity)Activity).FetchExpensesTableForMonthYear();
            //Toast.MakeText(Activity, "fired", ToastLength.Long).Show();
        }
    }
}