using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ExtensionBlocks.Test
{
    [TestFixture]
    public class TestMain
    {
        [Test]
        public void foobar()
        {
            var fooBytes = File.ReadAllBytes(@"C:\Temp\prop.bin");

            var foo = new PropertyStore(fooBytes);
            Debug.WriteLine(foo);
        }
    }
}
