/*
 * (C) 2012- 2014 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using System.Collections.Generic;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 发包方调查表
    /// </summary>
    [Serializable]
    public class ExportSenderTable : AgricultureWordBook
    {
        #region Fields
        private GetDictionary dic;//字典
        private const string defaultEmptyBookmarkValue = "/";

        #endregion

        #region Propertys

        #endregion

        #region Ctor

        public ExportSenderTable(string dictoryname)
        {
            dic = new GetDictionary(dictoryname);
            dic.Read();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            try
            {
                //TODO 父类方面未起作用，暂时弃用
                //base.OnSetParamValue(data);
                if (data is CollectivityTissue)
                {
                    Tissue = data as CollectivityTissue;
                }
                if (Tissue == null)
                {
                    return true;
                }
                WriteSenderInformation();
                string name = (Tissue != null && (int)Tissue.LawyerCredentType != 0) ? Tissue.LawyerCredentType.ToString() : eCredentialsType.IdentifyCard.ToString();
                if (name == "Other")
                {
                    name = "CredentialOther";
                }
                SetBookmarkValue(name, "R");
                SetBookmarkValue(name + "ZW", "R");
                SetBookmarkValue("SenderName", Tissue.Name);
                SetBookmarkValue("SenderCode", Tissue.Code);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return true;
        }

        private void WriteSenderInformation()
        {
            if (Tissue == null)
            {
                return;
            }
            string senderNameExpress = InitalizeSenderExpress();//发包方名称扩展如(第一村民小组)。
            string townName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            var zjlxinfo = Tissue.LawyerCredentType;
            var ZJLX = (int)zjlxinfo;
            Dictionary layerCard;
            layerCard = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.ZJLX && d.Code == ZJLX.ToString());

            SetBookmarkValue(AgricultureBookMark.SenderName, ConvertParamValue(Tissue.Name));//发包方名称
            SetBookmarkValue(AgricultureBookMark.SenderNameExpress, ConvertParamValue(senderNameExpress));//发包方名称扩展如(第一村民小组)
            SetBookmarkValue(AgricultureBookMark.SenderLawyerName, ConvertParamValue(Tissue.LawyerName));//发包方法人名称
            SetBookmarkValue(AgricultureBookMark.SenderLawyerTelephone, ConvertParamValue(Tissue.LawyerTelephone));//发包方法人联系方式
            SetBookmarkValue(AgricultureBookMark.SenderLawyerAddress, ConvertParamValue(Tissue.LawyerAddress));//发包方法人地址
            SetBookmarkValue(AgricultureBookMark.SenderLawyerPostNumber, ConvertParamValue(Tissue.LawyerPosterNumber));//发包方法人邮政编码
            // SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentType, layerCard != null ? layerCard.Name : "");//发包方法人证件类型

            // 发包方证件类型
            eCredentialsType mode = (eCredentialsType)Tissue.LawyerCredentType;
            string CardTypeText = string.Empty;
            switch (mode)
            {
                case eCredentialsType.IdentifyCard:
                    SetBookmarkValue(AgricultureBookMark.IdentifyCard, "R");//证件类型
                    SetBookmarkValue(AgricultureBookMark.IdentifyCard + "1", "R");//藏文
                    CardTypeText = "居民身份证";//证件类型
                    break;
                case eCredentialsType.OfficerCard:
                    SetBookmarkValue(AgricultureBookMark.OfficerCard, "R");//证件类型
                    SetBookmarkValue(AgricultureBookMark.OfficerCard + "1", "R");//藏文
                    CardTypeText = "军官证";//证件类型
                    break;
                case eCredentialsType.Other:
                case eCredentialsType.AgentCard:
                    SetBookmarkValue(AgricultureBookMark.CredentialOther, "R");//
                    SetBookmarkValue(AgricultureBookMark.CredentialOther + "1", "R");//藏文
                    CardTypeText = "其他";            // 因为没有“行政、企事业单位机构代码证或法人代码证”，所以归为其它
                    break;
                case eCredentialsType.Passport:
                    SetBookmarkValue(AgricultureBookMark.Passport, "R");//证件类型
                    SetBookmarkValue(AgricultureBookMark.Passport + "1", "R");//藏文
                    CardTypeText = "护照";//证件类型
                    break;
                case eCredentialsType.ResidenceBooklet:
                    SetBookmarkValue(AgricultureBookMark.ResidenceBooklet, "R");//证件类型
                    SetBookmarkValue(AgricultureBookMark.ResidenceBooklet + "1", "R");//藏文
                    CardTypeText = "户口簿";//证件类型
                    break;
                default:
                    break;
            }
            SetBookmarkValue("ContractorCardType", CardTypeText);//证件类型

            SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentNumber, ConvertParamValue(Tissue.LawyerCartNumber));//发包方法人证件类型
            SetBookmarkValue(AgricultureBookMark.SenderCode, ConvertParamValue(Tissue.Code));//发包方代码
            SetBookmarkValue(AgricultureBookMark.SenderTownName, ConvertParamValue(townName));//发包方到镇
            SetBookmarkValue(AgricultureBookMark.SenderVillageName, ConvertParamValue(villageName));//发包方到村
            SetBookmarkValue(AgricultureBookMark.SenderGroupName, ConvertParamValue(groupName));//发包方到组
            SetBookmarkValue(AgricultureBookMark.SenderSurveyChronicle, ConvertParamValue(Tissue.SurveyChronicle));//调查记事
            SetBookmarkValue(AgricultureBookMark.SenderSurveyPerson, ConvertParamValue(Tissue.SurveyPerson));//调查员
            if (Tissue.SurveyDate != null && Tissue.SurveyDate.HasValue)
                SetBookmarkValue(AgricultureBookMark.SenderSurveyDate, ToolDateTime.GetLongDateString(Tissue.SurveyDate.Value));//调查日期
            SetBookmarkValue(AgricultureBookMark.SenderCheckPerson, ConvertParamValue(Tissue.CheckPerson));//审核员
            if (Tissue.CheckDate != null && Tissue.CheckDate.HasValue)
                SetBookmarkValue(AgricultureBookMark.SenderCheckDate, ToolDateTime.GetLongDateString(Tissue.CheckDate.Value));//审核日期
            SetBookmarkValue(AgricultureBookMark.SenderChenkOpinion, ConvertParamValue(Tissue.CheckOpinion));//审核意见

        }

        private string ConvertParamValue(string oldParamValue)
        {
            return String.IsNullOrEmpty(oldParamValue) ? defaultEmptyBookmarkValue : oldParamValue;
        }

        /// <summary>
        /// 创建发包方扩展
        /// </summary>
        /// <returns></returns>
        private string InitalizeSenderExpress()
        {
            if (CurrentZone == null)
            {
                return string.Empty;
            }
            string number = ToolString.GetLeftNumberWithInString(CurrentZone.Name);
            if (string.IsNullOrEmpty(number))
            {
                number = CurrentZone.Code;
            }
            return "(第" + ToolMath.GetChineseLowNumber(number) + "村民小组)";
        }

        /// <summary>
        /// 初始化地域名称
        /// </summary>
        private string InitalizeZoneName(string zoneCode, int length)
        {
            string zoneName = string.Empty;
            if (ZoneList == null || ZoneList.Count == 0 || zoneCode.Length < length)
            {
                return zoneName;
            }
            string code = zoneCode.Substring(0, length);
            Zone zone = ZoneList.Find(t => t.FullCode == code);
            if (zone != null)
            {
                zoneName = zone.Name;
            }
            return zoneName;
        }

        #endregion
    }
}
