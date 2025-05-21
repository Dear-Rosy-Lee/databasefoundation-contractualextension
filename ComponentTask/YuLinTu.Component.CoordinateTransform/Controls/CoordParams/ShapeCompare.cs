using YuLinTu.Spatial;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 矢量图形比较
    /// 比较方式为 不同位置的两个图形，如果点数一致，面积相差不大，各个点相对位置差不多，认为是一个图形；
    /// </summary>
    static public class ShapeCompare
    {
        /// <summary>
        /// 比较图形
        /// </summary>
        static public bool ShapeCompareByTol(YuLinTu.Spatial.Geometry geo1, YuLinTu.Spatial.Geometry geo2, double tolarence = 0.01)
        {
            if (geo1 == null || geo2 == null)
                return false;
            try
            {
                if (CompareArea(geo1.Area(), geo2.Area(), tolarence))
                    return false;
                var gcodes1 = geo1.ToGroupCoordinates();
                var gcodes2 = geo2.ToGroupCoordinates();
                if (gcodes1.Count != gcodes2.Count)
                    return false;
                for (var i = 0; i < gcodes1.Count; i++)
                {
                    if (!CompareCoordNates(gcodes1[i], gcodes2[i], tolarence))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 点位置比较
        /// </summary>
        /// <returns></returns>
        static private bool CompareCoordNates(Coordinate[] coordinates1, Coordinate[] coordinates2, double tola)
        {
            if (coordinates1.Length != coordinates2.Length)
                return false;
            var standCoord1 = coordinates1[0];
            var standCoord2 = coordinates2[0];
            double disx = standCoord1.X - standCoord2.X;
            double disy = standCoord1.Y - standCoord2.Y;
            for (var i = 1; i < coordinates1.Length; i++)
            {
                var dx = coordinates1[i].X - coordinates2[i].X;
                var dy = coordinates1[i].Y - coordinates2[i].Y;
                if (!CompareArea(disx, dx, tola) || !CompareArea(disy, dy, tola))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// 数值比较
        /// </summary>
        static private bool CompareArea(double area1, double area2, double tolarence)
        {
            if (area1 > area2 + tolarence || area1 < area2 - tolarence) 
                return false;
            return true;
        }
    }
}
