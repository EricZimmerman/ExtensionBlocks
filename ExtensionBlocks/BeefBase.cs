using System;
using System.Text;

namespace ExtensionBlocks
{
    public abstract class BeefBase : IExtensionBlock
    {
        protected BeefBase(byte[] rawBytes)
        {
            if (rawBytes == null)
            {
                return;
            }

            Size = BitConverter.ToUInt16(rawBytes, 0);

            Version = BitConverter.ToUInt16(rawBytes, 2);

            Signature = BitConverter.ToUInt32(rawBytes, 4);

            Message = "";
        }

        public string Message { get; set; }
        public int Size { get; }

        public int Version { get; }

        public uint Signature { get; }

        public int VersionOffset { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Signature: 0x{Signature:x8}");
            sb.AppendLine($"Size: {Size:N0}");
            sb.AppendLine($"Version: {Version:N0}");
            sb.AppendLine($"Version Offset: 0x{VersionOffset:X2}");

            if (Message.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"Message: {Message}");
            }

            return sb.ToString();
        }
    }
}