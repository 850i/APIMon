using System;

namespace APIMonLib.Hooks.kernel32.dll
{

	///// <summary>
	///// Name of the hook class should be Hook_"API name"
	///// This name convention is assumed when looking for all classes implementing hooks
	///// </summary>
	//public class Hook_CreateFileMappingW : AbstractHookDescription
	//{
	//    //public override APIFullName api_full_name { get { return new APIFullName("kernel32.dll", "CreateFileMappingW"); } }

	//    protected override Delegate createHookDelegate()
	//    {
	//        return new Kernel32Support.DCreateFileMappingW(CreateFileMappingW_Hooked);
	//    }

	//    // this is where we are intercepting all file accesses!
	//    private IntPtr CreateFileMappingW_Hooked(IntPtr hFile, IntPtr lpFileMappingAttributes, Kernel32Support.FileMapProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName)
	//    {
	//        preprocessHook();

	//        TransferUnit transfer_unit = createTransferUnit();
	//        transfer_unit["FileHandle"] = hFile.ToInt32();
	//        transfer_unit["lpFileMappingAttributes"] = (uint)lpFileMappingAttributes;
	//        transfer_unit["flProtect"] = (uint)flProtect;
	//        transfer_unit["dwMaximumSizeHigh"] = dwMaximumSizeHigh;
	//        transfer_unit["dwMaximumSizeLow"] = (uint)dwMaximumSizeLow;
	//        transfer_unit["Name"] = lpName;

	//        // call original API through our Kernel32Support class
	//        IntPtr view_handle = Kernel32Support.CreateFileMappingW(hFile,  lpFileMappingAttributes,  flProtect,  dwMaximumSizeHigh,  dwMaximumSizeLow, lpName);

	//        transfer_unit["ViewHandle"] = view_handle;

	//        //Console.WriteLine("CreateFileMapping( FileHandle="+hFile+")");

	//        makeCallBack(transfer_unit);

	//        return view_handle;
	//    }
	//}
}
