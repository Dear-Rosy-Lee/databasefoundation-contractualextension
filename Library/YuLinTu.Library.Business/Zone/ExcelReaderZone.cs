/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using System.Collections;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 读取行政区域信息
    /// </summary>
    [Serializable]
    public class ExcelReaderZone : ExcelBase
    {
        #region Propertys

        /// <summary>
        /// 错误信息
        /// </summary>
        public ArrayList ErrorList { get; set; }

        public List<CollectivityTissue> Tissues { get; set; }

        #endregion Propertys

        #region

        public List<CollectivityTissue> GetTissuesValue()
        {
            return Tissues;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<Zone> GetZoneValue()
        {
            ErrorList = new ArrayList();
            List<Zone> zoneList = new List<Zone>();
            Tissues = new List<CollectivityTissue>();
            object[,] ranges = GetAllRangeValue(4);

            int count = GetRangeRowCount();
            if(ranges[0, 2] != null && ranges[0, 3] != null)
            {
                if (ranges[0, 2].ToString() == "原区域代码" && ranges[0, 3].ToString() == "原区域名称")
                {
                    for (int i = 1; i < count; i++)
                    {
                        if (ranges[i, 0] == null || ranges[i, 0].ToString().Length < 1)
                            ranges[i, 0] = "";
                        if (ranges[i, 1] == null || ranges[i, 1].ToString().Length < 1)
                            ranges[i, 1] = "";
                        if (ranges[i, 2] == null || ranges[i, 2].ToString().Length < 1)
                            ranges[i, 2] = "";
                        if (ranges[i, 3] == null || ranges[i, 3].ToString().Length < 1)
                            ranges[i, 3] = "";

                        string tissueKey = ranges[i, 0].ToString().TrimSafe();
                        string tissueValue = ranges[i, 1].ToString();
                        string zoneKey = ranges[i, 2].ToString();
                        string zoneValue = ranges[i, 3].ToString();

                        if (zoneKey == "合计" || zoneKey == "共计" || zoneKey == "总计")
                        {
                            break;
                        }
                        if (Tissues.Any(t => t.Code == tissueKey))
                        {
                            if (!string.IsNullOrEmpty(tissueKey))
                            {
                                ErrorList.Add(string.Format("第{0}行数据发包方编码{1}在表中重复存在!", i + 1, tissueKey));
                            }
                        }
                        if (zoneList.Any(t => t.FullCode == zoneKey))
                        {
                            if (!string.IsNullOrEmpty(zoneKey))
                            {
                                ErrorList.Add(string.Format("第{0}行数据地域编码{1}在表中重复存在!", i + 1, zoneKey));
                            }
                        }
                        else
                        {
                            if (tissueKey != string.Empty || tissueValue != string.Empty)
                            {
                                Tissues.Add(new CollectivityTissue() { Code = tissueKey, Name = tissueValue });
                            }
                            if (zoneKey != string.Empty || zoneValue != string.Empty)
                            {
                                zoneList.Add(new Zone() { FullCode = zoneKey, Name = zoneValue });
                            }
                        }
                        if (string.IsNullOrEmpty(zoneKey))
                        {
                            ErrorList.Add(string.Format("第{0}行数据地域编码为空!", i + 1));
                        }
                        if (string.IsNullOrEmpty(zoneValue))
                        {
                            ErrorList.Add(string.Format("第{0}行数据地域名称为空!", i + 1));
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i < count; i++)
                {
                    if (ranges[i, 0] == null || ranges[i, 0].ToString().Length < 1)
                        ranges[i, 0] = "";
                    if (ranges[i, 1] == null || ranges[i, 1].ToString().Length < 1)
                        ranges[i, 1] = "";
                    if (ranges[i, 2] == null || ranges[i, 2].ToString().Length < 1)
                        ranges[i, 2] = "";
                    if (ranges[i, 3] == null || ranges[i, 3].ToString().Length < 1)
                        ranges[i, 3] = "";
                    string key = ranges[i, 0].ToString().TrimSafe();
                    string value = ranges[i, 1].ToString();
                    string bkey = ranges[i, 2].ToString();
                    string bvalue = ranges[i, 3].ToString();
                    if (key == "合计" || key == "共计" || key == "总计")
                    {
                        break;
                    }
                    if (zoneList.Any(t => t.FullCode == key))
                    {
                        if (!string.IsNullOrEmpty(key))
                        {
                            ErrorList.Add(string.Format("第{0}行数据地域编码{1}在表中重复存在!", i + 1, key));
                        }
                    }
                    else
                    {
                        zoneList.Add(new Zone() { FullCode = key, Name = value, AliasName = bvalue, AliasCode = bkey });
                    }
                    if (string.IsNullOrEmpty(key))
                    {
                        ErrorList.Add(string.Format("第{0}行数据地域编码为空!", i + 1));
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        ErrorList.Add(string.Format("第{0}行数据地域名称为空!", i + 1));
                    }
                }
            }
            count = 0;
            ranges = null;
            return zoneList;
        }

        /// <summary>
        /// 获取单元格数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public string GetValue(string start, string end)
        {
            object[,] value = GetRangeToValue(start, end) as object[,];
            if (value == null || value[1, 1] == null)
            {
                return "";
            }
            string cellValue = value[1, 1].ToString();
            value = null;
            return cellValue;
        }

        #endregion

        #region Override

        /// <summary>
        /// 读取数据
        /// </summary>
        public override void Read()
        {
        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {
        }

        #endregion
    }
}