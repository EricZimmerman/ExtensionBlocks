using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtensionBlocks
{
    public class Utils
    {
        public static string GetDateTimeFormatWithMilliseconds()
        {
            string fullPattern =
                $"{DateTimeFormatInfo.CurrentInfo.ShortDatePattern} {DateTimeFormatInfo.CurrentInfo.LongTimePattern}";
            fullPattern = Regex.Replace(fullPattern, "(:ss|:s)", "$1.fff");

            fullPattern = $"{fullPattern} {" K"}";

            return fullPattern;
        }

        private class IdName:IEquatable<IdName>
        {
            public IdName(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public int Id { get; }
            public string Name { get; }


            public bool Equals(IdName other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return Id == other.Id && string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((IdName) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Id * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                }
            }
        }

        public enum ShellBagTypes
        {
            Directory,
            ZipFileContents,
            Variable,
            DriveLetter,
            Archive,
            FtpUri,
            Unknown,
            History,
            User,
            NotAssigned,
            MtpDevice,
            MtpFolder,
            MtpStorage,
            Network,
            Guid,
            ControlPanelCategory,
            File,
            HyperVResource,
            OpticalMedia,
            Backup,
            ParseError
        }

        private static Dictionary<string, HashSet<IdName>> PropertyStoreInfo = new Dictionary<string, HashSet<IdName>>()
        {
            //updated list provided by david.via@FireEye.com on 03/27/2019!

            {"46588ae2-4cbc-4338-bbfc-139326986dce",new HashSet<IdName>()
            {
                new IdName(4,"SID"),
            } },


            //new block
            {"dabd30ed-0043-4789-a7f8-d013a4736622",new HashSet<IdName>()
            {
                new IdName(100,"Item Folder Path Display Narrow"),
            } },

            {"28636aa6-953d-11d2-b5d6-00c04fd918d0",new HashSet<IdName>()
            {
                new IdName(    0, "Find Data"),
                new IdName(    1, "Network Resource"),
                new IdName(    2, "Description ID"),
                new IdName(    3, "Which Folder"),
                new IdName(    4, "Network Location"),
                new IdName(    5, "Computer Name"),
                new IdName(    6, "Namespace CLSID"),
                new IdName(    8, "Item Path Display Narrow"),
                new IdName(    9, "Perceived Type"),
                new IdName(    10, "Computer Simple Name"),
                new IdName(    11, "Item Type"),
                new IdName(    12, "File Count"),
                new IdName(    14, "Total File Size"),
                new IdName(    22, "Max Stack Count"),
                new IdName(    23, "List Description"),
                new IdName(    24, "Parsing Name"),
                new IdName(    25, "SFGAO Flags"),
                new IdName(    26, "Order"),
                new IdName(    27, "Computer Description"),
                new IdName(    29, "Contained Items"),
                new IdName(    30, "Parsing Path"),
                new IdName(    31, "Network Provider"),
                new IdName(    32, "Delegate ID List"),
                new IdName(    33, "Is SendTo Target"),
                new IdName(    34, "Hide On Desktop"),
                new IdName(    35, "Network Places Default Name"),
                new IdName(    36, "Storage System Type"),
                new IdName(    37, "Item SubType"),
            } },

            {"9f4c2855-9f79-4b39-a8d0-e1d42de1d5f3",new HashSet<IdName>()
            {
                new IdName(    2, "App User Model Relaunch Command"),
                new IdName(    3, "App User Model Relaunch Icon Resource"),
                new IdName(    4, "App User Model Relaunch Display Name Resource"),
                new IdName(    5, "App User Model ID"),
                new IdName(    6, "App User Model Is DestList Separator"),
                new IdName(    7, "App User Model Is DestList Link"),
                new IdName(    8, "App User Model Exclude From Show In New Install"),
                new IdName(    9, "App User Model Prevent Pinning"),
                new IdName(    10, "App User Model Best Shortcut"),
                new IdName(    11, "App User Model Is Dual Mode"),
                new IdName(    12, "App User Model Start Pin Option"),
                new IdName(    13, "App User Model Relevance"),
                new IdName(    14, "App User Model Host Environment"),
                new IdName(    15, "App User Model Package Install Path"),
                new IdName(    16, "App User Model Record State"),
                new IdName(    17, "App User Model Package Family Name"),
                new IdName(    18, "App User Model Installed By"),
                new IdName(    19, "App User Model Parent ID"),
                new IdName(    20, "App User Model Activation Context"),
                new IdName(    21, "App User Model Package Full Name"),
                new IdName(    22, "App User Model Package Relative Application ID"),
                new IdName(    23, "App User Model Excluded From Launcher"),
                new IdName(    24, "App User Model AppCompat ID"),
                new IdName(    25, "App User Model Run Flags"),
                new IdName(    26, "App User Model Toast Activator CLSID"),
                new IdName(    27, "App User Model DestList Provided Title"),
                new IdName(    28, "App User Model DestList Provided Description"),
                new IdName(    29, "App User Model DestList Logo Uri"),
                new IdName(    30, "App User Model DestList Provided Group Name"),
            } },

            {"446d16b1-8dad-4870-a748-402ea43d788c",new HashSet<IdName>()
            {
                new IdName(100,"Thumbnail Cache Id"),
                new IdName(104,"Volume Id"),
                new IdName(105,"Tooltip Thumbnail Stream"),
            } },

            {"fb8d2d7b-90d1-4e34-bf60-6eac09922bbf",new HashSet<IdName>()
            {
                new IdName(2,"WinX Hash"),
            } },

            {"f29f85e0-4ff9-1068-ab91-08002b27b3d9",new HashSet<IdName>()
            {
                new IdName(    3, "Subject"),
                new IdName(    4, "Author"),
                new IdName(    5, "Keywords"),
                new IdName(    6, "Comment"),
                new IdName(    7, "Document Template"),
                new IdName(    8, "Document Last Author"),
                new IdName(    9, "Document Revision Number"),
                new IdName(    10, "Document Total Editing Time"),
                new IdName(    11, "Document Date Printed"),
                new IdName(    12, "Document Date Created"),
                new IdName(    13, "Document Date Saved"),
                new IdName(    14, "Document Page Count"),
                new IdName(    15, "Document Word Count"),
                new IdName(    16, "Document Character Count"),
                new IdName(    17, "Thumbnail"),
                new IdName(    18, "Application Name"),
                new IdName(    19, "Document Security"),
                new IdName(    24, "High Keywords"),
                new IdName(    25, "Low Keywords"),
                new IdName(    26, "Medium Keywords"),
                new IdName(    27, "Thumbnail Stream"),
            } },


            {"841e4f90-ff59-4d16-8947-e81bbffab36d",new HashSet<IdName>()
            {
                new IdName(    2, "Publisher Display Name"),
                new IdName(    3, "Software Registered Owner"),
                new IdName(    4, "Software Registered Company"),
                new IdName(    5, "Software AppId"),
                new IdName(    6, "Software Support Url"),
                new IdName(    7, "Software Support Telephone"),
                new IdName(    8, "Software Help Link"),
                new IdName(    9, "Software Install Location"),
                new IdName(    10, "Software Install Source"),
                new IdName(    11, "Software Date Installed"),
                new IdName(    12, "Software Support Contact Name"),
                new IdName(    13, "Software ReadMe Url"),
                new IdName(    14, "Software Update Info Url"),
                new IdName(    15, "Software Times Used"),
                new IdName(    16, "Software Date Last Used"),
                new IdName(    17, "Software Tasks File Url"),
                new IdName(    18, "Software Parent Name"),
                new IdName(    19, "Software Product ID"),
                new IdName(    20, "Software Comments"),
                new IdName(    997, "Software Null Preview Total Size"),
                new IdName(    998, "Software Null Preview Subtitle"),
                new IdName(    999, "Software Null Preview Title"),
            } },

            {"86d40b4d-9069-443c-819a-2a54090dccec",new HashSet<IdName>()
            {
                new IdName(    2, "Tile Small Image Location"),
                new IdName(    4, "Tile Background Color"),
                new IdName(    5, "Tile Foreground Color"),
                new IdName(    11, "Tile Display Name"),
                new IdName(    12, "Tile Image Location"),
                new IdName(    13, "Tile Wide 310x150 Logo Path"),
                new IdName(    14, "Tile Unknown Flags"),
                new IdName(    15, "Tile Badge Logo Path"),
                new IdName(    16, "Tile Suite Display Name"),
                new IdName(    17, "Tile Suite Sor tName"),
                new IdName(    18, "Tile Display Name Language"),
                new IdName(    19, "Tile Square 310x310 Logo Path"),
                new IdName(    20, "Tile Square 70x70 Logo Path"),
                new IdName(    21, "Tile Fence Post"),
                new IdName(    22, "Tile Install Progress"),
                new IdName(    23, "Tile Encoded Target Path"),
            } },

            {"b725f130-47ef-101a-a5f1-02608c9eebac",new HashSet<IdName>()
            {
                new IdName(    2, "Item Folder Name Display"),
                new IdName(    3, "Search ClassID"),
                new IdName(    4, "Item Type Text"),
                new IdName(    8, "File Index"),
                new IdName(    9, "Search Last Change USN"),
                new IdName(    10, "Item Name Display"),
                new IdName(    12, "Size"),
                new IdName(    13, "File Attributes"),
                new IdName(    14, "Date Modified"),
                new IdName(    15, "Date Created"),
                new IdName(    16, "Date Accessed"),
                new IdName(    18, "File Allocation Size"),
                new IdName(    19, "Search Contents"),
                new IdName(    20, "Search ShortName"),
                new IdName(    21, "File FRN"),
                new IdName(    22, "Search Scope"),
                new IdName(    23, "Item Name Sort Override"),
                new IdName(    24, "Item Name Display Without Extension"),
                new IdName(    25, "Folder Name Display"),
            } },

            {"e3e0584c-b788-4a5a-bb20-7f5a44c9acdd",new HashSet<IdName>()
            {
                new IdName(  2, "Message Bcc Address"),
                new IdName(    3, "Message Bcc Name"),
                new IdName(    4, "Message Cc Address"),
                new IdName(    5, "Message Cc Name"),
                new IdName(    6, "Item Folder Path Display"),
                new IdName(    7, "Item Path Display"),
                new IdName(    9, "Communication Account Name"),
                new IdName(    10, "Is Read"),
                new IdName(    11, "Importance"),
                new IdName(    12, "Flag Status"),
                new IdName(    13, "Message From Address"),
                new IdName(    14, "Message From Name"),
                new IdName(    15, "Message Store"),
                new IdName(    16, "Message To Address"),
                new IdName(    17, "Message To Name"),
                new IdName(    18, "Contact Web Page"),
                new IdName(    19, "Message Date Sent"),
                new IdName(    20, "Message Date Received"),
                new IdName(    21, "Message Attachment Names"),
            } },

            {"00000000-0000-0000-0000-000000000000",new HashSet<IdName>()
            {
                new IdName(    0, "Null"),
            } },

            {"000214a1-0000-0000-c000-000000000046}",new HashSet<IdName>()
            {
                new IdName(    9, "Status"),
            } },


            {"00bc20a3-bd48-4085-872c-a88d77f5097e",new HashSet<IdName>()
            {
                new IdName(    105, "Music Composer Sort Override"),
            } },

            {"00f58a38-c54b-4c40-8696-97235980eae1",new HashSet<IdName>()
            {
                new IdName(    100, "Calendar Resources"),
            } },

            {"00f63dd8-22bd-4a5d-ba34-5cb0b9bdcb03",new HashSet<IdName>()
            {
                new IdName(    101, "Contact Job Info1 Yomi Company Name"),
                new IdName(    102, "Contact Job Info1 Company Name"),
                new IdName(    103, "Contact Job Info1 Title"),
                new IdName(    104, "Contact Job Info1 Office Location"),
                new IdName(    105, "Contact Job Info1 Manager"),
                new IdName(    106, "Contact Job Info1 Department"),
                new IdName(    107, "Contact Job Info2 Yomi Company Name"),
                new IdName(    108, "Contact Job Info2 Company Name"),
                new IdName(    109, "Contact Job Info2 Title"),
                new IdName(    110, "Contact Job Info2 Office Location"),
                new IdName(    112, "Contact Job Info2 Manager"),
                new IdName(    113, "Contact Job Info2 Department"),
                new IdName(    114, "Contact Job Info3 Yomi Company Name"),
                new IdName(    115, "Contact Job Info3 Company Name"),
                new IdName(    116, "Contact Job Info3 Title"),
                new IdName(    117, "Contact Job Info3 Office Location"),
                new IdName(    118, "Contact Job Info3 Manager"),
                new IdName(    119, "Contact Job Info3 Department"),
                new IdName(    120, "Contact Job Info1 Company Address"),
                new IdName(    121, "Contact Job Info2 Company Address"),
                new IdName(    123, "Contact Job Info3 Company Address"),
                new IdName(    124, "Contact Webpage 2"),
                new IdName(    125, "Contact Webpage 3"),
            } },


            {"026e516e-b814-414b-83cd-856d6fef4822",new HashSet<IdName>()
            {
                new IdName(    3, "Devices Interface Enabled"),
                new IdName(    4, "Devices Interface Class Guid"),
                new IdName(    6, "Devices Restricted Interface"),
            } },

            {"029c0252-5b86-46c7-aca0-2769ffc8e3d4",new HashSet<IdName>()
            { 
                new IdName (100, "GPS Latitude Ref"),
            } },

            {"02b0f689-a914-4e45-821d-1dda452ed2c4",new HashSet<IdName>()
            { 
                new IdName (100, "GPS Longitude Numerator"),
            } },

            {"03089873-8ee8-4191-bd60-d31f72b7900b",new HashSet<IdName>()
            { 
                new IdName (100, "Contact Display Other Phone Numbers"),
            } },

            


 {"0337ecec-39fb-4581-a0bd-4c4cc51e9914",new HashSet<IdName>()
             { 
new IdName (100, "Photo Aperture Numerator"),
} },

 {"048658ad-2db8-41a4-bbb6-ac1ef1207eb1",new HashSet<IdName>()
             { 
new IdName (100, "Item Class Type"),
} },

 {"05e932b1-7ca2-491f-bd69-99b4cb266cbb",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Disambiguation Text"),
} },

 {"06704b0c-e830-4c81-9178-91e4e95a80a0",new HashSet<IdName>()
             {
    new IdName (2, "Devices Notification Store"),
    new IdName (3, "Devices Notification"),
} },


 {"084d8a0a-e6d5-40de-bf1f-c8820e7c877c",new HashSet<IdName>()
             { 
new IdName (100, "Task CompletionStatus"),
} },

 {"08a65aa1-f4c9-43dd-9ddf-a33d8e7ead85",new HashSet<IdName>()
             { 
new IdName (100, "Contact HomeAddressCountry"),
} },

 {"08c7cc5f-60f2-4494-ad75-55e3e0b5add0",new HashSet<IdName>()
             { 
new IdName (100, "Task Owner"),
} },

 {"08f6d7c2-e3f2-44fc-af1e-5aa5c81a2d3e",new HashSet<IdName>()
             { 
new IdName (100, "Photo MaxAperture"),
} },

 {"09329b74-40a3-4c68-bf07-af9a572f607c",new HashSet<IdName>()
             { 
new IdName (100, "Is Folder"),
} },

 {"0933f3f5-4786-4f46-a8e8-d64dd37fa521",new HashSet<IdName>()
             { 
new IdName (100, "Photo Focal Plane X Resolution Denominator"),
} },

 {"09429607-582d-437f-84c3-de93a2b24c3c",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Optional AttendeeNames"),
} },

 {"09736039-456b-4219-ba3e-ec573b58cf97",new HashSet<IdName>()
             { 
new IdName (2, "Secondary Tile Is Uninstalled"),
} },

 {"09edd5b6-b301-43c5-9990-d00302effd46",new HashSet<IdName>()
             { 
new IdName (100, "Media Average Level"),
} },

 {"0a7b84ef-0c27-463f-84ef-06c5070001be",new HashSet<IdName>()
             { 
new IdName (10, "Device Interface Printer Name"),
} },

 {"0abe4d16-9384-426b-b41a-eac3c8e0f147",new HashSet<IdName>()
             { 
new IdName (2, "Search Content Snippet"),
} },

 {"0adef160-db3f-4308-9a21-06237b16fa2a",new HashSet<IdName>()
             { 
new IdName (100, "Contact Home Address Street"),
} },

 {"0b48f35a-be6e-4f17-b108-3c4073d1669a",new HashSet<IdName>()
             { 
new IdName (15, "Device Printer URL"),
} },

 {"0b63e343-9ccc-11d0-bcdb-00805fccce04",new HashSet<IdName>()
             {
    new IdName (2, "Search Url To Index"),
    new IdName (12, "Search Url To Index With Modification Time"),
    new IdName (23, "Search Is Closed Directory"),
    new IdName (24, "Search Is Fully Contained"),
    new IdName (25, "Search Provider Class"),
    new IdName (26, "Search Provider Web Domain"),
    new IdName (27, "Search Provider Result Limit"),
} },


 {"0b63e350-9ccc-11d0-bcdb-00805fccce04",new HashSet<IdName>()
             {
    new IdName (5, "MIME Type"),
    new IdName (8, "Search Gather Time"),
    new IdName (9, "Search Access Count"),
    new IdName (11, "Search Last Indexed Total Time"),
} },


 {"0b8bb018-2725-4b44-92ba-7933aeb2dde7",new HashSet<IdName>()
             {
    new IdName (2, "Contact Account Picture Dynamic Video"),
    new IdName (3, "Contact Account Picture Large"),
    new IdName (4, "Contact Account Picture Small"),
} },


 {"0ba7d6c3-568d-4159-ab91-781a91fb71e5",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Required Attendee Addresses"),
} },

 {"0bba1ede-7566-4f47-90ec-25fc567ced2a",new HashSet<IdName>()
             {
    new IdName (2, "Devices AepContainer Children"),
    new IdName (3, "Devices AepContainer Can Pair"),
    new IdName (4, "Devices AepContainer Is Paired"),
    new IdName (6, "Devices AepContainer Manufacturer"),
    new IdName (7, "Devices AepContainer Model Name"),
    new IdName (8, "Devices AepContainer Model Ids"),
    new IdName (9, "Devices AepContainer Categories"),
    new IdName (11, "Devices AepContainer Is Present"),
    new IdName (12, "Devices AepContainer Container Id"),
    new IdName (13, "Devices AepContainer Protocol Ids"),
} },


 {"0be1c8e7-1981-4676-ae14-fdd78f05a6e7",new HashSet<IdName>()
             { 
new IdName (100, "Message Sender Address"),
} },

 {"0be3fd71-3f87-40e0-aead-0294cf674635",new HashSet<IdName>()
             { 
new IdName (2, "Shell Is Dav Resource"),
} },

 {"0c73b141-39d6-4653-a683-cab291eaf95b",new HashSet<IdName>()
             {
    new IdName (2, "Supplemental Album Id"),
    new IdName (3, "Supplemental Resource Id"),
} },


 {"0c840a88-b043-466d-9766-d4b26da3fa77",new HashSet<IdName>()
             { 
new IdName (100, "Photo Subject Distance Denominator"),
} },

 {"0cb2bf5a-9ee7-4a86-8222-f01e07fdadaf",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Photo Advanced"),
} },

 {"0cef7d53-fa64-11d1-a203-0000f81fedee",new HashSet<IdName>()
             {
    new IdName (3, "File Description"),
    new IdName (4, "File Version"),
    new IdName (5, "Internal Name"),
    new IdName (6, "Original File Name"),
    new IdName (7, "Software Product Name"),
    new IdName (8, "Software Product Version"),
    new IdName (9, "Trademarks"),
    new IdName (11, "Platform"),
} },

 

 {"0cf8fb02-1837-42f1-a697-a7017aa289b9",new HashSet<IdName>()
             { 
new IdName (100, "GPS DOP"),
} },

 {"0da41cfa-d224-4a18-ae2f-596158db4b3a",new HashSet<IdName>()
             { 
new IdName (100, "Message Sender Name"),
} },

 {"0ded77b3-c614-456c-ae5b-285b38d7b01b",new HashSet<IdName>()
             {
    new IdName (2, "Launcher Order"),
    new IdName (3, "Launcher Group ID"),
    new IdName (6, "Launcher View ID"),
    new IdName (7, "Launcher App State"),
    new IdName (8, "Launcher Tile Size"),
    new IdName (9, "Launcher Group Name"),
    new IdName (10, "Launcher Splash Screen Image"),
    new IdName (11, "Launcher TileSize Timestamp"),
    new IdName (12, "Launcher ItemPosition Timestamp"),
    new IdName (13, "Launcher View ID Timestamp"),
    new IdName (14, "Launcher Group Membership Timestamp"),
    new IdName (15, "Launcher Group Name Timestamp"),
    new IdName (16, "Launcher Default Tile Size"),
    new IdName (17, "Launcher Placeholder Expiry Candidate"),
    new IdName (18, "Launcher Placeholder Expiry Candidate Timestamp"),
    new IdName (19, "Launcher Item Flags"),
    new IdName (20, "Launcher Group Position Timestamp"),
    new IdName (21, "Launcher Store Category"),
    new IdName (22, "Launcher Win Store Category Name"),
    new IdName (23, "Launcher SubgroupID"),
} },


 {"0f55cde2-4f49-450d-92c1-dcd16301b1b7",new HashSet<IdName>()
             { 
new IdName (100, "GPS Latitude Decimal"),
} },

 {"10984e0a-f9f2-4321-b7ef-baf195af4319",new HashSet<IdName>()
             { 
new IdName (100, "Parental Rating Reason"),
} },

 {"10b24595-41a2-4e20-93c2-5761c1395f32",new HashSet<IdName>()
             { 
new IdName (100, "GPS Img Direction Denominator"),
} },

 {"10dabe05-32aa-4c29-bf1a-63e2d220587f",new HashSet<IdName>()
             { 
new IdName (100, "Image Image Id"),
} },

 {"1173f62a-2a55-4f62-aed6-8c7112e0f7a3",new HashSet<IdName>()
             { 
new IdName (5, "Force Full Text"),
} },

 {"11d6336b-38c4-4ec9-84d6-eb38d0b150af",new HashSet<IdName>()
             { 
new IdName (100, "Contact Other Email Addresses"),
} },

 {"125491f4-818f-46b2-91b5-d537753617b2",new HashSet<IdName>()
             { 
new IdName (100, "GPS Status"),
} },

 {"12ea418f-d8cd-4cdf-9b23-457eaac7ff0d",new HashSet<IdName>()
             { 
new IdName (100, "Communication Directory Server"),
} },

 {"12fa14f5-c6fe-4545-bce2-1ed6cb6b8422",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Link Text"),
} },

 {"13673f42-a3d6-49f6-b4da-ae46e0c5237c",new HashSet<IdName>()
             { 
new IdName (2, "Devices DevObject Type"),
} },

 {"13eb7ffc-ec89-4346-b19d-ccc6f1784223",new HashSet<IdName>()
             { 
new IdName (101, "Music Album Title Sort Override"),
} },

 {"14977844-6b49-4aad-a714-a4513bf60460",new HashSet<IdName>()
             { 
new IdName (100, "Contact First Name"),
} },

 {"149c0b69-2c2d-48fc-808f-d318d78c4636",new HashSet<IdName>()
             { 
new IdName (2, "Volume Is Mapped Drive"),
} },

 {"14b81da1-0135-4d31-96d9-6cbfc9671a99",new HashSet<IdName>()
             {
    new IdName (259, "Image Compression"),
    new IdName (271, "Photo Camera Manufacturer"),
    new IdName (272, "Photo Camera Model"),
    new IdName (273, "Photo Camera Serial Number"),
    new IdName (274, "Photo Orientation"),
    new IdName (305, "Software Used"),
    new IdName (18248, "Photo Event"),
    new IdName (18258, "Date Imported"),
    new IdName (33432, "Image Copyright"),
    new IdName (33434, "Photo Exposure Time"),
    new IdName (33437, "Photo FNumber"),
    new IdName (34850, "Photo Exposure Program"),
    new IdName (34855, "Photo ISO Speed"),
    new IdName (36867, "Photo Date Taken"),
    new IdName (37377, "Photo Shutter Speed"),
    new IdName (37378, "Photo Aperture"),
    new IdName (37380, "Photo Exposure Bias"),
    new IdName (37382, "Photo Subject Distance"),
    new IdName (37383, "Photo Metering Mode"),
    new IdName (37384, "Photo Light Source"),
    new IdName (37385, "Photo Flash"),
    new IdName (37386, "Photo Focal Length"),
    new IdName (40096, "Image Property Bag"),
    new IdName (40961, "Image Color Space"),
    new IdName (41483, "Photo Flash Energy"),
} },


 {"1506935d-e3e7-450f-8637-82233ebe5f6e",new HashSet<IdName>()
             {
    new IdName (2, "Devices WiFi Direct Interface Address"),
    new IdName (3, "Devices WiFi Direct Interface Guid"),
    new IdName (4, "Devices WiFi Direct Group Id"),
    new IdName (5, "Devices WiFi Direct Is Connected"),
    new IdName (6, "Devices WiFi Direct Is Visible"),
    new IdName (7, "Devices WiFi Direct Is Legacy Device"),
    new IdName (8, "Devices WiFi Direct Miracast Version"),
    new IdName (9, "Devices WiFi Direct Is Miracast Lcp Supported"),
    new IdName (10, "Devices WiFi Direct Services"),
    new IdName (11, "Devices WiFi Direct Supported ChannelList"),
    new IdName (12, "Devices WiFi Direct Information Elements"),
    new IdName (13, "Devices WiFi Direct Device Address"),
} },


 {"16473c91-d017-4ed9-ba4d-b6baa55dbcf8",new HashSet<IdName>()
             { 
new IdName (100, "GPS Img Direction"),
} },

 {"16cbb924-6500-473b-a5be-f1599bcbe413",new HashSet<IdName>()
             { 
new IdName (100, "Photo Digital Zoom Numerator"),
} },

 {"16e634ee-2bff-497b-bd8a-4341ad39eeb9",new HashSet<IdName>()
             { 
new IdName (100, "GPS Latitude Denominator"),
} },

 {"16ea4042-d6f4-4bca-8349-7c78d30fb333",new HashSet<IdName>()
             { 
new IdName (100, "Photo Shutter Speed Numerator"),
} },

 {"176dc63c-2688-4e89-8143-a347800f25e9",new HashSet<IdName>()
             {
    new IdName (6, "Contact Job Title"),
    new IdName (7, "Contact Office Location"),
    new IdName (20, "Contact Home Telephone"),
    new IdName (25, "Contact Primary Telephone"),
    new IdName (35, "Contact Mobile Telephone"),
    new IdName (47, "Contact Birthday"),
    new IdName (48, "Contact Primary Email Address"),
    new IdName (65, "Contact Hom eAddress City"),
    new IdName (69, "Contact Personal Title"),
    new IdName (70, "Contact Given Name"),
    new IdName (71, "Contact Middle Name"),
    new IdName (73, "Contact Suffix"),
    new IdName (74, "Contact Nick Name"),
    new IdName (75, "Contact Prefix"),
} },


 {"1804d1fb-9fa4-441d-a536-76468ac43307",new HashSet<IdName>()
             { 
new IdName (100, "WebDav Path"),
} },

 {"182c1ea6-7c1c-4083-ab4b-ac6c9f4ed128",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Longitude Ref"),
} },

 {"188c1f91-3c40-4132-9ec5-d8b03b72a8a2",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Response Status"),
} },

 {"18bbd425-ecfd-46ef-b612-7b4a6034eda0",new HashSet<IdName>()
             { 
new IdName (100, "Contact Primary Address Postal Code"),
} },

 {"19b51fa6-1f92-4a5c-ab48-7df0abd67444",new HashSet<IdName>()
             { 
new IdName (100, "Image Resolution Unit"),
} },

 {"1a701bf6-478c-4361-83ab-3701bb053c58",new HashSet<IdName>()
             { 
new IdName (100, "Photo Brightness"),
} },

 {"1a9ba605-8e7c-4d11-ad7d-a50ada18ba1b",new HashSet<IdName>()
             { 
new IdName (2, "Message Participants"),
} },

 {"1b5439e7-eba1-4af8-bdd7-7af1d4549493",new HashSet<IdName>()
             { 
new IdName (100, "RecordedTV Station Name"),
} },

 {"1b97738a-fdfc-462f-9d93-1957e08be90c",new HashSet<IdName>()
             { 
new IdName (100, "Photo FNumber Numerator"),
} },

 

 {"30c8eef4-a832-41e2-ab32-e3c3ca28fd29",new HashSet<IdName>()
             {
    new IdName (2, "Home Grouping"),
    new IdName (3, "Home Sort Order"),
    new IdName (4, "Home Is Pinned"),
    new IdName (5, "Home PropList Sort"),
    new IdName (6, "Home Item Folder Path Display"),
} },


 {"3143bf7c-80a8-4854-8880-e2e40189bdd0",new HashSet<IdName>()
             { 
new IdName (100, "Message Attachment Contents"),
} },

 {"315b9c8d-80a9-4ef9-ae16-8e746da51d70",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Is Recurring"),
} },

 {"318a6b45-087f-4dc2-b8cc-05359551fc9e",new HashSet<IdName>()
             { 
new IdName (100, "Photo Related Sound File"),
} },

 {"31b37743-7c5e-4005-93e6-e953f92b82e9",new HashSet<IdName>()
             {
    new IdName (2, "Devices WiFi Direct Services Service Address"),
    new IdName (3, "Devices WiFi Direct Services Service Name"),
    new IdName (4, "Devices WiFi Direct Services Service Information"),
    new IdName (5, "Devices WiFi Direct Services Advertisement Id"),
    new IdName (6, "Devices WiFi Direct Services Service Config Methods"),
    new IdName (7, "Devices WiFi Direct Services Request Service Information"),
} },


 {"328d8b21-7729-4bfc-954c-902b329d56b0",new HashSet<IdName>()
             { 
new IdName (2, "Sync Copy In"),
} },

 {"32bcb03c-7f34-4e3f-bbb2-ebe63629f5e4",new HashSet<IdName>()
             { 
new IdName (100, "Is Simple Item"),
} },

 {"33dcf22b-28d5-464c-8035-1ee9efd25278",new HashSet<IdName>()
             { 
new IdName (100, "GPS Longitude Ref"),
} },

 {"341796f1-1df9-4b1c-a564-91bdefa43877",new HashSet<IdName>()
             { 
new IdName (100, "Photo PhotometricInterpretation"),
} },

 {"346c8bd1-2e6a-4c45-89a4-61b78e8e700f",new HashSet<IdName>()
             { 
new IdName (100, "Is Incomplete"),
} },

 {"35dbe6fe-44c3-4400-aaae-d2c799c407e8",new HashSet<IdName>()
             { 
new IdName (100, "GPS Track Ref"),
} },

 {"3602c812-0f3b-45f0-85ad-603468d69423",new HashSet<IdName>()
             { 
new IdName (100, "GPS Date"),
} },

 {"3633de59-6825-4381-a49b-9f6ba13a1471",new HashSet<IdName>()
             {
    new IdName (2, "Devices Playback State"),
    new IdName (3, "Devices Playback Title"),
    new IdName (4, "Devices Remaining Duration"),
    new IdName (5, "Devices Playback Position Percent"),
} },


 {"364028da-d895-41fe-a584-302b1bb70a76",new HashSet<IdName>()
             { 
new IdName (100, "Contact Display Business Phone Numbers"),
} },

 {"364b6fa9-37ab-482a-be2b-ae02f60d4318",new HashSet<IdName>()
             { 
new IdName (100, "Image Compressed Bits Per  Pixel"),
} },

 {"37ebd11f-7e72-4ebc-9d4c-c790f8c277c2",new HashSet<IdName>()
             { 
new IdName (2, "Device Interface Spb Controller Friendly Name"),
} },

 {"38965063-edc8-4268-8491-b7723172cf29",new HashSet<IdName>()
             { 
new IdName (100, "Contact Email Address 2"),
} },

 {"38d43380-d418-4830-84d5-46935a81c5c6",new HashSet<IdName>()
             { 
new IdName (32, "Security Allowed Enterprise Data Protection Identities"),
} },

 {"39a7f922-477c-48de-8bc8-b28441e342e3",new HashSet<IdName>()
             { 
new IdName (100, "Project"),
} },

 {"39b77f4f-a104-4863-b395-2db2ad8f7bc1",new HashSet<IdName>()
             { 
new IdName (100, "Contact Connected Service Display Name"),
} },

 {"3a372292-7fca-49a7-99d5-e47bb2d4e7ab",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Latitude Denominator"),
} },

 {"3b2ce006-5e61-4fde-bab8-9b8aac9b26df",new HashSet<IdName>()
             {
    new IdName (5, "Devices Aep Protocol Id"),
    new IdName (8, "Devices Aep Id"),
} },


 

 {"3c8cee58-d4f0-4cf9-b756-4e5d24447bcd",new HashSet<IdName>()
             {
    new IdName (100, "Contact Gender"),
    new IdName (101, "Contact Gender Value"),
} },


 {"3d658d4d-bc38-464a-b555-418d554a8df8",new HashSet<IdName>()
             { 
new IdName (100, "Fonts Description"),
} },

 {"3d75e4f5-a391-4952-81f7-c7072fe53025",new HashSet<IdName>()
             { 
new IdName (100, "File Reparse Point Tag"),
} },

 {"3f08e66f-2f44-4bb9-a682-ac35d2562322",new HashSet<IdName>()
             { 
new IdName (100, "Image Compression Text"),
} },

 {"3f5d9b45-5e9f-4d5c-8a5e-403181bf177b",new HashSet<IdName>()
             {
    new IdName (2, "Extensions Type"),
    new IdName (3, "Extensions Date Last Used"),
    new IdName (4, "Extensions Used Count"),
    new IdName (5, "Extensions Blocked Count"),
    new IdName (6, "Extensions CLSID"),
    new IdName (7, "Extensions Status"),
    new IdName (8, "Check State"),
    new IdName (9, "Extensions Suspect"),
    new IdName (10, "Extensions File Name"),
    new IdName (11, "Extensions File Path"),
    new IdName (12, "Extensions Flags"),
} },


 {"3f8472b5-e0af-4db2-8071-c53fe76ae7ce",new HashSet<IdName>()
             { 
new IdName (100, "Due Date"),
} },

 {"402b5934-ec5a-48c3-93e6-85e86a2d934e",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Address City"),
} },

 {"41cf5ae0-f75a-4806-bd87-59c7d9248eb9",new HashSet<IdName>()
             { 
new IdName (100, "File Name"),
} },

 {"425d69e5-48ad-4900-8d80-6eb6b8d0ac86",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Longitude Denominator"),
} },

 {"428040ac-a177-4c8a-9760-f6f761227f9a",new HashSet<IdName>()
             { 
new IdName (100, "Communication Date Item Expires"),
} },

 {"42864dfd-9da4-4f77-bded-4aad7b256735",new HashSet<IdName>()
             { 
new IdName (100, "Photo Gain Control Denominator"),
} },

 {"4340a6c5-93fa-4706-972c-7b648008a5a7",new HashSet<IdName>()
             {
    new IdName (8, "Devices Parent"),
    new IdName (9, "Devices Children"),
} },


 {"436f2667-14e2-4feb-b30a-146c53b5b674",new HashSet<IdName>()
             { 
new IdName (100, "Link Arguments"),
} },

 {"43f8d7b7-a444-4f87-9383-52271c9b915c",new HashSet<IdName>()
             { 
new IdName (100, "DateArchived"),
} },


 

 {"446f787f-10c4-41cb-a6c4-4d0343551597",new HashSet<IdName>()
 { 
     new IdName (100, "Contact Business Address State"),
 } },

 {"4530d076-b598-4a81-8813-9b11286ef6ea",new HashSet<IdName>()
 {
     new IdName (2, "Fonts Font Embeddability"),
     new IdName (5, "Fonts Type"),
     new IdName (7, "Fonts File Names"),
 } },


 {"4596208c-32fa-41d2-9695-af0cb9e8dcfe",new HashSet<IdName>()
 { 
     new IdName (100, "Stack Thumbnail Cache Ids"),
  } },

 {"45eae747-8e2a-40ae-8cbf-ca52aba6152a",new HashSet<IdName>()
 { 
     new IdName (100, "Flag Color Text"),
 } },


 {"4679c1b5-844d-4590-baf5-f322231f1b81",new HashSet<IdName>()
 { 
     new IdName (100, "GPS Longitude Decimal"),
 } },

 {"467ee575-1f25-4557-ad4e-b8b58b0d9c15",new HashSet<IdName>()
 { 
     new IdName (100, "GPS Satellites"),
 } },

 {"4684fe97-8765-4842-9c13-f006447b178c",new HashSet<IdName>()
 { 
     new IdName (100, "Recorded TV Original Broadcast Date"),
 } },

 {"46ac629d-75ea-4515-867f-6dc4321c5844",new HashSet<IdName>()
 { 
     new IdName (100, "GPS Altitude Ref"),
 } },

 {"46b4e8de-cdb2-440d-885c-1658eb65b914",new HashSet<IdName>()
 { 
     new IdName (100, "Note Color Text"),
 } },

 {"47166b16-364f-4aa0-9f31-e2ab3df449c3",new HashSet<IdName>()
 { 
     new IdName (100, "GPS DOP Numerator"),
 } },

 {"4776cafa-bce4-4cb1-a23e-265e76d8eb11",new HashSet<IdName>()
 { 
     new IdName (100, "Note Color"),
 } },

 {"47a96261-cb4c-4807-8ad3-40b9d9dbc6bc",new HashSet<IdName>()
 { 
     new IdName (100, "GPS DestLongitude"),
 } },

 {"48fd6ec8-8a12-4cdf-a03e-4ec5a511edde",new HashSet<IdName>()
 { 
     new IdName (100, "Start Date"),
 } },

 

 {"49237325-a95a-4f67-b211-816b2d45d2e0",new HashSet<IdName>()
 { 
     new IdName (100, "Photo Saturation"),
 } },

 {"49691c90-7e17-101a-a91c-08002b2ecda9",new HashSet<IdName>()
 {
     new IdName (2, "Search Results Rank"),
     new IdName (3, "Search Rank"),
     new IdName (4, "Search Hit Count"),
     new IdName (5, "Search Entry Id"),
     new IdName (8, "Search Reverse File Name"),
     new IdName (9, "Item Url"),
     new IdName (10, "Content Url"),
     new IdName (15, "Search Row Id"),
     new IdName (21, "Search Query Property Hits"),
     new IdName (22, "Search Completion"),
     new IdName (28, "Search Result Set Aggregate Attributes"),
 } },


 {"49753869-849c-4323-a41f-26d73f28b53b",new HashSet<IdName>()
 { 
     new IdName (100, "Fonts Vendors"),
 } },

 {"49cd1f76-5626-4b17-a4e8-18b4aa1a2213",new HashSet<IdName>()
 {
     new IdName (2, "Devices Signal Strength"),
     new IdName (3, "Devices Text Messages"),
     new IdName (4, "Devices New Pictures"),
     new IdName (5, "Devices Missed Calls"),
     new IdName (6, "Devices Voicemail"),
     new IdName (7, "Devices Network Name"),
     new IdName (8, "Devices Network Type"),
     new IdName (9, "Devices Roaming"),
     new IdName (10, "Devices Battery Life"),
     new IdName (11, "Devices Charging State"),
     new IdName (12, "Devices Storage Capacity"),
     new IdName (13, "Devices Storage Free Space"),
     new IdName (14, "Devices Storage Free Space Percent"),
     new IdName (22, "Devices Battery Plus Charging"),
     new IdName (23, "Devices Battery Plus Charging Text"),
 } },

 
 {"49d1091f-082e-493f-b23f-d2308aa9668c",new HashSet<IdName>()
 { 
     new IdName (100, "PropList Non Personal"),
 } },

 {"49eb6558-c09c-46dc-8668-1f848c290d0b",new HashSet<IdName>()
 {
     new IdName (1, "Shell Exclusion"),
     new IdName (3, "Shell Item Offline Status"),
 } },


 {"4ac903f8-e780-4e4b-b7b8-4d00a99804fc",new HashSet<IdName>()
 { 
     new IdName (100, "Home Group Sharing Status"),
 } },

 {"4b486401-5468-4381-9b5a-42df4cb49f53",new HashSet<IdName>()
 { 
     new IdName (100, "Fonts Category"),
 } },

 {"4bd13b3d-e68b-44ec-89ee-7611789d4070",new HashSet<IdName>()
 {
     new IdName (100, "Start Menu Group"),
     new IdName (101, "Start Menu Run Command"),
     new IdName (102, "Start Menu Query"),
     new IdName (103, "Start Menu Group Item"),
     new IdName (104, "Start Menu Include In Scope"),
     new IdName (105, "Start Menu Result Source Id"),
 } },


 {"4c6bf15c-4c03-4aac-91f5-64c0f852bcf4",new HashSet<IdName>()
 {
     new IdName (2, "Device Interface Serial Usb Vendor Id"),
     new IdName (3, "Device Interface Serial Usb Product Id"),
     new IdName (4, "Device Interface Serial Port Name"),
 } },


 {"4d1ebee8-0803-4774-9842-b77db50265e9",new HashSet<IdName>()
 {
     new IdName (2, "Storage Portable"),
     new IdName (3, "Storage Removable Media"),
     new IdName (4, "Storage System Critical"),
 } },


 {"4e9cfc01-5d36-406a-83cd-4e7423923604",new HashSet<IdName>()
 { 
     new IdName (2, "Offline Sync Time"),
 } },

 {"4f289a46-2bbb-4ae8-9eda-e5e034707a71",new HashSet<IdName>()
 {
     new IdName (2, "Lzh Folder Compressed Size"),
     new IdName (3, "Lzh Folder CRC16"),
     new IdName (4, "Lzh Folder Method"),
     new IdName (5, "Lzh Folder Ratio"),
 } },

 

 {"4fffe4d0-914f-4ac4-8d6f-c9c61de169b1",new HashSet<IdName>()
             { 
new IdName (100, "Photo Focal Plane Y Resolution"),
} },

 {"502cfeab-47eb-459c-b960-e6d8728f7701",new HashSet<IdName>()
             {
    new IdName (100, "Zone Identifier"),
    new IdName (101, "Last Writer Package Family Name"),
    new IdName (102, "App Zone Identifier"),
} },


 {"5068bcdf-d697-4d85-8c53-1f1cdab01763",new HashSet<IdName>()
             { 
new IdName (100, "Contact Display Home Phone Numbers"),
} },

 {"508161fa-313b-43d5-83a1-c1accf68622c",new HashSet<IdName>()
             { 
new IdName (100, "Contact Other Address"),
} },

 {"51236583-0c4a-4fe8-b81f-166aec13f510",new HashSet<IdName>()
             {
    new IdName (100, "Devices App Package Family Name"),
    new IdName (123, "Devices Glyph Icon"),
} },


 {"51ec3f47-dd50-421d-8769-334f50424b1e",new HashSet<IdName>()
             { 
new IdName (100, "Photo Sharpness Text"),
} },

 {"53da57cf-62c0-45c4-81de-7610bcefd7f5",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Show Time As Text"),
} },

 {"540b947e-8b40-45bc-a8a2-6a0b894cbda2",new HashSet<IdName>()
             {
    new IdName (5, "Devices Present"),
    new IdName (6, "Devices Device Has Problem"),
    new IdName (9, "Devices Physical Device Location"),
} },


 {"54b3a473-59aa-445b-aecd-77541ba8b7c9",new HashSet<IdName>()
             {
    new IdName (2, "User Name"),
    new IdName (3, "User Display Name"),
    new IdName (5, "User Profile Path"),
} },


 {"5567bf77-2be2-4222-befa-d0c9c9cc4b6e",new HashSet<IdName>()
             { 
new IdName (2, "Velocity Feature Id"),
} },

 {"55e98597-ad16-42e0-b624-21599a199838",new HashSet<IdName>()
             { 
new IdName (100, "Photo Exposure Time Denominator"),
} },

 {"560c36c0-503a-11cf-baa1-00004c752a9a",new HashSet<IdName>()
             {
    new IdName (2, "Search Auto Summary"),
    new IdName (3, "Search Query Focused Summary"),
    new IdName (4, "Search Query Focused Summary With Fallback"),
} },


 {"56310920-2491-4919-99ce-eadb06fafdb2",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Home Page"),
} },


 
 {"56a3372e-ce9c-11d2-9f0e-006097c686f6",new HashSet<IdName>()
             {
    new IdName (2, "Music Artist"),
    new IdName (4, "Music Album Title"),
    new IdName (5, "Media Year"),
    new IdName (7, "Music Track Number"),
    new IdName (11, "Music Genre"),
    new IdName (12, "Music Lyrics"),
    new IdName (13, "Music Album Artist"),
    new IdName (33, "Music Content Group Description"),
    new IdName (34, "Music Initial Key"),
    new IdName (35, "Music Beats Per Minute"),
    new IdName (36, "Music Conductor"),
    new IdName (37, "Music Part Of Set"),
    new IdName (38, "Media Sub Title"),
    new IdName (39, "Music Mood"),
    new IdName (100, "Music Album Id"),
} },


 {"56c90e9d-9d46-4963-886f-2e1cd9a694ef",new HashSet<IdName>()
             { 
new IdName (100, "Contact Home Email Addresses"),
} },

 {"57086c23-86c6-478f-afb2-236188c8f47f",new HashSet<IdName>()
             {
    new IdName (2, "Taskbar Tab Active"),
    new IdName (3, "Taskbar Tab List"),
} },


 {"5741cf9c-56fe-485b-8901-4786449e188d",new HashSet<IdName>()
             { 
new IdName (100, "Fonts Designed For"),
} },

 {"59569556-0a08-4212-95b9-fae2ad6413db",new HashSet<IdName>()
             { 
new IdName (2, "Devices Notifications New Voicemail"),
} },

 {"596fd41b-af9b-4ba8-9b49-33b16f16678c",new HashSet<IdName>()
             { 
new IdName (100, "Fonts Styles"),
} },

 {"59d49e61-840f-4aa9-a939-e2099b7f6399",new HashSet<IdName>()
             { 
new IdName (100, "GPS Processing Method"),
} },

 {"59dde9f2-5253-40ea-9a8b-479e96c6249a",new HashSet<IdName>()
             { 
new IdName (100, "Photo Contrast Text"),
} },

 {"5ab5c75f-15e1-4d65-924a-04754567243c",new HashSet<IdName>()
             {
    new IdName (2, "Setting Host Id"),
    new IdName (3, "Setting Setting Id"),
    new IdName (4, "Setting Page Id"),
    new IdName (5, "Setting Group Id"),
    new IdName (6, "Setting Condition"),
    new IdName (7, "Setting Glyph"),
    new IdName (8, "Setting Glyph Rtl"),
} },


 {"5bf396d4-5eb2-466f-bde9-2fb3f2361d6e",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Show Time As"),
} },

 {"5cbf2787-48cf-4208-b90e-ee5e5d420294",new HashSet<IdName>()
             {
    new IdName (1, "History Url Hash"),
    new IdName (2, "Link Target Url"),
    new IdName (3, "Url Scheme"),
    new IdName (4, "Url HostName"),
    new IdName (5, "History Url Extra Info"),
    new IdName (6, "History Code Page"),
    new IdName (7, "History Visit Count"),
    new IdName (8, "History Is History"),
    new IdName (9, "History I sDownload"),
    new IdName (10, "History Download Location"),
    new IdName (11, "History Download Size"),
    new IdName (12, "History Favorite IconKey"),
    new IdName (13, "History Is Favorite"),
    new IdName (14, "History Is Offline Favorite"),
    new IdName (15, "History Is Pinned Favorite"),
    new IdName (16, "History Is Typed Url"),
    new IdName (17, "History Is Top Level"),
    new IdName (18, "History Is Feed"),
    new IdName (19, "History Keywords"),
    new IdName (20, "History User Keywords"),
    new IdName (21, "Link Description"),
    new IdName (22, "History User Description"),
    new IdName (23, "Link Date Visited"),
    new IdName (24, "History Icon Bits"),
    new IdName (25, "Icon Path"),
    new IdName (26, "Icon Index"),
    new IdName (27, "History Icon Date"),
    new IdName (28, "History Points"),
    new IdName (29, "History Sessions"),
    new IdName (33, "History Subscription Cookie"),
    new IdName (34, "History Tracking"),
    new IdName (35, "Link Working Folder Path"),
    new IdName (36, "Link Hot Key"),
    new IdName (37, "Link Show Cmd"),
    new IdName (38, "Link Whats New"),
    new IdName (39, "History Date Changed"),
    new IdName (40, "History Flags"),
    new IdName (41, "History Watch"),
    new IdName (42, "History Favorite Icon Hash"),
    new IdName (43, "Icon Secondary Stream Name"),
} },


 {"5cda5fc8-33ee-4ff3-9094-ae7bd8868c4d",new HashSet<IdName>()
             { 
new IdName (100, "Is Deleted"),
} },

 {"5cde9f0e-1de4-4453-96a9-56e8832efa3d",new HashSet<IdName>()
             {
    new IdName (1, "Computer Domain Name"),
    new IdName (2, "Computer Workgroup"),
} },


 {"5d76b67f-9b3d-44bb-b6ae-25da4f638a67",new HashSet<IdName>()
             {
    new IdName (2, "Is Pinned To Name Space Tree"),
    new IdName (3, "Is Default Save Location"),
    new IdName (4, "Is Search Only Item"),
    new IdName (5, "Is Default Non Owner Save Location"),
    new IdName (6, "Owner SID"),
    new IdName (7, "Is Default Save Location For Display"),
    new IdName (8, "Is Location Supported"),
    new IdName (9, "Library Location Support Status"),
    new IdName (10, "Default Save Location Display"),
    new IdName (11, "Default Save Location Icon Container"),
} },


 {"5da84765-e3ff-4278-86b0-a27967fbdd03",new HashSet<IdName>()
             { 
new IdName (100, "Is Flagged"),
} },

 {"5dc2253f-5e11-4adf-9cfe-910dd01e3e70",new HashSet<IdName>()
             { 
new IdName (100, "Contact Hobbies"),
} },

 {"5f5aff6a-37e5-4780-97ea-80c7565cf535",new HashSet<IdName>()
             { 
new IdName (34, "Security Encryption Owners"),
} },

 {"5fbd34cd-561a-412e-ba98-478a6b0fef1d",new HashSet<IdName>()
             {
    new IdName (2, "Devices Aep Bluetooth Cod Major"),
    new IdName (3, "Devices Aep Bluetooth Cod Minor"),
    new IdName (4, "Devices Aep Bluetooth Cod Services Limited Discovery"),
    new IdName (5, "Devices Aep Bluetooth Cod Services Positioning"),
    new IdName (6, "Devices Aep Bluetooth Cod Services Networking"),
    new IdName (7, "Devices Aep Bluetooth Cod Services Rendering"),
    new IdName (8, "Devices Aep Bluetooth Cod Services Capturing"),
    new IdName (9, "Devices Aep Bluetooth Cod Services Object Xfer"),
    new IdName (10, "Devices Aep Bluetooth Cod Services Audio"),
    new IdName (11, "Devices Aep Bluetooth Cod Services Telephony"),
    new IdName (12, "Devices Aep Bluetooth Cod Services Information"),
} },


 {"61478c08-b600-4a84-bbe4-e99c45f0a072",new HashSet<IdName>()
             { 
new IdName (100, "Photo Saturation Text"),
} },

 {"61872cf7-6b5e-4b4b-ac2d-59da84459248",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Media"),
} },

 {"62d2d9ab-8b64-498d-b865-402d4796f865",new HashSet<IdName>()
             { 
new IdName (3, "Location Empty String"),
} },

 {"6336b95e-c7a7-426d-86fd-7ae3d39c84b4",new HashSet<IdName>()
             { 
new IdName (100, "Photo White Balance Text"),
} },

 {"635e9051-50a5-4ba2-b9db-4ed056c77296",new HashSet<IdName>()
             { 
new IdName (100, "Contact Full Name"),
} },

 {"63c25b20-96be-488f-8788-c09c407ad812",new HashSet<IdName>()
             { 
new IdName (100, "Contact Primary Address Street"),
} },

 {"641064ba-9329-47e6-8f36-5fa81aa461a0",new HashSet<IdName>()
             {
    new IdName (2, "OneNote Page Edit History"),
    new IdName (3, "OneNote Tagged Notes"),
    new IdName (4, "OneNote Linked Note Uri"),
} },


 

 {"6444048f-4c8b-11d1-8b70-080036b11a03",new HashSet<IdName>()
             {
    new IdName (3, "Image Horizontal Size"),
    new IdName (4, "Image Vertical Size"),
    new IdName (5, "Image Horizontal Resolution"),
    new IdName (6, "Image Vertical Resolution"),
    new IdName (7, "Image Bit Depth"),
    new IdName (12, "Media Frame Count"),
    new IdName (13, "Image Dimensions"),
} },


 {"64440490-4c8b-11d1-8b70-080036b11a03",new HashSet<IdName>()
             {
    new IdName (2, "Audio Format"),
    new IdName (3, "Media Duration"),
    new IdName (4, "Audio Encoding Bitrate"),
    new IdName (5, "Audio Sample Rate"),
    new IdName (6, "Audio Sample Size"),
    new IdName (7, "Audio Channel Count"),
    new IdName (8, "Audio Stream Number"),
    new IdName (9, "Audio Stream Name"),
    new IdName (10, "Audio Compression"),
} },


 {"64440491-4c8b-11d1-8b70-080036b11a03",new HashSet<IdName>()
             {
    new IdName (2, "Video Stream Name"),
    new IdName (3, "Video Frame Width"),
    new IdName (4, "Video Frame Height"),
    new IdName (6, "Video Frame Rate"),
    new IdName (8, "Video Encoding Bitrate"),
    new IdName (9, "Video Sample Size"),
    new IdName (10, "Video Compression"),
    new IdName (11, "Video Stream Number"),
    new IdName (42, "Video Horizontal Aspect Ratio"),
    new IdName (43, "Video Total Bitrate"),
    new IdName (44, "Video Four CC"),
    new IdName (45, "Video Vertical Aspect Ratio"),
    new IdName (46, "Video Transcoded For Sync"),
    new IdName (98, "Video Is Stereo"),
    new IdName (99, "Video Orientation"),
    new IdName (100, "Video Is Spherical"),
} },


 {"64440492-4c8b-11d1-8b70-080036b11a03",new HashSet<IdName>()
             {
    new IdName (7, "Media Status"),
    new IdName (9, "Rating"),
    new IdName (11, "Copyright"),
    new IdName (12, "Share User Rating"),
    new IdName (13, "Media Class Primary Id"),
    new IdName (14, "Media Class Secondary Id"),
    new IdName (15, "Media DVDID"),
    new IdName (16, "Media MCDI"),
    new IdName (17, "Media Metadata Content Provider"),
    new IdName (18, "Media Content Distributor"),
    new IdName (19, "Music Composer"),
    new IdName (20, "Video Director"),
    new IdName (21, "Parental Rating"),
    new IdName (22, "Media Producer"),
    new IdName (23, "Media Writer"),
    new IdName (24, "Media Collection Group Id"),
    new IdName (25, "Media Collection Id"),
    new IdName (26, "Media Content Id"),
    new IdName (27, "Media Creator Application"),
    new IdName (28, "Media Creator Application Version"),
    new IdName (30, "Media Publisher"),
    new IdName (31, "Music Period"),
    new IdName (32, "Media Author Url"),
    new IdName (33, "Media Promotion Url"),
    new IdName (34, "Media User Web Url"),
    new IdName (35, "Media Unique File Identifier"),
    new IdName (36, "Media Encoded By"),
    new IdName (37, "Media Encoding Settings"),
    new IdName (38, "Media Protection Type"),
    new IdName (39, "Media Provider Rating"),
    new IdName (40, "Media Provider Style"),
    new IdName (41, "Media User No Auto Info"),
    new IdName (42, "Media Series Name"),
    new IdName (47, "Media Thumbnail Large Path"),
    new IdName (48, "Media Thumbnail Large Uri"),
    new IdName (49, "Media ThumbnailSmallPath"),
    new IdName (50, "Media Thumbnail Small Uri"),
    new IdName (100, "Media Episode Number"),
    new IdName (101, "Media Season Number"),
} },


 {"644d37b4-e1b3-4bad-b099-7e7c04966aca",new HashSet<IdName>()
             { 
new IdName (100, "Contact Email Address3"),
} },

 {"656a3bb3-ecc0-43fd-8477-4ae0404a96cd",new HashSet<IdName>()
             {
    new IdName (8192, "Devices Manufacturer"),
    new IdName (8194, "Devices Model Name"),
    new IdName (8195, "Devices Model Number"),
    new IdName (8198, "Devices Presentation Url"),
    new IdName (12288, "Devices Friendly Name"),
    new IdName (12297, "Devices Ip Address"),
    new IdName (16384, "Devices Service Address"),
    new IdName (16385, "Devices Service Id"),
} },


 {"65a98875-3c80-40ab-abbc-efdaf77dbee2",new HashSet<IdName>()
             { 
new IdName (100, "Acquisition Id"),
} },

 {"660e04d6-81ab-4977-a09f-82313113ab26",new HashSet<IdName>()
             { 
new IdName (100, "Contact Home Fax Number"),
} },

 {"6614ef48-4efe-4424-9eda-c79f404edf3e",new HashSet<IdName>()
             { 
new IdName (2, "Devices Notifications Missed Call"),
} },

 {"668cdfa5-7a1b-4323-ae4b-e527393a1d81",new HashSet<IdName>()
             { 
new IdName (100, "Source Item"),
} },

 {"67df94de-0ca7-4d6f-b792-053a3e4f03cf",new HashSet<IdName>()
             { 
new IdName (100, "Flag Color"),
} },

 {"6845cc72-1b71-48c3-af86-b09171a19b14",new HashSet<IdName>()
             { 
new IdName (3, "Devices Dial Protocol Installed Applications"),
} },

 {"68dd6094-7216-40f1-a029-43fe7127043f",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Music"),
} },

 {"6a15e5a0-0a1e-4cd7-bb8c-d2f1b0c929bc",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Telephone"),
} },

 {"6af55d45-38db-4495-acb0-d4728a3b8314",new HashSet<IdName>()
             {
    new IdName (2, "Devices AepContainer Supports Audio"),
    new IdName (3, "Devices AepContainer Supports Video"),
    new IdName (4, "Devices AepContainer Supports Images"),
    new IdName (5, "Devices AepContainer Supported Uri Schemes"),
    new IdName (6, "Devices AepContainer Dial Protocol Installed Applications"),
    new IdName (7, "Devices AepContainer Supports Limited Discovery"),
    new IdName (8, "Devices AepContainer Supports Positioning"),
    new IdName (9, "Devices AepContainer Supports Networking"),
    new IdName (10, "Devices AepContainer Supports Rendering"),
    new IdName (11, "Devices AepContainer Supports Capturing"),
    new IdName (12, "Devices AepContainer Supports Object Transfer"),
    new IdName (13, "Devices AepContainer Supports Telephony"),
    new IdName (14, "Devices AepContainer Supports Information"),
} },


 {"6afe7437-9bcd-49c7-80fe-4a5c65fa5874",new HashSet<IdName>()
             { 
new IdName (104, "Music Disc Number"),
} },

 {"6b223b6a-162e-4aa9-b39f-05d678fc6d77",new HashSet<IdName>()
             { 
new IdName (100, "Music Synchronized Lyrics"),
} },

 {"6b8b68f6-200b-47ea-8d25-d8050f57339f",new HashSet<IdName>()
             { 
new IdName (100, "Photo Flash Text"),
} },

 {"6b8da074-3b5c-43bc-886f-0a2cdce00b6f",new HashSet<IdName>()
             { 
new IdName (100, "Item Name"),
} },

 {"6bdd1fc6-810f-11d0-bec7-08002be2092f",new HashSet<IdName>()
             { 
new IdName (2, "Devices Wia Device Type"),
} },

 {"6ccd0131-c397-4744-b2d8-d2c13f457026",new HashSet<IdName>()
             { 
new IdName (80, "Game Type"),
} },

 {"6d217f6d-3f6a-4825-b470-5f03ca2fbe9b",new HashSet<IdName>()
             { 
new IdName (100, "Photo Program Mode"),
} },

 {"6d24888f-4718-4bda-afed-ea0fb4386cd8",new HashSet<IdName>()
             { 
new IdName (100, "Offline Status"),
} },

 {"6d6d5d49-265d-4688-9f4e-1fdd33e7cc83",new HashSet<IdName>()
             { 
new IdName (100, "Identity Internet Sid"),
} },

 {"6d748de2-8d38-4cc3-ac60-f009b057c557",new HashSet<IdName>()
             {
    new IdName (2, "RecordedTV Episode Name"),
    new IdName (3, "RecordedTV Program Description"),
    new IdName (4, "RecordedTV Credits"),
    new IdName (5, "RecordedTV Station Call Sign"),
    new IdName (7, "RecordedTV Channe' Number"),
    new IdName (10, "RecordedTV Video Quality"),
    new IdName (12, "RecordedTV Is Closed Captioning Available"),
    new IdName (13, "RecordedTV Is Repeat Broadcast"),
    new IdName (14, "RecordedTV Is SAP"),
    new IdName (15, "RecordedTV Date Content Expires"),
    new IdName (16, "RecordedTV Is ATSC Content"),
    new IdName (17, "RecordedTV Is DTV Content"),
    new IdName (18, "RecordedTV Is HD Content"),
} },


 {"6e682923-7f7b-4f0c-a337-cfca296687bf",new HashSet<IdName>()
             { 
new IdName (100, "Contact Other Address City"),
} },

 {"6ebe6946-2321-440a-90f0-c043efd32476",new HashSet<IdName>()
             { 
new IdName (100, "Photo Brightness Denominator"),
} },

 {"6fa20de6-d11c-4d9d-a154-64317628c12d",new HashSet<IdName>()
             { 
new IdName (100, "Expand oProperties"),
} },

 {"702926f4-44a6-43e1-ae71-45627116893b",new HashSet<IdName>()
             { 
new IdName (100, "GPS Track Numerator"),
} },

 {"7036dcfc-69ab-4316-b5ac-50de702447b0",new HashSet<IdName>()
             {
    new IdName (102, "Structured Query Before"),
    new IdName (103, "Structured Query After"),
    new IdName (104, "Structured Query File"),
    new IdName (105, "Structured Query Custom Property Boolean"),
    new IdName (106, "Structured Query Custom Property Integer"),
    new IdName (107, "Structured Query Custom Property Floating Point"),
    new IdName (108, "Structured Query Custom Property String"),
    new IdName (109, "Structured Query Custom Property DateTime"),
    new IdName (110, "Structured Query Has"),
    new IdName (111, "Structured Query Is"),
    new IdName (112, "Structured Query Null"),
} },


 {"705ccb0f-5a0d-41ea-b2ca-2c9b5cc7db41",new HashSet<IdName>()
             { 
new IdName (100, "Verb Restrictions"),
} },

 {"705d8364-7547-468c-8c88-84860bcbed4c",new HashSet<IdName>()
             {
    new IdName (2, "SAM Name"),
    new IdName (3, "SAM Version"),
    new IdName (4, "SAM Date Changed"),
    new IdName (5, "SAM Password Last Set"),
    new IdName (6, "SAM Date Account Expires"),
    new IdName (7, "SAM Password Can Change"),
    new IdName (8, "SAM Password Must Change"),
    new IdName (9, "SAM Full Name"),
    new IdName (10, "SAM Home Directory"),
    new IdName (11, "SAM Home Directory Drive"),
    new IdName (12, "SAM Script Path"),
    new IdName (13, "SAM Profile Path"),
    new IdName (14, "SAM Admin Comment"),
    new IdName (15, "SAM Workstations"),
    new IdName (16, "SAM User Comment"),
    new IdName (17, "SAM Password"),
    new IdName (18, "SAM Security Id"),
    new IdName (19, "SAM User Account Control"),
    new IdName (20, "SAM Logon Hours"),
    new IdName (21, "SAM Country Code"),
    new IdName (22, "SAM Code Page"),
    new IdName (23, "SAM Password Expired"),
    new IdName (24, "SAM User Picture"),
    new IdName (25, "SAM Password Hint"),
    new IdName (26, "SAM Domain"),
    new IdName (31, "SAM Groups"),
    new IdName (32, "SAM Type"),
    new IdName (36, "SAM Interactive Login"),
    new IdName (37, "SAM Network Login"),
    new IdName (38, "SAM Batch Login"),
    new IdName (39, "SAM Service Login"),
    new IdName (40, "SAM Remote Interactive Login"),
    new IdName (41, "SAM Deny Interactive Login"),
    new IdName (42, "SAM Deny Network Login"),
    new IdName (43, "SAM Deny Batch Login"),
    new IdName (44, "SAM Deny Service Login"),
    new IdName (45, "SAM Deny Remote Interactive Login"),
    new IdName (46, "SAM Dont Show In Logon UI"),
    new IdName (47, "SAM Shell Admin Object Props"),
    new IdName (50, "SAM Password Is Empty"),
    new IdName (102, "SAM Group Members"),
    new IdName (103, "SAM Residual Id"),
    new IdName (200, "LOGON LU Id"),
    new IdName (201, "LOGON Authentication Package"),
    new IdName (202, "LOGON TS Session"),
    new IdName (203, "LOGON Logon Time"),
    new IdName (204, "LOGON Logon Server"),
    new IdName (205, "LOGON Dns Domain Name"),
    new IdName (206, "LOGON UPN"),
    new IdName (207, "LOGON Client Name"),
    new IdName (208, "LOGON WinS tation Name"),
    new IdName (209, "LOGON Status"),
    new IdName (500, "PROFILE Path"),
    new IdName (501, "PROFILE GUID"),
} },


 {"71724756-3e74-4432-9b59-e7b2f668a593",new HashSet<IdName>()
             {
    new IdName (2, "Devices AepService Friendly Name"),
    new IdName (3, "Devices AepService Service Class Id"),
    new IdName (4, "Devices AepService Container Id"),
} },


 {"71b377d6-e570-425f-a170-809fae73e54e",new HashSet<IdName>()
             { 
new IdName (100, "Contact Other Address State"),
} },

 {"720eb626-dbe4-4113-835c-9315e1e2ff77",new HashSet<IdName>()
             {
    new IdName (2, "Actions Action Name"),
    new IdName (3, "Actions Activation Context"),
} },


 {"7268af55-1ce4-4f6e-a41f-b6e4ef10e4a9",new HashSet<IdName>()
             { 
new IdName (100, "Contact Profession"),
} },

 {"72fab781-acda-43e5-b155-b2434f85e678",new HashSet<IdName>()
             { 
new IdName (100, "Date Completed"),
} },

 {"72fc5ba4-24f9-4011-9f3f-add27afad818",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Reminder Time"),
} },

 {"730fb6dd-cf7c-426b-a03f-bd166cc9ee24",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Address"),
} },

 {"73389854-0b42-4ea6-bc67-847d430899fd",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Require Template"),
} },

 {"733cb147-8b1f-4c48-9966-192fde353c75",new HashSet<IdName>()
             { 
new IdName (100, "Music Stack Thumbnail Cache Ids"),
} },

 {"738bf284-1d87-420b-92cf-5834bf6ef9ed",new HashSet<IdName>()
             { 
new IdName (100, "Photo Exposure Bias Numerator"),
} },

 {"744c8242-4df5-456c-ab9e-014efb9021e3",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Organizer Address"),
} },

 {"745baf0e-e5c1-4cfb-8a1b-d031a0a52393",new HashSet<IdName>()
             { 
new IdName (100, "Photo Digital Zoom Denominator"),
} },

 {"74a7de49-fa11-4d3d-a006-db7e08675916",new HashSet<IdName>()
             { 
new IdName (100, "Identity Provider Id"),
} },

 {"75ee72ae-7d5f-482f-9487-f1c46ca819c1",new HashSet<IdName>()
             { 
new IdName (100, "Camera Roll Deduplication Id"),
} },

 {"76c09943-7c33-49e3-9e7e-cdba872cfada",new HashSet<IdName>()
             { 
new IdName (100, "GPS Track"),
} },

 {"776b6b3b-1e3d-4b0c-9a0e-8fbaf2a8492a",new HashSet<IdName>()
             { 
new IdName (100, "Photo Focal Lengt hNumerator"),
} },

 {"78342dcb-e358-4145-ae9a-6bfe4e0f9f51",new HashSet<IdName>()
             { 
new IdName (100, "GPS Altitude Denominator"),
} },

 {"78c34fc8-104a-4aca-9ea4-524d52996e57",new HashSet<IdName>()
             {
    new IdName (52, "Devices Discovery Method"),
    new IdName (55, "Devices Connected"),
    new IdName (56, "Devices Paired"),
    new IdName (57, "Devices Icon"),
    new IdName (70, "Devices Local Machine"),
    new IdName (71, "Devices Metadata Path"),
    new IdName (77, "Devices Launch Device Stage From Explorer"),
    new IdName (81, "Devices Device Description1"),
    new IdName (82, "Devices Device Description2"),
    new IdName (83, "Devices NotWorking Properly"),
    new IdName (84, "Devices Is Shared"),
    new IdName (85, "Devices Is Network Connected"),
    new IdName (86, "Devices Is Default"),
    new IdName (90, "Devices Category Ids"),
    new IdName (91, "Devices Category"),
    new IdName (92, "Devices Category Plural"),
    new IdName (94, "Devices Category Group"),
    new IdName (256, "Devices Device Instance Id"),
} },

 

 {"79486778-4c6f-4dde-bc53-cd594311af99",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Local Weights"),
} },

 {"79d94e82-4d79-45aa-821a-74858b4e4ca6",new HashSet<IdName>()
             { 
new IdName (2, "Devices AepService IoT Service Interfaces"),
} },

 {"7a55582b-bd8c-4475-b94c-b87a388a7899",new HashSet<IdName>()
             { 
new IdName (100, "Status Icons"),
} },

 {"7a7d76f4-b630-4bd7-95ff-37cc51a975c9",new HashSet<IdName>()
             { 
new IdName (2, "Link Target Extension"),
} },

 {"7abcf4f8-7c3f-4988-ac91-8d2c2e97eca5",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Bearing Denominator"),
} },

 {"7b9f6399-0a3f-4b12-89bd-4adc51c918af",new HashSet<IdName>()
             { 
new IdName (100, "Contact Home Address Post Office Box"),
} },

 {"7ba3535d-69aa-4525-a938-f3ec79485377",new HashSet<IdName>()
             {
    new IdName (2, "SAM Allowed Logon"),
    new IdName (3, "SAM Dont Enumerate For Logon"),
} },


 {"7bd5533e-af15-44db-b8c8-bd6624e1d032",new HashSet<IdName>()
             {
    new IdName (2, "Sync Handler CollectionId"),
    new IdName (3, "Sync Handler Id"),
    new IdName (4, "Sync Event Description"),
    new IdName (5, "Sync Progress"),
    new IdName (6, "Sync Item Id"),
    new IdName (7, "Sync Date Synchronized"),
    new IdName (8, "Sync Handler Type"),
    new IdName (9, "Sync Handler Type Label"),
    new IdName (10, "Sync Status"),
    new IdName (11, "Sync Conflict Count"),
    new IdName (12, "Sync Error Count"),
    new IdName (13, "Sync Comments"),
    new IdName (14, "Sync Enabled"),
    new IdName (15, "Sync Hidden"),
    new IdName (16, "Sync Connected"),
    new IdName (17, "Sync Link"),
    new IdName (19, "Sync Context"),
    new IdName (20, "Sync Event Level"),
    new IdName (21, "Sync Event Flags"),
    new IdName (22, "Sync Sync Results"),
    new IdName (23, "Sync Progress Percentage"),
    new IdName (24, "Sync State"),
    new IdName (25, "Sync Item State"),
    new IdName (26, "Sync Item Status Text"),
    new IdName (27, "Sync Item Status Description"),
    new IdName (28, "Sync Item Status Action"),
    new IdName (29, "Sync Global Activity Message"),
    new IdName (30, "Sync Last Synced Message"),
} },


 {"7d122d5a-ae5e-4335-8841-d71e7ce72f53",new HashSet<IdName>()
             { 
new IdName (100, "GPS Speed Denominator"),
} },

 {"7d683fc9-d155-45a8-bb1f-89d19bcb792f",new HashSet<IdName>()
             { 
new IdName (100, "Identity Display Name"),
} },

 {"7ddaaad1-ccc8-41ae-b750-b2cb8031aea2",new HashSet<IdName>()
             { 
new IdName (100, "GPS Latitude Numerator"),
} },

 {"7fd7259d-16b4-4135-9f97-7c96ecd2fa9e",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Message"),
} },

 {"7fe3aa27-2648-42f3-89b0-454e5cb150c3",new HashSet<IdName>()
             { 
new IdName (100, "Photo Program Mode Text"),
} },

 {"807b653a-9e91-43ef-8f97-11ce04ee20c5",new HashSet<IdName>()
             { 
new IdName (100, "Communication Suffix"),
} },

 {"80d81ea6-7473-4b0c-8216-efc11a2c4c8b",new HashSet<IdName>()
             { 
new IdName (2, "Devices Model Id"),
} },

 {"80f41eb8-afc4-4208-aa5f-cce21a627281",new HashSet<IdName>()
             { 
new IdName (100, "Contact Connected Service Identities"),
} },

 {"813f4124-34e6-4d17-ab3e-6b1f3c2247a1",new HashSet<IdName>()
             { 
new IdName (100, "Photo Maker Note Offset"),
} },

 {"821437d6-9eab-4765-a589-3b1cbbd22a61",new HashSet<IdName>()
             { 
new IdName (100, "Photo Photometric Interpretation Text"),
} },

 {"827edb4f-5b73-44a7-891d-fdffabea35ca",new HashSet<IdName>()
             { 
new IdName (100, "GPS Altitude"),
} },

 {"83914d1a-c270-48bf-b00d-1c4e451b0150",new HashSet<IdName>()
             { 
new IdName (100, "Default Group Order"),
} },

 {"83a6347e-6fe4-4f40-ba9c-c4865240d1f4",new HashSet<IdName>()
             { 
new IdName (100, "Communication Followup Icon Index"),
} },

 {"83da6326-97a6-4088-9453-a1923f573b29",new HashSet<IdName>()
             { 
new IdName (9, "Devices Is Software Installing"),
} },


 {"847c66de-b8d6-4af9-abc3-6f4f926bc039",new HashSet<IdName>()
             { 
new IdName (14, "Device Interface Printer Driver Directory"),
} },

 {"84d8f337-981d-44b3-9615-c7596dba17e3",new HashSet<IdName>()
             { 
new IdName (100, "Contact Email Addresses"),
} },

 {"8589e481-6040-473d-b171-7fa89c2708ed",new HashSet<IdName>()
             { 
new IdName (100, "Contact Company Main Telephone"),
} },

 {"8619a4b6-9f4d-4429-8c0f-b996ca59e335",new HashSet<IdName>()
             { 
new IdName (100, "Communication Security Flags"),
} },

 {"86407db8-9df7-48cd-b986-f999adc19731",new HashSet<IdName>()
             { 
new IdName (2, "Share Target Description"),
} },


 {"8727cfff-4868-4ec6-ad5b-81b98521d1ab",new HashSet<IdName>()
             { 
new IdName (100, "GPS Latitude"),
} },

 {"880f70a2-6082-47ac-8aab-a739d1a300c3",new HashSet<IdName>()
             {
    new IdName (151, "Devices Shared Tooltip"),
    new IdName (152, "Devices Networked Tooltip"),
    new IdName (153, "Devices Default Tooltip"),
} },


 {"8859a284-de7e-4642-99ba-d431d044b1ec",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Media Advanced"),
} },

 {"8943b373-388c-4395-b557-bc6dbaffafdb",new HashSet<IdName>()
             {
    new IdName (2, "Devices Audio Device Raw Processing Supported"),
    new IdName (3, "Devices Audio Device Microphone Sensitivity In Dbfs"),
    new IdName (4, "Devices Audio Device Microphone Signal To Noise Ratio In Db"),
} },


 {"8969b275-9475-4e00-a887-ff93b8b41e44",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Description"),
} },

 {"897b3694-fe9e-43e6-8066-260f590c0100",new HashSet<IdName>()
             {
    new IdName (2, "Contact JA Company Name Phonetic"),
    new IdName (3, "Contact JA First Name Phonetic"),
    new IdName (4, "Contact JA Last Name Phonetic"),
} },


 {"8a2f99f9-3c37-465d-a8d7-69777a246d0c",new HashSet<IdName>()
             {
    new IdName (2, "Link Feed Item Local Id"),
    new IdName (5, "Link Target Url Host Name"),
    new IdName (6, "Link Target Url Path"),
} },


 {"8af4961c-f526-43e5-aa81-db768219178d",new HashSet<IdName>()
             { 
new IdName (100, "Photo SubjectDistanceNumerator"),
} },

 {"8afcc170-8a46-4b53-9eee-90bae7151e62",new HashSet<IdName>()
             { 
new IdName (100, "Contact Home Address Postal Code"),
} },

 {"8b26ea41-058f-43f6-aecc-4035681ce977",new HashSet<IdName>()
             { 
new IdName (100, "Contact Other Address Post Office Box"),
} },

 {"8bf6b9f6-b4f5-482f-a2c2-44bdad2fcfa9",new HashSet<IdName>()
             { 
new IdName (51, "SAM Account Is Disabled For Logon UI"),
} },

 {"8c3b93a4-baed-1a83-9a32-102ee313f6eb",new HashSet<IdName>()
             { 
new IdName (100, "Identity Blob"),
} },

 {"8c7ed206-3f8a-4827-b3ab-ae9e1faefc6c",new HashSet<IdName>()
             {
    new IdName (2, "Devices Container Id"),
    new IdName (4, "Devices In Local Machine Container"),
} },


 {"8d72aca1-0716-419a-9ac1-acb07b18dc32",new HashSet<IdName>()
             { 
new IdName (2, "File Attributes Display"),
} },

 {"8e531030-b960-4346-ae0d-66bc9a86fb94",new HashSet<IdName>()
             { 
new IdName (100, "Communication Direction"),
} },

 {"8e8ecf7c-b7b8-4eb8-a63f-0ee715c96f9e",new HashSet<IdName>()
             { 
new IdName (100, "Photo Gain Control Numerator"),
} },

 {"8f167568-0aae-4322-8ed9-6055b7b0e398",new HashSet<IdName>()
             { 
new IdName (100, "Contact Other Address Country"),
} },

 {"8f367200-c270-457c-b1d4-e07c5bcd90c7",new HashSet<IdName>()
             { 
new IdName (100, "Contact Last Name"),
} },

 {"8fdc6dea-b929-412b-ba90-397a257465fe",new HashSet<IdName>()
             { 
new IdName (100, "Contact Car Telephone"),
} },

 {"900a403b-097b-4b95-8ae2-071fdaeeb118",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Advanced"),
} },

 {"90197ca7-fd8f-4e8c-9da3-b57e1e609295",new HashSet<IdName>()
             { 
new IdName (100, "Rating Text"),
} },

 {"908696c7-8f87-44f2-80ed-a8c1c6894575",new HashSet<IdName>()
             {
    new IdName (2, "Library Locations Count"),
    new IdName (4, "Library Locations List"),
} },


 {"9098f33c-9a7d-48a8-8de5-2e1227a64e91",new HashSet<IdName>()
             { 
new IdName (100, "Message Proof In Progress"),
} },

 {"90e5e14e-648b-4826-b2aa-acaf790e3513",new HashSet<IdName>()
             { 
new IdName (10, "Is Encrypted"),
} },

 {"916d17ac-8a97-48af-85b7-867a88fad542",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Auto Complete"),
} },

 {"91eff6f3-2e27-42ca-933e-7c999fbe310b",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Fax Number"),
} },

 {"93112f89-c28b-492f-8a9d-4be2062cee8a",new HashSet<IdName>()
             { 
new IdName (100, "Photo Exposure Index Denominator"),
} },

 {"95beb1fc-326d-4644-b396-cd3ed90e6ddf",new HashSet<IdName>()
             { 
new IdName (100, "Journal Entry Type"),
} },

 {"95c656c1-2abf-4148-9ed3-9ec602e3b7cd",new HashSet<IdName>()
             { 
new IdName (100, "Contact Other Address Postal Code"),
} },

 {"95e127b5-79cc-4e83-9c9e-8422187b3e0e",new HashSet<IdName>()
             {
    new IdName (2, "Device Interface Win Usb Usb Vendor Id"),
    new IdName (3, "Device Interface Win Usb Usb Product Id"),
    new IdName (4, "Device Interface Win Usb Usb Class"),
    new IdName (5, "Device Interface Win Usb Usb Sub Class"),
    new IdName (6, "Device Interface Win Usb Usb Protocol"),
    new IdName (7, "Device Interface Win Usb Device Interface Classes"),
} },


 {"9660c283-fc3a-4a08-a096-eed3aac46da2",new HashSet<IdName>()
             { 
new IdName (100, "Contact Data Suppliers"),
} },

 {"967b5af8-995a-46ed-9e11-35b3c5b9782d",new HashSet<IdName>()
             { 
new IdName (100, "Photo Exposure Index"),
} },

 {"972e333e-ac7e-49f1-8adf-a70d07a9bcab",new HashSet<IdName>()
             { 
new IdName (100, "GPS Area Information"),
} },

 {"9744311e-7951-4b2e-b6f0-ecb293cac119",new HashSet<IdName>()
             {
    new IdName (1, "Devices Aep Bluetooth Issue Inquiry"),
    new IdName (2, "Devices Aep Bluetooth Le Active Scanning"),
    new IdName (3, "Devices Aep Bluetooth Le Scan Interval"),
    new IdName (4, "Devices Aep Bluetooth Le Scan Window"),
    new IdName (5, "Devices AepService Bluetooth Cache Mode"),
    new IdName (6, "Devices AepService Bluetooth Target Device"),
} },


 {"97b0ad89-df49-49cc-834e-660974fd755b",new HashSet<IdName>()
             { 
new IdName (100, "Contact Label"),
} },

 {"98f920d1-51e2-4722-9069-3c4b5cff5165",new HashSet<IdName>()
             { 
new IdName (100, "Is Barricade Page"),
} },

 {"98f98354-617a-46b8-8560-5b1b64bf1f89",new HashSet<IdName>()
             { 
new IdName (100, "Contact Home Address"),
} },

 {"995ef0b0-7eb3-4a8b-b9ce-068bb3f4af69",new HashSet<IdName>()
             {
    new IdName (1, "Devices Aep Bluetooth Le Appearance"),
    new IdName (2, "Devices Aep Bluetooth Le Advertisement"),
    new IdName (3, "Devices Aep Bluetooth Le Scan Response"),
    new IdName (4, "Devices Aep Bluetooth Le Address Type"),
    new IdName (5, "Devices Aep Bluetooth Le Appearance Category"),
    new IdName (6, "Devices Aep Bluetooth Le Appearance Subcategory"),
    new IdName (8, "Devices Aep Bluetooth Le Is Connectable"),
} },


 {"9973d2b5-bfd8-438a-ba94-5349b293181a",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Calendar"),
} },

 {"9a8ebb75-6458-4e82-bacb-35c0095b03bb",new HashSet<IdName>()
             { 
new IdName (100, "Photo Transcoded For Sync"),
} },

 {"9a93244d-a7ad-4ff8-9b99-45ee4cc09af6",new HashSet<IdName>()
             { 
new IdName (100, "Contact Assistant Telephone"),
} },

 {"9a9bc088-4f6d-469e-9919-e705412040f9",new HashSet<IdName>()
             { 
new IdName (100, "Message Is Fwd Or Reply"),
} },

 {"9ab84393-2a0f-4b75-bb22-7279786977cb",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Bearing Ref"),
} },

 {"9ad5badb-cea7-4470-a03d-b84e51b9949e",new HashSet<IdName>()
             { 
new IdName (100, "Contact Anniversary"),
} },

 {"9aebae7a-9644-487d-a92c-657585ed751a",new HashSet<IdName>()
             { 
new IdName (100, "Media Subscription Content Id"),
} },

 {"9b174b33-40ff-11d2-a27e-00c04fc30871",new HashSet<IdName>()
             {
    new IdName (2, "Recycle Deleted From"),
    new IdName (3, "Recycle Date Deleted"),
} },


 {"9b174b34-40ff-11d2-a27e-00c04fc30871",new HashSet<IdName>()
             {
    new IdName (4, "File Owner"),
    new IdName (8, "New Menu Preferred Types"),
    new IdName (10, "New Menu Allowed Types"),
} },


 {"9b174b35-40ff-11d2-a27e-00c04fc30871",new HashSet<IdName>()
             {
    new IdName (2, "Free Space"),
    new IdName (3, "Capacity"),
    new IdName (4, "Volume File System"),
    new IdName (5, "Percent Full"),
    new IdName (7, "Computer Decorated FreeSpace"),
    new IdName (10, "Volume Is Root"),
} },


 {"9b34bbb9-949c-488d-9a6d-eeb47c847a2f",new HashSet<IdName>()
             {
    new IdName (2, "Wireless Profile Name"),
    new IdName (4, "Wireless Security"),
    new IdName (5, "Wireless Radio Type"),
    new IdName (9, "Wireless Connection Mode"),
} },


 {"9bc2c99b-ac71-4127-9d1c-2596d0d7dcb7",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Distance Denominator"),
} },

 {"9c1fcf74-2d97-41ba-b4ae-cb2e3661a6e4",new HashSet<IdName>()
             {
    new IdName (5, "Priority"),
    new IdName (7, "Communication Newsgroup Name"),
    new IdName (8, "Message Has Attachments"),
    new IdName (10, "SAM Account Name"),
    new IdName (13, "Message Type"),
    new IdName (17, "Message Received"),
} },


 {"9cb0c358-9d7a-46b1-b466-dcc6f1a3d93d",new HashSet<IdName>()
             { 
new IdName (100, "Contact Display Mobile Phone Numbers"),
} },

 {"9d1d7cc5-5c39-451c-86b3-928e2d18cc47",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Latitude"),
} },

 {"9d2408b6-3167-422b-82b0-f583b7a7cfe3",new HashSet<IdName>()
             { 
new IdName (100, "Contact Spouse Name"),
} },

 {"9e7d118f-b314-45a0-8cfb-d654b917c9e9",new HashSet<IdName>()
             { 
new IdName (100, "Photo Brightness Numerator"),
} },


 {"a00742a1-cd8c-4b37-95ab-70755587767a",new HashSet<IdName>()
             { 
new IdName (3, "Device Interface Printer Enumeration Flag"),
} },

 {"a015ed5d-aaea-4d58-8a86-3c586920ea0b",new HashSet<IdName>()
             { 
new IdName (100, "GPS Measure Mode"),
} },

 {"a06992b3-8caf-4ed7-a547-b259e32ac9fc",new HashSet<IdName>()
             { 
new IdName (100, "Search Store"),
} },

 {"a09f084e-ad41-489f-8076-aa5be3082bca",new HashSet<IdName>()
             { 
new IdName (100, "Simple Rating"),
} },

 {"a0be94c5-50ba-487b-bd35-0654be8881ed",new HashSet<IdName>()
             { 
new IdName (100, "GPS DOP Denominator"),
} },

 {"a0e00ee1-f0c7-4d41-b8e7-26a7bd8d38b0",new HashSet<IdName>()
             {
    new IdName (2, "Devices Notifications Storage Full"),
    new IdName (3, "Devices Notifications Storage Full Link Text"),
} },


 {"a0e74609-b84d-4f49-b860-462bd9971f98",new HashSet<IdName>()
             { 
new IdName (100, "Photo Focal Length In Film"),
} },

 {"a11c005a-ff95-4785-8617-beaf92399c3c",new HashSet<IdName>()
             { 
new IdName (100, "HasLeafContainers"),
} },

 {"a1829ea2-27eb-459e-935d-b2fad7b07762",new HashSet<IdName>()
             { 
new IdName (2, "Devices Microphone Array Geometry"),
} },

 {"a19fb7a9-024b-4371-a8bf-4d29c3e4e9c9",new HashSet<IdName>()
             { 
new IdName (100, "Contact Connected Service Supported Actions"),
} },

 {"a26f4afc-7346-4299-be47-eb1ae613139f",new HashSet<IdName>()
             {
    new IdName (16, "Identity Key Provider Name"),
    new IdName (17, "Identity Key Provider Context"),
    new IdName (100, "Identity"),
} },


 {"a2e541c5-4440-4ba8-867e-75cfc06828cd",new HashSet<IdName>()
             { 
new IdName (100, "Photo Focal Plane Y Resolution Numerator"),
} },

 {"a3250282-fb6d-48d5-9a89-dbcace75cccf",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Longitude Numerator"),
} },

 {"a35996ab-11cf-4935-8b61-a6761081ecdf",new HashSet<IdName>()
             {
    new IdName (3, "Devices Aep Model Name"),
    new IdName (4, "Devices Aep Model Id"),
    new IdName (5, "Devices Aep Manufacturer"),
    new IdName (6, "Devices Aep Signal Strength"),
    new IdName (7, "Devices Aep Is Connected"),
    new IdName (9, "Devices Aep Is Present"),
    new IdName (12, "Devices Aep Device Address"),
    new IdName (16, "Devices Aep Is Paired"),
    new IdName (17, "Devices Aep Category"),
} },


 {"a399aac7-c265-474e-b073-ffce57721716",new HashSet<IdName>()
             { 
new IdName (2, "Devices AepService Bluetooth Service Guid"),
} },

 {"a3b29791-7713-4e1d-bb40-17db85f01831",new HashSet<IdName>()
             { 
new IdName (100, "Importance Text"),
} },

 {"a40294ef-d2b1-40ed-9512-dd3853b431f5",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Defer Image Prefetch"),
} },

 {"a4108708-09df-4377-9dfc-6d99986d5a67",new HashSet<IdName>()
             { 
new IdName (100, "Identity Is Me Identity"),
} },

 {"a45c254e-df1c-4efd-8020-67d146a850e0",new HashSet<IdName>()
             {
    new IdName (3, "Devices Hardware Ids"),
    new IdName (4, "Devices Compatible Ids"),
    new IdName (10, "Devices Class Guid"),
    new IdName (13, "Devices Device Manufacturer"),
    new IdName (17, "Devices Device Capabilities"),
    new IdName (29, "Devices Device Characteristics"),
    new IdName (37, "Devices Location Paths"),
} },


 {"a4790b72-7113-4348-97ea-292bbc1f6770",new HashSet<IdName>()
             {
    new IdName (5, "Visio Masters Keywords"),
    new IdName (6, "Visio Masters Details"),
} },


 {"a4aaa5b7-1ad0-445f-811a-0f8f6e67f6b5",new HashSet<IdName>()
             { 
new IdName (100, "GPS Img Direction Ref"),
} },

 {"a5477f61-7a82-4eca-9dde-98b69b2479b3",new HashSet<IdName>()
             { 
new IdName (100, "Recorded TV Recording Time"),
} },

 {"a63b464f-2ace-4d83-87ae-abaf011cc6ac",new HashSet<IdName>()
             { 
new IdName (1720, "Volume BitLocker Can Change Passphrase By Proxy"),
} },

 {"a6744477-c237-475b-a075-54f34498292a",new HashSet<IdName>()
             { 
new IdName (100, "Communication Task Status Text"),
} },

 {"a6f360d2-55f9-48de-b909-620e090a647c",new HashSet<IdName>()
             { 
new IdName (100, "Is Flagged Complete"),
} },

 {"a7b6f596-d678-4bc1-b05f-0203d27e8aa1",new HashSet<IdName>()
             {
    new IdName (101, "Contact Home Address1 Street"),
    new IdName (102, "Contact Home Address1 Locality"),
    new IdName (103, "Contact Home Address1 Region"),
    new IdName (104, "Contact Home Address1 Country"),
    new IdName (105, "Contact Home Address1 Postal Code"),
    new IdName (106, "Contact Home Address2 Street"),
    new IdName (107, "Contact Home Address2 Locality"),
    new IdName (108, "Contact Home Address2 Region"),
    new IdName (109, "Contact Home Address2 Country"),
    new IdName (110, "Contact Home Address2 Postal Code"),
    new IdName (111, "Contact Home Address3 Street"),
    new IdName (112, "Contact Home Address3 Locality"),
    new IdName (113, "Contact Home Address3 Region"),
    new IdName (114, "Contact Home Address3 Country"),
    new IdName (115, "Contact Home Address3 Postal Code"),
    new IdName (116, "Contact Business Address1 Street"),
    new IdName (117, "Contact Business Address1 Locality"),
    new IdName (118, "Contact Business Address1 Region"),
    new IdName (119, "Contact Business Address1 Country"),
    new IdName (120, "Contact Business Address1 Postal Code"),
    new IdName (121, "Contact Business Address2 Street"),
    new IdName (122, "Contact Business Address2 Locality"),
    new IdName (123, "Contact Business Address2 Region"),
    new IdName (124, "Contact Business Address2 Country"),
    new IdName (125, "Contact Business Address2 Postal Code"),
    new IdName (126, "Contact Business Address3 Street"),
    new IdName (127, "Contact Business Address3 Locality"),
    new IdName (128, "Contact Business Address3 Region"),
    new IdName (129, "Contact Business Address3 Country"),
    new IdName (130, "Contact Business Address3 Postal Code"),
    new IdName (131, "Contact Other Address1 Street"),
    new IdName (132, "Contact Other Address1 Locality"),
    new IdName (133, "Contact Other Address1 Region"),
    new IdName (134, "Contact Other Address1 Country"),
    new IdName (135, "Contact Other Address1 Postal Code"),
    new IdName (136, "Contact Other Address2 Street"),
    new IdName (137, "Contact Other Address2 Locality"),
    new IdName (138, "Contact Other Address2 Region"),
    new IdName (139, "Contact Other Address2 Country"),
    new IdName (140, "Contact Other Address2 Postal Code"),
    new IdName (141, "Contact Other Address3 Street"),
    new IdName (142, "Contact Other Address3 Locality"),
    new IdName (143, "Contact Other Address3 Region"),
    new IdName (144, "Contact Other Address3 Country"),
    new IdName (145, "Contact Other Address3 Postal Code"),
} },

 

 {"a7fe0840-1344-46f0-8d37-52ed712a4bf9",new HashSet<IdName>()
             { 
new IdName (100, "Parental Ratings Organization"),
} },

 {"a82d9ee7-ca67-4312-965e-226bcea85023",new HashSet<IdName>()
             { 
new IdName (100, "Message Flags"),
} },

 {"a8a74b92-361b-4e9a-b722-7c4a7330a312",new HashSet<IdName>()
             { 
new IdName (100, "Identity Provider Data"),
} },

 {"a8a7a412-1927-4a34-b1d4-45f67cc672fb",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Referrer Id"),
} },

 {"a93eae04-6804-4f24-ac81-09b266452118",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Distance"),
} },

 {"a94688b6-7d9f-4570-a648-e3dfc0ab2b3f",new HashSet<IdName>()
             { 
new IdName (100, "Offline Availability"),
} },

 {"a9ea193c-c511-498a-a06b-58e2776dcc28",new HashSet<IdName>()
             { 
new IdName (100, "Photo Orientation Text"),
} },

 {"aaa660f9-9865-458e-b484-01bc7fe3973e",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Organizer Name"),
} },

 {"aabaf6c9-e0c5-4719-8585-57b103e584fe",new HashSet<IdName>()
             { 
new IdName (100, "Photo Flash Manufacturer"),
} },

 {"aaf16bac-2b55-45e6-9f6d-415eb94910df",new HashSet<IdName>()
             { 
new IdName (100, "Contact TTY TDD Telephone"),
} },

 {"aaf4ee25-bd3b-4dd7-bfc4-47f77bb00f6d",new HashSet<IdName>()
             { 
new IdName (100, "GPS Differential"),
} },

 {"ab205e50-04b7-461c-a18c-2f233836e627",new HashSet<IdName>()
             { 
new IdName (100, "Photo Exposure Bias Denominator"),
} },

 {"acc9ce3d-c213-4942-8b48-6d0820f21c6d",new HashSet<IdName>()
             { 
new IdName (100, "GPS Speed Numerator"),
} },

 {"ad763ac7-f1ed-4039-9fb4-b7b84ef33cef",new HashSet<IdName>()
             { 
new IdName (2, "Search Provider Attributes"),
} },

 {"aeac19e4-89ae-4508-b9b7-bb867abee2ed",new HashSet<IdName>()
             {
    new IdName (2, "DRM Is Protected"),
    new IdName (3, "DRM Description"),
    new IdName (4, "DRM Play Count"),
    new IdName (5, "DRM Date Play Starts"),
    new IdName (6, "DRM Date Play Expires"),
    new IdName (7, "DRM Is Disabled"),
} },


 {"afc47170-14f5-498c-8f30-b0d19be449c6",new HashSet<IdName>()
             { 
new IdName (11, "DeviceInterface Printer Driver Name"),
} },

 {"afd97640-86a3-4210-b67c-289c41aabe55",new HashSet<IdName>()
             { 
new IdName (2, "Devices Safe Removal Required"),
} },

 {"b0b87314-fcf6-4feb-8dff-a50da6af561c",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Address Country"),
} },

 {"b180ad60-ed3f-4d16-bd43-f5b4fcf325a9",new HashSet<IdName>()
             {
    new IdName (2, "Sync Conflict ItemS hort Location"),
    new IdName (3, "Sync Conflict Item Full Location"),
} },


 {"b2f9b9d6-fec4-4dd5-94d7-8957488c807b",new HashSet<IdName>()
             {
    new IdName (2, "File Placeholder Status"),
    new IdName (3, "Storage Provider File Identifier"),
    new IdName (4, "Storage Provider File Version"),
    new IdName (5, "Storage Provider File Checksum"),
    new IdName (6, "Storage Provider File Version Waterline"),
    new IdName (7, "Storage Provider Caller Version Information"),
} },


 {"b33af30b-f552-4584-936c-cb93e5cda29f",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Required Attendee Names"),
} },

 {"b5c84c9e-5927-46b5-a3cc-933c21b78469",new HashSet<IdName>()
             { 
new IdName (100, "Contact Connected Service Name"),
} },


 {"b769d0fe-bc33-421a-8ce6-45add82ec756",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Suppress Local Hero"),
} },

 {"b771b352-8692-42e6-ac33-cc7b062ad950",new HashSet<IdName>()
             { 
new IdName (100, "Game Win SPR Recommended"),
} },

 {"b7b4d61c-5a64-4187-a52e-b1539f359099",new HashSet<IdName>()
             { 
new IdName (2, "Devices Win Phone8 Camera Flags"),
} },

 {"b812f15d-c2d8-4bbf-bacd-79744346113f",new HashSet<IdName>()
             { 
new IdName (100, "Photo Tag View Aggregate"),
} },

 {"b96eff7b-35ca-4a35-8607-29e3a54c46ea",new HashSet<IdName>()
             { 
new IdName (100, "Identity Provider Name"),
} },

 {"b9b4b3fc-2b51-4a42-b5d8-324146afcf25",new HashSet<IdName>()
             {
    new IdName (2, "Link Target Parsing Path"),
    new IdName (3, "Link Status"),
    new IdName (5, "Link Comment"),
    new IdName (6, "Item After"),
    new IdName (8, "Link Target SFGAO Flags"),
} },


 {"ba3b1da9-86ee-4b5d-a2a4-a271a429f0cf",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Bearing Numerator"),
} },

 {"bb44403b-1399-4650-95eb-03c53a57c2cf",new HashSet<IdName>()
             { 
new IdName (60, "Game Int Update Status"),
} },

 {"bc4e71ce-17f9-48d5-bee9-021df0ea5409",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Address Post Office Box"),
} },

 {"bccc8a3c-8cef-42e5-9b1c-c69079398bc7",new HashSet<IdName>()
             { 
new IdName (100, "Message To Do Title"),
} },

 {"bceee283-35df-4d53-826a-f36a3eefc6be",new HashSet<IdName>()
             { 
new IdName (100, "Search Container Hash"),
} },

 {"be1a72c6-9a1d-46b7-afe7-afaf8cef4999",new HashSet<IdName>()
             { 
new IdName (100, "Communication Task Status"),
} },

 {"be6e176c-4534-4d2c-ace5-31dedac1606b",new HashSet<IdName>()
             { 
new IdName (100, "GPS Longitude Denominator"),
} },

 {"bebe0920-7671-4c54-a3eb-49fddfc191ee",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Video"),
} },

 {"bf53d1c3-49e0-4f7f-8567-5a821d8ac542",new HashSet<IdName>()
             { 
new IdName (100, "Contact Callback Telephone"),
} },

 {"bf79c0ab-bb74-4cee-b070-470b5ae202ea",new HashSet<IdName>()
             {
    new IdName (2, "Devices Dnssd Service Name"),
    new IdName (3, "Devices Dnssd Domain"),
    new IdName (4, "Devices Dnssd Instance Name"),
    new IdName (5, "Devices Dnssd Full Name"),
    new IdName (6, "Devices Dnssd Text Attributes"),
    new IdName (7, "Devices Dnssd Host Name"),
    new IdName (8, "Devices Dnssd Weight"),
    new IdName (9, "Devices Dnssd Priority"),
    new IdName (10, "Devices Dnssd Ttl"),
    new IdName (11, "Devices Dnssd Network Adapte rId"),
    new IdName (12, "Devices Dnssd Port Number"),
} },


 {"bfee9149-e3e2-49a7-a862-c05988145cec",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Is Online"),
} },

 {"c06238b2-0bf9-4279-a723-25856715cb9d",new HashSet<IdName>()
             { 
new IdName (100, "Photo Gain Control Text"),
} },

 {"c0ac206a-827e-4650-95ae-77e2bb74fcc9",new HashSet<IdName>()
             { 
new IdName (100, "Contact Mailing Address"),
} },

 {"c107e191-a459-44c5-9ae6-b952ad4b906d",new HashSet<IdName>()
             { 
new IdName (100, "Photo Max Aperture Numerator"),
} },

 {"c2ea046e-033c-4e91-bd5b-d4942f6bbe49",new HashSet<IdName>()
             {
    new IdName (2, "Creator App Id"),
    new IdName (3, "Creator Open With UI Options"),
} },


 {"c4322503-78ca-49c6-9acc-a68e2afd7b6b",new HashSet<IdName>()
             { 
new IdName (100, "Identity User Name"),
} },

 {"c449d5cb-9ea4-4809-82e8-af9d59ded6d1",new HashSet<IdName>()
             { 
new IdName (100, "Music Is Compilation"),
} },

 {"c4c07f2b-8524-4e66-ae3a-a6235f103beb",new HashSet<IdName>()
             { 
new IdName (2, "Devices Notifications Low Battery"),
} },

 {"c4c4dbb2-b593-466b-bbda-d03d27d5e43a",new HashSet<IdName>()
             { 
new IdName (100, "GPS Longitude"),
} },

 {"c5043536-932e-219e-5fb9-1c2807d7b03e",new HashSet<IdName>()
             {
    new IdName (600, "Activity App Display Name"),
    new IdName (601, "Activity App Image Uri"),
    new IdName (602, "Activity Background Color"),
    new IdName (603, "Activity Content Image Uri"),
    new IdName (604, "Activity Content Uri"),
    new IdName (605, "Activity Description"),
    new IdName (606, "Activity Display Text"),
    new IdName (607, "Activity Tilexml"),
    new IdName (608, "Activity History Active Days"),
    new IdName (609, "Activity History Active Duration"),
    new IdName (610, "Activity History Active Hours"),
    new IdName (611, "Activity History App Activity Id"),
    new IdName (612, "Activity History App Id"),
    new IdName (613, "Activity History Device Display Name"),
    new IdName (614, "Activity History Device Id"),
    new IdName (615, "Activity History Display Text"),
    new IdName (616, "Activity History End Time"),
    new IdName (617, "Activity History Id"),
    new IdName (618, "Activity History Start Time"),
    new IdName (619, "Activity History Type"),
    new IdName (620, "Activity Activity Id"),
} },


 {"c53e42a9-db3c-4bc7-b0f3-83a524adf0ec",new HashSet<IdName>()
             { 
new IdName (1719, "Volume BitLocker Can Change Pin"),
} },

 {"c554493c-c1f7-40c1-a76c-ef8c0614003e",new HashSet<IdName>()
             { 
new IdName (100, "Contact Telex Number"),
} },

 {"c64a866e-41ae-4c8c-b3d5-dd6dbf70c9c1",new HashSet<IdName>()
             { 
new IdName (100, "Is Group"),
} },

 {"c66d4b3c-e888-47cc-b99f-9dca3ee34dea",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Bearing"),
} },

 {"c6f039e7-f6a4-4185-ae48-07938262c274",new HashSet<IdName>()
             { 
new IdName (100, "Hide In Grep Search"),
} },

 {"c75faa05-96fd-49e7-9cb4-9f601082d553",new HashSet<IdName>()
             { 
new IdName (100, "End Date"),
} },

 {"c77724d4-601f-46c5-9b89-c53f93bceb77",new HashSet<IdName>()
             { 
new IdName (100, "Photo Max Aperture Denominator"),
} },

 {"c89a23d0-7d6d-4eb8-87d4-776a82d493e5",new HashSet<IdName>()
             { 
new IdName (100, "Contact Home Address State"),
} },

 {"c8d1920c-01f6-40c0-ac86-2f3a4ad00770",new HashSet<IdName>()
             { 
new IdName (100, "GPS Track Denominator"),
} },

 {"c8ea94f0-a9e3-4969-a94b-9c62a95324e0",new HashSet<IdName>()
             { 
new IdName (100, "Contact Primary Address City"),
} },

 {"c9944a21-a406-48fe-8225-aec7e24c211b",new HashSet<IdName>()
             {
    new IdName (2, "PropList Full Details"),
    new IdName (3, "PropList Tile Info"),
    new IdName (4, "PropList Info Tip"),
    new IdName (5, "PropList Quick Tip"),
    new IdName (6, "PropList Preview Title"),
    new IdName (8, "PropList Preview Details"),
    new IdName (9, "PropList Extended Tile Info"),
    new IdName (10, "PropList File Operation Prompt"),
    new IdName (11, "PropList Conflict Prompt"),
    new IdName (12, "PropList Set Defaults For"),
    new IdName (13, "PropList Content View Mode For Browse"),
    new IdName (14, "PropList Content View Mode For Search"),
    new IdName (16, "PropList Status Icons"),
    new IdName (17, "Info Tip Text"),
    new IdName (18, "PropList Status Icons Display Flag"),
    new IdName (500, "Layout Pattern Content View Mode For Browse"),
    new IdName (501, "Layout Pattern Content View Mode For Search"),
    new IdName (502, "Layout Pattern Place Holder"),
    new IdName (503, "Layout Pattern Tiles View Mode"),
    new IdName (504, "Layout Pattern Group"),
    new IdName (510, "PropList Details Pane Null Select"),
    new IdName (511, "PropList Details Pane Null Select Title"),
} },



 
 {"c9b88dba-04db-4887-a200-cf0d3afe1146",new HashSet<IdName>()
             { 
new IdName (99, "Game Update Status"),
} },

 {"c9c141a9-1b4c-4f17-a9d1-f298538cadb8",new HashSet<IdName>()
             {
    new IdName (2, "Devices Aep Service Service Id"),
    new IdName (5, "Devices Aep Service Protocol Id"),
    new IdName (6, "Devices Aep Service Aep Id"),
    new IdName (7, "Devices Aep Service Parent Aep Is Paired"),
} },


 {"c9c34f84-2241-4401-b607-bd20ed75ae7f",new HashSet<IdName>()
             { 
new IdName (100, "Communication Header Item"),
} },

 {"cbf38310-4a17-4310-a1eb-247f0b67593b",new HashSet<IdName>()
             {
    new IdName (2, "Device Interface Hid Usage Page"),
    new IdName (3, "Device Interface Hid Usage Id"),
    new IdName (4, "Device Interface Hid Is Read Only"),
    new IdName (5, "Device Interface Hid Vendor Id"),
    new IdName (6, "Device Interface Hid Product Id"),
    new IdName (7, "Device Interface Hid Version Number"),
} } ,


 {"cc158e89-6581-4311-9637-a8da9002f118",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Require Install"),
} },

 {"cc301630-b192-4c22-b372-9f4c6d338e07",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup General"),
} },

 {"cc6f4f24-6083-4bd4-8754-674d0de87ab8",new HashSet<IdName>()
             { 
new IdName (100, "Contact Email Name"),
} },

 {"cd102c9c-5540-4a88-a6f6-64e4981c8cd1",new HashSet<IdName>()
             { 
new IdName (100, "Contact Assistant Name"),
} },

 {"cd9ed458-08ce-418f-a70e-f912c7bb9c5c",new HashSet<IdName>()
             { 
new IdName (103, "Message Message Class"),
} },

 {"cdbfc167-337e-41d8-af7c-8c09205429c7",new HashSet<IdName>()
             { 
new IdName (100, "Application Defined Properties"),
} },

 {"cdedcf30-8919-44df-8f4c-4eb2ffdb8d89",new HashSet<IdName>()
             { 
new IdName (100, "Photo Exposure Index Numerator"),
} },

 {"ce50c159-2fb8-41fd-be68-d3e042e274bc",new HashSet<IdName>()
             {
    new IdName (2, "Sync Handler Name"),
    new IdName (3, "Sync Item Name"),
    new IdName (4, "Sync Conflict Description"),
    new IdName (6, "Sync Conflict First Location"),
    new IdName (7, "Sync Conflict Second Location"),
    new IdName (10, "Sync Conflict Unresolvable"),
} },


 {"cea820b9-ce61-4885-a128-005d9087c192",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Latitude Ref"),
} },

 {"cebf9b37-26ae-466b-9fe9-c7550c4b0ce8",new HashSet<IdName>()
             { 
new IdName (100, "Transfer Path"),
} },

 {"cf5751fd-f4b3-443d-b31c-9a34740759ec",new HashSet<IdName>()
             { 
new IdName (100, "Search Scope"),
} },

 {"cfa31b45-525d-4998-bb44-3f7d81542fa4",new HashSet<IdName>()
             { 
new IdName (100, "Media Dlna Profile Id"),
} },

 {"cfc08d97-c6f7-4484-89dd-ebef4356fe76",new HashSet<IdName>()
             { 
new IdName (100, "Photo Focal Plane X Resolution"),
} },

 {"d042d2a1-927e-40b5-a503-6edbd42a517e",new HashSet<IdName>()
             { 
new IdName (100, "Contact Phone Numbers Canonical"),
} },

 {"d08dd4c0-3a9e-462e-8290-7b636b2576b9",new HashSet<IdName>()
             {
    new IdName (2, "Devices Interface Paths"),
    new IdName (3, "Devices Function Paths"),
    new IdName (10, "Devices Primary Category"),
    new IdName (257, "Devices Status 1"),
    new IdName (258, "Devices Status 2"),
    new IdName (259, "Devices Status"),
} },


 {"d0a04f0a-462a-48a4-bb2f-3706e88dbd7d",new HashSet<IdName>()
             { 
new IdName (100, "Item Authors"),
} },

 {"d0c7f054-3f72-4725-8527-129a577cb269",new HashSet<IdName>()
             { 
new IdName (100, "Sensitivity Text"),
} },

 {"d0dab0ba-368a-4050-a882-6c010fd19a4f",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Content"),
} },

 {"d21a7148-d32c-4624-8900-277210f79c0f",new HashSet<IdName>()
             { 
new IdName (100, "Image Compressed Bits Per Pixel Numerator"),
} },

 {"d35f743a-eb2e-47f2-a286-844132cb1427",new HashSet<IdName>()
             { 
new IdName (100, "Photo EXIF Version"),
} },

 {"d37d52c6-261c-4303-82b3-08b926ac6f12",new HashSet<IdName>()
             { 
new IdName (100, "Task Billing Information"),
} },

 {"d4729704-8ef1-43ef-9024-2bd381187fd5",new HashSet<IdName>()
             { 
new IdName (100, "Contact Children"),
} },

 {"d4bf61b3-442e-4ada-882d-fa7b70c832d9",new HashSet<IdName>()
             { 
new IdName (6, "Devices Aep Point Of Service Connection Types"),
} },

 {"d4d0aa16-9948-41a4-aa85-d97ff9646993",new HashSet<IdName>()
             { 
new IdName (100, "Item Participants"),
} },

 {"d55bae5a-3892-417a-a649-c6ac5aaaeab3",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Optional Attendee Addresses"),
} },

 {"d5cdd502-2e9c-101b-9397-08002b2cf9ae",new HashSet<IdName>()
             {
    new IdName (1, "Codepage"),
    new IdName (2, "Category"),
    new IdName (3, "Document Presentation Format"),
    new IdName (4, "Document ByteC ount"),
    new IdName (5, "Document Line Count"),
    new IdName (6, "Document Paragraph Count"),
    new IdName (7, "Document Slide Count"),
    new IdName (8, "Document Note Count"),
    new IdName (9, "Document Hidden Slide Count"),
    new IdName (10, "Document Multimedia Clip Count"),
    new IdName (11, "Scale"),
    new IdName (12, "Headingpair"),
    new IdName (13, "Document Parts"),
    new IdName (14, "Document Manager"),
    new IdName (15, "Company"),
    new IdName (16, "Document Links Dirty"),
    new IdName (26, "Content Type"),
    new IdName (27, "Content Status"),
    new IdName (28, "Language"),
    new IdName (29, "Document Version"),
} },


 {"d6304e01-f8f5-4f45-8b15-d024a6296789",new HashSet<IdName>()
             { 
new IdName (100, "Contact Pager Telephone"),
} },

 {"d68dbd8a-3374-4b81-9972-3ec30682db3d",new HashSet<IdName>()
             { 
new IdName (100, "Contact IM Address"),
} },

 {"d6942081-d53b-443d-ad47-5e059d9cd27a",new HashSet<IdName>()
             {
    new IdName (2, "Shell SFGAOFlagsStrings"),
    new IdName (3, "Link TargetSFGAOFlagsStrings"),
} },


 {"d6b5b883-18bd-4b4d-b2ec-9e38affeda82",new HashSet<IdName>()
             { 
new IdName (2, "Devices SmartCards ReaderKind"),
} },

 {"d6cf9145-d365-471b-bcb8-f0b4a96b891c",new HashSet<IdName>()
             { 
new IdName (100, "Fonts ActiveStatus"),
} },

 {"d7313ff1-a77a-401c-8c99-3dbdd68add36",new HashSet<IdName>()
             { 
new IdName (100, "Item Name Prefix"),
} },

 {"d76e7ba8-dfa6-48e7-9670-d62dfb07206b",new HashSet<IdName>()
             {
    new IdName (2, "Connected Search Contract Id"),
    new IdName (3, "Connected Search App Min Version"),
    new IdName (4, "Connected Search App Installed State"),
} },


 {"d7750ee0-c6a4-48ec-b53e-b87b52e6d073",new HashSet<IdName>()
             { 
new IdName (100, "Image Parsing Name"),
} },

 {"d7b61c70-6323-49cd-a5fc-c84277162c97",new HashSet<IdName>()
             { 
new IdName (100, "Photo Flash Energy Denominator"),
} },

 {"d98be98b-b86b-4095-bf52-9d23b2e0a752",new HashSet<IdName>()
             { 
new IdName (100, "Priority Text"),
} },

 {"d9c22960-532c-4bc6-9876-7b12b52593d7",new HashSet<IdName>()
             { 
new IdName (2, "Protocol Name"),
} },

 {"da520e51-f4e9-4739-ac82-02e0a95c9030",new HashSet<IdName>()
             { 
new IdName (100, "Identity Qualified User Name"),
} },

 {"da5d0862-6e76-4e1b-babd-70021bd25494",new HashSet<IdName>()
             { 
new IdName (100, "GPS Speed"),
} },


 {"dc54fd2e-189d-4871-aa01-08c2f57a4abc",new HashSet<IdName>()
             { 
new IdName (100, "Flag Status Text"),
} },

 {"dc5877c7-225f-45f7-bac7-e81334b6130a",new HashSet<IdName>()
             { 
new IdName (100, "GPS Img Direction Numerator"),
} },

 {"dc8f80bd-af1e-4289-85b6-3dfc1b493992",new HashSet<IdName>()
             {
    new IdName (100, "Message Conversation Id"),
    new IdName (101, "Message Conversation Index"),
} },


 {"dccb10af-b4e2-4b88-95f9-031b4d5ab490",new HashSet<IdName>()
             { 
new IdName (100, "Photo Focal Plane X Resolution Numerator"),
} },

 {"dce33a78-aa18-4b3d-b1df-a6621ac8bdd2",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Bypass View Action"),
} },

 {"dd141766-313a-4a30-90f0-056a7c968437",new HashSet<IdName>()
             {
    new IdName (2, "Print Status Document Count"),
    new IdName (3, "Print Status Error Status"),
    new IdName (4, "Print Status Location"),
    new IdName (5, "Print Status Comment"),
    new IdName (6, "Print Status Preferences"),
    new IdName (7, "Print Status Warning Status"),
    new IdName (8, "Print Status Info Status"),
    new IdName (9, "Scan Status Profile"),
} },


 {"ddd1460f-c0bf-4553-8ce4-10433c908fb0",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Address Street"),
} },

 {"de00de32-547e-4981-ad4b-542f2e9007d8",new HashSet<IdName>()
             { 
new IdName (100, "PropGroup Camera"),
} },

 {"de35258c-c695-4cbc-b982-38b0ad24ced0",new HashSet<IdName>()
             { 
new IdName (2, "Shell Omit From View"),
} },

 {"de41cc29-6971-4290-b472-f59f2e2f31e2",new HashSet<IdName>()
             { 
new IdName (100, "Media Date Released"),
} },

 {"de5ef3c7-46e1-484e-9999-62c5308394c1",new HashSet<IdName>()
             { 
new IdName (100, "Contact Primary Address Post Office Box"),
} },

 {"de621b8f-e125-43a3-a32d-5665446d632a",new HashSet<IdName>()
             { 
new IdName (25, "Security Encryption Owners Display"),
} },

 {"de9e220b-41d4-4690-8b6b-3d89e231eef1",new HashSet<IdName>()
             { 
new IdName (100, "Fonts Family Name"),
} },

 {"dea7c82c-1d89-4a66-9427-a4e3debabcb1",new HashSet<IdName>()
             { 
new IdName (100, "Journal Contacts"),
} },

 {"debda43a-37b3-4383-91e7-4498da2995ab",new HashSet<IdName>()
             {
    new IdName (5, "WNET Local Name"),
    new IdName (6, "WNET Remote Name"),
    new IdName (7, "WNET Comment"),
    new IdName (8, "WNET Provider"),
} },


 {"deeb2db5-0696-4ce0-94fe-a01f77a45fb5",new HashSet<IdName>()
             { 
new IdName (102, "Music Artist Sort Override"),
} },

 {"df975fd3-250a-4004-858f-34e29a3e37aa",new HashSet<IdName>()
             { 
new IdName (100, "Prop Group Contact"),
} },

 {"dfb9a04d-362f-4ca3-b30b-0254b17b5b84",new HashSet<IdName>()
             { 
new IdName (100, "Parsing Bind Context"),
} },

 {"e08805c8-e395-40df-80d2-54f0d6c43154",new HashSet<IdName>()
             { 
new IdName (100, "Document Document ID"),
} },

 {"e1277516-2b5f-4869-89b1-2e585bd38b7a",new HashSet<IdName>()
             { 
new IdName (100, "Photo Len sModel"),
} },

 {"e13d8975-81c7-4948-ae3f-37cae11e8ff7",new HashSet<IdName>()
             { 
new IdName (100, "Photo Shutter Speed Denominator"),
} },

 {"e1a9a38b-6685-46bd-875e-570dc7ad7320",new HashSet<IdName>()
             { 
new IdName (100, "Photo Aperture Denominator"),
} },

 {"e1ad4953-a752-443c-93bf-80c7525566c2",new HashSet<IdName>()
             {
    new IdName (2, "Connected Search Type"),
    new IdName (3, "Connected Search Rendering Template"),
    new IdName (4, "Connected Search Fallback Template"),
    new IdName (5, "Connected Search Telemetry Id"),
    new IdName (6, "Connected Search Impression Id"),
    new IdName (7, "Connected Search Is Visibility Tracked"),
    new IdName (8, "Connected Search Telemetry Data"),
    new IdName (9, "Connected Search Application Search Scope"),
    new IdName (10, "Connected Search Parent Id"),
    new IdName (11, "Connected Search Child Count"),
    new IdName (12, "Connected Search Top Level Id"),
    new IdName (13, "Connected Search Is Visible By Default"),
    new IdName (14, "Connected Search Is Activatable"),
    new IdName (15, "Connected Search Suggestion Context"),
    new IdName (16, "Connected Search Region Id"),
    new IdName (17, "Connected Search Item Source"),
    new IdName (18, "Connected Search Activation Command"),
    new IdName (19, "Connected Search Is History Item"),
    new IdName (20, "Connected Search Is App Available"),
    new IdName (21, "Connected Search History Title"),
    new IdName (22, "Connected Search History Description"),
    new IdName (23, "Connected Search History Glyph"),
    new IdName (27, "Connected Search Requires Consent"),
    new IdName (28, "Connected Search Copy Text"),
    new IdName (29, "Connected Search Add Open In Browser Command"),
    new IdName (30, "Connected Search Image Url"),
    new IdName (31, "Connected Search Image Prefetch Stage"),
    new IdName (32, "Connected Search Is Local Item"),
} },


 
 {"e1d4a09e-d758-4cd1-b6ec-34a8b5a73f80",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Address Postal Code"),
} },

 {"e2d40928-632c-4280-a202-e0c2ad1ea0f4",new HashSet<IdName>()
             {
    new IdName (2, "Connected Search Qs Code"),
    new IdName (3, "Connected Search Jump List"),
    new IdName (4, "Connected Search Voice Command Examples"),
} },


 {"e32596b0-1163-4e02-867a-12132db4ba06",new HashSet<IdName>()
             { 
new IdName (2, "IE FeedItem Local Id"),
} },

 {"e3690a87-0fa8-4a2a-9a9f-fce8827055ac",new HashSet<IdName>()
             { 
new IdName (100, "Prop Group Image"),
} },

 {"e3a7d2c1-80fc-4b40-8f34-30ea111bdc2e",new HashSet<IdName>()
             { 
new IdName (100, "Prop Group File System"),
} },


 {"e4f10a3c-49e6-405d-8288-a23bd4eeaa6c",new HashSet<IdName>()
             { 
new IdName (100, "File Extension"),
} },

 {"e53d799d-0f3f-466e-b2ff-74634a3cb7a4",new HashSet<IdName>()
             { 
new IdName (100, "Contact Primary Address Country"),
} },

 {"e5473742-4611-4aaf-9c49-a3417748cbc8",new HashSet<IdName>()
             { 
new IdName (100, "Invalid Path Value"),
} },

 {"e55fc3b0-2b60-4220-918e-b21e8bf16016",new HashSet<IdName>()
             { 
new IdName (100, "Identity Unique Id"),
} },

 {"e6822fee-8c17-4d62-823c-8e9cfcbd1d5c",new HashSet<IdName>()
             { 
new IdName (100, "Audio Is Variable Bit Rate"),
} },

 {"e6c3d9ad-7b32-4efe-a167-0a868ffdf3af",new HashSet<IdName>()
             { 
new IdName (100, "Game WinSPR Minimum"),
} },

 {"e6ddcaf7-29c5-4f0a-9a68-d19412ec7090",new HashSet<IdName>()
             { 
new IdName (100, "Photo Lens Manufacturer"),
} },

 {"e77e90df-6271-4f5b-834f-2dd1f245dda4",new HashSet<IdName>()
             {
    new IdName (2, "Storage Provider UI Status"),
    new IdName (3, "Storage Provider State"),
    new IdName (4, "Storage Provider Transfer Progress"),
} },


 {"e7b33238-6584-4170-a5c0-ac25efd9da56",new HashSet<IdName>()
             { 
new IdName (100, "Prop Group Recorded TV"),
} },

 {"e7c3fb29-caa7-4f47-8c8b-be59b330d4c5",new HashSet<IdName>()
             {
    new IdName (2, "Devices Aep Container Id"),
    new IdName (3, "Devices Aep Can Pair"),
} },


 {"e8309b6e-084c-49b4-b1fc-90a80331b638",new HashSet<IdName>()
             { 
new IdName (100, "Photo PeopleNames"),
} },

 {"e88dcce0-b7b3-11d1-a9f0-00aa0060fa31",new HashSet<IdName>()
             {
    new IdName (2, "Zip Folder Encrypted"),
    new IdName (3, "Zip Folder Method"),
    new IdName (4, "Zip Folder Ratio"),
    new IdName (5, "Zip Folder CRC32"),
    new IdName (6, "Zip Folder Compressed Size"),
} },


 {"e92a2496-223b-4463-a4e3-30eabba79d80",new HashSet<IdName>()
             { 
new IdName (100, "Photo FNumber Denominator"),
} },

 {"e9641eff-af25-4db7-947b-4128929f8ef5",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Suggestion Detail Text"),
} },

 {"e9edd392-0b4c-4cf2-82c0-b0d139666245",new HashSet<IdName>()
             {
    new IdName (102, "Structured Query Virtual Bcc"),
    new IdName (103, "Structured Query Virtual Cc"),
    new IdName (104, "Structured Query Virtual From"),
    new IdName (105, "Structured Query Virtual To"),
    new IdName (106, "Structured Query Virtual Organizer"),
    new IdName (107, "Structured Query Virtual Required Attendees"),
    new IdName (108, "Structured Query Virtual Optional Attendees"),
    new IdName (109, "Structured Query Virtual Resources"),
    new IdName (110, "Structured Query Virtual Date Created"),
    new IdName (111, "Structured Query Virtual Phone"),
    new IdName (112, "Structured Query Virtual Message Size"),
    new IdName (113, "Structured Query Virtual About"),
    new IdName (114, "Structured Query Virtual Is Read"),
    new IdName (115, "Structured Query Virtual Journal Duration"),
    new IdName (116, "Structured Query Virtual Is Encrypted"),
    new IdName (117, "Structured Query Virtual Type"),
    new IdName (118, "Structured Query Virtual Artist"),
} },


 {"ea810849-87ff-4b54-abd6-5b71adf466f8",new HashSet<IdName>()
             { 
new IdName (1, "Dui Control Resource"),
} },

 {"ec0b4191-ab0b-4c66-90b6-c6637cdebbab",new HashSet<IdName>()
             { 
new IdName (100, "Communication Policy Tag"),
} },

 {"ecf4b6f6-d5a6-433c-bb92-4076650fc890",new HashSet<IdName>()
             { 
new IdName (100, "GPS Dest Latitude Numerator"),
} },

 {"ecf7f4c9-544f-4d6d-9d98-8ad79adaf453",new HashSet<IdName>()
             { 
new IdName (100, "GPS Speed Ref"),
} },

 {"ed4df2d3-8695-450b-856f-f5c1c53acb66",new HashSet<IdName>()
             { 
new IdName (100, "GPS Des tDistance Ref"),
} },

 {"ee31306c-fb9b-4d62-8621-3575d972a9f9",new HashSet<IdName>()
             { 
new IdName (1718, "Volume BitLocker Requires Admin"),
} },

 {"ee3d3d8a-5381-4cfa-b13b-aaf66b5f4ec9",new HashSet<IdName>()
             { 
new IdName (100, "Photo White Balance"),
} },

 {"eec7b761-6f94-41b1-949f-c729720dd13c",new HashSet<IdName>()
             { 
new IdName (12, "Device Interface Printer Port Name"),
} },

 {"ef1167eb-cbfc-4341-a568-a7c91a68982c",new HashSet<IdName>()
             { 
new IdName (2, "Devices WiFi Interface Guid"),
} },

 {"ef884c5b-2bfe-41bb-aae5-76eedf4f9902",new HashSet<IdName>()
             {
    new IdName (100, "Is Shared"),
    new IdName (200, "Shared With"),
    new IdName (300, "Sharing Status"),
    new IdName (400, "Share Scope"),
} },


 {"f04bef95-c585-4197-a2b7-df46fdc9ee6d",new HashSet<IdName>()
             { 
new IdName (100, "Kind Text"),
} },

 {"f0f7984d-222e-4ad2-82ab-1dd8ea40e57e",new HashSet<IdName>()
             { 
new IdName (300, "Title Sort Override"),
} },

 {"f1176dfe-7138-4640-8b4c-ae375dc70a6d",new HashSet<IdName>()
             { 
new IdName (100, "Contact Primary Address State"),
} },

 {"f18dedf3-337f-42c0-9e03-cee08708a8c3",new HashSet<IdName>()
             { 
new IdName (100, "Identity Logon Status String"),
} },

 {"f1a24aa7-9ca7-40f6-89ec-97def9ffe8db",new HashSet<IdName>()
             { 
new IdName (100, "Contact File As Name"),
} },

 {"f1fdb4af-f78c-466c-bb05-56e92db0b8ec",new HashSet<IdName>()
             { 
new IdName (103, "Music Album Artist Sort Override"),
} },

 {"f21d9941-81f0-471a-adee-4e74b49217ed",new HashSet<IdName>()
             { 
new IdName (100, "Provider Item Id"),
} },

 {"f2275480-f782-4291-bd94-f13693513aec",new HashSet<IdName>()
             { 
new IdName (0, "Prop List XP Details Panel"),
} },

 {"f23f425c-71a1-4fa8-922f-678ea4a60408",new HashSet<IdName>()
             { 
new IdName (100, "Is Attachment"),
} },

 {"f271c659-7e5e-471f-ba25-7f77b286f836",new HashSet<IdName>()
             { 
new IdName (100, "Contact Business Email Addresses"),
} },

 {"f27abe3a-7111-4dda-8cb2-29222ae23566",new HashSet<IdName>()
             { 
new IdName (2, "Connected Search Disambiguation Id"),
} },


 {"f334115e-da1b-4509-9b3d-119504dc7abb",new HashSet<IdName>()
             { 
new IdName (100, "Document Contributor"),
} },

 {"f3713ada-90e3-4e11-aae5-fdc17685b9be",new HashSet<IdName>()
             { 
new IdName (100, "Prop Group GPS"),
} },

 {"f3aecac4-5b8d-436a-ad0c-64ab194fdaf3",new HashSet<IdName>()
             { 
new IdName (100, "Fonts Collection Name"),
} },

 {"f3c9b698-be85-47ce-888f-83874d9abcb4",new HashSet<IdName>()
             {
    new IdName (2, "App Contract Pinned"),
    new IdName (3, "App Contract Hidden"),
    new IdName (4, "App Contract Pinned Order"),
    new IdName (5, "App Contract Relevance"),
    new IdName (6, "App Contract Category"),
    new IdName (7, "App Contract Supported File Types"),
} },


 {"f3d8f40d-50cb-44a2-9718-40cb9119495d",new HashSet<IdName>()
             { 
new IdName (100, "Contact Initials"),
} },

 {"f50d2f5d-dda0-48d4-8d2b-e83729fb69a4",new HashSet<IdName>()
             { 
new IdName (100, "Item Query Condition"),
} },

 {"f6272d18-cecc-40b1-b26a-3911717aa7bd",new HashSet<IdName>()
             { 
new IdName (100, "Calendar Location"),
} },

 {"f628fd8c-7ba8-465a-a65b-c5aa79263a9e",new HashSet<IdName>()
             { 
new IdName (100, "Photo Metering Mode Text"),
} },

 {"f7db74b4-4287-4103-afba-f1b13dcd75cf",new HashSet<IdName>()
             { 
new IdName (100,  "Item Date"),
} },

 {"f8245476-2ec6-44be-b2f7-82ec2537fa2e",new HashSet<IdName>()
             {
    new IdName (100, "Condition"),
    new IdName (101, "Condition Key"),
} },


 {"f85bf840-a925-4bc2-b0c4-8e36b598679e",new HashSet<IdName>()
             { 
new IdName (100, "Photo Digital Zoom"),
} },

 {"f8d3f6ac-4874-42cb-be59-ab454b30716a",new HashSet<IdName>()
             { 
new IdName (100, "Sensitivity"),
} },

 {"f8fa7fa3-d12b-4785-8a4e-691a94f7a3e7",new HashSet<IdName>()
             { 
new IdName (100, "Contact Email Address"),
} },

 {"fa303353-b659-4052-85e9-bcac79549b84",new HashSet<IdName>()
             { 
new IdName (100, "Photo Maker Note"),
} },

 {"fa304789-00c7-4d80-904a-1e4dcc7265aa",new HashSet<IdName>()
             { 
new IdName (100, "Photo Gain Control"),
} },

 {"fb1de864-e06d-47f4-82a6-8a0aef44493c",new HashSet<IdName>()
             { 
new IdName (2, "Devices Audio Device Speech Processing Supported"),
} },

 {"fb3842cd-9e2a-4f83-8fcc-4b0761139ae9",new HashSet<IdName>()
             { 
new IdName (2, "Device Interface Proximity Supports Nfc"),
} },

 {"fc6976db-8349-4970-ae97-b3c5316a08f0",new HashSet<IdName>()
             { 
new IdName (100, "Photo Sharpness"),
} },

 {"fc9f7306-ff8f-4d49-9fb6-3ffe5c0951ec",new HashSet<IdName>()
             { 
new IdName (100, "Contact Department"),
} },

 {"fcad3d3d-0858-400f-aaa3-2f66cce2a6bc",new HashSet<IdName>()
             { 
new IdName (100, "Photo Flash Energy Numerator"),
} },

 {"fcc16823-baed-4f24-9b32-a0982117f7fa",new HashSet<IdName>()
             { 
new IdName (100, "Identity Primary Email Address"),
} },

 {"fceff153-e839-4cf3-a9e7-ea22832094b8",new HashSet<IdName>()
             {
    new IdName (100, "File Offline Availability Status"),
    new IdName (101, "Folder Kind"),
    new IdName (103, "Sync Transfer Status"),
    new IdName (104, "Transfer Position"),
    new IdName (105, "Transfer Size"),
    new IdName (106, "Transfer Order"),
    new IdName (107, "Last Sync Error"),
    new IdName (108, "Storage Provider Id"),
    new IdName (109, "Storage Provider Error"),
    new IdName (110, "Storage Provider Status"),
    new IdName (111, "Storage Provider Share Statuses"),
    new IdName (112, "Storage Provider File Remote Uri"),
    new IdName (113, "Cached File Updater Content Id For Stream"),
    new IdName (114, "Cached File Updater Content Id For Conflict Resolution"),
    new IdName (115, "Remote Conflicting File"),
    new IdName (116, "Storage Provider Thumbnail Dimensions"),
    new IdName (117, "Storage Provider Sharing Status"),
    new IdName (118, "Storage Provider Descendant Sharing Status"),
    new IdName (119, "Storage Provider Fully Qualified Id"),
    new IdName (120, "Storage Provider Custom States"),
    new IdName (121, "Item Custom State State List"),
    new IdName (122, "Item Custom State Values"),
    new IdName (123, "Item Custom State Icon References"),
    new IdName (124, "Storage Provider Aggregated Custom States"),
    new IdName (125, "Storage Provider Network Connected"),
    new IdName (126, "Storage Provider Warning Error State"),
    new IdName (127, "Storage Provider Protection Mode"),
} },


 {"fcfb52aa-c1e5-4cd8-88bc-f80fd7390f20",new HashSet<IdName>()
             { 
new IdName (100, "Not User Content"),
} },

 {"fd122953-fa93-4ef7-92c3-04c946b2f7c8",new HashSet<IdName>()
             { 
new IdName (100, "Music Display Artist"),
} },

 {"fd9d9fc7-38ec-436d-8fc6-ec39bad301e6",new HashSet<IdName>()
             {
    new IdName (100, "Computer Processor"),
    new IdName (101, "Computer Memory"),
} },


 {"fdf84370-031a-4add-9e91-0d775f1c6605",new HashSet<IdName>()
             { 
new IdName (100, "Mileage Information"),
} },

 {"fe83bb35-4d1a-42e2-916b-06f3e1af719e",new HashSet<IdName>()
             { 
new IdName (100, "Photo Flash Model"),
} },

 {"fe9e4c12-aacb-4aa3-966d-91a29e6128b5",new HashSet<IdName>()
             {
    new IdName (3, "Printer Default"),
    new IdName (4, "Printer Location"),
    new IdName (5, "Printer Model"),
    new IdName (6, "Printer Queue Size"),
    new IdName (7, "Printer Status"),
} },


 {"fec690b7-5f30-4646-ae47-4caafba884a3",new HashSet<IdName>()
             { 
new IdName (100, "Photo Exposure Program Text"),
} },

 {"fec7952b-4bf0-4c03-b6e1-2796818b7ca9",new HashSet<IdName>()
             { 
new IdName (100, "Fonts Version"),
} },

 {"ff1167eb-cbfc-4341-a568-a7c91a68982c",new HashSet<IdName>()
             { 
new IdName (2, "Devices Wwan Interface Guid"),
} },

 {"ff962609-b7d6-4999-862d-95180d529aea",new HashSet<IdName>()
             { 
new IdName (100, "Contact Other Address Street"),
} },

 {"ffae9db7-1c8d-43ff-818c-84403aa3732d",new HashSet<IdName>()
             { 
new IdName (100, "Source Package Family Name"),
} },





        };



        public static string GetDescriptionFromGuidAndKey(string guid, int key)
        {
            var desc = "(Description not available)";

            if (PropertyStoreInfo.ContainsKey(guid.ToLowerInvariant()))
            {
                var psi = PropertyStoreInfo[guid.ToLowerInvariant()];

                var d = psi.SingleOrDefault(t => t.Id == key);

                if (d != null)
                {
                    desc = d.Name;
                }
            }

            return desc;
        }


        public static IExtensionBlock GetExtensionBlockFromBytes(long signature, byte[] rawBytes)
        {
            IExtensionBlock block = null;

            switch (signature)
            {
                case 0xbeef0000:
                    var beef0000 = new Beef0000(rawBytes);
                    block = beef0000;

                    break;

                case 0xbeef0001:
                    var beef0001 = new Beef0001(rawBytes);
                    block = beef0001;

                    break;

                case 0xbeef0002:
                    var beef0002 = new Beef0002(rawBytes);
                    block = beef0002;

                    break;

                case 0xbeef0003:
                    var beef0003 = new Beef0003(rawBytes);
                    block = beef0003;

                    break;

                case 0xbeef0004:
                    var beef0004 = new Beef0004(rawBytes);
                    block = beef0004;

                    break;

                case 0xbeef0005:
                    var beef0005 = new Beef0005(rawBytes);
                    block = beef0005;

                    break;

                case 0xbeef0006:
                    var beef0006 = new Beef0006(rawBytes);
                    block = beef0006;

                    break;

                case 0xbeef0008:
                    var beef0008 = new Beef0008(rawBytes);
                    block = beef0008;

                    break;

                case 0xbeef0009:
                    var beef0009 = new Beef0009(rawBytes);
                    block = beef0009;

                    break;

                case 0xbeef000a:
                    var beef000A = new Beef000a(rawBytes);
                    block = beef000A;

                    break;

                case 0xbeef000c:
                    var beef000C = new Beef000c(rawBytes);
                    block = beef000C;

                    break;

//                case 0xbeef000e:
//                    var beef000e = new Beef000e(rawBytes);
//                    block = beef000e;
//
//                    break;

                case 0xbeef0010:
                    var beef0010 = new Beef0010(rawBytes);
                    block = beef0010;

                    break;

                case 0xbeef001b:
                    var beef001B = new Beef001b(rawBytes);
                    block = beef001B;

                    break;

                case 0xbeef0013:
                    var beef0013 = new Beef0013(rawBytes);
                    block = beef0013;

                    break;

                case 0xbeef0014:
                    var beef0014 = new Beef0014(rawBytes);
                    block = beef0014;

                    break;

                case 0xbeef0016:
                    var beef0016 = new Beef0016(rawBytes);
                    block = beef0016;

                    break;

                case 0xbeef0017:
                    var beef0017 = new Beef0017(rawBytes);
                    block = beef0017;

                    break;

                case 0xbeef0019:
                    var beef0019 = new Beef0019(rawBytes);
                    block = beef0019;

                    break;

                case 0xbeef001a:
                    var beef001a = new Beef001a(rawBytes);
                    block = beef001a;

                    break;


                case 0xbeef001d:
                    var beef001d = new Beef001d(rawBytes);
                    block = beef001d;

                    break;

                case 0xbeef001e:
                    var beef001e = new Beef001e(rawBytes);
                    block = beef001e;

                    break;

                case 0xbeef0021:
                    var beef0021 = new Beef0021(rawBytes);
                    block = beef0021;

                    break;
                case 0xbeef0024:
                    var beef0024 = new Beef0024(rawBytes);
                    block = beef0024;

                    break;
                

                case 0xbeef0025:
                    var beef0025 = new Beef0025(rawBytes);
                    block = beef0025;

                    break;

                case 0xbeef0026:
                    var beef0026 = new Beef0026(rawBytes);
                    block = beef0026;

                    break;

                case 0xbeef0027:
                    var beef0027 = new Beef0027(rawBytes);
                    block = beef0027;

                    break;

                case 0xbeef0029:
                    var beef0029 = new Beef0029(rawBytes);
                    block = beef0029;

                    break;

                default:
                    var feebunknown = new BeefUnknown(rawBytes);
                    block = feebunknown;
                    break;
            }

            return block;
        }

        private static string[] MultistringToStringArray(ref char[] arg)
        {
            // Search an array of bytes for a double-null before converting to string.
            int qty, j;
            for (qty = 0, j = 1;; qty++, j++)
            {
                var done = qty > (arg.Length - 1) || (arg[qty] == 0 && j < arg.Length && arg[j] == 0);
                if (done)
                {
                    break;
                } // if
            } // for
            var b = new string(arg, 0, qty);
            return b.Split(new[] {'\0'}, 9, StringSplitOptions.RemoveEmptyEntries);
        } // lengthDoubleNull

        public static List<string> GetStringsFromMultistring(byte[] rawBytes)
        {
               var outstrings = new List<string>();

            //drop last 2 bytes
            rawBytes = rawBytes.Take(rawBytes.Length - 2).ToArray();

            var bytestring = BitConverter.ToString(rawBytes);

            if (bytestring.StartsWith("00-00-00-") == false)
            {
                if (bytestring.EndsWith("-00-00-00") == false)
                {
                    bytestring = bytestring + "-00";
                }

                var index = 0;

                try
                {
                    var regexObj = new Regex("00-00-", RegexOptions.IgnoreCase);
                    var matchResults = regexObj.Match(bytestring);
                    while (matchResults.Success)
                    {
                        // matched text: matchResults.Value
                        // match start: matchResults.Index

                        var instancePosition = (matchResults.Index + 3) / 3;

                        if (instancePosition + 2 < rawBytes.Length)
                        {
                            outstrings.Add(
                                ReplaceNulls(Encoding.Unicode.GetString(rawBytes, index, instancePosition - index)));

                            index += instancePosition + 2;
                        }

                        // match length: matchResults.Length
                        matchResults = matchResults.NextMatch();
                    }
                    if (index + 4 <= rawBytes.Length)
                    {
                        outstrings.Add(ReplaceNulls(Encoding.Unicode.GetString(rawBytes, index, rawBytes.Length - index)));
                    }
                }
                catch (ArgumentException)
                {
                    throw;
                }
            }
            else
            {
                outstrings.Add(string.Empty);
            }
            return outstrings;
        }

        public static byte[] GetBytesFromBitConverterString(string input)
        {
            return input.Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();
        }

        public static string ReplaceNulls(string data)
        {
            if (data == null)
            {
                return string.Empty;
            }
            return data.Replace("\0", string.Empty);
        }

        public static byte[] ConvertHexStringToByteArray(string data)
        {
            var buffer = new byte[data.Length/2];
            for (var i = 0; i < buffer.Length; i++)
            {
                byte.TryParse(data.Substring(i*2, 2), NumberStyles.HexNumber,
                    null, out buffer[i]);
            }
            return buffer;
        }

        public static string ExtractGuidFromShellItem(byte[] rawguid)
        {
            var part1 = new byte[4];
            var part2 = new byte[2];
            var part3 = new byte[2];
            var part4 = new byte[2];
            var part5 = new byte[6];

            Array.Copy(rawguid, 0, part1, 0, 4);
            Array.Copy(rawguid, 4, part2, 0, 2);
            Array.Copy(rawguid, 6, part3, 0, 2);
            Array.Copy(rawguid, 8, part4, 0, 2);
            Array.Copy(rawguid, 10, part5, 0, 6);

            Array.Reverse(part1);
            Array.Reverse(part2);
            Array.Reverse(part3);

            var p1 = BitConverter.ToString(part1).Replace("-", "");
            var p2 = BitConverter.ToString(part2).Replace("-", "");
            var p3 = BitConverter.ToString(part3).Replace("-", "");
            var p4 = BitConverter.ToString(part4).Replace("-", "");
            var p5 = BitConverter.ToString(part5).Replace("-", "");

            return $"{p1}-{p2}-{p3}-{p4}-{p5}".ToLowerInvariant();
        }

        public static string GetFolderNameFromGuid(string guid)
        {
            return GuidMapping.GuidMapping.GetDescriptionFromGuid(guid);

        }

        public static DateTimeOffset? ExtractDateTimeOffsetFromBytes(byte[] rawBytes)
        {
            DateTimeOffset? thedate = null;

            var somedate = BitConverter.ToUInt16(rawBytes, 0);

            var day = (somedate & 0x1f);
            var month = (somedate & 0x1e0) >> 5;
            var year = ((somedate & 0xfe00) >> 9) + 1980;

            var sometime = BitConverter.ToUInt16(rawBytes, 2);

            var someTimeBinary = Convert.ToString(sometime, 2).PadLeft(16, '0');

            var chunkt1 = someTimeBinary.Substring(0, 5);
            var chunkt2 = someTimeBinary.Substring(5, 6);
            var chunkt3 = someTimeBinary.Substring(11, 5);

            var hour = (int) Convert.ToUInt32(chunkt1, 2);
            var minute = (int) Convert.ToUInt32(chunkt2, 2);
            var seconds = (int) Convert.ToUInt32(chunkt3, 2)*2;

            try
            {
                var dt = new DateTime(year, month, day, hour, minute, seconds, DateTimeKind.Utc);
                var dtoffset = new DateTimeOffset(dt, TimeZoneInfo.Utc.BaseUtcOffset);

                thedate = dtoffset;
            }
            catch (Exception)
            {
                
                    return null;
                
            }

            return thedate;
        }
    }
}
