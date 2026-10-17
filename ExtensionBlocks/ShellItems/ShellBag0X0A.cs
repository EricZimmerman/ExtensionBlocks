using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X0A : ShellBag
    {
        public ShellBag0X0A(int slot, int mruPosition, byte[] rawBytes, string bagPath)
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

            var index = 0x0e;


            var size = BitConverter.ToInt16(rawBytes, index);
            index += 2;

            var s1 = Encoding.Unicode.GetString(rawBytes, index, size).Trim('\0');

            index += size;

            index += 4; //skip 4 unknown

            size = BitConverter.ToInt16(rawBytes, index);
            index += 2;

            var s2 = Encoding.Unicode.GetString(rawBytes, index, size).Trim('\0');

            index += size;

            index += 4; //skip 4 unknown
            index += 2; //skip some kind of id

            var f = BitConverter.ToDouble(rawBytes, index);
            var d = new DateTimeOffset(  DateTime.FromOADate(f));

            Timestamp = d;

            index += 8;

            index += 4; //skip 4 unknown
            index += 2; //skip some kind of id

            f = BitConverter.ToDouble(rawBytes, index);
            d = new DateTimeOffset(DateTime.FromOADate(f));

            Timestamp2 = d;

            index += 8;

            index += 0x18;

            size = BitConverter.ToInt16(rawBytes, index);
            index += 2;

            if (size == 0)
            {
                Value = $"{s1}: {s2}";

                return;
            }

            var s3 = Encoding.Unicode.GetString(rawBytes, index, size).Trim('\0');
            index += size;

            index += 4; //skip 4 unknown
            index += 2; //skip some kind of id

            index += 5;//skip 5 unknown

            size = BitConverter.ToInt16(rawBytes, index);
            index += 2;

            

            var s4 = Encoding.Unicode.GetString(rawBytes, index, size).Trim('\0');
            index += size;

            Value = $"{s1}: {s2} // {s3}: {s4}" ;
        }

        public DateTimeOffset? Timestamp { get; }
        public DateTimeOffset? Timestamp2 { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Timestamp.HasValue)
            {
                sb.Append($"Timestamp: {Timestamp.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (Timestamp2.HasValue)
            {
                sb.Append($"\r\nTimestamp2: {Timestamp2.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            sb.AppendLine();
            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}