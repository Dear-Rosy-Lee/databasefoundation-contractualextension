using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包地字段常量
    /// </summary>
    public class Zd_cbdFields
    {
        public const string TABLE_NAME = "ZD_CBD";
        public const string RowID = "rowid";
        public const string ID = "ID";
        /// <summary>
        /// 地域代码
        /// </summary>
        public const string ZLDM = "ZLDM";
        public const string Shape = "shape";
        /// <summary>
        /// 权利人名称
        /// </summary>
        public const string Qlrmc = "qlrmc";
        public const string DKBM = "DKBM";
        /// <summary>
        /// 地块名称
        /// </summary>
        public const string DKMC = "DKMC";
        /// <summary>
        /// 扩展属性
        /// </summary>
        public const string DKKZXX = "DKKZXX";
        public const string DKLB = "DKLB";
        public const string TDLYLX = "TDLYLX";
        public const string DLDJ = "DLDJ";
        public const string TDYT = "TDYT";
        public const string SFJBNT = "SFJBNT";
        public const string SCMJ = "SCMJ";
        public const string DKDZ = "DKDZ";
        public const string DKXZ = "DKXZ";
        public const string DKNZ = "DKNZ";
        public const string DKBZ = "DKBZ";
        public const string DKBZXX = "DKBZXX";

        /// <summary>
        /// 扩展信息，用于保存最终相邻地块信息
        /// </summary>
        public const string YLD = "YLD";

    }

    /// <summary>
    /// 界址点字段常量
    /// </summary>
    public class JzdFields
    {
        public const string TABLE_NAME = "JZD";
        public const string ID = "ID";
        public const string DKID = "DKID";
        ///// <summary>
        ///// 标识码
        ///// </summary>
        //public const string BSM = "BSM";
        /// <summary>
        /// 统编界址点号
        /// </summary>
        public const string TBJZDH = "TBJZDH";
        /// <summary>
        /// 界址点号
        /// </summary>
        public const string JZDH = "JZDH";
        /// <summary>
        /// 界标类型
        /// </summary>
        public const string JBLX = "JBLX";
        /// <summary>
        /// 界址点类型
        /// </summary>
        public const string JZDLX = "JZDLX";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CJSJ = "CJSJ";
        /// <summary>
        /// 地域编码
        /// </summary>
        public const string DYBM = "DYBM";
        /// <summary>
        /// 地块编码
        /// </summary>
        public const string DKBM = "DKBM";
        /// <summary>
        /// 是否关键界址点
        /// </summary>
        public const string SFKY = "SFKY";

        public const string JZDSSQLLX = "JZDSSQLLX";

        public const string SHAPE = "shape";
    }

    /// <summary>
    /// 界址点实体
    /// </summary>
    public class JzdEntity
    {
        public int rowID;
        /// <summary>
        /// 界址点ID
        /// </summary>
        public string ID;
        ///// <summary>
        ///// 标识码
        ///// </summary>
        //public string BSM;
        /// <summary>
        /// 界址点号
        /// </summary>
        public short JZDH;

        /// <summary>
        /// 统编界址点号
        /// </summary>
        public int TBJZDH;

        /// <summary>
        /// 界标类型
        /// </summary>
        public short JBLX;
        /// <summary>
        /// 界址点类型
        /// </summary>
        public short JZDLX;
        ///// <summary>
        ///// 地块标识
        ///// </summary>
        //public string dkID;
        ///// <summary>
        ///// 地域编码
        ///// </summary>
        //public string DYBM;
        public string JZDSSQLLX;
        ///// <summary>
        ///// 地块编码
        ///// </summary>
        //public string dkBM;
        /// <summary>
        /// 是否关键界址点
        /// </summary>
        public bool SFKY = false;
        public Coordinate shape;
        /// <summary>
        /// 是否环的最后一个点
        /// </summary>
        public bool fRingLastPoint = false;
    }

    /// <summary>
    /// 线状地物表常量
    /// </summary>
    public class XzdwFields
    {
        public const string TABLE_NAME = "xzdw";
        public const string DWMC = "dwmc";
        public const string SHAPE = "shape";
        public const string RowID = "rowid";
        public const string BZ = "bz";
    }
    /// <summary>
    /// 面状地物表常量
    /// </summary>
    public class MzdwFields
    {
        public const string TABLE_NAME = "mzdw";
        public const string DWMC = "dwmc";
        public const string SHAPE = "shape";
        public const string RowID = "rowid";
        public const string BZ = "bz";
    }

    /// <summary>
    /// 线状地物实体
    /// </summary>
    public class XzdwEntity
    {        
        public int rowid;
        public string DWMC;
        public string BZ;
        public IGeometry Shape;
    }

    /// <summary>
    /// 面状地物实体
    /// </summary>
    public class MzdwEntity
    {       
        public int rowid;
        public string DWMC;
        public string BZ;
        public IGeometry Shape;
    }

    /// <summary>
    /// 界址线字段常量
    /// </summary>
    public class JzxFields
    {
        public const string TABLE_NAME = "JZX";
        public const string ID = "ID";
        public const string DKID = "DKBS";
        /// <summary>
        /// 界址线长度
        /// </summary>
        public const string JZXCD = "JZXCD";
        /// <summary>
        /// 界线性质
        /// </summary>
        public const string JXXZ = "JXXZ";
        /// <summary>
        /// 界址线类别
        /// </summary>
        public const string JZXLB = "JZXLB";
        /// <summary>
        /// 界址线位置
        /// </summary>
        public const string JZXWZ = "JZXWZ";
        /// <summary>
        /// 界址线顺序号
        /// </summary>
        public const string JZXSXH = "JZXSXH";
        /// <summary>
        /// 界址线起点对应界址点的ID
        /// </summary>
        public const string JZXQD = "JZXQD";
        /// <summary>
        /// 界址线止点对应界址点的ID
        /// </summary>
        public const string JZXZD = "JZXZD";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CJSJ = "CJSJ";
        public const string ZHXGSJ = "ZHXGSJ";
        public const string JZXSM = "JZXSM";
        /// <summary>
        /// 毗邻地物指界人
        /// </summary>
        public const string PLDWZJR = "PLDWZJR";
        /// <summary>
        /// 毗邻地物权利人
        /// </summary>
        public const string PLDWQLR = "PLDWQLR";
        /// <summary>
        /// 地域代码
        /// </summary>
        public const string DYDM = "DYDM";
        /// <summary>
        /// 
        /// </summary>
        public const string TDQSLX = "TDQSLX";
        /// <summary>
        /// 地块编码
        /// </summary>
        public const string DKBM = "DKBM";
        /// <summary>
        /// 界址线起点号
        /// </summary>
        public const string JZXQDH = "JZXQDH";
        /// <summary>
        /// 界址线止点号
        /// </summary>
        public const string JZXZDH = "JZXZDH";

        public const string SHAPE = "shape";
    }

    /// <summary>
    /// 界址线实体
    /// </summary>
    public class JzxEntity
    {
        public int rowid;
        public string ID;
        /// <summary>
        /// 地块ID
        /// </summary>
        public string DKID;
        /// <summary>
        /// 界址线长度[13,8]
        /// </summary>
        public double JZXCD;
        /// <summary>
        /// 界线性质
        /// </summary>
        public string JXXZ;
        /// <summary>
        /// 界址线类别
        /// </summary>
        public string JZXLB;
        /// <summary>
        /// 界址线位置
        /// </summary>
        public string JZXWZ;

        /// <summary>
        /// 界址线顺序号
        /// </summary>
        public int JZXSXH;
        /// <summary>
        /// 界址线起点对应界址点的ID
        /// </summary>
        public string JZXQD;
        /// <summary>
        /// 界址线止点对应界址点的ID
        /// </summary>
        public string JZXZD;
        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //public string CJSJ = "CJSJ";
        //public string ZHXGSJ = "ZHXGSJ";
        public string JZXSM;
        /// <summary>
        /// 毗邻地物指界人
        /// </summary>
        public string PLDWZJR;
        /// <summary>
        /// 毗邻地物权利人
        /// </summary>
        public string PLDWQLR;
        /// <summary>
        /// 地域代码
        /// </summary>
        public string DYDM;
        /// <summary>
        /// 
        /// </summary>
        public string TDQSLX;
        /// <summary>
        /// 地块编码
        /// </summary>
        public string DKBM;
        /// <summary>
        /// 界址线起点号
        /// </summary>
        public string JZXQDH;
        /// <summary>
        /// 界址线止点号
        /// </summary>
        public string JZXZDH;

        public IGeometry Shape;
        //public List<JzdEntity> lstJzd;
        public void Clear()
        {
            rowid = 0;
            ID = InitLandDotCoilUtil.CreateNewID();
        }
    }
}
