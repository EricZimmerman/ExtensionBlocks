using System;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    internal class Beef0003 : BeefBase
    {
        public Beef0003(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0003)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0003 but is {Signature}");
            }

            GUID1 = Utils.ExtractGuidFromShellItem(rawBytes.Skip(8).Take(16).ToArray());

            GUID1Folder = Utils.GetFolderNameFromGuid(GUID1);

            VersionOffset = BitConverter.ToInt16(rawBytes, 24);
        }

        public string GUID1 { get; }

        public string GUID1Folder { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            sb.AppendLine($"GUID 1: {GUID1}");
            sb.AppendLine($"GUID 1 Folder: {GUID1Folder}");

            return sb.ToString();
        }
    }
}