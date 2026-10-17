using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBagParseError : IShellBag
    {
        public ShellBagParseError(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = $"Type ID: 0x{rawBytes[2]:X2}";
            ShellBagType = Utils.ShellBagTypes.ParseError;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            ExtensionBlocks = new List<IExtensionBlock>();

            HexValue = rawBytes;

            AbsolutePath = string.Empty;

            BagPath = bagPath;

            Value = "!!! Parse error placeholder !!!";

            //Value = string.Format("!!! Parse error placeholder!!! ShellBag ID: {0}", rawBytes[2].ToString("x"));
        }

        public List<IExtensionBlock> ExtensionBlocks { get; set; }

        public string InternalId { get; }

        public string FriendlyName { get; }

        public bool IsDeleted { get; set; }

        public byte[] HexValue { get; }

        public string BagPath { get; }

        public string AbsolutePath { get; set; }
        public Utils.ShellBagTypes ShellBagType { get; set; }

        public int Slot { get; }

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