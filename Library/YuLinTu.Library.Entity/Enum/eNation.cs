using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 指定中国的民族。
    /// </summary>
    public enum eNation
    {
        /// <summary>
        /// 汉族。
        /// </summary>
        [EntityEnumName("key41351", IsLanguageName = true)]
        Han = 1,

        /// <summary>
        /// 藏族。
        /// </summary>
        [EntityEnumName("key41352", IsLanguageName = true)]
        Tibetan,

        /// <summary>
        /// 东乡族。
        /// </summary>
        [EntityEnumName("key41353", IsLanguageName = true)]
        Dongxiang,

        /// <summary>
        /// 布郞族。
        /// </summary>
        [EntityEnumName("key41354", IsLanguageName = true)]
        Blang,

        /// <summary>
        /// 瑶族。
        /// </summary>
        [EntityEnumName("key41355", IsLanguageName = true)]
        Yao,

        /// <summary>
        /// 鄂温克族。
        /// </summary>
        [EntityEnumName("key41356", IsLanguageName = true)]
        Ewenki,

        /// <summary>
        /// 高山族。
        /// </summary>
        [EntityEnumName("key41357", IsLanguageName = true)]
        Gaoshan,

        /// <summary>
        /// 达斡尔族。
        /// </summary>
        [EntityEnumName("key41358", IsLanguageName = true)]
        Daur,

        /// <summary>
        /// 景颇族。
        /// </summary>
        [EntityEnumName("key41359", IsLanguageName = true)]
        Jingpo,

        /// <summary>
        /// 俄罗斯族。
        /// </summary>
        [EntityEnumName("key41360", IsLanguageName = true)]
        Russians,

        /// <summary>
        /// 保安族。
        /// </summary>
        [EntityEnumName("key41361", IsLanguageName = true)]
        Bonan,

        /// <summary>
        /// 仡佬族。
        /// </summary>
        [EntityEnumName("key41362", IsLanguageName = true)]
        Gelao,

        /// <summary>
        /// 撒拉族。
        /// </summary>
        [EntityEnumName("key41363", IsLanguageName = true)]
        Salar,

        /// <summary>
        /// 裕固族。
        /// </summary>
        [EntityEnumName("key41364", IsLanguageName = true)]
        Yugur,

        /// <summary>
        /// 基诺族。
        /// </summary>
        [EntityEnumName("key41365", IsLanguageName = true)]
        Jino,

        /// <summary>
        /// 羌族。
        /// </summary>
        [EntityEnumName("key41366", IsLanguageName = true)]
        Qiang,

        /// <summary>
        /// 佤族。
        /// </summary>
        [EntityEnumName("key41367", IsLanguageName = true)]
        Va,

        /// <summary>
        /// 苗族。
        /// </summary>
        [EntityEnumName("key41368", IsLanguageName = true)]
        Miao,

        /// <summary>
        /// 畲族。
        /// </summary>
        [EntityEnumName("key41369", IsLanguageName = true)]
        She,

        /// <summary>
        /// 土族。
        /// </summary>
        [EntityEnumName("key41370", IsLanguageName = true)]
        Tu,

        /// <summary>
        /// 布依族。
        /// </summary>
        [EntityEnumName("key41371", IsLanguageName = true)]
        Buyei,

        /// <summary>
        /// 满族。
        /// </summary>
        [EntityEnumName("key41372", IsLanguageName = true)]
        Manchu,

        /// <summary>
        /// 德昂族。De'ang (德昂族 : Déáng Zú) 
        /// </summary>
        [EntityEnumName("key41373", IsLanguageName = true)]
        Deang,

        /// <summary>
        /// 赫哲族。
        /// </summary>
        [EntityEnumName("key41374", IsLanguageName = true)]
        Hezhen,

        /// <summary>
        /// 彝族。
        /// </summary>
        [EntityEnumName("key41375", IsLanguageName = true)]
        Yi,

        /// <summary>
        /// 毛南族。
        /// </summary>
        [EntityEnumName("key41376", IsLanguageName = true)]
        Maonan,

        /// <summary>
        /// 维吾尔族。
        /// </summary>
        [EntityEnumName("key41377", IsLanguageName = true)]
        Uyghur,

        /// <summary>
        /// 珞巴族。
        /// </summary>
        [EntityEnumName("key41378", IsLanguageName = true)]
        Lhoba,

        /// <summary>
        /// 鄂伦春族。
        /// </summary>
        [EntityEnumName("key41379", IsLanguageName = true)]
        Oroqen,

        /// <summary>
        /// 乌孜别克族。
        /// </summary>
        [EntityEnumName("key41380", IsLanguageName = true)]
        Uzbek,

        /// <summary>
        /// 傣族。
        /// </summary>
        [EntityEnumName("key41381", IsLanguageName = true)]
        Dai,

        /// <summary>
        /// 京族。
        /// </summary>
        [EntityEnumName("key41382", IsLanguageName = true)]
        Gin,

        /// <summary>
        /// 朝鲜族。
        /// </summary>
        [EntityEnumName("key41383", IsLanguageName = true)]
        Korean,

        /// <summary>
        /// 锡伯族。
        /// </summary>
        [EntityEnumName("key41384", IsLanguageName = true)]
        Xibe,

        /// <summary>
        /// 独龙族。
        /// </summary>
        [EntityEnumName("key41385", IsLanguageName = true)]
        Derung,

        /// <summary>
        /// 哈尼族。
        /// </summary>
        [EntityEnumName("key41386", IsLanguageName = true)]
        Hani,

        /// <summary>
        /// 阿昌族。
        /// </summary>
        [EntityEnumName("key41387", IsLanguageName = true)]
        Achang,

        /// <summary>
        /// 傈僳族。
        /// </summary>
        [EntityEnumName("key41388", IsLanguageName = true)]
        Lisu,

        /// <summary>
        /// 侗族。
        /// </summary>
        [EntityEnumName("key41389", IsLanguageName = true)]
        Dong,

        /// <summary>
        /// 哈萨克族。
        /// </summary>
        [EntityEnumName("key41390", IsLanguageName = true)]
        Kazak,

        /// <summary>
        /// 白族。
        /// </summary>
        [EntityEnumName("key41391", IsLanguageName = true)]
        Bai,

        /// <summary>
        /// 柯尔克孜族。
        /// </summary>
        [EntityEnumName("key41392", IsLanguageName = true)]
        Kirgiz,

        /// <summary>
        /// 土家族。
        /// </summary>
        [EntityEnumName("key41393", IsLanguageName = true)]
        Tujia,

        /// <summary>
        /// 塔塔尔族。
        /// </summary>
        [EntityEnumName("key41394", IsLanguageName = true)]
        Tatar,

        /// <summary>
        /// 回族。
        /// </summary>
        [EntityEnumName("key41395", IsLanguageName = true)]
        Hui,

        /// <summary>
        /// 门巴族。
        /// </summary>
        [EntityEnumName("key41396", IsLanguageName = true)]
        Monba,

        /// <summary>
        /// (纳西族 : Nàxī Zú) (includes the Mosuo (摩梭 : Mósuō)) 
        /// </summary>
        [EntityEnumName("key41397", IsLanguageName = true)]
        Naxi,

        /// <summary>
        /// 怒族。
        /// </summary>
        [EntityEnumName("key41398", IsLanguageName = true)]
        Nu,

        /// <summary>
        /// 壮族。
        /// </summary>
        [EntityEnumName("key41399", IsLanguageName = true)]
        Zhuang,

        /// <summary>
        /// 仫佬族。
        /// </summary>
        [EntityEnumName("key41400", IsLanguageName = true)]
        Mulao,

        /// <summary>
        /// 普米族。
        /// </summary>
        [EntityEnumName("key41401", IsLanguageName = true)]
        Pumi,

        /// <summary>
        /// 水族。
        /// </summary>
        [EntityEnumName("key41402", IsLanguageName = true)]
        Sui,

        /// <summary>
        /// 拉祜族。
        /// </summary>
        [EntityEnumName("key41403", IsLanguageName = true)]
        Lahu,

        /// <summary>
        /// 黎族。
        /// </summary>
        [EntityEnumName("key41404", IsLanguageName = true)]
        Li,

        /// <summary>
        /// 塔吉克族。
        /// </summary>
        [EntityEnumName("key41405", IsLanguageName = true)]
        Tajik,

        /// <summary>
        /// 蒙古族。
        /// </summary>
        [EntityEnumName("key41406", IsLanguageName = true)]
        Mongol,

        /// <summary>
        /// 未知。
        /// </summary>
        [EntityEnumName("key41407", IsLanguageName = true)]
        UnKnown
    }
}
