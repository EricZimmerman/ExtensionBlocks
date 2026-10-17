using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X50 : ShellBag
    {
        public ShellBag0X50(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Nokia sub folder";
            ShellBagType = Utils.ShellBagTypes.Directory;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0;

            if (rawBytes[0xa] == 6)
            {
                index = 0x22;
                var size2 = BitConverter.ToInt32(rawBytes, index);
                index += 4;

                var buff2 = new byte[size2];

                Buffer.BlockCopy(rawBytes, index, buff2, 0, size2);
                index += size2;

                var s1 = Encoding.Unicode.GetString(buff2).Split('\0');

                var s2 = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index).Split('\0').First();

                Value = $"{s1[0]} ({s1[2]}): {s2}";

                return;
            }

            if (rawBytes[0xa] == 0xc)
            {
                index = 0x3e;

                var s2 = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index).Split('\0').First();

                Value = s2;

                return;
            }

            index = 0x26;

            var size = BitConverter.ToInt32(rawBytes, index);
            var buff = new byte[size];

            Buffer.BlockCopy(rawBytes, index, buff, 0, size);

            var foo = Encoding.Unicode.GetString(buff, 0x8, buff.Length - 8).Split('\0').First();

            Value = foo;

            index += size;

            FileTime1 = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index)).ToUniversalTime();
            index += 8;
        }

        public DateTimeOffset? FileTime1 { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (FileTime1.HasValue)
            {
                sb.AppendLine($"Timestamp: {FileTime1}");
                sb.AppendLine();
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}