using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtensionBlocks
{
    public class ShellBag0X2E : ShellBag
    {
        public ShellBag0X2E(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0;

            if (rawBytes[3] == 0x80 || rawBytes.Length == 0x16)
            {
                ProcessGuid(rawBytes);
            }
            else
            {
                var postSig = BitConverter.ToUInt64(rawBytes, rawBytes.Length - 8);

                if (postSig == 0x0000ee306bfe9555 || postSig == 0xee306bfe9555c589)
                {
                    FriendlyName = "User profile";
                    ShellBagType = Utils.ShellBagTypes.User;

                    var la =
                        Utils.ExtractDateTimeOffsetFromBytes(
                            rawBytes.Skip(rawBytes.Length - 14).Take(4).ToArray());

                    LastAccessTime = la;

                    index = 10;

                    var tempString = Encoding.Unicode.GetString(rawBytes.Skip(index).ToArray()).Split('\0')[0];

                    if (tempString == "")
                    {
                        tempString = "(None)";
                    }

                    Value = tempString;

                    return;
                }


                var testSig2 = BitConverter.ToInt32(rawBytes, 5);

                if (testSig2 >= 0x15032601)
                {
                    FriendlyName = "Control panel category";
                    ShellBagType = Utils.ShellBagTypes.ControlPanelCategory;

                    index = 0x12;

                    var val1 = Encoding.Unicode.GetString(rawBytes, index, 0x48 * 2).Trim('\0');

                    index = 0x116;

                    var val2 = Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - 0x22 - index).Trim('\0');

                    var guidb = new byte[16];
                    index = rawBytes.Length - 0x22; //beginning of guids

                    Buffer.BlockCopy(rawBytes, index, guidb, 0, 16);

                    var g = new Guid(guidb);
                    var gf = GuidMapping.GuidMapping.GetDescriptionFromGuid(g.ToString());

                    index += 16;

                    guidb = new byte[16];
                    Buffer.BlockCopy(rawBytes, index, guidb, 0, 16);

                    var g2 = new Guid(guidb);
                    var g2f = GuidMapping.GuidMapping.GetDescriptionFromGuid(g2.ToString());

                    Value = val2;

                    DevicePath = val1;

                    Category = g2f;

                    return;
                }


                ProcessPropertyViewDefault(rawBytes);
            }
            
        }

        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        public PropertyStore PropertyStore { get; private set; }

        public string Category { get; }
        public string DevicePath { get; }

        private void ProcessPropertyViewDefault(byte[] rawBytes)
        {
            FriendlyName = "Users property view";
            ShellBagType = Utils.ShellBagTypes.User;
            var index = 10;

            var shellPropertySheetListSize = BitConverter.ToInt16(rawBytes, index);


            index += 2;

            var identifiersize = BitConverter.ToInt16(rawBytes, index);


            index += 2;

            var identifierData = new byte[identifiersize];

            Array.Copy(rawBytes, index, identifierData, 0, identifiersize);

            index += identifiersize;

            if (shellPropertySheetListSize > 0)
            {
                var propBytes = rawBytes.Skip(index).Take(shellPropertySheetListSize).ToArray();
                var propStore = new PropertyStore(propBytes);

                PropertyStore = propStore;

                var p = propStore.Sheets.Where(t => t.PropertyNames.ContainsKey("32"));

                if (p.Any())
                {
                    //we can now look thry prop bytes for extension blocks
                    //TODO this is a hack until we can process vectors natively

                    var extOffsets = new List<int>();
                    try
                    {
                        var regexObj = new Regex("([0-9A-F]{2})-00-EF-BE", RegexOptions.IgnoreCase);
                        var matchResult = regexObj.Match(BitConverter.ToString(propBytes));
                        while (matchResult.Success)
                        {
                            extOffsets.Add(matchResult.Index);
                            matchResult = matchResult.NextMatch();
                        }

                        foreach (var extOffset in extOffsets)
                        {
                            var binaryOffset = extOffset / 3 - 4;
                            var exSize = BitConverter.ToInt16(propBytes, binaryOffset);

                            var exBytes = propBytes.Skip(binaryOffset).Take(exSize).ToArray();

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
            }
            else


            {
                index += shellPropertySheetListSize;
            }

            index += 2; //move past end of property sheet terminator

            var rawguid = Utils.ExtractGuidFromShellItem(rawBytes.Skip(index).Take(16).ToArray());
            index += 16;

            rawguid = Utils.ExtractGuidFromShellItem(rawBytes.Skip(index).Take(16).ToArray());
            index += 16;

            var name = GuidMapping.GuidMapping.GetDescriptionFromGuid(rawguid);

            Value = name;

            var extBlockSize = BitConverter.ToInt16(rawBytes, index);

            if (extBlockSize > 0)
            {
                //process extension blocks

                while (extBlockSize > 0)
                {
                    var extBytes = rawBytes.Skip(index).Take(extBlockSize).ToArray();

                    index += extBlockSize;

                    var signature1 = BitConverter.ToUInt32(extBytes, 4);


                    var block1 = Utils.GetExtensionBlockFromBytesOrError(signature1, extBytes);

                    ExtensionBlocks.Add(block1);

                    extBlockSize = BitConverter.ToInt16(rawBytes, index);
                }
            }

            int terminator = BitConverter.ToInt16(rawBytes, index);

            if (terminator > 0)
            {
                throw new Exception($"Expected terminator of 0, but got {terminator}");
            }
        }

        private void ProcessGuid(byte[] rawBytes)
        {
            FriendlyName = "Root folder: GUID";
            ShellBagType = Utils.ShellBagTypes.Guid;

            //TODO split this out or at least use a different icon?

            var index = 2;

            index += 2; // move past index and a single unknown value

            var rawguid1 = new byte[16];

            Array.Copy(rawBytes, index, rawguid1, 0, 16);

            var rawguid = Utils.ExtractGuidFromShellItem(rawguid1);

            var foldername = GuidMapping.GuidMapping.GetDescriptionFromGuid(rawguid);

            index += 16;


            if (index == rawBytes.Length)
            {
                return;
            }


            var size = BitConverter.ToInt16(rawBytes, index);
            if (size == 0)
            {
                index += 2;
            }
            //TODO this should be the looping cut up thing

            while (index < rawBytes.Length)
            {
                var signature = BitConverter.ToUInt32(rawBytes, index + 4);

                //TODO does this need to check if its a 0xbeef?? regex?
                var block = Utils.GetExtensionBlockFromBytesOrError(signature, rawBytes.Skip(index).ToArray());

                ExtensionBlocks.Add(block);

                if (size <= 0)
                {
                    //not a size (damaged data): the index would not move forward
                    break;
                }

                index += size;

                if (index != rawBytes.Length)
                {
                    size = BitConverter.ToInt16(rawBytes, index);
                    if (size == 0)
                    {
                        index += 2;
                    }
                }
            }

            //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)

            Value = foldername;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (PropertyStore?.Sheets.Count > 0)
            {
                sb.AppendLine("Property Sheets");

                sb.AppendLine(PropertyStore.ToString());
            }

            if (LastAccessTime.HasValue)
            {
                sb.AppendLine(
                    $"Accessed On: {LastAccessTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
                sb.AppendLine();
            }

            if (Category != null)
            {
                sb.AppendLine(
                    $"Category: {Category}");
                sb.AppendLine();
            }

            if (DevicePath != null)
            {
                sb.AppendLine(
                    $"Device Path: {DevicePath}");
                sb.AppendLine();
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}