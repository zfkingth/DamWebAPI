using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using DamWebAPI.ViewModel;

namespace DXMain
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            // This code is used to test the app when using other cultures.
            //
            //System.Threading.Thread.CurrentThread.CurrentCulture =
            //    System.Threading.Thread.CurrentThread.CurrentUICulture =
            //        new System.Globalization.CultureInfo("it-IT");


            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            //
            // Set desired culture, for example here the French (France) locale. 
            System.Threading.Thread.CurrentThread.CurrentUICulture = new
            System.Globalization.CultureInfo("zh-Hans");
            //initial simple ioc
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //load app.config
            Uri serviceUri = new Uri(hammergo.GlobalConfig.PubConstant.ConfigData.ServiceURI);
            SimpleIoc.Default.Register<Uri>(() => serviceUri);
        }

        public App()
        {
            this.Exit += App_Exit;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "程序出错", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
          
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow window = new MainWindow();

            // Create the ViewModel to which 
            // the main window binds.

            var viewModel = MainWindowViewModel.Instance;

            // When the ViewModel asks to be closed, 
            // close the window.
            EventHandler handler = null;
            handler = delegate
            {
                viewModel.RequestClose -= handler;
                window.Close();
            };
            viewModel.RequestClose += handler;

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;

            window.Show();
        }
    }
}
