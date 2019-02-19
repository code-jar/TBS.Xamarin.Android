using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Tencent.Smtt.Sdk;
using tbs_app.utils;

namespace tbs_app
{
    [Activity(Label = "FilechooserActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class FilechooserActivity : Activity
    {

        private X5WebView webView;
        private IValueCallback uploadFile;
        private IValueCallback uploadFiles;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.filechooser_layout);


            webView = FindViewById<X5WebView>(Resource.Id.web_filechooser);
            webView.SetWebChromeClient(new CusWebChromeClient
            {
                OnOpenFileChooser = (arg0, arg1, arg2) =>
                {
                    Android.Util.Log.Info("test", "openFileChooser 1");
                    utils.LoggerManager.CurrentLogger.Debug($"OnOpenFileChooser arg1:{arg1},arg2:{arg2}");
                    // TODO 不知道啥玩意
                    uploadFile = arg0;
                    OpenFileChooseProcess();
                }
            });

            webView.LoadUrl("file:///android_asset/webpage/fileChooser.html");


        }

        private void OpenFileChooseProcess()
        {
            Intent i = new Intent(Intent.ActionGetContent);
            i.AddCategory(Intent.CategoryOpenable);
            i.SetType("*/*");
            StartActivityForResult(Intent.CreateChooser(i, "选择文件"), 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);


            if (resultCode == Result.Ok)
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
                        if (null != uploadFiles)
                        {
                            Android.Net.Uri result = data == null || resultCode != Result.Ok ? null : data.Data;
                            uploadFiles.OnReceiveValue(new Android.Net.Uri[] { result });
                            uploadFiles = null;
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

        protected override void OnDestroy()
        {
            if (this.webView != null)
            {
                webView.Destroy();
                webView = null;
            }

            base.OnDestroy();
        }



        internal class CusWebChromeClient : WebChromeClient
        {
            internal Action<IValueCallback, string, string> OnOpenFileChooser;

            public override void OpenFileChooser(IValueCallback p0, string p1, string p2)
            {
                OnOpenFileChooser?.Invoke(p0, p1, p2);
            }

        }

    }
}