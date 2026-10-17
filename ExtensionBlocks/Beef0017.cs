using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0017 : BeefBase
    {
        public Beef0017(byte[] rawBytes)
            : base(rawBytes)
        {
            Message = "The purpose of this extension block is unknown";

            var index = 40;

            if (rawBytes.Length > 40)
            {
                index = 40;
            }
            else
            {
                index = 6; //hack
            }


            var size = BitConverter.ToInt16(rawBytes, index);

            index += 2; //skip remaining bytes size
            size = BitConverter.ToInt16(rawBytes, index);


            var sb = new StringBuilder();


            //a size that is too small or runs past the block is damaged data: stop there and keep what was read
            while (size > 2 && index + size <= rawBytes.Length)
            {
                var bytes = new byte[size];
                Buffer.BlockCopy(rawBytes, index, bytes, 0, size);
                index += size;

                var id = bytes[2];

                try
                {
                    switch (id)
                    {
                        case 0x1f:
                            var oneF = new ShellBag0X1F(-1, -1, bytes, string.Empty);
                            sb.AppendLine(oneF.ToString());
                            break;
                        case 0x2f:
                            var twoF = new ShellBag0X2F(-1, -1, bytes, string.Empty);
                            sb.AppendLine(twoF.ToString());
                            break;
                        case 0x31:
                            var threeOne = new ShellBag0X31(-1, -1, bytes, string.Empty);
                            sb.AppendLine(threeOne.ToString());
                            break;
                        case 0x74:
                            var seventy4 = new ShellBagCFSF(-1, -1, bytes, string.Empty);
                            sb.AppendLine(seventy4.ToString());
                            break;
                        default:
                            sb.AppendLine(new ShellBagUnknown(-1, -1, bytes, string.Empty).ToString());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"Shell item 0x{id:X2} could not be parsed: {ex.Message}");
                }

                size = index + 2 <= rawBytes.Length ? BitConverter.ToInt16(rawBytes, index) : (short) 0;
            }


            SubShellDetails = sb.ToString();


            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 2);
        }

        public string SubShellDetails { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();


            sb.AppendLine(base.ToString());

            if (SubShellDetails?.Length > 0)
            {
                sb.AppendLine($"Sub shell items detail:\r\n\r\n{SubShellDetails}");
            }

            return sb.ToString();
        }
    }
}