using System;

namespace APIMonLib.Hooks.ntdll.dll
{
    public class Hook_ZwClose : AbstractHookDescription
    {

        protected override Delegate createHookDelegate()
        {
            return new  NtDllSupport.DZwClose(ZwClose_Hooked);
        }

        public UInt32 ZwClose_Hooked(IntPtr handle)
        {
            preprocessHook();

            // call original API...
            UInt32 result = NtDllSupport.ZwClose(handle);
			//Console.Write(".");

            //if (result == NtDllSupport.STATUS_SUCCESS) {
                TransferUnit transfer_unit = createTransferUnit();
                transfer_unit["handle"] = handle.ToInt32();
                transfer_unit["ntStatus"] = result;
                makeCallBack(transfer_unit);
            //}
            return result;
        }
    }
}
