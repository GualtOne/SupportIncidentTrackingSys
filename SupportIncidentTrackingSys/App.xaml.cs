using System.Configuration;
using System.Data;
using System.Windows;
using SupportIncidentTrackingSys.DBservice;

namespace SupportIncidentTrackingSys
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(args.Exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                MessageBox.Show((args.ExceptionObject as Exception)?.Message, "Критическая ошибка");
            };
            DBService.Connection();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Environment.Exit(0);
            base.OnExit(e);
        }
    }

}
