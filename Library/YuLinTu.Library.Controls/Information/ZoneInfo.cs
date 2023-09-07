/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 地域提示信息定义
    /// </summary>
    public class ZoneInfo
    {
        #region Files - Const

        /// <summary>
        /// 删除地域
        /// </summary>
        public const string ZoneDel = "删除地域";

        /// <summary>
        /// 添加地域
        /// </summary>
        public const string ZoneAdd = "添加地域";

        /// <summary>
        /// 上传地域
        /// </summary>
        public const string ZoneUpService = "上传地域";

        /// <summary>
        /// 上传地域服务地址为空
        /// </summary>
        public const string ZoneNoServiceAddress = "上传地域服务地址为空!";

        /// <summary>
        /// 确定上传地域
        /// </summary>
        public const string ZoneUpServiceMessage = "确定上传地域数据?";

        /// <summary>
        /// 选择上传的根级地域
        /// </summary>
        public const string ZoneUpServiceRoot = "请选择上传的根级地域";

        /// <summary>
        /// 添加的地域编码不能为空
        /// </summary>
        public const string ZoneCodeNull = "地域编码不能为空!";

        /// <summary>
        /// 地域编码已存在
        /// </summary>
        public const string ZoneCodeExist = "该地域编码已在系统中存在!";

        /// <summary>
        /// 地域名称已存在
        /// </summary>
        public const string ZoneNameExist = "该地域名称已在系统中存在,是否继续添加?";

        /// <summary>
        /// 地域名称已存在，继续修改
        /// </summary>
        public const string ZoneNameExistEdit = "该地域名称已在系统中存在,是否继续修改?";

        /// <summary>
        /// 添加无上级地域
        /// </summary>
        public const string AddNoUp = "请选择添加数据所在的上级地域!";

        /// <summary>
        /// 添加组下级地域
        /// </summary>
        public const string AddGroup = "组级区域不能再添加子级地域!";

        /// <summary>
        /// 添加地域数据失败
        /// </summary>
        public const string AddDataFaile = "添加地域数据失败!";

        /// <summary>
        /// 上传地域数据失败
        /// </summary>
        public const string UpServiceDataFaile = "上传地域数据失败!";

        /// <summary>
        /// 上传地域数据成功
        /// </summary>
        public const string UpServiceDataSuccess = "上传地域数据成功!";

        /// <summary>
        /// 删除地域数据失败
        /// </summary>
        public const string DelDataFaile = "删除地域数据失败!";

        /// <summary>
        /// 导出图斑
        /// </summary>
        public const string ExportShape = "导出地域图斑";

        /// <summary>
        /// 导出地域数据
        /// </summary>
        public const string ExportData = "导出地域数据";

        /// <summary>
        /// 导出地域数据描述
        /// </summary>
        public const string ExportDataComment = "导出{0}下所有地域数据";

        /// <summary>
        /// 导出地域压缩包
        /// </summary>
        public const string ExportPackage = "导出地域压缩包";

        /// <summary>
        /// 导出地域压缩包描述
        /// </summary>
        public const string ExportPackageComment = "导出{0}下地域数据压缩包";

        /// <summary>
        /// 导入地域调查表
        /// </summary>
        public const string ImportData = "导入地域调查表";

        /// <summary>
        /// 导入地域调查表描述
        /// </summary>
        public const string ImportDataComment = "导入地域调查表中地域数据";

        /// <summary>
        /// 导入地域图斑
        /// </summary>
        public const string ImportShape = "导入地域图斑";

        /// <summary>
        /// 导入地域图斑描述
        /// </summary>
        public const string ImportShapeComment = "导入地域Shape文件中地域数据";

        /// <summary>
        /// 导出地域图斑无上级
        /// </summary>
        public const string ExportShapeNoUp = "请选择导出地域图斑的根级地域!";

        /// <summary>
        /// 导入地域图斑无上级
        /// </summary>
        public const string ExportShapeComment = "导出{0}下所有地域图斑数据";

        /// <summary>
        /// 导出地域数据无上级
        /// </summary>
        public const string ExportDataNoUp = "请选择导出地域数据的根级地域!";

        /// <summary>
        /// 导出地域压缩包无上级
        /// </summary>
        public const string ExportPackageNoUp = "请选择导出地域压缩包的根级地域!";

        /// <summary>
        /// 删除地域
        /// </summary>
        public const string DelNull = "请选择一个地域进行删除!";

        /// <summary>
        /// 删除顶级地域
        /// </summary>
        public const string DelTop = "最高行政地域不能删除";

        /// <summary>
        /// 删除包含子集
        /// </summary>
        public const string DelContainsChildren = "该地域下已有子集地域不能删除!";

        /// <summary>
        /// 删除存在关联
        /// </summary>
        public const string DelContainsRelation = "该地域下存在关联数据,请先删除其关联数据后再进行该操作!";

        /// <summary>
        /// 编辑地域数据
        /// </summary>
        public const string ZoneEdit = "编辑地域";

        /// <summary>
        /// 编辑地域数据为空
        /// </summary>
        public const string EditNull = "请选择需要编辑的地域!";

        /// <summary>
        /// 清空地域
        /// </summary>
        public const string Clear = "清空地域";

        /// <summary>
        /// 清空确认
        /// </summary>
        public const string ClearConfirm = "确定清空地域数据?";

        /// <summary>
        /// 清空数据失败
        /// </summary>
        public const string ClearFaile = "清空数据失败!";

        /// <summary>
        /// 清空图斑成功
        /// </summary>
        public const string ClearShapeSuccess = "清空图斑数据成功!";

        /// <summary>
        /// 清空图斑确认
        /// </summary>
        public const string ClearShapeConfirm = "确定清空地域图斑数据?";

        #endregion
    }
}
