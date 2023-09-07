using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace YuLinTu.Library.Basic
{
    public class SingleBackgroundWorker
    {
        #region Properties

        #region Properties - System

        private BackgroundWorker BW
        {
            get
            {
                if (worker == null)
                {
                    are = new AutoResetEvent(false);
                    worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                    worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                    worker.RunWorkerAsync();
                }
                return worker;
            }
        }

        #endregion

        #endregion

        #region Fields

        #region Fields - Static

        private readonly object objSync = new object();

        #endregion

        #region Fields - System

        private DoWorkEventArgs doWorkArgs;
        private BackgroundWorker worker;
        private AutoResetEvent are;

        #endregion

        #endregion

        #region Events

        public event DoWorkEventHandler DoWork;
        public event ProgressChangedEventHandler ProgressChanged;

        #endregion

        #region Ctor

        public SingleBackgroundWorker()
        {
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void Set(object argument)
        {
            lock (objSync)
            {
                BW.ToString();

                if (doWorkArgs != null)
                    doWorkArgs.Cancel = true;

                doWorkArgs = new DoWorkEventArgs(argument);

                are.Set();
            }
        }

        public void ReportProgress(int percent, object argument)
        {
            BW.ReportProgress(percent, argument);
        }

        #endregion

        #region Methods - Events

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                are.WaitOne();

                if (DoWork != null)
                    DoWork(this, doWorkArgs);
            }
        }

        #endregion

        #endregion
    }
}
