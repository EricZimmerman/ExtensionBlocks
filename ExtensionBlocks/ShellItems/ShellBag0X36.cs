using System;

namespace ExtensionBlocks
{
    public class ShellBag0X36 : ShellBag
    {
        private readonly ShellBag0X31 clone;

        public ShellBag0X36(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            clone = new ShellBag0X31(slot, mruPosition, rawBytes, bagPath);

            Slot = clone.Slot;
            MruPosition = clone.MruPosition;
            FriendlyName = clone.FriendlyName;
            ChildShellBags = clone.ChildShellBags;
            InternalId = clone.InternalId;
            HexValue = clone.HexValue;
            ExtensionBlocks = clone.ExtensionBlocks;
            BagPath = clone.BagPath;
            Value = clone.Value;
            LastAccessTime = clone.LastAccessTime;
            LastModificationTime = clone.LastModificationTime;
            ShortName = clone.ShortName;
            ShellBagType = clone.ShellBagType;
        }

        public string ShortName { get; }

        /// <summary>
        ///     last modified time of BagPath
        /// </summary>
        public DateTimeOffset? LastModificationTime { get; set; }


        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        public override string AbsolutePath
        {
            get => clone.AbsolutePath;
            set => clone.AbsolutePath = value;
        }

        /// <summary>
        ///     The parts set from the registry after parsing (last write time, ...) are set on this item; the clone that
        ///     builds the view and the text needs them too (#237: Unicode name items, as accented names are, had no last
        ///     write or interacted times)
        /// </summary>
        private void Sync()
        {
            clone.LastWriteTime = LastWriteTime;
            clone.FirstInteracted = FirstInteracted;
            clone.LastInteracted = LastInteracted;
            clone.NodeSlot = NodeSlot;
            clone.IsDeleted = IsDeleted;
            clone.HasExplored = HasExplored;
            clone.ChildShellBags = ChildShellBags;
            clone.Slot = Slot;
            clone.MruPosition = MruPosition;
        }

        public override ShellBagView GetShellBagView()
        {
            Sync();
            return clone.GetShellBagView();
        }

        public override string ToString()
        {
            Sync();
            return clone.ToString();
        }
    }
}