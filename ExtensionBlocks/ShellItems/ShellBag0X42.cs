using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X42 : ShellBag
    {
        public ShellBag0X42(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Server UNC path";
            ShellBagType = Utils.ShellBagTypes.Network;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var strings = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, 5, rawBytes.Length - 5 - 4);
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