using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionBlocks.Test2
{
    class Program
    {
        static void Main(string[] args)
        {
            var fooBytes = File.ReadAllBytes(@"C:\Temp\prop.bin");

            var foo = new PropertyStore(fooBytes);
            Debug.WriteLine(foo);
        }
    }
}
