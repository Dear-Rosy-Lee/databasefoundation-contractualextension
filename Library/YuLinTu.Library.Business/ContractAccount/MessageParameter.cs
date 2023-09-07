/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 消息传递参数
    /// </summary>
    public class MessageParameter
    {
        #region  Fields


        #endregion

        #region Properties


        /// <summary>
        /// 承包方导出配置
        /// </summary>
        public FamilyOutputDefine FamilyOutputSet = FamilyOutputDefine.GetIntence();


        /// <summary>
        /// 承包方其它配置
        /// </summary>
        public FamilyOtherDefine FamilyOtherSet = FamilyOtherDefine.GetIntence();

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName
        { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public Zone CurrentZone
        { get; set; }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch
        { get; set; }

        #endregion

        #region Ctor

        public MessageParameter()
        {
            
        }

        #endregion

        #region Methods



        #endregion
    }
}
