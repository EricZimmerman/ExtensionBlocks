using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X70 : ShellBag
    {
        public ShellBag0X70(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();


            BagPath = bagPath;

            var index = 0x04;

            FriendlyName = "GUID";
            ShellBagType = Utils.ShellBagTypes.Guid;

            var rawguid1 = new byte[16];

            Array.Copy(rawBytes, index, rawguid1, 0, 16);

            var rawguid = Utils.ExtractGuidFromShellItem(rawguid1);

            var foldername = GuidMapping.GuidMapping.GetDescriptionFromGuid(rawguid);


            Value = foldername;
        }


        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}