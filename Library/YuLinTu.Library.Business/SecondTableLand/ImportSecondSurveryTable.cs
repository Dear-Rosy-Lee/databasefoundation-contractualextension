/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 输入二轮台账调查表业务
    /// </summary>
    [Serializable]
    public class ImportSecondSurveryTable : Task
    {
        #region Field

        private ToolProgress toolProgress;//进度工具
        private List<LandFamily> landFamilys = new List<LandFamily>();
        private bool isContaionTableValue;
        private bool isContaionTableLandValue;
        private int familyCount;//承包方数
        private int personCount;//共有人数
        private int landCount;//地块数
        public VirtualPersonBusiness personBusiness;
        public SecondTableLandBusiness landBusiness;

        #endregion

        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 是否清空数据
        /// </summary>
        public bool IsClear { get; set; }

        /// <summary>
        /// 行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        #endregion

        #region Ctor

        public ImportSecondSurveryTable()
        {
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += toolProgress_OnPostProgress;
            VirtualType = eVirtualType.SecondTable;
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            this.ReportProgress(progress, info);
        }

        #endregion

        #region Method—读取表格数据

        /// <summary>
        /// 读取二轮台账调查表数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public bool ReadLandTableInformation(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }
            string message = string.Empty;
            string information = string.Empty;
            bool isOpened = false;
            bool isRead = false;
            using (ReadLandInformation landInfo = new ReadLandInformation(filePath))
            {
                landInfo.FileName = filePath;
                landInfo.CurrentZone = CurrentZone;
                landInfo.TableType = TableType;
                landInfo.DbContext = DbContext;
                isOpened = landInfo.OpenLandFile();  //打开文件
                isRead = landInfo.ReadTableInformation(); //读取文件
                message = landInfo.ErrorInformation; //错误信息
                information = landInfo.PromptInformation; //提示信息
                landFamilys = landInfo.LandFamilyCollection;
                isContaionTableValue = landInfo.IsContaionTableValue;
                isContaionTableLandValue = landInfo.IsContaionTableLandValue;

            }
            if (!string.IsNullOrEmpty(information))
            {
                string[] messages = information.Split('!');
                for (int index = 0; index < messages.Length; index++)
                {
                    if (string.IsNullOrEmpty(messages[index]))
                    {
                        continue;
                    }
                    this.ReportError(messages[index]);
                }
                information = string.Empty;
                return false;
            }
            if (!string.IsNullOrEmpty(message))
            {
                string[] messages = message.Split('!');
                for (int index = 0; index < messages.Length; index++)
                {
                    if (string.IsNullOrEmpty(messages[index]))
                    {
                        continue;
                    }
                    this.ReportError(messages[index]);
                }
                message = string.Empty;
                return false;
            }
            if (!isOpened || !isRead)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Method—判断表格数据

        /// <summary>
        /// 校验承包台账调查表信息
        /// </summary>
        /// <returns></returns>
        public bool VerifyLandTableInformation()
        {
            VerifyLandInformation verifyLand = new VerifyLandInformation(landFamilys);
            bool success = verifyLand.CheckImportSecondTableData();
            ReportInformation(verifyLand.ErrorInformation, true);
            ReportInformation(verifyLand.WarnInformation, false);
            return success;
        }

        #endregion

        #region Method—导入数据库

        /// <summary>
        /// 导入实体
        /// </summary>
        public bool ImportLandEntity()
        {
            try
            {
                if (landFamilys == null || landFamilys.Count == 0)
                {
                    return false;
                }
                //清空数据
                ClearData();
                //获取目标数据源中当前地域下的所有户信息，用于之后的快速判断，减少数据库的交互次数
                toolProgress.InitializationPercent(landFamilys.Count, 80);
                int familyIndex = 1;
                foreach (LandFamily landFamily in landFamilys)
                {
                    ImportTableInformation(landFamily, familyIndex);//导入承包地、承包方
                    familyIndex++;
                    toolProgress.DynamicProgress();
                }
                this.ReportInfomation(string.Format("当前表中共有{0}户数据,成功导入{1}户、{2}条共有人记录、{3}宗地块记录!", landFamilys.Count, landFamilys.Count, personCount, landCount));
                //return true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteError(this, "ImportLandEntity(导入实体)", ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 清空所有相关数据
        /// </summary>
        public void ClearData()
        {
            personBusiness = new VirtualPersonBusiness(DbContext);
            personBusiness.VirtualType = this.VirtualType;
            landBusiness = new SecondTableLandBusiness(DbContext);
            landBusiness.VirtualType = this.VirtualType;
            personBusiness.ClearZoneData(CurrentZone.FullCode);
            landBusiness.DeleteLandByZoneCode(CurrentZone.FullCode);
        }

        /// <summary>
        /// 导入二轮承包方
        /// </summary>
        /// <param name="landFamily"></param>
        /// <param name="familyIndex"></param>
        /// <returns></returns>
        private void ImportTableInformation(LandFamily landFamily, int familyIndex)
        {
            if (isContaionTableValue)
            {
                InportTableVirtualPerson(landFamily);
            }
            if (isContaionTableLandValue)
            {
                ImportTableLandInformation(landFamily);
            }
        }

        /// <summary>
        /// 导入承包方信息
        /// </summary>
        /// <param name="landFamily"></param>
        private void InportTableVirtualPerson(LandFamily landFamily)
        {
            VirtualPerson vp = new VirtualPerson();
            vp.ID = landFamily.TableFamily.ID;
            vp.Name = landFamily.TableFamily.Name;
            vp.FamilyNumber = landFamily.TableFamily.FamilyNumber;
            vp.TotalArea = landFamily.TableFamily.TotalArea;
            vp.TotalTableArea = landFamily.TableFamily.TotalTableArea;
            vp.TotalActualArea = landFamily.TableFamily.TotalActualArea;
            vp.TotalAwareArea = landFamily.TableFamily.TotalAwareArea;
            vp.TotalModoArea = landFamily.TableFamily.TotalModoArea;
            List<Person> personList = new List<Person>();
            //生成承包方中的共有人信息
            foreach (Person per in landFamily.TablePersons)
            {
                if (per == null)
                {
                    continue;
                }
                if (per.Name == landFamily.TableFamily.Name)
                {
                    vp.Number = per.ICN;
                }
                personList.Add(per);
                personCount++;
            }
            vp.PersonCount = landFamily.TableFamily.PersonCount != null ? landFamily.TableFamily.PersonCount.ToString() : personList.Count.ToString();
            vp.SharePersonList = personList;
            vp.ZoneCode = CurrentZone.FullCode;
            vp.VirtualType = eVirtualPersonType.Family;
            vp.Address = CurrentZone.FullName;
            vp.CreationTime = DateTime.Now;
            vp.FamilyExpand = new VirtualPersonExpand();
            personBusiness.Add(vp);
            familyCount++;
        }

        /// <summary>
        /// 导入承包地块信息
        /// </summary>
        /// <param name="landFamily"></param>
        private void ImportTableLandInformation(LandFamily landFamily)
        {
            if (landFamily.TableLandCollection == null || landFamily.TableLandCollection.Count == 0)
            {
                return;
            }
            foreach (SecondTableLand land in landFamily.TableLandCollection)
            {
                land.OwnerId = landFamily.TableFamily.ID;
                landBusiness.AddLand(land);
                landCount++;
            }
        }

        #endregion

        #region Method—其它方法

        /// <summary>
        /// 报告信息
        /// </summary>
        private void ReportInformation(string message, bool isError)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            string[] messages = message.Split('!');
            for (int index = 0; index < messages.Length; index++)
            {
                if (string.IsNullOrEmpty(messages[index]))
                {
                    continue;
                }
                this.ReportError(messages[index]);
            }
        }

        #endregion
    }
}
