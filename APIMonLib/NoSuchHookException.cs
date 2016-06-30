using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace APIMonLib
{
    [Serializable]
    public class NoSuchHookException:RemoteHookingException
    {
        public NoSuchHookException()
        {
        }

        public NoSuchHookException(String reason)
            : base(reason)
        {
        }        

        public NoSuchHookException( Exception ex)
            : base("Smth goes wrong", ex)
        {
        }

        public NoSuchHookException(String reason, Exception ex)
            : base(reason, ex)
        {
        }

        protected NoSuchHookException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
