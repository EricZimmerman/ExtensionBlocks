using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X2C : ShellBag
    {
        public ShellBag0X2C(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Search? (Unconfirmed)";
            ShellBagType = Utils.ShellBagTypes.User;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0x28;

            var folderName = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index - 0x10).Trim('\0');

            Value = folderName;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}