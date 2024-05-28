/*
 * (C) 2014  鱼鳞图公司版权所有,保留所有权利
 * http://www.yulintu.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using Quality.Business.Entity;


namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 配置类
    /// </summary>
    public partial class FileChoiceControl : UserControl
    {
        #region Property

        /// <summary>
        /// 选择文件地址
        /// </summary>
        public string ImportFilePath
        {
            get
            {
                return (string)GetValue(ImportFilePathProperty);
            }
            set
            {
                SetValue(ImportFilePathProperty, value);
            }
        }

        public static readonly DependencyProperty ImportFilePathProperty =
            DependencyProperty.Register("ImportFilePath", typeof(string), typeof(FileChoiceControl));


        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage
        {
            get { return (IWorkpage)GetValue(WorkpageProperty); }
            set { SetValue(WorkpageProperty, value); }
        }

        public static readonly DependencyProperty WorkpageProperty =
            DependencyProperty.Register("Workpage", typeof(IWorkpage), typeof(FileChoiceControl));

        /// <summary>
        /// 绑定参数
        /// </summary>
        public object MetaData
        {
            get { return (object)GetValue(MetaDataProperty); }
            set { SetValue(MetaDataProperty, value); }
        }

        public static readonly DependencyProperty MetaDataProperty =
            DependencyProperty.Register("MetaData", typeof(object), typeof(FileChoiceControl));

        #endregion

        #region Ctor

        public FileChoiceControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 配置矢量元数据
        /// </summary>
        private void mbtnMeta_Click(object sender, RoutedEventArgs e)
        {
            mbtnMeta.IsEnabled = false;
            var dialog = new FileChoicePage();
            dialog.FileZonetConfig = MetaData as FileZoneEntity;            
            dialog.Workpage = Workpage;
            dialog.ImportFilePath = ImportFilePath;
            Workpage.Page.ShowMessageBox(dialog, (b, r) =>
            {
                mbtnMeta.IsEnabled = true;
                if (b == null || !b.Value)
                    return;
                MetaData = dialog.FileZonetConfig;
            });
        }

        #endregion
    }
}
