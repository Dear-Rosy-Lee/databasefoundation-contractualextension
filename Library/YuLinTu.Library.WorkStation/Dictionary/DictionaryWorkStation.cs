/*
 * (C) 2015 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 业务逻辑层类——用于实现接口中声明的方法
    /// </summary>
    public class DictionaryWorkStation : Workstation<Dictionary>, IDictionaryWorkStation
    {
        #region Properties

        /// <summary>
        /// 重写数据访问层属性
        /// </summary>
        public new IDictionaryRepository DefaultRepository
        {
            get { return base.DefaultRepository as IDictionaryRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 有参构造函数—用于传入数据访问层接口
        /// </summary>
        /// <param name="rep"></param>
        public DictionaryWorkStation(IDictionaryRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion

        #region method 实现

        /// <summary>
        /// 根据分组码获取字典内容
        /// </summary>
        public KeyValueList<string, string> GetCodeNameByGroupCode(string groupCode)
        {
            return DefaultRepository.GetCodeNameByGroupCode(groupCode);
        }

        /// <summary>
        /// 根据分组码获得该组内所有类型信息
        /// </summary>
        /// <param name="groupcode">分组码</param>
        public List<Dictionary> GetByGroupCodeWork(string groupcode)
        {
            return DefaultRepository.GetByGroupCode(groupcode);
        }

        /// <summary>
        /// 根据分组码获取字典List
        /// </summary>
        /// <param name="groupCode">组编码</param>
        /// <param name="isIncludeEmpty">是否包含空项</param>
        /// <returns></returns>
        public List<Dictionary> GetByGroupCode(string groupCode, bool isIncludeEmpty)
        {
            return DefaultRepository.GetByGroupCode(groupCode, isIncludeEmpty);
        }

        /// <summary>
        /// 根据属性类型名称获得该类型信息
        /// </summary>
        /// <param name="name">属性类型名称</param>
        public Dictionary GetByNameWork(string name)
        {
            return DefaultRepository.GetByName(name);
        }

        /// <summary>
        /// 修改字典中指定的实体对象
        /// </summary>
        /// <param name="value">实体对象</param>
        public int ModifyWork(Dictionary value)
        {
            DefaultRepository.Modify(value);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="values">实体对象集合</param>
        public int AddRangeToDictionary(List<Dictionary> values)
        {
            foreach (var item in values)
            {
                DefaultRepository.Add(item);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 逐条添加数据
        /// </summary>
        /// <param name="dict">实体对象</param>
        public int AddWork(Dictionary dict)
        {
            DefaultRepository.Add(dict);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 逐条删除数据
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public int DelWork(Dictionary dict)
        {
            DefaultRepository.Delete(dict);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="values">实体对象集合</param>
        /// <returns></returns>
        public int DeleteRangeFromDict(List<Dictionary> values)
        {
            foreach (var item in values)
            {
                DefaultRepository.Delete(item);
            }
            return TrySaveChanges(DefaultRepository);
        }
        /// <summary>
        /// 获取所有分组名称
        /// </summary>
        /// <returns></returns>
        public List<Dictionary> GetAllFZM()
        {
            return DefaultRepository.GetAllFZM();
        }

        #endregion
    }
}
