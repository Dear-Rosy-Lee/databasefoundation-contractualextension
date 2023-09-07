using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

//using YuLinTu.Library.Entity;
using YuLinTu.NetAux;
using YuLinTu.NetAux.CglLib;

namespace YuLinTu.Library.Business
{
    public class JzdEdge
    {
        /// <summary>
        /// 出度
        /// </summary>
        public Jzx OutEdge;

        /// <summary>
        /// 入度
        /// </summary>
        public Jzx InEdge;

        public ShortZd_cbd dk { get { return OutEdge.dk; } }
    }

    public class JzdEdges : List<JzdEdge>
    {
        public int jzdh;

        /// <summary>
        /// 是否关键界址点
        /// </summary>
        public bool? fKeyJzd = null;
    }

    /// <summary>
    /// 承包地缓存集合
    /// </summary>
    public class ShortZd_cbdCache : List<ShortZd_cbd>
    {
        public double x1;
    }

    /// <summary>
    /// 承包地缓存集合
    /// </summary>
    public class ShortZd_cbdCache1 : List<ShortZd_cbd1>
    {
        public double x1;
    }

    /// <summary>
    /// 记录每个地块的外切横坐标
    /// </summary>
    public struct XBounds
    {
        public int rowid;
        public int minx;
        public int maxx;
    }

    /// <summary>
    /// 点相等的比较
    /// </summary>
    public class JzdEqualComparer : IEqualityComparer<Coordinate>
    {
        private double _tolerace, _tolerace2;
        private Coordinate _tmpC = new Coordinate();

        public JzdEqualComparer(double tolerance)
        {
            _tolerace = tolerance;
            _tolerace2 = tolerance * tolerance;
        }

        public bool Equals(Coordinate a, Coordinate b)
        {
            return CglHelper.IsSame2(a, b, _tolerace2);
        }

        public int GetHashCode(Coordinate obj)
        {
            _tmpC.X = func(obj.X);// Math.Round(obj.X, 3);
            _tmpC.Y = func(obj.Y);// Math.Round(obj.Y, 3);
            return _tmpC.GetHashCode();
        }

        private static double func(double x)
        {
            //return Math.Round(x, 3);
            long n = (long)((x + 0.00000001) * 100);
            return n / 100.0;
        }
    }

    /// <summary>
    /// //按Y轴优先排序
    /// </summary>
    public class JzdComparer : Comparer<Coordinate>
    {
        private double _tolerace;

        public JzdComparer(double tolerace)
        {
            _tolerace = tolerace;
        }

        public override int Compare(Coordinate a, Coordinate b)
        {
            if (a.Y + _tolerace < b.Y)
                return -1;
            if (b.Y + _tolerace < a.Y)
                return 1;
            if (a.X + _tolerace < b.X)
                return -1;
            if (b.X + _tolerace < a.X)
                return 1;
            return 0;
        }
    }

    /// <summary>
    /// 初始化界址点界址线的参数类
    /// </summary>
    public class InitLandDotCoilParam
    {
        /// <summary>
        /// 领宗地距离，单位：米
        /// </summary>
        public double AddressLinedbiDistance = 1.5;

        public double Tolerance = 0.05;//视为同一个点的距离容差，单位：米

        /// <summary>
        /// 界址线类别字典
        /// </summary>
        public List<YuLinTu.Library.Entity.Dictionary> Jzxlbdics { set; get; }

        /// <summary>
        /// 界址线类别配置文件
        /// </summary>
        public List<JzxlbxmlClass> jzxdicxmlstrs { set; get; }

        /// <summary>
        /// 界址标识
        /// </summary>
        public string AddressPointPrefix = "J";

        /// <summary>
        /// 最小过滤角度值，单位：度
        /// </summary>
        public double? MinAngleFileter = 10;

        /// <summary>
        /// 最大过滤角度值，单位：度
        /// </summary>
        public double? MaxAngleFilter = 120;

        /// <summary>
        /// 最小过滤角度值，单位：度
        /// </summary>
        public double? MinAngleFileterExtend = 0;

        /// <summary>
        /// 最大过滤角度值，单位：度
        /// </summary>
        public double? MaxAngleFilterExtend = 90;

        /// <summary>
        /// 是否对界址点进行过滤
        /// </summary>
        public bool IsFilter = true;

        /// <summary>
        /// 一个地块包含的最少关键界址点个数
        /// </summary>
        public short MinKeyJzdCount = 3;

        /// <summary>
        /// 界址点类型
        /// </summary>
        public short AddressDotType = 1;

        /// <summary>
        /// 界标类型
        /// </summary>
        public short AddressDotMarkType = 3;

        /// <summary>
        /// 界线性质
        /// </summary>
        public string AddressLineType = "600001";

        /// <summary>
        /// 界址线类型
        /// </summary>
        public string AddressLineCatalog = "01";

        /// <summary>
        /// 界址线位置
        /// </summary>
        public string AddressLinePosition = "1";

        /// <summary>
        /// 界址线说明填写长度//批量初始化没有用
        /// </summary>
        public bool IsLineDescription { set; get; }

        /// <summary>
        /// 是否初始化界址线位置//批量初始化没有用
        /// </summary>
        public bool IsAddressLinePosition { set; get; }

        public bool UseAddAlgorithm { get; set; } = true;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime cjsj;

        /// <summary>
        /// 所属权利类型
        /// </summary>
        public string AddressLineRightType { set; get; }

        /// <summary>
        /// 是否初始化时设置界址线位置为中和内//就是将找出外的改为内。
        /// </summary>
        public bool IsSetAddressLinePosition { get; set; }

        /// <summary>
        /// 界址线说明初始化的类型
        /// </summary>
        public EnumDescription LineDescription { set; get; }

        /// <summary>
        /// 界址线说明是否全域统编
        /// </summary>
        public bool IsUnit { set; get; }

        public InitLandDotCoilParam()
        {
            AddressLineRightType = "4";//集体农用地 ((int)eLandPropertyRightType.AgricultureLand).ToString();
        }
    }

    /// <summary>
    /// 用整数表示坐标值
    /// </summary>
    public class IntCoordConter
    {
        private double _minValue;

        public void Init(double minValue)
        {
            _minValue = minValue;
        }

        public int toInt(double value)
        {
            return (int)((value - _minValue) * 10000);
        }

        public double toDouble(int value)
        {
            return value / 10000.0 + _minValue;
        }
    }

    public enum EnumDescription
    {
        /// <summary>
        /// 长度
        /// </summary>
        LineLength = 0,

        /// <summary>
        /// 长度及方向
        /// </summary>
        LineFind,

        /// <summary>
        /// 长度方向及类型
        /// </summary>
        LineFindType
    }

    /// <summary>
    /// 界址线类别配置文件使用的大类节点和包含项目
    /// </summary>
    public class JzxlbxmlClass
    {
        public string JzxlbdicNameCode;

        public string JzxlbdicNameContains;
    }
}