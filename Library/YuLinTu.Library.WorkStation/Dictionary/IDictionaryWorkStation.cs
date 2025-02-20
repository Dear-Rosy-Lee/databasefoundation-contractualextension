/*
 * (C) 2025 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 业务逻辑层接口——根据业务需求声明复合功能
    /// </summary>
    public interface IDictionaryWorkStation : IWorkstation<Dictionary>
    {
        #region method 声明

        /// <summary>
        /// 根据分组码获取字典内容
        /// </summary>
        KeyValueList<string, string> GetCodeNameByGroupCode(string groupCode);

        /// <summary>
        /// 根据分组码获得该组内所有类型信息
        /// </summary>
        /// <param name="groupcode">分组码</param>
        List<Dictionary> GetByGroupCodeWork(string groupcode);

        /// <summary>
        /// 根据属性类型名称获得该类型信息
        /// </summary>
        /// <param name="name">属性类型名称</param>
        Dictionary GetByNameWork(string name);

        /// <summary>
        /// 修改字典中指定的实体对象
        /// </summary>
        /// <param name="value">实体对象</param>
        int ModifyWork(Dictionary value);

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="values">实体对象集合</param>
        int AddRangeToDictionary(List<Dictionary> values);

        /// <summary>
        /// 逐条添加数据
        /// </summary>
        /// <param name="dict">实体对象</param>
        int AddWork(Dictionary dict);

        /// <summary>
        /// 逐条删除数据
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        int DelWork(Dictionary dict);

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="values">实体对象集合</param>
        int DeleteRangeFromDict(List<Dictionary> values);

        /// <summary>
        /// 根据分组码获取字典List
        /// </summary>
        /// <param name="groupCode">组编码</param>
        /// <param name="isIncludeEmpty">是否包含空项</param>
        /// <returns></returns>
        List<Dictionary> GetByGroupCode(string groupCode, bool isIncludeEmpty);
        /// <summary>
        /// 获取所有分组名称
        /// </summary>
        /// <returns></returns>
        List<Dictionary> GetAllFZM();

        #endregion
    }
}
