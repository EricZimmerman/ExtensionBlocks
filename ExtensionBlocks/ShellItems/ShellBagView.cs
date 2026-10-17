using System;
using System.Drawing;
using System.Text;

namespace ExtensionBlocks;

/// <summary>
///     This is an abstraction of a ShellBag. Each distinct type must be able to generate one of these so things can be
///     displayed in a somewhat consistent manner across different types
/// </summary>
public class ShellBagView
{
    public string Value { get; set; }

    //public Bitmap Icon { get; set; }

    public string ShellType { get; set; }

    public string BagPath { get; set; }

    public string AbsolutePath { get; set; }

    public string InternalID { get; set; }
    public int NodeSlot { get; set; }

    public string Miscellaneous { get; set; }

    public byte[] HexValue { get; set; }

    public int Slot { get; set; }

    public int MRUPosition { get; set; }

    public int ChildBags { get; set; }

    public DateTimeOffset? CreatedOn { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }

    public DateTimeOffset? AccessedOn { get; set; }

    public DateTimeOffset? LastWriteTime { get; set; }
    public DateTimeOffset? FirstInteracted { get; set; }
    public DateTimeOffset? LastInteracted { get; set; }

    public bool HasExplored { get; set; }

    public ulong? MFTEntry { get; set; }

    public int? MFTSequenceNumber { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Value: {Value}");
        sb.AppendLine($"Shell Type: {ShellType}");

        sb.AppendLine();

        sb.AppendLine($"Absolute Path: {AbsolutePath}");
        sb.AppendLine($"BagPath: {BagPath}");
        sb.AppendLine($"Slot #: {Slot}");
        sb.AppendLine($"MRU Position: {MRUPosition}");
        sb.AppendLine($"NodeSlot: {NodeSlot}");

        sb.AppendLine();

        sb.AppendLine($"# ChildBags: {ChildBags}");

        sb.AppendLine();

        sb.AppendLine($"Created On: {(CreatedOn.HasValue ? CreatedOn.Value.ToString() : "NA")}");
        sb.AppendLine($"Modified On: {(ModifiedOn.HasValue ? ModifiedOn.Value.ToString() : "NA")}");
        sb.AppendLine($"Accessed On: {(AccessedOn.HasValue ? AccessedOn.Value.ToString() : "NA")}");

        if (LastWriteTime.HasValue)
        {
            sb.AppendLine($"Last Write Time: {LastWriteTime.Value}");
        }

        if (MFTEntry.HasValue)
        {
            sb.AppendLine();

            sb.AppendLine($"MFT Entry #: {MFTEntry.Value}");

            if (MFTSequenceNumber.HasValue)
            {
                sb.AppendLine($"MFT Sequence #: {MFTSequenceNumber.Value}");
            }
        }

        sb.AppendLine();

        sb.AppendLine($"HexValue: {BitConverter.ToString(HexValue)}");

        return sb.ToString();
    }
}