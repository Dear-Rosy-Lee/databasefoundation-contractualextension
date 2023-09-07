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
    /// 承包地块界址线界面显示实体
    /// </summary>
    public class ConstructionLandCoilItem : NotifyCDObject
    {
        #region Property

        /// <summary>
        /// 序号
        /// </summary>
        public string CoilNumberUI
        {
            get { return coilNumberUI; }
            set { coilNumberUI = value; NotifyPropertyChanged("CoilNumberUI"); }
        }
        private string coilNumberUI;

        /// <summary>
        /// 起点
        /// </summary>
        public string StartPointUI
        {
            get { return startPointUI; }
            set { startPointUI = value; NotifyPropertyChanged("StartPointUI"); }
        }
        private string startPointUI;

        /// <summary>
        /// 终点
        /// </summary>
        public string EndPointUI
        {
            get { return endPointUI; }
            set { endPointUI = value; NotifyPropertyChanged("EndPointUI"); }
        }
        private string endPointUI;

        /// <summary>
        /// 长度
        /// </summary>
        public string CoilLengthUI
        {
            get { return coilLengthUI; }
            set { coilLengthUI = value; NotifyPropertyChanged("CoilLengthUI"); }
        }
        private string coilLengthUI;

        /// <summary>
        /// 界址线性质
        /// </summary>
        public string CoilPropertyUI
        {
            get { return coilPropertyUI; }
            set { coilPropertyUI = value; NotifyPropertyChanged("CoilPropertyUI"); }
        }
        private string coilPropertyUI;

        /// <summary>
        /// 界址线类别
        /// </summary>
        public string CoilTypeUI
        {
            get { return coilTypeUI; }
            set { coilTypeUI = value; NotifyPropertyChanged("CoilTypeUI"); }
        }
        private string coilTypeUI;

        /// <summary>
        /// 界址线位置
        /// </summary>
        public string CoilLocatioonUI
        {
            get { return coilLocatioonUI; }
            set { coilLocatioonUI = value; NotifyPropertyChanged("CoilLocatioonUI"); }
        }
        private string coilLocatioonUI;

        /// <summary>
        /// 毗邻承包方
        /// </summary>
        public string NeighborObligeeUI
        {
            get { return neighborObligeeUI; }
            set { neighborObligeeUI = value; NotifyPropertyChanged("NeighborObligeeUI"); }
        }
        private string neighborObligeeUI;

        /// <summary>
        /// 毗邻指界人
        /// </summary>
        public string NeighborReferorUI
        {
            get { return neighborReferorUI; }
            set { neighborReferorUI = value; NotifyPropertyChanged("NeighborReferorUI"); }
        }
        private string neighborReferorUI;
     
        /// <summary>
        /// 是否可见
        /// </summary>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public int Img { get; set; }

        /// <summary>
        /// 界址线实体
        /// </summary>
        public BuildLandBoundaryAddressCoil Entity { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConstructionLandCoilItem()
        {
        }

        #endregion

        #region Methods

        #endregion
    }
}
