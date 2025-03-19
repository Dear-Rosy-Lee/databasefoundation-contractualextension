using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Spatial;

namespace YuLinTu.Component.VectorDataTreatTask
{
    /// <summary>
    /// 相邻面要素节点重复
    /// </summary>
    public class AreaNodeRepeatCheck : AreaNodeRepeatPointCheckBase
    {
        private double _tolerance;
        private List<CheckGeometry> geolist;
        private int nProgressCount;

        /// <summary>
        /// 检查数据
        /// </summary>
        public void DoCheck(List<CheckGeometry> geolist, double overlapTolerance, double tolerance,
              Action<int, int, double, double, double, double, double> reportError,
              Action<int> reportProgress)
        {
            this.geolist = geolist;
            _tolerance = tolerance;
            Clear();
            //GridIndex::Grid idx([&](int rowID, CExtent & ext)->bool{ return GetExtent(rowID, ext); }, tolerance);
            Grid idx = new Grid(cidx);
            buildIndex(idx);

            nProgressCount = idx.calcIndexCount();// _shpFile->GetRecordCount();
            int nOldProgress = 0;
            int nProgress = 0;
            _processShpIDs.Clear();

            foreach (var pNode in idx._nodes)
            {
                List<MyPoint> vec = new List<MyPoint>();
                foreach (int shpID in pNode.shapes)
                {
                    foreach (var coordinate in geolist[shpID].Graphic.ToCoordinates())
                    {
                        vec.Add(new MyPoint() { x = coordinate.X, y = coordinate.Y, shpID = shpID });
                    }
                    //var p = _shpFile.ReadShapeObjEx(shpID);
                    //if (p != nullptr)
                    //{
                    //    conv(*p, vec);
                    //}
                }
                base.DoCheck(vec, overlapTolerance, _tolerance, reportError, reportProgress);
            }

            foreach (var it in _result)
            {
                var p = it.p1;
                var pi = it.p2;
                reportError(p.shpID, pi.shpID, p.x, p.y, pi.x, pi.y, 0);
            }
        }

        //public void conv(CheckGeometry o, List<MyPoint> vec)
        //{
        //    for (var i = 0; i < o.nVertices - 1; ++i)
        //    {
        //        int k = 0;
        //        if (k < o.nParts)
        //        {
        //            auto n = o.vecPartStart[k];
        //            if (i == n)
        //                ++k;
        //            else if (i == n - 1)
        //            {
        //                continue;
        //            }
        //        }

        //        MyPoint pt;

        //        pt.x = o.vecfX[i];
        //        pt.y = o.vecfY[i];
        //        pt.shpID = o.nShapeId;
        //        vec.push_back(pt);
        //    }
        //}

        bool cidx(int rowID, CExtent ext)
        { return GetExtent(rowID, ext); }//, tolerance);

        /// <summary>
        /// 建立索引
        /// </summary>
        public void buildIndex(Grid idx)
        {
            Grid g = idx;
            var fullExt = geolist[0].Graphic.Envelope();
            geolist.ForEach(f => fullExt.Union(f.Graphic.Envelope()));
            CExtent cExtent = new CExtent();
            cExtent.SetElements(fullExt.MinX(), fullExt.MinY(), fullExt.MaxX(), fullExt.MaxY());
            //var fullExt = () _shpFile.GetFullExtent();
            g.setFullExtent(cExtent);
            var nRecords = geolist.Count();
            for (int i = 0; i < nRecords; ++i)
            {
                g.AddShape(i);
            }
        }

        /// <summary>
        /// 计算范围
        /// </summary>
        /// <param name="row"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        bool GetExtent(int row, CExtent ext)
        {
            try
            {
                var envelope = geolist[row].Graphic.Envelope();

                //if (!_shpFile.GetExtent(row, ext.xmin, ext.ymin, ext.xmax, ext.ymax))
                //    return false;
                ext.Expand(_tolerance, _tolerance);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }


    /// <summary>
    /// 面节点重叠检查基础
    /// </summary>
    public class AreaNodeRepeatPointCheckBase
    {
        public AreaNodeRepeatPointCheckBase()
        {
            _processShpIDs = new HashSet<int>();
            _result = new HashSet<MyResult>();
        }

        public struct MyPoint
        {
            public double x, y;

            public int shpID;

            public bool IsEqual(MyPoint rhs)
            {
                return shpID == rhs.shpID && x == rhs.x && y == rhs.y;
            }

            // 重载小于运算符 <
            public static bool operator <(MyPoint left, MyPoint right)
            {
                if (left.shpID < right.shpID)
                    return true;
                if (left.shpID > right.shpID)
                    return false;
                if (left.x < right.x)
                    return true;
                if (right.x < left.x)
                    return false;
                if (left.y < right.y)
                    return true;
                return right.y < left.y;
            }

            // 重载大于运算符 >
            public static bool operator >(MyPoint left, MyPoint right)
            {
                if (left.shpID > right.shpID)
                    return true;
                if (left.shpID < right.shpID)
                    return false;
                if (left.x > right.x)
                    return true;
                if (right.x > left.x)
                    return false;
                if (left.y > right.y)
                    return true;
                return right.y > left.y;
            }
        };

        public struct MyResult
        {
            public MyPoint p1;
            public MyPoint p2;
            public MyResult(MyPoint p1_, MyPoint p2_)
            {
                if (p1_.shpID < p2_.shpID)
                {
                    p1 = p1_;
                    p2 = p2_;
                }
                else
                {
                    p1 = p2_;
                    p2 = p1_;
                }
            }
            public static bool operator <(MyResult left, MyResult right)
            {
                if (left.p1 < right.p1)
                    return true;
                if (right.p1 < left.p1)
                    return false;
                return left.p2 < right.p2;
            }

            public static bool operator >(MyResult left, MyResult right)
            {
                if (left.p1 > right.p1)
                    return true;
                if (right.p1 > left.p1)
                    return false;
                return left.p2 > right.p2;
            }
        };

        public int nProgressCount = 0, nProgress = 0;
        public int nOldProgress = 0;
        public HashSet<int> _processShpIDs;
        public HashSet<MyResult> _result;


        public void Clear()
        {
            _processShpIDs.Clear();
            _result.Clear();
        }


        public void DoCheck(List<MyPoint> vec, double overlapTolerance, double tolerance, Action<int, int, double, double, double, double, double> reportError,
              Action<int> reportProgress)
        {
            var tolerance2 = tolerance * tolerance;
            var overlapTolerance2 = overlapTolerance * overlapTolerance;
            vec.Sort((a, b) => { return a.x < b.x ? 1 : -1; });
            List<MyPoint> vecTmp = new List<MyPoint>();
            foreach (var p in vec)
            {
                if (_processShpIDs.Any(s => s == p.shpID))
                {
                    _processShpIDs.Add(p.shpID);
                    TopCheckUtil.reportProgress(reportProgress, nProgressCount, ++nProgress, ref nOldProgress);
                }
                for (int i = vecTmp.Count() - 1; i >= 0; --i)
                {
                    var pi = vecTmp[i];

                    var dx = p.x - pi.x;
                    if (dx > tolerance)
                    {
                        //for (int j = 0; j <= i; ++j){
                        //	auto pt=vecTmp[j];
                        //	auto it = mapError.find(pt);
                        //	if (it != mapError.end()){
                        //		for (auto pit : it->second){
                        //			reportError(pt->shpID, pit->shpID, pt->x, pt->y, pit->x, pit->y, 0);
                        //		}
                        //		mapError.erase(it);
                        //	}
                        //}
                        vecTmp.RemoveRange(0, i);// (vecTmp.begin(), vecTmp.begin() + i);
                        //vecTmp.erase(vecTmp.begin(), vecTmp.begin() + i);
                        break;
                    }
                    if (p.shpID == pi.shpID)
                        continue;
                    double dy = Math.Abs(p.y - pi.y);
                    if (dy <= tolerance)
                    {
                        var d2 = dx * dx + dy * dy;
                        if (d2 <= tolerance2 && d2 > overlapTolerance2)
                        {
                            MyResult mr = new MyResult(p, pi);
                            if (_result.Any(r => r.p1.IsEqual(mr.p1) && r.p2.IsEqual(mr.p2)))
                            {
                                //reportError(p.shpID, pi.shpID, p.x, p.y, pi.x, pi.y, d2);
                                _result.Add(mr);
                            }
                            //mapError[&p].push_back(&pi);
                            //mapError[&pi].push_back(&p);
                        }
                    }
                }
                vecTmp.Add(p);
            }
        }

    }

    public class CExtent
    {
        public double xmin, ymin, xmax, ymax;
        List<int> shapes;
        /// <summary>
        /// 处理容差
        /// </summary>
        public void Expand(double dx, double dy)
        {
            xmin -= dx;
            xmax += dx;
            ymin -= dy;
            ymax += dy;
        }

        public void SetElements(double xmin, double ymin, double xmax, double ymax)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
        }

        public Point GetCenterPoint()
        {
            Point ptCenter = new Point() { x = xmin + Width() / 2, y = ymin + Height() / 2 };
            return ptCenter;
        }

        double Width() { return xmax - xmin; }
        double Height() { return ymax - ymin; }

        public bool IsDisjunction(double xmin, double ymin, double xmax, double ymax)
        {
            if (this.xmin > xmax
                || this.xmax < xmin
                || this.ymin > ymax
                || this.ymax < ymin)
                return true;
            return false;
        }
    }


    public struct Point
    {
        public double x, y;
    };

    public class Node : CExtent
    {
        public short nodeDepth;
        public List<int> shapes;

        public Node(int depth)
        {
        }

        public Node(CExtent ext)
        {
            nodeDepth = 0;
        }
    }

    public class CheckGeometry
    {
        public int index { get; set; }

        public Geometry Graphic { get; set; }
    }

    /// <summary>
    /// 网格
    /// </summary>
    public class Grid
    {
        const int MAX_DEPTH = 8;
        const int MAX_NODES = 40000;
        private CExtent _fullExtent;
        HashSet<int> _atMultiNodeShps;

        //const Grid operator=(Grid rhs);

        //public Grid(Grid rhs);

        Func<int, CExtent, bool> _getShpExtent;
        double _tolerance;

        bool _fUseOneNode;//不构建索引，所有ID都加入到一个根节点
        Dictionary<int, CExtent> _cacheExtent;
        public List<Node> _nodes;
        public Grid(Func<int, CExtent, bool> onGetShpExtent, double tolerance = 0.0)
        {
        }
        ~Grid()
        {
            Clear();
        }
        public void SetTolerance(double tolerance)
        {
            _tolerance = tolerance;
        }
        public void setFullExtent(double xmin, double ymin, double xmax, double ymax, int nRecordCount = 0)
        {
            Clear();
            _fullExtent.SetElements(xmin, ymin, xmax, ymax);
            if (nRecordCount > 10000000)
            {
                int n = 32;
                double dx = (xmax - xmin) / n;
                double dy = (ymax - ymin) / n;
                double y = ymin;
                for (int r = 0; r < n; ++r)
                {
                    double y1 = y, y2 = y1 + dy;
                    double x = xmin;
                    for (int c = 0; c < n; ++c)
                    {
                        double x1 = x, x2 = x1 + dx;
                        var p = new Node(0);
                        p.xmin = x1; p.xmax = c == n - 1 ? xmax : x2;
                        p.ymin = y1; p.ymax = r == n - 1 ? ymax : y2;
                        _nodes.Add(p);
                        x = x2;
                    }
                    y = y2;
                }
            }
            else
            {
                Node p = new Node(_fullExtent);
                _nodes.Add(p);
            }
        }
        public void setFullExtent(CExtent ext, int nRecordCount = 0)
        {

            setFullExtent(ext.xmin, ext.ymin, ext.xmax, ext.ymax, nRecordCount);
        }
        public CExtent getFullExtent()
        {
            return _fullExtent;
        }
        public void Clear()
        {
            if (_nodes.Count == 0)
                return;
            _atMultiNodeShps.Clear();
            _nodes.Clear();
        }

        void ClearCache()
        {
            _cacheExtent.Clear();
        }

        //不构建索引，所有ID都加入到一个根节点
        public void SetUseOneNode(bool fUseOneNode)
        {
            _fUseOneNode = fUseOneNode;
            if (_fUseOneNode)
            {
                if (_nodes.Count > 1)
                {
                    var p = _nodes[0];
                    for (int i = _nodes.Count - 1; i >= 1; --i)
                    {
                        var pn = _nodes[i];
                        foreach (var id in pn.shapes)
                        {
                            bool fAdd = true;
                            foreach (var id0 in pn.shapes)
                            {
                                if (id0 == id)
                                {
                                    fAdd = false;
                                    break;
                                }
                            }
                            if (fAdd)
                            {
                                p.shapes.Add(id);
                            }
                        }
                        _nodes.RemoveAt(_nodes.Count - 1);
                    }
                }
            }
        }
        public bool AddShape(int shpID)
        {//, double xmin, double ymin, double xmax, double ymax){
            if (_fUseOneNode)
            {
                _nodes[0].shapes.Add(shpID);
                return true;
            }
            CExtent ext = new CExtent();
            if (!getShpExtent(shpID, ext))
            {
                return false;
            }
            if (_tolerance > 0)
            {
                ext.Expand(_tolerance, _tolerance);
            }
            int n = 0;
            foreach (var p in _nodes)
            {
                n += nodeAdd(p, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
            }
            if (n > 1)
            {
                //pShp->fInMultiNode = TRUE;//
                _atMultiNodeShps.Add(shpID);
            }
            else if (n == 0)
            {
                //_ASSERT(FALSE);
            }
            for (int i = _nodes.Count - 1; i >= 0; --i)
            {
                var pNode = _nodes[i];
                if (pNode.nodeDepth < MAX_DEPTH && pNode.shapes.Count > MAX_NODES)
                {
                    Node p1 = new Node(0);
                    Node p2 = new Node(0);
                    Node p3 = new Node(0);
                    nodeSplit(pNode, p1, p2, p3);
                    _nodes.Add(p1);
                    _nodes.Add(p2);
                    _nodes.Add(p3);
                }
            }
            //++_shpCount;
            return true;
        }
        //void AddShape(int shpID, const CExtent& ext){
        //	AddShape(shpID), ext.xmin, ext.ymin, ext.xmax, ext.ymax);
        //}
        bool isShpInMultiNode(int shpID)
        {
            return _atMultiNodeShps.Any(t => t == shpID);// != _atMultiNodeShps.end();
        }

        //使用后不释放返回值
        //const CExtent* getShpExtent(int shpID){
        //	return _getShpExtent(shpID);
        //}
        public bool getShpExtent(int shpID, CExtent e)
        {
            // var it = _cacheExtent.ContainsKey(shpID);
            if (_cacheExtent.ContainsKey(shpID))
            {
                e = _cacheExtent[shpID];
                return true;
            }
            bool fOK = _getShpExtent(shpID, e);
            if (_cacheExtent.Count >= 500000)
            {
                _cacheExtent.Clear();
            }
            _cacheExtent[shpID] = e;
            return fOK;
        }

        //计算_nodes包含的所有shape对象的个数
        public int calcIndexCount()
        {
            int n = 0;
            foreach (var nd in _nodes)
            {
                n += nd.shapes.Count;
            }
            return n;// _datas.size();
        }
        int atMultiNodeShpCount()
        {
            return _atMultiNodeShps.Count;
        }

        private void nodeSplit(Node pNode, Node pp1, Node pp2, Node pp3)
        {
            pNode.nodeDepth = pNode.nodeDepth++;
            pp1 = new Node(pNode.nodeDepth);
            pp2 = new Node(pNode.nodeDepth);
            pp3 = new Node(pNode.nodeDepth);
            CExtent i = pNode;
            var p = pNode.GetCenterPoint();
            pp1.SetElements(p.x, i.ymin, i.xmax, p.y);
            pp2.SetElements(p.x, p.y, i.xmax, i.ymax);
            pp3.SetElements(i.xmin, p.y, p.x, i.ymax);
            pNode.SetElements(i.xmin, i.ymin, p.x, p.y);

            List<int> shapes0 = new List<int>();
            foreach (var item in pNode.shapes)
            {
                shapes0.Add(item);
            }
            pNode.shapes = new List<int>();
            //pNode.shapes.swap(shapes0);
            CExtent ext = new CExtent();
            foreach (var shpID in shapes0)
            {
                if (!getShpExtent(shpID, ext))
                {
                    continue;
                }
                int n = nodeAdd(pNode, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
                n += nodeAdd(pp1, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
                n += nodeAdd(pp2, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
                n += nodeAdd(pp3, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
                if (n > 1)
                {
                    _atMultiNodeShps.Add(shpID);
                }
            }
        }
        public int nodeAdd(Node pNode, int shpID, double xmin, double ymin, double xmax, double ymax)
        {
            if (!pNode.IsDisjunction(xmin, ymin, xmax, ymax))
            {
                pNode.shapes.Add(shpID);
                return 1;
            }
            return 0;
        }
    };


    public static class TopCheckUtil
    {
        public static void reportProgress(Action<int> callback, int count, int i, ref int nOldProgress)
        {
            if (callback != null && count > 0)
            {
                int nProgress = (int)(i * 100.0 / count);
                if (nOldProgress != nProgress)
                {
                    callback(nProgress);
                    nOldProgress = nProgress;
                }
            }
        }
        public static void reportProgress(string msg, Action<int, string> callback, int count, int i, ref int nOldProgress)
        {
            if (callback != null && count > 0)
            {
                int nProgress = i * 100 / count;
                if (nOldProgress != nProgress)
                {
                    callback(nProgress, msg);
                    nOldProgress = nProgress;
                }
            }
        }
    };
}