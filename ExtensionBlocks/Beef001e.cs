using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef001e : BeefBase
    {
        public Beef001e(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef001e)
            {
                throw new Exception($"Signature mismatch! Should be Beef001e but is 0x{Signature:X}");
            }

            var index = 10;

         

            var uname = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length- 10 - 2).Trim('\0');

            PinType = uname;

            
            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length-2);
        }

        public string PinType { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            sb.AppendLine($"Pin Type: {PinType}");

            return sb.ToString();
        }
    }
}