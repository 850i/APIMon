using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ntdll.dll {
    public class Hook_ZwOpenFile : AbstractHookDescription {
        protected override Delegate createHookDelegate() {
            return new NtDllSupport.DZwOpenFile(ZwOpenFile_Hooked);
        }

        // this is where we are intercepting all file accesses!
        private UInt32 ZwOpenFile_Hooked(IntPtr ptr_to_FileHandle, Int32 DesiredAccess, IntPtr ObjectAttributes, IntPtr IoStatusBlock, Int32 ShareAccess, NtDllSupport.FileCreationFlags OpenOptions) {

            preprocessHook();

            UInt32 result = NtDllSupport.ZwOpenFile(ptr_to_FileHandle, DesiredAccess, ObjectAttributes, IoStatusBlock, ShareAccess, OpenOptions);

            if (result == NtDllSupport.STATUS_SUCCESS) {
                string object_name = string.Empty;
				//object_name = "YOOOO" + random.Next();
				int file_handle = -1;
				unsafe {
					int* pfile_handle = (int*)ptr_to_FileHandle.ToPointer();
					file_handle = *pfile_handle;
					NtDllSupport.OBJECT_ATTRIBUTES* lpobj_attr = (NtDllSupport.OBJECT_ATTRIBUTES*)ObjectAttributes.ToPointer();
					NtDllSupport.UNICODE_STRING* pstrng = lpobj_attr->ObjectName;
					object_name = pstrng->ToString();
				}

                TransferUnit transfer_unit = createTransferUnit();
                transfer_unit["ObjectName"] = object_name;
				transfer_unit["FileHandle"] = file_handle;

                makeCallBack(transfer_unit);
            }
            return result;
        }
    }
}
