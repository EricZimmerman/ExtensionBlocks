using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X3A : ShellBag
    {
        public ShellBag0X3A(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Directory";
            ShellBagType = Utils.ShellBagTypes.Directory;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            //a file entry, as Lnk and the RECmd plugins see them (size, file size, date, attributes, short name, 0xbeef0004):
            //read as a 0x31 item; ShellBags' 0x3A items have no 0xbeef0004 block
            if (Utils.IndexOf(rawBytes, Utils.Beef0004Signature) >= 0)
            {
                var entry = new ShellBag0X31(slot, mruPosition, rawBytes, bagPath);
                FriendlyName = entry.FriendlyName;
                ShellBagType = entry.ShellBagType;
                ExtensionBlocks = entry.ExtensionBlocks;
                Value = entry.Value;
                ShortName = entry.ShortName;
                LastModificationTime = entry.LastModificationTime;
                LastAccessTime = entry.LastAccessTime;
                return;
            }

            var index = 0xA;

            var size = (int) rawBytes[index];

            index = 0xB;

            if (rawBytes[0xB] == 0x1)
            {
                index = 0xC;
            }

            var rawVal = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, size);

            var segs = rawVal.Split('|');

            Value = segs.Last();
        }

        /// <summary>For a file entry</summary>
        public string ShortName { get; }

        /// <summary>For a file entry</summary>
        public DateTimeOffset? LastModificationTime { get; }

        /// <summary>For a file entry</summary>
        public DateTimeOffset? LastAccessTime { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}