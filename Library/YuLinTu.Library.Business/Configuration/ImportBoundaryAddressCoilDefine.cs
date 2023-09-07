/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入界址线图斑设置实体
    /// </summary>
    public class ImportBoundaryAddressCoilDefine : NotifyCDObject
    {
        #region Properties

        /// <summary>
        ///地域代码
        /// </summary>
        [DisplayLanguage("地域代码", IsLanguageName = false)]
        [DescriptionLanguage("地域代码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ZoneCodeIndex
        {
            get { return zoneCodeIndex; }
            set { zoneCodeIndex = value; NotifyPropertyChanged("ZoneCodeIndex"); }
        }
        private string zoneCodeIndex;


        /// <summary>
        /// 地块编码
        /// </summary>
        [DisplayLanguage("地块编码", IsLanguageName = false)]
        [DescriptionLanguage("地块编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandNumber
        {
            get { return landNumber; }
            set
            {
                landNumber = value;
                NotifyPropertyChanged("LandNumber");
            }
        }
        private string landNumber;

        /// <summary>
        /// 起点号
        /// </summary>
        [DisplayLanguage("起点号", IsLanguageName = false)]
        [DescriptionLanguage("起点号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string StartNumber
        {
            get { return startNumber; }
            set
            {
                startNumber = value;
                NotifyPropertyChanged("StartNumber");
            }
        }
        private string startNumber;

        /// <summary>
        /// 终点号
        /// </summary>
        [DisplayLanguage("终点号", IsLanguageName = false)]
        [DescriptionLanguage("终点号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string EndNumber
        {
            get { return endNumber; }
            set
            {
                endNumber = value;
                NotifyPropertyChanged("EndNumber");
            }
        }
        private string endNumber;

        /// <summary>
        /// 界址线长度
        /// </summary>
        [DisplayLanguage("界址线长度", IsLanguageName = false)]
        [DescriptionLanguage("界址线长度", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string CoilLength
        {
            get { return coilLength; }
            set
            {
                coilLength = value;
                NotifyPropertyChanged("CoilLength");
            }
        }
        private string coilLength;


        /// <summary>
        ///界线性质(见城镇地籍表29)
        /// </summary>
        [DisplayLanguage("界线性质", IsLanguageName = false)]
        [DescriptionLanguage("界线性质", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LineType
        {
            get { return lineType; }
            set
            {
                lineType = value;
                NotifyPropertyChanged("LineType");
            }
        }
        private string lineType;

        /// <summary>
        ///界址线类别(围墙、墙壁、红线、界线)
        /// </summary>
        [DisplayLanguage("界址线类别", IsLanguageName = false)]
        [DescriptionLanguage("界址线类别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string CoilType
        {
            get { return coilType; }
            set
            {
                coilType = value;
                NotifyPropertyChanged("CoilType");
            }
        }
        private string coilType;

        /// <summary>
        ///界址线位置(内、中、外)
        /// </summary>
        [DisplayLanguage("界址线位置", IsLanguageName = false)]
        [DescriptionLanguage("界址线位置", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string Position
        {
            get { return position; }
            set
            {
                position = value;
                NotifyPropertyChanged("Position");
            }
        }
        private string position;

        /// <summary>
        ///界址线所属权利类型
        /// </summary>        
        [DisplayLanguage("权属类型", IsLanguageName = false)]
        [DescriptionLanguage("权属类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandType
        {
            get { return landType; }
            set
            {
                landType = value;
                NotifyPropertyChanged("LandType");
            }
        }
        private string landType;


        /// <summary>
        ///界址线说明
        /// </summary>
        [DisplayLanguage("界址线说明", IsLanguageName = false)]
        [DescriptionLanguage("界址线说明", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }
        private string description;

        /// <summary>
        ///毗邻地块承包方
        /// </summary>
        [DisplayLanguage("毗邻地块承包方", IsLanguageName = false)]
        [DescriptionLanguage("毗邻地块承包方", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string NeighborPerson
        {
            get { return neighborPerson; }
            set
            {
                neighborPerson = value;
                NotifyPropertyChanged("NeighborPerson");
            }
        }
        private string neighborPerson;

        /// <summary>
        ///毗邻地块指界人
        /// </summary>
        [DisplayLanguage("毗邻地块指界人", IsLanguageName = false)]
        [DescriptionLanguage("毗邻地块指界人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string NeighborFefer
        {
            get { return neighborFefer; }
            set
            {
                neighborFefer = value;
                NotifyPropertyChanged("NeighborFefer");
            }
        }
        private string neighborFefer;
                
        /// <summary>
        ///界址线顺序号
        /// </summary>
        [DisplayLanguage("界址线顺序号", IsLanguageName = false)]
        [DescriptionLanguage("界址线顺序号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string OrderID
        {
            get { return orderID; }
            set
            {
                orderID = value;
                NotifyPropertyChanged("OrderID");
            }
        }
        private string orderID;
        
        /// <summary>
        /// 起点号
        /// </summary>
        [DisplayLanguage("起点ID", IsLanguageName = false)]
        [DescriptionLanguage("起点ID", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string StartID
        {
            get { return startid; }
            set
            {
                startid = value;
                NotifyPropertyChanged("StartID");
            }
        }
        private string startid;

        /// <summary>
        /// 终点号
        /// </summary>
        [DisplayLanguage("终点ID", IsLanguageName = false)]
        [DescriptionLanguage("终点ID", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string EndID
        {
            get { return endid; }
            set
            {
                endid = value;
                NotifyPropertyChanged("EndID");
            }
        }
        private string endid;

        #endregion


        public ImportBoundaryAddressCoilDefine()
        {
            lineType = "None";
            coilType = "None";
            landType = "None";
            position = "None";
            description = "None";
            neighborFefer = "None";
            neighborPerson = "None";
            landNumber = "None";
            coilLength = "None";
            startNumber = "None";
            endNumber = "None";
            zoneCodeIndex = "None";
            orderID = "None";
            startid = "None";
            endid = "None";
        }


        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static ImportBoundaryAddressCoilDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ImportBoundaryAddressCoilDefine>();
            var section = profile.GetSection<ImportBoundaryAddressCoilDefine>();
            return section.Settings;
        }


    }
}
