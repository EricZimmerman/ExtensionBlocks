using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ExtensionBlocks.Test
{
    /// <summary>
    ///     What came with the shell items from ShellBags (2026.10.1): the blocks it added, its fixes, and damaged data
    /// </summary>
    [TestFixture]
    public class SharedTests
    {
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

        /// <summary>The shell items in TestFiles/ShellItemSamples.txt (from the ShellBags tests): type, base64</summary>
        private static byte[][] Samples() =>
            File.ReadAllLines(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestFiles", "ShellItemSamples.txt"))
                .Where(t => t.StartsWith("#") == false && t.Contains('\t'))
                .Select(t => Convert.FromBase64String(t.Split('\t')[1]))
                .ToArray();

        /// <summary>The extension blocks in a shell item, by their signatures</summary>
        private static IEnumerable<byte[]> BlocksIn(byte[] item)
        {
            for (var i = 4; i + 4 <= item.Length; i++)
            {
                if (item[i + 1] == 0x00 && item[i + 2] == 0xef && item[i + 3] == 0xbe)
                {
                    var size = BitConverter.ToUInt16(item, i - 4);
                    if (size >= 10 && i - 4 + size <= item.Length)
                    {
                        yield return item.Skip(i - 4).Take(size).ToArray();
                    }
                }
            }
        }

        private static void Damage(Random random, byte[] b)
        {
            switch (random.Next(3))
            {
                case 0:
                    for (var k = random.Next(1, 8); k > 0; k--)
                    {
                        b[random.Next(8, b.Length)] = (byte) random.Next(256);
                    }

                    break;
                case 1:
                {
                    var at = random.Next(8, b.Length - 1);
                    b[at] = 0xff;
                    b[at + 1] = (byte) random.Next(256);
                    break;
                }
                default:
                {
                    var at = random.Next(8, Math.Max(9, b.Length - 4));
                    var v = BitConverter.GetBytes(random.Next(2) == 0 ? -1 : random.Next(0x10000));
                    Array.Copy(v, 0, b, at, Math.Min(4, b.Length - at));
                    break;
                }
            }
        }

        [Test]
        public void Beef0004LongNameIsKeptWithALocalizedName()
        {
            //the long name was left empty when a localized name followed it (Lnk showed @shell32.dll,-21813 for Users)
            var body = new List<byte>();
            body.AddRange(new byte[4 + 4]); // created, last access
            body.AddRange(BitConverter.GetBytes((ushort) 0x2e));
            body.AddRange(new byte[2 + 8 + 8 + 2 + 4 + 4]);
            body.AddRange(UnicodeZ("Users"));
            body.AddRange(UnicodeZ("@shell32.dll,-21813"));

            var block = (Beef0004) Utils.GetExtensionBlockFromBytes(0xbeef0004, Block(0xbeef0004, 9, body.ToArray(), 0x14));

            Assert.That(block.LongName, Is.EqualTo("Users"));
            Assert.That(block.LocalisedName, Is.EqualTo("@shell32.dll,-21813"));
        }

        [Test]
        public void Beef000bKeepsTheDriveLetter()
        {
            //StartsWith("\u0001") is true for every string under ICU, and the first character was cut off
            var body = new List<byte>();
            body.AddRange(new byte[8]);
            body.AddRange(UnicodeZ(@"C:\Users\someone\Downloads"));
            body.AddRange(new byte[4]);

            var block = (Beef000b) Utils.GetExtensionBlockFromBytes(0xbeef000b, Block(0xbeef000b, 5, body.ToArray(), 0));

            Assert.That(block.FileStrings, Does.Contain(@"C:\Users\someone\Downloads"));
            Assert.That(block.StringDate, Is.EqualTo(DateTimeOffset.MinValue), "no time in a block this short");
        }

        [Test]
        public void BlocksFromShellBagsAreRegistered()
        {
            Assert.That(Utils.GetExtensionBlockFromBytes(0xbeef0015, Block(0xbeef0015, 0, new byte[4], 0)), Is.TypeOf<Beef0015>());

            var b1c = Utils.GetExtensionBlockFromBytes(0xbeef001c, Block(0xbeef001c, 0, new byte[2].Concat(UnicodeZ("A一B")).ToArray(), 0x14));
            Assert.That(b1c, Is.TypeOf<Beef001c>());
            Assert.That(((Beef001c) b1c).Information, Is.EqualTo("A一B"), "a code unit with a 0x00 low byte is not the end");
        }

        [Test]
        public void DamagedBlockThrowsOrIsAParseError()
        {
            var cutOff = new byte[] {0x40, 0x00, 0x09, 0x00, 0x04, 0x00, 0xef, 0xbe, 0x01, 0x02};

            //Lnk catches the exception and records the block as damaged
            Assert.That(() => Utils.GetExtensionBlockFromBytes(0xbeef0004, cutOff), Throws.Exception);

            var error = Utils.GetExtensionBlockFromBytesOrError(0xbeef0004, cutOff);
            Assert.That(error, Is.TypeOf<BeefParseError>());
            Assert.That(error.Signature, Is.EqualTo(0xbeef0004));
            Assert.That(error.ToString(), Does.Contain("could not be parsed"));

            Assert.That(Utils.GetExtensionBlockFromBytesOrError(0, new byte[] {1, 2}), Is.TypeOf<BeefParseError>());
        }

        [Test]
        public void NumericSheetShowsGuidAndPropertyName()
        {
            var value = new List<byte>();
            value.AddRange(BitConverter.GetBytes(16));
            value.AddRange(BitConverter.GetBytes(10)); // Item Name Display
            value.Add(0);
            value.AddRange(BitConverter.GetBytes((ushort) 0x0002));
            value.AddRange(new byte[2]);
            value.AddRange(BitConverter.GetBytes((short) 3));
            value.Add(0);

            var sheet = new List<byte>();
            sheet.AddRange(BitConverter.GetBytes(4 + 4 + 16 + value.Count + 4));
            sheet.AddRange(Encoding.ASCII.GetBytes("1SPS"));
            sheet.AddRange(new Guid("b725f130-47ef-101a-a5f1-02608c9eebac").ToByteArray());
            sheet.AddRange(value);
            sheet.AddRange(new byte[4]);

            var text = new PropertySheet(sheet.ToArray()).ToString();

            Assert.That(text, Is.EqualTo("Guid: b725f130-47ef-101a-a5f1-02608c9eebac, Key: 10 ==> Item Name Display, Value: 3"));
        }

        [Test]
        public void IndexOfBytes()
        {
            var data = new byte[] {1, 4, 0, 0xef, 0xbe, 4, 0, 0xef, 0xbe};

            Assert.That(Utils.IndexOf(data, new byte[] {4, 0, 0xef, 0xbe}), Is.EqualTo(1));
            Assert.That(Utils.IndexOf(data, new byte[] {4, 0, 0xef, 0xbe}, 2), Is.EqualTo(5));
            Assert.That(Utils.IndexOf(data, new byte[] {9}), Is.EqualTo(-1));
            Assert.That(Utils.IndexOf(new byte[2], new byte[4]), Is.EqualTo(-1));
        }

        [Test]
        public void TimeFormatFollowsTheSetting()
        {
            var saved = Utils.DateTimeFormat;
            try
            {
                Utils.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                Assert.That(Utils.GetDateTimeFormatWithMilliseconds(), Is.EqualTo("yyyy-MM-dd HH:mm:ss.fff"));

                Utils.DateTimeFormat = "HH:mm:ss dd/MM/yyyy";
                Assert.That(Utils.GetDateTimeFormatWithMilliseconds(), Is.EqualTo("HH:mm:ss.fff dd/MM/yyyy"));

                Utils.DateTimeFormat = null;
                Assert.That(Utils.GetDateTimeFormatWithMilliseconds(), Does.EndWith(" K"));
            }
            finally
            {
                Utils.DateTimeFormat = saved;
            }
        }

        [Test]
        public void SampleShellItemsParse()
        {
            var samples = Samples();
            Assert.That(samples, Is.Not.Empty);

            var parsed = 0;
            foreach (var sample in samples)
            {
                var type = typeof(Utils).Assembly.GetType($"ExtensionBlocks.ShellBag0X{sample[2]:X2}");
                if (type == null)
                {
                    continue;
                }

                //ShellBags looks for signatures (MTP, CDBurn, ...) before the type byte, so not every sample is its type's class
                try
                {
                    var bag = (IShellBag) Activator.CreateInstance(type, -1, -1, sample, "");
                    Assert.That(bag.ToString(), Is.Not.Empty);
                    parsed += 1;
                }
                catch (System.Reflection.TargetInvocationException)
                {
                }
            }

            Assert.That(parsed, Is.GreaterThan(50));
        }

        /// <summary>
        ///     The extension blocks in the samples, damaged: GetExtensionBlockFromBytesOrError never throws, and nothing
        ///     hangs (negative sizes made some loops go back forever)
        /// </summary>
        [Test]
        public void DamagedBlocks()
        {
            var random = new Random(26);
            var blocks = Samples().SelectMany(BlocksIn).ToList();
            Assert.That(blocks.Count, Is.GreaterThan(50));

            var run = Task.Run(() =>
            {
                foreach (var block in blocks)
                {
                    for (var round = 0; round < 30; round++)
                    {
                        var b = (byte[]) block.Clone();
                        Damage(random, b);

                        var parsed = Utils.GetExtensionBlockFromBytesOrError(BitConverter.ToUInt32(b, 4), b);
                        _ = parsed.ToString();
                    }
                }
            });

            Assert.That(run.Wait(TimeSpan.FromSeconds(60)), Is.True, "damaged blocks did not finish");
            run.GetAwaiter().GetResult();
        }

        /// <summary>The sample shell items, damaged: an exception is fine (callers record the item as damaged), a hang is not</summary>
        [Test]
        public void DamagedShellItems()
        {
            var random = new Random(37);
            var samples = Samples();

            var run = Task.Run(() =>
            {
                foreach (var sample in samples)
                {
                    var type = typeof(Utils).Assembly.GetType($"ExtensionBlocks.ShellBag0X{sample[2]:X2}");
                    if (type == null || sample.Length < 12)
                    {
                        continue;
                    }

                    for (var round = 0; round < 20; round++)
                    {
                        var b = (byte[]) sample.Clone();
                        Damage(random, b);

                        try
                        {
                            var bag = (IShellBag) Activator.CreateInstance(type, -1, -1, b, "");
                            _ = bag.ToString();
                        }
                        catch (Exception)
                        {
                            //damaged
                        }
                    }
                }
            });

            Assert.That(run.Wait(TimeSpan.FromSeconds(120)), Is.True, "damaged shell items did not finish");
        }
    }
}
