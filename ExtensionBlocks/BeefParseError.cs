using System.Text;

namespace ExtensionBlocks
{
    /// <summary>
    ///     An extension block that could not be parsed (damaged data). The shell item it is in is kept, with this in its
    ///     list of extension blocks
    /// </summary>
    public class BeefParseError : BeefBase
    {
        public BeefParseError(byte[] rawBytes, string error)
            : base(rawBytes != null && rawBytes.Length >= 8 ? rawBytes : null)
        {
            RawBytes = rawBytes;
            Message = $"Extension block could not be parsed: {error}";
        }

        public byte[] RawBytes { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}
