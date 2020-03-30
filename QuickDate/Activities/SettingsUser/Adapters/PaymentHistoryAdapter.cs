using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;

namespace QuickDate.Activities.SettingsUser.Adapters
{
    public class PaymentHistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PaymentHistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<PaymentHistoryAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;

        public ObservableCollection<AffPayment> AffPaymentList = new ObservableCollection<AffPayment>();

        public PaymentHistoryAdapter(Activity context)
        {
            try
            {
                //HasStableIds = true;
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => AffPaymentList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_PaymentHistoryView
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_PaymentHistoryView, parent, false);
                var vh = new PaymentHistoryAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is PaymentHistoryAdapterViewHolder holder)
                {
                    var item = AffPaymentList[position];
                    if (item != null)
                    {
                        if (item.Id == "000")
                        {                         
                            //Dont Remove this code #####
                            FontUtils.SetFont(holder.Amount, Fonts.SfSemibold);
                            FontUtils.SetFont(holder.Requested, Fonts.SfSemibold);
                            FontUtils.SetFont(holder.Status, Fonts.SfSemibold);
                            //##### 

                            holder.Amount.Text =  item.Amount;
                            holder.Requested.Text = item.Time;
                            holder.Status.Text = item.Status; 
                        }
                        else
                        {
                            holder.Amount.Text = "$" + item.Amount;
                            holder.Requested.Text = Methods.Time.TimeAgo(Convert.ToInt32(item.Time), false);

                            switch (item.Status)
                            {
                                case "0":
                                    holder.Status.Text = ActivityContext.GetText(Resource.String.Lbl_PendingReview);
                                    break;
                                case "1":
                                    holder.Status.Text = ActivityContext.GetText(Resource.String.Lbl_Approved);
                                    break;
                                case "2":
                                    holder.Status.Text = ActivityContext.GetText(Resource.String.Lbl_Declined);
                                    break;
                            }
                        } 
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public AffPayment GetItem(int position)
        {
            return AffPaymentList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }
          
        private void Click(PaymentHistoryAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(PaymentHistoryAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class PaymentHistoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        public PaymentHistoryAdapterViewHolder(View itemView,  Action<PaymentHistoryAdapterClickEventArgs> clickListener, Action<PaymentHistoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Amount = MainView.FindViewById<TextView>(Resource.Id.amount);
                Requested = MainView.FindViewById<TextView>(Resource.Id.requested);
                Status = MainView.FindViewById<TextView>(Resource.Id.status);
                
                //Event  
                itemView.Click += (sender, e) => clickListener(new PaymentHistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new PaymentHistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

                //Dont Remove this code #####
                FontUtils.SetFont(Amount, Fonts.SfMedium);
                FontUtils.SetFont(Requested, Fonts.SfMedium);
                FontUtils.SetFont(Status, Fonts.SfMedium);
                //##### 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public TextView Amount { get; private set; }
        public TextView Requested { get; private set; } 
        public TextView Status { get; private set; } 
        
        #endregion
    }

    public class PaymentHistoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}