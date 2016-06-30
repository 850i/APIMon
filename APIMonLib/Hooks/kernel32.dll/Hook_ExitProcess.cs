using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.kernel32.dll {
	public class Hook_ExitProcess :AbstractHookDescription{
		protected override Delegate createHookDelegate() {
			return new Kernel32Support.DExitProcess(ExitProcess_Hooked);
		}

		public void ExitProcess_Hooked(uint uExitCode) {
			preprocessHook();
			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit["uExitCode"] = uExitCode;
			makeCallBack(transfer_unit);
			Console.WriteLine("Delay ExitProcess");
			const int DELAY = 10;
			for (int i = 0; i < DELAY; i++) {
				Console.Write(" " + (DELAY - i));
			}
			// call original API...
			Kernel32Support.ExitProcess(uExitCode);
			//Console.Write(".");

			//if (result == Kernel32Support.STATUS_SUCCESS) {

			//}
		}
	}
}
