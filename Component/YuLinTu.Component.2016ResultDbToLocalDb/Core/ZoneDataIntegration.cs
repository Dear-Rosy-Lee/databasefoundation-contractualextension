/*
 * (C) 2016鱼鳞图公司版权所有,保留所有权利
*/
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using YuLinTuQuality.Business.Entity;
using YuLinTuQuality.Business.TaskBasic;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using YuLinTu.tGISCNet;
using System;
using GeoAPI.Geometries;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    /// <summary>
    /// 行政区域数据操作
    /// </summary>
    public class ZoneDataIntegration
    {
        #region 获取空间地域矢量文件中数据

        /// <summary>
        /// 从矢量文件中获取地域信息
        /// </summary>
        /// <param name="fileEntry">数据文件</param>
        /// <returns>矢量地域集合</returns>
        public static List<Zone> ZoneVectorProgress(FilePathInfo fileEntry)
        {
            if (fileEntry == null || fileEntry.ShapeFileList == null || fileEntry.ShapeFileList.Count == 0)
            {
                return new List<Zone>();
            }
            List<Zone> zones = new List<Zone>();
            var fcdCounty = fileEntry.ShapeFileList.Find(t => t.Name == XJXZQ.TableName);
            var fcdTwon = fileEntry.ShapeFileList.Find(t => t.Name == XJQY.TableName);
            var fcdVillige = fileEntry.ShapeFileList.Find(t => t.Name == CJQY.TableName);
            var fcdGroup = fileEntry.ShapeFileList.Find(t => t.Name == ZJQY.TableName);
            var countys = InitalizeVectorZone(fcdCounty);
            var towns = InitalizeVectorZone(fcdTwon);
            var villiges = InitalizeVectorZone(fcdVillige);
            var groups = InitalizeVectorZone(fcdGroup);
            zones.AddRange(countys);
            zones.AddRange(towns);
            zones.AddRange(villiges);
            zones.AddRange(groups);
            return zones;
        }

        /// <summary>
        /// 根据文件初始化地域
        /// </summary>
        /// <param name="fcd">文件</param>
        /// <returns></returns>
        private static List<Zone> InitalizeVectorZone(FileCondition fcd)
        {
            List<Zone> zones = new List<Zone>();
            if (fcd == null || string.IsNullOrEmpty(fcd.FilePath))
            {
                return zones;
            }
            string fileName = fcd.FilePath;
            int index = 0;
            using (ShapefileDataReader dataReader = new ShapefileDataReader(fileName, GeometryFactory.Default))
            {
                DbaseFieldDescriptor[] fields = dataReader.DbaseHeader.Fields;
                while (dataReader.Read())
                {
                    Zone zone = InitalizeZoneVectorData(fcd.Name, fields, dataReader);
                    if (zone == null)
                    {
                        index++;
                        continue;
                    }
                    zone.Shape = dataReader.Geometry;
                    if (!zones.Exists(ze => ze.FullCode == zone.FullCode))
                    {
                        zones.Add(zone);
                    }
                    index++;
                }
                fields = null;
                GC.Collect();
            }
            return zones;
        }

        /// <summary>
        /// 初始化空间地域数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="fileds">字段集合</param>
        /// <param name="dataReader">数据读取器</param>
        /// <returns></returns>
        private static Zone InitalizeZoneVectorData(string tableName, DbaseFieldDescriptor[] fileds, ShapefileDataReader dataReader)
        {
            Zone zone = new Zone();
            switch (tableName)
            {
                case XJXZQ.TableName:
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
                    for (int i = 0; i < fileds.Length; i++)
                    {
                        if (fileds[i].Name == ZJQY.CZJQYDM)
                        {
                            zone.FullCode = dataReader.GetValue(i).ToString();
                        }
                        if (fileds[i].Name == ZJQY.CZJQYMC)
                        {
                            zone.Name = dataReader.GetValue(i).ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(zone.FullCode) && zone.FullCode.Length == 14)
                    {
                        zone.UpLevelCode = zone.FullCode.Substring(0, 12);
                        zone.Code = zone.FullCode.Substring(12);
                    }
                    zone.Level = eZoneLevel.Group;
                    break;
                default:
                    break;
            }
            return zone;
        }

        #endregion

        #region 获取属性地域表中数据

        /// <summary>
        /// 获取属性地域数据
        /// </summary>
        /// <param name="fileEntry"></param>
        /// <returns></returns>
        public static List<Zone> ZoneAttributeProgress(FilePathInfo fileEntry)
        {
            if (fileEntry == null || string.IsNullOrEmpty(fileEntry.ExcelTablePath))
            {
                return new List<Zone>();
            }
            List<Zone> zones = null;
            string fileNmae = fileEntry.DataBasePath.Replace(".mdb", "权属单位代码表.xls");
            if (!System.IO.File.Exists(fileNmae))
            {
                return null;
            }
            using (var reader = new DataZoneReader())
            {
                zones = reader.InitalizeZoneData(fileNmae);
            }
            if (zones == null || zones.Count == 0)
            {
                return new List<Zone>();
            }
            InitalizeAttributeZone(zones);
            return zones;
        }

        /// <summary>
        /// 初始化属性地域数据
        /// </summary>
        /// <returns>地域集合</returns>
        private static void InitalizeAttributeZone(List<Zone> zones)
        {
            if (zones == null || zones.Count == 0)
            {
                return;
            }
            zones.ForEach(ze =>
            {
                if (ze.FullCode.Length == Zone.ZONE_PROVICE_LENGTH || (ze.FullCode.Length > Zone.ZONE_PROVICE_LENGTH && ze.FullCode.Substring(Zone.ZONE_PROVICE_LENGTH) == "000000000000"))
                {
                    ze.FullCode = ze.FullCode.Substring(0, Zone.ZONE_PROVICE_LENGTH);
                    ze.UpLevelCode = "86";
                    ze.Code = ze.FullCode;
                    ze.Level = eZoneLevel.Province;
                    return;
                }
                if (ze.FullCode.Length == Zone.ZONE_CITY_LENGTH || (ze.FullCode.Length > Zone.ZONE_CITY_LENGTH && ze.FullCode.Substring(Zone.ZONE_CITY_LENGTH) == "0000000000"))
                {
                    ze.FullCode = ze.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH);
                    ze.UpLevelCode = ze.FullCode.Substring(0, Zone.ZONE_PROVICE_LENGTH);
                    ze.Code = ze.FullCode.Substring(Zone.ZONE_PROVICE_LENGTH);
                    ze.Level = eZoneLevel.City;
                    return;
                }
                if (ze.FullCode.Length == Zone.ZONE_COUNTY_LENGTH || (ze.FullCode.Length > Zone.ZONE_COUNTY_LENGTH && ze.FullCode.Substring(Zone.ZONE_COUNTY_LENGTH) == "00000000"))
                {
                    ze.FullCode = ze.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
                    ze.UpLevelCode = ze.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH);
                    ze.Code = ze.FullCode.Substring(Zone.ZONE_CITY_LENGTH);
                    ze.Level = eZoneLevel.County;
                    return;
                }
                if (ze.FullCode.Length == Zone.ZONE_TOWN_LENGTH || (ze.FullCode.Length > Zone.ZONE_TOWN_LENGTH && ze.FullCode.Substring(Zone.ZONE_TOWN_LENGTH) == "00000"))
                {
                    ze.FullCode = ze.FullCode.Substring(0, Zone.ZONE_TOWN_LENGTH);
                    ze.UpLevelCode = ze.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
                    ze.Code = ze.FullCode.Substring(Zone.ZONE_COUNTY_LENGTH);
                    ze.Level = eZoneLevel.Town;
                    return;
                }
                if (ze.FullCode.Length == Zone.ZONE_VILLAGE_LENGTH || (ze.FullCode.Length > Zone.ZONE_VILLAGE_LENGTH && ze.FullCode.Substring(Zone.ZONE_VILLAGE_LENGTH) == "00"))
                {
                    ze.FullCode = ze.FullCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH);
                    ze.UpLevelCode = ze.FullCode.Substring(0, Zone.ZONE_TOWN_LENGTH);
                    ze.Code = ze.FullCode.Substring(Zone.ZONE_TOWN_LENGTH);
                    ze.Level = eZoneLevel.Village;
                    return;
                }
                if (ze.FullCode.Length == 14)
                {
                    ze.UpLevelCode = ze.FullCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH);
                    ze.Code = ze.FullCode.Substring(Zone.ZONE_VILLAGE_LENGTH);
                    ze.Level = eZoneLevel.Group;
                }
            });
            InitalizeAllZoneData(zones);//初始化除中国外的行政地域
            zones.ForEach(ze =>
            {
                var region = zones.Find(reg => reg.FullCode == ze.UpLevelCode);
                ze.UpLevelName = region != null ? region.FullName : "";
                ze.Name = (!string.IsNullOrEmpty(ze.UpLevelName) && ze.FullName.Length > ze.UpLevelName.Length) ? ze.FullName.Substring(ze.UpLevelName.Length) : ze.FullName;
            });
            //Zone china = InitalizeZoneChina();
            //zones.Add(china);//添加中国
        }

        /// <summary>
        /// 初始化所有地域数据
        /// </summary>
        /// <param name="zones">地域</param>
        private static void InitalizeAllZoneData(List<Zone> zones)
        {
            Zone county = zones.Find(ze => ze.FullCode.Length == Zone.ZONE_COUNTY_LENGTH);
            Zone provice = zones.Exists(ze => ze.Level == eZoneLevel.Province) ? null : InitalizeZoneProvice(county);
            Zone city = zones.Exists(ze => ze.Level == eZoneLevel.City) ? null : InitalizeZoneCity(provice, county);
            if (provice != null)
            {
                zones.Add(provice);//添加省级行政区
            }
            if (city != null)
            {
                zones.Add(city);//添加市级行政区
            }
        }

        /// <summary>
        /// 初始化中国
        /// </summary>
        /// <returns></returns>
        private static Zone InitalizeZoneChina()
        {
            Zone zone = new Zone();
            zone.ID = Guid.NewGuid();
            zone.Level = eZoneLevel.State;
            zone.Code = "86";
            zone.FullCode = "86";
            zone.UpLevelCode = "";
            zone.UpLevelName = "";
            zone.Name = "中国";
            zone.FullName = "中国";
            return zone;
        }

        /// <summary>
        /// 初始化行政区域等级数据
        /// </summary>
        /// <param name="zones">地域集合</param>
        /// <param name="level">等级</param>
        /// <returns>地域集合</returns>
        private static Zone InitalizeZoneProvice(Zone county)
        {
            if (county == null)
            {
                return null;
            }
            var dic = InitalizeProviceCode();
            var smallDic = InitalizeAbbreviationProvice();
            Zone zone = new Zone();
            zone.ID = Guid.NewGuid();
            zone.Level = eZoneLevel.Province;
            zone.FullCode = county.FullCode.Substring(0, Zone.ZONE_PROVICE_LENGTH);
            zone.UpLevelCode = "86";
            zone.UpLevelName = "中国";
            zone.Code = county.FullCode.Substring(0, Zone.ZONE_PROVICE_LENGTH);
            try
            {
                zone.FullName = dic[zone.Code];
                zone.Name = dic[zone.Code];
                zone.CreateUser = smallDic[zone.Name];
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                dic = null;
                smallDic = null;
                GC.Collect();
            }
            return zone;
        }

        /// <summary>
        /// 初始化行政区域等级数据
        /// </summary>
        /// <param name="zones">地域集合</param>
        /// <param name="level">等级</param>
        /// <returns>地域集合</returns>
        private static Zone InitalizeZoneCity(Zone provice, Zone county)
        {
            if (provice == null || county == null)
            {
                return null;
            }
            Zone zone = new Zone();
            zone.ID = Guid.NewGuid();
            zone.Level = eZoneLevel.City;
            zone.Code = county.FullCode.Substring(Zone.ZONE_PROVICE_LENGTH, 2);
            zone.FullCode = county.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH);
            zone.UpLevelCode = provice.FullCode;
            zone.UpLevelName = provice.FullName;
            string name = county.FullName;
            if (county.FullName.Length > provice.FullName.Length)
                name = county.FullName.Substring(provice.FullName.Length);
            int index = name.IndexOf("市");
            if (index < 0)
            {
                index = name.IndexOf("州");
            }
            if (index > 0)
            {
                zone.Name = name.Substring(0, index + 1);
            }
            zone.FullName = zone.UpLevelName + zone.Name;
            return zone;
        }

        /// <summary>
        /// 初始化省简写
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> InitalizeAbbreviationProvice()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("北京市", "京");
            dic.Add("天津市", "津");
            dic.Add("河北省", "冀");
            dic.Add("山西省", "晋");
            dic.Add("内蒙古自治区", "内蒙古");
            dic.Add("辽宁省", "辽");
            dic.Add("吉林省", "吉");
            dic.Add("黑龙江省", "黑");
            dic.Add("上海市", "沪");
            dic.Add("江苏省", "苏");
            dic.Add("浙江省", "浙");
            dic.Add("安徽省", "皖");
            dic.Add("福建省", "闽");
            dic.Add("江西省", "赣");
            dic.Add("山东省", "鲁");
            dic.Add("河南省", "豫");
            dic.Add("湖北省", "鄂");
            dic.Add("湖南省", "湘");
            dic.Add("广东省", "粤");
            dic.Add("广西壮族自治区", "桂");
            dic.Add("海南省", "琼");
            dic.Add("重庆市", "渝");
            dic.Add("四川省", "川");
            dic.Add("贵州省", "贵");
            dic.Add("云南省", "云");
            dic.Add("西藏自治区", "藏");
            dic.Add("陕西省", "陕");
            dic.Add("甘肃省", "甘");
            dic.Add("青海省", "青");
            dic.Add("宁夏回族自治区", "宁");
            dic.Add("新疆维吾尔自治区", "新");
            dic.Add("香港特别行政区", "港");
            dic.Add("澳门特别行政区", "澳");
            dic.Add("台湾省", "台");
            return dic;
        }

        /// <summary>
        /// 初始化省编码
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> InitalizeProviceCode()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("11", "北京市");
            dic.Add("12", "天津市");
            dic.Add("13", "河北省");
            dic.Add("14", "山西省");
            dic.Add("15", "内蒙古自治区");
            dic.Add("21", "辽宁省");
            dic.Add("22", "吉林省");
            dic.Add("23", "黑龙江省");
            dic.Add("31", "上海市");
            dic.Add("32", "江苏省");
            dic.Add("33", "浙江省");
            dic.Add("34", "安徽省");
            dic.Add("35", "福建省");
            dic.Add("36", "江西省");
            dic.Add("37", "山东省");
            dic.Add("41", "河南省");
            dic.Add("42", "湖北省");
            dic.Add("43", "湖南省");
            dic.Add("44", "广东省");
            dic.Add("45", "广西壮族自治区");
            dic.Add("46", "海南省");
            dic.Add("50", "重庆市");
            dic.Add("51", "四川省");
            dic.Add("52", "贵州省");
            dic.Add("53", "云南省");
            dic.Add("54", "西藏自治区");
            dic.Add("61", "陕西省");
            dic.Add("62", "甘肃省");
            dic.Add("63", "青海省");
            dic.Add("64", "宁夏回族自治区");
            dic.Add("65", "新疆维吾尔自治区");
            dic.Add("81", "香港特别行政区");
            dic.Add("82", "澳门特别行政区");
            dic.Add("71", "台湾省");
            return dic;
        }

        #endregion

        #region 地域数据合并

        /// <summary>
        /// 整合地域数据
        /// </summary>
        /// <returns>地域集合</returns>
        public static List<Zone> IntegrateZoneData(FilePathInfo fileEntry)
        {
            if (fileEntry == null)
            {
                return new List<Zone>();
            }
            List<Zone> vectors = ZoneVectorProgress(fileEntry);
            List<Zone> zones = ZoneAttributeProgress(fileEntry);
            if (zones == null)
                return null;
            zones.ForEach(ze =>
            {
                var zone = vectors.Find(reg => reg.FullCode == ze.FullCode);
                if (zone == null)
                {
                    return;
                }
                ze.Shape = zone.Shape;
            });
            vectors = null;
            GC.Collect();
            return zones;
        }

        #endregion
    }
}
