using System;
using System.Text;

namespace ExtensionBlocks
{
    internal class Beef0029 : BeefBase
    {
        public Beef0029(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0029)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0029 but is {Signature}");
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