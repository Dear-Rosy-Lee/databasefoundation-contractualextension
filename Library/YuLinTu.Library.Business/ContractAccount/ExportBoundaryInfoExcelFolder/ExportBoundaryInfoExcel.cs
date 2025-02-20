/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Library.Office;
using System.IO;
using YuLinTu.Spatial;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    public class ExportBoundaryInfoExcel : ExportExcelBase
    {

        #region Fields

        private int index;//下标
        //private bool canShow;//是否可以显示
        private List<Dictionary> dictsJXXZ = new List<Dictionary>();   //界线性质
        private List<Dictionary> dictsJZXLB = new List<Dictionary>();  //界址线类别
        private List<Dictionary> dictsJZXWZ = new List<Dictionary>();  //界址线位置 
        private List<Dictionary> dictsJBLX = new List<Dictionary>();   //界标类型
        private List<Dictionary> dictsJZDLX = new List<Dictionary>();  //界址点类型
        private ToolProgress toolProgress;//进度工具

        #endregion

        #region Property

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 模版文件路径
        /// </summary>
        public string TempletePath { get; set; }

        /// <summary>
        /// 地块类型
        /// </summary>
        public LanderType LandorType { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; } //当前地域

        /// <summary>
        /// 当前地块
        /// </summary>
        public ContractLand CurrentLand { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 表头信息
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 当前地域下的含空间数据地块集合
        /// </summary>
        public List<ContractLand> CurrentZoneLandList { get; set; }

        /// <summary>
        //  当前地域下界址点
        /// <summary>
        public List<BuildLandBoundaryAddressDot> CurrentZoneDots { get; set; }

        /// <summary>
        /// 当前地域下界址线
        /// <summary>
        public List<BuildLandBoundaryAddressCoil> CurrentZoneCoils { get; set; }

        /// <summary>
        /// 已导出界址信息的地块个数
        /// </summary>
        public int ExportLandCount { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportBoundaryInfoExcel()
        {
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += toolProgress_OnPostProgress;
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

        #endregion

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="Metadata.FolderNameDestination"></param>
        public bool BeginToZone(Zone zone)
        {
            dictsJXXZ = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JXXZ);
            dictsJZXLB = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JZXLB);
            dictsJZXWZ = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JZXWZ);
            dictsJBLX = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JBLX);
            dictsJZDLX = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JZDLX);

            bool result = true;
            if (string.IsNullOrEmpty(TempletePath) || !System.IO.File.Exists(TempletePath))
            {
                PostErrorInfo("模板路径不存在!");
                return false;
            }
            if (zone == null)
            {
                PostErrorInfo("行政区域为空!");
                return false;
            }

            CurrentZone = zone;
            Write();//写数据
            return result;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public override void Read()
        {
        }

        public override void Write()
        {
            try
            {
                Open(TempletePath);
                index = 1;
                BeginWrite();
                if (File.Exists(SaveFilePath))
                {
                    File.SetAttributes(SaveFilePath, FileAttributes.Normal);
                    File.Delete(SaveFilePath);
                }
                SaveAs(SaveFilePath);    //保存文件
            }
            catch (System.Exception e)
            {
                //PostErrorInfo(e.Message.ToString());
                PostErrorInfo(string.Format("写入表格出错。可能原因是：{0}", e.Message.ToString()));
                Dispose();
            }
        }

        #endregion

        #region 开始生成Excel

        /// <summary>
        /// 开始写数据
        /// </summary>
        /// <returns></returns>
        private bool BeginWrite()
        {
            WriteTitle();
            try
            {
                //canShow = false;
                //eLandPropertyRightType landType = eLandPropertyRightType.AgricultureLand;
                //List<BuildLandBoundaryAddressDot> dotCollection = InitalizeDotValue(landType);
                //List<BuildLandBoundaryAddressCoil> lineCollection = InitalizeLineValue(landType);

                List<BuildLandBoundaryAddressDot> dotCollection = CurrentZoneDots;
                List<BuildLandBoundaryAddressCoil> lineCollection = CurrentZoneCoils;

                AgricultureLandProgress(dotCollection, lineCollection);

                dotCollection = null;
                lineCollection = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                return PostErrorInfo("生成Excel时出现错误：" + ex.Message.ToString());
            }

            SetLineType("A3", "M" + (index - 1));

            return true;
        }

        #region 写数据

        /// <summary>
        /// 书写界址信息
        /// </summary>
        private void InitalizeDotInformation(string landNumber, List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> lines)
        {
            int count = index + dots.Count * 2 + 1;
            SetRange("A" + index, "A" + count, 10, landNumber, false);
            int i = 0;
            foreach (BuildLandBoundaryAddressDot dot in dots)
            {
                Coordinate[] dotcdts = dot.Shape == null ? null : dot.Shape.ToCoordinates();
                SetRange("B" + index.ToString(), "B" + (index + 1).ToString(), 10, !string.IsNullOrEmpty(dot.UniteDotNumber) ? dot.UniteDotNumber : dot.DotNumber, false);
                var jbdlx = dictsJZDLX.Find(j => j.Code == dot.DotType);
                var jbdlxname = jbdlx == null ? "" : jbdlx.Name;
                SetRange("C" + index.ToString(), "C" + (index + 1).ToString(), 10, jbdlxname, false);
                var jblx = dictsJBLX.Find(j => j.Code == dot.LandMarkType);
                var jblxname = jblx == null ? "" : jblx.Name;
                SetRange("D" + index.ToString(), "D" + (index + 1).ToString(), 10, jblxname, false);

                SetRange("E" + index.ToString(), "E" + (index + 1).ToString(), 10, dotcdts == null ? "" : dotcdts[0].X.ToString(), false);
                SetRange("F" + index.ToString(), "F" + (index + 1).ToString(), 10, dotcdts == null ? "" : dotcdts[0].Y.ToString(), false);
                SetRange("G" + (index + 1).ToString(), "G" + (index + 2).ToString(), 11, ToolMath.SetNumbericFormat(lines[i].CoilLength.ToString(), 2));

                var jxxz = dictsJXXZ.Find(j => j.Code == lines[i].LineType);
                var jxxzname = jxxz == null ? "" : jxxz.Name;
                SetRange("H" + (index + 1).ToString(), "H" + (index + 2).ToString(), 11, jxxzname);

                var jxlb = dictsJZXLB.Find(j => j.Code == lines[i].CoilType);
                var jxlbname = jxlb == null ? "" : jxlb.Name;
                SetRange("I" + (index + 1).ToString(), "I" + (index + 2).ToString(), 11, jxlbname);

                var jxwz = dictsJZXWZ.Find(j => j.Code == lines[i].Position);
                var jxwzname = jxwz == null ? "" : jxwz.Name;
                SetRange("J" + (index + 1).ToString(), "J" + (index + 2).ToString(), 11, jxwzname);
                SetRange("K" + (index + 1).ToString(), "K" + (index + 2).ToString(), 11, lines[i].Description);

                SetRange("L" + (index + 1).ToString(), "L" + (index + 2).ToString(), 11, lines[i].NeighborPerson);
                SetRange("M" + (index + 1).ToString(), "M" + (index + 2).ToString(), 11, lines[i].NeighborFefer);

                index += 2;
                i++;
            }

            Coordinate[] dotcdt0s = dots[0].Shape == null ? null : dots[0].Shape.ToCoordinates();
            SetRange("B" + index.ToString(), "B" + (index + 1).ToString(), 10, !string.IsNullOrEmpty(dots[0].UniteDotNumber) ? dots[0].UniteDotNumber : dots[0].DotNumber, false);

            var jbdlx0 = dictsJZDLX.Find(j => j.Code == dots[0].DotType);
            var jbdlxname0 = jbdlx0 == null ? "" : jbdlx0.Name;
            SetRange("C" + index.ToString(), "C" + (index + 1).ToString(), 10, jbdlxname0, false);

            var jblx0 = dictsJBLX.Find(j => j.Code == dots[0].LandMarkType);
            var jblxname0 = jblx0 == null ? "" : jblx0.Name;
            SetRange("D" + index.ToString(), "D" + (index + 1).ToString(), 10, jblxname0, false);

            SetRange("E" + index.ToString(), "E" + (index + 1).ToString(), 10, dotcdt0s == null ? "" : dotcdt0s[0].X.ToString(), false);
            SetRange("F" + index.ToString(), "F" + (index + 1).ToString(), 10, dotcdt0s == null ? "" : dotcdt0s[0].Y.ToString(), false);
            index += 2;
        }

        /// <summary>
        /// 并行书写界址信息(写表格值不可取)
        /// </summary>
        private void InitalizeDotInformationParallel(string landNumber, List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> lines)
        {
            int count = index + dots.Count * 2 + 1;
            SetRange("A" + index, "A" + count, 10, landNumber, false);
            int i = 0;
            Parallel.ForEach(dots, dot =>
            {
                Coordinate[] dotcdts = dot.Shape == null ? null : dot.Shape.ToCoordinates();
                SetRange("B" + index.ToString(), "B" + (index + 1).ToString(), 10, !string.IsNullOrEmpty(dot.UniteDotNumber) ? dot.UniteDotNumber : dot.DotNumber, false);
                var jbdlx = dictsJZDLX.Find(j => j.Code == dot.DotType);
                var jbdlxname = jbdlx == null ? "" : jbdlx.Name;
                SetRange("C" + index.ToString(), "C" + (index + 1).ToString(), 10, jbdlxname, false);
                var jblx = dictsJBLX.Find(j => j.Code == dot.LandMarkType);
                var jblxname = jblx == null ? "" : jblx.Name;
                SetRange("D" + index.ToString(), "D" + (index + 1).ToString(), 10, jblxname, false);

                SetRange("E" + index.ToString(), "E" + (index + 1).ToString(), 10, dotcdts == null ? "" : dotcdts[0].X.ToString(), false);
                SetRange("F" + index.ToString(), "F" + (index + 1).ToString(), 10, dotcdts == null ? "" : dotcdts[0].Y.ToString(), false);
                SetRange("G" + (index + 1).ToString(), "G" + (index + 2).ToString(), 11, ToolMath.SetNumbericFormat(lines[i].CoilLength.ToString(), 2));

                var jxxz = dictsJXXZ.Find(j => j.Code == lines[i].LineType);
                var jxxzname = jxxz == null ? "" : jxxz.Name;
                SetRange("H" + (index + 1).ToString(), "H" + (index + 2).ToString(), 11, jxxzname);

                var jxlb = dictsJZXLB.Find(j => j.Code == lines[i].CoilType);
                var jxlbname = jxlb == null ? "" : jxlb.Name;
                SetRange("I" + (index + 1).ToString(), "I" + (index + 2).ToString(), 11, jxlbname);

                var jxwz = dictsJZXWZ.Find(j => j.Code == lines[i].Position);
                var jxwzname = jxwz == null ? "" : jxwz.Name;
                SetRange("J" + (index + 1).ToString(), "J" + (index + 2).ToString(), 11, jxwzname);
                SetRange("K" + (index + 1).ToString(), "K" + (index + 2).ToString(), 11, lines[i].Description);

                SetRange("L" + (index + 1).ToString(), "L" + (index + 2).ToString(), 11, lines[i].NeighborPerson);
                SetRange("M" + (index + 1).ToString(), "M" + (index + 2).ToString(), 11, lines[i].NeighborFefer);

                index += 2;
                i++;
            });

            Coordinate[] dotcdt0s = dots[0].Shape == null ? null : dots[0].Shape.ToCoordinates();
            SetRange("B" + index.ToString(), "B" + (index + 1).ToString(), 10, !string.IsNullOrEmpty(dots[0].UniteDotNumber) ? dots[0].UniteDotNumber : dots[0].DotNumber, false);

            var jbdlx0 = dictsJZDLX.Find(j => j.Code == dots[0].DotType);
            var jbdlxname0 = jbdlx0 == null ? "" : jbdlx0.Name;
            SetRange("C" + index.ToString(), "C" + (index + 1).ToString(), 10, jbdlxname0, false);

            var jblx0 = dictsJBLX.Find(j => j.Code == dots[0].LandMarkType);
            var jblxname0 = jblx0 == null ? "" : jblx0.Name;
            SetRange("D" + index.ToString(), "D" + (index + 1).ToString(), 10, jblxname0, false);

            SetRange("E" + index.ToString(), "E" + (index + 1).ToString(), 10, dotcdt0s == null ? "" : dotcdt0s[0].X.ToString(), false);
            SetRange("F" + index.ToString(), "F" + (index + 1).ToString(), 10, dotcdt0s == null ? "" : dotcdt0s[0].Y.ToString(), false);
            index += 2;
        }

        #endregion

        /// <summary>
        /// 书写表头
        /// </summary>
        private void WriteTitle()
        {
            string column = "M";


            SetRange("A" + index, column + index, 27, 18, true, Title + "界址点与界址线信息表");
            index += 3;
        }

        #endregion

        #endregion

        #region Business

        /// <summary>
        /// 农用地数据处理
        /// </summary>
        private void AgricultureLandProgress(List<BuildLandBoundaryAddressDot> dotCollection, List<BuildLandBoundaryAddressCoil> lineCollection)
        {
            if (dotCollection == null || lineCollection == null)
                return;
            int landCount = 0;
            toolProgress.InitializationPercent(CurrentZoneLandList.Count, 99, 1);

            foreach (ContractLand land in CurrentZoneLandList)
            {
                List<BuildLandBoundaryAddressDot> dots = dotCollection.FindAll(dt => dt.LandID == land.ID);
                List<BuildLandBoundaryAddressCoil> lines = lineCollection.FindAll(le => le.LandID == land.ID);
                dots = OrderByDotInformation(dots);
                lines = OrderByLineInformtion(lines);
                if (dots.Count == 0 && lines.Count == 0)
                {
                    string record = string.Format("未找到地块编码为{0}的界址点、线数据,已略过!", land.LandNumber);
                    PostExceptionInfo(record);
                    continue;
                }
                var validDots = dots.FindAll(c => c.IsValid);
                if (validDots == null || validDots.Count == 0)
                {
                    PostExceptionInfo(string.Format("未找到地块编码为{0}的有效界址点,已略过!", land.LandNumber));
                    continue;
                }
                InitalizeDotInformation(land.LandNumber, validDots, lines);
                toolProgress.DynamicProgress(string.Format("导出{0}的地块界址信息", land.OwnerName));
                landCount++;
                dots = null;
                lines = null;
                validDots = null;
            }

            ExportLandCount = landCount;
            CurrentZoneLandList = null;
        }

        #endregion

        #region Helper

        /// <summary>
        /// 初始化权属类型
        /// </summary>
        /// <param name="landerType"></param>
        /// <returns></returns>
        private eLandPropertyRightType InitalziePropertyType()
        {
            eLandPropertyRightType landType = eLandPropertyRightType.Other;
            switch (LandorType)
            {
                case LanderType.AgricultureLand:
                    landType = eLandPropertyRightType.AgricultureLand;
                    break;
                case LanderType.CollectiveLand:
                    landType = eLandPropertyRightType.CollectiveLand;
                    break;
                case LanderType.ConstructionLand:
                    landType = eLandPropertyRightType.ConstructionLand;
                    break;
                case LanderType.HomeSteadLand:
                    landType = eLandPropertyRightType.HomeSteadLand;
                    break;
                case LanderType.WoodLand:
                    landType = eLandPropertyRightType.Wood;
                    break;
                case LanderType.Irrigation:
                    landType = eLandPropertyRightType.Irrigation;
                    break;
                default:
                    break;
            }
            return landType;
        }

        /// <summary>
        /// 初始化界址点信息
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressDot> InitalizeDotValue(eLandPropertyRightType landType)
        {
            List<BuildLandBoundaryAddressDot> dotCollection = CurrentZoneDots;
            if (dotCollection != null && dotCollection.Count > 0)
            {
                foreach (var dot in dotCollection)
                {
                    if (int.Parse(dot.DotType) == 4)
                    {
                        dot.DotType = eBoundaryPointType.ResolvePoint.ToString();
                    }
                }
                return dotCollection;
            }

            return dotCollection;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineValue(eLandPropertyRightType landType)
        {
            List<BuildLandBoundaryAddressCoil> lineCollection = CurrentZoneCoils;
            if (lineCollection != null && lineCollection.Count > 0)
            {
                foreach (var line in lineCollection)
                {
                    if (int.Parse(line.LineType) == 0)
                    {
                        line.LineType = eBoundaryNatureType.FixBoundary.ToString();
                    }
                }
                return lineCollection;
            }


            return lineCollection;
        }

        /// <summary>
        /// 对界址点进行排序
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressDot> OrderByDotInformation(List<BuildLandBoundaryAddressDot> dotCollection)
        {
            if (dotCollection == null || dotCollection.Count == 0)
                return new List<BuildLandBoundaryAddressDot>();
            var orderBy = dotCollection.OrderBy(le =>
            {
                string number = ToolString.GetAllNumberWithInString(le.DotNumber);
                int val = 0;
                Int32.TryParse(number, out val);
                return val;
            });
            List<BuildLandBoundaryAddressDot> dotArray = new List<BuildLandBoundaryAddressDot>();
            foreach (var dot in orderBy)
            {
                dotArray.Add(dot);
            }
            return dotArray;
        }

        /// <summary>
        /// 對界址线进行排序
        /// </summary>
        /// <param name="lineCollection"></param>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> OrderByLineInformtion(List<BuildLandBoundaryAddressCoil> lineCollection)
        {
            if (lineCollection == null || lineCollection.Count == 0)
                return new List<BuildLandBoundaryAddressCoil>();
            var orderBy = lineCollection.OrderBy(le => le.OrderID);
            List<BuildLandBoundaryAddressCoil> lineArray = new List<BuildLandBoundaryAddressCoil>();
            foreach (var line in orderBy)
            {
                lineArray.Add(line);
            }
            return lineArray;
        }

        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }
        #endregion
    }
}
