using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    public class ZoneNavigatorCommands
    {
        #region Copy

        public static RoutedCommand CopyCommand
        {
            get
            {
                if (cmdCopy == null)
                    cmdCopy = new RoutedCommand("Copy", typeof(ZoneNavigatorCommands));
                return cmdCopy;
            }
        }
        private static RoutedCommand cmdCopy;

        public static CommandBinding CopyCommandBinding
        {
            get
            {
                if (cmdBindingCopy == null)
                    cmdBindingCopy = new CommandBinding(CopyCommand, OnCopyCommandExecuted);
                return cmdBindingCopy;
            }
        }
        private static CommandBinding cmdBindingCopy;

        private static void OnCopyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var navigator = sender as TreeViewNavigator;
            if (navigator == null)
                return;
            if (navigator.SelectedItem == null)
                return;
            var zone = navigator.SelectedItem.Object as Zone;
            if (zone == null)
                return;

            Clipboard.SetText(zone.FullCode);
        }

        #endregion
    }
}
