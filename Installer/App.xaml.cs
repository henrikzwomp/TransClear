using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Installer.Windows;

namespace Installer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender,
                       System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var error_window = new ErrorWindow();
            error_window.DataContext = new ErrorWindowLogic(this, e.Exception);
            error_window.Show();
            e.Handled = true;
        }
    }

    
    
}
