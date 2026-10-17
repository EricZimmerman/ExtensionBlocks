using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtensionBlocks
{
    public class ShellBag0X78 : ShellBag
    {
        private readonly List<PropertySheet> Sheets;

        public ShellBag0X78(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            PropertyStore = new PropertyStore();

            Sheets = new List<PropertySheet>();

            BagPath = bagPath;

          //  var index = 0; // TODO Dont like using bin.read*

            FriendlyName = "Users property view";
            ShellBagType = Utils.ShellBagTypes.User;

            //TODO remove binaryreader
            var bin = new BinaryReader(new MemoryStream(rawBytes));

            bin.ReadBytes(2); // skip size

            bin.ReadByte(); //skip indicator (0x1F)

            var sortIndicator = bin.ReadByte();

            var dataSize = bin.ReadUInt16(); // BitConverter.ToUInt16(rawBytes, index);

            var dataSignature = bin.ReadUInt32(); //  BitConverter.ToUInt32(rawBytes, index);

            //Debug.WriteLine(dataSignature);
            //Debug.WriteLine(dataSignature.ToString("X"));

            var propertyStoreSize = bin.ReadUInt16(); //BitConverter.ToUInt16(rawBytes, index);

            var identifierSize = bin.ReadUInt16(); //BitConverter.ToUInt16(rawBytes, index);

            if (identifierSize > 0)
            {
                bin.ReadBytes(identifierSize);
                //index += identifierSize; // whts here?
            }

            if (propertyStoreSize > 0)
            {
                // var propStore = new PropertyStore(rawBytes.Skip(index).Take(propertyStoreSize).ToArray());

                var propertysheetBytes = bin.ReadBytes(propertyStoreSize);

                var propStore = new PropertyStore(propertysheetBytes);

                var p = propStore.Sheets.Where(t => t.PropertyNames.ContainsKey("AutoList"));

                if (p.Any())
                {
                    //we can now look thry prop bytes for extension blocks
                    //TODO this is a hack until we can process vectors natively

                    var extOffsets = new List<int>();
                    try
                    {
                        var regexObj = new Regex("([0-9A-F]{2})-00-EF-BE", RegexOptions.IgnoreCase);
                        var matchResult = regexObj.Match(BitConverter.ToString(propertysheetBytes));
                        while (matchResult.Success)
                        {
                            extOffsets.Add(matchResult.Index);
                            matchResult = matchResult.NextMatch();
                        }

                        foreach (var extOffset in extOffsets)
                        {
                            var binaryOffset = extOffset / 3 - 4;
                            var exSize = BitConverter.ToInt16(propertysheetBytes, binaryOffset);

                            var exBytes = propertysheetBytes.Skip(binaryOffset).Take(exSize).ToArray();

                            var signature1 = BitConverter.ToUInt32(exBytes, 4);

                            var block1 = Utils.GetExtensionBlockFromBytesOrError(signature1, exBytes);

                            ExtensionBlocks.Add(block1);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // Syntax error in the regular expression
                    }
                }

                PropertyStore = propStore;
            }

            bin.ReadBytes(2); //skip end of property sheet marker

            var rawguid = Utils.ExtractGuidFromShellItem(bin.ReadBytes(16));
            //    index += 16;

            rawguid = Utils.ExtractGuidFromShellItem(bin.ReadBytes(16));
            //   index += 16;

            var name = GuidMapping.GuidMapping.GetDescriptionFromGuid(rawguid);

            var extsize1 = bin.ReadUInt16(); // BitConverter.ToUInt16(rawBytes, index);

            if (extsize1 > 0)
            {
                //TODO is it ever bigger than one block? if so loop it

                //move position back 2 so we get the entire block of data below
                bin.BaseStream.Position -= 2;

                while (bin.BaseStream.Position != bin.BaseStream.Length)
                {
                    extsize1 = bin.ReadUInt16();

                    if (extsize1 == 0)
                    {
                        break;
                    }

                    bin.BaseStream.Position -= 2;

                    var extBytes = bin.ReadBytes(extsize1);

                    var signature1 = BitConverter.ToUInt32(extBytes, 4);

                    var block1 = Utils.GetExtensionBlockFromBytesOrError(signature1, extBytes);

                    ExtensionBlocks.Add(block1);
                }

                //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
            }

            Value = name;
        }

        public PropertyStore PropertyStore { get; }


        public override string ToString()
        {
            var sb = new StringBuilder();

            if (PropertyStore.Sheets.Count > 0)
            {
                sb.AppendLine("Property Sheets");

                sb.AppendLine(PropertyStore.ToString());
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}