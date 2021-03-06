﻿using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Content;
using System.Collections.Generic;
using System;
using Android.Views;
using Java.Lang;
using Android.Widget;

namespace tbs_app
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class MainActivity : AppCompatActivity
    {

        public static bool firstOpening = true;
        //private static string[] titles = null;

        public static int MSG_WEBVIEW_CONSTRUCTOR = 1;
        public static int MSG_WEBVIEW_POLLING = 2;

        // /////////////////////////////////////////////////////////////////////////////////////////////////
        // add constant here
        private const int TBS_WEB = 0;
        private const int FULL_SCREEN_VIDEO = 1;
        private const int FILE_CHOOSER = 2;

        // /////////////////////////////////////////////////////////////////////////////////////////////
        // for view init
        private Context mContext = null;
        private GridView gridView;

        private static bool main_initialized = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main_advanced);

            mContext = this;
            if (!main_initialized)
            {
                this.Init();
            }
        }

        protected override void OnResume()
        {
            this.Init();

            base.OnResume();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Inflate the menu; this adds items to the action bar if it is present.
            return true;
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            // TODO Auto-generated method stub
            if (keyCode == Keycode.Back)
                this.TbsSuiteExit();

            return base.OnKeyDown(keyCode, e);
        }

        private void TbsSuiteExit()
        {
            new Android.App.AlertDialog.Builder(mContext)
                .SetTitle("X5功能演示")
                .SetPositiveButton("OK", new BrowserActivity.CusDownloadListener.CusDialogInterfaceOnClickListener((dialog, arg) =>
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }))
                .SetMessage("quit now?")
                .Create()
                .Show();
        }

        private void Init()
        {
            this.gridView = FindViewById<GridView>(Resource.Id.item_grid);

            if (gridView == null)
                throw new Java.Lang.IllegalArgumentException("the gridView is null");

            this.gridView.Adapter = new CusAdapter(this);
            this.gridView.OnItemClickListener = new OnItemClickListener(this);

            //var titles = Resources.GetStringArray(Resource.Array.index_titles);
            //int[] iconResourse = { Resource.Drawable.tbsweb, Resource.Drawable.fullscreen, Resource.Drawable.filechooser };

            //for (int i = 0; i < titles.Length; i++)
            //{
            //    items.Add(new Dictionary<string, object> { { "title", titles[i] }, { "icon", iconResourse[i] } });
            //}
            //this.gridAdapter = new SimpleAdapter(mContext, items, Resource.Layout.function_block, new string[] { "title", "icon" }, new int[] { Resource.Id.Item_text, Resource.Id.Item_bt });

            //if (null != this.gridView)
            //{
            //    //this.gridView.Adapter = gridAdapter;
            //    //this.gridAdapter.NotifyDataSetChanged();
            //    //this.gridView.OnItemClickListener = new OnItemClickListener(this);
            //}

            main_initialized = true;
        }

        internal class CusAdapter : BaseAdapter
        {
            readonly Activity currentActivity;
            readonly string[] titles;
            readonly int[] iconResources;

            public CusAdapter(Activity c)
            {
                currentActivity = c;

                titles = currentActivity.Resources.GetStringArray(Resource.Array.index_titles);
                iconResources = new int[] { Resource.Drawable.tbsweb, Resource.Drawable.fullscreen, Resource.Drawable.filechooser };
            }


            public override int Count => titles.Length;

            public override Java.Lang.Object GetItem(int position)
            {
                return null;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (convertView == null)
                    convertView = View.Inflate(currentActivity, Resource.Layout.function_block, null);

                ImageView iv = convertView.FindViewById<ImageView>(Resource.Id.Item_bt);
                TextView tv = convertView.FindViewById<TextView>(Resource.Id.Item_text);

                iv.SetImageResource(iconResources[position]);
                tv.Text = titles[position];

                return convertView;
            }
        }

        internal class OnItemClickListener : Java.Lang.Object, AdapterView.IOnItemClickListener
        {
            private readonly Activity currentActivity;

            internal OnItemClickListener(Activity activity)
            {
                currentActivity = activity;
            }

            public void OnItemClick(AdapterView parent, View view, int position, long id)
            {
                switch (position)
                {
                    case FILE_CHOOSER:
                        {
                            currentActivity.StartActivity(new Intent(currentActivity, typeof(FilechooserActivity)));
                        }
                        break;
                    case FULL_SCREEN_VIDEO:
                        {
                            currentActivity.StartActivity(new Intent(currentActivity, typeof(FullScreenActivity)));
                        }
                        break;
                    case TBS_WEB:
                        {
                            currentActivity.StartActivity(new Intent(currentActivity, typeof(BrowserActivity)));
                        }
                        break;

                }
            }
        }
    }
}