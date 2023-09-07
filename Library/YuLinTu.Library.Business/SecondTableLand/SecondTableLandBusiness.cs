/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Unity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Office;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 二轮台账地块业务处理
    /// </summary>
    public class SecondTableLandBusiness : Task
    {
        #region Fields

        /// <summary>
        /// 数据库
        /// </summary>
        private IDbContext dbContext;

        /// <summary>
        /// 是否有错
        /// </summary>
        private bool isErrorRecord;

        /// <summary>
        /// 承包方类型
        /// </summary>
        private eVirtualType virtualType;

        private SecondTableExportDefine secondTableDefine;

        #endregion

        #region Properties

        /// <summary>
        /// 二轮台账导出表格配置实体属性
        /// </summary>
        public SecondTableExportDefine SecondTableDefine
        {
            get { return secondTableDefine; }
            set { secondTableDefine = value; }
        }

        /// <summary>
        /// 二轮台账地块业务逻辑层
        /// </summary>
        public ISecondTableLandWorkStation Station { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType
        {
            get { return virtualType; }
            set { virtualType = value; }
        }

        /// <summary>
        /// 二轮台账(承包方)Station
        /// </summary>
        private IVirtualPersonWorkStation<TableVirtualPerson> tableStation;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db"></param>
        public SecondTableLandBusiness(IDbContext db)
        {
            dbContext = db;
            Station = db == null ? null : db.CreateSecondTableLandWorkstation();
            tableStation = db == null ? null : db.CreateVirtualPersonStation<TableVirtualPerson>();
        }

        #endregion

        #region Methods

        #region 数据处理

        /// <summary>
        /// 根据承包方id获取二轮台账地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>地块集合</returns>
        public List<SecondTableLand> GetCollection(Guid ownerId)
        {
            List<SecondTableLand> list = null;
            if (!CanContinue() || ownerId == null)
            {
                return list;
            }
            try
            {
                list = Station.GetCollection(ownerId);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取二轮台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取二轮台账地块集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        ///  根据地域编码获取二轮台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>地块集合</returns>
        public List<SecondTableLand> GetCollection(string zoneCode)
        {
            List<SecondTableLand> list = null;
            if (!CanContinue())
            {
                return list;
            }
            try
            {
                if (!string.IsNullOrEmpty(zoneCode))
                {
                    list = Station.GetCollection(zoneCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取二轮台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取二轮台账地块集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 逐条添加二轮地块信息
        /// </summary>
        /// <param name="secondLand">二轮地块对象</param>
        public int AddLand(SecondTableLand secondLand)
        {
            int addCount = 0;
            if (!CanContinue() || secondLand == null)
            {
                return addCount;
            }
            try
            {
                addCount = Station.Add(secondLand);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "AddLand(添加地块数据)", ex.Message + ex.StackTrace);
                this.ReportError("添加地块数据失败," + ex.Message);
            }
            return addCount;
        }

        /// <summary>
        /// 根据承包方ID删除下属地块信息
        /// </summary>
        /// <param name="guid">承包方标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteLandByPersonID(Guid guid)
        {
            int deltCount = 0;
            if (!CanContinue() || guid == null)
            {
                return deltCount;
            }
            try
            {
                deltCount = Station.DeleteLandByPersonID(guid);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "删除承包方下的地块", ex.Message + ex.StackTrace);
                this.ReportError("删除地块数据失败," + ex.Message);
            }
            return deltCount;
        }

        /// <summary>
        /// 逐条更新二轮地块信息
        /// </summary>
        /// <param name="secondLand">二轮地块</param>
        public int ModifyLand(SecondTableLand secondLand)
        {
            int modifyCount = 0;
            if (!CanContinue() || secondLand == null)
            {
                return modifyCount;
            }
            try
            {
                modifyCount = Station.Update(secondLand);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ModifyLand(编辑地块数据)", ex.Message + ex.StackTrace);
                this.ReportError("编辑地块数据失败," + ex.Message);
            }
            return modifyCount;
        }

        /// <summary>
        /// 逐条删除二轮地块信息
        /// </summary>
        /// <param name="secondLand">二轮地块</param>
        public int DeleteLand(SecondTableLand secondLand)
        {
            int DelCount = 0;
            if (!CanContinue() || secondLand == null)
            {
                return DelCount;
            }
            try
            {
                DelCount = Station.Delete(c => c.ID == secondLand.ID);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteLand(删除地块数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除地块数据失败," + ex.Message);
            }
            return DelCount;
        }

        /// <summary>
        ///  根据承包方id更新承包方名称
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="ownerName">承包方名称</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Guid guid, string ownerName)
        {
            int updateCount = 0;
            if (!CanContinue() || guid == null)
            {
                return updateCount;
            }
            try
            {
                updateCount = Station.Update(guid, ownerName);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "修改地块的权利认名称", ex.Message + ex.StackTrace);
                this.ReportError("修改地块的承包方名称数据失败," + ex.Message);
            }
            return updateCount;
        }

        /// <summary>
        /// 根据行政地域编码删除该地域下的所有地块
        /// </summary>
        /// <param name="zoneCode">行政地域编码</param>
        public int DeleteLandByZoneCode(string zoneCode)
        {
            int DelAllCount = 0;
            if (!CanContinue())
            {
                return DelAllCount;
            }
            try
            {
                if (!string.IsNullOrEmpty(zoneCode))
                {
                    DelAllCount = Station.DeleteByZoneCode(zoneCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteLandByZoneCode(删除地块数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除地块数据失败," + ex.Message);
            }
            return DelAllCount;
        }

        #endregion

        #region 导入

        /// <summary>
        /// 导入二轮台账调查表
        /// </summary>
        /// <param name="zone">行政地域</param>
        /// <param name="fileName">文件路径</param>
        /// <param name="isClear">是否清空</param>
        public void ImportData(Zone zone, string fileName, bool isClear)
        {
            isErrorRecord = false;
            try
            {
                using (ImportSecondSurveryTable secondTableImport = new ImportSecondSurveryTable())
                {
                    secondTableImport.ProgressChanged += ReportPercent;
                    secondTableImport.Alert += ReportInfo;
                    secondTableImport.CurrentZone = zone;
                    secondTableImport.IsClear = isClear;
                    secondTableImport.DbContext = this.dbContext;
                    secondTableImport.TableType = 1;   //这个是什么意思？
                    this.ReportProgress(1, "开始读取数据");
                    bool isReadSuccess = secondTableImport.ReadLandTableInformation(fileName);  //读取二轮台账调查表数据
                    this.ReportProgress(10, "开始检查数据");
                    bool canImport = secondTableImport.VerifyLandTableInformation();  //检查二轮台账调查表数据
                    if (isReadSuccess && canImport && !isErrorRecord)
                    {
                        this.ReportProgress(20, "开始处理数据");
                        secondTableImport.ImportLandEntity(); //将检查完毕的数据导入数据库
                        this.ReportProgress(100, "完成");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportData(导入二轮台账调查表数据)", ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 辅助判断方法
        /// </summary>
        public bool CanContinue()
        {
            if (Station == null)
            {
                this.ReportError("尚未初始化数据字典的访问接口");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        /// <summary>
        /// 错误信息报告
        /// </summary>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
                if (e.Grade == eMessageGrade.Error)
                    isErrorRecord = true;
            }
        }

        #endregion

        #region 导出二轮台账摸底调查公示表数据

        /// <summary>
        /// 导出二轮台账摸底调查公示表
        /// </summary>
        /// <param name="zone">地域</param>
        /// <param name="filePath">文件路径</param>
        public void ExportSurveyPublicTable(Zone zone, string filePath)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                List<SecondTableLandFamily> secondFamilyCollection = new List<SecondTableLandFamily>();

                //得到承包方的集合
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    this.ReportError("未获取到二轮台账数据!");
                    return;
                }
                var orderdVps = vps.OrderBy(vp =>
                {
                    int num = 0;
                    Int32.TryParse(vp.FamilyNumber, out num);
                    return num;
                });
                foreach (VirtualPerson vp in orderdVps)
                {
                    SecondTableLandFamily secondTableFamily = new SecondTableLandFamily();
                    secondTableFamily.CurrentFamily = vp;
                    secondTableFamily.Persons = vp.SharePersonList;
                    //得到承包地
                    secondTableFamily.LandCollection = GetCollection(vp.ID);
                    secondFamilyCollection.Add(secondTableFamily);
                }

                string reInfo = string.Format("成功导出{0}条二轮台账数据!", vps.Count);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.SecondTablePublicityExcel);
                using (ExportContractSurveyExcel export = new ExportContractSurveyExcel())
                {
                    export.SaveFilePath = Path.GetTempPath() + TemplateFile.SecondTablePublicityExcel + ".xls";
                    export.CurrentZone = zone;
                    export.TemplateFile = tempPath;
                    export.SecondTableLandFamilyList = secondFamilyCollection;
                    export.UnitName = GetUinitName(zone);            //得到单位名称 
                    export.DeclareTableName = GetTitleName(zone);    //得到表标题
                    export.SecondTableDefine = SecondTableDefine;  //配置信息
                    export.PostProgressEvent +=export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginToZone(zone.FullCode);
                    if (result)
                    {
                        System.Diagnostics.Process.Start(export.SaveFilePath);   //打开文件
                        this.ReportInfomation(reInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出二轮台账摸底调查公示表
        /// </summary>
        public void ExportSecondSurveyTable(Zone zone, string filePath)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                List<SecondTableLandFamily> secondFamilyCollection = new List<SecondTableLandFamily>();

                //得到承包方的集合
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    this.ReportError("未获取到二轮台账数据!");
                    return;
                }
                var orderdVps = vps.OrderBy(vp =>
                {
                    int num = 0;
                    Int32.TryParse(vp.FamilyNumber, out num);
                    return num;
                });
                foreach (VirtualPerson vp in orderdVps)
                {
                    SecondTableLandFamily secondTableFamily = new SecondTableLandFamily();
                    secondTableFamily.CurrentFamily = vp;
                    secondTableFamily.Persons = vp.SharePersonList;
                    //得到承包地
                    secondTableFamily.LandCollection = GetCollection(vp.ID);
                    secondFamilyCollection.Add(secondTableFamily);
                }

                string reInfo = string.Format("成功导出{0}条二轮台账数据!", vps.Count);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.SecondTableRealQueryExcel);
                using (ExportSecondLandPublicExcel export = new ExportSecondLandPublicExcel())
                {
                    export.SaveFilePath = Path.GetTempPath() + TemplateFile.SecondTableRealQueryExcel + ".xls";
                    export.CurrentZone = zone;
                    export.TemplateFile = tempPath;
                    export.SecondTableLandFamilyList = secondFamilyCollection;
                    export.UnitName = GetUinitName(zone);            //得到单位名称 
                    export.DeclareTableName = GetTitleName(zone);    //得到表标题
                    export.SecondTableDefine = SecondTableDefine;    //配置信息
                    export.PostProgressEvent +=export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginToZone(zone.FullCode);
                    if (result)
                    {
                        System.Diagnostics.Process.Start(export.SaveFilePath);   //打开文件
                        this.ReportInfomation(reInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出二轮台账摸底调查公示确认表
        /// </summary>
        public void ExportSecondLandSignTable(Zone zone, string filePath)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                List<SecondTableLandFamily> secondFamilyCollection = new List<SecondTableLandFamily>();

                //得到承包方的集合
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    this.ReportError("未获取到二轮台账数据!");
                    return;
                }
                var orderdVps = vps.OrderBy(vp =>
                {
                    int num = 0;
                    Int32.TryParse(vp.FamilyNumber, out num);
                    return num;
                });
                foreach (VirtualPerson vp in orderdVps)
                {
                    SecondTableLandFamily secondTableFamily = new SecondTableLandFamily();
                    secondTableFamily.CurrentFamily = vp;
                    secondTableFamily.Persons = vp.SharePersonList;
                    //得到承包地
                    secondTableFamily.LandCollection = GetCollection(vp.ID);
                    secondFamilyCollection.Add(secondTableFamily);
                }

                string reInfo = string.Format("成功导出{0}条二轮台账数据!", vps.Count);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.SecondTableSignExcel);
                using (ExportSecondLandSignExcel export = new ExportSecondLandSignExcel())
                {
                    export.SaveFilePath = Path.GetTempPath() + TemplateFile.SecondTableSignExcel + ".xls";
                    export.CurrentZone = zone;
                    export.TemplateFile = tempPath;
                    export.SecondTableLandFamilyList = secondFamilyCollection;
                    export.UnitName = GetUinitName(zone);            //得到单位名称 
                    export.DeclareTableName = GetTitleName(zone);    //得到表标题
                    export.SecondTableDefine = SecondTableDefine;  //配置信息
                    export.PostProgressEvent +=export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginToZone(zone.FullCode);
                    if (result)
                    {
                        System.Diagnostics.Process.Start(export.SaveFilePath);   //打开文件
                        this.ReportInfomation(reInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出二轮台账用户确认表
        /// </summary>
        /// <param name="zone"></param>
        public void ExportSecondUserSignTable(Zone zone, string fileName, DateTime? time)
        {

            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    this.ReportError("未获取到承包方数据!");
                    return;
                }

                this.ReportProgress(1, null);
                int index = 1;
                string templatePath = TemplateHelper.ExcelTemplate(TemplateFile.SecondTableUserSignExcel);
                this.ReportProgress(10, null);
                double percent = 90 / (double)vps.Count;
                foreach (VirtualPerson family in vps)
                {
                    if (family == null)
                    {
                        continue;
                    }
                    index++;
                    this.ReportProgress((int)(10 + percent * index), family.Name);
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);

                    using (ExportSecondLandFamilyExcel exportFamily = new ExportSecondLandFamilyExcel())
                    {
                        List<SecondTableLand> landCollection = GetCollection(family.ID);
                        List<Person> persons = family.SharePersonList;
                        if (persons == null)
                        {
                            persons = new List<Person>();
                        }
                        if (landCollection == null)
                        {
                            landCollection = new List<SecondTableLand>();
                        }
                        exportFamily.VP = family;
                        exportFamily.Persons = persons;
                        exportFamily.LandCollection = landCollection;
                        exportFamily.SaveFilePath = fileName;
                        exportFamily.UnitName = GetUinitName(zone); //得到单位名称
                        exportFamily.DeclareTableName = GetTitleName(zone);   //得到表标题
                        exportFamily.CurrentZone = zone;
                        exportFamily.SecondTableDefine = SecondTableDefine;  //配置信息
                        exportFamily.TemplateFileName = templatePath;
                        exportFamily.BeginToVirtualPerson();

                    }
                }
                this.ReportProgress(100, "完成");
                this.ReportInfomation(string.Format("成功导出{0}个" + TemplateFile.SecondTableUserSignExcel, index - 1));
                vps = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportSecondLandFamilyExcel(单用户确认表)", ex.Message + ex.StackTrace);
            }


        }

        /// <summary>
        /// 根据地域编码获取承包方
        /// </summary>
        public List<VirtualPerson> GetByZone(string zoneCode)
        {
            List<VirtualPerson> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                list = tableStation.GetByZoneCode(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ZonesByCode(获取承包方数据集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包方数据出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
        public string GetUinitName(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = VirtualPersonMessage.VIRTUALPERSON_UNITNAME;
            TheBns.Current.Message.Send(this, arg);
            return arg.ReturnValue.ToString();
        }

        /// <summary>
        /// 获取表的标题
        /// </summary>
        public string GetTitleName(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone.FullCode;
            arg.Name = SecondTableLandMessage.CURRENTZONE_UNITNAME;
            TheBns.Current.Message.Send(this, arg);
            return arg.ReturnValue.ToString();
        }

        /// <summary>
        /// 报告进度
        /// </summary>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        /// <summary>
        ///  错误信息报告
        /// </summary>
        private void export_PostErrorInfoEvent(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.ReportError(message);
            }
        }

        #endregion

        #region 导出勘界确权数据

        /// <summary>
        /// 导出勘界调查表
        /// </summary>
        /// <param name="zone">行政地域对象</param>
        /// <param name="fileName">文件名称</param>
        public void ExportBoundarySettleTable(Zone zone, string fileName)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                List<SecondTableLandFamily> secondFamilyCollection = new List<SecondTableLandFamily>();
                //得到承包方的集合
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    this.ReportError("未获取到承包方数据!");
                    return;
                }
                var orderdVps = vps.OrderBy(vp =>
                {
                    int num = 0;
                    Int32.TryParse(vp.FamilyNumber, out num);
                    return num;
                });
                foreach (VirtualPerson vp in orderdVps)
                {
                    SecondTableLandFamily secondTableFamily = new SecondTableLandFamily();
                    secondTableFamily.CurrentFamily = vp;
                    secondTableFamily.Persons = vp.SharePersonList;
                    //得到承包地
                    secondTableFamily.LandCollection = GetCollection(vp.ID);
                    secondFamilyCollection.Add(secondTableFamily);
                }
                string reInfo = string.Format("成功导出{0}条二轮台账数据!", vps.Count);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.SecondTableBoundarySettleExcel);
                using (ExportBoundarySettleExcel export = new ExportBoundarySettleExcel())
                {
                    export.SaveFilePath = Path.GetTempPath() + TemplateFile.SecondTableBoundarySettleExcel + ".xls";
                    export.DbContext = this.dbContext;
                    export.CurrentZone = zone;
                    export.TemplateFile = tempPath;
                    export.SecondTableLandFamilyList = secondFamilyCollection;
                    export.UnitName = GetUinitName(zone);            //得到单位名称 
                    export.TableName = GetTitleName(zone);    //得到表标题  
                    export.SecondTableDefine = SecondTableDefine;     //配置信息
                    export.PostProgressEvent +=export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginToBoundarySettle(zone.FullCode, tempPath);
                    if (result)
                    {
                        System.Diagnostics.Process.Start(export.SaveFilePath);   //打开文件
                        this.ReportInfomation(reInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportBoundarySettleTable(导出勘界调查数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出单户调查表
        /// </summary>
        /// <param name="zone">行政地域对象</param>
        /// <param name="fileName">文件名称</param>
        public void ExportSingleFamilyTable(Zone zone, string fileName)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                //获取模板路径
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.SecondTableSingleFamilyExcel);
                //获取单位名称
                string unitName = GetUinitName(zone);
                //获取标题名称
                string tableName = GetTitleName(zone);
                //得到承包方集合
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                //得到地块集合
                List<SecondTableLand> lands = GetCollection(zone.FullCode);
                int index = 0;
                if (vps == null || vps.Count == 0)
                {
                    this.ReportError("未获取到承包方数据!");
                    return;
                }
                foreach (var vp in vps)
                {
                    index++;
                    if (vp == null)
                    {
                        continue;
                    }
                    string familyNumber = ToolString.ExceptSpaceString(vp.FamilyNumber);
                    //获取保存路径 
                    string savePath = fileName + @"\" + familyNumber + "-" + vp.Name + TemplateFile.SecondTableSingleFamilyExcel + ".xls";
                    ExportSingleFamilyExcel export = new ExportSingleFamilyExcel();
                    export.SaveFilePath = savePath;
                    export.DbContext = this.dbContext;
                    export.CurrentZone = zone;
                    export.TemplateFile = tempPath;
                    export.UnitName = unitName;
                    export.TableName = tableName;
                    export.SecondTableDefine = SecondTableDefine;  //配置信息
                    export.listTableLand = lands;
                    export.listPersons = vps;
                    export.PostProgressEvent +=export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginToSingleFamily(vp, savePath);
                }
                this.ReportProgress(100, "完成");
                this.ReportInfomation(string.Format("成功导出{0}个" + TemplateFile.SecondTableSingleFamilyExcel, index));
                vps = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSingleFamilyTable(导出单户调查数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #endregion
    }
}
