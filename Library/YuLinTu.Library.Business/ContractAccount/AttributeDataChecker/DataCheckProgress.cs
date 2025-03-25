using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class DataCheckProgress
    {
        #region Property

        public TaskAttributeDataCheckerArgument DataArgument { get; set; }

        public IDbContext dbContext { get; set; }

        public Zone zone { get; set; }

        public List<ContractLand> lands { get; set; }

        public static string ErrorInfo { get; private set; }

        #endregion Property

        public string Check()
        {
            dbContext = DataArgument.DbContext;
            
            var res = MandatoryFieldCheck();
            //res = DataCorrectnessCheck();
            //res = RuleOfIDCheck();
            //res = DataLogicCheck();
            //res = DataRepeataBilityCheck();
            //res = UniquenessCheck();
            return res;
        }

        private string MandatoryFieldCheck()
        {
            var info = "必填字段完整性检查：";

            if (DataArgument.MandatoryField.MandatoryFieldSender == true)
            {
                info += "\r\n发包方属性：";
                var senderStation = dbContext.CreateSenderWorkStation();
                var sender = senderStation.GetByCode(zone.FullCode);
                //发包方名称
                if (sender.Name.IsNullOrEmpty())
                {
                    info += "发包方名称未填写;";
                }
                //发包方编码
                if (sender.Code.IsNullOrEmpty())
                {
                    info += "发包方编码未填写;";
                }
                //发包方负责人
                if (sender.LawyerName.IsNullOrEmpty())
                {
                    info += "发包方负责人未填写;";
                }
                //发包方负责人证件类型
                if (sender.LawyerCredentType.ToString().IsNullOrEmpty())
                {
                    info += "发包方负责人证件类型未填写;";
                }
                //发包方负责人证件号
                if (sender.LawyerCartNumber.IsNullOrEmpty())
                {
                    info += "发包方负责人证件号未填写;";
                }
                //发包方地址
                if (sender.LawyerAddress.IsNullOrEmpty())
                {
                    info += "发包方地址未填写;";
                }
                //调查员
                if (sender.SurveyPerson.IsNullOrEmpty())
                {
                    info += "发包方调查员未填写;";
                }
                //调查日期
                if (sender.SurveyDate.ToString().IsNullOrEmpty())
                {
                    info += "发包方调查日期未填写;";
                }

            }
            //TODO
            if (DataArgument.MandatoryField.MandatoryFieldVP == true)
            {

            }
            if (DataArgument.MandatoryField.MandatoryFieldMember == true)
            {

            }
            if (DataArgument.MandatoryField.MandatoryFieldLand == true)
            {

            }
            if (DataArgument.MandatoryField.MandatoryFieldContract == true)
            {

            }
            return info;
        }
        private bool DataCorrectnessCheck()
        {
            if (DataArgument.DataCorrectness.DataCorrectnessField == true)
            {

            }
            if (DataArgument.DataCorrectness.DataCorrectnessVPName == true)
            {

            }
            if (DataArgument.DataCorrectness.DataCorrectnessSurveyName == true)
            {

            }
            if (DataArgument.DataCorrectness.DataCorrectnessEvent == true)
            {

            }
            return true;
        }
        private bool RuleOfIDCheck()
        {
            if (DataArgument.RuleOfIDCheck.RuleOfID == true)
            {

            }
            return true;
        }
        private bool DataLogicCheck()
        {
            if (DataArgument.DataLogic.DataLogicLandType == true)
            {

            }
            if (DataArgument.DataLogic.DataLogicLandLimit == true)
            {

            }
            return true;
        }
        private bool DataRepeataBilityCheck()
        {
            if (DataArgument.DataRepeataBility.DataRepeataBilityCheck == true)
            {

            }
            if (DataArgument.DataRepeataBility.DataRepeataBilityCheckCode == true)
            {

            }
            return true;
        }
        private bool UniquenessCheck()
        {
            if (DataArgument.Uniqueness.UniquenessCheck == true)
            {

            }
            return true;
        }

    }
}
