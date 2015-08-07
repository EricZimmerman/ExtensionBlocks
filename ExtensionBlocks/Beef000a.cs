using System;
using System.Text;

namespace ExtensionBlocks
{
    internal class Beef000a : BeefBase
    {
        public Beef000a(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef000a)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef000a but is {Signature}");
            }

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