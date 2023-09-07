using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTuQuality.Business.Entity;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    public class LocalComplexSpaceEntity
    {
        public LocalComplexSpaceEntity()
        {

        }

        public List<Library.Entity.DZDW> DZDW { get; set; }
        public List<Library.Entity.FarmLandConserve> JBNTBHQ { get; set; }
        //public List<KZD> KZD { get; set; }
        public List<Library.Entity.MZDW> MZDW { get; set; }
        public List<Library.Entity.ZoneBoundary> QYJX { get; set; }
        //public List<SGSJ> SGSJ { get; set; }
        public List<Library.Entity.XZDW> XZDW { get; set; }
        public List<Library.Entity.ControlPoint> KZD { get; set; }
        //public List<ZJ> ZJ { get; set; }
        public string ZoneCode { get; set; }

        public static LocalComplexSpaceEntity From(ComplexSpaceEntity value)
        {
            var obj = new LocalComplexSpaceEntity();
            obj.DZDW = From(value.DZDW);
            obj.JBNTBHQ = From(value.JBNTBHQ);
            obj.MZDW = From(value.MZDW);
            obj.QYJX = From(value.QYJX);
            obj.XZDW = From(value.XZDW);
            obj.KZD = From(value.KZD);
            obj.ZoneCode = value.ZoneCode;

            return obj;
        }

        private static List<ControlPoint> From(List<KZD> kZD)
        {
            List<Library.Entity.ControlPoint> list = new List<Library.Entity.ControlPoint>();
            if (kZD == null)
                return list;

            foreach (var item in kZD)
                list.Add(From(item));

            return list;
        }

        private static List<Library.Entity.XZDW> From(List<YuLinTuQuality.Business.Entity.XZDW> xZDW)
        {
            List<Library.Entity.XZDW> list = new List<Library.Entity.XZDW>();
            if (xZDW == null)
                return list;

            foreach (var item in xZDW)
                list.Add(From(item));

            return list;
        }

        private static List<ZoneBoundary> From(List<QYJX> qYJX)
        {
            List<ZoneBoundary> list = new List<ZoneBoundary>();
            if (qYJX == null)
                return list;

            foreach (var item in qYJX)
                list.Add(From(item));

            return list;
        }

        private static List<Library.Entity.MZDW> From(List<YuLinTuQuality.Business.Entity.MZDW> mZDW)
        {
            List<Library.Entity.MZDW> list = new List<Library.Entity.MZDW>();
            if (mZDW == null)
                return list;

            foreach (var item in mZDW)
                list.Add(From(item));

            return list;
        }

        private static List<FarmLandConserve> From(List<JBNTBHQ> jBNTBHQ)
        {
            List<FarmLandConserve> list = new List<FarmLandConserve>();
            if (jBNTBHQ == null)
                return list;

            foreach (var item in jBNTBHQ)
                list.Add(From(item));

            return list;
        }

        public static List<Library.Entity.DZDW> From(List<YuLinTuQuality.Business.Entity.DZDW> dZDW)
        {
            List<Library.Entity.DZDW> list = new List<Library.Entity.DZDW>();
            if (dZDW == null)
                return list;

            foreach (var item in dZDW)
                list.Add(From(item));

            return list;
        }

        private static ControlPoint From(KZD item)
        {
            var obj = new Library.Entity.ControlPoint();
            obj.BsType = item.BSLX;
            obj.BzType = item.BZLX;
            obj.Code = item.BSM;
            obj.Dzj = item.DZJ == null ? string.Empty : item.DZJ.ToString();
            obj.ID = Guid.NewGuid();
            obj.FeatureCode = "111000";
            obj.Shape = item.Shape == null ? null : Spatial.Geometry.FromBytes(Spatial.Geometry.FromInstance(item.Shape as GeoAPI.Geometries.IGeometry).AsBinary(), (item.Shape as GeoAPI.Geometries.IGeometry).SRID);
            obj.PointName = item.KZDMC;
            obj.PointNumber = item.KZDDH;
            obj.PointRank = item.KZDDJ;
            obj.PointState = item.KZDZT;
            obj.PointType = item.KZDLX;
            obj.X2000 = item.X2000;
            obj.X80 = item.X80;
            obj.Y2000 = item.Y2000;
            obj.Y80 = item.Y80;

            return obj;
        }

        private static Library.Entity.XZDW From(YuLinTuQuality.Business.Entity.XZDW item)
        {
            var obj = new Library.Entity.XZDW();
            obj.BSM = item.BSM;
            obj.CD = item.CD != null ? item.CD.Value : 0;
            obj.Comment = item.BZ;
            obj.DWMC = item.DWMC;
            obj.ID = Guid.NewGuid();
            obj.KD = item.KD != null ? item.KD.Value : 0;
            obj.Shape = item.Shape == null ? null : Spatial.Geometry.FromBytes(Spatial.Geometry.FromInstance(item.Shape as GeoAPI.Geometries.IGeometry).AsBinary(), (item.Shape as GeoAPI.Geometries.IGeometry).SRID);
            obj.YSDM = item.YSDM;

            return obj;
        }

        private static ZoneBoundary From(QYJX item)
        {
            var obj = new Library.Entity.ZoneBoundary();
            obj.BoundaryLineNature = item.JXXZ;
            obj.BoundaryLineType = item.JXLX;
            obj.Code = item.BSM;
            obj.FeatureCode = item.YSDM;
            obj.ID = Guid.NewGuid();
            obj.Shape = item.Shape == null ? null : Spatial.Geometry.FromBytes(Spatial.Geometry.FromInstance(item.Shape as GeoAPI.Geometries.IGeometry).AsBinary(), (item.Shape as GeoAPI.Geometries.IGeometry).SRID);

            return obj;
        }

        private static Library.Entity.MZDW From(YuLinTuQuality.Business.Entity.MZDW item)
        {
            var obj = new Library.Entity.MZDW();
            obj.BSM = item.BSM;
            obj.Area = Math.Round(item.MJ != null ? item.MJ.Value : 0, 2);
            obj.Comment = item.BZ;
            obj.DWMC = item.DWMC;
            obj.ID = Guid.NewGuid();
            obj.Shape = item.Shape == null ? null : Spatial.Geometry.FromBytes(Spatial.Geometry.FromInstance(item.Shape as GeoAPI.Geometries.IGeometry).AsBinary(), (item.Shape as GeoAPI.Geometries.IGeometry).SRID);
            obj.YSDM = item.YSDM;

            return obj;
        }

        private static FarmLandConserve From(JBNTBHQ item)
        {
            var obj = new Library.Entity.FarmLandConserve();
            obj.Code = item.BSM;
            obj.ConserveNumber = item.BHQBH;
            obj.FarmLandArea = item.JBNTMJ;
            obj.ID = Guid.NewGuid();
            obj.Shape = item.Shape == null ? null : Spatial.Geometry.FromBytes(Spatial.Geometry.FromInstance(item.Shape as GeoAPI.Geometries.IGeometry).AsBinary(), (item.Shape as GeoAPI.Geometries.IGeometry).SRID);
            obj.FeatureCode = item.YSDM;

            return obj;
        }

        private static Library.Entity.DZDW From(YuLinTuQuality.Business.Entity.DZDW item)
        {
            var obj = new Library.Entity.DZDW();
            obj.BSM = item.BSM;
            obj.Comment = item.BZ;
            obj.DWMC = item.DWMC;
            obj.ID = Guid.NewGuid();
            obj.Shape = item.Shape == null ? null : Spatial.Geometry.FromBytes(Spatial.Geometry.FromInstance(item.Shape as GeoAPI.Geometries.IGeometry).AsBinary(), (item.Shape as GeoAPI.Geometries.IGeometry).SRID);
            obj.YSDM = item.YSDM;

            return obj;
        }
    }
}
