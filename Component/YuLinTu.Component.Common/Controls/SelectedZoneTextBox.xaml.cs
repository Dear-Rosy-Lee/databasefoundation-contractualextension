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
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.Data;

namespace YuLinTu.Component.Common
{
    /// <summary>
    /// 选择地域文本框界面
    /// </summary>
    public partial class SelectedZoneTextBox : MetroTextBox
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectedZoneTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 依赖属性地域全名称
        /// </summary>
        public string FullZoneNameAndCode
        {
            get { return (string)GetValue(FullZoneNameAndCodeProperty); }
            set { SetValue(FullZoneNameAndCodeProperty, value); }
        }

        public static readonly DependencyProperty FullZoneNameAndCodeProperty = DependencyProperty.Register("FullZoneNameAndCode", typeof(string), typeof(SelectedZoneTextBox), new PropertyMetadata((s, a) =>
        {
            var nameAndCode = a.NewValue as string;
            if (!nameAndCode.IsNullOrBlank())
                (s as MetroTextBox).Text = (nameAndCode.Split('#'))[0];
        }));

        /// <summary>
        /// 工作空间属性
        /// </summary>
        public IWorkpage WorkPage { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public IDbContext DbContext { get; set; }

        #endregion

        #region Event

        /// <summary>
        /// 地域选择按钮选择
        /// </summary>
        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            //TODO 此处弹出地域选择界面
            string[] nameAndCode = this.FullZoneNameAndCode.IsNullOrBlank() ? null : this.FullZoneNameAndCode.Split('#');
            ZoneSelectorPanel zoneSelectPage = new ZoneSelectorPanel();
            zoneSelectPage.DbContext = DbContext;
            zoneSelectPage.SelectorZone = new ZoneDataItem() { FullCode = nameAndCode == null ? string.Empty : nameAndCode[1] };

            WorkPage.Workspace.Window.ShowDialog(zoneSelectPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                //TODO 地域传值
                var currentZone = zoneSelectPage.RootZone;
                if (currentZone == null)
                {
                    FullZoneNameAndCode = string.Empty;
                }
                else
                {
                    FullZoneNameAndCode = currentZone.FullName + "#" + currentZone.FullCode;
                }
            });
        }

        #endregion

    }
}
