using System;
using System.Collections.Generic;
using System.Text;
using APIMonLib.Hooks.kernel32.dll;

namespace APIMonLib.Hooks.ws2_32.dll
{
    public class Hook_WSASocketW : AbstractHookDescription
    {
        //public override APIFullName api_full_name { get { return new APIFullName("ws2_32.dll", "WSASocketW"); } }

        protected override Delegate createHookDelegate()
        {
            return new WS2_32Support.DWSASocketW(WSASocket_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public IntPtr WSASocket_Hooked(WS2_32Support.ADDRESS_FAMILIES af, WS2_32Support.SOCKET_TYPE socket_type, WS2_32Support.PROTOCOL protocol,
            IntPtr lpProtocolInfo, Int32 group, WS2_32Support.OPTION_FLAGS_PER_SOCKET dwFlags)
        {
            preprocessHook();

			// call original API...
            IntPtr socket_handle = WS2_32Support.WSASocketW(af, socket_type, protocol, lpProtocolInfo, group, dwFlags);

			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit[Color.AddressFamily] = af;
			transfer_unit[Color.SocketType] = socket_type;
			transfer_unit[Color.Protocol] = protocol;
			transfer_unit[Color.Flags] = dwFlags;
			transfer_unit[Color.Handle] = socket_handle.ToInt32();

            if (socket_handle.ToInt32() != Kernel32Support.INVALID_HANDLE_VALUE) makeCallBack(transfer_unit);
            return socket_handle;
        }

		public struct Color {
            public const string AddressFamily="AddressFamily";
            public const string SocketType="SocketType";
            public const string Protocol="Protocol";
            public const string Flags="Flags";
			public const string Handle = "SocketHandle";
		}
    }
}
