using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ntdll.dll {
    public class Hook_ZwCreateFile : AbstractHookDescription {


        protected override Delegate createHookDelegate() {
            return new NtDllSupport.DZwCreateFile(ZwCreateFile_Hooked);
        }

        // this is where we are intercepting all file accesses!
		private UInt32 ZwCreateFile_Hooked(	IntPtr ptr_to_FileHandle, 
											NtDllSupport.AccessRightsFlags DesiredAccess, 
											IntPtr ObjectAttributes, 
											IntPtr IoStatusBlock, 
											Int32 AllocationSize, Int32 FileAttributes, NtDllSupport.ShareAccessFlags ShareAccess, Int32 CreateDisposition, NtDllSupport.FileCreationFlags CreateOptions, IntPtr EaBuffer, Int32 EaLength) {

            preprocessHook();

			UInt32 result = NtDllSupport.ZwCreateFile(ptr_to_FileHandle, DesiredAccess, ObjectAttributes, IoStatusBlock, AllocationSize, FileAttributes, ShareAccess, CreateDisposition, CreateOptions, EaBuffer, EaLength);

            if (result == NtDllSupport.STATUS_SUCCESS) {
                string object_name = string.Empty;
				//object_name = "YOOOO"+random.Next();
				int file_handle=-1;
				unsafe {
					int* pfile_handle = (int*)ptr_to_FileHandle.ToPointer();
					file_handle = *pfile_handle;
					NtDllSupport.OBJECT_ATTRIBUTES* lpobj_attr = (NtDllSupport.OBJECT_ATTRIBUTES*)ObjectAttributes.ToPointer();
					NtDllSupport.UNICODE_STRING* pstrng = lpobj_attr->ObjectName;
					object_name = pstrng->ToString();
				}

                TransferUnit transfer_unit = createTransferUnit();
                transfer_unit[Color.ObjectName] = object_name;
				transfer_unit[Color.FileHandle] = file_handle;
				transfer_unit[Color.DesiredAccess] = DesiredAccess;
				transfer_unit[Color.ShareAccess] = ShareAccess;				
				transfer_unit[Color.FileCreationFlags] = CreateOptions;
                makeCallBack(transfer_unit);
            }
            return result;
        }
		public struct Color {
			public const string ObjectName = "ObjectName";
			public const string FileHandle = "FileHandle";
			public const string DesiredAccess = "DesiredAccess";
			public const string ShareAccess = "ShareAccess";
			public const string FileCreationFlags = "FileCreationFlags";
		}
    }
}
