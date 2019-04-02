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
                throw new Exception($"Signature mismatch! Should be Beef0026 but is {Signature}");
            }


            if (rawBytes[8] == 0x11 || rawBytes[8] == 0x10|| rawBytes[8] == 0x12 || rawBytes[8] == 0x34 || rawBytes[8] == 0x31)
            {
                var ft1 = DateTimeOffset.FromFileTime((long) BitConverter.ToUInt64(rawBytes, 12)).ToUniversalTime();

                CreatedOn = ft1.ToUniversalTime();


                var ft2 = DateTimeOffset.FromFileTime((long) BitConverter.ToUInt64(rawBytes, 20)).ToUniversalTime();

                LastModified = ft2.ToUniversalTime();


                var ft3 = DateTimeOffset.FromFileTime((long) BitConverter.ToUInt64(rawBytes, 28)).ToUniversalTime();

                LastAccessed = ft3.ToUniversalTime();


                return;
            }

            var shellPropertySheetListSize = BitConverter.ToUInt16(rawBytes, 8);
            var propBytes = rawBytes.Skip(8).Take(shellPropertySheetListSize).ToArray();
            PropertyStore = new PropertyStore(propBytes);

            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 4);
        }

        public PropertyStore PropertyStore { get; }

        public DateTimeOffset? CreatedOn { get; }

        public DateTimeOffset? LastModified { get; }
        public DateTimeOffset? LastAccessed { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());
            sb.AppendLine();

            if (CreatedOn.HasValue)
            {
                sb.AppendLine(
                    $"Created: {CreatedOn.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}");
            }

            if (LastModified.HasValue)
            {
                sb.AppendLine(
                    $"Last modified: {LastModified.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}");
            }

            if (LastAccessed.HasValue)
            {
                sb.AppendLine(
                    $"Last accessed: {LastAccessed.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}");
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