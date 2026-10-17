using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0XE8 : ShellBag
    {
        public ShellBag0XE8(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0x4;

            var nameLen = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            var name = Encoding.Unicode.GetString(rawBytes, index, nameLen*2).Trim('\0');

            Value = name;
            
                FriendlyName = "Directory";
                ShellBagType = Utils.ShellBagTypes.Directory;
            
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}