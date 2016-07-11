using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32;

namespace Installer
{
    public interface IRegistryAccess
    {
        IRegKeyItem OpenClassesRoot();
        IRegKeyItem OpenClassesKey();
        IRegKeyItem OpenApprovedShellExtensionsKey();
    }

    public class RegistryAccess : IRegistryAccess
    {
        // If you request a 64-bit view on a 32-bit operating system, the returned keys will be in the 32-bit view.

        public RegistryAccess() { }

        public IRegKeyItem OpenClassesRoot()
        {
            return new RegKeyItem(RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64));
        }

        public IRegKeyItem OpenClassesKey()
        {
            return  OpenClassesRoot().OpenSubKey("CLSID");
        }

        public IRegKeyItem OpenApprovedShellExtensionsKey()
        {
            return new RegKeyItem(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved"
                , RegistryKeyPermissionCheck.ReadWriteSubTree));
        }

    }
}
