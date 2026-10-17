using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef001c : BeefBase
    {
        public Beef001c(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef001c)
            {
                throw new Exception($"Signature mismatch! Should be Beef001c but is 0x{Signature:X}");
            }

            var len = 0;
            var index = 10;

            // Step a whole UTF-16 code unit at a time so a 0x00 high byte followed by a 0x00 low byte isn't taken as the terminator

            while (rawBytes[index + len] != 0x0 || rawBytes[index + len + 1] != 0x0)
            {
                len += 2;
            }

            var uname = Encoding.Unicode.GetString(rawBytes, index, len);

            Information = uname;

            index += len + 2; // move past string and end of string marker

            VersionOffset = BitConverter.ToInt16(rawBytes, index);

            index += 2;
        }

        public string Information { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            sb.AppendLine($"File name: {Information}");

            return sb.ToString();
        }
    }
}