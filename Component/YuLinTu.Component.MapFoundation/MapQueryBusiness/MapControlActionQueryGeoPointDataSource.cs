using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using YuLinTu.Data.Dynamic;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    public class MapControlActionQueryGeoPointDataSource : IDataPagerProvider
    {
        #region Properties

        public Layer Layer { get; private set; }
        public IGeoSource DataSource { get; private set; }

        /// <summary>
        /// 当前第几组设置，默认第0组显示
        /// </summary>
        public int resourcePage = 0;
        /// <summary>
        /// 二维数据源
        /// </summary>
        public Coordinate[][] geoCoordinate;

        public string FilterExpression { get; set; }
        public bool IsEnabledFilter { get; set; }

        #endregion

        #region Ctor

        public MapControlActionQueryGeoPointDataSource(Coordinate[][] geocoordinate)
        {
            geoCoordinate = geocoordinate;
        }

        #endregion

        #region Methods

        public List<object> Paging(int pageIndex, int pageSize, string orderPropertyName, eOrder order)
        {
            List<object> list = new List<object>();
            if (geoCoordinate == null) return list;
            //获取当前组别所有记录
            Coordinate[] tableuse = geoCoordinate[resourcePage];
            int[] id = new int[tableuse.Length];
            for (int idindex = 0; idindex < id.Length; idindex++) 
            {
                id[idindex] = idindex + 1;
            }

            double[] distence = new double[tableuse.Length];

            for (int d = 0; d <tableuse.Length; d++)
            {
             if (d == 0)
               {
                distence[0] = 0;
               }
            else
               {
                distence[d] = Math.Sqrt((tableuse[d].X - tableuse[d - 1].X) * (tableuse[d].X - tableuse[d - 1].X) + (tableuse[d].Y - tableuse[d - 1].Y) * (tableuse[d].Y - tableuse[d - 1].Y));
               } 
            }            

            //用于显示当前组别第几页数据
            Coordinate[] nowtabledisplay = null;
            List<Coordinate> nowtabledisplaylist = new List<Coordinate>();

            int[] nowtalbledisplayid = null;
            List<int> nowtabledisplayidlist = new List<int>();

            double[] nowtalbledisplaydistence = null;
            List<double> nowtabledisplaydistenclist = new List<double>();
            
            if(pageIndex == -1)
            {
                return null;             
            }
            if (pageIndex == 0)
            {
                for (int i = 0; i < pageIndex * pageSize + pageSize; i++)
                {
                    if (i >= tableuse.Length) break;
                    nowtabledisplaylist.Add(tableuse[i]);

                    nowtabledisplayidlist.Add(id[i]);

                    nowtabledisplaydistenclist.Add(distence[i]);                 
                                        
                }
                nowtabledisplay = nowtabledisplaylist.ToArray();
                //添加对应ID              
                nowtalbledisplayid = nowtabledisplayidlist.ToArray();
                //添加对应距离
                nowtalbledisplaydistence = nowtabledisplaydistenclist.ToArray();
            }
            else
              {
               for (int i = pageIndex * pageSize; i < pageIndex * pageSize + pageSize; i++)
                {
                    if (i >= tableuse.Length) break;
                    nowtabledisplaylist.Add(tableuse[i]);

                    nowtabledisplayidlist.Add(id[i]);

                    nowtabledisplaydistenclist.Add(distence[i]);
                }
                nowtabledisplay = nowtabledisplaylist.ToArray();
                nowtalbledisplayid = nowtabledisplayidlist.ToArray();
                nowtalbledisplaydistence = nowtabledisplaydistenclist.ToArray();
              }

            foreach (var item in getGeoPointUIList(nowtabledisplay, nowtalbledisplayid,nowtalbledisplaydistence))
            {
                list.Add(item);
            }

            return list;
        }

        public int Count()
        {
            if (resourcePage > geoCoordinate.Length) return 0;
            return geoCoordinate[resourcePage].Length;
        }

        public string GetDefaultOrderPropertyName()
        {
            if (DataSource == null)
                return string.Empty;

            var name = DataSource.PrimaryPropertyNames.Count > 0 ? DataSource.PrimaryPropertyNames[0] : string.Empty;
            if (!name.IsNullOrBlank())
                return name;

            return DataSource.GetAttributePropertyNames()[0];
        }

        public Data.Dynamic.PropertyMetadata[] GetAttributeProperties()
        {
            List<PropertyMetadata> md = new List<PropertyMetadata>();
            PropertyMetadata md0 = new PropertyMetadata();
            md0.AliasName = "序号";
            md0.ColumnName = "ID";
            md0.ColumnType = eDataType.Double;

            PropertyMetadata md1 = new PropertyMetadata();
            md1.AliasName = "X坐标";
            md1.ColumnName = "XCoordinate";
            md1.ColumnType = eDataType.Double;

            PropertyMetadata md2 = new PropertyMetadata();
            md2.AliasName = "Y坐标";
            md2.ColumnName = "YCoordinate";
            md2.ColumnType = eDataType.Double;

            PropertyMetadata md3 = new PropertyMetadata();
            md3.AliasName = "距离";
            md3.ColumnName = "Distance";
            md3.ColumnType = eDataType.Double;
            md.Add(md0);
            md.Add(md1);
            md.Add(md2);
            md.Add(md3);
            return md.ToArray();
        }

        public Data.Dynamic.PropertyMetadata[] GetProperties()
        {           
            List<PropertyMetadata> md = new List<PropertyMetadata>();
            PropertyMetadata md0 = new PropertyMetadata();
            md0.AliasName = "序号";
            md0.ColumnName = "ID";
            md0.ColumnType = eDataType.Double;

            PropertyMetadata md1 = new PropertyMetadata();
            md1.AliasName = "X坐标";
            md1.ColumnName = "XCoordinate";
            md1.ColumnType = eDataType.Double;

            PropertyMetadata md2 = new PropertyMetadata();
            md2.AliasName = "Y坐标";
            md2.ColumnName = "YCoordinate";
            md2.ColumnType = eDataType.Double;

            PropertyMetadata md3 = new PropertyMetadata();
            md3.AliasName = "距离";
            md3.ColumnName = "Distance";
            md3.ColumnType = eDataType.Double;
            md.Add(md0);
            md.Add(md1);
            md.Add(md2);
            md.Add(md3);
            return md.ToArray();
          
        }

        /// <summary>
        /// 根据坐标获取界面显示实体，获取的界面实体将被用于显示,需要制定坐标组组别
        /// </summary>        
        private List<GeoPointUI> getGeoPointUIList(Coordinate[] coordinates,int[] id,double[] distence)
        {
          
            List<GeoPointUI> geos = new List<GeoPointUI>();
            if (resourcePage > geoCoordinate.Length) return null;

            for (int m = 0; m < coordinates.Length; m++)
            {
                var geop = new GeoPointUI();
                geop.XCoordinate = coordinates[m].X;
                geop.YCoordinate = coordinates[m].Y;
                geop.ID = id[m];
                geop.Distance = distence[m];
                
                geos.Add(geop);
           
            }

            return geos;
        }
        #endregion

    }
}
