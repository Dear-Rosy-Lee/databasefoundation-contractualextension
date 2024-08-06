/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightBase.Helper;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase.ExportTask
{
    /// <summary>
    /// 导出承包地块调查表任务类
    /// </summary>
    public class ExportWordTask : Task
    {

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }


        /// <summary>
        /// 待导出调查表的承包方集合
        /// </summary>
        public List<VirtualPerson> SelectedPersons { get; set; }


        /// <summary>
        /// 选择文件路径
        /// </summary>
        public string FileName { get; set; }


        public string TempName { get; set; }

        public AgricultureWordBook Book { get; set; }


        /// <summary>
        /// 开始执行任务
        /// </summary> 
        protected override void OnGo()
        {
            try
            {
                var listPerson = SelectedPersons;
                var senderStation = DbContext.CreateSenderWorkStation();
                var landStation = DbContext.CreateContractLandWorkstation();
                var dictStation = DbContext.CreateDictWorkStation();
                var listDict = dictStation.Get();
                var sender = senderStation.Get(CurrentZone.ID);
                var bookStation = DbContext.CreateRegeditBookStation();
                var connordStation = DbContext.CreateConcordStation();
                var connords = connordStation.GetByZoneCode(CurrentZone.FullCode);
                var books=bookStation.GetByZoneCode(CurrentZone.FullCode,eSearchOption.Precision);
                var zoneList = DbContext.CreateZoneWorkStation().Get();
                var dotList = DbContext.CreateBoundaryAddressDotWorkStation().Get(o=>o.ZoneCode==CurrentZone.FullCode);
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", CurrentZone.FullName));
                    return;
                }
                var listLand = landStation.Get(o=>o.ZoneCode.StartsWith(CurrentZone.FullCode)&&o.IsStockLand);
                if (listLand == null || listLand.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包地块数据!", CurrentZone.FullName));
                    return;
                }
                bool canOpen = ExportLandSurveyWordTable(zoneList, listPerson, listLand, listDict, sender,books,dotList);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportPublishWordOperation(导出"+Name+"任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出"+Name+"出现异常!");
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(FileName);
            base.OpenResult();
        }

        /// <summary>
        /// 导出数据到承包地块调查表
        /// </summary>
        public virtual bool ExportLandSurveyWordTable(List<Zone> zoneList,List<VirtualPerson> listPerson, List<ContractLand> listLand,
            List<Dictionary> listDict, CollectivityTissue tissue, List<Library.Entity.ContractRegeditBook> books, List<BuildLandBoundaryAddressDot> dotList)
        {
            bool result = false;
            try
            {
                if (CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                this.ReportProgress(1, "开始");
                int indexOfVp = 1;
                double vpPercent = 99 / (double)listPerson.Count;
                //var concordSet = ConcordSettingModel.GetIntence();
                //var warantSet = WarantSettingModel.GetIntence();
                foreach (VirtualPerson person in listPerson)
                {
                    if (person.IsStockFarmer)
                    {
                        var stockLandsvp = DbContext.CreateBelongRelationWorkStation().GetLandByPerson(person.ID, CurrentZone.FullCode);
                        if (stockLandsvp.Count == 0)
                            continue;
                        Book.StockLands = stockLandsvp;
                    }
                    string tempPath = TemplateHelper.WordTemplate(TempName);
                    Book.DbContext = DbContext;
                    Book.CurrentZone = CurrentZone;
                    Book.TemplateName = TempName;
                    Book.Contractor = person;
                    Book.DictList = listDict ?? new List<Dictionary>();
                    Book.Concord = DbContext.CreateStockConcordWorkStation().Get(o => o.ZoneCode == CurrentZone.FullCode && o.ContracterId == person.ID).FirstOrDefault();
                    if (Book.Concord == null)
                    {
                        this.ReportInfomation(person.Name + "没有合同");
                        continue;
                    }
                    Book.Book = DbContext.CreateStockWarrantWorkStation().Get(o => o.ID == Book.Concord.ID).FirstOrDefault();
                    if (Book is RegisterBookWord || Book is WarrantWord)
                    {
                        if (Book.Book == null)
                        {
                            this.ReportInfomation(person.Name + "没有没有权证");
                            continue;
                        }
                    }
                    Book.ListLandDots = dotList;
                    Book.Tissue = tissue; //发包方
                    Book.ZoneList = zoneList;
                    //Book.OpenTemplate(tempPath);
                    string fileName = FileName + @"\" + person.Name + "-" + Name;
                    if (Book as WarrantWord == null)
                    {
                        Book.OpenTemplate(tempPath);
                        Book.SaveAs(person, fileName);
                    } 
                    else
                    {
                        var warrant = Book as WarrantWord;
                        warrant.ExportContractLand(fileName);
                    }
                    string strDesc = string.Format("{0}", person.Name);
                    this.ReportProgress((int)(1 + vpPercent * indexOfVp), strDesc);
                    indexOfVp++;
                }
                result = true;
                string info = string.Format("{0}导出{1}户承包方,共{2}个"+Name, CurrentZone.FullName, listPerson.Count, indexOfVp - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出"+Name+"失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandWord(导出"+Name+")", ex.Message + ex.StackTrace);
            }
            return result;
        }


   

    }
}
