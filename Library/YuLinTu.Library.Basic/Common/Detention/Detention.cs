using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace YuLinTu.Library.Basic
{
    public class DetentionReporter
    {
        #region Properties

        public double MillisecondsTimeout
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        #endregion

        #region Fields

        private readonly object objSync;

        private Timer timer;
        private object currentValue;

        #endregion

        #region Events

        public event DetentionElapsedEventHandler Elapsed;

        #endregion

        #region Ctor

        public DetentionReporter()
        {
            objSync = new object();

            timer = new Timer();
            timer.Interval = 500;
            timer.AutoReset = false;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void Set(object value)
        {
            lock (objSync)
            {
                currentValue = value;
                timer.Stop();
                timer.Start();
            }
        }

        #endregion

        #region Methods - Events

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Elapsed == null)
                return;

            Elapsed(this, new DetentionElapsedEventArgs(currentValue));
        }

        #endregion

        #endregion
    }
}
