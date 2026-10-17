using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X32 : ShellBag
    {
        private static bool HasBeef0004(byte[] rawBytes)
        {
            for (var i = 0; i + 4 <= rawBytes.Length; i++)
            {
                if (rawBytes[i] == 0x04 && rawBytes[i + 1] == 0x00 && rawBytes[i + 2] == 0xef && rawBytes[i + 3] == 0xbe)
                {
                    return true;
                }
            }

            return false;
        }

        public ShellBag0X32(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "File";
            ShellBagType = Utils.ShellBagTypes.File;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            ShortName = string.Empty;

            BagPath = bagPath;


            var index = 2;


            //offset 4 is the file size in a file entry, so 0x10 there is not enough: a file entry has a 0xbeef0004 block
            //(both corpus items with 0x10 were files, 250k.zip and WINHEX~2.ZIP, and failed to parse)
            if (rawBytes[4] == 0x10 && HasBeef0004(rawBytes) == false)
            {
                FriendlyName = "ZipFileContents";
                ShellBagType = Utils.ShellBagTypes.ZipFileContents;
                
                 index = 16;
                LastModificationTime = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index)).ToUniversalTime();


                index = 28;

                var len1 = BitConverter.ToInt16(rawBytes, index);
                var len2 = BitConverter.ToInt16(rawBytes, index+2);
                
                index = 32;

               //the lengths can be damaged: what is there
               var nameLength = Math.Min(Math.Max(0, (len1 + len2) * 2), Math.Max(0, rawBytes.Length - index));
               var name = Encoding.Unicode.GetString(rawBytes, index, nameLength).Replace("\0", "");
               Value = name;
               return;
            }

           


            index += 1;

            //skip unknown byte
            index += 1;

            var fileSize = BitConverter.ToUInt32(rawBytes, index);


            FileSize = (int) fileSize;

            index += 4; // skip file size since always 0 for directory

            var tempBytes = new byte[4];
            Array.Copy(rawBytes, index, tempBytes, 0, 4);
            var lastmodifiedUtcRaw = tempBytes;

            LastModificationTime = Utils.ExtractDateTimeOffsetFromBytes(lastmodifiedUtcRaw);

            index += 4;

            //   string fileAttribues = BitConverter.ToString(rawBytes, index, 2);


            index += 2;

            var len = 0;

            while (index + len < rawBytes.Length && rawBytes[index + len] != 0x0)
            {
                len += 1;
            }

            tempBytes = new byte[len];
            Array.Copy(rawBytes, index, tempBytes, 0, len);

            index += len;

            var shortName = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(tempBytes);

            ShortName = shortName;

            //where the 0xbeef0004 block's signature is (0 when it isn't there), looked for after the name so bytes in the
            //header or name can't be taken for it (from Lnk)
            var beefPos = Math.Max(0, Utils.IndexOf(rawBytes, Utils.Beef0004Signature, index));

            if (beefPos == 0)
            {
                var hackName = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, index, rawBytes.Length - index);

                var segs = hackName.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

                ShortName = string.Join("|", segs);

                Value = ShortName;
                return;
            }

            index = beefPos - 4;//header

            //we are at extension blocks, so cut them up and process
            // here is where we need to cut up the rest into extension blocks
            var chunks = new List<byte[]>();

            while (index < rawBytes.Length)
            {
                var subshellitemdatasize = BitConverter.ToInt16(rawBytes, index);
                //index += 2;

                if (subshellitemdatasize == 0)
                {
                    index += 2; //some kind of separator
                }

                if (subshellitemdatasize == 1)
                {
                    //some kind of separator
                    index += 2;
                }
                else
                {
                    if (subshellitemdatasize > 0)
                    {
                        //a block longer than what is left is read as far as it goes (as Lnk does), not an error that loses the item
                        var shellBuff = new byte[Math.Min(subshellitemdatasize, rawBytes.Length - index)];
                        Buffer.BlockCopy(rawBytes, index, shellBuff, 0, shellBuff.Length);

                        chunks.Add(shellBuff);
                        index += subshellitemdatasize;
                    }
                    else if (subshellitemdatasize < 0)
                    {
                        //not a size (damaged data): the index would not move
                        break;
                    }
                }
            }

            foreach (var bytes in chunks)
            {
                index = 0;

                var extsize = BitConverter.ToInt16(bytes, index);

                var signature = BitConverter.ToUInt32(bytes, 0x04);

                //TODO does this need to check if its a 0xbeef?? regex?
                var block = Utils.GetExtensionBlockFromBytesOrError(signature, bytes);

                ExtensionBlocks.Add(block);

                var beef0004 = block as Beef0004;
                if (beef0004 != null)
                {
                    Value = beef0004.LongName;
                }

                index += extsize;
            }
        }

        public int FileSize { get; }

        /// <summary>
        ///     last modified time of BagPath
        /// </summary>
        public DateTimeOffset? LastModificationTime { get; set; }


        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        public string ShortName { get; }

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
                AccessedOn = la,
                CreatedOn = co,
                ModifiedOn = LastModificationTime,
                LastWriteTime = LastWriteTime,
                MFTEntry = entry,
                MFTSequenceNumber = seq,
                FirstInteracted = FirstInteracted,
                LastInteracted = LastInteracted,
                Miscellaneous = misc
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ShortName.Length > 0)
            {
                sb.AppendLine($"Short name: {ShortName}");
            }

            if (FileSize > 0)
            {
                sb.AppendLine();

                sb.AppendLine($"File size: {FileSize:N0}");
            }

            //TODO denote custom properties vs stadard ones
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

            sb.AppendLine();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}