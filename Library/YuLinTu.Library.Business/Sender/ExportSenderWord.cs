/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 发包方调查表
    /// </summary>
    public class ExportSenderWord : WordBase
    {
        #region Fields

        #endregion

        #region Properties


        #endregion

        #region Ctor

        public ExportSenderWord()
        {
            base.TemplateName = "发包方调查表";
        }

        #endregion

        #region Methods

        #region Override

        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            if (data == null)
            {
                return true;
            }
            CollectivityTissue tissue = data as CollectivityTissue;
            if (tissue == null)
            {
                return true;
            }
            try
            {
                string code = InitalzieTissueCode(tissue.ZoneCode);
                string tissueCode = tissue.Code.Length == 14 ? tissue.Code : code;
                string surveyDate = (tissue.SurveyDate != null && tissue.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(tissue.SurveyDate.Value) : "";
                string checkDate = (tissue.CheckDate != null && tissue.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(tissue.CheckDate.Value) : "";
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("SenderName" + (i == 0 ? "" : i.ToString()), tissue.Name == null || tissue.Name == "" ? "/" : tissue.Name);//发包方名称
                    SetBookmarkValue("SenderCode" + (i == 0 ? "" : i.ToString()), tissueCode == null || tissueCode == "" ? "/" : tissueCode);//发包方编码
                    SetBookmarkValue("SenderLawyerName" + (i == 0 ? "" : i.ToString()), tissue.LawyerName == null || tissue.LawyerName == "" ? "/" : tissue.LawyerName);//发包方法人代表名称
                    SetBookmarkValue("SenderLawyerTelephone" + (i == 0 ? "" : i.ToString()), tissue.LawyerTelephone.GetSettingEmptyReplacement());//发包方法人代表联系电话
                    SetBookmarkValue("SenderLawyerAddress" + (i == 0 ? "" : i.ToString()), tissue.LawyerAddress == null || tissue.LawyerAddress == "" ? "/" : tissue.LawyerAddress);//发包方法人代表地址
                    SetBookmarkValue("SenderLawyerPostNumber" + (i == 0 ? "" : i.ToString()), tissue.LawyerPosterNumber == null || tissue.LawyerPosterNumber == "" ? "/" : tissue.LawyerPosterNumber);//发包方法人代表邮政编码
                    SetBookmarkValue("SenderLawyerCredentType" + (i == 0 ? "" : i.ToString()), EnumNameAttribute.GetDescription(tissue.LawyerCredentType));//发包方法人代表证件类型
                    SetBookmarkValue("SenderLawyerCredentNumber" + (i == 0 ? "" : i.ToString()), tissue.LawyerCartNumber == null || tissue.LawyerCartNumber == "" ? "/" : tissue.LawyerCartNumber);//发包方法人代表证件号码
                    SetBookmarkValue("SenderSurveyChronicle" + (i == 0 ? "" : i.ToString()),  tissue.SurveyChronicle.GetSettingEmptyReplacement());//发包方调查记事
                    SetBookmarkValue("SenderSurveyPerson" + (i == 0 ? "" : i.ToString()), tissue.SurveyPerson.GetSettingEmptyReplacement());//发包方调查员
                    if (!string.IsNullOrEmpty(surveyDate))
                    {
                        SetBookmarkValue("SenderSurveyDate" + (i == 0 ? "" : i.ToString()), surveyDate.GetSettingEmptyReplacement());//发包方调查日期
                    }
                    SetBookmarkValue("SenderChenkOpinion" + (i == 0 ? "" : i.ToString()), tissue.CheckOpinion.GetSettingEmptyReplacement());//发包方审核记事
                    SetBookmarkValue("SenderCheckPerson" + (i == 0 ? "" : i.ToString()), tissue.CheckPerson.GetSettingEmptyReplacement());//发包方审核员
                    if (!string.IsNullOrEmpty(checkDate))
                    {
                        SetBookmarkValue("SenderCheckDate" + (i == 0 ? "" : i.ToString()), checkDate.GetSettingEmptyReplacement());//发包方审核日期
                    }
                    SetBookmarkValue("SocialCode" + (i == 0 ? "" : i.ToString()), tissue.SocialCode);//社会信用代码
                }
                string name = (tissue != null ? tissue.LawyerCredentType : eCredentialsType.IdentifyCard).ToString();
                if (tissue.LawyerCredentType == eCredentialsType.AgentCard)
                {
                    name = eCredentialsType.Other.ToString();
                }
                SetBookmarkValue(name, "R");//证件号码
                Disponse();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return true;
            }
            return true;
        }

        /// <summary>
        /// 初始化集体经济组织代码
        /// </summary>
        /// <param name="tissueCode"></param>
        /// <returns></returns>
        private string InitalzieTissueCode(string zoneCode)
        {
            string tissueCode = string.Empty;
            switch (zoneCode.Length)
            {
                case Zone.ZONE_TOWN_LENGTH:
                    tissueCode = zoneCode + "00000";
                    break;
                case Zone.ZONE_VILLAGE_LENGTH:
                    tissueCode = zoneCode + "00";
                    break;
                case Zone.ZONE_GROUP_LENGTH:
                    tissueCode = zoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + zoneCode.Substring(Zone.ZONE_VILLAGE_LENGTH, 2);
                    break;
                default:
                    tissueCode = zoneCode;
                    break;
            }
            return tissueCode;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            GC.Collect();
        }

        #endregion

        #endregion
    }
}
