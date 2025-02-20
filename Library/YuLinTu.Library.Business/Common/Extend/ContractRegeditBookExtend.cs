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
    /// 承包权证扩展类
    /// </summary>
    public class ContractRegeditBookExtend
    {
        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static bool SerializeSelectedSetInfo(ContractRegeditBookPreviewSetInfo info)
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\ContractRegeditBookPreviewSetInfo.xml";
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                ToolSerialization.SerializeXml(fileName, info);
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
        public static ContractRegeditBookPreviewSetInfo DeserializeSelectedSetInfo()
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\ContractRegeditBookPreviewSetInfo.xml";
                if (!File.Exists(fileName))
                {
                    return new ContractRegeditBookPreviewSetInfo();
                }
                ContractRegeditBookPreviewSetInfo info = ToolSerialization.DeserializeXml(fileName, typeof(ContractRegeditBookPreviewSetInfo)) as ContractRegeditBookPreviewSetInfo;
                if (info == null)
                    return new ContractRegeditBookPreviewSetInfo();
                else
                {
                    return info;
                }
            }
            catch
            {
                return new ContractRegeditBookPreviewSetInfo();
            }
        }
    }
}
