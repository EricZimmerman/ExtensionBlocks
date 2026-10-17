using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBag0X54 : ShellBag
    {
        public ShellBag0X54(int slot, int mruPosition, byte[] rawBytes, string bagPath)
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

            var name = Encoding.Unicode.GetString(rawBytes, 4, rawBytes.Length - 6).Replace("\0", "");

            
            if (rawBytes[4] == 0x10)
            {
                FriendlyName = "ZipFileContents";
                ShellBagType = Utils.ShellBagTypes.ZipFileContents;
               var index = 16;
               ModifiedOn = Utils.FromFileTime(BitConverter.ToInt64(rawBytes, index)).ToUniversalTime();


                 index = 28;

                var len1 = BitConverter.ToInt16(rawBytes, index);
                var len2 = BitConverter.ToInt16(rawBytes, index+2);
                


                index = 32;

                name = Encoding.Unicode.GetString(rawBytes, index, (len1+len2)*2).Replace("\0", "");
                
            }


            Value = name;
        }

       public DateTimeOffset? ModifiedOn { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ModifiedOn.HasValue)
            {
                sb.AppendLine(
                    $"Modified On: {ModifiedOn.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");

                sb.AppendLine();
            }

            sb.AppendLine(base.ToString());
            sb.AppendLine();


            return sb.ToString();
        }
    }
}