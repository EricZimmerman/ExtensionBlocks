using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X46 : ShellBag
    {
        public ShellBag0X46(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Microsoft Windows Network";
            ShellBagType = Utils.ShellBagTypes.Network;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            if (rawBytes.Length - 5 - 4 < 0)
            {
                Value = "(None)";
                return;
            }

            var strings = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, 5, rawBytes.Length - 5 - 4);
                // the extra 4 at the end is the 02 00 00 00
            var segs = strings.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);

            if (segs.Length == 1)
            {
                Value = $"{segs[0]}";
            }
            else
            {
                Value = $"{segs[0]} ({segs[1]})";
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}