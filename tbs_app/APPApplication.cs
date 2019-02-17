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

namespace tbs_app
{
    [Application]
    public class APPApplication : Application
    {
        public APPApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }


        public override void OnCreate()
        {
            base.OnCreate();

            utils.LoggerManager.Configure();


            utils.LoggerManager.CurrentLogger.Debug("Application OnCreate");

            AndroidEnvironment.UnhandledExceptionRaiser += AppUnhandledExceptionRaiser;
            CrashExceptionHandler.Instance.Init(this);

            QbSdk.IPreInitCallback cb = new CusPreInitCallback();
            //x5内核初始化接口
            QbSdk.InitX5Environment(ApplicationContext, cb);

        }


        private void AppUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {

            System.Threading.Tasks.Task.Run(() =>
            {

                Looper.Prepare();

                Toast.MakeText(this, "AppUnhandledException:" + e.Exception.Message, ToastLength.Long).Show();

                Looper.Loop();

            });

            System.Threading.Thread.Sleep(2000);

            e.Handled = true;
        }

        protected override void Dispose(bool disposing)
        {
            AndroidEnvironment.UnhandledExceptionRaiser -= AppUnhandledExceptionRaiser;

            base.Dispose(disposing);
        }
    }

    internal class CusPreInitCallback : Java.Lang.Object, QbSdk.IPreInitCallback
    {
        public void OnCoreInitFinished()
        {
            // TODO Auto-generated method stub
        }

        public void OnViewInitFinished(bool arg0)
        {
            // TODO Auto-generated method stub
            //x5內核初始化完成的回调，为true表示x5内核加载成功，否则表示x5内核加载失败，会自动切换到系统内核。
            Android.Util.Log.Debug("app", " onViewInitFinished is " + arg0);
        }
    }

    public class CrashExceptionHandler : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
    {
        //系统默认的UncaughtException处理类 
        private Java.Lang.Thread.IUncaughtExceptionHandler mDefaultHandler;
        //CrashHandler实例
        public static CrashExceptionHandler Instance = new CrashExceptionHandler();
        //程序的Context对象
        private Context mContext;


        private CrashExceptionHandler()
        {
        }

        public void UncaughtException(Java.Lang.Thread t, Java.Lang.Throwable e)
        {
            if (!HandleException(e) && mDefaultHandler != null)
            {
                mDefaultHandler.UncaughtException(t, e);
            }
            else
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                Java.Lang.JavaSystem.Exit(1);
            }
        }

        private bool HandleException(Java.Lang.Throwable e)
        {
            if (e == null)
            {
                return false;
            }

            System.Threading.Tasks.Task.Run(() =>
            {

                Looper.Prepare();

                Toast.MakeText(mContext, "ThreadUncaughtException:" + e.Message, ToastLength.Long).Show();

                Looper.Loop();

            });

            System.Threading.Thread.Sleep(2000);

            return true;
        }

        public void Init(Context ctx)
        {
            this.mContext = ctx;
            mDefaultHandler = Java.Lang.Thread.DefaultUncaughtExceptionHandler;

            Java.Lang.Thread.DefaultUncaughtExceptionHandler = this;
        }
    }

}