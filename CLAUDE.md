# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A `netstandard2.0` NuGet library that parses the `0xbeefXXXX` extension blocks found at the end of Windows shell items (ShellBags, LNK files, jump lists). Its main consumers are other Eric Zimmerman tools and libraries, e.g. the `Lnk` package, which hands each extension block it finds to this library. GUID → name mapping comes from the `GuidMapping` package.

## Commands

The SDK is pinned in `global.json` (.NET 10). The library targets `netstandard2.0` (default C# 7.3, so avoid newer syntax there); tests target `net10.0` and use NUnit 4.

```bash
dotnet build                                          # builds ExtensionBlocks.slnx
dotnet test                                           # CI runs: dotnet test --configuration Release
dotnet test --filter "Name~Beef0006"                  # a subset of tests
dotnet pack --configuration Release                   # package includes README.md and icon.png from the repo root
```

## Architecture

- **Entry point:** `Utils.GetExtensionBlockFromBytes(signature, rawBytes)` switches on the signature and returns a `BeefXXXX` instance (one class per signature, named after it). Unknown signatures return `BeefUnknown`. `rawBytes` is the whole block, starting at its 2-byte size.
- **`BeefBase`** (implements `IExtensionBlock`) reads the common header: size (UInt16 @0), version (UInt16 @2) and signature (UInt32 @4). Subclasses check the signature, parse their body, and set `VersionOffset` (usually the last 2 bytes of the block). Placeholder classes (`Beef0001`, `0002`, `0008`, ...) parse nothing and set `Message`.
- **Property stores:** `Beef0010`, `0021`, `0024`, `0026` and `0027` wrap a `PropertyStore`, which is a list of `PropertySheet`s ("1SPS" serialized property storage). A sheet is either named (format ID `d5cdd505-...`) or numeric. Numeric keys are resolved to names through the big `Utils.PropertyStoreInfo` table (`GetDescriptionFromGuidAndKey`), which most edits to `Utils.cs` touch.
- **`Beef0004`** (file entry: timestamps, MFT reference, long/localised name) appears in almost every shell item, so its speed matters. The name strings go through `Utils.GetStringsFromMultistring`, which is known to be slow and fragile but is left as is until it can be checked against real samples.
- `ShellBag`/`ShellBag0X31`/`IShellBag` are only used by `Beef000e`, which is *not* registered in the factory switch.
- Shared helpers in `Utils`: `ExtractGuidFromShellItem` (16 bytes → lowercase `D` GUID string), `ExtractDateTimeOffsetFromBytes` (MS-DOS date/time; returns null when the value is unset or invalid), `GetFolderNameFromGuid`.
- Parsing assumes well-formed input and throws on malformed data (signature mismatch, out-of-range reads). Callers such as `Lnk` catch these exceptions and record the block as damaged.

## Tests

`ExtensionBlocks.Test/TestMain.cs` builds blocks in memory (the `Block` helper writes size/version/signature/body/version offset), since there are no sample files in the repo. Add a test like these for any parser change, including the edge cases (empty strings, non-ASCII text, invalid timestamps).

## Versioning and release

- MinVer sets the version from the git tag (bare tags like `2026.10.0`); there is no `<Version>` in the csproj. Untagged commits build as `x.y.z-alpha.0.N`.
- To release: push to `master`, then `git tag <ver> && git push origin <ver> && gh release create <ver> --verify-tag --generate-notes`.
- `.github/workflows/publish.yml` packs, validates and tests on PRs and on pushes to `master`. When a GitHub Release is published, it pushes to NuGet.org using trusted publishing (`NuGet/login` with the `NUGET_USER` repo variable; there is no API key secret).
- Dependabot (`.github/dependabot.yml`) opens monthly PRs for GitHub Actions and NuGet updates.
