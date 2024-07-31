using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Data;

namespace YuLinTu.Component.StockRightShuShan.Model
{

    public class WarantSettingModel:NotifyInfoCDObject
    {

        [Enabled(false)]
        public IDbContext DbContext { get; set; }

        private Zone _currentZone;

        [Enabled(false)]
        public Zone CurrentZone
        {
            get
            {
                return _currentZone;
            }
            set
            {
                _currentZone = value;
                if (_isUseShort)
                    SendOffice = CreateAwareAbbreviationUnit();
                else
                    SendOffice = CreateAwareUnit();
                WriteOffice = CreateWriteUnit();
            }
        }

        private string _sendOffice;
        public string SendOffice
        {
            get { return _sendOffice; }
            set { _sendOffice = value; NotifyPropertyChanged(nameof(SendOffice)); }
        }


        private bool _isUseShort;
        public bool IsUseShort
        {
            get { return _isUseShort; }
            set
            {
                _isUseShort = value;
                if (_isUseShort)
                    SendOffice = CreateAwareAbbreviationUnit();
                else
                    SendOffice = CreateAwareUnit();
                NotifyPropertyChanged(nameof(IsUseShort));
            }
        }

        private string _writeOffice;
        public string WriteOffice
        {
            get
            {
                return _writeOffice;
            }
            set
            {
                _writeOffice = value;
                NotifyPropertyChanged(nameof(WriteOffice));
            }
        }


        private string _contractor;
        public string Contractor
        {
            get
            {
                return _contractor;
            }
            set
            {
                _contractor = value;
                NotifyPropertyChanged(nameof(Contractor));
            }
        }

        private DateTime _writeTime=DateTime.Now;
        public DateTime WriteTime
        {
            get { return _writeTime; }
            set
            {
                _writeTime = value;
                NotifyPropertyChanged(nameof(WriteTime));
            }
        }

        private int _year=DateTime.Now.Year;
        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                NotifyPropertyChanged(nameof(Year));
            }
        }

        private DateTime _sendTime=DateTime.Now;
        public DateTime SendTime
        {
            get
            {
                return _sendTime;
            }
            set
            {
                _sendTime = value;
                NotifyPropertyChanged(nameof(SendTime));
            }
        }


        private string _registerPerson;
        public string RegisterPerson
        {
            get
            {
                return _registerPerson;
            }
            set
            {
                _registerPerson = value;
                NotifyPropertyChanged(nameof(RegisterPerson));
            }
        }


        private DateTime _registerTime=DateTime.Now;
        public DateTime RegisterTime
        {
            get { return _registerTime; }
            set
            {
                _registerTime = value;
                NotifyPropertyChanged(nameof(RegisterTime));
            }
        }


        private string _registerComment;
        public string RegisterComment
        {
            get { return _registerComment; }
            set
            {
                _registerComment = value;
                NotifyPropertyChanged(nameof(RegisterComment));
            }
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }
            set
            {
                _serialNumber = value;
                NotifyPropertyChanged(nameof(SerialNumber));
            }
        }


        /// <summary>
        /// 创建颁证单位
        /// </summary>
        /// <returns></returns>
        private string CreateAwareUnit()
        {
            string zoneCode = CurrentZone?.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            Zone county = GetZoneByCode(zoneCode);
            if (county != null)
            {
                return county.Name + "人民政府";
            }
            return "";
        }



        /// <summary>
        /// 创建颁证单位
        /// </summary>
        /// <returns></returns>
        private string CreateAwareAbbreviationUnit()
        {
            string zoneCode = CurrentZone?.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            Zone county = GetZoneByCode(zoneCode);
            if (county != null)
            {
                return county.Name.Substring(0, 1) + "府";
            }
            return "";
        }

        /// <summary>
        /// 创建填证单位
        /// </summary>
        /// <returns></returns>
        private string CreateWriteUnit()
        {
            string zoneCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            Zone county = GetZoneByCode(zoneCode);
            if (county != null)
            {
                return county.Name + "农业局";
            }
            return "";
        }

        /// <summary>
        /// 获取地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private Zone GetZoneByCode(string zoneCode)
        {
            return DbContext.CreateZoneWorkStation().Get(o => o.FullCode == zoneCode)?.FirstOrDefault();
        }


        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static WarantSettingModel GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<WarantSettingModel>();
            var section = profile.GetSection<WarantSettingModel>();
            return section.Settings;
        }

        public WarantSettingModel(IDbContext dbContext,Zone currentZone)
        {
            this.DbContext = dbContext;
            this.CurrentZone = currentZone;
        }

        public WarantSettingModel()
        { }
    }
}
