namespace YuLinTu.Component.BatchDataBaseTask
{
    /// <summary>
    /// 导出使用的中间实体
    /// </summary>
    internal class ExportEntity
    {
        /// <summary>
        /// 承包方姓名
        /// </summary>
        public string ContractorName { get; set; }
        /// <summary>
        /// 地块编码
        /// </summary>
        public string LandNumber { get; set; }
        /// <summary>
        /// 地域编码
        /// </summary>
        public string ZoneCode { get; set; }
        /// <summary>
        /// shape数据
        /// </summary>
        public YuLinTu.Spatial.Geometry Shape { get; set; }
    }
}