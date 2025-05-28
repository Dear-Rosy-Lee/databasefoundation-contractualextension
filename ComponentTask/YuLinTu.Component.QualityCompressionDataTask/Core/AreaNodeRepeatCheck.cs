using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Spatial;

namespace YuLinTu.Component.QualityCompressionDataTask
{
    /// <summary>
    /// 相邻面要素节点重复
    /// </summary>
    public class AreaNodeRepeatCheck : AreaNodeRepeatPointCheckBase
    {
        public Action<string> ReportErrorMethod { get; set; }

        private void ReportError(string msg)
        {
            if (ReportErrorMethod != null)
            {
                this.ReportErrorMethod(msg);
            }
        }

        private double _tolerance;
        private List<CheckGeometry> geolist;

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
            Grid idx = new Grid(Cidx, tolerance);
            idx.DelegetShpExtent += (index, extent) =>
            {
                var cg = geolist.Find(t => t.Index == index);
                if (cg == null || cg.Graphic == null)
                    return false;
                var env = cg.Graphic.Envelope();
                extent.SetElements(env.MinX(), env.MinY(), env.MaxX(), env.MaxY());
                return true;
            };
            BuildIndex(idx);
            nProgressCount = idx.CalcIndexCount();// _shpFile->GetRecordCount();
            Clear();

            var result = new List<MyResult>();
            int pnodeindx = 0;
            foreach (var pNode in idx.Nodes)
            {
                pnodeindx++;
                //this.ReportError($"开始检查相邻要素节点重复---开始检查节点{pnodeindx}");
                List<MyPoint> vec = new List<MyPoint>();
                foreach (int shpID in pNode.shapes)
                {
                    foreach (var coordinate in geolist[shpID].Graphic.ToCoordinates())
                    {
                        vec.Add(new MyPoint() { x = coordinate.X, y = coordinate.Y, shpID = shpID });
                    }
                }
                vec.Sort((a, b) => { return a.x < b.x ? 1 : -1; });
                //this.ReportError("开始检查相邻要素节点重复---开始检查");
                var recollection = CheckRepeatPoint(vec, overlapTolerance, _tolerance, reportProgress);
                //this.ReportError($"开始检查相邻要素节点重复---返回到报告中{recollection.Count}个");
                foreach (var r in recollection)
                {
                    result.Add(r);
                }
                //this.ReportError("开始检查相邻要素节点重复---加入报告列表完成");
            }
            //this.ReportError("开始检查相邻要素节点重复---检查完成开始报告");
            foreach (var it in result)
            {
                var p = it.p1;
                var pi = it.p2;
                var distanc = Math.Sqrt(Math.Pow((p.x - pi.x), 2) + Math.Pow((p.y - pi.y), 2));
                reportError(p.shpID, pi.shpID, p.x, p.y, pi.x, pi.y, distanc);
            }
        }
        /// <summary>
        /// 检查重复点
        /// </summary>
        public HashSet<MyResult> CheckRepeatPoint(List<MyPoint> vec, double overlapTolerance, double tolerance, Action<int> reportProgress)
        {
            var tolerance2 = tolerance * tolerance;
            var overlapTolerance2 = overlapTolerance * overlapTolerance;
            List<MyPoint> vecTmp = new List<MyPoint>();
            HashSet<MyResult> result = new HashSet<MyResult>();
            foreach (var p in vec)
            {
                if (!_processShpIDs.Any(s => s == p.shpID))
                {
                    _processShpIDs.Add(p.shpID);
                    TopCheckUtil.ReportProgress(reportProgress, nProgressCount, ++nProgress, ref nOldProgress);
                }
                int removeindex = -1;
                for (int i = vecTmp.Count() - 1; i >= 0; --i)
                {
                    var pi = vecTmp[i];
                    var dx = p.x - pi.x;
                    if (dx > tolerance)
                    {
                        removeindex = i;
                        //vecTmp.RemoveRange(0, i);
                        break;
                    }
                    if (p.shpID == pi.shpID)
                        continue;
                    double dy = Math.Abs(p.y - pi.y);
                    if (dy > tolerance)
                    {
                        continue;
                    }
                    var d2 = dx * dx + dy * dy;
                    if (d2 <= tolerance2 && d2 > overlapTolerance2)
                    {
                        MyResult mr = new MyResult(p, pi);
                        if (!result.Any(r => r.p1.IsEqual(mr.p1) && r.p2.IsEqual(mr.p2)))
                        {
                            result.Add(mr);
                        }
                    }
                }
                if (removeindex > 0)
                {
                    vecTmp.RemoveRange(0, removeindex);
                }
                vecTmp.Add(p);
            }
            return result;
        }

        public bool Cidx(int rowID, CExtent ext)
        {
            return GetExtent(rowID, ext);
        }//, tolerance);

        /// <summary>
        /// 建立索引
        /// </summary>
        public void BuildIndex(Grid idx)
        {
            Grid g = idx;
            var fullExt = geolist[0].Graphic.Envelope();
            CExtent cExtent = new CExtent();
            cExtent.SetElements(fullExt.MinX(), fullExt.MinY(), fullExt.MaxX(), fullExt.MaxY());
            geolist.ForEach(f =>
            {
                var env = f.Graphic.Envelope();
                cExtent.UpElements(env.MinX(), env.MinY(), env.MaxX(), env.MaxY()); //= fullExt.Union(f.Graphic.Envelope())                
            });
            //CExtent cExtent = new CExtent();
            //cExtent.SetElements(fullExt.MinX(), fullExt.MinY(), fullExt.MaxX(), fullExt.MaxY());
            //var fullExt = () _shpFile.GetFullExtent();
            g.SetFullExtent(cExtent);
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
        }

        public class MyPoint
        {
            public double x, y;

            public int shpID;

            public bool IsEqual(MyPoint rhs)
            {
                return shpID == rhs.shpID && x == rhs.x && y == rhs.y;
            }
            /*
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

            */
        };

        public class MyResult
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
            //public static bool operator <(MyResult left, MyResult right)
            //{
            //    if (left.p1 < right.p1)
            //        return true;
            //    if (right.p1 < left.p1)
            //        return false;
            //    return left.p2 < right.p2;
            //}

            //public static bool operator >(MyResult left, MyResult right)
            //{
            //    if (left.p1 > right.p1)
            //        return true;
            //    if (right.p1 > left.p1)
            //        return false;
            //    return left.p2 > right.p2;
            //}
        };

        public int nProgressCount = 0, nProgress = 0;
        public int nOldProgress = 0;
        public HashSet<int> _processShpIDs;


        public void Clear()
        {
            _processShpIDs.Clear();
        }
    }

    public class CExtent
    {
        public double xmin, ymin, xmax, ymax;
        public List<int> shapes;
        public CExtent()
        {
            shapes = new List<int>();
        }

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

        public void UpElements(double xmin, double ymin, double xmax, double ymax)
        {
            this.xmin = this.xmin < xmin ? this.xmin : xmin;
            this.xmax = this.xmax > xmax ? this.xmax : xmax;
            this.ymin = this.ymin < ymin ? this.ymin : ymin;
            this.ymax = this.ymax > ymax ? this.ymax : ymax;
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
        public int nodeDepth;

        public Node(int depth)
        {
            this.nodeDepth = depth;
            shapes = new List<int>();
        }

        public Node(CExtent ext)
        {
            nodeDepth = 0;
            this.xmin = ext.xmin;
            this.xmax = ext.xmax;
            this.ymin = ext.ymin;
            this.ymax = ext.ymax;
            shapes = new List<int>();
        }
    }

    public class CheckGeometry
    {
        public int Index { get; set; }

        public string Bm { get; set; }

        public Geometry Graphic { get; set; }
    }

    /// <summary>
    /// 网格
    /// </summary>
    public class Grid
    {
        private const int MAX_DEPTH = 8;
        private const int MAX_NODES = 40000;
        private CExtent fullExtent;
        private HashSet<int> atMultiNodeShps;
        private double _tolerance;
        private bool _fUseOneNode;//不构建索引，所有ID都加入到一个根节点
        private Dictionary<int, CExtent> _cacheExtent;

        public List<Node> Nodes;
        public Func<int, CExtent, bool> DelegetShpExtent;

        public Grid(Func<int, CExtent, bool> onGetShpExtent, double tolerance = 0.0)
        {
            fullExtent = new CExtent();
            atMultiNodeShps = new HashSet<int>();
            Nodes = new List<Node>();
            _cacheExtent = new Dictionary<int, CExtent>();
            this._tolerance = tolerance;
        }

        public void SetTolerance(double tolerance)
        {
            _tolerance = tolerance;
        }

        public void SetFullExtent(double xmin, double ymin, double xmax, double ymax, int nRecordCount = 0)
        {
            Clear();
            fullExtent.SetElements(xmin, ymin, xmax, ymax);
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
                        var p = new Node(0)
                        {
                            xmin = x1,
                            xmax = c == n - 1 ? xmax : x2,
                            ymin = y1,
                            ymax = r == n - 1 ? ymax : y2
                        };
                        Nodes.Add(p);
                        x = x2;
                    }
                    y = y2;
                }
            }
            else
            {
                Node p = new Node(fullExtent);
                Nodes.Add(p);
            }
        }

        public void SetFullExtent(CExtent ext, int nRecordCount = 0)
        {
            SetFullExtent(ext.xmin, ext.ymin, ext.xmax, ext.ymax, nRecordCount);
        }

        public CExtent GetFullExtent()
        {
            return fullExtent;
        }

        public void Clear()
        {
            Console.WriteLine("开始检查相邻要素节点重复---清空数据");
            if (Nodes.Count == 0)
                return;
            atMultiNodeShps.Clear();
            Nodes.Clear();
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
                if (Nodes.Count > 1)
                {
                    var p = Nodes[0];
                    for (int i = Nodes.Count - 1; i >= 1; --i)
                    {
                        var pn = Nodes[i];
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
                        Nodes.RemoveAt(Nodes.Count - 1);
                    }
                }
            }
        }

        public bool AddShape(int shpID)
        {//, double xmin, double ymin, double xmax, double ymax){
            if (_fUseOneNode)
            {
                Nodes[0].shapes.Add(shpID);
                return true;
            }
            CExtent ext = new CExtent();
            if (!GetShpExtent(shpID, ext))
            {
                return false;
            }
            if (_tolerance > 0)
            {
                ext.Expand(_tolerance, _tolerance);
            }
            int n = 0;
            foreach (var p in Nodes)
            {
                n += NodeAdd(p, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
            }
            if (n > 1)
            {
                //pShp->fInMultiNode = TRUE;//
                atMultiNodeShps.Add(shpID);
            }
            else if (n == 0)
            {
                //_ASSERT(FALSE);
            }
            for (int i = Nodes.Count - 1; i >= 0; --i)
            {
                var pNode = Nodes[i];
                if (pNode.nodeDepth < MAX_DEPTH && pNode.shapes.Count > MAX_NODES)
                {
                    pNode.nodeDepth = pNode.nodeDepth++;
                    var p1 = new Node(pNode.nodeDepth);
                    var p2 = new Node(pNode.nodeDepth);
                    var p3 = new Node(pNode.nodeDepth);
                    NodeSplit(pNode, p1, p2, p3);
                    Nodes.Add(p1);
                    Nodes.Add(p2);
                    Nodes.Add(p3);
                }
            }
            //++_shpCount;
            return true;
        }
        //void AddShape(int shpID, const CExtent& ext){
        //	AddShape(shpID), ext.xmin, ext.ymin, ext.xmax, ext.ymax);
        //}
        bool IsShpInMultiNode(int shpID)
        {
            return atMultiNodeShps.Any(t => t == shpID);// != atMultiNodeShps.end();
        }

        //使用后不释放返回值
        //const CExtent* GetShpExtent(int shpID){
        //	return DelegetShpExtent(shpID);
        //}
        public bool GetShpExtent(int shpID, CExtent e)
        {
            // var it = _cacheExtent.ContainsKey(shpID);
            if (_cacheExtent.ContainsKey(shpID))
            {
                e = _cacheExtent[shpID];
                return true;
            }
            bool fOK = DelegetShpExtent(shpID, e);
            if (_cacheExtent.Count >= 500000)
            {
                _cacheExtent.Clear();
            }
            _cacheExtent[shpID] = e;
            return fOK;
        }

        //计算_nodes包含的所有shape对象的个数
        public int CalcIndexCount()
        {
            int n = 0;
            foreach (var nd in Nodes)
            {
                n += nd.shapes.Count;
            }
            return n;// _datas.size();
        }

        int AtMultiNodeShpCount()
        {
            return atMultiNodeShps.Count;
        }

        private void NodeSplit(Node pNode, Node pp1, Node pp2, Node pp3)
        {
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
                if (!GetShpExtent(shpID, ext))
                {
                    continue;
                }
                int n = NodeAdd(pNode, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
                n += NodeAdd(pp1, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
                n += NodeAdd(pp2, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
                n += NodeAdd(pp3, shpID, ext.xmin, ext.ymin, ext.xmax, ext.ymax);
                if (n > 1)
                {
                    atMultiNodeShps.Add(shpID);
                }
            }
        }

        public int NodeAdd(Node pNode, int shpID, double xmin, double ymin, double xmax, double ymax)
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
        public static void ReportProgress(Action<int> callback, int count, int i, ref int nOldProgress)
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
        public static void ReportProgress(string msg, Action<int, string> callback, int count, int i, ref int nOldProgress)
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