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

namespace tbs_app.utils
{
    public interface IWebViewJavaScriptFunction
    {
        void OnJsFunctionCalled(string tag);
    }
}