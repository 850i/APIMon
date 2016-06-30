using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.kernel32.dll
{
	//public class Hook_CreateRemoteThread : AbstractHookDescription
	//{

	//    //public override APIFullName api_full_name { get { return new APIFullName("kernel32.dll", "CreateRemoteThread"); } }

	//    protected override Delegate createHookDelegate()
	//    {
	//        return new Kernel32Support.DCreateRemoteThread(CreateRemoteThread_Hooked);
	//    }

	//    // this is where we are intercepting all file accesses!
	//    private IntPtr CreateRemoteThread_Hooked(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId)
	//    {
	//        preprocessHook();

	//        TransferUnit transfer_unit = createTransferUnit();
	//        transfer_unit["hProcess"] = hProcess.ToInt32();
	//        transfer_unit["lpThreadAttributes"] = lpThreadAttributes;
	//        transfer_unit["dwStackSize"] = dwStackSize;
	//        transfer_unit["lpStartAddress"] = lpStartAddress;
	//        transfer_unit["lpParameter"] = lpParameter;
	//        transfer_unit["dwCreationFlags"] = dwCreationFlags;
	//        transfer_unit["lpThreadId"] = lpThreadId;

	//        // call original API through our Kernel32Support class
	//        IntPtr thread_handle = Kernel32Support.CreateRemoteThread(hProcess, lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId);

	//        transfer_unit["handle"] = thread_handle.ToInt32();

	//        makeCallBack(transfer_unit);

	//        return thread_handle;
	//    }
	//}
}
