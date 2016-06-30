using System;


namespace APIMonLib.Hooks.kernel32.dll {
	public class Hook_CreateProcessInternalW  : AbstractHookDescription {
		protected override Delegate createHookDelegate() {
			return new Kernel32Support.DCreateProcessInternalW(CreateProcessInternalW_Hooked);
		}

		private int CreateProcessInternalW_Hooked(Int32 unknown1, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, Kernel32Support.ProcessCreationFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, IntPtr lpStartupInfo, IntPtr lpProcessInformation, Int32 unknown2) {
			preprocessHook();

			IntPtr he, ho, hi, h_process, h_thread;
			Int32 dwProcessId, dwThreadId;

            // call original API through our Kernel32Support class
            if (Configuration.FOLLOW_PROCESS_TREE) dwCreationFlags |= Kernel32Support.ProcessCreationFlags.CREATE_SUSPENDED;
            int result = Kernel32Support.FALSE;
            try {
                result = Kernel32Support.CreateProcessInternalW(unknown1, lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes, bInheritHandles, dwCreationFlags, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation, unknown2);
            } catch {
                Console.WriteLine("Failed to launch subprocess");
            }

			unsafe {
				Kernel32Support.STARTUPINFO* lp_stratup_info = (Kernel32Support.STARTUPINFO*)lpStartupInfo.ToPointer();
				he = lp_stratup_info->hStdError;
				ho = lp_stratup_info->hStdOutput;
				hi = lp_stratup_info->hStdInput;
				Kernel32Support.PROCESS_INFORMATION* lp_process_info = (Kernel32Support.PROCESS_INFORMATION*)lpProcessInformation.ToPointer();
				h_process =		lp_process_info->hProcess;
				h_thread =		lp_process_info->hThread;
				dwProcessId =	lp_process_info->dwProcessId;
				dwThreadId =	lp_process_info->dwThreadId;
			}
			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit[Color.StdErrHandle] = he.ToInt32();
			transfer_unit[Color.StdOutHandle] = ho.ToInt32();
			transfer_unit[Color.StdInHandle] = hi.ToInt32();
            if ((lpApplicationName == null) && (lpCommandLine!=null))
            {
                string[] command_lines=APIMonLib.Hooks.shell32.dll.Shell32DllSupport.CommandLineToArgs(lpCommandLine);
                if (command_lines.Length > 0) lpApplicationName = System.IO.Path.GetFileName(command_lines[0]);
                else throw new Exception("Hook_CreateProcessInternalW Can not infer application name");
            }
			transfer_unit[Color.ApplicationName] = lpApplicationName;
			transfer_unit[Color.CommandLine] = lpCommandLine;
			transfer_unit[Color.ProcessHandle] = h_process.ToInt32();
			transfer_unit[Color.FirstThreadHandle] = h_thread.ToInt32();
			transfer_unit[Color.ProcessId] = dwProcessId;
			transfer_unit[Color.FirstThreadId] = dwThreadId;
			transfer_unit[Color.ProcessCreationFlags] = dwCreationFlags;

			//if (result!=Kernel32Support.FALSE) 
                makeCallBack(transfer_unit);

			return result;
		}

		public struct Color {
			public const string StdErrHandle = "std_ERR_Handle";
			public const string StdInHandle = "std_IN_Handle";
			public const string StdOutHandle = "std_OUT_Handle";
			public const string ApplicationName = "ApplicationName";
			public const string CommandLine = "CommandLine";
			public const string ProcessHandle = "ProcessHandle";
			public const string FirstThreadHandle = "FirstThreadHandle";
			public const string ProcessId = "ProcessId";
			public const string FirstThreadId = "FirstThreadId";
			public const string ProcessCreationFlags = "ProcessCreationFlags";				
		}
	}
}

//typedef struct _PROCESS_INFORMATION {
//  HANDLE hProcess;
//  HANDLE hThread;
//  DWORD  dwProcessId;
//  DWORD  dwThreadId;
//} PROCESS_INFORMATION, *LPPROCESS_INFORMATION;