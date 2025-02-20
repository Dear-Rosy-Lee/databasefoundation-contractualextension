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
    /// 数据访问层类——用于实现接口中声明的方法
    /// </summary>
    public class DictionaryRepository : RepositoryDbContext<Dictionary>, IDictionaryRepository
    {
        #region Ctor

        /// <summary>
        /// 构造函数—初始化数据源
        /// </summary>
        /// <param name="ds">数据源</param>
        public DictionaryRepository(IDataSource ds)
            : base(ds)
        {
        }

        #endregion

        #region method 实现

        /// <summary>
        /// 根据分组码获取字典内容
        /// </summary>
        public KeyValueList<string, string> GetCodeNameByGroupCode(string groupCode)
        {
            if (string.IsNullOrEmpty(groupCode))
                return null;
            var entity = (from dict in DataSource.CreateQuery<Dictionary>()
                          where dict.GroupCode == groupCode && dict.Code != ""
                          select new { DictCode = dict.Code, DictName = dict.Name }).ToList();
            KeyValueList<string, string> kvDicts = new KeyValueList<string, string>();
            entity.ForEach(c => kvDicts.Add(new KeyValue<string, string> { Key = c.DictCode, Value = c.DictName }));
            return kvDicts;
        }

        /// <summary>
        /// 根据分组码获得该组内所有类型信息
        /// </summary>
        /// <param name="groupcode">分组码</param>
        public List<Dictionary> GetByGroupCode(string groupcode)
        {
            List<Dictionary> dicts = new List<Dictionary>();
            dicts = base.Get(c => c.GroupCode == groupcode);
            return dicts.ToList();
        }
        /// <summary>
        /// 获取所有分组名称
        /// </summary>
        /// <returns></returns>
        public List<Dictionary> GetAllFZM()
        {
            var dict = DataSource.CreateQuery<Dictionary>();
          
            var dgroup = from i in dict
                      group i by new { i.GroupName,i.GroupCode } into g
                      select new
                      {
                          Key = g.Key.GroupCode,
                          Name=g.Key.GroupName
                      };

            var t = dgroup.ToList().Select(c => new Dictionary
            {
                GroupCode = c.Key,
                GroupName=c.Name,
                ID=new Guid()                
            }).ToList();

            return t;
        }

        /// <summary>
        /// 根据属性类型名称获得该类型信息
        /// </summary>
        /// <param name="name">属性类型名称</param>
        public Dictionary GetByName(string name)
        {
            return base.FirstOrDefault(c => c.Name == name);
        }

        /// <summary>
        /// 修改字典中指定的实体对象
        /// </summary>
        /// <param name="value">实体对象</param>
        public int Modify(Dictionary value)
        {
            return base.Update(value, c => c.ID == value.ID);
        }

        /// <summary>
        /// 向数据字典中添加属性信息
        /// </summary>
        /// <param name="dict">实体对象</param>
        /// <returns></returns>
        //public int Add(Dictionary dict)
        //{
        //    return base.Add(dict);
        //}

        /// <summary>
        /// 删除属性记录
        /// </summary>
        /// <param name="dict">属性信息</param>
        public int Delete(Dictionary dict)
        {
            return base.Delete(c => c.ID == dict.ID);
        }


        public List<Dictionary> GetByGroupCode(string groupCode, bool isIncludeEmpty)
        {
            if (isIncludeEmpty)
            {
                return Get(o => o.GroupCode == groupCode);
            }
            return Get(o => o.GroupCode == groupCode).FindAll(o => !string.IsNullOrEmpty(o.Name));
        }

        #endregion
    }
}
