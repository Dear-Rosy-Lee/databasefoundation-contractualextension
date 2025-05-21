/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入界址调查信息
    /// </summary>
    public class AgricultureLandDotSurvey : Task
    {
        #region Fields

        private Zone currentZone;//当前地域
        private IDbContext dbContext;//数据库
        private LanderType landorType;//土地类型
        private string fileName;//文件
        private ToolProgress toolProgress;//进度工具
        /// <summary>
        /// 界址点类型
        /// </summary>
        private List<Dictionary> dotTypeList;
        /// <summary>
        /// 界线性质
        /// </summary>
        private List<Dictionary> linePropertyList;
        /// <summary>
        /// 界址线位置
        /// </summary>
        private List<Dictionary> linePositionList;
        /// <summary>
        /// 界址线类型
        /// </summary>
        private List<Dictionary> lineTypeList;
        /// <summary>
        /// 界标类型
        /// </summary>
        private List<Dictionary> markTypeList;

        #endregion

        #region Ctor

        public AgricultureLandDotSurvey()
        {
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += toolProgress_OnPostProgress;
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            this.ReportProgress(progress, info);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 开始
        /// </summary>
        protected override void OnGo()
        {
            bool canContinue = CanContinue();
            if (!canContinue)
            {
                return;
            }
            this.ReportProgress(1, null);
            try
            {
                ImportDotInformation();
                this.ReportProgress(100, null);
            }
            catch (System.Exception ex)
            {
                this.ReportAlert(eMessageGrade.Exception, null, "初始化数据时出错,可能是无数据文件!" + ex.Message);
            }
        }

        /// <summary>
        /// 是否可继续
        /// </summary>
        private bool CanContinue()
        {
            bool canContinue = true;
            TaskImportBoundarytArgument boundaryArg = Argument as TaskImportBoundarytArgument;
            if (boundaryArg == null)
                canContinue = false;
            else
            {
                currentZone = boundaryArg.CurrentZone;
                dbContext = boundaryArg.Dbcontext;
                landorType = boundaryArg.LandorType;
                fileName = boundaryArg.FileName;
                dotTypeList = boundaryArg.DicList.FindAll(t => t.Code != "" && t.GroupCode == DictionaryTypeInfo.JZDLX);
                markTypeList = boundaryArg.DicList.FindAll(t => t.Code != "" && t.GroupCode == DictionaryTypeInfo.JBLX);
                lineTypeList = boundaryArg.DicList.FindAll(t => t.Code != "" && t.GroupCode == DictionaryTypeInfo.JZXLB);
                linePropertyList = boundaryArg.DicList.FindAll(t => t.Code != "" && t.GroupCode == DictionaryTypeInfo.JXXZ);
                linePositionList = boundaryArg.DicList.FindAll(t => t.Code != "" && t.GroupCode == DictionaryTypeInfo.JZXWZ);
            }
            if (currentZone == null || dbContext == null || string.IsNullOrEmpty(fileName))
                canContinue = false;
            return canContinue;
        }

        /// <summary>
        /// 导入界址调查信息
        /// </summary>
        private void ImportDotInformation()
        {
            this.ReportProgress(1, "正在读取数据");
            this.ReportInfomation("正在读取数据...");
            ImportDotSurveyTable import = new ImportDotSurveyTable();
            var targetSpatialReference = dbContext.CreateSchema().GetElementSpatialReference(
                 ObjectContext.Create(typeof(Zone)).Schema,
                 ObjectContext.Create(typeof(Zone)).TableName);
            import.Srid = targetSpatialReference.WKID;
            import.FileName = fileName;
            import.LandorType = landorType;
            import.DotTypeList = dotTypeList;
            import.MarkTypeList = markTypeList;
            import.LinePositionList = linePositionList;
            import.LinePropertyList = linePropertyList;
            import.LineTypeList = lineTypeList;
            import.Read();
            bool canContinue = import.ReadInformation();
            if (!canContinue)
            {
                foreach (string information in import.ErrorCollection)
                {
                    this.ReportAlert(eMessageGrade.Error, null, information);
                }
                import.ErrorCollection.Clear();
                return;
            }
            this.ReportProgress(5, "开始导入...");
            this.ReportInfomation("开始导入...");
            string landType = InitalziePropertyType();
            var coilDb = dbContext.CreateBoundaryAddressCoilWorkStation();
            var dotDb = dbContext.CreateBoundaryAddressDotWorkStation();
            dotDb.DeleteByZoneCode(currentZone.FullCode, eSearchOption.Precision, landType);
            coilDb.DeleteByZoneCode(currentZone.FullCode, eSearchOption.Precision, landType);
            int dotCount = import.DotCollection.Count;
            int lineCount = import.LineCollection.Count;
            try
            {
                switch (landorType)
                {
                    case LanderType.AgricultureLand:
                        AddAgricultureDotInformation(import);
                        break;
                    case LanderType.CollectiveLand:
                        AddCollectiveLandDotInformation(import);
                        break;
                    case LanderType.ConstructionLand:
                        AddConstructionLandDotInformation(import);
                        break;
                    case LanderType.HomeSteadLand:
                        AddHomeSteadLandDotInformation(import);
                        break;
                    case LanderType.WoodLand:
                        AddWoodLandDotInformation(import);
                        break;
                    case LanderType.Irrigation:
                        AddIrrigationDotInformation(import);
                        break;
                    default:
                        break;
                }
                string totalInformation = string.Format("共处理数据{0}宗、界址点{1}个、界址线{2}条!", import.LandCount, dotCount, lineCount);
                this.ReportAlert(eMessageGrade.Infomation, null, totalInformation);
                totalInformation = string.Format("成功处理数据{0}宗、导入界址点{1}个、导入界址线{2}条!", import.LandCountHandle, import.DotCount, import.LineCount);
                this.ReportAlert(eMessageGrade.Infomation, null, totalInformation);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            finally
            {
                import.Dispose();
            }
        }

        #endregion

        #region AgricultureLand

        /// <summary>
        /// 添加承包地界址信息
        /// </summary>
        /// <param name="import"></param>
        private void AddAgricultureDotInformation(ImportDotSurveyTable import)
        {
            var landStation = dbContext.CreateContractLandWorkstation();
            List<ContractLand> landCollection = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
            Func<ContractLand, BuildLandBoundaryAddressDot, bool> serchdotLambda = (land, dot) =>
            {
                bool result = land.LandNumber == dot.LandNumber;
                return result;
            };
            Func<ContractLand, BuildLandBoundaryAddressCoil, bool> serchCoilLambda = (land, line) =>
            {
                bool result = land.LandNumber == line.LandNumber;
                return result;
            };
            string landType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
            string errorDot = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            string errorCoil = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            try
            {
                dbContext.BeginTransaction();
                AddDataInformation(landCollection, import, landType, serchdotLambda, serchCoilLambda, errorDot, errorCoil);
                dbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                Log.Log.WriteError(this, "AddAgricultureDotInformation", ex.Message + ex.StackTrace);
                this.ReportError("导入界址调查表发生错误");
            }
            landCollection = null;
            GC.Collect();
        }

        #endregion

        #region CollectiveLand

        /// <summary>
        /// 添加界址信息
        /// </summary>
        /// <param name="import"></param>
        private void AddCollectiveLandDotInformation(ImportDotSurveyTable import)
        {
            //var landStation = dbContext.CreateContractLandWorkstation();
            //List<CollectiveLand> landCollection = landStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
            //Func<ContractLand, BuildLandBoundaryAddressDot, bool> serchdotLambda = (ld, dot) => ContractLand.GetLandNumber(ld.CadastralNumber) == dot.LandNumber;
            //Func<ContractLand, BuildLandBoundaryAddressCoil, bool> serchCoilLambda = (ld, line) => ContractLand.GetLandNumber(ld.CadastralNumber) == line.LandNumber;
            //string landType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
            //string errorDot = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //string errorCoil = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //AddDataInformation(landCollection, import, landType, serchdotLambda, serchCoilLambda, errorDot, errorCoil);
            //landCollection = null;
            GC.Collect();
        }

        #endregion

        #region ConstructionLand

        /// <summary>
        /// 添加界址信息
        /// </summary>
        /// <param name="import"></param>
        private void AddConstructionLandDotInformation(ImportDotSurveyTable import)
        {
            //var landStation = dbContext.CreateContractLandWorkstation();
            //List<CollectiveLand> landCollection = landStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
            //Func<ContractLand, BuildLandBoundaryAddressDot, bool> serchdotLambda = (ld, dot) => ContractLand.GetLandNumber(ld.CadastralNumber) == dot.LandNumber;
            //Func<ContractLand, BuildLandBoundaryAddressCoil, bool> serchCoilLambda = (ld, line) => ContractLand.GetLandNumber(ld.CadastralNumber) == line.LandNumber;
            //string landType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
            //string errorDot = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //string errorCoil = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //AddDataInformation(landCollection, import, landType, serchdotLambda, serchCoilLambda, errorDot, errorCoil);
            //landCollection = null;
            GC.Collect();
        }

        #endregion

        #region HomeStead

        /// <summary>
        /// 添加界址信息
        /// </summary>
        /// <param name="import"></param>
        private void AddHomeSteadLandDotInformation(ImportDotSurveyTable import)
        {
            //var landStation = dbContext.CreateContractLandWorkstation();
            //List<CollectiveLand> landCollection = landStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
            //Func<ContractLand, BuildLandBoundaryAddressDot, bool> serchdotLambda = (ld, dot) => ContractLand.GetLandNumber(ld.CadastralNumber) == dot.LandNumber;
            //Func<ContractLand, BuildLandBoundaryAddressCoil, bool> serchCoilLambda = (ld, line) => ContractLand.GetLandNumber(ld.CadastralNumber) == line.LandNumber;
            //string landType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
            //string errorDot = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //string errorCoil = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //AddDataInformation(landCollection, import, landType, serchdotLambda, serchCoilLambda, errorDot, errorCoil);
            //landCollection = null;
            GC.Collect();
        }

        #endregion

        #region WoodLand

        /// <summary>
        /// 添加界址信息
        /// </summary>
        /// <param name="import"></param>
        private void AddWoodLandDotInformation(ImportDotSurveyTable import)
        {
            //var landStation = dbContext.CreateContractLandWorkstation();
            //List<CollectiveLand> landCollection = landStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
            //Func<CollectiveLand, BuildLandBoundaryAddressDot, bool> serchdotLambda = (ld, dot) => ContractLand.GetLandNumber(ld.LandNumber) == dot.LandNumber;
            //Func<CollectiveLand, BuildLandBoundaryAddressCoil, bool> serchCoilLambda = (ld, line) => ContractLand.GetLandNumber(ld.LandNumber) == line.LandNumber;
            //string landType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
            //string errorDot = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //string errorCoil = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //AddDataInformation(landCollection, import, landType, serchdotLambda, serchCoilLambda, errorDot, errorCoil);
            //landCollection = null;
            //GC.Collect();
        }

        #endregion

        #region Irrigation

        /// <summary>
        /// 添加界址信息
        /// </summary>
        /// <param name="import"></param>
        private void AddIrrigationDotInformation(ImportDotSurveyTable import)
        {
            //var landStation = dbContext.CreateContractLandWorkstation();
            //List<CollectiveLand> landCollection = landStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
            //Func<ContractLand, BuildLandBoundaryAddressDot, bool> serchdotLambda = (ld, dot) => ContractLand.GetLandNumber(ld.CadastralNumber) == dot.LandNumber;
            //Func<ContractLand, BuildLandBoundaryAddressCoil, bool> serchCoilLambda = (ld, line) => ContractLand.GetLandNumber(ld.CadastralNumber) == line.LandNumber;
            //string landType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
            //string errorDot = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //string errorCoil = "不存在地块编码为{0}的地块,已略过点号为{1}的数据!";
            //AddDataInformation(landCollection, import, landType, serchdotLambda, serchCoilLambda, errorDot, errorCoil);
            //landCollection = null;
            GC.Collect();
        }

        #endregion

        #region 处理界址信息

        /// <summary>
        /// 添加界址信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="landCollection">地块集合</param>
        /// <param name="import">导入数据</param>
        /// <param name="dotPredicate">条件</param>
        /// <param name="landType">土地类型</param>
        /// <param name="dotFunc">查找界址点表达式</param>
        /// <param name="coilFunc">查找界址线表达式</param>
        /// <param name="errorDot">界址点错误信息</param>
        /// <param name="errorCoil">界址线错误信息</param>
        private void AddDataInformation<T>(List<T> landCollection, ImportDotSurveyTable import, string landType, Func<T, BuildLandBoundaryAddressDot, bool> dotPredicate,
            Func<T, BuildLandBoundaryAddressCoil, bool> coilPredicate, string errorDot, string errorCoil)
        {
            if (landCollection == null || dbContext == null)
            {
                return;
            }
            List<BuildLandBoundaryAddressDot> dotCollection = import.DotCollection;
            List<BuildLandBoundaryAddressCoil> lineCollection = import.LineCollection;
            dotCollection = InitalizeDotCollection(landCollection, dotCollection, dotPredicate, errorDot);
            import.DotCollection = dotCollection.Clone() as List<BuildLandBoundaryAddressDot>;
            lineCollection = InitalizeLineCollection(landCollection, lineCollection, dotCollection, coilPredicate, errorCoil);
            import.LineCollection = lineCollection.Clone() as List<BuildLandBoundaryAddressCoil>;
            toolProgress.InitializationPercent(dotCollection.Count, 95);
            List<BuildLandBoundaryAddressDot> dots = new List<BuildLandBoundaryAddressDot>();
            var dotWorkStation = dbContext.CreateBoundaryAddressDotWorkStation();
            var coilWorkStation = dbContext.CreateBoundaryAddressCoilWorkStation();
            int landCount = 0;
            foreach (BuildLandBoundaryAddressCoil line in lineCollection)
            {
                BuildLandBoundaryAddressDot startPoint = dotCollection.Find(dt => dt.ID == line.StartPointID);
                BuildLandBoundaryAddressDot endPoint = dotCollection.Find(dt => dt.ID == line.EndPointID);
                if (startPoint == null || endPoint == null)
                {
                    continue;
                }
                T entity = landCollection.Find(c => (bool)dotPredicate.DynamicInvoke(c, startPoint));
                if (entity == null)
                {
                    landCount++;
                    continue;
                }
                Guid landid = (Guid)ObjectExtension.GetPropertyValue(entity, "ID");
                line.LandID = landid;
                line.ZoneCode = currentZone.FullCode;
                line.LandType = landType;
                line.Shape = ToolGeometry.CreateLineByPoint(new List<Spatial.Geometry>() { startPoint.Shape, endPoint.Shape }, startPoint.Shape.Srid);
                coilWorkStation.Add(line);//界址线入库db.BuildLandBoundaryAddressCoil.Add(line);//
                import.LineCount++;
                AddDotData(import, dots, startPoint, dotWorkStation, landid, landType);
                AddDotData(import, dots, endPoint, dotWorkStation, landid, landType);
                toolProgress.DynamicProgress(string.Format("正在导入{0}的界址信息", currentZone.Name));
            }
            import.LandCountHandle = landCollection.Count - landCount;
            landCollection = null;
            dotCollection = null;
            lineCollection = null;
            dots = null;
            GC.Collect();
        }

        /// <summary>
        /// 添加界址点数据入库
        /// </summary>
        private void AddDotData(ImportDotSurveyTable import, List<BuildLandBoundaryAddressDot> dots, BuildLandBoundaryAddressDot addressDot,
            IBuildLandBoundaryAddressDotWorkStation dotWorkStation, Guid landid, string landType)
        {
            addressDot.ZoneCode = currentZone.FullCode;
            addressDot.LandID = landid;
            addressDot.LandType = landType;
            if (!dots.Exists(dt => dt.ID == addressDot.ID))
            {
                GeoAPI.Geometries.IPoint point = addressDot.Shape.Instance as GeoAPI.Geometries.IPoint;
                addressDot.Description = point.X.ToString() + "," + point.Y.ToString();
                dotWorkStation.Add(addressDot);//界址点入库 db.BuildLandBoundaryAddressDot.Add(startPoint);//
                import.DotCount++;
                dots.Add(addressDot);
            }
        }

        /// <summary>
        /// 初始化有效界址点信息
        /// </summary>
        private List<BuildLandBoundaryAddressDot> InitalizeDotCollection<T>(List<T> landCollection, List<BuildLandBoundaryAddressDot> dotCollection,
            Func<T, BuildLandBoundaryAddressDot, bool> dotPredicate, string errorInfo)
        {
            List<BuildLandBoundaryAddressDot> dots = new List<BuildLandBoundaryAddressDot>();
            foreach (BuildLandBoundaryAddressDot dot in dotCollection)
            {
                //landCollection.Find(ld => ContractLand.GetLandNumber(ld.CadastralNumber) == dot.LandNumber);
                T t = landCollection.Find(c => (bool)dotPredicate.DynamicInvoke(c, dot));
                if (t == null)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, string.Format(errorInfo, dot.LandNumber, dot.DotNumber));//string.Format("不存在工程编号为{0}的工程,已略过点号为{1}的数据!", dot.LandNumber, dot.DotNumber));
                    continue;
                }
                dots.Add(dot);
            }
            return dots;
        }

        /// <summary>
        /// 初始化有效界址线信息
        /// </summary>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineCollection<T>(List<T> landCollection, List<BuildLandBoundaryAddressCoil> lineCollection,
            List<BuildLandBoundaryAddressDot> dotCollection, Func<T, BuildLandBoundaryAddressCoil, bool> coilPredicate, string errorInfo)
        {
            List<BuildLandBoundaryAddressCoil> lines = new List<BuildLandBoundaryAddressCoil>();
            foreach (BuildLandBoundaryAddressCoil line in lineCollection)
            {
                BuildLandBoundaryAddressDot startPoint = dotCollection.Find(dt => dt.ID == line.StartPointID);
                BuildLandBoundaryAddressDot endPoint = dotCollection.Find(dt => dt.ID == line.EndPointID);
                if (startPoint == null || endPoint == null)
                {
                    continue;
                }
                T t = landCollection.Find(c => (bool)coilPredicate.DynamicInvoke(c, line));
                if (t == null)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, string.Format(errorInfo, startPoint.LandNumber, startPoint.DotNumber));//string.Format("不存在地块编码为{0}的地块,已略过点号为{1}的数据!", startPoint.LandNumber, startPoint.DotNumber));
                    continue;
                }
                lines.Add(line);
            }
            return lines;
        }

        #endregion

        #region Helper

        /// <summary>
        /// 初始化权属类型
        /// </summary>
        private string InitalziePropertyType()
        {
            eLandPropertyRightType landType = eLandPropertyRightType.Other;
            switch (landorType)
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
            return ((int)landType).ToString();
        }

        #endregion
    }
}
