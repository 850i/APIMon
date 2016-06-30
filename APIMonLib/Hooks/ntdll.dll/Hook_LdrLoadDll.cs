using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ntdll.dll
{
    public class Hook_LdrLoadDll : AbstractHookDescription {
        private static Object sync_object = new Object();
        protected override Delegate createHookDelegate() {
            return new NtDllSupport.DLdrLoadDll(LdrLoadDll_Hooked);
        }

        // this is where we are intercepting all file accesses!
        private uint LdrLoadDll_Hooked(IntPtr PathToFile, NtDllSupport.LoadLibraryFlags dwFlags, ref NtDllSupport.UNICODE_STRING ModuleFileName, ref IntPtr ModuleHandle) {
            lock (sync_object) {
                Console.WriteLine("Begin ---------------------LdrLoadDll(\"" + ModuleFileName + "\")");
                preprocessHook();

                TransferUnit transfer_unit = createTransferUnit();
                transfer_unit["PathToFile"] = PathToFile;
                transfer_unit["ModuleFileName"] = ModuleFileName.ToString();
                transfer_unit["dwFlags"] = dwFlags;

                // call original API through our Kernel32Support class
                uint result = NtDllSupport.LdrLoadDll(PathToFile, dwFlags, ref ModuleFileName, ref ModuleHandle);

                transfer_unit["ModuleHandle"] = ModuleHandle;

                HookRegistry.checkHooksToInstall();

                makeCallBack(transfer_unit);
                Console.WriteLine("End -----------------------LdrLoadDll(\"" + ModuleFileName + "\")=");
                return result;
            }
        }
    }
}
