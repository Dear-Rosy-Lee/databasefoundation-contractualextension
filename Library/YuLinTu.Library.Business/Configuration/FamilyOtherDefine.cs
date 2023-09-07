/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NPOI.OpenXmlFormats.Dml.Chart;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方其他设置实体类
    /// </summary>
    public class FamilyOtherDefine : NotifyCDObject
    {
        #region Fields

        private bool hasTelephoneNumber;
        private bool hasFamilyRelation;
        private bool isCheckCardNumber;
        private bool isPromiseCardNumberNull;
        private bool showFamilyInfomation;
        private bool familyInstructionDate;
        private bool proxyDefineDate;
        private bool uniqueInstructionDate;
        private bool numberIcnValueRepeat;
        private bool surveyRequireDate;

        #endregion

        #region Method

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static FamilyOtherDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<FamilyOtherDefine>();
            var section = profile.GetSection<FamilyOtherDefine>();
            return  section.Settings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 检查电话号码是否填写
        /// </summary>
        public bool HasTelephoneNumber
        {
            get { return hasTelephoneNumber; }
            set { hasTelephoneNumber = value; NotifyPropertyChanged("HasTelephoneNumber"); }
        }

        /// <summary>
        /// 检查共有人家庭关系是否填写
        /// </summary>
        public bool HasFamilyRelation
        {
            get { return hasFamilyRelation; }
            set { hasFamilyRelation = value; NotifyPropertyChanged("HasFamilyRelation"); }
        }

        /// <summary>
        /// 检查证件号码是否填写及正确性
        /// </summary>
        public bool IsCheckCardNumber
        {
            get { return isCheckCardNumber; }
            set
            {
                isCheckCardNumber = value;
                if (isCheckCardNumber)
                {
                    isPromiseCardNumberNull = false;
                }
                _isPromiseCardNumberNullEnable = !isCheckCardNumber;
                NotifyPropertyChanged("IsPromiseCardNumberNullEnable");
                NotifyPropertyChanged("IsPromiseCardNumberNull");
                NotifyPropertyChanged("IsCheckCardNumber");
            }
        }

        private bool _isPromiseCardNumberNullEnable;

        /// <summary>
        /// 允许不填写证件号码是否可用
        /// </summary>
        public bool IsPromiseCardNumberNullEnable
        {
            get { return _isPromiseCardNumberNullEnable; }
            set
            {
                _isPromiseCardNumberNullEnable = value;
                NotifyPropertyChanged("IsPromiseCardNumberNullEnable");
            }
        }


        /// <summary>
        /// 允许不填写证件号码
        /// </summary>
        public bool IsPromiseCardNumberNull
        {
            get { return isPromiseCardNumberNull; }
            set
            {
                isPromiseCardNumberNull = value;
                NotifyPropertyChanged("IsPromiseCardNumberNull");
            }
        }

        /// <summary>
        /// 呈现数据是只呈现家庭户信息
        /// </summary>
        public bool ShowFamilyInfomation
        {
            get { return showFamilyInfomation; }
            set { showFamilyInfomation = value; NotifyPropertyChanged("ShowFamilyInfomation"); }
        }

        /// <summary>
        /// 设置户主代表声明书日期
        /// </summary>
        public bool FamilyInstructionDate
        {
            get { return familyInstructionDate; }
            set { familyInstructionDate = value; NotifyPropertyChanged("FamilyInstructionDate"); }
        }

        /// <summary>
        /// 设置委托代理声明书日期
        /// </summary>
        public bool ProxyDefineDate
        {
            get { return proxyDefineDate; }
            set { proxyDefineDate = value; NotifyPropertyChanged("ProxyDefineDate"); }
        }

        /// <summary>
        /// 设置公示无异议声明书日期
        /// </summary>
        public bool UniqueInstructionDate
        {
            get { return uniqueInstructionDate; }
            set { uniqueInstructionDate = value; NotifyPropertyChanged("UniqueInstructionDate"); }
        }

        /// <summary>
        /// 设置测绘申请书日期
        /// </summary>
        public bool SurveyRequireDate
        {
            get { return surveyRequireDate; }
            set { surveyRequireDate = value; NotifyPropertyChanged("SurveyRequireDate"); }
        }

        /// <summary>
        /// 允许调查表中身份证号码重复存在
        /// </summary>
        public bool NumberIcnValueRepeat
        {
            get { return numberIcnValueRepeat; }
            set { numberIcnValueRepeat = value; NotifyPropertyChanged("NumberIcnValueRepeat"); }
        }

        #endregion

        #region Ctor

        public FamilyOtherDefine()
        {
            HasTelephoneNumber = true;
            HasFamilyRelation = true;
            IsCheckCardNumber = false;
            IsPromiseCardNumberNull = true;
            ShowFamilyInfomation = false;
            FamilyInstructionDate = false;
            ProxyDefineDate = false;
            UniqueInstructionDate = true;
            NumberIcnValueRepeat = false;
            SurveyRequireDate = false;
        }



        #endregion
    }
}
