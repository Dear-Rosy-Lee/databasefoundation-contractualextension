using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.Business;
using GeoAPI.Geometries;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出界址调查信息
    /// </summary>
    public class ExportDotResultTable : ExportExcelBase
    {
        #region Fields

        private int index;//下标
        private bool useUnitNumber;//是否使用统编号

        #endregion Fields

        #region Property

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 模版文件路径
        /// </summary>
        public string TempletePath { get; set; }

        /// <summary>
        /// 承包地块
        /// </summary>
        public ContractLand CurrentLand { get; set; }

        public string WritePerson { get; set; }//填表人
        public string CheckPerson { get; set; }//制表人
        public DateTime StartDate { get; set; }//制表日期

        public List<BuildLandBoundaryAddressDot> DotCollection { get; set; }

        public List<BuildLandBoundaryAddressCoil> LineCollection { get; set; }

        public VirtualPerson Contractor { get; set; }

        #endregion Property

        #region Ctor

        public ExportDotResultTable(string templetePath)
        {
            this.TempletePath = templetePath;
            //useUnitNumber = YuLinTu.Library.Business.AgricultureSetting.AgricultureLandTableUseUnitNumber;
            useUnitNumber = true;
        }

        #endregion Ctor

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="Metadata.FolderNameDestination"></param>
        public void BeginToZone(Zone zone)
        {
            if (string.IsNullOrEmpty(TempletePath) || !System.IO.File.Exists(TempletePath))
            {
                PostErrorInfo("模板路径不存在!");
                return;
            }
            if (zone == null)
            {
                PostErrorInfo("行政区域为空!");
                return;
            }
            CurrentZone = zone;
            Write();//写数据
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
                Open(TempletePath, 0);
                index = 10;
                BeginWrite();
            }
            catch (System.Exception e)
            {
                PostErrorInfo(e.Message.ToString());
                Dispose();
                if (e is TaskStopException)
                    throw e;
            }
        }

        #endregion 开始生成Excel之前的一系列操作

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
                int startIndex = index;
                AgricultureLandProgress(DotCollection, LineCollection);
                if (DotCollection.Count <= 0)
                    index += 3;
                else
                    index++;
                object value = GetRangeToValue("A" + index.ToString(), "E" + index.ToString());
                string writeInfo = "";
                if (value != null)
                {
                    List<string> strs = new List<string>();
                    string[] str = value.ToString().Split(':');
                    for (int i = 0; i < str.Length; i++)
                    {
                        string[] s = str[i].Split('：');
                        for (int j = 0; j < s.Length; j++)
                        {
                            strs.Add(s[j]);
                        }
                    }
                    writeInfo = strs[0] + ":" + WritePerson + strs[1] + ":" + CheckPerson + strs[2] + ":" + StartDate.Year + "年" + StartDate.Month + "月" + StartDate.Day + "日";
                }
                InitalizeRangeValue("A" + index.ToString(), "E" + index.ToString(), writeInfo);
                DotCollection = null;
                LineCollection = null;
                CurrentLand = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                return PostErrorInfo("生成Excel时出现错误：" + ex.Message.ToString());
            }
            return true;
        }

        #region 写数据

        /// <summary>
        /// 书写界址信息
        /// </summary>
        /// <param name="land"></param>
        /// <param name="dots"></param>
        /// <param name="lines"></param>
        private void InitalizeDotInformation(List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> lines)
        {
            int count = dots.Count * 2;
            int i = 0;
            InsertRowCell(index + 1, count);
            foreach (BuildLandBoundaryAddressDot dot in dots)
            {
                var edx = (dot.Shape.Instance as IPoint).X;
                var edy = (dot.Shape.Instance as IPoint).Y;
                InitalizeRangeValue("A" + index.ToString(), "A" + (index + 1).ToString(), useUnitNumber ? dot.UniteDotNumber : dot.DotNumber);//界址点号
                InitalizeRangeValue("B" + index.ToString(), "B" + (index + 1).ToString(), edx.ToString("f3"));//x轴
                InitalizeRangeValue("C" + index.ToString(), "C" + (index + 1).ToString(), edy.ToString("f3"));//Y轴
                InitalizeRangeValue("D" + (index + 1).ToString(), "D" + (index + 2).ToString(), lines[i].CoilLength.ToString("f2"));//边长
                InitalizeRangeValue("E" + index.ToString(), "E" + (index + 1).ToString(), "");//说明
                index += 2;
                i++;
            }
            var edx1 = (dots[0].Shape.Instance as IPoint).X;
            var edy1 = (dots[0].Shape.Instance as IPoint).Y;
            InitalizeRangeValue("A" + index.ToString(), "A" + (index + 1).ToString(), useUnitNumber ? dots[0].UniteDotNumber : dots[0].DotNumber);
            InitalizeRangeValue("B" + index.ToString(), "B" + (index + 1).ToString(), edx1.ToString("f3"));
            InitalizeRangeValue("C" + index.ToString(), "C" + (index + 1).ToString(), edy1.ToString("f3"));
            InitalizeRangeValue("E" + index.ToString(), "E" + (index + 1).ToString(), "");//说明
            index += 2;
        }

        #endregion 写数据

        /// <summary>
        /// 书写表头
        /// </summary>
        private void WriteTitle()
        {
            string landNumber = ContractLand.GetLandNumber(CurrentLand.CadastralNumber);
            if (landNumber.Length > AgricultureSetting.AgricultureLandWordLandNumberMedian)
            {
                landNumber = landNumber.Substring(AgricultureSetting.AgricultureLandWordLandNumberMedian);
            }
            InitalizeRangeValue("B4", "C4", CreatFamarNumber(Contractor.FamilyNumber));//承包方编码
            InitalizeRangeValue("B6", "C6", landNumber);//宗地编码
            InitalizeRangeValue("E6", "E6", CurrentLand.ActualArea.ToString("f2"));//宗地面积
        }

        /// <summary>
        /// 创建农户号
        /// </summary>
        private string CreatFamarNumber(string familyNumber)
        {
            string farmerNumber = familyNumber.PadLeft(4, '0');
            string zoneCode = CurrentZone.FullCode;
            if (CurrentZone.Level == eZoneLevel.Group && zoneCode.Length == 16)
            {
                zoneCode = zoneCode.Substring(0, 12) + zoneCode.Substring(14, 2);
            }
            return zoneCode + farmerNumber;
        }

        #endregion 开始生成Excel

        #endregion Methods

        #region Business

        /// <summary>
        /// 农用地数据处理
        /// </summary>
        private void AgricultureLandProgress(List<BuildLandBoundaryAddressDot> dotCollection, List<BuildLandBoundaryAddressCoil> lineCollection)
        {
            List<BuildLandBoundaryAddressDot> dots = dotCollection.ToList();
            List<BuildLandBoundaryAddressCoil> lines = lineCollection.ToList();
            dots = OrderByDotInformation(dots);
            lines = OrderByLineInformtion(lines);
            if (dots.Count > 0 && lines.Count > 0)
            {
                InitalizeDotInformation(dots, lines);
            }
            dots = null;
            lines = null;
        }

        #endregion Business

        #region Helper

        /// <summary>
        /// 对界址点进行排序
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressDot> OrderByDotInformation(List<BuildLandBoundaryAddressDot> dotCollection)
        {
            var orderBy = dotCollection.OrderBy(le =>
            {
                string number = Library.Business.ToolString.GetAllNumberWithInString(le.DotNumber);
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
            var orderBy = lineCollection.OrderBy(le => le.OrderID);
            List<BuildLandBoundaryAddressCoil> lineArray = new List<BuildLandBoundaryAddressCoil>();
            foreach (var line in orderBy)
            {
                lineArray.Add(line);
            }
            return lineArray;
        }

        #endregion Helper
    }
}