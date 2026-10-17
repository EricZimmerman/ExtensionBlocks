using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X41 : ShellBag
    {
        public ShellBag0X41(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Domain/Workgroup name";
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