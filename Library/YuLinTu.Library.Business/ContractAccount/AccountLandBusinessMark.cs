/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账地块业务处理
    /// </summary>
    public class AccountLandBusinessMark : Task
    {
        #region Fields

        private IDbContext dbContext;       
        private IContractLandMarkWorkStation landStation;//承包台账地块业务逻辑层
        private IVirtualPersonWorkStation<LandVirtualPerson> tableStation;  //承包台账(承包方)Station
        private IVirtualPersonWorkStation<LandVirtualPerson> landVirtualPersonStation;
        #endregion

        #region Properties

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
        /// 地块图斑导入设置实体
        /// </summary>
        public ImportZDBZDefine ImportLandShapeInfoDefine =
            ImportZDBZDefine.GetIntence();


        /// <summary>
        /// 读取的shp所有字段名称
        /// </summary>
        public List<KeyValue<int, string>> shapeAllcolNameList { get; set; }

        /// <summary>
        /// 空间参考系
        /// </summary>
        public YuLinTu.Spatial.SpatialReference shpRef { get; set; }
        #endregion

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db"></param>
        public AccountLandBusinessMark(IDbContext db)
        {
            dbContext = db;
            landStation = db == null ? null : db.CreateContractLandMarkWorkstation();
            landVirtualPersonStation = db == null ? null : db.CreateVirtualPersonStation<LandVirtualPerson>();
            tableStation = db == null ? null : db.CreateVirtualPersonStation<LandVirtualPerson>();
        }

        #endregion

        #region Methods

 

        #region 数据处理

    
    
        /// <summary>
        /// 根据地域编码和匹配等级获取承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="option">匹配等级</param>
        /// <returns>地块集合</returns>
        public List<ContractLandMark> GetCollection(string zoneCode, eLevelOption option)
        {
            List<ContractLandMark> list = null;
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
        /// 逐条更新承包地块信息
        /// </summary>
        /// <param name="secondLand">承包地块</param>
        public int ModifyLand(ContractLandMark secondLand)
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

  

        #endregion

        #region 导入数据      

        /// <summary>
        /// 导入地块图斑数据shape等信息-最小以组为单位
        /// </summary>
        /// <param name="shapeDataList">读取的shp数据</param>
        /// <param name="currentPercent">当前的进度</param>
        /// <param name="indexPercent">总进度分区</param>
        public int ImportLandShapeDataInfo(Zone zone, IList shapeDataList, double currentPercent, double indexPercent = 0.0)
        {
            //导出个数统计
            int importCount = 0;
            importCount = shapeDataList.Count;
            //RefreshMapControlSpatialUnit();
            //获取下拉列表所有字段，包括未选的，与地块配置实体属性顺序保持一致性
            List<string> allGetSelectColList = getSelectColList();
            ContractLandMark resLand = null;
            ContractLandMark addZoneLand = null;
            string zoneNameInfo = GetMarkDesc(zone);

            var targetSpatialReference = dbContext.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(ContractLandMark)).Schema,
                ObjectContext.Create(typeof(ContractLandMark)).TableName);

            double indexZonePercent = 0;

            if (dbContext == null)
            {
                this.ReportError(DataBaseSource.ConnectionError);
                return 0;
            }
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
                //List<ContractLandMark> ListLand = new List<ContractLandMark>();
                for (int i = 0; i < shapeDataList.Count; i++)
                {
                    addZoneLand = new ContractLandMark();
                    resLand = new ContractLandMark();
                    resLand = modifyContractLandinfo(addZoneLand, shapeDataList[i], allGetSelectColList, zoneNameInfo);
                    if (resLand == null) { dbContext.RollbackTransaction(); return 0; }
                    if (resLand.Shape != null)
                    {
                        resLand.Shape.SpatialReference = targetSpatialReference;
                    }
                    resLand.SenderCode = zone.FullCode;
                    resLand.SenderName = zone.FullName;
                    //ListLand.Add(resLand);
                    int result = landStation.Add(resLand);
                    if (result == -1)
                        continue;

                    currentPercent = currentPercent + indexZonePercent;
                    this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + resLand.OwnerName));
                }
                //int resultInt = landStation.AddRange(ListLand);
                //if(resultInt==-1)
                //{
                //    this.ReportProgress(100);
                //    this.ReportError("插入宗地标注失败");
                //    return -1;
                //}
                this.ReportInfomation(string.Format("{0}共导入{1}条信息", zoneNameInfo, importCount));
                dbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                this.ReportError("导入Shape数据时发生错误！"+ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportShape", ex.Message + ex.StackTrace);
            }
            ////按照地块编码来导入修改
            //if (UseLandCodeBindImport)
            //{
            //    //获取地域下所有地
            //    List<ContractLandMark> zoneLandList = new List<ContractLandMark>();
            //    zoneLandList = GetCollection(zone.FullCode, eLevelOption.Self);
            //    if (zoneLandList == null) return 0;
            //    ContractLandMark modifyLand = null;
            //    List<ContractLandMark> modifyLandList = new List<ContractLandMark>();
            //    IList<object> modifyData = new List<object>();

            //    //循环每一个shape地域下地块,获取需要修改的地块集合
            //    foreach (var shpLandItem in shapeDataList)
            //    {
            //        modifyLand = new ContractLandMark();
            //        try
            //        {
            //            var shplandnum = (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[2]) as string).TrimEnd('\0');
            //            modifyLand = zoneLandList.Find(t => t.LandNumber == shplandnum);
            //        }
            //        catch
            //        {
            //            this.ReportError("当前地块编码选择项下无匹配字段数据");
            //            return 0;
            //        }
            //        if (modifyLand == null)
            //        {
            //            this.ReportWarn(string.Format("Shape地块编码为{0}的数据在{1}台账中未找到匹配项", (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[2]) as string).TrimEnd('\0'), zone.FullName));
            //            continue;
            //        }
            //        modifyLandList.Add(modifyLand);
            //        modifyData.Add(shpLandItem);
            //        importCount++;
            //    }
            //    if (modifyLandList.Count == 0)
            //    {
            //        this.ReportError(string.Format("{0}下无匹配数据", zone.FullName));
            //        return 0;
            //    }
            //    Parallel.ForEach(zoneLandList, new Action<ContractLand>((Item) =>
            //    {
            //        lock (zoneLandList)
            //        {
            //            var noLandNumberLand = modifyLandList.Find(s => s.LandNumber == Item.LandNumber);
            //            if (noLandNumberLand == null)
            //            {
            //                this.ReportWarn(string.Format("{0}台账地块编码为{1}的数据在Shape图斑中未找到匹配项", zone.FullName, Item.LandNumber));
            //            }
            //        }
            //    }));

            //    if (indexPercent != 0.0)
            //    {
            //        indexZonePercent = indexPercent / (double)(importCount == 0 ? 1 : importCount);
            //    }
            //    else
            //    {
            //        indexZonePercent = 99 / (double)(importCount == 0 ? 1 : importCount);
            //    }
            //    dbContext.BeginTransaction();
            //    try
            //    {
            //        for (int i = 0; i < modifyLandList.Count; i++)
            //        {
            //            resLand = new ContractLandMark();
            //            resLand = modifyContractLandinfo(modifyLandList[i], modifyData[i], allGetSelectColList, zoneNameInfo);
            //            if (resLand == null) { dbContext.RollbackTransaction(); return 0; }
            //            if (resLand.Shape != null)
            //            {
            //                resLand.Shape.SpatialReference = targetSpatialReference;
            //            }

            //            int resultInt = ModifyLand(resLand);
            //            if (resultInt == -1) continue;
            //            currentPercent = currentPercent + indexZonePercent;
            //            this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + modifyLandList[i].OwnerName));
            //        }
            //        this.ReportInfomation(string.Format("{0}共导入{1}条信息", zoneNameInfo, importCount));
            //        dbContext.CommitTransaction();
            //    }
            //    catch (Exception ex)
            //    {
            //        dbContext.RollbackTransaction();
            //        this.ReportError("导入Shape数据时发生错误！");
            //        YuLinTu.Library.Log.Log.WriteException(this, "ImportShape", ex.Message + ex.StackTrace);
            //    }
            //}
            //按照承包方信息来修改-需要先删除之前所有的地块数据
            //if (UseContractorInfoImport)
            //{
            //    //获取地域下所有人
            //    List<VirtualPerson> zonePersonList = new List<VirtualPerson>();
            //    zonePersonList = landVirtualPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
            //    DeleteLandByZoneCode(zone.FullCode);

            //    ContractLandMark addZoneLand;
            //    VirtualPerson addPerson = null;
            //    List<VirtualPerson> addPersonList = new List<VirtualPerson>();
            //    IList<object> addData = new List<object>();
            //    //获取对应的人
            //    foreach (var shpLandItem in shapeDataList)
            //    {
            //        try
            //        {
            //            addPerson = zonePersonList.Find(t => t.Name == (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[0]) as string));
            //        }
            //        catch
            //        {
            //            this.ReportError("当前承包方名称选择项下无匹配字段数据");
            //            return 0;
            //        }
            //        if (addPerson == null)
            //        {
            //            this.ReportWarn(string.Format("Shape承包方名称为{0}的数据在{1}台账中未找到匹配项", (ObjectExtensions.GetPropertyValue(shpLandItem, allGetSelectColList[0]) as string), zone.FullName));
            //            continue;
            //        }
            //        addPersonList.Add(addPerson);
            //        addData.Add(shpLandItem);
            //        importCount++;
            //    }
            //    if (addPersonList.Count == 0)
            //    {
            //        this.ReportError(string.Format("{0}下无匹配数据", zone.FullName));
            //        return 0;
            //    }
            //    Parallel.ForEach(zonePersonList, new Action<VirtualPerson>((Item) =>
            //    {
            //        lock (zonePersonList)
            //        {
            //            var noOwenerNameLand = addPersonList.Find(s => s.Name == Item.Name);
            //            if (noOwenerNameLand == null)
            //            {
            //                this.ReportWarn(string.Format("{0}台账承包方名称为{1}的数据在Shape图斑中未找到匹配项", zone.FullName, Item.Name));
            //            }
            //        }
            //    }));

            //    //如果分区进度为0，则表示为组级别，需要从新制定百分比
            //    if (indexPercent != 0.0)
            //    {
            //        indexZonePercent = indexPercent / (double)(importCount == 0 ? 1 : importCount);
            //    }
            //    else
            //    {
            //        indexZonePercent = 99 / (double)(importCount == 0 ? 1 : importCount);
            //    }
            //    dbContext.BeginTransaction();
            //    try
            //    {
            //        for (int i = 0; i < addPersonList.Count; i++)
            //        {
            //            addZoneLand = new ContractLandMark();
            //            resLand = new ContractLandMark();

            //            resLand = modifyContractLandinfo(addZoneLand, addData[i], allGetSelectColList, zoneNameInfo);
            //            if (resLand == null) { dbContext.RollbackTransaction(); return 0; }
            //            resLand.OwnerId = addPersonList[i].ID;
            //            resLand.ZoneCode = addPersonList[i].SenderCode;
            //            resLand.ZoneName = addPersonList[i].Address;

            //            // resLand.Shape = YuLinTu.Spatial.Geometry.FromInstance(resLand.Shape.Instance);
            //            if (resLand.Shape != null)
            //                resLand.Shape.SpatialReference = targetSpatialReference;
            //            int resultInt = AddLand(resLand);
            //            if (resultInt == -1) continue;
            //            currentPercent = currentPercent + indexZonePercent;
            //            this.ReportProgress((int)currentPercent, string.Format("{0}", zoneNameInfo + addPersonList[i].Name));
            //        }
            //        this.ReportInfomation(string.Format("{0}共导入{1}条信息", zoneNameInfo, importCount));
            //        dbContext.CommitTransaction();
            //    }
            //    catch (Exception ex)
            //    {
            //        dbContext.RollbackTransaction();
            //        this.ReportError("导入Shape数据时发生错误！");
            //        YuLinTu.Library.Log.Log.WriteException(this, "ImportShape", ex.Message + ex.StackTrace);
            //    }
            //}
            return importCount;
        }

        #region 获取选择的匹配项

        private string GetproertValue(object shapeData,string Value)
        {
            object obj = ObjectExtensions.GetPropertyValue(shapeData, Value);
            if (obj == null)
                return "";
            return obj.ToString();
        }
        /// <summary>
        /// 获取选择的文件配置匹配修改土地对象
        /// </summary>
        private ContractLandMark modifyContractLandinfo(ContractLandMark targetLand, object shapeData, List<string> selectColNameList, string zoneNameInfo)
        {
            //循环每个配置属性，如果被下拉，对应地块就要修改属性
            PropertyInfo[] infoList = typeof(ImportZDBZDefine).GetProperties();
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
                //targetLand.OwnerName = (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[0]).ToString());
            }
            if ((string)infoList[1].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.Name = GetproertValue(shapeData, selectColNameList[1]);//(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[1])).ToString();
            }
            if ((string)infoList[2].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.LandNumber = GetproertValue(shapeData, selectColNameList[2]); // (ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[2]).ToString()).TrimEnd('\0');
            }
            try
            {
                YuLinTu.Spatial.Geometry g = (ObjectExtensions.GetPropertyValue(shapeData, "Shape") as Geometry);
                targetLand.Shape = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
            }
            catch
            {
                this.ReportError("当前Shape数据Shape无效");

                return null;
            }
            if ((string)infoList[3].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.ImageNumber = GetproertValue(shapeData, selectColNameList[3]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[3]).ToString());
            }
            if ((string)infoList[4].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    string mj= GetproertValue(shapeData, selectColNameList[4]);
                    if (mj != "")
                        targetLand.TableArea = double.Parse(mj);
                    else
                        targetLand.TableArea = 0;
                }
                catch
                {
                    this.ReportError(string.Format("当前Shape数据台账面积'{0}'错误，无法转换", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[4]).ToString()));

                    return null;
                }
            }
            if ((string)infoList[5].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {   //实测面积处理
                double getActualArea = 0.0;
                try
                {
                    var actualArea = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[5]);

                    if (actualArea == null)
                    {
                        getActualArea = 0.0;
                    }
                    else
                    {
                        getActualArea = (double)actualArea;
                    }
                }
                catch
                {
                    this.ReportError(string.Format("当前Shape数据实测面积'{0}'错误，无法转换", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[5]).ToString()));

                    return null;
                }
                targetLand.ActualArea = getActualArea;
            }

            if ((string)infoList[6].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborEast = GetproertValue(shapeData, selectColNameList[6]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[6]).ToString());
            }
            if ((string)infoList[7].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborSouth = GetproertValue(shapeData, selectColNameList[7]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[7]).ToString());
            }
            if ((string)infoList[8].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborWest = GetproertValue(shapeData, selectColNameList[8]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[8]).ToString());
            }
            if ((string)infoList[9].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.NeighborNorth = GetproertValue(shapeData, selectColNameList[9]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[9]).ToString());
            }
            if ((string)infoList[10].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var dictTDYT = DictList.Find(c => c.Name == (GetproertValue(shapeData, selectColNameList[10])) && c.GroupCode == DictionaryTypeInfo.TDYT);
                if (dictTDYT == null)
                {
                    this.ReportError(string.Format("当前Shape数据土地用途名称'{0}'错误，无法入库", GetproertValue(shapeData, selectColNameList[10])));

                    return null;
                }
                else
                {
                    targetLand.Purpose = dictTDYT.Code;
                }
            }
            if ((string)infoList[11].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var dictDLDJ = DictList.Find(c => c.Name == (GetproertValue(shapeData, selectColNameList[11])) && c.GroupCode == DictionaryTypeInfo.DLDJ);
                if (dictDLDJ == null)
                {
                    this.ReportError(string.Format("当前Shape数据地力等级名称'{0}'错误，无法入库", GetproertValue(shapeData, selectColNameList[11])));

                    return null;
                }
                else
                {
                    targetLand.LandLevel = dictDLDJ.Code;
                }
            }
            if ((string)infoList[12].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var dictTDLYLX = DictList.Find(c => c.Name == (GetproertValue(shapeData, selectColNameList[12])) && c.GroupCode == DictionaryTypeInfo.TDLYLX);
                if (dictTDLYLX == null)
                {
                    this.ReportError(string.Format("当前Shape数据土地利用类型名称'{0}'错误，无法入库", GetproertValue(shapeData, selectColNameList[12])));

                    return null;
                }
                else
                {
                    targetLand.LandCode = dictTDLYLX.Code;
                    targetLand.LandName = dictTDLYLX.Name;
                }
            }
            if ((string)infoList[13].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var value = GetproertValue(shapeData, selectColNameList[13]);
                bool boolValue = value == "是" || value == "true" || value == "True" || value == "TRUE" ? true : false;
                if (value == null)
                {
                    this.ReportError(string.Format("当前Shape数据是否基本农田名称'{0}'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[13]).ToString()));

                    return null;
                }
                else
                {
                    targetLand.IsFarmerLand = boolValue;
                }
            }
            if ((string)infoList[14].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.ReferPerson = GetproertValue(shapeData, selectColNameList[14]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[14]).ToString());
            }
            if ((string)infoList[15].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                if ((GetproertValue(shapeData, selectColNameList[15])) == "承包地")
                {
                    targetLand.LandCategory = "10";
                }
                else
                {
                    var dictDKLB = DictList.Find(c => c.Name == (GetproertValue(shapeData, selectColNameList[15])) && c.GroupCode == DictionaryTypeInfo.DKLB);
                    if (dictDKLB == null)
                    {
                        this.ReportError(string.Format("当前Shape数据地块类别名称'{0}'错误，无法入库", GetproertValue(shapeData, selectColNameList[15])));

                        return null;
                    }
                    else
                    {
                        targetLand.LandCategory = dictDKLB.Code;
                    }
                }
            }
            if ((string)infoList[16].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    targetLand.AwareArea = double.Parse(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[16]).ToString());
                }
                catch
                {
                    this.ReportError(string.Format("当前Shape数据颁证面积'{0}'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[16]).ToString()));

                    return null;
                }
            }
            if ((string)infoList[17].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    targetLand.MotorizeLandArea = double.Parse(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[17]).ToString());
                }
                catch
                {
                    this.ReportError(string.Format("当前Shape数据机动地面积'0'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[17]).ToString()));

                    return null;
                }
            }
            if ((string)infoList[18].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string value= GetproertValue(shapeData, selectColNameList[18]);
                var dictCBFS = DictList.Find(c => c.Name == (value) && c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
                if (dictCBFS == null)
                {
                    this.ReportError(string.Format("当前Shape数据承包方式名称'{0}'错误，无法入库", value));

                    return null;
                }
                else
                { targetLand.ConstructMode = dictCBFS.Code; }
            }
            else
            {
                //如果没有选，则默认为家庭承包
                targetLand.ConstructMode = ((int)eConstructMode.Family).ToString();
            }
            if ((string)infoList[19].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.PlotNumber = GetproertValue(shapeData, selectColNameList[19]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[19]).ToString());
            }
            if ((string)infoList[20].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string value= GetproertValue(shapeData, selectColNameList[20]);
                var dictZZLX = DictList.Find(c => c.Name == (value) && c.GroupCode == DictionaryTypeInfo.ZZLX);
                if (dictZZLX == null)
                {
                    this.ReportError(string.Format("当前Shape数据种植类型名称'{0}'错误，无法入库", value));

                    return null;
                }
                else
                {
                    targetLand.PlatType = dictZZLX.Code;
                }
            }
            if ((string)infoList[21].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string value= GetproertValue(shapeData, selectColNameList[21]);
                var dictJYFS = DictList.Find(c => c.Name == (value) && c.GroupCode == DictionaryTypeInfo.JYFS);
                if (dictJYFS == null)
                {
                    this.ReportError(string.Format("当前Shape数据经营方式名称'{0}'错误，无法入库", value));

                    return null;
                }
                else
                {
                    targetLand.ManagementType = dictJYFS.Code;
                }
            }
            if ((string)infoList[22].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.PlantType = GetproertValue(shapeData, selectColNameList[22]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[22]).ToString());
            }
            if ((string)infoList[23].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.FormerPerson = GetproertValue(shapeData, selectColNameList[23]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[23]).ToString());
            }
            if ((string)infoList[24].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.ZoneName = GetproertValue(shapeData, selectColNameList[24]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[24]).ToString());
            }
            if ((string)infoList[25].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                var value = GetproertValue(shapeData, selectColNameList[25]); //ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[25]).ToString();
                bool boolValue = value == "是" || value == "true" || value == "True" || value == "TRUE" ? true : false;
                if (value == null)
                {
                    this.ReportError(string.Format("当前Shape数据是否流转名称'{0}'错误，无法入库", value));

                    return null;
                }
                else
                {
                    targetLand.IsTransfer = boolValue;
                }
            }
            if ((string)infoList[26].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                string value= GetproertValue(shapeData, selectColNameList[26]);
                var dictLZLX = DictList.Find(c => c.Name == (value) && c.GroupCode == DictionaryTypeInfo.LZLX);
                if (dictLZLX == null)
                {
                    this.ReportError(string.Format("当前Shape数据流转类型名称'{0}'错误，无法入库", value));

                    return null;
                }
                else
                { targetLand.TransferType = dictLZLX.Code; }
            }
            if ((string)infoList[27].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.TransferTime = GetproertValue(shapeData, selectColNameList[27]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[27]).ToString());
            }
            if ((string)infoList[28].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    targetLand.PertainToArea = double.Parse(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[28]).ToString());
                }
                catch
                {
                    this.ReportError(string.Format("当前Shape数据流转面积'{0}'错误，无法入库", ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[28]).ToString()));

                    return null;
                }
            }
            if ((string)infoList[29].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.SurveyPerson = GetproertValue(shapeData, selectColNameList[29]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[29]).ToString());
            }
            if ((string)infoList[30].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    var datetime = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[30]).ToString();
                    if (datetime == "" || datetime == null)
                    {
                        expand.SurveyDate = null;
                    }
                    else
                    {
                        expand.SurveyDate = DateTime.Parse(datetime);
                    }
                }
                catch
                {
                    this.ReportError("当前Shape数据调查日期类型错误，无法入库");

                    return null;
                }
            }
            if ((string)infoList[31].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.SurveyChronicle = GetproertValue(shapeData, selectColNameList[31]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[31]).ToString());
            }
            if ((string)infoList[32].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.CheckPerson = GetproertValue(shapeData, selectColNameList[32]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[32]).ToString());
            }
            if ((string)infoList[33].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                try
                {
                    var datetime = ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[33]).ToString();
                    if (datetime == "" || datetime == null)
                    {
                        expand.CheckDate = null;
                    }
                    else
                    {
                        expand.CheckDate = DateTime.Parse(datetime);
                    }
                }
                catch
                {
                    this.ReportError("当前Shape数据审核日期类型错误，无法入库");

                    return null;
                }
            }
            if ((string)infoList[34].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                expand.CheckOpinion = GetproertValue(shapeData, selectColNameList[34]); //(ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[34]).ToString());
            }
            if ((string)infoList[35].GetValue(ImportLandShapeInfoDefine, null) != "None")
            {
                targetLand.Comment = GetproertValue(shapeData, selectColNameList[35]); //ObjectExtensions.GetPropertyValue(shapeData, selectColNameList[35]).ToString();
            }
            targetLand.ExpandInfo = ToolSerialize.SerializeXmlString<AgricultureLandExpand>(expand);
            return targetLand;
        }
      

        /// <summary>
        /// 获取选择的土地地块弹出框导入配置下拉列表所有字段名称，与配置实体保持对应
        /// </summary>
        private List<string> getSelectColList()
        {
            PropertyInfo[] infoList = typeof(ImportZDBZDefine).GetProperties();
            //获取下拉框中对应承包方 地块编码的下拉字段名称

            string contractorName = infoList[0].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landName = infoList[1].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string cadastralNumber = infoList[2].GetValue(ImportLandShapeInfoDefine, null).ToString();
            //string shape = infoList[3].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string ImageNumber = infoList[3].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string tableArea = infoList[4].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string actualArea = infoList[5].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string east = infoList[6].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string sourth = infoList[7].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string west = infoList[8].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string north = infoList[9].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landPurpose = infoList[10].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landLevel = infoList[11].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landType = infoList[12].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string isFarmerLand = infoList[13].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string referPerson = infoList[14].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string arableType = infoList[15].GetValue(ImportLandShapeInfoDefine, null).ToString();
            //string totalTableArea = infoList[17].GetValue(ImportLandShapeInfoDefine, null)+1].Value;
            //string totalActualArea = infoList[18].GetValue(ImportLandShapeInfoDefine, null)+1].Value;
            string awareArea = infoList[16].GetValue(ImportLandShapeInfoDefine, null).ToString();
            //string totalAwareArea = infoList[20].GetValue(ImportLandShapeInfoDefine, null)+1].Value;
            string motorizeArea = infoList[17].GetValue(ImportLandShapeInfoDefine, null).ToString();
            //string totalMotorizeArea = infoList[22].GetValue(ImportLandShapeInfoDefine, null)+1].Value;
            string constructMode = infoList[18].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string plotNumber = infoList[19].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string platType = infoList[20].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string managementType = infoList[21].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landPlant = infoList[22].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string sourceName = infoList[23].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landLocation = infoList[24].GetValue(ImportLandShapeInfoDefine, null).ToString();
            //string sharePerson = infoList[26].GetValue(ImportLandShapeInfoDefine, null)+1].Value;
            //string concord = infoList[26].GetValue(ImportLandShapeInfoDefine, null)+1].Value;
            //string regeditBook = infoList[27].GetValue(ImportLandShapeInfoDefine, null)+1].Value;
            string isTranster = infoList[25].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string transterMode = infoList[26].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string transterTerm = infoList[27].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string transterArea = infoList[28].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string landSurveyPersonIndex = infoList[29].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landSurveyDateIndex = infoList[30].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landSurveyChronicleIndex = infoList[31].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landCheckPersonIndex = infoList[32].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landCheckDateIndex = infoList[33].GetValue(ImportLandShapeInfoDefine, null).ToString();
            string landCheckOpinionIndex = infoList[34].GetValue(ImportLandShapeInfoDefine, null).ToString();

            string comment = infoList[35].GetValue(ImportLandShapeInfoDefine, null).ToString();

            //已经下拉选取的字段名称集合-序号与配置实体顺序一致
            List<string> userSelectColNameList = new List<string>();
            userSelectColNameList.Add(contractorName);
            userSelectColNameList.Add(landName);
            userSelectColNameList.Add(cadastralNumber);
            //userSelectColNameList.Add(shape);
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
            //userSelectColNameList.Add(totalTableArea);
            //userSelectColNameList.Add(totalActualArea);
            userSelectColNameList.Add(awareArea);
            //userSelectColNameList.Add(totalAwareArea);
            userSelectColNameList.Add(motorizeArea);
            //userSelectColNameList.Add(totalMotorizeArea);
            userSelectColNameList.Add(constructMode);
            userSelectColNameList.Add(plotNumber);
            userSelectColNameList.Add(platType);
            userSelectColNameList.Add(managementType);
            userSelectColNameList.Add(landPlant);
            userSelectColNameList.Add(sourceName);
            userSelectColNameList.Add(landLocation);
            //userSelectColNameList.Add(sharePerson);
            //userSelectColNameList.Add(concord);
            //userSelectColNameList.Add(regeditBook);
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

            return userSelectColNameList;
        }    


        #endregion

        #endregion


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
    

        #endregion

        #endregion
    }
}
