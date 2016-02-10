using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class PropertySheet
    {
        public enum PropertySheetTypeEnum
        {
            Named,
            Numeric
        }

        public PropertySheet(byte[] contents)
        {
            PropertyNames = new Dictionary<string, string>();

            var sheetindex = 0;

            var serializedSize = BitConverter.ToInt32(contents, sheetindex);
            sheetindex = 4; //skip size

            Size = serializedSize;

            var serializedVersion = BitConverter.ToString(contents, sheetindex, 4);

            sheetindex += 4;

            if (serializedVersion != "31-53-50-53")
            {
                throw new Exception($"Version mismatch! {serializedVersion} != 31-53-50-53");
            }

            Version = serializedVersion;

            var rawguidshellProperty = new byte[16];

            Array.Copy(contents, sheetindex, rawguidshellProperty, 0, 16);

            var formatClassIdguid = Utils.ExtractGuidFromShellItem(rawguidshellProperty);

            sheetindex += 16;

            GUID = formatClassIdguid;

            if (formatClassIdguid == "d5cdd505-2e9c-101b-9397-08002b2cf9ae")
            {
                //all serialized property values are named properties
                PropertySheetType = PropertySheetTypeEnum.Named;

                var valueSize = 0;
                var propertyName = "";

                var propertyValues = new Dictionary<int, byte[]>();
                var propertySlotNumber = 0;

                while (sheetindex < contents.Length)
                {
                    //cut up shellPropertySheetList into byte arrays based on length, then process each one
                    valueSize = BitConverter.ToInt32(contents, sheetindex);

                    if (valueSize == 0)
                    {
                        break; // we are out of lists
                    }

                    var sheetListBytes = new byte[valueSize];
                    Array.Copy(contents, sheetindex, sheetListBytes, 0, valueSize);

                    propertyValues.Add(propertySlotNumber, sheetListBytes);
                    propertySlotNumber += 1;

                    sheetindex += valueSize;
                } //end of while in shellPropertySheetList

                foreach (var propertyValue in propertyValues)
                {
                    var propertyIndex = 0;

                    valueSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                    propertyIndex += 4;

                    var nameSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                    propertyIndex += 4;

                    propertyIndex += 1; //reserved

                    propertyName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex, nameSize - 2);

                    propertyIndex += (nameSize);

                    var namedType = BitConverter.ToUInt16(propertyValue.Value, propertyIndex);

                    propertyIndex += 2; //skip type
                    propertyIndex += 2; //skip padding?

                    //TODO Combine these with what is below. Make a function to take the type, process and return a string?
                    switch (namedType)
                    {
                        case 0x000b:
                            //VT_BOOL (0x000B)
                            var boolInt = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 8;

                            var boolval = boolInt > 0;

                            PropertyNames.Add(propertyName,
                                boolval.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0:
                        case 0x1:
                            PropertyNames.Add(propertyName, "");
                            break;

                        case 0x0002:
                            PropertyNames.Add(propertyName,
                                BitConverter.ToInt16(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));
                            break;

                        case 0x0003:
                            PropertyNames.Add(propertyName,
                                BitConverter.ToInt32(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));
                            break;

                        case 0x0004:
                            PropertyNames.Add(propertyName,
                                BitConverter.ToSingle(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));
                            break;

                        case 0x0005:
                            PropertyNames.Add(propertyName,
                                BitConverter.ToDouble(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));
                            break;

                        case 0x0008:

                            var uniLength = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            var unicodeName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                                uniLength - 2);
                            propertyIndex += (uniLength);

                            PropertyNames.Add(propertyName, unicodeName);

                            //  PropertyNames.Add(propertyName, BitConverter.ToDouble(propertyValue.Value, propertyIndex).ToString(CultureInfo.InvariantCulture));
                            break;


                        case 0x000a:
                            PropertyNames.Add(propertyName,
                                BitConverter.ToUInt32(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));
                            break;

                        case 0x0014:
                            //VT_I8 (0x0014)  MUST be an 8-byte signed integer.

                            PropertyNames.Add(propertyName,
                                BitConverter.ToInt64(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0015:
                            //VT_I8 (0x0014)  MUST be an 8-byte unsigned integer.

                            PropertyNames.Add(propertyName,
                                BitConverter.ToUInt64(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0016:
                            //VT_I8 (0x0014)  MUST be an 4-byte signed integer.

                            PropertyNames.Add(propertyName,
                                BitConverter.ToInt32(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0013:
                        case 0x0017:
                            //VT_I8 (0x0014)  MUST be an 4-byte unsigned integer.

                            PropertyNames.Add(propertyName,
                                BitConverter.ToUInt32(propertyValue.Value, propertyIndex)
                                    .ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x001f: //unicode string

                            uniLength = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            unicodeName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                                (uniLength*2) - 2);
                            propertyIndex += (uniLength*2);

                            PropertyNames.Add(propertyName, unicodeName);

                            break;

                        case 0x0040:
                            // VT_FILETIME 0x0040 Type is FILETIME, and the minimum property set version is 0.

                            var hexNumber = BitConverter.ToInt64(propertyValue.Value, propertyIndex);
                            // "01CDF407";

                            propertyIndex += 8;

                            var dd = DateTime.FromFileTimeUtc(hexNumber);

                            PropertyNames.Add(propertyName,
                                dd.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0041:
                            //VT_BLOB 0x0041 Type is binary large object (BLOB), and the minimum property set version is 0

                            //TODO FINISH THIS

                            var blobSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            var bytes = propertyValue.Value.Skip(0x69).ToArray();

                            var props = new PropertyStore(bytes);

                            PropertyNames.Add(propertyName,
                                $"BLOB data: {BitConverter.ToString(propertyValue.Value, propertyIndex)}");

                            foreach (var prop in props.Sheets)
                            {
                                foreach (var name in prop.PropertyNames)
                                {
                                    PropertyNames.Add($"{name.Key}", name.Value); // (From BLOB data)
                                }
                            }

                            propertyIndex += blobSize;

                            break;

                        case 0x0042:
                            //TODO FINISH THIS

                            //Type is Stream, and the minimum property set version is 0. VT_STREAM is not allowed in a simple property set.
                            PropertyNames.Add(propertyName,
                                "VT_STREAM not implemented (yet) See extension block section for contents for now");

                            break;

                        default:
                            PropertyNames.Add(propertyName,
                                $"Unknown property type: {namedType.ToString("X")}, Hex data (after property type): {BitConverter.ToString(propertyValue.Value, propertyIndex)}");

                            throw new Exception($"Unknown property type: {namedType.ToString("X")}");
                    }
                }

                var terminator = BitConverter.ToInt32(contents, sheetindex);

                if (terminator != 0)
                {
                    throw new Exception($"Expected terminator of 0, but got {terminator}");
                }
            }
            else
            {
                //treat as numeric property values

                PropertySheetType = PropertySheetTypeEnum.Numeric;

                var valueSize = 0;
                var propertyId = 0;

                var propertyValues = new Dictionary<int, byte[]>();
                var propertySlotNumber = 0;

                while (sheetindex < contents.Length)
                {
                    //cut up shellPropertySheetList into byte arrays based on length, then process each one
                    var sheetSize = BitConverter.ToInt32(contents, sheetindex);

                    if (sheetSize == 0)
                    {
                        break; // we are out of lists
                    }

                    var sheetListBytes = new byte[sheetSize];
                    Array.Copy(contents, sheetindex, sheetListBytes, 0, sheetSize);

                    propertyValues.Add(propertySlotNumber, sheetListBytes);
                    propertySlotNumber += 1;

                    sheetindex += sheetSize;
                } //end of while in shellPropertySheetList

                foreach (var propertyValue in propertyValues)
                {
                    var propertyIndex = 0;

                    valueSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                    propertyIndex += 4;

                    propertyId = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                    propertyIndex += 4;

                    propertyIndex += 1; //skip reserved

                    var numericType = BitConverter.ToUInt16(propertyValue.Value, propertyIndex);

                    propertyIndex += 2; //skip type
                    propertyIndex += 2; //skip padding?

                    //TODO Combine these with what is below. Make a function to take the type, process and return a string?
                    switch (numericType)
                    {
                        case 0x1048:
                            //MUST be a VectorHeader followed by a sequence of GUID (Packet Version) packets.

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                "VT_VECTOR data not implemented (yet)");

                            break;

                        case 0x001f: //unicode string

                            var uniLength = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            var unicodeName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                                (uniLength*2) - 2);
                            propertyIndex += (uniLength*2);

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), unicodeName);

                            break;

                        case 0x000b:
                            //VT_BOOL (0x000B) MUST be a VARIANT_BOOL as specified in [MS-OAUT] section 2.2.27, followed by zero padding to 4 bytes.

                            var boolInt = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 8;

                            var boolval = boolInt > 0;

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                boolval.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0003:
                            //VT_I4 (0x0003) MUST be a 32-bit signed integer.

                            var signedInt = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                signedInt.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0015:
                            //VT_UI8 (0x0015) MUST be an 8-byte unsigned integer

                            var unsigned8int = BitConverter.ToUInt64(propertyValue.Value, propertyIndex);
                            propertyIndex += 8;

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                unsigned8int.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0042:
                            //VT_STREAM (0x0042) MUST be an IndirectPropertyName. The storage representing the
                            //(non-simple) property set MUST have a stream element with this name

                            //defer for now

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                "VT_STREAM not implemented");

                            break;

                        case 0x0013:
                            //VT_UI4 (0x0013) MUST be a 4-byte unsigned integer

                            var unsigned4int = BitConverter.ToUInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                unsigned4int.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0001:
                            //VT_NULL (0x0001) MUST be zero bytes in length.

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), "Null");

                            break;

                        case 0x0002:
                            //VT_I2 (0x0002) Either the specified type, or the type of the element or contained field MUST be a 2-byte signed int

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), BitConverter.ToUInt16(propertyValue.Value, propertyIndex).ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x101f:
                            //VT_VECTOR | VT_LPWSTR 0x101F Type is Vector of UnicodeString, and the minimum property set version is 0

                            propertyIndex += 4;

                            uniLength = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            unicodeName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                                (uniLength*2) - 2);
                            propertyIndex += (uniLength*2);

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), unicodeName);

                            break;

                        case 0x0048:
                            //VT_CLSID 0x0048 Type is CLSID, and the minimum property set version is 0.

                            var rawguid1 = new byte[16];

                            Array.Copy(propertyValue.Value, propertyIndex, rawguid1, 0, 16);

                            propertyIndex += 16;

                            var rawguid = Utils.ExtractGuidFromShellItem(rawguid1);

                            var foldername = Utils.GetFolderNameFromGuid(rawguid);

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), foldername);

                            break;

                        case 0x1011:
                            //VT_VECTOR | VT_UI1 0x1011 Type is Vector of 1-byte unsigned integers, and the minimum property  set version is 0.

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                "VT_VECTOR data not implemented (yet) See extension block section for contents for now");

                            //TODO i see indicators from 0x00, case 0x23febbee: ProcessPropertyViewGUID(rawBytes) in the bits for this
                            // can we pull out the property sheet and add them to the property names here?

                            break;

                        case 0x0040:
                            //VT_FILETIME 0x0040 Type is FILETIME, and the minimum property set version is 0.

                            var hexNumber = BitConverter.ToInt64(propertyValue.Value, propertyIndex);
                            // "01CDF407";

                            propertyIndex += 8;

                            var dd = DateTime.FromFileTimeUtc(hexNumber);

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                dd.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0008:

                            var codePageSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            var codePageName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                                codePageSize - 2);
                            propertyIndex += (codePageSize);

                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), codePageName);

                            break;

                        default:
                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                                $"Unknown property type: {numericType.ToString("X")}, Hex data (after property type): {BitConverter.ToString(propertyValue.Value, propertyIndex)}");

                            throw new Exception($"Unknown property type: {numericType.ToString("X")}");
                    }
                }

                var terminator = BitConverter.ToInt32(contents, sheetindex);

                if (terminator != 0)
                {
                    throw new Exception($"Expected terminator of 0, but got {terminator}");
                }
            }
        }

        public int Size { get; private set; }

        public string Version { get; private set; }

        public string GUID { get; private set; }

        public byte[] HexValue { get; set; }

        public Dictionary<string, string> PropertyNames { get; }

        public PropertySheetTypeEnum PropertySheetType { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var s = string.Join("; ", PropertyNames.Select(x => $"Key: {x.Key}, Value: {x.Value}"));

            sb.Append(s);

            return sb.ToString();
        }
    }
}