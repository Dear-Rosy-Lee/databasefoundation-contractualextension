/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 二轮台账数据参数类型
    /// </summary>
    public enum eSecondTableArgType
    {
        #region Fields

        /// <summary>
        /// 二轮台账摸底调查表
        /// </summary>
        RealQueryExcel = 1,

        /// <summary>
        /// 二轮台账摸底调查公示表
        /// </summary>
        PublicityExcel = 2,

        /// <summary>
        /// 二轮台账摸底调查公示确认表
        /// </summary>
        IdentifyExcel = 3,

        /// <summary>
        /// 二轮台账用户确认表
        /// </summary>
        UserIdentify = 4,

        /// <summary>
        /// 导入二轮台账调查表数据
        /// </summary>
        ImportData = 5,

        /// <summary>
        /// 导出勘界确权调查表
        /// </summary>
        ExportBoundarySettleExcel = 6,

        /// <summary>
        /// 导出单户确权调查表
        /// </summary>
        ExportSingleFamilyExcel = 7,

        #endregion
    }
}
