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
                throw new Exception($"Signature mismatch! Should be Beef001a but is 0x{Signature:X}");
            }

            var len = 0;
            var index = 10;

            // Step a whole UTF-16 code unit at a time so a 0x00 high byte followed by a 0x00 low byte isn't taken as the terminator
            while ((rawBytes[index + len] != 0x0 || rawBytes[index + len + 1] != 0x0))
            {
                len += 2;
            }

            var uname = Encoding.Unicode.GetString(rawBytes, index, len);

            FileDocumentTypeString = uname;

            index += len + 2; // move past string and end of string marker

            //is index 24?

            //TODO get shell item list

//            if (index != 38)
//            {
//                Message =
//                    "Unsupported Extension block. Please report to Report to saericzimmerman@gmail.com to get it added!";
//            }
            
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