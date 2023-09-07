/*
 * (C) 2014 鱼鳞图公司版权所有,保留所有权利
*/
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
//using YuLinTu.Business.Achievement;
using YuLinTu.Data;
using YuLinTuQuality.Business.Entity;
using YuLinTuQuality.Business.TaskBasic;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    /// <summary>
    /// 数据检查类
    /// </summary>
    public class DataInfoCheck : Task
    {
        #region Fields

        #endregion

        #region Property

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 成果信息
        /// </summary>
        public GainInfo Gain { get; set; }

        /// <summary>
        /// 数据路径
        /// </summary>
        public FilePathInfo CurrentPath { get; private set; }

        /// <summary>
        /// 容差值
        /// </summary>
        public double VolumeValue { get; set; }

        #endregion

        #region Ctor

        public DataInfoCheck()
        { }

        #endregion

        #region Methods
               

        /// <summary>
        /// 获取当前文件路径
        /// </summary>
        public static FilePathInfo GetCurrent(string filePath, string extentName)
        {
            FilePathInfo currentPath = FilePathManager.GetCurrentPath(filePath);
            FilePathManager fpm = new FilePathManager();
            string victorName = fpm.VictorName;
            string categoryName = fpm.CategoryName;
            CheckVictorFolder(currentPath.VictorFilePath, victorName, extentName, currentPath);
            CheckBaseFolder(currentPath.ThroneFilePath, categoryName, extentName, currentPath);
            fpm = null;
            return currentPath;
        }

        /// <summary>
        /// 获取矢量文件路径
        /// </summary>
        private static void CheckVictorFolder(string folderPath, string folderName, string extentName, FilePathInfo currentPath)
        {
            string[] fileArray = Directory.GetFiles(folderPath);
            List<FileCondition> extentFileList = new List<FileCondition>();
            List<FileCondition> fileList = FilePathManager.CreateNamePair(fileArray, extentName, extentFileList);
            List<FileCondition> rightList = new List<FileCondition>();
            foreach (FileCondition bs in ShapeFileInfo.TableNameList)
            {
                FileCondition entity = fileList.Find(s => s.Name == bs.Name);
                if (entity != null)
                {
                    fileList.Remove(entity);
                    rightList.Add(entity);
                }
            }
            currentPath.ShapeFileList = rightList;
            fileList = null;
        }

        /// <summary>
        /// 检查属性数据目录
        /// </summary>
        private static void CheckBaseFolder(string folderPath, string folderName, string extentName, FilePathInfo currentPath)
        {
            string dataBasefilePath = string.Empty;
            string[] files = Directory.GetFiles(folderPath);
            foreach (string path in files)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                string fullname = Path.GetFileName(path);
                if (path.ToLower().LastIndexOf(".mdb") > 0 && name.ToLower().Equals(extentName))
                {
                    dataBasefilePath = path;
                }
            }
            currentPath.DataBasePath = dataBasefilePath;
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        private void Report_ProgressChanged(object sender, TaskProgressChangedEventArgs e)
        {
            this.ReportProgress(e);
        }

        /// <summary>
        /// 信息报告
        /// </summary>
        private void Report_Alert(object sender, TaskAlertEventArgs e)
        {
            this.ReportAlert(e);
        }

        #endregion
    }
}