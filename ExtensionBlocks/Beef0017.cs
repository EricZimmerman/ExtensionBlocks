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