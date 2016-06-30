using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib.Hooks.ws2_32.dll
{
    public class Hook_WSASend:AbstractHookDescription
    {
        public const int BUFFER_SAMPLE_LENGTH = 100;

        protected override Delegate createHookDelegate() {
            return new WS2_32Support.DWSASend(WSASend_Hooked);
        }

        // this is where we are intercepting all file accesses!
        public int WSASend_Hooked(IntPtr socket_handle, IntPtr lpBuffers, Int32 dwBufferCount, ref Int32 lpNumberOfBytesSent, int flags, IntPtr lpOverlapped, IntPtr lpCompletionRoutine) {
            preprocessHook();

            WS2_32Support.WSABUF[] buffers = new WS2_32Support.WSABUF[dwBufferCount];
            unsafe {
                WS2_32Support.WSABUF* lpbuffer = (WS2_32Support.WSABUF*)lpBuffers.ToPointer();
                for (int i = 0; i < dwBufferCount; i++) {
                    buffers[i] = lpbuffer[i];
                }
            }
            string z = "";
            for (int i = 0; i < dwBufferCount; i++) {
                z += AbstractHookDescription.extractBufferAsString(buffers[i].buf, (int)(buffers[i].len < BUFFER_SAMPLE_LENGTH ? buffers[i].len : buffers[i].len));
            }
            z.Replace("\r\n", " ");
            //Console.WriteLine(z);
			Console.WriteLine("ws2_32.WSASend intercepted");
            Func<int, string, string> gen = null;
            gen = (num, symb) => num == 0 ? "" : gen(num - 1, symb) + symb;
            //Console.WriteLine(gen(10, "<") + gen(10, ">"));

			TransferUnit transfer_unit = createTransferUnit();
			transfer_unit[Color.Handle] = socket_handle.ToInt32();
			transfer_unit[Color.Buffer] = z;

			//call original API
            int result = WS2_32Support.WSASend( socket_handle,  lpBuffers,  dwBufferCount, ref  lpNumberOfBytesSent,  flags,  lpOverlapped,  lpCompletionRoutine);

            if (result != WS2_32Support.SOCKET_ERROR) makeCallBack(transfer_unit);
            return result;
        }

		public struct Color {
			public const string Handle = "SocketHandle";
			public const string Buffer = "buffer";
		}
    }
}
