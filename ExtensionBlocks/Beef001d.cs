using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef001d : BeefBase
    {
        public Beef001d(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef001d)
            {
                throw new Exception($"Signature mismatch! Should be Beef001d but is 0x{Signature:X}");
            }

            var index = 10;

        
            var uname = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length- 10 - 2).Trim('\0');

            Executable = uname;

            
            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length-2);
        }

        public string Executable { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            sb.AppendLine($"Executable: {Executable}");

            return sb.ToString();
        }
    }
}