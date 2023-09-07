// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 集体建设用地第字号表的数据访问接口
    /// </summary>
    public interface IBuildLandWarrantRepository :   IRepository<BuildLandWarrant>
    {
        #region Methods

        /// <summary>
        /// 根据权证编号判断其是否存在，这里的权证编号是13位的
        /// </summary>
        /// <param name="number">权证编号</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        bool ExistByWarrantNumber(string number);

        /// <summary>
        /// 根据权证编号统计集体建设用地第字号对象的数量？
        /// </summary>
        /// <param name="number">权证编号</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        bool ExistAllNumber(string number);

        /// <summary>
        ///根据标识码获取集体建设用地第字号对象
        /// </summary>
        /// <param name="id">识码</param>
        /// <returns>集体建设用地第字号对象集合</returns>
        BuildLandWarrant Get(Guid id);

        /// <summary>
        /// 获取与目标权证编号右相似的集体建设用地第字号对象
        /// </summary>
        /// <param name="warrant">目标权证编号</param>
        /// <returns>集体建设用地第字号对象集合</returns>
        BuildLandWarrantCollection GetByWarrantNumbe_Right(string warrant);

        /// <summary>
        /// 获取与目标权证编号相同的集体建设用地第字号对象
        /// </summary>
        /// <param name="warrant">目标权证编号</param>
        /// <returns>集体建设用地第字号对象集合</returns>
        BuildLandWarrantCollection GetByWarrantNumbe_All(string warrant);

        /// <summary>
        /// 根据地标识码删除集体建设用地第字号表
        /// </summary>
        /// <param name="buildLandWarrantID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid buildLandWarrantID);

        /// <summary>
        /// 根据地域编号删除集体建设用地第字号表
        /// </summary>
        /// <param name="zoneCode">地域编号</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode);

        /// <summary>
        /// 更新集体建设用地第字号对象
        /// </summary>
        /// <param name="buildLandWarrant">集体建设用地第字号对象</param>
        /// <returns>-1（错误）/0（失败）/1（成功）</returns>
        int Update(BuildLandWarrant buildLandWarrant);

        /// <summary>
        /// 根据标识码将集体建设用地第字号对象的打印次数加一
        /// </summary>
        /// <param name="landID">标识码</param>
        void RecordPrintCount(Guid landID);
		
		#endregion
	}
}