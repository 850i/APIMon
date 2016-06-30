using System;

namespace APIMonLib.Hooks.kernel32.dll
{
	//public class Hook_CreateFileW : AbstractHookDescription
	//{
	//    //public override APIFullName api_full_name { get { return new APIFullName("kernel32.dll", "CreateFileW"); } }

	//    protected override Delegate createHookDelegate() {
	//        return new Kernel32Support.DCreateFileW(CreateFile_Hooked);
	//    }

	//    // this is where we are intercepting all file accesses!
	//    private IntPtr CreateFile_Hooked(
	//          string lpFileName,
	//          Kernel32Support.EFileAccess dwDesiredAccess,
	//          Kernel32Support.EFileShare dwShareMode,
	//          IntPtr lpSecurityAttributes,
	//          Kernel32Support.ECreationDisposition dwCreationDisposition,
	//          Kernel32Support.EFileAttributes dwFlagsAndAttributes,
	//          IntPtr hTemplateFile) {
	//        preprocessHook();

	//        TransferUnit transfer_unit = createTransferUnit();
	//        transfer_unit["InFileName"] = lpFileName;
	//        transfer_unit["InDesiredAccess"] = (uint)dwDesiredAccess;
	//        transfer_unit["InShareMode"] = (uint)dwShareMode;
	//        transfer_unit["InSecurityAttributes"] = lpSecurityAttributes;
	//        transfer_unit["InCreationDisposition"] = (uint)dwCreationDisposition;
	//        transfer_unit["InFlagsAndAttributes"] = (uint)dwFlagsAndAttributes;
	//        transfer_unit["InTemplateFile"] = hTemplateFile;

	//        // call original API through our Kernel32Support class
	//        IntPtr file_handle = Kernel32Support.CreateFile(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
	//        //Console.WriteLine("CreateFile: " + lpFileName);
	//        transfer_unit["FileHandle"] = file_handle.ToInt32();
	//        //check if call was successful and make callback
	//        if (!(file_handle.ToInt32() == Kernel32Support.INVALID_HANDLE_VALUE)) makeCallBack(transfer_unit);

	//        return file_handle;
	//    }

	//}

    //[Serializable]
    //public class CreateFileTransferUnit : TransferUnit
    //{
    //    public IntPtr FileHandle;

    //    public String InFileName;

    //    public UInt32 InDesiredAccess;

    //    public UInt32 InShareMode;

    //    public IntPtr InSecurityAttributes;

    //    public UInt32 InCreationDisposition;

    //    public UInt32 InFlagsAndAttributes;

    //    public IntPtr InTemplateFile;

    //    public override String ToString()
    //    {
    //        return base.ToString() + "\tCreateFile( " +
    //                                "InFileName=\"" + this.InFileName + "\"" +
    //                                ", InDesiredAccess=0x" + this.InDesiredAccess.ToString("X") +
    //                                ", InShareMode=0x" + this.InShareMode.ToString("X") +
    //                                ", InSecurityAttributes=" + this.InSecurityAttributes +
    //                                ", InCreationDisposition=0x" + this.InCreationDisposition.ToString("X") +
    //                                ", InFlagsAndAttributes=0x" + this.InFlagsAndAttributes.ToString("X") +
    //                                ", InTemplateFile=" + this.InTemplateFile +
    //                                "  )";
    //    }
    //}

}
