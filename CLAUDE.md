# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A `netstandard2.0` NuGet library that parses the `0xbeefXXXX` extension blocks found at the end of Windows shell items (ShellBags, LNK files, jump lists), and (since 2026.10.1) the shell items themselves, moved here from the ShellBags library in the ShellBagsExplorer repo so they are shared. GUID → name mapping comes from the `GuidMapping` package.

Consumers:
- `ShellBags` (ShellBagsExplorer repo: SBECmd and ShellBags Explorer): the shell items, the blocks, `GetExtensionBlockFromBytesOrError`. It keeps only the registry side (`RawRegistryData`: BagMRU keys, the class by type byte `ExtensionBlocks.ShellBag0X{type}` found by reflection in this assembly). It sets `Utils.DateTimeFormat` to `yyyy-MM-dd HH:mm:ss` when it loads, and `Utils.TimeZone`.
- `Lnk` (and `JumpList` through it): since Lnk 2026.10.6 its shell items are this package's: Lnk splits the ID lists and creates each item with `ShellItemFactory.Create(bytes, -1, -1, "", codepage)` (an unknown or failing item becomes Lnk's `ShellBagDamaged`, which derives from `ShellBag`), and `LnkFile.TargetIDs` is a `List<IShellBag>`. It also uses the blocks (`GetExtensionBlockFromBytes`, which throws on damage) and `PropertyStore`/`PropertySheet`. LECmd and JLECmd show the items' and blocks' properties and text, and take a target's MFT entry from the last 0xbeef0004 of `DelegateExtensionBlocks` then `ExtensionBlocks` for a 0x00 property view.
- `RegistryPluginBase` (RegistryPlugins): the blocks; LastVisitedPidlMRU, OpenSavePidlMRU and Taskband use the shell items through `ShellItemFactory` (since 2026.10.3; they had their own older copies).
- Checked against Lnk's own classes before it moved: `Value` and `FriendlyName` the same for all 9,410 shell items of the LNK files in the corpus; for the 90,098 in the jump lists' LNKs, 81 differ, all read better here (MTP devices, storage and folders and CD burning items that Lnk read as control panel items with garbage). LECmd's CSV over 2,494 LNK files and JLECmd's over the jump lists are the same.

## Behavior to keep

- Public names and visibility: every block and shell item class is public; block classes are named with lower case hex (`Beef000a`, `Beef001d`), shell items with upper case (`ShellBag0X31`, as ShellBags finds them by name). `Beef001d.Executable`, `Beef0005.InternalBags`, `Utils.ShellBagTypes` are public API.
- `GetExtensionBlockFromBytes` throws on damaged data (Lnk relies on it); `GetExtensionBlockFromBytesOrError` never throws (the block's type from its own signature byte, as ShellBags did).
- Check the public API against the last release before releasing: `apicompat -l <old ExtensionBlocks.dll> -r <new> --lref <GuidMapping, netstandard ref, CodePages folders> --rref <same>` (`dotnet tool install -g Microsoft.DotNet.ApiCompat.Tool`). 2026.10.1 added `IShellBag.GetShellBagView()` (nothing outside implements the package's `IShellBag`) and changed `PropertySheet.Version` to a `uint` (back to the string in 2026.10.2).
- Defaults keep Lnk's output: `Utils.DateTimeFormat` null = the culture's pattern with milliseconds and the offset; `Beef0026` keeps `yyyy-MM-dd HH:mm:ss.fffffff` unless a format is set; `Utils.TimeZone` UTC.
- `GetDateTimeFormatWithMilliseconds` is cached (it was a regular expression per call and made property sheets twice as slow).
- `PropertySheet` keeps the shell items found in VT_VECTOR, VT_STREAM and VT_BLOB values as `ExtraData`, made when first read (`LazyText`): building their text while parsing made Lnk parse jump lists 60% slower. Only the 0x00 item in streams is made while parsing (it has its own "possible false positive" message). Sheet text: `Guid: ..., Key: n ==> property name, Value: ...` for numeric sheets.
- `PropertyStore` keeps the sheets it could read (`ParseErrors`), and throws only when none could be read.
- A shell item's `ToString()` has the registry lines (explored, child bags, deleted) only when it has a `BagPath` (a ShellBags item).
- Wrappers (`ShellBag0X35`, `0X36`, `0X39`, `0X3D`, `0XB1`, `0XB5`) parse with a `ShellBag0X31` clone, which builds the view and the text. `Sync()` copies the wrapper's registry-side fields and `FriendlyName` to it (0x36 is a `File`; the clone says `Directory`), and `GetShellBagView()` copies the clone's `LastInteracted` back (the view sets it for MRU position 0, as every item does; without it the next `Sync` nulled it and the text had no "Last interacted", 2026.10.6). A field the clone sets for itself must come back the same way.
- Chunk loops read a block longer than what is left as far as it goes (as Lnk does), instead of failing the item.
- Differences from Lnk's old classes, on purpose: a 0x00 property view with no sheets says `No Property sheets found` (Lnk: `!!! Unable to determine Value !!!`); a 0x32 item with a / at 0x28 is a file (Lnk took it for zip contents); and the extension blocks of the shell items in a property view's Delegate ID List are in `DelegateExtensionBlocks`, not `ExtensionBlocks` (they are those items', a path, and ShellBags takes an item's times and MFT entry from its first 0xbeef0004 block).
- Names are what paths are made of: a network item's value is its location (its provider, such as Microsoft Network, is `Description`), and a zip contents item's is its name (the folder in the zip file is `ParentFolder`; it was `name/folder/`). Both were changed in 2026.10.5 for Lnk (LnkParse3 agrees), and SBECmd's paths lost the suffixes.
- `Utils.CodePage` (thread static, 1252 by default) is the code page of the items' ANSI strings: `ShellItemFactory.Create(..., codepage)` sets it while it reads an item. The factory reads types with no class of their own as Lnk did: 0x22 as 0x23 when it has a drive letter, 0x77 as 0x74, 0xAA/0xAE as zip contents when they read as one (ShellBags' 0xAA items don't); 0x3A file entries (with a 0xbeef0004 block) are read as 0x31 items. Zip contents are found by a / in a date with byte 8 0, or with any byte 8 when a whole `MM/dd/yyyy  HH:mm:ss` is at 0x24 (some have data in bytes 4-9).
- A 0x2E item is a GUID item when byte 3 is 0x80, it is 0x14/0x16 bytes, or a 0xbeef block follows the GUID (0x48 items with 0xbeef0026: the GUID's bytes at 5 passed the control panel category check, which threw reading past the end).
- Input may be corrupt or hostile: no loop may depend on a size from the data without moving forward, and no `Trace.Assert`/`Debug.Assert` on data. A string check against a control character must be ordinal (`StartsWith("\u0001")` is true for every string under ICU: Beef000b lost the drive letter).

## Commands

The SDK is pinned in `global.json` (.NET 10). The library targets `netstandard2.0` with `LangVersion` 10; tests target `net10.0` and use NUnit 5.

```bash
dotnet build                                          # builds ExtensionBlocks.slnx
dotnet test                                           # CI runs: dotnet test --configuration Release
dotnet test --filter "Name~Beef0006"                  # a subset of tests
dotnet pack --configuration Release                   # package includes README.md and icon.png from the repo root
```

## Architecture

- **Entry point:** `Utils.GetExtensionBlockFromBytes(signature, rawBytes)` switches on the signature and returns a `BeefXXXX` instance (one class per signature, named after it). Unknown signatures return `BeefUnknown`. `rawBytes` is the whole block, starting at its 2-byte size.
- **`BeefBase`** (implements `IExtensionBlock`) reads the common header: size (UInt16 @0), version (UInt16 @2) and signature (UInt32 @4). Subclasses check the signature, parse their body, and set `VersionOffset` (usually the last 2 bytes of the block). Placeholder classes parse nothing and set `Message`.
- **Property stores:** `Beef0010`, `0021`, `0022`, `0024`, `0026` and `0027` wrap a `PropertyStore`, a list of `PropertySheet`s ("1SPS" serialized property storage). Numeric keys are named through the big `Utils.PropertyStoreInfo` table (`GetDescriptionFromGuidAndKey`).
- **`Beef0004`** (file entry: timestamps, MFT reference, long/localised name) is in almost every shell item, so its speed matters. `LongName` is the first string, `LocalisedName` the second.
- **`ShellItemFactory.Create(bytes, slot, mru, bagPath)`**: the class for an item: signatures first (CD burning, MTP, CFSF), then items found by their layout whatever the type byte (`ShellBagFindData.IsFindData`: GUID 1ca7ea69-e26a-44c9-9182-a9128722c66f at 4 and the item's find data twice, seen with OneDrive folders, which program writes them is not known; `ShellBag0X00.IsFtpItem`: FTP folders, 0x00030005 at 6, whose bytes 2-5 are 0 in some hives and other values in others, and FTP files, 0x00030009 at 6, the same layout with the file size at 0x0E (ComDlg32's MRUs; `IsFtpFile`, `FileSize`); `ShellBagDocumentDrive.IsDocumentDrive`: 0xFF folders and 0xFC files with 0x55 at 3; `ShellBagArchiveContents.IsArchiveContents`: Windows 11 archive folders, whose bytes 2-3 vary, so ShellBags sent them as unknown types of every value and other classes misread them), zip contents, then `ShellBag0X{type}` by name (0x61 URI items read nothing after their scheme: in ComDlg32's MRUs the item ends there, and a size read after it threw, RegistryPlugins #66 and Issues #261; 0x78/0x79 property views can end right after their two GUIDs, with no extension block size), else `ShellBagUnknown`; an item that can't be parsed throws (the parser's own exception). ShellBags and the plugins use it.
- **Shell items** (`ShellItems/`): `ShellBag` (abstract) and `IShellBag`, one class per type byte (`ShellBag0X31`, ...) or signature (`ShellBagMTPDevice`, `ShellBagCFSF`, `ShellBagCDBurn`, `ShellBagZipContents`) or layout (`ShellBagArchiveContents`, `ShellBagDocumentDrive`, `ShellBagFindData`), `ShellBagUnknown`, `ShellBagParseError`, `ShellBagView`. The registry-side properties (last write, slot, node slot, child bags, interacted times) are set by ShellBags. Blocks that contain shell items: `Beef0005`, `Beef000e`, `Beef0017`, and property sheet values.
- Helpers in `Utils`: `ExtractGuidFromShellItem`, `ExtractDateTimeOffsetFromBytes` (MS-DOS date/time, converted to `TimeZone`; null when unset or invalid), `FromFileTime` (`DateTimeOffset.MinValue` for an invalid FILETIME), `IndexOf` (bytes), `GetFolderNameFromGuid`.

## Tests

- `TestMain.cs` builds blocks in memory (the `Block` helper), since there are no sample files from the corpus in the repo. Add a test like these for any parser change, including the edge cases (empty strings, non-ASCII text, invalid timestamps).
- `CorpusTests.cs`: every item of the ShellBags corpus (`Corpus/ShellBagsDataCorpus` next to the repo, or `SHELLBAGS_CORPUS`; skipped without it, as on CI). Items in the type and class folders must not throw, and a class folder's (`!Zip`, `!Archive`, ...) must be that class; the `!!Unknown(ToReverse)` ones may throw but must finish, and the ones that parse now are listed (to move). Run it for any parser change; it takes about a second.
- `SharedTests.cs`: what came from ShellBags, and robustness: `TestFiles/ShellItemSamples.txt` (base64 shell items from the ShellBags tests) parsed, and their extension blocks and the items randomly damaged, each run with a time limit.
- Validation of a change that consumers see: `CorpusTests`, ShellBags' corpus dump (`CorpusDumpTests` in the ShellBagsExplorer repo) and SBECmd over the Registry corpus, old vs new; and Lnk/JumpList over the LNK and jump list corpus with the released package vs this project (a scratch console app that dumps every shell item's and extra block's `ToString()`, built twice).

## Versioning and release

- MinVer sets the version from the git tag (bare tags like `2026.10.0`); there is no `<Version>` in the csproj. Untagged commits build as `x.y.z-alpha.0.N`.
- `Directory.Build.props`: pdbs are embedded in the assemblies (`DebugType` embedded, `EmbedUntrackedSources`) and Release builds set `ContinuousIntegrationBuild` (source paths `/_/...`, not this machine's), so stack traces (Exceptionless) have file and line numbers and no .pdb file ships; the nupkg's dll carries the pdb, so there is no .snupkg. Don't add `DebugType`, `IncludeSymbols` or `SymbolPackageFormat` to project files.
- To release: push to `master`, then `git tag <ver> && git push origin <ver> && gh release create <ver> --verify-tag --generate-notes`.
- `.github/workflows/publish.yml` packs, validates and tests on PRs and on pushes to `master`. When a GitHub Release is published, it pushes to NuGet.org using trusted publishing (`NuGet/login` with the `NUGET_USER` repo variable; there is no API key secret).
- Dependabot (`.github/dependabot.yml`) opens monthly PRs for GitHub Actions and NuGet updates.
