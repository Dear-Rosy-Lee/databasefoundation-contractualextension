/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 消息创建
    /// </summary>
    public class MessageExtend
    {
        /// <summary>
        /// 地域消息参数创建
        /// </summary>
        public static ModuleMsgArgs ZoneMsg(IDbContext db, string name, object parameter)
        {
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = name;
            args.Datasource = db == null ? DataBaseSource.GetDataBaseSource() : db;
            args.Parameter = parameter;
            return args;
        }

        /// <summary>
        /// 发包方消息参数创建
        /// </summary>
        public static ModuleMsgArgs SenderMsg(IDbContext db, string name, object parameter, string zoneCode = "", object tag = null)
        {
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = name;
            args.ZoneCode = zoneCode;
            args.Tag = tag;
            args.Datasource = db == null ? DataBaseSource.GetDataBaseSource() : db;
            args.Parameter = parameter;
            return args;
        }

        /// <summary>
        /// 承包方消息参数创建
        /// </summary>
        public static ModuleMsgArgs VirtualPersonMsg(IDbContext db, string name, object parameter, string zoneCode = "", object tag = null)
        {
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = name;
            args.ZoneCode = zoneCode;
            args.Tag = tag;
            args.Datasource = db == null ? DataBaseSource.GetDataBaseSource() : db;
            args.Parameter = parameter;
            return args;
        }

        /// <summary>
        /// 二轮台账消息参数创建
        /// </summary>
        public static ModuleMsgArgs SecondTableMsg(IDbContext db, string name, object parameter, string groupCode, object tag = null)
        {
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = name;
            args.GroupCode = groupCode;
            args.Tag = tag;
            args.Datasource = db == null ? DataBaseSource.GetDataBaseSource() : db;
            args.Parameter = parameter;
            return args;
        }

        /// <summary>
        /// 承包台账消息参数创建
        /// </summary>
        public static ModuleMsgArgs ContractAccountMsg(IDbContext db, string name, object parameter, string zoneCode = "", object tag = null)
        {
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = name;
            args.ZoneCode = zoneCode;
            args.Tag = tag;
            args.Datasource = db == null ? DataBaseSource.GetDataBaseSource() : db;
            args.Parameter = parameter;    
            return args;
        }

        /// <summary>
        /// 承包合同消息参数创建
        /// </summary>
        public static ModuleMsgArgs ContractConcordMsg(IDbContext db, string name, object parameter, string zoneCode = "", object tag = null)
        {
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = name;
            args.ZoneCode = zoneCode;
            args.Tag = tag;
            args.Datasource = db == null ? DataBaseSource.GetDataBaseSource() : db;
            args.Parameter = parameter;
            return args;
        }

        /// <summary>
        /// 承包权证消息参数创建
        /// </summary>
        public static ModuleMsgArgs ContractWarrantMsg(IDbContext db, string name, object parameter, string zoneCode = "", object tag = null)
        {
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = name;
            args.ZoneCode = zoneCode;
            args.Tag = tag;
            args.Datasource = db == null ? DataBaseSource.GetDataBaseSource() : db;
            args.Parameter = parameter;
            return args;
        }


        /// <summary>
        /// 消息参数创建
        /// </summary>
        public static ModuleMsgArgs CreateMsg(IDbContext db, string name, object parameter, string zoneCode = "", string groupCode = "", object tag = null)
        {
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = name;
            args.GroupCode = groupCode;
            args.ZoneCode = zoneCode;
            args.Tag = tag;
            args.Datasource = db == null ? DataBaseSource.GetDataBaseSource() : db;
            args.Parameter = parameter;
            return args;
        }
    }
}
