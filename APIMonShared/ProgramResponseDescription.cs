using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonShared {
    [Serializable]
    public class ProgramResponseDescription :ProgramStartDescription {
        public ProgramResponseDescription():base() {
        }

        public ProgramResponseDescription(ProgramStartDescription start_description)
            : base(start_description) {
        }

        public string desciption = string.Empty;
    }
}
