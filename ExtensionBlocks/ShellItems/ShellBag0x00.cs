using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X00 : ShellBag
    {
        public ShellBag0X00(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();


            ShortName = string.Empty;

            FriendlyName = "Variable";
            ShellBagType = Utils.ShellBagTypes.Variable;

            PropertyStores = new List<PropertyStore>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            // There are a few special cases for 0x00 items, so pull a special sig and see if we have one of those
            var specialDataSig = BitConverter.ToUInt32(rawBytes, 4);

            switch (specialDataSig)
            {
                case 0x94606aeb:

                    var unknown = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, 0x14, 0x40);

                    Value = unknown;


                    return;

                case 0xc001b000: //00-B0-01-C0-
                    ProcessURLContainer(rawBytes);

                    return;

                case 0x49534647: // this is a game folder shell item “GFSI”
                    ProcessGameFolderShellItem(rawBytes);

                    return;

                case 0x10:

                    FriendlyName = "ZipFileContents";
                    ShellBagType = Utils.ShellBagTypes.ZipFileContents;
                    var sb = new ShellBag0X65(slot, mruPosition, rawBytes, bagPath);
                    Value = sb.Value;
                    LastModificationTime = sb.ModifiedOn;

                    return;

                case 0xffffff38: // Control panel CPL file shell item

                    throw new Exception("Send this hive to saericzimmerman@gmail.com so support can be added!");

                //  return;
            }

            //if we are here this should be a users property view

            var index = 0;

            var shellItemSize = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            index += 1; // move past signature
            index += 1; // move past unknown

            var dataSize = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            var dataSig = BitConverter.ToUInt32(rawBytes, index);
            index += 4;

            var identifierSize = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            switch (dataSig)
            {
                case 0x0a291cf0:
                    //a collection of property sheets
                    index = 0xa;
                    var sSize = BitConverter.ToInt32(rawBytes, index);
                    while (sSize != 0)
                    {
                        var sBytes = new byte[sSize];
                        Buffer.BlockCopy(rawBytes, index, sBytes, 0, sSize);

                        var sPs = new PropertyStore(sBytes);

                        PropertyStores.Add(sPs);

                        index += sSize;
                        sSize = BitConverter.ToInt32(rawBytes, index);
                    }

                    break;
                case 0x00030005:
                case 0x00000005:

                    ProcessFTPSubItem(rawBytes);

                    break;
                case 0x23febbee:
                    ProcessPropertyViewGUID(rawBytes);

                    return;

                case 0x04152004:
                    var strs = Encoding.Unicode.GetString(rawBytes, 0x76,rawBytes.Length - 0x76).Split('\0');
                    Value = strs.First();
                    return;

                default: // includes known 0xbeebee00
                    if (rawBytes.Length <= 0x60) //as Lnk: longer ones are property views (0x64 took one for a server name)
                    {
                        FriendlyName = "Server name";
                        ShellBagType = Utils.ShellBagTypes.Network;
                        Value = Encoding.Unicode.GetString(rawBytes, 6, rawBytes.Length - 6).Replace("\0", string.Empty);
                    }
                    else
                    {
                        ProcessPropertyViewDefault(rawBytes);
                    }

                    break;
            }
        }

        //    public PropertyStore PropertyStore { get;  set; }
        public List<PropertyStore> PropertyStores { get; set; }

        /// <summary>
        ///     last modified time of BagPath
        /// </summary>
        public DateTimeOffset? LastModificationTime { get; set; }

        /// <summary>
        ///     Created time of BagPath
        /// </summary>
        public DateTimeOffset? CreatedOnTime { get; set; }

        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        public DateTimeOffset? FTPFolderTime { get; set; }

        ///// <summary>
        /////     For files and directories, the MFT entry #
        ///// </summary>
        //public long? MFTEntryNumber { get; set; }

        ///// <summary>
        /////     For files and directories, the MFT sequence #
        ///// </summary>
        //public int? MFTSequenceNumber { get; set; }

        public int FileSize { get; private set; }

        public string Miscellaneous { get; private set; }

        public string FullUrl { get; set; }

        public string ShortName { get; private set; }

        private void ProcessURLContainer(byte[] rawBytes)
        {
            FriendlyName = "Variable: HTTP URI";
            ShellBagType = Utils.ShellBagTypes.Variable;

            var index = 0x14;

            var size = BitConverter.ToUInt32(rawBytes, index);
            index += 4;

            var url = Encoding.Unicode.GetString(rawBytes, index, (int) size);
            index += (int) size;


            Value = Uri.UnescapeDataString(url.Replace("\0", ""));

            //the full URL is 16 bytes on in ShellBags' items and 4 in OneDrive ones in OpenSavePidlMRU (which threw, and
            //the item was lost); the first gap whose size fits is used, and there is none when neither does
            foreach (var gap in new[] {16, 4})
            {
                if (index + gap + 4 > rawBytes.Length)
                {
                    continue;
                }

                size = BitConverter.ToUInt32(rawBytes, index + gap);
                if (size == 0 || size > rawBytes.Length - (index + gap + 4))
                {
                    continue;
                }

                url = Encoding.Unicode.GetString(rawBytes, index + gap + 4, (int) size);

                FullUrl = url.Replace("\0", "");
                break;
            }
        }

        private void ProcessFTPSubItem(byte[] rawBytes)
        {
            FriendlyName = "Variable: FTP URI";
            ShellBagType = Utils.ShellBagTypes.FtpUri;

            var index = 0x16;

            var ft1 = Utils.FromFileTime((long) BitConverter.ToUInt64(rawBytes, index)).ToUniversalTime();

            index += 8;

            var fileTime1 = ft1.ToUniversalTime();

            if (fileTime1.Year > 1601)
            {
                FTPFolderTime = TimeZoneInfo.ConvertTime(fileTime1, Utils.TimeZone);
            }

            index += 8;

            var len1 = 0;

            while (rawBytes[index + len1] != 0x00)
            {
                len1 += 1;
            }

            var s1 = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, index, len1);

            ShortName = s1;

            index += len1;

            while (rawBytes[index] == 0)
            {
                index += 1;
            }


            len1 = 0;

            while (rawBytes[index + len1] != 0x00 || rawBytes[index + len1 + 1] != 0x00)
            {
                len1 += 1;
            }

            s1 = Encoding.Unicode.GetString(rawBytes, index, len1 + 1);


            index += len1 + 1;

            Value = s1;
        }

        public override ShellBagView GetShellBagView()
        {
            var beef4 = ExtensionBlocks.FirstOrDefault(u => u is Beef0004);
            ulong? entry = null;
            int? seq = null;
            DateTimeOffset? la = null;
            DateTimeOffset? co = null;
            if (beef4 != null)
            {
                var b4 = (Beef0004) beef4;
                if (b4.MFTInformation.MFTEntryNumber.HasValue)
                {
                    entry = b4.MFTInformation.MFTEntryNumber.Value;
                }

                if (b4.MFTInformation.MFTSequenceNumber.HasValue)
                {
                    seq = b4.MFTInformation.MFTSequenceNumber.Value;
                }

                la = b4.LastAccessTime;
                co = b4.CreatedOnTime;
            }


            if (MruPosition == 0 && LastWriteTime.HasValue)
            {
                LastInteracted = LastWriteTime.Value;
            }

            var misc = "";

            if (entry > 0 && seq > 0)
            {
                misc = "NTFS file system";
            }

            if (entry > 0 && seq == 0)
            {
                misc = "FAT file system";

                if (la.HasValue)
                {
                    if (la.Value.Minute == 0 && la.Value.Second == 0 && la.Value.Millisecond == 0)
                    {
                        misc = "FAT file system";
                    }
                    else
                    {
                        misc = "exFAT file system";
                    }
                }
            }

            if (entry == 0 && seq == 0)
            {
                misc = "Network/special item";
            }


            return new ShellBagView
            {
                BagPath = BagPath,
                ChildBags = ChildShellBags.Count,
                Value = Value,
                Slot = Slot,
                MRUPosition = MruPosition,
                AbsolutePath = AbsolutePath,
                HexValue = HexValue,
                NodeSlot = NodeSlot,
                InternalID = InternalId,
                ShellType = FriendlyName,
                ModifiedOn = LastModificationTime,
                AccessedOn = la,
                CreatedOn = co,
                LastWriteTime = LastWriteTime,
                MFTEntry = entry,
                MFTSequenceNumber = seq,
                FirstInteracted = FirstInteracted,
                LastInteracted = LastInteracted,
                Miscellaneous = misc
            };
        }


        private void ProcessPropertyViewDefault(byte[] rawBytes)
        {
            //as Lnk reads it (Lnk's was checked against LnkParse3): the value is worked out whether or not extension blocks follow
            FriendlyName = "Variable: Users property view";
            ShellBagType = Utils.ShellBagTypes.User;
            var index = 10;

            var shellPropertySheetListSize = BitConverter.ToInt16(rawBytes, index);

            index += 2;

            var identifiersize = BitConverter.ToInt16(rawBytes, index);

            index += 2;

            if (identifiersize > rawBytes.Length)
            {
                //not a property view: strings
                index = 0xc;

                var strs = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, index, rawBytes.Length - index).Split('\0');

                var p2 = string.Join(",", strs.Skip(1).ToList());

                Value = $"{strs[0]} ({p2})";

                return;
            }

            var identifierData = new byte[identifiersize];

            Array.Copy(rawBytes, index, identifierData, 0, identifiersize);

            index += identifiersize;

            if (shellPropertySheetListSize > 0)
            {
                var propBytes = rawBytes.Skip(index).Take(shellPropertySheetListSize).ToArray();
                var propStore = new PropertyStore(propBytes);

                PropertyStores.Add(propStore);

                //Lnk also adds the extension blocks of the shell items in the Delegate ID List (property 32) to this item.
                //They are those items' (a path: Users, Public, Pictures), not this one's, and ShellBags takes an item's
                //times and MFT entry from its first 0xbeef0004 block, so they are not: the items, with their blocks, are
                //in the property sheet's ExtraData
            }
            else
            {
                //no property sheets: zip contents (a date with /) or CD burning data
                if (rawBytes.Length > 0x28 && (rawBytes[0x28] == 0x2f ||
                                               rawBytes[0x24] == 0x4e && rawBytes[0x26] == 0x2f && rawBytes[0x28] == 0x41))
                {
                    var zip = new ShellBagZipContents(Slot, MruPosition, rawBytes, BagPath);
                    FriendlyName = zip.FriendlyName;
                    ShellBagType = zip.ShellBagType;
                    Value = zip.Value;

                    return;
                }

                if (rawBytes.Length > 8 && rawBytes[4] == 0x41 && rawBytes[5] == 0x75 && rawBytes[6] == 0x67 && rawBytes[7] == 0x4D)
                {
                    var cdb = new ShellBagCDBurn(Slot, MruPosition, rawBytes, BagPath);
                    FriendlyName = cdb.FriendlyName;
                    ShellBagType = cdb.ShellBagType;
                    Value = cdb.Value;

                    return;
                }

                Value = "No Property sheets found"; //(Lnk says !!! Unable to determine Value !!!; this says why)
            }

            index += shellPropertySheetListSize;

            index += 2; //move past end of property sheet terminator

            if (shellPropertySheetListSize > 0 && index + 2 <= rawBytes.Length)
            {
                var extBlockSize = BitConverter.ToInt16(rawBytes, index);

                while (extBlockSize > 0)
                {
                    var extBytes = rawBytes.Skip(index).Take(extBlockSize).ToArray();

                    index += extBlockSize;

                    var signature1 = extBytes.Length >= 8 ? BitConverter.ToUInt32(extBytes, 4) : 0;

                    ExtensionBlocks.Add(Utils.GetExtensionBlockFromBytesOrError(signature1, extBytes));

                    if (index + 2 > rawBytes.Length)
                    {
                        break;
                    }

                    extBlockSize = BitConverter.ToInt16(rawBytes, index);
                }
            }

            if (PropertyStores.Count == 0)
            {
                return;
            }

            var valuestring = (from propertySheet in PropertyStores.First().Sheets
                from propertyName in propertySheet.PropertyNames
                where propertyName.Key == "10"
                select propertyName.Value).FirstOrDefault();

            if (valuestring == null)
            {
                var namesList =
                    (from propertySheet in PropertyStores.First().Sheets
                        from propertyName in propertySheet.PropertyNames
                        select propertyName.Value)
                    .ToList();

                valuestring = string.Join("::", namesList.ToArray());
            }

            if (valuestring == "")
            {
                valuestring = "No Property sheet value found";
            }

            Value = valuestring;
        }
        private void ProcessPropertyViewGUID(byte[] rawBytes)
        {
            // this is a guid

            var index = 4;

            var dataSize = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            index += 4; // move past signature

            var propertyStoreSize = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            var identifierSize = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            if (identifierSize > 0)
            {
                var raw00guid = new byte[16];

                Array.Copy(rawBytes, index, raw00guid, 0, 16);


                var preguid = Utils.ExtractGuidFromShellItem(raw00guid);


                var tempString = GuidMapping.GuidMapping.GetDescriptionFromGuid(preguid);

                Value = tempString;

                index += 16;
            }

            if (propertyStoreSize > 0)
            {
                var newsize = BitConverter.ToInt16(rawBytes, index);
                var propBytes = new byte[newsize];
                Buffer.BlockCopy(rawBytes, index, propBytes, 0, newsize);

                var propStore = new PropertyStore(propBytes);

                PropertyStores.Add(propStore);

                index += newsize;
            }

            index += 2; // skip 2 unknown in common

            if (index == rawBytes.Length)
            {
                return;
            }

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

                    if (rawBytes.Length == index)
                    {
                        return;
                    }

                    extBlockSize = BitConverter.ToInt16(rawBytes, index);
                }
            }

            int terminator = BitConverter.ToInt16(rawBytes, index);

            if (terminator > 0)
            {
                throw new Exception($"Expected terminator of 0, but got {terminator}");
            }
        }

        private void ProcessGameFolderShellItem(byte[] rawBytes)
        {
            FriendlyName = "Variable: Game folder";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ShortName.Length > 0)
            {
                sb.AppendLine($"Short name: {ShortName}");
            }

            if (FTPFolderTime.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine(
                    $"FTP folder time: {FTPFolderTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (FullUrl != null)
            {
                sb.AppendLine($"Full URL: {FullUrl}");
            }

            //TODO denote custom properties vs standard ones
            if (CreatedOnTime.HasValue)
            {
                sb.AppendLine(
                    $"Created On: {CreatedOnTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            //TODO denote custom properties vs standard ones
            if (LastModificationTime.HasValue)
            {
                sb.AppendLine(
                    $"Modified On: {LastModificationTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (LastAccessTime.HasValue)
            {
                sb.AppendLine(
                    $"Accessed On: {LastAccessTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (PropertyStores.Count > 0)
            {
                var psNumber = 0;

                sb.AppendLine();
                sb.AppendLine($"Property stores found: {PropertyStores.Count}");
                foreach (var propertyStore in PropertyStores)
                {
                    if (propertyStore.Sheets.Count > 0)
                    {
                        sb.AppendLine($"Property Sheets for Property store {psNumber:N0}");

                        sb.AppendLine(propertyStore.ToString());
                        //  sb.AppendLine();
                    }
                    psNumber += 1;
                }
            }


            sb.AppendLine();
            sb.AppendLine(base.ToString());


            return sb.ToString();
        }
    }
}