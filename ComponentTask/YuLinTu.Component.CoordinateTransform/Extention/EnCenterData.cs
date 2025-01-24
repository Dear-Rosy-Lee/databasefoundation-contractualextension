using Common.CShapeExport;
using System;
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Component.CoordinateTransformTask
{
    public class EnData
    {
        public Guid ID { get; set; }

        public int XIndex { get; set; }
        public int YIndex { get; set; }

        public string ZoneCode { get; set; }

        public Geometry Shape { get; set; }

        public double A0 { get; set; }
        public double A1 { get; set; }
        public double A2 { get; set; }

        public double B0 { get; set; }
        public double B1 { get; set; }
        public double B2 { get; set; }

        static public List<FieldInfo> FieldInfos()
        {
            var fieldList = new List<FieldInfo>();
            fieldList.Add(new FieldInfo()
            {
                Name = "Index",
                Type = DBFFieldType.FTInteger,
                Length = 10,
                Precision = 9
            });

            fieldList.Add(new FieldInfo()
            {
                Name = "A0",
                Type = DBFFieldType.FTDouble,
                Length = 15,
                Precision = 9
            });

            fieldList.Add(new FieldInfo()
            {
                Name = "A1",
                Type = DBFFieldType.FTDouble,
                Length = 150,
                Precision = 9
            });

            fieldList.Add(new FieldInfo()
            {
                Name = "A2",
                Type = DBFFieldType.FTDouble,
                Length = 15,
                Precision = 9
            });

            fieldList.Add(new FieldInfo()
            {
                Name = "B0",
                Type = DBFFieldType.FTDouble,
                Length = 15,
                Precision = 9
            });

            fieldList.Add(new FieldInfo()
            {
                Name = "B1",
                Type = DBFFieldType.FTDouble,
                Length = 15,
                Precision = 9
            });

            fieldList.Add(new FieldInfo()
            {
                Name = "B2",
                Type = DBFFieldType.FTDouble,
                Length = 15,
                Precision = 9
            });

            return fieldList;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataTable("EnCenterData")]
    public class EnCenterData// : EnData
    {
        public Guid ID { get; set; }

        public Geometry EnvelopShape { get; set; }

        /// <summary>
        /// 所有地块
        /// </summary>
        [DataColumn(Enabled = false)]
        public HashSet<CodeData> AllLandList { get; set; }

        /// <summary>
        /// 行政地域编码
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        [DataColumn(Enabled = false)]
        public int HH { get; set; }


        /// <summary>
        /// 列号
        /// </summary>
        [DataColumn(Enabled = false)]
        public int LH { get; set; }

        /// <summary>
        /// 中心区域地块编码
        /// </summary>
        //public HashSet<CodeData> CenterlandList { get; set; }
        //public List<EnCenterData> ChildrenList { get; set; }

        public EnCenterData()
        {
            ID = Guid.NewGuid();
            AllLandList = new HashSet<CodeData>();
            //CenterlandList = new HashSet<CodeData>();
            //ChildrenList = new List<EnCenterData>();
        }
    }

    public class CodeData
    {
        /// <summary>
        /// En的ID
        /// </summary>
        public Guid EnId { get; set; }

        public int IsBase { get; set; }

        public string Code { get; set; }

        public int RowNumber { get; set; }

        //public Coordinate Coord { get; set; }
        public Geometry Shape { get; set; }

    }

    [Serializable]
    public class IndexCode
    {
        public int Index { get; set; }

        public String Code { get; set; }
    }

}
