using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Windows;
using System.IO;
using YuLinTu.Library.Result;
using System.Windows;
using System.Threading;

namespace ManualSlaveProcessTask
{
    class Program
    {
        #region Fields

        private static ProcessApplication app;

        #endregion

        #region Methods

        #region Methods - User

        private static void Initialize()
        {
            typeof(ExportShapeTask).ToString();
        }

        #endregion

        #region Methods - System

        #region Methods - Private

        private static void Main(string[] args)
        {
            Initialize();
            new TheApp().ToString();
            TheApp.Current.Message.Received += Message_Received;
            app = new ProcessApplication();
            app.Startup(new AppStartupEventArgs(0, args));
            app.Shutdown(new AppExitEventArgs() { Args = args });
            TheApp.Current.Message.Received -= Message_Received;
            Thread.Sleep(5000);
        }

        private static void Message_Received(object sender, MsgEventArgs e)
        {
            switch (e.ID)
            {
                case EdWindows.langSlaveProcessRequested:
                    MessageBox.Show(e.ToString());
                    SlaveProcessRequested(sender, e as SlaveProcessRequestedEventArgs);
                    break;
                default:
                    break;
            }
        }

        private static void SlaveProcessRequested(object sender, SlaveProcessRequestedEventArgs e)
        {
            if (e.Object is ProcessTaskStopRequest && app.TaskContext != null)
                app.TaskContext.Task.Stop();
        }

        #endregion

        #endregion

        #endregion

    }
}
