using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Controls
{
    public class TermCheckArgument : CDObject
    {
        #region Property

        /// <summary>
        /// 必填字段完整性-发包方属性
        /// </summary>
        public bool MandatoryFieldSender { get; set; }

        /// <summary>
        /// 必填字段完整性-承包方属性
        /// </summary>
        public bool MandatoryFieldVP { get; set; }

        /// <summary>
        /// 必填字段完整性-家庭成员属性
        /// </summary>
        public bool MandatoryFieldMember { get; set; }

        /// <summary>
        /// 必填字段完整性-地块信息属性
        /// </summary>
        public bool MandatoryFieldLand { get; set; }

        /// <summary>
        /// 必填字段完整性-合同信息属性
        /// </summary>
        public bool MandatoryFieldContract { get; set; }
      
        /// <summary>
        /// 数据填写正确性-字段填写
        /// </summary>
        public bool DataCorrectnessField { get; set; }
     
        /// <summary>
        /// 数据填写正确性-承包方姓名、成员姓名
        /// </summary>
        public bool DataCorrectnessVPName { get; set; }
      
        /// <summary>
        /// 数据填写正确性-  调查人员、审核人员姓名
        /// </summary>
        public bool DataCorrectnessSurveyName { get; set; }
       
        /// <summary>
        /// 数据填写正确性- 调查记事、审核记事
        /// </summary>
        public bool DataCorrectnessEvent { get; set; }

        /// <summary>
        /// 证件号规则性校验- 发包方、承包方、家庭成员等证件号码
        /// </summary>
        public bool RuleOfID { get; set; }
      
        /// <summary>
        /// 数据逻辑正确性- 地块类型
        /// </summary>
        public bool DataLogicLandType { get; set; }

        /// <summary>
        /// 数据逻辑正确性- 地块四至
        /// </summary>
        public bool DataLogicLandLimit { get; set; }

        /// <summary>
        /// 发包方内数据重复性检查- 不能有多个集体
        /// </summary>
        public bool DataRepeataBilityCheck { get; set; }

        /// <summary>
        /// 发包方内数据重复性检查- 编码唯一
        /// </summary>
        public bool DataRepeataBilityCheckCode { get; set; }

        /// <summary>
        /// 整库唯一性检查- 编码等整库必须唯一
        /// </summary>
        public bool UniquenessCheck { get; set; }

        #endregion

        /// <summary>
        /// 初始化参数信息
        /// </summary>
        public void InitaillArg()
        {
            MandatoryFieldSender = true;
            MandatoryFieldVP = true;
            MandatoryFieldMember = true;
            MandatoryFieldLand = true;
            MandatoryFieldContract = true;
            DataCorrectnessField = true;
            DataCorrectnessVPName = true;
            DataCorrectnessSurveyName = true;
            DataCorrectnessEvent = true;
            RuleOfID = true;
            DataLogicLandType = true;
            DataLogicLandLimit = true;
            DataRepeataBilityCheck = true;
            DataRepeataBilityCheckCode = true;
            UniquenessCheck = true;
        }
    }
}
