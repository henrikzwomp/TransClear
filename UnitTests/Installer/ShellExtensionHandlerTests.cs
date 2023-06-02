using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Installer;
using Microsoft.Win32;
using Moq;

namespace UnitTests.Installer
{
    [TestFixture]
    public class ShellExtensionHandlerTests
    {
        public class TestShellExtension : ShellExtensionData
        {
            public TestShellExtension()
            {
                DisplayName = "LxfThumbnailHandler";
                CLSID = "{0f4e448d-a22d-35a2-804a-9e0b149b3cdd}";
                ClassName = "LxfThumbnailHandler.LxfThumbnailHandler";
                AssemblyVersion = "1.0.0.0";
                AssemblyFullName = "LxfTestHandler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                RuntimeVersion = "v4.0.30319";
                CodeBaseValue = @"file:///X:/DevProjects/LxfTestHandler/bin/Debug/LxfTestHandler.dll";
                FileExtension = ".lxf";
            }
        }

        [Test]
        public void CanInstallExtension()
        {
            var data = new TestShellExtension();

            var version_key = new Mock<IRegKeyItem>();

            var inproc32_key = new Mock<IRegKeyItem>();
            inproc32_key.Setup(x => x.CreateOrOpenSubKey(data.AssemblyVersion)).Returns(version_key.Object);

            var server_key = new Mock<IRegKeyItem>();
            server_key.Setup(x => x.CreateOrOpenSubKey("InprocServer32")).Returns(inproc32_key.Object);

            var classes_key = new Mock<IRegKeyItem>();
            classes_key.Setup(x => x.CreateOrOpenSubKey(data.CLSID)).Returns(server_key.Object);

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenClassesKey()).Returns(classes_key.Object);

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.InstallExtension(data);

            classes_key.Verify(x => x.CreateOrOpenSubKey(data.CLSID));
            server_key.Verify(x => x.CreateOrOpenSubKey("InprocServer32"));
            inproc32_key.Verify(x => x.CreateOrOpenSubKey(data.AssemblyVersion));

            server_key.Verify(x => x.SetValue(null, data.DisplayName));
            inproc32_key.Verify(x => x.SetValue(null, "mscoree.dll"));

            inproc32_key.Verify(x => x.SetValue("Assembly", data.AssemblyFullName));
            inproc32_key.Verify(x => x.SetValue("Class", data.ClassName));
            inproc32_key.Verify(x => x.SetValue("RuntimeVersion", data.RuntimeVersion));
            inproc32_key.Verify(x => x.SetValue("ThreadingModel", "Both"));
            inproc32_key.Verify(x => x.SetValue("CodeBase", data.CodeBaseValue));

            version_key.Verify(x => x.SetValue("Assembly", data.AssemblyFullName));
            version_key.Verify(x => x.SetValue("Class", data.ClassName));
            version_key.Verify(x => x.SetValue("RuntimeVersion", data.RuntimeVersion));
            version_key.Verify(x => x.SetValue("CodeBase", data.CodeBaseValue));
        }

        [Test]
        public void CanRegisterThumbnailHandlerForLxfFiles()
        {
            var server_key = new Mock<IRegKeyItem>();

            var classes_key = new Mock<IRegKeyItem>();
            classes_key.Setup(x => x.CreateOrOpenSubKey(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf")))
                .Returns(server_key.Object);

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenClassesRoot()).Returns(classes_key.Object);

            var data = new TestShellExtension();

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.RegisterThumbnailHandler(data);

            server_key.Verify(x => x.SetValue(null, data.CLSID));
        }

        [Test]
        public void CanApproveExtension()
        {
            var approved_key = new Mock<IRegKeyItem>();

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenApprovedShellExtensionsKey()).Returns(approved_key.Object);

            var data = new TestShellExtension(); 

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.ApproveExtension(data);

            approved_key.Verify(x => x.SetValue(data.CLSID, data.DisplayName));
        }

        [Test]
        public void CanUnapproveExtension()
        {
            var approved_key = new Mock<IRegKeyItem>();

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenApprovedShellExtensionsKey()).Returns(approved_key.Object);

            var data = new TestShellExtension(); 

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.UnapproveExtension(data);

            approved_key.Verify(x => x.DeleteValue(data.CLSID));
        }

        [Test]
        public void CanUnregisterThumbnailHandlerForLxfFiles()
        {
            var association_key = new Mock<IRegKeyItem>();

            var classes_key = new Mock<IRegKeyItem>();
            classes_key.Setup(x => x.OpenSubKey(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf")))
                .Returns(association_key.Object);

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenClassesRoot()).Returns(classes_key.Object);

            var data = new TestShellExtension();

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.UnregisterServerAssociations(data);

            classes_key.Verify(x => x.DeleteSubKeyTree(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf")));
        }

        [Test]
        public void CanUninstallExtension()
        {
            var data = new TestShellExtension();

            IList<string> sub_keys = new List<string>() { "Something", "Wrong", data.CLSID, "Blipp" };

            var classes_key = new Mock<IRegKeyItem>();
            classes_key.Setup(x => x.GetSubKeyNames())
                .Returns(sub_keys);

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenClassesKey()).Returns(classes_key.Object);


            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.UninstallExtension(data);

            classes_key.Verify(x => x.DeleteSubKeyTree(data.CLSID));
        }
        
        [Test]
        public void WillNotBreakIfUninstallingMissingInstallation()
        {
            var data = new TestShellExtension();

            IList<string> sub_keys = new List<string>() { "Something", "Wrong", "Blipp" };

            var classes_key = new Mock<IRegKeyItem>();
            classes_key.Setup(x => x.GetSubKeyNames())
                .Returns(sub_keys);

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenClassesKey()).Returns(classes_key.Object);

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.UninstallExtension(data);

            classes_key.Verify(x => x.DeleteSubKeyTree(data.CLSID), Times.Never());
        }

        [Test]
        public void WillNotBreakIfUnregisteringMissingInstallation()
        {
            var classes_key = new Mock<IRegKeyItem>();
            //classes_key.Setup(x => x.OpenSubKey(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf"))).Returns(null);

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenClassesRoot()).Returns(classes_key.Object);

            var data = new TestShellExtension();

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.UnregisterServerAssociations(data);

            classes_key.Verify(x => x.DeleteSubKeyTree(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf"))
                , Times.Never());
        }

        [Test]
        public void WillNotBreakIfUnapprovingMissingInstallation()
        {
            var approved_key = new Mock<IRegKeyItem>();

            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenApprovedShellExtensionsKey()).Returns(approved_key.Object);

            var data = new TestShellExtension();

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            server_handler.UnapproveExtension(data);

            approved_key.Verify(x => x.DeleteValue(data.CLSID));
        }
        
        [Test]
        public void CanDetermineIfInstalled()
        {
            var reg_access = new Mock<IRegistryAccess>();
            var data = new TestShellExtension();
            var server_handler = new ShellExtensionHandler(reg_access.Object);

            var classes_key = new Mock<IRegKeyItem>();
            reg_access.Setup(x => x.OpenClassesKey()).Returns(classes_key.Object);

            Assert.That(server_handler.IsInstalled(data), Is.EqualTo(false));

            var version_key = new Mock<IRegKeyItem>();

            var inproc32_key = new Mock<IRegKeyItem>();
            inproc32_key.Setup(x => x.OpenSubKey(data.AssemblyVersion)).Returns(version_key.Object);

            var server_key = new Mock<IRegKeyItem>();
            server_key.Setup(x => x.OpenSubKey("InprocServer32")).Returns(inproc32_key.Object);

            classes_key.Setup(x => x.OpenSubKey(data.CLSID)).Returns(server_key.Object);

            server_key.Setup(x => x.GetValue(null)).Returns(data.DisplayName);
            inproc32_key.Setup(x => x.GetValue(null)).Returns("mscoree.dll");

            inproc32_key.Setup(x => x.GetValue("Assembly")).Returns(data.AssemblyFullName);
            inproc32_key.Setup(x => x.GetValue("Class")).Returns(data.ClassName);
            inproc32_key.Setup(x => x.GetValue("RuntimeVersion")).Returns(data.RuntimeVersion);
            inproc32_key.Setup(x => x.GetValue("ThreadingModel")).Returns("Both");
            inproc32_key.Setup(x => x.GetValue("CodeBase")).Returns(data.CodeBaseValue);

            version_key.Setup(x => x.GetValue("Assembly")).Returns(data.AssemblyFullName);
            version_key.Setup(x => x.GetValue("Class")).Returns(data.ClassName);
            version_key.Setup(x => x.GetValue("RuntimeVersion")).Returns(data.RuntimeVersion);
            version_key.Setup(x => x.GetValue("CodeBase")).Returns(data.CodeBaseValue);

            Assert.That(server_handler.IsInstalled(data), Is.EqualTo(true));
        }

        [Test]
        public void CanDetermineIfRegistered()
        {
            var reg_access = new Mock<IRegistryAccess>();
            var data = new TestShellExtension();

            var classes_key = new Mock<IRegKeyItem>();

            reg_access.Setup(x => x.OpenClassesRoot()).Returns(classes_key.Object);

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            Assert.That(server_handler.IsRegistered(data), Is.EqualTo(false));

            var server_key = new Mock<IRegKeyItem>();

            classes_key.Setup(x => x.OpenSubKey(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf")))
                .Returns(server_key.Object);

            server_key.Setup(x => x.GetValue(null)).Returns(data.CLSID);

            Assert.That(server_handler.IsRegistered(data), Is.EqualTo(true));
        }

        [Test]
        public void CanDetermineIfApproved()
        {
            var data = new TestShellExtension();
            var approved_key = new Mock<IRegKeyItem>();
            var reg_access = new Mock<IRegistryAccess>();
            reg_access.Setup(x => x.OpenApprovedShellExtensionsKey()).Returns(approved_key.Object);

            var server_handler = new ShellExtensionHandler(reg_access.Object);

            Assert.That(server_handler.IsApproved(data), Is.EqualTo(false));

            approved_key.Setup(x => x.GetValue(data.CLSID)).Returns(data.DisplayName);

            Assert.That(server_handler.IsApproved(data), Is.EqualTo(true));
        }
    }

}
