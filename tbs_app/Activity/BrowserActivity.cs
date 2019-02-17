using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Tencent.Smtt.Export.External.Interfaces;
using Com.Tencent.Smtt.Sdk;
using Com.Tencent.Smtt.Utils;
using Java.Lang;
using tbs_app.utils;

namespace tbs_app
{
    [Activity(Label = "BrowserActivity")]
    public class BrowserActivity : Activity
    {
        private static Activity currentActivity;

        /**
	 * 作为一个浏览器的示例展示出来，采用android+web的模式
	 */
        private static X5WebView mWebView;
        private static ViewGroup mViewParent;
        private static ImageButton mBack;
        private static ImageButton mForward;
        private static ImageButton mExit;
        private static ImageButton mHome;
        private ImageButton mMore;
        private Button mGo;
        private EditText mUrl;

        private static readonly string mHomeUrl = "http://app.html5.qq.com/navi/index";
        //private static readonly string TAG = "SdkDemo";
        private static readonly int MAX_LENGTH = 14;
        private static bool mNeedTestPage = false;

        private static readonly int disable = 120;
        private static readonly int enable = 255;

        private static ProgressBar mPageLoadingProgressBar = null;

        private IValueCallback uploadFile;

        private static Java.Net.URL mIntentUrl;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            utils.LoggerManager.CurrentLogger.Debug("BrowserActivity OnCreate");

            currentActivity = this;

            Window.SetFormat(Android.Graphics.Format.Translucent);

            Intent intent = Intent;

            if (intent != null && intent.Data != null)
            {
                mIntentUrl = new Java.Net.URL(intent.Data.ToString());
            }


            try
            {
                if (int.Parse(Android.OS.Build.VERSION.Sdk) >= 11)
                {
                    Window.SetFlags(WindowManagerFlags.HardwareAccelerated, WindowManagerFlags.HardwareAccelerated);
                }
            }
            catch (Java.Lang.Exception e)
            {
            }


            //getWindow().addFlags(
            //android.view.WindowManager.LayoutParams.FLAG_FULLSCREEN);

            SetContentView(Resource.Layout.activity_main);
            mViewParent = FindViewById<ViewGroup>(Resource.Id.webView1);

            InitBtnListenser();

            mTestHandler.SendEmptyMessageDelayed(MSG_INIT_UI, 10);

        }

        private void InitBtnListenser()
        {
            mBack = FindViewById<ImageButton>(Resource.Id.btnBack1);
            mForward = FindViewById<ImageButton>(Resource.Id.btnForward1);
            mExit = FindViewById<ImageButton>(Resource.Id.btnExit1);
            mHome = FindViewById<ImageButton>(Resource.Id.btnHome1);
            mGo = FindViewById<Button>(Resource.Id.btnGo1);
            mUrl = FindViewById<EditText>(Resource.Id.editUrl1);
            mMore = FindViewById<ImageButton>(Resource.Id.btnMore);
            if (int.Parse(Android.OS.Build.VERSION.Sdk) >= 16)
            {
                mBack.SetAlpha(disable);
                mForward.SetAlpha(disable);
                mHome.SetAlpha(disable);
            }
            mHome.Enabled = false;


            mBack.SetOnClickListener(new CusOnClickListener(view =>
            {
                if (mWebView != null && mWebView.CanGoBack())
                    mWebView.GoBack();
            }));

            mForward.SetOnClickListener(new CusOnClickListener(view =>
            {
                if (mWebView != null && mWebView.CanGoForward())
                    mWebView.GoForward();
            }));

            mGo.SetOnClickListener(new CusOnClickListener(view =>
            {
                mWebView.LoadUrl(mUrl.Text);
                mWebView.RequestFocus();
            }));

            mMore.SetOnClickListener(new CusOnClickListener(view =>
            {
                Toast.MakeText(this, "not completed", ToastLength.Long).Show();
            }));

            mUrl.OnFocusChangeListener = new CusOnFocusChangeListener(this, mUrl, mGo);

            mUrl.AddTextChangedListener(new CusTextChangedListener(mUrl, mGo));

            mHome.SetOnClickListener(new CusOnClickListener(view =>
            {
                if (mWebView != null)
                    mWebView.LoadUrl(mHomeUrl);
            }));

            mExit.SetOnClickListener(new CusOnClickListener(view =>
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }));
        }

        //bool[] m_selected = new bool[] { true, true, true, true, false, false, true };
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                if (mWebView != null && mWebView.CanGoBack())
                {
                    mWebView.GoBack();
                    if (int.Parse(Android.OS.Build.VERSION.Sdk) >= 16)
                        ChangGoForwardButton(mWebView);
                    return true;
                }
                else
                {
                    return base.OnKeyDown(keyCode, e);
                }
            }

            return base.OnKeyDown(keyCode, e);
        }

        private static void ChangGoForwardButton(WebView view)
        {
            if (view.CanGoBack())
                mBack.SetAlpha(enable);
            else
                mBack.SetAlpha(disable);
            if (view.CanGoForward())
                mForward.SetAlpha(enable);
            else
                mForward.SetAlpha(disable);
            if (view.Url != null && view.Url.Equals(mHomeUrl, StringComparison.CurrentCultureIgnoreCase))
            {
                mHome.SetAlpha(disable);
                mHome.Enabled = false;
            }
            else
            {
                mHome.SetAlpha(enable);
                mHome.Enabled = true;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            utils.LoggerManager.CurrentLogger.Debug("onActivityResult, requestCode:" + requestCode + ",resultCode:" + resultCode);

            if (resultCode == Android.App.Result.Ok)
            {
                switch (requestCode)
                {
                    case 0:
                        if (null != uploadFile)
                        {
                            Android.Net.Uri result = data == null || resultCode != Result.Ok ? null : data.Data;
                            uploadFile.OnReceiveValue(result);
                            uploadFile = null;
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (resultCode == Result.Canceled)
            {
                if (null != uploadFile)
                {
                    uploadFile.OnReceiveValue(null);
                    uploadFile = null;
                }

            }


        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent == null || mWebView == null || intent.Data == null)
                return;

            mWebView.LoadUrl(intent.Data.ToString());
        }

        protected override void OnDestroy()
        {

            if (mTestHandler != null)
                mTestHandler.RemoveCallbacksAndMessages(null);

            if (mWebView != null)
            {
                mWebView.Destroy();
                mWebView = null;
            }

            base.OnDestroy();
        }

        public const int MSG_OPEN_TEST_URL = 0;
        public const int MSG_INIT_UI = 1;
        private const int mUrlStartNum = 0;
        private static int mCurrentUrl = mUrlStartNum;
        private static Handler mTestHandler = new CusHandler(mWebView);

        internal class CusHandler : Handler
        {

            private readonly X5WebView webview;

            internal CusHandler(X5WebView view)
            {
                webview = view;
            }

            public override void HandleMessage(Message msg)
            {
                switch (msg.What)
                {
                    case MSG_OPEN_TEST_URL:
                        {
                            if (!mNeedTestPage)
                            {
                                return;
                            }

                            string testUrl = "file:///sdcard/outputHtml/html/" + mCurrentUrl + ".html";
                            if (webview != null)
                            {
                                webview.LoadUrl(testUrl);
                            }

                            mCurrentUrl++;
                        }
                        break;
                    case MSG_INIT_UI:
                        Init();
                        break;
                }

                base.HandleMessage(msg);
            }

        }

        private static void Init()
        {
            mWebView = new X5WebView(currentActivity, null);

            mViewParent.AddView(mWebView, new FrameLayout.LayoutParams(FrameLayout.LayoutParams.FillParent, FrameLayout.LayoutParams.FillParent));

            InitProgressBar();

            mWebView.SetWebViewClient(new CusWebViewClient());
            mWebView.SetWebChromeClient(new CusWebChromeClient(currentActivity));
            mWebView.SetDownloadListener(new CusDownloadListener(currentActivity));

            WebSettings webSetting = mWebView.Settings;
            webSetting.AllowFileAccess = true;
            webSetting.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);
            webSetting.SetSupportZoom(true);
            webSetting.BuiltInZoomControls = true;
            webSetting.UseWideViewPort = true;
            webSetting.SetSupportMultipleWindows(false);
            // webSetting.setLoadWithOverviewMode(true);
            webSetting.SetAppCacheEnabled(true);
            // webSetting.setDatabaseEnabled(true);
            webSetting.DomStorageEnabled = true;
            webSetting.JavaScriptEnabled = true;
            webSetting.SetGeolocationEnabled(true);
            webSetting.SetAppCacheMaxSize(Long.MaxValue);
            webSetting.SetAppCachePath(currentActivity.GetDir("appcache", FileCreationMode.Private).Path);
            webSetting.DatabasePath = currentActivity.GetDir("databases", FileCreationMode.Private).Path;
            webSetting.SetGeolocationDatabasePath(currentActivity.GetDir("geolocation", FileCreationMode.Private).Path);
            // webSetting.setPageCacheCapacity(IX5WebSettings.DEFAULT_CACHE_CAPACITY);
            webSetting.SetPluginState(WebSettings.PluginState.OnDemand);
            // webSetting.setRenderPriority(WebSettings.RenderPriority.HIGH);
            // webSetting.setPreFectch(true);
            //long time = System.currentTimeMillis();
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            if (mIntentUrl == null)
            {
                mWebView.LoadUrl(mHomeUrl);
            }
            else
            {
                mWebView.LoadUrl(mIntentUrl.ToString());
            }
            watch.Stop();
            TbsLog.D("time-cost", "cost time: " + watch.ElapsedMilliseconds);
            CookieSyncManager.CreateInstance(currentActivity);
            CookieSyncManager.Instance.Sync();

        }

        private static void InitProgressBar()
        {
            mPageLoadingProgressBar = currentActivity.FindViewById<ProgressBar>(Resource.Id.progressBar1);
            // new
            // ProgressBar(getApplicationContext(),
            // null,
            // android.R.attr.progressBarStyleHorizontal);
            mPageLoadingProgressBar.Max = 100;
            mPageLoadingProgressBar.ProgressDrawable = currentActivity.Resources.GetDrawable(Resource.Drawable.color_progressbar);
        }

        internal class CusOnClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private Action<View> _action;
            internal CusOnClickListener(Action<View> action)
            {
                _action = action;
            }

            public void OnClick(View v)
            {
                _action(v);
            }
        }

        internal class CusTextChangedListener : Java.Lang.Object, Android.Text.ITextWatcher
        {
            private readonly EditText mUrl;
            private readonly Button mGo;

            internal CusTextChangedListener(EditText mUrl, Button mGo)
            {
                this.mUrl = mUrl;
                this.mGo = mGo;
            }


            public void AfterTextChanged(IEditable s)
            {
                string url = null;
                if (!string.IsNullOrWhiteSpace(mUrl.Text))
                {
                    url = mUrl.Text;
                }

                if (string.IsNullOrWhiteSpace(mUrl.Text))
                {
                    mGo.Text = "请输入网址";
                    mGo.SetTextColor(new Android.Graphics.Color(0X6F0F0F0F));
                }
                else
                {
                    mGo.Text = "进入";
                    mGo.SetTextColor(new Android.Graphics.Color(0X6F0000CD));
                }
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
            }
        }

        internal class CusOnFocusChangeListener : Java.Lang.Object, View.IOnFocusChangeListener
        {
            private readonly EditText mUrl;
            private readonly Button mGo;
            private readonly Android.Content.Context ctx;

            internal CusOnFocusChangeListener(Context ctx, EditText mUrl, Button mGo)
            {
                this.ctx = ctx;
                this.mUrl = mUrl;
                this.mGo = mGo;
            }

            public void OnFocusChange(View v, bool hasFocus)
            {
                if (hasFocus)
                {
                    mGo.Visibility = ViewStates.Visible;
                    if (null == mWebView.Url)
                        return;

                    if (mWebView.Url.Equals(mHomeUrl, StringComparison.CurrentCultureIgnoreCase))
                    {
                        mUrl.Text = "";
                        mGo.Text = "首页";
                        mGo.SetTextColor(new Android.Graphics.Color(0X6F0F0F0F));
                    }
                    else
                    {
                        mUrl.Text = mWebView.Url;
                        mGo.Text = "进入";
                        mGo.SetTextColor(new Android.Graphics.Color(0X6F0000CD));
                    }
                }
                else
                {
                    mGo.Visibility = ViewStates.Gone;
                    string title = mWebView.Title;
                    if (title != null && title.Length > MAX_LENGTH)
                        mUrl.Text = title.Substring(0, MAX_LENGTH) + "...";
                    else
                        mUrl.Text = title;


                    var imm = ctx.GetSystemService(Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
                    imm.HideSoftInputFromWindow(v.WindowToken, 0);
                }
            }
        }

        public class CusDownloadListener : Java.Lang.Object, Com.Tencent.Smtt.Sdk.IDownloadListener
        {
            private readonly Activity currentActivity;

            public CusDownloadListener(Activity activity)
            {
                this.currentActivity = activity;
            }

            public void OnDownloadStart(string p0, string p1, string p2, string p3, long p4)
            {
                utils.LoggerManager.CurrentLogger.Debug($"OnDownloadStart url:{p0}");

                new AlertDialog.Builder(currentActivity)
                    .SetTitle("allow to download?")
                    .SetPositiveButton("yes", new CusDialogInterfaceOnClickListener((dialog, which) =>
                    {
                        Toast.MakeText(currentActivity, "fake message: i'll download...", ToastLength.Short).Show();
                    }))
                    .SetNegativeButton("no", new CusDialogInterfaceOnClickListener((dialog, which) =>
                    {
                        Toast.MakeText(currentActivity, "fake message: refuse download...", ToastLength.Short).Show();
                    }))
                    .SetOnCancelListener(new CusDialogInterfaceOnCancelListener(dialog =>
                    {
                        Toast.MakeText(currentActivity, "fake message: refuse download...", ToastLength.Short).Show();
                    }))
                    .Show();

            }


            internal class CusDialogInterfaceOnClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
            {
                private Action<IDialogInterface, int> _onClickAction;

                internal CusDialogInterfaceOnClickListener(Action<IDialogInterface, int> onclick)
                {
                    _onClickAction = onclick;
                }

                public void OnClick(IDialogInterface dialog, int which)
                {
                    _onClickAction(dialog, which);
                }
            }

            internal class CusDialogInterfaceOnCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
            {
                private Action<IDialogInterface> _onCancelAction;

                internal CusDialogInterfaceOnCancelListener(Action<IDialogInterface> onCancel)
                {
                    _onCancelAction = onCancel;
                }

                public void OnCancel(IDialogInterface dialog)
                {
                    _onCancelAction(dialog);
                }
            }
        }

        internal class CusWebChromeClient : WebChromeClient
        {
            private Activity currentActivity;
            private View myVideoView;
            private View myNormalView;
            private IX5WebChromeClientCustomViewCallback callback;


            private readonly Activity ctx;

            internal CusWebChromeClient(Activity activity)
            {
                this.currentActivity = activity;
            }


            public override bool OnJsConfirm(WebView p0, string p1, string p2, IJsResult p3)
            {
                return base.OnJsConfirm(p0, p1, p2, p3);
            }


            // 全屏播放配置
            public override void OnShowCustomView(View view, IX5WebChromeClientCustomViewCallback customViewCallback)
            {
                FrameLayout normalView = currentActivity.FindViewById<FrameLayout>(Resource.Id.web_filechooser);
                ViewGroup viewGroup = (ViewGroup)normalView.Parent;
                viewGroup.RemoveView(normalView);
                viewGroup.AddView(view);
                myVideoView = view;
                myNormalView = normalView;
                callback = customViewCallback;
            }

            public override void OnHideCustomView()
            {
                if (callback != null)
                {
                    callback.OnCustomViewHidden();
                    callback = null;
                }
                if (myVideoView != null)
                {
                    ViewGroup viewGroup = (ViewGroup)myVideoView.Parent;
                    viewGroup.RemoveView(myVideoView);
                    viewGroup.AddView(myNormalView);
                }
            }

            public override bool OnJsAlert(WebView p0, string p1, string p2, IJsResult p3)
            {
                // 这里写入你自定义的window alert

                return base.OnJsAlert(p0, p1, p2, p3);
            }
        }

        internal class CusWebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView p0, string p1)
            {
                return false;
            }

            public override void OnPageFinished(WebView view, string p1)
            {
                base.OnPageFinished(view, p1);

                // mTestHandler.sendEmptyMessage(MSG_OPEN_TEST_URL);
                mTestHandler.SendEmptyMessageDelayed(MSG_OPEN_TEST_URL, 5000);// 5s?
                if (int.Parse(Android.OS.Build.VERSION.Sdk) >= 16)
                    ChangGoForwardButton(view);
                /* mWebView.showLog("test Log"); */
            }
        }
    }
}