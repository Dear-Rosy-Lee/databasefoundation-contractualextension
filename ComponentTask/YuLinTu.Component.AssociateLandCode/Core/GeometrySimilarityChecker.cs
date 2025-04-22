using System;
using System.Linq;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using System.Collections.Generic;
using System.Windows;

public class GeometrySimilarityChecker
{
    // 综合相似性检查（返回相似度得分，0~1）
    public double CheckSimilarity(Geometry geom1, Geometry geom2)
    {
        // 1. 归一化处理（平移至原点，缩放至相同大小）
        var geotuple = NormalizeGeometries(geom1, geom2);

        // 2. 计算豪斯多夫距离（形状相似性）
        double hausdorffDistance = CalculateHausdorffDistance(geotuple.Item1, geotuple.Item2);

        // 3. 计算面积和周长比
        double areaRatio = CalculateAreaRatio(geotuple.Item1, geotuple.Item2);
        double perimeterRatio = CalculatePerimeterRatio(geotuple.Item1, geotuple.Item2);

        // 4. 方向相似性（基于最小外接矩形方向角）
        double directionDiff = CalculateDirectionDifference(geotuple.Item1, geotuple.Item2);

        // 5. 顶点分布相似性（采样点匹配）
        double vertexSimilarity = CalculateVertexSimilarity(geotuple.Item1, geotuple.Item2);

        // 综合评分（权重可调）
        double score = 0.7 * (1 - hausdorffDistance) +
                      0.1 * (areaRatio + perimeterRatio) / 2 +
                      0.1 * (1 - directionDiff / 180) +
                      0.1 * vertexSimilarity;

        return Math.Max(0, Math.Min(1, score));
    }

    //-----------------------------------------
    // 归一化几何图形（平移至原点，缩放至单位大小）
    //-----------------------------------------
    private Tuple<Geometry, Geometry> NormalizeGeometries(Geometry geom1, Geometry geom2)
    {
        // 计算包围盒并平移至原点
        var envelope1 = geom1.EnvelopeInternal;
        var envelope2 = geom2.EnvelopeInternal;

        var translated1 = geom1.Clone() as Geometry;
        translated1.Apply(new TranslationFilter(-envelope1.MinX, -envelope1.MinY));

        var translated2 = geom2.Clone() as Geometry;
        translated2.Apply(new TranslationFilter(-envelope2.MinX, -envelope2.MinY));

        // 缩放至相同大小（基于最大边长）
        double scale1 = Math.Max(envelope1.Width, envelope1.Height);
        double scale2 = Math.Max(envelope2.Width, envelope2.Height);
        double maxScale = Math.Max(scale1, scale2);

        translated1.Apply(new ScaleFilter(1 / maxScale, 1 / maxScale));
        translated2.Apply(new ScaleFilter(1 / maxScale, 1 / maxScale));

        return new Tuple<Geometry, Geometry>(translated1, translated2);
    }

    //-----------------------------------------
    // 计算豪斯多夫距离（衡量形状相似性）
    //-----------------------------------------
    private double CalculateHausdorffDistance(Geometry geom1, Geometry geom2)
    {
        var densifyFrac = 0.25; // 采样密度
        var hausdorffDistance = new NetTopologySuite.Algorithm.Distance.DiscreteHausdorffDistance(geom1, geom2);
        hausdorffDistance.DensifyFraction = densifyFrac;
        return hausdorffDistance.Distance();
        //return NetTopologySuite.Algorithm.Distance.HausdorffDistance(geom1, geom2, densifyFrac);
    }

    //-----------------------------------------
    // 计算面积比（归一化后）
    //-----------------------------------------
    private double CalculateAreaRatio(Geometry geom1, Geometry geom2)
    {
        double area1 = geom1.Area;
        double area2 = geom2.Area;
        return (area1 + area2 == 0) ? 1 : 2 * Math.Min(area1, area2) / (area1 + area2);
    }

    //-----------------------------------------
    // 计算周长比
    //-----------------------------------------
    private double CalculatePerimeterRatio(Geometry geom1, Geometry geom2)
    {
        double perimeter1 = geom1.Length;
        double perimeter2 = geom2.Length;
        return (perimeter1 + perimeter2 == 0) ? 1 : 2 * Math.Min(perimeter1, perimeter2) / (perimeter1 + perimeter2);
    }

    //-----------------------------------------
    // 计算方向差异（基于最小外接矩形方向角）
    //-----------------------------------------
    private double CalculateDirectionDifference(Geometry geom1, Geometry geom2)
    {
        var mbr1 = geom1.Envelope as Polygon;
        var mbr2 = geom2.Envelope as Polygon;
        double angle1 = CalculateMBRAngle(mbr1);
        double angle2 = CalculateMBRAngle(mbr2);
        return Math.Abs(angle1 - angle2);
    }

    // 计算最小外接矩形方向角（单位：度）
    private double CalculateMBRAngle(Polygon mbr)
    {
        var coordinates = mbr.Shell.Coordinates;
        var dplist = new List<System.Drawing.PointF>();
        foreach (var coord in coordinates)
        {
            dplist.Add(new System.Drawing.PointF((float)coord.X, (float)coord.Y));
        }
        var rect = GetBoundingRectangleForPolygonAccurate(dplist);
        //var vector = new Coordinate(coordinates.X - coordinates.X, coordinates.Y - coordinates.Y);
        var center = new System.Drawing.PointF((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2);
        return Math.Atan2(center.Y, center.X) * (180 / Math.PI);
    }

    public System.Drawing.RectangleF GetBoundingRectangleForPolygonAccurate(List<System.Drawing.PointF> points)
    {
        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
        {
            path.AddLines(points.ToArray()); // 将点添加到路径中
            path.CloseFigure(); // 闭合路径（如果需要）
            return path.GetBounds(); // 获取边界矩形
        }
    }

    //-----------------------------------------
    // 顶点分布相似性（采样点匹配）
    //-----------------------------------------
    private double CalculateVertexSimilarity(Geometry geom1, Geometry geom2)
    {
        // 采样固定数量点（例如50个）

        var count1 = geom1.Coordinates.Count();
        var count2 = geom1.Coordinates.Count();
        int cayd = count1 > count2 ? count2 : count1;
        if (cayd > 50)
        {
            cayd = 50;
        }
        var points1 = SamplePoints(geom1, cayd);
        var points2 = SamplePoints(geom2, cayd);

        // 计算平均最近邻距离
        double totalDistance = 0;
        foreach (var p1 in points1)
        {
            double minDist = points2.Min(p2 => p1.Distance(p2));
            totalDistance += minDist;
        }
        double avgDistance = totalDistance / points1.Length;

        // 距离越小得分越高（阈值可调）
        return Math.Exp(-avgDistance * 10);
    }

    // 在几何体上均匀采样点
    private Coordinate[] SamplePoints(Geometry geom, int numPoints)
    {
        var cds = geom.Coordinates;
        var points = new Coordinate[numPoints];
        double step = geom.Length / numPoints;
        for (int i = 0; i < numPoints; i++)
        {
            var point = cds[(int)(step * i)];
            points[i] = point;
        }
        return points;
    }
}

//-----------------------------------------
// 辅助类：坐标变换过滤器
//-----------------------------------------
public class TranslationFilter : ICoordinateFilter
{
    private readonly double _dx, _dy;
    public TranslationFilter(double dx, double dy) { _dx = dx; _dy = dy; }
    public void Filter(Coordinate coord) { coord.X += _dx; coord.Y += _dy; }
}

public class ScaleFilter : ICoordinateFilter
{
    private readonly double _sx, _sy;
    public ScaleFilter(double sx, double sy) { _sx = sx; _sy = sy; }
    public void Filter(Coordinate coord) { coord.X *= _sx; coord.Y *= _sy; }
}
