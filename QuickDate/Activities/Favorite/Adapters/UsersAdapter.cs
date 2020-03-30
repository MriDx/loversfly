using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Refractored.Controls;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Favorite.Adapters
{
    public class UsersAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        #region Variables Basic

        private readonly Activity ActivityContext;
        private readonly HomeActivity HomeActivity;
        public ObservableCollection<UserInfoObject> UserList = new ObservableCollection<UserInfoObject>();
        public event EventHandler<UsersAdapterClickEventArgs> OnItemClick;
        public event EventHandler<UsersAdapterClickEventArgs> OnItemLongClick;
        public readonly string NameButton;

        #endregion

        public UsersAdapter(Activity context, HomeActivity homeActivity , string nameButton)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                HomeActivity = homeActivity;
                NameButton = nameButton;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => UserList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_FavoriteView
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_FavoriteView, parent, false);
                var vh = new UsersAdapterViewHolder(this, itemView, Click, LongClick);
                return vh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is UsersAdapterViewHolder holder)
                {
                    var item = UserList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext,item.Avater, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        holder.ImageOnline.Visibility = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online) ? ViewStates.Visible: ViewStates.Gone;

                        holder.Name.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item), 14);

                        switch (NameButton)
                        {
                            case "FavoriteUsers":
                                holder.Button.Text = ActivityContext.GetString(Resource.String.Lbl_UnFavorite);
                                break;
                            case "Friends":
                            {
                                if (item.IsFriend != null && item.IsFriend.Value)
                                {
                                    holder.Button.Text = ActivityContext.GetString(Resource.String.Lbl_UnFriend);
                                    holder.Button.Tag = "true";
                                }
                                else if (item.IsFriend != null && !item.IsFriend.Value)
                                {
                                    if (item.IsFriendRequest != null && !item.IsFriendRequest.Value)
                                    {
                                        holder.Button.Text = ActivityContext.GetString(Resource.String.Lbl_AddFriend);
                                        holder.Button.Tag = "false";
                                    }
                                    else
                                    {
                                        holder.Button.Text = ActivityContext.GetString(Resource.String.Lbl_UnFriend);
                                        holder.Button.Tag = "true";
                                    }
                                } 
                            }  break; 
                        } 
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void FavoriteButtonClick(UsersClickEventArgs e)
        {
            try
            {
                if (e.UserClass == null)
                    e.UserClass = GetItem(e.Position);

                if (e.UserClass != null)
                { 
                    var index = UserList.IndexOf(UserList.FirstOrDefault(a => a.Id == e.UserClass.Id));
                    if (index != -1)
                    {
                        UserList.Remove(e.UserClass);
                        NotifyItemRemoved(index);
                        NotifyItemRangeRemoved(0, ItemCount);
                    }

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.Remove_Favorite(e.UserClass);
                    sqlEntity.Dispose();

                  var countList =  HomeActivity?.ProfileFragment?.FavoriteFragment?.MAdapter?.ItemCount;
                  if (countList == 0)
                  {
                      HomeActivity?.ProfileFragment?.FavoriteFragment?.ShowEmptyPage();
                  }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }
         
        public void FriendButtonClick(UsersClickEventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    return;
                }

                if (e.UserClass == null)
                    e.UserClass = GetItem(e.Position);

                if (e.UserClass != null)
                {
                    if (e.ButtonFollow.Tag.ToString() == "true")
                    {
                        e.ButtonFollow.Text = ActivityContext.GetString(Resource.String.Lbl_AddFriend);
                        e.ButtonFollow.Tag = "false";
                        e.UserClass.IsFriend = false;
                        e.UserClass.IsFriendRequest = false; 

                        var index = UserList.IndexOf(UserList.FirstOrDefault(a => a.Id == e.UserClass.Id));
                        if (index != -1)
                        {
                            UserList.Remove(e.UserClass);
                            NotifyItemRemoved(index);
                            NotifyItemRangeRemoved(0, ItemCount);
                        }

                        var countList = HomeActivity?.ProfileFragment?.FriendsFragment?.MAdapter?.ItemCount;
                        if (countList == 0)
                        {
                            HomeActivity?.ProfileFragment?.FriendsFragment?.ShowEmptyPage();
                        }
                        // Send Api Remove
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Friends.AddOrRemoveFriendsAsync(e.UserClass.Id.ToString()) });
                    } 
                    else if (e.ButtonFollow.Tag.ToString() == "false")
                    {
                        e.ButtonFollow.Text = ActivityContext.GetString(Resource.String.Lbl_UnFriend);
                        e.ButtonFollow.Tag = "true";
                        e.UserClass.IsFriend = true;
                        e.UserClass.IsFriendRequest = true;

                        var index = UserList.IndexOf(UserList.FirstOrDefault(a => a.Id == e.UserClass.Id));
                        if (index != -1)
                        {
                            NotifyItemChanged(index);
                        }

                        // Send Api Add
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Friends.AddOrRemoveFriendsAsync(e.UserClass.Id.ToString()) });
                    }
                }
               
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }
         
     
         
        public UserInfoObject GetItem(int position)
        {
            return UserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        private void Click(UsersAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        private void LongClick(UsersAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UserList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Avater != "")
                {
                    d.Add(item.Avater);
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
            return Glide.With(ActivityContext).Load(p0.ToString())
                .Apply(new RequestOptions().CircleCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class UsersAdapterViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        #region Variables Basic

        private UsersAdapter UsersAdapter;
        public View MainView { get; }
        public ImageView Image { get; private set; }
        public CircleImageView ImageOnline { get; private set; }

        public TextView Name { get; private set; }
        public TextView LastTimeOnline { get; private set; }
        public Button Button { get; private set; }
        
        #endregion

        public UsersAdapterViewHolder(UsersAdapter users , View itemView, Action<UsersAdapterClickEventArgs> clickListener, Action<UsersAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                UsersAdapter = users;

                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.people_profile_sos);
                ImageOnline = MainView.FindViewById<CircleImageView>(Resource.Id.ImageLastseen);
                Name = MainView.FindViewById<TextView>(Resource.Id.people_profile_name);
                LastTimeOnline = MainView.FindViewById<TextView>(Resource.Id.people_profile_time);
                Button = MainView.FindViewById<Button>(Resource.Id.btn_UnFavorite);
                 
                //Dont Remove this code #####
                FontUtils.SetFont(Name, Fonts.SfRegular);
                FontUtils.SetFont(LastTimeOnline, Fonts.SfMedium);
                FontUtils.SetFont(Button, Fonts.SfRegular);
                //#####

                Button.SetOnClickListener(this);

                itemView.Click += (sender, e) => clickListener(new UsersAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
                itemView.LongClick += (sender, e) => longClickListener(new UsersAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                if (AdapterPosition == RecyclerView.NoPosition) return;

                if (v.Id == Button.Id)
                {
                    switch (UsersAdapter.NameButton)
                    {
                        case "FavoriteUsers":
                            UsersAdapter.FavoriteButtonClick(new UsersClickEventArgs { View = MainView, UserClass = null, Position = AdapterPosition, ButtonFollow = Button });
                            break;
                        case "Friends":
                            UsersAdapter.FriendButtonClick(new UsersClickEventArgs { View = MainView, UserClass = null, Position = AdapterPosition, ButtonFollow = Button });
                            break;
                    } 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class UsersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; } 
        public ImageView Image { get; set; } 
    }

    public class UsersClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public UserInfoObject UserClass { get; set; }
        public Button ButtonFollow { get; set; }
    }
}