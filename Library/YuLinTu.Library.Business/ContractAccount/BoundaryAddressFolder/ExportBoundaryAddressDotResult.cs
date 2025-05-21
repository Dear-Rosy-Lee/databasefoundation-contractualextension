/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出界址点成果表
    /// </summary>
    public class ExportBoundaryAddressDotResult : ExportExcelBase
    {
        #region Ctor

        private SystemSetDefine systemset = SystemSetDefine.GetIntence();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportBoundaryAddressDotResult()
        {
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
        }

        /// <summary>
        /// 进度提示
        /// </summary>    
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }


        #endregion

        #region Fields

        private ToolProgress toolProgress;
        private IDbContext dbContext;   //数据库
        private CollectiveLand land;//集体土地
        private ConstructionLand consLand;//集体建设用地
        private BuildLandProperty homeLand;//农村宅基地
        private ContractLand conLand;//承包经营权
        private int index;//下标
        private int currentIndex;//当前序号
        private string templatePath;
        private List<BuildLandBoundaryAddressDot> dotCollection;
        private List<BuildLandBoundaryAddressCoil> lineCollection;

        #endregion

        #region Properties

        /// <summary>
        /// 导出类型
        /// </summary>
        public int Exporttype { get; set; }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SavePath { get; set; }

        #endregion

        #region Methods

        #region Method-开始生成Excel之前的一系列操作

        /// <summary>
        /// 写表格数据
        /// </summary>  
        public bool CommenceLandExcel(ContractLand land, string templatePath)
        {
            try
            {
                PostProgress(1);

                if (!File.Exists(templatePath))
                    return PostErrorInfo("模板路径不存在！");

                if (land == null)
                    return PostErrorInfo("承包经营权不存在！");

                dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                    return PostErrorInfo(DataBaseSource.ConnectionError);

                this.conLand = land;
                this.templatePath = templatePath;
                index = 9;
                currentIndex = 1;

                Write();//写数据

                PostProgress(100);
            }
            catch (SystemException ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "CommenceLandExcel(写表格数据失败)", ex.Message + ex.StackTrace);
            }
            return true;
        }

        /// <summary>
        /// 重写写数据
        /// </summary>
        public override void Write()
        {
            try
            {
                PostProgress(5);
                OpenExcelFile();
                PostProgress(15);
                dbContext.OpenConnection();
                Guid landGuid = Guid.Empty;
                if (land != null)
                {
                    landGuid = land.ID;
                }
                else if (consLand != null)
                {
                    landGuid = consLand.ID;
                }
                else if (homeLand != null)
                {
                    landGuid = homeLand.ID;
                }
                else if (conLand != null)
                {
                    landGuid = conLand.ID;
                }
                bool canContinue = BeginWrite(landGuid);
                dbContext.CloseConnection();
                if (!canContinue)
                {
                    Dispose();
                    return;
                }
                string saveFPath = "";
                //if (Exporttype == 1)
                //{
                //    Show();
                //}
                if (Exporttype == 2)
                {
                    Print();
                    Dispose();
                    Clear();
                }
                else
                {
                    if (land != null)
                    {
                        saveFPath = Path.Combine(SavePath, land.LandNumber.Substring(land.SenderCode.Length) + ".xls");
                        SaveAs(saveFPath);
                    }
                    else if (consLand != null)
                    {
                        saveFPath = Path.Combine(SavePath, consLand.OwnerName + ContractLand.GetLandNumber(consLand.CadastralNumber) + ".xls");
                        SaveAs(saveFPath);
                    }
                    else if (homeLand != null)
                    {
                        saveFPath = Path.Combine(SavePath, homeLand.HouseHolderName + ContractLand.GetLandNumber(homeLand.CadastralNumber) + ".xls");
                        SaveAs(saveFPath);
                    }
                    else if (conLand != null)
                    {
                        saveFPath = Path.Combine(SavePath, conLand.OwnerName + ContractLand.GetLandNumber(conLand.CadastralNumber) + ".xls");
                        SaveAs(saveFPath);
                    }
                    else { }
                    if (Exporttype == 1)
                        PrintView(saveFPath);
                    Dispose();
                    Clear();
                }
            }
            catch (System.Exception e)
            {
                dbContext.CloseConnection();
                PostErrorInfo(e.Message.ToString());
                Dispose();
                Clear();
                YuLinTu.Library.Log.Log.WriteException(this, "Write(导出界址点成果表格失败)", e.Message + e.StackTrace);
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(templatePath);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        private void Clear()
        {
            land = null;//集体土地
            consLand = null;//集体建设用地
            homeLand = null;//农村宅基地
            conLand = null;//承包经营权
            lineCollection = null;
            lineCollection = new List<BuildLandBoundaryAddressCoil>();
            dotCollection = null;
            dotCollection = new List<BuildLandBoundaryAddressDot>();
        }

        #endregion

        #region Method-开始打印Excel

        /// <summary>
        /// 开始写数据
        /// </summary>
        private bool BeginWrite(Guid landGuid)
        {
            WriteTitle();
            return WriteContent(landGuid);
        }

        /// <summary>
        /// 书写标题
        /// </summary>
        private void WriteTitle()
        {
            if (land != null)
            {
                SetRange("A3", "E3", "    宗地号: " + land.LandNumber.Substring(land.SenderCode.Length), true);   //ZoneCode为权属单位代码
                SetRange("A4", "E4", "    承包方: " + land.OwnUnitName.InitalizeFamilyName(systemset.KeepRepeatFlag), true);
                bool useSquareArea = true;   //使用平方米
                if (useSquareArea)
                {
                    SetRange("A5", "E5", "    宗地面积(平方米): " + ToolMath.SetNumbericFormat((land.Area.HasValue ? land.Area.Value : 0.0).ToString(), 2));
                }
                else
                {
                    SetRange("A5", "E5", "    宗地面积(公顷): " + ToolMath.SetNumbericFormat((land.Area.HasValue ? land.Area.Value * 0.0001 : 0.0).ToString(), 4));
                }
                return;
            }
            if (consLand != null)
            {
                SetRange("A3", "E3", "    宗地号: " + ContractLand.GetLandNumber(consLand.CadastralNumber), true);
                SetRange("A4", "E4", "    承包方: " + consLand.OwnerName.InitalizeFamilyName(systemset.KeepRepeatFlag), true);
                SetRange("A5", "E5", "    宗地面积(平方米): " + ToolMath.SetNumbericFormat(consLand.SelfArea.ToString(), 2));
                return;
            }
            if (homeLand != null)
            {
                SetRange("A3", "E3", "    宗地号: " + ContractLand.GetLandNumber(homeLand.CadastralNumber), true);
                SetRange("A4", "E4", "    承包方: " + homeLand.HouseHolderName, true);
                SetRange("A5", "E5", "    宗地面积(平方米): " + ToolMath.SetNumbericFormat(homeLand.SelfArea.ToString(), 2));
            }
            if (conLand != null)
            {
                SetRange("B3", "B3", conLand.OwnerName.InitalizeFamilyName(systemset.KeepRepeatFlag), true);
                SetRange("B4", "B4", conLand.LandNumber, true);
                SetRange("B5", "B5", ToolMath.RoundNumericFormat(conLand.ActualArea, 4).ToString() + "(亩)");
                SetRange("E3", "E3", conLand.Name, true);
                SetRange("E4", "E4", conLand.LandName, true);
                SetRange("E5", "E5", (conLand.IsFarmerLand != null && conLand.IsFarmerLand.HasValue) ? (conLand.IsFarmerLand.Value ? "是" : "否") : "");
            }
        }

        /// <summary>
        /// 书写内容
        /// </summary>
        private bool WriteContent(Guid landGuid)
        {
            string message = InitalizeData(landGuid);
            if (lineCollection.Count == 0 && dotCollection.Count == 0)
            {
                PostExceptionInfo(message);
                return false;
            }
            if (dotCollection.Count > 0 && lineCollection.Count == 0
                || dotCollection.Count == 0 && lineCollection.Count > 0)
            {
                return false;
            }
            int dint = 1;
            for (int i = 0; i < dotCollection.Count - 1; i++)
            {
                SetDotValue(dotCollection[i], dotCollection[dint], true);
                index += 2;
                currentIndex++;
                PostCurrentProgress(currentIndex);
                int value = 61 + (index / 69) * 69;
                if (index == value)
                {
                    index++;
                    SetTitle();
                    currentIndex--;
                    SetDotValue(dotCollection[i], dotCollection[dint], true);
                    index += 2;
                    currentIndex++;
                }
                ++dint;
            }
            SetDotValue(dotCollection[dotCollection.Count - 1], dotCollection[0], true);
            index += 2;
            currentIndex++;
            SetDotValue(dotCollection[0], dotCollection[dotCollection.Count - 1], false);
            index += 2;
            while (index < 61 + (index / 69) * 69)
            {
                SetDotNullValue();
                index += 2;
            }
            SetRange("E2", "E2", "共 " + index / 61 + " 页");
            dotCollection = null;
            lineCollection = null;
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private string InitalizeData(Guid landGuid)
        {
            try
            {
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                dotCollection = dotStation.GetByLandID(landGuid);
                lineCollection = coilStation.GetByLandID(landGuid);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetDotData/GetCoilData(获取界址点/界址线数据失败)", ex.Message + ex.StackTrace);
                PostErrorInfo("获取界址点/界址线数据失败");
            }

            LanderType landerType = LanderType.AgricultureLand;

            if (land != null)
            {
                landerType = LanderType.CollectiveLand;
            }
            if (consLand != null)
            {
                landerType = LanderType.ConstructionLand;
            }
            if (homeLand != null)
            {
                landerType = LanderType.HomeSteadLand;
            }
            if (conLand != null)
            {
                landerType = LanderType.AgricultureLand;
            }
            string message = string.Empty;

            if (dotCollection.Count == 0 || lineCollection.Count == 0)
            {
                switch (landerType)
                {
                    case LanderType.CollectiveLand:
                        message = "宗地编码:" + land.LandNumber.Replace(land.SenderCode, "") + "无空间信息!";
                        break;
                    case LanderType.ConstructionLand:
                        message = "宗地编码:" + consLand.CadastralNumber + "无空间信息!";
                        break;
                    case LanderType.HomeSteadLand:
                        message = "宗地编码:" + ContractLand.GetLandNumber(homeLand.CadastralNumber) + "无空间信息!";
                        break;
                    case LanderType.AgricultureLand:
                        message = "地块编码:" + ContractLand.GetLandNumber(conLand.CadastralNumber) + "无空间信息!";
                        break;
                }
                if (dotCollection.Count == 0)
                {
                    dotCollection = conLand.InitialDotShape();   //生成界址点
                }
                if (lineCollection.Count == 0)
                {
                    lineCollection = dotCollection.InitialCoilShape();   //生成界址线
                }
            }
            if (lineCollection != null && lineCollection.Count > 0)
            {
                lineCollection.Sort();
            }
            if (dotCollection != null && dotCollection.Count > 0)
            {
                dotCollection.Sort();
            }
            if (lineCollection == null)
            {
                lineCollection = new List<BuildLandBoundaryAddressCoil>();
            }
            if (dotCollection == null)
            {
                dotCollection = new List<BuildLandBoundaryAddressDot>();
            }
            return message;
        }

        /// <summary>
        /// 设置点值
        /// </summary>
        private void SetDotValue(BuildLandBoundaryAddressDot dot, BuildLandBoundaryAddressDot nextdot, bool writeLine)
        {
            try
            {
                var geo = dot.Shape.Instance;
                GeoAPI.Geometries.IPoint dotPoint = geo as GeoAPI.Geometries.IPoint;

                var nextgeo = nextdot.Shape.Instance;
                GeoAPI.Geometries.IPoint nextdotPoint = nextgeo as GeoAPI.Geometries.IPoint;

                if (dotPoint != null)
                {
                    SetRange(string.Format("C{0}", index), string.Format("C{0}", index + 1), ToolMath.SetNumbericFormat(ToolMath.RoundNumericFormat(dotPoint.X, 3).ToString(), 3), true);
                    SetRange(string.Format("D{0}", index), string.Format("D{0}", index + 1), ToolMath.SetNumbericFormat(ToolMath.RoundNumericFormat(dotPoint.Y, 3).ToString(), 3), true);
                }
                SetRange(string.Format("A{0}", index), string.Format("A{0}", index + 1), currentIndex.ToString(), true);
                SetRange(string.Format("B{0}", index), string.Format("B{0}", index + 1), string.IsNullOrEmpty(dot.UniteDotNumber) ? dot.DotNumber : dot.UniteDotNumber, true);
                int value = 61 + (index / 69) * 69;
                if (writeLine && index != value)
                {
                    //SetRange(string.Format("E{0}", index + 1), string.Format("E{0}", index + 2), SearchLine(lineCollection[currentIndex - 1].StartPointID, lineCollection[currentIndex - 1].EndPointID), true);
                    SetRange(string.Format("E{0}", index + 1), string.Format("E{0}", index + 2), GetDotDistance(dotPoint.X, dotPoint.Y, nextdotPoint.X, nextdotPoint.Y), true);
                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// 两点间的距离
        /// </summary>
        /// <returns></returns>
        private string GetDotDistance(double dx1, double dy1, double dx2, double dy2)
        {
            string dotDistance = "";

            var dis = Math.Sqrt(Math.Pow((dx2 - dx1), 2) + Math.Pow((dy2 - dy1), 2));

            if (dis == 0)
            {
                dotDistance = "0";
            }
            else
            {
                dotDistance = ToolMath.SetNumbericFormat(ToolMath.RoundNumericFormat(dis, 2).ToString(), 2);
            }

            return dotDistance;
        }





        /// <summary>
        /// 搜索线
        /// </summary>
        private string SearchLine(Guid startID, Guid endID)
        {
            foreach (var line in lineCollection)
            {
                if (line.StartPointID == startID && line.EndPointID == endID)
                    return ToolMath.SetNumbericFormat(line.CoilLength.ToString(), 2).ToString();
            }
            return "";
        }

        /// <summary>
        /// 报告当前进度
        /// </summary>
        private void PostCurrentProgress(int persent)
        {
            if (persent > 15 && persent < 100)
            {
                PostProgress(persent);
            }
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        private void SetTitle()
        {
            SetRange("A" + index.ToString(), "A" + (index + 1).ToString(), "序 号", true);
            SetRange("B" + index.ToString(), "B" + (index + 1).ToString(), "点 号", true);
            SetRange("C" + index.ToString(), "C" + (index + 1).ToString(), "x(m)", true);
            SetRange("D" + index.ToString(), "D" + (index + 1).ToString(), "y(m)", true);
            SetRange("E" + index.ToString(), "E" + (index + 1).ToString(), "界址边长 (m)", true);
            index += 2;
        }

        /// <summary>
        /// 设置点值
        /// </summary>
        private void SetDotNullValue()
        {
            SetRange(string.Format("A{0}", index), string.Format("A{0}", index + 1), "", true);
            SetRange(string.Format("B{0}", index), string.Format("B{0}", index + 1), "", true);
            SetRange(string.Format("C{0}", index), string.Format("C{0}", index + 1), "", true);
            SetRange(string.Format("D{0}", index), string.Format("D{0}", index + 1), "", true);
            SetRange(string.Format("E{0}", index - 1), string.Format("E{0}", index), "", true);
        }

        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }

        #endregion

        #endregion
    }
}
