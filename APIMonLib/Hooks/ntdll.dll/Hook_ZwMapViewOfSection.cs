using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ntdll.dll {
    public class Hook_ZwMapViewOfSection :AbstractHookDescription{

        protected override Delegate createHookDelegate() {
            return new NtDllSupport.DZwMapViewOfSection(ZwMapViewOfSection_Hooked);
        }

        private UInt32 ZwMapViewOfSection_Hooked(IntPtr SectionHandle, IntPtr ProcessHandle, IntPtr BaseAddress, Int32 ZeroBits, Int32 CommitSize, IntPtr SectionOffset, IntPtr ViewSize, NtDllSupport.SECTION_INHERIT InheritDisposition, Int32 AllocationType, Int32 Win32Protect) {

            preprocessHook();

            UInt32 result = NtDllSupport.ZwMapViewOfSection( SectionHandle,  ProcessHandle,  BaseAddress,  ZeroBits,  CommitSize,  SectionOffset,  ViewSize,  InheritDisposition,  AllocationType,  Win32Protect);

            if (result == NtDllSupport.STATUS_SUCCESS) {

                TransferUnit transfer_unit = createTransferUnit();
                transfer_unit[Color.BaseAddress] = BaseAddress.ToInt32();
                transfer_unit[Color.SectionHandle] = SectionHandle.ToInt32();
                transfer_unit[Color.ProcessHandle] = ProcessHandle.ToInt32();

                makeCallBack(transfer_unit);

            }
            return result;
        }

		public struct Color {
			public const string BaseAddress="BaseAddress";
				public const string SectionHandle="SectionHandle";
					public const string ProcessHandle="ProcessHandle";
		}
    }
}
