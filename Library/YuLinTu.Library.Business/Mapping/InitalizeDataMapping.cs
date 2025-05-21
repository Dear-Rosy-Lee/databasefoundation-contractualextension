/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using YuLinTu.Library.Entity;
using NetTopologySuite.IO;
using System.IO;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化数据
    /// </summary>
    public partial class InitalizeMappingData
    {
        #region 行政区域

        /// <summary>
        /// 遍历图层中所有实体
        /// </summary>
        private bool EnumArcGisZoneEntitys(YuLinTuDataComment data, ShapefileDataReader dataReader)
        {
            SpatialReference reference = ReferenceHelper.GetShapeReference(FileName);
            SortedList zoneCollection = new SortedList();
            while (dataReader.Read())
            {
                foreach (KeyValuePair<string, string> keyValue in data.Mapping)
                {
                    if (keyValue.Value == "None")
                    {
                        continue;
                    }
                    object val = dataReader.GetValue(data.RowValue);
                    YuLinTu.Spatial.Geometry geometry = YuLinTu.Spatial.Geometry.FromInstance(dataReader.Geometry);
                    geometry.SpatialReference = reference;
                    switch (keyValue.Key)
                    {
                        case "全编码":
                            if (!zoneCollection.Contains(val.ToString()))
                                zoneCollection.Add(val.ToString(), geometry);
                            break;
                    }
                }
            }
            data.CurrentObject = zoneCollection;
            return true;
        }


        #endregion
    }
}
