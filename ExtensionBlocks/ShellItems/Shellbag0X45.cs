using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X45 : ShellBag
    {
        public ShellBag0X45(int slot, int mruPosition, byte[] rawBytes, string bagPath)
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

            var index = 0;

            if (rawBytes[0x02] == 0x45 && rawBytes[0x04] == 0x41 && rawBytes[0x06] == 0x53 && rawBytes[0x08] == 0x59) //easy in unicode
            {
                index = 0x18;

                var strings = new List<string>();

                var s = Encoding.Unicode.GetString(rawBytes, index, 0x46).Trim('\0').Trim();

                strings.Add(s);

                index = 0x5e;

                while (index<rawBytes.Length)
                {
                    if (rawBytes.Length - index < 4)
                    {
                        break;
                    }

                    var size = BitConverter.ToInt32(rawBytes, index);

                    index += 4;

                    var buff = new byte[size];

                    Buffer.BlockCopy(rawBytes, index, buff, 0, size);

                    s = Encoding.Unicode.GetString(buff, 0, size).Trim('\0').Trim();

                    strings.Add(s);

                    index += size;
                    if (rawBytes[index] == 0x2)
                    {
                        index += 6; //unknown 6 bytes
                    }

                    if (rawBytes[index] == 0x2)
                    {
                        index = rawBytes.Length;
                    }
                }

                Value = string.Join(", ", strings);

                return;
            }

            index = 0x04;

            //best effort
            var outlier = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index - 0x54).Split(new[] { '\0' },StringSplitOptions.RemoveEmptyEntries);

            Value = string.Join(", ", outlier);
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}