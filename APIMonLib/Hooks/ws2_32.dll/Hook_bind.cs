using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.ws2_32.dll
{
    public class Hook_bind : AbstractHookDescription
    {
        //public override APIFullName api_full_name { get { return new APIFullName("ws2_32.dll", "bind"); } }

        protected override Delegate createHookDelegate()
        {
            return new WS2_32Support.Dbind(bind_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public int bind_Hooked(IntPtr socket, IntPtr lpSockAddr, int namelen)
        {
            preprocessHook();

            // call original API...
            int result = WS2_32Support.bind(socket, lpSockAddr, namelen);

			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit[Color.Handle] = socket.ToInt32();

            if(result!=WS2_32Support.SOCKET_ERROR) makeCallBack(transfer_unit);

            return result;
        }
		public struct Color {
			public const string Handle = "SocketHandle";
		}
    }
}
