using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X03 : ShellBag
    {
        public ShellBag0X03(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0x2a; //first string

            if (rawBytes[0x2c] == 0xe4 && rawBytes[0x2d] == 0x40)
            {
                index = 0x2e;

            //    var sss = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index).Trim('\0');


            }

            var strings = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index).Split('\0');

            var s1 = strings[0];
            var s2 = strings[2];
            var s3 = strings[4];

            FriendlyName = "Unknown (Unconfirmed purpose)";
            ShellBagType = Utils.ShellBagTypes.Unknown;

            index = 0x1e;

            var f = BitConverter.ToDouble(rawBytes, index);
            var d = DateTime.FromOADate(f);

            if (d.Year > 1900)
            {
                Value = $"{s1} {s2} {s3} Timestamp: {d}";
                return;
            }

            Value = $"{s1} {s2} {s3} (No date found)";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}