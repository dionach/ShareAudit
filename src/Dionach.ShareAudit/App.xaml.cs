using Dionach.ShareAudit.Modules.Services;
using Dionach.ShareAudit.Modules.UserInterface;
using Dionach.ShareAudit.Views;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dionach.ShareAudit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ServicesModule>();
            moduleCatalog.AddModule<UserInterfaceModule>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetupExceptionHandling();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        private static void ShowUnhandledException(Exception e, string source)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"An application error occurred in {source}");
            sb.AppendLine();
            sb.AppendLine($"Error: {e.GetType().Name} - {e.Message}");
            sb.AppendLine($"Stack Trace:");
            sb.AppendLine(e.StackTrace);
            while (e.InnerException != null)
            {
                e = e.InnerException;

                sb.AppendLine();
                sb.AppendLine($"Error: {e.GetType().Name} - {e.Message}");
                sb.AppendLine($"Stack Trace:");
                sb.AppendLine(e.StackTrace);
            }

            new ErrorWindow(sb.ToString()).ShowDialog();

            Application.Current.Shutdown();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetupExceptionHandling()
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                ShowUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            };

            DispatcherUnhandledException += (s, e) =>
            {
                e.Handled = true;
                ShowUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                ShowUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
            };

#endif
        }
    }
}
