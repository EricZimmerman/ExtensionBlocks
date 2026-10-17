using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBagCDBurn : ShellBag
    {
        public ShellBagCDBurn(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            ChildShellBags = new List<IShellBag>();

            ShortName = string.Empty;

            PropertyStore = new PropertyStore();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            FriendlyName = "CDBurn";
            ShellBagType = Utils.ShellBagTypes.OpticalMedia;

            var sig = BitConverter.ToInt32(rawBytes, 4);

            if (sig != 0x4d677541)
            {
                throw new Exception("Invalid signature!");
            }

            var index = 8; //reset index to after signature

            var number16BitVals = BitConverter.ToInt32(rawBytes, index);
            index += 4;
            for (var i = 0; i < number16BitVals; i++)
            {
                index += 2;
            }

            //we are now at the start of the next section

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
                var internalIndex = 2; //skip size

                var typeIndicator = bytes[internalIndex];

                internalIndex += 1;
                internalIndex += 1; //skip unknown empty value

                var filesize = BitConverter.ToInt32(bytes, internalIndex);
                internalIndex += 4;

                FileSize = filesize;

                var dateBytes = new byte[4];
                Buffer.BlockCopy(bytes, internalIndex, dateBytes, 0, 4);
                internalIndex += 4;

                var modDate =
                    Utils.ExtractDateTimeOffsetFromBytes(dateBytes);

                LastModificationTime = modDate;

                internalIndex += 2;

                //start of shortname
                var shortName = string.Empty;

                var beefStart = BitConverter.ToInt16(bytes, bytes.Length - 2);
                var strLen = beefStart - 0xe;

                if (typeIndicator == 0x35)
                {
                    shortName = Encoding.Unicode.GetString(bytes, internalIndex, strLen).Trim('\0');

                    internalIndex += strLen;
                }
                else
                {
                    shortName = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(bytes, internalIndex, strLen).Trim('\0');

                    internalIndex += strLen;
                }


                ShortName = shortName;

                while (internalIndex < bytes.Length)
                {
                    var beefLen = BitConverter.ToInt16(bytes, internalIndex);

                    if (beefLen == 0)
                    {
                        internalIndex += 2;
                        continue;
                    }

                    var buff = new byte[beefLen];
                    Buffer.BlockCopy(bytes, internalIndex, buff, 0, beefLen);

                    var extsig = BitConverter.ToUInt32(bytes, internalIndex + 4);
                    var extBlock = Utils.GetExtensionBlockFromBytesOrError(extsig, buff);

                    ExtensionBlocks.Add(extBlock);

                    var beef0004A = extBlock as Beef0004;
                    if (beef0004A != null)
                    {
                        CreatedOnTime = beef0004A.CreatedOnTime;
                        LastAccessTime = beef0004A.LastAccessTime;
                        Value = beef0004A.LongName;
                    }

                    internalIndex += beefLen;
                }

                //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
            }
        }

        public PropertyStore PropertyStore { get; }


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

        public int FileSize { get; }

        public string Miscellaneous { get; private set; }


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


        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ShortName.Length > 0)
            {
                sb.AppendLine($"Short name: {ShortName}");
            }

            if (FileSize > 0)
            {
                sb.AppendLine($"File size: {FileSize:N0}");
            }

            if (LastModificationTime.HasValue)
            {
                sb.AppendLine($"Modified On: {LastModificationTime.Value}");
            }

            if (PropertyStore.Sheets.Count > 0)
            {
                sb.AppendLine("Property Sheets");

                sb.AppendLine(PropertyStore.ToString());
            }
            else
            {
                sb.AppendLine();
            }


            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}