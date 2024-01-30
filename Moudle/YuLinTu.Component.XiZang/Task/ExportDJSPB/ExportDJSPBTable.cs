/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Library.Business;
using YuLinTu.Common.Office;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 预览登记审批表
    /// </summary>
    public class ExportDJSPBTable : AgricultureWordBook
    {
        #region Fields

        private int sharePersonCount;
        //家庭承包方式总户数
        private int familyconstrctVPCount;
        private int otherContractVpCount;
        #endregion

        #region Properties     

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> ALLLands { get; set; }

        /// <summary>
        /// 发包方集合个数
        /// </summary>
        public int SendersCount { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<YuLinTu.Library.Entity.VirtualPerson> VirtualPersons { get; set; }

        #endregion

        #region Ctor     

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
            //初始化并计算
            InitalizeDataInformation();
            base.OnSetParamValue(data);
            List<ContractLand> otherLands = LandCollection.FindAll(ld => !ld.IsFamilyContract());//非家庭承包
            List<ContractLand> FamilyLands = LandCollection.FindAll(ld => ld.IsFamilyContract());//家庭承包
            List<ContractLand> contractLands = LandCollection.FindAll(ld => ld.LandCategory == ((int)eLandCategoryType.ContractLand).ToString());

            double ActualAreaCount = 0.0;
            LandCollection.ForEach(l => ActualAreaCount += l.ActualArea);

            double familycontractarea = 0.0;
            double othercontractarea = 0.0;

            double familyconcordarea = 0.0;
            double otherconcordarea = 0.0;

            FamilyLands.ForEach(l => { familycontractarea += l.ActualArea; familyconcordarea += (l.TableArea != null ? l.TableArea.Value : 0); });
            otherLands.ForEach(l => { othercontractarea += l.ActualArea; otherconcordarea += (l.TableArea != null ? l.TableArea.Value : 0); });

            double allfamilyarea = familycontractarea + familyconcordarea;
            double allotherarea = othercontractarea + otherconcordarea;

            SetBookmarkValue("SenderCount", SendersCount.ToString());
            SetBookmarkValue("AllSharePerson", sharePersonCount.ToString());
            SetBookmarkValue("ContractorCount", familyconstrctVPCount.ToString());
            SetBookmarkValue("OtherLandCount", otherContractVpCount.ToString());
            SetBookmarkValue("ContractLandCount", contractLands.Count.ToString());
            SetBookmarkValue("ActualAreaCount", ActualAreaCount.FormatArea());


            //for (int i = 0; i < 6; i++)
            //{
            //    SetBookmarkValue("familyConcordAreaCount" + (i == 0 ? "" : i.ToString()), (familyconcordarea == 0 ? "" : familyconcordarea.ToString("0.00")));
            //    SetBookmarkValue("familyActualAreaCount" + (i == 0 ? "" : i.ToString()), (familycontractarea == 0 ? "" : familycontractarea.ToString("0.00")));
            //    SetBookmarkValue("familyAllAreaCount" + (i == 0 ? "" : i.ToString()), (allfamilyarea == 0 ? "" : allfamilyarea.ToString("0.00")));
            //    SetBookmarkValue("OtherConcordAreaCount" + (i == 0 ? "" : i.ToString()), (otherconcordarea == 0 ? "" : otherconcordarea.ToString("0.00")));
            //    SetBookmarkValue("OtherActualAreaCount" + (i == 0 ? "" : i.ToString()), (othercontractarea == 0 ? "" : othercontractarea.ToString("0.00")));
            //    SetBookmarkValue("OtherAllAreaCount" + (i == 0 ? "" : i.ToString()), (allotherarea == 0 ? "" : allotherarea.ToString("0.00")));

            //}

            InitalizeAreaInformtion(FamilyLands, 6, true);
            InitalizeAreaInformtion(otherLands, 11, false);
            sharePersonCount = 0;
            familyconstrctVPCount = 0;
            ALLLands = null;
            SendersCount = 0;
            VirtualPersons = null;
            base.Destroyed();
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitalizeDataInformation()
        {
            VirtualPersons.ForEach(vp =>
            {
                sharePersonCount += vp.SharePersonList.Count;
                if (vp.GetConcord()!=null&&vp.IsFamilyConcord())
                {
                    familyconstrctVPCount++;
                }
                else if(vp.GetConcord() != null && !vp.IsFamilyConcord())
                {
                    otherContractVpCount++;
                }
            }
            );

        }

        /// <summary>
        /// 初始化面积信息
        /// </summary>
        private void InitalizeAreaInformtion(List<ContractLand> landCollection, int rowIndex, bool familyMode)
        {
            double ownerArea = landCollection.Sum(ld => ((ld.OwnRightType == ((int)eLandPropertyType.UsageState).ToString() ||
            ld.OwnRightType == ((int)eLandPropertyType.Stated).ToString()) &&
            (familyMode ? ld.ConstructMode == ((int)eConstructMode.Family).ToString() : ld.ConstructMode != ((int)eConstructMode.Family).ToString())) ? (ld.TableArea != null &&
            ld.TableArea.HasValue ? ld.TableArea.Value : 0.00) : 0.00);

            ownerArea = Math.Round(ownerArea, 4);
            SetTableCellValue(0, rowIndex, 1, ToolMath.SetNumbericFormat(ownerArea.ToString(), 2));
            double villageArea = landCollection.Sum(ld => (ld.OwnRightType == ((int)eLandPropertyType.VillageCollective).ToString() && (familyMode ? ld.ConstructMode == ((int)eConstructMode.Family).ToString() : ld.ConstructMode != ((int)eConstructMode.Family).ToString())) ? (ld.TableArea != null && ld.TableArea.HasValue ? ld.TableArea.Value : 0.00) : 0.00);
            villageArea = Math.Round(villageArea, 4);
            SetTableCellValue(0, rowIndex, 2, ToolMath.SetNumbericFormat(villageArea.ToString(), 2));
            double groupArea = landCollection.Sum(ld => ((ld.OwnRightType == ((int)eLandPropertyType.GroupOfPeople).ToString() || ld.OwnRightType == ((int)eLandPropertyType.Collectived).ToString()) && (familyMode ? ld.ConstructMode == ((int)eConstructMode.Family).ToString() : ld.ConstructMode != ((int)eConstructMode.Family).ToString())) ? (ld.TableArea != null && ld.TableArea.HasValue ? ld.TableArea.Value : 0.00) : 0.00);
            groupArea = Math.Round(groupArea, 4);
            SetTableCellValue(0, rowIndex, 3, ToolMath.SetNumbericFormat(groupArea.ToString(), 2));
            double totalTableArea = villageArea + groupArea + ownerArea;
            totalTableArea = Math.Round(totalTableArea, 4);
            SetTableCellValue(0, rowIndex, 4, ToolMath.SetNumbericFormat(totalTableArea.ToString(), 2));
            double actOwnerArea = landCollection.Sum(ld => ((familyMode ? ld.ConstructMode == ((int)eConstructMode.Family).ToString() : ld.ConstructMode != ((int)eConstructMode.Family).ToString()) && (ld.OwnRightType == ((int)eLandPropertyType.UsageState).ToString() || ld.OwnRightType == ((int)eLandPropertyType.Stated).ToString())) ? ld.ActualArea : 0.00);
            actOwnerArea = Math.Round(actOwnerArea, 4);
            SetTableCellValue(0, rowIndex + 1, 1, ToolMath.SetNumbericFormat(actOwnerArea.ToString(), 2));
            double actVillageArea = landCollection.Sum(ld => ((familyMode ? ld.ConstructMode == ((int)eConstructMode.Family).ToString() : ld.ConstructMode != ((int)eConstructMode.Family).ToString()) && ld.OwnRightType == ((int)eLandPropertyType.VillageCollective).ToString()) ? ld.ActualArea : 0.00);
            actVillageArea = Math.Round(actVillageArea, 4);
            SetTableCellValue(0, rowIndex + 1, 2, ToolMath.SetNumbericFormat(actVillageArea.ToString(), 2));
            double actGroupArea = landCollection.Sum(ld => ((familyMode ? ld.ConstructMode == ((int)eConstructMode.Family).ToString() : ld.ConstructMode != ((int)eConstructMode.Family).ToString()) && (ld.OwnRightType == ((int)eLandPropertyType.GroupOfPeople).ToString() || ld.OwnRightType == ((int)eLandPropertyType.Collectived).ToString())) ? ld.ActualArea : 0.00);
            actGroupArea = Math.Round(actGroupArea, 4);
            SetTableCellValue(0, rowIndex + 1, 3, ToolMath.SetNumbericFormat(actGroupArea.ToString(), 2));
            double totalActualArea = actVillageArea + actGroupArea + actOwnerArea;
            totalActualArea = Math.Round(totalActualArea, 4);
            SetTableCellValue(0, rowIndex + 1, 4, ToolMath.SetNumbericFormat(totalActualArea.ToString(), 2));
            double totalCollArea = ownerArea + actOwnerArea;
            totalCollArea = Math.Round(totalCollArea, 4);
            SetTableCellValue(0, rowIndex + 2, 1, ToolMath.SetNumbericFormat(totalCollArea.ToString(), 2));
            double totalGroupArea = groupArea + actGroupArea;
            totalGroupArea = Math.Round(totalGroupArea, 4);
            SetTableCellValue(0, rowIndex + 2, 3, ToolMath.SetNumbericFormat(totalGroupArea.ToString(), 2));
            double totalVillageArea = villageArea + actVillageArea;
            totalVillageArea = Math.Round(totalVillageArea, 4);
            SetTableCellValue(0, rowIndex + 2, 2, ToolMath.SetNumbericFormat(totalVillageArea.ToString(), 2));
        }

        #endregion

        #endregion
    }
}
