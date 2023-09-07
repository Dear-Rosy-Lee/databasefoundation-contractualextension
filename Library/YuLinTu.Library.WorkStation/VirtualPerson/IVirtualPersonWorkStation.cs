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
    /// 承包方模块接口定义
    /// </summary>
    public interface IVirtualPersonWorkStation<T> : IWorkstation<T> where T : VirtualPerson
    {
        #region Methods

        /// <summary>
        /// 根据id获取承包方对象
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>承包方对象</returns>
        VirtualPerson Get(Guid id);

        /// <summary>
        /// 根据“承包方”名称从指定的地域中获取“承包方”的信息。
        /// </summary>
        /// <param name="name">“承包方”名称</param>
        /// <param name="code">地域代码</param>
        /// <returns>“承包方”的信息</returns>
        VirtualPerson Get(string name, string code);

        /// <summary>
        /// 根据“承包方”编号从指定的地域中获取“承包方”的信息
        /// </summary>
        /// <param name="number">“承包方”编号</param>
        /// <param name="code">地域代码</param>
        /// <returns>“承包方”的信息</returns>
        VirtualPerson GetByNumber(string number, string code);

        /// <summary>
        /// 根据“承包方”户号从指定的地域中获取“承包方”的信息
        /// </summary>
        /// <param name="familyNumber">“承包方”户号</param>
        /// <param name="code">地域代码</param>
        /// <returns>“承包方”的信息</returns>
        VirtualPerson GetFamilyNumber(string familyNumber, string code);

        /// <summary>
        /// 根据“承包方”名称、户号从指定的地域中获取“承包方”的信息
        /// </summary>
        /// <param name="name">“承包方”名称</param>
        /// <param name="number">“承包方”户号</param>
        /// <param name="code">地域代码</param>
        /// <returns>“承包方”的信息</returns>
        VirtualPerson Get(string name, string number, string code);

        /// <summary>
        /// 根据“承包方”名称及其类型从指定的地域中获取“承包方”的信息。
        /// </summary>
        /// <param name="name">“承包方”的名称。</param>
        /// <param name="zoneCode">地域编码。</param>
        /// <param name="virtualPersonType">“承包方”的类型。</param>
        /// <returns>返回 <see cref="VirtualPerson"/> 类的实例，如果未找到相应的“承包方”则返回 <c>null</c>。</returns>
        VirtualPerson Get(string name, string zoneCode, eVirtualPersonType virtualPersonType);

        /// <summary>
        /// 根据“承包方”名称从指定的地域中获取“承包方”的信息
        /// </summary>
        /// <param name="name">“承包方”名称</param>
        /// <param name="code">地域编码</param>
        /// <returns>“承包方”的信息</returns>
        int Count(string name, string code);

        /// <summary>
        /// 统计指定的地域中“承包方”的数量
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int CountByZone(string code);

        /// <summary>
        /// 获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <returns>“承包方”的集合</returns>
        List<VirtualPerson> GetByZoneCode(string code);

        /// <summary>
        /// 获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="levelOption">匹配级别</param>
        /// <returns>“承包方”的集合</returns>
        List<VirtualPerson> GetByZoneCode(string code, eLevelOption levelOption);

        /// <summary>
        /// 根据承包方状态获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="status">承包方状态</param>
        /// <param name="levelOption">地域编码的匹配级别</param>
        /// <returns>以名称排序的“承包方”的集合</returns>
        List<VirtualPerson> GetByZoneCode(string code, eVirtualPersonStatus status, eLevelOption levelOption);

        /// <summary>
        /// 根据承包方类型获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方状态</param>
        /// <returns></returns>
        List<VirtualPerson> GetCollection(string code, eVirtualPersonType virtualType);

        /// <summary>
        ///  根据承包方类型获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方类型</param>
        /// <param name="levelOption">地域编码的匹配级别</param>
        /// <returns>“承包方”的集合</returns>
        List<VirtualPerson> GetCollection(string code, eVirtualPersonType virtualType, eLevelOption levelOption);

        /// <summary>
        /// 根据承包方的名称获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="Name">承包方的名称</param>
        /// <returns>“承包方”的集合</returns>
        List<VirtualPerson> GetCollection(string code, string Name);

        /// <summary>
        /// 更新承包方对象
        /// </summary>
        /// <param name="virtualPerson">承包方对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(VirtualPerson virtualPerson);

        /// <summary>
        /// 根据id删除承包方信息
        /// </summary>
        /// <param name="ID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid ID);

        /// <summary>
        /// 根据承包方身份证号删除承包方信息
        /// </summary>
        /// <param name="ID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(string cardId);

        /// <summary>
        /// 删除指定地域下的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string code);

        /// <summary>
        /// 删除指定地域下指定的承包方类型的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string code, eVirtualPersonType virtualType);

        /// <summary>
        /// 删除指定地域下的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string code, eLevelOption levelOption);

        /// <summary>
        /// 删除指定地域下指定的承包方类型的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方类型</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <returns></returns>
        int DeleteByZoneCode(string code, eVirtualPersonType virtualType, eLevelOption levelOption);

        /// <summary>
        /// 删除指定地域下指定的承包方状态的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方状态</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        int DeleteByZoneCode(string code, eVirtualPersonStatus virtualStatus, eLevelOption levelOption);

        /// <summary>
        /// 根据地域代码与承包方名称判断是否存在承包方对象
        /// </summary>
        /// <param name="code">地域代码</param>
        /// <param name="Name">承包方名称</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistsByZoneCodeAndName(string code, string Name);

        /// <summary>
        /// 根据地域代码与承包方名称判断是否存在状态处于锁定的承包方对象
        /// </summary>
        /// <param name="code">地域代码</param>
        /// <param name="Name">承包方名称</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistsLockByZoneCodeAndName(string code, string Name);

        ///// <summary>
        ///// 判断指定地域下的“承包方”是否已经初始化。
        ///// </summary>
        ///// <param name="zoneCode">地域编码。</param>
        ///// <returns>当指定的地域下包含有“承包方”数据时则返回 <c>true</c> 以表示已经初始化，否则返回 <c>false</c>。</returns>
        ///// <history>
        /////     2011年1月17日 16:55:20  Roc 创建
        ///// </history>
        //bool HasInitialized(string zoneCode);

        /// <summary>
        /// 创建承包方编码
        /// </summary>
        /// <param name="vp"></param>
        /// <returns></returns>
        string CreateVirtualPersonNum(string zoneCode, eContractorType contractorType = eContractorType.Farmer);

        /// <summary>
        /// 存在承包方数据的地域集合
        /// </summary>
        /// <param name="zoneList">地域集合</param>
        List<Zone> ExistZones(List<Zone> zoneList);

        /// <summary>
        /// 批量添加承包方数据
        /// </summary>
        /// <param name="listPerson">承包方对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<VirtualPerson> listPerson);

        /// <summary>
        /// 根据地域删除下面承包方所有数据
        /// </summary> 
        void ClearZoneVirtualPersonALLData(string zoneCode);

        /// <summary>
        /// 根据承包方id集合删除承包方关联数据
        /// </summary>
        int DeleteRelationDataByVps(List<Guid> vpIds);

        List<LandVirtualPerson> GetVirtualPersonsByLand(Guid landId);

        int AddBelongRelation(BelongRelation belongRelation);

        int DeleteRelationDataByZone(string zoneCode);

        BelongRelation GetRelationByID(Guid personId, Guid landId);
        /// <summary>
        /// 根据承包方ID获取确股数据
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        List<BelongRelation> GetRelationsByVpID(Guid personID);

        List<BelongRelation> GetRelationByZone(string zoneCode);

        int UpdatePersonList(List<VirtualPerson> persons);

        #endregion
    }
}
