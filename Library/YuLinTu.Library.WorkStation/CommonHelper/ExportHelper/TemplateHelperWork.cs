/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 模板文件帮助类
    /// </summary>
    public class TemplateHelperWork
    {
        #region Methods

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public static string ExcelTemplate(string templateName)
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                string.Format(@"Template\{0}.xlt", templateName));
            if (!File.Exists(fileName))
                throw new Exception(string.Format("模板文件:{0}.xlt不存在", templateName));
            return fileName;
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public static string GetTemplate(string templateName)
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                string.Format(@"Template\{0}", templateName));
            if (!File.Exists(fileName))
                throw new Exception(string.Format("模板文件:{0}不存在", templateName));
            return fileName;
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public static string WordTemplate(string templateName)
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                string.Format(@"Template\{0}.dot", templateName));
            if (!File.Exists(fileName))
                throw new Exception(string.Format("模板文件:{0}.dot不存在", templateName));
            return fileName;
        }

        #endregion
    }
}