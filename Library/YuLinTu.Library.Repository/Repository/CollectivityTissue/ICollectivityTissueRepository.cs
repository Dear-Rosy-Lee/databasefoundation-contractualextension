// (C) 2015 鱼鳞图公司版权所有，保留所有权利
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
    /// 集体经济组织的数据访问接口
    /// </summary>
    public interface ICollectivityTissueRepository :   IRepository<CollectivityTissue>
    {
        #region Methods

        /// <summary>
        /// 检测指定的地域是否是集体经济组织。
        /// </summary>
        /// <param name="zoneCode">地域全编码。</param>
        /// <returns>如果指定的地域是集体经济组织则返回 true， 否则返回 false。</returns>
        bool Exists(string zoneCode);

        /// <summary>
        /// 检测指定的地域与集体经济组织名称 查看是否存在。
        /// </summary>
        bool Exists(string zoneCode, string tissueName);

        /// <summary>
        /// 确定指定的注册编码是否已经存在。
        /// </summary>
        /// <param name="code">组织的编码。</param>
        /// <returns>如果存在则返回 true， 否则返回 false。</returns>  
        bool CodeExists(string code);

        /// <summary>
        /// 根据区域代码获得最大的组织编码
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns>最大的组织编码</returns>
        int GetValidCodeByZone(string codeZone);

        /// <summary>
        /// 获取指定地域下的集体经济组织。
        /// </summary>
        /// <param name="zoneCode">地域的全编码。</param>
        List<CollectivityTissue> GetTissues(string zoneCode);

        /// <summary>
        /// 获取集体经济组织对象
        /// </summary>
        CollectivityTissue Get(Guid guid);

        /// <summary>
        /// 获取与目标集体经济组织名称相同的集体经济组织对象
        /// </summary>
        /// <param name="name">集体经济组织名称</param>
        /// <returns>集体经济组织对象</returns>
        CollectivityTissue Get(string name);

        /// <summary>
        /// 获取指定区域下指定集体经济组织名称的集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="name">集体经济组织名称</param>
        /// <returns>集体经济组织对象</returns>
        CollectivityTissue Get(string zoneCode, string name);

        /// <summary>
        /// 判断集体经济组织对象是否存在
        /// </summary>        
        bool Exists(Guid guid);

        /// <summary>
        /// 确定指定的注册编码除指定的组织使用外是否已经存在。
        /// </summary>
        /// <param name="code">组织编码。</param>
        /// <param name="tissueID">集体经济组织的 ID。</param>
        /// <returns>如果存在则返回 true， 否则返回 false。</returns>
        bool CodeExists(string code, Guid tissueId);

        /// <summary>
        /// 确定指定的集体经济组织的名称是否已经存在。
        /// </summary>
        /// <param name="name">集体经济组织的名称。</param>
        /// <returns>如果存在则返回 true， 否则返回 false。</returns> 
        bool NameExists(string name);

        /// <summary>
        /// 确定指定的集体经济组织的名称除现有集体组织使用外是否已经存在。
        /// </summary>
        /// <param name="name">集体经济组织的名称。</param>
        /// <param name="tissueID">集体经济组织的 ID。</param>
        /// <returns>如果存在则返回 true， 否则返回 false。</returns> 
        bool NameExists(string name, Guid tissueId);

        /// <summary>
        /// 统计指定地域下的集体经济组织的数量。
        /// </summary>
        /// <param name="zoneCode">地域的全编码。</param>
        /// <returns>指定地域下的集体经济组织的数量。</returns> 
        int Count(string zoneCode);

        /// <summary>
        /// 统计指定地域下的集体经济组织的数量。
        /// </summary>
        /// <param name="zoneCode">地域的全编码。</param>
        /// <param name="searchOption">指定统计操作应包括所有子地域还是仅包括当前地域。</param>
        /// <returns>指定地域下的集体经济组织的数量。</returns> 
        int Count(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 获取指定地域下的集体经济组织。
        /// </summary>
        /// <param name="zoneCode">地域的全编码。</param>
        /// <param name="searchOption">指定统计操作应包括所有子地域还是仅包括当前地域。</param> 
        List<CollectivityTissue> GetTissues(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按名称与搜索方式查找。
        /// </summary>
        /// <param name="name">集体经济组织名称。</param>
        /// <param name="searchOption">搜索类型。</param> 
        List<CollectivityTissue> SearchByName(string name, eSearchOption searchOption);

        /// <summary>
        /// 按法人证明书编号与搜索方式查找。
        /// </summary>
        /// <param name="cartNumber">法人证明书编号。</param>
        /// <param name="searchOption">搜索类型。</param> 
        List<CollectivityTissue> SearchByLawyerCartNumber(string cartNumber, eSearchOption searchOption);

        /// <summary>
        /// 更新集体经济组织对象
        /// </summary> 
        int Update(CollectivityTissue tissue);

        /// <summary>
        /// 删除
        /// </summary> 
        int Delete(Guid zoneID);

        /// <summary>
        /// 根据集体经济组织对象删除
        /// </summary>
        /// <param name="tissue">集体经济组织</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(CollectivityTissue tissue);

        /// <summary>
        /// 删除所属区域下的所有集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">所属区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 添加集体经济组织对象集合
        /// </summary>
        /// <param name="listTissue">集体经济组织对象集合</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <param name="action"></param>
        /// <returns></returns>
        int Add(List<CollectivityTissue> listTissue, bool overwrite, Action<CollectivityTissue, int, int> action);

        /// <summary>
        /// 根据指定区域数组获得其下的所有集体经济组织对象集合
        /// </summary>
        /// <param name="codeZones">区域代码数组</param>
        /// <param name="action"></param>
        /// <returns>集体经济对象集合</returns>
        List<CollectivityTissue> GetTissues(string[] codeZones, Action<string, int, int> action);

        /// <summary>
        /// 清空发包方
        /// </summary>
        void ClearTissue();
        #endregion
    }
}
