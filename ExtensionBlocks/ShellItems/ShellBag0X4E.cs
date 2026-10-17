using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X4E : ShellBag
    {
        public ShellBag0X4E(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            ProcessGuid(rawBytes);
        }

        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        public PropertyStore PropertyStore { get; private set; }

        private void ProcessGuid(byte[] rawBytes)
        {
            FriendlyName = "Root folder: GUID";
            ShellBagType = Utils.ShellBagTypes.Guid;

            //TODO split this out or at least use a different icon?

            var index = 2;

            index += 2; // move past index and a single unknown value

            var rawguid1 = new byte[16];

            Array.Copy(rawBytes, index, rawguid1, 0, 16);

            var rawguid = Utils.ExtractGuidFromShellItem(rawguid1);

            var foldername = GuidMapping.GuidMapping.GetDescriptionFromGuid(rawguid);

            index += 16;

            var size = BitConverter.ToInt16(rawBytes, index);
            if (size == 0)
            {
                index += 2;
            }

            //TODO this should be the looping cut up thing

            if (index < rawBytes.Length)
            {
                var signature = BitConverter.ToUInt32(rawBytes, index + 4);

                var block = Utils.GetExtensionBlockFromBytesOrError(signature, rawBytes.Skip(index).ToArray());

                ExtensionBlocks.Add(block);

                index += size;
            }

            Value = foldername;
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            if (PropertyStore?.Sheets.Count > 0)
            {
                sb.AppendLine("Property Sheets");

                sb.AppendLine(PropertyStore.ToString());
            }

            if (LastAccessTime.HasValue)
            {
                sb.AppendLine(
                    $"Accessed On: {LastAccessTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
                sb.AppendLine();
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}