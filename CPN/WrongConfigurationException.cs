using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CPN
{
    public class WrongConfigurationException:Exception
    {
        public WrongConfigurationException()
        {
        }

        public WrongConfigurationException(String reason)
            : base(reason)
        {
        }        

        public WrongConfigurationException( Exception ex)
            : base("Wrong type exception has been raised", ex)
        {
        }

        public WrongConfigurationException(String reason, Exception ex): base(reason, ex)
        {
        }

        protected WrongConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
