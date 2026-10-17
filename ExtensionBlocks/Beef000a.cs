using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef000a : BeefBase
    {
        public Beef000a(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef000a)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef000a but is 0x{Signature:X}");
            }

            Message = $"The purpose of this extension block is unknown. Bytes: {BitConverter.ToString(rawBytes)}";

            VersionOffset = BitConverter.ToInt16(rawBytes, 12);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}