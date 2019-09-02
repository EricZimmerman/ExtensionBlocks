using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public abstract class ShellBag : IShellBag
    {
        public static bool ShowHexInString { get; set; }
        public string InternalId { get; set; }

        public string FriendlyName { get; set; }

        public byte[] HexValue { get; set; }

        public string BagPath { get; set; }

        public virtual string AbsolutePath { get; set; }

        public int Slot { get; set; }

        public bool IsDeleted { get; set; }

        public int MruPosition { get; set; }
        public int NodeSlot { get; set; }

        public List<IShellBag> ChildShellBags { get; set; }

        public string Value { get; set; }

        public DateTimeOffset? LastWriteTime { get; set; }

        public DateTimeOffset? FirstInteracted { get; set; }
        public bool HasExplored { get; set; }

        public DateTimeOffset? LastInteracted { get; set; }

        public Utils.ShellBagTypes ShellBagType { get; set; }


        public List<IExtensionBlock> ExtensionBlocks { get; set; }

        public int NodeId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Value: {Value}");
            sb.AppendLine($"Shell Type: {FriendlyName}");

            sb.AppendLine();

            if (BagPath.Length > 0)
            {
                sb.AppendLine($"Bag Path: {BagPath}, Slot #: {Slot}, MRU Position: {MruPosition}, Node Slot: {NodeSlot}");
                sb.AppendLine($"Absolute Path: {AbsolutePath}");
                sb.AppendLine();
            }

            sb.AppendLine($"Has been explored: {HasExplored}");
            sb.AppendLine();

            if (IsDeleted)
            {
                sb.AppendLine("Deleted: True");
                sb.AppendLine();
            }

            sb.AppendLine($"# Child Bags: {ChildShellBags.Count}");

            if (FirstInteracted.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine(
                    $"First interacted: {FirstInteracted.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (LastInteracted.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine(
                    $"Last interacted: {LastInteracted.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (ExtensionBlocks.Count > 0)
            {
                var extensionNumber = 0;

                sb.AppendLine();
                sb.AppendLine($"Extension blocks found: {ExtensionBlocks.Count}");

                foreach (var extensionBlock in ExtensionBlocks)
                {
                    if (extensionBlock is BeefPlaceHolder)
                    {
                        continue;
                    }

                    sb.AppendLine($"---------------------- Block {extensionNumber:N0} ----------------------");

                    sb.AppendLine(extensionBlock.ToString());

                    extensionNumber += 1;
                }

                sb.AppendLine("--------------------------------------------------");
            }

            if (LastWriteTime.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine(
                    $"Last Write Time: {LastWriteTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (ShowHexInString)
            {
                sb.AppendLine($"\r\nHex Value: {BitConverter.ToString(HexValue)}");
            }

            return sb.ToString();
        }
    }
}