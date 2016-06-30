using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ntdll.dll {
	public class Hook_LdrShutdownProcess : AbstractHookDescription {
		protected override Delegate createHookDelegate() {
			return new NtDllSupport.DLdrShutdownProcess(LdrShutdownProcess_Hooked);
		}

		public void LdrShutdownProcess_Hooked() {
			preprocessHook();
			Console.WriteLine("Delay LdrShutdownProcess");
			const int DELAY = 10;
			for (int i = 0; i < DELAY; i++) {
				Console.Write(" " + (DELAY - i));
			}
			// call original API...
			NtDllSupport.LdrShutdownProcess();
			//Console.Write(".");

			//if (result == NtDllSupport.STATUS_SUCCESS) {
			//TransferUnit transfer_unit = createTransferUnit();
			//makeCallBack(transfer_unit);
		}
	}
}
