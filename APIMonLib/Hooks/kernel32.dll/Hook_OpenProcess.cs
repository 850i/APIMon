using System;

namespace APIMonLib.Hooks.kernel32.dll
{
    public class Hook_OpenProcess : AbstractHookDescription
    {
        //public override APIFullName api_full_name { get { return new APIFullName("kernel32.dll", "OpenProcess"); } }

        protected override Delegate createHookDelegate()
        {
            return new Kernel32Support.DOpenProcess(OpenProcess_Hooked);
        }

        // this is where we are intercepting all file accesses!
        private IntPtr OpenProcess_Hooked(Kernel32Support.ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, uint dwProcessId)
        {
            preprocessHook();

            TransferUnit transfer_unit = createTransferUnit();
            transfer_unit["dwDesiredAccess"] = dwDesiredAccess;
            transfer_unit["bInheritHandle"] = bInheritHandle;
            transfer_unit["dwProcessId"] = dwProcessId;

            // call original API through our Kernel32Support class
            IntPtr handle = Kernel32Support.OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId);

            transfer_unit["handle"] = handle.ToInt32();

            if(handle.ToInt32()!=Kernel32Support.NULL)makeCallBack(transfer_unit);

            return handle;
        }

    }
}
