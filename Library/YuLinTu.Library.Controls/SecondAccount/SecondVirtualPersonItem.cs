/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 二轮台账承包方绑定实体
    /// </summary>
    public class SecondVirtualPersonItem : VirtualPerson
    {
        #region Fields

        /// <summary>
        /// 承包方名称
        /// </summary>
        private string name;

        /// <summary>
        /// 台账总面积
        /// </summary>
        private double? tableArea;

        /// <summary>
        /// 可见性
        /// </summary>
        private Visibility visibility;

        /// <summary>
        /// 图片
        /// </summary>
        private bool img;

        #endregion

        #region Properties

        /// <summary>
        /// 承包方名称
        /// </summary>
        public new string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 台账总面积
        /// </summary>
        public double? TableArea
        {
            get { return tableArea; }
            set
            {
                tableArea = value;
                NotifyPropertyChanged("TableArea");
            }
        }

        /// <summary>
        /// 二轮台账地块
        /// </summary>
        public ObservableCollection<SecondLandBinding> Children
        {
            get;
            private set;
        }

        /// <summary>
        /// 可见性
        /// </summary>
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                NotifyPropertyChanged("Visibility");
            }
        }

        /// <summary>
        /// 图片
        /// </summary>
        public bool Img
        {
            get { return img; }
            set { img = value; NotifyPropertyChanged("Img"); }
        }

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Tag { get; set; }

        #endregion

        #region Ctor

        public SecondVirtualPersonItem()
        {
            Children = new ObservableCollection<SecondLandBinding>();
            this.Visibility = Visibility.Visible;
            //Img = true;
        }

        #endregion
    }

    /// <summary>
    /// 二轮台账地块绑定实体
    /// </summary>
    public class SecondLandBinding : SecondTableLand
    {
        #region Fields

        ///// <summary>
        ///// 地块图片
        ///// </summary>
        //private bool img;

        /// <summary>
        /// 地块可见性
        /// </summary>
        private Visibility visibility;

        #endregion

        #region Properties

        /// <summary>
        /// 地块可见性
        /// </summary>
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                NotifyPropertyChanged("Visibility");
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SecondLandBinding()
        {
            this.Visibility = Visibility.Visible;
        }

        #endregion
    }
}
