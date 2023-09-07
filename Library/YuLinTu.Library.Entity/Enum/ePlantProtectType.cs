using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// ��������
    /// </summary>
    public enum ePlantProtectType
    {
        /// <summary>
        /// һ��(��ʳ��������)
        /// </summary>
        [EntityEnumName("key41301", IsLanguageName = true)]
        FirstGrade = 1,

        /// <summary>
        /// ����(��ľ����)
        /// </summary>
        [EntityEnumName("key41302", IsLanguageName = true)]
        SecondGrade = 2,

        /// <summary>
        /// ����
        /// </summary>
        [EntityEnumName("key41303", IsLanguageName = true)]
        UnKnown = 3
    };
}
