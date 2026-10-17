using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBagCFSF : ShellBag
    {
        public ShellBagCFSF(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Users Files Folder";
            ShellBagType = Utils.ShellBagTypes.User;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 2;

            index += 2; // move past type  and an unknown

            var size = BitConverter.ToUInt16(rawBytes, index);

            index += 2;

            if (BitConverter.ToInt32(rawBytes, 6) != 0x46534643)
            {
                throw new Exception($"Invalid signature at offset 0x06! Should be CFSF!");
            }

            index += 4;

            var subShellSize = BitConverter.ToUInt16(rawBytes, index);
            index += 2;

            var subClasstype = rawBytes[index];
            index += 1;

            index += 1; // skip unknown

            var filesize = BitConverter.ToUInt32(rawBytes, index);

            index += 4;

            FileSize = (int) filesize;

            var tempBytes = new byte[4];
            Array.Copy(rawBytes, index, tempBytes, 0, 4);

            index += 4;

            LastModificationTime = Utils.ExtractDateTimeOffsetFromBytes(tempBytes);

            index += 2; //skip file attribute flag

            var len = 0;

            while (rawBytes[index + len] != 0x0)
            {
                len += 1;
            }

            tempBytes = new byte[len];
            Array.Copy(rawBytes, index, tempBytes, 0, len);

            index += len;

            var primaryName = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(tempBytes);

            while (rawBytes[index] == 0x0)
            {
                index += 1;
            }

            var delegateGuidRaw = new byte[16];

            Array.Copy(rawBytes, index, delegateGuidRaw, 0, 16);

            var delegateGuid = Utils.ExtractGuidFromShellItem(delegateGuidRaw);

            //5e591a74-df96-48d3-8d67-1733bcee28ba

            if (delegateGuid != "5e591a74-df96-48d3-8d67-1733bcee28ba")
            {
                throw new Exception(
                    $"Delegate guid not expected value of 5e591a74-df96-48d3-8d67-1733bcee28ba. Actual value: {delegateGuid}");
            }

            index += 16;

            var itemIdentifierGuidRaw = new byte[16];
            Array.Copy(rawBytes, index, itemIdentifierGuidRaw, 0, 16);

            var itemIdentifierGuid = Utils.ExtractGuidFromShellItem(itemIdentifierGuidRaw);

            var itemName = GuidMapping.GuidMapping.GetDescriptionFromGuid(itemIdentifierGuid);
            index += 16;

            //0xbeef0004 section

            //we are at extensnon blocks, so cut them up and process
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
                        var shellBuff = new byte[subshellitemdatasize];
                        Buffer.BlockCopy(rawBytes, index, shellBuff, 0, subshellitemdatasize);

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

        ///// <summary>
        /////     Created time of BagPath
        ///// </summary>
        //public DateTimeOffset? CreatedOnTime { get; set; }

        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        ///// <summary>
        /////     For files and directories, the MFT entry #
        ///// </summary>
        //public long? MFTEntryNumber { get; set; }

        ///// <summary>
        /////     For files and directories, the MFT sequence #
        ///// </summary>
        //public int? MFTSequenceNumber { get; set; }

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
                LastWriteTime = LastWriteTime,
                MFTEntry = entry,
                MFTSequenceNumber = seq,
                ModifiedOn = LastModificationTime,
                CreatedOn = co,
                AccessedOn = la,
                FirstInteracted = FirstInteracted,
                LastInteracted = LastInteracted,
                Miscellaneous = misc
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (FileSize > 0)
            {
                sb.AppendLine($"File size: {FileSize:N0}");
            }

            if (LastModificationTime.HasValue)
            {
                sb.AppendLine($"Modified On: {LastModificationTime.Value}");
                sb.AppendLine();
            }

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}