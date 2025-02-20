/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包台账承包方绑定实体
    /// </summary>
    public class ContractLandPersonItem : NotifyCDObject//: VirtualPerson
    {
        #region Fields

        /// <summary>
        /// 承包方户号
        /// </summary>
        private string vpNumber;

        /// <summary>
        /// 承包方可见性
        /// </summary>
        private Visibility visibility;

        /// <summary>
        /// 图片
        /// </summary>
        private int img;

        ///// <summary>
        ///// 承包方状态
        ///// </summary>
        //private eVirtualPersonStatus status;

        /// <summary>
        /// 承包方名称
        /// </summary>
        private string name;

        /// <summary>
        /// 单户的总实测面积
        /// </summary>
        private string sumActualArea;

        /// <summary>
        /// 单户的总确权面积
        /// </summary>
        private string sumAwareArea;

        /// <summary>
        /// 单户的总二轮合同面积
        /// </summary>
        private string sumTableArea;

        private string sumContractDelayArea;

        private string _quantificitionArea;
        private string _obligateArea;
        private string _landPurpose;
        private string oldLandCode = "1";

        #endregion Fields

        #region Properties

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// 承包方名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 原地块编码
        /// </summary>
        public string OldLandCode
        {
            get { return oldLandCode; }
            set
            {
                oldLandCode = value;
                NotifyPropertyChanged("OldLandCode");
            }
        }

        /// <summary>
        /// 承包方户号
        /// </summary>
        public string LandNumber
        {
            get { return vpNumber; }
            set
            {
                vpNumber = value;
                NotifyPropertyChanged("LandNumber");
            }
        }

        /// <summary>
        /// 承包台账地块
        /// </summary>
        public ObservableCollection<ContractLandBinding> Children
        {
            get;
            set;
        }

        /// <summary>
        /// 单户的总实测面积
        /// </summary>
        public string ActualAreaUI
        {
            get { return sumActualArea; }
            set
            {
                sumActualArea = value;
                NotifyPropertyChanged("ActualAreaUI");
            }
        }

        /// <summary>
        /// 单户的总确权面积
        /// </summary>
        public string AwareAreaUI
        {
            get { return sumAwareArea; }
            set
            {
                sumAwareArea = value;
                NotifyPropertyChanged("AwareAreaUI");
            }
        }

        /// <summary>
        /// 单户的总二轮合同面积
        /// </summary>
        public string TableAreaUI
        {
            get { return sumTableArea; }
            set
            {
                sumTableArea = value;
                NotifyPropertyChanged("TableAreaUI");
            }
        }

        public string ContractDelayAreaUI
        {
            get { return sumContractDelayArea; }
            set
            {
                sumContractDelayArea = value;
                NotifyPropertyChanged("ContractDelayAreaUI");
            }
        }

        /// <summary>
        /// 量化面积
        /// </summary>
        public string QuantificitionArea
        {
            get { return _quantificitionArea; }
            set
            {
                _quantificitionArea = value;
                NotifyPropertyChanged("QuantificitionArea");
            }
        }

        /// <summary>
        /// 预留面积
        /// </summary>
        public string ObligateArea
        {
            get { return _obligateArea; }
            set
            {
                _obligateArea = value;
                NotifyPropertyChanged("ObligateArea");
            }
        }

        /// <summary>
        /// 土地用途
        /// </summary>
        public string LandPurpose
        {
            get { return _landPurpose; }
            set
            {
                _landPurpose = value;
                NotifyPropertyChanged("LandPurpose");
            }
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
        public int Img
        {
            get { return img; }
            set { img = value; NotifyPropertyChanged("Img"); }
        }

        ///// <summary>
        ///// 承包方状态
        ///// </summary>
        //public eVirtualPersonStatus Status
        //{
        //    get { return status; }
        //    set { status = value; NotifyPropertyChanged("Status"); }
        //}

        /// <summary>
        /// 权利人
        /// </summary>
        public VirtualPerson Tag { get; set; }

        #endregion Properties

        #region Ctor

        public ContractLandPersonItem()
        {
            Img = 0;  //未被锁定承包方
            this.Visibility = Visibility.Collapsed;
            Children = new ObservableCollection<ContractLandBinding>();
        }

        #endregion Ctor
    }

    /// <summary>
    /// 台账地块绑定实体
    /// </summary>
    public class ContractLandBinding : ContractLand
    {
        #region Fields

        /// <summary>
        /// 地块可见性
        /// </summary>
        private Visibility visibility;

        /// <summary>
        /// 地块类别名称
        /// </summary>
        private string landCategoryUI;

        /// <summary>
        /// 地力等级名称
        /// </summary>
        private string landLevelUI;

        /// <summary>
        /// 是否基本农田
        /// </summary>
        private string isFarmerLandUI;

        /// <summary>
        /// 底层实体
        /// </summary>
        private ContractLand tag;

        /// <summary>
        /// 二轮合同面积
        /// </summary>
        private string tableAreaUI;

        private string contractDelayAreaUI;

        /// <summary>
        /// 实测面积
        /// </summary>
        private string actualAreaUI;

        /// <summary>
        /// 确权面积
        /// </summary>
        private string awareAreaUI;

        private string oldLandCode;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 原地块编码
        /// </summary>
        public string OldLandCode
        {
            get { return oldLandCode; }
            set
            {
                oldLandCode = value;
                NotifyPropertyChanged("OldLandCode");
            }
        }


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

        /// <summary>
        /// 二轮合同面积
        /// </summary>
        public string TableAreaUI
        {
            get { return tableAreaUI; }
            set
            {
                tableAreaUI = value;
                NotifyPropertyChanged("TableAreaUI");
            }
        }

        /// <summary>
        /// 延包面积
        /// </summary>
        public string ContractDelayAreaUI
        {
            get { return contractDelayAreaUI; }
            set
            {
                contractDelayAreaUI = value;
                NotifyPropertyChanged("ContractDelayAreaUI");
            }
        }

        /// <summary>
        /// 实测面积
        /// </summary>
        public string ActualAreaUI
        {
            get { return actualAreaUI; }
            set
            {
                actualAreaUI = value;
                NotifyPropertyChanged("ActualAreaUI");
            }
        }

        /// <summary>
        /// 确权面积
        /// </summary>
        public string AwareAreaUI
        {
            get { return awareAreaUI; }
            set
            {
                awareAreaUI = value;
                NotifyPropertyChanged("AwareAreaUI");
            }
        }

        /// <summary>
        /// 地块类别名称
        /// </summary>
        public string LandCategoryUI
        {
            get { return landCategoryUI; }
            set
            {
                landCategoryUI = value;
                NotifyPropertyChanged("LandCategoryUI");
            }
        }

        /// <summary>
        /// 地力等级名称
        /// </summary>
        public string LandLevelUI
        {
            get { return landLevelUI; }
            set
            {
                landLevelUI = value;
                NotifyPropertyChanged("LandLevelUI");
            }
        }

        /// <summary>
        /// 是否基本农田
        /// </summary>
        public string IsFarmerLandUI
        {
            get { return isFarmerLandUI; }
            set
            {
                isFarmerLandUI = value;
                NotifyPropertyChanged("IsFarmerLandUI");
            }
        }

        /// <summary>
        /// 底层承包地块
        /// </summary>
        public ContractLand Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                NotifyPropertyChanged("Tag");
            }
        }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandBinding(ContractLand land)
        {
            Tag = land;
            Name = land.Name;
            LandNumber = land.LandNumber;
            TableAreaUI = (land.TableArea == 0 || land.TableArea == null) ? "0.00" : ToolMath.SetNumbericFormat(land.TableArea.ToString(), 2);//land.TableArea.Value.ToString("f2");
            ActualAreaUI = land.ActualArea == 0 ? "0.00" : ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2);//land.ActualArea.ToString("f2");
            AwareAreaUI = land.AwareArea == 0 ? "0.00" : ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2);//land.AwareArea.ToString("f2");
            ContractDelayAreaUI = land.ContractDelayArea == 0 ? "0.00" : ToolMath.SetNumbericFormat(land.ContractDelayArea.ToString(), 2);
            LandName = land.LandName;
            Comment = land.Comment;
            Img = land.Shape != null ? 1 : 2;
            IsStockLand = land.IsStockLand;
            OldLandCode = land.OldLandNumber;
            //Tag.Shape = null;
            //if (land.Shape != null)
            //{
            //    Img = 1;
            //}
            //else
            //{
            //    Img = 2;
            //}
            this.Visibility = Visibility.Visible;
        }

        #endregion Ctor
    }
}