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
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 发包方模块接口定义
    /// </summary>
    public interface ISenderWorkStation : IWorkstation<CollectivityTissue>
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
        /// 获取有效编码
        /// </summary>
        /// <param name="codeZone"></param>
        /// <returns></returns>
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
        /// 根据名称得到集体经济组织
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        CollectivityTissue Get(string name);

        /// <summary>
        /// 根据名称及编码获取发包方
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        CollectivityTissue Get(string zoneCode, string name);

        /// <summary>
        /// 根据发包方编码获取发包方
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        CollectivityTissue GetByCode(string tissueCode);

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
        /// <param name="searchType">搜索类型。</param>
        List<CollectivityTissue> SearchByName(string name, eSearchOption searchType);

        /// <summary>
        /// 按法人证明书编号与搜索方式查找。
        /// </summary>
        /// <param name="cartNumber">法人证明书编号。</param>
        /// <param name="searchType">搜索类型。</param>
        List<CollectivityTissue> SearchByLawyerCartNumber(string cartNumber, eSearchOption searchType);

        /// <summary>
        /// 更新集体经济组织对象
        /// </summary>
        int Update(CollectivityTissue tissue);

        /// <summary>
        /// 删除
        /// </summary>
        int Delete(Guid zoneID);

        int Delete(CollectivityTissue tissue);

        /// <summary>
        /// 根据地域删除集体经济组织
        /// </summary>
        int DeleteByZoneCode(string zoneCode, eLevelOption searchOption);

        int Add(List<CollectivityTissue> listTissue, bool overwrite, Action<CollectivityTissue, int, int> action);

        /// <summary>
        /// 更具地域编码获取多个发包方
        /// </summary>
        /// <param name="codeZones"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        List<CollectivityTissue> GetTissues(string[] codeZones, Action<string, int, int> action);

        /// <summary>
        /// 是否是默认发包方
        /// </summary>
        /// <returns></returns>
        bool IsDefaultTissue(CollectivityTissue tissue);

        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        bool Clear();

        /// <summary>
        /// 获取地域
        /// </summary>
        Zone GetZone(string zoneCode);

        /// <summary>
        /// 批量添加发包方数据
        /// </summary>
        /// <param name="listPerson">发包方对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<CollectivityTissue> listTissue);

        /// <summary>
        /// 批量删除发包方数据
        /// </summary>
        /// <param name="listPerson">发包方对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int DelectSenders(List<string> listTissueCode);

        #endregion
    }
}
