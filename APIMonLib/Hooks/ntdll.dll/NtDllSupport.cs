using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace APIMonLib.Hooks.ntdll.dll {
	/// <summary>
	/// This class accumulates P-Invoke definitions for all needed API calls to ntdll.dll
	/// </summary>
	class NtDllSupport {
		// just use a P-Invoke implementation to get native API access from C# (this step is not necessary for C++.NET)
		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwClose(IntPtr handle);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwClose(IntPtr handle);

		[Flags]
		public enum LoadLibraryFlags : uint {
			DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
			LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
			LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
			LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
			LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
			LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
		}




		public const int STATUS_SUCCESS = 0x0;

		[StructLayout(LayoutKind.Sequential)]
		public struct UNICODE_STRING : IDisposable {
			public ushort Length;
			public ushort MaximumLength;
			private IntPtr buffer;

			public UNICODE_STRING(string s) {
				Length = (ushort)(s.Length * 2);
				MaximumLength = (ushort)(Length + 2);
				buffer = Marshal.StringToHGlobalUni(s);
			}

			public void Dispose() {
				Marshal.FreeHGlobal(buffer);
				buffer = IntPtr.Zero;
			}

			public override string ToString() {
				return Marshal.PtrToStringUni(buffer);
			}
		}


		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 LdrLoadDll(IntPtr PathToFile, NtDllSupport.LoadLibraryFlags dwFlags, ref NtDllSupport.UNICODE_STRING ModuleFileName, ref IntPtr ModuleHandle);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate UInt32 DLdrLoadDll(IntPtr PathToFile, NtDllSupport.LoadLibraryFlags dwFlags, ref NtDllSupport.UNICODE_STRING ModuleFileName, ref IntPtr ModuleHandle);

		[StructLayout(LayoutKind.Sequential)]
		public unsafe struct OBJECT_ATTRIBUTES {
			public Int32 Length;
			public IntPtr RootDirectory;
			public UNICODE_STRING* ObjectName;
			public int Attributes;
			public IntPtr SecurityDescriptor;
			public IntPtr SecurityQualityOfService;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct LARGE_INTEGER {
			[System.Runtime.InteropServices.FieldOffset(0)]
			public Int32 LowPart;
			[System.Runtime.InteropServices.FieldOffset(4)]
			public Int32 HighPart;
			[System.Runtime.InteropServices.FieldOffset(0)]
			public Int64 QuadPart;
		};

		[StructLayout(LayoutKind.Explicit)]
		public struct IO_STATUS_BLOCK {
			[System.Runtime.InteropServices.FieldOffset(0)]
			public Int32 Status;
			[System.Runtime.InteropServices.FieldOffset(0)]
			public Int32 Pointer;
			[System.Runtime.InteropServices.FieldOffset(4)]
			public Int32 Information;
		};



		//  The following are masks for the predefined standard access types
		[Flags]
		public enum StandardAccessTypes : uint {
			DELETE =		0x00010000,
			READ_CONTROL =	0x00020000,
			WRITE_DAC =		0x00040000,
			WRITE_OWNER =	0x00080000,
			SYNCHRONIZE =	0x00100000,
			//
			// AccessSystemAcl access type
			//
			ACCESS_SYSTEM_SECURITY = 0x01000000,
			//
			// MaximumAllowed access type
			//
			MAXIMUM_ALLOWED=0x02000000
		}

		[Flags]
		public enum StandardRights : uint {
			STANDARD_RIGHTS_REQUIRED = 0x000F0000,
			STANDARD_RIGHTS_READ = StandardAccessTypes.READ_CONTROL,
			STANDARD_RIGHTS_WRITE = StandardAccessTypes.READ_CONTROL,
			STANDARD_RIGHTS_EXECUTE = StandardAccessTypes.READ_CONTROL,
			STANDARD_RIGHTS_ALL = 0x001F0000,
			SPECIFIC_RIGHTS_ALL = 0x0000FFFF
		}

		[Flags]
		public enum GenericRights : uint {
		GENERIC_READ=0x80000000,
		GENERIC_WRITE=0x40000000,
		GENERIC_EXECUTE=0x20000000,
		GENERIC_ALL=0x10000000
		}

		[Flags]
		public enum AccessRightsFlags : uint {
			DELETE = 0x00010000,
			READ_CONTROL = 0x00020000,
			WRITE_DAC = 0x00040000,
			WRITE_OWNER = 0x00080000,
			SYNCHRONIZE = 0x00100000,
			//
			// AccessSystemAcl access type
			//
			ACCESS_SYSTEM_SECURITY = 0x01000000,
			//
			// MaximumAllowed access type
			//
			MAXIMUM_ALLOWED = 0x02000000,

			STANDARD_RIGHTS_REQUIRED = 0x000F0000,
			STANDARD_RIGHTS_READ = StandardAccessTypes.READ_CONTROL,
			STANDARD_RIGHTS_WRITE = StandardAccessTypes.READ_CONTROL,
			STANDARD_RIGHTS_EXECUTE = StandardAccessTypes.READ_CONTROL,
			STANDARD_RIGHTS_ALL = 0x001F0000,
			SPECIFIC_RIGHTS_ALL = 0x0000FFFF,

			GENERIC_READ = 0x80000000,
			GENERIC_WRITE = 0x40000000,
			GENERIC_EXECUTE = 0x20000000,
			GENERIC_ALL = 0x10000000,

			FILE_READ_DATA = 0x0001,    // file & pipe
			FILE_LIST_DIRECTORY = 0x0001,    // directory

			FILE_WRITE_DATA = 0x0002,    // file & pipe
			FILE_ADD_FILE = 0x0002,    // directory

			FILE_APPEND_DATA = 0x0004,    // file
			FILE_ADD_SUBDIRECTORY = 0x0004,    // directory
			FILE_CREATE_PIPE_INSTANCE = 0x0004,    // named pipe

			FILE_READ_EA = 0x0008,    // file & directory
			FILE_WRITE_EA = 0x0010,    // file & directory
			FILE_EXECUTE = 0x0020,    // file
			FILE_TRAVERSE = 0x0020,    // directory
			FILE_DELETE_CHILD = 0x0040,    // directory
			FILE_READ_ATTRIBUTES = 0x0080,    // all
			FILE_WRITE_ATTRIBUTES = 0x0100,    // all

			FILE_ALL_ACCESS = StandardRights.STANDARD_RIGHTS_REQUIRED | StandardAccessTypes.SYNCHRONIZE | 0x1FF,

			FILE_GENERIC_READ = StandardRights.STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | StandardAccessTypes.SYNCHRONIZE,

			FILE_GENERIC_WRITE = StandardRights.STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | StandardAccessTypes.SYNCHRONIZE,

			FILE_GENERIC_EXECUTE = StandardRights.STANDARD_RIGHTS_EXECUTE | FILE_READ_ATTRIBUTES | FILE_EXECUTE | StandardAccessTypes.SYNCHRONIZE
		}

		[Flags]
		public enum FileCreationFlags : uint {
			FILE_DIRECTORY_FILE = 0x00000001,
			FILE_WRITE_THROUGH = 0x00000002,
			FILE_SEQUENTIAL_ONLY = 0x00000004,
			FILE_NO_INTERMEDIATE_BUFFERING = 0x00000008,

			FILE_SYNCHRONOUS_IO_ALERT = 0x00000010,
			FILE_SYNCHRONOUS_IO_NONALERT = 0x00000020,
			FILE_NON_DIRECTORY_FILE = 0x00000040,
			FILE_CREATE_TREE_CONNECTION = 0x00000080,

			FILE_COMPLETE_IF_OPLOCKED = 0x00000100,
			FILE_NO_EA_KNOWLEDGE = 0x00000200,

			FILE_RANDOM_ACCESS = 0x00000800,

			FILE_DELETE_ON_CLOSE = 0x00001000,
			FILE_OPEN_BY_FILE_ID = 0x00002000,
			FILE_OPEN_FOR_BACKUP_INTENT = 0x00004000,
			FILE_NO_COMPRESSION = 0x00008000,

			FILE_RESERVE_OPFILTER = 0x00100000,
			FILE_TRANSACTED_MODE = 0x00200000,
			FILE_OPEN_OFFLINE_FILE = 0x00400000,

			FILE_VALID_OPTION_FLAGS = 0x007fffff,
			FILE_VALID_PIPE_OPTION_FLAGS = 0x00000032,
			FILE_VALID_MAILSLOT_OPTION_FLAGS = 0x00000032,
			FILE_VALID_SET_FLAGS = 0x00001036
		}

		[Flags]
		public enum ShareAccessFlags : uint {
			FILE_SHARE_READ = 0x00000001,
			FILE_SHARE_WRITE = 0x00000002,
			FILE_SHARE_DELETE = 0x00000004
		}


		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwCreateFile(IntPtr ptr_to_FileHandle, AccessRightsFlags DesiredAccess, IntPtr ObjectAttributes, IntPtr IoStatusBlock, Int32 AllocationSize, Int32 FileAttributes, ShareAccessFlags ShareAccess, Int32 CreateDisposition, FileCreationFlags CreateOptions, IntPtr EaBuffer, Int32 EaLength);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwCreateFile(IntPtr ptr_to_FileHandle, AccessRightsFlags DesiredAccess, IntPtr ObjectAttributes, IntPtr IoStatusBlock, Int32 AllocationSize, Int32 FileAttributes, ShareAccessFlags ShareAccess, Int32 CreateDisposition, FileCreationFlags CreateOptions, IntPtr EaBuffer, Int32 EaLength);

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwOpenFile(IntPtr ptr_to_FileHandle, Int32 DesiredAccess, IntPtr ObjectAttributes, IntPtr IoStatusBlock, Int32 ShareAccess, FileCreationFlags OpenOptions);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwOpenFile(IntPtr ptr_to_FileHandle, Int32 DesiredAccess, IntPtr ObjectAttributes, IntPtr IoStatusBlock, Int32 ShareAccess, FileCreationFlags OpenOptions);

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwReadFile(IntPtr FileHandle, IntPtr Event, IntPtr ApcRoutine, IntPtr ApcContext, IntPtr IoStatusBlock, IntPtr Buffer, Int32 Length, IntPtr ByteOffset, IntPtr Key);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwReadFile(IntPtr FileHandle, IntPtr Event, IntPtr ApcRoutine, IntPtr ApcContext, IntPtr IoStatusBlock, IntPtr Buffer, Int32 Length, IntPtr ByteOffset, IntPtr Key);

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwWriteFile(IntPtr FileHandle, IntPtr Event, IntPtr ApcRoutine, IntPtr ApcContext, IntPtr IoStatusBlock, IntPtr Buffer, Int32 Length, IntPtr ByteOffset, IntPtr Key);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwWriteFile(IntPtr FileHandle, IntPtr Event, IntPtr ApcRoutine, IntPtr ApcContext, IntPtr IoStatusBlock, IntPtr Buffer, Int32 Length, IntPtr ByteOffset, IntPtr Key);

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwCreateSection(IntPtr SectionHandle, Int32 DesiredAccess, IntPtr ObjectAttributes, IntPtr MaximumSize, Int32 SectionPageProtection, Int32 AllocationAttributes, IntPtr FileHandle);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwCreateSection(IntPtr SectionHandle, Int32 DesiredAccess, IntPtr ObjectAttributes, IntPtr MaximumSize, Int32 SectionPageProtection, Int32 AllocationAttributes, IntPtr FileHandle);

		public enum SECTION_INHERIT : uint {
			ViewShare = 0x00000001,
			ViewUnmap = 0x00000002
		}

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwMapViewOfSection(IntPtr SectionHandle, IntPtr ProcessHandle, IntPtr BaseAddress, Int32 ZeroBits, Int32 CommitSize, IntPtr SectionOffset, IntPtr ViewSize, SECTION_INHERIT InheritDisposition, Int32 AllocationType, Int32 Win32Protect);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwMapViewOfSection(IntPtr SectionHandle, IntPtr ProcessHandle, IntPtr BaseAddress, Int32 ZeroBits, Int32 CommitSize, IntPtr SectionOffset, IntPtr ViewSize, SECTION_INHERIT InheritDisposition, Int32 AllocationType, Int32 Win32Protect);

//IN HANDLE               SourceProcessHandle,
//  IN PHANDLE              SourceHandle,
//  IN HANDLE               TargetProcessHandle,
//  OUT PHANDLE             TargetHandle,
//  IN ACCESS_MASK          DesiredAccess OPTIONAL,
//  IN BOOLEAN              InheritHandle,
//  IN ULONG                Options



		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwDuplicateObject(IntPtr SourceProcessHandle, IntPtr SourceHandle,IntPtr TargetProcessHandle, IntPtr ptr_TargetHandle, Int32 DesiredAccess, Int32 InheritHandle, Int32 Options);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwDuplicateObject(IntPtr SourceProcessHandle, IntPtr SourceHandle,IntPtr TargetProcessHandle, IntPtr ptr_TargetHandle, Int32 DesiredAccess, Int32 InheritHandle, Int32 Options);

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern UInt32 ZwTerminateProcess(IntPtr ProcessHandle, UInt32 ExitStatus);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate UInt32 DZwTerminateProcess(IntPtr ProcessHandle, UInt32 ExitStatus);

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern void LdrShutdownProcess();
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public delegate void DLdrShutdownProcess();
	}
}
