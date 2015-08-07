using System.Text;

namespace ExtensionBlocks
{
    internal class BeefPlaceHolder : BeefBase
    {
        public BeefPlaceHolder(byte[] rawBytes)
            : base(rawBytes)
        {
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(
                "This is a placeholder bag to account for additional extension blocks inside internal ShellBags");


            return sb.ToString();
        }
    }
}