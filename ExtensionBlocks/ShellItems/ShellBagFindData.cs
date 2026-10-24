using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    /// <summary>
    ///     A folder (or file) item with GUID 1ca7ea69-e26a-44c9-9182-a9128722c66f at offset 4 and the item's find data
    ///     (as WIN32_FIND_DATA) twice. Seen with OneDrive folders (OneDrive, Camera Roll, ...) of 2015-2018; which program
    ///     writes it is not known. Bytes 2-3 vary (they came in as unknown items of most type bytes, and 0x1F ones as an
    ///     unmapped GUID), so it is found by its layout (<see cref="IsFindData" />):
    ///     0x00 size, 0x02 ?, 0x04 GUID, 0x14 name length, 0x16 second name length (characters), 0x18 name offset (0x68),
    ///     0x1A second name offset, 0x1C count (1), 0x20 and 0x44 find data (attributes, created, accessed, modified
    ///     FILETIMEs, size: 36 bytes), 0x68 name (UTF-16, null), second name (UTF-16, null), 4 bytes
    /// </summary>
    public class ShellBagFindData : ShellBag
    {
        public static readonly Guid ItemGuid = new Guid("1ca7ea69-e26a-44c9-9182-a9128722c66f");

        public ShellBagFindData(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            FileAttributes = BitConverter.ToUInt32(rawBytes, 0x20);

            if ((FileAttributes & 0x10) != 0)
            {
                FriendlyName = "Directory";
                ShellBagType = Utils.ShellBagTypes.Directory;
            }
            else
            {
                FriendlyName = "File";
                ShellBagType = Utils.ShellBagTypes.File;
            }

            CreatedOn = Time(rawBytes, 0x24);
            AccessedOn = Time(rawBytes, 0x2C);
            ModifiedOn = Time(rawBytes, 0x34);
            FileSize = BitConverter.ToUInt64(rawBytes, 0x3C);

            var nameLength = BitConverter.ToUInt16(rawBytes, 0x14);
            var secondLength = BitConverter.ToUInt16(rawBytes, 0x16);

            Value = Encoding.Unicode.GetString(rawBytes, BitConverter.ToUInt16(rawBytes, 0x18), nameLength * 2);
            SecondName = Encoding.Unicode.GetString(rawBytes, BitConverter.ToUInt16(rawBytes, 0x1A), secondLength * 2);
        }

        public uint FileAttributes { get; }
        public ulong FileSize { get; }
        public DateTimeOffset? CreatedOn { get; }
        public DateTimeOffset? AccessedOn { get; }
        public DateTimeOffset? ModifiedOn { get; }

        /// <summary>The second name (the same as the name, except for some OneDrive folders)</summary>
        public string SecondName { get; }

        private static DateTimeOffset? Time(byte[] rawBytes, int offset)
        {
            var value = BitConverter.ToInt64(rawBytes, offset);
            if (value <= 0)
            {
                return null;
            }

            var time = Utils.FromFileTime(value);
            return time == DateTimeOffset.MinValue ? (DateTimeOffset?) null : TimeZoneInfo.ConvertTime(time, Utils.TimeZone);
        }

        /// <summary>The layout of <see cref="ShellBagFindData" />, whatever bytes 2-3 are</summary>
        public static bool IsFindData(byte[] rawBytes)
        {
            if (rawBytes.Length < 0x6C)
            {
                return false;
            }

            var size = BitConverter.ToUInt16(rawBytes, 0);
            if (size < 0x6C || size > rawBytes.Length)
            {
                return false;
            }

            var guid = new byte[16];
            Buffer.BlockCopy(rawBytes, 4, guid, 0, 16);
            if (new Guid(guid) != ItemGuid)
            {
                return false;
            }

            var nameLength = BitConverter.ToUInt16(rawBytes, 0x14);
            var secondLength = BitConverter.ToUInt16(rawBytes, 0x16);
            var nameOffset = BitConverter.ToUInt16(rawBytes, 0x18);
            var secondOffset = BitConverter.ToUInt16(rawBytes, 0x1A);

            if (nameLength == 0 || nameOffset < 0x68 || secondOffset < nameOffset + (nameLength + 1) * 2)
            {
                return false;
            }

            return IsString(rawBytes, nameOffset, nameLength, size) && IsString(rawBytes, secondOffset, secondLength, size);
        }

        /// <summary>length UTF-16 characters, none of them null, and a null after them, within size</summary>
        private static bool IsString(byte[] rawBytes, int offset, int length, int size)
        {
            var end = offset + length * 2;
            if (end + 2 > size || BitConverter.ToUInt16(rawBytes, end) != 0)
            {
                return false;
            }

            for (var i = offset; i < end; i += 2)
            {
                if (BitConverter.ToUInt16(rawBytes, i) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public override ShellBagView GetShellBagView()
        {
            var view = base.GetShellBagView();
            view.CreatedOn = CreatedOn;
            view.AccessedOn = AccessedOn;
            view.ModifiedOn = ModifiedOn;
            view.Miscellaneous = SecondName != Value ? $"Item GUID {ItemGuid}, second name: {SecondName}" : $"Item GUID {ItemGuid}";
            return view;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (SecondName != Value)
            {
                sb.AppendLine($"Second name: {SecondName}");
            }

            sb.AppendLine($"Item GUID: {ItemGuid}");
            sb.AppendLine($"File attributes: 0x{FileAttributes:X}");

            if (FileSize > 0)
            {
                sb.AppendLine($"File size: {FileSize:N0}");
            }

            if (CreatedOn.HasValue)
            {
                sb.AppendLine($"Created On: {CreatedOn.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (ModifiedOn.HasValue)
            {
                sb.AppendLine($"Modified On: {ModifiedOn.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (AccessedOn.HasValue)
            {
                sb.AppendLine($"Accessed On: {AccessedOn.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            sb.AppendLine();
            sb.AppendLine(base.ToString());
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
