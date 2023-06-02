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
            _lxf_data = ShellExtensionLoader.LoadServer<LxfShellExtension>(@"TransClear2.dll");
            _io_data = ShellExtensionLoader.LoadServer<IoShellExtension>(@"TransClear2.dll");
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

            _handler.InstallExtension(_io_data);
            _handler.RegisterThumbnailHandler(_io_data);
            _handler.ApproveExtension(_io_data);

            Shell32dll.RefreshShell();
            MessageBox.Show("TransClear has been successfully installed. Click OK to exit.", "Success");
            Application.Current.Shutdown();
        }

        private void UninstallTransClear()
        {
            _handler.UnapproveExtension(_io_data);
            _handler.UnregisterServerAssociations(_io_data);
            _handler.UninstallExtension(_io_data);

            _handler.UnapproveExtension(_io_data);
            _handler.UnregisterServerAssociations(_io_data);
            _handler.UninstallExtension(_io_data);

            Shell32dll.RefreshShell();
            MessageBox.Show("TransClear has been successfully uninstalled. Click OK to exit.", "Success");
            Application.Current.Shutdown();
        }

        private void CheckState()
        {
            bool lxf_is_installed = _handler.IsInstalled(_lxf_data);
            bool lxf_is_registed = _handler.IsRegistered(_lxf_data);
            bool lxf_is_approved = _handler.IsApproved(_lxf_data);

            bool io_is_installed = _handler.IsInstalled(_io_data);
            bool io_is_registed = _handler.IsRegistered(_io_data);
            bool io_is_approved = _handler.IsApproved(_io_data);

            if (lxf_is_installed && lxf_is_registed && lxf_is_approved && io_is_installed && io_is_registed && io_is_approved)
            {
                CurrentStateMessage = "Current state: TransClear2 is fully installed";
            }
            else if(lxf_is_installed || lxf_is_registed || lxf_is_approved || io_is_installed || io_is_registed || io_is_approved)
            {
                CurrentStateMessage = "Current state: TransClear2 is not fully installed";
            }
            else
            {
                CurrentStateMessage = "Current state: TransClear2 is not installed";
            }
        }
    }
}
