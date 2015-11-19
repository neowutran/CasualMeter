using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CasualMeter.Common.Helpers;
using log4net;
using log4net.Core;
using Lunyx.Common.UI.Wpf;

namespace CasualMeter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ISingleInstanceApp
    {
        private const string Unique = "283055BC-D5AC-46CC-B1A9-51C053BB9028";

        private static readonly ILog Logger = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                Logger.Info("Starting up.");

                // Initialize process helper
                ProcessHelper.Instance.Initialize();

                var application = new App();
                application.InitializeComponent();

                // register unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                application.Run();

                Logger.Info("Cleaning up.");
                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        private static void HandleException(Exception e)
        {
            if (e == null) return;
            if (e.InnerException != null)
            {
                HandleException(e.InnerException);
            }
            else
            {
                Logger.Error(e);
            }
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            HandleException(e.Exception);
        }

        private static void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleException(e.Exception);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            // ...
            return true;
        }
    }
}
