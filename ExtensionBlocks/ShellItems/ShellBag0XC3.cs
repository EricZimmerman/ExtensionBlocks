using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0XC3 : ShellBag
    {
        public ShellBag0XC3(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Network location";
            ShellBagType = Utils.ShellBagTypes.Network;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0;

            var size = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            var classtypeIndicator = rawBytes[index] & 0x70;
            index += 1;

            var unknown1 = rawBytes[index];
            index += 1;

            var flags = rawBytes[index];
            index += 1;

            index = 5;

            //find separator
            var sepLocation = index;
            while (BitConverter.ToInt16(rawBytes, sepLocation) != 0x0002)
            {
                sepLocation += 1;

                if (sepLocation + 1 == rawBytes.Length)
                {
                    sepLocation = -1;
                    break;
                }
            }
            if (sepLocation == -1)
            {
                var stringsRaw2 = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, rawBytes.Length - index);
                var strings2 = stringsRaw2.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);

                Value = strings2[0];
                if (strings2.Length > 1)
                {
                    LocationDescription = strings2[1];
                }


                return;
            }


            var stringRaw = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, sepLocation - 5 - 1);
            var strings = stringRaw.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);

            index = sepLocation;
            index += 2; //move past 00

            Value = strings[0];

            LocationDescription = string.Join(", ", strings.Skip(1));

            if (index == rawBytes.Length)
            {
                return;
            }

            var extBlockSize = BitConverter.ToInt16(rawBytes, index);

            if (extBlockSize > 0)
            {
                //process extension blocks

                while (extBlockSize > 0)
                {
                    var extBytes = rawBytes.Skip(index).Take(extBlockSize).ToArray();

                    index += extBlockSize;

                    var signature1 = BitConverter.ToUInt32(extBytes, 4);


                    var block1 = Utils.GetExtensionBlockFromBytesOrError(signature1, extBytes);

                    ExtensionBlocks.Add(block1);

                    if (index == rawBytes.Length)
                    {
                        return;
                    }

                    extBlockSize = BitConverter.ToInt16(rawBytes, index);
                }
            }
        }

        public string LocationDescription { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Location description: {LocationDescription}\r\n");

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}