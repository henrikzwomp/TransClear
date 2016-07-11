using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer
{
    public class MyShellExtension : ShellExtensionData
    {
        public MyShellExtension()
        {
            DisplayName = "LxfThumbnailHandler";
            CLSID = "{1e037f76-49a8-4d7c-b7c6-7b46d2d07ac3}";
            ClassName = "LxfHandler.LxfThumbnailHandler";
        }
    }

    /*public class SharpShellExtension : ShellExtensionData
    {
        public SharpShellExtension()
        {
            DisplayName = "LxfThumbnailHandler";
            CLSID = new Guid("0f4e448d-a22d-35a2-804a-9e0b149b3cdd").ToString("B");
            ClassName = "LxfThumbnailHandler.LxfThumbnailHandler";
        }
    }*/

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

    }
}
