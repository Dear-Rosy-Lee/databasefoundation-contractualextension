/*
 * (C) 2015 鱼鳞图公司版权所有，保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 数据字典分组类型信息类
    /// </summary>
    public class DictionaryTypeInfo
    {
        #region Field-Constc

        /// <summary>
        /// 控制点类型及等级
        /// </summary>
        public const string KZDLX = "C1";

        /// <summary>
        /// 标石类型
        /// </summary>
        public const string BSLX = "C2";

        /// <summary>
        /// 标志类型
        /// </summary>
        public const string BZLX = "C3";

        /// <summary>
        /// 界线类型
        /// </summary>
        public const string JXLX = "C4";

        /// <summary>
        /// 界线性质
        /// </summary>
        public const string JXXZ = "C5";

        /// <summary>
        /// 所有权性质
        /// </summary>
        public const string SYQXZ = "C6";

        /// <summary>
        /// 地块类别
        /// </summary>
        public const string DKLB = "C7";

        /// <summary>
        /// 地力等级
        /// </summary>
        public const string DLDJ = "C8";

        /// <summary>
        /// 土地用途
        /// </summary>
        public const string TDYT = "C9";

        /// <summary>
        /// 承包经营权取得方式
        /// </summary>
        public const string CBJYQQDFS = "C10";

        /// <summary>
        /// 界址点类型
        /// </summary>
        public const string JZDLX = "C11";

        /// <summary>
        /// 界标类型
        /// </summary>
        public const string JBLX = "C12";

        /// <summary>
        /// 界址线类别
        /// </summary>
        public const string JZXLB = "C13";

        /// <summary>
        /// 界址线位置
        /// </summary>
        public const string JZXWZ = "C14";

        /// <summary>
        /// 证件类型
        /// </summary>
        public const string ZJLX = "C15";

        /// <summary>
        /// 承包方类型
        /// </summary>
        public const string CBFLX = "C16";

        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "C17";

        /// <summary>
        /// 成员备注
        /// </summary>
        public const string CYBZ = "C18";

        /// <summary>
        /// 是否
        /// </summary>
        public const string SF = "C19";

        /// <summary>
        /// 耕保种类
        /// </summary>
        public const string GBZL = "C20";

        /// <summary>
        /// 耕地坡度级
        /// </summary>
        public const string GDPDJ = "C21";

        /// <summary>
        /// 经营方式
        /// </summary>
        public const string JYFS = "C22";

        /// <summary>
        /// 流转类型
        /// </summary>
        public const string LZLX = "C23";

        /// <summary>
        /// 实体状态
        /// </summary>
        public const string STZT = "C24";

        /// <summary>
        /// 种植类型
        /// </summary>
        public const string ZZLX = "C25";

        /// <summary>
        /// 土地利用类型
        /// </summary>
        public const string TDLYLX = "C26";

        /// <summary>
        /// 土地权属类型
        /// </summary>
        public const string TDQSLX = "C27";

        #region 地块类别

        /// <summary>
        /// 承包地块
        /// </summary>
        public const string CBDK = "10";

        /// <summary>
        /// 自留地
        /// </summary>
        public const string ZLD = "21";

        /// <summary>
        /// 机动地
        /// </summary>
        public const string JDD = "22";

        /// <summary>
        /// 开荒地
        /// </summary>
        public const string KHD = "23";

        /// <summary>
        /// 其他集体土地
        /// </summary>
        public const string QTJTTD = "99";

        /// <summary>
        /// 经济地
        /// </summary>
        public const string JJJ = "3";

        /// <summary>
        /// 饲料地
        /// </summary>
        public const string SLD = "7";

        /// <summary>
        /// 撂荒地
        /// </summary>
        public const string LHD = "8";

        #endregion 地块类别

        #region 承包方类型

        /// <summary>
        /// 农户
        /// </summary>
        public const string NH = "1";

        /// <summary>
        /// 个人
        /// </summary>
        public const string GR = "2";

        /// <summary>
        /// 单位
        /// </summary>
        public const string DW = "3";

        #endregion 承包方类型

        #region 土地权属类型

        /// <summary>
        /// 集体土地
        /// </summary>
        public const string JTTD = "1";

        /// <summary>
        /// 集体建设用地
        /// </summary>
        public const string JTJSYD = "2";

        /// <summary>
        /// 农村宅基地
        /// </summary>
        public const string NCZJD = "3";

        /// <summary>
        /// 集体农用地
        /// </summary>
        public const string JTNYD = "4";

        /// <summary>
        /// 房屋
        /// </summary>
        public const string FW = "5";

        /// <summary>
        /// 其它
        /// </summary>
        public const string QT = "20";

        #endregion 土地权属类型

        #region 是否

        /// <summary>
        /// 是
        /// </summary>
        public const string S = "1";

        /// <summary>
        /// 否
        /// </summary>
        public const string F = "2";

        #endregion 是否

        #region 地力等级

        /// <summary>
        /// 一等地
        /// </summary>
        public const string YDD = "01";

        /// <summary>
        /// 二等地
        /// </summary>
        public const string EDD = "02";

        /// <summary>
        /// 三等地
        /// </summary>
        public const string SDD = "03";

        /// <summary>
        /// 四等地
        /// </summary>
        public const string SIDD = "04";

        /// <summary>
        /// 五等地
        /// </summary>
        public const string WDD = "05";

        /// <summary>
        /// 六等地
        /// </summary>
        public const string LDD = "06";

        /// <summary>
        /// 七等地
        /// </summary>
        public const string QDD = "07";

        /// <summary>
        /// 八等地
        /// </summary>
        public const string BDD = "08";

        /// <summary>
        /// 九等地
        /// </summary>
        public const string JIUDD = "09";

        /// <summary>
        /// 十等地
        /// </summary>
        public const string SHIDD = "10";

        /// <summary>
        ///
        /// </summary>
        public const string K = "900";

        #endregion 地力等级

        #region 所有权性质

        /// <summary>
        /// 国有土地所有权
        /// </summary>
        public const string GYTDSYQ = "10";

        /// <summary>
        /// 集体土地所有权
        /// </summary>
        public const string JTTDSYQ = "30";

        /// <summary>
        /// 村民小组
        /// </summary>
        public const string CMXZ = "31";

        /// <summary>
        /// 村级集体经济组织
        /// </summary>
        public const string CJJTJJZZ = "32";

        /// <summary>
        /// 乡级集体经济组织
        /// </summary>
        public const string XJJTJJZZ = "33";

        /// <summary>
        /// 其他农民集体经济组织
        /// </summary>
        public const string QTNNJTJJZZ = "34";

        #endregion 所有权性质

        #endregion Field-Constc
    }
}