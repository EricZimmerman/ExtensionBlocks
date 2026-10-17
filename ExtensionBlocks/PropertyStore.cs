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

            if (rawBytes.Length < 4)
            {
                return;
            }

            while (shellPropertyIndex + 4 <= rawBytes.Length)
            {
                //cut up shellPropertySheetList into byte arrays based on length, then process each one
                var serializedSize = BitConverter.ToInt32(rawBytes, shellPropertyIndex);

                if (serializedSize == 0)
                {
                    break; // we are out of lists
                }

                //damaged data: keep the sheets before it
                if (serializedSize < 0 || serializedSize > rawBytes.Length - shellPropertyIndex)
                {
                    ParseErrors.Add($"Property sheet at offset 0x{shellPropertyIndex:X} has a size of {serializedSize}, but only {rawBytes.Length - shellPropertyIndex} bytes are left");
                    break;
                }

                var sheetListBytes = new byte[serializedSize];
                Array.Copy(rawBytes, shellPropertyIndex, sheetListBytes, 0, serializedSize);

                sheetLists.Add(sheetListslot, sheetListBytes);
                sheetListslot += 1;

                shellPropertyIndex += serializedSize;
            } //end of while in shellPropertySheetList


            foreach (var sheetList in sheetLists)
            {
                try
                {
                    var sheet = new PropertySheet(sheetList.Value);

                    Sheets.Add(sheet);
                }
                catch (Exception ex)
                {
                    //damaged data: the other sheets are still read
                    ParseErrors.Add($"Property sheet #{sheetList.Key} could not be parsed: {ex.Message}");
                }
            }

            //nothing could be read: as before, the item reading it decides (often a false positive: not a property store)
            if (Sheets.Count == 0 && ParseErrors.Count > 0)
            {
                throw new Exception(ParseErrors[0]);
            }
        }

        public List<PropertySheet> Sheets { get; }

        /// <summary>Damaged sheets that were left out</summary>
        public List<string> ParseErrors { get; } = new List<string>();

        public override string ToString()
        {
            var sb = new StringBuilder();

            var sheetNumber = 0;

            foreach (var propertySheet in Sheets)
            {
                sb.AppendLine($"Sheet #{sheetNumber} => {propertySheet}");

                sheetNumber += 1;
            }

            foreach (var error in ParseErrors)
            {
                sb.AppendLine($"Parse error: {error}");
            }

            return sb.ToString();
        }
    }
}