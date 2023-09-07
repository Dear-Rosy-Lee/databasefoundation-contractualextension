using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using YuLinTu.Data;
using YuLinTu;
using System.Xml.Linq;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 要素类型代码
    /// </summary>
    [Serializable]
    public class FeatureTypeCode
    {
        #region Filds

        public const string JCDLXX = "100000";// 基础地理信息要素a
        public const string DWJC = "110000";// 定位基础
        public const string KZD = "111000";// 控制点
        public const string KZDZJ = "112000";// 控制点注记
        public const string JJYGXQY = "160000";// 境界与管辖区域b
        public const string GXQYHJ = "161000";// 管辖区域划界
        public const string QYJX = "161051";// 区域界线
        public const string QYZJ = "161052";// 区域注记
        public const string GXQY = "162000";// 管辖区域
        public const string XJXZQ = "162010";// 县级行政区
        public const string XJQY = "162020";// 乡级区域
        public const string CJQY = "162030";// 村级区域
        public const string ZJQY = "162040";// 组级区域
        public const string QTDW = "190000";// 其他地物c
        public const string DZDW = "196011";// 点状地物
        public const string DZDWZJ = "196012";// 点状地物注记
        public const string XZDW = "196021";// 线状地物
        public const string XZDWZJ = "196022";// 线状地物注记
        public const string MZDW = "196031";// 面状地物
        public const string MZDWZJ = "196032";// 面状地物注记
        public const string NCTDQS = "200000";// 农村土地权属要素
        public const string CBD = "210000";// 承包地块要素
        public const string DK = "211011";// 地块
        public const string DKZJ = "211012";// 地块注记
        public const string JZD = "211021";// 界址点
        public const string JZDZJ = "211022";// 界址点注记
        public const string JZX = "211031";// 界址线
        public const string JZXZJ = "211032";// 界址线注记
        public const string JBNT = "250000";// 基本农田要素
        public const string JBNTBHQY = "251000";// 基本农田保护区域
        public const string JBNTBHQ = "251100";// 基本农田保护区
        public const string SGSJ = "300000";// 栅格数据
        public const string SZZSYXT = "310000";// 数字正射影像图
        public const string SZSGDT = "320000";// 数字栅格地图
        public const string QTSGSJ = "390000";// 其它栅格数据

        #endregion

        #region Ctor

        public FeatureTypeCode()
        {
        }

        #endregion

    }
}