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
    /// 导入土地调查压缩包任务参数
    /// </summary>
    public class ArcLandImporArgument : TaskArgument
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public List<Zone> ZoneList { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 土地类型
        /// </summary>
        public LanderType LandorType { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string OpratorName { get; set; }

     

        /// <summary>
        /// SessionCode
        /// </summary>
        public string SessionCode { get; set; }

        /// <summary>
        /// 登录用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 使用安全验证
        /// </summary>
        public bool UseSafeTrans { get; set; }

     

     
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ArcLandImporArgument()
        {
        }

        #endregion

    }
}
