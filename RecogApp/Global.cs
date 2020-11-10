using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecogApp
{
    class Global
    {
        public static bool g_bWorkStop = true;
        public static bool g_bThreadRunning = false;
        public static Form1 g_frmMain;
        public static Thread g_thrMainThread = null;
    }
}
