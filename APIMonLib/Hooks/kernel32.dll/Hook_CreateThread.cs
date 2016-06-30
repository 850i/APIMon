using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.kernel32.dll
{
	//public class Hook_CreateThread : AbstractHookDescription
	//{

	//    //public override APIFullName api_full_name { get { return new APIFullName("kernel32.dll", "CreateThread"); } }

	//    protected override Delegate createHookDelegate()
	//    {
	//        return new Kernel32Support.DCreateThread(CreateThread_Hooked);
	//    }

	//    // this is where we are intercepting all file accesses!
	//    private IntPtr CreateThread_Hooked(ref Kernel32Support.SECURITY_ATTRIBUTES securityAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint threadId)
	//    {
	//        preprocessHook();

	//        TransferUnit transfer_unit = createTransferUnit();
	//        try
	//        {
	//            transfer_unit["securityAttributes"] = securityAttributes;
	//        }
	//        catch (NullReferenceException)
	//        {
	//            //do nothing
	//        }
	//        transfer_unit["dwStackSize"] = dwStackSize;
	//        transfer_unit["lpStartAddress"] = lpStartAddress;
	//        transfer_unit["lpParameter"] = lpParameter;
	//        transfer_unit["dwCreationFlags"] = dwCreationFlags;

	//        // call original API through our Kernel32Support class
	//        IntPtr thread_handle = Kernel32Support.CreateThread( ref securityAttributes,  dwStackSize,  lpStartAddress,  lpParameter,  dwCreationFlags, out threadId);

	//        transfer_unit["lpThreadId"] = threadId;

	//        transfer_unit["handle"] = thread_handle.ToInt32();

	//        makeCallBack(transfer_unit);

	//        return thread_handle;
	//    }
	//}
}
