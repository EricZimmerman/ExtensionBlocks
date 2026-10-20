using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    /// <summary>
    ///     A folder (type 0xFF) or file (0xFC) of a document drive namespace extension (vendor unknown: Korean folder names,
    ///     roots like TB_ROOT, R_ROOT, MY_OWNED_ROOT_ID), 0x55 at offset 3 (<see cref="IsDocumentDrive" />).
    ///     Folder: 0x13, 0x1B, 0x23 FILETIMEs, 0x79 item ID, 0xA5 parent ID, 0xD1 5, 0xD2 name (UTF-16, null terminated).
    ///     File: 0x09, 0x11, 0x19 FILETIMEs, 0x29 name.
    ///     The first FILETIME is the oldest: taken as created, then modified and accessed.
    /// </summary>
    public class ShellBagDocumentDrive : ShellBag
    {
        public ShellBagDocumentDrive(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            int timeOffset;
            if (rawBytes[2] == 0xFF)
            {
                FriendlyName = "Document drive folder";
                ShellBagType = Utils.ShellBagTypes.Directory;
                timeOffset = 0x13;

                ItemId = ReadString(rawBytes, 0x79);
                ParentId = ReadString(rawBytes, 0xA5);
                Value = ReadString(rawBytes, 0xD2);
            }
            else
            {
                FriendlyName = "Document drive file";
                ShellBagType = Utils.ShellBagTypes.File;
                timeOffset = 0x09;

                Value = ReadString(rawBytes, 0x29);
            }

            CreatedOn = ReadFileTime(rawBytes, timeOffset);
            ModifiedOn = ReadFileTime(rawBytes, timeOffset + 8);
            AccessedOn = ReadFileTime(rawBytes, timeOffset + 16);
        }

        public string ItemId { get; }
        public string ParentId { get; }

        public DateTimeOffset? CreatedOn { get; }
        public DateTimeOffset? ModifiedOn { get; }
        public DateTimeOffset? AccessedOn { get; }

        /// <summary>0x55 at offset 3, with type 0xFF (folder, its name after a 5 at 0xD1) or 0xFC (file)</summary>
        public static bool IsDocumentDrive(byte[] rawBytes)
        {
            if (rawBytes.Length < 0x2C || rawBytes[3] != 0x55)
            {
                return false;
            }

            return rawBytes[2] == 0xFC || (rawBytes[2] == 0xFF && rawBytes.Length > 0xD4 && rawBytes[0xD1] == 5);
        }

        private static DateTimeOffset? ReadFileTime(byte[] rawBytes, int offset)
        {
            var value = BitConverter.ToInt64(rawBytes, offset);
            if (value <= 0)
            {
                return null;
            }

            var time = Utils.FromFileTime(value);
            return time == DateTimeOffset.MinValue ? null : time.ToUniversalTime();
        }

        /// <summary>A null terminated UTF-16 string (not aligned), or the rest of the item</summary>
        private static string ReadString(byte[] rawBytes, int offset)
        {
            var end = offset;
            while (end + 1 < rawBytes.Length && (rawBytes[end] != 0 || rawBytes[end + 1] != 0))
            {
                end += 2;
            }

            return Encoding.Unicode.GetString(rawBytes, offset, Math.Min(end, rawBytes.Length) - offset);
        }

        public override ShellBagView GetShellBagView()
        {
            var view = base.GetShellBagView();
            view.CreatedOn = CreatedOn;
            view.ModifiedOn = ModifiedOn;
            view.AccessedOn = AccessedOn;
            view.Miscellaneous = ItemId == null ? "" : $"Item ID: {ItemId}, Parent ID: {ParentId}";
            return view;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ItemId != null)
            {
                sb.AppendLine($"Item ID: {ItemId}");
                sb.AppendLine($"Parent ID: {ParentId}");
                sb.AppendLine();
            }

            var format = Utils.GetDateTimeFormatWithMilliseconds();
            if (CreatedOn.HasValue)
            {
                sb.AppendLine($"Created On: {CreatedOn.Value.ToString(format)}");
            }

            if (ModifiedOn.HasValue)
            {
                sb.AppendLine($"Modified On: {ModifiedOn.Value.ToString(format)}");
            }

            if (AccessedOn.HasValue)
            {
                sb.AppendLine($"Accessed On: {AccessedOn.Value.ToString(format)}");
            }

            sb.AppendLine();
            sb.AppendLine(base.ToString());
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
