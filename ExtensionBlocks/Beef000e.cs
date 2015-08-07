using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// namespaces...
namespace ExtensionBlocks
{
    // internal classes...
    internal class Beef000e : BeefBase
    {
        // public constructors...
        public Beef000e(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef000e)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef000e but is {Signature}");
            }

            ExtensionBlocks = new List<IExtensionBlock>();

            Bags = new List<IShellBag>();

            var rawguid1 = new byte[16];

            var index = 16;

            Array.Copy(rawBytes, index, rawguid1, 0, 16);

            var rawguid = ShellBagUtils.ExtractGuidFromShellItem(rawguid1);

            var foldername = ShellBagUtils.GetFolderNameFromGuid(rawguid);

            GUIDName = foldername;

            index += 16;

            index += 18;

            PropertyStores = new List<PropertyStore>();

            for (var i = 0; i < 3; i++)
            {
                var len = BitConverter.ToUInt32(rawBytes, index);

                var propStore = new PropertyStore(rawBytes.Skip(index).Take((int)len).ToArray());

                PropertyStores.Add(propStore);

                index += (int)len;
            }



            index += 11;



            var chunks = new List<string>();
            var len1 = 0;
            var s1 = string.Empty;

            var maxLoop = 0;

            while (maxLoop < 3)
            {
                len1 = 0;
                while (rawBytes[ index + len1] != 0x00)
                {
                    len1 += 1;
                }

                s1 = Encoding.ASCII.GetString(rawBytes, index, len1);

                chunks.Add(s1);
                index += len1 + 1;

                maxLoop += 1;
            }

         

            index += 16;


            while (rawBytes[index + len1] != 0x00)
            {
                len1 += 1;
            }

            s1 = Encoding.ASCII.GetString(rawBytes, index, len1);

            chunks.Add(s1);
            index += len1 + 1;


            index += 1;

            var extSize = 0;
            extSize = BitConverter.ToUInt16(rawBytes, index);

            var sig = BitConverter.ToUInt32(rawBytes, index + 4);

            var block = ShellBagUtils.GetExtensionBlockFromBytes(sig, rawBytes.Skip(index).Take(extSize).ToArray());

            ExtensionBlocks.Add(block);
            index += extSize;

            extSize = BitConverter.ToUInt16(rawBytes, index);

             sig = BitConverter.ToUInt32(rawBytes, index + 4);

             block = ShellBagUtils.GetExtensionBlockFromBytes(sig, rawBytes.Skip(index).Take(extSize).ToArray());

            ExtensionBlocks.Add(block);
            index += extSize;

            extSize = BitConverter.ToUInt16(rawBytes, index);

        

            while (extSize > 0)
            {
                var sb = new ShellBagTypes.ShellBag0X31(-1, -1, rawBytes.Skip(index).Take(extSize).ToArray(), "Inside Beef000e block");

                Bags.Add(sb);

                index += extSize; // end of the bag
                extSize = BitConverter.ToUInt16(rawBytes, index);
            }

            index += 2; //skip empty bag
            
            

            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 2);
        }

        // public properties...
        public List<IExtensionBlock> ExtensionBlocks { get; set; }
        public string GUIDName { get; set; }
        public List<PropertyStore> PropertyStores { get; private set; }
        
        public List<IShellBag> Bags;

        // public methods...
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            var sheetNumber = 0;

            sb.AppendLine($"{GUIDName}");
            sb.AppendLine();

            foreach (var propertyStore in PropertyStores)
            {
                foreach (var propertySheet in propertyStore.Sheets)
                {
                    sb.AppendLine($"Sheet #{sheetNumber} => {propertySheet}");

                    sheetNumber += 1;


                    foreach (var propertyName in propertySheet.PropertyNames)
                    {
                        sb.AppendLine($"Key: {propertyName.Key}, Value: {propertyName.Value}");
                    }
                }
            }

            if (ExtensionBlocks.Count > 0)
            {
                var extensionNumber = 0;

                sb.AppendLine();
                sb.AppendLine($"Extension blocks found: {ExtensionBlocks.Count}");

                foreach (var extensionBlock in ExtensionBlocks)
                {
                    sb.AppendLine($"---------------------- Block {extensionNumber:N0} ----------------------");

                    sb.AppendLine(extensionBlock.ToString());

                    extensionNumber += 1;
                }

                sb.AppendLine("--------------------------------------------------");
            }

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
