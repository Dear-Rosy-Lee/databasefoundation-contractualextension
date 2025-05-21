/*
 * (C) 2017 鱼鳞图公司版权所有,保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu;
using YuLinTu.Library.Office;

namespace YuLinTu.Component.AssociateLandCode
{
    /// <summary>
    /// 读取行政区域列表
    /// </summary>
    public class ZoneExcelReader : ExcelBase
    {
        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ZoneExcelReader()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 写信息
        /// </summary>
        public override void Write() { }

        /// <summary>
        /// 读信息
        /// </summary>
        public override void Read() { }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化域信息
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="fbfZones">发包方行政地域</param>
        /// <returns></returns>
        public List<RelationZone> InitalizeZoneData(string fileName, bool readextent = false)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new List<RelationZone>();
            }
            bool canContinue = Open(fileName);
            if (!canContinue)
            {
                return new List<RelationZone>();
            }
            object[,] allItem = GetAllRangeValue();//获取所有使用域值
            if (allItem == null)
            {
                return new List<RelationZone>();
            }
            int startindex = 0;
            int rowCount = GetRangeRowCount();
            int columnCount = GetRangeColumnCount();
            if (rowCount < 2 || columnCount < 2)
            {
                return new List<RelationZone>();
            }

            var zones = new List<RelationZone>();
            for (int index = 1; index < rowCount; index++)
            {
                var zone = new RelationZone();
                zone.OldName = GetString(allItem[index, startindex + 1]).TrimSafe();
                zone.OldZoneCode = GetString(allItem[index, startindex]).TrimSafe();
                if (columnCount == 4)
                {
                    zone.NewZoneCode = GetString(allItem[index, startindex + 2]).TrimSafe();
                    zone.NewName = GetString(allItem[index, startindex + 3]).TrimSafe();
                }
                if (string.IsNullOrEmpty(zone.NewName) || string.IsNullOrEmpty(zone.OldZoneCode))
                {
                    continue;
                }
                if (!zones.Any(ze => ze.OldZoneCode == zone.OldZoneCode))
                {
                    zones.Add(zone);
                }
            }
            return zones;
        }
        #endregion

    }

    /// <summary>
    /// 地域关系映射表对象
    /// </summary>
    public class RelationZone
    {
        public string OldZoneCode { get; set; }
        public string NewZoneCode { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
    }
}
