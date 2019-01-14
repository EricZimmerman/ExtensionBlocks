using System;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef0010 : BeefBase
    {
        public Beef0010(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef0010)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef0010 but is 0x{Signature:X}");
            }


            var propStore = new PropertyStore(rawBytes.Skip(16).ToArray());


            PropertyStore = propStore;


            VersionOffset = BitConverter.ToInt16(rawBytes, rawBytes.Length - 2);
        }

        public PropertyStore PropertyStore { get; }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            var sheetNumber = 0;

            foreach (var propertySheet in PropertyStore.Sheets)
            {
                sb.AppendLine($"Sheet #{sheetNumber} => {propertySheet}");

                sheetNumber += 1;

                //foreach (var propertyName in propertySheet.PropertyNames)
                //{
                //    sb.AppendLine(string.Format("Key: {0}, Value: {1}", propertyName.Key, propertyName.Value));
                //}
            }

            return sb.ToString();
        }
    }
}