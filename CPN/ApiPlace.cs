using System;
using System.Collections.Generic;
using System.Text;
using APIMonLib;
using APIMonLib.Hooks;

namespace CPN {
	public class ApiPlace : Place {

		private APIFullName _api_call_name = null;

		public APIFullName api_call_name {
			get { return _api_call_name; }
		}

		/// <summary>
		/// This constructor creates ApiPlace which will receive
		/// intercepted API calls.
		/// </summary>
		/// <param name="name_of_api_call">Name of API call this place takes care of</param> 
		public ApiPlace(APIFullName name_of_api_call)
			: base(name_of_api_call + "_API") {
			_api_call_name = name_of_api_call;
			ApiDispatcher.registerApiPlace(this);
		}

		public ApiPlace(string library_name, string api_call_name) : this(new APIFullName(library_name, api_call_name)) { }

		public ApiPlace(AbstractHookDescription hd) : this(hd.api_full_name) { }

		public ApiPlace(Type type) : this((AbstractHookDescription)(Activator.CreateInstance(type))) { }
	}
}
