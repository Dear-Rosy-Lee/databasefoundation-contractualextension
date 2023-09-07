using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace YuLinTu.Component.MapFoundation
{
    class UIHelper
    {
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate (object f)
                {
                    ((DispatcherFrame)f).Continue = false;

                    return null;
                }
                    ), frame);
            Dispatcher.PushFrame(frame);
        }
        public static void ShowExceptionMessage(Exception ex)
        {
            MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
