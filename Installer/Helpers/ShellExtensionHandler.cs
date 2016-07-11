using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer
{
    public partial class ShellExtensionHandler
    {
        private ShellExtensionData _data;
        private IRegistryAccess _reg_access;

        public ShellExtensionHandler(ShellExtensionData data, IRegistryAccess reg_access)
        {
            _data = data;
            _reg_access = reg_access;
        }

        public void InstallExtension()
        {
            //  Open the classes.
            using (var classes_key = _reg_access.OpenClassesKey())
            {
                //  Create key for extension.
                using (var extension_key = classes_key.CreateOrOpenSubKey(_data.CLSID))
                {
                    extension_key.SetValue(null, _data.DisplayName);

                    //  Create the inproc key.
                    using (var inproc32_key = extension_key.CreateOrOpenSubKey("InprocServer32"))
                    {
                        inproc32_key.SetValue(null, "mscoree.dll"); // a reference to Mscoree.dll is used in place of a traditional COM type library to indicate that the common language runtime creates the managed object. 

                        //  Register all details at server level.
                        inproc32_key.SetValue("Assembly", _data.AssemblyFullName);
                        inproc32_key.SetValue("Class", _data.ClassName);
                        inproc32_key.SetValue("RuntimeVersion", _data.RuntimeVersion);
                        inproc32_key.SetValue("ThreadingModel", "Both");
                        inproc32_key.SetValue("CodeBase", _data.CodeBaseValue);

                        //  Create the version key.
                        using (var version_key = inproc32_key.CreateOrOpenSubKey(_data.AssemblyVersion))
                        {
                            version_key.SetValue("Assembly", _data.AssemblyFullName);
                            version_key.SetValue("Class", _data.ClassName);
                            version_key.SetValue("RuntimeVersion", _data.RuntimeVersion);
                            version_key.SetValue("CodeBase", _data.CodeBaseValue);
                        }
                    }
                }
            }

        }

        public void RegisterThumbnailHandlerForLxfFiles()
        {
            using (var classes_key = _reg_access.OpenClassesRoot())
            {
                //  Create key for Thumbnail handler on .lxf file format.
                using (var server_key = classes_key.CreateOrOpenSubKey(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf")))
                {
                    // Register our class
                    server_key.SetValue(null, _data.CLSID);
                }
            }

        }

        public void ApproveExtension()
        {
            using (var approved_key = _reg_access.OpenApprovedShellExtensionsKey())
            {
                approved_key.SetValue(_data.CLSID, _data.DisplayName);
            }
        }

        public void UnapproveExtension()
        {
            using (var approved_key = _reg_access.OpenApprovedShellExtensionsKey())
            {
                approved_key.DeleteValue(_data.CLSID);
            }
        }

        public void UnregisterServerAssociations()
        {
            using (var classes_key = _reg_access.OpenClassesRoot())
            {
                //  Get the key for the association.
                var association_key_path = string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf");

                using (var associationKey = classes_key.OpenSubKey(association_key_path))
                    if (associationKey == null)
                        return;

                classes_key.DeleteSubKeyTree(association_key_path);
            }

        }

        public void UninstallExtension()
        {
            using (var classes_key = _reg_access.OpenClassesKey())
            {
                if (classes_key.GetSubKeyNames().Any(skn => skn.Equals(_data.CLSID, StringComparison.OrdinalIgnoreCase)))
                    classes_key.DeleteSubKeyTree(_data.CLSID);
            }
        }

        public bool IsInstalled()
        {
            using (var classes_key = _reg_access.OpenClassesKey())
            {
                using (var extension_key = classes_key.OpenSubKey(_data.CLSID))
                {
                    if (extension_key == null)
                        return false;

                    if(extension_key.GetValue(null) != _data.DisplayName)
                        return false;

                    using (var inproc32_key = extension_key.OpenSubKey("InprocServer32"))
                    {
                        if (inproc32_key == null)
                            return false;

                        if (inproc32_key.GetValue(null) != "mscoree.dll")
                            return false;

                        if (inproc32_key.GetValue("Assembly") != _data.AssemblyFullName)
                            return false;

                        if (inproc32_key.GetValue("Class") != _data.ClassName)
                            return false;

                        if (inproc32_key.GetValue("RuntimeVersion") != _data.RuntimeVersion)
                            return false;

                        if (inproc32_key.GetValue("ThreadingModel") != "Both")
                            return false;

                        if (inproc32_key.GetValue("CodeBase") != _data.CodeBaseValue)
                            return false;
                        
                        using (var version_key = inproc32_key.OpenSubKey(_data.AssemblyVersion))
                        {
                            if (version_key == null)
                                return false;

                            if (version_key.GetValue("Assembly") != _data.AssemblyFullName)
                                return false;

                            if (version_key.GetValue("Class") != _data.ClassName)
                                return false;

                            if (version_key.GetValue("RuntimeVersion") != _data.RuntimeVersion)
                                return false;

                            if (version_key.GetValue("CodeBase") != _data.CodeBaseValue)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool IsRegistered()
        {
            using (var classes_key = _reg_access.OpenClassesRoot())
            {
                using (var server_key = classes_key.OpenSubKey(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf")))
                {
                    if (server_key == null)
                        return false;

                    if (server_key.GetValue(null) != _data.CLSID)
                        return false;
                }
            }

            return true;
        }

        public bool IsApproved()
        {
            using (var approved_key = _reg_access.OpenApprovedShellExtensionsKey())
            {
                if (approved_key.GetValue(_data.CLSID) != _data.DisplayName)
                    return false;
            }

            return true;
        }

    }
}
