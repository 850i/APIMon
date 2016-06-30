using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ntdll.dll {
	public class Hook_ZwTerminateProcess : AbstractHookDescription {
		protected override Delegate createHookDelegate() {
			return new NtDllSupport.DZwTerminateProcess(ZwTerminateProcess_Hooked);
		}

		public UInt32 ZwTerminateProcess_Hooked(IntPtr ProcessHandle, UInt32 ExitStatus) {
			preprocessHook();

			// call original API...
			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit[Color.ProcessHandle] = ProcessHandle.ToInt32();
			transfer_unit[Color.ExitStatus] = ExitStatus;
			//transfer_unit[Color.Result] = result;
			makeCallBack(transfer_unit);
			Console.WriteLine("Delay ZwTerminateProcess");
			const int DELAY=10;
			for (int i = 0; i < DELAY; i++) {
				Console.Write(" "+(DELAY-i));
			}
			UInt32 result = NtDllSupport.ZwTerminateProcess(ProcessHandle, ExitStatus);
			return result;
		}

		public struct Color {
			public const string ProcessHandle = "ProcessHandle";
			public const string ExitStatus = "ExitStatus";
			//public const string Result = "Result";
		}
	}
}
