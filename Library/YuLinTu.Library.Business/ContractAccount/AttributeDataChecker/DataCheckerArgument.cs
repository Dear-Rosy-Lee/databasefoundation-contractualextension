using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    [NavigationItem("必填字段完整性检查")]
    public class MandatoryField : INotifyPropertyChanged
    {
        private bool _mandatoryFieldSender;
        private bool _mandatoryFieldVP;
        private bool _mandatoryFieldMember;
        private bool _mandatoryFieldLand;
        private bool _mandatoryFieldContract;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 发包方属性
        /// </summary>
        [CheckItem("MandatoryFieldSender", "发包方属性", "发包方名称、发包方编码、发包方负责人、证件类型、证件号、发包方地址、调查员、调查日期等不能为空")]
        public bool MandatoryFieldSender
        {
            get => _mandatoryFieldSender;
            set
            {
                _mandatoryFieldSender = value;
                OnPropertyChanged(nameof(MandatoryFieldSender));
            }
        }

        /// <summary>
        /// 承包方属性
        /// </summary>
        [CheckItem("MandatoryFieldVP", "承包方属性", "承包方户编号、承包方名称、承包方类型、承包方地址等不能为空")]
        public bool MandatoryFieldVP
        {
            get => _mandatoryFieldVP;
            set
            {
                _mandatoryFieldVP = value;
                OnPropertyChanged(nameof(MandatoryFieldVP));
            }
        }

        /// <summary>
        /// 家庭成员属性
        /// </summary>
        [CheckItem("MandatoryFieldMember", "家庭成员属性", "成员姓名、成员性别、成员证件类型、证件号码、家庭关系等不能为空")]
        public bool MandatoryFieldMember
        {
            get => _mandatoryFieldMember;
            set
            {
                _mandatoryFieldMember = value;
                OnPropertyChanged(nameof(MandatoryFieldMember));
            }
        }

        /// <summary>
        /// 地块信息属性
        /// </summary>
        [CheckItem("MandatoryFieldLand", "地块信息属性", "地块名称、地块类别、土地利用类型、土地用途、实测面积等不能为空")]
        public bool MandatoryFieldLand
        {
            get => _mandatoryFieldLand;
            set
            {
                _mandatoryFieldLand = value;
                OnPropertyChanged(nameof(MandatoryFieldLand));
            }
        }

        /// <summary>
        /// 合同信息属性
        /// </summary>
        [CheckItem("MandatoryFieldContract", "合同信息属性", "合同编码、签订日期、承包期限起、承包期限止、承包方式，合同面积亩等不能为空")]
        public bool MandatoryFieldContract
        {
            get => _mandatoryFieldContract;
            set
            {
                _mandatoryFieldContract = value;
                OnPropertyChanged(nameof(MandatoryFieldContract));
            }
        }

        public MandatoryField()
        {
            MandatoryFieldSender = true;
            MandatoryFieldVP = true;
            MandatoryFieldMember = true;
            MandatoryFieldLand = true;
            MandatoryFieldContract = true;
        }
    }

    [NavigationItem("填写规范性检查")]
    public class DataCorrectness : INotifyPropertyChanged
    {
        private bool _dataCorrectnessField;
        private bool _dataCorrectnessVPName;
        private bool _dataCorrectnessSurveyName;
        private bool _dataCorrectnessEvent;
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 字段填写值域检查
        /// </summary>
        [CheckItem("DataCorrectnessField","字段填写", "承包方类型、证件类型、性别、家庭关系、地块类别、土地用途、变化情况等字段填写满足值域检查要求")]
        public bool DataCorrectnessField
        {
            get => _dataCorrectnessField;
            set
            {
                _dataCorrectnessField = value;
                OnPropertyChanged(nameof(DataCorrectnessField));
            }
        }

        /// <summary>
        /// 承包方姓名、成员姓名检查
        /// </summary>
        [CheckItem("DataCorrectnessVPName","承包方姓名、成员姓名", "承包方姓名、成员姓名不能包含数字、空格，除了 · 以外的的特殊字符")]
        public bool DataCorrectnessVPName
        {
            get => _dataCorrectnessVPName;
            set
            {
                _dataCorrectnessVPName = value;
                OnPropertyChanged(nameof(DataCorrectnessVPName));
            }
        }

        /// <summary>
        /// 调查人员、审核人员姓名检查
        /// </summary>
        [CheckItem("DataCorrectnessSurveyName","调查人员、审核人员姓名", "调查人员、审核人员姓名不能包含数字、空格，除了 · 以外的的特殊字符；如果是多个人名，使用 、，；, ; 的分拆字符串后的名字符合名字的校验规则")]
        public bool DataCorrectnessSurveyName
        {
            get => _dataCorrectnessSurveyName;
            set
            {
                _dataCorrectnessSurveyName = value;
                OnPropertyChanged(nameof(DataCorrectnessSurveyName));
            }
        }

        /// <summary>
        /// 调查记事、审核记事
        /// </summary>
        [CheckItem("DataCorrectnessEvent","调查记事、审核记事", "调查记事、审核记事等内容的长度不超过500个字符")]
        public bool DataCorrectnessEvent
        {
            get => _dataCorrectnessEvent;
            set
            {
                _dataCorrectnessEvent = value;
                OnPropertyChanged(nameof(DataCorrectnessEvent));
            }
        }
        public DataCorrectness()
        {
            DataCorrectnessField = true;
            DataCorrectnessVPName = true;
            DataCorrectnessSurveyName = true;
            DataCorrectnessEvent = true;
        }
    }

    [NavigationItem("证件号正确性检查")]
    public class RuleOfIDCheck : INotifyPropertyChanged
    {
        private bool _ruleOfID;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 证件号码
        /// </summary>
        [CheckItem("RuleOfID","证件号码", "发包方、承包方、家庭成员等证件号码符合身份证的规则")]
        public bool RuleOfID
        {
            get => _ruleOfID;
            set
            {
                _ruleOfID = value;
                OnPropertyChanged(nameof(RuleOfID));
            }
        }
        public RuleOfIDCheck()
        {
            RuleOfID = true;
        }
    }

    [NavigationItem("数据逻辑性检查")]
    public class DataLogic: INotifyPropertyChanged
    {
        private bool _dataLogicLandType;
        private bool _dataLogicLandLimit;
      

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 地块类型
        /// </summary>
        [CheckItem("DataLogicLandType","地块类型", "地块类型为“承包地块”时，确权面积必须大于0；地块的承包方式必须为家庭承包方式")]
        public bool DataLogicLandType
        {
            get => _dataLogicLandType;
            set
            {
                _dataLogicLandType = value;
                OnPropertyChanged(nameof(DataLogicLandType));
            }
        }

        /// <summary>
        /// 地块四至
        /// </summary>
        [CheckItem("DataLogicLandLimit","地块四至", "地块四至的东、南、西、北至，至少有3个方向不能为空")]
        public bool DataLogicLandLimit
        {
            get => _dataLogicLandLimit;
            set
            {
                _dataLogicLandLimit = value;
                OnPropertyChanged(nameof(DataLogicLandLimit));
            }
        }

        public DataLogic()
        {
            DataLogicLandType = true;
            DataLogicLandLimit = true;
        }

    }

    [NavigationItem("组内唯一性检查")]
    public class DataRepeataBility : INotifyPropertyChanged
    {
        private bool _dataRepeataBilityCheck;
        private bool _dataRepeataBilityCheckCode;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 集体唯一性
        /// </summary>
        [CheckItem("DataRepeataBilityCheck","集体唯一性", "单个发包方下不能有多个集体 (集体名称为：村集体、社集体、集体、集体地、组集体")]
        public bool DataRepeataBilityCheck
        {
            get => _dataRepeataBilityCheck;
            set
            {
                _dataRepeataBilityCheck = value;
                OnPropertyChanged(nameof(DataRepeataBilityCheck));
            }
        }

        /// <summary>
        /// 编码唯一性
        /// </summary>
        [CheckItem("DataRepeataBilityCheckCode","编码唯一性", "单个发包方内承包方编码、承包方证件号码、成员身份证号码、地块编码等必须唯一")]
        public bool DataRepeataBilityCheckCode
        {
            get => _dataRepeataBilityCheckCode;
            set
            {
                _dataRepeataBilityCheckCode = value;
                OnPropertyChanged(nameof(DataRepeataBilityCheckCode));
            }
        }

        public DataRepeataBility()
        {
            DataRepeataBilityCheck = true;
            DataRepeataBilityCheckCode = true;
        }

    }

    [NavigationItem("整库唯一性检查")]
    public class Uniqueness : INotifyPropertyChanged
    {
        private bool _uniquenessCheck;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 整库编码唯一性
        /// </summary>
        [CheckItem("UniquenessCheck","整库编码唯一性", "承包方编码、承包方证件号码、成员身份证号码、地块编码等整库必须唯一")]
        public bool UniquenessCheck
        {
            get => _uniquenessCheck;
            set
            {
                _uniquenessCheck = value;
                OnPropertyChanged(nameof(UniquenessCheck));
            }
        }

        public Uniqueness()
        {
            UniquenessCheck = true;
        }

    }
}
