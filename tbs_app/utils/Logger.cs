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
using NLog;

namespace tbs_app.utils
{
    public class LoggerManager
    {

        public static void Configure()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var fileName = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/tbs/logs/log.txt";
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = fileName,
                FileNameKind = NLog.Targets.FilePathKind.Absolute,
                Layout = NLog.Layouts.Layout.FromString("${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=StackTrace}")
            };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

        }


        public static NLog.ILogger CurrentLogger => NLog.LogManager.GetCurrentClassLogger();

    }
}