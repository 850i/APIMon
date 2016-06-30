using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonShared {
    public interface RemoteControlInterface :RemoteInterface{
        void launchProgram(ProgramStartDescription program_start_description);
        ProgramResponseDescription[] getLaunchResults();
        bool isLaunchResultsAvailable();
        void exit();
    }
}
