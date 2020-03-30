using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using AT.Markushi.UI;
using QuickDateClient.Classes.Global;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.MyProfile.Adapters
{
    public class AllMediaAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<AllMediaAdapterClickEventArgs> OnRemoveItemClick;
        public event EventHandler<AllMediaAdapterClickEventArgs> OnItemClick;
        public event EventHandler<AllMediaAdapterClickEventArgs> OnItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<MediaFile> MediaList = new ObservableCollection<MediaFile>();

        public AllMediaAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_MediaView
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_MediaView, parent, false);
                var vh = new AllMediaAdapterViewHolder(itemView, RemoveClick, Click, LongClick);
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
                if (viewHolder is AllMediaAdapterViewHolder holder)
                {
                    var item = MediaList[position];
                    if (item != null)
                    {
                        var type = Methods.AttachmentFiles.Check_FileExtension(item.Full);
                        if (type == "Video" || item.Avater.Contains("video_thumb"))
                        { 
                            GlideImageLoader.LoadImage(ActivityContext, item.Avater, holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                            holder.PlayIcon.Visibility = ViewStates.Visible;
                        }
                        else if (type == "Image")
                        {
                            GlideImageLoader.LoadImage(ActivityContext, item.Avater, holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                            holder.PlayIcon.Visibility = ViewStates.Gone;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
          
        public void Remove(MediaFile item)
        {
            try
            {
                var index = MediaList.IndexOf(MediaList.FirstOrDefault(a => a.Id == item.Id));
                if (index != -1)
                {
                    MediaList.Remove(item);
                    NotifyItemRemoved(index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override int ItemCount => MediaList?.Count ?? 0;

        public MediaFile GetItem(int position)
        {
            return MediaList[position];
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

        void RemoveClick(AllMediaAdapterClickEventArgs args) => OnRemoveItemClick?.Invoke(this, args);
        void Click(AllMediaAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(AllMediaAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = MediaList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (!string.IsNullOrEmpty(item.Full))
                {
                    d.Add(item.Full);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString()).Apply(new RequestOptions().CircleCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class AllMediaAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; } 
        public ImageView Image { get; private set; }
        public FrameLayout Frame { get; private set; }
        public CardView Card { get; private set; }
        public TextView PlayIcon { get; private set; }
        public CircleButton RemoveButton { get; private set; }
 
        #endregion

        public AllMediaAdapterViewHolder(View itemView, Action<AllMediaAdapterClickEventArgs> removeClickListener, Action<AllMediaAdapterClickEventArgs> clickListener, Action<AllMediaAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                Image  = (ImageView)MainView.FindViewById(Resource.Id.Image);
                Frame = MainView.FindViewById<FrameLayout>(Resource.Id.viewfrm); 
                Card = (CardView)MainView.FindViewById(Resource.Id.card);
                PlayIcon = (TextView)MainView.FindViewById(Resource.Id.playIcon);
                RemoveButton = (CircleButton)MainView.FindViewById(Resource.Id.removeButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, PlayIcon, FontAwesomeIcon.PlayCircle);

                //Event
                RemoveButton.Click += (sender, e) => removeClickListener(new AllMediaAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.Click += (sender, e) => clickListener(new AllMediaAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new AllMediaAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class AllMediaAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}