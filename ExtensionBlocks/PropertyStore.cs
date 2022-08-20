using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class PropertyStore
    {
        public PropertyStore()
        {
            Sheets = new List<PropertySheet>();
        }

        public PropertyStore(byte[] rawBytes)
        {
            Sheets = new List<PropertySheet>();

            //shellPropertySheetList now contains what we need to parse for the rest of this process

            var shellPropertyIndex = 0;

            var sheetLists = new Dictionary<int, byte[]>();
            var sheetListslot = 0;

            while (shellPropertyIndex < rawBytes.Length)
            {
                //cut up shellPropertySheetList into byte arrays based on length, then process each one
                var serializedSize = BitConverter.ToInt32(rawBytes, shellPropertyIndex);

                if (serializedSize == 0 || (uint)serializedSize >= rawBytes.Length)
                {
                    break; // we are out of lists
                }

                var sheetListBytes = new byte[serializedSize];
                Array.Copy(rawBytes, shellPropertyIndex, sheetListBytes, 0, serializedSize);

                sheetLists.Add(sheetListslot, sheetListBytes);
                sheetListslot += 1;

                shellPropertyIndex += serializedSize;
            } //end of while in shellPropertySheetList

            foreach (var sheetList in sheetLists)
            {
                var sheet = new PropertySheet(sheetList.Value);

                Sheets.Add(sheet);
            }
        }

        public List<PropertySheet> Sheets { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var sheetNumber = 0;

            foreach (var propertySheet in Sheets)
            {
                sb.AppendLine($"Sheet #{sheetNumber} => {propertySheet}");

                sheetNumber += 1;

            }

            return sb.ToString();
        }
    }
}
