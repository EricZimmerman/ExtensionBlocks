# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A `netstandard2.0` NuGet library that parses the `0xbeefXXXX` extension blocks found at the end of Windows shell items (ShellBags, LNK files, jump lists), and (since 2026.10.1) the shell items themselves, moved here from the ShellBags library in the ShellBagsExplorer repo so they are shared. GUID → name mapping comes from the `GuidMapping` package.

Consumers:
- `ShellBags` (ShellBagsExplorer repo: SBECmd and ShellBags Explorer): the shell items, the blocks, `GetExtensionBlockFromBytesOrError`. It keeps only the registry side (`RawRegistryData`: BagMRU keys, the class by type byte `ExtensionBlocks.ShellBag0X{type}` found by reflection in this assembly). It sets `Utils.DateTimeFormat` to `yyyy-MM-dd HH:mm:ss` when it loads, and `Utils.TimeZone`.
- `Lnk` (and `JumpList` through it): only the blocks (`GetExtensionBlockFromBytes`, which throws on damage; Lnk records the block as damaged) and `PropertyStore`/`PropertySheet`. Lnk has its own shell item classes. Its output (LECmd, JLECmd) shows the blocks' and sheets' `ToString()`.
- `RegistryPluginBase` (RegistryPlugins): the blocks; three plugins (LastVisitedPidlMRU, OpenSavePidlMRU, Taskband) have their own older copies of the shell items.

## Behavior to keep

- Public names and visibility: every block and shell item class is public; block classes are named with lower case hex (`Beef000a`, `Beef001d`), shell items with upper case (`ShellBag0X31`, as ShellBags finds them by name). `Beef001d.Executable`, `Beef0005.InternalBags`, `Utils.ShellBagTypes` are public API.
- `GetExtensionBlockFromBytes` throws on damaged data (Lnk relies on it); `GetExtensionBlockFromBytesOrError` never throws (the block's type from its own signature byte, as ShellBags did).
- Defaults keep Lnk's output: `Utils.DateTimeFormat` null = the culture's pattern with milliseconds and the offset; `Beef0026` keeps `yyyy-MM-dd HH:mm:ss.fffffff` unless a format is set; `Utils.TimeZone` UTC.
- `GetDateTimeFormatWithMilliseconds` is cached (it was a regular expression per call and made property sheets twice as slow).
- `PropertySheet` keeps the shell items found in VT_VECTOR, VT_STREAM and VT_BLOB values as `ExtraData`, made when first read (`LazyText`): building their text while parsing made Lnk parse jump lists 60% slower. Only the 0x00 item in streams is made while parsing (it has its own "possible false positive" message). Sheet text: `Guid: ..., Key: n ==> property name, Value: ...` for numeric sheets.
- `PropertyStore` keeps the sheets it could read (`ParseErrors`), and throws only when none could be read.
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
- **Shell items** (`ShellItems/`): `ShellBag` (abstract) and `IShellBag`, one class per type byte (`ShellBag0X31`, ...) or signature (`ShellBagMTPDevice`, `ShellBagCFSF`, `ShellBagCDBurn`, `ShellBagZipContents`), `ShellBagUnknown`, `ShellBagParseError`, `ShellBagView`. The registry-side properties (last write, slot, node slot, child bags, interacted times) are set by ShellBags. Blocks that contain shell items: `Beef0005`, `Beef000e`, `Beef0017`, and property sheet values.
- Helpers in `Utils`: `ExtractGuidFromShellItem`, `ExtractDateTimeOffsetFromBytes` (MS-DOS date/time, converted to `TimeZone`; null when unset or invalid), `FromFileTime` (`DateTimeOffset.MinValue` for an invalid FILETIME), `IndexOf` (bytes), `GetFolderNameFromGuid`.

## Tests

- `TestMain.cs` builds blocks in memory (the `Block` helper), since there are no sample files from the corpus in the repo. Add a test like these for any parser change, including the edge cases (empty strings, non-ASCII text, invalid timestamps).
- `SharedTests.cs`: what came from ShellBags, and robustness: `TestFiles/ShellItemSamples.txt` (base64 shell items from the ShellBags tests) parsed, and their extension blocks and the items randomly damaged, each run with a time limit.
- Validation of a change that consumers see: ShellBags' corpus dump (`CorpusDumpTests` in the ShellBagsExplorer repo) and SBECmd over the Registry corpus, old vs new; and Lnk/JumpList over the LNK and jump list corpus with the released package vs this project (a scratch console app that dumps every shell item's and extra block's `ToString()`, built twice).

## Versioning and release

- MinVer sets the version from the git tag (bare tags like `2026.10.0`); there is no `<Version>` in the csproj. Untagged commits build as `x.y.z-alpha.0.N`.
- To release: push to `master`, then `git tag <ver> && git push origin <ver> && gh release create <ver> --verify-tag --generate-notes`.
- `.github/workflows/publish.yml` packs, validates and tests on PRs and on pushes to `master`. When a GitHub Release is published, it pushes to NuGet.org using trusted publishing (`NuGet/login` with the `NUGET_USER` repo variable; there is no API key secret).
- Dependabot (`.github/dependabot.yml`) opens monthly PRs for GitHub Actions and NuGet updates.
