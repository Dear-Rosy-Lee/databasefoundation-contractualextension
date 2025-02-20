/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Spatial;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 模板文件帮助类
    /// </summary>
    public class TemplateHelper
    {
        #region Methods

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public static string ExcelTemplate(string templateName)
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                string.Format(@"Template\{0}.xlt", templateName));
            if (!File.Exists(fileName))
                throw new Exception(string.Format("模板文件:{0}.xlt不存在", templateName));
            return fileName;
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public static string GetTemplate(string templateName)
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                string.Format(@"Template\{0}", templateName));
            if (!File.Exists(fileName))
                throw new Exception(string.Format("模板文件:{0}不存在", templateName));
            return fileName;
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public static string WordTemplate(string templateName)
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                string.Format(@"Template\{0}.dot", templateName));
            if (!File.Exists(fileName))
                throw new Exception(string.Format("模板文件:{0}.dot不存在", templateName));
            return fileName;
        }

        #endregion
    }

    /// <summary>
    /// 获取村级地域地块的帮助类
    /// </summary>
    public class ContractLandHeler
    {
        public static List<ContractLand> GetCurrentVillageContractLand(Zone currentZone, IDbContext db, double buffer)
        {
            List<ContractLand> VillageLands = new List<ContractLand>();
            if (currentZone == null || db == null)
                return VillageLands;
            // 提高速度的方法：不要用like—即StartWith,EndWith之类的；只取需要的字段，不要取全部字段
            //if (currentZone.Level == eZoneLevel.Village)
            //{
            VillageLands = db.CreateQuery<ContractLand>().
                Where(c => c.ZoneCode.Contains(currentZone.FullCode) && c.Shape != null).
                Select(c => new ContractLand()
                {
                    Shape = c.Shape,
                    //ID = c.ID,
                    //LandName = c.LandName,
                    //OwnerName = c.OwnerName
                }).ToList();
            if (VillageLands.Count == 0) return VillageLands;
            //}
            //else if (currentZone.Level == eZoneLevel.Group)
            //{
            //    var query = db.CreateQuery<ContractLand>();
            //    var qc = db.CreateQuery<Zone>().Where(z => z.UpLevelCode == currentZone.UpLevelCode).Select(z => z.FullCode);  // 取所有的组级编码
            //    VillageLands = query.Where(c => qc.Contains(c.ZoneCode) && c.Shape != null).
            //        Select(c => new ContractLand()
            //        {
            //            Shape = c.Shape,
            //            //ID = c.ID,
            //            //LandName = c.LandName,
            //            //OwnerName = c.OwnerName
            //        }).ToList();
            //}
            //Spatial.Geometry envelope = null;
            Spatial.Envelope extent = new Envelope();
            foreach (var item in VillageLands)
            {
                var ext = item.Shape.GetEnvelope();
                extent.Union(ext);
            }
            var landQuery = db.CreateQuery<ContractLand>();
            var geoExtent = extent.ToGeometry();
            if (buffer > 0)
            {
                geoExtent = geoExtent.Buffer(buffer);
            }
            VillageLands = landQuery.Where(c => c.Shape.Intersects(geoExtent)).
                Select(c => new ContractLand()
                {
                    Shape = c.Shape,
                    ID = c.ID,
                    LandNumber = c.LandNumber,
                    OwnerName = c.OwnerName
                }).ToList();
            return VillageLands;
        }

        /// <summary>
        /// 获取导出地块示意图的地块
        /// </summary>
        public static List<ContractLand> GetParcelLands(string zoneCode, IDbContext db, bool containsOtherZoneLand)
        {
            var lands = new List<ContractLand>();
            if (string.IsNullOrEmpty(zoneCode) || db == null)
                return lands;

            var qy = db.CreateQuery<ContractLand>().Where(c => c.ZoneCode.Equals(zoneCode) && c.Shape != null);
            if (containsOtherZoneLand && zoneCode.Length == 14)//是否按村来搜索地块
            {
                string vz = zoneCode.Substring(0, 12);
                qy = db.CreateQuery<ContractLand>().Where(c => c.ZoneCode.StartsWith(vz) && c.Shape != null);
            }

            lands = qy.Select(c => new ContractLand()
            {
                Shape = c.Shape,
                ID = c.ID,
                LandNumber = c.LandNumber,
                OwnerName = c.OwnerName,
                ActualArea = c.ActualArea,
                AwareArea = c.AwareArea,
                TableArea = c.TableArea,
                // Comment = c.Comment,
                LandCategory = c.LandCategory,
                ConcordArea = c.ConcordArea,
                ConcordId = c.ConcordId,
                //ConstructMode = c.ConstructMode,
                //IsStockLand = c.IsStockLand,
                LandCode = c.LandCode,
                LandLevel = c.LandLevel,
                //LandName = c.LandName,
                SenderName = c.SenderName,
                SenderCode = c.SenderCode,
                Name = c.Name,
                NeighborEast = c.NeighborEast,
                NeighborNorth = c.NeighborNorth,
                NeighborSouth = c.NeighborSouth,
                NeighborWest = c.NeighborWest,
                OwnerId = c.OwnerId,
                //Purpose = c.Purpose,
                //PlatType = c.PlatType,
            }).ToList();

            //if (containsOtherZoneLand && lands.Count > 0)
            //{
            //    Geometry eg = null;
            //    foreach (var item in lands)
            //    {
            //        if (item.Shape == null)
            //            continue;
            //        eg = eg == null ? item.Shape.Envelope() : eg.Union(item.Shape.Envelope());
            //    }
            //    lands = db.CreateQuery<ContractLand>().
            //    Where(c => (c.ZoneCode.Equals(zoneCode) && c.Shape != null) || c.Shape.Intersects(eg)).
            //    Select(c => new ContractLand()
            //    {
            //        Shape = c.Shape,
            //        ID = c.ID,
            //        LandNumber = c.LandNumber,
            //        OwnerName = c.OwnerName,
            //        ActualArea = c.ActualArea,
            //        AwareArea = c.AwareArea,
            //        TableArea = c.TableArea,
            //        // Comment = c.Comment,
            //        LandCategory = c.LandCategory,
            //        ConcordArea = c.ConcordArea,
            //        ConcordId = c.ConcordId,
            //        //ConstructMode = c.ConstructMode,
            //        //IsStockLand = c.IsStockLand,
            //        LandCode = c.LandCode,
            //        LandLevel = c.LandLevel,
            //        //LandName = c.LandName,
            //        SenderName = c.SenderName,
            //        SenderCode = c.SenderCode,
            //        Name = c.Name,
            //        NeighborEast = c.NeighborEast,
            //        NeighborNorth = c.NeighborNorth,
            //        NeighborSouth = c.NeighborSouth,
            //        NeighborWest = c.NeighborWest,
            //        OwnerId = c.OwnerId,
            //        //Purpose = c.Purpose,
            //        //PlatType = c.PlatType,
            //    }).ToList();
            //}
            return lands;
        }
    }
}