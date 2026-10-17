using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X21 : ShellBag
    {
        public ShellBag0X21(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0x19;

            FriendlyName = "GUID (Unconfirmed purpose)";
            ShellBagType = Utils.ShellBagTypes.Guid;

            var rawguid1 = new byte[16];

            Array.Copy(rawBytes, index, rawguid1, 0, 16);

            var rawguid = Utils.ExtractGuidFromShellItem(rawguid1);

            var foldername = GuidMapping.GuidMapping.GetDescriptionFromGuid(rawguid);

            Value = foldername;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}