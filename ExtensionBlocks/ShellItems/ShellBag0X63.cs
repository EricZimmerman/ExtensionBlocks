using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X63 : ShellBag
    {
        public ShellBag0X63(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            //This is the same as 5c, so do not dupe code
            var foo = new ShellBag0X5C(slot, mruPosition, rawBytes, bagPath);

            Slot = foo.Slot;
            MruPosition = foo.MruPosition;
            FriendlyName = foo.FriendlyName;
            ShellBagType = foo.ShellBagType;
            ChildShellBags = foo.ChildShellBags;
            InternalId = foo.InternalId;
            HexValue = foo.HexValue;
            ExtensionBlocks = foo.ExtensionBlocks;
            BagPath = foo.BagPath;
            Value = foo.Value;
            Description = foo.Description;
        }

        public string Description { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Description.Length > 0)
            {
                sb.AppendLine($"Description: {Description}");
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}