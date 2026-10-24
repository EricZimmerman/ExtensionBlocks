using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ExtensionBlocks.Test
{
    /// <summary>
    ///     Every shell item of the ShellBags corpus (ShellBagsDataCorpus: &lt;TT&gt;_&lt;SHA1&gt;.bin files in a folder per type
    ///     byte or class, the unknown ones in !!Unknown(ToReverse)). It is private and not in the repo: Corpus/ShellBagsDataCorpus
    ///     next to the repo, or SHELLBAGS_CORPUS; skipped without it (CI)
    /// </summary>
    [TestFixture]
    public class CorpusTests
    {
        private const string ToReverse = "!!Unknown(ToReverse)";

        // the class the items in a class folder must parse as
        private static readonly Dictionary<string, Type> ClassFolders = new Dictionary<string, Type>
        {
            {"!Archive", typeof(ShellBagArchiveContents)},
            {"!DocumentDrive", typeof(ShellBagDocumentDrive)},
            {"!Zip", typeof(ShellBagZipContents)},
            {"!AugM", typeof(ShellBagCDBurn)},
            {"!CFSF", typeof(ShellBagCFSF)},
            {"!MtpDevice", typeof(ShellBagMTPDevice)},
            {"!MtpStorage", typeof(ShellBagMTPStorage)},
            {"!MtpFolder", typeof(ShellBagMTPFolder)},
            {"!Ftp", typeof(ShellBag0X00)},
            {"!FindData", typeof(ShellBagFindData)}
        };

        private static string Corpus()
        {
            var fromEnvironment = Environment.GetEnvironmentVariable("SHELLBAGS_CORPUS");
            if (!string.IsNullOrEmpty(fromEnvironment) && Directory.Exists(fromEnvironment))
            {
                return fromEnvironment;
            }

            // Corpus/ShellBagsDataCorpus next to the repo (the folder with ExtensionBlocks.slnx)
            for (var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); dir?.Parent != null; dir = dir.Parent)
            {
                if (File.Exists(Path.Combine(dir.FullName, "ExtensionBlocks.slnx")))
                {
                    var corpus = Path.Combine(dir.Parent.FullName, "Corpus", "ShellBagsDataCorpus");
                    return Directory.Exists(corpus) ? corpus : null;
                }
            }

            return null;
        }

        private static List<(string Folder, string File)> Items()
        {
            var corpus = Corpus();
            if (corpus == null)
            {
                Assert.Ignore("no ShellBagsDataCorpus (Corpus/ShellBagsDataCorpus next to the repo, or SHELLBAGS_CORPUS)");
            }

            return Directory.EnumerateFiles(corpus, "*.bin", SearchOption.AllDirectories)
                .Select(t => (Folder: Path.GetRelativePath(corpus, Path.GetDirectoryName(t)!).Split(Path.DirectorySeparatorChar)[0], File: t))
                .OrderBy(t => t.File, StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>The item's class, or what it threw (the inner exception of the reflection call)</summary>
        private static (IShellBag Bag, Exception Error) Parse(string file)
        {
            try
            {
                var bag = ShellItemFactory.Create(File.ReadAllBytes(file), -1, -1, "BagMRU");
                _ = bag.ToString();
                return (bag, null);
            }
            catch (Exception ex)
            {
                return (null, ex is TargetInvocationException {InnerException: { }} t ? t.InnerException : ex);
            }
        }

        private static string Describe(string file, Exception error)
        {
            var frame = error.StackTrace?.Split('\n').FirstOrDefault(t => t.Contains("ExtensionBlocks."))?.Trim();
            return $"{Path.GetFileName(Path.GetDirectoryName(file))}/{Path.GetFileName(file)}: {error.GetType().Name}: {error.Message} {frame}";
        }

        [Test]
        public void KnownItemsParse()
        {
            // the items in the type and class folders: none throws, and a class folder's items are that class
            var items = Items().Where(t => t.Folder != ToReverse).ToList();
            var failures = new List<string>();
            var unknown = 0;

            var run = Task.Run(() =>
            {
                foreach (var (folder, file) in items)
                {
                    var (bag, error) = Parse(file);
                    if (error != null)
                    {
                        failures.Add(Describe(file, error));
                    }
                    else if (ClassFolders.TryGetValue(folder, out var type) && bag.GetType() != type)
                    {
                        failures.Add($"{folder}/{Path.GetFileName(file)}: {bag.GetType().Name}, not {type.Name}");
                    }
                    else if (bag is ShellBagUnknown)
                    {
                        // type bytes with no class of their own (samples kept for when there is one)
                        unknown++;
                    }
                }
            });

            Assert.That(run.Wait(TimeSpan.FromMinutes(5)), Is.True, "the corpus did not finish");
            TestContext.Out.WriteLine($"{items.Count} items, {unknown} of types with no class, {failures.Count} failures");
            Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures.Take(50)));
        }

        [Test]
        public void UnknownItemsFinish()
        {
            // the items to reverse may throw (an item that can't be parsed does), but each must finish; the ones that
            // parse now are listed (to move out of the folder)
            var items = Items().Where(t => t.Folder == ToReverse).ToList();
            var parsed = new List<string>();
            var threw = 0;

            foreach (var (_, file) in items)
            {
                var run = Task.Run(() => Parse(file));
                Assert.That(run.Wait(TimeSpan.FromSeconds(10)), Is.True, $"{file} did not finish");

                var (bag, error) = run.Result;
                if (error != null)
                {
                    threw++;
                }
                else if (!(bag is ShellBagUnknown))
                {
                    parsed.Add($"{Path.GetFileName(Path.GetDirectoryName(file))}/{Path.GetFileName(file)}: {bag.GetType().Name}");
                }
            }

            TestContext.Out.WriteLine($"{items.Count} items to reverse: {parsed.Count} parse now, {threw} throw");
            foreach (var line in parsed)
            {
                TestContext.Out.WriteLine($"  {line}");
            }
        }
    }
}
