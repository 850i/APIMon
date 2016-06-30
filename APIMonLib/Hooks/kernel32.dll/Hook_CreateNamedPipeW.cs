using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.kernel32.dll
{
    public class Hook_CreateNamedPipeW : AbstractHookDescription
    {
        protected override Delegate createHookDelegate()
        {
            return new Kernel32Support.DCreateNamedPipe(CreateNamedPipe_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public IntPtr CreateNamedPipe_Hooked(   string lpName, 
                                                Kernel32Support.PipeOpenModeFlags dwOpenMode, 
                                                Kernel32Support.PipeModeFlags dwPipeMode, 
            Int32 nMaxInstances, Int32 nOutBufferSize, 
            Int32 nInBufferSize, Int32 nDefaultTimeOut,
			IntPtr lpSecurityAttributes)
        {
            preprocessHook();

			Console.Write("Creating pipe " + lpName);
			// call original API...
            IntPtr result = Kernel32Support.CreateNamedPipe( lpName,  dwOpenMode,  dwPipeMode,  nMaxInstances,  nOutBufferSize,  nInBufferSize,  nDefaultTimeOut, lpSecurityAttributes);

			if (result.ToInt32() != Kernel32Support.INVALID_HANDLE_VALUE) {
				TransferUnit transfer_unit = createTransferUnit();
				transfer_unit[Color.PipeName] = lpName;
				transfer_unit[Color.PipeHandle] = result.ToInt32();
				makeCallBack(transfer_unit);
				Console.WriteLine("\tSUCCESS ");
			} else {
				Console.WriteLine("\tFAILURE ");
			}

            return result;
        }

		public struct Color {
			public const string PipeHandle = "PipeHandle";
			public const string PipeName = "name";
		}
    }
}
