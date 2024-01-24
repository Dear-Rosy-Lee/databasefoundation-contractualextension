/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;
using YuLinTu.Windows.Wpf;
using YuLinTu.Diagrams;
using System.Windows;
using YuLinTu.NetAux.CglLib;
using YuLinTu.NetAux;
using YuLinTu.Library.Business;


namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    ///  农村土地承包经营权标准地块示意图
    /// </summary>
    public class ExportContractLandParcelWordLZ : AgricultureWordBook
    {
        #region Fields

        private List<Dictionary> dictDKLB;    //地块类别数据字典集合
        private bool isSingleLand;//是否单宗地
        private List<ContractLand> geoLandCollection;  //空间地块集合
        private int fromthirdPageTableCount;//从第三页开始的表个数，包括第三页

        #endregion
        class MyElements
        {
            private readonly List<DiagramBase> _lstElements = new List<DiagramBase>();
            private CglEnvelope _visibleBounds;
            public MyElements(CglEnvelope visibleBounds)
            {
                _visibleBounds = visibleBounds;
            }
            public void Clear()
            {
                _lstElements.Clear();
            }
            public void AddElement(DiagramBase element)
            {
                if (_lstElements.Count == 0)
                {
                    _lstElements.Add(element);
                    element.Background = new SolidColorBrush(Colors.Beige);
                    return;
                }
                var m = element.Model;
                double wi = m.Width * 0.75;// -m.Width * 15.0 / 48;
                foreach (var e in _lstElements)
                {
                    var ie = intersect(e, element);
                    if (ie == null)
                        continue;
                    var x = (CglEnvelope)ie;
                    if (true)
                    {
                        var deltaUp = m.Y - (e.Model.Y - m.Height);
                        var deltaDown = e.Model.Y + e.Model.Height - m.Y;
                        var deltaLeft = m.X - (e.Model.X - wi);// m.Width);
                        var deltaRight = e.Model.X + e.Model.Width - m.X;
                        var sa = new double[] { deltaUp, deltaDown, deltaLeft, deltaRight };
                        int i = 0;
                        double minDelta = sa[0];
                        for (int j = 1; j < sa.Length; ++j)
                        {
                            if (sa[j] < minDelta)
                            {
                                minDelta = sa[j];
                                i = j;
                            }
                        }
                        if (i == 0)
                        {
                            m.Y = e.Model.Y - m.Height;
                        }
                        else if (i == 1)
                        {
                            m.Y = e.Model.Y + e.Model.Height;
                        }
                        else if (i == 2)
                        {
                            m.X = e.Model.X - wi;// m.Width;
                        }
                        else
                        {
                            m.X = e.Model.X + e.Model.Width;
                        }
                        if (m.X < 0)
                            m.X = 0;
                        if (m.Y < 0)
                            m.Y = 0;
                    }
                    else
                    {
                    }
                }
                _lstElements.Add(element);
            }
            private static CglEnvelope? intersect(DiagramBase a, DiagramBase b)
            {
                double wi = a.Model.Width - a.Model.Width * 15.0 / 48;
                var ae = new CglEnvelope(a.Model.X, a.Model.Y, a.Model.X + wi, a.Model.Y + a.Model.Height);
                wi = b.Model.Width - b.Model.Width * 15.0 / 48;
                var be = new CglEnvelope(b.Model.X, b.Model.Y, b.Model.X + wi, b.Model.Y + b.Model.Height);
                //var ae = new CglEnvelope(a.Model.X, a.Model.Y, a.Model.X + a.Model.Width, a.Model.Y + a.Model.Height);
                //var be = new CglEnvelope(b.Model.X, b.Model.Y, b.Model.X + b.Model.Width, b.Model.Y + b.Model.Height);
                if (!ae.Intersects(be))
                    return null;

                double left = Math.Max(ae.MinX, be.MinX);
                double right = Math.Min(ae.MaxX, be.MaxX);
                double top = Math.Max(ae.MinY, be.MinY);
                double bottom = Math.Min(ae.MaxY, be.MaxY);
                return new CglEnvelope(left, top, right, bottom);
            }
            private static CglEnvelope calcRealEnvelope(DiagramBase d)
            {
                var e = new CglEnvelope();
                //bool f
                //(diagram.Model as TextShape).HorizontalAlignment = HorizontalAlignment.Center;
                return e;
            }
        }

        #region Field - Const

        private const double mapW = 136;
        private const double mapH = 120;

        #endregion

        #region Properties

        /// <summary>
        /// Word文档保存路径
        /// </summary>
        public string SavePathOfWord { get; set; }

        /// <summary>
        /// 宗地图文件保存路径
        /// </summary>
        public string SavePathOfImage { get; set; }

        /// <summary>
        /// 合同集合(当前地域)
        /// </summary>
        public List<ContractConcord> ListConcord { get; set; }

        /// <summary>
        /// 权证集合(当前地域)
        /// </summary>
        public List<YuLinTu.Library.Entity.ContractRegeditBook> ListBook { get; set; }

        /// <summary>
        /// 发包方集合(当前地域)
        /// </summary>
        public List<CollectivityTissue> ListTissue { get; set; }

        /// <summary>
        /// 空间地块集合(当前地域)
        /// </summary>
        public List<ContractLand> ListGeoLand { get; set; }

        /// <summary>
        /// 线状地物(当前地域)
        /// </summary>
        public List<XZDW> ListLineFeature { get; set; }

        /// <summary>
        /// 数据字典集合(所有)
        /// </summary>
        public List<Dictionary> DictDKLB
        {
            get { return dictDKLB; }
            set
            {
                dictDKLB = value;
            }
        }

        /// <summary>
        /// 界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListDot { get; set; }

        /// <summary>
        /// 界址线集合
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> ListCoil { get; set; }

        /// <summary>
        /// 有效界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListValidDot { get; set; }

        public DiagramsView ViewOfAllMultiParcel { get; set; }

        public DiagramsView ViewOfNeighorParcels { get; set; }

        public ContractBusinessParcelWordSettingDefine SettingDefine { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportContractLandParcelWordLZ(IDbContext db)
        {
            DbContext = db;
            DictDKLB = new List<Dictionary>();
        }

        #endregion

        #region Methods

        #region Override

        /// <summary>
        /// 填写数据
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            if (data == null || DbContext == null)
            {
                return false;
            }
            try
            {
                if (!InitalizeDataInformation(data))
                {
                    return false;
                }
                base.OnSetParamValue(data);
                GetAllMultiParcel();                  //生成全局图
                GetNeighorParcels();                  //生成地块图
                InitalizeAllEngleView();
                AgricultureStrandardLandProgress();

            }
            catch (SystemException ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSetParamValue(导出宗地图失败)", ex.Message + ex.StackTrace);

                return false;
            }
            return true;
        }

        /// <summary>
        /// 注销
        /// </summary>
        protected override void Destroyed()
        {
            base.Destroyed();
            ListGeoLand.Clear();
            ListGeoLand = null;
            ListConcord.Clear();
            ListConcord = null;
            ListBook.Clear();
            ListBook = null;
            GC.Collect();
        }

        #endregion

        #region ContractLand

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="data">承包方</param> 
        private bool InitalizeDataInformation(object data)
        {
            Contractor = data as VirtualPerson;
            List<ContractConcord> concords = Contractor != null ? ListConcord.FindAll(c => c.ContracterId == Contractor.ID) : new List<ContractConcord>();   //合同
            Concord = concords.Find(cd => cd.ArableLandType == "110");  //家庭承包方式
            Book = Concord != null ? ListBook.Find(c => c.ID == Concord.ID) : null;   //权证
            Tissue = Concord != null ? ListTissue.Find(c => c.ID == Concord.SenderId) : null;  //发包方
            geoLandCollection = Contractor != null ? ListGeoLand.FindAll(c => c.OwnerId == Contractor.ID) : new List<ContractLand>();//得到地块集合
            List<ContractLand> lands = new List<ContractLand>();
            foreach (ContractLand land in geoLandCollection)
            {
                if (dictDKLB.Any(c => !string.IsNullOrEmpty(c.Code) && c.Code.Equals(land.LandCategory)))
                {
                    continue;
                }
                lands.Add(land);
            }
            foreach (ContractLand land in lands)
            {
                geoLandCollection.Remove(land);
            }
            InitalizeAgricultureValue();
            isSingleLand = false;
            return true;
        }

        /// <summary>
        /// 初始化地块列表
        /// </summary>
        private void InitalizeAgricultureValue()
        {
            if (geoLandCollection == null || geoLandCollection.Count == 0)
            {
                DeleteTable(1);
                return;
            }
            if (geoLandCollection.Count <= 6)
            {
                DeleteTable(1);
                DeleteParagraph();

            }
            if (AgricultureSetting.AgricultureLandWordLandSortCatalog)
            {
                //是否按照地块类别进行排序
                InitalizeAgricultureLandSortValue();
                return;
            }
            List<ContractLand> landArray = InitalizeAgricultureLandPosition(geoLandCollection);
            if (SettingDefine.IsLandNumberStart)
            {
                if (SettingDefine.LandNumberIndex > 1)
                {
                    int index = (int)(SettingDefine.LandNumberIndex);
                    if (landArray.Count >= index)
                    {
                        landArray.RemoveRange(0, index - 1);
                    }
                }
            }
            geoLandCollection.Clear();
            LandCollection = new List<ContractLand>();
            foreach (var land in landArray)
            {
                geoLandCollection.Add(land);
                LandCollection.Add(land);
            }
            landArray.Clear();
        }

        /// <summary>
        /// 初始化农用地值
        /// </summary>
        private void InitalizeAgricultureLandSortValue()
        {
            List<ContractLand> lands = new List<ContractLand>();
            List<ContractLand> landValue = new List<ContractLand>();
            foreach (var dict in dictDKLB)
            {
                landValue = InitalizeAgricultureLandPosition(LandCollection.FindAll(ld => ld.LandCategory == dict.Code));
                foreach (var land in landValue)
                {
                    lands.Add(land);
                }
            }
            LandCollection.Clear();
            foreach (var land in lands)
            {
                LandCollection.Add(land);
            }
            lands.Clear();
        }

        /// <summary>
        /// 初始化地块的排序位置
        /// </summary>
        private List<ContractLand> InitalizeAgricultureLandPosition(List<ContractLand> lands)
        {
            int landType = AgricultureSetting.AgricultureLandWordLandSortType;
            List<ContractLand> landArray = new List<ContractLand>();
            switch (landType)
            {
                case 1:
                    var numberOrder = lands.OrderBy(ld =>
                    {
                        string landNumber = ContractLand.GetLandNumber(ld.LandNumber);
                        landNumber = landNumber.Length > 5 ? landNumber.Substring(landNumber.Length - 5) : landNumber;
                        return landNumber;
                    });
                    foreach (var land in numberOrder)
                    {
                        landArray.Add(land);
                    }
                    break;
                case 2:
                    var nameOrder = lands.OrderBy(ld => ld.Name);
                    foreach (var land in nameOrder)
                    {
                        landArray.Add(land);
                    }
                    break;
                case 3:
                    var areaOrder = lands.OrderBy(ld => ld.ActualArea);
                    foreach (var land in areaOrder)
                    {
                        landArray.Add(land);
                    }
                    break;
                default:
                    break;
            }
            return landArray;
        }


        /// <summary>
        /// 书写地块书签信息
        /// </summary>
        private void WriteLandBookMark()
        {
            if (LandCollection == null || LandCollection.Count < 1)
            {
                return;
            }
            try
            {
                for (int i = 0; i < LandCollection.Count; i++)
                {
                    ContractLand land = LandCollection[i];
                    string landString = AgricultureBookMark.AgricultureShape + (i + 1).ToString();
                    InsertImageShape(land, landString);
                    landString = InitalizeLandInformation(land);
                    SetBookmarkValue(AgricultureBookMark.AgricultureString + (i + 1).ToString(), landString);
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 插入文件
        /// </summary>
        private void InsertImageShape(ContractLand land, string bookMark)
        {
            try
            {
                string imagePath = SavePathOfImage + @"\" + land.LandNumber + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    InsertImageCellWithoutPading(bookMark, imagePath, AgricultureSetting.AgricultureLandWordLandWidth, AgricultureSetting.AgricultureLandWordLandHeight);
                }
                System.IO.File.Delete(imagePath);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 插入文件
        /// </summary>
        private void InsertImageShape(ContractLand land, int rowIndex, int columnIndex, int tableIndex = 0)
        {
            try
            {
                string imagePath = SavePathOfImage + @"\" + land.LandNumber + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    SetTableCellValue(tableIndex, rowIndex, columnIndex, imagePath, 100, 100, false);
                }
                System.IO.File.Delete(imagePath);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

        }

        /// <summary>
        /// 插入文件
        /// </summary>
        private void InsertImageShapeBig(ContractLand land, int rowIndex, int columnIndex, int tableIndex = 0)
        {
            try
            {
                string imagePath = SavePathOfImage + @"\" + land.LandNumber + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    SetTableCellValue(tableIndex, rowIndex, columnIndex, imagePath, 150, 150, false);
                }
                System.IO.File.Delete(imagePath);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

        }

        /// <summary>
        /// 插入文件
        /// </summary>
        private void InsertImageShapeFirstPage(ContractLand land, int rowIndex, int columnIndex, int tableIndex = 0)
        {
            try
            {
                string imagePath = SavePathOfImage + @"\" + land.LandNumber + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    SetTableCellValue(tableIndex, rowIndex, columnIndex, imagePath, 120, 120, false);
                }
                System.IO.File.Delete(imagePath);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

        }
        /// <summary>
        /// 初始化地块信息
        /// </summary>
        private string InitalizeLandInformation(ContractLand land)
        {
            string landString = "";
            if (AgricultureSetting.AgricultureLandWordShowLandNumber && !AgricultureSetting.AgricultureLandWordLandLabel)
            {
                string landNumber = ContractLand.GetLandNumber(land.LandNumber);
                if (landNumber.Length > AgricultureSetting.AgricultureLandWordLandNumberMedian)
                {
                    landNumber = landNumber.Substring(AgricultureSetting.AgricultureLandWordLandNumberMedian);
                }
                landString += AgricultureSetting.AgricultureLandWordLandNumberMedian == 0 ? "地块编码:" : "地块缩编码:";
                landString += landNumber;
            }
            if (AgricultureSetting.AgricultureLandWordShowLandName)
            {
                landString += (!string.IsNullOrEmpty(landString)) ? "\n" : "";
                landString += "地块名称:";
                landString += land.Name;
            }
            if (AgricultureSetting.AgricultureLandWordShowTableArea)
            {
                string tableAreaString = (land.TableArea != null && land.TableArea.HasValue) ? (land.TableArea.Value > 0.0 ? ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2) : "") : "";
                tableAreaString = (land.TableArea != null && land.TableArea.HasValue && land.TableArea.Value == 0.0) ? (WriteNullArea ? "    " : "0.00") : tableAreaString;
                landString += (!string.IsNullOrEmpty(landString)) ? "\n" : "";
                if (string.IsNullOrEmpty(AgricultureSetting.AgricultureLandWordTableAreaAliseName))
                {
                    landString += "二轮合同面积:";
                }
                else
                {
                    landString += AgricultureSetting.AgricultureLandWordTableAreaAliseName + ":";
                }
                landString += tableAreaString;
                landString += "亩";
            }
            if (AgricultureSetting.AgricultureLandWordShowActualArea && !AgricultureSetting.AgricultureLandWordLandLabel)
            {
                string actualAreaString = land.ActualArea > 0.0 ? ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2) : "";
                actualAreaString = land.ActualArea == 0.0 ? (WriteNullArea ? "    " : "0.00") : actualAreaString;
                landString += (!string.IsNullOrEmpty(landString)) ? "\n" : "";
                landString += "实测面积:";
                landString += actualAreaString;
                landString += "亩";
            }
            if (AgricultureSetting.AgricultureLandWordShowAwareArea)
            {
                string awareAreaString = land.AwareArea > 0.0 ? ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2) : "";
                awareAreaString = land.AwareArea == 0.0 ? (WriteNullArea ? "    " : "0.00") : awareAreaString;
                landString += (!string.IsNullOrEmpty(landString)) ? "\n" : "";
                landString += "确权面积:";
                landString += awareAreaString;
                landString += "亩";
            }
            if (AgricultureSetting.AgricultureLandWordLandNeighbor)
            {
                landString += InitalizeLandNeighbor(land);
            }
            if (AgricultureSetting.AgricultureLandWordLandCatalog)
            {
                landString += (!string.IsNullOrEmpty(landString)) ? "\n" : "";
                landString += "地块类别:";
                landString += EnumNameAttribute.GetDescription(land.LandCategory);
            }
            return landString;
        }

        /// <summary>
        /// 初始化地块四至信息
        /// </summary>
        private string InitalizeLandNeighbor(ContractLand land, string extend = "")
        {
            string landString = string.Empty;
            landString += "\n";
            landString += "东" + extend + ":";
            landString += string.IsNullOrEmpty(land.NeighborEast) ? "      " : land.NeighborEast;

            landString += AgricultureSetting.AgricultureLandWordLandNeighborMode ? " " : "\n";
            landString += "南" + extend + ":";
            landString += string.IsNullOrEmpty(land.NeighborSouth) ? "      " : land.NeighborSouth;

            landString += "\n";
            landString += "西" + extend + ":";
            landString += string.IsNullOrEmpty(land.NeighborWest) ? "      " : land.NeighborWest;

            landString += AgricultureSetting.AgricultureLandWordLandNeighborMode ? " " : "\n";
            landString += "北" + extend + ":";
            landString += string.IsNullOrEmpty(land.NeighborNorth) ? "      " : land.NeighborNorth;

            return landString;
        }

        ///// <summary>
        ///// 填写承包地块信息
        ///// </summary>
        //private void WriteLandInformation()
        //{
        //    if (LandCollection == null || LandCollection.Count < 1)
        //    {
        //        return;
        //    }
        //    InsertTableRowValue();
        //    InsertLandImageInformation(4, 0, 0, LandCollection.Count, false);
        //    string fileName = SavePathOfImage + Contractor.ZoneCode + Contractor.Name + ".jpg";
        //    if (System.IO.File.Exists(fileName))
        //    {
        //        SetTableCellValue(0, 1, 1, fileName, AgricultureSetting.AgricultureLandWordLandAllDataWidth, AgricultureSetting.AgricultureLandWordLandAllDataHeight, !AgricultureSetting.AgricultureLandWordEngleScapeFiexed);
        //    }
        //    System.IO.File.Delete(fileName);
        //}

        ///// <summary>
        ///// 插入表格
        ///// </summary>
        //private void InsertTableRowValue()
        //{
        //    if (LandCollection == null || LandCollection.Count < 1)
        //    {
        //        return;
        //    }
        //    int landCount = AgricultureSetting.AgricultureLandWordLandPageNumber;
        //    int rowCount = LandCollection.Count - AgricultureSetting.AgricultureLandWordLandRowCount * 2;
        //    if (rowCount <= 0)
        //    {
        //        return;
        //    }
        //    rowCount = rowCount % 2 == 0 ? rowCount : ++rowCount;
        //    rowCount /= 2;
        //    int rowValue = rowCount / landCount;
        //    int num = rowCount % landCount;
        //    rowCount = num == 0 ? rowValue : ++rowValue;
        //    rowCount = rowCount == 0 ? 1 : rowCount;
        //    rowCount = rowCount * landCount;
        //    InsertTableCell(0, rowCount);
        //}

        ///// <summary>
        ///// 插入地块影像信息
        ///// </summary>  
        //private int InsertLandImageInformation(int rowIndex, int tableIndex, int startInex, int endInex, bool isLaiZhouData)
        //{
        //    for (int i = startInex; i < endInex; i += 2)
        //    {
        //        ContractLand land = LandCollection[i];
        //        InsertImageShape(land, rowIndex, 0, tableIndex);
        //        string landString = InitalizeLandInformation(land);
        //        SetTableCellValue(tableIndex, rowIndex, 1, landString);
        //        if (i + 1 < endInex)
        //        {
        //            land = LandCollection[i + 1];
        //            InsertImageShape(land, rowIndex, 2, tableIndex);
        //            landString = InitalizeLandInformation(land);
        //            SetTableCellValue(tableIndex, rowIndex, 3, landString);
        //        }
        //        rowIndex++;
        //    }
        //    return rowIndex;
        //}

        #endregion

        #region Strandard

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        private void AgricultureStrandardLandProgress()
        {
            if (geoLandCollection == null || geoLandCollection.Count < 1)
            {
                return;
            }
            AddCartographyInfo();
            InitalizeLandRowInformation();
            InitalizeLandBookMarkInformation();
            //InitalizeLandImageInformation();
        }

        /// <summary>
        /// 初始化地块行信息
        /// </summary>
        private void InitalizeLandRowInformation()
        {
            if (geoLandCollection == null || geoLandCollection.Count == 0)
            {
                return;
            }
            if (geoLandCollection != null && geoLandCollection.Count <= 9)
            {
                SetTableCellValue(0, 7, 1, "1/1");
                DeleteTable(1);
                DeleteParagraph();
                return;
            }
            int landCount = geoLandCollection.Count - 9;
            int pageCount = landCount / 9;
            pageCount = landCount % 9 == 0 ? pageCount : (pageCount + 1);

            fromthirdPageTableCount = pageCount ;
            int totalPage = fromthirdPageTableCount + 1;//总页数

            //从第三页后添加28的表格
            if (totalPage > 1)
            {
                for (int i = 0; i < fromthirdPageTableCount ; i++)
                {
                    AddTable(0);
                }
            }

            SetTableCellValue(0, 7, 1, "1/" + totalPage.ToString());
            for (int index = 1; index < fromthirdPageTableCount + 1; index++)
            {
                SetTableCellValue(index, 7, 1, (index + 1).ToString() + "/" + totalPage.ToString());
            }
        }

        /// <summary>
        /// 插入地块影像信息
        /// </summary> 
        private void InitalizeLandBookMarkInformation()
        {
            if (geoLandCollection == null || geoLandCollection.Count == 0)
            {
                return;
            }
            int landIndex = geoLandCollection.Count;
            ContractLand land = new ContractLand();

            for (int i = 0; i < landIndex && i < 9; i++)
            {
                switch (i)
                {
                    case 0:
                        land = geoLandCollection[0];
                        InsertImageShapeFirstPage(land, 1, 1, 0);
                        break;
                    case 1:
                        land = geoLandCollection[1];
                        InsertImageShapeFirstPage(land, 1, 2, 0);
                        break;
                    case 2:
                        land = geoLandCollection[2];
                        InsertImageShapeFirstPage(land, 1, 3, 0);
                        break;
                    case 3:
                        land = geoLandCollection[3];
                        InsertImageShapeFirstPage(land, 2, 1, 0);
                        break;
                    case 4:
                        land = geoLandCollection[4];
                        InsertImageShapeFirstPage(land, 2, 2, 0);
                        break;
                    case 5:
                        land = geoLandCollection[5];
                        InsertImageShapeFirstPage(land, 2, 3, 0);
                        break;
                    case 6:
                        land = geoLandCollection[6];
                        InsertImageShapeFirstPage(land, 4, 2, 0);
                        break;
                    case 7:
                        land = geoLandCollection[7];
                        InsertImageShapeFirstPage(land, 4, 3, 0);
                        break;
                    case 8:
                        land = geoLandCollection[8];
                        InsertImageShapeFirstPage(land, 4, 4, 0);
                        break;
                    default:
                        break;
                }
            }
            int pageCount = landIndex / 9;
            pageCount = landIndex % 9 == 0 ? pageCount : (pageCount + 1);

            fromthirdPageTableCount = pageCount + 1;
            int totalPage = fromthirdPageTableCount + 1;//总页数

            for (int i = 1; i < totalPage; i++)
            {
                InitalizeLandBookMarkInformationOther(i * 9, i);
            }
        }

        /// <summary>
        /// 插入地块影像信息
        /// </summary> 
        private void InitalizeLandBookMarkInformationOther(int startIndex, int tableIndex)
        {
            if (geoLandCollection == null || geoLandCollection.Count == 0)
            {
                return;
            }
            int landIndex = geoLandCollection.Count;
            ContractLand land = new ContractLand();
            for (int i = 0; i + (tableIndex * 9) < landIndex && i < 9; i++)
            {
                switch (i)
                {
                    case 0:
                        land = geoLandCollection[startIndex + 0];
                        InsertImageShapeFirstPage(land, 1, 1, tableIndex);
                        break;
                    case 1:
                        land = geoLandCollection[startIndex + 01];
                        InsertImageShapeFirstPage(land, 1, 2, tableIndex);
                        break;
                    case 2:
                        land = geoLandCollection[startIndex + 02];
                        InsertImageShapeFirstPage(land, 1, 3, tableIndex);
                        break;
                    case 3:
                        land = geoLandCollection[startIndex + 03];
                        InsertImageShapeFirstPage(land, 2, 1, tableIndex);
                        break;
                    case 4:
                        land = geoLandCollection[startIndex + 04];
                        InsertImageShapeFirstPage(land, 2, 2, tableIndex);
                        break;
                    case 5:
                        land = geoLandCollection[startIndex + 05];
                        InsertImageShapeFirstPage(land, 2, 3, tableIndex);
                        break;
                    case 6:
                        land = geoLandCollection[startIndex + 06];
                        InsertImageShapeFirstPage(land, 4, 2, tableIndex);
                        break;
                    case 7:
                        land = geoLandCollection[startIndex + 07];
                        InsertImageShapeFirstPage(land, 4, 3, tableIndex);
                        break;
                    case 8:
                        land = geoLandCollection[startIndex + 08];
                        InsertImageShapeFirstPage(land, 4, 4, tableIndex);
                        break;
                    default:
                        break;
                }
            }
        }

        ///// <summary>
        ///// 插入地块影像信息
        ///// </summary>     
        //private void InitalizeLandImageInformation()
        //{
        //    if (geoLandCollection == null || geoLandCollection.Count == 0 || geoLandCollection.Count <= 9)
        //    {
        //        DeleteTable(1);
        //        DeleteParagraph();
        //        return;
        //    }
        //    if (geoLandCollection.Count > 9)
        //    {
        //        geoLandCollection.RemoveRange(0, 9);
        //    }
        //    int landIndex = 0;

        //    for (int index = 0; index < fromthirdPageTableCount; index++)
        //    {
        //        for (int rowIndex = 1; rowIndex < 4; rowIndex++)
        //        {
        //            for (int colInex = 0; colInex < 3; colInex++)
        //            {
        //                if (landIndex >= geoLandCollection.Count)
        //                {
        //                    continue;
        //                }
        //                ContractLand land = geoLandCollection[landIndex];
        //                InsertImageShapeBig(land, rowIndex, colInex, index + 1);
        //                landIndex++;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 显示所有鹰眼图
        /// </summary>
        private void InitalizeAllEngleView()
        {
            string fileName = SavePathOfImage + @"\" + Contractor.ZoneCode + "-" + Contractor.Name + ".jpg";
            if (System.IO.File.Exists(fileName))
            {
                InsertImageCellWithoutPading(AgricultureBookMark.AgricultureAllShape, fileName, 220, 230);
            }
            System.IO.File.Delete(fileName);
        }

        /// <summary>
        /// 添加制图-审图信息
        /// </summary>
        private void AddCartographyInfo()
        {
            for (int i = 0; i < 2; i++)
            {
                SetBookmarkValue("Cartographer" + (i == 0 ? "" : i.ToString()), SettingDefine.Cartographer);
                SetBookmarkValue("CartographyDate" + (i == 0 ? "" : i.ToString()), (SettingDefine.CartographyDate == null) ? "" : SettingDefine.CartographyDate.Value.ToString("yyyy-MM-dd"));
                SetBookmarkValue("CartographyUnit" + (i == 0 ? "" : i.ToString()), SettingDefine.CartographyUnit);
                SetBookmarkValue("CheckPerson" + (i == 0 ? "" : i.ToString()), SettingDefine.CheckPerson);
                SetBookmarkValue("CheckDate" + (i == 0 ? "" : i.ToString()), (SettingDefine.CheckDate == null) ? "" : SettingDefine.CheckDate.Value.ToString("yyyy-MM-dd"));
            }
        }


        #endregion

        #region Parcel

        /// <summary>
        /// 获取全宗地示意图
        /// </summary>
        public void GetAllMultiParcel()
        {
            try
            {
                MapShapeUI map = null;
                BitmapSource image = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (ViewOfAllMultiParcel == null)
                    {
                        ViewOfAllMultiParcel = new DiagramsView();
                        ViewOfAllMultiParcel.Paper.Model.Width = 326;
                        ViewOfAllMultiParcel.Paper.Model.Height = 357;
                        //ViewOfAllMultiParcel.Paper.Model.Width = 150;
                        //ViewOfAllMultiParcel.Paper.Model.Height = 190;
                    }
                }));
                List<FeatureObject> listAllFeature = new List<FeatureObject>();
                List<FeatureObject> listOwenrFeature = new List<FeatureObject>();
                //首先创建邻宗图层
                foreach (var land in ListGeoLand)
                {
                    FeatureObject fo = new FeatureObject();
                    fo.Object = land;
                    fo.Geometry = land.Shape;
                    fo.GeometryPropertyName = "Shape";
                    listAllFeature.Add(fo);
                }
                //创建本宗图层
                foreach (var child in geoLandCollection)
                {
                    FeatureObject fo = new FeatureObject();
                    fo.Object = child;
                    fo.Geometry = child.Shape;
                    fo.GeometryPropertyName = "Shape";
                    listOwenrFeature.Add(fo);
                }
                //当前地域下的所有空间地块图形导出
                DiagramBase diagram = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (ViewOfAllMultiParcel.Items.Count == 0)
                    {
                        diagram = new MapShape() { }.CreateDiagram();
                        ViewOfAllMultiParcel.Items.Add(diagram);
                        diagram.Model.Width = 326;
                        diagram.Model.Height = 398;
                        //diagram.Model.Width = 150;
                        //diagram.Model.Height = 210;

                        diagram.Model.BorderWidth = 0;
                        diagram.Model.X = 0;
                        diagram.Model.Y = 0;
                    }
                    else
                    {
                        diagram = ViewOfAllMultiParcel.Items[0];
                    }
                    map = diagram.Content as MapShapeUI;
                    map.MapControl.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                }));

                //Application.Current.Dispatcher.Invoke(new Action(() =>
                //{
                //    map = diagram.Content as MapShapeUI;
                //    map.MapControl.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                //}));

                DynamicGeoSource allGeo = null;  //邻宗数据源 
                VectorLayer lyer = null;
                DynamicGeoSource ownerGeo = null;  //本宗数据源
                VectorLayer lyerOwnerExtent = null;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (map.MapControl.Layers.Count == 0)
                    {
                        lyer = new VectorLayer();  //创建邻宗矢量图层
                        lyer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                        lyer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                        {
                            BackgroundColor = Color.FromArgb(255, 255, 255, 255),
                            BorderStrokeColor = Color.FromArgb(255, 0, 255, 64),
                            BorderThickness = 1
                        });
                        lyer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
                        {
                            FontSize = 10
                        })
                        {
                            Enabled = false,
                            LabelProperty = "OwnerName"
                        };
                        allGeo = new DynamicGeoSource();
                        allGeo.AddRange(listAllFeature.ToArray());
                        lyer.DataSource = allGeo;
                        map.MapControl.Extend = allGeo.FullExtend.ToGeometry().Buffer(0).GetEnvelope();
                        map.MapControl.SpatialReference = new SpatialReference(0);
                        map.MapControl.Layers.Add(lyer);
                    }
                    else
                    {
                        lyer = map.MapControl.Layers[0] as VectorLayer;
                        allGeo = lyer.DataSource as DynamicGeoSource;
                        allGeo.Clear();
                        allGeo.AddRange(listAllFeature.ToArray());
                        //map.MapControl.NavigateTo(allGeo.FullExtend.ToGeometry().Buffer(20).GetEnvelope());
                    }
                }));
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (map.MapControl.Layers.Count == 1)
                    {
                        lyerOwnerExtent = new VectorLayer();   //创建本宗矢量图层
                        lyerOwnerExtent.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                        lyerOwnerExtent.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                        {
                            BackgroundColor = Color.FromArgb(255, 255, 62, 62),
                            BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                            BorderThickness = 1
                        });
                        lyerOwnerExtent.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
                        {
                            FontSize = 10
                        })
                        {
                            Enabled = false,
                            LabelProperty = "OwnerName"
                        };
                        ownerGeo = new DynamicGeoSource();
                        ownerGeo.AddRange(listOwenrFeature.ToArray());
                        lyerOwnerExtent.DataSource = ownerGeo;
                        map.MapControl.Layers.Add(lyerOwnerExtent);
                    }
                    else
                    {
                        lyerOwnerExtent = map.MapControl.Layers[1] as VectorLayer;
                        ownerGeo = lyerOwnerExtent.DataSource as DynamicGeoSource;
                        ownerGeo.Clear();
                        ownerGeo.AddRange(listOwenrFeature.ToArray());
                        map.MapControl.NavigateTo(map.MapControl.Extend.Clone() as Envelope);
                    }
                }));

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    AddMainCompass(ViewOfAllMultiParcel, diagram);    //添加主视图指北针
                    //保存为图片
                    image = ViewOfAllMultiParcel.SaveToImage();
                    if (image == null)
                        return;
                    string fileName = SavePathOfImage + @"\" + Contractor.ZoneCode + "-" + Contractor.Name + ".jpg";
                    image.SaveToJpgFile(fileName);

                }));
                listAllFeature.Clear();
                listAllFeature = null;
                listOwenrFeature.Clear();
                listOwenrFeature = null;
            }
            catch (Exception ex)
            {
                throw new YltException("获取全宗地示意图失败!");
            }
        }


        /// <summary>
        /// 确保元素完全可见
        /// </summary>
        /// <param name="visibleBounds"></param>
        /// <param name="element"></param>
        private void EnsureFullVisible(CglEnvelope visibleBounds, DiagramBase element)
        {
            var w = visibleBounds.MaxX + 15;
            //using (var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            //{
            //    w=w*96.0/ g.DpiX;
            //}
            var leftX = -2;
            var m = element.Model;
            if (m.X < leftX)
            {
                m.X = leftX;// visibleBounds.MinX;
            }
            else if (m.X + m.Width > w)// visibleBounds.MaxX)
            {
                m.X = w - m.Width;// visibleBounds.MaxX - m.Width;
            }
            var topY = 0;
            if (m.Y < topY)
            {
                m.Y = topY;
            }
        }

        /// <summary>
        /// 获取邻宗地示意图
        /// </summary>
        public void GetNeighorParcels()

        {
            var visibleBounds = new CglEnvelope(0, 0, 136, 136);
            try
            {
                MapShapeUI map = null;
                List<FeatureObject> listFeature = new List<FeatureObject>();
                var targetSpatialReference = DbContext.CreateSchema().GetElementSpatialReference(ObjectContext.Create(typeof(ContractLand)).Schema, ObjectContext.Create(typeof(ContractLand)).TableName);

                //各个空间地块的邻宗地图导出
                foreach (var geoLand in geoLandCollection)
                {
                    BitmapSource image = null;
                    DiagramBase diagram = null;
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (ViewOfNeighorParcels == null)
                        {
                            ViewOfNeighorParcels = new DiagramsView();
                            ViewOfNeighorParcels.Paper.Model.Width = visibleBounds.Width;// 136;
                            ViewOfNeighorParcels.Paper.Model.Height = visibleBounds.Height;// 136;
                        }
                        if (ViewOfNeighorParcels.Items.Count == 0)
                        {
                            diagram = new MapShape() { }.CreateDiagram();
                            diagram.Model.Width = mapW;
                            diagram.Model.Height = mapH;
                            diagram.Model.BorderWidth = 0;
                            diagram.Model.X = 0;
                            diagram.Model.Y = 0;
                            ViewOfNeighorParcels.Items.Add(diagram);
                        }
                        else
                        {
                            diagram = ViewOfNeighorParcels.Items[0];
                            diagram.Model.Width = mapW;
                            diagram.Model.Height = mapH;
                            diagram.Model.BorderWidth = 0;
                            diagram.Model.X = 0;
                            diagram.Model.Y = 0;
                        }
                        ViewOfNeighorParcels.Paper.Model.BorderWidth = 1;
                        ViewOfNeighorParcels.Paper.Model.BorderColor = Colors.White;
                        map = diagram.Content as MapShapeUI;
                        map.MapControl.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    }));

                    //首先创建邻宗图层                  
                    var tempLands = ListGeoLand.FindAll(c => c.Shape.Intersects(geoLand.Shape) && c.ID != geoLand.ID);  //缓冲区(除去目标地块本身)
                    List<Spatial.Geometry> interlines = new List<Spatial.Geometry>();//临宗边线
                    foreach (var ld in tempLands)
                    {
                        var templines = ld.Shape.ToSegmentLines();
                        foreach (var itemline in templines)
                        {
                            if (itemline.Intersects(geoLand.Shape))
                            {
                                FeatureObject fo = new FeatureObject();
                                fo.Object = ld;
                                fo.Geometry = itemline;
                                fo.GeometryPropertyName = "Shape";
                                listFeature.Add(fo);
                                interlines.Add(itemline);
                            }
                        }
                    }
                    DynamicGeoSource otherGeos = null;   //邻宗数据源
                    VectorLayer lyer = null;  //创建邻宗矢量图层
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (map.MapControl.Layers.Count == 0)
                        {
                            lyer = new VectorLayer();  //创建邻宗矢量图层
                            lyer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                            lyer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                            {
                                StrokeColor = System.Windows.Media.Color.FromArgb(255, 0, 255, 64),
                                StrokeThickness = 1
                            });

                            otherGeos = new DynamicGeoSource();
                            otherGeos.AddRange(listFeature.ToArray());
                            lyer.DataSource = otherGeos;
                            map.MapControl.Layers.Add(lyer);
                            map.MapControl.SpatialReference = targetSpatialReference;
                        }
                        else
                        {
                            lyer = map.MapControl.Layers[0] as VectorLayer;
                            otherGeos = lyer.DataSource as DynamicGeoSource;
                            otherGeos.Clear();
                            otherGeos.AddRange(listFeature.ToArray());
                            //map.MapControl.NavigateTo(otherGeos.FullExtend);
                        }
                    }));
                    listFeature.Clear();

                    var bufferLine = geoLand.Shape.Buffer(4.0);
                    var tempLines = ListLineFeature.FindAll(c => c.Shape.Intersects(bufferLine));  //线状地物
                    foreach (var line in tempLines)
                    {
                        FeatureObject fo = new FeatureObject();
                        fo.Object = line;
                        fo.Geometry = line.Shape;
                        fo.GeometryPropertyName = "Shape";
                        listFeature.Add(fo);
                    }
                    DynamicGeoSource lineGeos = null;   //线状地物数据源
                    VectorLayer lineLayer = null;  //创建线状地物矢量图层
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (map.MapControl.Layers.Count == 1)
                        {
                            lineLayer = new VectorLayer();  //创建线状地物矢量图层
                            lineLayer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                            lineLayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                            {
                                StrokeColor = Color.FromArgb(255, 0, 255, 64),
                                StrokeThickness = 1
                            });
                            lineGeos = new DynamicGeoSource();
                            lineGeos.AddRange(listFeature.ToArray());
                            lineLayer.DataSource = lineGeos;
                            map.MapControl.Layers.Add(lineLayer);
                            map.MapControl.SpatialReference = targetSpatialReference;
                        }
                        else
                        {
                            lineLayer = map.MapControl.Layers[1] as VectorLayer;
                            lineGeos = lineLayer.DataSource as DynamicGeoSource;
                            lineGeos.Clear();
                            lineGeos.AddRange(listFeature.ToArray());
                        }
                    }));

                    ////创建本宗图层
                    var fot = new FeatureObject();
                    fot.Object = geoLand;
                    fot.Geometry = geoLand.Shape;
                    fot.GeometryPropertyName = "Shape";

                    DynamicGeoSource selfGeods = null;  //本宗数据源
                    VectorLayer lyerOwner = null;     //创建本宗矢量图层
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (map.MapControl.Layers.Count == 2)
                        {
                            lyerOwner = new VectorLayer();   //创建本宗矢量图层
                            lyerOwner.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                            lyerOwner.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                            {
                                BackgroundColor = Colors.Transparent,
                                BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                                BorderThickness = 1
                            });
                            lyerOwner.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
                            {
                                FontSize = 14
                            })
                            {
                                Enabled = false,
                                LabelProperty = "OwnerName"
                            };
                            selfGeods = new DynamicGeoSource();
                            selfGeods.Add(fot);
                            lyerOwner.DataSource = selfGeods;
                            //map.MapControl.Extend = selfGeods.FullExtend.ToGeometry().Buffer(5).GetEnvelope();
                            map.MapControl.SpatialReference = targetSpatialReference;
                            map.MapControl.Layers.Add(lyerOwner);
                        }
                        else
                        {
                            lyerOwner = map.MapControl.Layers[2] as VectorLayer;
                            selfGeods = lyerOwner.DataSource as DynamicGeoSource;
                            selfGeods.Clear();
                            selfGeods.Add(fot);
                            //map.MapControl.NavigateTo(selfGeods.FullExtend.ToGeometry().Buffer(5).GetEnvelope()); 
                        }
                    }));

                    //创建节点标注图层
                    AddNodeLayer(geoLand, tempLands, map, selfGeods, interlines);

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        var env = map.MapControl.Extend;

                        var mElements = new MyElements(visibleBounds);

                        AddScaleText(ViewOfNeighorParcels, diagram, selfGeods, env);    //添加比例尺文本标注
                        var ce = AddCenterLable(ViewOfNeighorParcels, diagram, selfGeods, env, geoLand);  //添加中心文本标注
                        mElements.AddElement(ce);
                        AddLine(ViewOfNeighorParcels, diagram, selfGeods, env, geoLand);
                        AddCompass(ViewOfNeighorParcels, diagram);  //添加指北针
                        var index = 5;
                        foreach (var tempitem in tempLands)
                        {
                            var element = AddNeiberLandlabel(ViewOfNeighorParcels, diagram, selfGeods, env, tempitem, geoLand, index++);  //添加临宗文本标注
                            EnsureFullVisible(visibleBounds, element);
                            mElements.AddElement(element);
                        }
                        for (int i = index; i < ViewOfNeighorParcels.Items.Count; i++)
                        {
                            (ViewOfNeighorParcels.Items[i].Model as TextShape).Text = null;
                        }
                        image = ViewOfNeighorParcels.SaveToImage();   //保存为图片
                        if (image != null)
                        {
                            string fileName = SavePathOfImage + geoLand.LandNumber + ".jpg";
                            image.SaveToJpgFile(fileName);
                        }
                    }));

                    listFeature.Clear();
                }
            }
            catch (Exception ex)
            {
                throw new YltException("获取邻宗地示意图失败!");
            }
        }

        /// <summary>
        /// 添加比例尺标注
        /// </summary>
        private void AddScaleText(DiagramsView view, DiagramBase diagram, DynamicGeoSource geods, Envelope extent)
        {
            //var res = Math.Min(geods.FullExtend.Width / 300, geods.FullExtend.Height / 200);
            var res = Math.Max(extent.Width / mapW, extent.Height / mapH);
            var result = res * 96 * 0.3937008 * 100;
            var integer = Math.Truncate(result * 0.01);
            if (integer == 0)
                result = 100;
            else
                result = integer * 100;
            if (view.Items.Count == 1)
            {
                diagram = new TextShape()
                {
                    FontSize = 10,
                    FontColor = Color.FromRgb(0, 0, 0),
                }.CreateDiagram();
                diagram.Model.Width = 50;
                diagram.Model.Height = 17;
                diagram.Model.BorderWidth = 0;
                diagram.Tag = true;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[1];
            }
            (diagram.Model as TextShape).Text = string.Format("1:{0:F0}", result);
            diagram.Model.X = 45;
            diagram.Model.Y = 120;
        }

        /// <summary>
        /// 添加中心标注
        /// </summary>
        private DiagramBase AddCenterLable(DiagramsView view, DiagramBase diagram, DynamicGeoSource geods, Envelope extent, ContractLand geoLand)
        {
            var geo = geoLand.Shape as YuLinTu.Spatial.Geometry;
            var obj = geods.Get()[0];
            var code = ObjectExtension.GetPropertyValue<string>(obj.Object, "LandNumber");

            var text = string.Format("{0}\n{1}", geoLand.ActualArea.ToString("0.00"), code.Length < 5 ? code : code.Substring(code.Length - 5, 5));

            Size sz;
            //var location = GetLocation(geods.FullExtend, 300, 200, 100, 150, geo);
            if (view.Items.Count == 2)
            {
                diagram = new TextShape()
                {
                    FontSize = 10,
                    FontColor = Color.FromRgb(0, 0, 0),
                    //BackgroundColor=Color.FromArgb(100,0,0,255),
                }.CreateDiagram();

                sz = calcTextSize(text, diagram);

                diagram.Model.Width = sz.Width;// 100;
                diagram.Model.Height = sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                (diagram.Model as TextShape).HorizontalAlignment = HorizontalAlignment.Center;
                (diagram.Model as TextShape).VerticalAlignment = VerticalAlignment.Center;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[2];
                sz = calcTextSize(text, diagram);
                diagram.Model.Width = sz.Width;// 100;
                diagram.Model.Height = sz.Height;// 60;
            }
            (diagram.Model as TextShape).Text = text;// string.Format("{0}\n{1}", geoLand.ActualArea.ToString("0.00"), code.Length < 5 ? code : code.Substring(code.Length - 5, 5));

            // var location = GetLocation(extent, mapW, mapH, 100, 60, geo);
            var location = GetLocation(extent, mapW, mapH, sz.Width, sz.Height, geo);

            diagram.Model.X = location.X + 0;
            diagram.Model.Y = location.Y + 0;
            return diagram;
        }

        private static Size calcTextSize(string text, DiagramBase e)// FontFamily fontFamily, double fontSize, FontWeights fontWeights)
        {
            text = text != null ? text : "";
            if (true)
            {
                var typeFace = new Typeface(e.FontFamily,// new System.Windows.Media.FontFamily(fontName),
                              e.FontStyle,// System.Windows.FontStyles.Normal,
                              e.FontWeight,// bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
                              e.FontStretch);// System.Windows.FontStretches.Normal);

                //var typeFace = GetTypeface(font.FontFamily.Name, font.Bold);

                var fontSize = (double)new System.Windows.FontSizeConverter().ConvertFrom(e.FontSize + "pt");
                var ft = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight
                    , typeFace, fontSize, System.Windows.Media.Brushes.Black);
                return new Size(ft.Width, ft.Height * 0.77);
            }
            if (true)
            {
                var font = new System.Drawing.Font(e.FontFamily.ToString(), (float)e.FontSize);

                if (false)
                {
                    using (var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                    {
                        var m = g.MeasureString(text, font);
                        return new Size(m.Width, m.Height);
                    }
                }

                var r = TextUtil.CalcTextRect(text, font);
                double w = r.Width();// *72.0 / 96.0;
                                     //double h = r.Height() *72.0 / 96.0;
                if (text.Length > 0 && !text.Contains('\n'))
                {
                    w -= text.Length;// *3;
                }

                return new Size(w, r.Height());
            }
            else
            {
                var tf = new System.Windows.Media.Typeface(e.FontFamily,
                  e.FontStyle,// System.Windows.FontStyles.Normal,
                  e.FontWeight,// bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
                  e.FontStretch);// System.Windows.FontStretches.Normal);                
                var r = TextUtil.CalcTextRect(text, tf, e.FontSize);
                return new Size(r.Width(), r.Height());
            }
        }

        /// <summary>
        /// 添加相邻宗地标注
        /// </summary>
        private DiagramBase AddNeiberLandlabel(DiagramsView view, DiagramBase diagram, DynamicGeoSource geods, Envelope extent, ContractLand tempLand, ContractLand geoLand, int index)
        {
            double Distance = 3;//线段延长的距离；
            var templandgeo = tempLand.Shape.Normalized();
            var tempgetitem = templandgeo.Intersection(extent.ToGeometry());
            var templandcenterptn = new Coordinate(tempgetitem.Instance.PointOnSurface.X, tempgetitem.Instance.PointOnSurface.Y);//终点

            var geolandgeo = geoLand.Shape.Normalized();
            var geolandcenterptn = new Coordinate(geolandgeo.Instance.PointOnSurface.X, geolandgeo.Instance.PointOnSurface.Y);//起点
            List<Coordinate> uselinecdts = new List<Coordinate>();
            uselinecdts.Add(geolandcenterptn);
            uselinecdts.Add(templandcenterptn);
            Spatial.Geometry useline = Spatial.Geometry.CreatePolyline(uselinecdts, templandgeo.Srid);

            var res = Math.Max(extent.Width / mapW, extent.Height / mapH);
            var distancebettwocdt = Math.Sqrt(Math.Pow(geolandcenterptn.X - templandcenterptn.X, 2) + Math.Pow(geolandcenterptn.Y - templandcenterptn.Y, 2));
            var screendistancetwocdt = distancebettwocdt / res;
            var newDistancePtn = new Spatial.Geometry();
            if (screendistancetwocdt < 30)
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance + 15 * res);//获取新距离下的坐标点；
            }
            else if (screendistancetwocdt > 60)
            {
                newDistancePtn = Deflection_Distance(useline, 0, -Distance + 1);//获取新距离下的坐标点；
            }
            else
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance);//获取新距离下的坐标点； 
            }

            var landOwnerName = tempLand.OwnerName;
            double elementWidth = 100;
            double elementHeight = 60;
            if (view.Items.Count == index)
            {
                diagram = new TextShape()
                {
                    FontSize = 12,
                    FontColor = System.Windows.Media.Color.FromRgb(0, 0, 0),
                    //BackgroundColor = Color.FromArgb(100,0, 255,0),
                }.CreateDiagram();

                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60


                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                (diagram.Model as TextShape).HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                (diagram.Model as TextShape).VerticalAlignment = System.Windows.VerticalAlignment.Center;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60
                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
            }
            (diagram.Model as TextShape).Text = landOwnerName;

            var location = GetLocation(extent, mapW, mapH, elementWidth, elementHeight, newDistancePtn);

            diagram.Model.X = location.X + 0;
            diagram.Model.Y = location.Y + 0;
            return diagram;
        }
        /// <summary>
        /// 添加标注线
        /// </summary>
        private void AddLine(DiagramsView view, DiagramBase diagram, DynamicGeoSource geods, Envelope extent, ContractLand geoLand)
        {
            var geo = geoLand.Shape as YuLinTu.Spatial.Geometry;
            //var location = GetLocation(geods.FullExtend, 300, 200, 50, 1, geo);
            var location = GetLocation(extent, mapW, mapH, 40, 1, geo);
            if (view.Items.Count == 3)
            {
                diagram = new RectangleShape()
                {
                    BorderWidth = 0,
                    IsBold = false,
                    BackgroundColor = Color.FromRgb(0, 0, 0),
                    FontColor = Color.FromRgb(0, 0, 0),
                }.CreateDiagram();
                diagram.Model.Width = 30;
                diagram.Model.Height = 1;
                diagram.Model.BorderWidth = 0;
                //diagram.Model.BackgroundColor = Color.FromRgb(0, 0, 0);
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[3];
            }
            diagram.Model.X = location.X + 4;
            diagram.Model.Y = location.Y + 0;
        }

        /// <summary>
        /// 获取插入位置
        /// </summary>
        private Point GetLocation(Envelope mapExtent, double mapWidth, double mapHeight, double diagramWidth, double diagramHeight, YuLinTu.Spatial.Geometry geo)
        {
            var res = Math.Max(mapExtent.Width / mapWidth, mapExtent.Height / mapHeight);

            var dWidth = res * mapWidth;
            var dHeight = res * mapHeight;

            var extent = new Envelope()
            {
                MinX = mapExtent.MinX - dWidth / 2 + mapExtent.Width / 2,
                MaxX = mapExtent.MaxX + dWidth / 2 - mapExtent.Width / 2,
                MaxY = mapExtent.MaxY + dHeight / 2 - mapExtent.Height / 2,
                MinY = mapExtent.MinY - dHeight / 2 + mapExtent.Height / 2
            };

            var geo2 = geo.Normalized();

            var center = new Coordinate(geo2.Instance.PointOnSurface.X, geo2.Instance.PointOnSurface.Y);

            var x = (center.X - extent.MinX) / res - diagramWidth / 2;
            var y = (extent.MaxY - center.Y) / res - diagramHeight / 2;

            return new Point(x, y);

        }

        /// <summary>
        /// 添加节点标注
        /// </summary>
        private void AddNodeLayer(ContractLand geoLand, List<ContractLand> listGeoLand, MapShapeUI map, DynamicGeoSource geos, List<Spatial.Geometry> interlines)
        {
            if (geos == null)
                return;
            var geo = geoLand.Shape as YuLinTu.Spatial.Geometry;
            var points = geo.ToPoints();
            var filterNode = FilterNodesByPoint(geoLand, listGeoLand, points, interlines);//过滤节点
            List<FeatureObject> fos = new List<FeatureObject>();
            filterNode.ForEach(c =>
            {
                var fo = new FeatureObject() { Geometry = c };
                fos.Add(fo);
            });
            //List<FeatureObject> fos = FilterObjectNodeByValidDot(geoLand, points);
            DynamicGeoSource geoSource = null;
            VectorLayer pointLyer = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == 3)
                {
                    pointLyer = new VectorLayer();
                    pointLyer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    pointLyer.Renderer = new SimpleRenderer(new SimplePointSymbol()
                    {
                        BackgroundColor = Colors.White,
                        BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                        Size = 4
                    });
                    geoSource = new DynamicGeoSource();
                    geoSource.AddRange(fos.ToArray());
                    pointLyer.DataSource = geoSource;
                    map.MapControl.Extend = geos.FullExtend.ToGeometry().Buffer(10).GetEnvelope();
                    map.MapControl.Layers.Add(pointLyer);
                    map.MapControl.SpatialReference = new SpatialReference(0);
                }
                else
                {
                    pointLyer = map.MapControl.Layers[3] as VectorLayer;
                    geoSource = pointLyer.DataSource as DynamicGeoSource;
                    geoSource.Clear();
                    geoSource.AddRange(fos.ToArray());
                    map.MapControl.NavigateTo(geos.FullExtend.ToGeometry().Buffer(10).GetEnvelope());
                }
            }));
        }

        /// <summary>
        /// 过滤节点
        /// </summary>
        private List<YuLinTu.Spatial.Geometry> FilterNodesByPoint(ContractLand geoLand, List<ContractLand> listGeoLand, YuLinTu.Spatial.Geometry[] points, List<Spatial.Geometry> interlines)
        {
            List<YuLinTu.Spatial.Geometry> listGeos = new List<YuLinTu.Spatial.Geometry>();
            List<YuLinTu.Spatial.Geometry> lineCollection = new List<Spatial.Geometry>();
            listGeoLand.ForEach(c => listGeos.Add(c.Shape as YuLinTu.Spatial.Geometry));
            double angleMaxSet = 155;
            double angleMinSet = 10;
            foreach (var geo in listGeos)
            {
                var geoSegments = geo.ToSegmentLines();
                if (geoSegments == null || geoSegments.Count() == 0)
                    continue;
                lineCollection.AddRange(geoSegments);
            }
            listGeos.Clear();

            var pts = points.ToList();
            foreach (var lineitem in interlines)
            {
                var linecdts = lineitem.ToCoordinates();
                var ap = pts.Find(p => p.ToCoordinates()[0].X == linecdts[0].X && p.ToCoordinates()[0].Y == linecdts[0].Y);
                var bp = pts.Find(p => p.ToCoordinates()[0].X == linecdts[1].X && p.ToCoordinates()[0].Y == linecdts[1].Y);
                if (ap != null && bp != null) continue;
                if (ap == null && bp == null) continue;
                if (ap != null) listGeos.Add(ap);
                if (bp != null) listGeos.Add(bp);
            }

            for (int i = 1; i < points.Length - 1; i++)
            {
                var pt0 = points[i];
                var pt1 = points[i - 1];
                var pt2 = points[i + 1];
                var angle = 0.0;

                angle = MathHelper.GetVectorAngle(pt0.ToCoordinates()[0].X, pt0.ToCoordinates()[0].Y, pt1.ToCoordinates()[0].X, pt1.ToCoordinates()[0].Y, pt2.ToCoordinates()[0].X, pt2.ToCoordinates()[0].Y);

                if (angle > angleMinSet && angle < angleMaxSet)
                {
                    if (!listGeos.Any(d => d.ToCoordinates()[0].X == pt0.ToCoordinates()[0].X && d.ToCoordinates()[0].Y == pt0.ToCoordinates()[0].Y))
                        listGeos.Add(points[i]);
                }
            }

            //判断第一个点
            var pt00 = points[0];
            var pt11 = points[points.Count() - 1];
            var pt22 = points[1];


            var angle1 = MathHelper.GetVectorAngle(pt00.ToCoordinates()[0].X, pt00.ToCoordinates()[0].Y, pt11.ToCoordinates()[0].X, pt11.ToCoordinates()[0].Y, pt22.ToCoordinates()[0].X, pt22.ToCoordinates()[0].Y);

            if (angle1 > angleMinSet && angle1 < angleMaxSet)
            {
                if (!listGeos.Any(d => d.ToCoordinates()[0].X == pt00.ToCoordinates()[0].X && d.ToCoordinates()[0].Y == pt00.ToCoordinates()[0].Y))
                    listGeos.Add(pt00);
            }

            //判断最后一个点
            var pt000 = points[points.Count() - 1];
            var pt111 = points[points.Count() - 2];
            var pt222 = points[0];


            var angle2 = MathHelper.GetVectorAngle(pt000.ToCoordinates()[0].X, pt000.ToCoordinates()[0].Y, pt111.ToCoordinates()[0].X, pt111.ToCoordinates()[0].Y, pt222.ToCoordinates()[0].X, pt222.ToCoordinates()[0].Y);

            if (angle2 > angleMinSet && angle2 < angleMaxSet)
            {
                if (!listGeos.Any(d => d.ToCoordinates()[0].X == pt000.ToCoordinates()[0].X && d.ToCoordinates()[0].Y == pt000.ToCoordinates()[0].Y))
                    listGeos.Add(pt000);
            }

            lineCollection.Clear();
            return listGeos;
        }

        /// <summary>
        /// 添加指北针
        /// </summary>
        private void AddCompass(DiagramsView view, DiagramBase diagram)
        {
            if (view.Items.Count == 4)
            {
                diagram = new CompassShape()
                {
                    FontSize = 1,
                    BorderColor = Color.FromRgb(50, 50, 50),
                }.CreateDiagram();
                diagram.Model.Height = 25;
                diagram.Model.Width = 15;
                diagram.Model.BorderWidth = 0.5;
                (diagram.Model as CompassShape).BorderWidthN = 1;
                diagram.Model.X = 0;
                diagram.Model.Y = 0;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[4];
            }
        }

        /// <summary>
        /// 添加主视图指北针
        /// </summary>
        private void AddMainCompass(DiagramsView view, DiagramBase diagram)
        {
            if (view.Items.Count == 1)
            {
                diagram = new CompassShape()
                {
                    FontSize = 2,
                    BorderColor = Color.FromRgb(50, 50, 50),
                }.CreateDiagram();
                diagram.Model.Height = 30;
                diagram.Model.Width = 15;
                diagram.Model.BorderWidth = 1;
                (diagram.Model as CompassShape).BorderWidthN = 1;
                diagram.Model.X = 20;
                diagram.Model.Y = 20;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[1];
            }

            //view.Dispose();
        }

        /// <summary>
        /// 按照线段起点-终点-输入长度-延长线进行获取新的点(角度设置为0
        /// </summary>   
        /// <returns></returns>
        private Spatial.Geometry Deflection_Distance(Spatial.Geometry ls, double angle, double distance)
        {
            Spatial.Geometry ptn;
            var linecdts = ls.ToCoordinates();
            var Startcdt = linecdts[0];
            var Endcdt = linecdts[1];
            double s1_x = Endcdt.X - Startcdt.X;
            double s1_y = Endcdt.Y - Startcdt.Y;
            double d = Math.Sqrt(s1_x * s1_x + s1_y * s1_y);
            if (d == 0)
            {
                ptn = Spatial.Geometry.CreatePoint(Endcdt, ls.Srid);
                return ptn;
            }
            else
            {
                angle = angle * Math.PI / 180.0f;//角度转换为弧度
                double r_normal = 1.0f / d;
                s1_x *= r_normal;
                s1_y *= r_normal;
                double tempx = s1_x * Math.Cos(angle) - s1_y * Math.Sin(angle);
                double tempy = s1_x * Math.Sin(angle) + s1_y * Math.Cos(angle);
                s1_x = tempx * distance;
                s1_y = tempy * distance;
                var retX = Endcdt.X + s1_x;
                var retY = Endcdt.Y + s1_y;

                Coordinate retptn = new Coordinate(retX, retY);
                ptn = Spatial.Geometry.CreatePoint(retptn, ls.Srid);

                return ptn;
            }
        }

        #endregion

        #endregion
    }
}
