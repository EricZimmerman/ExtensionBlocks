using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0016 : BeefBase
    {
        public Beef0016(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0016)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0016 but is {Signature}");
            }

            Value = Encoding.Unicode.GetString(rawBytes, 10, rawBytes.Length - 14);

            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 2);
        }

        public string Value { get; }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine($"Value: {Value}");

            return sb.ToString();
        }
    }
}