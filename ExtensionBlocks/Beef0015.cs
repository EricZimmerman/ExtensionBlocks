using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0015 : BeefBase
    {
        public Beef0015(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0015)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0015 but is 0x{Signature:X}");
            }

            Message = "The purpose of this extension block is unknown";

            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 2);
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());


            return sb.ToString();
        }
    }
}