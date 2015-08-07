namespace ExtensionBlocks
{
    public interface IExtensionBlock
    {
        int Size { get; }

        int Version { get; }

        uint Signature { get; }

        int VersionOffset { get; }
    }
}