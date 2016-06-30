using System;

namespace APIMonLib.Hooks.kernel32.dll
{
	//public class Hook_MapViewOfFileEx : AbstractHookDescription
	//{
	//    //public override APIFullName api_full_name { get { return new APIFullName("kernel32.dll", "MapViewOfFileEx"); } }

	//    protected override Delegate createHookDelegate()
	//    {
	//        return new Kernel32Support.DMapViewOfFileEx(MapViewOfFile_Hooked);
	//    }

	//    // this is where we are intercepting all file accesses!
	//    private IntPtr MapViewOfFile_Hooked(IntPtr hFileMappingObject,
	//       Kernel32Support.EFileMapAccessType dwDesiredAccess,
	//       uint dwFileOffsetHigh,
	//       uint dwFileOffsetLow,
	//       UIntPtr dwNumberOfBytesToMap,
	//       IntPtr lpBaseAddress)
	//    {
	//        preprocessHook();

	//        TransferUnit transfer_unit = createTransferUnit();
	//        transfer_unit["hFileMappingObject"] = hFileMappingObject;
	//        transfer_unit["dwDesiredAccess"] = dwDesiredAccess;
	//        transfer_unit["dwFileOffsetHigh"] = dwFileOffsetHigh;
	//        transfer_unit["dwFileOffsetLow"] = dwFileOffsetLow;
	//        transfer_unit["dwNumberOfBytesToMap"] = dwNumberOfBytesToMap;
	//        transfer_unit["lpBaseAddress"] = lpBaseAddress;

	//        // call original API through our Kernel32Support class
	//        IntPtr starting_address = Kernel32Support.MapViewOfFileEx(hFileMappingObject, dwDesiredAccess, dwFileOffsetHigh, dwFileOffsetLow, dwNumberOfBytesToMap, lpBaseAddress);

	//        transfer_unit["starting_address"] = starting_address;

	//        makeCallBack(transfer_unit);

	//        return starting_address;
	//    }

	//}

    //[Serializable]
    //public class MapViewOfFileExTransferUnit : TransferUnit
    //{

    //    public IntPtr hFileMappingObject;
    //    public Kernel32Support.EFileMapAccessType dwDesiredAccess;
    //    public uint dwFileOffsetHigh;
    //    public uint dwFileOffsetLow;
    //    public UIntPtr dwNumberOfBytesToMap;
    //    public IntPtr lpBaseAddress;
    //    public IntPtr starting_address;
    //    public override String ToString()
    //    {
    //        return base.ToString() + "\tMapViewOfFileEx(" +
    //                                "  hFileMappingObject=0x\"" + this.hFileMappingObject.ToString("X") + "\"" +
    //                                ", dwDesiredAccess=0x" + this.dwDesiredAccess.ToString("X") +
    //                                ", dwFileOffsetHigh=0x" + this.dwFileOffsetHigh.ToString("X") +
    //                                ", dwFileOffsetLow=0x" + this.dwFileOffsetLow.ToString("X") +
    //                                ", dwNumberOfBytesToMap=" + this.dwNumberOfBytesToMap +
    //                                ", lpBaseAddress=0x" + this.lpBaseAddress.ToString("X") +
    //                                "  ) = 0x" + starting_address.ToString("X");
    //    }
    //}
}

