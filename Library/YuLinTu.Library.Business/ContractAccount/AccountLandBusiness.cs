/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Scripting.Utils;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Diagrams;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账地块业务处理
    /// </summary>
    public class AccountLandBusiness : Task
    {
        #region Fields

        private IDbContext dbContext;
        private bool isErrorRecord;
        public List<string> ErrorInformation;
        private eVirtualType virtualType;
        private IContractLandWorkStation landStation;//承包台账地块业务逻辑层
        private IVirtualPersonWorkStation<LandVirtualPerson> tableStation;  //承包台账(承包方)Station
        private double projectionUnit = 0.0015; //空间参考系单位换算亩系数

        private IVirtualPersonWorkStation<LandVirtualPerson> landVirtualPersonStation;
        private bool _isCheckLandNumberRepeat = true;
        private ContractBusinessParcelWordSettingDefine ParcelWordSettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();

        #endregion Fields

        #region Properties

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 是否验证地块编码重复 不要删除此属性  安徽插件使用
        /// </summary>
        public bool IsCheckLandNumberRepeat
        {
            get { return _isCheckLandNumberRepeat; }
            set { _isCheckLandNumberRepeat = value; }
        }

        /// <summary>
        /// 表格类型
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType
        {
            get { return virtualType; }
            set { virtualType = value; }
        }

        /// <summary>
        /// 任务
        /// </summary>
        public TaskContractAccountArgument meta
        { get; set; }

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList
        {
            get
            {
                DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
                List<Dictionary> dictList = dictBusiness.GetAll();
                return dictList;
            }
        }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 当前百分比
        /// </summary>
        public double percent { get; set; }

        #region Properties - 导入地块图斑

        /// <summary>
        /// 按照地块编码绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseLandCodeBindImport { get; set; }

        /// <summary>
        /// 按照承包方信息绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseContractorInfoImport { get; set; }

        /// <summary>
        /// 按照承包方户号绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseContractorNumberImport { get; set; }

        /// <summary>
        /// 按照原地块编码绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseOldLandCodeBindImport { get; set; }

        /// <summary>
        /// 地块图斑导入设置实体
        /// </summary>
        public ImportAccountLandShapeSettingDefine ImportLandShapeInfoDefine =
            ImportAccountLandShapeSettingDefine.GetIntence();

        /// <summary>
        /// 读取的shp所有字段名称
        /// </summary>
        public List<KeyValue<int, string>> shapeAllcolNameList { get; set; }

        /// <summary>
        /// 空间参考系
        /// </summary>
        public YuLinTu.Spatial.SpatialReference shpRef { get; set; }

        #endregion Properties - 导入地块图斑

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db"></param>
        public AccountLandBusiness(IDbContext db)
        {
            dbContext = db;
            landStation = db == null ? null : db.CreateContractLandWorkstation();
            landVirtualPersonStation = db == null ? null : db.CreateVirtualPersonStation<LandVirtualPerson>();
            tableStation = db == null ? null : db.CreateVirtualPersonStation<LandVirtualPerson>();
        }

        #endregion Ctor

        #region Methods

        #region 数据处理

        /// <summary>
        /// 根据id获取承包台账地块
        /// </summary>
        /// <param name="id">id</param>
        public ContractLand GetLandById(Guid id)
        {
            ContractLand land = null;
            if (!CanContinue() || id == null)
            {
                return land;
            }
            try
            {
                land = landStation.Get(id);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Get(获取承包台账地块失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包台账地块失败," + ex.Message);
            }
            return land;
        }

        /// <summary>
        /// 判断地块编码是否存在
        /// </summary>
        /// <param name="landNumber">地块编码</param>
        /// <param name="id">地块ID</param>
        /// <returns></returns>
        public bool IsLandNumberReapet(string landNumber, Guid id, string zoneCode)
        {
            if (!CanContinue())
            {
                return false;
            }
            if (string.IsNullOrEmpty(landNumber))
            {
                return false;
            }
            bool result = false;
            try
            {
                if (id == null)
                {
                    result = landStation.Any(t => t.LandNumber == landNumber && t.ZoneCode == zoneCode);
                }
                else
                {
                    result = landStation.Any(t => t.LandNumber == landNumber && t.ID != id && t.ZoneCode == zoneCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "IsLandNumberReapet(判断地块编码是否存在)", ex.Message + ex.StackTrace);
                this.ReportError("判断地块编码是否存在失败," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 根据id删除承包台账地块
        /// </summary>
        /// <param name="id">id</param>
        public int Delete(Guid id)
        {
            int land = 0;
            if (!CanContinue() || id == null)
            {
                return land;
            }
            try
            {
                land = landStation.Delete(id);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Delete(删除承包台账地块失败)", ex.Message + ex.StackTrace);
                this.ReportError("删除承包台账地块失败," + ex.Message);
            }
            return land;
        }

        /// <summary>
        /// 根据承包方id获取承包台账地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>地块集合</returns>
        public List<ContractLand> GetPersonCollection(Guid ownerId)
        {
            List<ContractLand> list = null;
            if (!CanContinue() || ownerId == null)
            {
                return list;
            }
            try
            {
                list = landStation.GetCollection(ownerId);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包台账地块集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 根据合同Id获取地块信息
        /// </summary>
        /// <param name="concordId">合同Id</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLand> GetByConcordId(Guid concordId)
        {
            List<ContractLand> list = null;
            if (!CanContinue() || concordId == null)
            {
                return list;
            }
            try
            {
                list = landStation.GetByConcordId(concordId);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByConcordId(获取承包台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包台账地块集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 获取新地域编码
        /// </summary>
        public string GetNewLandNumber(string zoneCode)
        {
            var dbq = dbContext.CreateQuery();
            int maxLandNumber = 0;

            string sql = "select max(cast(subStr(DKBM,-5,5) as int)) from ZD_CBD " +
                          "where ZLDM = '" + zoneCode + "'";

            dbq.CommandContext.CommandText = new StringBuilder(sql);
            dbq.CommandContext.Type = eCommandType.Select;
            dbq.CommandContext.ExecuteArgument = eDbExecuteType.Scalar;
            var retsult = dbq.Execute();
            int.TryParse(retsult.ToString(), out maxLandNumber);
            maxLandNumber++;

            //重新组建编码
            string unitCode = zoneCode;
            if (unitCode.Length == 16)
                unitCode = unitCode.Substring(0, 12) + unitCode.Substring(14, 2);
            unitCode = unitCode.PadRight(14, '0');
            string code = unitCode + maxLandNumber.ToString().PadLeft(5, '0');

            return code;
        }

        /// <summary>
        ///  根据地域编码获取承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>地块集合</returns>
        public List<ContractLand> GetLandCollection(string zoneCode)
        {
            List<ContractLand> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                if (zoneCode.Length == 14)
                    //list = landStation.Get(c => c.ZoneCode == zoneCode);
                    list = DataBaseSource.GetDataBaseSource().CreateQuery<ContractLand>().Where(l => l.ZoneCode == zoneCode).ToList();
                else
                    list = landStation.GetCollection(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包台账地块集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        ///  根据承包方编码地块集合
        /// </summary>
        /// <param name="persons">地域编码</param>
        /// <returns>地块集合</returns>
        public List<ContractLand> GetLandbyPersons(List<Guid> personids)
        {
            List<ContractLand> list = null;
            if (!CanContinue() || personids.Count == 0)
            {
                return list;
            }
            try
            {
                list = landStation.GetLandsByObligeeIds(personids.ToArray());
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包台账地块集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        ///  根据地域编码统计承包地块数量
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>地块数</returns>
        public int CountLandByZone(string zoneCode)
        {
            int count = 0;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return count;
            }
            try
            {
                count = landStation.Count(zoneCode, eLevelOption.Self);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "CountLandByZone(统计地域下地块数量)", ex.Message + ex.StackTrace);
                this.ReportError("根据地域编码统计承包地块数量失败," + ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 根据地域编码和匹配等级获取承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="option">匹配等级</param>
        /// <returns>地块集合</returns>
        public List<ContractLand> GetCollection(string zoneCode, eLevelOption option, bool thr = false)
        {
            List<ContractLand> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                list = landStation.GetCollection(zoneCode, option);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包台账地块集合失败," + ex.Message);
                if (thr)
                {
                    throw new Exception("获取承包台账地块集合失败，请检查或升级数据库");
                }
            }
            return list;
        }

        /// <summary>
        /// 根据承包方id获取承包台账地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLand> GetCollection(Guid ownerId)
        {
            List<ContractLand> list = null;
            if (!CanContinue())
            {
                return list;
            }
            try
            {
                list = landStation.GetCollection(ownerId);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包台账地块集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 逐条添加承包地块信息
        /// </summary>
        /// <param name="secondLand">承包地块对象</param>
        public int AddLand(ContractLand secondLand)
        {
            int addCount = 0;
            if (!CanContinue() || secondLand == null)
            {
                return addCount;
            }
            try
            {
                addCount = landStation.Add(secondLand);
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
                deltCount = landStation.DeleteLandByPersonID(guid);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "删除承包方下的地块", ex.Message + ex.StackTrace);
                this.ReportError("删除地块数据失败," + ex.Message);
            }
            return deltCount;
        }

        /// <summary>
        /// 逐条更新承包地块信息
        /// </summary>
        /// <param name="secondLand">承包地块</param>
        public int ModifyLand(ContractLand secondLand)
        {
            int modifyCount = 0;
            if (!CanContinue() || secondLand == null)
            {
                return modifyCount;
            }
            try
            {
                modifyCount = landStation.Update(secondLand);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ModifyLand(编辑地块数据)", ex.Message + ex.StackTrace);
                this.ReportError("编辑地块数据失败," + ex.Message);
            }
            return modifyCount;
        }

        /// <summary>
        /// 逐条删除承包地块信息
        /// </summary>
        /// <param name="secondLand">承包地块</param>
        public int DeleteLand(ContractLand secondLand)
        {
            int DelCount = 0;
            if (!CanContinue() || secondLand == null)
            {
                return DelCount;
            }
            try
            {
                DelCount = landStation.Delete(c => c.ID == secondLand.ID);
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
                updateCount = landStation.Update(guid, ownerName);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "修改地块的权利认名称", ex.Message + ex.StackTrace);
                this.ReportError("修改地块的承包方名称数据失败," + ex.Message);
            }
            return updateCount;
        }

        /// <summary>
        ///  批量更新地块发包方信息
        /// </summary>
        public int UpdateLands(string zoneCode, CollectivityTissue tissue)
        {
            int updateCount = 0;
            if (!CanContinue())
            {
                return updateCount;
            }
            try
            {
                var lands = landStation.GetCollection(zoneCode);
                foreach (var land in lands)
                {
                    land.ZoneCode = tissue.ZoneCode;
                    land.SenderCode = tissue.Code;
                }
                updateCount = landStation.UpdateRange(lands);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "修改地块的权利认名称", ex.Message + ex.StackTrace);
                this.ReportError("修改地块的承包方名称数据失败," + ex.Message);
            }
            return updateCount;
        }

        public int UpdateLands(List<ContractLand> lands)
        {
            return landStation.UpdateRange(lands);
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
                    DelAllCount = landStation.DeleteByZoneCode(zoneCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteLandByZoneCode(删除地块数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除地块数据失败," + ex.Message);
            }
            return DelAllCount;
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
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        list = landVirtualPersonStation.GetByZoneCode(zoneCode);
                        break;
                        //case eVirtualType.Yard:
                        //    list = yardStation.GetByZoneCode(zoneCode);
                        //    break;
                        //case eVirtualType.House:
                        //    list = houseStation.GetByZoneCode(zoneCode);
                        //    break;
                        //case eVirtualType.Wood:
                        //    list = woodStation.GetByZoneCode(zoneCode);
                        //    break;
                        //case eVirtualType.CollectiveLand:
                        //    list = colleStation.GetByZoneCode(zoneCode);
                        //    break;
                        //case eVirtualType.SecondTable:
                        //    list = tableStation.GetByZoneCode(zoneCode);
                        //    break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ZonesByCode(获取承包方数据集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包方数据出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 根据地域编码获取二轮承包方
        /// </summary>
        public List<VirtualPerson> GetTableByZone(string zoneCode)
        {
            List<VirtualPerson> list = null;
            IVirtualPersonWorkStation<TableVirtualPerson> tbStation = dbContext.CreateVirtualPersonStation<TableVirtualPerson>();
            if (tbStation == null || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                list = tbStation.GetByZoneCode(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetTableByZone(获取二轮承包方数据集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取二轮承包方数据出错," + ex.Message);
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

        #endregion 数据处理

        #region 导入数据

        /// <summary>
        /// 导入承包台账地块调查表
        /// </summary>
        public bool ImportData(Zone zone, string fileName, bool isNotLand)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                this.ReportProgress(100, null);
                this.ReportWarn(zone.FullName + "未获取地块调查表,或选择表格路径不正确,请检查执行导入操作!");
                return false;
            }
            bool isSuccess = false;
            isErrorRecord = false;
            try
            {
                using (var landTableImport = new ImportContractAccountLandTable())
                {
                    //List<VirtualPerson> persons = GetByZone(zone.FullCode);
                    string excelName = GetMarkDesc(zone);
                    landTableImport.ProgressChanged += ReportPercent; //进度条
                    landTableImport.Alert += ReportInfo;              //记录错误信息
                    landTableImport.CurrentZone = zone;
                    landTableImport.ExcelName = excelName;
                    landTableImport.VirtualType = this.VirtualType;
                    landTableImport.TableType = TableType;
                    landTableImport.DbContext = this.dbContext;
                    landTableImport.Percent = 95.0;
                    landTableImport.CurrentPercent = 5.0;
                    landTableImport.IsCheckLandNumberRepeat = IsCheckLandNumberRepeat;
                    //landTableImport.ListPerson = persons;
                    this.ReportProgress(1, "开始读取数据");
                    bool isReadSuccess = landTableImport.ReadLandTableInformation(fileName, isNotLand);  //读取承包台账调查表数据
                    //landTableImport.MergeHouseData();
                    this.ReportProgress(3, "开始检查数据");
                    bool canImport = landTableImport.VerifyLandTableInformation();   //检查承包台账调查表数据
                    if (isReadSuccess && canImport && !isErrorRecord)
                    {
                        this.ReportProgress(5, "开始处理数据");
                        landTableImport.ImportLandEntity();   //将检查完毕的数据导入数据库
                        this.ReportProgress(100, "完成");
                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError("导入承包台账地块调查表失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ImportData(导入承包台账调查表地块数据)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        /// <summary>
        /// 批量导入地块数据调查表
        /// </summary>
        public void BatchImport(Zone zone, string fileName, bool isClear)
        {
            var tableFiles = Directory.GetFiles(fileName);   //获得文件夹下的所有表格文件
            if (tableFiles.Count() == 0)
            {
                return;
            }
            this.ReportProgress(1, "开始");
            List<Zone> zones = GetChildrenZone(zone);     //获取当期村级地域下的所有子级组地域集合
            zones.Add(zone);   //
            //this.ReportProgress(10, null);
            int count = 0;   //计数器
            int index = 0;
            double percent = 99 / (double)tableFiles.Count();
            foreach (var tableFile in tableFiles)
            {
                foreach (var zoneItem in zones)
                {
                    string zoneName = zoneItem.FullName;
                    string tableName = Path.GetFileNameWithoutExtension(tableFile).Replace("承包地块调查表", "");
                    if (zoneName.Contains(tableName))
                    {
                        //按地域名称进行匹配
                        double percentCount = 1 + index * percent;
                        this.ReportProgress((int)percentCount, tableName);
                        bool isSuccess = true;  // ImportData(zoneItem, tableFile, isClear, false, percent, percentCount);
                        index++;
                        if (isSuccess)
                        {
                            count++;
                        }
                    }
                }
            }
            this.ReportProgress(100, null);
            this.ReportInfomation(string.Format("导入成功{0}个表格,失败{1}个表格", count, tableFiles.Count() - count));
        }

        public bool ImportLandTies(Zone zone, string fileName, eImportTypes eImport)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                this.ReportProgress(100, null);
                this.ReportWarn(zone.FullName + "未获取地块调查表,或选择表格路径不正确,请检查执行导入操作!");
                return false;
            }
            bool isSuccess = false;
            isErrorRecord = false;
            try
            {
                ImportLandTiesTable landTableImport = new ImportLandTiesTable();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(landTableImport);
                if (temp != null)
                {
                    if (temp is ImportLandTiesTable)
                    {
                        landTableImport = (ImportLandTiesTable)temp;
                    }
                }

                #endregion

                //List<VirtualPerson> persons = GetByZone(zone.FullCode);
                string excelName = GetMarkDesc(zone);
                landTableImport.ProgressChanged += ReportPercent; //进度条
                landTableImport.Alert += ReportInfo;              //记录错误信息
                landTableImport.CurrentZone = zone;
                landTableImport.ExcelName = excelName;
                landTableImport.TableType = TableType;
                landTableImport.DbContext = this.dbContext;
                landTableImport.Percent = 95.0;
                landTableImport.CurrentPercent = 5.0;
                landTableImport.IsCheckLandNumberRepeat = IsCheckLandNumberRepeat;
                landTableImport.ImportType = eImport;
                //landTableImport.ListPerson = persons;
                this.ReportProgress(1, "开始读取数据");
                bool isReadSuccess = landTableImport.ReadLandTableInformation(fileName);  //读取承包台账调查表数据
                ErrorInformation = landTableImport.ErrorInformation;
                //landTableImport.MergeHouseData();
                this.ReportProgress(3, "开始检查数据");
                //bool canImport = landTableImport.VerifyLandTableInformation();   //检查承包台账调查表数据
                bool canImport = true;
                if (isReadSuccess && canImport && !isErrorRecord)
                {
                    this.ReportProgress(5, "开始处理数据");
                    landTableImport.ImportLandEntity();   //将检查完毕的数据导入数据库
                    this.ReportProgress(100, "完成");
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ErrorInformation.Add("导入承包台账地块调查表失败! \n" + ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportData(导入承包台账调查表地块数据)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        /// <summary>
        /// 导入地块图斑数据shape等信息-最小以组为单位
        /// </summary>
        /// <param name="shapeDataList">读取的shp数据</param>
        /// <param name="currentPercent">当前的进度</param>
        /// <param name="indexPercent">总进度分区</param>
        public int ImportLandShapeDataInfo(Zone zone, IList shapeDataList, double currentPercent, double indexPercent = 0.0)
        {
            //导出个数统计
            int successCount = 0;
            //RefreshMapControlSpatialUnit();
            //获取下拉列表所有字段，包括未选的，与地块配置实体属性顺序保持一致性
            List<string> allGetSelectColList = getSelectColList();
            string zoneNameInfo = GetMarkDesc(zone);

            var targetSpatialReference = dbContext.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(ContractLand)).Schema,
                ObjectContext.Create(typeof(ContractLand)).TableName);
            if (dbContext == null)
            {
                this.ReportError(DataBaseSource.ConnectionError);
                return 0;
            }

            //按照地块编码来导入修改
            if (UseLandCodeBindImport)
            {
                successCount = ImportByLandCode(zone, shapeDataList, allGetSelectColList, currentPercent, indexPercent, zoneNameInfo, targetSpatialReference);
            }
            //按照承包方信息来修改-需要先删除之前所有的地块数据
            if (UseContractorInfoImport)
            {
                successCount = ImportByFamilyCode(zone, shapeDataList, allGetSelectColList, currentPercent, indexPercent, zoneNameInfo, targetSpatialReference);
            }
            //按照承包方户号来修改-需要先删除之前所有的地块数据
            if (UseContractorNumberImport)
            {
                successCount = ImportByFamilyNumberCode(zone, shapeDataList, allGetSelectColList, currentPercent, indexPercent, zoneNameInfo, targetSpatialReference);
            }
            //按照原地块编码来导入修改
            if (UseOldLandCodeBindImport)
            {
                successCount = ImportByOldLandCode(zone, shapeDataList, allGetSelectColList, currentPercent, indexPercent, zoneNameInfo, targetSpatialReference);
            }
            return successCount;
        }

        /// <summary>
        /// 按照地块编码来导入修改
        /// </summary>
        private int ImportByLandCode(Zone zone, IList shapeDataList, List<string> allGetSelectColList,
           double currentPercent, double indexPercent, string zoneNameInfo, SpatialReference targetSpatialReference)
        {
            int importCount = 0;
            int successCount = 0;
            double indexZonePercent = 0;
            //获取地域下所有地
            List<ContractLand> zoneLandList = new List<ContractLand>();
            zoneLandList = GetCollection(zone.FullCode, eLevelOption.Self);
            if (zoneLandList == null) return 0;
            ContractLand modifyLand = null;
            List<ContractLand> modifyLandList = new List<ContractLand>();
            IList<object> modifyData = new List<object>();

            //循环每一个shape地域下地块,获取需要修改的地块集合
            foreach (var shpLandItem in shapeDataList)
            {
                modifyLand = new ContractLand();
                try
                {
                    var shplandnum = (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[3]) as string).TrimEnd('\0');
                    modifyLand = zoneLandList.Find(t => t.LandNumber == shplandnum);
                }
                catch
                {
                    this.ReportError("当前地块编码选择项下无匹配字段数据,请检查Shape数据中地块编码是否有误");
                    return 0;
                }
                if (modifyLand == null)
                {
                    this.ReportWarn(string.Format("Shape地块编码为{0}的数据在{1}台账中未找到匹配项", (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[3]) as string).TrimEnd('\0'), zone.FullName));
                    continue;
                }
                modifyLandList.Add(modifyLand);
                modifyData.Add(shpLandItem);
                importCount++;
            }
            if (modifyLandList.Count == 0)
            {
                this.ReportError(string.Format("{0}下无匹配数据", zone.FullName));
                return 0;
            }
            Parallel.ForEach(zoneLandList, new Action<ContractLand>((Item) =>
            {
                lock (zoneLandList)
                {
                    var noLandNumberLand = modifyLandList.Find(s => s.LandNumber == Item.LandNumber);
                    if (noLandNumberLand == null)
                    {
                        this.ReportWarn(string.Format("{0}台账地块编码为{1}的数据在Shape图斑中未找到匹配项", zone.FullName, Item.LandNumber));
                    }
                }
            }));

            if (indexPercent != 0.0)
            {
                indexZonePercent = indexPercent / (double)(importCount == 0 ? 1 : importCount);
            }
            else
            {
                indexZonePercent = 99 / (double)(importCount == 0 ? 1 : importCount);
            }
            dbContext.BeginTransaction();
            try
            {
                var dicTDYT = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT);
                var dicDLDJ = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
                var dicTDLYLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX);
                for (int i = 0; i < modifyLandList.Count; i++)
                {
                    var resLand = new ContractLand();
                    resLand = modifyContractLandinfo(modifyLandList[i], modifyData[i], allGetSelectColList, zoneNameInfo, i);//, dicTDYT, dicDLDJ, dicTDYT);
                    if (resLand == null)
                    {
                        //dbContext.RollbackTransaction(); return 0;
                        this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + modifyLandList[i].Name));
                        continue;
                    }
                    if (resLand.Shape != null)
                    {
                        resLand.Shape.SpatialReference = targetSpatialReference;
                    }

                    int resultInt = ModifyLand(resLand);
                    if (resultInt == -1) continue;
                    currentPercent = currentPercent + indexZonePercent;
                    successCount++;
                    this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + modifyLandList[i].OwnerName));
                }
                this.ReportInfomation(string.Format("{0}共导入{1}条信息", zoneNameInfo, successCount));
                dbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                this.ReportError("导入Shape数据时发生错误:" + ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportShape", ex.Message + ex.StackTrace);
            }
            return successCount;
        }

        /// <summary>
        /// 按照地块编码来导入修改
        /// </summary>
        private int ImportByOldLandCode(Zone zone, IList shapeDataList, List<string> allGetSelectColList,
           double currentPercent, double indexPercent, string zoneNameInfo, SpatialReference targetSpatialReference)
        {
            int importCount = 0;
            int successCount = 0;
            double indexZonePercent = 0;
            //获取地域下所有地
            List<ContractLand> zoneLandList = new List<ContractLand>();
            zoneLandList = GetCollection(zone.FullCode, eLevelOption.Self);
            if (zoneLandList == null) return 0;
            ContractLand modifyLand = null;
            List<ContractLand> modifyLandList = new List<ContractLand>();
            IList<object> modifyData = new List<object>();

            //循环每一个shape地域下地块,获取需要修改的地块集合
            foreach (var shpLandItem in shapeDataList)
            {
                modifyLand = new ContractLand();
                try
                {
                    var shplandnum = (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[4]) as string).TrimEnd('\0');
                    modifyLand = zoneLandList.Find(t => t.OldLandNumber == shplandnum);
                }
                catch
                {
                    this.ReportError("当前地块编码选择项下无匹配字段数据,请检查Shape数据中地块编码是否有误");
                    return 0;
                }
                if (modifyLand == null)
                {
                    this.ReportWarn(string.Format("Shape地块编码为{0}的数据在{1}台账中未找到匹配项", (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[4]) as string).TrimEnd('\0'), zone.FullName));
                    continue;
                }
                modifyLandList.Add(modifyLand);
                modifyData.Add(shpLandItem);
                importCount++;
            }
            if (modifyLandList.Count == 0)
            {
                this.ReportError(string.Format("{0}下无匹配数据", zone.FullName));
                return 0;
            }
            Parallel.ForEach(zoneLandList, new Action<ContractLand>((Item) =>
            {
                lock (zoneLandList)
                {
                    var noLandNumberLand = modifyLandList.Find(s => s.LandNumber == Item.LandNumber);
                    if (noLandNumberLand == null)
                    {
                        this.ReportWarn(string.Format("{0}台账地块编码为{1}的数据在Shape图斑中未找到匹配项", zone.FullName, Item.LandNumber));
                    }
                }
            }));

            if (indexPercent != 0.0)
            {
                indexZonePercent = indexPercent / (double)(importCount == 0 ? 1 : importCount);
            }
            else
            {
                indexZonePercent = 99 / (double)(importCount == 0 ? 1 : importCount);
            }
            dbContext.BeginTransaction();
            try
            {
                for (int i = 0; i < modifyLandList.Count; i++)
                {
                    var resLand = new ContractLand();
                    resLand = modifyContractOldLandinfo(modifyLandList[i], modifyData[i], allGetSelectColList, zoneNameInfo, i);
                    if (resLand == null)
                    {
                        //dbContext.RollbackTransaction(); return 0;
                        this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + modifyLandList[i].Name));
                        continue;
                    }
                    if (resLand.Shape != null)
                    {
                        resLand.Shape.SpatialReference = targetSpatialReference;
                    }

                    int resultInt = ModifyLand(resLand);
                    if (resultInt == -1) continue;
                    currentPercent = currentPercent + indexZonePercent;
                    successCount++;
                    this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + modifyLandList[i].OwnerName));
                }
                this.ReportInfomation(string.Format("{0}共导入{1}条信息", zoneNameInfo, successCount));
                dbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                this.ReportError("导入Shape数据时发生错误:" + ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportShape", ex.Message + ex.StackTrace);
            }
            return successCount;
        }

        /// <summary>
        /// 按照承包方编码来导入修改
        /// </summary>
        private int ImportByFamilyCode(Zone zone, IList shapeDataList, List<string> allGetSelectColList,
           double currentPercent, double indexPercent, string zoneNameInfo, SpatialReference targetSpatialReference)
        {
            int importCount = 0;
            int successCount = 0;
            double indexZonePercent = 0;
            //获取地域下所有人
            List<VirtualPerson> zonePersonList = new List<VirtualPerson>();
            zonePersonList = landVirtualPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
            DeleteLandByZoneCode(zone.FullCode);

            ContractLand addZoneLand;
            VirtualPerson addPerson = null;
            List<VirtualPerson> addPersonList = new List<VirtualPerson>();
            IList<object> addData = new List<object>();
            //获取对应的人
            foreach (var shpLandItem in shapeDataList)
            {
                try
                {
                    addPerson = zonePersonList.Find(t => t.Name == (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[0]) as string));
                }
                catch
                {
                    this.ReportError("当前承包方名称选择项下无匹配字段数据");
                    return 0;
                }
                if (addPerson == null)
                {
                    this.ReportWarn(string.Format("Shape承包方名称为{0}的数据在{1}台账中未找到匹配项", (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[0]) as string), zone.FullName));
                    continue;
                }
                addPersonList.Add(addPerson);
                addData.Add(shpLandItem);
                importCount++;
            }
            if (addPersonList.Count == 0)
            {
                this.ReportError(string.Format("{0}下无匹配数据", zone.FullName));
                return 0;
            }
            Parallel.ForEach(zonePersonList, new Action<VirtualPerson>((Item) =>
            {
                lock (zonePersonList)
                {
                    var noOwenerNameLand = addPersonList.Find(s => s.Name == Item.Name);
                    if (noOwenerNameLand == null)
                    {
                        this.ReportWarn(string.Format("{0}台账承包方名称为{1}的数据在Shape图斑中未找到匹配项", zone.FullName, Item.Name));
                    }
                }
            }));

            //如果分区进度为0，则表示为组级别，需要从新制定百分比
            if (indexPercent != 0.0)
            {
                indexZonePercent = indexPercent / (double)(importCount == 0 ? 1 : importCount);
            }
            else
            {
                indexZonePercent = 99 / (double)(importCount == 0 ? 1 : importCount);
            }
            dbContext.BeginTransaction();
            try
            {
                for (int i = 0; i < addPersonList.Count; i++)
                {
                    addZoneLand = new ContractLand();
                    var resLand = new ContractLand();

                    resLand = modifyContractLandinfo(addZoneLand, addData[i], allGetSelectColList, zoneNameInfo, i);
                    currentPercent = currentPercent + indexZonePercent;
                    if (resLand == null)
                    {
                        this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + addPersonList[i].Name));
                        continue;
                        //dbContext.RollbackTransaction();this.ReportProgress(100); return 0;
                    }
                    resLand.OwnerId = addPersonList[i].ID;
                    resLand.OwnerName = addPersonList[i].Name;
                    resLand.ZoneCode = addPersonList[i].ZoneCode;
                    resLand.ZoneName = addPersonList[i].Address;

                    // resLand.Shape = YuLinTu.Spatial.Geometry.FromInstance(resLand.Shape.Instance);
                    if (resLand.Shape != null)
                        resLand.Shape.SpatialReference = targetSpatialReference;
                    int resultInt = AddLand(resLand);
                    if (resultInt == -1) continue;
                    successCount++;
                    this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + addPersonList[i].Name));
                }
                this.ReportInfomation(string.Format("{0}共导入{1}条信息", zoneNameInfo, successCount));
                dbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                this.ReportError("导入Shape数据时发生错误:" + ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportShape", ex.Message + ex.StackTrace);
            }
            return successCount;
        }

        /// <summary>
        /// 按照承包方户号来导入修改
        /// </summary>
        private int ImportByFamilyNumberCode(Zone zone, IList shapeDataList, List<string> allGetSelectColList,
           double currentPercent, double indexPercent, string zoneNameInfo, SpatialReference targetSpatialReference)
        {
            int importCount = 0;
            int successCount = 0;
            double indexZonePercent = 0;
            //获取地域下所有人
            List<VirtualPerson> zonePersonList = new List<VirtualPerson>();
            zonePersonList = landVirtualPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);

            ContractLand addZoneLand;
            VirtualPerson addPerson = null;
            List<VirtualPerson> addPersonList = new List<VirtualPerson>();
            IList<object> addData = new List<object>();
            //获取对应的人
            foreach (var shpLandItem in shapeDataList)
            {
                try
                {
                    var fnum = ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[1]) as string;
                    addPerson = zonePersonList.Find(t => t.FamilyNumber == fnum);
                    if (fnum != null && addPerson == null)
                    {
                        if (fnum.Length == 18)
                        {
                            addPerson = zonePersonList.Find(t => (t.ZoneCode + t.FamilyNumber.PadLeft(4, '0')) == fnum);
                        }
                        if (fnum.Length == 4)
                        {
                            addPerson = zonePersonList.Find(t => t.FamilyNumber.PadLeft(4, '0') == fnum);
                        }
                    }
                }
                catch
                {
                    this.ReportError("当前承包方名称选择项下无匹配字段数据");
                    return 0;
                }
                if (addPerson == null)
                {
                    this.ReportWarn(string.Format("Shape承包方户号为{0}的数据在{1}台账中未找到匹配项", (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[1]) as string), zone.FullName));
                    continue;
                }
                addPersonList.Add(addPerson);
                addData.Add(shpLandItem);
                importCount++;
            }
            if (addPersonList.Count == 0)
            {
                this.ReportError(string.Format("{0}下无匹配数据", zone.FullName));
                return 0;
            }
            Parallel.ForEach(zonePersonList, new Action<VirtualPerson>((Item) =>
            {
                lock (zonePersonList)
                {
                    var noOwenerNameLand = addPersonList.Find(s => s.FamilyNumber == Item.FamilyNumber);
                    if (noOwenerNameLand == null)
                    {
                        this.ReportWarn(string.Format("{0}台账承包方户号为{1}的数据在Shape图斑中未找到匹配项", zone.FullName, Item.FamilyNumber));
                    }
                }
            }));

            //如果分区进度为0，则表示为组级别，需要从新制定百分比
            if (indexPercent != 0.0)
            {
                indexZonePercent = indexPercent / (double)(importCount == 0 ? 1 : importCount);
            }
            else
            {
                indexZonePercent = 99 / (double)(importCount == 0 ? 1 : importCount);
            }
            dbContext.BeginTransaction();
            try
            {
                DeleteLandByZoneCode(zone.FullCode);

                for (int i = 0; i < addPersonList.Count; i++)
                {
                    addZoneLand = new ContractLand();
                    var resLand = new ContractLand();
                    resLand = modifyContractLandinfo(addZoneLand, addData[i], allGetSelectColList, zoneNameInfo, i);

                    currentPercent = currentPercent + indexZonePercent;
                    if (resLand == null)
                    {
                        this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + addPersonList[i].Name));
                        continue;
                        //dbContext.RollbackTransaction();this.ReportProgress(100); return 0;
                    }
                    resLand.OwnerId = addPersonList[i].ID;
                    resLand.OwnerName = addPersonList[i].Name;
                    resLand.ZoneCode = addPersonList[i].ZoneCode;
                    resLand.ZoneName = addPersonList[i].Address;

                    // resLand.Shape = YuLinTu.Spatial.Geometry.FromInstance(resLand.Shape.Instance);
                    if (resLand.Shape != null)
                        resLand.Shape.SpatialReference = targetSpatialReference;
                    int resultInt = AddLand(resLand);
                    if (resultInt == -1) continue;
                    successCount++;
                    this.ReportProgress((int)currentPercent, $"{zoneNameInfo + addPersonList[i].Name}({successCount}/{addPersonList.Count}");
                }
                this.ReportInfomation(string.Format("{0}共导入{1}条信息", zoneNameInfo, successCount));
                dbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                this.ReportError("导入Shape数据时发生错误:" + ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportShape", ex.Message + ex.StackTrace);
            }
            return successCount;
        }

        #region 获取选择的匹配项

        private string GetproertValue(object shapeData, string Value)
        {
            object obj = ObjectExtensions.GetPropertyValue(shapeData, Value);
            if (obj == null)
                return "";
            return obj.ToString();
        }

        /// <summary>
        /// 获取选择的文件配置匹配修改土地对象
        /// </summary>
        private ContractLand modifyContractLandinfo(ContractLand targetLand, object shapeData, List<string> selectColNameList, string zoneNameInfo, int i)
        {
            bool falg = true;
            //循环每个配置属性，如果被下拉，对应地块就要修改属性
            PropertyInfo[] infoList = typeof(ImportAccountLandShapeSettingDefine).GetProperties();
            AgricultureLandExpand expand = targetLand.LandExpand;
            if (expand == null)
            {
                expand = new AgricultureLandExpand();
                expand.ID = targetLand.ID;
                expand.Name = targetLand.Name;
                expand.HouseHolderName = targetLand.Name;
            }
            targetLand.OwnerName = PropertString(shapeData, infoList, selectColNameList, "NameIndex");
            targetLand.Name = PropertString(shapeData, infoList, selectColNameList, "LandNameIndex");
            targetLand.LandNumber = PropertString(shapeData, infoList, selectColNameList, "CadastralNumberIndex");
            //if ((string)infoList[4].GetValue(ImportLandShapeInfoDefine, null) != "None")
            //{
            //    if (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[3]) != null)
            //    {
            //        targetLand.LandNumber = (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[3]).ToString()).TrimEnd('\0');
            //    }
            //}
            if ((string)infoList[5].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                if (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[5]) != null)
                {
                    targetLand.SurveyNumber = (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[5]).ToString()).TrimEnd('\0');
                }
            }
            else
            { //如果没有选择调查编码，赋值为地块编码，昌松说 2016-11-4
                targetLand.SurveyNumber = targetLand.LandNumber;
            }
            try
            {
                YuLinTu.Spatial.Geometry g = (ObjectExtensions.GetPropertyValue(shapeData, "Shape") as Geometry);
                targetLand.Shape = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
            }
            catch
            {
                this.ReportError("第" + i + "条Shape数据Shape无效");
                falg = false;
                //return null;
            }
            if ((string)infoList[6].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.ImageNumber = GetproertValue(shapeData, selectColNameList[6]);
            }
            if ((string)infoList[7].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                double getTableArea = 0.0;
                try
                {
                    var tableArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[7]);
                    if (tableArea == null)
                        getTableArea = 0.0;
                    else
                        getTableArea = Convert.ToDouble(tableArea);
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据台账面积'{0}'错误，无法转换",
                        ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[7]).ToString()));
                    falg = false;
                }
                targetLand.TableArea = getTableArea;
            }
            if ((string)infoList[8].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {   //实测面积处理
                double getActualArea = 0.0;
                try
                {
                    var actualArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[8]);
                    if (actualArea == null)
                    {
                        double area = targetLand.Shape.Area();
                        getActualArea = Math.Round(area * 0.0015, 4);
                    }
                    else
                    {
                        getActualArea = Convert.ToDouble(actualArea);
                    }
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据实测面积'{0}'错误，无法转换",
                        ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[8]).ToString()));
                    falg = false;
                }
                targetLand.ActualArea = getActualArea;
            }
            if ((string)infoList[9].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborEast = GetproertValue(shapeData, selectColNameList[9]);
            }
            if ((string)infoList[10].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborSouth = GetproertValue(shapeData, selectColNameList[10]);
            }
            if ((string)infoList[11].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborWest = GetproertValue(shapeData, selectColNameList[11]);
            }
            if ((string)infoList[12].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborNorth = GetproertValue(shapeData, selectColNameList[12]);
            }
            if ((string)infoList[13].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = (GetproertValue(shapeData, selectColNameList[13]));
                bool isNumeric = Regex.IsMatch(s, @"^\d+$");
                if (isNumeric)
                {
                    targetLand.Purpose = s;
                }
                else
                {
                    var dictTDYT = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.TDYT);
                    if (dictTDYT == null)
                    {
                        this.ReportError(string.Format("第" + i + "条Shape数据土地用途名称 {0} ，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                        falg = false;
                    }
                    else
                    {
                        targetLand.Purpose = dictTDYT.Code;
                    }
                }
            }
            if ((string)infoList[14].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var fieldvalue = GetproertValue(shapeData, selectColNameList[14]);

                var dictDLDJ = DictList.Find(c => c.GroupCode == DictionaryTypeInfo.DLDJ && (c.Name == fieldvalue || c.Code == fieldvalue));
                if (dictDLDJ == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据地力等级名称{0}，无法入库", fieldvalue.IsNullOrEmpty() ? "为空" : fieldvalue + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.LandLevel = dictDLDJ.Code;
                }
            }
            if ((string)infoList[15].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[15]);
                bool isNumeric = Regex.IsMatch(s, @"^\d+$");
                if (isNumeric)
                {
                    targetLand.LandCode = s;
                }
                else
                {
                    var dictTDLYLX = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.TDLYLX);
                    if (dictTDLYLX == null)
                    {
                        //targetLand.LandCode = "";
                        //targetLand.LandName = "";
                        this.ReportError(string.Format("第" + i + "条Shape数据土地利用类型名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                        falg = false;
                    }
                    else
                    {
                        targetLand.LandCode = dictTDLYLX.Code;
                        targetLand.LandName = dictTDLYLX.Name;
                    }
                }
            }
            if ((string)infoList[16].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var value = GetproertValue(shapeData, selectColNameList[16]);
                if (value.IsNullOrEmpty())
                {
                    //this.ReportError(string.Format("当前Shape数据是否基本农田名称'{0}'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[14]) as string));
                    //return null;
                    targetLand.IsFarmerLand = null;
                }
                else
                {
                    bool boolValue = value == "是" || value == "true" || value == "1" || value == "True" || value == "TRUE" ? true : false;
                    targetLand.IsFarmerLand = boolValue;
                }
            }
            if ((string)infoList[17].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.ReferPerson = GetproertValue(shapeData, selectColNameList[17]);
            }
            if ((string)infoList[18].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[18]);
                bool isNumeric = Regex.IsMatch(s, @"^\d+$");
                if (isNumeric)
                {
                    targetLand.LandCategory = s;
                }
                else
                {
                    if (s == "承包地")
                    {
                        targetLand.LandCategory = "10";
                    }
                    else
                    {
                        var dictDKLB = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.DKLB);
                        if (dictDKLB == null)
                        {
                            this.ReportError(string.Format("第" + i + "条Shape数据地块类别名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                            falg = false;
                        }
                        else
                        {
                            targetLand.LandCategory = dictDKLB.Code;
                        }
                    }
                }
            }
            if ((string)infoList[19].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                double getAwareArea = 0.0;
                try
                {
                    var awareArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[19]);
                    if (awareArea != null)
                        getAwareArea = (double)awareArea;
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据颁证面积'{0}'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[18]).ToString()));
                    falg = false;
                }
                targetLand.AwareArea = getAwareArea;
            }
            if ((string)infoList[20].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    var motorizeArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[20]);
                    if (motorizeArea != null)
                        targetLand.MotorizeLandArea = (double)motorizeArea;
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据机动地面积'0'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[19]).ToString()));
                    falg = false;
                }
            }
            if ((string)infoList[21].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[21]);
                var dictCBFS = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
                if (dictCBFS == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据承包方式名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.ConstructMode = dictCBFS.Code;
                }
            }
            if ((string)infoList[22].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.PlotNumber = GetproertValue(shapeData, selectColNameList[22]);
            }
            if ((string)infoList[23].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[23]);
                var dictZZLX = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.ZZLX);
                if (dictZZLX == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据种植类型名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.PlatType = dictZZLX.Code;
                }
            }
            if ((string)infoList[24].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[24]);
                var dictJYFS = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.JYFS);
                if (dictJYFS == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据经营方式名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.ManagementType = dictJYFS.Code;
                }
            }
            if ((string)infoList[25].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                //targetLand.PlantType = GetproertValue(shapeData, selectColNameList[23]);
                string s = GetproertValue(shapeData, selectColNameList[25]);
                var dictGBLX = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.GBZL);
                if (dictGBLX == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据耕保类型名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.PlantType = dictGBLX.Code;
                }
            }
            if ((string)infoList[26].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.FormerPerson = GetproertValue(shapeData, selectColNameList[26]);
            }
            if ((string)infoList[27].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.ZoneName = GetproertValue(shapeData, selectColNameList[27]);
            }
            if ((string)infoList[28].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[28]);
                bool boolValue = s == "是" || s == "true" || s == "True" || s == "TRUE" ? true : false;
                targetLand.IsTransfer = boolValue;
            }
            if ((string)infoList[29].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[29]);
                var dictLZLX = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.LZLX);
                if (dictLZLX == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据流转类型名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                { targetLand.TransferType = dictLZLX.Code; }
            }
            if ((string)infoList[30].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                Object obj = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[30]);
                if (obj == null)
                    targetLand.TransferTime = null;
                else
                    targetLand.TransferTime = obj.ToString();
            }
            if ((string)infoList[31].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    //object obj = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[29]);
                    //string s = obj.ToString();
                    targetLand.PertainToArea = double.Parse(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[31]).ToString());
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据流转面积'{0}'错误，无法入库", GetproertValue(shapeData, selectColNameList[30])));
                    falg = false;
                }
            }
            if ((string)infoList[32].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.SurveyPerson = GetproertValue(shapeData, selectColNameList[32]);
            }
            if ((string)infoList[33].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    var datetime = GetproertValue(shapeData, selectColNameList[33]);
                    if (datetime == "" || datetime == null)
                    {
                        expand.SurveyDate = null;
                    }
                    else
                    {
                        expand.SurveyDate = DateTime.Parse(datetime);
                    }
                }
                catch (Exception ex)
                {
                    expand.SurveyDate = null;
                    this.ReportInfomation(string.Format("第" + i + "条Shape数据调查日期'{0}'错误" + ex.Message, GetproertValue(shapeData, selectColNameList[32])));
                }
            }
            if ((string)infoList[34].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.SurveyChronicle = GetproertValue(shapeData, selectColNameList[34]);
            }
            if ((string)infoList[35].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.CheckPerson = GetproertValue(shapeData, selectColNameList[35]);
            }
            if ((string)infoList[36].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    var datetime = GetproertValue(shapeData, selectColNameList[36]);
                    if (datetime == "" || datetime == null)
                    {
                        expand.CheckDate = null;
                    }
                    else
                    {
                        expand.CheckDate = DateTime.Parse(datetime);
                    }
                }
                catch (Exception ex)
                {
                    expand.CheckDate = null;
                    this.ReportInfomation(string.Format("第" + i + "条Shape数据审核日期'{0}'错误" + ex.Message, GetproertValue(shapeData, selectColNameList[35])));
                }
            }
            if ((string)infoList[37].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.CheckOpinion = GetproertValue(shapeData, selectColNameList[37]);
            }
            if ((string)infoList[38].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.Comment = GetproertValue(shapeData, selectColNameList[38]);
            }
            if ((string)infoList[39].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var value = GetproertValue(shapeData, selectColNameList[39]);
                targetLand.AliasNameA = value == null ? "" : value.Trim();
            }
            targetLand.ExpandInfo = ToolSerialize.SerializeXmlString<AgricultureLandExpand>(expand);
            if (falg)
                return targetLand;
            else
                return null;
        }

        private ContractLand modifyContractOldLandinfo(ContractLand targetLand, object shapeData, List<string> selectColNameList, string zoneNameInfo, int i)
        {
            bool falg = true;
            //循环每个配置属性，如果被下拉，对应地块就要修改属性
            PropertyInfo[] infoList = typeof(ImportAccountLandShapeSettingDefine).GetProperties();
            AgricultureLandExpand expand = targetLand.LandExpand;
            if (expand == null)
            {
                expand = new AgricultureLandExpand();
                expand.ID = targetLand.ID;
                expand.Name = targetLand.Name;
                expand.HouseHolderName = targetLand.Name;
            }

            if ((string)infoList[0].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.OwnerName = GetproertValue(shapeData, selectColNameList[0]);
            }
            if ((string)infoList[2].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.Name = GetproertValue(shapeData, selectColNameList[2]);
            }
            if ((string)infoList[3].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                if (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[3]) != null)
                {
                    targetLand.LandNumber = targetLand.LandNumber;
                }
            }
            if ((string)infoList[4].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                if (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[4]) != null)
                {
                    targetLand.SurveyNumber = (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[4]).ToString()).TrimEnd('\0');
                }
            }
            else
            { //如果没有选择调查编码，赋值为地块编码，昌松说 2016-11-4
                targetLand.SurveyNumber = targetLand.LandNumber;
            }
            try
            {
                YuLinTu.Spatial.Geometry g = (ObjectExtensions.GetPropertyValue(shapeData, "Shape") as Geometry);
                targetLand.Shape = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
            }
            catch
            {
                this.ReportError("第" + i + "条Shape数据Shape无效");
                falg = false;
                //return null;
            }
            if ((string)infoList[5].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.ImageNumber = GetproertValue(shapeData, selectColNameList[5]);
            }
            if ((string)infoList[6].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                double getTableArea = 0.0;
                try
                {
                    var tableArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[6]);
                    if (tableArea == null)
                        getTableArea = 0.0;
                    else
                        getTableArea = (double)tableArea;
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据台账面积'{0}'错误，无法转换", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[6]).ToString()));
                    falg = false;
                }
                targetLand.TableArea = getTableArea;
            }
            if ((string)infoList[7].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {   //实测面积处理
                double getActualArea = 0.0;
                try
                {
                    var actualArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[7]);
                    if (actualArea == null)
                    {
                        double area = targetLand.Shape.Area();
                        getActualArea = Math.Round(area * 0.0015, 4);
                    }
                    else
                    {
                        getActualArea = (double)actualArea;
                    }
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据实测面积'{0}'错误，无法转换", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[7]).ToString()));
                    falg = false;
                }
                targetLand.ActualArea = getActualArea;
            }
            if ((string)infoList[8].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborEast = GetproertValue(shapeData, selectColNameList[8]);
            }
            if ((string)infoList[9].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborSouth = GetproertValue(shapeData, selectColNameList[9]);
            }
            if ((string)infoList[10].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborWest = GetproertValue(shapeData, selectColNameList[10]);
            }
            if ((string)infoList[11].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborNorth = GetproertValue(shapeData, selectColNameList[11]);
            }
            if ((string)infoList[12].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = (GetproertValue(shapeData, selectColNameList[12]));
                bool isNumeric = Regex.IsMatch(s, @"^\d+$");
                if (isNumeric)
                {
                    targetLand.Purpose = s;
                }
                else
                {
                    var dictTDYT = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.TDYT);
                    if (dictTDYT == null)
                    {
                        this.ReportError(string.Format("第" + i + "条Shape数据土地用途名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                        falg = false;
                    }
                    else
                    {
                        targetLand.Purpose = dictTDYT.Code;
                    }
                }
            }
            if ((string)infoList[13].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var fieldvalue = GetproertValue(shapeData, selectColNameList[13]);

                var dictDLDJ = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == fieldvalue && c.GroupCode == DictionaryTypeInfo.DLDJ);
                if (dictDLDJ == null)
                {
                    //targetLand.LandLevel = "";
                    this.ReportError(string.Format("第" + i + "条Shape数据地力等级名称{0}，无法入库", fieldvalue.IsNullOrEmpty() ? "为空" : fieldvalue + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.LandLevel = dictDLDJ.Code;
                }
                //}
            }
            if ((string)infoList[14].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[14]);
                bool isNumeric = Regex.IsMatch(s, @"^\d+$");
                if (isNumeric)
                {
                    targetLand.LandCode = s;
                }
                else
                {
                    var dictTDLYLX = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.TDLYLX);
                    if (dictTDLYLX == null)
                    {
                        //targetLand.LandCode = "";
                        //targetLand.LandName = "";
                        this.ReportError(string.Format("第" + i + "条Shape数据土地利用类型名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                        falg = false;
                    }
                    else
                    {
                        targetLand.LandCode = dictTDLYLX.Code;
                        targetLand.LandName = dictTDLYLX.Name;
                    }
                }
            }
            if ((string)infoList[15].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var value = GetproertValue(shapeData, selectColNameList[15]);
                if (value.IsNullOrEmpty())
                {
                    //this.ReportError(string.Format("当前Shape数据是否基本农田名称'{0}'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[14]) as string));
                    //return null;
                    targetLand.IsFarmerLand = null;
                }
                else
                {
                    bool boolValue = value == "是" || value == "true" || value == "1" || value == "True" || value == "TRUE" ? true : false;
                    targetLand.IsFarmerLand = boolValue;
                }
            }
            if ((string)infoList[16].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.ReferPerson = GetproertValue(shapeData, selectColNameList[16]);
            }
            if ((string)infoList[17].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[17]);
                bool isNumeric = Regex.IsMatch(s, @"^\d+$");
                if (isNumeric)
                {
                    targetLand.LandCategory = s;
                }
                else
                {
                    if (s == "承包地")
                    {
                        targetLand.LandCategory = "10";
                    }
                    else
                    {
                        var dictDKLB = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.DKLB);
                        if (dictDKLB == null)
                        {
                            this.ReportError(string.Format("第" + i + "条Shape数据地块类别名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                            falg = false;
                        }
                        else
                        {
                            targetLand.LandCategory = dictDKLB.Code;
                        }
                    }
                }
            }
            if ((string)infoList[18].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                double getAwareArea = 0.0;
                try
                {
                    var awareArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[18]);
                    if (awareArea != null)
                        getAwareArea = (double)awareArea;
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据颁证面积'{0}'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[18]).ToString()));
                    falg = false;
                }
                targetLand.AwareArea = getAwareArea;
            }
            if ((string)infoList[19].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    var motorizeArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[19]);
                    if (motorizeArea != null)
                        targetLand.MotorizeLandArea = (double)motorizeArea;
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据机动地面积'0'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[19]).ToString()));
                    falg = false;
                }
            }
            if ((string)infoList[20].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[20]);
                var dictCBFS = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
                if (dictCBFS == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据承包方式名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.ConstructMode = dictCBFS.Code;
                }
            }
            if ((string)infoList[21].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.PlotNumber = GetproertValue(shapeData, selectColNameList[21]);
            }
            if ((string)infoList[22].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[22]);
                var dictZZLX = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.ZZLX);
                if (dictZZLX == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据种植类型名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.PlatType = dictZZLX.Code;
                }
            }
            if ((string)infoList[23].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[23]);
                var dictJYFS = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.JYFS);
                if (dictJYFS == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据经营方式名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.ManagementType = dictJYFS.Code;
                }
            }
            if ((string)infoList[24].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                //targetLand.PlantType = GetproertValue(shapeData, selectColNameList[23]);
                string s = GetproertValue(shapeData, selectColNameList[24]);
                var dictGBLX = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.GBZL);
                if (dictGBLX == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据耕保类型名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.PlantType = dictGBLX.Code;
                }
            }
            if ((string)infoList[25].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.FormerPerson = GetproertValue(shapeData, selectColNameList[25]);
            }
            if ((string)infoList[26].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.ZoneName = GetproertValue(shapeData, selectColNameList[26]);
            }
            if ((string)infoList[27].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[27]);
                bool boolValue = s == "是" || s == "true" || s == "True" || s == "TRUE" ? true : false;
                if (s.IsNullOrEmpty())
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据是否流转名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                {
                    targetLand.IsTransfer = boolValue;
                }
            }
            if ((string)infoList[28].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string s = GetproertValue(shapeData, selectColNameList[28]);
                var dictLZLX = DictList.Find(c => (c.Name.IsNullOrEmpty() ? "" : c.Name.ToString().Trim()) == s && c.GroupCode == DictionaryTypeInfo.LZLX);
                if (dictLZLX == null)
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据流转类型名称{0}，无法入库", s.IsNullOrEmpty() ? "为空" : s + "错误"));
                    falg = false;
                }
                else
                { targetLand.TransferType = dictLZLX.Code; }
            }
            if ((string)infoList[29].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                Object obj = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[29]);
                if (obj == null)
                    targetLand.TransferTime = null;
                else
                    targetLand.TransferTime = obj.ToString();
            }
            if ((string)infoList[30].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    //object obj = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[29]);
                    //string s = obj.ToString();
                    targetLand.PertainToArea = double.Parse(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[30]).ToString());
                }
                catch
                {
                    this.ReportError(string.Format("第" + i + "条Shape数据流转面积'{0}'错误，无法入库", GetproertValue(shapeData, selectColNameList[30])));
                    falg = false;
                }
            }
            if ((string)infoList[31].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.SurveyPerson = GetproertValue(shapeData, selectColNameList[31]);
            }
            if ((string)infoList[32].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    var datetime = GetproertValue(shapeData, selectColNameList[32]);
                    if (datetime == "" || datetime == null)
                    {
                        expand.SurveyDate = null;
                    }
                    else
                    {
                        expand.SurveyDate = DateTime.Parse(datetime);
                    }
                }
                catch (Exception ex)
                {
                    expand.SurveyDate = null;
                    this.ReportInfomation(string.Format("第" + i + "条Shape数据调查日期'{0}'错误" + ex.Message, GetproertValue(shapeData, selectColNameList[32])));
                }
            }
            if ((string)infoList[33].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.SurveyChronicle = GetproertValue(shapeData, selectColNameList[33]);
            }
            if ((string)infoList[34].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.CheckPerson = GetproertValue(shapeData, selectColNameList[34]);
            }
            if ((string)infoList[35].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    var datetime = GetproertValue(shapeData, selectColNameList[35]);
                    if (datetime == "" || datetime == null)
                    {
                        expand.CheckDate = null;
                    }
                    else
                    {
                        expand.CheckDate = DateTime.Parse(datetime);
                    }
                }
                catch (Exception ex)
                {
                    expand.CheckDate = null;
                    this.ReportInfomation(string.Format("第" + i + "条Shape数据审核日期'{0}'错误" + ex.Message, GetproertValue(shapeData, selectColNameList[35])));
                }
            }
            if ((string)infoList[36].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.CheckOpinion = GetproertValue(shapeData, selectColNameList[36]);
            }
            if ((string)infoList[37].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.Comment = GetproertValue(shapeData, selectColNameList[37]);
            }
            if ((string)infoList[38].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var value = GetproertValue(shapeData, selectColNameList[38]);
                targetLand.AliasNameA = value == null ? "" : value.Trim();
            }
            targetLand.ExpandInfo = ToolSerialize.SerializeXmlString<AgricultureLandExpand>(expand);
            if (falg)
                return targetLand;
            else
                return null;
        }

        private string PropertString(object shapeData, PropertyInfo[] infoList, List<string> selectColNameList, string name)
        {
            var indx = FindIndex(infoList, name);
            if (indx != -1)
            {
                return GetproertValue(shapeData, selectColNameList[indx]);
            }
            return null;
        }

        private int FindIndex(PropertyInfo[] infoList, string name)
        {
            var info = infoList.FirstOrDefault(f => f.Name == name);
            if (info != null)
            {
                var r = info.GetValue(ImportLandShapeInfoDefine, null).ToString();
                if (r != "None")
                {
                    return infoList.FindIndex(t => t.Name == name);
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取选择的土地地块弹出框导入配置下拉列表所有字段名称，与配置实体保持对应
        /// </summary>
        private List<string> getSelectColList()
        {
            PropertyInfo[] infoList = typeof(ImportAccountLandShapeSettingDefine).GetProperties();
            //获取下拉框中对应承包方 地块编码的下拉字段名称

            string contractorName = infoList[0].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string vpfamilynumber = infoList[1].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landName = infoList[2].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string cadastralNumber = infoList[3].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string surveyNumber = infoList[4].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string ImageNumber = infoList[5].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string tableArea = infoList[6].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string actualArea = infoList[7].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string east = infoList[8].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string sourth = infoList[9].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string west = infoList[10].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string north = infoList[11].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landPurpose = infoList[12].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landLevel = infoList[13].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landType = infoList[14].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string isFarmerLand = infoList[15].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string referPerson = infoList[16].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string arableType = infoList[17].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string awareArea = infoList[18].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string motorizeArea = infoList[19].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string constructMode = infoList[20].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string plotNumber = infoList[21].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string platType = infoList[22].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string managementType = infoList[23].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landPlant = infoList[24].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string sourceName = infoList[25].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string landLocation = infoList[26].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string isTranster = infoList[27].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string transterMode = infoList[28].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string transterTerm = infoList[29].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string transterArea = infoList[30].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string landSurveyPersonIndex = infoList[31].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landSurveyDateIndex = infoList[32].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landSurveyChronicleIndex = infoList[33].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landCheckPersonIndex = infoList[34].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landCheckDateIndex = infoList[35].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string landCheckOpinionIndex = infoList[36].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string comment = infoList[37].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string isRegisterIndex = infoList[38].GetValue(ImportLandShapeInfoDefine, null).ToString();

            //已经下拉选取的字段名称集合-序号与配置实体顺序一致
            List<string> userSelectColNameList = new List<string>();
            userSelectColNameList.Add(contractorName);
            userSelectColNameList.Add(vpfamilynumber);
            userSelectColNameList.Add(landName);
            userSelectColNameList.Add(cadastralNumber);
            userSelectColNameList.Add(surveyNumber);

            userSelectColNameList.Add(ImageNumber);
            userSelectColNameList.Add(tableArea);
            userSelectColNameList.Add(actualArea);
            userSelectColNameList.Add(east);
            userSelectColNameList.Add(sourth);

            userSelectColNameList.Add(west);
            userSelectColNameList.Add(north);
            userSelectColNameList.Add(landPurpose);
            userSelectColNameList.Add(landLevel);
            userSelectColNameList.Add(landType);

            userSelectColNameList.Add(isFarmerLand);
            userSelectColNameList.Add(referPerson);
            userSelectColNameList.Add(arableType);
            userSelectColNameList.Add(awareArea);
            userSelectColNameList.Add(motorizeArea);

            userSelectColNameList.Add(constructMode);
            userSelectColNameList.Add(plotNumber);
            userSelectColNameList.Add(platType);
            userSelectColNameList.Add(managementType);
            userSelectColNameList.Add(landPlant);

            userSelectColNameList.Add(sourceName);
            userSelectColNameList.Add(landLocation);
            userSelectColNameList.Add(isTranster);
            userSelectColNameList.Add(transterMode);
            userSelectColNameList.Add(transterTerm);

            userSelectColNameList.Add(transterArea);
            userSelectColNameList.Add(landSurveyPersonIndex);
            userSelectColNameList.Add(landSurveyDateIndex);
            userSelectColNameList.Add(landSurveyChronicleIndex);
            userSelectColNameList.Add(landCheckPersonIndex);

            userSelectColNameList.Add(landCheckDateIndex);
            userSelectColNameList.Add(landCheckOpinionIndex);
            userSelectColNameList.Add(comment);
            userSelectColNameList.Add(isRegisterIndex);

            return userSelectColNameList;
        }

        /// <summary>
        /// 获取选择的文件配置匹配项-返回已经下拉选中的配置文件列表
        /// </summary>
        private List<string> GetSelectFromSetting()
        {
            if (ImportLandShapeInfoDefine == null)
                return new List<string>();
            List<string> list = new List<string>();
            PropertyInfo[] infoList = typeof(ImportAccountLandShapeSettingDefine).GetProperties();
            for (int i = 0; i < infoList.Length; i++)
            {
                PropertyInfo info = infoList[i];
                int propertyValue = (int)info.GetValue(ImportLandShapeInfoDefine, null);
                if (propertyValue != -1)
                {
                    list.Add(info.Name);
                }
            }
            return list;
        }

        /// <summary>
        /// 坐标系发生改变时获取坐标单位，具体实现
        /// </summary>
        private void RefreshMapControlSpatialUnit()
        {
            try
            {
                if (shpRef.IsPROJCS())
                {
                    var projectionInfo = YuLinTu.Spatial.SpatialReferences.CreateProjectionInfo(shpRef);

                    if (projectionInfo == null) return;
                    switch (projectionInfo.Unit.Name)
                    {
                        case "Kilometer":
                            projectionUnit = 1500;
                            break;

                        case "Meter":
                            projectionUnit = 0.0015;
                            break;

                        case "Decimeter":
                            projectionUnit = 0.000015;
                            break;

                        case "Centimeter":
                            projectionUnit = 1.5 * Math.Pow(Math.E, -7);
                            break;

                        case "Millimeter":
                            projectionUnit = 1.5 * Math.Pow(Math.E, -9);
                            break;

                        case "Mile":
                            projectionUnit = 3884.9821655;
                            break;

                        case "Foot":
                            projectionUnit = 0.0001394;
                            break;

                        case "Yard":
                            projectionUnit = 0.0012542;
                            break;

                        case "Inch":
                            projectionUnit = 9.6774 * Math.Pow(Math.E, -7);
                            break;

                        default:
                            projectionUnit = 0.0015;
                            break;
                    }
                }
                else if (shpRef.IsGEOGCS() || !shpRef.IsValid())
                {
                    projectionUnit = 0.0015;
                }
            }
            catch
            {
                projectionUnit = 0.0015;
            }
        }

        #endregion 获取选择的匹配项

        #endregion 导入数据

        #region 导出数据

        /// <summary>
        /// 导出数据到Excel表-承包台账4个报表导出
        /// </summary>
        public void ExportDataExcel(Zone zone, string fileName, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = GetByZone(zone.FullCode);

                //判断是否显示集体信息-勾选常规设置后就设置为不显示
                if (SettingDefine.DisplayCollectUsingCBdata)
                {
                    //List<VirtualPerson> vpList = vps.FindAll(fm => (fm.Name.IndexOf("机动地") >= 0 || fm.Name.IndexOf("集体") >= 0));
                    //foreach (VirtualPerson vpn in vpList)
                    //{
                    //    vps.Remove(vpn);
                    //}
                    //vpList.Clear();

                    vps.RemoveAll(c => c.FamilyExpand.ContractorType != eContractorType.Farmer);
                }

                string fileType = "承包地块调查表.xls";
                switch (TableType)
                {
                    case 2:
                        fileType = "土地承包经营权公示表.xls";
                        break;

                    case 3:
                        fileType = "土地承包经营权签字表.xls";
                        break;

                    case 4:
                        fileType = "土地承包经营权村组公示表.xls";
                        break;

                    case 5:
                        fileType = "土地承包经营权单户确认表.xls";
                        break;

                    default:
                        break;
                }

                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSurveyExceltemp);
                string zoneName = GetUinitName(zone);
                List<ContractLand> landArrays = GetLandCollection(zone.Code);
                var dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                    return;
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var listConcords = concordStation.GetContractsByZoneCode(zone.FullCode);
                var listBooks = bookStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                string filePath = string.Empty;
                ExportContractorSurveyExcel export = new ExportContractorSurveyExcel();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractorSurveyExcel)
                    {
                        export = (ExportContractorSurveyExcel)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.SaveFilePath = fileName + @"\" + excelName + fileType;
                export.CurrentZone = zone;
                export.Familys = vps;
                export.ExcelName = excelName;
                export.UnitName = zoneName;
                export.TableType = TableType;
                export.DictionList = DictList;
                export.LandArrays = landArrays;
                export.Percent = averagePercent;
                export.CurrentPercent = currentPercent;
                export.ConcordCollection = listConcords;
                export.BookColletion = listBooks;
                export.PostProgressEvent += export_PostProgressEvent;
                export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                bool result = export.BeginExcel(null, null, zone.FullCode.ToString(), tempPath);
                filePath = export.SaveFilePath;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出数据到Excel表-承包台账单户确认报表单个导出
        /// </summary>
        public void ExportSingleFamilyConfirmExcel(Zone zone, List<VirtualPerson> selectVirtualPerson, string fileName)
        {
            try
            {
                if (selectVirtualPerson == null)
                {
                    this.ReportError("未选择导出数据的承包方!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = new List<VirtualPerson>();
                this.ReportProgress(0, "开始");
                string reInfo = string.Format("从{0}下成功导出{1}条承包方数据!", excelName, selectVirtualPerson.Count.ToString());
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSurveyExceltemp);
                string zoneName = GetUinitName(zone);
                if (SystemSet.CountryTableHead)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    zoneName = zoneStation.GetVillageName(zone);
                }
                int percent = (100 / selectVirtualPerson.Count);
                this.ReportProgress(10, "正在获取数据");
                int percentIndex = 1;
                foreach (var item in selectVirtualPerson)
                {
                    List<ContractLand> landArrays = GetPersonCollection(item.ID);
                    landArrays.LandNumberFormat(SystemSet);
                    int familyNumber = 0;
                    Int32.TryParse(item.FamilyNumber, out familyNumber);
                    this.ReportProgress(10 + percent * percentIndex, item.Name);
                    ++percentIndex;
                    using (ExportContractorSurveyExcel export = new ExportContractorSurveyExcel())
                    {
                        export.SaveFilePath = fileName + @"\" + familyNumber + "-" + item.Name + "-" + "单户确认表" + ".xls";
                        export.CurrentZone = zone;
                        export.Familys = vps;
                        export.UnitName = zoneName;
                        export.TableType = TableType;
                        export.DictionList = DictList;
                        export.LandArrays = landArrays;
                        export.Contractor = item;
                        //export.PostProgressEvent +=export_PostProgressEvent;
                        export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                        bool result = export.BeginExcel(null, null, zone.FullCode.ToString(), tempPath);
                        //filePath = export.SaveFilePath;
                        //if (result && selectVirtualPerson.Count == 1)
                        //{
                        //    System.Diagnostics.Process.Start(filePath);
                        //}
                    }
                }
                this.ReportProgress(100, "完成");
                this.ReportInfomation(reInfo);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出数据到Excel表-承包台账单户确认报表批量导出
        /// </summary>
        public void ExportSingleFamilyConfirmExcel(Zone zone, VirtualPerson personItem, string fileName)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = GetByZone(zone.FullCode);

                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSurveyExceltemp);
                string zoneName = GetUinitName(zone);
                if (SystemSet.CountryTableHead)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    zoneName = zoneStation.GetVillageName(zone);
                }
                string filePath = string.Empty;

                List<ContractLand> landArrays = GetPersonCollection(personItem.ID);
                landArrays.LandNumberFormat(SystemSet);
                int familyNumber = 0;
                Int32.TryParse(personItem.FamilyNumber, out familyNumber);
                using (ExportContractorSurveyExcel export = new ExportContractorSurveyExcel())
                {
                    export.SaveFilePath = fileName + @"\" + familyNumber + "-" + personItem.Name + "-" + "单户确认表" + ".xls";
                    export.CurrentZone = zone;
                    export.Familys = new List<VirtualPerson>();
                    export.UnitName = zoneName;
                    export.TableType = TableType;
                    export.DictionList = DictList;
                    export.LandArrays = landArrays;
                    export.Contractor = personItem;
                    export.NotShow = true;
                    //export.PostProgressEvent +=export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginExcel(null, null, zone.FullCode.ToString(), tempPath);
                    filePath = export.SaveFilePath;
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出数据到Excel表-承包台账单户调查报表单个导出
        /// </summary>
        public void ExportSingleFamilySurveyExcel(Zone zone, List<VirtualPerson> selectVirtualPerson, string fileName)
        {
            try
            {
                if (selectVirtualPerson == null)
                {
                    this.ReportError("未选择导出数据的承包方!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                var tablePersonStation = dbContext.CreateVirtualPersonStation<TableVirtualPerson>();
                List<VirtualPerson> tableVps = tablePersonStation.GetByZoneCode(zone.FullCode);
                this.ReportProgress(0, "开始");
                string reInfo = string.Format("从{0}下成功导出{1}条承包方数据!", excelName, selectVirtualPerson.Count.ToString());
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSingleSurveyExceltemp);
                string zoneName = GetUinitName(zone);
                int percent = (100 / selectVirtualPerson.Count);
                this.ReportProgress(10, "正在获取数据");
                int percentIndex = 1;
                SecondTableLandBusiness secondlandbus = new SecondTableLandBusiness(dbContext);
                foreach (var item in selectVirtualPerson)
                {
                    List<ContractLand> landArrays = GetPersonCollection(item.ID);
                    List<SecondTableLand> tableLandArrays = secondlandbus.GetCollection(item.ID);

                    string filePath = string.Empty;
                    int familyNumber = 0;
                    Int32.TryParse(item.FamilyNumber, out familyNumber);
                    this.ReportProgress(10 + percent * percentIndex, item.Name);
                    ++percentIndex;
                    using (ExportContractorLandSingleSurveyTable export = new ExportContractorLandSingleSurveyTable())
                    {
                        export.SaveFilePath = fileName + @"\" + familyNumber + "-" + item.Name + "-" + "单户调查表" + ".xls";
                        export.CurrentZone = zone;
                        export.TableFamilys = tableVps;
                        export.ShowValue = false;
                        if (selectVirtualPerson.Count == 1)
                        {
                            export.ShowValue = true;
                        }

                        export.UnitName = zoneName;
                        export.TableLandArrays = tableLandArrays;
                        export.DictionList = DictList;
                        export.LandArrays = landArrays;
                        export.Contractor = item;
                        //export.PostProgressEvent +=export_PostProgressEvent;
                        export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                        bool result = export.BeginToVirtualPerson(item, tempPath);
                        filePath = export.SaveFilePath;
                    }
                }
                this.ReportProgress(100, "完成");
                this.ReportInfomation(reInfo);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出数据到Excel表-承包台账单户调查报表批量导出
        /// </summary>
        public void ExportSingleFamilySurveyExcel(Zone zone, VirtualPerson personItem, string fileName)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);

                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSingleSurveyExceltemp);
                string zoneName = GetUinitName(zone);
                if (SystemSet.CountryTableHead)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    zoneName = zoneStation.GetVillageName(zone);
                }
                string filePath = string.Empty;
                List<SecondTableLand> tableLandArrays = new List<SecondTableLand>();
                SecondTableLandBusiness secondlandbus = new SecondTableLandBusiness(dbContext);
                var tablePersonStation = dbContext.CreateVirtualPersonStation<TableVirtualPerson>();
                List<VirtualPerson> tableVps = tablePersonStation.GetByZoneCode(zone.FullCode);
                List<ContractLand> landArrays = GetPersonCollection(personItem.ID);
                landArrays.LandNumberFormat(SystemSet);
                int familyNumber = 0;
                Int32.TryParse(personItem.FamilyNumber, out familyNumber);
                tableLandArrays = secondlandbus.GetCollection(personItem.ID);

                using (ExportContractorLandSingleSurveyTable export = new ExportContractorLandSingleSurveyTable())
                {
                    export.SaveFilePath = fileName + @"\" + familyNumber + "-" + personItem.Name + "-" + "单户调查表" + ".xls";
                    export.CurrentZone = zone;
                    export.ShowValue = false;
                    export.TableFamilys = tableVps;
                    export.UnitName = zoneName;
                    export.TableLandArrays = tableLandArrays;
                    export.DictionList = DictList;
                    export.LandArrays = landArrays;
                    export.Contractor = personItem;
                    //export.PostProgressEvent +=export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginToVirtualPerson(personItem, tempPath);
                    filePath = export.SaveFilePath;
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出数据到Word
        /// </summary>
        public void ExportDataWord(Zone zone, string filePath)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string markDesc = GetMarkDesc(zone);
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    this.ReportError(string.Format("在{0}下未获取到承包方数据!", markDesc));
                    return;
                }
                this.ReportProgress(1, null);
                int index = 1;
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyWord);
                string zoneName = GetUinitName(zone);
                this.ReportProgress(10, null);
                double percent = 90 / (double)vps.Count;
                foreach (VirtualPerson family in vps)
                {
                    //是否显示集体户信息
                    //if (FamilyOtherSet.ShowFamilyInfomation && (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0))
                    //{
                    //    continue;
                    //}
                    index++;
                    this.ReportProgress((int)(10 + percent * index), family.Name);
                    ExportContractorTable export = new ExportContractorTable();

                    #region 通过反射等机制定制化具体的业务处理类

                    var temp = WorksheetConfigHelper.GetInstance(export);
                    if (temp != null && temp.TemplatePath != null)
                    {
                        if (temp is ExportContractorTable)
                        {
                            export = (ExportContractorTable)temp;
                        }
                        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }

                    #endregion 通过反射等机制定制化具体的业务处理类

                    export.DateValue = null;
                    export.OpenTemplate(tempPath);
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    export.SaveAs(family, filePath + @"\" + familyNuber + "-" + family.Name + "-农村土地承包经营权承包方调查表.doc");
                }
                this.ReportProgress(100, "完成");
                this.ReportInfomation(string.Format("从{0}下成功导出{1}条承包方数据", markDesc, index - 1));
                vps = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出单个数据到Word
        /// </summary>
        public void ExportDataWord(Zone zone, VirtualPerson vp)
        {
            try
            {
                if (vp == null)
                {
                    return;
                }
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyWord);
                string zoneName = GetUinitName(zone);
                ExportContractorTable export = new ExportContractorTable();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractorTable)
                    {
                        export = (ExportContractorTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.DateValue = null;
                export.OpenTemplate(tempPath);
                string previewPath = SystemSet.DefaultPath + @"\" + vp.Name;
                export.PrintPreview(vp, previewPath);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        #endregion 导出数据

        #region 导出调查报表

        /// <summary>
        /// 导出发包方调查表(Word)
        /// </summary>
        public void ExportSenderWord(Zone zone, CollectivityTissue tissue, string fileName)
        {
            isErrorRecord = false;
            try
            {
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.SenderSurveyWord); //模板文件
                ExportSenderWord senderTable = new ExportSenderWord();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(senderTable);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportSenderWord)
                    {
                        senderTable = (ExportSenderWord)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                senderTable.OpenTemplate(tempPath);
                senderTable.SaveAs(tissue, fileName + @"\" + tissue.Name + "(" + tissue.Code + ")" + TemplateFile.SenderSurveyWord);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderWord(导出发包方)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出承包方调查表(Word)
        /// </summary>
        public bool ExportVPWord(Zone zone, VirtualPerson vp, string filename = "", string markDesc = "")
        {
            bool flag = false;
            try
            {
                if (vp == null)
                {
                    return flag;
                }
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractSurveyWord);  //模板文件
                ExportContractorWordTable export = new ExportContractorWordTable();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractorWordTable)
                    {
                        export = (ExportContractorWordTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.SystemSet.KeepRepeatFlag = SystemSet.KeepRepeatFlag;
                export.Contractor = vp;
                export.MarkDesc = markDesc;
                export.Tissue = GetTissue(zone.ID);  //发包方
                export.OpenTemplate(tempPath);
                string familyNuber = ToolString.ExceptSpaceString(vp.FamilyNumber);
                string filePath = filename + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.ContractSurveyWord;
                export.SaveAs(vp, filePath);
                flag = true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word表)", ex.Message + ex.StackTrace);
            }
            return flag;
        }

        /// <summary>
        /// 导出公示结果归户表(Word)
        /// </summary>
        public bool ExportPublishWord(Zone zone, VirtualPerson vp, List<ContractLand> landItems, string filename = "", bool exportdelempty = false)
        {
            bool flag = false;
            try
            {
                if (vp == null)
                {
                    return flag;
                }
                var zonelist = GetParentZone(zone);
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.PublicityWord);
                var concordStation = dbContext.CreateConcordStation();
                var concords = concordStation.GetAllConcordByFamilyID(vp.ID);

                ExportPublicityWordTable export = new ExportPublicityWordTable();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportPublicityWordTable)
                    {
                        export = (ExportPublicityWordTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.CurrentZone = zone;
                export.Contractor = vp;
                export.DictList = DictList;
                export.ZoneList = zonelist;
                export.LandCollection = landItems;  //地块集合
                export.ExportPublicTableDeleteEmpty = exportdelempty;
                var sender = GetTissue(zone.ID); //发包方
                if (sender == null)
                {
                    var tissueStation = dbContext.CreateCollectivityTissueWorkStation();
                    var tissues = tissueStation.GetTissues(zone.FullCode, eLevelOption.Self);
                    if (tissues != null && tissues.Count > 0)
                        sender = tissues[0];
                }
                if (SystemSet.ExportTableSenderDesToVillage)
                {
                    sender.Name = GetVillageLevelDesc(zone);
                }
                if (concords != null && concords.Count > 0)
                {
                    export.Concord = concords.Find(d => d.ArableLandType == "110");
                }
                else
                {
                    export.Concord = null;
                }
                export.Tissue = sender;
                export.OpenTemplate(tempPath);
                //string fileName = filename + @"\" + vp.Name + "-" + TemplateFile.PublicityWord;
                //export.SaveAs(vp, fileName);  //合同
                string previewPath = SystemSet.DefaultPath + @"\" + vp.FamilyNumber + "-" + vp.Name + "-" + TemplateFile.PublicityWord;
                export.PrintPreview(vp, previewPath);
                flag = true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word表)", ex.Message + ex.StackTrace);
            }
            return flag;
        }

        /// <summary>
        /// 导出地块调查表(Word)
        /// </summary>
        public bool ExportLandWord(Zone zone, ContractLand land, VirtualPerson vp, string filename)
        {
            bool flag = true;
            int dotCount = 0;
            try
            {
                if (land == null)
                {
                    return flag;
                }
                var db = DataBaseSource.GetDataBaseSource();
                if (db == null)
                    this.ReportError(DataBaseSource.ConnectionError);
                var dotStation = db.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var concord = (land.ConcordId != null && land.ConcordId.HasValue) ? concordStation.Get(land.ConcordId.Value) : null;
                var tissue = concord != null ? senderStation.Get(concord.SenderId) : null;
                if (tissue == null && zone != null)
                {
                    tissue = senderStation.Get(zone.ID);
                }
                if (SystemSet.ExportTableSenderDesToVillage && zone.Level == eZoneLevel.Group)
                {
                    var Senders = senderStation.GetTissues(zone.UpLevelCode, eLevelOption.Self);
                    if (Senders.Count > 0)
                    {
                        tissue = Senders[0];
                    }
                }
                var listLandCoil = coilStation.GetByLandID(land.ID);
                var listLandDot = dotStation.GetByLandID(land.ID);
                var listValidLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot.FindAll(c => c.IsValid == true);
                ExportLandSurveyWordTable export = new ExportLandSurveyWordTable();
                //dotCount = export.InitalizeDotCount(land);
                dotCount = listValidLandDot.Count == 0 ? (listLandDot == null ? 0 : listLandDot.Count) : (listValidLandDot.Count);
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWord);  //模板文件
                if (dotCount > 6 && dotCount <= 21)
                {
                    tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordTwo);  //模板文件2页
                }
                else if (dotCount > 21 && dotCount <= 36)
                {
                    tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordThree);  //模板文件3页
                }
                else if (dotCount > 36)
                {
                    tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordOther);  //模板文件其它
                }
                else { }

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportLandSurveyWordTable)
                    {
                        export = (ExportLandSurveyWordTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.Contractor = vp;
                export.DictList = DictList;
                export.CurrentZone = zone;
                export.Concord = concord;
                export.Tissue = tissue; //GetTissue(zone.ID); //发包方
                export.ListLandCoil = listLandCoil == null ? new List<BuildLandBoundaryAddressCoil>() : listLandCoil;
                export.ListLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot;
                export.ListLandValidDot = listValidLandDot;
                export.OpenTemplate(tempPath);
                string landNumber = land.LandNumber.Length > 5 ? land.LandNumber.Substring(land.LandNumber.Length - 5) : land.LandNumber;
                string filePath = filename + @"\" + landNumber + "-" + TemplateFile.ContractAccountLandSurveyWord;
                export.SaveAs(land, filePath);
            }
            catch (Exception ex)
            {
                flag = false;
                this.ReportError("导出地块调查表失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word表)", ex.Message + ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
            return flag;
        }

        /// <summary>
        /// 导出地块调查表(Word)
        /// </summary>
        public bool ExportLandWord(Zone zone, ContractLand land, VirtualPerson vp, List<Dictionary> lstDict)
        {
            bool flag = true;
            int dotCount = 0;
            try
            {
                if (land == null)
                {
                    return flag;
                }
                var db = DataBaseSource.GetDataBaseSource();
                if (db == null)
                    this.ReportError(DataBaseSource.ConnectionError);
                var dotStation = db.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var concord = (land.ConcordId != null && land.ConcordId.HasValue) ? concordStation.Get(land.ConcordId.Value) : null;
                var tissue = concord != null ? senderStation.Get(concord.SenderId) : null;
                if (tissue == null && zone != null)
                {
                    tissue = senderStation.Get(zone.ID);
                }
                if (SystemSet.ExportTableSenderDesToVillage && zone.Level == eZoneLevel.Group)
                {
                    var Senders = senderStation.GetTissues(zone.UpLevelCode, eLevelOption.Self);
                    if (Senders.Count > 0)
                    {
                        tissue = Senders[0];
                    }
                }
                var listLandCoil = coilStation.GetByLandID(land.ID);
                var listLandDot = dotStation.GetByLandID(land.ID);
                var listValidLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot.FindAll(c => c.IsValid == true);
                ExportLandSurveyWordTable export = new ExportLandSurveyWordTable();
                export.DictList = DictList;
                dotCount = listValidLandDot.Count == 0 ? (listLandDot == null ? 0 : listLandDot.Count) : (listValidLandDot.Count);
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWord);  //模板文件
                if (dotCount > 6 && dotCount <= 21)
                {
                    tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordTwo);  //模板文件2页
                }
                else if (dotCount > 21 && dotCount <= 36)
                {
                    tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordThree);  //模板文件3页
                }
                else if (dotCount > 36)
                {
                    tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordOther);  //模板文件其它
                }
                else { }

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportLandSurveyWordTable)
                    {
                        export = (ExportLandSurveyWordTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.Contractor = vp;
                export.DictList = lstDict;
                export.CurrentZone = zone;
                export.Concord = concord;
                export.Tissue = tissue; //GetTissue(zone.ID); //发包方
                export.ListLandCoil = listLandCoil == null ? new List<BuildLandBoundaryAddressCoil>() : listLandCoil;
                export.ListLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot;
                export.ListLandValidDot = listValidLandDot;
                export.OpenTemplate(tempPath);
                string previewPath = SystemSet.DefaultPath + @"\" + vp.Name + @"\" + land.LandNumber + "-" + TemplateFile.ContractAccountLandSurveyWord;
                export.PrintPreview(land, previewPath);
            }
            catch (Exception ex)
            {
                flag = false;
                this.ReportError("导出地块调查表失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word表)", ex.Message + ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
            return flag;
        }

        /// <summary>
        /// 导出发包方调查表（Excel）
        /// </summary>
        public void ExportSenderExcel(Zone zone, string filename)
        {
            try
            {
                this.ReportProgress(1, "正在获取发包方数据");
                string messageName = SenderMessage.SENDER_GETCHILDRENDATA;
                ModuleMsgArgs args = new ModuleMsgArgs();
                args.Name = messageName;
                args.Parameter = zone.FullCode;
                args.Datasource = DataBaseSource.GetDataBaseSource();
                TheBns.Current.Message.Send(this, args);
                string excelName = GetMarkDesc(zone);
                List<CollectivityTissue> list = args.ReturnValue as List<CollectivityTissue>;
                this.ReportProgress(10, "完成发包方数据获取");
                if (list.Count > 0)
                {
                    string tempName = TemplateHelper.ExcelTemplate(TemplateFile.SenderSurveyExcel);
                    this.ReportProgress(15, string.Format("导出{0}", excelName));
                    using (ExportSenderSurveyTable export = new ExportSenderSurveyTable())
                    {
                        export.TissueCollection = list;
                        //export.PostProgressEvent += new YuLinTu.Library.Office.ExportExcelBase.PostProgressDelegate(export_PostProgressEvent;
                        export.ShowValue = false;
                        export.SaveFileName = filename + @"\" + excelName + "发包方调查表";
                        export.BeginToZone(tempName);
                    }
                    this.ReportProgress(100);
                    this.ReportInfomation(string.Format("成功导出{0}条发包方数据", list.Count));
                }
                else
                {
                    this.ReportError("未获取到发包方数据!");
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderExcel(导出发包方调查表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出承包方调查表（Excel）
        /// </summary>
        public void ExportVirtualPersonExcel(Zone zone, string filename, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                List<VirtualPerson> tbVps = GetTableByZone(zone.FullCode);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.VirtualPersonSurveyExcel);
                string zoneName = GetUinitName(zone);
                var ownerIds = new List<Guid>();
                vps.ForEach(c => ownerIds.Add(c.ID));
                var landList = landStation.GetCollection(ownerIds);
                using (ExportContractorExcel export = new ExportContractorExcel())
                {
                    export.SaveFilePath = filename + @"\" + excelName + TemplateFile.VirtualPersonSurveyExcel + ".xls";
                    export.PersonType = VirtualType;
                    export.CurrentZone = zone;
                    export.FamilyList = vps;
                    export.TableFamilyList = tbVps;
                    export.LandList = landList;
                    export.UnitName = zoneName;
                    export.Percent = averagePercent;
                    export.CurrentPercent = currentPercent;
                    export.ZoneDesc = excelName;
                    //export.TemplateFile = tempPath;
                    export.PostProgressEvent += export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginExcel(tempPath);
                }
                vps = null;
                tbVps = null;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出公示调查表（Excel）
        /// </summary>
        public void ExportPublishExcel(Zone zone, string filename, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                List<ContractAccountLandFamily> accountFamilyCollection = new List<ContractAccountLandFamily>();
                //得到承包方的集合
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                string excelName = GetMarkDesc(zone);

                foreach (VirtualPerson vp in vps)
                {
                    ContractAccountLandFamily accountLandFamily = new ContractAccountLandFamily();
                    accountLandFamily.CurrentFamily = vp;
                    accountLandFamily.Persons = vp.SharePersonList;
                    //得到承包地
                    accountLandFamily.LandCollection = GetPersonCollection(vp.ID);
                    accountFamilyCollection.Add(accountLandFamily);
                }
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.LandInfomationExcel);
                string zoneName = GetUinitName(zone);
                ExportLandInfomationExcelTable export = new ExportLandInfomationExcelTable();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportLandInfomationExcelTable)
                    {
                        export = (ExportLandInfomationExcelTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.SaveFilePath = filename + @"\" + excelName + TemplateFile.LandInfomationExcel + ".xls";
                export.CurrentZone = zone;
                export.TemplateFile = tempPath;
                export.AccountLandFamily = accountFamilyCollection;
                //export.UnitName = GetUinitName(zone);             //得到单位名称
                //export.TitleName = GetTitleName(zone);            //得到表标题
                export.Tissue = GetTissue(zone.ID);               //发包方
                export.Percent = averagePercent;
                export.CurrentPercent = currentPercent;
                export.ZoneDesc = excelName;
                export.StartDate = this.PublishDateSetting.PublishStartDate;
                export.EndDate = this.PublishDateSetting.PublishEndDate;
                export.DrawPerson = this.PublishDateSetting.CreateTablePerson;
                export.DrawDate = this.PublishDateSetting.CreateTableDate;
                export.CheckPerson = this.PublishDateSetting.CheckTablePerson;
                export.CheckDate = this.PublishDateSetting.CheckTableDate;
                //配置信息
                export.PostProgressEvent += export_PostProgressEvent;
                export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                bool result = export.BeginToZone(tempPath);
                export.SaveAs(export.SaveFilePath);
                this.ReportInfomation(export.Information);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出农村土地承包经营权数据汇总表
        /// </summary>
        public void ExportSummaryExcel(Zone zone, string fileName, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                List<VirtualPerson> vpsSecond = GetTableByZone(zone.FullCode);   //二轮承包方
                List<ContractLand> lands = GetCollection(zone.FullCode, eLevelOption.Self);   //当前地域下的所有承包地块集合
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractSummaryExcel);
                string uinitName = GetUinitName(zone);   //单位名称
                using (ExportContractorSummaryExcel summaryExport = new ExportContractorSummaryExcel(dbContext))
                {
                    summaryExport.SaveFilePath = fileName + @"\" + excelName + TemplateFile.ContractSummaryExcel + ".xls";
                    summaryExport.CurrentZone = zone;
                    summaryExport.ListPerson = vps;
                    summaryExport.ListLand = lands;
                    summaryExport.UnitName = uinitName;
                    summaryExport.StatuDes = excelName + "数据汇总表";
                    summaryExport.ArgType = this.ArgType;
                    summaryExport.Percent = averagePercent;
                    summaryExport.CurrentPercent = currentPercent;
                    //summaryExport.PostProgressEvent +=export_PostProgressEvent;
                    summaryExport.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = summaryExport.BeginExcel(tempPath);   //开始导表
                }
                vps = null;
                vpsSecond = null;
                lands = null;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSummaryExcel(导出汇总数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 批量导出村组公示公告表Word
        /// </summary>
        /// <param name="zone">当前导出行政地域</param>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="dateSetting">时间等设置</param>
        public void VolumnExportVillagesDeclareWord(Zone zone, string fileName, DateSetting dateSetting = null)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string stampUnit = string.Empty;  //盖章单位
                string address = string.Empty;    //申请单位与地址
                string temp = string.Empty;
                if (zone.Level == eZoneLevel.Town)
                {
                    //不应该导镇下的数据
                    return;
                }
                else if (zone.Level == eZoneLevel.Village)
                {
                    //此时在导某村(社区)的公示表
                    stampUnit = zone.FullName.Replace(zone.UpLevelName, "") + "村委会";
                    temp = zone.FullName.Replace("四川省", "");
                    address = stampUnit + "(地址:" + temp + "村委会)";
                    dateSetting.StampUnit = stampUnit;
                    dateSetting.Address = address;
                }
                else if (zone.Level == eZoneLevel.Group)
                {
                    //此时在导某村下的某组的公示表
                    Zone tempZone = GetParent(zone);
                    stampUnit = zone.UpLevelName.Replace(tempZone.UpLevelName, "") + "村委会";
                    temp = tempZone.FullName.Replace("四川省", "");
                    address = stampUnit + "(地址:" + temp + "村委会)";
                    dateSetting.StampUnit = stampUnit;
                    dateSetting.Address = address;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> listVp = GetByZone(zone.FullCode);
                List<ContractLand> listLand = GetCollection(zone.FullCode, eLevelOption.Self);
                if (listVp == null || listVp.Count == 0)
                {
                    this.ReportError(string.Format("在{0}下未获取到承包方数据!", excelName));
                    return;
                }
                string statueDes = excelName + "公示公告";
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.AnnouncementWord);  //模板路径
                string savePath = fileName + @"\" + excelName + TemplateFile.AnnouncementWord + ".doc";   //保存文件全路径
                string unitName = GetUinitName(zone);
                ExportAnnouncementWord exportWord = new ExportAnnouncementWord(dbContext);
                exportWord.CurrentZone = zone;
                exportWord.DateSettingForAnnoucementWord = dateSetting;
                exportWord.ListPerson = listVp;
                exportWord.ListLand = listLand;
                if (exportWord.OpenTemplate(templatePath))   //打开模板
                    exportWord.SaveAs(zone, savePath);
                listVp = null;
                listLand = null;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "VolumnExportVillagesDeclareWord(批量导出村组公示公告Word)", ex.Message + ex.StackTrace);
            }
        }

        #endregion 导出调查报表

        #region 地籍数据处理

        /// <summary>
        /// 预览单户的多地块示意图
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="person">当前户</param>
        /// <param name="filePath">保存文件路径</param>
        /// <param name="save">是否保存</param>
        public void ExportMultiParcelWord(Zone currentZone, List<ContractLand> listLand, VirtualPerson person, string filePath, bool save = false, string imageSavePath = "", bool? isStockLand = false)
        {
            //List<ContractLand> listLand = new List<ContractLand>();
            List<ContractLand> listGeoLand = new List<ContractLand>(1000);
            List<XZDW> listLine = new List<XZDW>(1000);
            List<DZDW> listPoint = new List<DZDW>(1000);
            List<MZDW> listPolygon = new List<MZDW>(1000);
            List<ContractConcord> listConcord = new List<ContractConcord>(1000);
            List<ContractRegeditBook> listBook = new List<ContractRegeditBook>(1000);
            List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
            List<Dictionary> dictDKLB = new List<Dictionary>(500);
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(1000);
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(1000);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(1000);
            DiagramsView viewOfAllMultiParcel = null;
            DiagramsView viewOfNeighorParcels = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                viewOfAllMultiParcel = new DiagramsView();
                viewOfNeighorParcels = new DiagramsView();
            }));
            try
            {
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var lineStation = dbContext.CreateXZDWWorkStation();
                var PointStation = dbContext.CreateDZDWWorkStation();
                var PolygonStation = dbContext.CreateMZDWWorkStation();
                string templatePath = string.Empty;
                if (isStockLand != null)
                    templatePath = TemplateHelper.WordTemplate(((bool)isStockLand ? "安徽" : "") + TemplateFile.ParcelWord);
                else
                    templatePath = TemplateHelper.WordTemplate(TemplateFile.ParcelWord);
                string savePathOfImage = string.IsNullOrEmpty(imageSavePath) ? filePath : imageSavePath;
                string savePathOfWord = InitalizeLandImageName(filePath, person); // filePath + @"\" + familyNuber + "-" + person.Name + "-" + TemplateFile.ParcelWord + ".doc";

                //listLand = GetCollection(currentZone.FullCode, eLevelOption.Self);
                var VillageZone = GetParent(currentZone);
                if (isStockLand != null)
                {
                    if ((bool)isStockLand)
                    {
                        var stockLandsvp = dbContext.CreateBelongRelationWorkStation().GetLandByPerson(person.ID, currentZone.FullCode);
                        listLand = stockLandsvp;
                    }
                    else
                    {
                        // 导出确权地块示意图，要排除掉股地
                        listLand = listLand.FindAll(c => !c.IsStockLand);
                    }
                }
                listGeoLand = listLand.FindAll(c => c.Shape != null);
                listGeoLand = InitalizeAgricultureLandSortValue(listGeoLand);
                listLine = lineStation.GetByZoneCode(currentZone.FullCode);
                dictDKLB = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                listTissue = landStation.GetTissuesByConcord(currentZone);
                if (listTissue.Count == 0)
                {
                    var senderStation = dbContext.CreateSenderWorkStation();
                    var tissue = senderStation.Get(currentZone.ID);

                    if (tissue == null)
                    {
                        listTissue = senderStation.GetTissues(currentZone.FullCode, eLevelOption.Self);
                    }
                    else
                    {
                        listTissue.Add(tissue);
                    }
                }

                listDot = dotStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listPoint = PointStation.GetByZoneCode(currentZone.FullCode);
                listPolygon = PolygonStation.GetByZoneCode(currentZone.FullCode);
                listLine.RemoveAll(l => l.Shape == null);
                listPoint.RemoveAll(l => l.Shape == null);
                listPolygon.RemoveAll(l => l.Shape == null);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel.Paper.Model.Width = 236;
                    viewOfAllMultiParcel.Paper.Model.Height = 217;
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;
                }));

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfNeighorParcels.Paper.Model.Width = 160;
                    viewOfNeighorParcels.Paper.Model.Height = 160;
                    viewOfNeighorParcels.Paper.Model.BorderWidth = 0;
                    viewOfNeighorParcels.Paper.Model.X = 0;
                    viewOfNeighorParcels.Paper.Model.Y = 0;
                }));

                var parcelWord = new ExportContractLandParcelWord(dbContext);

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(parcelWord);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractLandParcelWord)
                    {
                        parcelWord = (ExportContractLandParcelWord)temp;
                    }
                    templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                parcelWord.IsStockLand = isStockLand;
                parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                parcelWord.CurrentZone = currentZone;
                parcelWord.ZoneList = GetParentZone(currentZone, dbContext);
                parcelWord.VillageZone = VillageZone;
                parcelWord.DictList = DictList;
                parcelWord.SavePathOfImage = savePathOfImage;
                parcelWord.SavePathOfWord = savePathOfWord;
                parcelWord.DictDKLB = dictDKLB;
                parcelWord.ListTissue = listTissue;
                parcelWord.ListGeoLand = listGeoLand;
                parcelWord.ListLineFeature = listLine;
                parcelWord.ListPointFeature = listPoint;
                parcelWord.ListPolygonFeature = listPolygon;
                parcelWord.ListValidDot = listValidDot;
                parcelWord.ParcelCheckDate = DateTime.Now;
                parcelWord.ParcelDrawDate = DateTime.Now;
                parcelWord.IsSave = save;
                parcelWord.OwnedPerson = person;
                parcelWord.OpenTemplate(templatePath);
                if (!save)
                    parcelWord.PrintPreview(person, savePathOfWord);
                else
                    parcelWord.SaveAsMultiFile(person, savePathOfWord, ParcelWordSettingDefine.SaveParcelPCAsPDF);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message + "，请检查有无发包方信息等");
                throw ex;
            }
            finally
            {
                listLand.Clear();
                listGeoLand.Clear();
                listLine.Clear();
                listConcord.Clear();
                listBook.Clear();
                listTissue.Clear();
                dictDKLB.Clear();
                listDot.Clear();
                listCoil.Clear();
                listValidDot.Clear();

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel.Dispose();
                    viewOfAllMultiParcel = null;
                    viewOfNeighorParcels.Dispose();
                    viewOfNeighorParcels = null;
                }));

                GC.Collect();
                GC.WaitForFullGCComplete();
            }
        }

        /// <summary>
        /// 获取地域集合
        /// </summary>
        public List<YuLinTu.Library.Entity.Zone> GetParentZone(YuLinTu.Library.Entity.Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_TOPROVINCEZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<YuLinTu.Library.Entity.Zone>);
        }

        /// <summary>
        /// 按照设置进行地块类别筛选导出
        /// </summary>
        private List<ContractLand> InitalizeAgricultureLandSortValue(List<ContractLand> geoLandCollection)
        {
            if (geoLandCollection.Count == 0) return new List<ContractLand>();
            if (ParcelWordSettingDefine.ExportContractLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.ContractLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportPrivateLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.PrivateLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportMotorizeLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.MotorizeLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportWasteLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.WasteLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportCollectiveLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.CollectiveLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportEncollecLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.EncollecLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportFeedLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.FeedLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportAbandonedLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.AbandonedLand).ToString());
            }
            return geoLandCollection;
        }

        /// <summary>
        /// 初始化地块示意图名称
        /// </summary>
        /// <returns></returns>
        private string InitalizeLandImageName(string filePath, VirtualPerson family)
        {
            if (family == null)
            {
                return "";
            }
            string zoneCode = family.ZoneCode;
            if (!string.IsNullOrEmpty(zoneCode) && zoneCode.Length != 14)
                zoneCode = zoneCode.PadRight(14, '0');
            string imagePath = filePath + "\\" + zoneCode;
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            string imageName = imagePath + "\\" + "DKSYT" + zoneCode;
            int number = 0;
            Int32.TryParse(family.FamilyNumber, out number);
            imageName += string.Format("{0:D4}", number);
            imageName += "J";
            return imageName;
        }

        /// <summary>
        /// 导出界址点成果表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="land">承包地块</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="exportType">导出类型(直接预览：1、打印：2、保存：3)</param>
        public bool ExportDotResultExcel(Zone currentZone, ContractLand land, string filePath = "", int exportType = 1)
        {
            bool isSuccess = true;
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return false;
            }
            try
            {
                land.LandNumberFormat(SystemSet);
                string templatePath = TemplateHelper.ExcelTemplate(TemplateFile.BoundaryAddressDotResultExcel);
                ExportBoundaryAddressDotResult dotResultExport = new ExportBoundaryAddressDotResult();
                //dotResultExport.PostProgressEvent -=export_PostProgressEvent;
                dotResultExport.PostErrorInfoEvent += export_PostErrorInfoEvent;
                dotResultExport.SavePath = filePath;
                dotResultExport.Exporttype = exportType;   //直接弹出
                isSuccess = dotResultExport.CommenceLandExcel(land, templatePath);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDotResultExcel(导出界址点成果表Excel)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        #endregion 地籍数据处理

        #region 工具之初始数据

        /// <summary>
        /// 初始化承包地块属性信息
        /// </summary>
        /// <param name="metadata">任务参数(主要传递初始化界面上设置的一些参数)</param>
        /// <param name="listLand">待初始化的地块集合</param>
        /// <param name="currentZone">当前初始化地块集合所在的地域</param>
        public void ContractLandInitialTool(TaskInitialLandInfoArgument metadata, List<ContractLand> listLand, Zone currentZone)
        {
            string markDesc = GetMarkDesc(currentZone);

            //获取地域下所有人
            List<VirtualPerson> zonePersonList = new List<VirtualPerson>();
            zonePersonList = landVirtualPersonStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
            zonePersonList.Sort((a, b) =>
            {
                long aNumber = Convert.ToInt64(a.FamilyNumber);
                long bNumber = Convert.ToInt64(b.FamilyNumber);
                return aNumber.CompareTo(bNumber);
            });

            var db = DataBaseSource.GetDataBaseSource();
            try
            {
                if (currentZone == null)
                {
                    this.ReportError("未选择初始化数据的地域!");
                    return;
                }
                if (db == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return;
                }
                var landStation = db.CreateContractLandWorkstation();
                var senderStation = db.CreateSenderWorkStation();
                var sender = senderStation.Get(currentZone.ID);
                if (sender == null)
                {
                    var senders = senderStation.GetTissues(currentZone.FullCode, eLevelOption.Self);
                    if (senders != null && senders.Count > 0)
                        sender = senders[0];
                    if (sender == null)
                    {
                        this.ReportError(currentZone.Name + "下发包方为空!");
                        return;
                    }
                }
                var landsOfStatus = landStation.GetCollection(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
                landsOfStatus.AddRange(landStation.Get(o => o.IsStockLand == true && o.ZoneCode == currentZone.FullCode).ToList());
                if (landsOfStatus == null || landsOfStatus.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取没有锁定的承包地块", markDesc));
                    return;
                }
                //if (metadata.InitialLandNumber && metadata.IsNew && metadata.InitialNull)是否按照从左到右初始化
                if (metadata.InitialLandNumberByUpDown)
                {
                    landsOfStatus = OrderLandByPosition(currentZone, landsOfStatus);
                }
                db.BeginTransaction();

                //if (metadata.InitialLandNeighborInfo)
                //{
                //    var tolerance = ContractBusinessParcelWordSettingDefine.GetIntence().Neighborlandbufferdistence;
                //    using (var dba = new DBSpatialite())
                //    {
                //        var dbFile = dbContext.DataSource.ConnectionString;
                //        dbFile = dbFile.Substring(dbFile.IndexOf('=') + 1);

                //        dba.Open(dbFile);//@"C:\myprojects\工单\20160816建库工具\通川区安云乡.sqlite");
                //        var prms = new InitLandDotCoilParam();

                //        prms.Tolerance = tolerance;

                //        var t = new InitLandNeighborInfo(dba, prms);
                //        t.ReportProgress += (msg, i) =>
                //        {
                //            this.ReportProgress(i, msg);
                //        };
                //        t.ReportInfomation += msg =>
                //        {
                //            this.ReportInfomation(msg);
                //        };

                //        string wh = null;
                //        wh = string.Format(Zd_cbdFields.ZLDM + " like '{0}%'", currentZone.FullCode);// zldm like '511702208200%'";
                //                                                                                          //}
                //        t.DoInit(wh);
                //    }
                //}
                ProcessLandInformationInstall(landStation, landsOfStatus, metadata, zonePersonList, currentZone, sender, markDesc, listLand.Count);
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                this.ReportError("初始化地块基本信息失败");
                YuLinTu.Library.Log.Log.WriteError(this, "ContractLandInitialTool(提交初始化数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 地块按方位排序
        /// </summary>
        /// <returns></returns>
        public List<ContractLand> OrderLandByPosition(Zone currenzone, List<ContractLand> InitialLands)
        {
            var newLandList = new List<ContractLand>();
            var idSet = new HashSet<Guid>();
            var minEnvelopeWidth = 5.0;
            var minEnvelopeHeight = 5.0;
            var datafullextend = GetALLExtend(currenzone, InitialLands, ref minEnvelopeWidth, ref minEnvelopeHeight);
            var startNumber = currenzone.FullCode.PadRight(14, '0');
            var sr = dbContext.DataSource.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).Schema,
                ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).TableName);

            var minX = datafullextend.MinX;
            var minY = datafullextend.MinY;

            var maxX = datafullextend.MaxX;
            var maxY = datafullextend.MaxY;

            var startX = minX;
            var startY = maxY;

            var widthCount = (maxX - minX) / minEnvelopeWidth + 1;
            var heightCount = (maxY - minY) / minEnvelopeHeight + 1;
            double percent = 98 / (double)InitialLands.Count;

            for (int i = 0; i < heightCount; i++)
            {
                Envelope item = new Envelope(sr);
                item.MinX = minX;
                item.MaxX = maxX;
                item.MinY = startY - (i + 1) * minEnvelopeHeight;
                item.MaxY = startY - i * minEnvelopeHeight;
                var interLands = InitialLands.FindAll(ls => ls.Shape != null && ls.Shape.Intersects(item.ToGeometry()));
                interLands = interLands.OrderBy(s => s.Shape.MinX()).ToList();
                foreach (var landitem in interLands)
                {
                    if (!idSet.Contains(landitem.ID))
                    {
                        newLandList.Add(landitem);
                        idSet.Add(landitem.ID);
                    }
                }
            }
            return newLandList;
        }

        /// <summary>
        /// 获取数据的全边框，最小的边框宽度 长度
        /// </summary>
        /// <returns></returns>
        public Envelope GetALLExtend(Zone zone, List<ContractLand> InitialLands, ref double minEnvelopeWidth, ref double minEnvelopeHeight)
        {
            Envelope extend = null;
            if (zone.Shape != null)
            {
                extend = zone.Shape.GetEnvelope();

                minEnvelopeWidth = 5;
                minEnvelopeHeight = 5;
            }
            else
            {
                foreach (var item in InitialLands)
                {
                    var itemgeo = item.Shape;
                    if (itemgeo == null) continue;
                    if (extend == null)
                    {
                        extend = itemgeo.GetEnvelope();

                        minEnvelopeWidth = extend.Width;
                        minEnvelopeHeight = extend.Height;
                    }
                    else
                    {
                        var itemgeoEnv = itemgeo.GetEnvelope();
                        if (itemgeoEnv.MinX > 577122.26277372276 - 0.00001 && itemgeoEnv.MinX < 577122.26277372276 + 0.00001)
                        {
                        }
                        if (itemgeoEnv.Width < minEnvelopeWidth)
                        {
                            minEnvelopeWidth = itemgeoEnv.Width;
                        }
                        if (itemgeoEnv.Height < minEnvelopeHeight)
                        {
                            minEnvelopeHeight = itemgeoEnv.Height;
                        }

                        extend.Union(itemgeoEnv);
                    }
                }
            }
            return extend;
        }

        /// <summary>
        /// 初始化地块信息
        /// </summary>
        private void ProcessLandInformationInstall(IContractLandWorkStation landStation, List<ContractLand> landsOfStatus, TaskInitialLandInfoArgument metadata,
          List<VirtualPerson> zonePersonList, Zone currentZone, CollectivityTissue sender, string markDesc, int listLandCount)
        {
            int index = 1;   //地块索引
            int landIndex = 1;
            int successCount = 0;
            double landPercent = 0.0;  //百分比

            this.ReportProgress(1, null);
            landPercent = 99 / (double)landsOfStatus.Count;
            bool isNULL = metadata.InitialNull;
            var otherLand = landsOfStatus.FindAll(c => !string.IsNullOrEmpty(c.LandCategory) && c.LandCategory != DictionaryTypeInfo.CBDK);

            foreach (var land in landsOfStatus)
            {
                if (metadata.HandleContractLand && !string.IsNullOrEmpty(land.LandCategory) &&
                    !(land.LandCategory.Equals(DictionaryTypeInfo.CBDK)))
                {
                    //仅初始化承包地块
                    continue;
                }
                if (metadata.InitialLandNeighbor)
                {
                    land.NeighborEast = GetLandNeighborStrStart(land.NeighborEast);
                    land.NeighborNorth = GetLandNeighborStrStart(land.NeighborNorth);
                    land.NeighborSouth = GetLandNeighborStrStart(land.NeighborSouth);
                    land.NeighborWest = GetLandNeighborStrStart(land.NeighborWest);
                }

                if (!isNULL || (isNULL && (land.LandName == null || land.LandName.Trim() == string.Empty)))
                {
                    if (metadata.InitialLandName)
                    {
                        land.LandName = metadata.LandName.Name;
                        land.LandCode = metadata.LandName.Code;
                    }
                }
                if (!isNULL || (isNULL && (land.OwnRightType == null || land.OwnRightType.Trim() == string.Empty)))
                {
                    if (metadata.InitialQSXZ)
                    {
                        land.OwnRightType = metadata.QSXZ.Code;
                    }
                }
                if (!isNULL || (isNULL && (land.LandLevel == null || land.LandLevel.Trim() == string.Empty || land.LandLevel == "900")))
                {
                    if (metadata.InitialLandLevel)
                    {
                        land.LandLevel = metadata.LandLevel.Code;
                    }
                }
                if (!isNULL || (isNULL && (land.Purpose == null || land.Purpose.Trim() == string.Empty)))
                {
                    if (metadata.InitialLandPurpose)
                    {
                        land.Purpose = metadata.LandPurpose.Code;
                    }
                }
                if (!isNULL || (isNULL && land.IsFarmerLand == null))
                {
                    if (metadata.InitialIsFamer)
                    {
                        land.IsFarmerLand = metadata.IsFamer;
                        //if (metadata.IsFamer.Code.IsNullOrBlank())
                        //    land.IsFarmerLand = null;
                        //else
                        //{
                        //    if (metadata.IsFamer.Code.Equals(DictionaryTypeInfo.S))
                        //        land.IsFarmerLand = true;
                        //    else if (metadata.IsFamer.Code.Equals(DictionaryTypeInfo.F))
                        //        land.IsFarmerLand = false;
                        //    else
                        //        land.IsFarmerLand = null;
                        //}
                    }
                }
                if (!isNULL || (isNULL && land.AwareArea == 0))
                {
                    if (metadata.InitialAwareArea)
                    {
                        if (metadata.AwareAreaEqualActual)
                        {
                            //确权面积等于实测面积
                            land.AwareArea = land.ActualArea;
                        }
                        else
                        {
                            //确权面积等于二轮合同面积
                            land.AwareArea = (double)land.TableArea;
                        }
                    }
                }
                if (!isNULL || (isNULL && (land.LandNumber == null || land.LandNumber.Trim() == string.Empty)))
                {
                    if (metadata.InitialLandOldNumber)
                    {
                        land.OldLandNumber = land.LandNumber;
                        land.SourceNumber = land.LandNumber;
                        if (metadata.IsCombination)
                        {
                            //如果地块编码是19位的，不处理。如果调查编码是空的，就用地块编码。
                            if (land.SurveyNumber.IsNullOrEmpty() == false)
                            {
                                land.LandNumber = sender.Code + land.SurveyNumber.PadLeft(5, '0');
                            }
                            else
                            {
                                if (land.LandNumber != null && land.LandNumber.Length <= 5)
                                {
                                    land.LandNumber = sender.Code + land.LandNumber.PadLeft(5, '0');
                                }
                                if (land.LandNumber.Length == 19 &&
                                    !land.LandNumber.StartsWith(land.SenderCode))
                                {
                                    land.SurveyNumber = land.LandNumber.Substring(14);
                                    land.LandNumber = sender.Code + land.SurveyNumber;
                                }
                            }
                        }
                        else
                        {
                            if (metadata.VillageInlitialSet)
                            {
                                //按村发包
                                string unitCode = string.Empty;
                                if (currentZone.Level == eZoneLevel.Group)
                                    unitCode = currentZone.UpLevelCode;
                                else
                                    unitCode = currentZone.FullCode;
                                if (unitCode.Length == 16)
                                    unitCode = unitCode.Substring(0, 12) + unitCode.Substring(14, 2);
                                unitCode = unitCode.PadRight(14, '0');
                                if (metadata.HandleContractLand)
                                    metadata.CombinationLandNumber[0] = CheckLandNumber(unitCode, metadata.CombinationLandNumber[0], otherLand);
                                land.LandNumber = unitCode + metadata.CombinationLandNumber[0].ToString().PadLeft(5, '0');
                            }
                            else
                            {
                                //按组发包
                                string unitCode = currentZone.FullCode;
                                if (unitCode.Length == 16)
                                    unitCode = unitCode.Substring(0, 12) + unitCode.Substring(14, 2);
                                unitCode = unitCode.PadRight(14, '0');
                                if (metadata.HandleContractLand)
                                    landIndex = CheckLandNumber(unitCode, landIndex, otherLand);
                                string landnumber = unitCode + landIndex.ToString().PadLeft(5, '0');
                                land.LandNumber = landnumber;
                            }
                        }
                    }
                    else
                    {
                        land.SourceNumber = land.LandNumber;
                        if (metadata.IsCombination)
                        {
                            //如果地块编码是19位的，不处理。如果调查编码是空的，就用地块编码。
                            if (land.SurveyNumber.IsNullOrEmpty() == false)
                            {
                                land.LandNumber = sender.Code + land.SurveyNumber.PadLeft(5, '0');
                            }
                            else
                            {
                                if (land.LandNumber != null && land.LandNumber.Length <= 5)
                                {
                                    land.LandNumber = sender.Code + land.LandNumber.PadLeft(5, '0');
                                }
                                if (land.LandNumber.Length == 19 &&
                                    !land.LandNumber.StartsWith(land.SenderCode))
                                {
                                    land.SurveyNumber = land.LandNumber.Substring(14);
                                    land.LandNumber = sender.Code + land.SurveyNumber;
                                }
                            }
                        }
                        else
                        {
                            if (metadata.VillageInlitialSet)
                            {
                                //按村发包
                                string unitCode = string.Empty;
                                if (currentZone.Level == eZoneLevel.Group)
                                    unitCode = currentZone.UpLevelCode;
                                else
                                    unitCode = currentZone.FullCode;
                                if (unitCode.Length == 16)
                                    unitCode = unitCode.Substring(0, 12) + unitCode.Substring(14, 2);
                                unitCode = unitCode.PadRight(14, '0');
                                if (metadata.HandleContractLand)
                                    metadata.CombinationLandNumber[0] = CheckLandNumber(unitCode, metadata.CombinationLandNumber[0], otherLand);
                                land.LandNumber = unitCode + metadata.CombinationLandNumber[0].ToString().PadLeft(5, '0');
                            }
                            else
                            {
                                //按组发包
                                string unitCode = currentZone.FullCode;
                                if (unitCode.Length == 16)
                                    unitCode = unitCode.Substring(0, 12) + unitCode.Substring(14, 2);
                                unitCode = unitCode.PadRight(14, '0');
                                if (metadata.HandleContractLand)
                                    landIndex = CheckLandNumber(unitCode, landIndex, otherLand);
                                string landnumber = unitCode + landIndex.ToString().PadLeft(5, '0');
                                land.LandNumber = landnumber;
                            }
                        }
                    }
                   
                }
                AgricultureLandExpand landExpand = land.LandExpand;
                if (!isNULL)
                {
                    if (metadata.InitLandComment)
                        land.Comment = metadata.LandComment;
                    if (metadata.InitialMapNumber)
                        landExpand.ImageNumber = metadata.LandExpand.ImageNumber;
                    if (metadata.InitialSurveyPerson)
                        landExpand.SurveyPerson = metadata.LandExpand.SurveyPerson;
                    if (metadata.InitialSurveyDate)
                        landExpand.SurveyDate = metadata.LandExpand.SurveyDate;
                    if (metadata.InitialSurveyInfo)
                        landExpand.SurveyChronicle = metadata.LandExpand.SurveyChronicle;
                    if (metadata.InitialCheckPerson)
                        landExpand.CheckPerson = metadata.LandExpand.CheckPerson;
                    if (metadata.InitialCheckDate)
                        landExpand.CheckDate = metadata.LandExpand.CheckDate;
                    if (metadata.InitialCheckInfo)
                        landExpand.CheckOpinion = metadata.LandExpand.CheckOpinion;
                    if (metadata.InitialReferPerson)
                    {
                        if (metadata.InitialReferPersonByOwner)
                        {
                            //var vp = zonePersonList.Find(v => land.OwnerId == v.ID);
                            //metadata.LandExpand.ReferPerson = vp.Name;
                            //land.LandExpand = metadata.LandExpand;

                            var vp = zonePersonList.Find(v => land.OwnerId == v.ID);
                            landExpand.ReferPerson = vp.Name;
                        }
                        else
                        {
                            landExpand.ReferPerson = metadata.LandExpand.ReferPerson;
                        }
                    }
                    land.LandExpand = landExpand;
                }
                else
                {
                    if (metadata.InitLandComment && land.Comment.IsNullOrEmpty())
                        land.Comment = metadata.LandComment;
                    if (metadata.InitialMapNumber && (landExpand.ImageNumber == null || landExpand.ImageNumber.Trim().Equals("")))
                        landExpand.ImageNumber = metadata.LandExpand.ImageNumber;
                    if (metadata.InitialSurveyPerson && (landExpand.SurveyPerson == null || landExpand.SurveyPerson.Trim().Equals("")))
                        landExpand.SurveyPerson = metadata.LandExpand.SurveyPerson;
                    if (metadata.InitialSurveyDate && (landExpand.SurveyDate == null || landExpand.SurveyDate.Value.Equals("")))
                        landExpand.SurveyDate = metadata.LandExpand.SurveyDate;
                    if (metadata.InitialSurveyInfo && (landExpand.SurveyChronicle == null || landExpand.SurveyChronicle.Trim().Equals("")))
                        landExpand.SurveyChronicle = metadata.LandExpand.SurveyChronicle;
                    if (metadata.InitialCheckPerson && (landExpand.CheckPerson == null || landExpand.CheckPerson.Trim().Equals("")))
                        landExpand.CheckPerson = metadata.LandExpand.CheckPerson;
                    if (metadata.InitialCheckDate && (landExpand.CheckDate == null || landExpand.CheckDate.Value.Equals("")))
                        landExpand.CheckDate = metadata.LandExpand.CheckDate;
                    if (metadata.InitialCheckInfo && (landExpand.CheckOpinion == null || landExpand.CheckOpinion.Trim().Equals("")))
                        landExpand.CheckOpinion = metadata.LandExpand.CheckOpinion;
                    if (metadata.InitialReferPerson && (landExpand.ReferPerson == null || landExpand.ReferPerson.Trim().Equals("")))
                    {
                        if (metadata.InitialReferPersonByOwner)
                        {
                            //var vp = zonePersonList.Find(v => land.OwnerId == v.ID);
                            //metadata.LandExpand.ReferPerson = vp.Name;
                            //land.LandExpand = metadata.LandExpand;

                            var vp = zonePersonList.Find(v => land.OwnerId == v.ID);
                            landExpand.ReferPerson = vp.Name;
                        }
                        else
                        {
                            landExpand.ReferPerson = metadata.LandExpand.ReferPerson;
                        }
                    }
                    
                }
                land.LandExpand = landExpand;
                if (metadata.InitialLandOldNumber)
                {
                    int upCount = landStation.UpdateOldLandCode(land);
                    if (upCount > 0)
                        successCount++;
                }
                else
                {
                    int upCount = landStation.Update(land);
                    if (upCount > 0)
                        successCount++;
                }
                
                //if (metadata.InitialReferPersonByOwner)    //以地块当前承包方为指界人
                //{
                //    var vp = zonePersonList.Find(v => land.OwnerId == v.ID);
                //    metadata.LandExpand.ReferPerson = vp.Name;
                //    land.LandExpand = metadata.LandExpand;
                //}


                //ModifyLand(land);
                this.ReportProgress((int)(percent + landPercent * index), string.Format("{0}", markDesc + land.OwnerName));
                if (metadata.VillageInlitialSet)
                {
                    metadata.CombinationLandNumber[0]++;
                }
                index++;
                landIndex++;
            }

            this.ReportProgress(100, null);
            if (successCount == 0)
            {
                this.ReportInfomation(string.Format("没有地块初始化成功，请检查数据是否正确，是否包含在地域图斑中，是否有飞地等"));
            }
            else
            {
                this.ReportInfomation(string.Format("{0}共{1}个地块,成功初始化{2}个地块基本信息", markDesc, listLandCount, successCount));
            }
        }

        private string GetLandNeighborStrStart(string neighborStr)
        {
            if (neighborStr.IsNullOrEmpty()) return neighborStr;

            string liststrs = neighborStr;
            if (neighborStr.Contains("("))
            {
                liststrs = neighborStr.Split('(')[0];
            }
            else if (neighborStr.Contains("（"))
            {
                liststrs = neighborStr.Split('（')[0];
            }

            return liststrs;
        }

        /// <summary>
        /// 初始化承包地块编码时，如果只初始化承包地块时，避免与非承包地块编码重复
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="index"></param>
        /// <param name="landsOfStatus"></param>
        /// <returns></returns>
        private int CheckLandNumber(string unitCode, int index, List<ContractLand> landsOfStatus)
        {
            string landnumber = unitCode + index.ToString().PadLeft(5, '0');
            var findLands = landsOfStatus.FindAll(c => c.LandNumber == landnumber);
            if (findLands != null && findLands.Count > 0)
                return CheckLandNumber(unitCode, index + 1, landsOfStatus);
            else
                return index;
        }

        /// <summary>
        /// 初始化承包地块面积-面积量算
        /// </summary>
        /// <param name="metadata">任务参数(主要传递面积初始化界面上设置的一些参数)</param>
        /// <param name="listLand">待初始化的地块集合(具有空间数据的地块个数)</param>
        /// <param name="CurrentZone">当前初始化地块集合所在的地域</param>
        public void ContractLandAreaInitialTool(TaskInitialAreaArgument metadata, List<ContractLand> listGeoLand, Zone currentZone)
        {
            int index = 1;  //索引,记录具有空间数据的地块个数
            int successCount = 0;
            double geoLandPercent = 0.0;  //百分比
            string markDesc = GetMarkDesc(currentZone);
            var db = DataBaseSource.GetDataBaseSource();
            try
            {
                if (db == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return;
                }
                var landStation = db.CreateContractLandWorkstation();
                var landsOfStatus = landStation.GetCollection(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
                if (landsOfStatus == null || landsOfStatus.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取没有锁定的承包地块", markDesc));
                    return;
                }
                var shapeLandsOfStatus = landsOfStatus.FindAll(c => c.Shape != null);
                if (shapeLandsOfStatus == null || shapeLandsOfStatus.Count == 0)
                {
                    this.ReportProgress(0, string.Format("初始化{0}地块面积", markDesc));
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取没有锁定的空间地块", markDesc));
                    return;
                }
                db.BeginTransaction();
                this.ReportProgress(1, null);
                geoLandPercent = 99 / (double)shapeLandsOfStatus.Count;
                foreach (var land in shapeLandsOfStatus)
                {
                    var landGeo = land.Shape as YuLinTu.Spatial.Geometry;
                    //var areaDraw = ToolMath.CutNumericFormat((landGeo.Area()) * projectionUnit, 2);  //图形面积
                    //var areaNew = Math.Round((landGeo.Area()) * projectionUnit, 2);//四舍五入计算图形面积
                    // var areaDraw = Math.Round(landGeo.Area() * projectionUnit, 4);
                    var landgeoarea = landGeo.Area();
                    var areaDraw = ToolMath.SetNumericFormat(landgeoarea * 0.0015, metadata.ToAreaNumeric, metadata.ToAreaModule);
                    if (metadata.ToActualArea)
                    {
                        //把图形面积赋值给实测面积
                        land.ActualArea = areaDraw;
                    }
                    if (metadata.ToAwareArea)
                    {
                        //把图形面积赋值给确权面积
                        land.AwareArea = areaDraw;
                    }
                    var expad = land.LandExpand;
                    expad.MeasureArea = ToolMath.SetNumericFormat(landgeoarea, 2, metadata.ToAreaModule);
                    land.LandExpand = expad;
                    int upCount = landStation.Update(land);
                    if (upCount > 0)
                        successCount++;
                    this.ReportProgress((int)(percent + geoLandPercent * index), string.Format("{0}", markDesc + land.OwnerName));
                    index++;
                }
                this.ReportProgress(100, null);
                this.ReportInfomation(string.Format("{0}共{1}个空间地块,其中{2}个地块被锁定,成功初始化{3}个地块面积", markDesc, listGeoLand.Count,
                   listGeoLand.Count - shapeLandsOfStatus.Count, successCount));
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteError(this, "ContractLandAreaInitialTool(提交初始化面积数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 截取承包地块面积小数位
        /// </summary>
        /// <param name="metadata">任务参数(主要传递面积初始化界面上设置的一些参数)</param>
        /// <param name="listLand">待初始化的地块集合(具有空间数据的地块个数)</param>
        /// <param name="CurrentZone">当前初始化地块集合所在的地域</param>
        public void ContractLandAreaNumericFormatTool(TaskInitialAreaNumericFormatArgument metadata, List<ContractLand> listLand, Zone currentZone)
        {
            int index = 1;  //索引,记录具有空间数据的地块个数
            int successCount = 0;
            double landPercent = 0.0;  //百分比
            string markDesc = GetMarkDesc(currentZone);
            var db = DataBaseSource.GetDataBaseSource();
            try
            {
                if (currentZone == null)
                {
                    this.ReportError("未选择初始化数据的地域!");
                    return;
                }
                if (db == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return;
                }
                var landStation = db.CreateContractLandWorkstation();
                var landsOfStatus = landStation.GetCollection(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
                if (landsOfStatus == null || landsOfStatus.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取没有锁定的承包地块", markDesc));
                    return;
                }
                db.BeginTransaction();
                this.ReportProgress(1, null);
                landPercent = 99 / (double)landsOfStatus.Count;
                foreach (var land in landsOfStatus)
                {
                    if (metadata.ToActualArea)
                    {
                        //if (land.ActualArea == 0)
                        //{
                        //    this.ReportInfomation(string.Format("{0}的地块实测面积为0，不进行截取", land.OwnerName));
                        //    continue;
                        //}
                        //处理实测面积
                        land.ActualArea = ToolMath.SetNumericFormat(land.ActualArea, metadata.ToAreaNumeric, metadata.ToAreaModule);
                    }
                    if (metadata.ToAwareArea)
                    {
                        //if (land.AwareArea == 0)
                        //{
                        //    this.ReportInfomation(string.Format("{0}的地块确权面积为0，不进行截取", land.OwnerName));
                        //    continue;
                        //}
                        //处理确权面积
                        land.AwareArea = ToolMath.SetNumericFormat(land.AwareArea, metadata.ToAreaNumeric, metadata.ToAreaModule);
                    }
                    if (metadata.ToTableArea)
                    {
                        //if (land.TableArea == 0)
                        //{
                        //    this.ReportInfomation(string.Format("{0}的地块台账面积为0，不进行截取", land.OwnerName));
                        //    continue;
                        //}
                        //处理台账面积
                        land.TableArea = ToolMath.SetNumericFormat(land.TableArea.Value, metadata.ToAreaNumeric, metadata.ToAreaModule);
                    }
                    int upCount = landStation.Update(land);
                    if (upCount > 0)
                        successCount++;
                    this.ReportProgress((int)(percent + landPercent * index), string.Format("{0}", markDesc + land.OwnerName));
                    index++;
                }
                this.ReportProgress(100, null);
                this.ReportInfomation(string.Format("{0}共{1}个地块,其中{2}个地块被锁定,成功截取{3}个地块面积", markDesc, listLand.Count,
                   listLand.Count - landsOfStatus.Count, successCount));
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteError(this, "ContractLandAreaInitialTool(提交初始化面积数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 初始化承包地块是否基本农田
        /// </summary>
        /// <param name="metadata">任务参数(主要传递初始化界面上选择的Shape文件路径)</param>
        /// <param name="listLand">待做拓扑的地块集合</param>
        /// <param name="currentZone">待做拓扑的地块集合所在的地域</param>
        public void ContractLandIsFarmerInitialTool(TaskInitialIsFarmerArgument metadata, List<ContractLand> listGeoLand, Zone currentZone)
        {
            int index = 1;    //索引,用于记录当前做拓扑的地块索引
            int successCount = 0;
            double geoLandPercent = 0.0;   //百分比
            string markDesc = GetMarkDesc(currentZone);
            var db = DataBaseSource.GetDataBaseSource();
            try
            {
                if (db == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return;
                }
                string shapeName = Path.GetFileNameWithoutExtension(metadata.ShapeFileName);
                var dataSource = ProviderShapefile.CreateDataSource(Path.GetDirectoryName(metadata.ShapeFileName), false) as IDbContext;
                var dyQuery = new DynamicQuery(dataSource);
                var result = dyQuery.Get(null, shapeName, QuerySection.Property("Shape", "Shape"));
                IList protectAreaList = result.Result as IList;
                if (protectAreaList == null || protectAreaList.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn("未获得基本农田保护图斑数据!");
                    return;
                }
                List<Geometry> geoList = new List<Geometry>();
                foreach (var p in protectAreaList)
                {
                    Geometry geo = ObjectExtensions.GetPropertyValue(p, "Shape") as Geometry;
                    if (geo == null)
                        continue;
                    geoList.Add(geo);
                }
                if (geoList.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn("导入的图斑没有空间数据!");
                    return;
                }
                var landStation = db.CreateContractLandWorkstation();
                var landsOfStatus = landStation.GetCollection(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
                if (landsOfStatus == null || landsOfStatus.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取没有锁定的承包地块", markDesc));
                    return;
                }
                var shapeLandsOfStatus = landsOfStatus.FindAll(c => c.Shape != null);
                if (shapeLandsOfStatus == null || shapeLandsOfStatus.Count == 0)
                {
                    this.ReportProgress(0, string.Format("初始化{0}是否基本农田", markDesc));
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取没有锁定的空间地块", markDesc));
                    return;
                }
                db.BeginTransaction();
                this.ReportProgress(1, null);
                geoLandPercent = 99 / (double)shapeLandsOfStatus.Count;
                foreach (var land in shapeLandsOfStatus)
                {
                    var landGeo = land.Shape as YuLinTu.Spatial.Geometry;
                    bool overlap = geoList.Any(c => c.Intersects(landGeo));
                    if (overlap)
                    {
                        land.IsFarmerLand = true;   //标记为是基本农田
                    }
                    else
                    {
                        land.IsFarmerLand = false;  //标记为不是基本农田
                    }
                    int upCount = landStation.Update(land);
                    if (upCount > 0)
                        successCount++;
                    this.ReportProgress((int)(percent + geoLandPercent * index), string.Format("{0}", markDesc + land.OwnerName));
                    index++;
                }
                this.ReportProgress(100, null);
                this.ReportInfomation(string.Format("{0}共{1}个空间地块,其中{2}个地块被锁定,成功初始化{3}个地块基本农田信息", markDesc, listGeoLand.Count,
                   listGeoLand.Count - shapeLandsOfStatus.Count, successCount));
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                this.ReportError("初始化地块是否基本农田信息失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ContractLandIsFarmerInitialTool(提交初始化是否基本农田数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 初始化地块图幅编号
        /// </summary>
        /// <param name="metadata">初始化图幅编号任务参数</param>
        /// <param name="listGeoLand">当前地域下的所有空间地块集合</param>
        /// <param name="currentZone">当前地域</param>
        public void ContractLandImageNumberInitialTool(TaskInitialImageNumberArgument metadata, List<ContractLand> listGeoLand, Zone currentZone)
        {
            string markDesc = GetMarkDesc(currentZone);
            int scropeIndex = metadata.ScropeIndex;
            int scalerIndex = metadata.ScalerIndex;
            bool isUseYX = metadata.IsUseYX;
            bool isInitialAllnumber = metadata.IsInitialAllImageNumber;
            string imageNumber = string.Empty;
            int index = 1;
            int successCount = 0;
            double percent = 0.0;
            try
            {
                if (metadata.DbContext == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return;
                }
                var landStaion = metadata.DbContext.CreateContractLandWorkstation();
                var landsOfStatus = landStation.GetCollection(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
                if (landsOfStatus == null || landsOfStatus.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取没有锁定的承包地块", markDesc));
                    return;
                }
                var shapeLandsOfStatus = landsOfStatus.FindAll(c => c.Shape != null);
                if (shapeLandsOfStatus == null || shapeLandsOfStatus.Count == 0)
                {
                    this.ReportProgress(0, string.Format("初始化{0}图幅编号", markDesc));
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取没有锁定的空间地块", markDesc));
                    return;
                }
                metadata.DbContext.BeginTransaction();
                this.ReportProgress(1, null);
                percent = 99 / (double)shapeLandsOfStatus.Count;
                foreach (var geoLand in shapeLandsOfStatus)
                {
                    try
                    {
                        imageNumber = GetImgeNumber(geoLand, scropeIndex, scalerIndex, isUseYX, isInitialAllnumber);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    //geoLand.LandExpand.ImageNumber = imageNumber;
                    AgricultureLandExpand landExpand = geoLand.LandExpand;
                    landExpand.ImageNumber = imageNumber;
                    geoLand.LandExpand = landExpand;
                    int upCount = landStaion.Update(geoLand);
                    if (upCount > 0)
                        successCount++;
                    this.ReportProgress((int)(index * percent), string.Format("{0}", markDesc + geoLand.OwnerName));
                    index++;
                }
                this.ReportProgress(100, null);
                this.ReportInfomation(string.Format("{0}共{1}个空间地块,其中{2}个地块被锁定,成功初始化{3}个地块的图幅编号", markDesc, listGeoLand.Count,
               listGeoLand.Count - shapeLandsOfStatus.Count, successCount));
                metadata.DbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ContractLandImageNumberInitialTool(初始化地块图幅编号失败)", ex.Message + ex.StackTrace);
                this.ReportError("初始化地块图幅编号失败");
                metadata.DbContext.RollbackTransaction();
            }
        }

        /// <summary>
        /// 获取图幅编号
        /// </summary>
        private string GetImgeNumber(ContractLand geoLand, int scropeIndex, int scalerIndex, bool isUseYX, bool isInitialAllNumber)
        {
            string imageNumber = string.Empty;
            Spatial.Geometry landShape = geoLand.Shape;
            List<Coordinate> points = new List<Coordinate>();
            if (isInitialAllNumber)
            {
                points = GetSahapPoints(landShape.ToCoordinates().ToList(), scalerIndex);
            }
            else
            {
                points = landShape.ToCoordinates().ToList();
            }
            //var points = landShape.ToCoordinates();
            Spatial.Envelope envelop = new Envelope();
            envelop = landShape.GetEnvelope();
            if (envelop == null)
                return imageNumber;
            double evMinX = envelop.MinX;
            double evMaxY = envelop.MaxY;
            double landPointX = points[0].X;
            double landPointY = points[0].Y;
            double distance = Math.Pow((evMinX - points[0].X), 2) + Math.Pow((evMaxY - points[0].Y), 2);
            //double tempDistance = 0.0;
            //var cd = points.Min(c => (c.X - evMinX) * (c.X - evMinX) + (c.Y - evMaxY) * (c.Y - evMaxY));
            //points.Where(c => (c.X - evMinX) * (c.X - evMinX) + (c.Y - evMaxY) * (c.Y - evMaxY) == cd);
            ArrayList numberList = new ArrayList();
            foreach (var point in points)
            {
                string number = InitalizeImageNumber(point, scropeIndex, scalerIndex, isUseYX);
                if (!numberList.Contains(number))
                {
                    numberList.Add(number);
                }
                //tempDistance = Math.Pow((evMinX - point.X), 2) + Math.Pow((evMaxY - point.Y), 2);
                //if (tempDistance <= distance)
                //{
                //    distance = tempDistance;
                //    landPointX = point.X;
                //    landPointY = point.Y;
                //}
            }

            if (!isInitialAllNumber)
            {
                var epoints = envelop.ToGeometry().ToCoordinates();
                foreach (var point in epoints)
                {
                    string number = InitalizeImageNumber(point, scropeIndex, scalerIndex, isUseYX);
                    if (!numberList.Contains(number))
                    {
                        numberList.Add(number);
                    }
                }
            }
            //Coordinate cd = new Coordinate(envelop.MinX, envelop.MaxY);
            //string pointNumber = InitalizeImageNumber(cd, scropeIndex, scalerIndex, isUseYX);
            //if (!numberList.Contains(pointNumber))
            //{
            //    numberList.Add(pointNumber);
            //}

            foreach (string number in numberList)
            {
                imageNumber += number;
                imageNumber += "、";
            }
            imageNumber = imageNumber.TrimEnd('、');//.Substring(0, imageNumber.Length - 1);
            numberList = null;
            return imageNumber;
        }

        /// <summary>
        /// 获取图形点，这里是中间打断
        /// </summary>
        /// <param name="points">图形原有的点</param>
        /// <param name="scalerIndex">比例尺规格</param>
        /// <returns></returns>
        public List<Coordinate> GetSahapPoints(List<Coordinate> points, int scalerIndex)
        {
            List<Coordinate> newPoints = new List<Coordinate>();
            Coordinate firstPoint = new Coordinate();
            Coordinate secondePoint = new Coordinate();
            double distance = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (!newPoints.Contains(points[i]))
                {
                    newPoints.Add(points[i]);
                }
                if (i + 1 == points.Count)
                {
                    firstPoint = points[i];
                    secondePoint = points[0];
                    distance = Distance(points[i], points[0]);
                }
                else
                {
                    firstPoint = points[i];
                    secondePoint = points[i + 1];
                    distance = Distance(points[i], points[i + 1]);
                }
                switch (scalerIndex)
                {
                    case 2:
                        //比例尺是1:500
                        if (distance > 250)
                        {
                            AddPoint(firstPoint, secondePoint, 250);
                            newPoints.AddRange(addPointList);
                            addPointList.Clear();
                        }
                        break;

                    case 1:
                        //比例尺是1:1000
                        if (distance > 500)
                        {
                            AddPoint(firstPoint, secondePoint, 500);
                            newPoints.AddRange(addPointList);
                            addPointList.Clear();
                        }
                        break;

                    case 0:
                        //比例尺是1:2000
                        if (distance > 1000)
                        {
                            AddPoint(firstPoint, secondePoint, 1000);
                            newPoints.AddRange(addPointList);
                            addPointList.Clear();
                        }
                        break;

                    default:
                        break;
                }
            }
            return newPoints;
        }

        /// <summary>
        /// 计算2个点的距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double Distance(Coordinate p1, Coordinate p2)
        {
            double width = Math.Abs(p1.X - p2.X);
            double height = Math.Abs(p1.Y - p2.Y);
            double result = (width * width) + (height * height);
            result = Math.Sqrt(result);//根号
            return result;
        }

        private List<Coordinate> addPointList = new List<Coordinate>();

        /// <summary>
        /// 增加2个点之间新增加的点
        /// </summary>
        /// <param name="p1">第一个点</param>
        /// <param name="p2">第二个点</param>
        /// <param name="scale">比例尺</param>
        public void AddPoint(Coordinate p1, Coordinate p2, double scale)
        {
            double width = Math.Abs(p1.X - p2.X);
            double height = Math.Abs(p1.Y - p2.Y);
            double dis = Distance(p1, p2);
            if (dis < scale)
            {
                return;
            }
            double newWidth = scale * width / dis;
            double px, py;
            if (p1.X - p2.X < 0)
            {
                px = p1.X + newWidth;
            }
            else
            {
                px = p1.X - newWidth;
            }
            double newHeight = scale * height / dis;
            if (p1.Y - p2.Y < 0)
            {
                py = p1.Y + newHeight;
            }
            else
            {
                py = p1.Y - newHeight;
            }
            Coordinate cd = new Coordinate();
            cd.X = px;
            cd.Y = py;
            if (!addPointList.Contains(cd))
            {
                addPointList.Add(cd);
            }
            AddPoint(cd, p2, scale);
        }

        /// <summary>
        /// 初始化点图幅号
        /// </summary>
        /// <param name="point"></param>
        /// <param name="scropeIndex"></param>
        /// <param name="scalerIndex"></param>
        /// <param name="isUseYX"></param>
        /// <returns></returns>
        private string InitalizeImageNumber(Coordinate point, int scropeIndex, int scalerIndex, bool isUseYX)
        {
            double landPointX = 0.001 * point.X;
            double landPointY = 0.001 * point.Y;

            double integerX = Math.Truncate(landPointX);
            double integerY = Math.Truncate(landPointY);
            double decimalX = landPointX - integerX;
            double decimalY = landPointY - integerY;
            double imageValueX = integerX + InitialDecimalValue(decimalX, scalerIndex);  //整数+小数
            double imageValueY = integerY + InitialDecimalValue(decimalY, scalerIndex);  //整数+小数
            string strX = string.Empty;
            string strY = string.Empty;
            if (scalerIndex == 2)
            {
                strX = ToolMath.SetNumbericFormat(imageValueX.ToString(), 2);
                strY = ToolMath.SetNumbericFormat(imageValueY.ToString(), 2);
            }
            else
            {
                strX = ToolMath.SetNumbericFormat(imageValueX.ToString(), 1);
                strY = ToolMath.SetNumbericFormat(imageValueY.ToString(), 1);
            }
            string imageNumberX = strX;
            string imageNumberY = strY;
            string imageNumber = "";
            if (isUseYX)
            {
                imageNumber = imageNumberX + "-" + imageNumberY;
            }
            else
            {
                imageNumber = imageNumberY + "-" + imageNumberX;
            }
            return imageNumber;
        }

        /// <summary>
        /// 初始化小数值
        /// </summary>
        private double InitialDecimalValue(double digit, int scalerIndex)
        {
            double decimalValue = 0.0;
            switch (scalerIndex)
            {
                case 2:
                    //比例尺是1:500
                    decimalValue = Initial25X25Decimal(digit);
                    break;

                case 1:
                    //比例尺是1:1000
                    decimalValue = Initial50X50Decimal(digit);
                    break;

                case 0:
                    //比例尺是1:2000
                    decimalValue = 0.0;
                    break;

                default:
                    break;
            }
            return decimalValue;
        }

        /// <summary>
        /// 初始化1:500小数后缀值
        /// </summary>
        private double Initial25X25Decimal(double digit)
        {
            double decimalValue = 0.0;
            if (digit >= 0.0 && digit <= 0.25)
            {
                decimalValue = 0.00;
            }
            else if (digit >= 0.25 && digit <= 0.50)
            {
                decimalValue = 0.25;
            }
            else if (digit >= 0.50 && digit <= 0.75)
            {
                decimalValue = 0.50;
            }
            else
            {
                decimalValue = 0.75;
            }
            return decimalValue;
        }

        /// <summary>
        /// 初始化1:1000小数后缀值
        /// </summary>
        /// <param name="digit"></param>
        /// <returns></returns>
        private double Initial50X50Decimal(double digit)
        {
            double decimalValue = 0.0;
            if (digit >= 0.0 && digit <= 0.50)
            {
                decimalValue = 0.0;
            }
            else
            {
                decimalValue = 0.5;
            }
            return decimalValue;
        }

        #endregion 工具之初始数据

        #region 辅助方法

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone)
        {
            Zone parent = GetParent(zone);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parent.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent);
                excelName = parentTowm.Name + parent.Name + zone.Name;
            }
            return excelName;
        }

        /// <summary>
        /// 查找，到村的描述
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="Database"></param>
        /// <returns></returns>
        private string GetVillageLevelDesc(Zone zone)
        {
            Zone parent = GetParent(zone);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parent.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent);
                excelName = parentTowm.Name + parent.Name;
            }
            return excelName;
        }

        /// <summary>
        /// 初始化权属地块类型
        /// </summary>
        private string InitalizeLandRightName()
        {
            string templateName = "承包地田块";
            switch (VirtualType)
            {
                case eVirtualType.Land:
                    templateName = "承包地田块";
                    break;

                case eVirtualType.Yard:
                    templateName = "建设用地";
                    break;

                case eVirtualType.House:
                    templateName = "房屋";
                    break;

                case eVirtualType.Wood:
                    templateName = "林地";
                    break;

                default:
                    break;
            }
            return templateName;
        }

        /// <summary>
        /// 初始化权属类型
        /// </summary>
        private string InitalizeRightType()
        {
            string templateName = "农村土地承包经营权";
            switch (VirtualType)
            {
                case eVirtualType.Land:
                    templateName = "农村土地承包经营权";
                    break;

                case eVirtualType.Yard:
                    templateName = "集体建设用地使用权";
                    break;

                case eVirtualType.House:
                    templateName = "房屋所有权";
                    break;

                case eVirtualType.Wood:
                    templateName = "林权";
                    break;

                default:
                    break;
            }
            return templateName;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public PublicityConfirmDefine InitalizeFamilyDefine()
        {
            PublicityConfirmDefine define = null;
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\Config\" + "PublicityConfirmDefine.xml";
            if (File.Exists(filePath))
            {
                define = ToolSerialization.DeserializeXml(filePath, typeof(PublicityConfirmDefine)) as PublicityConfirmDefine;
            }
            if (define == null)
            {
                define = new PublicityConfirmDefine();
            }
            //if (!define.ContractorTypeValue)
            //{
            //    define.ColumnCount -= 1;
            //}
            return define;
        }

        /// <summary>
        /// 获取地域集合
        /// </summary>
        public List<Zone> GetParentZone(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        /// <summary>
        /// 获取地域下所有子级地域集合
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public List<Zone> GetChildrenZone(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_CHILDREN_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        /// <summary>
        /// 获取发包方
        /// </summary>
        public CollectivityTissue GetTissue(Guid id)
        {
            if (id == null)
            {
                return null;
            }
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = this.dbContext;
            arg.Parameter = id;
            arg.Name = SenderMessage.SENDER_GET_ID;
            TheBns.Current.Message.Send(this, arg);
            CollectivityTissue tissue = arg.ReturnValue as CollectivityTissue;
            return tissue;
        }

        /// <summary>
        /// 辅助判断方法
        /// </summary>
        public bool CanContinue()
        {
            if (landStation == null)
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

        /// <summary>
        /// 获取确股地块
        /// </summary>
        /// <param name="currentZone"></param>
        /// <returns></returns>
        public List<ContractLand> GetStockRightLand(Zone currentZone)
        {
            var landStocks = GetLandCollection(currentZone.FullCode).Where(o => o.IsStockLand).ToList();//鑾峰彇纭偂鍦板潡
            if (landStocks == null || landStocks.Count == 0)
                return new List<ContractLand>();
            var landStockAreaAll = landStocks.Sum(o => o.ActualArea).ToString("N2");
            landStocks.ForEach(o => o.Comment = "共有宗地总面积" + landStockAreaAll);
            return landStocks;
        }

        #endregion 辅助方法

        #endregion Methods
    }
}