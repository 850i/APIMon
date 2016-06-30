using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIMonShared;

namespace VisualProgramLauncher {
    [Serializable]
    public class ProgramStatusModel : ProgramStartDescription {
        public enum Status : byte {
            NotLaunched = 0,
            Launched = 1,
            Running = 2,
            Exited = 3,
            Killed = 4,
            Unknown=5
        }

        public delegate void ProgramStatusHasChanged();
        private ProgramStatusHasChanged _programStatusHasChanged = null;

        public void addStateHasChangedSubscriber(ProgramStatusHasChanged state_has_changed_subscriber) {
            _programStatusHasChanged += state_has_changed_subscriber;
        }

        private Status _status = Status.NotLaunched;

        public Status status {
            get { return _status; }
            set { 
                if (_status!=value){
                    _status = value; 
                    _programStatusHasChanged();
                }               
            }
        }

        public ProgramStatusModel():base() {
        }

        public ProgramStatusModel(ProgramStartDescription start_description)
            : base(start_description) {
        }

        public ProgramStatusModel(ProgramStatusModel status_model)
            : base(status_model) {
            _status = status_model.status;
        }
    }
}
