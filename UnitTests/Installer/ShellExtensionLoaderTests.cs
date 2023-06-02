using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Installer;

namespace UnitTests.Installer
{
    [TestFixture]
    public class ShellExtensionLoaderTests
    {
        [Test]
        public void CanLoad()
        {
            var unloaded_server = new LxfShellExtension();

            Assert.That(unloaded_server.AssemblyVersion, Is.Null);
            Assert.That(unloaded_server.AssemblyFullName, Is.Null);
            Assert.That(unloaded_server.RuntimeVersion, Is.Null);
            Assert.That(unloaded_server.CodeBaseValue, Is.Null);

            var server = ShellExtensionLoader.LoadServer<LxfShellExtension>(@"LxfHandler.dll");

            Assert.That(server.AssemblyVersion, Is.Not.Null);
            Assert.That(server.AssemblyFullName, Is.Not.Null);
            Assert.That(server.RuntimeVersion, Is.Not.Null);
            Assert.That(server.CodeBaseValue, Is.Not.Null);
        }
    }
}
