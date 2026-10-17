using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X5C : ShellBag
    {
        public ShellBag0X5C(int slot, int mruPosition, byte[] rawBytes, string bagPath)
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

            var len = 0;
            var index = 3;

            while (rawBytes[index + len] != 0)
            {
                len += 1;
            }

            var val = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, 3, len);

            Value = val;

            index = rawBytes.Length - 5;

            var zeroCount = 0;

            while (zeroCount < 2)
            {
                index -= 1;
                if (rawBytes[index] == 0)
                {
                    zeroCount += 1;
                }
            }

            index += 1;

            var strings = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, index, rawBytes.Length - index - 4);
            var segs = strings.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);

            Description = string.Empty;

            Description = $"{segs[0]} ({segs[1]})";
        }

        public string Description { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Description.Length > 0)
            {
                sb.AppendLine($"Description: {Description}");
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}