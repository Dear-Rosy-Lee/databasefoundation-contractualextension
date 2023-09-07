/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractRegeditBook
{
    /// <summary>
    /// ContractRegeditBookQRCodeConfig.xaml 的交互逻辑
    /// </summary>
    public partial class ContractRegeditBookQRCodeConfig : WorkpageOptionsEditor
    {
        public ContractRegeditBookQRCodeConfig(IWorkpage workpage):base(workpage)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                qrPanel.SaveTemplate();
            }));
            
        }

    }
}
