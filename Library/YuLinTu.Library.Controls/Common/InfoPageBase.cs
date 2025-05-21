/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 自定义窗口基类
    /// </summary>
    public abstract class InfoPageBase : TabDialog
    {
        #region Fields
        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public InfoPageBase()
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/Utils.Controls;component/Styles/AutoComplete.Styles.xaml", UriKind.RelativeOrAbsolute);
            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        #endregion

        #region Methods

        #endregion
    }
}