using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// ��ֲ����
    /// </summary>
    public enum ePlantingType
    {
        /// <summary>
        /// ����
        /// </summary>
        [EntityEnumName("����", IsLanguageName = false)]
        GrainAndOil = 1,

        /// <summary>
        /// �߲�
        /// </summary>
        [EntityEnumName("�߲�", IsLanguageName = false)]
        Vegetables = 2,

        /// <summary>
        /// ��Ҷ
        /// </summary>
        [EntityEnumName("��Ҷ", IsLanguageName = false)]
        Tea = 3,

        /// <summary>
        /// ����
        /// </summary>
        [EntityEnumName("����", IsLanguageName = false)]
        FruitTree = 4,

        /// <summary>
        /// �в�ҩ
        /// </summary>
        [EntityEnumName("�в�ҩ", IsLanguageName = false)]
        HerbalMedicine = 5,
        
        /// <summary>
        /// ������ľ
        /// </summary>
        [EntityEnumName("������ľ", IsLanguageName = false)]
        FlowerAndWood = 6,

        /// <summary>
        /// ����
        /// </summary>
        [EntityEnumName("����", IsLanguageName = false)]
        Other = 0
    };
}
