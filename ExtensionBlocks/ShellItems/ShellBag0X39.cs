using System;

namespace ExtensionBlocks
{
    public class ShellBag0X39 : ShellBag
    {
        private readonly ShellBag0X31 clone;

        public ShellBag0X39(int slot, int mruPosition, byte[] rawBytes, string bagPath)
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
        ///     write or interacted times; 0x36's file name)
        /// </summary>
        private void Sync()
        {
            clone.FriendlyName = FriendlyName;
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
            var view = clone.GetShellBagView();
            //the view sets the last interacted time (MRU position 0) on the clone; keep it here too, as every other item
            //does, or the next Sync puts it back to null (ToString had no "Last interacted" line)
            LastInteracted = clone.LastInteracted;
            return view;
        }

        public override string ToString()
        {
            Sync();
            return clone.ToString();
        }
    }
}