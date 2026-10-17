using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var serializedVersion = BitConverter.ToUInt32(contents, sheetindex);

            sheetindex += 4;

            if (serializedVersion != 0x53505331)
            {
                throw new Exception($"Version mismatch! {BitConverter.ToString(contents, sheetindex - 4, 4)} != 31-53-50-53");
            }

            Version = BitConverter.ToString(contents, sheetindex - 4, 4);

            var rawguidshellProperty = new byte[16];

            Array.Copy(contents, sheetindex, rawguidshellProperty, 0, 16);

            var formatClassIdguid = Utils.ExtractGuidFromShellItem(rawguidshellProperty);

            sheetindex += 16;

            GUID = formatClassIdguid;

            var propertyValues = new Dictionary<int, byte[]>();

            if (formatClassIdguid == "d5cdd505-2e9c-101b-9397-08002b2cf9ae")
            {
                //all serialized property values are named properties
                PropertySheetType = PropertySheetTypeEnum.Named;

                var valueSize = 0;
                var propertyName = "";


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

                    propertyIndex += nameSize;

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
                            propertyIndex += uniLength;

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
                                uniLength * 2 - 2);
                            propertyIndex += uniLength * 2;

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

                            var remB = new byte[propertyValue.Value.Length - propertyIndex];
                            Buffer.BlockCopy(propertyValue.Value, propertyIndex, remB, 0, remB.Length);

                            var chunks = new List<byte[]>();

                            var index = 0;

                            index += 2; //skip remB size

                            while (index < remB.Length)
                            {
                                var size = BitConverter.ToInt16(remB, index);

                                if (size == 0)
                                {
                                    break;
                                }

                                var chunk = new byte[size];
                                Buffer.BlockCopy(remB, index, chunk, 0, size);

                                chunks.Add(chunk);

                                index += size;
                            }

                            var stringb = new LazyText();

                            foreach (var bytese in chunks)
                            {
                                switch (bytese[2])
                                {
                                    case 0x1f:
                                        stringb.AppendLine(() => new ShellBag0X1F(-1, -1, bytese, ""));
                                        //  tempVals.Add(f.Value);
                                        //  ExtensionBlocks.AddRange(f.ExtensionBlocks);
                                        break;

                                    case 0x2e:
                                        stringb.AppendLine(() => new ShellBag0X2E(-1, -1, bytese, ""));
                                        //   tempVals.Add(te.Value);
                                        //    ExtensionBlocks.AddRange(te.ExtensionBlocks);
                                        break;
                                    case 0x2f:
                                        stringb.AppendLine(() => new ShellBag0X2F(-1, -1, bytese, ""));
                                        //   tempVals.Add(tf.Value);
                                        //   ExtensionBlocks.AddRange(tf.ExtensionBlocks);
                                        break;
                                    case 0x31:
                                        stringb.AppendLine(() => new ShellBag0X31(-1, -1, bytese, ""));
                                        //   tempVals.Add(t1.Value);
                                        //   ExtensionBlocks.AddRange(t1.ExtensionBlocks);
                                        break;
                                    case 0x32:
                                        stringb.AppendLine(() => new ShellBag0X32(-1, -1, bytese, ""));
                                        //   tempVals.Add(t1.Value);
                                        //   ExtensionBlocks.AddRange(t1.ExtensionBlocks);
                                        break;
                                    case 0x00:
                                        stringb.AppendLine(() => new ShellBag0X00(-1, -1, bytese, ""));
                                        //   tempVals.Add(t1.Value);
                                        //   ExtensionBlocks.AddRange(t1.ExtensionBlocks);
                                        break;
                                    case 0x35:
                                        stringb.AppendLine(() => new ShellBag0X35(-1, -1, bytese, ""));
                                        //   tempVals.Add(t1.Value);
                                        //   ExtensionBlocks.AddRange(t1.ExtensionBlocks);
                                        break;
                                    case 0xc3:
                                        stringb.AppendLine(() => new ShellBag0XC3(-1, -1, bytese, ""));
                                        //   tempVals.Add(t1.Value);
                                        //   ExtensionBlocks.AddRange(t1.ExtensionBlocks);
                                        break;
                                    default:
                                        throw new Exception($"Unknown type id 0x{bytese[2]:X2}");
                                }
                            }

                            PropertyNames.Add(propertyName,
                                $"VT_BLOB data (See ExtraData for details)");

                            _extraData.Add(stringb);

                            break;

//                            var bytes = propertyValue.Value.Skip(0x69).ToArray();
//
//                            var props = new PropertyStore(bytes);
//
//                            PropertyNames.Add(propertyName,
//                                $"BLOB data: {BitConverter.ToString(propertyValue.Value, propertyIndex)}");
//
//                            foreach (var prop in props.Sheets)
//                            {
//                                foreach (var name in prop.PropertyNames)
//                                {
//                                    PropertyNames.Add($"{name.Key}", name.Value); // (From BLOB data)
//                                }
//                            }
//
//                            propertyIndex += blobSize;
//
//                            break;

                        case 0x0042:
                            //TODO FINISH THIS

//                            var type = propertyValue.Value[4];
//
//                            var outDir = Path.Combine(@"C:\temp\TestingNamed", type.ToString("X2"));
//                            if (Directory.Exists(outDir) == false)
//                            {
//                                Directory.CreateDirectory(outDir);
//                            }
//                            var outfile = Path.Combine(outDir, $"0x{type:X2}_{Guid.NewGuid()}.bin");
//                            File.WriteAllBytes(outfile,propertyValue.Value);


                            var valType = propertyValue.Value[4];

                            switch (valType)
                            {
                                case 0x12:
                                    //This is the case for AutoList, propertyValue.Value[4] == 0x14
                                    var index2 = propertyIndex;

                                    var nSize = BitConverter.ToInt32(propertyValue.Value, index2);
                                    index2 += 4;
                                    var nName = Encoding.Unicode.GetString(propertyValue.Value, index2, nSize)
                                        .Trim('\0');
                                    index2 += nSize;

                                    index2 += 2; //unknown
                                    index2 += 4; //unknown

                                    var sgb = new byte[16];
                                    Buffer.BlockCopy(propertyValue.Value, index2, sgb, 0, 16);
                                    var sg = new Guid(sgb);
                                    var sgf = GuidMapping.GuidMapping.GetDescriptionFromGuid(sg.ToString());
                                    index2 += 16;

                                    index2 += 4; //skip unknown

                                    var anotherGuid = BitConverter.ToInt32(propertyValue.Value, index2);
                                    index2 += 4;
                                    if (anotherGuid == 1)
                                    {
                                        //second guid
                                        sgb = new byte[16];
                                        Buffer.BlockCopy(propertyValue.Value, index2, sgb, 0, 16);
                                        var sg1 = new Guid(sgb);
                                        var sg1f = GuidMapping.GuidMapping.GetDescriptionFromGuid(sg1.ToString());
                                        index2 += 16;
                                    }
                                    else
                                    {
                                        index2 += 0x18; //jump to next data structure
                                    }

                                    var extraBlocks = BitConverter.ToInt32(propertyValue.Value, index2);
                                    index2 += 4;

                                    //at start of data blocks

                                    var sChunks = new List<byte[]>();

                                    if (extraBlocks == 1)
                                    {
                                        var sizeV1 = BitConverter.ToInt16(propertyValue.Value, index2);

                                        if (sizeV1 == 2)
                                        {
                                            index2 += 4;
                                            sizeV1 = BitConverter.ToInt16(propertyValue.Value, index2);
                                        }

                                        var singleChunk = new byte[sizeV1];

                                        Buffer.BlockCopy(propertyValue.Value, index2, singleChunk, 0, sizeV1);
                                        index2 += sizeV1;

                                        sChunks.Add(singleChunk);

                                        index2 += 2; //separator

                                        var moreData = BitConverter.ToInt16(propertyValue.Value, index2);

                                        if (moreData == 1)
                                        {
                                            index2 += 0x1c;
                                        }

                                        while (moreData != 0)
                                        {
                                            sizeV1 = BitConverter.ToInt16(propertyValue.Value, index2);
                                            singleChunk = new byte[sizeV1];

                                            Buffer.BlockCopy(propertyValue.Value, index2, singleChunk, 0, sizeV1);
                                            index2 += sizeV1;

                                            sChunks.Add(singleChunk);

                                            index2 += 2; //separator

                                            moreData = BitConverter.ToInt16(propertyValue.Value, index2);

                                            if (moreData == 1)
                                            {
                                                index2 += 0xc;
                                                moreData = 0;
                                            }
                                        }

                                        if (moreData == 0)
                                        {
                                            index2 += 0x10;
                                        }
                                    }
                                    else
                                    {
                                        var sizeV1 = BitConverter.ToInt16(propertyValue.Value, index2);
                                  

                                        if (sizeV1 > 0)
                                        {
                                            var singleChunk = new byte[sizeV1];
                                            
                                            Buffer.BlockCopy(propertyValue.Value, index2, singleChunk, 0, sizeV1);
                                            index2 += sizeV1;

                                            sChunks.Add(singleChunk);
                                            
                                            index2 += 2; //separator

                                        var moreData = BitConverter.ToInt32(propertyValue.Value, index2);
                                        while (moreData != 0x0000)
                                        {
                                            index2 += 16;

                                            sizeV1 = BitConverter.ToInt16(propertyValue.Value, index2);

                                            if (sizeV1 > 0)
                                            {
                                                singleChunk = new byte[sizeV1];

                                                Buffer.BlockCopy(propertyValue.Value, index2, singleChunk, 0, sizeV1);
                                                index2 += sizeV1;

                                                sChunks.Add(singleChunk);
                                                }
                                            

                                            index2 += 2; //separator

                                            moreData = BitConverter.ToInt32(propertyValue.Value, index2);
                                        }

                                        index2 += 0x20; //unknown before "item"
                                            
                                            
                                        }
                                        
                                        
                                    }

                                    var test = BitConverter.ToInt32(propertyValue.Value, index2);
                                    if (test != 0x00690004)
                                    {
                                        Debug.WriteLine("Item Missed!!");
                                    }

                                    var sb1 = new LazyText();

                                    //TODO Process sChunks
                                    //    Debug.WriteLine($"Chunk lengths. Total chunks: {sChunks.Count}");
                                    //   Debug.WriteLine("New--------------------------------------------------------------Start");
                                    foreach (var schunk in sChunks)
                                    {
                                        //Sometimes there are 2 byte chunks which isnt usable, so skip
                                        if (schunk.Length == 2)
                                        {
                                          //  Debug.WriteLine("Skipping length 2 bytes");
                                            continue;
                                        }

                                        var subChunks = new List<byte[]>();


                                        if (schunk.Length == 0x14)
                                        {
                                            subChunks.Add(schunk);


                                        }
                                        else
                                        {


                                            var sig = BitConverter.ToInt32(schunk, 4);
                                            if (sig == 0x53505331)
                                            {
                                                var ps1 = new PropertyStore(schunk);
                                                sb1.AppendLine(ps1);
                                            }

                                            else
                                            {
                                                
                                          
                           

                                                var chunkIndex = 2; //skip overall size

                                                var chunkSize = BitConverter.ToInt16(schunk, chunkIndex);

                                                while (chunkIndex != schunk.Length)
                                                {
                                                    //damaged data: the index would not move forward
                                                    if (chunkSize <= 0)
                                                    {
                                                        break;
                                                    }

                                                    var chunkB = new byte[chunkSize];
                                                    Buffer.BlockCopy(schunk, chunkIndex, chunkB, 0, chunkSize);

                                                    subChunks.Add(chunkB);

                                                    chunkIndex += chunkSize;

                                                    if (chunkIndex == schunk.Length)
                                                    {
                                                        break;
                                                    }

                                                    chunkSize = BitConverter.ToInt16(schunk, chunkIndex);
                                                }
                                            }

                                        }

                                        foreach (var subChunk in subChunks)
                                        {
                                            var id = subChunk[2];

                                            const uint mtpDeviceSig = 0x08312006;
                                            const uint mtpStorageSig = 0x10312005;
                                            const uint mtpFolderSig = 0x07192006;
                                            const uint cfsfSig = 0x46534643;

                                            var sig6 = BitConverter.ToUInt32(subChunk, 6);

                                            switch (sig6)
                                            {
                                                case mtpDeviceSig:
                                                    sb1.AppendLine(() => new ShellBagMTPDevice(-1, -1, subChunk, string.Empty));
                                                    continue;

                                                case mtpStorageSig:
                                                    sb1.AppendLine(() => new ShellBagMTPStorage(-1, -1, subChunk,
                                                        string.Empty));
                                                    continue;

                                                case mtpFolderSig:
                                                    sb1.AppendLine(() => new ShellBagMTPFolder(-1, -1, subChunk, string.Empty));
                                                    continue;
                                                case cfsfSig:
                                                    sb1.AppendLine(() => new ShellBagCFSF(-1, -1, subChunk, string.Empty));
                                                    continue;
                                            }

                                            switch (id)
                                            {
                                                case 0x1f:
                                                    sb1.AppendLine(() => new ShellBag0X1F(-1, -1, subChunk, string.Empty));
                                                    break;
                                                case 0x2f:
                                                    sb1.AppendLine(() => new ShellBag0X2F(-1, -1, subChunk, string.Empty));
                                                    break;
                                                case 0x2e:
//                                                  
                                                    sb1.AppendLine(() => new ShellBag0X2E(-1, -1, subChunk, string.Empty));
//                                                    
                                                    break;
                                                case 0x32:
                                                    sb1.AppendLine(() => new ShellBag0X32(-1, -1, subChunk, string.Empty));
                                                    break;
                                                case 0xc3:
                                                    sb1.AppendLine(() => new ShellBag0XC3(-1, -1, subChunk, string.Empty));
                                                    break;
                                                case 0x31:
                                                    sb1.AppendLine(() => new ShellBag0X31(-1, -1, subChunk, string.Empty));
                                                    break;


                                                case 0x35:
                                                    sb1.AppendLine(() => new ShellBag0X35(-1, -1, subChunk, string.Empty));
                                                    break;

                                                case 0x39:
                                                    sb1.AppendLine(() => new ShellBag0X39(-1, -1, subChunk, string.Empty));
                                                    break;

                                                case 0x01:
                                                    sb1.AppendLine(() => new ShellBag0X01(-1, -1, subChunk, string.Empty));
                                                    break;

                                                case 0x71:
                                                    sb1.AppendLine(() => new ShellBag0X71(-1, -1, subChunk, string.Empty));
                                                    break;
                                                case 0x61:
                                                    sb1.AppendLine(() => new ShellBag0X61(-1, -1, subChunk, string.Empty));
                                                    break;

                                                case 0xB1:
                                                    sb1.AppendLine(() => new ShellBag0XB1(-1, -1, subChunk, string.Empty));
                                                    break;
                                                case 0x1E:
                                                    sb1.AppendLine(() => new ShellBag0X1E(-1, -1, subChunk, string.Empty));
                                                    break;

                                                case 0x00:
                                                    try
                                                    {
                                                        //made here, not when the text is: a 0x00 item that can't be read gets the message below
                                                        sb1.AppendLine(new ShellBag0X00(-1, -1, subChunk, string.Empty));
                                                    }
                                                    catch (Exception)
                                                    {
//                                                        var outf1 = @"C:\Temp\!!Fixing";
//                                                        var outf21 = Path.Combine(outf1,
//                                                            Guid.NewGuid() + "_000000000.bin");
//
//                                                        File.WriteAllBytes(outf21, subChunk);

                                                        var possibleString00 =
                                                            Encoding.Unicode.GetString(subChunk, 6, subChunk.Length - 6)
                                                                .Trim('\0');

                                                 


                                                        var msg1 =
                                                            $"******************* Error parsing shell item. Possible false positive when parsing a property store. String data: {possibleString00} Hex data: {BitConverter.ToString(subChunk)} *******************";
                                                        sb1.AppendLine(msg1);
                                                        //     var l1 = LogManager.GetCurrentClassLogger();
                                                        //   l1.Warn(msg1);
                                                    }

                                                    break;
                                                default:
                                                    var possibleStringD =
                                                        Encoding.Unicode.GetString(subChunk, 4, subChunk.Length - 4)
                                                            .Trim('\0');

                                                    if (possibleStringD.StartsWith("StickyNotes:"))
                                                    {
                                                        sb1.AppendLine(possibleStringD);
                                                    }
                                                    else
                                                    {

                                                        var msg =
                                                            $"******************* Unknown type id in 0x0042 stream: 0x{id:X2}. Possible string: {possibleStringD} Hex data: {BitConverter.ToString(subChunk)} *******************";
                                                        sb1.AppendLine(msg);
                                                        
                                                        // throw new Exception($"Unknown type id in 0x0042 stream: 0x{id:X2}");
                                                       
                                                    }

                                                    break;
                                            }
                                        }
                                    }

                                    PropertyNames.Add(propertyName,
                                        $"VT_STREAM 0x0042 (AutoList) (See ExtraData for details)");

                                    _extraData.Add(sb1);

                                    //    Debug.WriteLine("New--------------------------------------------------------------End\r\n\r\n");

                                    break;

                                default:
                                    PropertyNames.Add(propertyName,
                                        "VT_STREAM 0x0042 (Non-AutoList) not implemented (yet)");
                                    break;
                            }

                            break;

                        case 0x0048:

                            var gb = new byte[16];
                            Buffer.BlockCopy(propertyValue.Value, propertyIndex, gb, 0, 16);

                            var g = new Guid(gb);

                            PropertyNames.Add(propertyName,
                                g.ToString());

                            break;

                        default:
                            PropertyNames.Add(propertyName,
                                $"Unknown property type: {namedType:X2}, Hex data (after property type): {BitConverter.ToString(propertyValue.Value, propertyIndex)}");

                            throw new Exception($"Unknown property type: {namedType:X2}");
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


                var propertySlotNumber1 = 0;

                while (sheetindex < contents.Length)
                {
                    //cut up shellPropertySheetList into byte arrays based on length, then process each one
                    var sheetSize1 = BitConverter.ToInt32(contents, sheetindex);

                    if (sheetSize1 == 0)
                    {
                        break; // we are out of lists
                    }

                    var sheetListBytes1 = new byte[sheetSize1];

                    Buffer.BlockCopy(contents, sheetindex, sheetListBytes1, 0, sheetSize1);

                    propertyValues.Add(propertySlotNumber1, sheetListBytes1);
                    propertySlotNumber1 += 1;

                    sheetindex += sheetSize1;
                } //end of while in shellPropertySheetList

                foreach (var propertyValue1 in propertyValues)
                {
                    var propertyIndex1 = 0;

                    BitConverter.ToInt32(propertyValue1.Value, propertyIndex1);
                    propertyIndex1 += 4;

                    var propertyId1 = BitConverter.ToInt32(propertyValue1.Value, propertyIndex1);
                    propertyIndex1 += 4;

                    propertyIndex1 += 1; //skip reserved

                    var numericType = BitConverter.ToUInt16(propertyValue1.Value, propertyIndex1);

                    propertyIndex1 += 2; //skip type
                    propertyIndex1 += 2; //skip padding?

                    //TODO Combine these with what is below. Make a function to take the type, process and return a string?
                    switch (numericType)
                    {
                        case 0x1048:
                            //MUST be a VectorHeader followed by a sequence of GUID (Packet Version) packets.

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                "VT_VECTOR data not implemented (yet)");

                            break;

                        case 0x001e:
                            var uniLength1e = BitConverter.ToInt32(propertyValue1.Value, propertyIndex1);
                            propertyIndex1 += 4;

                            var unicodeName1e =
                                Encoding.Unicode.GetString(propertyValue1.Value, propertyIndex1, uniLength1e)
                                    .Split('\0')
                                    .First();

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), unicodeName1e);

                            break;
                        case 0x0002:
                            //VT_I2 (0x0002) Either the specified type, or the type of the element or contained field MUST be a 2-byte signed int

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), BitConverter.ToInt16(propertyValue1.Value, propertyIndex1).ToString(CultureInfo.InvariantCulture));

                            break;
                        case 0x001f: //unicode string

                            var uniLength = BitConverter.ToInt32(propertyValue1.Value, propertyIndex1);
                            propertyIndex1 += 4;

                            if (uniLength == 0)
                            {
                                PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), "(None)");
                                break;
                            }

                            var unicodeName = Encoding.Unicode.GetString(propertyValue1.Value, propertyIndex1,
                                uniLength * 2 - 2);
                            propertyIndex1 += uniLength * 2;

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), unicodeName);

                            break;

                        case 0x000b:
                            //VT_BOOL (0x000B) MUST be a VARIANT_BOOL as specified in [MS-OAUT] section 2.2.27, followed by zero padding to 4 bytes.

                            var boolInt = BitConverter.ToInt32(propertyValue1.Value, propertyIndex1);
                            propertyIndex1 += 8;

                            var boolval = boolInt > 0;

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                boolval.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0003:
                            //VT_I4 (0x0003) MUST be a 32-bit signed integer.

                            var signedInt = BitConverter.ToInt32(propertyValue1.Value, propertyIndex1);
                            propertyIndex1 += 4;

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                signedInt.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0015:
                            //VT_UI8 (0x0015) MUST be an 8-byte unsigned integer

                            var unsigned8int = BitConverter.ToUInt64(propertyValue1.Value, propertyIndex1);
                            propertyIndex1 += 8;

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                unsigned8int.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0042:
                            //VT_STREAM (0x0042) MUST be an IndirectPropertyName. The storage representing the
                            //(non-simple) property set MUST have a stream element with this name

                            //defer for now

//                            var type = propertyValue1.Value[4];
//
//                            var outDir = Path.Combine(@"C:\temp\TestingNumeric", type.ToString("X2"));
//                            if (Directory.Exists(outDir) == false)
//                            {
//                                Directory.CreateDirectory(outDir);
//                            }
//                            var outfile = Path.Combine(outDir, $"0x{type:X2}_{Guid.NewGuid()}.bin");
//                            File.WriteAllBytes(outfile, propertyValue1.Value);

                            var index = propertyIndex1;
                            var nSize = BitConverter.ToInt32(propertyValue1.Value, index);

                            if (nSize == 0)
                            {
                                PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                    $"VT_STREAM: Name: (NONE)), Data: (NA)");
                                break;
                            }
                            index += 4;
                            var nName = Encoding.Unicode.GetString(propertyValue1.Value, index, nSize).Trim('\0');
                            index += nSize;

                            index += 10; //sizes and some unknown

                            //this should be the start of a property sheet
                            var psSize = BitConverter.ToInt32(propertyValue1.Value, index);
                            var pb = new byte[psSize];
                            Buffer.BlockCopy(propertyValue1.Value, index, pb, 0, psSize);

                            var ps = new PropertyStore(pb);

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                $"VT_STREAM: Name: {nName}, Data: {ps}");

                            break;

                        case 0x0013:
                            //VT_UI4 (0x0013) MUST be a 4-byte unsigned integer

                            var unsigned4int = BitConverter.ToUInt32(propertyValue1.Value, propertyIndex1);
                            propertyIndex1 += 4;

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                unsigned4int.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0001:
                            //VT_NULL (0x0001) MUST be zero bytes in length.

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), "Null");

                            break;

                        case 0x101f:
                            //VT_VECTOR | VT_LPWSTR 0x101F Type is Vector of UnicodeString, and the minimum property set version is 0

                            propertyIndex1 += 4;

                            if (propertyIndex1 == propertyValue1.Value.Length)
                            {
                                PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), "Null");
                                break;
                            }

                            uniLength = BitConverter.ToInt32(propertyValue1.Value, propertyIndex1);
                            propertyIndex1 += 4;

                            unicodeName = Encoding.Unicode.GetString(propertyValue1.Value, propertyIndex1,
                                uniLength * 2 - 2);
                            propertyIndex1 += uniLength * 2;

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), unicodeName);

                            break;

                        case 0x0048:
                            //VT_CLSID 0x0048 Type is CLSID, and the minimum property set version is 0.

                            var rawguid1 = new byte[16];

                            Array.Copy(propertyValue1.Value, propertyIndex1, rawguid1, 0, 16);

                            propertyIndex1 += 16;

                            var rawguid = Utils.ExtractGuidFromShellItem(rawguid1);

                            var foldername = GuidMapping.GuidMapping.GetDescriptionFromGuid(rawguid);

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), foldername);

                            break;

                        case 0x1011:
                            //VT_VECTOR | VT_UI1 0x1011 Type is Vector of 1-byte unsigned integers, and the minimum property  set version is 0.

                            var vecBytes = propertyValue1.Value;

                            var vecindex = 0x11;

                            var vecTypes = new List<string>();

                            var stringb = new LazyText();

                            while (vecindex != vecBytes.Length)
                            {
                                var vecsize = BitConverter.ToInt16(vecBytes, vecindex);

                                if (vecsize == 0)
                                {
                                    vecindex = vecBytes.Length;
                                    continue;
                                }

                                var vecbuff = new byte[vecsize];
                                Buffer.BlockCopy(vecBytes, vecindex, vecbuff, 0, vecsize);
                                vecindex += vecsize;

                                vecTypes.Add($"0x{vecbuff[2]:X2}");

                                var ss1 = BitConverter.ToUInt32(vecbuff, 6);
                                if (ss1 == 0x46534643)
                                {
                                    stringb.AppendLine(() => new ShellBagCFSF(-1, -1, vecbuff, ""));
                                }
                                else
                                {
                                    switch (vecbuff[2])
                                    {
                                        case 0x00:
                                            stringb.AppendLine(() => new ShellBag0X00(-1, -1, vecbuff, ""));
                                            break;
                                        case 0x1f:
                                            stringb.AppendLine(() => new ShellBag0X1F(-1, -1, vecbuff, ""));
                                            break;
                                        case 0x2f:
                                            stringb.AppendLine(() => new ShellBag0X2F(-1, -1, vecbuff, ""));
                                            break;
                                        case 0x31:
                                            stringb.AppendLine(() => new ShellBag0X31(-1, -1, vecbuff, ""));
                                            break;
                                        case 0x32:
                                            stringb.AppendLine(() => new ShellBag0X32(-1, -1, vecbuff, ""));
                                            break;

                                        case 0xc3:
                                            stringb.AppendLine(() => new ShellBag0XC3(-1, -1, vecbuff, ""));
                                            break;

                                        case 0x2e:
                                            stringb.AppendLine(() => new ShellBag0X2E(-1, -1, vecbuff, ""));
                                            break;

                                        case 0xb1:
                                            stringb.AppendLine(() => new ShellBag0XB1(-1, -1, vecbuff, ""));
                                            break;

                                        case 0x35:
                                            stringb.AppendLine(() => new ShellBag0X35(-1, -1, vecbuff, ""));
                                            break;

                                        case 0x36:
                                            stringb.AppendLine(() => new ShellBag0X36(-1, -1, vecbuff, ""));
                                            break;

                                        default:

                                            var specialSig = BitConverter.ToUInt32(vecbuff, 6);
                                            if (specialSig == 0x46534643)
                                            {
                                                stringb.AppendLine(() => new ShellBagCFSF(-1, -1, vecbuff, ""));
                                            }
                                            else
                                            {
                                                throw new Exception(
                                                    $"Unsupported ShellBag type 0x{vecbuff[2]:X2} in vector data. Raw bytes: {Convert.ToBase64String(vecbuff)}");
                                            }


                                            break;
                                    }
                                }

                                
                            }


                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                $"VT_VECTOR data (See ExtraData for details)");

                            _extraData.Add(stringb);

                            //TODO i see indicators from 0x00, case 0x23febbee: ProcessPropertyViewGUID(rawBytes) is the bits for this
                            // can we pull out the property sheet and add them to the property names here?

                            break;

                        case 0x0040:
                            //VT_FILETIME 0x0040 Type is FILETIME, and the minimum property set version is 0.

                            var hexNumber = BitConverter.ToInt64(propertyValue1.Value, propertyIndex1);
                            // "01CDF407";

                            propertyIndex1 += 8;

                            var dd = DateTime.FromFileTimeUtc(hexNumber);

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                dd.ToString(CultureInfo.InvariantCulture));

                            break;

                        case 0x0008:

                            var codePageSize = BitConverter.ToInt32(propertyValue1.Value, propertyIndex1);
                            propertyIndex1 += 4;

                            var codePageName = Encoding.Unicode.GetString(propertyValue1.Value, propertyIndex1,
                                codePageSize - 2);
                            propertyIndex1 += codePageSize;

                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture), codePageName);

                            break;

                        default:
                            PropertyNames.Add(propertyId1.ToString(CultureInfo.InvariantCulture),
                                $"Unknown property type: {numericType:X2}, Hex data (after property type): {BitConverter.ToString(propertyValue1.Value, propertyIndex1)}");

                            throw new Exception($"Unknown property type: {numericType:X2}");
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

        /// <summary>The sheet's version as its bytes ("31-53-50-53": 1SPS)</summary>
        public string Version { get; private set; }

        public string GUID { get; private set; }

        public byte[] HexValue { get; set; }

        public Dictionary<string, string> PropertyNames { get; }

        //the shell items in vectors, streams and blobs: their text is made when it is first asked for (making it while
        //parsing made reading property stores several times slower for callers that never look at it)
        private readonly List<LazyText> _extraData = new List<LazyText>();
        private string _extraDataText;

        public string ExtraData => _extraDataText ??= string.Concat(_extraData.Select(t => "\r\n" + t));

        public PropertySheetTypeEnum PropertySheetType { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            //numeric sheets: the property's name from the GUID and key, as far as it is known
            var s = PropertySheetType == PropertySheetTypeEnum.Numeric
                ? string.Join("; ", PropertyNames.Select(x => $"Guid: {GUID}, Key: {x.Key} ==> {(int.TryParse(x.Key, out var key) ? Utils.GetDescriptionFromGuidAndKey(GUID, key) : x.Key)}, Value: {x.Value}"))
                : string.Join("; ", PropertyNames.Select(x => $"Guid: {GUID}, Key: {x.Key} ==> {x.Key}, Value: {x.Value}"));

            sb.Append(s);

            if (ExtraData.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"Extra data: {ExtraData}");
            }

            

            return sb.ToString();
        }
    }
}