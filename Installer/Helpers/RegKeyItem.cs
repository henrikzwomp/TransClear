using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Security.AccessControl; // Really needed? Used for RegistryRights class

namespace Installer
{
    public interface IRegKeyItem : IDisposable
    {
        void SetValue(string key_name, string key_value);
        IRegKeyItem OpenSubKey(string subkey);
        void DeleteSubKeyTree(string subkey);
        void DeleteValue(string name);
        IList<string> GetSubKeyNames();
        IRegKeyItem CreateOrOpenSubKey(string subkey);
        string GetValue(string name);
    }

    public class RegKeyItem : IRegKeyItem
    {
        private RegistryKey _key;

        public RegKeyItem(RegistryKey key)
        {
            if (key == null)
                throw new Exception("Supplied key was null");

            _key = key;
        }

        public void SetValue(string key_name, string key_value)
        {
            _key.SetValue(key_name, key_value, RegistryValueKind.String);
        }

        public IRegKeyItem CreateSubKey(string subkey)
        {
            return new RegKeyItem(_key.CreateSubKey(subkey));
        }

        public IRegKeyItem OpenSubKey(string subkey)
        {
            var item = _key.OpenSubKey(subkey, true);

            if (item == null)
                return null;

            return new RegKeyItem(item);
        }

        public void DeleteSubKeyTree(string subkey)
        {
            _key.DeleteSubKeyTree(subkey);
        }

        public void DeleteValue(string name)
        {
            _key.DeleteValue(name, false);
        }

        public IList<string> GetSubKeyNames()
        {
            return new List<string>(_key.GetSubKeyNames());
        }

        public void Dispose()
        {
            _key.Dispose();
        }

        public string GetValue(string name)
        {
            var result = _key.GetValue(name);

            if (result == null)
                return null;

            return result.ToString();
        }

        public IRegKeyItem CreateOrOpenSubKey(string subkey)
        {
            var result = OpenSubKey(subkey);

            if (result != null)
                return result;

            return CreateSubKey(subkey);
        }
    }
}
