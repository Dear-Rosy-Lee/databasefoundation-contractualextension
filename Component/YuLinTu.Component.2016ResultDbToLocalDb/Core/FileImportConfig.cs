/*
 * (C) 2016 鱼鳞图公司版权所有，保留所有权利
 */
using Quality.Business.TaskBasic;
using System.Collections.Generic;
using System.IO;
using YuLinTu.Library.Controls;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    /// <summary>
    /// 文件/地域设置实体
    /// </summary>
    public class FileImportConfig : CDObject
    {
        #region Propertys

        /// <summary>
        /// 地域选择
        /// </summary>
        public ZoneSelectInfo ZoneInfo { get; set; }

        /// <summary>
        /// 根级地域选择
        /// </summary>
        public ZoneSelectInfo RootZoneInfo { get; set; }

        /// <summary>
        /// 导入文件配置
        /// </summary>
        public ImportFileEntity ImportFile { get; set; }

        /// <summary>
        /// 是否选择地域
        /// </summary>
        public bool IsSelectZone { get; set; }

        /// <summary>
        /// 是否导入业务数据
        /// </summary>
        public bool IsImportBusinessData { get; set; }

        #endregion

        #region Ctor

        public FileImportConfig()
        {
            ImportFile = new ImportFileEntity();
            IsSelectZone = false;
            IsImportBusinessData = true;
        }

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
            var extentFileList = new List<FileCondition>();
            var fileList = FilePathManager.CreateNamePair(fileArray, extentName, extentFileList);
            var rightList = new List<FileCondition>();
            foreach (FileCondition bs in ShapeFileInfo.TableNameList)
            {
                var entity = fileList.FindAll(s => s.FullName.ToLower().StartsWith(bs.Name.ToLower() + extentName));
                if (entity != null && entity.Count > 0)
                {
                    entity.ForEach(t =>
                    {
                        fileList.Remove(t);
                        if (t.Name.Contains("-"))
                        {
                            int idx = t.Name.IndexOf("-");
                            if (idx > 0)
                            {
                                t.Name = t.Name.Substring(0, idx);
                            }
                        }
                    });
                    rightList.AddRange(entity);
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

        #endregion

        #region Override

        public override object Clone()
        {
            var fic = new FileImportConfig();
            fic.CopyPropertiesFrom(this);
            if (fic.ZoneInfo != null)
                fic.ZoneInfo = this.ZoneInfo.Clone() as ZoneSelectInfo;
            if (fic.RootZoneInfo != null)
                fic.RootZoneInfo = this.RootZoneInfo.Clone() as ZoneSelectInfo;
            if (fic.ImportFile != null)
                fic.ImportFile = this.ImportFile as ImportFileEntity;
            return fic;
        }

        public override bool Equals(object obj)
        {
            bool equal = true;
            var fic = obj as FileImportConfig;
            if (fic.IsImportBusinessData != this.IsImportBusinessData ||
                fic.IsSelectZone != this.IsSelectZone)
                equal = false;
            if (fic.ImportFile.Equals(this.ImportFile))
                equal = false;
            return equal;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
