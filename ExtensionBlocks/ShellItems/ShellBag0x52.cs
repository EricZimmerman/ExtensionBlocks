using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X52 : ShellBag
    {
        public ShellBag0X52(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            var index = 0;

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            FriendlyName = "Acronis";
            ShellBagType = Utils.ShellBagTypes.Backup;

            index = 46;

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


            var stringlen = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            var stringValue = Encoding.Unicode.GetString(rawBytes, index, stringlen * 2 - 2);

            


            Value = stringValue;
        }

        DateTimeOffset? ModifiedOn;

        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (LastAccessTime.HasValue)
            {
                sb.AppendLine(
                    $"Accessed On: {LastAccessTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");

                sb.AppendLine();
            }

            if (ModifiedOn.HasValue)
            {
                sb.AppendLine(
                    $"Modified On: {ModifiedOn.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");

                sb.AppendLine();
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}