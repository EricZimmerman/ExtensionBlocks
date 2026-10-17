using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X47 : ShellBag
    {
        public ShellBag0X47(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Entire Network";
            ShellBagType = Utils.ShellBagTypes.Network;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            Value = Utils.GetNetworkLocationValue(rawBytes);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}