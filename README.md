# ExtensionBlocks
Extension blocks as found in ShellBags and other places in the Registry

Parses the `0xbeefXXXX` extension blocks found at the end of Windows shell items (ShellBags, LNK files, jump lists). Pass the signature and the block's bytes to `Utils.GetExtensionBlockFromBytes` to get a typed block (`Beef0004`, `Beef0026`, ...); unknown signatures return `BeefUnknown`.
