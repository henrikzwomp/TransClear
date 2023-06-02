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
            CLSID = "{0C48FFE4-B4A1-44A8-972E-7F2A405EE191}"; // Unique GUIDs for class
            ClassName = "TransClear2.LxfThumbnailHandler";
            FileExtension = ".lxf";
        }
    }

    public class IoShellExtension : ShellExtensionData
    {
        public IoShellExtension()
        {
            DisplayName = "IoThumbnailHandler";
            CLSID = "{2976097A-5C30-4DA1-A92B-69A0CCD74528}"; // Unique GUIDs for class
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
