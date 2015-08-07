using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0013 : BeefBase
    {
        public Beef0013(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0013)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0013 but is {Signature}");
            }

            Message = "The purpose of this extension block is unknown";

            VersionOffset = BitConverter.ToInt16(rawBytes, 40);
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());


            return sb.ToString();
        }
    }
}