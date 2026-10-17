using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0005 : BeefBase
    {
        public Beef0005(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0005)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0005 but is 0x{Signature:X}");
            }

            var guid = Utils.ExtractGuidFromShellItem(rawBytes.Skip(8).Take(16).ToArray());

            var index = 24;

            var chunks = new List<byte[]>();

            while (index + 2 <= rawBytes.Length)
            {
                var size = BitConverter.ToInt16(rawBytes, index);

                //0 ends the list; a negative size is damaged data (the index would go back), and an item has at least 3 bytes
                if (size <= 2)
                {
                    break;
                }

                chunks.Add(rawBytes.Skip(index).Take(size).ToArray());
                index += size;
            }

            InternalBags = new List<IShellBag>();

            foreach (var chunk in chunks)
            {
                try
                {
                    switch (chunk[2])
                    {
                        case 0x1f:
                            var f1 = new ShellBag0X1F(-1, -1, chunk, "");
                            InternalBags.Add(f1);
                            break;

                        case 0x2f:
                            var f2 = new ShellBag0X2F(-1, -1, chunk, "");
                            InternalBags.Add(f2);
                            break;

                        case 0x2e:
                            var e2 = new ShellBag0X2E(-1, -1, chunk, "");
                            InternalBags.Add(e2);
                            break;

                        case 0x31:
                            var d = new ShellBag0X31(-1, -1, chunk, "");
                            InternalBags.Add(d);
                            break;

                        default:
                            InternalBags.Add(new ShellBagUnknown(-1, -1, chunk, ""));
                            break;
                    }
                }
                catch (Exception)
                {
                    InternalBags.Add(new ShellBagParseError(-1, -1, chunk, ""));
                }
            }


            VersionOffset = BitConverter.ToInt16(rawBytes, 12);
        }

        public List<IShellBag> InternalBags { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            foreach (var bag in InternalBags)
            {
                sb.AppendLine(bag.ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}