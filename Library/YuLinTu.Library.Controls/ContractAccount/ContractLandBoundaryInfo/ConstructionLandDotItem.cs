/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 界址点界面显示实体
    /// </summary>
    public class ConstructionLandDotItem : NotifyCDObject
    {
        #region Property

        /// <summary>
        /// 是否可以编辑
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// 是否有用
        /// </summary>
        public bool IsValidUI
        {
            get { return isValidUI; }
            set { isValidUI = value; NotifyPropertyChanged("IsValidUI"); }
        }
        private bool isValidUI;

        /// <summary>
        /// 点号
        /// </summary>
        public string DotNumberUI
        {
            get { return dotNumberUI; }
            set { dotNumberUI = value; NotifyPropertyChanged("DotNumberUI"); }
        }
        private string dotNumberUI;

        private string unityNumberUI;
        /// <summary>
        /// 统编号
        /// </summary>
        public string UnityNumberUI
        {
            get { return unityNumberUI; }
            set { unityNumberUI = value;NotifyPropertyChanged("UnityNumberUI"); }
        }
        /// <summary>
        /// X坐标值
        /// </summary>
        public string XCoordinateUI
        {
            get { return xCoordinateUI; }
            set { xCoordinateUI = value; NotifyPropertyChanged("XCoordinateUI"); }
        }
        private string xCoordinateUI;

        /// <summary>
        /// Y坐标值
        /// </summary>
        public string YCoordinateUI
        {
            get { return yCoordinateUI; }
            set { yCoordinateUI = value; NotifyPropertyChanged("YCoordinateUI"); }
        }
        private string yCoordinateUI;

        /// <summary>
        /// 界标类型
        /// </summary>
        public string DotMarkType
        {
            get { return dotMarkType; }
            set { dotMarkType = value; NotifyPropertyChanged("DotMarkType"); }
        }
        private string dotMarkType;

        /// <summary>
        /// 界址点类型
        /// </summary>
        public string DotType
        {
            get { return dotType; }
            set { dotType = value; NotifyPropertyChanged("DotType"); }
        }
        private string dotType;

        /// <summary>
        /// 是否可见
        /// </summary>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public int Img { get; set; }

        /// <summary>
        /// 界址点实体
        /// </summary>
        public BuildLandBoundaryAddressDot Entity { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConstructionLandDotItem()
        {
            IsEditable = true;
        }

        #endregion

        #region Methods

        #endregion
    }
}
