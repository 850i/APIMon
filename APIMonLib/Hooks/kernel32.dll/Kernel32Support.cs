using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace APIMonLib.Hooks.kernel32.dll {
	/// <summary>
	/// This class accumulates P-Invoke definitions for all needed API calls to kernel32.dll
	/// </summary>
	public class Kernel32Support {

		public const int INVALID_HANDLE_VALUE = -1;
		public const int NULL = 0;
		public const int FALSE = 0;

		[Flags]
		public enum EFileAccess : uint {
			GenericRead = 0x80000000,
			GenericWrite = 0x40000000,
			GenericExecute = 0x20000000,
			GenericAll = 0x10000000
		}

		[Flags]
		public enum EFileShare : uint {
			None = 0x00000000,
			/// <summary>
			/// Enables subsequent open operations on an object to request read access. 
			/// Otherwise, other processes cannot open the object if they request read access. 
			/// If this flag is not specified, but the object has been opened for read access, the function fails.
			/// </summary>
			Read = 0x00000001,
			/// <summary>
			/// Enables subsequent open operations on an object to request write access. 
			/// Otherwise, other processes cannot open the object if they request write access. 
			/// If this flag is not specified, but the object has been opened for write access, the function fails.
			/// </summary>
			Write = 0x00000002,
			/// <summary>
			/// Enables subsequent open operations on an object to request delete access. 
			/// Otherwise, other processes cannot open the object if they request delete access.
			/// If this flag is not specified, but the object has been opened for delete access, the function fails.
			/// </summary>
			Delete = 0x00000004
		}

		public enum ECreationDisposition : uint {
			/// <summary>
			/// Creates a new file. The function fails if a specified file exists.
			/// </summary>
			New = 1,
			/// <summary>
			/// Creates a new file, always. 
			/// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes, 
			/// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
			/// </summary>
			CreateAlways = 2,
			/// <summary>
			/// Opens a file. The function fails if the file does not exist. 
			/// </summary>
			OpenExisting = 3,
			/// <summary>
			/// Opens a file, always. 
			/// If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
			/// </summary>
			OpenAlways = 4,
			/// <summary>
			/// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
			/// The calling process must open the file with the GENERIC_WRITE access right. 
			/// </summary>
			TruncateExisting = 5
		}

		[Flags]
		public enum EFileAttributes : uint {
			Readonly = 0x00000001,
			Hidden = 0x00000002,
			System = 0x00000004,
			Directory = 0x00000010,
			Archive = 0x00000020,
			Device = 0x00000040,
			Normal = 0x00000080,
			Temporary = 0x00000100,
			SparseFile = 0x00000200,
			ReparsePoint = 0x00000400,
			Compressed = 0x00000800,
			Offline = 0x00001000,
			NotContentIndexed = 0x00002000,
			Encrypted = 0x00004000,
			Write_Through = 0x80000000,
			Overlapped = 0x40000000,
			NoBuffering = 0x20000000,
			RandomAccess = 0x10000000,
			SequentialScan = 0x08000000,
			DeleteOnClose = 0x04000000,
			BackupSemantics = 0x02000000,
			PosixSemantics = 0x01000000,
			OpenReparsePoint = 0x00200000,
			OpenNoRecall = 0x00100000,
			FirstPipeInstance = 0x00080000
		}

		[Flags]
		public enum ProcessAccessFlags : uint {
			All = 0x001F0FFF,
			Terminate = 0x00000001,
			CreateThread = 0x00000002,
			VMOperation = 0x00000008,
			VMRead = 0x00000010,
			VMWrite = 0x00000020,
			DupHandle = 0x00000040,
			SetInformation = 0x00000200,
			QueryInformation = 0x00000400,
			Synchronize = 0x00100000
		}

		[Flags]
		public enum ProcessCreationFlags : int {
			/// <summary>
			/// The child processes of a process associated with a job are not associated with the job. 
			/// If the calling process is not associated with a job, this constant has no effect. If the calling process is associated with a job, the job must set the JOB_OBJECT_LIMIT_BREAKAWAY_OK limit.
			/// </summary>
			CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
			/// <summary>
			/// The new process does not inherit the error mode of the calling process. Instead, the new process gets the default error mode. 
			/// This feature is particularly useful for multi-threaded shell applications that run with hard errors disabled.
			/// The default behavior is for the new process to inherit the error mode of the caller. Setting this flag changes that default behavior.
			/// </summary>
			CREATE_DEFAULT_ERROR_MODE = 0x04000000,
			/// <summary>
			/// The new process has a new console, instead of inheriting its parent's console (the default). For more information, see Creation of a Console. 
			/// This flag cannot be used with DETACHED_PROCESS.
			/// </summary>
			CREATE_NEW_CONSOLE = 0x00000010,
			/// <summary>
			/// The new process is the root process of a new process group. The process group includes all processes that are descendants of this root process. The process identifier of the new process group is the same as the process identifier, which is returned in the lpProcessInformation parameter. Process groups are used by the GenerateConsoleCtrlEvent function to enable sending a CTRL+BREAK signal to a group of console processes.
			/// If this flag is specified, CTRL+C signals will be disabled for all processes within the new process group.
			/// This flag is ignored if specified with CREATE_NEW_CONSOLE.
			/// </summary>
			CREATE_NEW_PROCESS_GROUP = 0x00000200,

			/// <summary>
			/// The process is a console application that is being run without a console window. Therefore, the console handle for the application is not set.
			/// This flag is ignored if the application is not a console application, or if it is used with either CREATE_NEW_CONSOLE or DETACHED_PROCESS.
			/// </summary>
			CREATE_NO_WINDOW = 0x08000000,
			/// <summary>
			/// The process is to be run as a protected process. The system restricts access to protected processes and the threads of protected processes. For more information on how processes can interact with protected processes, see Process Security and Access Rights.
			/// To activate a protected process, the binary must have a special signature. This signature is provided by Microsoft but not currently available for non-Microsoft binaries. There are currently four protected processes: media foundation, audio engine, Windows error reporting, and system. Components that load into these binaries must also be signed. Multimedia companies can leverage the first two protected processes. For more information, see Overview of the Protected Media Path.
			/// Windows Server 2003 and Windows XP/2000:  This value is not supported.
			/// </summary>
			CREATE_PROTECTED_PROCESS = 0x00040000,
			/// <summary>
			/// Allows the caller to execute a child process that bypasses the process restrictions that would normally be applied automatically to the process.
			/// Windows 2000:  This value is not supported.
			/// </summary>
			CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
			/// <summary>
			/// This flag is valid only when starting a 16-bit Windows-based application. If set, the new process runs in a private Virtual DOS Machine (VDM). By default, all 16-bit Windows-based applications run as threads in a single, shared VDM. The advantage of running separately is that a crash only terminates the single VDM; any other programs running in distinct VDMs continue to function normally. Also, 16-bit Windows-based applications that are run in separate VDMs have separate input queues. That means that if one application stops responding momentarily, applications in separate VDMs continue to receive input. The disadvantage of running separately is that it takes significantly more memory to do so. You should use this flag only if the user requests that 16-bit applications should run in their own VDM.
			/// </summary>
			CREATE_SEPARATE_WOW_VDM = 0x00000800,
			/// <summary>
			/// The flag is valid only when starting a 16-bit Windows-based application. If the DefaultSeparateVDM switch in the Windows section of WIN.INI is TRUE, this flag overrides the switch. The new process is run in the shared Virtual DOS Machine.
			/// </summary>
			CREATE_SHARED_WOW_VDM = 0x00001000,
			/// <summary>
			/// The primary thread of the new process is created in a suspended state, and does not run until the ResumeThread function is called.
			/// </summary>
			CREATE_SUSPENDED = 0x00000004,
			/// <summary>
			/// If this flag is set, the environment block pointed to by lpEnvironment uses Unicode characters. Otherwise, the environment block uses ANSI characters.
			/// </summary>
			CREATE_UNICODE_ENVIRONMENT = 0x00000400,
			/// <summary>
			/// The calling thread starts and debugs the new process. It can receive all related debug events using the WaitForDebugEvent function.
			/// </summary>
			DEBUG_ONLY_THIS_PROCESS = 0x00000002,
			/// <summary>
			/// The calling thread starts and debugs the new process and all child processes created by the new process. It can receive all related debug events using the WaitForDebugEvent function. 
			/// A process that uses DEBUG_PROCESS becomes the root of a debugging chain. This continues until another process in the chain is created with DEBUG_PROCESS.
			/// If this flag is combined with DEBUG_ONLY_THIS_PROCESS, the caller debugs only the new process, not any child processes.
			/// </summary>
			DEBUG_PROCESS = 0x00000001,
			/// <summary>
			/// For console processes, the new process does not inherit its parent's console (the default). The new process can call the AllocConsole function at a later time to create a console. For more information, see Creation of a Console. 
			/// This value cannot be used with CREATE_NEW_CONSOLE.
			/// </summary>
			DETACHED_PROCESS = 0x00000008,
			/// <summary>
			/// The process is created with extended startup information; the lpStartupInfo parameter specifies a STARTUPINFOEX structure.
			/// Windows Server 2003 and Windows XP/2000:  This value is not supported.
			/// </summary>
			EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
			/// <summary>
			/// //The process inherits its parent's affinity. If the parent process has threads in more than one processor group, the new process inherits the group-relative affinity of an arbitrary group in use by the parent.
			/// Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP/2000:  This value is not supported.
			/// </summary>
			INHERIT_PARENT_AFFINITY = 0x00010000,

		}


		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate IntPtr DOpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

		[StructLayout(LayoutKind.Sequential)]
		public unsafe struct STARTUPINFO {
			public Int32 cb;
			public IntPtr lpReserved;
			public IntPtr lpDesktop;
			public IntPtr lpTitle;
			public Int32 dwX;
			public Int32 dwY;
			public Int32 dwXSize;
			public Int32 dwYSize;
			public Int32 dwXCountChars;
			public Int32 dwYCountChars;
			public Int32 dwFillAttribute;
			public Int32 dwFlags;
			public Int16 wShowWindow;
			public Int16 cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		};


		[StructLayout(LayoutKind.Sequential)]
		public unsafe struct PROCESS_INFORMATION {
			public IntPtr hProcess;
			public IntPtr hThread;
			public Int32 dwProcessId;
			public Int32 dwThreadId;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct SECURITY_ATTRIBUTES {
			public int nLength;
			public IntPtr lpSecurityDescriptor;
			public int bInheritHandle;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern int CreateProcessInternalW(Int32 unknown1, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, ProcessCreationFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, IntPtr lpStartupInfo, IntPtr lpProcessInformation, Int32 unknown2);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate int DCreateProcessInternalW(Int32 unknown1, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, ProcessCreationFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, IntPtr lpStartupInfo, IntPtr lpProcessInformation, Int32 unknown2);


		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate IntPtr DCreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);



		//[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		//public static extern IntPtr CreateThread([In] ref SECURITY_ATTRIBUTES SecurityAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint threadId);
		//[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		//public delegate IntPtr DCreateThread([In] ref SECURITY_ATTRIBUTES SecurityAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint threadId);


		//intercepting VirtualAllocEx leads to deadlock somwhere inside EasyHook
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate IntPtr DVirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		[Flags]
		public enum PipeOpenModeFlags : uint {
			PIPE_ACCESS_DUPLEX = 0x00000003,
			PIPE_ACCESS_INBOUND = 0x00000001,
			PIPE_ACCESS_OUTBOUND = 0x00000002,
			FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000,
			FILE_FLAG_WRITE_THROUGH = 0x80000000,
			FILE_FLAG_OVERLAPPED = 0x40000000,
			WRITE_DAC = 0x00040000,
			WRITE_OWNER = 0x00080000,
			ACCESS_SYSTEM_SECURITY = 0x01000000
		}

		[Flags]
		public enum PipeModeFlags : uint {
			//One of the following type modes can be specified. The same type mode must be specified for each instance of the pipe.
			PIPE_TYPE_BYTE = 0x00000000,
			PIPE_TYPE_MESSAGE = 0x00000004,
			//One of the following read modes can be specified. Different instances of the same pipe can specify different read modes
			PIPE_READMODE_BYTE = 0x00000000,
			PIPE_READMODE_MESSAGE = 0x00000002,
			//One of the following wait modes can be specified. Different instances of the same pipe can specify different wait modes.
			PIPE_WAIT = 0x00000000,
			PIPE_NOWAIT = 0x00000001,
			//One of the following remote-client modes can be specified. Different instances of the same pipe can specify different remote-client modes.
			PIPE_ACCEPT_REMOTE_CLIENTS = 0x00000000,
			PIPE_REJECT_REMOTE_CLIENTS = 0x00000008
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr CreateNamedPipe(string lpName, PipeOpenModeFlags dwOpenMode, PipeModeFlags dwPipeMode, Int32 nMaxInstances, Int32 nOutBufferSize, Int32 nInBufferSize, Int32 nDefaultTimeOut, IntPtr lpSecurityAttributes);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate IntPtr DCreateNamedPipe(string lpName, PipeOpenModeFlags dwOpenMode, PipeModeFlags dwPipeMode, Int32 nMaxInstances, Int32 nOutBufferSize, Int32 nInBufferSize, Int32 nDefaultTimeOut, IntPtr lpSecurityAttributes);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern int ConnectNamedPipe(IntPtr hNamedPipe, IntPtr lpOverlapped);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate int DConnectNamedPipe(IntPtr hNamedPipe, IntPtr lpOverlapped);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr LoadLibraryW(string lpFileName);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate IntPtr DLoadLibraryW(string lpFileName);

		[Flags]
		public enum LoadLibraryFlags : uint {
			DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
			LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
			LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
			LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
			LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
			LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
		}

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr LoadLibraryExW(string lpFileName, IntPtr hFile, LoadLibraryFlags dwFlags);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate IntPtr DLoadLibraryExW(string lpFileName, IntPtr hFile, LoadLibraryFlags dwFlags);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern void ExitProcess(uint uExitCode);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate void DExitProcess(uint uExitCode);
	}
}
