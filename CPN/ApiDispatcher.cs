using System;
using System.Collections.Generic;
using System.Text;
using APIMonLib;
namespace CPN
{

    //TODO It would be really useful for debugging to implement recording of the stream of data and replaying it later.

    /// <summary>
    /// This class gets intercepted API calls and
    /// directs them into proper API places
    /// </summary>
    public class ApiDispatcher
    {
        private static Dictionary<APIFullName, ApiPlace> api_places_mapping = new Dictionary<APIFullName, ApiPlace>();

        public static void registerApiPlace(ApiPlace api_place)
        {
            api_places_mapping.Add(api_place.api_call_name, api_place);
        }

        public static void dispatchToken(Token token)
        {
            ApiPlace api_place = null;
            if (api_places_mapping.TryGetValue(token.apiCallName,out api_place))
            {
                api_place.putToken(token);
            }
            else
            {
                throw new Exception("Can not find API place_for the API call arrived:\n "+ token.toString ("----"));
            }
        }

        /// <summary>
        /// Returns names of API calls as they registered in API dispatcher by
        /// API places.
        /// </summary>
        /// <returns></returns>
        public static APIFullName[] getListOfApiCalls()
        {
            APIFullName[] result = new APIFullName[api_places_mapping.Keys.Count];
            api_places_mapping.Keys.CopyTo(result, 0);
            return result;
        }
    }
}
