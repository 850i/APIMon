using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.kernel32.dll
{
    public class Hook_ConnectNamedPipe : AbstractHookDescription
    {
        //public override APIFullName api_full_name { get { return new APIFullName("kernel32.dll", "ConnectNamedPipe"); } }

        protected override Delegate createHookDelegate()
        {
            return new Kernel32Support.DConnectNamedPipe(ConnectNamedPipe_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public int ConnectNamedPipe_Hooked(IntPtr hNamedPipe, IntPtr lpOverlapped)
        {
            preprocessHook();

            TransferUnit transfer_unit = createTransferUnit();
            transfer_unit[Color.PipeHandle] = hNamedPipe.ToInt32();

            // call original API...
            int result = Kernel32Support.ConnectNamedPipe(hNamedPipe, lpOverlapped);

			if (result != Kernel32Support.FALSE) makeCallBack(transfer_unit);

            return result;
        }

		public struct Color {
			public const string PipeHandle = "PipeHandle";
		}
    }
}
