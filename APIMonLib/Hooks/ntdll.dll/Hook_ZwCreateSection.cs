using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ntdll.dll {
    public class Hook_ZwCreateSection : AbstractHookDescription {
        protected override Delegate createHookDelegate() {
            return new NtDllSupport.DZwCreateSection(ZwCreateSection_Hooked);
        }

        private UInt32 ZwCreateSection_Hooked(IntPtr ptr_SectionHandle, Int32 DesiredAccess, IntPtr ObjectAttributes, IntPtr MaximumSize, Int32 SectionPageProtection, Int32 AllocationAttributes, IntPtr FileHandle) {

            preprocessHook();

            UInt32 result = NtDllSupport.ZwCreateSection(ptr_SectionHandle,  DesiredAccess,  ObjectAttributes,  MaximumSize,  SectionPageProtection,  AllocationAttributes,  FileHandle);

            if (result == NtDllSupport.STATUS_SUCCESS) {
                string object_name = string.Empty;
				//object_name = "YOOOO" + random.Next();
				int section_handle = -1;
				unsafe {
					section_handle = *(int*)ptr_SectionHandle.ToPointer();
					NtDllSupport.OBJECT_ATTRIBUTES* lpobj_attr = (NtDllSupport.OBJECT_ATTRIBUTES*)ObjectAttributes.ToPointer();
					if (lpobj_attr != null) {
						NtDllSupport.UNICODE_STRING* pstrng = lpobj_attr->ObjectName;
						object_name = pstrng->ToString();
					}
				}

                TransferUnit transfer_unit = createTransferUnit();
                transfer_unit[Color.ObjectName] = object_name;
				transfer_unit[Color.SectionHandle] = section_handle;
                transfer_unit[Color.FileHandle] = FileHandle.ToInt32();

                makeCallBack(transfer_unit);
            }
            return result;
        }

		public struct Color {
			public const string ObjectName="ObjectName";
				public const string SectionHandle="SectionHandle";
				public const string FileHandle = "FileHandle";
		}

    }
}
