using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer
{
    public class LxfShellExtension : ShellExtensionData
    {
        public LxfShellExtension()
        {
            DisplayName = "LxfThumbnailHandler";
            CLSID = "{1e037f76-49a8-4d7c-b7c6-7b46d2d07ac3}";
            ClassName = "TransClear2.LxfThumbnailHandler";
            FileExtension = ".lxf";
        }
    }

    public class IoShellExtension : ShellExtensionData
    {
        public IoShellExtension()
        {
            DisplayName = "IoThumbnailHandler";
            CLSID = "{1e037f76-49a8-4d7c-b7c6-7b46d2d07ac3}";
            ClassName = "TransClear2.IoThumbnailHandler";
            FileExtension = ".io";
        }
    }

    public abstract class ShellExtensionData
    {
        public ShellExtensionData()
        {

        }

        public string AssemblyVersion { get; set; }

        public string AssemblyFullName { get; set; }

        public string ClassName { get; protected set; }

        public string CLSID { get; protected set; }

        public string CodeBaseValue { get; set; }

        public string DisplayName { get; protected set; }

        public string RuntimeVersion { get; set; }

        public string FileExtension { get; set; }

    }
}
