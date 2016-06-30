using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib
{
    [Serializable]
    public class MessageFromInjector
    {
        public string channel_name;
        public int[] thread_ids;
    }
}
