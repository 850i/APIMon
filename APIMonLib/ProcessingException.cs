using System;
using System.Runtime.Serialization;

namespace APIMonLib {
	[Serializable]
	public class ProcessingException : Exception {
		public ProcessingException() {
		}

		public ProcessingException(String reason)
			: base(reason) {
		}

		public ProcessingException(Exception ex)
			: base(ex.Message, ex) {
		}

		public ProcessingException(String reason, Exception ex)
			: base(reason, ex) {
		}

		protected ProcessingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
