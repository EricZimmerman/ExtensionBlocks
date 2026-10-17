using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBagMTPFolder : ShellBag
    {
        public ShellBagMTPFolder(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "MTP folder";
            ShellBagType = Utils.ShellBagTypes.MtpFolder;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0;

            var size = BitConverter.ToInt16(rawBytes, index);
            index += 2;

            var typeid = rawBytes[index];
            index += 1;

            index += 1; //unknown

            var remainingBytesSize = BitConverter.ToInt16(rawBytes, index);
            index += 2;

            index = 0x1a; //offset to modified time
            var modTime = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index));
            index += 8;
            var createTime = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index));
            index += 8;

            var guidBuff = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBuff, 0, 16);
            index += 16;

            var contentGuid = new Guid(guidBuff);

            var contentFolder = GuidMapping.GuidMapping.GetDescriptionFromGuid(contentGuid.ToString());

            var subshellSize = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            var lenFolderName = BitConverter.ToInt32(rawBytes, index);
            index += 4;
            var lenFolderName2 = BitConverter.ToInt32(rawBytes, index);
            index += 4;
            var lenFolderId = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            FolderName = Encoding.Unicode.GetString(rawBytes, index, lenFolderName * 2).Trim('\0');
            index += lenFolderName * 2;
            FolderName2 = Encoding.Unicode.GetString(rawBytes, index, lenFolderName2 * 2).Trim('\0');
            index += lenFolderName2 * 2;
            var folderId = Encoding.Unicode.GetString(rawBytes, index, lenFolderId * 2).Trim('\0');
            index += lenFolderId * 2;

            Value = FolderName2;

            index += 4; //skip unknown

            guidBuff = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBuff, 0, 16);
            index += 16;

            var portableDevGuid = new Guid(guidBuff);

            var portableDevGuidFolder = GuidMapping.GuidMapping.GetDescriptionFromGuid(portableDevGuid.ToString());

            var expectedPropertyCount = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            //start properties

            //read guid, read id, read type, act on type

            PropertyArrays = new List<PropertyArray>();

            while (index < rawBytes.Length)
            {
                if (index + 4 >= rawBytes.Length)
                {
                    index += 4;
                    break;
                }
                if (BitConverter.ToInt32(rawBytes, index) == 0x00)
                {
                    index += 4;
                    continue;
                }

                if (BitConverter.ToInt16(rawBytes, index) == 0)
                {
                    index += 2;
                    
                    var spsSize = BitConverter.ToInt16(rawBytes, index);
                    var spsBuff = new byte[spsSize];
                    
                    Buffer.BlockCopy(rawBytes,index,spsBuff,0,spsSize);


                    var bbb = new Beef0010(spsBuff);

                    ExtensionBlocks.Add(bbb);

                    index += spsSize;
                    index += 2;

                    break;
                }

                guidBuff = new byte[16];
                Buffer.BlockCopy(rawBytes, index, guidBuff, 0, 16);
                index += 16;

                var guid = new Guid(guidBuff);
                var propertyId = BitConverter.ToInt32(rawBytes, index);
                index += 4;
                var propertyType = (OlePropertyType) BitConverter.ToInt32(rawBytes, index);
                index += 4;

                switch (propertyType)
                {
                    case OlePropertyType.Unknown:

                        var gBuff1 = new byte[16];
                        Buffer.BlockCopy(rawBytes, index, gBuff1, 0, 16);
                        index += 16;

                        var unknownCount = BitConverter.ToInt32(rawBytes, index);

                        var countBytes = new byte[4];
                        Buffer.BlockCopy(rawBytes, index, countBytes, 0, 4);
                        index += 4;

                        var unknownBytesLen = unknownCount * 0x10;

                        var unknownBytes = new byte[unknownBytesLen];
                        if (unknownCount > 0)
                        {
                            Buffer.BlockCopy(rawBytes, index, unknownBytes, 0, unknownBytesLen);
                            //TODO decode this
                            index += unknownCount * 0x10;
                        }

                        //guid-count-data
                        var totalDataLen = 16 + 4 + unknownBytesLen;
                        var totalDataBytes = new byte[totalDataLen];
                        Buffer.BlockCopy(gBuff1, 0, totalDataBytes, 0, 16);
                        Buffer.BlockCopy(countBytes, 0, totalDataBytes, 16, 4);
                        Buffer.BlockCopy(unknownBytes, 0, totalDataBytes, 20, unknownBytesLen);

                        var paUnknown = new PropertyArray(guid, propertyId, propertyType, totalDataBytes);
                        PropertyArrays.Add(paUnknown);

                        break;
                    case OlePropertyType.OleDate:
                        var timeBytes = new byte[8];
                        Buffer.BlockCopy(rawBytes, index, timeBytes, 0, 8);
                        index += 8;

                        var paDate = new PropertyArray(guid, propertyId, propertyType, timeBytes);
                        PropertyArrays.Add(paDate);

                        break;
                    case OlePropertyType.Boolean:

                        var boolBytes = new byte[2];
                        Buffer.BlockCopy(rawBytes, index, boolBytes, 0, 2);

                        index += 2;

                        var paBool = new PropertyArray(guid, propertyId, propertyType, boolBytes);
                        PropertyArrays.Add(paBool);

                        break;
                    case OlePropertyType.Guid:
                        var gBuff = new byte[16];
                        Buffer.BlockCopy(rawBytes, index, gBuff, 0, 16);
                        index += 16;

                        var paGuid = new PropertyArray(guid, propertyId, propertyType, gBuff);
                        PropertyArrays.Add(paGuid);

                        break;
                    case OlePropertyType.UnicodeString:
                        var unisize = BitConverter.ToInt32(rawBytes, index);
                        index += 4;
                        var uniBytes = new byte[unisize];
                        Buffer.BlockCopy(rawBytes, index, uniBytes, 0, unisize);
                        index += unisize;

                        var paUni = new PropertyArray(guid, propertyId, propertyType, uniBytes);
                        PropertyArrays.Add(paUni);

                        break;

                    case OlePropertyType.Int16Unsigned:
                        var int16uBytes = new byte[2];
                        Buffer.BlockCopy(rawBytes, index, int16uBytes, 0, 2);

                        index += 2;

                        var paInt16u = new PropertyArray(guid, propertyId, propertyType, int16uBytes);
                        PropertyArrays.Add(paInt16u);

                        break;
                    case OlePropertyType.Int32Unsigned:
                        var int32uBytes = new byte[4];
                        Buffer.BlockCopy(rawBytes, index, int32uBytes, 0, 4);

                        index += 4;

                        var paInt32u = new PropertyArray(guid, propertyId, propertyType, int32uBytes);
                        PropertyArrays.Add(paInt32u);

                        break;

                    case OlePropertyType.Int64Unsigned:
                        var int64uBytes = new byte[8];
                        Buffer.BlockCopy(rawBytes, index, int64uBytes, 0, 8);

                        index += 8;

                        var paInt64u = new PropertyArray(guid, propertyId, propertyType, int64uBytes);
                        PropertyArrays.Add(paInt64u);

                        break;

                    case OlePropertyType.Error:
                        var errorBytes = new byte[4];
                        Buffer.BlockCopy(rawBytes, index, errorBytes, 0, 4);

                        index += 4;

                        var paError = new PropertyArray(guid, propertyId, propertyType, errorBytes);
                        PropertyArrays.Add(paError);

                        break;
                    case OlePropertyType.Int8Unsigned:
                        var int8uBytes = new byte[1];
                        Buffer.BlockCopy(rawBytes, index, int8uBytes, 0, 1);

                        index += 1;

                        var paInt8u = new PropertyArray(guid, propertyId, propertyType, int8uBytes);
                        PropertyArrays.Add(paInt8u);

                        break;
                    default:
                        throw new Exception($"unhandled for {propertyType}");
                }
            }


            //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
            //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
        }

        public string FolderName { get; }
        public string FolderName2 { get; }
        public List<PropertyArray> PropertyArrays { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Folder name: {FolderName}");
            if (FolderName2.Equals(FolderName) == false)
            {
                sb.AppendLine($"Folder name 2: {FolderName2}");
            }

            sb.AppendLine();

            sb.AppendLine("Property arrays (Description: Value)");
            foreach (var propArray in PropertyArrays)
            {
                sb.AppendLine($"{propArray.GetDescrption()}: {propArray.PropertyValue()}");

                //                sb.AppendLine(
                //                    $"Guid: {propArray.FormatClassId}, Id: 0x{propArray.PropertyId:X4}, Type: {propArray.PropertyType} ({propArray.GetDescrption()}), Value: {propArray.PropertyValue()}");
            }

            sb.AppendLine();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}