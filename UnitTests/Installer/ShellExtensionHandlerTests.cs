using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Installer;
using Microsoft.Win32;
using Rhino.Mocks;

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
            }
        }

        [Test]
        public void CanInstallExtension()
        {
            var data = new TestShellExtension();

            var version_key = MockRepository.GenerateMock<IRegKeyItem>();

            var inproc32_key = MockRepository.GenerateMock<IRegKeyItem>();
            inproc32_key.Stub(x => x.CreateOrOpenSubKey(Arg<string>.Is.Equal(data.AssemblyVersion))).Return(version_key);

            var server_key = MockRepository.GenerateMock<IRegKeyItem>();
            server_key.Stub(x => x.CreateOrOpenSubKey(Arg<string>.Is.Equal("InprocServer32"))).Return(inproc32_key);

            var classes_key = MockRepository.GenerateMock<IRegKeyItem>();
            classes_key.Stub(x => x.CreateOrOpenSubKey(Arg<string>.Is.Equal(data.CLSID))).Return(server_key);

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenClassesKey()).Return(classes_key);

            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.InstallExtension();

            classes_key.AssertWasCalled(x => x.CreateOrOpenSubKey(Arg<string>.Is.Same(data.CLSID)));
            server_key.AssertWasCalled(x => x.CreateOrOpenSubKey(Arg<string>.Is.Same("InprocServer32")));
            inproc32_key.AssertWasCalled(x => x.CreateOrOpenSubKey(Arg<string>.Is.Same(data.AssemblyVersion)));

            server_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Null, Arg<string>.Is.Same(data.DisplayName)));
            inproc32_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Null, Arg<string>.Is.Same("mscoree.dll")));

            inproc32_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("Assembly"), Arg<string>.Is.Equal(data.AssemblyFullName)));
            inproc32_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("Class"), Arg<string>.Is.Same(data.ClassName)));
            inproc32_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("RuntimeVersion"), Arg<string>.Is.Same(data.RuntimeVersion)));
            inproc32_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("ThreadingModel"), Arg<string>.Is.Same("Both")));
            inproc32_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("CodeBase"), Arg<string>.Is.Same(data.CodeBaseValue)));

            version_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("Assembly"), Arg<string>.Is.Same(data.AssemblyFullName)));
            version_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("Class"), Arg<string>.Is.Same(data.ClassName)));
            version_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("RuntimeVersion"), Arg<string>.Is.Same(data.RuntimeVersion)));
            version_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Same("CodeBase"), Arg<string>.Is.Same(data.CodeBaseValue)));
        }

        [Test]
        public void CanRegisterThumbnailHandlerForLxfFiles()
        {
            var server_key = MockRepository.GenerateMock<IRegKeyItem>();

            var classes_key = MockRepository.GenerateMock<IRegKeyItem>();
            classes_key.Stub(x => x.CreateOrOpenSubKey(Arg<string>.Is.Equal(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf"))))
                .Return(server_key);

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenClassesRoot()).Return(classes_key);

            var data = new TestShellExtension();

            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.RegisterThumbnailHandlerForLxfFiles();

            server_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Null, Arg<string>.Is.Equal(data.CLSID)));
        }

        [Test]
        public void CanApproveExtension()
        {
            var approved_key = MockRepository.GenerateMock<IRegKeyItem>();

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenApprovedShellExtensionsKey()).Return(approved_key);

            var data = new TestShellExtension(); 

            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.ApproveExtension();

            approved_key.AssertWasCalled(x => x.SetValue(Arg<string>.Is.Equal(data.CLSID), Arg<string>.Is.Equal(data.DisplayName)));
        }

        [Test]
        public void CanUnapproveExtension()
        {
            var approved_key = MockRepository.GenerateMock<IRegKeyItem>();

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenApprovedShellExtensionsKey()).Return(approved_key);

            var data = new TestShellExtension(); 

            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.UnapproveExtension();

            approved_key.AssertWasCalled(x => x.DeleteValue(Arg<string>.Is.Equal(data.CLSID)));
        }

        [Test]
        public void CanUnregisterThumbnailHandlerForLxfFiles()
        {
            var association_key = MockRepository.GenerateMock<IRegKeyItem>();

            var classes_key = MockRepository.GenerateMock<IRegKeyItem>();
            classes_key.Stub(x => x.OpenSubKey(Arg<string>.Is.Equal(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf"))))
                .Return(association_key);

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenClassesRoot()).Return(classes_key);

            var data = new TestShellExtension();

            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.UnregisterServerAssociations();

            classes_key.AssertWasCalled(x => x.DeleteSubKeyTree(Arg<string>.Is.Equal(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf"))));
        }

        [Test]
        public void CanUninstallExtension()
        {
            var data = new TestShellExtension();

            IList<string> sub_keys = new List<string>() { "Something", "Wrong", data.CLSID, "Blipp" };

            var classes_key = MockRepository.GenerateMock<IRegKeyItem>();
            classes_key.Stub(x => x.GetSubKeyNames())
                .Return(sub_keys);

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenClassesKey()).Return(classes_key);


            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.UninstallExtension();

            classes_key.AssertWasCalled(x => x.DeleteSubKeyTree(data.CLSID));
        }
        
        [Test]
        public void WillNotBreakIfUninstallingMissingInstallation()
        {
            var data = new TestShellExtension();

            IList<string> sub_keys = new List<string>() { "Something", "Wrong", "Blipp" };

            var classes_key = MockRepository.GenerateMock<IRegKeyItem>();
            classes_key.Stub(x => x.GetSubKeyNames())
                .Return(sub_keys);

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenClassesKey()).Return(classes_key);

            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.UninstallExtension();

            classes_key.AssertWasNotCalled(x => x.DeleteSubKeyTree(data.CLSID));
        }

        [Test]
        public void WillNotBreakIfUnregisteringMissingInstallation()
        {
            var classes_key = MockRepository.GenerateMock<IRegKeyItem>();
            classes_key.Stub(x => x.OpenSubKey(Arg<string>.Is.Equal(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf"))))
                .Return(null);

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenClassesRoot()).Return(classes_key);

            var data = new TestShellExtension();

            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.UnregisterServerAssociations();

            classes_key.AssertWasNotCalled(x => x.DeleteSubKeyTree(Arg<string>.Is.Equal(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf"))));
        }

        [Test]
        public void WillNotBreakIfUnapprovingMissingInstallation()
        {
            var approved_key = MockRepository.GenerateMock<IRegKeyItem>();

            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenApprovedShellExtensionsKey()).Return(approved_key);

            var data = new TestShellExtension();

            var server_handler = new ShellExtensionHandler(data, reg_access);

            server_handler.UnapproveExtension();

            approved_key.AssertWasCalled(x => x.DeleteValue(Arg<string>.Is.Equal(data.CLSID)));
        }
        
        [Test]
        public void CanDetermineIfInstalled()
        {
            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            var data = new TestShellExtension();
            var server_handler = new ShellExtensionHandler(data, reg_access);

            var classes_key = MockRepository.GenerateMock<IRegKeyItem>();
            reg_access.Stub(x => x.OpenClassesKey()).Return(classes_key);

            Assert.That(server_handler.IsInstalled(), Is.EqualTo(false));

            var version_key = MockRepository.GenerateMock<IRegKeyItem>();

            var inproc32_key = MockRepository.GenerateMock<IRegKeyItem>();
            inproc32_key.Stub(x => x.OpenSubKey(Arg<string>.Is.Equal(data.AssemblyVersion))).Return(version_key);

            var server_key = MockRepository.GenerateMock<IRegKeyItem>();
            server_key.Stub(x => x.OpenSubKey(Arg<string>.Is.Equal("InprocServer32"))).Return(inproc32_key);

            classes_key.Stub(x => x.OpenSubKey(Arg<string>.Is.Equal(data.CLSID))).Return(server_key);

            server_key.Stub(x => x.GetValue(Arg<string>.Is.Null)).Return(data.DisplayName);
            inproc32_key.Stub(x => x.GetValue(Arg<string>.Is.Null)).Return("mscoree.dll");

            inproc32_key.Stub(x => x.GetValue(Arg<string>.Is.Same("Assembly"))).Return(data.AssemblyFullName);
            inproc32_key.Stub(x => x.GetValue(Arg<string>.Is.Same("Class"))).Return(data.ClassName);
            inproc32_key.Stub(x => x.GetValue(Arg<string>.Is.Same("RuntimeVersion"))).Return(data.RuntimeVersion);
            inproc32_key.Stub(x => x.GetValue(Arg<string>.Is.Same("ThreadingModel"))).Return("Both");
            inproc32_key.Stub(x => x.GetValue(Arg<string>.Is.Same("CodeBase"))).Return(data.CodeBaseValue);

            version_key.Stub(x => x.GetValue(Arg<string>.Is.Same("Assembly"))).Return(data.AssemblyFullName);
            version_key.Stub(x => x.GetValue(Arg<string>.Is.Same("Class"))).Return(data.ClassName);
            version_key.Stub(x => x.GetValue(Arg<string>.Is.Same("RuntimeVersion"))).Return(data.RuntimeVersion);
            version_key.Stub(x => x.GetValue(Arg<string>.Is.Same("CodeBase"))).Return(data.CodeBaseValue);

            Assert.That(server_handler.IsInstalled(), Is.EqualTo(true));
        }

        [Test]
        public void CanDetermineIfRegistered()
        {
            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            var data = new TestShellExtension();

            var classes_key = MockRepository.GenerateMock<IRegKeyItem>();

            reg_access.Stub(x => x.OpenClassesRoot()).Return(classes_key);

            var server_handler = new ShellExtensionHandler(data, reg_access);

            Assert.That(server_handler.IsRegistered(), Is.EqualTo(false));

            var server_key = MockRepository.GenerateMock<IRegKeyItem>();

            classes_key.Stub(x => x.OpenSubKey(Arg<string>.Is.Equal(string.Format(@"{0}\shellex\{{e357fccd-a995-4576-b01f-234630154e96}}", ".lxf"))))
                .Return(server_key);

            server_key.Stub(x => x.GetValue(Arg<string>.Is.Null)).Return(data.CLSID);

            Assert.That(server_handler.IsRegistered(), Is.EqualTo(true));
        }

        [Test]
        public void CanDetermineIfApproved()
        {
            var data = new TestShellExtension();
            var approved_key = MockRepository.GenerateMock<IRegKeyItem>();
            var reg_access = MockRepository.GenerateMock<IRegistryAccess>();
            reg_access.Stub(x => x.OpenApprovedShellExtensionsKey()).Return(approved_key);

            var server_handler = new ShellExtensionHandler(data, reg_access);

            Assert.That(server_handler.IsApproved(), Is.EqualTo(false));

            approved_key.Stub(x => x.GetValue(Arg<string>.Is.Equal(data.CLSID))).Return(data.DisplayName);

            Assert.That(server_handler.IsApproved(), Is.EqualTo(true));
        }
    }
}
