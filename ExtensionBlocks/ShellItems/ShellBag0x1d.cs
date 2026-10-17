using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X1D : ShellBag
    {
        public ShellBag0X1D(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "URI";
            ShellBagType = Utils.ShellBagTypes.NotAssigned;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;


            var index = 0x16;

            FileTime1 = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index));
            index += 8;

            index = 0x26;

            var len = 0;

            while (rawBytes[index + len] != 0)
            {
                len += 1;
            }

            Value = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, len);
        }


        public DateTimeOffset? FileTime1 { get; }

        public string URI { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Time stamp: {FileTime1}");
            sb.AppendLine();


            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}