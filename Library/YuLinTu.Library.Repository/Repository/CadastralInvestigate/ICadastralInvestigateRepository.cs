// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 地籍调查表的数据访问接口
    /// </summary>
    public interface ICadastralInvestigateRepository :   IRepository<CadastralInvestigate>
    {
        #region Methods

        /// <summary>
        /// 获得与目标标识码相同的籍调查表对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns></returns>
        CadastralInvestigate Get(Guid id);

        /// <summary>
        /// 获得与目标标识码相同的籍调查表对象数量
        /// </summary>
        /// <param name="id">标识码</param>
        bool Exist(Guid id);

        /// <summary>
        /// 根据集体建设用地ID获得地籍调查信息
        /// </summary>
        /// <param name="houseHolderId">集体建设用地ID</param>
        /// <returns>地籍调查表对象</returns>
        CadastralInvestigate GetByLandID(Guid landID);

        /// <summary>
        /// 更新地籍调查信息
        /// </summary>
        /// <param name="investigate">地籍调查信息</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(CadastralInvestigate investigate);

        /// <summary>
        /// 根据标识码删除地籍调查信息
        /// </summary>
        /// <param name="investigate">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid investigateID);

        /// <summary>
        /// 根据集体建设用地ID删除地籍调查信息
        /// </summary>
        /// <param name="landID">集体建设用地ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByLandID(Guid LandID);

        /// <summary>
        /// 根据所在地域删除地籍调查信息
        /// </summary>
        /// <param name="zoneCode">所在地域</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption);
        
		#endregion
	}
}