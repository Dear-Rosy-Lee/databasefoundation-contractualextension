/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账模块消息名称定义
    /// </summary>
    public class ContractAccountMessage
    {
        #region Fields-Const

        /// <summary>
        /// 承包地块添加完成
        /// </summary>
        public const string CONTRACTLAND_ADD_COMPLETE = "ContractLand_Add_Complete";

        /// <summary>
        /// 承包地块编辑完成
        /// </summary>
        public const string CONTRACTLAND_EDIT_COMPLETE = "ContractLand_Edit_Complete";

        /// <summary>
        /// 承包地块删除完成
        /// </summary>
        public const string CONTRACTLAND_DELETE_COMPLETE = "ContractLand_Delete_Complete";

        /// <summary>
        /// 导入地块数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_IMPORT_COMPLETE = "ContractAccount_Import_Complate";

        /// <summary>
        /// 导入地块shp数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_IMPORTSHP_COMPLETE = "ContractAccount_ImportShp_Complate";

        /// <summary>
        /// 导入压缩包数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_IMPORTZIP_COMPLATE = "ContractAccount_ImportZip_Complate";

        /// <summary>
        /// 导入界址数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_IMPORTBOUNDARY_COMPLETE = "ContractAccount_ImportBoundary_Complate";

        /// <summary>
        /// 批量导入界址数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_MULTIIMPORTBOUNDARY_COMPLETE = "ContractAccount_MultiImportBoundary_Complate";

        /// <summary>
        /// 清空数据完成(需要清空承包方)
        /// </summary>
        public const string CONTRACTACCOUNT_CLEARLANDANDPERSON_COMPLETE = "ContractAccount_ClearLandAndPerson_Complate";

        /// <summary>
        /// 清空数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_CLEAR_COMPLETE = "ContractAccount_Clear_Complate";

        /// <summary>
        /// 初始化数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_INITIALLAND_COMPLETE = "ContractAccount_InitialLand_Complate";

        /// <summary>
        /// 初始化地块面积数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_INITIALAREA_COMPLETE = "ContractAccount_InitialArea_Complate";

        /// <summary>
        /// 初始化地块是否基本农田
        /// </summary>
        public const string CONTRACTACCOUNT_INITIALISFARMER_COMPLETE = "ContractAccount_InitialIsFarmer_Complate";

        /// <summary>
        /// 初始化界址点界址线
        /// </summary>
        public const string CONTRACTACCOUNT_INITIALDOTCOIL_COMPLETE = "ContractAccount_Initialdotcoil_Complate";

        /// <summary>
        /// 查找四至
        /// </summary>
        public const string CONTRACTACCOUNT_SEEKLANDNEIGHBOR_COMPLETE = "ContractAccount_SeekLandNeighbor_Complate";

        /// <summary>
        /// 空间查看地块
        /// </summary>
        public const string CONTRACTACCOUNT_FINDLANDS_COMPLETE = "ContractAccount_FindLands_Complate";

        /// <summary>
        /// 空间查看地块界址点
        /// </summary>
        public const string CONTRACTACCOUNT_FINDDOT_COMPLETE = "ContractAccount_FindDot_Complete";

        /// <summary>
        /// 空间查看地块坐标点
        /// </summary>
        public const string CONTRACTACCOUNT_FINDCOORDINATE_COMPLETE = "ContractAccount_FindCoordinate_Complete";

        /// <summary>
        /// 获取鱼鳞图地图模块工作页
        /// </summary>
        public const string CONTRACTACCOUNT_GETMAPPAGE = "ContractAccount_GetMapPage";

        /// <summary>
        /// 导入界址点图斑数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE = "ContractAccount_ImportDotShape_Complete";

        /// <summary>
        /// 导出地块图斑数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_EXPORTLANDSHAPE_COMPLETE = "ContractAccount_ExportLandShape_Complete";

        /// <summary>
        /// 导出地块界址点数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_EXPORTLANDDOTSHAPE_COMPLETE = "ContractAccount_ExportLandDotShape_Complete";

        /// <summary>
        /// 导出地块界址线数据完成
        /// </summary>
        public const string CONTRACTACCOUNT_EXPORTLANDCOILSHAPE_COMPLETE = "ContractAccount_ExportLandCoilShape_Complete";

        /// <summary>
        /// 初始化图幅编号
        /// </summary>
        public const string CONTRACTACCOUNT_INITIALIMAGENUMBER_COMPLETE = "ContractAccount_InitialImageNumber_Complete";

        /// <summary>
        /// 编辑图形结束
        /// </summary>
        public const string CONTRACTACCOUNT_EDITGEOMETRY_COMPLETE = "ContractAccount_EditGemetry_Complete";

        /// <summary>
        /// 刷新界面
        /// </summary>
        public const string CONTRACTACCOUNT_Refresh = "ContractAccount_Refresh";

        #endregion
    }
}
