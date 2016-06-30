using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using APIMonShared;
using APIMonLib;
using System.Runtime.Remoting;

namespace VisualProgramLauncher {
    public class CPNMonitorLauncher : RemoteControlInterface {

        private RemoteControlClient remote_control = new RemoteControlClient(Configuration.REMOTE_CONTROL_CHANNEL_NAME);

        public delegate void RemoteServerStateHasChanged();
        private RemoteServerStateHasChanged _remoteServerStateHasChanged = null;


        public void addStateHasChangedSubscriber(RemoteServerStateHasChanged state_has_changed_subscriber) {
            _remoteServerStateHasChanged += state_has_changed_subscriber;
        }

        private bool _server_present = false;

        public bool server_present {
            get { return _server_present; }
            private set {
                if (_server_present != value) {
                    _server_present = value;
                    _remoteServerStateHasChanged();
                }
            }
        }

        public void launchCPNMonitor() {
            testConnection();
            if (!server_present) {
                Process.Start(".\\APIMon.exe");
            }
            testConnection();
        }

        private void stopCPNMonitor() {
            testConnection();
            if (server_present) {
                remote_control.exit();
            }
            testConnection();
        }

        /// <summary>
        /// Sets current state of the model for remote server according to the presence of remote server.
        /// This method might block for several seconds if remote server doesn't repsond
        /// </summary>
        private void testConnection() {
            try {
                remote_control.testConnection();
                server_present = true;
            } catch (RemotingException) {
                server_present = false;
            }
        }

        #region RemoteControlInterface Members

        public void launchProgram(ProgramStartDescription program_start_description) {

            try {
                remote_control.launchProgram(program_start_description);
                server_present = true;
            } catch (RemotingException) {
                server_present = false;
            }
        }

        public void ping() {
            try {
                remote_control.ping();
                server_present = true;
            } catch (RemotingException) {
                server_present = false;
            }
        }

        public void exit() {
            try {
                remote_control.exit();
                server_present = true;
            } catch (RemotingException) {
                server_present = false;
            }
        }

        public ProgramResponseDescription[] getLaunchResults() {
            ProgramResponseDescription[] result=new ProgramResponseDescription[0];
            try {
                result=remote_control.getLaunchResults();
                server_present = true;
            } catch (RemotingException) {
                server_present = false;
            }
            return result;
        }

        public bool isLaunchResultsAvailable() {
            bool result = false;
            try {
                result= remote_control.isLaunchResultsAvailable();
                server_present = true;
            } catch (RemotingException) {
                server_present = false;
            }
            return result;
        }

        #endregion
    }
}
