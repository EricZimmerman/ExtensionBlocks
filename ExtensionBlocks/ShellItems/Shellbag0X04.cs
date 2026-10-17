using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X04 : ShellBag
    {
        public ShellBag0X04(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0x6; //first string

            var strings = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index).Split('\0');

            var s1 = strings[0];
         //   var s2 = strings[2];
        //    var s3 = strings[4];

            FriendlyName = "Directory";
            ShellBagType = Utils.ShellBagTypes.Directory;

        //    index = 0x1e;

        //    var ff = string.Join(", ", strings);

            Value = $"{s1}";

//            var sb = new StringBuilder();
//            foreach (var s in strings.Skip(1))
//            {
//                if (s.Length > 0)
//                {
//                    sb.AppendLine(s);
//                }
//            }

        //    throw new Exception("Finish me");

           // ExtraData = "(See hex view for possible, additional information)";

            //  Value = ff;
        }

        public string ExtraData { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ExtraData?.Length > 0)
            {
                sb.AppendLine($"Extra data: {ExtraData}");
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}