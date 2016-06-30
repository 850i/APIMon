using System;

namespace APIMonLib
{
    public interface HookDescription
    {
        //String library_name { get; }
        //String api_call_name { get; }
        APIFullName api_full_name {get; }
        void installHook(InterceptorCallBackInterface link);
    }
}
