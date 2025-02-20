/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Unity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Office;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据汇总导出扩展类
    /// </summary>
    public class DataSummarySelectPersonsExtend
    {
        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static bool SerializeSelectedInfo(List<PersonSelectedInfo> infos)
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\DataSummarySelectPersons.xml";
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                ToolSerialization.SerializeXml(fileName, infos);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static List<PersonSelectedInfo> DeserializeSelectedInfo()
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\DataSummarySelectPersons.xml";
                if (!File.Exists(fileName))
                {
                    return new List<PersonSelectedInfo>();
                }
                List<PersonSelectedInfo> infos = ToolSerialization.DeserializeXml(fileName, typeof(List<PersonSelectedInfo>)) as List<PersonSelectedInfo>;
                if (infos == null)
                    return new List<PersonSelectedInfo>();
                else
                {
                    return infos;
                }
            }
            catch
            {
                return new List<PersonSelectedInfo>();
            }
        }

    }
}
