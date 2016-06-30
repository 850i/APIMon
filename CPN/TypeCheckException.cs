using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CPN
{
    public class TypeCheckException: Exception
    {
        public TypeCheckException()
        {
        }

        public TypeCheckException(String reason)
            : base(reason)
        {
        }        

        public TypeCheckException( Exception ex)
            : base("Wrong type exception has been raised", ex)
        {
        }

        public TypeCheckException(String reason, Exception ex): base(reason, ex)
        {
        }

        protected TypeCheckException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
