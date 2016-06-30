using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CPN
{
    class WrongSelectorProvidedException:Exception
    {
        public WrongSelectorProvidedException()
        {
        }

        public WrongSelectorProvidedException(String reason)
            : base(reason)
        {
        }        

        public WrongSelectorProvidedException( Exception ex)
            : base("Wrong type exception has been raised", ex)
        {
        }

        public WrongSelectorProvidedException(String reason, Exception ex): base(reason, ex)
        {
        }

        protected WrongSelectorProvidedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
