/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Component.DataCheckerTask
{
    /// <summary>
    /// 导出成果数据库实体设置
    /// </summary>
    public class DataCheckerSetDefine : NotifyCDObject
    {
        #region Properties

        public string ZoneNameAndCode
        {
            get { return _zoneNameAndCode; }
            set { _zoneNameAndCode = value.TrimSafe(); NotifyPropertyChanged("ZoneNameAndCode"); }
        }
        private string _zoneNameAndCode;

        public bool IsCheckCardNumber
        {
            get { return _IsCheckCardNumber; }
            set { _IsCheckCardNumber = value; NotifyPropertyChanged("IsCheckCardNumber"); }
        }
        private bool _IsCheckCardNumber;


        public bool IsCheckFBF
        {
            get { return _IsCheckFBF; }
            set { _IsCheckFBF = value; NotifyPropertyChanged("IsCheckFBF"); }
        }
        private bool _IsCheckFBF;

        public bool IsCheckCBF
        {
            get { return _IsCheckCBF; }
            set { _IsCheckCBF = value; NotifyPropertyChanged("IsCheckCBF"); }
        }
        private bool _IsCheckCBF;

        public bool IsCheckLand
        {
            get { return _IsCheckLand; }
            set { _IsCheckLand = value; NotifyPropertyChanged("IsCheckLand"); }
        }
        private bool _IsCheckLand;

        public bool IsCheckHT
        {
            get { return _IsCheckHT; }
            set { _IsCheckHT = value; NotifyPropertyChanged("IsCheckHT"); }
        }
        private bool _IsCheckHT;
        public DataCheckerSetDefine()
        {

            ZoneNameAndCode = "";
            //HasJZX = "false";
            IsCheckCardNumber = true;
            IsCheckCBF = true;
            IsCheckFBF = true;
            IsCheckHT = true;
            IsCheckLand = true;
        }

        #endregion
    }
}
