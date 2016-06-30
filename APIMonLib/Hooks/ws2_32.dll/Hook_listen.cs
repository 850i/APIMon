using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib.Hooks.ws2_32.dll
{
    public class Hook_listen : AbstractHookDescription
    {
        //public override APIFullName api_full_name { get { return new APIFullName("ws2_32.dll", "listen"); } }

        protected override Delegate createHookDelegate()
        {
            return new WS2_32Support.Dlisten(listen_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public int listen_Hooked(IntPtr socket, int backlog)
        {
            preprocessHook();

            // call original API...
            int result = WS2_32Support.listen(socket, backlog);
			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit[Color.Handle] = socket.ToInt32();

			if (result != WS2_32Support.SOCKET_ERROR) makeCallBack(transfer_unit);

            return result;
        }

		public struct Color {
			public const string Handle = "SocketHandle";
		}
    }
}
