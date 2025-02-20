// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public interface IAgricultureAffairRepository : IRepository<AgricultureAffair>
    {
        #region Methods - Obsolete

        #region Methods - Read

        /// <summary>
        /// 统计数据
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>对象数量</returns>
        int Count(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="idZone">id</param>
        /// <returns>对象</returns>
        AgricultureAffair Get(Guid id);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        bool Exists(Guid id);

        /// <summary>
        /// 根据名称搜索
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>对象集合</returns>
        AgricultureAffairCollection SearchByName(string zoneName, eSearchOption searchOption);

        /// <summary>
        /// 根据名称与地域获取对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>对象集合</returns>
        AgricultureAffairCollection SearchByName(string zoneName, string zoneCode, eSearchOption searchOption);

        /// <summary>
        /// 根据地域编码获取对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>对象集合</returns>
        AgricultureAffairCollection SearchByZoneCode(string zoneCode, eSearchOption searchOption);

        #endregion

        #region Methods - Modify

        int Delete(Guid id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="zoneID">区域id</param>
        /// <returns>-1(参数错误)/0（失败）/1(成功)</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="affair">用来更新的对象</param>
        /// <returns>0（失败）/1(成功)</returns>
        int Update(AgricultureAffair affair);

        #endregion

        #endregion
    }
}
