using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtensionBlocks
{
    public class PropertyArray
    {
        public PropertyArray(Guid formatClassId, int propertyId, OlePropertyType propertyType, byte[] propertyBytes)
        {
            FormatClassId = formatClassId;
            PropertyId = propertyId;
            PropertyType = propertyType;
            PropertyBytes = propertyBytes;
        }

        public Guid FormatClassId { get; }

        public int PropertyId { get; }

        public OlePropertyType PropertyType { get; }

        /// <summary>
        ///     Bytes for property.
        /// </summary>
        /// <remarks>Must be interpreted according to PropertyType</remarks>
        public byte[] PropertyBytes { get; }

        public string PropertyValue()
        {
            switch (PropertyType)
            {
                case OlePropertyType.Unknown:

                    if (PropertyBytes.Length == 0)
                    {
                        return $"(No data present)";
                    }
                    var index = 0;
                    var gBuff1 = new byte[16];
                    Buffer.BlockCopy(PropertyBytes, 0, gBuff1, 0, 16);
                    index += 16;

                    var g1 = new Guid(gBuff1);
                    var gf1 = GuidMapping.GuidMapping.GetDescriptionFromGuid(g1.ToString());

                    var unknownCount = BitConverter.ToInt32(PropertyBytes, index);
                    index += 4;
                    if (unknownCount > 0)
                    {
                        //TODO decode this
                        index += unknownCount * 0x10;
                    }
//
//                    //TODO this should pull in all bytes once decoded
//                    var paUnknown = new PropertyArray(guid, propertyId, propertyType, new byte[0]);
//                    propertyArrays.Add(paUnknown);

                    return $"Guid: {g1} ({gf1}) Items: {unknownCount}";

                case OlePropertyType.OleDate:
                    var mdou = BitConverter.ToDouble(PropertyBytes, 0);

                    var timestampDT = DateTime.FromOADate(mdou);

                    var dto = new DateTimeOffset(timestampDT).ToUniversalTime();

                    //TODO verify this
                    //  return timestampDT.ToString(Utils.GetDateTimeFormatWithMilliseconds());
                    return dto.ToString(Utils.GetDateTimeFormatWithMilliseconds());


                case OlePropertyType.Boolean:

                    var isTrue = BitConverter.ToInt16(PropertyBytes, 0) != 0x00;

                    return isTrue.ToString();

                case OlePropertyType.Guid:

                    var g = new Guid(PropertyBytes);
                    var gf = GuidMapping.GuidMapping.GetDescriptionFromGuid(g.ToString());

                    return $"Guid: {g} ({gf})";

                case OlePropertyType.UnicodeString:
                    return Encoding.Unicode.GetString(PropertyBytes).Trim('\0');

                case OlePropertyType.Int16Unsigned:
                    return BitConverter.ToUInt16(PropertyBytes, 0).ToString("N0");

                case OlePropertyType.Int32Unsigned:
                    return BitConverter.ToUInt32(PropertyBytes, 0).ToString("N0");

                case OlePropertyType.Int32Signed:

                    return BitConverter.ToInt32(PropertyBytes, 0).ToString("N0");

                case OlePropertyType.Int64Unsigned:
                    return BitConverter.ToUInt64(PropertyBytes, 0).ToString("N0");

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

                    return BitConverter.ToUInt32(PropertyBytes, 0).ToString("N0");
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
                case OlePropertyType.Int8Unsigned:
                    return PropertyBytes[0].ToString();
                default:
                    throw new Exception($"Unhandled PropertyType: {PropertyType}");
            }
        }

        public static string SplitCamelCase(string input)
        {
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        public string GetDescrption()
        {
            switch (FormatClassId.ToString())
            {
                case "01a3057a-74d6-4e80-bea7-dc4c212ce50a":
                    var oneADesc = (PropertyEnumType_01a3057a_74d6_4e80_bea7_dc4c212ce50a) PropertyId;
                    return SplitCamelCase(oneADesc.ToString());

                case "2ed8ba05-0ad3-42dc-b0d0-bc95ac396ac8":
                    var twoEDesc = (PropertyEnumType_2ed8ba05_0ad3_42dc_b0d0_bc95ac396ac8) PropertyId;
                    return SplitCamelCase(twoEDesc.ToString());

                case "8f052d93-abca-4fc5-a5ac-b01df4dbe598":
                    var eightF = (PropertyIdType_8f052d93_abca_4fc5_a5ac_b01df4dbe598) PropertyId;
                    return SplitCamelCase(eightF.ToString());

                case "ef6b490d-5cd8-437a-affc-da8b60ee4a3c":
                    var eF = (PropertyIdType_ef6b490d_5cd8_437a_affc_da8b60ee4a3c) PropertyId;
                    return SplitCamelCase(eF.ToString());

                case "4d545058-4fce-4578-95c8-8698a9bc0f49":
                    var fourD = (PropertyEnumType_4d545058_4fce_4578_95c8_8698a9bc0f49) PropertyId;

                    if (fourD != PropertyEnumType_4d545058_4fce_4578_95c8_8698a9bc0f49.Timestamp)
                    {
                        return $"Unknown Description (TypeId: {PropertyType})";
                    }

                    return SplitCamelCase(fourD.ToString());

                default:
                    return "Unknown";
            }
        }
    }

    public enum PropertyEnumType_4d545058_4fce_4578_95c8_8698a9bc0f49
    {
        Timestamp = 0xdc4e
    }

    public enum PropertyEnumType_01a3057a_74d6_4e80_bea7_dc4c212ce50a
    {
        StorageType = 0x0002,
        StorageFileSystem = 0x0003,
        StorageCapacity = 0x0004,
        StorageFreeSpaceBytes = 0x0005,
        StorageFreeSpaceObjects = 0x0006,
        StorageDescription = 0x0007,
        StorageSerialNumber = 0x0008,
        StorageMaxObjectSize = 0x0009,
        StorageCapacityInObjects = 0x000a,
        StorageAccessCapability = 0x000b
    }

    public enum PropertyEnumType_2ed8ba05_0ad3_42dc_b0d0_bc95ac396ac8
    {
        MediaBitrate = 0x0002,
        MediaBitRateType = 0x0003,
        MediaCopyright = 0x0004,
        MediaSubscriptionContentId = 0x0005,
        MediaUseCount = 0x0006,
        MediaSkipCount = 0x0007,
        MediaLastAccessedTime = 0x0008,
        MediaParentalRating = 0x0009,
        MediaMetaGenre = 0x000a,
        MediaComposer = 0x000b,
        MediaEffectiveRating = 0x000c,
        MediaSubTitle = 0x000d,
        MediaReleaseDate = 0x000e,
        MediaSampleRate = 0x000f,
        MediaStarRating = 0x00010,
        MediaUserEffectiveRating = 0x00011,
        MediaTitle = 0x00012,
        MediaDuration = 0x00013,
        MediaBuyNow = 0x00014,
        MediaEncodingProfile = 0x00015,
        MediaWidth = 0x00016,
        MediaHeight = 0x00017,
        MediaArtist = 0x00018,
        MediaAlbumArtist = 0x00019,
        MediaOwner = 0x0001a,
        MediaManagingEditor = 0x0001b,
        MediaWebmaster = 0x0001c,
        MediaSourceUrl = 0x0001d,
        MediaDestinationUrl = 0x0001e,
        MediaDescription = 0x0001f,
        MediaGenre = 0x00020,
        MediaTimeBookmark = 0x00021,
        MediaObjectBookmark = 0x00022,
        MediaLastBuildDate = 0x00023,
        MediaByteBookmark = 0x00024,
        MediaTimeToLive = 0x00025,
        MediaGuid = 0x00026,
        MediaSubDescription = 0x00027,
        MediaAudioEncodingProfile = 0x00031
    }

    public enum PropertyIdType_8f052d93_abca_4fc5_a5ac_b01df4dbe598
    {
        WpdFunctionalObjectCategory = 0x0002
    }

    public enum PropertyIdType_ef6b490d_5cd8_437a_affc_da8b60ee4a3c
    {
        ObjectId = 0x0002,
        ObjectParent = 0x0003,
        ObjectName = 0x0004,
        ObjectPersistentUniqueId = 0x0005,
        ObjectFormat = 0x0006,
        ObjectContentType = 0x0007,
        ObjectHidden = 0x0009,
        ObjectIsSystem = 0x000a,
        ObjectSize = 0x000b,
        ObjectOriginalFileName = 0x000c,
        ObjectNonConsumable = 0x000d,
        ObjectReferences = 0x000e,
        ObjectKeywords = 0x000f,
        ObjectSyncId = 0x0010,
        ObjectIsDrmProtected = 0x0011,
        ObjectCreatedOn = 0x0012,
        ObjectModifiedOn = 0x0013,
        ObjectAuthoredOn = 0x0014,
        ObjectBackReferences = 0x0015,

        ObjectContainerFunctionalObjectId = 0x0017,
        ObjectGenerateThumbnailFromResource = 0x0018,
        ObjectHintLocationDisplayName = 0x0019,
        ObjectCanDelete = 0x001a,
        ObjectLanguageLocale = 0x001b
    }

    public enum OlePropertyType
    {
        Empty = 0x0000,
        Null = 0x0001,
        Int16Signed = 0x0002,
        Int32Signed = 0x0003,
        Float32Bit = 0x0004,
        Float64Bit = 0x0005,
        Currency = 0x0006,
        OleDate = 0x0007,
        BinaryString = 0x0008,
        Dispatch = 0x0009,
        Error = 0x000a,
        Boolean = 0x000b, //0x00 == false, else true
        Variant = 0x000c,
        Unknown = 0x000d,
        Decimal = 0x000e,
        Unknown2 = 0x000f,
        Int8Signed = 0x0010,
        Int8Unsigned = 0x0011,
        Int16Unsigned = 0x0012,
        Int32Unsigned = 0x0013,
        Int64Signed = 0x0014,
        Int64Unsigned = 0x0015,
        IntSigned = 0x0016, //32 bit in storage
        IntUnsigned = 0x0017,
        Void = 0x0018,
        StandardReturn = 0x0019,
        Pointer = 0x001a,
        SafeArray = 0x001b,
        CAarray = 0x001c,
        UserDefined = 0x001d,
        AsciiString = 0x001e,
        UnicodeString = 0x001f,
        Unknown3 = 0x0020,
        Unknown4 = 0x0021,
        Unknown5 = 0x0022,
        Unknown6 = 0x0023,
        BRecord = 0x0024,
        Int16Or32Signed = 0x0025,
        Int16Or32Unsigned = 0x0026,
        Unknown10 = 0x0028,
        Unknown11 = 0x002b,
        Unknown12 = 0x002c,
        Unknown13 = 0x002e,
        Unknown14 = 0x002f,
        Unknown15 = 0x0030,
        Unknown16 = 0x0031,
        Unknown17 = 0x0033,
        Unknown18 = 0x0035,
        Unknown19 = 0x0036,
        Unknown20 = 0x0038,
        Unknown21 = 0x003f,
        FileTime = 0x0040,
        Blob = 0x0041, //size prefixed binary data
        Stream = 0x0042,
        Storage = 0x0043,
        StreamedObject = 0x0044,
        StoragedObject = 0x0045,
        BlobObject = 0x0046,
        Clipboard = 0x0047,
        Guid = 0x0048,
        VersionedStream = 0x0049,
        Unknown22 = 0x004a,
        Unknown23 = 0x004b,
        Unknown24 = 0x006f,
        Unknown25 = 0x0074,
        Unknown26 = 0x0075,
        Unknown27 = 0x007f,
        Unknown28 = 0x0081,
        Unknown29 = 0x0092,
        Unknown30 = 0x009f,
        Unknown31 = 0x00e8
    }
}