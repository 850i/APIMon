using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPN {
    /// <summary>
    /// This Place supposed to be used as a detection place. It provides a functionality that can be useful for special analysis
    /// </summary>
    public class DetectionPlace :Place {

        /// <summary>
        /// Creates DetectionPlace and configures it to print
        /// </summary>
        /// <param name="name">Use unique name for this place in the whole CPN</param>
        public DetectionPlace(String name)
			: base(name) {
            this.setPrintLevel(Place.PrintLevel.High).addPutReaction(new Place.Reaction(PrintReactionProvider.getProvider(ConsoleColor.Red).advancedPrintToken));
		}
    }
}
