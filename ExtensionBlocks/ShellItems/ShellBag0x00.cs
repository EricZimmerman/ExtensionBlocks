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

                    var unknown = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, 0x14, 0x40);

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
                case 0x00030009:
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

        /// <summary>The first property store (Lnk's 0x00 item had one); an empty one when there is none</summary>
        public PropertyStore PropertyStore => PropertyStores?.FirstOrDefault() ?? new PropertyStore();
        public List<PropertyStore> PropertyStores { get; set; }

        /// <summary>
        ///     For a property view: the extension blocks of the shell items in its Delegate ID List (property 32), deepest
        ///     last. They are those items', not this one's (its own are in ExtensionBlocks)
        /// </summary>
        public List<IExtensionBlock> DelegateExtensionBlocks { get; } = new List<IExtensionBlock>();

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

        /// <summary>For an FTP folder or file, its modified time</summary>
        public DateTimeOffset? FTPFolderTime { get; set; }

        /// <summary>An FTP file (0x00030009), not a folder; FileSize is its size</summary>
        public bool IsFtpFile { get; private set; }

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

        /// <summary>
        ///     An FTP folder or file (a child of an FTP URI item) whatever bytes 2-5 are: they are 0 in some hives and hold other
        ///     values in others (those came in as unknown items of most type bytes, or were misread by the type's class).
        ///     0x00 size, 0x06 0x00030005 (a folder) or 0x00030009 (a file, in ComDlg32's MRUs), 0x0A file attributes, 0x0E
        ///     size, 0x16 modified (FILETIME), 0x1E Unix permissions, 0x26 name (ANSI, null, padded to 4 bytes), name (UTF-16, null)
        /// </summary>
        public static bool IsFtpItem(byte[] rawBytes)
        {
            if (rawBytes.Length < 0x2C || BitConverter.ToUInt32(rawBytes, 6) != FtpFolderSignature &&
                BitConverter.ToUInt32(rawBytes, 6) != FtpFileSignature)
            {
                return false;
            }

            var size = BitConverter.ToUInt16(rawBytes, 0);
            var attributes = BitConverter.ToUInt32(rawBytes, 0x0A);
            if (size < 0x2C || size > rawBytes.Length || attributes == 0 || attributes > 0xFFFF)
            {
                return false;
            }

            var modified = BitConverter.ToInt64(rawBytes, 0x16);
            if (modified != 0 && Utils.FromFileTime(modified) == DateTimeOffset.MinValue)
            {
                return false;
            }

            if (!FtpNames(rawBytes, size, out var ansiLength, out var unicodeStart, out var unicodeLength) || ansiLength == 0 ||
                unicodeLength == 0)
            {
                return false;
            }

            for (var i = unicodeStart; i < unicodeStart + unicodeLength * 2; i += 2)
            {
                if (BitConverter.ToUInt16(rawBytes, i) < 0x20)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Where an FTP folder's names are: the ANSI one at 0x26 (its length without the null), the UTF-16 one after it,
        ///     at the next multiple of 4 from 0x26 (its start and length in characters). False when they don't end within size
        /// </summary>
        private static bool FtpNames(byte[] rawBytes, int size, out int ansiLength, out int unicodeStart, out int unicodeLength)
        {
            ansiLength = 0;
            unicodeStart = 0;
            unicodeLength = 0;

            const int ansiStart = 0x26;
            while (ansiStart + ansiLength < size && rawBytes[ansiStart + ansiLength] != 0)
            {
                ansiLength += 1;
            }

            unicodeStart = ansiStart + (ansiLength + 1 + 3) / 4 * 4;
            if (ansiStart + ansiLength >= size || unicodeStart + 2 > size)
            {
                return false;
            }

            while (unicodeStart + unicodeLength * 2 + 2 <= size && BitConverter.ToUInt16(rawBytes, unicodeStart + unicodeLength * 2) != 0)
            {
                unicodeLength += 1;
            }

            return unicodeStart + unicodeLength * 2 + 2 <= size;
        }

        private const uint FtpFolderSignature = 0x00030005;
        private const uint FtpFileSignature = 0x00030009;

        private void ProcessFTPSubItem(byte[] rawBytes)
        {
            FriendlyName = "Variable: FTP URI";
            ShellBagType = Utils.ShellBagTypes.FtpUri;

            //a file (opened from an FTP folder in a file dialog): its size is at 0x0E (it was read as a server name, its
            //Value bytes 6 on as UTF-16)
            if (BitConverter.ToUInt32(rawBytes, 6) == FtpFileSignature)
            {
                FriendlyName = "Variable: FTP file";
                IsFtpFile = true;
                var fileSize = BitConverter.ToUInt64(rawBytes, 0x0E);
                FileSize = fileSize > int.MaxValue ? int.MaxValue : (int) fileSize;
            }

            var index = 0x16;

            var ft1 = Utils.FromFileTime((long) BitConverter.ToUInt64(rawBytes, index)).ToUniversalTime();

            var fileTime1 = ft1.ToUniversalTime();

            if (fileTime1.Year > 1601)
            {
                FTPFolderTime = TimeZoneInfo.ConvertTime(fileTime1, Utils.TimeZone);
            }

            var size = Math.Min((int) BitConverter.ToUInt16(rawBytes, 0), rawBytes.Length);

            //the UTF-16 name starts at the next multiple of 4 after the ANSI one (a search for the first byte that isn't 0
            //missed characters whose low byte is 0, and read past the end of damaged items)
            if (!FtpNames(rawBytes, size, out var ansiLength, out var unicodeStart, out var unicodeLength))
            {
                throw new Exception("The names of an FTP folder item don't end within the item");
            }

            ShortName = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, 0x26, ansiLength);

            Value = Encoding.Unicode.GetString(rawBytes, unicodeStart, unicodeLength * 2);
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

                var strs = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, rawBytes.Length - index).Split('\0');

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

                //the extension blocks of the shell items in the Delegate ID List (property 32): those items' (a path:
                //Users, Public, Pictures), not this one's, so they are kept apart. ShellBags takes an item's times and MFT
                //entry from its first 0xbeef0004 block, which would be the first folder's; LECmd and JLECmd take the
                //target's MFT entry from the last one, the deepest item's (Lnk added them to the item's blocks)
                if (propStore.Sheets.Any(t => t.PropertyNames.ContainsKey("32")))
                {
                    foreach (var extOffset in Utils.FindExtensionBlockSignatures(propBytes))
                    {
                        var binaryOffset = extOffset - 4;
                        if (binaryOffset < 0)
                        {
                            continue;
                        }

                        var exSize = BitConverter.ToInt16(propBytes, binaryOffset);
                        var exBytes = propBytes.Skip(binaryOffset).Take(exSize).ToArray();

                        DelegateExtensionBlocks.Add(Utils.GetExtensionBlockFromBytesOrError(0, exBytes));
                    }
                }
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
                    $"FTP {(IsFtpFile ? "file" : "folder")} time: {FTPFolderTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (IsFtpFile)
            {
                sb.AppendLine($"File size: {FileSize:N0}");
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