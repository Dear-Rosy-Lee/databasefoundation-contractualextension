/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
   public class ImportMZDWDefine: NotifyCDObject
    {
      
        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DisplayLanguage("标识码", IsLanguageName = false)]
        [DescriptionLanguage("标识码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "面状地物信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string BSM
        {
            get { return bsm; }
            set { bsm = value; NotifyPropertyChanged("BSM"); }
        }
        private string bsm;

        /// <summary>
        ///要素代码
        /// </summary>
        [DisplayLanguage("要素代码", IsLanguageName = false)]
        [DescriptionLanguage("要素代码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "面状地物信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string YSDM
        {
            get { return ysdm; }
            set { ysdm = value; NotifyPropertyChanged("YSDM"); }
        }
        private string ysdm;

        /// <summary>
        ///地物名称
        /// </summary>
        [DisplayLanguage("地物名称", IsLanguageName = false)]
        [DescriptionLanguage("地物名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "面状地物信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string DWMC
        {
            get { return dwmc; }
            set { dwmc = value; NotifyPropertyChanged("DWMC"); }
        }
        private string dwmc;

        /// <summary>
        ///面积
        /// </summary>
        [DisplayLanguage("面积", IsLanguageName = false)]
        [DescriptionLanguage("面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "面状地物信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string MJ
        {
            get { return mj; }
            set { mj = value; NotifyPropertyChanged("MJ"); }
        }
        private string mj;

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayLanguage("备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "面状地物信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string Comment
        {
            get { return comment; }
            set { comment = value; NotifyPropertyChanged("Comment"); }
        }
        private string comment;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportMZDWDefine()
        {
            BSM = "None";
            YSDM = "None";
            DWMC = "None";
            MJ = "None";
            Comment = "None";
        }

        #endregion
    }
}
