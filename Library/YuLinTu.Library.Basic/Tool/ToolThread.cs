using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace YuLinTu.Library.Basic
{
    public class ToolThread
    {
        #region Methods - Thread

        public static void StartThread(ThreadStart method)
        {
            Thread t = new Thread(method);
            t.Start();
        }

        public static void StartThread(ParameterizedThreadStart method, object argument)
        {
            Thread t = new Thread(method);
            t.Start(argument);
        }

        /// <summary>
        /// 打开进程
        /// </summary>
        /// <param name="method"></param>
        /// <param name="argument"></param>
        public static bool StartProgress(string fileName)
        {
            try
            {
                System.Diagnostics.Process.Start(fileName);
                return true;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return false;
        }



        #endregion

        #region Methods - Events

        #endregion
    }
}
