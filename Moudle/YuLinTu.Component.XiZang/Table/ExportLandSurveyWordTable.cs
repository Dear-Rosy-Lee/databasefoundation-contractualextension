/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Business;
using YuLinTu.Spatial;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出地块调查表
    /// </summary>
    [Serializable]
    public class ExportLandSurveyWordTable : AgricultureWordBook
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportLandSurveyWordTable()
        {

        }

        #endregion

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
                return false;
            }
            VirtualPerson contractor = data as VirtualPerson;
            try
            {
                WriteOtherInfo();
                WriteLandInformation(contractor);
            }
            catch (SystemException ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSetParamValue(导出地块调查表失败!)", ex.Message + ex.StackTrace);
                return false;
            }
            finally
            {
                Disponse();
            }
            return true;
        }

        #endregion

        #region 属性信息

        /// <summary>
        /// 书写地块信息
        /// </summary>
        private void WriteLandInformation(VirtualPerson contractor)
        {

            if (LandCollection == null || LandCollection.Count == 0)
            {
                return;
            }
            int tableIndex = 0;
            int rowIndex = 2;
            int rowCount = LandCollection.Count - 3;
            if (rowCount > 0)
            {
                InsertTableRow(0, 3, rowCount);
            }
            LandCollection = SortLandCollection(LandCollection);
            foreach (var land in LandCollection)
            {
                string landNumber = GetLandNumber(land);
                SetTableCellValue(tableIndex, rowIndex, 0, landNumber);//地块编码
                SetTableCellValue(tableIndex, rowIndex, 1, CurrentZone.FullCode.PadRight(14, '0') + contractor.FamilyNumber.PadLeft(4, '0'));//承包方编码
                SetTableCellValue(tableIndex, rowIndex, 2, land.Name);//地块名称
                SetTableCellValue(tableIndex, rowIndex, 3, land.TableArea.HasValue && land.TableArea.Value != 0 ? land.TableArea.Value.AreaFormat(2) : "/");//原承包合同面积
                var callog = land.LandCategory.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.DKLB && d.Code == land.LandCategory);
                SetTableCellValue(tableIndex, rowIndex, 4, callog != null ? callog.Name : "");//地块类别
                //var typeDic = land.LandCode.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.TDLYLX && d.Code == land.LandCode);
                //string landCode = typeDic != null ? typeDic.Name : "";
                SetTableCellValue(tableIndex, rowIndex, 5, land.LandName.IsNullOrEmpty() ? "" : land.LandName);//土地利用类型
                var purpose = land.Purpose.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.TDYT && d.Code == land.Purpose);
                string landPurpose = purpose != null ? purpose.Name : "";
                SetTableCellValue(tableIndex, rowIndex, 6, (string.IsNullOrEmpty(landPurpose) ||
                    Library.Business.ToolMath.MatchEntiretyNumber(landPurpose)) ? "种植业" : landPurpose);//土地用途
                SetTableCellValue(tableIndex, rowIndex, 7, land.Comment);//地块备注
                rowIndex++;
            }
        }


        /// <summary>
        /// 书写其他信息
        /// </summary>
        /// <param name="contractor"></param>
        private void WriteOtherInfo()
        {
            if (Contractor == null)
            {
                return;
            }
            string townName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            SetBookmarkValue("SenderCode", Tissue.Name);
            SetBookmarkValue(AgricultureBookMark.SenderGroupName, groupName);//发包方到组
            //2017.10.20修改，读取承包方下的调查记事的内容
            SetBookmarkValue("LandSurveyChronicle", Contractor.FamilyExpand.SurveyChronicle.GetSettingEmptyReplacement());//调查记事
            SetBookmarkValue("LandSurveyPerson", Contractor.FamilyExpand.SurveyPerson.GetSettingEmptyReplacement());//调查员
            
            SetBookmarkValue("LandSurveyDate", Contractor.FamilyExpand.SurveyDate == null ? string.Empty.GetSettingEmptyReplacement() : 
                    Library.Business.ToolDateTime.GetLongDateString(Contractor.FamilyExpand.SurveyDate.GetValueOrDefault()));//调查日期
         
            SetBookmarkValue("LandFeferPerson", InitalizeFamilyName(Contractor.Name));//指界者填写为当前承包方
            SetBookmarkValue("LandCheckPerson", Tissue.CheckPerson.GetSettingEmptyReplacement());//审核员
        
            SetBookmarkValue("LandCheckDate", Tissue.CheckDate == null ? string.Empty.GetSettingEmptyReplacement() :
                    Library.Business.ToolDateTime.GetLongDateString(Tissue.CheckDate.GetValueOrDefault()));//审核日期
           
        }

        #region helper

        /// <summary>
        /// 根据系统配置获取截取后的地块编码
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        private string GetLandNumber(ContractLand land)
        {
            string landNumber = land.LandNumber;
            var systemDefine = SystemSetDefine.GetIntence();
            int subStartIndex = systemDefine.LandNumericFormatValueSet;
            bool canSubstring = systemDefine.LandNumericFormatSet;
            if (canSubstring && landNumber.Length > subStartIndex)
            {
                landNumber = landNumber.Substring(subStartIndex);
            }

            return landNumber;
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

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<ContractLand> SortLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
                if (landNumber.Length > 14)
                {
                    landNumber = landNumber.Substring(14);
                }
                int index = landNumber.IndexOf("J");
                if (index < 0)
                {
                    index = landNumber.IndexOf("Q");
                }
                if (index > 0)
                {
                    landNumber = landNumber.Substring(index + 1);
                }
                Int32.TryParse(landNumber, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            List<ContractLand> landCollection = new List<ContractLand>();
            foreach (var land in orderdVps)
            {
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }
        #endregion

        private void Disponse()
        {
            GC.Collect();
        }

        #endregion
    }
}
