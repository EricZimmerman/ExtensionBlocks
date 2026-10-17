using System;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0025 : BeefBase
    {
        public Beef0025(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0025)
            {
                throw new Exception($"Signature mismatch! Should be Beef0025 but is 0x{Signature:X}");
            }

            var ft1 = Utils.FromFileTime((long) BitConverter.ToUInt64(rawBytes, 12)).ToUniversalTime();

            FileTime1 = ft1.ToUniversalTime();

            FileTime1 = TimeZoneInfo.ConvertTime(FileTime1.Value, Utils.TimeZone);

            var ft2 = Utils.FromFileTime((long) BitConverter.ToUInt64(rawBytes, 20)).ToUniversalTime();

            FileTime2 = ft2.ToUniversalTime();

            FileTime2 = TimeZoneInfo.ConvertTime(FileTime2.Value, Utils.TimeZone);

            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 4);
        }

        public DateTimeOffset? FileTime1 { get; }

        public DateTimeOffset? FileTime2 { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());
            sb.AppendLine();

            if (FileTime1.HasValue)
            {
                sb.AppendLine(
                    $"FileTime 1: {FileTime1.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (FileTime2.HasValue)
            {
                sb.AppendLine(
                    $"FileTime 2: {FileTime2.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            return sb.ToString();
        }
    }
}