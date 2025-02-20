using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Controls
{
    public class ConfigInformation : CDObject, INotifyPropertyChanged
    {
        #region Property

        /// <summary>
        /// 配置项
        /// </summary>
        public DeployInformation DeployInfo { get; set; }

        /// <summary>
        /// 检查参数
        /// </summary>
        public TermCheckArgument TermArg { get; set; }

        #endregion
        #region Ctor

        public ConfigInformation()
        {
            DeployInfo = new DeployInformation();
            TermArg = new TermCheckArgument();
            TermArg.InitaillArg();
        }


        #endregion
        #region Method

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
    /// <summary>
    /// 检查配置项
    /// </summary>
    [Serializable]
    public class DeployInformation : CDObject, INotifyPropertyChanged
    {

        #region Properties

        #region 数据完整性

        /// <summary>
        /// 必填字段完整性-发包方属性
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition MandatoryFieldSender { get; set; }
        public const string CMandatoryFieldSender = "MandatoryFieldSender";

        /// <summary>
        /// 必填字段完整性-承包方属性
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition MandatoryFieldVP { get; set; }
        public const string CMandatoryFieldVP = "MandatoryFieldVP";

        /// <summary>
        /// 必填字段完整性-家庭成员属性
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition MandatoryFieldMember { get; set; }
        public const string CMandatoryFieldMember = "MandatoryFieldMember";

        /// <summary>
        /// 必填字段完整性-地块信息属性
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition MandatoryFieldLand { get; set; }
        public const string CMandatoryFieldLand = "MandatoryFieldLand";

        /// <summary>
        /// 必填字段完整性-合同信息属性
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition MandatoryFieldContract { get; set; }
        public const string CMandatoryFieldContract = "MandatoryFieldContract";

       

        /// <summary>
        /// 数据填写正确性-字段填写
        /// </summary>
        [TermDescription(Category = "数据填写正确性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition DataCorrectnessField { get; set; }
        public const string CDataCorrectnessField = "DataCorrectnessField";

        /// <summary>
        /// 数据填写正确性-承包方姓名、成员姓名
        /// </summary>
        [TermDescription(Category = "数据填写正确性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition DataCorrectnessVPName { get; set; }
        public const string CDataCorrectnessVPName = "DataCorrectnessVPName";

        /// <summary>
        /// 数据填写正确性-  调查人员、审核人员姓名
        /// </summary>
        [TermDescription(Category = "数据填写正确性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition DataCorrectnessSurveyName { get; set; }
        public const string CDataCorrectnessSurveyName = "DataCorrectnessSurveyName";

        /// <summary>
        /// 数据填写正确性- 调查记事、审核记事
        /// </summary>
        [TermDescription(Category = "数据填写正确性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition DataCorrectnessEvent { get; set; }
        public const string CDataCorrectnessEvent = "DataCorrectnessEvent";

        /// <summary>
        /// 证件号规则性校验- 发包方、承包方、家庭成员等证件号码
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition RuleOfID { get; set; }
        public const string CRuleOfID = "RuleOfID";

        /// <summary>
        /// 数据逻辑正确性- 地块类型
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition DataLogicLandType { get; set; }
        public const string CDataLogicLandType = "DataLogicLandType";

        /// <summary>
        /// 数据逻辑正确性- 地块四至
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition DataLogicLandLimit { get; set; }
        public const string CDataLogicLandLimit = "DataLogicLandLimit";

        /// <summary>
        /// 发包方内数据重复性检查- 不能有多个集体
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition DataRepeataBilityCheck { get; set; }
        public const string CDataRepeataBilityCheck = "DataRepeatabilityCheck";

        /// <summary>
        /// 发包方内数据重复性检查- 编码唯一
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition DataRepeataBilityCheckCode { get; set; }
        public const string CDataRepeataBilityCheckCode = "DataRepeatabilityCheckCode";

        /// <summary>
        /// 整库唯一性检查- 编码等整库必须唯一
        /// </summary>
        [TermDescription(Category = "必填字段完整性检查",
            UriImage32 = "pack://application:,,,/Quality.Business.TaskBasic;component/Resources/Folder.png")]
        public TermCondition UniquenessCheck { get; set; }
        public const string CUniquenessCheck = "UniquenessCheck";

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public DeployInformation()
        {
            CreateAllInitiallize();
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public DeployInformation(int type = 1)
        {
            switch (type)
            {
                case 1:
                    CreateAllInitiallize();
                    break;
                case 2:
                    CreateAllInitiallize();
                    CreateBaseInitiallize();
                    break;
                case 3:
                    CreateAllInitiallize();
                    CreateQualityInitiallize();
                    break;
            }
        }

        #endregion

        #region Method

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 初始化基础检查项
        /// </summary>
        public void CreateBaseInitiallize()
        {
            var plist = typeof(DeployInformation).GetProperties();
            foreach (var item in plist)
            {
                if (item.PropertyType != typeof(TermCondition))
                    continue;
                var tc = item.GetValue(this, null) as TermCondition;
                if (tc != null)
                {
                    tc.Value = false;
                }
            }
            this.MandatoryFieldSender.Value = true;
            this.MandatoryFieldVP.Value = true;
            this.MandatoryFieldMember.Value = true;
            this.MandatoryFieldLand.Value = true;
            this.MandatoryFieldContract.Value = true;
            this.DataCorrectnessField.Value = true;
            this.DataCorrectnessVPName.Value = true;
            this.DataCorrectnessSurveyName.Value = true;
            this.DataCorrectnessEvent.Value = true;
            this.RuleOfID.Value = true;
            this.DataLogicLandType.Value = true;
            this.DataLogicLandLimit.Value = true;
            this.DataRepeataBilityCheck.Value = true;
            this.DataRepeataBilityCheckCode.Value = true;
            this.UniquenessCheck.Value = true;
        }
        /// <summary>
        /// 初始化质检检查项
        /// </summary>
        public void CreateQualityInitiallize()
        {
            var plist = typeof(DeployInformation).GetProperties();
            
            foreach (var item in plist)
            {
                if (item.PropertyType != typeof(TermCondition))
                    continue;
                var tc = item.GetValue(this, null) as TermCondition;
                if (tc != null)//&&nameList.Any(t=>t==tc.Name))
                {
                    //tc.Value = true;
                    tc.IsEnable = false;
                }
            }
        }

        /// <summary>
        /// 初始化绑定的检查项
        /// </summary>
        public void CreateAllInitiallize()
        {
            int index = 1;
            MandatoryFieldSender = new TermCondition() { Index = index++, Value = true, Name = CMandatoryFieldSender, Description = "必填字段完整性", Comment = "发包方名称、发包方编码、发包方负责人、证件类型、证件号、发包方地址、调查员、调查日期等不能为空" };
            MandatoryFieldVP = new TermCondition() { Index = index++, Value = true, Name = CMandatoryFieldVP, Description = "必填字段完整性", Comment = "承包方户编号、承包方名称、承包方类型、承包方地址等不能为空" };
            MandatoryFieldMember = new TermCondition() { Index = index++, Value = true, Name = CMandatoryFieldMember, Description = "必填字段完整性", Comment = "成员姓名、成员性别、成员证件类型、证件号码、家庭关系等不能为空" };
            MandatoryFieldLand = new TermCondition() { Index = index++, Value = true, Name = CMandatoryFieldLand, Description = "必填字段完整性", Comment = "地块名称、地块类别、土地利用类型、土地用途、实测面积等不能为空" };
            MandatoryFieldContract = new TermCondition() { Index = index++, Value = true, Name = CMandatoryFieldContract, Description = "必填字段完整性", Comment = "合同编码、签订日期、承包期限起、承包期限止、承包方式，合同面积亩等不能为空" };
            DataCorrectnessField = new TermCondition() { Index = index++, Value = true, Name = CDataCorrectnessField, Description = "数据填写正确性", Comment = "承包方类型、证件类型、性别、家庭关系、地块类别、土地用途、变化情况等字段填写满足值域检查要求" };
            DataCorrectnessVPName = new TermCondition() { Index = index++, Value = true, Name = CDataCorrectnessVPName, Description = "数据填写正确性", Comment = "承包方姓名、成员姓名不能包含数字、空格，除了 · 以外的的特殊字符" };
            DataCorrectnessSurveyName = new TermCondition() { Index = index++, Value = true, Name = CDataCorrectnessSurveyName, Description = "数据填写正确性", Comment = "调查人员、审核人员姓名不能包含数字、空格，除了 · 以外的的特殊字符；如果是多个人名，使用 、，；, ; 的分拆字符串后的名字符合名字的校验规则" };
            DataCorrectnessEvent = new TermCondition() { Index = index++, Value = true, Name = CDataCorrectnessEvent, Description = "数据填写正确性", Comment = "调查记事、审核记事等内容的长度不超过500个字符" };
            RuleOfID = new TermCondition() { Index = index++, Value = true, Name = CRuleOfID, Description = "证件号规则性校验", Comment = "发包方、承包方、家庭成员等证件号码符合身份证的规则" };
            DataLogicLandType = new TermCondition() { Index = index++, Value = true, Name = CDataLogicLandType, Description = "数据逻辑正确性", Comment = "地块类型为“承包地块”时，确权面积必须大于0；地块的承包方式必须为家庭承包方式" };
            DataLogicLandLimit = new TermCondition() { Index = index++, Value = true, Name = CDataLogicLandLimit, Description = "数据逻辑正确性", Comment = "地块四至的东、南、西、北至，至少有3个方向不能为空" };
            DataRepeataBilityCheck = new TermCondition() { Index = index++, Value = true, Name = CDataRepeataBilityCheck, Description = "发包方内数据重复性", Comment = "单个发包方内承包方编码、承包方证件号码、成员身份证号码、地块编码等必须唯一" };
            DataRepeataBilityCheckCode = new TermCondition() { Index = index++, Value = true, Name = CDataRepeataBilityCheckCode, Description = "发包方内数据重复性", Comment = "单个发包方下不能有多个集体 (集体名称为：村集体、社集体、集体、集体地、组集体）" };
            UniquenessCheck = new TermCondition() { Index = index++, Value = true, Name = CUniquenessCheck, Description = "整库唯一性", Comment = "承包方编码、承包方证件号码、成员身份证号码、地块编码等整库必须唯一" };
            
           
        }

        #endregion
    }
    #endregion
}
