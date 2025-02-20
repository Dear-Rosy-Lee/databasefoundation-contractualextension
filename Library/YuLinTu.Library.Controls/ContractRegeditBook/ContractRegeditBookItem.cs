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
    /// 权证绑定实体
    /// </summary>
    public class ContractRegeditBookItem : NotifyCDObject
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
        public ObservableCollection<BindContractRegeditBook> Children { get; set; }

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ContractRegeditBookItem()
        {
            Children = new ObservableCollection<BindContractRegeditBook>();
            img = 1;
        }

        #endregion

        #region Methods

        #endregion
    }

    /// <summary>
    /// 权证绑定实体
    /// </summary>
    public class BindContractRegeditBook : NotifyCDObject
    {
        #region Filds

        private Visibility visibility;
        private string year;
        private string number;
        private string regeditNumber;
        private string serialNumber;
        private string senderName;

        private string countActualArea;
        private string countAwareArea;
        private int countLand;
        private int countPrint;
        private string contractTime;
        private int img;
        private string comment;
        private string contractRegeditBookExcursus;

        #endregion

        #region Property

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 权证年号
        /// </summary>      
        public string Name
        {
            get { return year; }
            set
            {
                year = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 权证编号（组级地域+4位流水号）
        /// </summary>       
        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                if (string.IsNullOrEmpty(number))
                    return;
                number = number.Trim();
                NotifyPropertyChanged("Number");
            }
        }

        /// <summary>
        ///登记薄编号
        /// </summary>       
        public string RegeditNumber
        {
            get { return regeditNumber; }
            set
            {
                regeditNumber = value;
                if (string.IsNullOrEmpty(regeditNumber))
                    return;
                regeditNumber = regeditNumber.Trim();
                NotifyPropertyChanged("RegeditNumber");
            }
        }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNumber
        {
            get { return serialNumber; }
            set
            {
                serialNumber = value;
                if (string.IsNullOrEmpty(serialNumber))
                    return;
                serialNumber = serialNumber.Trim();
                NotifyPropertyChanged("SerialNumber");
            }
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
        ///实测总面积
        /// </summary>       
        public string CountActualArea
        {
            get { return countActualArea; }
            set { countActualArea = value; NotifyPropertyChanged("CountActualArea"); }
        }
        /// <summary>
        ///确权总面积
        /// </summary>       
        public string CountAwareArea
        {
            get { return countAwareArea; }
            set { countAwareArea = value; NotifyPropertyChanged("CountAwareArea"); }
        }

        /// <summary>
        ///地块总数
        /// </summary>       
        public int CountLand
        {
            get { return countLand; }
            set { countLand = value; NotifyPropertyChanged("CountLand"); }
        }
        public string ContractRegeditBookExcursus
        {
            get { return contractRegeditBookExcursus; }
            set { contractRegeditBookExcursus = value; NotifyPropertyChanged("ContractRegeditBookExcursus"); }
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
        /// 打印次数
        /// </summary>
        public int CountPrint
        {
            get { return countPrint; }
            set { countPrint = value; NotifyPropertyChanged("CountPrint"); }
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
        /// 权证
        /// </summary>
        public ContractRegeditBook Tag { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 构造方法
        /// </summary>
        public BindContractRegeditBook(ContractRegeditBook regeditBook, ContractConcord concord, int landCout)
        {
            if (regeditBook == null)
            {
                return;
            }
            ID = regeditBook.ID;
            string SNumber = string.IsNullOrEmpty(regeditBook.SerialNumber) ? "" : regeditBook.SerialNumber;
            if (SNumber.Trim().Equals(""))
                SNumber = "/无";
            else if (SNumber.Trim().Equals("/"))
                SNumber = "/无";
            else
                SNumber = "/" + SNumber.PadLeft(6, '0');
            Name = regeditBook.Year != null ? regeditBook.Year : "";
            Name += SNumber;
            Number = regeditBook.Number != null ? regeditBook.Number : "";
            if (regeditBook.SerialNumber != null)
            {
                int sernumber = 0;
                int.TryParse(regeditBook.SerialNumber, out sernumber);
                SerialNumber = sernumber == 0 ? regeditBook.SerialNumber : sernumber.ToString();
            }
            RegeditNumber = regeditBook.RegeditNumber != null ? regeditBook.RegeditNumber : "";
            SenderName = concord.SenderName != null ? concord.SenderName : "";
            CountActualArea = ToolMath.SetNumbericFormat(concord.CountActualArea.ToString(), 2);
            CountAwareArea = ToolMath.SetNumbericFormat(concord.CountAwareArea.ToString(), 2);
            CountLand = landCout;
            ContractTime = concord.ManagementTime != null ?
                (concord.ManagementTime.TrimEnd('年') + "年") : "";
            CountPrint = regeditBook.Count;
            Comment = regeditBook.Comment != null ? regeditBook.Comment : "";
            ContractRegeditBookExcursus = regeditBook.ContractRegeditBookExcursus != null ? regeditBook.ContractRegeditBookExcursus : "";

            Img = 2;
            Tag = regeditBook;
        }

        #endregion
    }
}
