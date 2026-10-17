using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X3A : ShellBag
    {
        public ShellBag0X3A(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Directory";
            ShellBagType = Utils.ShellBagTypes.Directory;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0xA;

            var size = (int) rawBytes[index];

            index = 0xB;

            if (rawBytes[0xB] == 0x1)
            {
                index = 0xC;
            }

            var rawVal = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, index, size);

            var segs = rawVal.Split('|');

            Value = segs.Last();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}