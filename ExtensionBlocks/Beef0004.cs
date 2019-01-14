using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0004 : BeefBase
    {
        public Beef0004(byte[] rawBytes)
            : base(rawBytes)
        {
            LocalisedName = string.Empty;

            MFTInformation = new MFTInformation();

            if (Signature != 0xbeef0004)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0004 but is 0x{Signature:X}");
            }

            var createdDate =
                Utils.ExtractDateTimeOffsetFromBytes(rawBytes.Skip(8).Take(4).ToArray());

            CreatedOnTime = createdDate;

            var lastAccessDate =
                Utils.ExtractDateTimeOffsetFromBytes(rawBytes.Skip(12).Take(4).ToArray());

            LastAccessTime = lastAccessDate;

            Identifier = BitConverter.ToInt16(rawBytes, 16);

            var index = 18;

            if (Version >= 7)
            {
                index += 2; // skip empty 2

                MFTInformation = new MFTInformation(rawBytes.Skip(index).Take(8).ToArray());

                index += 8; //skip mft data

                // skip 8 unknown
                index += 8;
            }

            var longstringsize = 0;

            if (Version >= 3)
            {
                index += 2;
            }

            if (Version >= 9)
            {
                //skip 4 unknown
                index += 4;
            }

            if (Version >= 8)
            {
                // unknown, but skip 4
                index += 4;
            }

            // in this case we want the rest of the extension data, but again, we arent interested in the version offset yet
            longstringsize = rawBytes.Length - index;

            var stringBytes = rawBytes.Skip(index).Take(longstringsize).ToArray();

            var stringpieces =
                Utils.GetStringsFromMultistring(stringBytes);

            LongName = stringpieces[0];

            if (stringpieces.Count > 1)
            {
                LocalisedName = stringpieces[1];
            }

            index += longstringsize - 2;

            if (index > rawBytes.Length)
            {
                index = rawBytes.Length - 2;
            }

            VersionOffset = BitConverter.ToUInt16(rawBytes, index);

            index += 2;

#if DEBUG
            Trace.Assert(rawBytes.Length == index, "Still have bytes in beef0004");
#endif
        }

        /// <summary>
        ///     Created time of BagPath
        /// </summary>
        public DateTimeOffset? CreatedOnTime { get; }

        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; }

        public int Identifier { get; set; }

        public MFTInformation MFTInformation { get; }

        public string LongName { get; }

        public string LocalisedName { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            var os = "Unknown operating system";
            switch (Identifier)
            {
                case 0x14:
                    os = "Windows XP, 2003";
                    break;
                case 0x26:
                    os = "Windows Vista";
                    break;
                case 0x2a:
                    os = "Windows 2008, 7, 8";
                    break;
                case 0x2e:
                    os = "Windows 8.1, 10";
                    break;
                
            }

            sb.AppendLine($"Identifier: {Identifier:X2} ({os})");

            if (CreatedOnTime.HasValue)
            {
                sb.AppendLine();

                sb.AppendLine($"Created On: {CreatedOnTime.Value}");
            }

            if (LastAccessTime.HasValue)
            {
                sb.AppendLine($"Last Access: {LastAccessTime.Value}");
            }

            sb.AppendLine();

            sb.AppendLine($"Long Name: {LongName}");

            if (LocalisedName.Length > 0)
            {
                sb.AppendLine($"Localised Name: {LocalisedName}");
            }

            if (MFTInformation.MFTEntryNumber.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine($"MFT Entry Number: {MFTInformation.MFTEntryNumber.Value}");
            }

            if (MFTInformation.MFTSequenceNumber.HasValue)
            {
                sb.AppendLine($"MFT Sequence Number: {MFTInformation.MFTSequenceNumber.Value}");
            }

            sb.AppendLine();


            if (MFTInformation.MFTEntryNumber.HasValue && MFTInformation.MFTSequenceNumber.HasValue)
            {
                if (MFTInformation.MFTEntryNumber.Value > 0 && MFTInformation.MFTSequenceNumber.Value > 0)
                {
                    MFTInformation.Note = "NTFS";
                }
            }

            if (MFTInformation.MFTEntryNumber.HasValue && MFTInformation.MFTSequenceNumber.HasValue == false)
            {
                if (LastAccessTime.HasValue)
                {
                    if (LastAccessTime.Value.Minute == 0 && LastAccessTime.Value.Second == 0 &&
                        LastAccessTime.Value.Millisecond == 0)
                    {
                        MFTInformation.Note = "FAT";
                    }
                    else
                    {
                        MFTInformation.Note = "exFAT";
                    }
                }
            }


            sb.AppendLine($"File system hint: {MFTInformation.Note}");

            return sb.ToString();
        }
    }
}