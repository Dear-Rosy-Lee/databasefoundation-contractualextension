/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using NetTopologySuite.IO;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化映射数据
    /// </summary>
    public partial class InitalizeMappingData
    {
        #region Fields

        private string information;

        #endregion

        #region Propertys

        /// <summary>
        /// 数据集合
        /// </summary>
        public YuLinTuDataCommentCollection DataCollection { get; set; }

        /// <summary>
        /// Shape读取
        /// </summary>
        public ShapefileDataReader DataReader { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        #endregion

        #region Ctor

        public InitalizeMappingData()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 开始创建数据
        /// </summary>
        public bool StartCreate()
        {
            information = string.Empty;
            bool canContinue = true;
            foreach (YuLinTuDataComment data in DataCollection)
            {
                if (!data.Checked || DataReader == null)
                {
                    continue;
                }
                string layName = !string.IsNullOrEmpty(FileName) ? System.IO.Path.GetFileNameWithoutExtension(FileName) : "";
                if (data.LayerName != layName)
                {
                    continue;
                }
                bool isRight = EnumAllDataEntitys(data, DataReader);
                if (!isRight)
                {
                    canContinue = false;
                }
            }
            DataReader.Close();
            return canContinue;
        }

        /// <summary>
        /// 遍历所有数据实体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="featureLayer"></param>
        private bool EnumAllDataEntitys(YuLinTuDataComment data, ShapefileDataReader featureLayer)
        {
            bool canContinue = true;
            switch (data.AliseName)
            {
                case Zone.YULINTUZONESTRING:
                    canContinue = EnumArcGisZoneEntitys(data, featureLayer);
                    break;
                //case YuLinTuLand.YULINTULANDSTRING:
                //    canContinue = EnumArcLandComplexEntitys(data, featureLayer);
                //    break;
                //case YuLinTuRoad.YULINTUROADSTRING:
                //    canContinue = EnumArcRoadEntitys(data, featureLayer);
                //    break;
                //case YuLinTuWater.YULINTUWATERSTRING:
                //    canContinue = EnumArcWaterEntitys(data, featureLayer);
                //    break;
                //case YuLinTuLandSpot.YULINTULANDSPOTSTRING:
                //    canContinue = EnumArcLandSpotEntitys(data, featureLayer);
                //    break;
                //default:
                //    break;
            }
            return canContinue;
        }

        /// <summary>
        /// 初始化Shape文件字段名称
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private Dictionary<int, string> InitalizeShapeFileField(ShapefileDataReader dataReader)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();
            for (int index = 0; index < dataReader.FieldCount; index++)
            {
                list.Add(index, dataReader.GetName(index));
            }
            return list;
        }

        /// <summary>
        /// 遍历字段中字段名称相同的索引值
        /// </summary>
        /// <returns></returns>
        private int EnumeratorFieldNameIndex(Dictionary<int, string> fieldList, string fieldName)
        {
            int fieldIndex = -1;
            foreach (KeyValuePair<int, string> dic in fieldList)
            {
                if (dic.Value == fieldName)
                {
                    fieldIndex = dic.Key;
                    break;
                }
            }
            return fieldIndex;
        }

        #endregion

    }
}
