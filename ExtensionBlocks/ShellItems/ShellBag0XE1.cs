using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0XE1 : ShellBag
    {
        public ShellBag0XE1(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0x14;

            var swn1Sig = 0x2e572e53;

            while (BitConverter.ToInt32(rawBytes, index) != swn1Sig)
            {
                index += 1;
            }

            var swnPosition = index;
            index = 0x14;

            var name = Encoding.Unicode.GetString(rawBytes, index, swnPosition - index).Trim('\0');

            Value = name;

            index = swnPosition;
            index += 7; //skip signature

            var t = rawBytes[index];
            if (t == 1)
            {
                //dir
                FriendlyName = "Directory";
                ShellBagType = Utils.ShellBagTypes.Directory;
            }
            else if (t == 2)
            {
                FriendlyName = "File";
                ShellBagType = Utils.ShellBagTypes.File;
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