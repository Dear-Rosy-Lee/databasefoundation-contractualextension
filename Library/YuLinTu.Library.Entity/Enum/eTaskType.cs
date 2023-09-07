/*
 * (C) 2012-2016 鱼鳞图公司版权所有，保留所有权利
*/
using System;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 枚举事务类型。
    /// </summary>
    [Serializable]
    public enum eTaskType
    {
        /// <summary>
        /// 未知。
        /// </summary>
        [EntityEnumName("未知", IsLanguageName = false)]
        Unknown = 0,

        /// <summary>
        /// 初始登记。
        /// </summary>
        [EntityEnumName("初始登记", IsLanguageName = false)]
        InitializationRegister = 1,

        /// <summary>
        /// 一般变更登记。
        /// </summary>
        [EntityEnumName("变更登记", IsLanguageName = false)]
        ChangeRegister = 2,

        /// <summary>
        /// 注销登记。
        /// </summary>
        [EntityEnumName("注销登记", IsLanguageName = false)]
        CancellationRegister = 3,

        /// <summary>
        /// 权证打印。
        /// </summary>
        [EntityEnumName("权证打印", IsLanguageName = false)]
        Print = 4,

        /// <summary>
        /// 分户登记。
        /// </summary>
        [EntityEnumName("分户登记", IsLanguageName = false)]
        SplitFamilyRegister = 5,

        /// <summary>
        /// 合户登记。
        /// </summary>
        [EntityEnumName("合户登记", IsLanguageName = false)]
        MergeFamiliesRegister = 6,

        /// <summary>
        /// 互换登记。
        /// </summary>
        [EntityEnumName("互换登记", IsLanguageName = false)]
        ExhangeRegister = 7,

        /// <summary>
        /// 转让登记。
        /// </summary>
        [EntityEnumName("转让登记", IsLanguageName = false)]
        TransferRegister = 8,

        /// <summary>
        /// 备案登记。
        /// </summary>
        [EntityEnumName("备案登记", IsLanguageName = false)]
        RecordRegister = 9,

        /// <summary>
        /// 纠纷仲裁。
        /// </summary>
        [EntityEnumName("纠纷仲裁", IsLanguageName = false)]
        Arbitrate = 12
    }
}