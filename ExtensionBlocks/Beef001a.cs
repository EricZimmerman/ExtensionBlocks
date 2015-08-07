using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef001a : BeefBase
    {
        public Beef001a(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef001a)
            {
                throw new Exception($"Signature mismatch! Should be Beef001a but is {Signature}");
            }

            var len = 0;
            var index = 10;

            while ((rawBytes[index + len] != 0x0 || rawBytes[index + len + 1] != 0x0))
            {
                len += 1;
            }

            var uname = Encoding.Unicode.GetString(rawBytes, index, len + 1);

            FileDocumentTypeString = uname;

            index += len + 3; // move past string and end of string marker

            //is index 24?

            //TODO get shell item list

            Message =
                "Unsupported Extension block. Please report to Report to saericzimmerman@gmail.com to get it added!";

            VersionOffset = BitConverter.ToInt16(rawBytes, index);
        }

        public string FileDocumentTypeString { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            sb.AppendLine($"File Document Type String: {FileDocumentTypeString}");

            return sb.ToString();
        }
    }
}