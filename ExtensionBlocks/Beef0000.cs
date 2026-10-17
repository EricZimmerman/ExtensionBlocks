using System;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0000 : BeefBase
    {
        public Beef0000(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0000)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0000 but is 0x{Signature:X}");
            }

            if (rawBytes.Length == 0xe)
            {
                return;
            }

            //TODO lose linq stuff
            GUID1 = Utils.ExtractGuidFromShellItem(rawBytes.Skip(8).Take(16).ToArray());

            GUID1Folder = GuidMapping.GuidMapping.GetDescriptionFromGuid(GUID1);

            if (rawBytes.Length == 0x18)
            {
                GUID2 = "(Not found)";
                GUID2Folder = "(NA)";
                return;
            }

            GUID2 = Utils.ExtractGuidFromShellItem(rawBytes.Skip(24).Take(16).ToArray());

            GUID2Folder = GuidMapping.GuidMapping.GetDescriptionFromGuid(GUID2);

            VersionOffset = BitConverter.ToInt16(rawBytes, 40);
        }

        public string GUID1 { get; }

        public string GUID1Folder { get; }

        public string GUID2 { get; }

        public string GUID2Folder { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            sb.AppendLine($"GUID 1: {GUID1}");
            sb.AppendLine($"GUID 1 Folder: {GUID1Folder}");
            sb.AppendLine($"GUID 2: {GUID2}");
            sb.AppendLine($"GUID 2 Folder: {GUID2Folder}");

            return sb.ToString();
        }
    }
}