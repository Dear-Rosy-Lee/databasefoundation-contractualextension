/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 合同绑定实体
    /// </summary>
    public class ConcordItem : NotifyCDObject
    {
        #region Fields

        private Visibility visibility;
        private string name;
        private int img;

        #endregion

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public int Img
        {
            get { return img; }
            set { img = value; NotifyPropertyChanged("Img"); }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        /// <summary>
        /// 是否可见
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
        /// 承包方
        /// </summary>
        public VirtualPerson Tag { get; set; }

        /// <summary>
        /// 合同
        /// </summary>
        public ObservableCollection<BindConcord> Children { get; set; }

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConcordItem()
        {
            Children = new ObservableCollection<BindConcord>();
            img = 1;
        }

        #endregion

        #region Methods

        #endregion
    }

    /// <summary>
    /// 合同绑定实体
    /// </summary>
    public class BindConcord : NotifyCDObject
    {
        #region Filds

        private Visibility visibility;
        private string name;
        private string senderName;
        private string concordNumber;
        private string concordArea;
        private string actualArea;
        private string awareArea;
        private string contractWay;
        private string landPurpose;
        private string contractTime;
        private int img;
        private string comment;

        #endregion

        #region Property

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 户号
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        /// <summary>
        /// 发包方名称
        /// </summary>
        public string SenderName
        {
            get { return senderName; }
            set { senderName = value; NotifyPropertyChanged("SenderName"); }
        }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string ConcordNumber
        {
            get { return concordNumber; }
            set { concordNumber = value; NotifyPropertyChanged("ConcordNumber"); }
        }

        /// <summary>
        /// 合同面积
        /// </summary>
        public string ConcordArea
        {
            get { return concordArea; }
            set { concordArea = value; NotifyPropertyChanged("ConcordArea"); }
        }

        /// <summary>
        /// 实测面积
        /// </summary>
        public string ActualArea
        {
            get { return actualArea; }
            set { actualArea = value; NotifyPropertyChanged("ActualArea"); }
        }

        /// <summary>
        /// 确权面积
        /// </summary>
        public string AwareArea
        {
            get { return awareArea; }
            set { awareArea = value; NotifyPropertyChanged("AwareArea"); }
        }

        /// <summary>
        /// 承包方式
        /// </summary>
        public string ContractWay
        {
            get { return contractWay; }
            set { contractWay = value; NotifyPropertyChanged("ContractWay"); }
        }

        /// <summary>
        /// 土地用途
        /// </summary>
        public string LandPurpose
        {
            get { return landPurpose; }
            set { landPurpose = value; NotifyPropertyChanged("LandPurpose"); }
        }

        /// <summary>
        /// 承包期限
        /// </summary>
        public string ContractTime
        {
            get { return contractTime; }
            set { contractTime = value; NotifyPropertyChanged("ContractTime"); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; NotifyPropertyChanged("Comment"); }
        }

        /// <summary>
        /// 是否可见
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
        public int Img
        {
            get { return img; }
            set { img = value; NotifyPropertyChanged("Img"); }
        }

        /// <summary>
        /// 合同
        /// </summary>
        public ContractConcord Tag { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 构造方法
        /// </summary>
        public BindConcord(ContractConcord concord, List<Dictionary> contractWayList, List<Dictionary> landPurposeList)
        {
            if (concord == null)
            {
                return;
            }
            ID = concord.ID;
            Dictionary wayDic = contractWayList.Find(t => t.Code == concord.ArableLandType);
            Dictionary purposeDic = landPurposeList.Find(t => t.Code == concord.LandPurpose);
            SenderName = concord.SenderName;
            ConcordNumber = concord.ConcordNumber;
            ConcordArea = concord.TotalTableArea == null ? "0.00" : ToolMath.SetNumbericFormat(concord.TotalTableArea.Value.ToString(), 2);// concord.TotalTableArea.Value.ToString("f2");
            ActualArea = concord.CountActualArea == 0 ? "0.00" : ToolMath.SetNumbericFormat(concord.CountActualArea.ToString(), 2);
            AwareArea = concord.CountAwareArea == 0 ? "0.00" : ToolMath.SetNumbericFormat(concord.CountAwareArea.ToString(), 2) ;
            ContractWay = wayDic == null ? "" : wayDic.Name;
            LandPurpose = purposeDic == null ? "" : purposeDic.Name;
            if (concord.ManagementTime == "长久")
                ContractTime = concord.ManagementTime;
            else
                ContractTime = concord.ManagementTime + "年";
            Comment = concord.Comment;
            Img = 2;
            Tag = concord;
        }

        #endregion
    }
}
