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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.Data;

namespace YuLinTu.Component.Common
{
    /// <summary>
    /// SelectedSummaryExportZoneTB.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedSummaryExportZoneTB : MetroTextBox
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectedSummaryExportZoneTB()
        {
            InitializeComponent();
            this.DataContext = this;
            btSelectPersons.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion Ctor

        #region Properties

        /// <summary>
        /// 依赖属性地域全名称
        /// </summary>
        public string SelectZoneAndPersonInfo
        {
            get { return (string)GetValue(SelectZoneAndPersonInfoProperty); }
            set { SetValue(SelectZoneAndPersonInfoProperty, value); }
        }

        public static readonly DependencyProperty SelectZoneAndPersonInfoProperty = DependencyProperty.Register("SelectZoneAndPersonInfo", typeof(string), typeof(SelectedSummaryExportZoneTB), new PropertyMetadata((s, a) =>
           {
               var nameAndCodeinfo = a.NewValue as string;
               if (!nameAndCodeinfo.IsNullOrBlank())
                   (s as MetroTextBox).Text = (nameAndCodeinfo.Split('#'))[0];
           }));

        /// <summary>
        /// 工作空间属性
        /// </summary>
        public IWorkpage WorkPage { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public IDbContext DbContext { get; set; }

        #endregion Properties

        #region Event

        /// <summary>
        /// 地域选择按钮选择
        /// </summary>
        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            string[] nameAndCode = this.SelectZoneAndPersonInfo.IsNullOrBlank() ? null : this.SelectZoneAndPersonInfo.Split('#');
            ZoneSelectorPanel zoneSelectPage = new ZoneSelectorPanel();
            zoneSelectPage.DbContext = DbContext;
            zoneSelectPage.SelectorZone = new ZoneDataItem() { FullCode = nameAndCode == null ? string.Empty : nameAndCode[1] };

            WorkPage.Workspace.Window.ShowDialog(zoneSelectPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                var currentZone = zoneSelectPage.RootZone;
                if (currentZone == null)
                {
                    SelectZoneAndPersonInfo = string.Empty;
                }
                else
                {
                    SelectZoneAndPersonInfo = currentZone.FullName + "#" + currentZone.FullCode;
                }
            });
        }

        private void ImageButton_Click_2(object sender, RoutedEventArgs e)
        {
        }

        #endregion Event
    }
}