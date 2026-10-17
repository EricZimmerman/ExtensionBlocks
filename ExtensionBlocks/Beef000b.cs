using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    public class Beef000b : BeefBase
    {
        public Beef000b(byte[] rawBytes)
            : base(rawBytes)
        {
            if (Signature != 0xbeef000b)
            {
                throw new Exception($"Signature mismatch! Should be 0xbeef000b but is 0x{Signature:X}");
            }

            var strings = Encoding.Unicode.GetString(rawBytes);

            var ss = strings.Split('\0');

            FileStrings = new HashSet<string>();

            foreach (var s in ss)
            {
                if (s.Length < 5)
                {
                    continue;
                }

                var goods = s;

                //an ordinal check: StartsWith("\u0001") is culture sensitive, and \u0001 is ignored by ICU, so it was true for every string (and the drive letter was cut off)
                if (s[0] == '\u0001')
                {
                    goods = s.Substring(1);
                }

                FileStrings.Add(goods);
            }

            //the time is in the 14th string, if it is there
            var tsb = ss.Length > 13 ? Encoding.Unicode.GetBytes(ss[13]) : Array.Empty<byte>();
            StringDate = tsb.Length >= 8 ? Utils.FromFileTime(BitConverter.ToInt64(tsb, 0)).ToUniversalTime() : DateTimeOffset.MinValue;

            VersionOffset = BitConverter.ToInt16(rawBytes, 12);
        }

        public HashSet<string> FileStrings { get; }

        public DateTimeOffset StringDate { get; }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            sb.AppendLine();

            foreach (var fileString in FileStrings)
            {
                sb.AppendLine($"File: {fileString}");
            }

            sb.AppendLine($"String date: {StringDate.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");


            return sb.ToString();
        }
    }
}