using System;
using EasyHook;
using System.Text;

namespace APIMonLib.Hooks {
	/// <summary>
	/// This class defines basic methods needed in all hook classes
	/// </summary>
	abstract public class AbstractHookDescription : HookDescription {

		/// <summary>
		/// This field is increased every time TransferUnit is created by method createTransferUnit
		/// S.t. later TransferUnits might be arranged as a time series.
		/// </summary>
		private static int hook_sequence_number = 0;
		private static object hook_sequence_number_sync = new object();

		protected static Random random = new Random();

		/// <summary>
		/// Easyhook hook
		/// </summary>
		private LocalHook hook;

		/// <summary>
		/// Description of the current hook in the form of APIFullName structure
		/// </summary>
		public virtual APIFullName api_full_name {
			get {
				string dll_name = this.GetType().Namespace.Replace(this.GetType().BaseType.Namespace + ".", "");
				string api_call_name = this.GetType().Name.Replace("Hook_", "");
				return new APIFullName(dll_name, api_call_name);
			}
		}

		protected abstract Delegate createHookDelegate();

		static AbstractHookDescription() {
			//LocalHook.EnableRIPRelocation();
		}

		public AbstractHookDescription() {
			HookRegistry.loadHookInfo(this);
		}

		/// <summary>
		/// Installs hook.
		/// </summary>
		/// <param name="link">Link to call back interface i.e. to the method which will be called by makeCallBack method from the hook handler</param>
		public void installHook(InterceptorCallBackInterface link) {
			hook = LocalHook.Create(
				LocalHook.GetProcAddress(api_full_name.library_name, api_full_name.api_name),
				createHookDelegate(),
				link);
			Console.WriteLine("Installing hook: " + api_full_name);
			hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
		}

		private InterceptorCallBackInterface callback_interface = null;

		/// <summary>
		/// This method must be called by hook processing method before
		/// anything. It sets up hook runtime information
		/// </summary>
		protected void preprocessHook() {
			callback_interface = (InterceptorCallBackInterface)HookRuntimeInfo.Callback;
		}


		/// <summary>
		/// This method should be called from the hook handlers. It routes transfer unit to the place of futher processing.
		/// </summary>
		/// <param name="tu"></param>
		protected void makeCallBack(TransferUnit tu) {
			try {
				callback_interface.dataHasBeenIntercepted(tu);
			} catch (Exception) {
				Console.WriteLine("Error in callback. Disposing hook <" + this.api_full_name.library_name + "." + this.api_full_name.api_name + ">");
				//Console.WriteLine("-----------------------------------------------------------------------");
				//Console.WriteLine(e);
				//Console.WriteLine("-----------------------------------------------------------------------");
				hook.Dispose();
			}
		}


		/// <summary>
		/// Create new TransferUnit and fill out process Id and thread Id fields
		/// </summary>
		/// <returns>new transfer unit</returns>
		protected TransferUnit createTransferUnit() {
			TransferUnit t = new TransferUnit();
			t.PID = RemoteHooking.GetCurrentProcessId();
			t.TID = RemoteHooking.GetCurrentThreadId();
			t.apiCallName = api_full_name;
			lock (hook_sequence_number_sync) {
				t.Hook_sequence_number = hook_sequence_number++;
			}
			return t;
		}

		private const char REPLACEMENT_SYMBOL = '_';

		/// <summary>
		/// Unsafe code which extracts contents of the buffer pointed by lpBuffer.
		/// It replaces all unprintable symbols with REPLACEMENT_SYMBOL
		/// </summary>
		/// <param name="lpBuffer">Safe pointer to buffer</param>
		/// <param name="buflen">length to be extracted in bytes</param>
		/// <returns>extracted buffer</returns>
		unsafe protected static String extractBufferAsString(IntPtr lpBuffer, int buflen) {
			String z;
			unsafe {
				sbyte* buffer = (sbyte*)lpBuffer.ToPointer();
				byte[] tmp_buffer = new byte[buflen];
				z = new string(buffer, 0, buflen);
			}
			StringBuilder result = new StringBuilder(buflen * 2);
			for (int i = 0; i < z.Length; i++) {
				if ((z[i] >= 32) && (z[i] < 255)) {
					result.Append(z[i]);
				} else {
					result.Append(REPLACEMENT_SYMBOL);
				}
			}
			return (string)result.ToString();
		}

	}
}