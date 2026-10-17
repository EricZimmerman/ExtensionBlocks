using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X02 : ShellBag
    {
        public ShellBag0X02(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Hyper-V Directory";
            ShellBagType = Utils.ShellBagTypes.HyperVResource;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 2;

            index += 1; //skip type

            //skip unknown bytes
            index += 15;

            index += 4; // skip unknown

            var ts = BitConverter.ToInt64(rawBytes, index);

            CreatedOnTime = Utils.FromFileTime(ts).ToUniversalTime();
            index += 8;

            ts = BitConverter.ToInt64(rawBytes, index);
            LastAccessTime = Utils.FromFileTime(ts).ToUniversalTime();

            if (LastAccessTime.Value.Year == 1601)
            {
                LastAccessTime = null;
            }

            index += 8;
            ts = BitConverter.ToInt64(rawBytes, index);

            try
            {
                LastModificationTime = Utils.FromFileTime(ts).ToUniversalTime();
            }
            catch (Exception)
            {
                }
            

            index += 8;

            index += 4; //unknown

            Value = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index);

            Value = Value.Replace('\0', ' ').Trim();
        }

        /// <summary>
        ///     last modified time of BagPath
        /// </summary>
        public DateTimeOffset? LastModificationTime { get; set; }

        /// <summary>
        ///     Created time of BagPath
        /// </summary>
        public DateTimeOffset? CreatedOnTime { get; set; }

        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        public string ShortName { get; private set; }

        public override ShellBagView GetShellBagView()
        {
            if (MruPosition == 0 && LastWriteTime.HasValue)
            {
                LastInteracted = LastWriteTime.Value;
            }

            const string misc = "Network/special item";

            return new ShellBagView
            {
                BagPath = BagPath,
                ChildBags = ChildShellBags.Count,
                Value = Value,
                Slot = Slot,
                AbsolutePath = AbsolutePath,
                MRUPosition = MruPosition,
                HexValue = HexValue,
                NodeSlot = NodeSlot,
                InternalID = InternalId,
                ShellType = FriendlyName,
                AccessedOn = LastAccessTime,
                CreatedOn = CreatedOnTime,
                LastWriteTime = LastWriteTime,
                MFTEntry = null,
                MFTSequenceNumber = null,
                ModifiedOn = LastModificationTime,
                FirstInteracted = FirstInteracted,
                LastInteracted = LastInteracted,
                Miscellaneous = misc
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (CreatedOnTime.HasValue)
            {
                sb.AppendLine(
                    $"Created On: {CreatedOnTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (LastModificationTime.HasValue)
            {
                sb.AppendLine(
                    $"Modified: {LastModificationTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (LastAccessTime.HasValue)
            {
                sb.AppendLine(
                    $"Last Access: {LastAccessTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            sb.AppendLine();
            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}