using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Installer.MVVM;

namespace Installer.Windows
{
    public class MainWindowLogic : ViewModelBase
    {
        private ShellExtensionHandler _handler;
        private string _current_state = "Current state: Unknown";
        private ShellExtensionData _lxf_data;
        private ShellExtensionData _io_data;

        public MainWindowLogic()
        {
            _lxf_data = ShellExtensionLoader.LoadServer<IoShellExtension>(@"TransClear2.dll");
            _io_data = ShellExtensionLoader.LoadServer<LxfShellExtension>(@"TransClear2.dll");
            _handler = new ShellExtensionHandler(new RegistryAccess());
            CheckState();
        }

        public string CurrentStateMessage
        {
            get
            {
                return _current_state;
            }
            set
            {
                _current_state = value;
                PropertyHasChanged("CurrentStateMessage");
            }
        }

        public ICommand InstallCommand
        {
            get { return new DelegateCommand(InstallTransClear); }
        }

        public ICommand UninstallCommand
        {
            get { return new DelegateCommand(UninstallTransClear); }
        }

        private void InstallTransClear()
        {
            _handler.InstallExtension(_lxf_data);
            _handler.RegisterThumbnailHandler(_lxf_data);
            _handler.ApproveExtension(_lxf_data);
            Shell32dll.RefreshShell();
            MessageBox.Show("TransClear has been successfully installed. Click OK to exit.", "Success");
            Application.Current.Shutdown();
        }

        private void UninstallTransClear()
        {
            _handler.UnapproveExtension(_lxf_data);
            _handler.UnregisterServerAssociations(_lxf_data);
            _handler.UninstallExtension(_lxf_data);
            Shell32dll.RefreshShell();
            MessageBox.Show("TransClear has been successfully uninstalled. Click OK to exit.", "Success");
            Application.Current.Shutdown();
        }

        private void CheckState()
        {
            bool is_installed = _handler.IsInstalled(_lxf_data);
            bool is_registed = _handler.IsRegistered(_lxf_data);
            bool is_approved = _handler.IsApproved(_lxf_data);

            if (is_installed && is_registed && is_approved)
            {
                CurrentStateMessage = "Current state: TransClear is fully installed";
            }
            else if(is_installed || is_registed || is_approved)
            {
                CurrentStateMessage = "Current state: TransClear is not fully installed";
            }
            else
            {
                CurrentStateMessage = "Current state: TransClear is not installed";
            }
        }
    }
}
