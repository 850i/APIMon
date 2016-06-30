using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonLib {
    public class Configuration {
        public const bool DEBUG=true;
        public const string REMOTE_CONTROL_CHANNEL_NAME = "APIMon_RemoteControl";
        /// <summary>
        /// Determines if the system will try to inject into children processes of inject process
        /// true means try to inject. false means do not inject into child process
        /// </summary>
        public const bool FOLLOW_PROCESS_TREE=true;
    }
}
