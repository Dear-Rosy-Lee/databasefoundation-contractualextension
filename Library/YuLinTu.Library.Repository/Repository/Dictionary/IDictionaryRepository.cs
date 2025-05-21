/*
 * (C) 2025 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 数据访问层接口——声明一些对数据库的增、该、查等基本操作
    /// </summary>
    public interface IDictionaryRepository : IRepository<Dictionary>
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
        List<Dictionary> GetByGroupCode(string groupcode);

        /// <summary>
        /// 根据属性类型名称获得该类型信息
        /// </summary>
        /// <param name="name">属性类型名称</param>
        Dictionary GetByName(string name);

        /// <summary>
        /// 修改字典中指定的实体对象
        /// </summary>
        /// <param name="dict">实体对象</param>
        int Modify(Dictionary dict);

        /// <summary>
        /// 向字典中添加属性记录
        /// </summary>
        /// <param name="dict">实体对象</param>
        /// <returns></returns>
        //int Add(Dictionary dict);

        /// <summary>
        /// 删除属性记录
        /// </summary>
        /// <param name="dict">属性信息</param>
        int Delete(Dictionary dict);

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
