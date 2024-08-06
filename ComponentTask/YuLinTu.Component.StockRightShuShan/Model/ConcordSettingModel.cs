using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightShuShan.Helper;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using System.Collections.ObjectModel;

namespace YuLinTu.Component.StockRightShuShan.Model
{
    public class ConcordSettingModel : NotifyCDObject
    {

        private string _sender;
        public string Sender
        {
            get { return _sender; }
            set
            {
                _sender = value;
                NotifyPropertyChanged(nameof(Sender));
            }
        }

        private bool _isEver;
        public bool IsEver
        {
            get
            {
                return _isEver;
            }
            set
            {
                _isEver = value;
                NotifyPropertyChanged(nameof(IsEver));
            }
        }

        private DateTime _startTime;//DateTime.Now;
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                NotifyPropertyChanged(nameof(StartTime));
            }
        }


        private DateTime _endTime;//= DateTime.Now;
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                NotifyPropertyChanged(nameof(EndTime));
            }
        }

        private string _contractWays="家庭承包";

        /// <summary>
        /// 承包方式
        /// </summary>
        public string ContractWay
        {
            get
            {
                return _contractWays;
            }
            set
            {
                _contractWays = value;
                NotifyPropertyChanged(nameof(ContractWay));
            }
        }

        private string _landPurpose="种植业";
        public string LandPurpose
        {
            get
            {
                return _landPurpose;
            }
            set
            {
                _landPurpose = value;
                NotifyPropertyChanged(nameof(LandPurpose));
            }
        }

        private DateTime? _contractTime;// = DateTime.Now;
        public DateTime? ContractTime
        {
            get { return _contractTime; }
            set { _contractTime = value; NotifyPropertyChanged(nameof(ContractTime)); }
        }

        private DateTime? _senderTime;//= DateTime.Now;
        public DateTime? SenderTime
        {
            get { return _senderTime; }
            set
            {
                _senderTime = value;
                NotifyPropertyChanged(nameof(SenderTime));
            }
        }

        private string _landCategory;
        public string LandCategory
        {
            get
            {
                return _landCategory;
            }
            set
            {
                _landCategory = value;
                NotifyPropertyChanged(nameof(LandCategory));
            }
        }

        private ObservableCollection<string> _contractWaysDicts=new ObservableCollection<string> ();

        [Enabled(false)]
        public ObservableCollection<string> ContractWaysDicts=>new ObservableCollection<string> { "家庭承包", "其他方式" };

        [Enabled(false)]
        public ObservableCollection<string> LandPurposeDicts => new ObservableCollection<string> { "种植业", "林业", "畜牧业", "渔业", "非农业用途" };

        private string _recordPerson;
        public string RecordPerson
        {
            get
            {
                return _recordPerson;
            }
            set
            {
                _recordPerson = value;
                NotifyPropertyChanged(nameof(RecordPerson));
            }
        }

        private DateTime? _recordTime;//=DateTime.Now;
        public DateTime? RecordTime
        {
            get
            {
                return _recordTime;
            }
            set
            {
                _recordTime = value;
                NotifyPropertyChanged(nameof(RecordTime));
            }
        }

        private string _recordThings;
        public string RecordThings
        {
            get { return _recordThings; }
            set { _recordThings = value; NotifyPropertyChanged(nameof(RecordThings)); }
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


        private DateTime? _publicResultTime;
        public DateTime? PublicResultTime
        {
            get
            {
                return _publicResultTime;
            }
            set
            {
                _publicResultTime = value;
                NotifyPropertyChanged(nameof(PublicResultTime));
            }
        }


        private string _contractorOpinion;
        public string ContractorOpinion
        {
            get
            {
                return _contractorOpinion;
            }
            set
            {
                _contractorOpinion = value;
                NotifyPropertyChanged(nameof(ContractorOpinion));
            }
        }

        private string _checkPerson;
        public string CheckPerson
        {
            get
            {
                return _checkPerson;
            }
            set
            {
                _checkPerson = value;
            }
        }


        private DateTime? _checkTime;//=DateTime.Now;
        public DateTime? CheckTime
        {
            get { return _checkTime; }
            set { _checkTime = value;NotifyPropertyChanged(nameof(CheckTime)); }
        }

        private string _checkOpinion;
        public string CheckOpinion
        {
            get
            {
                return _checkOpinion;
            }
            set
            {
                _checkOpinion = value;
                NotifyPropertyChanged(nameof(CheckOpinion));
            }
        }


        private string _comment;
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }


        private bool _isCBD=true;
        public bool IsCBD
        {
            get
            {
                return _isCBD;
            }
            set
            {
                SetLandCatelage();
                _isCBD = value;
                NotifyPropertyChanged(nameof(IsCBD));
            }
        }


        private bool _isZLD;
        public bool IsZLD
        {
            get
            {
                return _isZLD;
            }
            set
            {
                SetLandCatelage();
                _isZLD = value;
                NotifyPropertyChanged(nameof(IsZLD));
            }
        }


        private bool _isKHD;
        public bool IsKHD
        {
            get
            {
                return _isKHD;
            }
            set
            {
                SetLandCatelage();
                _isKHD = value;
                NotifyPropertyChanged(nameof(IsKHD));
            }
        }

        private bool _isJDD;
        public bool IsJDD
        {
            get
            {
                return _isJDD;
            }
            set
            {
                SetLandCatelage();
                _isJDD = value;
                NotifyPropertyChanged(nameof(IsJDD));
            }
        }

        private bool _isQTJTTD;
        public bool IsQTJTTD
        {
            get
            {
                return _isQTJTTD;
            }
            set
            {
                SetLandCatelage();
                _isQTJTTD = value;
                NotifyPropertyChanged(nameof(IsQTJTTD));
            }
        }

        private bool _isJJD;
        public bool IsJJD
        {
            get
            {
                return _isJJD;
            }
            set
            {
                SetLandCatelage();
                _isJJD = value;
                NotifyPropertyChanged(nameof(IsJJD));
            }
        }

        private bool _isSLD;
        public bool IsSLD
        {
            get
            {
                return _isSLD;
            }
            set
            {
                SetLandCatelage();
                _isSLD = value;
                NotifyPropertyChanged(nameof(IsSLD));
            }
        }


        private bool _isLHD;
        public bool IsLHD
        {
            get
            {
                return _isLHD;
            }
            set
            {
                SetLandCatelage();
                _isLHD = value;
                NotifyPropertyChanged(nameof(IsLHD));
            }
        }

        private void SetLandCatelage()
        {
            //_isCBD = false;
            //_isJDD = false;
            //_isJJD = false;
            //_isKHD = false;
            //_isLHD = false;
            //_isQTJTTD = false;
            //_isSLD = false;
            //_isZLD = false;

            //NotifyPropertyChanged(nameof(IsCBD));
            //NotifyPropertyChanged(nameof(IsJDD));
            //NotifyPropertyChanged(nameof(IsJJD));
            //NotifyPropertyChanged(nameof(IsKHD));
            //NotifyPropertyChanged(nameof(IsLHD));
            //NotifyPropertyChanged(nameof(IsQTJTTD));
            //NotifyPropertyChanged(nameof(IsSLD));
            //NotifyPropertyChanged(nameof(IsZLD));
        }



        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static ConcordSettingModel GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ConcordSettingModel>();
            var section = profile.GetSection<ConcordSettingModel>();
            return section.Settings;
        }

    }
}
