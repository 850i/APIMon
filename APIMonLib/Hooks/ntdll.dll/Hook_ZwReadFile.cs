using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ntdll.dll {
    public class Hook_ZwReadFile : AbstractHookDescription {
        protected override Delegate createHookDelegate() {
            return new NtDllSupport.DZwReadFile(ZwReadFile_Hooked);
        }

        private const int BUFFER_LIMIT = 100;

        // this is where we are intercepting all file accesses!
        private UInt32 ZwReadFile_Hooked(IntPtr FileHandle, IntPtr Event, IntPtr ApcRoutine, IntPtr ApcContext, IntPtr IoStatusBlock, IntPtr Buffer, Int32 Length, IntPtr ByteOffset, IntPtr Key) {

            preprocessHook();

            UInt32 result = NtDllSupport.ZwReadFile(  FileHandle,  Event,  ApcRoutine,  ApcContext,  IoStatusBlock,  Buffer,  Length,   ByteOffset,  Key);

            if (result == NtDllSupport.STATUS_SUCCESS) {
                int bytes_read = 0;
				unsafe {
					NtDllSupport.IO_STATUS_BLOCK* io_status_block = (NtDllSupport.IO_STATUS_BLOCK*)IoStatusBlock.ToPointer();
					bytes_read = io_status_block->Information;
				}
				string buffer = AbstractHookDescription.extractBufferAsString(Buffer, bytes_read > BUFFER_LIMIT ? BUFFER_LIMIT : bytes_read);

                TransferUnit transfer_unit = createTransferUnit();
                transfer_unit["FileHandle"] = FileHandle.ToInt32();
				transfer_unit["buffer"] = buffer;
                transfer_unit["BytesRead"] = bytes_read;

                makeCallBack(transfer_unit);
            }

            return result;
        }
    }
}
