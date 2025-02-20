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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地域模块消息定义
    /// </summary>
    public class ZoneMessage
    {
        #region Const

        /// <summary>
        /// 添加地域
        /// </summary>
        public const string ZONE_ADD = "Zone_Add";

        /// <summary>
        /// 添加地域完成
        /// </summary>
        public const string ZONE_ADD_COMPLETE = "Zone_Add_Complete";

        /// <summary>
        /// 更新地域
        /// </summary>
        public const string ZONE_UPDATE = "Zone_Update";

        /// <summary>
        /// 更新地域完成
        /// </summary>
        public const string ZONE_UPDATE_COMPLETE = "Zone_Update_Complete";

        /// <summary>
        /// 删除地域
        /// </summary>
        public const string ZONE_DELETE = "Zone_Delete";

        /// <summary>
        /// 删除地域完成
        /// </summary>
        public const string ZONE_DELETE_COMPLETE = "Zone_Delete_Complete";

        /// <summary>
        /// 导入地域调查表
        /// </summary>
        public const string ZONE_IMPORTTABLE = "Zone_ImportTalbe";

        /// <summary>
        /// 导入地域调查表完成
        /// </summary>
        public const string ZONE_IMPORTTABLE_COMPLETE = "Zone_ImportTalbe_Complete";

        /// <summary>
        /// 导入地域图斑
        /// </summary>
        public const string ZONE_IMPORTSHAPE = "Zone_ImportShape";

        /// <summary>
        /// 导入地域图斑完成
        /// </summary>
        public const string ZONE_IMPORTSHAPE_COMPLETE = "Zone_ImportShape_Complete";

        /// <summary>
        /// 导出地域调查表
        /// </summary>
        public const string ZONE_EXPORTTABLE = "Zone_ExportTalbe";

        /// <summary>
        /// 导出地域调查表完成
        /// </summary>
        public const string ZONE_EXPORTTABLE_COMPLETE = "Zone_ExportTalbe_Complete";

        /// <summary>
        /// 导出地域图斑
        /// </summary>
        public const string ZONE_EXPORTSHAPE = "Zone_ExportShape";

        /// <summary>
        /// 导出地域图斑完成
        /// </summary>
        public const string ZONE_EXPORTSHAPE_COMPLETE = "Zone_ExportShape_Complete";

        /// <summary>
        /// 导出地域压缩包
        /// </summary>
        public const string ZONE_EXPORTPACKAGE = "Zone_ExportPackage";

        /// <summary>
        /// 导出地域压缩包完成
        /// </summary>
        public const string ZONE_EXPORTPACKAGE_COMPLETE = "Zone_ExportPackage_Complete";

        /// <summary>
        /// 清空地域
        /// </summary>
        public const string ZONE_CLEAR = "Zone_Clear";

        /// <summary>
        /// 清空地域完成
        /// </summary>
        public const string ZONE_CLEAR_COMPLETE = "Zone_Clear_Complete";

        /// <summary>
        /// 刷新
        /// </summary>
        public const string ZONE_REFRESH = "Zone_Refresh";

        /// <summary>
        /// 获取数据
        /// </summary>
        public const string ZONE_GETDATA = "Zone_GetData";

        /// <summary>
        /// 获取指定编码的地域
        /// </summary>
        public const string ZONE_GET = "Zone_Get";

        /// <summary>
        /// 获取所有数据
        /// </summary>
        public const string ZONE_GETALLDATA = "Zone_GetAllData";

        /// <summary>
        /// 地域下存在承包方
        /// </summary>
        public const string ZONE_EXIST_VIRTUALPERSON = "Zone_Exist_VirtualPerson";

        /// <summary>
        /// 地域下存在农用地
        /// </summary>
        public const string ZONE_EXIST_AGRICULTURELAND = "Zone_Exist_AgricultureLand";

        /// <summary>
        /// 地域下存在宅基地
        /// </summary>
        public const string ZONE_EXIST_HOMESTEADLAND = "Zone_Exist_HomeSteadLand";

        /// <summary>
        /// 地域下存在建设用地
        /// </summary>
        public const string ZONE_EXIST_CONSTRUCTIONLAND = "Zone_Exist_ConstructionLand";

        /// <summary>
        /// 地域下存在集体土地
        /// </summary>
        public const string ZONE_EXIST_COLLECTIVELAND = "Zone_Exist_CollectiveLand";

        /// <summary>
        /// 当前地域的父级地域(县级)
        /// </summary>
        public const string ZONE_PARENTS_ZONE = "Zone_Parents_Zone";

        /// <summary>
        /// 当前地域的父级地域(省级)
        /// </summary>
        public const string ZONE_PARENTS_TOPROVINCEZONE = "Zone_Parents_ToProvinceZone";

        /// <summary>
        /// 当前地域的父级地域(镇级)
        /// </summary>
        public const string ZONE_PARENTSTOTOWN_ZONE = "Zone_ParentsToTown_Zone";

        /// <summary>
        /// 当前地域的子级地域集合
        /// </summary>
        public const string ZONE_CHILDREN_ZONE = "Zone_Children_Zone";

        /// <summary>
        /// 当前地域的全部子级地域集合
        /// </summary>
        public const string ZONE_ALLCHILDREN_ZONE = "Zone_AllChildren_Zone";

        /// <summary>
        /// 当前地域之上级地域
        /// </summary>
        public const string ZONE_PARENT_ZONE = "Zone_Parent_Zone";

        /// <summary>
        /// 承包方所在地域
        /// </summary>
        public const string VIRTUALPERSON_ZONE = "VirtualPerson_Zone";

        #endregion
    }
}
