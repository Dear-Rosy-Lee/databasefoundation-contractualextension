/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.IO;
using System.Collections;
using YuLinTu.Library.Office;
using System.Text.RegularExpressions;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using NPOI.SS.Formula.Functions;
using System.Data.SqlTypes;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入地域数据类
    /// </summary>
    [Serializable]
    public class ImportZoneData : Task
    {
        #region Fields

        private ToolProgress toolProgress;//进度工具
        private List<Zone> zones;//地域集合
        private SortedList parentZoneList;//父级集合
        private List<Zone> zoneList;//地域列表
        private List<CollectivityTissue> Tissues;
        private ArrayList errorList;//错误列表
        private int addCount;//添加数
        private int updateCount;//更新数
        private MultiObjectArg multiArg;

        #region ErrorMessage

        private string ZonesError = "Excel数据错误,其中没有信息";
        private string ZoneError2 = "Excel数据错误，编号:{0}应为纯数字！";
        private string ZoneNotParentError = "没有发现{0}的父级地域";
        private string ZoneError = "{0}的信息错误";
        private string EntitysErrorInfo = "文件格式错误或没有数据!";
        private string IdErrorInfo = "{0}的地域ID为Null";
        private string NameErrorInfo = "地域名称或全称为空!";
        private string CodeErrorInfo = "{0}的地域编码或全编码为空!";
        private string LevelErrorInfo = "{0}的地域级别错误!";
        private string ZoneCodeAndFullCodeError = "{0}的地域编码与全编码不匹配!";
        private string ZoneUpLevelCodeError = "{0}的上级编码错误!";
        private string ZoneNameError = "{0}的名称错误!";
        private string ZoneCodeError = "{0}的地域编码错误!";
        private string ZoneFullCodeError = "{0}的地域全编码错误!";
        private string ZoneLevelError = "{0}与父级{1}的地域级别不匹配!";

        #endregion ErrorMessage

        #endregion Fields

        #region Propertys

        /// <summary>
        /// 数据库实例(开启事物)
        /// </summary>
        public IDbContext DataInstance { get; set; }

        /// <summary>
        /// 获取的地域数据
        /// </summary>
        public List<Zone> ZoneList
        { get { return zones; } }

        /// <summary>
        /// 导入完成参数
        /// </summary>
        public MultiObjectArg MultiArg
        { get { return multiArg; } }

        /// <summary>
        /// 工作站(执行数据插入修改)
        /// </summary>
        public IZoneWorkStation Station { get; set; }

        #endregion Propertys

        #region Ctor

        public ImportZoneData()
        {
            parentZoneList = new SortedList();
            zoneList = new List<Zone>();
            toolProgress = new ToolProgress();
        }

        #endregion Ctor

        #region Methods - ReadInformation

        /// <summary>
        /// 读取地域信息
        /// </summary>
        public void ReadZoneInformation(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            ExcelReaderZone readerZone = new ExcelReaderZone();
            try
            {
                readerZone.Open(fileName);
                zoneList = readerZone.GetZoneValue();
                Tissues = readerZone.GetTissuesValue();
                errorList = readerZone.ErrorList;
            }
            catch (Exception e)
            {
                this.ReportError(string.Format("{0}", e.Message.ToString()));
                readerZone.Dispose();
                return;
            }
            finally
            {
                readerZone.Dispose();
            }
            if (zoneList == null || zoneList.Count < 1)
            {
                this.ReportError(ZonesError);
            }
        }

        #endregion Methods - ReadInformation

        #region Methods - Initalzie

        /// <summary>
        /// 初始化地域列表
        /// </summary>
        public bool InitalizeZoneList()
        {
            zones = new List<Zone>();
            var list = zoneList.OrderBy(t => t.FullCode.Length).ToList();
            InitalizeZoneGroup(list);
            InitalizeZoneParent();
            return true;
        }

        /// <summary>
        /// 分组初始化
        /// </summary>
        private void InitalizeZoneGroup(List<Zone> zlist)
        {
            string fullcode = string.Empty;
            string name = string.Empty;
            for (int i = 0; i < zlist.Count; i++)
            {
                fullcode = zlist[i].FullCode;
                //if (string.IsNullOrEmpty(fullcode.Trim()))
                //{
                //    this.ReportError(string.Format("第{0}行数据地域编码为空!", i));
                //    return false;
                //}
                name = zlist[i].Name;
                //if (string.IsNullOrEmpty(name.Trim()))
                //{
                //    this.ReportError(string.Format("第{0}行数据地域名称为空!", i));
                //    return false;
                //}
                Zone zone = InitzlieZoneInformation(zlist[i]);
                if (zone != null)
                {
                    zones.Add(zone);
                }
            }
        }

        /// <summary>
        /// 初始化地域信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullcode"></param>
        /// <returns></returns>
        private Zone InitzlieZoneInformation(Zone zone)
        {
            zone.ID = Guid.NewGuid();
            zone.Name = ToolString.ExceptSpaceString(zone.Name);
            zone.FullCode = ToolString.ExceptSpaceString(zone.FullCode);
            if (Regex.Replace(zone.FullCode, @"[\d]", "").Length > 0)
            {
                this.ReportError(string.Format(ZoneError2, zone.FullCode));
                return null;
            }
            toolProgress.DynamicProgress();
            switch (zone.FullCode.Length)
            {
                case 2:
                    if (zone.FullCode != "86")
                    {
                        zone.UpLevelCode = "86";
                        zone.Level = eZoneLevel.Province;
                        zone.UpLevelName = "中国";
                    }
                    else
                    {
                        zone.Level = eZoneLevel.State;
                        zone.UpLevelCode = string.Empty;
                    }
                    zone.Code = zone.FullCode;
                    zone.FullCode = zone.Code;
                    zone.FullName = zone.Name;
                    parentZoneList.Add(zone.FullCode, zone.FullName);
                    return zone;

                case 4:
                    zone.Level = eZoneLevel.City;
                    zone.Code = zone.FullCode.Substring(2, 2);
                    zone.UpLevelCode = zone.FullCode.Substring(0, 2);
                    break;

                case 6:
                    zone.Level = eZoneLevel.County;
                    zone.Code = zone.FullCode.Substring(4, 2);
                    zone.UpLevelCode = zone.FullCode.Substring(0, 4);
                    break;

                case 9:
                    zone.Level = eZoneLevel.Town;
                    zone.Code = zone.FullCode.Substring(6, 3);
                    zone.UpLevelCode = zone.FullCode.Substring(0, 6);
                    break;

                case 12:
                    zone.Level = eZoneLevel.Village;
                    zone.Code = zone.FullCode.Substring(9, 3);
                    zone.UpLevelCode = zone.FullCode.Substring(0, 9);
                    break;

                case 14:
                    zone.Level = eZoneLevel.Group;
                    zone.Code = zone.FullCode.Substring(12, 2);
                    zone.UpLevelCode = zone.FullCode.Substring(0, 12);
                    zone.Name = zone.Name.Remove(0, zone.Name.IndexOf("镇") + 1);
                    break;

                case 16:
                    zone.Level = eZoneLevel.Group;
                    zone.Code = zone.FullCode.Substring(14, 2);
                    zone.FullCode = ZoneHelper.ChangeCodeShort(zone.FullCode);
                    zone.UpLevelCode = zone.FullCode.Substring(0, 12);
                    var count1 = zone.Name.IndexOf("村") + 1;
                    var count2 = zone.Name.IndexOf("区") + 1;
                    if (count1 > 0)
                    {
                        zone.Name = zone.Name.Remove(0, count1);
                    }
                    else
                    {
                        zone.Name = zone.Name.Remove(0, count2);
                    }
                    break;

                default:
                    zone.Level = eZoneLevel.Group;
                    break;
            }
            if (string.IsNullOrEmpty(zone.FullCode))
            {
                return null;
            }
            if (string.IsNullOrEmpty(zone.UpLevelCode))
            {
                this.ReportError(string.Format("{0}的地域编码不符合行政区域编码规则!", zone.Name));
                return null;
            }
            if (parentZoneList[zone.UpLevelCode] == null || parentZoneList[zone.UpLevelCode].ToString() == "")
            {
                this.ReportError(string.Format(ZoneNotParentError, zone.FullCode));
                return null;
            }
            zone.FullName = parentZoneList[zone.UpLevelCode] + zone.Name;
            bool isRight = CheckZoneIsNull(zone);
            if (!isRight)
            {
                parentZoneList.Add(zone.FullCode, zone.FullName);
            }
            else
            {
                this.ReportError(string.Format(ZoneError, zone.FullCode));
                return null;
            }
            return zone;
        }

        /// <summary>
        /// 检查地域是否为空值
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private bool CheckZoneIsNull(Zone zone)
        {
            if (string.IsNullOrEmpty(zone.Code) || zone.Code.Length < 1)
                return true;
            if (string.IsNullOrEmpty(zone.FullCode) || zone.FullCode.Length < 1)
                return true;
            if (string.IsNullOrEmpty(zone.FullName) || zone.Code.Length < 1)
                return true;
            if (zone.ID == null || zone.ID == Guid.Empty)
                return true;
            if ((int)zone.Level < 1 || (int)zone.Level > 7)
                return true;
            if (string.IsNullOrEmpty(zone.Name) || zone.Name.Length < 1)
                return true;
            if (string.IsNullOrEmpty(zone.UpLevelCode) || zone.UpLevelCode.Length < 1)
                return true;
            return false;
        }

        /// <summary>
        /// 初始化地域父级信息
        /// </summary>
        private void InitalizeZoneParent()
        {
            var china = ZoneHelper.China;
            foreach (Zone item in zones)
            {
                if (item.FullCode == china.FullCode)
                {
                    continue;
                }
                item.UpLevelName = GetParentZoneName(item.UpLevelCode);
                if (item.UpLevelName == "" && item.Level == eZoneLevel.Province)
                {
                    item.UpLevelName = china.Name;
                }
                if (item.UpLevelName == "ErrorToUpLevelNameIsNull")
                {
                    this.ReportError(string.Format("编码为{0}的地域信息在表中找不到其上级行政区域!", item.Code));
                    break;
                }
            }
        }

        /// <summary>
        /// 获取父级地域名称
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        private string GetParentZoneName(string parentCode)
        {
            if (string.IsNullOrEmpty(parentCode))
                return "ErrorToUpLevelNameIsNull";
            object name = parentZoneList[parentCode];
            if (name == null)
            {
                if (parentCode == "86")
                {
                    return "";
                }
                else
                {
                    this.ReportError(string.Format("编码为{0}的地域信息在表中找不到其上级行政区域!", parentCode));
                    return "ErrorToUpLevelNameIsNull";
                }
            }
            return name.ToString();
        }

        #endregion Methods - Initalzie

        #region Methods - Check

        /// <summary>
        /// 行政区域信息检查
        /// </summary>
        /// <returns></returns>
        public bool ZoneInformtionCheck()
        {
            if (errorList != null && errorList.Count != 0)
            {
                foreach (string information in errorList)
                {
                    this.ReportError(information);
                }
                errorList.Clear();
                return false;
            }
            if (zones == null || zones.Count < 1)
            {
                this.ReportError(EntitysErrorInfo);
                return false;
            }
            bool result = true;
            foreach (Zone zone in zones)
            {
                if (zone.Name.Length < 1 || zone.FullName.Length < 1)
                {
                    this.ReportError(NameErrorInfo);
                    result = false;
                }
                if (zone.ID == Guid.Empty)
                {
                    this.ReportError(string.Format(IdErrorInfo, zone.Name));
                    result = false;
                }
                if (zone.Code.Length < 1 || zone.FullCode.Length < 1)
                {
                    this.ReportError(string.Format(CodeErrorInfo, zone.Name));
                    result = false;
                }
                if ((int)zone.Level < 0 || (int)zone.Level > 7)
                {
                    this.ReportError(string.Format(LevelErrorInfo, zone.Name));
                    result = false;
                }
                if (!CheckZoneIsError(zone))
                {
                    result = false;
                }
            }
            foreach (Zone zone in zones)
            {
                if ((int)zone.Level >= 6)
                {
                    continue;
                }
                Zone upZone = zoneList.Find(t => t.FullCode == zone.UpLevelCode);
                if (upZone != null)
                {
                    if (((int)upZone.Level - (int)zone.Level) != 1)
                        return this.ReportError(string.Format(ZoneLevelError, zone.FullName, upZone.FullName));
                }
                else
                {
                    if (zone.FullCode != ZoneHelper.China.FullCode && Station.Get(zone.UpLevelCode) == null)
                        return this.ReportError(string.Format(ZoneUpLevelCodeError, zone.FullName));
                }
            }
            return result;
        }

        /// <summary>
        /// 检查地域是否错误
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private bool CheckZoneIsError(Zone zone)
        {
            switch (zone.FullCode.Length)
            {
                #region 国家级、省级验证

                case 2:
                    if (zone.Level != eZoneLevel.Province && zone.Level != eZoneLevel.State)
                        return this.ReportError(string.Format(LevelErrorInfo, zone.Name));

                    if (zone.Level == eZoneLevel.Province)
                    {
                        if (zone.Code != zone.FullCode)
                            return this.ReportError(string.Format(ZoneCodeAndFullCodeError, zone.Name));
                        if (zone.UpLevelCode != "86")
                            return this.ReportError(string.Format(ZoneUpLevelCodeError, zone.Name));
                    }
                    if (zone.Level == eZoneLevel.State)
                    {
                        if (zone.Name != "中国" && zone.Name != "中华人民共和国")
                            return this.ReportError(string.Format(ZoneNameError, zone.Name));

                        if (zone.Code != "86")
                            return this.ReportError(string.Format(ZoneCodeError, zone.Name));

                        if (zone.FullCode != "86")
                            return this.ReportError(string.Format(ZoneFullCodeError, zone.Name));

                        zone.UpLevelCode = string.Empty;
                    }
                    break;

                #endregion 国家级、省级验证

                #region 市级验证

                case 4:
                    if (zone.Level != eZoneLevel.City)
                        return this.ReportError(string.Format(LevelErrorInfo, zone.Name));

                    if (int.Parse(zone.Code) != int.Parse(zone.FullCode.Substring(2, 2)) && zone.Code.Length > 2)
                        return this.ReportError(string.Format(ZoneCodeAndFullCodeError, zone.Name));

                    if (zone.UpLevelCode != zone.FullCode.Substring(0, 2))
                        return this.ReportError(string.Format(ZoneUpLevelCodeError, zone.Name));

                    break;

                #endregion 市级验证

                #region 区县级验证

                case 6:
                    if (zone.Level != eZoneLevel.County)
                        return this.ReportError(string.Format(LevelErrorInfo, zone.Name));

                    if (int.Parse(zone.Code) != int.Parse(zone.FullCode.Substring(4, 2)) && zone.Code.Length > 2)
                        return this.ReportError(string.Format(ZoneCodeAndFullCodeError, zone.Name));

                    if (zone.UpLevelCode != zone.FullCode.Substring(0, 4))
                        return this.ReportError(string.Format(ZoneUpLevelCodeError, zone.Name));

                    break;

                #endregion 区县级验证

                #region 乡镇级验证

                case 9:
                    if (zone.Level != eZoneLevel.Town)
                        this.ReportError(string.Format(LevelErrorInfo, zone.Name));

                    if (int.Parse(zone.Code) != int.Parse(zone.FullCode.Substring(6, 3)) && zone.Code.Length > 3)
                        this.ReportError(string.Format(ZoneCodeAndFullCodeError, zone.Name));

                    if (zone.UpLevelCode != zone.FullCode.Substring(0, 6))
                        this.ReportError(string.Format(ZoneUpLevelCodeError, zone.Name));

                    break;

                #endregion 乡镇级验证

                #region 村级验证

                case 12:
                    if (zone.Level != eZoneLevel.Village)
                        this.ReportError(string.Format(LevelErrorInfo, zone.Name));

                    if (int.Parse(zone.Code) != int.Parse(zone.FullCode.Substring(9, 3)) && zone.Code.Length > 3)
                        this.ReportError(string.Format(ZoneCodeAndFullCodeError, zone.Name));

                    if (zone.UpLevelCode != zone.FullCode.Substring(0, 9))
                        this.ReportError(string.Format(ZoneUpLevelCodeError, zone.Name));

                    break;

                #endregion 村级验证

                #region 组级验证

                case 14:
                    if (zone.Level != eZoneLevel.Group)
                        this.ReportError(string.Format(LevelErrorInfo, zone.Name));

                    if (int.Parse(zone.Code) != int.Parse(zone.FullCode.Substring(12, 2)) && zone.Code.Length > 2)
                        this.ReportError(string.Format(ZoneCodeAndFullCodeError, zone.Name));

                    if (zone.UpLevelCode != zone.FullCode.Substring(0, 12))
                        this.ReportError(string.Format(ZoneUpLevelCodeError, zone.Name));

                    break;

                #endregion 组级验证

                #region 默认错误

                default:
                    return this.ReportError(zone.FullName + "的地域编码不符合行政区域编码规则");

                    #endregion 默认错误
            }
            return true;
        }

        #endregion Methods - Check

        #region Methods - Import

        /// <summary>
        /// 导入实体
        /// </summary>
        public void InportZoneEntity()
        {
            toolProgress.InitializationPercent(zones.Count, 80, 20);
            try
            {
                DataInstance.OpenConnection();
                DataInstance.BeginTransaction();
                InportZoneToDatabase(zones);
                DataInstance.CommitTransaction();
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                DataInstance.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "InportZoneEntity", ex.Message + ex.StackTrace);
                throw new Exception("连接数据库失败,请检查数据库连接路径是否有效!");
            }
            catch (SystemException ex)
            {
                DataInstance.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "InportZoneEntity", ex.Message + ex.StackTrace);
            }
            finally
            {
                DataInstance.CloseConnection();
            }
            this.ReportInfomation(string.Format("当前表中共有{0}条数据,成功导入{1}条记录", zones.Count, addCount + updateCount));//成功更新{2}条记录,
        }

        /// <summary>
        /// 导入地域数据到数据库中
        /// </summary>
        /// <param name="zones"></param>
        /// <returns></returns>
        private bool InportZoneToDatabase(List<Zone> zones)
        {
            try
            {
                multiArg = new MultiObjectArg();
                multiArg.ParameterA = zones;
                List<Zone> dbZoneList = new List<Zone>();
                ContainerFactory factroy = new ContainerFactory(DataInstance);
                IZoneWorkStation station = factroy.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
                int index = 1;
                double step = 80 / (double)zones.Count;
                var vpStation = DataInstance.CreateVirtualPersonStation<LandVirtualPerson>();
                var landStation = DataInstance.CreateContractLandWorkstation();
                var vps = new List<VirtualPerson>();
                var lands = new List<ContractLand>();
                foreach (Zone zone in zones)
                {
                    string alertName = GetZoneName(zone, zones);
                    Zone desZone = station.Get(zone.FullCode);//取数据库中的地域
                    if (desZone != null)
                    {
                        dbZoneList.Add(desZone);
                    }
                    if (AddZone(zone, desZone))
                    {
                        if (zone.AliasCode.IsNotNullOrEmpty() && zone.AliasName.IsNotNullOrEmpty())
                        {
                            var oldNumber = zone.AliasCode.Split('/');
                            for (int i = 0; i < oldNumber.Length; i++)
                            {
                                var oldCBFs = vpStation.GetByZoneCode(oldNumber[i]);
                                oldCBFs.ForEach(t => { t.OldZoneCode = t.ZoneCode; t.ZoneCode = zone.FullCode; vps.Add(t); });
                                var oldLands = landStation.GetShapeCollection(oldNumber[i], eLevelOption.SelfAndSubs);
                                oldLands.ForEach(t => { t.ZoneCode = zone.FullCode; lands.Add(t); });
                            }
                            var senderStation = DataInstance.CreateSenderWorkStation();
                            var sender = senderStation.GetByCode(zone.FullCode);
                            if (sender == null)
                            {
                                var newsender = new CollectivityTissue();
                                newsender.Name = GetNmaeToCounty(zone);
                                newsender.Code = zone.FullCode;
                                senderStation.Add(newsender);
                            }
                        }
                        this.ReportProgress(20 + (int)(step * index), "导入" + alertName);
                        index++;
                    }
                }
                vps.ForEach(x => { vpStation.UpdateZoneCode(x); });
                lands.ForEach(x => { landStation.UpdateZoneCode(x); });
                multiArg.ParameterB = dbZoneList;
                GC.Collect();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ClearExistFiles", ex.Message + ex.StackTrace);
            }
            return true;
        }

        /// <summary>
        /// 获取地域名称
        /// </summary>
        private string GetZoneName(Zone zone, List<Zone> zones)
        {
            string nameString = zone.Name;
            Zone z = zones.Find(t => t.FullCode == zone.UpLevelCode);
            if (z != null)
            {
                nameString = z.Name + nameString;
            }
            return nameString;
        }

        /// <summary>
        /// 添加地域信息
        /// </summary>
        /// <param name="zone">当前地域</param>
        /// <param name="dbZone">数据库中地域</param>
        /// <returns></returns>
        private bool AddZone(Zone zone, Zone desZone)
        {
            zone.Code = ToolString.ExceptSpaceString(zone.Code);
            zone.FullCode = ToolString.ExceptSpaceString(zone.FullCode);
            zone.Name = ToolString.ExceptSpaceString(zone.Name);
            zone.FullName = ToolString.ExceptSpaceString(zone.FullName);
            zone.CreateTime = DateTime.Now;
            zone.CreateUser = "Admin";
            zone.LastModifyTime = DateTime.Now;
            
            if (desZone != null)
            {
                zone.ID = desZone.ID;
                int upCnt = Station.Update(zone);
                if (upCnt < 1)
                {
                    string errorInfo = string.Format("{0} 在写入数据库时出错", zone.FullName);
                    return this.ReportError(errorInfo);
                }
                else
                {
                    updateCount++;
                }
            }
            else
            {
                int addCnt = Station.Add(zone);
                if (addCnt < 1)
                {
                    string errorInfo = string.Format("{0} 在写入数据库时出错", zone.FullName);
                    this.ReportError(errorInfo);
                }
                else
                {
                    addCount++;
                }
            }
            return true;
        }
        private string GetNmaeToCounty(Zone zone)
        {
            var zoneStation = DataInstance.CreateZoneWorkStation();
            var name = zoneStation.GetZoneNameByLevel(zone.FullCode, eZoneLevel.County) + zoneStation.GetTownZoneName(zone);
            
            return name;
        }
        #endregion Methods - Import
    }
}