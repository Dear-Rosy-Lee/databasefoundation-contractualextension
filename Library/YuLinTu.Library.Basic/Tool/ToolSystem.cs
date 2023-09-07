using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace YuLinTu.Library.Basic
{
    public class ToolSystem
    {
        #region Properties

        #region Properties - Private

        private static List<Stopwatch> Stopwatches
        {
            get
            {
                if (listStopwatch == null)
                    listStopwatch = new List<Stopwatch>();

                return listStopwatch;
            }
        }

        #endregion

        #endregion

        #region Fields

        private static readonly object objSync;
        private static List<Stopwatch> listStopwatch;

        #endregion

        #region Ctor

        static ToolSystem()
        {
            objSync = new object();
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_ToolSystem);
        }

        #endregion

        #region Methods - Stopwatch

        public static void StartStopwatch()
        {
            Stopwatch watch = Stopwatch.StartNew();
            Stopwatches.Add(watch);
        }

        public static TimeSpan? StopStopwatch()
        {
            Stopwatch watch = GetLastStopwatch();
            if (watch == null)
                return null;

            watch.Stop();
            Stopwatches.Remove(watch);

            return watch.Elapsed;
        }

        private static Stopwatch GetLastStopwatch()
        {
            if (Stopwatches.Count == 0)
                return null;

            return Stopwatches[Stopwatches.Count - 1];
        }

        #endregion

        #region Methods - Process

        public static bool StartProcess(string path)
        {
            try
            {
                Process.Start(path);
                return true;
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    LanguageAttribute.GetLanguage("ex20107"),
                    ToolException.GetExceptionMessage(ex));

                Trace.WriteLine(new Log()
                {
                    Description = msg,
                    EventID = 20107,
                    Grade = eLogGrade.Error,
                    Source = typeof(ToolSystem).FullName,
                });

                return false;
            }
        }

        #endregion

        #region Methods - MessageBox

        public static DialogResult ShowMessageBoxWarning(IWin32Window owner, string text)
        {
            return MessageBox.Show(owner, text, null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion
    }
}
