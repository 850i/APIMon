using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyHook;
using System.Runtime.Remoting;
using System.Threading;

namespace APIMonShared {
    public class RemoteControlClient:RemoteControlInterface {
        private RemoteControlInterface remote_control = null;

        public RemoteControlClient(string channel_name) {
            remote_control = (RemoteControlInterface)RemoteHooking.IpcConnectClient<MarshalByRefObject>(channel_name);
        }


        /// <summary>
        /// Tries to ping remote server. If connection fails throws RemotingException
        /// </summary>
        public void testConnection(){
            int fail_count = 7;
            while ((fail_count--) > 0) {
                try {
                    ping();
                    break;
                } catch (RemotingException re) {
                    if (fail_count > 0) {
                        Console.WriteLine("Can not connect to server.");
                        Thread.Sleep(500);
                    } else {
                        throw re;
                    }
                }
            }
        }

        #region RemoteControlInterface Members

        public void launchProgram(ProgramStartDescription program_start_description) {
            remote_control.launchProgram(program_start_description);
        }

        /// <summary>
        /// use it to check the connection
        /// </summary>
        public void ping() {
            remote_control.ping();
        }

        public void exit() {
            remote_control.exit();
        }

        public ProgramResponseDescription[] getLaunchResults() {
            return remote_control.getLaunchResults();
        }

        public bool isLaunchResultsAvailable() {
            return remote_control.isLaunchResultsAvailable();
        }

        #endregion
    }
}
