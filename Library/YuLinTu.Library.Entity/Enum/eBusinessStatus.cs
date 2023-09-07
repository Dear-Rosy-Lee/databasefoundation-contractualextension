using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 业务流程状态
    /// </summary>
    public enum eBusinessStatus
    {
        /// <summary>
        /// 初始登记
        /// </summary>
        [EntityEnumName("初始登记", IsLanguageName = false)]
        InitialRegistration = 1,

        /// <summary>
        /// 变更登记
        /// </summary>
        [EntityEnumName("变更登记", IsLanguageName = false)]
        ExchangeRegistration = 20,

        /// <summary>
        /// 分户登记
        /// </summary>
        [EntityEnumName("分户登记", IsLanguageName = false)]
        SplitFamilyRegister = 22,

        /// <summary>
        /// 合户登记
        /// </summary>
        [EntityEnumName("合户登记", IsLanguageName = false)]
        MergeFamiliesRegister = 24,

        /// <summary>
        /// 转让登记
        /// </summary>
        [EntityEnumName("转让登记", IsLanguageName = false)]
        TransferRegister = 26,

        /// <summary>
        /// 互换登记
        /// </summary>
        [EntityEnumName("互换登记", IsLanguageName = false)]
        ExhangeRegister = 28,

        /// <summary>
        /// 抵押登记
        /// </summary>
        [EntityEnumName("抵押登记", IsLanguageName = false)]
        MortgagedRegistration = 50,

        /// <summary>
        /// 注销登记
        /// </summary>
        [EntityEnumName("注销登记", IsLanguageName = false)]
        CancellationRegistration = 80,

        /// <summary>
        /// 变更待定
        /// </summary>
        [EntityEnumName("变更待定", IsLanguageName = false)]
        ExchangePending = 85,

        /// <summary>
        /// 注销待定
        /// </summary>
        [EntityEnumName("注销待定", IsLanguageName = false)]
        CancelPending = 88,

        /// <summary>
        /// 销户待定
        /// </summary>
        [EntityEnumName("销户待定", IsLanguageName = false)]
        CancelFamilyPending = 89,

        /// <summary>
        /// 结束
        /// </summary>
        [EntityEnumName("结束", IsLanguageName = false)]
        End = 90,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("未知", IsLanguageName = false)]
        UnKnown = 100
    }
}
