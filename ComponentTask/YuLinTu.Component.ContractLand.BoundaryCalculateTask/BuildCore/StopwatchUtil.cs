using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.BuildJzdx
{
    public class StopwatchUtil
    {
        private static Stopwatch _stopwath = new Stopwatch();
        public static void Start(string msg = null)
        {
            if (msg != null)
                Console.WriteLine(msg);
            _stopwath.Start();
        }
        public static void Stop(bool fWriteConsole=true)
        {
            _stopwath.Stop();
            if (fWriteConsole)
            {
                Console.WriteLine("耗时："+ _stopwath.Elapsed);
            }
        }
        //public static TimeSpan Elapsed
        //{
        //    get { return _stopwath.Elapsed; }
        //}
    }
}
