using System;

namespace ExtensionBlocks
{
    /// <summary>
    ///     The shell item class for an item's bytes (moved from ShellBags): its signatures first (CD burning, MTP device,
    ///     storage and folder, CFSF, find data, FTP folder or file, document drive, archive contents, zip contents), then the class for its type byte (ShellBag0X{type}), else
    ///     ShellBagUnknown. An item that can't be parsed throws: callers record it as damaged
    /// </summary>
    public static class ShellItemFactory
    {
        private const int AugmSignature = 0x4d677541;
        private const int GfsiSignature = 0x49534647; // a game folder shell item, "GFSI" at offset 4 (ShellBag0X00 reads the 0x00 ones)
        private const uint ControlPanelCplSignature = 0xffffff38;
        private const uint MtpDeviceSignature = 0x08312006;
        private const uint MtpStorageSignature = 0x10312005;
        private const uint MtpFolderSignature = 0x07192006;
        private const uint CfsfSignature = 0x46534643;

        /// <param name="rawBytes">The item, starting at its 2 byte size</param>
        /// <param name="slot">Where it is (the value name in a BagMRU key; -1 elsewhere)</param>
        /// <param name="mruPosition">Its position in an MRU list (-1 elsewhere)</param>
        /// <param name="bagPath">The key it is in (empty elsewhere)</param>
        public static IShellBag Create(byte[] rawBytes, int slot = -1, int mruPosition = -1, string bagPath = "")
        {
            return CreateItem(rawBytes, slot, mruPosition, bagPath);
        }

        /// <summary>As Create, with the item's ANSI strings read with codepage (Utils.CodePage while it is read)</summary>
        public static IShellBag Create(byte[] rawBytes, int slot, int mruPosition, string bagPath, int codepage)
        {
            var previous = Utils.CodePage;
            Utils.CodePage = codepage;
            try
            {
                return CreateItem(rawBytes, slot, mruPosition, bagPath);
            }
            finally
            {
                Utils.CodePage = previous;
            }
        }

        /// <summary>A Unicode MM/dd/yyyy  HH:mm:ss at offset, as zip contents have</summary>
        private static bool HasZipDate(byte[] rawBytes, int offset)
        {
            const string pattern = "00/00/0000  00:00:00";
            if (rawBytes.Length < offset + pattern.Length * 2)
            {
                return false;
            }

            for (var i = 0; i < pattern.Length; i++)
            {
                var c = (char) BitConverter.ToUInt16(rawBytes, offset + i * 2);
                if (pattern[i] == '0' ? c < '0' || c > '9' : c != pattern[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static IShellBag CreateItem(byte[] rawBytes, int slot, int mruPosition, string bagPath)
        {
            if (rawBytes == null || rawBytes.Length < 3)
            {
                throw new Exception($"A shell item has at least 3 bytes, not {rawBytes?.Length ?? 0}");
            }

            if (rawBytes.Length >= 8)
            {
                var signature = BitConverter.ToInt32(rawBytes, 4);

                if (BitConverter.ToUInt32(rawBytes, 4) == ControlPanelCplSignature || signature == GfsiSignature && rawBytes[2] != 0x00)
                {
                    throw new Exception("Unsupported shell item. Send it to saericzimmerman@gmail.com so support can be added!");
                }

                if (signature == AugmSignature)
                {
                    return new ShellBagCDBurn(slot, mruPosition, rawBytes, bagPath);
                }
            }

            if (rawBytes.Length >= 10)
            {
                switch (BitConverter.ToUInt32(rawBytes, 6))
                {
                    case MtpDeviceSignature:
                        return new ShellBagMTPDevice(slot, mruPosition, rawBytes, bagPath);
                    case MtpStorageSignature:
                        return new ShellBagMTPStorage(slot, mruPosition, rawBytes, bagPath);
                    case MtpFolderSignature:
                        return new ShellBagMTPFolder(slot, mruPosition, rawBytes, bagPath);
                    case CfsfSignature:
                        return new ShellBagCFSF(slot, mruPosition, rawBytes, bagPath);
                }
            }

            //found by their layout whatever the type byte: archive contents' bytes 2-3 vary (ShellBags sent them as unknown types of
            //every value, and other types misread them, some as an unmapped GUID); so do FTP folders' and files' bytes 2-5 and find data
            //items' bytes 2-3
            if (ShellBagFindData.IsFindData(rawBytes))
            {
                return new ShellBagFindData(slot, mruPosition, rawBytes, bagPath);
            }

            if (ShellBag0X00.IsFtpItem(rawBytes))
            {
                return new ShellBag0X00(slot, mruPosition, rawBytes, bagPath);
            }

            if (ShellBagDocumentDrive.IsDocumentDrive(rawBytes))
            {
                return new ShellBagDocumentDrive(slot, mruPosition, rawBytes, bagPath);
            }

            if (ShellBagArchiveContents.IsArchiveContents(rawBytes))
            {
                return new ShellBagArchiveContents(slot, mruPosition, rawBytes, bagPath);
            }

            //zip contents: a / in a date (or N/A) at one of these offsets; byte 8 is 0, or there is a whole date at 0x24 (some have
            //data in bytes 4-9: they were unknown items of type 0x6C, 0x7F and 0x8A)
            if (rawBytes.Length > 0x28 && (rawBytes[0x8] == 0 || HasZipDate(rawBytes, 0x24)) &&
                (rawBytes[0x28] == 0x2f || rawBytes[0x26] == 0x2f || rawBytes[0x1a] == 0x2f || rawBytes[0x1c] == 0x2f))
            {
                try
                {
                    return new ShellBagZipContents(slot, mruPosition, rawBytes, bagPath);
                }
                catch (Exception)
                {
                    //not zip contents after all
                }
            }

            var type = typeof(ShellItemFactory).Assembly.GetType($"ExtensionBlocks.ShellBag0X{rawBytes[2]:X2}");

            if (type == null)
            {
                //types with no class of their own that Lnk reads with another type's
                switch (rawBytes[2])
                {
                    case 0x22 when rawBytes.Length > 4 && char.IsLetter((char) rawBytes[3]) && rawBytes[4] == ':':
                        //a volume item as 0x23 (a drive letter), when it has one (ShellBags has damaged data with 0x22)
                        return new ShellBag0X23(slot, mruPosition, rawBytes, bagPath);
                    case 0x77:
                        return new ShellBag0X74(slot, mruPosition, rawBytes, bagPath);
                    case 0xaa:
                    case 0xae:
                        //zip contents in LNK files; ShellBags' 0xAA items are something else (still unknown)
                        try
                        {
                            var zip = new ShellBagZipContents(slot, mruPosition, rawBytes, bagPath);
                            if (zip.Value != null && zip.Value.Contains("Unable to determine value") == false)
                            {
                                return zip;
                            }
                        }
                        catch (Exception)
                        {
                            //not zip contents
                        }

                        break;
                }

                return new ShellBagUnknown(slot, mruPosition, rawBytes, bagPath);
            }

            try
            {
                return (IShellBag) Activator.CreateInstance(type, slot, mruPosition, rawBytes, bagPath);
            }
            catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
            {
                //the parser's own exception, not reflection's
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
