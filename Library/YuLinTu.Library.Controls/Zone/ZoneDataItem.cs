/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 地域绑定实体
    /// </summary>
    public class ZoneDataItem : Zone
    {
        #region Property

        /// <summary>
        /// 子地域
        /// </summary>
        public ObservableCollection<ZoneDataItem> Children { get;private set; }

        /// <summary>
        /// 图片
        /// </summary>
        public BitmapImage Img { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public Visibility Visibility { get; set; }
        public object ObjectExtension { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ZoneDataItem()
        {
            Children = new ObservableCollection<ZoneDataItem>();
        }

        #endregion

        #region Methods

        #endregion
    }
}
