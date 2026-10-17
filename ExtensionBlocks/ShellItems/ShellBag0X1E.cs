using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X1E : ShellBag
    {
        public ShellBag0X1E(int slot, int mruPosition, byte[] rawBytes, string bagPath)
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

            var index = 0;

            if (rawBytes[5] != 0x14)
            {
                index = 0x26;
                var len = 0;
                while (rawBytes[index + len] != 0)
                {
                    len += 1;
                }

                Value = CodePagesEncodingProvider.Instance.GetEncoding(Utils.CodePage).GetString(rawBytes, index, len);
                return;
            }

            index = 0x14;

            var chunks = new List<byte[]>();
            while (index < rawBytes.Length)
            {
                var size = BitConverter.ToInt16(rawBytes, index);
                if (size == 0)
                {
                    index += 2;
                    continue;
                }
                var buff = new byte[size];
                Buffer.BlockCopy(rawBytes, index, buff, 0, size);

                chunks.Add(buff);

                index += size;
            }

            var tempVals = new List<string>();

            foreach (var bytese in chunks)
            {
                switch (bytese[2])
                {
                    case 0x1f:
                        var f = new ShellBag0X1F(slot, mruPosition, bytese, bagPath);
                        tempVals.Add(f.Value);
                        ExtensionBlocks.AddRange(f.ExtensionBlocks);
                        break;

                    case 0x2e:
                        var te = new ShellBag0X2E(slot, mruPosition, bytese, bagPath);

                        tempVals.Add(te.Value);
                        ExtensionBlocks.AddRange(te.ExtensionBlocks);
                        break;
                    case 0x2f:
                        var tf = new ShellBag0X2F(slot, mruPosition, bytese, bagPath);
                        tempVals.Add(tf.Value);
                        ExtensionBlocks.AddRange(tf.ExtensionBlocks);
                        break;
                    case 0x31:
                        var t1 = new ShellBag0X31(slot, mruPosition, bytese, bagPath);
                        tempVals.Add(t1.Value);
                        ExtensionBlocks.AddRange(t1.ExtensionBlocks);
                        break;
                    case 0x00:
                        var zero0 = new ShellBag0X00(slot, mruPosition, bytese, bagPath);
                        tempVals.Add(zero0.Value);
                        ExtensionBlocks.AddRange(zero0.ExtensionBlocks);
                        break;
                    case 0xc3:
                        var c3 = new ShellBag0XC3(slot, mruPosition, bytese, bagPath);
                        tempVals.Add(c3.Value);
                        ExtensionBlocks.AddRange(c3.ExtensionBlocks);
                        break;
                    case 0x74:
                        var sig6 = BitConverter.ToUInt32(bytese, 6);

                        const uint cfsfSig = 0x46534643;

                        if (sig6 == cfsfSig)
                        {
                            var cfs = new ShellBagCFSF(slot, mruPosition, bytese, bagPath);
                            tempVals.Add(cfs.Value);
                            ExtensionBlocks.AddRange(cfs.ExtensionBlocks);
                        }
                        else
                        {
                            var seven4 = new ShellBag0X74(slot, mruPosition, bytese, bagPath);
                            tempVals.Add(seven4.Value);
                            ExtensionBlocks.AddRange(seven4.ExtensionBlocks);
                        }

                        
                        break;
                    default:
                        throw new Exception($"Unknown type id 0x{bytese[2]:X2}");
                }
            }

            Value = string.Join(" | ", tempVals);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}