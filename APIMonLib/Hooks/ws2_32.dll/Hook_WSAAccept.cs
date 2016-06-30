using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.ws2_32.dll
{
    public class Hook_WSAAccept : AbstractHookDescription
    {
        //public override APIFullName api_full_name { get { return new APIFullName("ws2_32.dll", "WSAAccept"); } }

        protected override Delegate createHookDelegate()
        {
            return new WS2_32Support.DWSAAccept(WSAAccept_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public IntPtr WSAAccept_Hooked(IntPtr socket, IntPtr lpSockAddr, IntPtr int_addrlen, IntPtr lpConditionProc, Int32 dwCallbackData)
        {
            preprocessHook();

            // call original API...
            IntPtr result = WS2_32Support.WSAAccept( socket,  lpSockAddr,  int_addrlen,  lpConditionProc,  dwCallbackData);

			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit[Color.ListeningSocketHandle] = socket.ToInt32();
            transfer_unit[Color.Handle] = result.ToInt32();

			if (result.ToInt32() != WS2_32Support.INVALID_SOCKET) makeCallBack(transfer_unit);

            return result;
        }
		public struct Color {
			public const string ListeningSocketHandle = "ListeningSocketHandle";
			public const string Handle = "SocketHandle";
		}
    }
}
