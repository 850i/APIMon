using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIMonShared;

namespace APIMon {
    /// <summary>
    /// This class is created for glueing together independent creation of objects by .NET
    /// for processing client requests and my single persistent RemoteControlServer.
    /// .NET might create a lot of instances of Proxy but all of them will be using 
    /// the same mediator RemoteControlServer.RemoteConrtolImpl from RemoteControlServer
    /// </summary>
    public class RemoteControlStsukoProxy: MarshalByRefObject, RemoteControlInterface {
        RemoteControlInterface communication_point = RemoteControlServer.instance;


        #region RemoteControlInterface Members

        public void launchProgram(ProgramStartDescription start_description) {
            communication_point.launchProgram(start_description);
        }

        public void ping() {
            communication_point.ping();
        }

        public void exit() {
            communication_point.exit();
        }


        public ProgramResponseDescription[] getLaunchResults() {
            return communication_point.getLaunchResults();
        }

        public bool isLaunchResultsAvailable() {
            return communication_point.isLaunchResultsAvailable();
        }

        #endregion
    }
}
