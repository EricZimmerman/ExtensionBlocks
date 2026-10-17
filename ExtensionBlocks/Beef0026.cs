using System;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0026 : BeefBase
    {
        public Beef0026(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0026)
            {
                throw new Exception($"Signature mismatch! Should be Beef0026 but is 0x{Signature:X}");
            }


            if (rawBytes[8] == 0x11 || rawBytes[8] == 0x10 || rawBytes[8] == 0x12 || rawBytes[8] == 0x34 || rawBytes[8] == 0x31)
            {
                var ft1 = Utils.FromFileTime((long) BitConverter.ToUInt64(rawBytes, 12)).ToUniversalTime();

                CreatedOn = ft1.ToUniversalTime();

                CreatedOn = TimeZoneInfo.ConvertTime(CreatedOn.Value, Utils.TimeZone);

                var ft2 = Utils.FromFileTime((long) BitConverter.ToUInt64(rawBytes, 20)).ToUniversalTime();

                LastModified = ft2.ToUniversalTime();

                LastModified = TimeZoneInfo.ConvertTime(LastModified.Value, Utils.TimeZone);

                var ft3 = Utils.FromFileTime((long) BitConverter.ToUInt64(rawBytes, 28)).ToUniversalTime();

                LastAccessed = ft3.ToUniversalTime();

                LastAccessed = TimeZoneInfo.ConvertTime(LastAccessed.Value, Utils.TimeZone);

                return;
            }

            var shellPropertySheetListSize = BitConverter.ToUInt16(rawBytes, 8);
            if (shellPropertySheetListSize > rawBytes.Length - 8)
            {
                return;
            }

            var propBytes = rawBytes.Skip(8).Take(shellPropertySheetListSize).ToArray();
            PropertyStore = new PropertyStore(propBytes);

            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 4);
        }

        public PropertyStore PropertyStore { get; }

        public DateTimeOffset? CreatedOn { get; }

        public DateTimeOffset? LastModified { get; }
        public DateTimeOffset? LastAccessed { get; }

        //the format this block has always used, unless one is set

        private static string TimeFormat => Utils.DateTimeFormat == null ? "yyyy-MM-dd HH:mm:ss.fffffff" : Utils.GetDateTimeFormatWithMilliseconds();


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());
            sb.AppendLine();

            if (CreatedOn.HasValue)
            {
                sb.AppendLine(
                    $"Created: {CreatedOn.Value.ToString(TimeFormat)}");
            }

            if (LastModified.HasValue)
            {
                sb.AppendLine(
                    $"Last modified: {LastModified.Value.ToString(TimeFormat)}");
            }

            if (LastAccessed.HasValue)
            {
                sb.AppendLine(
                    $"Last accessed: {LastAccessed.Value.ToString(TimeFormat)}");
            }

            if (PropertyStore != null)
            {
                if (PropertyStore.Sheets.Count > 0)
                {
                    sb.AppendLine("Property Sheets");

                    sb.AppendLine(PropertyStore.ToString());
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}