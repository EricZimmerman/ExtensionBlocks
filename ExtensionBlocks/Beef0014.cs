using System.Text;

namespace ExtensionBlocks
{
    internal class Beef0014 : BeefBase
    {
        public Beef0014(byte[] rawBytes)
            : base(rawBytes)
        {
            Message =
                "Unsupported Extension block. Please report to Report to saericzimmerman@gmail.com to get it added!";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            return sb.ToString();
        }
    }
}