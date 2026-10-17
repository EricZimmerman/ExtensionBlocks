using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X31 : ShellBag
    {
        public ShellBag0X31(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "Directory";
            ShellBagType = Utils.ShellBagTypes.Directory;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 2;


            index += 1;

            //skip unknown byte
            index += 1;

            index += 4; // skip file size since always 0 for directory

            LastModificationTime = Utils.ExtractDateTimeOffsetFromBytes(rawBytes.Skip(index).Take(4).ToArray());

            index += 4;

            index += 2;

            var len = 0;


            //where the 0xbeef0004 block's signature is (0 when it isn't there), found in the bytes (it was found in a hex string of them)
            var beefPos = Math.Max(0, Utils.IndexOf(rawBytes, Utils.Beef0004Signature));

            if (beefPos == 0)
            {
                var hackName = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, index, rawBytes.Length - index);

                var segs = hackName.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);

                ShortName = string.Join("|", segs);

                Value = ShortName;
                return;
            }


            beefPos = beefPos - 4; //add header back for beef

            var strLen = beefPos - index;

            if (rawBytes[2] == 0x35|| rawBytes[2] == 0x36)
            {
                len = strLen;
            }
            else
            {
                while (rawBytes[index + len] != 0x0)
                {
                    len += 1;
                }
            }

            var tempBytes = new byte[len];
            Array.Copy(rawBytes, index, tempBytes, 0, len);

            var shortName = "";

            if (rawBytes[2] == 0x35|| rawBytes[2] == 0x36)
            {
                shortName = Encoding.Unicode.GetString(tempBytes);
            }
            else
            {
                shortName = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(tempBytes);
            }

            ShortName = shortName;

            Value = shortName;

            index = beefPos;

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

                if (block.Signature.ToString("X").StartsWith("BEEF00", StringComparison.Ordinal))
                {
                    ExtensionBlocks.Add(block);
                }


                var beef0004 = block as Beef0004;
                if (beef0004 != null && beef0004.LongName.Length > 0)
                {
                    Value = beef0004.LongName;
                }

                var beef0005 = block as Beef0005;
                if (beef0005 != null)
                {
                    foreach (var internalBag in beef0005.InternalBags)
                    {
                        ExtensionBlocks.Add(new BeefPlaceHolder(null));
                    }
                }

                index += extsize;
            }
        }

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

            if (entry > 0 && seq == null)
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

            if (entry == null && seq == null)
            {
                misc = "Network/special item";
            }


            return new ShellBagView
            {
                BagPath = BagPath,
                ChildBags = ChildShellBags.Count,
                Value = Value,
                Slot = Slot,
                AbsolutePath = AbsolutePath,
                MRUPosition = MruPosition,
                HexValue = HexValue,
                NodeSlot = NodeSlot,
                InternalID = InternalId,
                ShellType = FriendlyName,
                AccessedOn = la,
                CreatedOn = co,
                LastWriteTime = LastWriteTime,
                MFTEntry = entry,
                MFTSequenceNumber = seq,
                ModifiedOn = LastModificationTime,
                FirstInteracted = FirstInteracted,
                LastInteracted = LastInteracted,
                Miscellaneous = misc,
                HasExplored = HasExplored
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ShortName.Length > 0)
            {
                sb.AppendLine($"Short name: {ShortName}");
            }

            if (LastModificationTime.HasValue)
            {
                sb.AppendLine(
                    $"Modified: {LastModificationTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (LastAccessTime.HasValue)
            {
                sb.AppendLine(
                    $"Last Access: {LastAccessTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            sb.AppendLine();
            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}