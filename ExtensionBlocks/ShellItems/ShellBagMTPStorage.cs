using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBagMTPStorage : ShellBag
    {
        public ShellBagMTPStorage(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "MTP storage";
            ShellBagType = Utils.ShellBagTypes.MtpStorage;

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

            index = 0x26; //length if device name
            var storageNameSize = BitConverter.ToInt32(rawBytes, index);
            index += 4;
            var storageIdSize = BitConverter.ToInt32(rawBytes, index);
            index += 4;
            var fileSystemNameSize = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            var numWpdEventHandlers = BitConverter.ToInt32(rawBytes, index);
            index += 4; //unknown

            StorageName = Encoding.Unicode.GetString(rawBytes, index, storageNameSize * 2).Trim('\0');
            index += storageNameSize * 2;

            Value = StorageName;

            PersistentUniqueId = Encoding.Unicode.GetString(rawBytes, index, storageIdSize * 2).Trim('\0');
            ;
            index += storageIdSize * 2;

            FileSystemName = Encoding.Unicode.GetString(rawBytes, index, fileSystemNameSize * 2).Trim('\0');
            index += fileSystemNameSize * 2;

            var eventHanderSize = 78 * numWpdEventHandlers;

            var wpdEventHandersRaw = Encoding.Unicode.GetString(rawBytes, index, eventHanderSize);

            WpdEventHandlers = wpdEventHandersRaw.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries).ToList();

            index += eventHanderSize;

            index += 4; //unknown

            var guidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBytes, 0, 16);
            index += 16;

            var portableDeviceGuid = new Guid(guidBytes);
            //PortableDevice = GuidMapping.GuidMapping.GetDescriptionFromGuid(portableDeviceGuid.ToString());

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

                var guidBuff = new byte[16];
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
                        index += 4;
                        if (unknownCount > 0)
                        {
                            //TODO decode this
                            index += unknownCount * 0x10;
                        }

                        //TODO this should pull in all bytes once decoded
                        var paUnknown = new PropertyArray(guid, propertyId, propertyType, new byte[0]);
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

                        if (guid.ToString() == "01a3057a-74d6-4e80-bea7-dc4c212ce50a" && propertyId == 0x0008)
                        {
                            StorageSerialNumber = paUni.PropertyValue();
                        }

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

                    case OlePropertyType.Int32Signed:
                        var int32Bytes = new byte[4];
                        Buffer.BlockCopy(rawBytes, index, int32Bytes, 0, 4);

                        index += 4;

                        var paInt32 = new PropertyArray(guid, propertyId, propertyType, int32Bytes);
                        PropertyArrays.Add(paInt32);

                        break;

                    case OlePropertyType.Int64Unsigned:
                        var int64uBytes = new byte[8];
                        Buffer.BlockCopy(rawBytes, index, int64uBytes, 0, 8);

                        index += 8;

                        var paInt64u = new PropertyArray(guid, propertyId, propertyType, int64uBytes);
                        PropertyArrays.Add(paInt64u);

                        break;

//                    case OlePropertyType.Currency:
//                        var currencyBytes = new byte[8];
//                        Buffer.BlockCopy(rawBytes, index, currencyBytes, 0, 8);
//
//                        var curr = BitConverter.ToUInt64(currencyBytes, 0);
//                        index += 8;
//
//                        var paCurrency = new PropertyArray(guid, propertyId, propertyType, currencyBytes);
//                        propertyArrays.Add(paCurrency);
//
//                        break;
//
//                    case OlePropertyType.Float64Bit:
//                        var doubleBytes = new byte[8];
//                        Buffer.BlockCopy(rawBytes, index, doubleBytes, 0, 8);
//
//                        var iDouble = BitConverter.ToDouble(doubleBytes, 0);
//                        index += 8;
//
//                        var paDouble = new PropertyArray(guid, propertyId, propertyType, doubleBytes);
//                        propertyArrays.Add(paDouble);
//
//                        break;

                    case OlePropertyType.Error:
                        var errorBytes = new byte[4];
                        Buffer.BlockCopy(rawBytes, index, errorBytes, 0, 4);

                        index += 4;

                        var paError = new PropertyArray(guid, propertyId, propertyType, errorBytes);
                        PropertyArrays.Add(paError);

                        break;
//                    case OlePropertyType.Int8Unsigned:
//                        var int8uBytes = new byte[1];
//                        Buffer.BlockCopy(rawBytes, index, int8uBytes, 0, 1);
//
//                        var i8u = int8uBytes[0];
//                        index += 1;
//
//                        var paInt8u = new PropertyArray(guid, propertyId, propertyType, int8uBytes);
//                        propertyArrays.Add(paInt8u);
//
//                        break;
                    default:
                        throw new Exception($"unhandled for {propertyType}");
                }
            }


            //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
            //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
        }

        public List<PropertyArray> PropertyArrays { get; }
        public string FileSystemName2 { get; }


        public string StorageName { get; }
        public string PersistentUniqueId { get; }
        public string FileSystemName { get; }
        public List<string> WpdEventHandlers { get; }
        public string StorageSerialNumber { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Storage name: {StorageName}");

            sb.AppendLine($"Storage serial number: {StorageSerialNumber}");

            sb.AppendLine();

            sb.AppendLine($"File system name: {FileSystemName}");

            sb.AppendLine();

            sb.AppendLine($"Persistent Unique Id: {PersistentUniqueId}");

            sb.AppendLine();

            sb.AppendLine($"WPD Event Handlers {string.Join(",", WpdEventHandlers)}");
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