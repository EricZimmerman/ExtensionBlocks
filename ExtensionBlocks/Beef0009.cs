using System.Text;

namespace ExtensionBlocks
{
    internal class Beef0009 : BeefBase
    {
        public Beef0009(byte[] rawBytes)
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