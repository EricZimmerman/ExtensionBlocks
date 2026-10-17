using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ExtensionBlocks
{
    public class ShellBagMTPDevice : ShellBag
    {
        public ShellBagMTPDevice(int slot, int mruPosition, byte[] rawBytes, string bagPath)
        {
            Slot = slot;
            MruPosition = mruPosition;

            FriendlyName = "MTP device";
            ShellBagType = Utils.ShellBagTypes.MtpDevice;

            ChildShellBags = new List<IShellBag>();

            InternalId = Guid.NewGuid().ToString();

            HexValue = rawBytes;

            ExtensionBlocks = new List<IExtensionBlock>();

            BagPath = bagPath;

            var index = 0;
            var size = BitConverter.ToInt16(rawBytes, index);
            index += 2;

            var typeid = rawBytes[index];
            index += 1;

            index = 0x1e; //length if device name
            var deviceNameSize = BitConverter.ToInt32(rawBytes, index);
            index += 4;
            var devicePathSize = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            index += 2; //unknown

            DeviceName = Encoding.Unicode.GetString(rawBytes, index, deviceNameSize * 2).Trim('\0');
            index += deviceNameSize * 2;

            DevicePath = Encoding.Unicode.GetString(rawBytes, index, devicePathSize * 2).Trim('\0');
            ;
            index += devicePathSize * 2;

            index += 4; //unknown

            var guidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBytes, 0, 16);
            index += 16;

            var classIdGuid = new Guid(guidBytes);
            ClassId = GuidMapping.GuidMapping.GetDescriptionFromGuid(classIdGuid.ToString());

            index += 4; //unknown

            guidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBytes, 0, 16);
            index += 16;

            DevicePropsGuid = new Guid(guidBytes);

            index += 8; //unknown

            var deviceNameSizeUnicode = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            var deviceNameUnicode = Encoding.Unicode.GetString(rawBytes, index, deviceNameSizeUnicode).Trim('\0');
            index += deviceNameSizeUnicode;

            Value = deviceNameUnicode;

            guidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBytes, 0, 16);
            index += 16;

            FunctionalObjectGuid = new Guid(guidBytes);

            index += 8; //unknown

            guidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBytes, 0, 16);
            index += 16;

            FunctionalCategoryGuid = new Guid(guidBytes);

            index += 2; //unknown

            guidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBytes, 0, 16);
            index += 16;

            DelegateFolderGuid = new Guid(guidBytes);

            guidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, index, guidBytes, 0, 16);
            index += 16;

            var portableDeviceGuid = new Guid(guidBytes);
            PortableDeviceId = GuidMapping.GuidMapping.GetDescriptionFromGuid(portableDeviceGuid.ToString());

            //(no assert: extra data is not an error, and a failed Trace.Assert ends the process)
        }

        public string ClassId { get; }
        public string DeviceName { get; }
        public string DevicePath { get; }
        public Guid DevicePropsGuid { get; }
        public Guid FunctionalObjectGuid { get; }
        public Guid FunctionalCategoryGuid { get; }
        public Guid DelegateFolderGuid { get; }
        public string PortableDeviceId { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Class Id: {ClassId}");
            sb.AppendLine($"Device path: {DevicePath}");
            sb.AppendLine($"Device properties GUID: {DevicePropsGuid}");
            sb.AppendLine($"Functional object GUID: {FunctionalObjectGuid}");
            sb.AppendLine($"Functional category GUID: {FunctionalCategoryGuid}");
            sb.AppendLine($"Delegate folder GUID: {DelegateFolderGuid}");
            sb.AppendLine($"Portable device Id: {PortableDeviceId}");
            sb.AppendLine("");

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}