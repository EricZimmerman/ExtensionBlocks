using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X1A : ShellBag
    {
        public ShellBag0X1A(int slot, int mruPosition, byte[] rawBytes, string bagPath)
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

            var index = 0x00;


            if (rawBytes[3] == 0x0)
            {
                index = 0x16; //datetime

                var ts = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index));

                index = 0x26;

                Timestamp = ts;

                if (ts.Year == 1600)
                {
                    Timestamp = null;
                }

                var s = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index,rawBytes.Length - index).Split('\0').First();

                Value = s;
                return;
            }

            index = 0xb8;

            var outlier = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, rawBytes.Length - index).Trim('\0');

            Value = outlier;
        }

        public DateTimeOffset? Timestamp { get;  }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Timestamp.HasValue)
            {
                sb.Append($"Timestamp: {Timestamp.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}