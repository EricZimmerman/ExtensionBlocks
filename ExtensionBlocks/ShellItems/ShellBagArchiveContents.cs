using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    /// <summary>
    ///     A folder inside an archive opened with the archive support of Windows 11 (zip, 7z, rar, tar...). Bytes 2-3
    ///     vary from item to item (not a type byte: these came in as "unknown TypeID" with every value), so it is found by
    ///     its layout (<see cref="IsArchiveContents" />):
    ///     0x00 size, 0x02 ?, 0x04 file attributes, 0x08 file size, 0x10 modified (FILETIME), 0x18 ?,
    ///     0x1C name length, 0x1E parent path length (characters, with its trailing \), 0x20 parent path + name (UTF-16), null
    /// </summary>
    public class ShellBagArchiveContents : ShellBag
    {
        public ShellBagArchiveContents(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Archive contents";
            ShellBagType = Utils.ShellBagTypes.ZipFileContents;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            FileAttributes = BitConverter.ToUInt32(rawBytes, 0x04);
            FileSize = BitConverter.ToUInt64(rawBytes, 0x08);

            var modified = BitConverter.ToInt64(rawBytes, 0x10);
            if (modified > 0)
            {
                ModifiedOn = Utils.FromFileTime(modified).ToUniversalTime();
            }

            var nameLength = BitConverter.ToUInt16(rawBytes, 0x1C);
            var parentLength = BitConverter.ToUInt16(rawBytes, 0x1E);

            ArchivePath = Encoding.Unicode.GetString(rawBytes, 0x20, (nameLength + parentLength) * 2);

            // the parent folders are bags of their own: the value is the name only
            Value = ArchivePath.Substring(parentLength);
        }

        public uint FileAttributes { get; }
        public ulong FileSize { get; }
        public DateTimeOffset? ModifiedOn { get; }

        /// <summary>The path in the archive, the name included</summary>
        public string ArchivePath { get; }

        /// <summary>The layout of <see cref="ShellBagArchiveContents" />, whatever bytes 2-3 are</summary>
        public static bool IsArchiveContents(byte[] rawBytes)
        {
            if (rawBytes.Length < 0x24)
            {
                return false;
            }

            var size = BitConverter.ToUInt16(rawBytes, 0);
            var attributes = BitConverter.ToUInt32(rawBytes, 0x04);
            if (size > rawBytes.Length || (attributes & 0x10) == 0 || attributes > 0xFFFF)
            {
                return false;
            }

            var nameLength = BitConverter.ToUInt16(rawBytes, 0x1C);
            var parentLength = BitConverter.ToUInt16(rawBytes, 0x1E);
            var end = 0x20 + (nameLength + parentLength) * 2;
            if (nameLength == 0 || end + 2 > size || BitConverter.ToUInt16(rawBytes, end) != 0)
            {
                return false;
            }

            for (var i = 0x20; i < end; i += 2)
            {
                if (BitConverter.ToUInt16(rawBytes, i) == 0)
                {
                    return false;
                }
            }

            // the parent path ends with a backslash
            if (parentLength > 0 && BitConverter.ToUInt16(rawBytes, 0x20 + (parentLength - 1) * 2) != '\\')
            {
                return false;
            }

            var modified = BitConverter.ToInt64(rawBytes, 0x10);
            return modified == 0 || Utils.FromFileTime(modified) != DateTimeOffset.MinValue;
        }

        public override ShellBagView GetShellBagView()
        {
            var view = base.GetShellBagView();
            view.ModifiedOn = ModifiedOn;
            view.Miscellaneous = $"Archive path: {ArchivePath}";
            return view;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Archive path: {ArchivePath}");
            sb.AppendLine($"File attributes: 0x{FileAttributes:X}");

            if (FileSize > 0)
            {
                sb.AppendLine($"File size: {FileSize:N0}");
            }

            if (ModifiedOn.HasValue)
            {
                sb.AppendLine(
                    $"Modified On: {ModifiedOn.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            sb.AppendLine();
            sb.AppendLine(base.ToString());
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
