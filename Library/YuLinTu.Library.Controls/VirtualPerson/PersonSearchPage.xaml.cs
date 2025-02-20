/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 查找承包方
    /// </summary>
    public partial class PersonSearchPage : MetroWindowBase
    {
        #region Fields

        public delegate SearchNumber SearchVirtualPerson(string name, string code, int type);

        #endregion

        #region Propertys

        /// <summary>
        /// 名称
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string PersonCode { get; set; }

        /// <summary>
        /// 查询方式 正常(2),向上(1),向下(3)
        /// </summary>
        public int SearchType { get; set; }

        /// <summary>
        /// 查询委托
        /// </summary>
        public SearchVirtualPerson Search { get; set; }

        #endregion

        #region Public

        /// <summary>
        /// 构造方法
        /// </summary>
        public PersonSearchPage()
        {
            InitializeComponent();
            btnDown.IsEnabled = false;
            btnUp.IsEnabled = false;
        }

        #endregion

        #region Events

        /// <summary>
        /// 查询上一个
        /// </summary>
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            PersonName = txtName.Text.Trim();
            PersonCode = txtNumber.Text.Trim();
            SearchType = 1;
            if (Search == null)
            {
                return;
            }
            var isEn = Search(PersonName, PersonCode, SearchType);
            if (isEn.DataCount > 0)
            {
                if (isEn.CurrentIndex == 0 && isEn.DataCount>1)
                {
                    btnDown.IsEnabled = true;
                    btnUp.IsEnabled = false;
                }
                else if (isEn.DataCount == 1)
                {
                    btnDown.IsEnabled = false;
                    btnUp.IsEnabled = false;
                }
                else
                {
                    btnDown.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 查询下一个
        /// </summary>
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            PersonName = txtName.Text.Trim();
            PersonCode = txtNumber.Text.Trim();
            SearchType = 3;
            if (Search == null)
            {
                return;
            }
            var isEn = Search(PersonName, PersonCode, SearchType);
            if (isEn.DataCount > 0)
            {
                if (isEn.DataCount - 1 == isEn.CurrentIndex && isEn.DataCount > 1)
                {
                    btnDown.IsEnabled = false;
                    btnUp.IsEnabled = true;
                }
                else if (isEn.DataCount == 1)
                {
                    btnDown.IsEnabled = false;
                    btnUp.IsEnabled = false;
                }
                else
                {
                    btnUp.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            PersonName = txtName.Text.Trim();
            PersonCode = txtNumber.Text.Trim();
            SearchType = 2;
            if (Search != null)
            {
                var en = Search(PersonName, PersonCode, SearchType);
                if (en.DataCount == 0)
                {
                    btnDown.IsEnabled = true;
                    btnUp.IsEnabled = true;
                }
                else
                {
                    btnDown.IsEnabled = true;
                    btnUp.IsEnabled = false;
                }
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) && string.IsNullOrEmpty(txtNumber.Text))
            {
                btnSearch.IsEnabled = false;
            }
            else
            {
                btnSearch.IsEnabled = true;
            }
            btnDown.IsEnabled = false;
            btnUp.IsEnabled = false;
        }

        private void txtNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) && string.IsNullOrEmpty(txtNumber.Text))
            {
                btnSearch.IsEnabled = false;
            }
            else
            {
                btnSearch.IsEnabled = true;
            }
            btnDown.IsEnabled = false;
            btnUp.IsEnabled = false;
        }

        #endregion
    }
}
