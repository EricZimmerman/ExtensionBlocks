﻿using System;
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
            var fooBytes = File.ReadAllBytes(@"C:\Temp\beef0024.bin");


            var aa = Utils.GetExtensionBlockFromBytes(0xbeef0024, fooBytes);

        }


        [Test]
        public void guidLookup()
        {

            var aaa = Utils.GetDescriptionFromGuidAndKey("de35258c-c695-4cbc-b982-38b0ad24ced0", 2);

            Assert.Equals(aaa,"Shell Omit From View");


//            {"de35258c-c695-4cbc-b982-38b0ad24ced0",new HashSet<IdName>()
//            { 
//                new IdName (2, "Shell Omit From View"),
//            } },
        }
    }
}
