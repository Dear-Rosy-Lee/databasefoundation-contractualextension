/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using Microsoft.Win32;
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
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 初始化承包地块是否基本农田界面
    /// </summary>
    public partial class ContractLandInitialIsFarmer : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandInitialIsFarmer(bool isBatch)
        {
            InitializeComponent();
            this.Header = isBatch ? "批量初始化地块是否基本农田" : "初始化地块是否基本农田";
            btnConfirm.IsEnabled = false;
        }

        #endregion

        #region Field

        private Zone currentZone;

        #endregion

        #region Properties

        /// <summary>
        /// 当前选择地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                txtZoneName.Text = currentZone.FullName;
            }
        }

        /// <summary>
        /// Shape文件路径
        /// </summary>
        public string FileName { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 选择基本农田数据文件
        /// </summary>
        private void btnFarmerData_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.Filter = "ESRI Shapefile(*.shp)|*.shp";
            openDlg.Title = "请选择基本农田数据:";
            var val = openDlg.ShowDialog();
            if (val == null || !val.Value)
            {
                return;
            }
            txtFarmerData.Text = openDlg.FileName;
        }

        /// <summary>
        /// 文件框内容改变(根据内容是否为空或者是否存在决定按钮的可用性)
        /// </summary>
        private void txtFarmerData_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = txtFarmerData.Text.Trim();
            if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && !System.IO.File.Exists(name)))
            {
                this.FileName = string.Empty;
                btnConfirm.IsEnabled = false;
            }
            else
            {
                this.FileName = name;
                btnConfirm.IsEnabled = true;
            }
        }

        #endregion

    }
}
