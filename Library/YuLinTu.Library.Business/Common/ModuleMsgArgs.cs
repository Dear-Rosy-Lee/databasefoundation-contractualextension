/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 自定义消息类型
    /// </summary>
    public class ModuleMsgArgs : MsgEventArgs
    {
        /// <summary>
        /// 数据
        /// </summary>
        public IDbContext Datasource { get; set; }

        /// <summary>
        /// 地域编码
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 字典分组码
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public Dictionary DictionaryEntity { get; set; }

        /// <summary>
        /// 属性数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 设置参数方法
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="para">消息参数</param>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="db">数据库</param>
        /// <param name="tag">Tag可用于存储参数</param>
        /// <param name="id">int类型消息ID</param>
        public ModuleMsgArgs Set(string name, object para, string zoneCode = "", IDbContext db = null, object tag = null, int id = 0)
        {
            this.Name = name;
            this.ID = id;
            this.Parameter = para;
            this.Datasource = db;
            this.ZoneCode = zoneCode;
            this.Tag = tag;
            return this;
        }
    }
}
