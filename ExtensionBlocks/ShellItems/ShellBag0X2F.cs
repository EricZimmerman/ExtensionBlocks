using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X2F : ShellBag
    {
        public ShellBag0X2F(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Drive letter";
            ShellBagType = Utils.ShellBagTypes.DriveLetter;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var driveLetter = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, 3, 2);

            Value = driveLetter;

            if (rawBytes.Length <= 0x30)
            {
                return;
            }

            var index = 0x19;

            var signature1 = BitConverter.ToUInt32(rawBytes, index + 4);

            var block1 = Utils.GetExtensionBlockFromBytesOrError(signature1, rawBytes.Skip(index).ToArray());

            ExtensionBlocks.Add(block1);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}