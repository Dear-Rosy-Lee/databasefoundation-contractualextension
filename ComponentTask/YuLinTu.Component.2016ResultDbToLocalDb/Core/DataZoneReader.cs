using Quality.Business.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 读取行政区域列表
    /// </summary>
    public class DataZoneReader : ExcelBase
    {
        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public DataZoneReader()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 写信息
        /// </summary>
        public override void Write()
        {
        }

        /// <summary>
        /// 读信息
        /// </summary>
        public override void Read()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化域信息
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        /// <param name="value">值</param>
        public List<Zone> InitalizeZoneData(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new List<Zone>();
            }
            try
            {
                bool canContinue = Open(fileName);
                if (!canContinue)
                {
                    return new List<Zone>();
                }
                object[,] allItem = GetAllRangeValue();//获取所有使用域值
                if (allItem == null)
                {
                    return new List<Zone>();
                }
                int rowCount = GetRangeRowCount();
                int columnCount = GetRangeColumnCount();
                if (rowCount < 2 || columnCount < 2)
                {
                    return new List<Zone>();
                }
                List<Zone> zones = new List<Zone>();
                for (int index = 1; index < rowCount; index++)
                {
                    Zone zone = new Zone();
                    zone.ID = Guid.NewGuid();
                    zone.FullName = GetString(allItem[index, 1]).TrimSafe();
                    zone.FullCode = GetString(allItem[index, 0]).TrimSafe();
                    if (string.IsNullOrEmpty(zone.FullName) || string.IsNullOrEmpty(zone.FullCode))
                    {
                        continue;
                    }
                    if (zones.Any(ze => ze.FullCode == zone.FullCode))
                    {
                        continue;
                    }
                    zones.Add(zone);
                }
                return zones;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return new List<Zone>();
        }

        #endregion

    }
}
