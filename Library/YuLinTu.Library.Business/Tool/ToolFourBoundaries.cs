
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    public class ToolFourBoundaries
    {
        /// <summary>
        /// 坐标
        /// </summary>
        private int Srid;

        /// <summary>
        /// 方位
        /// </summary>
        private readonly string[] Orientation = { "东", "南", "西", "北" };

        /// <summary>
        /// 容差
        /// </summary>
        private readonly double Tolerance = 0.01; 

        public List<BoundaryAddressCoilInfomation> WriteLandDotLineInfo(ContractLand land)
        {
            Srid = land.Shape.Srid;
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
            var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
            List<BuildLandBoundaryAddressCoil> coilList = coilStation.GetByLandId(land.ID);
            List<BuildLandBoundaryAddressDot> dotList = dotStation.GetByLandId(land.ID);
            if (coilList == null || coilList.Count == 0 || dotList == null || dotList.Count == 0)
            {
                return null;
            }
            var centerGravity = getCenterOfGravityPoint(land.Shape.ToCoordinates().ToList());
            var centroid = land.Shape.Centroid();
            var centroid2 = Geometry.CreatePoint(centerGravity, Srid);
            // 获取四角点
            var quadrantDots = CreateQuadrantDots(dotList.Clone(), centroid);

            // 将地块根据四角点划分为4部分
            var fourCoils = CreateFourCoils(dotList.Clone(), quadrantDots.Clone());


            // 四至线集合
            var fourBoundaryCoils = new Dictionary<BuildLandBoundaryAddressCoil, string>();
            foreach (var coil in coilList)
            {
                //var startDot = dotList.Find(dot => dot.ID == coil.StartPointID);
                //var endDot = dotList.Find(dot => dot.ID == coil.EndPointID);

                try
                {
                    bool isIntersects = false;
                    var intersections = new List<BuildLandBoundaryAddressDot>(); // 交点集合
                    var quadrantDotsClone = quadrantDots.Clone();
                    for (int i = 0; i < quadrantDotsClone.Count; i++)
                    {
                        //var intersection = coil.Shape.Contains(quadrantDots[i].Shape); // 交点
                        // 若界址线与四角点相交
                        //if (IsLineIntersectDot(coil.Shape, quadrantDotsClone[i].Shape, Tolerance))
                        if(IsLineIntersectDot(coil, quadrantDotsClone[i], dotList.Clone()))
                        {
                            isIntersects = true;
                            intersections.Add(quadrantDotsClone[i]);
                        }

                    }

                    if (isIntersects)
                    {
                        // 若界址线与四角点相交，方位根据所占长度决定        
                        var partCoils = SplitCoil(BoundaryAddressCoilToDot(dotList.Clone(), coil), intersections);
                        double maxLength = 0.0;
                        string description = string.Empty;
                        foreach (var partCoil in partCoils)
                        {
                            if (partCoil.CoilLength > maxLength)
                            {
                                maxLength = partCoil.CoilLength;
                                description = partCoil.Description;
                            }
                        }
                        fourBoundaryCoils.Add(coil, description);
                    }
                    else
                    {
                        // 若界址线与四角点不相交
                        foreach (var fourCoil in fourCoils)
                        {
                            //if (IsLineIntersectLine(fourCoil.Shape, coil.Shape, Tolerance))
                            if (IsLineIntersectLine(fourCoil, coil, dotList.Clone()))
                            {
                                fourBoundaryCoils.Add(coil, fourCoil.Description);
                            }
                            //if (fourCoil.Shape.Intersects(coil.Shape))
                            //{
                            //fourBoundaryCoils.Add(coil, fourCoil.Description);
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

                
            }

            // 处理四至线集合

            Log.Log.WriteException(land, "", land.CadastralNumber + "-重心1：" + centroid.Instance.ToString());
            Log.Log.WriteException(land, "", land.CadastralNumber + "-重心2：" + centroid2.Instance.ToString());
            foreach (var item in quadrantDots)
            {
                Log.Log.WriteException(item, "", land.CadastralNumber + "-四角点：" + item.DotNumber + "-" + item.Description);
            }
            foreach (var item in fourBoundaryCoils)
            {
                Log.Log.WriteException(item, "", land.CadastralNumber + "-四至线：" + item.Key.StartNumber + "-" + item.Key.EndNumber + "-" + item.Value);
            }



            var result = new List<BoundaryAddressCoilInfomation>(4);
            result = DisposeResult(dotList, fourBoundaryCoils);

            return result;

        }

        private List<BoundaryAddressCoilInfomation> DisposeResult(List<BuildLandBoundaryAddressDot> dotList, Dictionary<BuildLandBoundaryAddressCoil, string> fourBoundaryCoils)
        {
            var result = new List<BoundaryAddressCoilInfomation>(4);

            for (int i = 0; i < 4; i++)
            {
                var addressCoil = fourBoundaryCoils.Where(q => q.Value.Equals(Orientation[i])).Select(q => q.Key);
                var addressCoilInfomation = new BoundaryAddressCoilInfomation();
                if (addressCoil == null || addressCoil.IsNullOrEmpty())
                {
                    result.Add(addressCoilInfomation);
                    continue;
                }
                foreach (var item in addressCoil)
                {
                    // 首尾界址点
                    BuildLandBoundaryAddressDot startDot = dotList.Find(dot => dot.ID == item.StartPointID);
                    BuildLandBoundaryAddressDot endDot = dotList.Find(dot => dot.ID == item.EndPointID);
                    if (addressCoilInfomation.StartAndEndDots.ContainsValue(startDot))
                    {
                        var key = addressCoilInfomation.StartAndEndDots.Where(q => q.Value.Equals(startDot)).Select(q => q.Key).FirstOrDefault();
                        addressCoilInfomation.StartAndEndDots[key] = endDot;
                    }
                    else if (addressCoilInfomation.StartAndEndDots.ContainsKey(endDot))
                    {
                        var value = addressCoilInfomation.StartAndEndDots[endDot];
                        addressCoilInfomation.StartAndEndDots.Remove(endDot);
                        addressCoilInfomation.StartAndEndDots.Add(startDot, value);
                    }
                    else
                    {
                        addressCoilInfomation.StartAndEndDots.Add(startDot, endDot);
                    }
                }

                // 连接界址线
                if (addressCoilInfomation.StartAndEndDots.Count > 1)
                {
                    var tempList = new List<BuildLandBoundaryAddressDot>();
                    foreach (var item in addressCoilInfomation.StartAndEndDots.Values)
                    {
                        if (addressCoilInfomation.StartAndEndDots.ContainsKey(item))
                        {
                            tempList.Add(item);
                        }
                    }
                    foreach (var item in tempList)
                    {
                        var value = addressCoilInfomation.StartAndEndDots[item];
                        var key = addressCoilInfomation.StartAndEndDots.Where(q => q.Value.Equals(item)).Select(q => q.Key).FirstOrDefault();
                        addressCoilInfomation.StartAndEndDots[key] = value;
                        addressCoilInfomation.StartAndEndDots.Remove(item);
                    }
                }

                var startAndEndDot = addressCoilInfomation.StartAndEndDots.FirstOrDefault();
                // 方向
                addressCoilInfomation.Direction = GetLineDirection(startAndEndDot.Key, startAndEndDot.Value);
                // 长度
                addressCoilInfomation.Length = addressCoil.Sum(t => t.CoilLength);
                // 界址线类型
                addressCoilInfomation.CoilType = addressCoil.FirstOrDefault().CoilType;

                // 位置
                addressCoilInfomation.Position = addressCoil.FirstOrDefault().Position;

                foreach (var coil in addressCoil)
                {
                    // 毗邻权利人
                    addressCoilInfomation.NeighborPersons.Add(coil.NeighborPerson);
                    // 毗邻指界人
                    addressCoilInfomation.NeighborFefers.Add(coil.NeighborFefer);
                }


                result.Add(addressCoilInfomation);
            }

            return result;
        }

        public class BoundaryAddressCoilInfomation
        {
            /// <summary>
            /// 首尾界址点
            /// </summary>
            public Dictionary<BuildLandBoundaryAddressDot, BuildLandBoundaryAddressDot> StartAndEndDots { get; set; }

            /// <summary>
            /// 方向
            /// </summary>
            public string Direction { get; set; }

            /// <summary>
            /// 长度
            /// </summary>
            public double Length { get; set; }

            /// <summary>
            /// 截止线类型
            /// </summary>
            public string CoilType { get; set; }

            /// <summary>
            /// 位置（内、中、外）
            /// </summary>
            public string Position { get; set; }

            /// <summary>
            /// 毗邻权利人集合
            /// </summary>
            public HashSet<string> NeighborPersons { get; set; }

            /// <summary>
            /// 毗邻指界人集合
            /// </summary>
            public HashSet<string> NeighborFefers { get; set; }

            public BoundaryAddressCoilInfomation()
            {
                StartAndEndDots = new Dictionary<BuildLandBoundaryAddressDot, BuildLandBoundaryAddressDot>();
                NeighborPersons = new HashSet<string>();
                NeighborFefers = new HashSet<string>();
            }
        }

        /// <summary>
        /// 获取两界址点之间的方向
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="ed"></param>
        /// <returns></returns>
        private string GetLineDirection(BuildLandBoundaryAddressDot sd, BuildLandBoundaryAddressDot ed)
        {
            if (sd == null || ed == null) return string.Empty;
            var edx = (ed.Shape.Instance as GeoAPI.Geometries.IPoint).X;
            var edy = (ed.Shape.Instance as GeoAPI.Geometries.IPoint).Y;
            var sdx = (sd.Shape.Instance as GeoAPI.Geometries.IPoint).X;
            var sdy = (sd.Shape.Instance as GeoAPI.Geometries.IPoint).Y;
            //double xLength = Math.Abs(edx - sdx);
            //double yLength = Math.Abs(edy - sdy);

            double xLength = edx - sdx;
            double yLength = edy - sdy;

            string direction = "";
            if (xLength > 0.0 && yLength > 0.0)
            {
                direction = "东北";
            }
            else if (xLength < 0.0 && yLength < 0.0)
            {
                direction = "西南";
            }
            else if (xLength < 0.0 && yLength > 0.0)
            {
                direction = "西北";
            }
            else if (xLength > 0.0 && yLength < 0.0)
            {
                direction = "东南";
            }
            else if (xLength == 0.0 && yLength > 0.0)
            {
                direction = "正北";
            }
            else if (xLength == 0.0 && yLength < 0.0)
            {
                direction = "正南";
            }
            else if (xLength > 0.0 && yLength == 0.0)
            {
                direction = "正东";
            }
            else if (xLength < 0.0 && yLength == 0.0)
            {
                direction = "正西";
            }
            return direction;
        }


        /// <summary>
        /// 获取不规则多边形重心点 
        /// </summary>
        /// <param name="mPoints"></param>
        /// <returns></returns>
        public static Coordinate getCenterOfGravityPoint(List<Coordinate> mPoints)
        {
            double area = 0.0;//多边形面积  
            double Gx = 0.0, Gy = 0.0;// 重心的x、y  
            for (int i = 1; i <= mPoints.Count; i++)
            {
                double iLat = mPoints[i % mPoints.Count].X;
                double iLng = mPoints[i % mPoints.Count].Y;
                double nextLat = mPoints[i - 1].X;
                double nextLng = mPoints[i - 1].Y;
                double temp = (iLat * nextLng - iLng * nextLat) / 2.0;
                area += temp;
                Gx += temp * (iLat + nextLat) / 3.0;
                Gy += temp * (iLng + nextLng) / 3.0;
            }
            Gx = Gx / area;
            Gy = Gy / area;
            return new Coordinate(Gx, Gy);
        }

        /// <summary>
        /// 根据总界址点拆分界址线获取界址点
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressDot> BoundaryAddressCoilToDot(List<BuildLandBoundaryAddressDot> dotList, BuildLandBoundaryAddressCoil coil)
        {
            var dots = new List<BuildLandBoundaryAddressDot>();
            var firstNumber = Convert.ToInt32(ToolString.GetAllNumberWithInString(coil.StartNumber));
            var endNumber = Convert.ToInt32(ToolString.GetAllNumberWithInString(coil.EndNumber));
            var createdots = new List<BuildLandBoundaryAddressDot>();
            int index = firstNumber - 1;
            for (int i = 0; i <= dotList.Count; i++) // 多循环一次，构成环形
            {
                if (index == endNumber) break;
                index = index % dotList.Count;
                dots.Add(dotList[index]);
                index++;
            }

            return dots;
        }

        /// <summary>
        /// 根据界址点号判断点线是否相交
        /// </summary>
        private bool IsLineIntersectDot(BuildLandBoundaryAddressCoil coil, BuildLandBoundaryAddressDot dot, List<BuildLandBoundaryAddressDot> dotList)
        {
            var coilDots = BoundaryAddressCoilToDot(dotList, coil);
            var count = coilDots.Count(t => t.DotNumber.Equals(dot.DotNumber));
            return count > 0;
        }

        /// <summary>
        /// 点线是否相交
        /// </summary>
        /// <param name="line"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool IsLineIntersectDot(Geometry line, Geometry point, double tolerance)
        {
            if (line == null || point == null) return false;
            bool isIntersect = false;
            var p = point.Instance as GeoAPI.Geometries.IPoint;
            var coordinate = new GeoAPI.Geometries.Coordinate(p.X, p.Y);
            var coordinates = line.ToCoordinates();
            foreach (var item in coordinates)
            {
                if (coordinate.Equals2D(ToCoordinate(item), tolerance))
                {
                    isIntersect = true;
                    break;
                }
            }
            return isIntersect;
        }

        /// <summary>
        /// 根据界址点号判断线线是否相交
        /// </summary>
        private bool IsLineIntersectLine(BuildLandBoundaryAddressCoil fourCoil, BuildLandBoundaryAddressCoil coil, List<BuildLandBoundaryAddressDot> dotList)
        {
            var fourCoilDots = BoundaryAddressCoilToDot(dotList, fourCoil);
            var coilDots = BoundaryAddressCoilToDot(dotList, coil);
            foreach (var dot in fourCoilDots)
            {
                var count = coilDots.Count(t => t.DotNumber.Equals(dot.DotNumber));
                if (count > 0) return true;
            }
            return false;
        }

        /// <summary>
        /// 线线是否相交
        /// </summary>
        /// <param name="line"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool IsLineIntersectLine(Geometry line, Geometry targetLine, double tolerance)
        {
            if (line == null || targetLine == null) return false;
            bool isIntersect = false;
            var coordinates = line.ToCoordinates();
            var targetCoordinates = targetLine.ToCoordinates();
            foreach (var coordinate in coordinates)
            {
                foreach (var targetCoordinate in targetCoordinates)
                {
                    if (ToCoordinate(coordinate).Equals2D(ToCoordinate(targetCoordinate), tolerance))
                    {
                        isIntersect = true;
                        break;
                    }
                }
            }
            return isIntersect;
        }

        private GeoAPI.Geometries.Coordinate ToCoordinate(Coordinate coordinate)
        {
            return new GeoAPI.Geometries.Coordinate(coordinate.X, coordinate.Y);
        }

        /// <summary>
        /// 创建四角点
        /// </summary>
        private List<BuildLandBoundaryAddressDot> CreateQuadrantDots(List<BuildLandBoundaryAddressDot> dotList, Geometry centroid)
        {
            var originX = (centroid.Instance as GeoAPI.Geometries.IPoint).X;
            var originY = (centroid.Instance as GeoAPI.Geometries.IPoint).Y;
            var maxDistances = new List<double>(new double[4]);
            var quadrantDots = new List<BuildLandBoundaryAddressDot>(4);
            for (int i = 0; i < 4; i++)
            {
                quadrantDots.Add(new BuildLandBoundaryAddressDot());
            }

            foreach (var dot in dotList)
            {
                var point = dot.Shape.Instance as GeoAPI.Geometries.IPoint;
                // 以重心创建坐标系
                // 第一象限
                if (point.X > originX && point.Y > originY)
                {
                    GetMaxDistanceDot(dot, centroid, maxDistances, quadrantDots, 0);
                }
                // 第二象限
                if (point.X > originX && point.Y < originY)
                {
                    GetMaxDistanceDot(dot, centroid, maxDistances, quadrantDots, 1);
                }
                // 第三象限
                if (point.X < originX && point.Y < originY)
                {
                    GetMaxDistanceDot(dot, centroid, maxDistances, quadrantDots, 2);
                }
                // 第四象限
                if (point.X < originX && point.Y > originY)
                {
                    GetMaxDistanceDot(dot, centroid, maxDistances, quadrantDots, 3);
                }
            }
            return quadrantDots;
        }



        /// <summary>
        /// 创建四界线
        /// </summary>
        private List<BuildLandBoundaryAddressCoil> CreateFourCoils(List<BuildLandBoundaryAddressDot> dotList, List<BuildLandBoundaryAddressDot> quadrantDots)
        {

            var fourCoils = new List<BuildLandBoundaryAddressCoil>(); // 四界线
            var firstDot = new BuildLandBoundaryAddressDot();
            for (int i = 1; i <= 4; i++)
            {
                // 以第一象限的点为起始，若没有则继续循环
                firstDot = quadrantDots.Find(t => i.ToString().Equals(t.Description));
                if (firstDot != null) break;
            }
            var firstNumber = Convert.ToInt32(ToolString.GetAllNumberWithInString(firstDot.DotNumber));
            foreach (var dot in dotList)
            {
                dot.Description = string.Empty;
                dot.IsValid = false;
                for (int i = 0; i < quadrantDots.Count; i++)
                {
                    if (quadrantDots[i].ID.Equals(dot.ID))
                    {
                        dot.IsValid = true;
                        dot.Description = (i + 1).ToString(); // 使用Description字段存放是第几象限
                    }
                }
            }
            var createdots = new List<BuildLandBoundaryAddressDot>();
            short sxh = 1;
            bool hasStartKeyDot = false; //是否已经找到开始界址点
            bool hasEndKeyDot = false; //是否已经找到结束界址点
            int index = firstNumber - 2;
            for (int i = 0; i <= dotList.Count; i++) // 多循环一次，构成环形
            {
                index++;
                index = index % dotList.Count;

                if (dotList[index].IsValid && hasStartKeyDot == false)
                {
                    createdots.Add(dotList[index]);
                    hasStartKeyDot = true;
                    continue;
                }
                if (dotList[index].IsValid == false)
                {
                    createdots.Add(dotList[index]);
                    continue;
                }
                if (dotList[index].IsValid && hasStartKeyDot && hasEndKeyDot == false)
                {
                    createdots.Add(dotList[index]);

                    var coil = CreateAddressCoil(createdots, sxh);
                    fourCoils.Add(coil);

                    createdots.Clear();
                    createdots.Add(dotList[index]);
                    hasStartKeyDot = true;
                    hasEndKeyDot = false;
                    sxh++;
                }
            }
            return fourCoils;
        }

        /// <summary>
        /// 拆分界址线
        /// </summary>
        /// <param name="dotList"></param>
        /// <param name="quadrantDots"></param>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> SplitCoil(List<BuildLandBoundaryAddressDot> dotList, List<BuildLandBoundaryAddressDot> quadrantDots)
        {
            if (dotList.Count < 2) return null;
            var resCoils = new List<BuildLandBoundaryAddressCoil>();
            dotList.ForEach(t =>
            {
                t.IsValid = false;
                t.Description = string.Empty;
            });

            int firstQuadrant = Convert.ToInt32(quadrantDots[0].Description);
            int lastQuadrant = Convert.ToInt32(quadrantDots[quadrantDots.Count - 1].Description);
            dotList[0].IsValid = true;
            //dotList[0].Description = firstQuadrant == 1 ? "4" : (firstQuadrant - 1).ToString();
            dotList[dotList.Count - 1].IsValid = true;
            //dotList[dotList.Count - 1].Description = (lastQuadrant % 4 + 1).ToString();

            foreach (var dot in dotList)
            {
                for (int i = 0; i < quadrantDots.Count; i++)
                {
                    if (quadrantDots[i].ID.Equals(dot.ID))
                    {
                        dot.IsValid = true;
                        dot.Description = quadrantDots[i].Description;
                    }
                }
            }

            var createdots = new List<BuildLandBoundaryAddressDot>();
            short sxh = 1;
            bool hasStartKeyDot = false; //是否已经找到开始界址点
            bool hasEndKeyDot = false; //是否已经找到结束界址点

            foreach (var item in dotList)
            {
                if (item.IsValid && hasStartKeyDot == false)
                {
                    createdots.Add(item);
                    hasStartKeyDot = true;
                    continue;
                }
                if (item.IsValid == false)
                {
                    createdots.Add(item);
                    continue;
                }
                if (item.IsValid && hasStartKeyDot && hasEndKeyDot == false)
                {
                    createdots.Add(item);

                    var coil = CreateAddressCoil(createdots);
                    resCoils.Add(coil);

                    createdots.Clear();
                    createdots.Add(item);
                    hasStartKeyDot = true;
                    hasEndKeyDot = false;
                    sxh++;
                }
            }
            return resCoils;
        }




        /// <summary>
        /// 获取四角点
        /// </summary>
        private void GetMaxDistanceDot(BuildLandBoundaryAddressDot dot, Geometry centroid, List<double> maxDistances, List<BuildLandBoundaryAddressDot> quadrantDots, int index)
        {
            if (dot.Shape.Distance(centroid) > maxDistances[index])
            {
                maxDistances[index] = dot.Shape.Distance(centroid);
                quadrantDots[index] = dot;
                quadrantDots[index].Description = (index + 1).ToString();
            }
        }

        /// <summary>
        /// 创建界址线
        /// </summary>
        private BuildLandBoundaryAddressCoil CreateAddressCoil(List<BuildLandBoundaryAddressDot> list, short sxh)
        {
            var line = CreatLine(list);
            string description = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                if ((i + 1).ToString().Equals(list[0].Description) && ((i + 1) % 4 + 1).ToString().Equals(list[list.Count - 1].Description))
                {
                    description = Orientation[i];
                }
            }
            var coil = new BuildLandBoundaryAddressCoil()
            {
                ID = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                Shape = line,
                Modifier = "",
                ModifiedTime = DateTime.Now,
                Founder = "",
                StartPointID = list[0].ID,
                StartNumber = list[0].DotNumber,
                EndPointID = list[list.Count - 1].ID,
                EndNumber = list[list.Count - 1].DotNumber,
                OrderID = sxh,
                CoilLength = line.Length(),
                Description = description
            };
            return coil;
        }

        private BuildLandBoundaryAddressCoil CreateAddressCoil(List<BuildLandBoundaryAddressDot> list)
        {
            var line = CreatLine(list);
            string description = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                if ((i + 1).ToString().Equals(list[0].Description) && ((i + 1) % 4 + 1).ToString().Equals(list[list.Count - 1].Description))
                {
                    description = Orientation[i];
                }
            }
            if (string.IsNullOrEmpty(list[0].Description))
            {
                /*
                 若首点不是四角点,则该界址线的方位为：
                 尾点Desc - 方位 - 方位索引
                     1    -  北  -    3
                     2    -  东  -    0
                     3    -  南  -    1
                     4    -  西  -    2
                 */
                int index = Convert.ToInt32(list[list.Count - 1].Description) - 1;
                if (index == 0) description = Orientation[3];
                else description = Orientation[(index - 1) % 3];
            }
            if (string.IsNullOrEmpty(list[list.Count - 1].Description))
            {
                int index = Convert.ToInt32(list[0].Description) - 1;
                description = Orientation[index];
            }

            var coil = new BuildLandBoundaryAddressCoil()
            {
                ID = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                Shape = line,
                Modifier = "",
                ModifiedTime = DateTime.Now,
                Founder = "",
                StartPointID = list[0].ID,
                StartNumber = list[0].DotNumber,
                EndPointID = list[list.Count - 1].ID,
                EndNumber = list[list.Count - 1].DotNumber,
                CoilLength = line.Length(),
                Description = description
            };
            return coil;
        }

        /// <summary>
        /// 创建线
        /// </summary>
        private Geometry CreatLine(List<BuildLandBoundaryAddressDot> dots)
        {
            Coordinate[] corrds = new Coordinate[dots.Count];
            for (int i = 0; i < dots.Count; i++)
            {
                corrds[i] = dots[i].Shape.ToCoordinates()[0];
            }
            var geo = Geometry.CreatePolyline(corrds.ToList());
            return geo;
        }
    }
}
