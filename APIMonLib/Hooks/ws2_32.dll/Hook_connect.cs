using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.ws2_32.dll
{
    public class Hook_connect : AbstractHookDescription
    {
        //public override APIFullName api_full_name { get { return new APIFullName("ws2_32.dll", "connect"); } }

        protected override Delegate createHookDelegate()
        {
            return new WS2_32Support.Dconnect(connect_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public int connect_Hooked(IntPtr socket, IntPtr lpSockAddr, int namelen)
        {
            preprocessHook();

            TransferUnit transfer_unit = createTransferUnit();
            transfer_unit[Color.Handle] = socket.ToInt32();

            // call original API...
            int result = WS2_32Support.connect( socket,  lpSockAddr,  namelen);
            transfer_unit[Color.Result] = result;

			//Discovered in opera. Connect returns -1. But opera sends data anyway through this socket.
			//So we disable error checking here
			//if (result != WS2_32Support.SOCKET_ERROR) 
				makeCallBack(transfer_unit);
			//else {
			//    int error = WS2_32Support.WSAGetLastError();

			//    WS2_32Support.WSASetLastError(error);
			//}

            return result;
        }
		public struct Color {
			public const string Handle = "SocketHandle";
			public const string Result = "result";
		}
    }
}
