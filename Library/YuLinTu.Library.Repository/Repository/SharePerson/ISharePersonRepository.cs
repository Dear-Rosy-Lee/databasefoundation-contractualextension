// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 共有人的数据访问接口
    /// </summary>
    public interface ISharePersonRepository :   IRepository<SharePerson>
    {
        #region Methods


        /// <summary>
        /// 根据ID删除共有人对象
        /// </summary>
        /// <param name="guid">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid guid);

        /// <summary>
        /// 更新共有人对象
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(SharePerson entity);

        /// <summary>
        /// 根据ID获取共有人对象
        /// </summary>
        SharePerson Get(Guid guid);

        /// <summary>
        /// 根据ID判断共有人对象是否存在
        /// </summary>
        bool Exists(Guid guid);

        #endregion

        #region ExtendMethod

        /// <summary>
        /// 根据农村土地承包合同ID获取共有人
        /// </summary>
        /// <param name="concordID">农村土地承包合同ID</param>
        /// <returns>共有人对象集合</returns>
        SharePersonCollection GetByConcordID(Guid concordID);

        /// <summary>
        /// 根据农村土地承包合同ID删除共有人对象
        /// </summary>
        /// <param name="concordID">农村土地承包合同ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByConcordID(Guid concordID);

        #endregion
	}
}