/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using System.Threading;
using YuLinTu.Appwork;
using YuLinTu.Library.Controls;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using System.Collections;
using System.Reflection;
using System.IO;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// ImportShapeField.xaml 的交互逻辑
    /// </summary>
    public partial class ImportShapeField : InfoPageBase
    {

        /// <summary>
        /// 导入文件名称
        /// </summary>
        public string FileName { get; private set; }

        public ImportShapeField()
        {
            InitializeComponent();
            Confirm += ImportShapePage_Confirm;
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        private void ImportShapePage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                    {
                        string name = txt_file.Text.Trim();
                        if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && !System.IO.File.Exists(name)))
                        {
                            FileName = name;
                        }
                    }));

                e.Parameter = true;
            }
            catch
            {
                e.Parameter = false;
            }
        }

        private void txt_file_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string name = txt_file.Text.Trim();
            if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && !System.IO.File.Exists(name)))
            {
                FileName = string.Empty;
                btnExcuteImport.IsEnabled = false;
            }
            else
            {
                FileName = name;
                btnExcuteImport.IsEnabled = true;
            }
        }

        private void btnFileSelect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "文件类型(*.shp)|*.shp";
            var val = ofd.ShowDialog();
            if (val != null && val.Value)
            {
                txt_file.Text = ofd.FileName;
            }
            if (txt_file.Text == "") return;
        }

        private void btnExcuteImport_Click_1(object sender, RoutedEventArgs e)
        {
            ConfirmAsync();
        }



    }
}
