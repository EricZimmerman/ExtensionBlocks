using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0006 : BeefBase
    {
        public Beef0006(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0006)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0006 but is 0x{Signature:X}");
            }

            var len = 0;
            var index = 8;

            // Step a whole UTF-16 code unit at a time so a 0x00 high byte followed by a 0x00 low byte isn't taken as the terminator

            while (rawBytes[index + len] != 0x0 || rawBytes[index + len + 1] != 0x0)
            {
                len += 2;
            }

            var uname = Encoding.Unicode.GetString(rawBytes, index, len);

            UserName = uname;

            index += len + 2; // move past string and end of string marker

            VersionOffset = BitConverter.ToInt16(rawBytes, index);
        }

        public string UserName { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            sb.AppendLine($"User Name: {UserName}");

            return sb.ToString();
        }
    }
}