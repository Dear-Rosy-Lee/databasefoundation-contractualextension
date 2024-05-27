using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.NetAux;

namespace YuLinTu.Library.BuildJzdx
{
    public class TGISCHelper
    {
        private delegate void Callback1_o(object g);
        [DllImport("tGISC.dll", EntryPoint = "TGIS_Geometry_Simplify", CallingConvention = CallingConvention.Cdecl)]
        private static extern void TGIS_Geometry_Simplify(byte[] g, Callback1_o callback);

        /// <summary>
        /// 类似ArcObjects的Simplify方法
        /// </summary>
        /// <param name="g"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        public static IGeometry Simplify(IGeometry g)//, int srid = 2380)
        {
            IGeometry r = null;
            TGIS_Geometry_Simplify(g.AsBinary(), o =>
            {
                r =WKBHelper.fromWKB((byte[])o);
            });
            return r;
        }
    }
}
