using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Accepted arguments:");
                Console.WriteLine("- CreatFromRandomFileInParentFolder");
                Console.WriteLine("- DuplicateLastFileCreated");
            }
            else if(args[0].ToLower() == "CreatFromRandomFileInParentFolder".ToLower())
            {
                var tfc = new TestFileCreator();
                tfc.CreatFromRandomFileInParentFolder();
            }
            else if (args[0].ToLower() == "DuplicateLastFileCreated".ToLower())
            {
                var tfc = new TestFileCreator();
                tfc.DuplicateLastFileCreated();
            }
        }
    }
}
