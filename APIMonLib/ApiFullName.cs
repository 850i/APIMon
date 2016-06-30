using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib
{
    [Serializable]
    public class APIFullName
    {
        public string library_name;
        public string api_name;

        public APIFullName(string _library_name, string _api_name){
            this.library_name=_library_name;
            this.api_name=_api_name;
        }

        public override string ToString()
        {
            return library_name+"."+api_name;
        }

        public override bool Equals(object obj)
        {
            if (obj is APIFullName){
                APIFullName r = (APIFullName)obj;
                return (this.api_name.Equals(r.api_name)) && (this.library_name.Equals(r.library_name));
            }else return false;
        }

        public override int GetHashCode()
        {
            return this.api_name.GetHashCode ()+this.library_name.GetHashCode ();
        }
    }
}
