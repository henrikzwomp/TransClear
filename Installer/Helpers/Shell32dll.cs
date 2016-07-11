using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer
{
    public class Shell32dll
    {
        /// <summary>
        /// Tell the Shell that a file type association has changed.
        /// </summary>
        public static void RefreshShell()
        {
            SHChangeNotify(SHCNE_ASSOCCHANGED, 0, IntPtr.Zero, IntPtr.Zero);
        }

        private const int SHCNE_ASSOCCHANGED = 0x08000000;
        private const int SHCNF_IDLIST = 0x0000;

        /// <summary>
        /// Notifies the system of an event that an application has performed. An application should use this function if it performs an action that may affect the Shell.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/bb762118%28v=vs.85%29.aspx for more information.
        /// </summary>
        /// <param name="eventId">Describes the event that has occurred.</param>
        /// <param name="flags">Flags that, when combined bitwise with SHCNF_TYPE, indicate the meaning of the dwItem1 and dwItem2 parameters. </param>
        /// <param name="item1">Optional. First event-dependent value.</param>
        /// <param name="item2">Optional. Second event-dependent value.</param>
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern void SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);
    }
    
}
