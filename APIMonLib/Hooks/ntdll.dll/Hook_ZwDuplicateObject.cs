using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace APIMonLib.Hooks.ntdll.dll {//LOCKS EASYHOOK on preprocessHook() stage
	//public class Hook_ZwDuplicateObject : AbstractHookDescription {
	//    protected override Delegate createHookDelegate() {
	//        return new NtDllSupport.DZwDuplicateObject(ZwDuplicateObject_Hooked);
	//    }

	//    private UInt32 ZwDuplicateObject_Hooked(IntPtr SourceProcessHandle, IntPtr SourceHandle, IntPtr TargetProcessHandle, IntPtr ptr_TargetHandle, Int32 DesiredAccess, Int32 InheritHandle, Int32 Options) {
	//        Console.WriteLine("Before ZwDuplicate called");
	//        //preprocessHook();

	//        Thread.Sleep(1000);

	//        UInt32 result = NtDllSupport.ZwDuplicateObject(SourceProcessHandle, SourceHandle, TargetProcessHandle, ptr_TargetHandle, DesiredAccess, InheritHandle, Options);
	//        Console.WriteLine("After ZwDuplicate called");
	//        //preprocessHook();
	//        //Console.WriteLine("After preprocessHook called");
	//        //if (result == NtDllSupport.STATUS_SUCCESS) {
	//        //    int target_handle = -1;
	//        //    unsafe {
	//        //        target_handle = *(int*)ptr_TargetHandle.ToPointer();
	//        //    }

	//        //    TransferUnit transfer_unit = createTransferUnit();
	//        //    transfer_unit[Color.SourceProcessHandle] = SourceProcessHandle.ToInt32();
	//        //    transfer_unit[Color.SourceHandle] = SourceHandle.ToInt32();
	//        //    transfer_unit[Color.TargetProcessHandle] = TargetProcessHandle.ToInt32();
	//        //    transfer_unit[Color.TargetHandle] = target_handle;
	//        //    transfer_unit[Color.DesiredAccess] = DesiredAccess;
	//        //    transfer_unit[Color.InheritHandle] = InheritHandle;
	//        //    transfer_unit[Color.Options] = Options;

	//        //    makeCallBack(transfer_unit);

	//        //}
	//        return result;
	//    }

	//    public struct Color {

	//        public const string SourceProcessHandle = "SourceProcessHandle";
	//        public const string SourceHandle = "SourceHandle";
	//        public const string TargetProcessHandle = "TargetProcessHandle";
	//        public const string TargetHandle = "TargetHandle";
	//        public const string DesiredAccess = "DesiredAccess";
	//        public const string InheritHandle = "InheritHandle";
	//        public const string Options = "Options";
	//    }
	//}
}
