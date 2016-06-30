using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ws2_32.dll
{
    public class Hook_send:AbstractHookDescription
    {
        public const int BUFFER_SAMPLE_LENGTH=100;

        protected override Delegate createHookDelegate()
        {
            return new WS2_32Support.Dsend(send_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public int send_Hooked(IntPtr socket_handle, IntPtr lpBuffer, int buflen, int flags)
        {
            preprocessHook();

            //String z = extractBufferAsString(lpBuffer, buflen < BUFFER_SAMPLE_LENGTH ? buflen : BUFFER_SAMPLE_LENGTH);
            String z = extractBufferAsString(lpBuffer, buflen);
            z.Replace("\r\n", " ");
            //Console.WriteLine(z);
			Console.WriteLine("ws2_32.send intercepted");
            Func<int, string, string> gen=null;
            gen = (num, symb) => num==0?"":gen(num-1,symb)+symb;
            //Console.WriteLine(gen(10,"<")+gen(10,">"));

			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit[Color.Handle] = socket_handle.ToInt32();
            transfer_unit[Color.Buffer] = z;

            int result = WS2_32Support.send(socket_handle, lpBuffer, buflen, flags);

            if (result != WS2_32Support.SOCKET_ERROR) makeCallBack(transfer_unit);
            return result;
        }
		public struct Color {
			public const string Handle = "SocketHandle";
			public const string Buffer = "buffer";
		}
    }
}
