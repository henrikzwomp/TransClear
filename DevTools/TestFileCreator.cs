using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DevTools
{
    public class TestFileCreator
    {
        public void CreatFromRandomFileInParentFolder()
        {
            string[] files = Directory.GetFiles("..", "*.lxf");

            Random rnd = new Random();
            string source_file = files[rnd.Next(0, files.Length)];

            string new_file_name = CreateName.FromCurrentTime();

            File.Copy(source_file, new_file_name); 
        }

        public void DuplicateLastFileCreated()
        {
            string[] files_array = Directory.GetFiles(".", "*.lxf");

            var files = files_array.Select(x => x.Substring(x.LastIndexOf("\\") + 1));

            files = files.Where(x => x.Length == 21);

            files = files.OrderBy(x => x);

            string source_file = files.Last();

            // 20150328162834593.lxf
            // 123456789012345678901

            string new_file_name = CreateName.FromCurrentTime();

            File.Copy(source_file, new_file_name); 
        }
    }
}
