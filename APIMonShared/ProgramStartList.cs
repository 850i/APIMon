using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIMonShared {
    public class ProgramStartList : List<ProgramStartDescription> {

        public ProgramStartDescription last() {
            if (this.Count == 0) {
                this.Add(new ProgramStartDescription());
            }
            return this.Last();
        }

        public static ProgramStartList operator +(ProgramStartList i, string new_params) {
            i.Add(i.Last().useDifferentCommandLine(new_params));
            return i;
        }

        public static ProgramStartList operator +(ProgramStartList i, ProgramStartDescription new_program) {
            i.Add(new_program);
            return i;
        }

        public static ProgramStartList operator +(ProgramStartList i, string[] program_names) {
            foreach (string image_name in program_names) {
                i.Add(i.Last().useDifferentImageFile(image_name));
            }
            return i;
        }
    }
}
