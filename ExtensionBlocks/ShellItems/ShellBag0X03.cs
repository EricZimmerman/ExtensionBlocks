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

            if (rawBytes.Length > 0x2d && rawBytes[0x2c] == 0xe4 && rawBytes[0x2d] == 0x40)
            {
                index = 0x2e;

            //    var sss = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index).Trim('\0');


            }

            var strings = rawBytes.Length > index ? Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index).Split('\0') : new string[0];

            //items with fewer strings (as in OpenSavePidlMRU) threw, and the whole value was lost
            var s1 = strings.Length > 0 ? strings[0] : string.Empty;
            var s2 = strings.Length > 2 ? strings[2] : string.Empty;
            var s3 = strings.Length > 4 ? strings[4] : string.Empty;

            FriendlyName = "Unknown (Unconfirmed purpose)";
            ShellBagType = Utils.ShellBagTypes.Unknown;

            index = 0x1e;

            var d = DateTime.MinValue;
            if (rawBytes.Length >= index + 8)
            {
                try
                {
                    d = DateTime.FromOADate(BitConverter.ToDouble(rawBytes, index));
                }
                catch (ArgumentException)
                {
                    //not a date
                }
            }

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