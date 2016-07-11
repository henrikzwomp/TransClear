using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Installer;
using Microsoft.Win32;

namespace UnitTests.Installer
{
    [TestFixture]
    public class RegKeyItemTests
    {
        string _test_path = @"Software\TransClear\UnitTest";

        [SetUp]
        public void SetUp()
        {
            var key = Registry.CurrentUser.OpenSubKey(_test_path, true);

            if (key != null)
                Registry.CurrentUser.DeleteSubKeyTree(_test_path);
        }

        [TearDown]
        public void TearDown()
        {
            var key = Registry.CurrentUser.OpenSubKey(_test_path, true);

            if (key != null)
                Registry.CurrentUser.DeleteSubKeyTree(_test_path);
        }

        [Test]
        public void CanDeleteValue()
        {
            var regkey = new RegKeyItem(Registry.CurrentUser.CreateSubKey(_test_path));

            regkey.SetValue("Hello", "World");

            var registry_key = Registry.CurrentUser.CreateSubKey(_test_path);
            Assert.That(registry_key.GetValue("Hello", null), Is.EqualTo("World"));

            regkey.DeleteValue("Hello");

            registry_key = Registry.CurrentUser.CreateSubKey(_test_path);
            Assert.That(registry_key.GetValue("Hello", null), Is.Null);
        }

        [Test]
        public void CanGetSubKeyNames()
        {
            Registry.CurrentUser.CreateSubKey(_test_path);

            var regkey = new RegKeyItem(Registry.CurrentUser.OpenSubKey(@"Software\TransClear"));

            var result = regkey.GetSubKeyNames();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Equals("UnitTest", StringComparison.OrdinalIgnoreCase), Is.True);
        }

        [Test]
        public void CanDeleteSubKeyTree()
        {
            var reg_key_item = new RegKeyItem(Registry.CurrentUser);

            var sub_key = reg_key_item.CreateSubKey(_test_path);
            var sub_key2 = sub_key.CreateOrOpenSubKey("HelloWorld");

            Assert.That(sub_key, Is.Not.Null);
            Assert.That(sub_key2, Is.Not.Null);

            var result1 = Registry.CurrentUser.OpenSubKey(_test_path + "\\HelloWorld");
            Assert.That(result1, Is.Not.Null);

            sub_key.DeleteSubKeyTree("HelloWorld");

            var result2 = Registry.CurrentUser.OpenSubKey(_test_path + "\\HelloWorld");
            Assert.That(result2, Is.Null);
        }

        [Test]
        public void CanOpenSubKeyWrittable()
        {
            var reg_key_item = new RegKeyItem(Registry.CurrentUser);

            var result = reg_key_item.CreateSubKey(_test_path);

            Assert.That(result, Is.Not.Null);

            result.CreateOrOpenSubKey("HelloWorld");

            var result2 = Registry.CurrentUser.OpenSubKey(_test_path + "\\HelloWorld");
            Assert.That(result2, Is.Not.Null);
        }

        [Test]
        public void CanOpenSubKey()
        {
            var reg_key_item = new RegKeyItem(Registry.CurrentUser );

            Registry.CurrentUser.CreateSubKey(_test_path);

            var result = reg_key_item.OpenSubKey(_test_path);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CanSetValue()
        {
            Assert.That(Registry.CurrentUser.OpenSubKey(_test_path, true), Is.Null);

            var reg_key_item = new RegKeyItem(Registry.CurrentUser.CreateSubKey(_test_path));

            reg_key_item.SetValue("MyFirsttest", "HelloWorld");

            var result = Registry.CurrentUser.OpenSubKey(_test_path);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetValue("MyFirsttest"), Is.EqualTo("HelloWorld"));
        }

        [Test]
        public void CanCreateSubKey()
        {
            var reg_key_item = new RegKeyItem(Registry.CurrentUser.CreateSubKey(_test_path));

            reg_key_item.CreateSubKey("HelloWorld");

            var result = Registry.CurrentUser.OpenSubKey(_test_path + "\\HelloWorld");
            Assert.That(result, Is.Not.Null);
        }

        [Test, ExpectedException]
        public void WillThrowExceptionWhenKeyIsNull()
        {
            var reg_key = Registry.CurrentUser;

            var result = new RegKeyItem(reg_key.OpenSubKey(_test_path)); // Path should not exist now
        }

        [Test]
        public void WillReturnNullIfSubKeyIsMissing()
        {
            var reg_key_item = new RegKeyItem(Registry.CurrentUser);

            var result = reg_key_item.OpenSubKey(_test_path); // Path should not exist now

            Assert.That(result, Is.Null);
        }

        [Test]
        public void CanGetValue()
        {
            var regkey = Registry.CurrentUser.CreateSubKey(_test_path);
            regkey.SetValue("Hello", "World");

            var regkeyitem = new RegKeyItem(regkey);

            Assert.That(regkeyitem.GetValue("Hello"), Is.EqualTo("World"));
        }

        [Test]
        public void WillReturnNullIfValueDoesntExist()
        {
            var regkey = Registry.CurrentUser.CreateSubKey(_test_path);

            var regkeyitem = new RegKeyItem(regkey);

            Assert.That(regkeyitem.GetValue("Hello"), Is.Null);
        }

        [Test]
        public void CallToCreateOrOpenSubKeyMethodWorksWhenSubKeyExists()
        {
            var reg_key_item = new RegKeyItem(Registry.CurrentUser.CreateSubKey(_test_path));

            reg_key_item.CreateSubKey("HelloWorld");

            var result = Registry.CurrentUser.OpenSubKey(_test_path + "\\HelloWorld");
            Assert.That(result, Is.Not.Null);

            var new_key = reg_key_item.CreateOrOpenSubKey("HelloWorld");
            Assert.That(new_key, Is.Not.Null);
        }

        [Test]
        public void CallToCreateOrOpenSubKeyMethodWorksWhenSubKeyIsMissing()
        {
            var reg_key_item = new RegKeyItem(Registry.CurrentUser.CreateSubKey(_test_path));
            Assert.That(reg_key_item, Is.Not.Null);

            var new_key = reg_key_item.CreateOrOpenSubKey("HelloWorld");
            Assert.That(new_key, Is.Not.Null);

            var result = Registry.CurrentUser.OpenSubKey(_test_path + "\\HelloWorld");
            Assert.That(result, Is.Not.Null);
        }

        /*[Test]
        public void TestRegisterBehaivor()
        {
            var current_user_key = Registry.CurrentUser;
            var test_key = current_user_key.CreateSubKey(_test_path);

            test_key.CreateSubKey("HelloWorld");

            var result = Registry.CurrentUser.OpenSubKey(_test_path + "\\HelloWorld");
            Assert.That(result, Is.Not.Null);
        }*/
    }
}
