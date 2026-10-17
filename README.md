# ExtensionBlocks
Extension blocks and shell items as found in ShellBags, LNK files, jump lists and other places in the Registry

Parses the `0xbeefXXXX` extension blocks found at the end of Windows shell items (ShellBags, LNK files, jump lists). Pass the signature and the block's bytes to `Utils.GetExtensionBlockFromBytes` to get a typed block (`Beef0004`, `Beef0026`, ...); unknown signatures return `BeefUnknown`, and a damaged block throws. `Utils.GetExtensionBlockFromBytesOrError` returns a `BeefParseError` for a damaged block instead.

Since 2026.10.1 it also holds the shell items themselves (`ShellBag0X31`, `ShellBag0X1F`, `ShellBagMTPFolder`, ... one class per type, all `IShellBag`), moved from ShellBags Explorer so SBECmd, ShellBags Explorer and other tools can share them. Times are shown in `Utils.TimeZone` (UTC by default) with `Utils.DateTimeFormat` (the culture's format by default).
