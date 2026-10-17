using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X61 : ShellBag
    {
        // Fields...


        public ShellBag0X61(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "URI";
            ShellBagType = Utils.ShellBagTypes.FtpUri;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 2; //past size

            if (rawBytes[4] == 0x10)
            {
                FriendlyName = "ZipFileContents";
                ShellBagType = Utils.ShellBagTypes.ZipFileContents;
                
                index = 16;
                ModifiedOn = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index)).ToUniversalTime();


                index = 28;

                var len1 = BitConverter.ToInt16(rawBytes, index);
                var len2 = BitConverter.ToInt16(rawBytes, index+2);
                
                index = 32;

                var name = Encoding.Unicode.GetString(rawBytes, index, (len1+len2)*2).Replace("\0", "");
                Value = name;
                return;
            }

            index += 1; // move past type and a single unknown value

            var flags = rawBytes[index];
            index += 1;

            var dataSize = BitConverter.ToUInt16(rawBytes, index);


            if (dataSize > 0)
            {
                index += 2; // past size
                index += 4; // skip unknown
                index += 4; // skip unknown

                var ft1 = Utils.FromFileTime((long) BitConverter.ToUInt64(rawBytes, index)).ToUniversalTime();

                FileTime1 = ft1.ToUniversalTime();

                FileTime1 = TimeZoneInfo.ConvertTime(FileTime1.Value, Utils.TimeZone);

                index += 8;
                index += 4; // skip unknown FF FF FF FF
                index += 12; // skip unknown 12 0's
                index += 4; // skip unknown

                var strSize = BitConverter.ToUInt32(rawBytes, index);
                index += 4;

                var str = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, (int) strSize);

                Value = str.Replace("\0", "");

                index += (int) strSize;

                strSize = BitConverter.ToUInt32(rawBytes, index);
                index += 4;

                if (strSize > 0)
                {
                    str = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, (int) strSize);

                    UserName = str.Replace("\0", "");
                    ;
                    index += (int) strSize;
                }

                strSize = BitConverter.ToUInt32(rawBytes, index);
                index += 4;


                if (strSize > 0)
                {
                    str = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, (int) strSize);
                    index += (int) strSize;
                }

                var len1 = 0;

                while (rawBytes[index + len1] != 0x00)
                {
                    len1 += 1;
                }

                URI = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, len1);


                index += len1 + 1;
            }
            else
            {
                var uri = Encoding.Unicode.GetString(rawBytes, 0x8, rawBytes.Length - 0x8).Trim('\0');
                URI = uri;
                //the URI is also the item's name (it was left empty; as Lnk does)
                Value = uri.Split('\0')[0];
                index = rawBytes.Length - 2;
            }
            dataSize = BitConverter.ToUInt16(rawBytes, index);
            index += 2;


            //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
            //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
        }

        public DateTimeOffset? FileTime1 { get; }
        public DateTimeOffset? ModifiedOn { get; }

        public string URI { get; set; }


        public string UserName { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"URI: {URI}");
            sb.AppendLine();

            if (ModifiedOn.HasValue)
            {
                sb.AppendLine(
                    $"Modified On: {ModifiedOn.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");

                sb.AppendLine();
            }


            if (FileTime1.HasValue)
            {
                sb.AppendLine(
                    $"Connect time: {FileTime1.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
                sb.AppendLine();
            }

            if (UserName?.Length > 0)
            {
                sb.AppendLine($"Username: {UserName}");
                sb.AppendLine();
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}