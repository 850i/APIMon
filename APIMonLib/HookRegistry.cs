using System;
using System.Collections.Generic;
using APIMonLib.Hooks;

namespace APIMonLib
{
    /// <summary>
    /// This class provides access to all hooks available for use.
    /// </summary>
    public class HookRegistry
    {

        private static Dictionary<APIFullName, HookDescription> flat_hook_db = new Dictionary<APIFullName, HookDescription>();
        private static HashSet<APIFullName> hooks_to_install = new HashSet<APIFullName>();
        private static HashSet<APIFullName> hooks_installed = new HashSet<APIFullName>();
        private static InterceptorCallBackInterface call_back = null;
        private static Object sync_object = new Object();

        public static void setHooks(APIFullName[] list_of_api_names, InterceptorCallBackInterface _call_back)
        {
            lock (sync_object)
            {
                hooks_to_install.UnionWith(list_of_api_names);
                //hooks_to_install.Add(new APIMonLib.Hooks.ntdll.dll.Hook_LdrLoadDll().api_full_name);
                hooks_to_install.ExceptWith(hooks_installed);
            }
            call_back = _call_back;
            checkHooksToInstall();
        }

        public static void checkHooksToInstall()
        {
            try
            {
	            lock (sync_object)
	            {
	                hooks_to_install.ExceptWith(hooks_installed);
	                if (hooks_to_install.Count == 0) return;
	                foreach (APIFullName api_full_name in hooks_to_install)
	                {
	                    HookDescription hd = getHookDescription(api_full_name);
	                    if (hd == null) throw new NoSuchHookException("No hook found for API " + api_full_name);
	                    try
	                    {
	                        hd.installHook(call_back);
	                        hooks_installed.Add(api_full_name);
	                    }
	                    catch (System.DllNotFoundException)
	                    {
	                        Console.WriteLine("" + api_full_name.library_name + " wasn't found.\n\t Hook " + api_full_name + " wasn't installed.");
	                    }
	                }
	            }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("HookRegistry.checkHooksToInstall \n---------------------\n"+ex+"\n------------------------------------");
            }
        }

        private static HookDescription getHookDescription(APIFullName api_full_name)
        {
            HookDescription result;
            Type type = System.Type.GetType(typeof(AbstractHookDescription).Namespace + "." + api_full_name.library_name + ".Hook_" + api_full_name.api_name);
            //Console.WriteLine("Looking for type " + typeof(AbstractHookDescription).Namespace + "." + api_full_name.library_name + ".Hook_" + api_full_name.api_name);

            if (type == null)
            {
                Console.WriteLine("Could not find type " + typeof(AbstractHookDescription).Namespace + "." + api_full_name.library_name + ".Hook_" + api_full_name.api_name);
                System.Threading.Thread.Sleep(3000);
                throw new NoSuchHookException("No hook type found for API " + api_full_name);
            }
            Activator.CreateInstance(type);
            flat_hook_db.TryGetValue(api_full_name, out result);
            return result;
        }

        /// <summary>
        /// This method loads hook description object into the database
        /// </summary>
        /// <param name="hd"></param>
        public static void loadHookInfo(HookDescription hd)
        {
            try
            {
                flat_hook_db.Add(hd.api_full_name, hd);
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Hook " + hd.api_full_name + " was registered already");
            }
        }
    }
}
