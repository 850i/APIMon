using System;
using System.Runtime.Serialization;

namespace APIMonLib
{
    [Serializable]
    public class RemoteHookingException:Exception
    {
        public RemoteHookingException()
        {
        }

        public RemoteHookingException(String reason)
            : base(reason)
        {
        }        

        public RemoteHookingException( Exception ex)
            : base(ex.Message, ex)
        {
        }

        public RemoteHookingException(String reason, Exception ex): base(reason, ex)
        {
        }

        protected RemoteHookingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
