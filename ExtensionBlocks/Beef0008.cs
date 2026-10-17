using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0008 : BeefBase
    {
        public Beef0008(byte[] rawBytes)
            : base(rawBytes)
        {
            var index = 0x10;

            Timestamp = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index));
            index += 8;

            index += 4;

            var stringRaw = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index - 2);
            var strings = stringRaw.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);

            OriginalPath = strings[0];
            RecycleBinPath = strings[1];
        }

        public string RecycleBinPath { get; }

        public string OriginalPath { get; }

        public DateTimeOffset? Timestamp { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            if (Timestamp.HasValue)
            {
                sb.AppendLine();

                sb.AppendLine(
                    $"Timestamp: {Timestamp.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            sb.AppendLine($"Original path: {OriginalPath}");
            sb.AppendLine($"Recycle Bin path {RecycleBinPath}");

            return sb.ToString();
        }
    }
}