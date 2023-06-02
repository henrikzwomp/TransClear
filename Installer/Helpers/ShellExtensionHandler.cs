using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer
{
    public partial class ShellExtensionHandler
    {
        const string CLSID_for_IThumbnailProvider_implementations = "e357fccd-a995-4576-b01f-234630154e96";

        private IRegistryAccess _reg_access;

        public ShellExtensionHandler(IRegistryAccess reg_access)
        {
            _reg_access = reg_access;
        }

        public void InstallExtension(ShellExtensionData data)
        {
            //  Open the classes.
            using (var classes_key = _reg_access.OpenClassesKey())
            {
                //  Create key for extension.
                using (var extension_key = classes_key.CreateOrOpenSubKey(data.CLSID))
                {
                    extension_key.SetValue(null, data.DisplayName);

                    //  Create the inproc key.
                    using (var inproc32_key = extension_key.CreateOrOpenSubKey("InprocServer32"))
                    {
                        inproc32_key.SetValue(null, "mscoree.dll"); // a reference to Mscoree.dll is used in place of a traditional COM type library to indicate that the common language runtime creates the managed object. 

                        //  Register all details at server level.
                        inproc32_key.SetValue("Assembly", data.AssemblyFullName);
                        inproc32_key.SetValue("Class", data.ClassName);
                        inproc32_key.SetValue("RuntimeVersion", data.RuntimeVersion);
                        inproc32_key.SetValue("ThreadingModel", "Both");
                        inproc32_key.SetValue("CodeBase", data.CodeBaseValue);

                        //  Create the version key.
                        using (var version_key = inproc32_key.CreateOrOpenSubKey(data.AssemblyVersion))
                        {
                            version_key.SetValue("Assembly", data.AssemblyFullName);
                            version_key.SetValue("Class", data.ClassName);
                            version_key.SetValue("RuntimeVersion", data.RuntimeVersion);
                            version_key.SetValue("CodeBase", data.CodeBaseValue);
                        }
                    }
                }
            }

        }

        public void RegisterThumbnailHandler(ShellExtensionData data)
        {
            using (var classes_key = _reg_access.OpenClassesRoot())
            {
                //  Create key for Thumbnail handler on .lxf file format.
                using (var server_key = classes_key.CreateOrOpenSubKey(string.Format(@"{0}\shellex\{{" + CLSID_for_IThumbnailProvider_implementations + "}}", data.FileExtension)))
                {
                    // Register our class
                    server_key.SetValue(null, data.CLSID);
                }
            }

        }

        public void ApproveExtension(ShellExtensionData data)
        {
            using (var approved_key = _reg_access.OpenApprovedShellExtensionsKey())
            {
                approved_key.SetValue(data.CLSID, data.DisplayName);
            }
        }

        public void UnapproveExtension(ShellExtensionData data)
        {
            using (var approved_key = _reg_access.OpenApprovedShellExtensionsKey())
            {
                approved_key.DeleteValue(data.CLSID);
            }
        }

        public void UnregisterServerAssociations(ShellExtensionData data)
        {
            using (var classes_key = _reg_access.OpenClassesRoot())
            {
                //  Get the key for the association.
                var association_key_path = string.Format(@"{0}\shellex\{{" + CLSID_for_IThumbnailProvider_implementations + "}}", data.FileExtension);

                using (var associationKey = classes_key.OpenSubKey(association_key_path))
                    if (associationKey == null)
                        return;

                classes_key.DeleteSubKeyTree(association_key_path);
            }

        }

        public void UninstallExtension(ShellExtensionData data)
        {
            using (var classes_key = _reg_access.OpenClassesKey())
            {
                if (classes_key.GetSubKeyNames().Any(skn => skn.Equals(data.CLSID, StringComparison.OrdinalIgnoreCase)))
                    classes_key.DeleteSubKeyTree(data.CLSID);
            }
        }

        public bool IsInstalled(ShellExtensionData data)
        {
            using (var classes_key = _reg_access.OpenClassesKey())
            {
                using (var extension_key = classes_key.OpenSubKey(data.CLSID))
                {
                    if (extension_key == null)
                        return false;

                    if(extension_key.GetValue(null) != data.DisplayName)
                        return false;

                    using (var inproc32_key = extension_key.OpenSubKey("InprocServer32"))
                    {
                        if (inproc32_key == null)
                            return false;

                        if (inproc32_key.GetValue(null) != "mscoree.dll")
                            return false;

                        if (inproc32_key.GetValue("Assembly") != data.AssemblyFullName)
                            return false;

                        if (inproc32_key.GetValue("Class") != data.ClassName)
                            return false;

                        if (inproc32_key.GetValue("RuntimeVersion") != data.RuntimeVersion)
                            return false;

                        if (inproc32_key.GetValue("ThreadingModel") != "Both")
                            return false;

                        if (inproc32_key.GetValue("CodeBase") != data.CodeBaseValue)
                            return false;
                        
                        using (var version_key = inproc32_key.OpenSubKey(data.AssemblyVersion))
                        {
                            if (version_key == null)
                                return false;

                            if (version_key.GetValue("Assembly") != data.AssemblyFullName)
                                return false;

                            if (version_key.GetValue("Class") != data.ClassName)
                                return false;

                            if (version_key.GetValue("RuntimeVersion") != data.RuntimeVersion)
                                return false;

                            if (version_key.GetValue("CodeBase") != data.CodeBaseValue)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool IsRegistered(ShellExtensionData data)
        {
            using (var classes_key = _reg_access.OpenClassesRoot())
            {
                using (var server_key = classes_key.OpenSubKey(string.Format(@"{0}\shellex\{{" + CLSID_for_IThumbnailProvider_implementations + "}}", data.FileExtension)))
                {
                    if (server_key == null)
                        return false;

                    if (server_key.GetValue(null) != data.CLSID)
                        return false;
                }
            }

            return true;
        }

        public bool IsApproved(ShellExtensionData data)
        {
            using (var approved_key = _reg_access.OpenApprovedShellExtensionsKey())
            {
                if (approved_key.GetValue(data.CLSID) != data.DisplayName)
                    return false;
            }

            return true;
        }

    }
}
