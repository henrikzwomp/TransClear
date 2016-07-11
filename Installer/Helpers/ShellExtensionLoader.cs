using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.ComponentModel.Composition.Hosting;

namespace Installer
{
    public class ShellExtensionLoader
    {
        public static t LoadServer<t>(string path) where t : ShellExtensionData, new()
        {
            var catalog = new AssemblyCatalog(Path.GetFullPath(path));
            var data = new t();
            data.AssemblyVersion = catalog.Assembly.GetName().Version.ToString();
            data.AssemblyFullName = catalog.Assembly.FullName;
            data.RuntimeVersion = catalog.Assembly.ImageRuntimeVersion;
            data.CodeBaseValue = catalog.Assembly.CodeBase;
            return data;
        }
    }
}
