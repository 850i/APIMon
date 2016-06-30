using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.ws2_32.dll
{
    public class Hook_WSAConnect : AbstractHookDescription
    {

        //public override APIFullName api_full_name { get { return new APIFullName("ws2_32.dll", "WSAConnect"); } }

        protected override Delegate createHookDelegate()
        {
            return new WS2_32Support.DWSAConnect(WSAConnect_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public int WSAConnect_Hooked(IntPtr socket, IntPtr lpSockAddr, int namelen, IntPtr lpCallerData, IntPtr lpCalleeData, IntPtr lpSQOS, IntPtr lpGQOS)
        {
            preprocessHook();

            TransferUnit transfer_unit = createTransferUnit();
            transfer_unit[Color.Handle] = socket.ToInt32();

            // call original API...
            int result = WS2_32Support.WSAConnect( socket,  lpSockAddr,  namelen,  lpCallerData,  lpCalleeData,  lpSQOS,  lpGQOS);
            transfer_unit[Color.Result] = result;

			if (result != WS2_32Support.SOCKET_ERROR) makeCallBack(transfer_unit);

            return result;
        }

		public struct Color {
			public const string Handle = "SocketHandle";
			public const string Result = "result";
		}
    }
}
