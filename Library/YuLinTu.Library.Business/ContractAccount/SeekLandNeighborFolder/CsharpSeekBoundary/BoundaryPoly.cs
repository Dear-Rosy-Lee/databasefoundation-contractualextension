using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundOrientation
{
    /*多边形的类*/
    public class BoundaryPoly : Poly
    {
        public List<BoundaryEdge> boundary_edge_out;
        public List<BoundaryEdge> boundary_edge_in;
        public List<List<BoundaryEdge>> boundary_array_list;

        public BoundaryPoly(string ID, List<List<Point>> point_array_list, List<List<BoundaryEdge>> boundary_array_list) : base(ID, point_array_list)
        {
            this.boundary_edge_out = new List<BoundaryEdge>();
            this.boundary_edge_in = new List<BoundaryEdge>();
            this.boundary_array_list = boundary_array_list;
            for (int i = 0; i < boundary_array_list.Count; i++)
            {
                foreach (var boundary_array in boundary_array_list[i])
                {
                    var boundary_edge_out_temp = new BoundaryEdge(boundary_array.name, boundary_array.pointList, ID);
                    if (i == 0)
                    {
                        this.boundary_edge_out.Add(boundary_edge_out_temp);
                    }
                    else
                    {
                        this.boundary_edge_in.Add(boundary_edge_out_temp);
                    }
                }
            }
        }
    }
}

