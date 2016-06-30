using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace APIMonLib {
    public class ConsolePrinter {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();
        private static bool console_allocated = false;

        private static void ensureConsoleAllocated() {
            if (!console_allocated) {
                AllocConsole();
                console_allocated = true;
            }
        }

        public static string writeMessage(string message) {
            ensureConsoleAllocated();
            if (Configuration.DEBUG) Console.WriteLine(message);
            return message;
        }
    }
}
