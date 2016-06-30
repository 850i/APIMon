using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using EasyHook;
using APIMonLib;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace APIMonInject
{
    public class Main : EasyHook.IEntryPoint, InterceptorCallBackInterface
    {
        TransferUnitSender_New tu_sender;
        System.Reflection.Assembly assembly_1 = null;
        //NativeHooksLoader t1clr = new NativeHooksLoader();



        public Main(RemoteHooking.IContext InContext, MessageFromInjector message)
        {
            ConsolePrinter.writeMessage("Console allocated");
            //AssemblyName an = AssemblyName.GetAssemblyName("z:\\HookingProjects\\APIMonitoring\\Debug\\AlmostNativeHooks.dll");
            //AssemblyName an_1 = AssemblyName.GetAssemblyName("z:\\HookingProjects\\APIMonitoring\\Debug\\msvcm90d.dll");
            //System.Reflection.Assembly.Load(an_1);
            //assembly_1=System.Reflection.Assembly.Load(an);
			tu_sender = new TransferUnitSender_New(message.channel_name);
        }

		/// <summary>
		/// This method sets inclusive mask for thread interception if list of threads to intercept provided.
		/// Otherwise it sets mask to intercept all.
		/// </summary>
		/// <param name="message">Message from injector that might contain list of threads to intercept</param>
        private void maskThreadsToIntercept(MessageFromInjector message) 
        {
            if (message.thread_ids != null)
            {
                LocalHook.GlobalThreadACL.SetInclusiveACL(message.thread_ids);
            }
            else
            {
                ConsolePrinter.writeMessage("No threads to mask");
                LocalHook.GlobalThreadACL.SetExclusiveACL(new Int32[]{0});
            }
        }

        public void Run(RemoteHooking.IContext InContext, MessageFromInjector message)
        {
			//test connection
            tu_sender.ping();

            maskThreadsToIntercept(message);
			//set hooks
			try {
				APIFullName[] to_intercept = tu_sender.getApiCallsToIntercept();
				HashSet<string> libraries = new HashSet<string>();
				foreach (APIFullName api in to_intercept) {
					libraries.Add(api.library_name);
				}
				foreach (string library_name in libraries) {
					APIMonLib.Hooks.kernel32.dll.Kernel32Support.LoadLibraryW(library_name);
				}
				HookRegistry.setHooks(to_intercept, this);
				//t1clr.installNativeHooks();
			} catch (Exception ExtInfo) {
				tu_sender.sendException(new RemoteHookingException(ExtInfo));
				return;
			}

			//Report
            ConsolePrinter.writeMessage("Hooks have been installed.");
            try
            {
                tu_sender.sendTextMessage("Hooks have been installed at process PID=" + RemoteHooking.GetCurrentProcessId());
            }
            catch
            {
            }
			//Here any attempt to send something will actually be performed by injected .NET. So we might wait and blacklist all requests.
            ConsolePrinter.writeMessage("");
            ConsolePrinter.writeMessage("Delay before waking the process ");
			int START_DELAY_S=4;
			for (int hh = START_DELAY_S; hh >= 0; hh--) {
                ConsolePrinter.writeMessage(" " + hh);
				Thread.Sleep(1000);
			}

			//enable sending of transfer units
            ConsolePrinter.writeMessage("Enable sending of TransferUnits");
			tu_sender.enableTransferUnitSend();
            ConsolePrinter.writeMessage("Waking the process now...");
            RemoteHooking.WakeUpProcess();

            foreach(System.Diagnostics.ProcessThread thread in Process.GetCurrentProcess().Threads){
                Console.WriteLine("Thread "+thread.Id+" is "+thread.ThreadState);
            }

            ConsolePrinter.writeMessage("After waking the process");

            try
            {
                try
                {
                    tu_sender.sendTextMessage("Inject report: Entering processing stage PID=" + RemoteHooking.GetCurrentProcessId());
                    tu_sender.blockUntilFinishedProcessing();
                }
                catch(Exception ex)
                {
                    tu_sender.sendException(new RemoteHookingException("Something wrong with send request processing",ex));
                }
            }
            catch
            {
                ConsolePrinter.writeMessage("Problem with remote receiver.");
                // we can't do anything.
            }
        }

        #region CallBackInterface Members

        public void dataHasBeenIntercepted(TransferUnit tu)
        {
            ////Console.WriteLine("Making callback for "+tu.apiCallName);

			//try {
				tu_sender.sendTransferUnit(tu);
			//} catch {
			//    Console.WriteLine("Zabivaem na oshibku");
			//}
        }

        #endregion
    }
}
