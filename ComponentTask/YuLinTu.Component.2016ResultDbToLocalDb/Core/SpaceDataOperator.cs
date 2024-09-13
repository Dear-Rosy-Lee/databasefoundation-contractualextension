/*
 * (C) 2016鱼鳞图公司版权所有,保留所有权利
*/
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Data.SQLite;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 空间数据操作
    /// </summary>
    public class SpaceDataOperator
    {
        #region Fields

        public int srid;
        private Dictionary<string, int> codeIndex;
        private string dkShapeFilePath;//用于判断是否读取了不同的地块shp
        #endregion

        #region Properties

        /// <summary>
        /// 矢量文件集合
        /// </summary>
        public List<FileCondition> ShapeFileList { get; set; }

        private object _lock = new object();
        private IDbContext sqliteDb;

        #endregion

        #region Ctor

        public SpaceDataOperator(int sr = 0)
        {
            srid = sr;
            codeIndex = new Dictionary<string, int>();
        }

        public SpaceDataOperator(int sr, List<FileCondition> shpFileList)
        {
            srid = sr;
            ShapeFileList = shpFileList;
            codeIndex = new Dictionary<string, int>();
        }

        #endregion

        #region 获取编码中的上级地域编码

        /// <summary>
        /// 获取上级地域编码
        /// </summary>
        /// <returns></returns>
        private static string GetUpLevelCode(eZoneLevel level, string fullcode)
        {
            string code = string.Empty;
            int length = Zone.ZONE_CITY_LENGTH;
            switch (level)
            {
                case eZoneLevel.Group:
                    length = Zone.ZONE_VILLAGE_LENGTH;
                    break;
                case eZoneLevel.Village:
                    length = Zone.ZONE_TOWN_LENGTH;
                    break;
                case eZoneLevel.Town:
                    length = Zone.ZONE_COUNTY_LENGTH;
                    break;
            }
            if (fullcode.Length > length)
            {
                code = fullcode.Substring(0, length);
            }
            return code;
        }

        #endregion

        #region 获取地域矢量文件中的数据

        /// <summary>
        /// 从矢量文件中获取地域信息
        /// </summary>
        public static List<Zone> ProcessZoneData(FilePathInfo currntPath, bool isStandCode)
        {
            if (currntPath == null || currntPath.ShapeFileList == null || currntPath.ShapeFileList.Count == 0)
            {
                return null;
            }
            List<Zone> zoneCollection = new List<Zone>();
            var fnpCounty = currntPath.ShapeFileList.Find(t => t.Name == XJXZQ.TableName);
            var fnpTwon = currntPath.ShapeFileList.Find(t => t.Name == XJQY.TableName);
            var fnpVillige = currntPath.ShapeFileList.Find(t => t.Name == CJQY.TableName);
            var fnpGroup = currntPath.ShapeFileList.Find(t => t.Name == ZJQY.TableName);
            var countyZones = GetZone(fnpCounty, isStandCode);
            var townZones = GetZone(fnpTwon, isStandCode);
            var villigeZones = GetZone(fnpVillige, isStandCode);
            var groupZones = GetZone(fnpGroup, isStandCode);

            CompleteEntity(zoneCollection, countyZones, null, eZoneLevel.County);
            CompleteEntity(zoneCollection, townZones, countyZones, eZoneLevel.Town);
            CompleteEntity(zoneCollection, villigeZones, townZones, eZoneLevel.Village);
            CompleteEntity(zoneCollection, groupZones, villigeZones, eZoneLevel.Group);

            countyZones.Clear();
            townZones.Clear();
            villigeZones.Clear();
            groupZones.Clear();

            return zoneCollection;
        }

        /// <summary>
        /// 转换实体
        /// </summary>
        /// <param name="zoneCollection">地域集合</param>
        /// <param name="zones">当前集合</param>
        /// <param name="upLevelZones">上级地域集合</param>
        /// <param name="level">当前级别</param>
        /// <param name="localService">服务</param>
        private static void CompleteEntity(List<Zone> zoneCollection, Dictionary<string, Zone> zones,
            Dictionary<string, Zone> upLevelZones, eZoneLevel level)
        {
            if (zones == null || zones.Count == 0)
            {
                return;
            }
            foreach (var z in zones)
            {
                Zone upZone = null;
                Zone cZone = z.Value;
                if (upLevelZones == null) continue;
                if (upLevelZones.ContainsKey(cZone.UpLevelCode))
                {
                    upZone = upLevelZones[cZone.UpLevelCode];
                }
                if (upZone != null)
                {
                    cZone.UpLevelName = upZone.FullName;
                    cZone.FullName = cZone.UpLevelName + cZone.Name;
                }
                zoneCollection.Add(cZone);
            }
        }

        /// <summary>
        /// 根据文件取出地域
        /// </summary>
        private static Dictionary<string, Zone> GetZone(FileCondition fnp, bool isStandCode)
        {
            Dictionary<string, Zone> zones = new Dictionary<string, Zone>();
            if (fnp == null || string.IsNullOrEmpty(fnp.FilePath))
            {
                return zones;
            }
            string fileName = fnp.FilePath;
            int index = 0;
            using (ShapefileDataReader dataReader = new ShapefileDataReader(fileName, GeometryFactory.Default))
            {
                DbaseFieldDescriptor[] fileds = dataReader.DbaseHeader.Fields;
                while (dataReader.Read())
                {
                    Zone zone = GetZoneByFileds(fnp.Name, fileds, dataReader, isStandCode);
                    if (zone != null)
                    {
                        zone.Shape = dataReader.Geometry;
                        if (zones.ContainsKey(zone.FullCode) == false)
                            zones.Add(zone.FullCode, zone);
                    }
                    index++;
                }
            }
            return zones;
        }

        /// <summary>
        /// 创建地域
        /// </summary>
        private static Zone GetZoneByFileds(string name, DbaseFieldDescriptor[] fileds,
            ShapefileDataReader dataReader, bool isStandCode)
        {
            Zone zone = null;
            switch (name)
            {
                case XJXZQ.TableName:
                    zone = new Zone();
                    for (int i = 0; i < fileds.Length; i++)
                    {
                        if (fileds[i].Name == XJXZQ.CXZQDM)
                        {
                            zone.FullCode = dataReader.GetValue(i).ToString();
                        }
                        if (fileds[i].Name == XJXZQ.CXZQMC)
                        {
                            zone.Name = dataReader.GetValue(i).ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(zone.FullCode) && zone.FullCode.Length == 6)
                    {
                        zone.Code = zone.FullCode.Substring(4, 2);
                        zone.UpLevelCode = zone.FullCode.Substring(0, 4);
                    }
                    zone.Level = eZoneLevel.County;
                    break;
                case XJQY.TableName:
                    zone = new Zone();
                    for (int i = 0; i < fileds.Length; i++)
                    {
                        if (fileds[i].Name == XJQY.CXJQYDM)
                        {
                            zone.FullCode = dataReader.GetValue(i).ToString();
                        }
                        if (fileds[i].Name == XJQY.CXJQYMC)
                        {
                            zone.Name = dataReader.GetValue(i).ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(zone.FullCode) && zone.FullCode.Length == 9)
                    {
                        zone.Code = zone.FullCode.Substring(6, 3);
                        zone.UpLevelCode = zone.FullCode.Substring(0, 6);
                    }
                    zone.Level = eZoneLevel.Town;
                    break;
                case CJQY.TableName:
                    zone = new Zone();
                    for (int i = 0; i < fileds.Length; i++)
                    {
                        if (fileds[i].Name == CJQY.CCJQYDM)
                        {
                            zone.FullCode = dataReader.GetValue(i).ToString();
                        }
                        if (fileds[i].Name == CJQY.CCJQYMC)
                        {
                            zone.Name = dataReader.GetValue(i).ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(zone.FullCode) && zone.FullCode.Length == 12)
                    {
                        zone.Code = zone.FullCode.Substring(9, 3);
                        zone.UpLevelCode = zone.FullCode.Substring(0, 9);
                    }
                    zone.Level = eZoneLevel.Village;
                    break;
                case ZJQY.TableName:
                    zone = new Zone();
                    string code = string.Empty;
                    for (int i = 0; i < fileds.Length; i++)
                    {
                        if (fileds[i].Name == ZJQY.CZJQYDM)
                        {
                            code = dataReader.GetValue(i).ToString();
                        }
                        if (fileds[i].Name == ZJQY.CZJQYMC)
                        {
                            zone.Name = dataReader.GetValue(i).ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(code) && code.Length == 14)
                    {
                        zone.Code = isStandCode ? code.Substring(12, 2) : "00" + code.Substring(12, 2);
                        zone.UpLevelCode = code.Substring(0, 12);
                        zone.FullCode = zone.UpLevelCode + zone.Code;
                    }
                    zone.Level = eZoneLevel.Group;
                    break;
            }
            return zone;
        }

        #endregion

        #region 获取Shape数据

        /// <summary>
        /// 取shape文件中的地块
        /// </summary>
        public List<DKEX> InitiallLandList(List<FileCondition> fnps, string zoneCode = "")
        {
            var dkList = new List<DKEX>();
            if (fnps == null || fnps.Count == 0)
                return dkList;

            foreach (var item in fnps)
            {
                var dks = InitiallShapeLandList(item, zoneCode);
                dkList.AddRange(dks);
            }
            return dkList;
        }

        /// <summary>
        /// 从数据库中获取地块数据
        /// </summary> 
        public List<DKEX> GetLandFromDatabase(string keyCode, bool getjzdx = true)
        {
            return GetLandFromDatabase(keyCode, ShapeFileList, getjzdx);
        }

        /// <summary>
        /// 根据地域获取地块
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="files"></param>
        /// <param name="getJzdx">是否获取界址点线</param>
        /// <returns></returns>
        private List<DKEX> GetLandFromDatabase(string keyCode, List<FileCondition> files, bool getJzdx = true)
        {
            var dkquery = sqliteDb.CreateQuery<DKST>();
            var jzdquery = sqliteDb.CreateQuery<JZDST>();
            var jzxquery = sqliteDb.CreateQuery<JZXST>();

            var bmList = dkquery.Where(t => t.DKBM.StartsWith(keyCode)).ToList();
            var jzdbmList = jzdquery.Where(t => t.DKBM.Contains(keyCode)).ToList();
            var jzxbmList = jzxquery.Where(t => t.DKBM.Contains(keyCode)).ToList();

            List<DK> dkList = new List<DK>();

            var group = bmList.GroupBy(t => t.TCBS).ToList();
            foreach (var item in group)
            {
                var bs = item.Key == "0" ? "" : item.Key;
                var file = files.Find(t => t.Name.StartsWith("DK") && t.FullName.Contains((bs == "" ? "" : "-" + bs)));
                if (file == null)
                    file = files.Find(t => t.Name.StartsWith("DK") && t.FullName.Contains((bs == "" ? "" : "_" + bs)));
                var recodeList = item.ToList();
                var list = InitiallDataList<DK, DKST>(file, recodeList);
                dkList.AddRange(list);
            }

            var jzdList = new Dictionary<string, List<JZD>>();
            var jzxList = new Dictionary<string, List<JZX>>();

            if (getJzdx)
            {
                var jzdgroup = jzdbmList.GroupBy(t => t.TCBS).ToList();
                foreach (var item in jzdgroup)
                {
                    var bs = item.Key == "0" ? "" : item.Key;
                    var file = files.Find(t => t.Name == "JZD" && t.FullName.Contains((bs == "" ? "" : "-" + bs)));
                    var recodeList = item.ToList();
                    var list = InitiallDataEnum<JZD, JZDST>(file, recodeList);
                    foreach (var jzd in list)
                    {
                        if (jzdList.ContainsKey(jzd.DKBM))
                            jzdList[jzd.DKBM].Add(jzd);
                        else
                            jzdList.Add(jzd.DKBM, new List<JZD>() { jzd });
                    }
                }

                var jzxgroup = jzxbmList.GroupBy(t => t.TCBS).ToList();
                foreach (var item in jzxgroup)
                {
                    var bs = item.Key == "0" ? "" : item.Key;
                    var file = files.Find(t => t.Name == "JZX" && t.FullName.Contains((bs == "" ? "" : "-" + bs)));
                    var recodeList = item.ToList();
                    var list = InitiallDataEnum<JZX, JZXST>(file, recodeList);
                    foreach (var jzx in list)
                    {
                        if (jzxList.ContainsKey(jzx.DKBM))
                            jzxList[jzx.DKBM].Add(jzx);
                        else
                            jzxList.Add(jzx.DKBM, new List<JZX>() { jzx });
                    }
                }
            }
            var dkexList = new ConcurrentBag<DKEX>();
            Parallel.ForEach(dkList, land =>
            {
                lock (_lock)
                {
                    var dkex = ObjectExtension.ConvertTo<DKEX>(land);
                    dkex.JZD = new List<JZD>();
                    dkex.JZX = new List<JZX>();
                    if (getJzdx)
                    {
                        var jzds = jzdList.Where(t => t.Key.Contains(land.DKBM)).Select(t => t.Value).ToList();
                        var jzxs = jzxList.Where(t => t.Key.Contains(land.DKBM)).Select(t => t.Value).ToList();
                        foreach (var jzd in jzds)
                        {
                            foreach (var j in jzd)
                            {
                                var newj = ObjectExtension.ConvertTo<JZD>(j);
                                //newj.ID = Guid.NewGuid();
                                dkex.JZD.Add(newj);
                            }
                        }
                        foreach (var jzx in jzxs)
                        {
                            foreach (var j in jzx)
                            {
                                var newj = ObjectExtension.ConvertTo<JZX>(j);
                                //newj.ID = Guid.NewGuid();
                                dkex.JZX.Add(newj);
                            }
                        }
                    }

                    dkexList.Add(dkex);
                }
            });
            return dkexList.ToList();
        }


        /// <summary>
        /// 取shape文件中的地块
        /// </summary>
        public List<DKEX> InitiallShapeLandList(FileCondition fnp, string zoneCode = "")
        {
            var dkList = new List<DKEX>();

            if (fnp == null || string.IsNullOrEmpty(fnp.FilePath))
            {
                return dkList;
            }
            //codeIndex.Clear();
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(fnp.FilePath);
                if (!string.IsNullOrEmpty(err))
                {
                    LogWrite.WriteErrorLog("读取地块Shape文件发生错误" + err);
                    return null;
                }
                foreach (var dk in ForEnumRecord<DKEX>(shp, fnp.FilePath, codeIndex, DK.CDKBM, zoneCode))
                {
                    dkList.Add(dk);
                }
            }
            return dkList;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="fnp"></param>
        /// <returns></returns>
        public List<T> GetShapeData<T>(ref string msg) where T : class, new()
        {
            var fielInfo = typeof(T).GetField("TableName");
            var fielInfoCN = typeof(T).GetField("TableNameCN");

            string name = fielInfo.GetValue(null) as string;
            string nameCN = fielInfoCN.GetValue(null) as string;

            if (this.ShapeFileList == null)
                return new List<T>();
            var fnp = ShapeFileList.Find(t => t.Name == name);

            if (fnp == null || string.IsNullOrEmpty(fnp.FilePath))
            {
                return new List<T>();
            }
            var list = GetDataFromShape<T>(fnp);
            msg = "," + nameCN + list.Count + "条";
            return list;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="fnp"></param>
        /// <returns></returns>
        public List<T> GetDataFromShape<T>(FileCondition fnp) where T : class, new()
        {
            List<T> list = new List<T>();
            if (fnp == null || string.IsNullOrEmpty(fnp.FilePath))
            {
                return list;
            }
            using (var dataReader = new ShapefileDataReader(fnp.FilePath, GeometryFactory.Default))
            {
                foreach (var dk in ForEnumRecord<T>(dataReader, fnp.FilePath))
                {
                    list.Add(dk);
                }
            }
            return list;
        }

        /// <summary>
        /// 循环获取空间记录
        /// </summary>
        public IEnumerable<T> ForEnumRecord<T>(ShapefileDataReader dataReader, string fileName,
            string mainField = "", string zoneCode = "") where T : class, new()
        {
            DbaseFieldDescriptor[] fileds = dataReader.DbaseHeader.Fields;
            PropertyInfo[] infoArray = typeof(T).GetProperties();
            bool isSelect = (mainField != "" && zoneCode != "") ? true : false;
            while (dataReader.Read())
            {
                if (dataReader.Geometry == null)
                {
                    continue;
                }
                if (isSelect)
                {
                    var pi = infoArray.Where(t => t.Name == mainField).FirstOrDefault();
                    if (pi == null)
                        continue;
                    int index = GetIndexFromArray(fileds, pi.Name);
                    if (index < 0)
                    {
                        continue;
                    }
                    if (!FieldValue(index, dataReader, pi).ToString().StartsWith(zoneCode))
                    {
                        continue;
                    }
                }
                var entity = new T();
                for (int i = 0; i < infoArray.Length; i++)
                {
                    PropertyInfo info = infoArray[i];
                    int index = GetIndexFromArray(fileds, info.Name);
                    if (index < 0)
                    {
                        continue;
                    }
                    info.SetValue(entity, FieldValue(index, dataReader, info), null);
                }
                ObjectExtension.SetPropertyValue(entity, "Shape", SetGeometry(dataReader.Geometry, srid));
                yield return entity;
            }
        }

        /// <summary>
        /// 循环获取空间记录
        /// </summary>
        public IEnumerable<T> ForEnumRecord<T>(ShapeFile shp, string fileName, Dictionary<string, int> codeIndex,
            string mainField = "", string zoneCode = "", bool setGeo = true) where T : class, new()
        {
            bool isSameDkShp = true;
            if (dkShapeFilePath.IsNullOrEmpty())
            {
                dkShapeFilePath = fileName;
            }
            else if (dkShapeFilePath.Equals(fileName) == false)
            {
                isSameDkShp = false;
            }

            var infoArray = typeof(T).GetProperties();
            var fieldIndex = new Dictionary<string, int>();
            bool isSelect = (mainField != "" && zoneCode != "") ? true : false;

            int dkbmindex = -1;
            for (int i = 0; i < infoArray.Length; i++)
            {
                var info = infoArray[i];
                var index = shp.FindField(info.Name);
                if (index >= 0)
                {
                    fieldIndex.Add(info.Name, index);
                }

                if (info.Name == mainField)
                {
                    dkbmindex = index;
                }
            }

            if (codeIndex.Count > 0 && isSameDkShp)
            {
                foreach (var item in codeIndex)
                {
                    if (isSelect)
                    {
                        if (dkbmindex < 0)
                            continue;

                        if (!item.Key.StartsWith(zoneCode))
                            continue;
                    }
                    var en = new T();
                    for (int i = 0; i < infoArray.Length; i++)
                    {
                        var info = infoArray[i];
                        if (!fieldIndex.ContainsKey(info.Name))
                            continue;
                        info.SetValue(en, FieldValue(item.Value, fieldIndex[info.Name], shp, info), null);
                    }
                    ObjectExtension.SetPropertyValue(en, "Shape", shp.GetGeometry(item.Value, srid));
                    yield return en;
                }
            }
            else
            {
                var shapeCount = shp.GetRecordCount();
                for (int i = 0; i < shapeCount; i++)
                {
                    var en = new T();

                    if (isSelect)
                    {
                        if (dkbmindex < 0)
                            continue;

                        var strValue = shp.GetFieldString(i, dkbmindex);
                        if (strValue == null)
                        {
                            continue;
                        }
                        if (!codeIndex.ContainsKey(strValue))
                            codeIndex.Add(strValue, i);
                        if (!strValue.StartsWith(zoneCode))
                            continue;
                    }
                    for (int j = 0; j < infoArray.Length; j++)
                    {
                        var info = infoArray[j];
                        if (!fieldIndex.ContainsKey(info.Name))
                            continue;
                        var value = FieldValue(i, fieldIndex[info.Name], shp, info);
                        info.SetValue(en, value, null);

                    }
                    if (setGeo)
                        ObjectExtension.SetPropertyValue(en, "Shape", shp.GetGeometry(i, srid));
                    yield return en;
                }
            }

        }

        /// <summary>
        /// 清除地块字典
        /// </summary>
        public void ClearLandDictionary()
        {
            codeIndex.Clear();
        }

        /// <summary>
        /// 字段值获取
        /// </summary>
        private object FieldValue(int index, ShapefileDataReader dataReader, PropertyInfo info)
        {
            object value = null;
            if (info.Name == "BSM")
            {
                int bsm = 0;
                int.TryParse(dataReader.GetValue(index).ToString(), out bsm);
                value = bsm;
            }
            else if (info.Name.EndsWith("MJ") || info.Name.EndsWith("MJM") || info.Name == "CD" || info.Name == ZJ.CKD || info.Name == JZD.CXZBZ || info.Name == JZD.CYZBZ ||
                 info.Name == KZD.CX80 || info.Name == KZD.CY80 || info.Name == KZD.CY2000 || info.Name == KZD.CX2000)
            {
                double scmj = 0;
                double.TryParse(dataReader.GetValue(index).ToString(), out scmj);
                value = scmj;
            }
            else
            {
                var obj = dataReader.GetValue(index);
                value = obj.ToString();
            }
            return value;
        }

        /// <summary>
        /// 字段值获取
        /// </summary>
        private object FieldValue(int row, int colum, ShapeFile dataReader, PropertyInfo info)
        {
            object value = null;
            if (info.Name == "BSM")
            {
                int bsm = 0;
                int.TryParse(dataReader.GetFieldString(row, colum), out bsm);
                value = bsm;
            }
            else if (info.Name.EndsWith("MJ") || info.Name.EndsWith("MJM") || info.Name == "CD" || info.Name == ZJ.CKD || info.Name == JZD.CXZBZ || info.Name == JZD.CYZBZ ||
                 info.Name == KZD.CX80 || info.Name == KZD.CY80 || info.Name == KZD.CY2000 || info.Name == KZD.CX2000)
            {
                double scmj = 0;
                var mjstr = dataReader.GetFieldString(row, colum);
                double.TryParse(mjstr.IsNullOrEmpty() ? "0" : mjstr, out scmj);
                value = scmj;
            }
            else
            {
                value = dataReader.GetFieldString(row, colum);
                value = value == null ? "" : value;
            }
            return value;
        }


        /// <summary>
        /// 获取字段index
        /// </summary>
        private int GetIndexFromArray(DbaseFieldDescriptor[] fileds, string name)
        {
            for (int i = 0; i < fileds.Length; i++)
            {
                DbaseFieldDescriptor filed = fileds[i];
                if (filed.Name == name)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 设置图形信息
        /// </summary>
        private object SetGeometry(GeoAPI.Geometries.IGeometry value, int srid)
        {
            value.SRID = srid;
            YuLinTu.Spatial.Geometry geo = YuLinTu.Spatial.Geometry.FromInstance(value);
            return value;
        }

        public void InsertIndexToDataBase(List<FileCondition> files, bool containsjzdx)
        {
            //bool isindexExists = false;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\Upload.sqlite");
            var dbPath = dataBasePath(path);
            var dataSource = ProviderDbCSQLite.CreateDataSourceByFileName(dbPath);//, false);
            sqliteDb = DataSource.Create<IDbContext>(dataSource.ProviderName, dataSource.ConnectionString);
            FileInfo info = new FileInfo(dbPath);
            if (info.Length > 44490400)
            {
                //isindexExists = tsrue;
                return;
            }
            var query = sqliteDb.CreateQuery<DKST>();
            var jzdquery = sqliteDb.CreateQuery<JZDST>();
            var jzxquery = sqliteDb.CreateQuery<JZXST>();
            var dks = files.FindAll(t => t.Name.StartsWith(DK.TableName));
            foreach (var fnp in dks)
            {
                var layeIndex = GetIndex(fnp.FullName);
                DataToBase<DKST>(fnp, query, layeIndex);
            }
            if (!containsjzdx)
                return;
            var jzds = files.FindAll(t => t.Name == JZD.TableName);
            foreach (var fnp in jzds)
            {
                var layeIndex = GetIndex(fnp.FullName);
                DataToBase<JZDST>(fnp, jzdquery, layeIndex);
            }

            var jzxs = files.FindAll(t => t.Name == JZX.TableName);
            foreach (var fnp in jzxs)
            {
                var layeIndex = GetIndex(fnp.FullName);
                DataToBase<DKST>(fnp, query, layeIndex);
            }
        }

        private string GetIndex(string fullname)
        {
            var index = -1;
            string name = Path.GetFileNameWithoutExtension(fullname);
            index = name.IndexOf("-");
            if (index > 0)
                return name.Substring(index + 1);
            index = name.IndexOf("_");
            if (index > 0)
                return name.Substring(index + 1);
            return index == -1 ? "0" : index.ToString();
        }

        private string dataBasePath(string tempPath)
        {
            var span = DateTime.Now.ToString("yyyyMMddHHmm").GetHashCode();
            var name = Convert.ToInt64(Math.Abs(span)).ToString();
            var newpath = Path.Combine(Path.GetTempPath(), name + ".sqlite");
            if (!System.IO.File.Exists(newpath))
                System.IO.File.Copy(tempPath, newpath, true);
            return newpath;
        }

        /// <summary>
        /// 数据入库
        /// </summary>
        private void DataToBase<T>(FileCondition fnp, IQueryContext query, string layerIndex) where T : BSST, new()
        {
            var list = new List<object>(100000);
            try
            {
                using (var lineShp = new ShapeFile())
                {
                    var err = lineShp.Open(fnp.FilePath);
                    if (err != null)
                    {
                        return;
                    }
                    var properties = typeof(T).GetProperties();
                    int recordCount = lineShp.GetRecordCount();
                    var nameIndex = new Dictionary<int, PropertyInfo>();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var fieldName = properties[i].Name;
                        var index = lineShp.FindField(fieldName);
                        if (index > -1)
                            nameIndex.Add(index, properties[i]);
                    }

                    for (int i = 0; i < recordCount; i++)
                    {
                        var en = new T();
                        en.SJHH = i;
                        en.TCBS = layerIndex;
                        foreach (var naDic in nameIndex)
                        {
                            var strValue = lineShp.GetFieldString(i, naDic.Key);
                            naDic.Value.SetValue(en, strValue, null);
                        }
                        list.Add(en);
                        if (list.Count == 100000)
                        {
                            query.AddRange(list.ToArray()).Save();
                            list.Clear();
                        }
                    }
                    if (list.Count > 0)
                    {
                        query.AddRange(list.ToArray()).Save();
                        list.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog(ex.ToString());
                throw new Exception("获取矢量数据制作索引出错:" + ex.Message);
            }
        }

        /// <summary>
        /// 取shape文件中的地块
        /// </summary>
        public List<T> InitiallDataList<T, P>(FileCondition fnp, List<P> bsList)
            where T : class, new() where P : BSST
        {
            var list = new List<T>();
            var eumQuery = InitiallDataEnum<T, P>(fnp, bsList);
            foreach (var en in eumQuery)
            {
                list.Add(en);
            }
            return list;
        }

        /// <summary>
        /// 取shape文件中的地块
        /// </summary>
        public IEnumerable<T> InitiallDataEnum<T, P>(FileCondition fnp, List<P> bsList)
            where T : class, new() where P : BSST
        {
            var dkList = new List<DKEX>();
            var infoArray = typeof(T).GetProperties();
            var fieldIndex = new Dictionary<string, int>();
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(fnp.FilePath);

                if (!err.IsNullOrEmpty())
                    yield break;//  return new List<T>();
                for (int i = 0; i < infoArray.Length; i++)
                {
                    var info = infoArray[i];
                    var index = shp.FindField(info.Name);
                    if (index >= 0)
                    {
                        fieldIndex.Add(info.Name, index);
                    }
                }
                foreach (var bs in bsList)
                {
                    var en = new T();
                    for (int i = 0; i < infoArray.Length; i++)
                    {
                        var info = infoArray[i];
                        if (!fieldIndex.ContainsKey(info.Name))
                            continue;
                        var obj = FieldValue(bs.SJHH, fieldIndex[info.Name], shp, info);
                        info.SetValue(en, obj, null);
                    }
                    var geo = shp.GetGeometry(bs.SJHH, srid);
                    ObjectExtension.SetPropertyValue(en, "Shape", geo);
                    yield return en;

                    //list.Add(en);
                }
            }
        }

        #endregion
    }
}
