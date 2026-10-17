using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0XCA : ShellBag
    {
        public ShellBag0XCA(int slot, int mruPosition, byte[] rawBytes, string bagPath)
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

            var index = 0x16;
            
            var ts = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index));
            Timestamp = ts;

            if (ts.Year == 1600)
            {
                Timestamp = null;
            }

            index = 0x26;

            var s = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, index, rawBytes.Length - index).Split('\0').First();

            Value = s;
        }

        public DateTimeOffset? Timestamp { get; }


        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Timestamp.HasValue)
            {
                sb.Append($"Timestamp: {Timestamp.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }
      

            sb.AppendLine();
            sb.AppendLine(base.ToString());


            return sb.ToString();
        }
    }
}