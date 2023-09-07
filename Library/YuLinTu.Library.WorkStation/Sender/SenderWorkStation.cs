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
using YuLinTu.Library.Repository;
using YuLinTu.Windows;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 发包方模块接口定义实现
    /// </summary>
    public sealed class SenderWorkStation : Workstation<CollectivityTissue>, ISenderWorkStation
    {
        #region Properties

        public new ICollectivityTissueRepository DefaultRepository
        {
            get { return base.DefaultRepository as ICollectivityTissueRepository; }
            set { base.DefaultRepository = value; }
        }

        public IZoneRepository ZoneRepository { get; set; }

        #endregion

        #region Ctor

        public SenderWorkStation(ICollectivityTissueRepository rep)
        {
            DefaultRepository = rep;
        }

        public SenderWorkStation(ICollectivityTissueRepository rep, IZoneRepository zonerep)
        {
            DefaultRepository = rep;
            ZoneRepository = zonerep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 根据地域编码判断集体经济组织对象是否存在
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns></returns>
        public bool Exists(string zoneCode)
        {
            return DefaultRepository.Exists(zoneCode);
        }

        /// <summary>
        /// 根据目标集体经济组织编码判断集体经济组织对象是否存在
        /// </summary>
        /// <param name="guid">目标集体经济组织编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid id)
        {
            return DefaultRepository.Exists(id);
        }

        /// <summary>
        /// 判断地域编码下是否存在该名称的发包方
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="tissueName"></param>
        /// <returns></returns>
        public bool Exists(string zoneCode, string tissueName)
        {
            return DefaultRepository.Exists(zoneCode, tissueName);
        }

        /// <summary>
        /// 确定指定的注册编码是否已经存在。
        /// </summary>
        /// <param name="code">组织的编码。</param>
        /// <returns>如果存在则返回 true， 否则返回 false。</returns>  
        public bool CodeExists(string code)
        {
            return DefaultRepository.CodeExists(code);
        }

        /// <summary>
        /// 根据区域代码获得最大的组织编码
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns>最大的组织编码</returns>
        public int GetValidCodeByZone(string codeZone)
        {
            return DefaultRepository.GetValidCodeByZone(codeZone);
        }

        /// <summary>
        /// 获取指定地域下的集体经济组织。
        /// </summary>
        /// <param name="zoneCode">地域的全编码。</param>
        public List<CollectivityTissue> GetTissues(string zoneCode)
        {
            return DefaultRepository.GetTissues(zoneCode);
        }

        /// <summary>
        /// 获取集体经济组织对象
        /// </summary>
        public CollectivityTissue Get(Guid guid)
        {
            return DefaultRepository.Get(guid);
        }

        /// <summary>
        /// 获取与目标集体经济组织名称相同的集体经济组织对象
        /// </summary>
        /// <param name="name">集体经济组织名称</param>
        /// <returns>集体经济组织对象</returns>
        public CollectivityTissue Get(string name)
        {
            return DefaultRepository.Get(name);
        }

        /// <summary>
        /// 获取指定区域下指定集体经济组织名称的集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="name">集体经济组织名称</param>
        /// <returns>集体经济组织对象</returns>
        public CollectivityTissue Get(string zoneCode, string name)
        {
            return DefaultRepository.Get(zoneCode, name);
        }

        /// <summary>
        /// 确定指定的注册编码除指定的组织使用外是否已经存在。
        /// </summary>
        /// <param name="code">组织编码。</param>
        /// <param name="tissueID">集体经济组织的 ID。</param>
        /// <returns>如果存在则返回 true， 否则返回 false。</returns>
        public bool CodeExists(string code, Guid tissueId)
        {
            return DefaultRepository.CodeExists(code, tissueId);
        }

        /// <summary>
        /// 确定指定的集体经济组织的名称是否已经存在。
        /// </summary>
        /// <param name="name">集体经济组织的名称。</param>
        /// <returns>如果存在则返回 true， 否则返回 false。</returns> 
        public bool NameExists(string name)
        {
            return DefaultRepository.NameExists(name);
        }

        /// <summary>
        /// 确定指定的集体经济组织的名称除现有集体组织使用外是否已经存在。
        /// </summary>
        /// <param name="name">集体经济组织的名称。</param>
        /// <param name="tissueID">集体经济组织的 ID。</param>
        /// <returns>如果存在则返回 true， 否则返回 false。</returns> 
        public bool NameExists(string name, Guid tissueId)
        {
            return DefaultRepository.NameExists(name, tissueId);
        }

        /// <summary>
        /// 统计指定地域下的集体经济组织的数量。
        /// </summary>
        /// <param name="zoneCode">地域的全编码。</param>
        /// <returns>指定地域下的集体经济组织的数量。</returns> 
        public int Count(string zoneCode)
        {
            return DefaultRepository.Count(zoneCode);
        }

        /// <summary>
        /// 统计指定地域下的集体经济组织的数量。
        /// </summary>
        /// <param name="zoneCode">地域的全编码。</param>
        /// <param name="searchOption">指定统计操作应包括所有子地域还是仅包括当前地域。</param>
        /// <returns>指定地域下的集体经济组织的数量。</returns> 
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.Count(zoneCode, searchOption);
        }

        /// <summary>
        /// 获取指定地域下的集体经济组织。
        /// </summary>
        /// <param name="zoneCode">地域的全编码。</param>
        /// <param name="searchOption">指定统计操作应包括所有子地域还是仅包括当前地域。</param> 
        public List<CollectivityTissue> GetTissues(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.GetTissues(zoneCode, searchOption);
        }

        /// <summary>
        /// 按名称与搜索方式查找。
        /// </summary>
        /// <param name="name">集体经济组织名称。</param>
        /// <param name="searchOption">搜索类型。</param> 
        public List<CollectivityTissue> SearchByName(string name, eSearchOption searchType)
        {
            return DefaultRepository.SearchByName(name, searchType);
        }

        /// <summary>
        /// 按法人证明书编号与搜索方式查找。
        /// </summary>
        /// <param name="cartNumber">法人证明书编号。</param>
        /// <param name="searchOption">搜索类型。</param> 
        public List<CollectivityTissue> SearchByLawyerCartNumber(string cartNumber, eSearchOption searchType)
        {
            return DefaultRepository.SearchByLawyerCartNumber(cartNumber, searchType);
        }

        /// <summary>
        /// 更新集体经济组织对象
        /// </summary> 
        public int Update(CollectivityTissue tissue)
        {
            DefaultRepository.Update(tissue);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除
        /// </summary> 
        public int Delete(Guid zoneID)
        {
            DefaultRepository.Delete(zoneID);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除
        /// </summary> 
        public int Delete(CollectivityTissue tissue)
        {
            DefaultRepository.Delete(tissue);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除所属区域下的所有集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">所属区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, searchOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 添加集体经济组织对象集合
        /// </summary>
        /// <param name="listTissue">集体经济组织对象集合</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int Add(List<CollectivityTissue> listTissue, bool overwrite, Action<CollectivityTissue, int, int> action)
        {
            DefaultRepository.Add(listTissue, overwrite, action);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据指定区域数组获得其下的所有集体经济组织对象集合
        /// </summary>
        /// <param name="codeZones">区域代码数组</param>
        /// <param name="action"></param>
        /// <returns>集体经济对象集合</returns>
        public List<CollectivityTissue> GetTissues(string[] codeZones, Action<string, int, int> action)
        {
            return DefaultRepository.GetTissues(codeZones, action);
        }

        /// <summary>
        /// 根据集体经济组织编码获取
        /// </summary>
        /// <returns></returns>
        public CollectivityTissue GetByCode(string tissueCode)
        {
            return DefaultRepository.Get(t => t.Code == tissueCode).FirstOrDefault();
        }

        /// <summary>
        /// 是否是默认发包方
        /// </summary>
        /// <param name="tissue"></param>
        /// <returns></returns>
        public bool IsDefaultTissue(CollectivityTissue tissue)
        {
            bool result = true;
            string zoneCode = tissue.ZoneCode;
            Zone zone = ZoneRepository.Get(zoneCode);
            if (zone == null)
                result = false;
            else if (zone.ID != tissue.ID)
                result = false;
            return result;
        }

        /// <summary>
        /// 清空发包方
        /// </summary>
        /// <param name="tissue">发包方</param>
        /// <returns></returns>
        public bool Clear()
        {
            DefaultRepository.Delete();
            TrySaveChanges(DefaultRepository);
            return true;
        }

        /// <summary>
        /// 获取地域
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns></returns>
        public Zone GetZone(string zoneCode)
        {
            return ZoneRepository.Get(zoneCode);
        }

        /// <summary>
        /// 批量添加发包方数据
        /// </summary>
        /// <param name="listPerson">发包方对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<CollectivityTissue> listTissue)
        {
            int addCount = 0;
            if (listTissue == null || listTissue.Count == 0)
            {
                return addCount;
            }
            foreach (var tissue in listTissue)
            {
                DefaultRepository.Add(tissue);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        #endregion
    }
}
