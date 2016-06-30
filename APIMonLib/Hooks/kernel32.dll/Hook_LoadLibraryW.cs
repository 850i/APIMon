using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.kernel32.dll
{
	//public class Hook_LoadLibraryExW : AbstractHookDescription
	//{
	//    //we need to hook load library and check if there some ordered hooks pending for set up in that library
	//    //Need to perform load and then check for hooks needed to be installed after that

	//    protected override Delegate createHookDelegate()
	//    {
	//        return new Kernel32Support.DLoadLibraryExW(LoadLibraryExW_Hooked);
	//    }

	//    // this is where we are intercepting all file accesses!
	//    private IntPtr LoadLibraryExW_Hooked(string lpFileName, IntPtr hFile, Kernel32Support.LoadLibraryFlags dwFlags)
	//    {
	//        Console.WriteLine("Begin ---------------------LoadLibraryExW(\"" + lpFileName + "\")");
	//        preprocessHook();

	//        TransferUnit transfer_unit = createTransferUnit();
	//        transfer_unit["InFileName"] = lpFileName;
	//        transfer_unit["hFile"] = hFile;
	//        transfer_unit["dwFlags"] = dwFlags;

	//        // call original API through our Kernel32Support class
	//        IntPtr module_handle = Kernel32Support.LoadLibraryExW( lpFileName,  hFile,  dwFlags);

	//        transfer_unit["ModuleHandle"] = module_handle;

	//        HookRegistry.checkHooksToInstall();

	//        makeCallBack(transfer_unit);
	//        Console.WriteLine("End -----------------------LoadLibraryW(\"" + lpFileName + "\")=" + module_handle);
	//        return module_handle;
	//    }


	//}
}
