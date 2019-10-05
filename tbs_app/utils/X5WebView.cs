using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Tencent.Smtt.Sdk;

namespace tbs_app.utils
{
    public class X5WebView : WebView
    {

        private TextView title;

        private WebViewClient client = new CusWebViewClient();


        public X5WebView(Context arg0) : base(arg0)
        {
            SetBackgroundColor(new Android.Graphics.Color(85621));
        }

        [Android.Annotation.SuppressLint(Value = new string[] { "SetJavaScriptEnabled" })]
        public X5WebView(Context arg0, Android.Util.IAttributeSet arg1) : base(arg0, arg1)
        {
            this.WebViewClient = client;

            // this.setWebChromeClient(chromeClient);
            // WebStorage webStorage = WebStorage.getInstance();
            InitWebViewSettings();
            this.View.Clickable = true;
        }

        protected override bool DrawChild(Canvas canvas, View child, long drawingTime)
        {
            bool ret = base.DrawChild(canvas, child, drawingTime);
            canvas.Save();
            Paint paint = new Paint
            {
                Color = new Color(0x7fff0000),
                TextSize = 24.0f,
                AntiAlias = true
            };
            if (X5WebViewExtension != null)
            {
                canvas.DrawText(this.Context.PackageName + "-pid:" + Android.OS.Process.MyPid(), 10, 50, paint);
                canvas.DrawText("X5  Core:" + QbSdk.GetTbsVersion(this.Context), 10, 100, paint);
            }
            else
            {
                canvas.DrawText(this.Context.PackageName + "-pid:" + Android.OS.Process.MyPid(), 10, 50, paint);
                canvas.DrawText("Sys Core", 10, 100, paint);
            }
            canvas.DrawText(Build.Manufacturer, 10, 150, paint);
            canvas.DrawText(Build.Model, 10, 200, paint);
            canvas.Restore();
            return ret;
        }

        private void InitWebViewSettings()
        {
            WebSettings webSetting = Settings;
            webSetting.JavaScriptEnabled = true;
            webSetting.JavaScriptCanOpenWindowsAutomatically = true;
            webSetting.AllowFileAccess = true;
            webSetting.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);
            webSetting.SetSupportZoom(true);
            webSetting.BuiltInZoomControls = true;
            webSetting.UseWideViewPort = true;
            webSetting.SetSupportMultipleWindows(true);
            // webSetting.setLoadWithOverviewMode(true);
            webSetting.SetAppCacheEnabled(true);
            // webSetting.setDatabaseEnabled(true);
            webSetting.DomStorageEnabled = true;
            webSetting.SetGeolocationEnabled(true);
            webSetting.SetAppCacheMaxSize(long.MaxValue);
            // webSetting.setPageCacheCapacity(IX5WebSettings.DEFAULT_CACHE_CAPACITY);
            webSetting.SetPluginState(WebSettings.PluginState.OnDemand);
            // webSetting.setRenderPriority(WebSettings.RenderPriority.HIGH);
            webSetting.CacheMode = WebSettings.LoadNoCache;

            // this.getSettingsExtension().setPageCacheCapacity(IX5WebSettings.DEFAULT_CACHE_CAPACITY);//extension
            // settings 的设计
        }
    }

    internal class CusWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            return true;
        }
    }
}