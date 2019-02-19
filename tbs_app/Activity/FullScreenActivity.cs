using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using tbs_app.utils;

namespace tbs_app
{
    [Activity(Label = "FullScreenActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class FullScreenActivity : Activity
    {
        private X5WebView webView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            utils.LoggerManager.CurrentLogger.Debug("FullScreenActivity OnCreate");

            SetContentView(Resource.Layout.filechooser_layout);
            webView = FindViewById<X5WebView>(Resource.Id.web_filechooser);
            webView.LoadUrl("file:///android_asset/webpage/fullscreenVideo.html");

            Window.SetFormat(Android.Graphics.Format.Translucent);

            webView.View.OverScrollMode = OverScrollMode.Always;

            IWebViewJavaScriptFunction iview = new CusWebViewJavaScriptFunction(this, webView);
            webView.AddJavascriptInterface((Java.Lang.Object)iview, "Android");
        }

        protected override void OnDestroy()
        {
            if (webView != null)
            {
                webView.Destroy();
                webView = null;
            }

            base.OnDestroy();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {

            try
            {
                base.OnConfigurationChanged(newConfig);

                if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape)
                {

                }
                else if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
                {

                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }


        public class CusWebViewJavaScriptFunction : Java.Lang.Object, IWebViewJavaScriptFunction
        {
            private readonly Activity currentActivity;
            private readonly X5WebView webView;

            public CusWebViewJavaScriptFunction(Activity activity, X5WebView view)
            {
                currentActivity = activity;
                webView = view;
            }


            public void OnJsFunctionCalled(string tag)
            {
                // TODO Auto-generated method stub
            }


            [Android.Webkit.JavascriptInterface]
            public void OnX5ButtonClicked()
            {
                EnableX5FullscreenFunc();
            }

            [Android.Webkit.JavascriptInterface]
            public void OnCustomButtonClicked()
            {
                DisableX5FullscreenFunc();
            }

            [Android.Webkit.JavascriptInterface]
            public void OnLiteWndButtonClicked()
            {
                EnableLiteWndFunc();
            }

            [Android.Webkit.JavascriptInterface]
            public void OnPageVideoClicked()
            {
                EnablePageVideoFunc();
            }




            #region 向webview发出信息

            private void EnableX5FullscreenFunc()
            {

                if (webView.X5WebViewExtension != null)
                {
                    Toast.MakeText(currentActivity, "开启X5全屏播放模式", ToastLength.Long).Show();
                    Bundle data = new Bundle();

                    data.PutBoolean("standardFullScreen", false);// true表示标准全屏，false表示X5全屏；不设置默认false，

                    data.PutBoolean("supportLiteWnd", false);// false：关闭小窗；true：开启小窗；不设置默认true，

                    data.PutInt("DefaultVideoScreen", 2);// 1：以页面内开始播放，2：以全屏开始播放；不设置默认：1

                    webView.X5WebViewExtension.InvokeMiscMethod("setVideoParams", data);
                }
            }

            private void DisableX5FullscreenFunc()
            {
                if (webView.X5WebViewExtension != null)
                {
                    Toast.MakeText(currentActivity, "恢复webkit初始状态", ToastLength.Long).Show();
                    Bundle data = new Bundle();

                    data.PutBoolean("standardFullScreen", true);// true表示标准全屏，会调起onShowCustomView()，false表示X5全屏；不设置默认false，

                    data.PutBoolean("supportLiteWnd", false);// false：关闭小窗；true：开启小窗；不设置默认true，

                    data.PutInt("DefaultVideoScreen", 2);// 1：以页面内开始播放，2：以全屏开始播放；不设置默认：1

                    webView.X5WebViewExtension.InvokeMiscMethod("setVideoParams", data);
                }
            }

            private void EnableLiteWndFunc()
            {
                if (webView.X5WebViewExtension != null)
                {
                    Toast.MakeText(currentActivity, "开启小窗模式", ToastLength.Long).Show();
                    Bundle data = new Bundle();

                    data.PutBoolean("standardFullScreen", false);// true表示标准全屏，会调起onShowCustomView()，false表示X5全屏；不设置默认false，

                    data.PutBoolean("supportLiteWnd", true);// false：关闭小窗；true：开启小窗；不设置默认true，

                    data.PutInt("DefaultVideoScreen", 2);// 1：以页面内开始播放，2：以全屏开始播放；不设置默认：1

                    webView.X5WebViewExtension.InvokeMiscMethod("setVideoParams", data);
                }
            }

            private void EnablePageVideoFunc()
            {
                if (webView.X5WebViewExtension != null)
                {
                    Toast.MakeText(currentActivity, "页面内全屏播放模式", ToastLength.Long).Show();
                    Bundle data = new Bundle();

                    data.PutBoolean("standardFullScreen", false);// true表示标准全屏，会调起onShowCustomView()，false表示X5全屏；不设置默认false，

                    data.PutBoolean("supportLiteWnd", false);// false：关闭小窗；true：开启小窗；不设置默认true，

                    data.PutInt("DefaultVideoScreen", 1);// 1：以页面内开始播放，2：以全屏开始播放；不设置默认：1

                    webView.X5WebViewExtension.InvokeMiscMethod("setVideoParams", data);
                }
            }

            #endregion


        }
    }
}