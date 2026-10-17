using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X53 : ShellBag
    {
        public ShellBag0X53(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            FriendlyName = "Root folder: GUID";
            ShellBagType = Utils.ShellBagTypes.Guid;

            var delegateGuid = Utils.ExtractGuidFromShellItem(rawBytes.Skip(20).Take(16).ToArray());

            var folderGuid = Utils.ExtractGuidFromShellItem(rawBytes.Skip(36).Take(16).ToArray());

            var foldername = GuidMapping.GuidMapping.GetDescriptionFromGuid(folderGuid);

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