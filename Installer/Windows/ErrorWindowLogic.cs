using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Installer.MVVM;

namespace Installer.Windows
{
    public class ErrorWindowLogic : ViewModelBase
    {
        private Exception _exception;
        private App _app;

        public ErrorWindowLogic(App app, Exception exception)
        {
            _exception = exception;
            _app = app;
        }

        public string ExceptionMessage
        {
            get
            {
                return _exception.Message;
            }
        }

        public string ExceptionStackTrace
        {
            get
            {
                return _exception.StackTrace;
            }
        }

        public ICommand CloseCommand
        {
            get { return new DelegateCommand(CloseWindow); }
        }

        private void CloseWindow()
        {
            _app.Shutdown();
        }
    }
}
