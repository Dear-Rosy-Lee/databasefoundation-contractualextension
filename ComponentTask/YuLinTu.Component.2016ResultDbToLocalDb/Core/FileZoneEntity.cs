/*
 * (C) 2014-2015 鱼鳞图公司版权所有，保留所有权利
 */
using Quality.Business.TaskBasic;
using YuLinTu.Library.Controls;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 文件/地域设置实体
    /// </summary>
    public partial class FileZoneEntity
    {
        #region Propertys

        /// <summary>
        /// 地域选择
        /// </summary>
        public ZoneDataItem ZoneInfo { get; set; }

        /// <summary>
        /// 根级地域选择
        /// </summary>
        public ZoneDataItem RootZoneInfo { get; set; }

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

        public FileZoneEntity()
        {
            ImportFile = new ImportFileEntity();
            IsSelectZone = false;
            IsImportBusinessData = true;
        }

        #endregion
    }
}
