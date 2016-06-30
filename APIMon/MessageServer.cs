using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIMonLib;
using APIMonShared;
using System.Diagnostics;
using EasyHook;
using CPN;
using System.Runtime.Remoting;
using AliasProcessCreationFlags = APIMonLib.Hooks.kernel32.dll.Kernel32Support.ProcessCreationFlags;

namespace APIMon
{
    class MessageServer
    {
        private String ChannelName = null;

        private static MessageServer _instance = null;
        public static MessageServer instance {
            get {
                if (_instance == null) _instance = new MessageServer();
                return _instance;
            }
        }

        private MessageServer() {
        }

        /// <summary>
        /// Returns path to the directory where main module of the program (the file from which process was started) is located 
        /// </summary>
        /// <returns></returns>
        public string getMainModuleDirectory()
        {
            string main_module_path = Process.GetCurrentProcess().MainModule.FileName;
            string main_module_name = Process.GetCurrentProcess().MainModule.ModuleName;
            main_module_path = main_module_path.Replace(main_module_name, "");
            return main_module_path;
        }


        #region CallBacks for remote hooking

        /// <summary>
        /// This method accepts transfer unti and dispatches it through ApiDispatcher 
        /// which transfers it to appropriate places in CPNet
        /// </summary>
        /// <param name="tu"></param>
        private void receiveTransferUnit(TransferUnit tu)
        {
            try
            {
                ApiDispatcher.dispatchToken(new Token(tu));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Receives remote text message and prints it immediately
        /// </summary>
        /// <param name="message"></param>
        private void receiveTextMessage(String message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Receives remote exception and prints it
        /// </summary>
        /// <param name="ex"></param>
        private void receiveException(RemoteHookingException ex)
        {
            //TODO Do something with remote excpetion. Probably shutdown remote hook
            Console.WriteLine("------------------------------Remote Exception--------------------------------");
            Console.WriteLine(ex);
            Console.WriteLine("-----------------------End of Remote Exception--------------------------------");
        }

        /// <summary>
        /// This method is actually designed for remote system. It returns API calls that should be intercepted by remote system
        /// </summary>
        /// <returns></returns>
        private APIFullName[] getListOfInterceptedApiCalls()
        {
            return ApiDispatcher.getListOfApiCalls();
        }

        /// <summary>
        /// This method connects defined here callback functions as delegates to TransferUnitReceiverEvents
        /// </summary>
        private void attachCallbacks()
        {
            TransferUnitReceiver.instance.addTransferUnitReceiver(new TransferUnitReceiver.ReceiveTransferUnit(receiveTransferUnit));
            TransferUnitReceiver.instance.addTextMessageReceiver(new TransferUnitReceiver.ReceiveTextMessage(receiveTextMessage));
            TransferUnitReceiver.instance.addExceptionReceiver(new TransferUnitReceiver.ReceiveRemoteHookingException(receiveException));
            TransferUnitReceiver.instance.addGetApiCallsToInterceptReceiver(new TransferUnitReceiver.ReceiveGetApiCallsToIntercept(getListOfInterceptedApiCalls));
        }

		public void waitForTheEndOfProcessing() {
			TransferUnitReceiver.instance.waitForAllTransferUnitsGetProcessed();
		}
        #endregion

        public void initEnvironment()
        {
            //RemoteHooking.ExecuteAsService<Object>("Config.Register", new Object[] { "A FileMon like demo application.",
            //    "FileMon.exe",
            //    "FileMonInject.dll",
            //    "native_class_library.dll" });
            Config.Register(
                "A FileMon like demo application.",
                "APIMon.exe",
                "APIMonInject.dll",
                "AlmostNativeHooks.dll",
                //"msvcm90d.dll",
                //"msvcp90d.dll",
                //"msvcr90d.dll",
                "APIMonLib.dll"
                );

            attachCallbacks();

            RemoteHooking.IpcCreateServer<ChannelReceiverStsukoProxy>(ref ChannelName, WellKnownObjectMode.SingleCall);
        }

        /// <summary>
        /// Must be called before shutting down.
        /// Closes remote connections. 
        /// </summary>
		public void closeEnvironment() {
			TransferUnitReceiver.instance.stopReceiver();
		}

        /// <summary>
        /// Injects hooking library into started processes. 
        /// </summary>
        /// <param name="process_names">array of process names to watch w/o .exe extention(e.g. msimn, Test_Files_Handles)</param>
        /// <param name="as_service">set this parameter to true for injecting from service. Needed for hooking services</param>
        /// <param name="path_to_library_for_injection">Path to library for injection. Library should be located on local disk. Can be null for as_service=false</param>
        public void watchProcesses(string[] process_names, bool as_service, string path_to_library_for_injection)
        {
            foreach (String process_name in process_names)
            {
                foreach (Process p in Process.GetProcessesByName(process_name))
                {
                    try
                    {
                        if (as_service)
                        {
                            injectLibraryAsService(p, path_to_library_for_injection);
                        }
                        else
                        {

                            injectLibrary(p);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\n\nException while processing PID=" + p.Id + " -------------------------------------\n");
                        Console.WriteLine("-------------------------------------------------------------------------------------\n");
                        Console.WriteLine(e);
                    }
                }
            }
        }

        public void injectLibrary(Process p)
        {
            MessageFromInjector message = new MessageFromInjector();
            message.channel_name = ChannelName;
            List<int> thread_ids = new List<int>();
            foreach (ProcessThread t in p.Threads)
            {
                thread_ids.Add(t.Id);
            }
            message.thread_ids = thread_ids.ToArray();
            RemoteHooking.Inject(p.Id, "APIMonInject.dll", null, message);

        }

        public void injectLibrary(Process p, int TID) {
            MessageFromInjector message = new MessageFromInjector();
            message.channel_name = ChannelName;
            List<int> thread_ids = new List<int>();
            foreach (ProcessThread t in p.Threads) {
                thread_ids.Add(t.Id);
            }
            message.thread_ids = thread_ids.ToArray();
            RemoteHooking.Inject(p.Id, (Int32)TID, "APIMonInject.dll", null, message);

        }

        private void injectLibraryAsService(Process p, string library_location)
        {
            MessageFromInjector message = new MessageFromInjector();
            message.channel_name = ChannelName;
            List<int> thread_ids = new List<int>();
            foreach (ProcessThread t in p.Threads)
            {
                thread_ids.Add(t.Id);
            }
            message.thread_ids = thread_ids.ToArray();
            Console.WriteLine("Injecting through service");
            RemoteHooking.ExecuteAsService<RemoteHooking>("Inject", new Object[] { p.Id, library_location, null, message });
        }


		///// <summary>
		///// Starts process and injects library with redirection of standard input, output and error to handles provided
		///// </summary>
		///// <param name="what_to_start"></param>
		///// <param name="command_line_parameters"></param>
		///// <returns>process ID of newly created process</returns>
		//public Process startProcessAndInject(string what_to_start, string command_line_parameters, IntPtr stdIn, IntPtr stdOut, IntPtr stdErr)
		//{
		//    int process_id;
		//    MessageFromInjector message = new MessageFromInjector();
		//    message.channel_name = ChannelName;

		//    RemoteHooking.CreateAndInjectEx(what_to_start, command_line_parameters, getMainModuleDirectory() + "APIMonInject.dll", null, out process_id,
		//        stdIn, stdOut, stdErr, message);
		//    return System.Diagnostics.Process.GetProcessById(process_id);
		//}

        /// <summary>
        /// Starts process and injects library
        /// </summary>
        /// <param name="what_to_start"></param>
        /// <param name="command_line_parameters"></param>
        /// <returns>process ID of newly created process</returns>
        public Process startProcessAndInject(ProgramStartDescription tp)
        {
			int process_id;
			MessageFromInjector message = new MessageFromInjector();
			message.channel_name = ChannelName;
			RemoteHooking.CreateAndInject(tp.image_path, tp.command_line, (int)AliasProcessCreationFlags.CREATE_NEW_CONSOLE, getMainModuleDirectory() + "APIMonInject.dll", null, out process_id, message);
			return System.Diagnostics.Process.GetProcessById(process_id);

			//Process notePad = new Process();

			//notePad.StartInfo.FileName = tp.image_path;
			//notePad.StartInfo.Arguments = tp.command_line;
			//notePad.Start();
			//return notePad;
        }



		/// <summary>
		/// Waits for process to exit in time specified otherwise kills it. This method blocks for at most ime specified.
		/// </summary>
		/// <param name="p">which process to wait</param>
		/// <param name="time_in_sec">how much time to wait for process exit</param>
		/// <returns>true when process exited itself. False when process was killed due to timeout</returns>
        public bool waitForProcessToEnd(Process p, int time_in_sec)
        {
			if (!p.WaitForExit(time_in_sec * 1000)) {
				killProcess(p);
				return false;
			} else {
				return true;
			}
        }

        //public void setExitHandler(Process p, EventHandler eh){
        //    p.EnableRaisingEvents = true;
        //    p.Exited+=eh;
        //}

        private const int MAX_TRIES_TO_KILL = 5;

        private void killProcess(Process p)
        {
            int tries_to_kill = MAX_TRIES_TO_KILL;
            bool successful_kill = false;
            while ((--tries_to_kill >= 0)&&(!p.HasExited))
            {
                try
                {
                    p.Kill(); 
                    successful_kill = true;
                    break;
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    Console.WriteLine("Failed to stop process");
                    Console.WriteLine("Will retry for " + tries_to_kill + " times.");
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception)
                {
                    Console.WriteLine("Looks like process has exited");
                    break;
                }
            }
            if (p.HasExited)
            {
                Console.WriteLine("Process exited");
            }
            else
            {
                if (!successful_kill) throw new Exception("Couldn't stop process ");
                else Console.WriteLine("Process was killed but not exited yet.");
            }
        }
    }
}
