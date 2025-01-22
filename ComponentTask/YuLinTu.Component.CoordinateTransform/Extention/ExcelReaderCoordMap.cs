using System;
using System.Collections.Generic;

namespace YuLinTu.Component.CoordinateTransformTask
{
    [Serializable]
    public class ExcelReaderCoordMap : ExcelBase
    {
        /// <summary>
        /// 获取导入Excel文件的数据信息
        /// </summary>
        /// <param name="fileName"></param>
        public List<CoordinateMap> ReadCoordInfo(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            Open(fileName);
            return GetValue();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        private List<CoordinateMap> GetValue()
        {
            //try
            //{
            List<CoordinateMap> CoordMaps = new List<CoordinateMap>();
            object[,] ranges = GetAllRangeValue(6);
            int count = GetRangeRowCount();
            for (int i = 2; i < count; i++)
            {
                if (ranges[i, 1] == null ||
                    !ranges[i, 1].ToString().StartsWith("CGCS2000") &&
                    !ranges[i, 1].ToString().StartsWith("Beijing") &&
                    !ranges[i, 1].ToString().StartsWith("Xian"))
                    continue;

                var coord = new CoordinateMap();
                coord.EPSG = Convert.ToInt32(ranges[i, 0]);
                coord.CoordinateName = ranges[i, 1].ToString();
                coord.MinLongitude = Convert.ToDecimal(ranges[i, 2]);
                coord.MaxLongitude = Convert.ToDecimal(ranges[i, 3]);
                coord.CentralMeridian = Convert.ToDecimal(ranges[i, 4]);
                coord.Remark = ranges[i, 5]?.ToString();

                CoordMaps.Add(coord);
            }

            return CoordMaps;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.ToString());
            //}
        }

        #region Override

        /// <summary>
        /// 读取数据
        /// </summary>
        public override void Read()
        { }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        { }
        #endregion
    }
}
