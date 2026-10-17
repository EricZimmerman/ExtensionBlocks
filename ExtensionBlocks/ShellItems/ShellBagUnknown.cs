using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBagUnknown : IShellBag
    {
        public ShellBagUnknown(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "**** Unknown";
            ShellBagType = Utils.ShellBagTypes.Unknown;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            ExtensionBlocks = new List<IExtensionBlock>();

            HexValue = rawBytes;

            AbsolutePath = string.Empty;

            BagPath = bagPath;

            Value = "**** Unknown";

            Value =
                $"**** Unknown type! ShellBag ID: {rawBytes[2].ToString("x")}, Hex: {BitConverter.ToString(rawBytes)}";
        }

        public List<IExtensionBlock> ExtensionBlocks { get; set; }

        public string InternalId { get; }

        public string FriendlyName { get; }

        public byte[] HexValue { get; }

        public string BagPath { get; }

        public string AbsolutePath { get; set; }

        public Utils.ShellBagTypes ShellBagType { get; set; }

        public int Slot { get; }

        public bool IsDeleted { get; set; }


        public int MruPosition { get; }
        public int NodeSlot { get; set; }

        public List<IShellBag> ChildShellBags { get; set; }

        public string Value { get; }

        public DateTimeOffset? LastWriteTime { get; set; }

        public DateTimeOffset? FirstInteracted { get; set; }
        public bool HasExplored { get; set; }

        public DateTimeOffset? LastInteracted { get; set; }


        public ShellBagView GetShellBagView()
        {
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
                LastWriteTime = LastWriteTime
            };
        }

        public int NodeId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Value: {Value}");
            sb.AppendLine($"Shell Type: {FriendlyName}");

            sb.AppendLine();

            sb.AppendLine($"Absolute path: {AbsolutePath}");
            sb.AppendLine($"BagPath: {BagPath}");
            sb.AppendLine($"Slot #: {Slot}");
            sb.AppendLine($"Node Slot: {NodeSlot}");
            sb.AppendLine($"MRU Position: {MruPosition}");

            sb.AppendLine();

            sb.AppendLine($"# ChildBags: {ChildShellBags.Count}");

            sb.AppendLine();

            sb.AppendLine($"Hex Value: {BitConverter.ToString(HexValue)}");

            return sb.ToString();
        }
    }
}