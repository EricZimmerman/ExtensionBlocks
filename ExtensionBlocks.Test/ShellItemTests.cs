using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace ExtensionBlocks.Test
{
    /// <summary>
    ///     The shell items with what came from Lnk's and the RECmd plugins' own copies (2026.10.3), on items built here
    /// </summary>
    [TestFixture]
    public class ShellItemTests
    {
        /// <summary>A shell item: its size, then the bytes</summary>
        private static byte[] Item(params byte[][] parts)
        {
            var body = parts.SelectMany(t => t).ToArray();
            return BitConverter.GetBytes((ushort) (body.Length + 2)).Concat(body).ToArray();
        }

        private static byte[] Ansi(string s) => Encoding.GetEncoding(1252).GetBytes(s + "\0");

        private static byte[] Unicode(string s) => Encoding.Unicode.GetBytes(s + "\0");

        [OneTimeSetUp]
        public void CodePages()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Test]
        public void RootFolderOf20Bytes()
        {
            //type 0x2E, a byte, the GUID: no extension block list (it had no value)
            var item = Item(new byte[] {0x2e, 0x80}, new Guid("b4bfcc3a-db2c-424c-b029-7fe99a87c641").ToByteArray());

            var bag = ShellItemFactory.Create(item);

            Assert.That(item.Length, Is.EqualTo(0x14));
            Assert.That(bag, Is.TypeOf<ShellBag0X2E>());
            Assert.That(bag.Value, Is.EqualTo(GuidMapping.GuidMapping.GetDescriptionFromGuid("b4bfcc3a-db2c-424c-b029-7fe99a87c641")));
            Assert.That(bag.Value, Is.Not.Empty);
        }

        [Test]
        public void RootFolderWithBlocksAfterTheGuid()
        {
            //from Exceptionless: type 0x2E, 0x48, the Downloads GUID and a 0xbeef0026 block; the GUID's bytes at 5 read as a
            //control panel category signature and it threw
            var item = Convert.FromBase64String("OgAuSJDiTTc/EmVFkWQ5xJJeRnsmAAEAJgDvvhEAAAAc9LVLuEbZAS5yGKAhStoBpqiHxiFK2gEUAAAA");

            var bag = ShellItemFactory.Create(item);

            Assert.That(bag, Is.TypeOf<ShellBag0X2E>());
            Assert.That(bag.Value, Is.EqualTo(GuidMapping.GuidMapping.GetDescriptionFromGuid("374de290-123f-4565-9164-39c4925e467b")));
            Assert.That(bag.Value, Is.Not.Empty);
            Assert.That(bag.ExtensionBlocks.Single(), Is.TypeOf<Beef0026>());
        }

        [Test]
        public void ZipContentsWithDataInBytes4To9()
        {
            //type 0x8A, data in bytes 4-9 (byte 8 is not 0), the date at 0x24, then the name's lengths and the name: these were
            //unknown items
            var item = Item(new byte[] {0x8a, 0xfb, 0xfe, 0x07, 0x00, 0x00, 0xee, 0x00}, new byte[0x16], new byte[] {0x10, 0x00, 0x01, 0x00},
                Encoding.Unicode.GetBytes("02/03/2017  16:37:02"), new byte[] {0x00, 0x00, 0xfa, 0x31, 0x00, 0x00, 0x00, 0x00},
                BitConverter.GetBytes(7), new byte[4], Unicode("Scripts"), new byte[4]);

            var bag = ShellItemFactory.Create(item);

            Assert.That(item[8], Is.Not.Zero);
            Assert.That(bag, Is.TypeOf<ShellBagZipContents>());
            Assert.That(bag.Value, Is.EqualTo("Scripts"));
        }

        [Test]
        public void ShortControlPanelCategory()
        {
            //the control panel signature on an item too short for its strings: read as something else, not out of range
            var item = Item(new byte[] {0x2e, 0x00, 0x00, 0x01, 0x26, 0x03, 0x15}, new byte[0x40]);

            Exception error = null;
            try
            {
                ShellItemFactory.Create(item);
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException();
            }

            Assert.That(error, Is.Not.InstanceOf<ArgumentOutOfRangeException>());
        }

        [TestCase((byte) 0x82, "Microsoft Network", TestName = "Network location with a description")]
        [TestCase((byte) 0x02, null, TestName = "Network location without a description")]
        public void NetworkLocation(byte flags, string description)
        {
            var strings = flags == 0x82
                ? Ansi("Aldec_lyon").Concat(Ansi("Microsoft Network")).ToArray()
                : Ansi("Aldec_lyon").Concat(Unicode("Aldec_lyon")).ToArray();
            var item = Item(new byte[] {0x41, 0x00, flags}, strings, new byte[] {0x02, 0x00});

            var bag = ShellItemFactory.Create(item);

            Assert.That(bag, Is.TypeOf<ShellBag0X41>());
            //the location is the value (as Lnk and LnkParse3 have it); the description only when flag 0x80 says there is one
            Assert.That(bag.Value, Is.EqualTo("Aldec_lyon"));
            Assert.That(((ShellBag0X41) bag).Description, Is.EqualTo(description));
        }

        [Test]
        public void UnicodeFileIsAFile()
        {
            //0x36: a file (0x32) with a Unicode name (0x04)
            var item = Item(new byte[] {0x36, 0x00}, new byte[4 + 4 + 2], Unicode("Изображение 453.jpg"), new byte[2]);

            var bag = ShellItemFactory.Create(item);

            Assert.That(bag.FriendlyName, Is.EqualTo("File"));
            Assert.That(bag.ShellBagType, Is.EqualTo(Utils.ShellBagTypes.File));
        }

        [TestCase((byte) 0x31, "Directory")]
        [TestCase((byte) 0x35, "Directory")]
        [TestCase((byte) 0x36, "File")]
        [TestCase((byte) 0x39, "Directory")]
        [TestCase((byte) 0x3D, "Directory")]
        [TestCase((byte) 0xB1, "Directory")]
        [TestCase((byte) 0xB5, "Directory")]
        public void WrapperKeepsWhatTheViewSets(byte type, string shellType)
        {
            //0x35, 0x36, 0x39, 0xB1 and 0xB5 are read by a 0x31 clone: the last interacted time the view sets (MRU
            //position 0) was lost by the next Sync, so the text had no "Last interacted" line; 0x36's view and text said
            //Directory
            var name = type == 0x35 || type == 0x36 ? Unicode("Изображение") : Ansi("Folder");
            var item = Item(new byte[] {type, 0x00}, new byte[4 + 4 + 2], name, new byte[2]);
            var lastWrite = new DateTimeOffset(2026, 10, 20, 12, 34, 56, TimeSpan.Zero);

            var bag = ShellItemFactory.Create(item, 3, 0, @"BagMRU\1");
            bag.LastWriteTime = lastWrite;

            var view = bag.GetShellBagView();
            var text = bag.ToString();

            Assert.That(view.LastInteracted, Is.EqualTo(lastWrite));
            Assert.That(bag.LastInteracted, Is.EqualTo(lastWrite));
            Assert.That(text, Does.Contain("Last interacted: "));
            Assert.That(view.ShellType, Is.EqualTo(shellType));
            Assert.That(text, Does.Contain($"Shell Type: {shellType}"));
            Assert.That(bag.FriendlyName, Is.EqualTo(shellType));
        }

        [Test]
        public void UriWithoutData()
        {
            //0x61 with no data block: the URI is the item's value (it was empty)
            var item = Item(new byte[] {0x61, 0x80, 0x00, 0x00, 0x00, 0x00}, Unicode("ms-settings:dateandtime"), new byte[2]);

            var bag = (ShellBag0X61) ShellItemFactory.Create(item);

            Assert.That(bag.Value, Is.EqualTo("ms-settings:dateandtime"));
            Assert.That(bag.URI, Is.EqualTo("ms-settings:dateandtime"));
        }

        [Test]
        public void Type03WithOneString()
        {
            //an OpenSavePidlMRU 0x03 item has one name: it threw, and the whole MRU value was lost
            var body = new byte[0x2a - 2];
            body[0] = 0x03;
            var item = Item(body, Unicode("en_windows.iso"), new byte[2]);

            var bag = ShellItemFactory.Create(item);

            Assert.That(bag.Value, Does.StartWith("en_windows.iso"));
        }

        [Test]
        public void FileWithACutOffBlock()
        {
            //a 0x32 file whose last extension block says it is longer than the item: read as far as it goes (as Lnk does)
            var beef0004 = new List<byte>();
            beef0004.AddRange(BitConverter.GetBytes((ushort) 0x2a));
            beef0004.AddRange(BitConverter.GetBytes((ushort) 3));
            beef0004.AddRange(BitConverter.GetBytes(0xbeef0004));
            beef0004.AddRange(new byte[4 + 4 + 2 + 2]);
            beef0004.AddRange(Unicode("Internet Explorer.lnk"));
            while (beef0004.Count < 0x28) beef0004.Add(0);
            beef0004.RemoveRange(0x28, beef0004.Count - 0x28);
            beef0004.AddRange(BitConverter.GetBytes((ushort) 0x14));

            var cutOff = new List<byte>();
            cutOff.AddRange(BitConverter.GetBytes((ushort) 0x52)); // 82 bytes, but only 14 are there
            cutOff.AddRange(BitConverter.GetBytes((ushort) 0));
            cutOff.AddRange(BitConverter.GetBytes(0xbeef001d));
            cutOff.AddRange(new byte[6]);

            var item = Item(new byte[] {0x32, 0x00}, new byte[4 + 4 + 2], Ansi("INTERN~1.LNK"), beef0004.ToArray(), cutOff.ToArray());

            var bag = ShellItemFactory.Create(item);

            Assert.That(bag, Is.TypeOf<ShellBag0X32>());
            Assert.That(bag.ExtensionBlocks.Select(t => t.Signature), Does.Contain(0xbeef001d));
        }

        [Test]
        public void GameFolderIsRead()
        {
            //a 0x00 item with GFSI at 4: ShellBag0X00 reads it (the factory threw "send this hive")
            var item = Item(new byte[] {0x00, 0x00}, Encoding.ASCII.GetBytes("GFSI"), new byte[0x40]);

            var bag = ShellItemFactory.Create(item);

            Assert.That(bag, Is.TypeOf<ShellBag0X00>());
            Assert.That(bag.FriendlyName, Does.Contain("Game"));
        }

        [Test]
        public void UnknownTypeAndTooShort()
        {
            Assert.That(ShellItemFactory.Create(Item(new byte[] {0xee, 0x00, 0x01, 0x02})), Is.TypeOf<ShellBagUnknown>());
            Assert.That(() => ShellItemFactory.Create(new byte[] {1, 0}), Throws.Exception.With.Message.Contains("at least 3 bytes"));
        }

        [Test]
        public void ExtensionBlockSignaturesAsTheRegex()
        {
            //the byte search finds what the regular expression over BitConverter.ToString found
            var random = new Random(5);
            for (var round = 0; round < 2000; round++)
            {
                var data = new byte[random.Next(0, 64)];
                random.NextBytes(data);
                for (var k = random.Next(0, 4); k > 0 && data.Length >= 4; k--)
                {
                    var at = random.Next(0, data.Length - 3);
                    data[at + 1] = 0x00;
                    data[at + 2] = 0xef;
                    data[at + 3] = 0xbe;
                }

                var expected = new List<int>();
                var match = new Regex("([0-9A-F]{2})-00-EF-BE", RegexOptions.IgnoreCase).Match(BitConverter.ToString(data));
                while (match.Success)
                {
                    expected.Add(match.Index / 3);
                    match = match.NextMatch();
                }

                Assert.That(Utils.FindExtensionBlockSignatures(data), Is.EqualTo(expected), BitConverter.ToString(data));
            }
        }

        [Test]
        public void ItemsOutsideTheRegistryHaveNoBagLines()
        {
            var item = Item(new byte[] {0x2e, 0x80}, new Guid("b4bfcc3a-db2c-424c-b029-7fe99a87c641").ToByteArray());

            var text = ShellItemFactory.Create(item).ToString();

            Assert.That(text, Does.Not.Contain("Child Bags"));
            Assert.That(text, Does.Not.Contain("Has been explored"));
        }

        [Test]
        public void ZipContentsNameAndFolder()
        {
            //the second name is the folder in the zip file the item is in: VIA/10_Audio/ was a path that isn't one
            var item = new byte[84 + 8 + 4 * 2 + 9 * 2 + 4];
            BitConverter.GetBytes((ushort) item.Length).CopyTo(item, 0);
            Encoding.Unicode.GetBytes("N/A").CopyTo(item, 0x24);
            BitConverter.GetBytes((ushort) 3).CopyTo(item, 84);
            BitConverter.GetBytes((ushort) 9).CopyTo(item, 88);
            Encoding.Unicode.GetBytes("VIA\0").CopyTo(item, 92);
            Encoding.Unicode.GetBytes("10_Audio/\0").CopyTo(item, 100);

            var bag = (ShellBagZipContents) ShellItemFactory.Create(item);

            Assert.That(bag.Value, Is.EqualTo("VIA"));
            Assert.That(bag.ParentFolder, Is.EqualTo("10_Audio/"));
        }

        private static byte[] Beef0004(string longName)
        {
            var block = new List<byte>();
            block.AddRange(BitConverter.GetBytes((ushort) 3));
            block.AddRange(BitConverter.GetBytes(0xbeef0004));
            block.AddRange(new byte[4 + 4 + 2 + 2]);
            block.AddRange(Unicode(longName));
            block.AddRange(BitConverter.GetBytes((ushort) 0x14));
            block.InsertRange(0, BitConverter.GetBytes((ushort) (block.Count + 2)));
            return block.ToArray();
        }

        [Test]
        public void FileEntryOf3A()
        {
            //0x3A file entries (in LNK files and MRU values) are read as 0x31 items; ShellBags' 0x3A items have no 0xbeef0004
            var item = Item(new byte[] {0x3a, 0x00}, new byte[4 + 4 + 2], Ansi("PLISTE~1.LNK"), Beef0004("plist Editor Pro.lnk"), new byte[2]);

            var bag = (ShellBag0X3A) ShellItemFactory.Create(item);

            Assert.That(bag.ShortName, Is.EqualTo("PLISTE~1.LNK"));
            Assert.That(bag.Value, Is.EqualTo("plist Editor Pro.lnk"));
        }

        [Test]
        public void CodePageForAnsiNames()
        {
            //Lnk reads LNK files from systems with other code pages (Shift-JIS here)
            var name = Encoding.GetEncoding(932).GetBytes("テスト.TXT");
            var item = Item(new byte[] {0x32, 0x00}, new byte[4 + 4 + 2], name, new byte[] {0}, Beef0004("テスト.TXT"), new byte[2]);

            var shiftJis = (ShellBag0X32) ShellItemFactory.Create(item, -1, -1, string.Empty, 932);
            Assert.That(shiftJis.ShortName, Is.EqualTo("テスト.TXT"));
            Assert.That(Utils.CodePage, Is.EqualTo(1252), "the code page is back as it was");

            var ansi = (ShellBag0X32) ShellItemFactory.Create(item);
            Assert.That(ansi.ShortName, Is.Not.EqualTo("テスト.TXT"));
        }

        [Test]
        public void VolumeAndZipAliases()
        {
            //0x22 is a volume item read as 0x23 when it has a drive letter, 0xAA zip contents when it is one; else unknown
            Assert.That(ShellItemFactory.Create(Item(new byte[] {0x22}, Encoding.ASCII.GetBytes("C:\\\0"), new byte[16])).Value, Is.EqualTo("C:"));
            Assert.That(ShellItemFactory.Create(Item(new byte[] {0x22, 0x00, 0xb8}, new byte[20])), Is.TypeOf<ShellBagUnknown>());
            Assert.That(ShellItemFactory.Create(Item(new byte[] {0xaa, 0x03, 0x58, 0x00}, new byte[20])), Is.TypeOf<ShellBagUnknown>());
        }
        [Test]
        public void ArchiveContentsWhateverTheTypeByte()
        {
            //Windows 11 archive folders (ShellBags): bytes 2-3 vary, so the item is found by its layout, not by its type byte
            //(0x54 here, whose class read it only partly; other bytes were unknown types or misread as GUIDs)
            var item = Convert.FromBase64String("RgBUABAAAAAAAAAAAAAAAIAl6Cd3qdwBAwAAAAUADQBSAGUAcABvAHIAdABzAFwAMgAwADIANgBcAE0AYQByAGMAaAAAAAAA");

            var bag = (ShellBagArchiveContents) ShellItemFactory.Create(item);

            Assert.That(bag.Value, Is.EqualTo("March"), "the parent folders are items of their own");
            Assert.That(bag.ArchivePath, Is.EqualTo(@"Reports\2026\March"));
            Assert.That(bag.FileAttributes, Is.EqualTo(0x10));
            Assert.That(bag.ModifiedOn, Is.EqualTo(new DateTimeOffset(2026, 3, 1, 12, 30, 15, TimeSpan.Zero)));
            Assert.That(bag.GetShellBagView().ModifiedOn, Is.EqualTo(bag.ModifiedOn));
            Assert.That(bag.ToString(), Does.Contain(@"Archive path: Reports\2026\March"));

            //a read only folder at the root of the archive, 0xC3 at offset 2
            var root = Convert.FromBase64String("LgDDehEAAAAAAAAAAAAAAABAVk2rdNwBAAAAAAYAAABiAGEAYwBrAHUAcAAAAAAA");
            Assert.That(ShellItemFactory.Create(root).Value, Is.EqualTo("backup"));

            //the lengths have to fit the item
            root[0x1C] = 7;
            Assert.That(ShellBagArchiveContents.IsArchiveContents(root), Is.False);
        }

        [Test]
        public void DocumentDriveFolderAndFile()
        {
            //0xFF folders and 0xFC files with 0x55 at offset 3 (a document drive namespace extension, from ShellBags)
            var folder = Convert.FromBase64String(
                "5AD/VQwDDAAAAAABAAAAEAAAAACoo2JaSNoBAJC+piu13AEA+IIINLXcAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADEAQQBiAEMAZABFAGYARwBoAEkAagAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMQBLAGwATQBuAE8AcABRAHIAUwB0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAF9awgxyAA9NNUswAAAAAAAAAAAAA=");
            var bag = (ShellBagDocumentDrive) ShellItemFactory.Create(folder);

            Assert.That(bag.Value, Is.EqualTo("공유 폴더"));
            Assert.That(bag.FriendlyName, Is.EqualTo("Document drive folder"));
            Assert.That(bag.ItemId, Is.EqualTo("1AbCdEfGhIj"));
            Assert.That(bag.ParentId, Is.EqualTo("1KlMnOpQrSt"));
            Assert.That(bag.CreatedOn, Is.EqualTo(new DateTimeOffset(2024, 1, 16, 9, 0, 0, TimeSpan.Zero)));
            Assert.That(bag.ModifiedOn, Is.EqualTo(new DateTimeOffset(2026, 3, 16, 10, 0, 0, TimeSpan.Zero)));
            Assert.That(bag.AccessedOn, Is.EqualTo(new DateTimeOffset(2026, 3, 16, 11, 0, 0, TimeSpan.Zero)));
            Assert.That(bag.GetShellBagView().CreatedOn, Is.EqualTo(bag.CreatedOn));

            var file = (ShellBagDocumentDrive) ShellItemFactory.Create(Convert.FromBase64String(
                "OwD8VS0QAAAAgPffOz4E3QEAjng8PgTdAYAkET0+BN0BAAAAAAAAAACsrAHIHMFfAHYAMgAAAAAAAAAAAA=="));

            Assert.That(file.Value, Is.EqualTo("견적서_v2"));
            Assert.That(file.FriendlyName, Is.EqualTo("Document drive file"));
            Assert.That(file.ItemId, Is.Null);
            Assert.That(file.ModifiedOn, Is.EqualTo(new DateTimeOffset(2026, 6, 25, 1, 2, 4, TimeSpan.Zero)));
        }

        [Test]
        public void FolderOf3D()
        {
            //0x3D (ShellBags) is a 0x31 folder: a code page short name and a 0xbeef0004 block
            var item = Convert.FromBase64String("UgA9AAAAAABXXJMtEAAwrvi8JMFYzgAAOgAJAAQA775XXJYlV1yTLS4AAAB2FAAAAAABAAAAAAAAAAAAAAAAAAAAmQ4BATCu+LwkwVjOAAAYAAAA");

            var bag = ShellItemFactory.Create(item);

            Assert.That(bag, Is.TypeOf<ShellBag0X3D>());
            Assert.That(bag.Value, Is.EqualTo("기본설치"));
            Assert.That(bag.ShellBagType, Is.EqualTo(Utils.ShellBagTypes.Directory));
        }
    }
}
