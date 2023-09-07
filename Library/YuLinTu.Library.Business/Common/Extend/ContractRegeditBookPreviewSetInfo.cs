/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 预览权证共有人和地块数目设置
    /// </summary>
    public class ContractRegeditBookPreviewSetInfo
    {

        #region Properties

        /// <summary>
        ///证书共有人数
        /// </summary>
        public int CRBSharePersonCount { get; set; }

        /// <summary>
        /// 证书地块数
        /// </summary>
        public int CRBLandCount { get; set; }
        /// <summary>
        /// 证书编号样式
        /// </summary>
        public string CRBBookNumerSetting { get; set; }

        #endregion

    }
}
