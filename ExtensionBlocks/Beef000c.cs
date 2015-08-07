using System.Text;

namespace ExtensionBlocks
{
    internal class Beef000c : BeefBase
    {
        public Beef000c(byte[] rawBytes)
            : base(rawBytes)
        {
            Message =
                "Unsupported Extension block. Please report to Report to saericzimmerman@gmail.com to get it added!";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}