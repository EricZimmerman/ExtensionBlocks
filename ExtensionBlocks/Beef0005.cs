using System;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0005 : BeefBase
    {
        public Beef0005(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0005)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0005 but is {Signature}");
            }

            var guid = Utils.ExtractGuidFromShellItem(rawBytes.Skip(8).Take(16).ToArray());

            Message =
                "Unsupported Extension block. Please report to Report to saericzimmerman@gmail.com to get it added!";

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