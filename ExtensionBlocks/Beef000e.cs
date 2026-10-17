using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// namespaces...

namespace ExtensionBlocks
{
    // internal classes...
    public class Beef000e : BeefBase
    {
        public List<IShellBag> Bags;


        // public constructors...
        public Beef000e(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef000e)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef000e but is 0x{Signature:X}");
            }

            ExtensionBlocks = new List<IExtensionBlock>();

            Bags = new List<IShellBag>();

            PropertyStores = new List<PropertyStore>();


            var index = 8;

            //sometimes the bags start at a different offset, so find it
            while (rawBytes[index] != 0x14)
            {
                index += 1;
            }

//            if (rawBytes[index] == 0x01)
//            {
//                index = 0xC;
//            }
//            else
//            {
//                index = 0x0A;
//            }

            var chks = new List<byte[]>();

            var size = BitConverter.ToInt16(rawBytes,index);

            while (size>0)
            {
                var chB = new byte[size];

                Buffer.BlockCopy(rawBytes,index,chB,0,size);

                chks.Add(chB);


                index += size;



                size = BitConverter.ToInt16(rawBytes, index);
            }


            foreach (var bytese in chks)
            {

                var id = bytese[2].ToString("X2");

                var className = $"ExtensionBlocks.ShellBag0X{id}";

                var t = Type.GetType(className);

                if (t == null)
                {
                    var foo= new ShellBagUnknown(-1, -1, bytese, string.Empty);
                    Bags.Add(foo);
                    continue;
                }

                var bar =  (IShellBag)Activator.CreateInstance(t, -1, -1, bytese, string.Empty);

                Bags.Add(bar);

             
            }

            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 2);

        

          
        }

        // public properties...
        public List<IExtensionBlock> ExtensionBlocks { get; set; }
        public List<PropertyStore> PropertyStores { get; }

        // public methods...
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());


            if (Bags.Count > 0)
            {
                var bagNumber = 0;

                sb.AppendLine();
                sb.AppendLine($"Internal ShellBags found: {Bags.Count}");

                foreach (var bag in Bags)
                {
                    sb.AppendLine($"---------------------- Bag {bagNumber:N0} ----------------------");

                    sb.AppendLine(bag.ToString());


                    bagNumber += 1;
                }

                sb.AppendLine("--------------------------------------------------");
            }


            return sb.ToString();
        }
    }
}