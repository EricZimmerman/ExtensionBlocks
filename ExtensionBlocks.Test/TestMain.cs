using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ExtensionBlocks.Test
{
    [TestFixture]
    public class TestMain
    {
        // Builds an extension block: size, version, signature, body, then the 2-byte version offset
        private static byte[] Block(uint signature, ushort version, byte[] body, ushort versionOffset)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((ushort) (8 + body.Length + 2)));
            bytes.AddRange(BitConverter.GetBytes(version));
            bytes.AddRange(BitConverter.GetBytes(signature));
            bytes.AddRange(body);
            bytes.AddRange(BitConverter.GetBytes(versionOffset));
            return bytes.ToArray();
        }

        private static byte[] UnicodeZ(string s)
        {
            return Encoding.Unicode.GetBytes(s + "\0");
        }

        [Test]
        public void GuidLookup()
        {
            Assert.That(Utils.GetDescriptionFromGuidAndKey("de35258c-c695-4cbc-b982-38b0ad24ced0", 2),
                Is.EqualTo("Shell Omit From View"));
            Assert.That(Utils.GetDescriptionFromGuidAndKey("DE35258C-C695-4CBC-B982-38B0AD24CED0", 2),
                Is.EqualTo("Shell Omit From View"));
        }

        [Test]
        public void GuidLookupMisses()
        {
            Assert.That(Utils.GetDescriptionFromGuidAndKey("de35258c-c695-4cbc-b982-38b0ad24ced0", 12345),
                Is.EqualTo("(Description not available)"));
            Assert.That(Utils.GetDescriptionFromGuidAndKey("00000000-0000-0000-0000-000000000001", 2),
                Is.EqualTo("(Description not available)"));
        }

        [Test]
        public void ExtractGuidUsesShellItemByteOrder()
        {
            var raw = new byte[]
            {
                0xe0, 0x4f, 0xd0, 0x20, 0xea, 0x3a, 0x69, 0x10, 0xa2, 0xd8, 0x08, 0x00, 0x2b, 0x30, 0x30, 0x9d
            };

            Assert.That(Utils.ExtractGuidFromShellItem(raw), Is.EqualTo("20d04fe0-3aea-1069-a2d8-08002b30309d"));
            Assert.That(Utils.ExtractGuidFromShellItem(raw.Concat(new byte[] {0xff, 0xff}).ToArray()),
                Is.EqualTo("20d04fe0-3aea-1069-a2d8-08002b30309d"));
        }

        [Test]
        public void DosDateTimeIsDecoded()
        {
            // 2019-03-27 13:45:30
            ushort date = (2019 - 1980) << 9 | 3 << 5 | 27;
            ushort time = 13 << 11 | 45 << 5 | 30 / 2;
            var raw = BitConverter.GetBytes(date).Concat(BitConverter.GetBytes(time)).ToArray();

            Assert.That(Utils.ExtractDateTimeOffsetFromBytes(raw),
                Is.EqualTo(new DateTimeOffset(2019, 3, 27, 13, 45, 30, TimeSpan.Zero)));
        }

        [TestCase((ushort) 0, (ushort) 0, TestName = "Unset")]
        [TestCase((ushort) ((2019 - 1980) << 9 | 2 << 5 | 30), (ushort) 0, TestName = "February 30")]
        [TestCase((ushort) ((2019 - 1980) << 9 | 13 << 5 | 1), (ushort) 0, TestName = "Month 13")]
        [TestCase((ushort) ((2019 - 1980) << 9 | 1 << 5 | 1), (ushort) (24 << 11), TestName = "Hour 24")]
        [TestCase((ushort) ((2019 - 1980) << 9 | 1 << 5 | 1), (ushort) 30, TestName = "Second 60")]
        public void InvalidDosDateTimeIsNull(ushort date, ushort time)
        {
            var raw = BitConverter.GetBytes(date).Concat(BitConverter.GetBytes(time)).ToArray();

            Assert.That(Utils.ExtractDateTimeOffsetFromBytes(raw), Is.Null);
        }

        [Test]
        public void UnknownSignatureIsBeefUnknown()
        {
            var raw = Block(0xbeef7777, 1, new byte[4], 0);

            Assert.That(Utils.GetExtensionBlockFromBytes(0xbeef7777, raw), Is.TypeOf<BeefUnknown>());
        }

        [Test]
        public void SignatureMismatchThrows()
        {
            var raw = Block(0xbeef0003, 1, new byte[16], 0);

            Assert.That(() => new Beef0006(raw), Throws.Exception.With.Message.Contains("Signature mismatch"));
        }

        [TestCase("eric")]
        [TestCase("")]
        [TestCase("A一B", TestName = "Code unit with a 0x00 low byte")]
        public void Beef0006UserName(string userName)
        {
            var raw = Block(0xbeef0006, 0, UnicodeZ(userName), 0x0014);

            var block = (Beef0006) Utils.GetExtensionBlockFromBytes(0xbeef0006, raw);

            Assert.That(block.UserName, Is.EqualTo(userName));
            Assert.That(block.VersionOffset, Is.EqualTo(0x14));
        }

        [Test]
        public void Beef001aFileDocumentType()
        {
            var body = new byte[2].Concat(UnicodeZ("Microsoft Word Document")).ToArray();
            var raw = Block(0xbeef001a, 0, body, 0x0014);

            var block = (Beef001a) Utils.GetExtensionBlockFromBytes(0xbeef001a, raw);

            Assert.That(block.FileDocumentTypeString, Is.EqualTo("Microsoft Word Document"));
            Assert.That(block.VersionOffset, Is.EqualTo(0x14));
        }

        [Test]
        public void Beef0004Version9()
        {
            ushort date = (2019 - 1980) << 9 | 3 << 5 | 27;
            ushort time = 13 << 11 | 45 << 5 | 30 / 2;

            var body = new List<byte>();
            body.AddRange(BitConverter.GetBytes(date));
            body.AddRange(BitConverter.GetBytes(time)); // created
            body.AddRange(BitConverter.GetBytes(date));
            body.AddRange(BitConverter.GetBytes(time)); // last access
            body.AddRange(BitConverter.GetBytes((ushort) 0x2e)); // identifier
            body.AddRange(new byte[2]);
            body.AddRange(BitConverter.GetBytes(0x1234u)); // MFT entry
            body.AddRange(BitConverter.GetBytes((ushort) 0));
            body.AddRange(BitConverter.GetBytes((ushort) 5)); // MFT sequence
            body.AddRange(new byte[8 + 2 + 4 + 4]);
            body.AddRange(UnicodeZ("Program Files"));

            var raw = Block(0xbeef0004, 9, body.ToArray(), 0x0014);

            var block = (Beef0004) Utils.GetExtensionBlockFromBytes(0xbeef0004, raw);

            Assert.That(block.Version, Is.EqualTo(9));
            Assert.That(block.Identifier, Is.EqualTo(0x2e));
            Assert.That(block.CreatedOnTime, Is.EqualTo(new DateTimeOffset(2019, 3, 27, 13, 45, 30, TimeSpan.Zero)));
            Assert.That(block.MFTInformation.MFTEntryNumber, Is.EqualTo(0x1234));
            Assert.That(block.MFTInformation.MFTSequenceNumber, Is.EqualTo(5));
            Assert.That(block.LongName, Is.EqualTo("Program Files"));
            Assert.That(block.VersionOffset, Is.EqualTo(0x14));
        }

        [Test]
        public void NumericPropertySheetSignedShort()
        {
            var value = new List<byte>();
            value.AddRange(BitConverter.GetBytes(16)); // value size
            value.AddRange(BitConverter.GetBytes(7)); // property id
            value.Add(0); // reserved
            value.AddRange(BitConverter.GetBytes((ushort) 0x0002)); // VT_I2
            value.AddRange(new byte[2]);
            value.AddRange(BitConverter.GetBytes((short) -2));
            value.Add(0);

            var sheet = new List<byte>();
            sheet.AddRange(BitConverter.GetBytes(4 + 4 + 16 + value.Count + 4));
            sheet.AddRange(Encoding.ASCII.GetBytes("1SPS"));
            sheet.AddRange(new Guid("de35258c-c695-4cbc-b982-38b0ad24ced0").ToByteArray());
            sheet.AddRange(value);
            sheet.AddRange(new byte[4]); // terminator

            var ps = new PropertySheet(sheet.ToArray());

            Assert.That(ps.PropertySheetType, Is.EqualTo(PropertySheet.PropertySheetTypeEnum.Numeric));
            Assert.That(ps.GUID, Is.EqualTo("de35258c-c695-4cbc-b982-38b0ad24ced0"));
            Assert.That(ps.PropertyNames["7"], Is.EqualTo("-2"));
        }

        [Test]
        public void ShellBag0X31NegativeChunkSizeDoesNotHang()
        {
            var raw = new List<byte>();
            raw.AddRange(BitConverter.GetBytes((ushort) 24));
            raw.Add(0x31);
            raw.Add(0);
            raw.AddRange(new byte[4 + 4 + 2]); // file size, modified, attributes
            raw.AddRange(Encoding.ASCII.GetBytes("A\0"));
            raw.AddRange(new byte[] {0xff, 0xff, 0x00, 0x00, 0x04, 0x00, 0xef, 0xbe});

            var parse = Task.Run(() => new ShellBag0X31(0, 0, raw.ToArray(), ""));

            Assert.That(parse.Wait(TimeSpan.FromSeconds(10)), Is.True, "ShellBag0X31 did not finish");
            Assert.That(parse.Result.ShortName, Is.EqualTo("A"));
            Assert.That(parse.Result.ExtensionBlocks, Is.Empty);
        }
    }
}
