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
using System.Text.RegularExpressions;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    ///  农村土地承包经营权地块示意图
    /// </summary>
    public class ExportContractLandParcelWord : AgricultureWordBook
    {
        #region Fields

        private List<Dictionary> dictDKLB;    //地块类别数据字典集合
        protected List<ContractLand> geoLandCollection;  //空间地块集合-用户的地块集合
        private SpatialReference spatialReference;
        protected int fromTwoPageTableCount;//从第二页开始的表个数，包括第二页
        private ExportLandParcelMainOperation exportLandParcelMainOperation;

        private int ROW1;
        private int COL1;
        private int ROW2;
        private int COL2;
        private int PAGECOUNT1;
        private int PAGECOUNT2;

        #endregion Fields

        #region Field - Const

        private double mapW = 160;//250
        private double mapH = 150;//240

        #endregion Field - Const

        #region Properties

        /// <summary>
        /// Word文档保存路径
        /// </summary>
        public string SavePathOfWord { get; set; }

        /// <summary>
        /// 编辑过的地块示意图集合
        /// </summary>
        public List<DiagramsView> LstViewOfNeighorParcels { get; set; }

        /// <summary>
        /// 宗地图文件保存路径
        /// </summary>
        public string SavePathOfImage { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
       // public IDbContext DbContext { get; set; }

        /// <summary>
        /// 合同集合(当前地域)
        /// </summary>
        public List<ContractConcord> ListConcord { get; set; }

        /// <summary>
        /// 权证集合(当前地域)
        /// </summary>
        public List<ContractRegeditBook> ListBook { get; set; }

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
        /// 点状地物(当前地域)
        /// </summary>
        public List<DZDW> ListPointFeature { get; set; }

        /// <summary>
        /// 面状地物(当前地域)
        /// </summary>
        public List<MZDW> ListPolygonFeature { get; set; }

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

        public ContractBusinessParcelWordSettingDefine SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();

        /// <summary>
        /// 村级地域
        /// </summary>
        public Zone VillageZone { get; set; }

        // 当前村级地域的所有地块
        public List<ContractLand> VillageContractLands { get; set; } = new List<ContractLand>();

        /// <summary>
        /// 是否保存
        /// </summary>
        public bool IsSave { get; set; } = true;

        /// <summary>
        /// 地块所有人
        /// </summary>
        public VirtualPerson OwnedPerson { get; set; }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportContractLandParcelWord(IDbContext db)
        {
            DbContext = db;
            spatialReference = DbContext.CreateSchema().GetElementSpatialReference(ObjectContext.Create(typeof(ContractLand)).Schema, ObjectContext.Create(typeof(ContractLand)).TableName);
            DictDKLB = new List<Dictionary>();
            exportLandParcelMainOperation = new ExportLandParcelMainOperation();
            base.TemplateName = "地块示意图";
            base.Tags = new object[1];
            base.Tags[0] = db;
        }

        #endregion Ctor

        #region Methods

        #region Override

        /// <summary>
        /// 填写数据
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            exportLandParcelMainOperation.ListLineFeature = ListLineFeature;
            exportLandParcelMainOperation.ListPointFeature = ListPointFeature;
            exportLandParcelMainOperation.ListPolygonFeature = ListPolygonFeature;
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
                if (geoLandCollection.Count == 0)
                {
                    return false;
                }

                base.OnSetParamValue(data);
                // 2017/06/28根据安徽金寨，导出确权确股地块示意图时，重新计算承包地块总面积
                if (!IsStockLand.HasValue)
                    RecalculateActualArea();

                GetAllMultiParcel();
                GetNeighorParcels();//生成图片的方式，基本不用修改

                AgricultureStrandardLandProgress();//和模板格局有关的业务，需要经常调整
            }
            catch (SystemException ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSetParamValue(导出宗地图失败)", ex.Message + ex.StackTrace);
                throw ex;
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

        #endregion Override

        #region ContractLand

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="data">承包方</param>
        protected virtual bool InitalizeDataInformation(object data)
        {
            if (ListGeoLand == null || ListGeoLand.Count == 0)
                return false;
            Contractor = data as VirtualPerson;

            if (IsStockLand != null)
            {
                if ((bool)IsStockLand)
                    geoLandCollection = ListGeoLand;
                else
                    geoLandCollection = Contractor != null ? ListGeoLand.FindAll(c => c.OwnerId == Contractor.ID) : new List<ContractLand>();//得到地块集合
            }
            else
            {
                // 确权确股先获取非股地，再获取股地
                geoLandCollection = Contractor != null ? ListGeoLand.FindAll(c => c.OwnerId == Contractor.ID && !c.IsStockLand) : new List<ContractLand>(); //得到地块集合
                var stockLandsvp = DbContext.CreateBelongRelationWorkStation().GetLandByPerson(Contractor.ID, CurrentZone.FullCode);
                foreach (var stockLand in stockLandsvp)
                {
                    if (!geoLandCollection.Any(t => t.ID == stockLand.ID))
                        geoLandCollection.Add(stockLand);
                }
            }
            List<ContractLand> lands = new List<ContractLand>();

            if (IsStockLand != null)
            {
                if (!(bool)IsStockLand)
                {
                    foreach (ContractLand land in geoLandCollection)
                    {
                        if (IsLegal(land))
                            continue;
                        lands.Add(land);
                    }
                }
                else
                {
                    lands = ListGeoLand.Clone();
                }
            }
            else
            {
                foreach (ContractLand land in geoLandCollection)
                {
                    if (land.IsStockLand)
                    {
                        if (IsLegal(land))
                            continue;
                        lands.Add(land);
                    }
                }
            }
            foreach (ContractLand land in lands)
            {
                geoLandCollection.Remove(land);
            }
            InitalizeAgricultureValue();

            return true;
        }

        /// <summary>
        /// 判断地块是否符合业务需求
        /// </summary>
        /// <param name="land">地块</param>
        /// <returns></returns>
        private bool IsLegal(ContractLand land)
        {
            bool isLegal = false;
            if (dictDKLB.Any(c => !string.IsNullOrEmpty(c.Code) && c.Code.Equals(land.LandCategory)))
            {
                isLegal = true;
            }
            if (land.LandCategory == null)
            {
                YuLinTu.Library.Log.Log.WriteWarn(land, "地块示意图错误", "当前地块" + land.LandNumber + "地块类别为空");
                throw new ArgumentNullException("地块示意图错误:" + "当前地块" + land.LandNumber + "地块类别为空");
            }
            return isLegal;
        }

        /// <summary>
        /// 初始化地块列表
        /// </summary>
        protected virtual void InitalizeAgricultureValue()
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

            List<ContractLand> landArray = InitalizeAgricultureLandPosition(geoLandCollection);
            geoLandCollection.Clear();

            LandCollection = new List<ContractLand>();
            foreach (var land in landArray)
            {
                geoLandCollection.Add(land);
                LandCollection.Add(land);
            }

            if (SettingDefine.IsLandNumberStart)
            {
                if (SettingDefine.LandNumberIndex > 1)
                {
                    int index = (int)(SettingDefine.LandNumberIndex);
                    if (geoLandCollection.Count >= index)
                    {
                        geoLandCollection.RemoveRange(0, index - 1);
                    }
                }
            }
            landArray.Clear();
        }

        /// <summary>
        /// 初始化地块的排序位置-是否按照地块编码排序
        /// </summary>
        protected List<ContractLand> InitalizeAgricultureLandPosition(List<ContractLand> lands)
        {
            List<ContractLand> landArray = new List<ContractLand>();
            if (SettingDefine.IsLandTypeSort)
            {
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
            }
            else
            {
                foreach (var item in lands)
                {
                    landArray.Add(item);
                }
            }
            return landArray;
        }

        /// <summary>
        /// 插入文件
        /// </summary>
        protected void InsertImageShape(ContractLand land, int rowIndex, int columnIndex, int tableIndex = 0)
        {
            try
            {
                var uselandnumber = Regex.Replace(land.LandNumber, @"[^\d]", "_");
                string imagePath = SavePathOfImage + @"\" + uselandnumber + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    if (SettingDefine.IsFixedLandGeoWordExtend)
                    {
                        SetTableCellValue(tableIndex, rowIndex, columnIndex, imagePath, 130, 175, false);
                    }
                    else
                    {
                        SetTableCellValue(tableIndex, rowIndex, columnIndex, imagePath, SettingDefine.LandGeoWordWidth, SettingDefine.LandGeoWordHeight, false);
                    }
                }
                System.IO.File.Delete(imagePath);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        #endregion ContractLand

        #region 提供重写方法，根据具体的模板样例填入已生成好的地块图片和书签信息

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        protected virtual void AgricultureStrandardLandProgress()
        {
            if (geoLandCollection == null || geoLandCollection.Count < 1)
            {
                return;
            }

            InitalizeAllEngleView();//全域图片的插入
            WriteBookmarkInfo();//填写模板书签信息
            WriteLandInfo();//插入地块图片信息
        }

        /// <summary>
        /// 显示所有全域鹰眼图
        /// </summary>
        protected virtual void InitalizeAllEngleView()
        {
            string fileName = SavePathOfImage + @"\" + Contractor.ZoneCode + "-" + Contractor.Name + ".jpg";
            if (System.IO.File.Exists(fileName))
            {
                if (SettingDefine.IsFixedViewOfAllLandGeoWordExtend)
                {
                    InsertImageCellWithoutPading(AgricultureBookMark.AgricultureAllShape, fileName, 180, 250);
                }
                else
                {
                    InsertImageCellWithoutPading(AgricultureBookMark.AgricultureAllShape, fileName, SettingDefine.ViewOfAllMultiParcelWitdh, SettingDefine.ViewOfAllMultiParcelHeight);
                }
            }
            System.IO.File.Delete(fileName);
        }

        /// <summary>
        ///填写书签信息
        /// </summary>
        protected virtual void WriteBookmarkInfo()
        {
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("Cartographer" + (i == 0 ? "" : i.ToString()), SettingDefine.Cartographer);
                SetBookmarkValue("CartographyDate" + (i == 0 ? "" : i.ToString()), (SettingDefine.CartographyDate == null) ? "" : SettingDefine.CartographyDate.Value.ToString("yyyy年MM月dd日"));
                SetBookmarkValue("CartographyUnit" + (i == 0 ? "" : i.ToString()), SettingDefine.CartographyUnit);
                SetBookmarkValue("CheckPerson" + (i == 0 ? "" : i.ToString()), SettingDefine.CheckPerson);
                SetBookmarkValue("CheckDate" + (i == 0 ? "" : i.ToString()), (SettingDefine.CheckDate == null) ? "" : SettingDefine.CheckDate.Value.ToString("yyyy年MM月dd日"));
            }
        }

        /// <summary>
        /// 填写地块信息
        /// </summary>
        protected virtual void WriteLandInfo()
        {
            ROW1 = 2;
            COL1 = 4;
            PAGECOUNT1 = ROW1 * COL1;

            // 示意图页数
            var landCount = geoLandCollection.Count;
            int pageSize = landCount > PAGECOUNT1 ?
                landCount % PAGECOUNT1 == 0 ?
                    landCount / PAGECOUNT1 :
                    landCount / PAGECOUNT1 + 1 :
                1;

            // 扩展页页数

            // 总页数
            var totalPageSize = pageSize;

            WriteLandInfoVertical(pageSize, totalPageSize);
        }

        /// <summary>
        /// 写地块信息（竖版）
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="otherInfo"></param>
        protected virtual void WriteLandInfoVertical(int pageSize, int totalPageSize)
        {
            // 复制页
            for (int i = 1; i < pageSize; i++)
            {
                AddSection();
            }

            for (int i = 0; i < pageSize; i++)
            {
                SetTableCellValue(i, 0, 8, 1, (i + 1).ToString() + "-" + totalPageSize.ToString());
                InitalizeLandImageInformation(
                    geoLandCollection.Skip(PAGECOUNT1 * i).Take(PAGECOUNT1).ToList(), i, 0, ROW1, COL1, 2, 1);
            }
        }

        /// <summary>
        /// 将示意图写入对应表格
        /// </summary>
        /// <param name="lands">地块</param>
        /// <param name="tableIndex">表格索引</param>
        /// <param name="row">表格行数</param>
        /// <param name="col">表格列数</param>
        /// <param name="isHorizontal">是否横版</param>
        private void InitalizeLandImageInformation(List<ContractLand> lands, int sectionIndex, int tableIndex, int row, int col, int startRow, int startCol, bool isHorizontal = false)
        {
            if (lands == null || lands.Count == 0)
            {
                return;
            }
            int landIndex = 0;
            if (isHorizontal)
            {
                for (int colInex = startRow; colInex < row + startRow; colInex++)
                {
                    for (int rowIndex = col - 1 + startCol; rowIndex >= startCol; rowIndex--)
                    {
                        if (landIndex >= lands.Count)
                        {
                            return;
                        }
                        ContractLand land = lands[landIndex];
                        InsertImageShape(land, sectionIndex, tableIndex, rowIndex, colInex);
                        landIndex++;
                    }
                }
            }
            else
            {
                for (int rowIndex = startRow; rowIndex < row + startRow; rowIndex++)
                {
                    for (int colInex = startCol; colInex < col + startCol; colInex++)
                    {
                        if (landIndex >= lands.Count)
                        {
                            return;
                        }
                        ContractLand land = lands[landIndex];
                        InsertImageShape(land, sectionIndex, tableIndex, rowIndex, colInex);
                        landIndex++;
                    }
                }
            }
        }

        private void InsertImageShape(ContractLand land, int sectionIndex, int tableIndex, int rowIndex, int columnIndex)
        {
            try
            {
                var uselandnumber = Regex.Replace(land.LandNumber, @"[^\d]", "_");
                string imagePath = SavePathOfImage + @"\" + uselandnumber + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    if (SettingDefine.IsFixedLandGeoWordExtend)
                    {
                        SetTableCellValue(sectionIndex, tableIndex, rowIndex, columnIndex, imagePath, 130, 175, false);
                    }
                    else
                    {
                        SetTableCellValue(sectionIndex, tableIndex, rowIndex, columnIndex, imagePath, SettingDefine.LandGeoWordWidth, SettingDefine.LandGeoWordHeight, false);
                    }
                }
                System.IO.File.Delete(imagePath);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 重新计算承包地块的总面积——既有确权地和确股地的农户，其承包地总面积=确权地的面积（确股的地中确权的部分也会统计对）
        /// </summary>
        private void RecalculateActualArea()
        {
            double conLandActualArea = 0.0;
            foreach (var land in geoLandCollection)
                conLandActualArea += land.AwareArea;
            SetBookmarkValue(AgricultureBookMark.AgricultureContractLandActualAreaCount, conLandActualArea.AreaFormat());//承包地块总实测面积
        }

        #endregion 提供重写方法，根据具体的模板样例填入已生成好的地块图片和书签信息

        #region Parcel

        /// <summary>
        /// 获取全地块示意图-本宗设置
        /// </summary>
        public void GetAllMultiParcel()
        {
            try
            {
                MapShapeUI map = null;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (SettingDefine.IsFixedViewOfAllLandGeoWordExtend)
                    {
                        ViewOfAllMultiParcel.Paper.Model.Width = 230;
                        ViewOfAllMultiParcel.Paper.Model.Height = 320;
                    }
                    else
                    {
                        ViewOfAllMultiParcel.Paper.Model.Width = SettingDefine.ViewOfAllMultiParcelWitdh;
                        ViewOfAllMultiParcel.Paper.Model.Height = SettingDefine.ViewOfAllMultiParcelHeight;
                    }
                }));
                var listAllFeature = new List<FeatureObject>();
                var listOwenrFeature = new List<FeatureObject>();
                var listdzdwFeature = new List<FeatureObject>();
                var listxzdwFeature = new List<FeatureObject>();
                var listmzdwFeature = new List<FeatureObject>();
                var listgroupZoneFeature = new List<FeatureObject>();
                var listvillageZoneFeature = new List<FeatureObject>();

                //当前地域下的所有空间地块图形导出
                DiagramBase diagram = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (ViewOfAllMultiParcel.Items.Count == 0)
                    {
                        diagram = new MapShape() { }.CreateDiagram();
                        ViewOfAllMultiParcel.Items.Add(diagram);
                        if (SettingDefine.IsFixedViewOfAllLandGeoWordExtend)
                        {
                            diagram.Model.Width = 190;
                            diagram.Model.Height = 210;
                        }
                        else
                        {
                            diagram.Model.Width = SettingDefine.ViewOfAllMultiParcelWitdh > 36 ? SettingDefine.ViewOfAllMultiParcelWitdh - 36 : SettingDefine.ViewOfAllMultiParcelWitdh;
                            diagram.Model.Height = SettingDefine.ViewOfAllMultiParcelHeight > 57 ? SettingDefine.ViewOfAllMultiParcelHeight - 57 : SettingDefine.ViewOfAllMultiParcelHeight;
                        }
                        diagram.Model.BorderWidth = 0;
                        diagram.Model.X = 15;
                        diagram.Model.Y = 10;
                    }
                    else
                    {
                        diagram = ViewOfAllMultiParcel.Items[0];
                    }
                    map = diagram.Content as MapShapeUI;
                    map.MapControl.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                }));
                int layerCountIndex = 0;
                DynamicGeoSource allGeos = null;
                DynamicGeoSource ownerGeos = null;
                if (SettingDefine.IsShowOtherVPallLands)
                    allGeos = GetSetALLOtherlands(listAllFeature, map, layerCountIndex++);
                if (SettingDefine.IsShowVPallLands)
                    ownerGeos = GetSetALLOwnerlands(listOwenrFeature, map, layerCountIndex++);
                if (SettingDefine.ShowdxmzdwHandle)
                {
                    GetSetALLdzdwGeos(listdzdwFeature, map, layerCountIndex++);
                    GetSetALLxzdwGeos(listxzdwFeature, map, layerCountIndex++);
                    GetSetALLmzdwGeos(listmzdwFeature, map, layerCountIndex++);
                }
                if (SettingDefine.IsShowGroupZoneBoundary)
                    GetSetGroupZoneGeos(listgroupZoneFeature, map, layerCountIndex++);
                if (SettingDefine.IsShowVillageZoneBoundary)
                    GetSetVillageZoneGeos(listvillageZoneFeature, map, layerCountIndex++);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    map.MapControl.SpatialReference = spatialReference;
                    Envelope geosFullExtend = null;
                    try
                    {
                        if (allGeos != null && SettingDefine.IsShowOtherVPallLands)
                        {
                            geosFullExtend = allGeos.FullExtend.ToGeometry().Union(allGeos.FullExtend.ToGeometry()).Buffer(10).GetEnvelope();
                        }
                        if ((ownerGeos != null && !SettingDefine.IsShowOtherVPallLands) ||
                        (ownerGeos != null && listAllFeature.Count == 0 && listOwenrFeature.Count > 0))
                        {
                            geosFullExtend = ownerGeos.FullExtend.ToGeometry().Buffer(10).GetEnvelope();
                        }
                        if (geosFullExtend != null)
                        {
                            map.MapControl.Extend = geosFullExtend;
                            map.MapControl.NavigateTo(geosFullExtend);
                            map.MapControl.Extend = geosFullExtend;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString(), "错误");
                        return;
                    }
                }));

                for (int mi = layerCountIndex; mi < map.MapControl.Layers.Count; mi++)
                {
                    var pointLyer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    var geoSource = pointLyer.DataSource as DynamicGeoSource;
                    geoSource.Clear();
                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var env = map.MapControl.Extend;
                    int itemindex = 1;
                    exportLandParcelMainOperation.AddMainCompass(ViewOfAllMultiParcel, diagram, itemindex++);    //添加主视图指北针
                    if (SettingDefine.IsShowViewOfAllScale)
                    {
                        exportLandParcelMainOperation.AddScaleText(ViewOfAllMultiParcel, diagram, env, itemindex++, SettingDefine.ViewOfAllScaleWH, true);    //添加比例尺文本标注
                    }

                    //保存为图片
                    var image = ViewOfAllMultiParcel.SaveToImage(3);
                    if (image == null)
                        return;
                    string fileName = SavePathOfImage + @"\" + Contractor.ZoneCode + "-" + Contractor.Name + ".jpg";
                    image.SaveToJpgFile(fileName);
                }));
                listAllFeature.Clear();
                listAllFeature = null;
                listOwenrFeature.Clear();
                listOwenrFeature = null;
                listdzdwFeature.Clear();
                listdzdwFeature = null;
                listxzdwFeature.Clear();
                listxzdwFeature = null;
                listmzdwFeature.Clear();
                listmzdwFeature = null;
            }
            catch (Exception ex)
            {
                throw new YltException("获取全地块示意图失败!" + ex.ToString());
            }
        }

        /// <summary>
        /// 获取邻地块示意图
        /// </summary>
        public void GetNeighorParcels()
        {
            mapW = 190;
            mapH = 220;

            //if (SettingDefine.IsFixedLandGeoWordExtend)
            //{
            //    mapW = 190;
            //    mapH = 220;
            //}
            //else
            //{
            //    mapW = SettingDefine.LandGeoWordWidth;
            //    mapH = SettingDefine.LandGeoWordHeight;
            //}
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ViewOfNeighorParcels.Paper.Model.Width = mapW;
                ViewOfNeighorParcels.Paper.Model.Height = mapH + 20;
            }));
            exportLandParcelMainOperation.mapH = mapH;
            exportLandParcelMainOperation.mapW = mapW;

            var visibleBounds = new CglEnvelope(0, 0, mapW, mapH + 20);
            try
            {
                MapShapeUI map = null;
                List<FeatureObject> listFeature = new List<FeatureObject>();
                exportLandParcelMainOperation.targetSpatialReference = spatialReference;
                var landStation = DbContext.CreateContractLandWorkstation();

                //获取当前集合地块的所有的邻宗地块信息，调用四至算法,是否有地块。
                SearchNeighborCommon snc = new SearchNeighborCommon();
                var neiborghinfodics = new Dictionary<string, Dictionary<string, bool>>();
                if (SettingDefine.ShowlandneighborLabel && SettingDefine.NeighborlandSearchUseUserAlgorithm == false)
                {
                    snc.GetCurrentZoneIntersects(CurrentZone, DbContext, SettingDefine.Neighborlandbufferdistence, geoLandCollection);

                    var queryLandList = new List<ContractLand>();
                    foreach (var geoLand in geoLandCollection)
                    {
                        List<ContractLand> tempLands = new List<ContractLand>();
                        if (geoLand.AliasNameD.IsNullOrEmpty() == false && geoLand.AliasNameD.Length > 0)
                        {
                            var landids = geoLand.AliasNameD.Split(',');
                            foreach (var item in landids)
                            {
                                var itemid = Guid.Parse(item);
                                if (itemid == null) continue;
                                var itemland = landStation.Get(itemid);
                                if (itemland == null) continue;

                                if (tempLands.Any(fdsa => fdsa.LandNumber == geoLand.LandNumber) == false)
                                {
                                    tempLands.Add(itemland);
                                }
                            }
                        }

                        if (queryLandList.Any(fdsa => fdsa.LandNumber == geoLand.LandNumber) == false)
                        {
                            queryLandList.Add(geoLand);
                        }

                        foreach (var item in tempLands)
                        {
                            if (queryLandList.Any(fdsa => fdsa.LandNumber == item.LandNumber) == false)
                            {
                                queryLandList.Add(item);
                            }
                        }
                    }

                    var landstr = snc.GetQueryString(queryLandList);
                    var xzdwstr = snc.GetXzdwXY(snc.CurrentZonexzdws);
                    var mzdwstr = snc.GetMzdwXY(snc.CurrentZonemzdws);
                    snc.CurrentZoneQueryLandList = queryLandList;
                    var queryResultStr = snc.QueryNeighborString(landstr + xzdwstr + mzdwstr, SettingDefine.Neighborlandbufferdistence);
                    neiborghinfodics = snc.InitializeNeighborInfo(queryLandList, queryResultStr);
                }

                int x = 0;//标注当前为第几个地块
                //各个空间地块的邻宗地图导出
                foreach (var geoLand in geoLandCollection)
                {
                    if (LstViewOfNeighorParcels != null && x < LstViewOfNeighorParcels.Count)
                    {
                        BitmapSource imageMap = null;
                        imageMap = LstViewOfNeighorParcels[x].SaveToImage(3);   //保存为图片
                        if (imageMap != null)
                        {
                            var uselandnumber = Regex.Replace(geoLand.LandNumber, @"[^\d]", "_");
                            string fileName = Path.Combine(SavePathOfImage, uselandnumber + ".jpg");

                            imageMap.SaveToJpgFile(fileName);
                        }
                        x++;
                        continue;
                    }
                    x++;

                    BitmapSource image = null;
                    DiagramBase diagram = null;
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (ViewOfNeighorParcels == null)
                        {
                            ViewOfNeighorParcels = new DiagramsView();
                        }
                        ViewOfNeighorParcels.Paper.Model.Width = visibleBounds.Width;// 136;
                        ViewOfNeighorParcels.Paper.Model.Height = visibleBounds.Height + 20;// 136;
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
                    YuLinTu.Spatial.Geometry geolandbuffershape = null;
                    if (geoLand.Shape != null && geoLand.Shape.IsValid())
                    {
                        if (SettingDefine.OwnerLandBufferType == "外边缓冲")
                        {
                            geolandbuffershape = geoLand.Shape.Buffer(SettingDefine.Neighborlandbufferdistence);
                        }
                        else if (SettingDefine.OwnerLandBufferType == "边框缓冲")
                        {
                            try
                            {
                                var shapeextend = geoLand.Shape.GetEnvelope().ToGeometry();
                                geolandbuffershape = shapeextend.Buffer(SettingDefine.Neighborlandbufferdistence);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString(), "错误");
                                return;
                            }
                        }
                    }

                    //var tempLands = landStation.GetIntersectLands(geoLand, geolandbuffershape);
                    // 避免每次获取相交地块，都去读整个地块的数据，只去和本村的数据作比较
                    //var tempLands = GetIntersectLands(geoLand, geolandbuffershape);
                    List<ContractLand> tempLands = new List<ContractLand>();
                    if (geoLand.AliasNameD.IsNullOrEmpty() == false && geoLand.AliasNameD.Length > 0)
                    {
                        var landids = geoLand.AliasNameD.Split(',');
                        foreach (var item in landids)
                        {
                            var itemid = Guid.Parse(item);
                            if (itemid == null) continue;
                            var itemland = landStation.Get(itemid);
                            if (itemland == null) continue;

                            tempLands.Add(itemland);
                        }
                    }
                    //else
                    //{
                    //    tempLands = landStation.GetIntersectLands(geoLand, geolandbuffershape);
                    //}
                    Dictionary<string, bool> landneighborhasland = new Dictionary<string, bool>();
                    if (SettingDefine.ShowlandneighborLabel && SettingDefine.NeighborlandSearchUseUserAlgorithm == false)
                    {
                        //if (VillageContractLands.Count > 0)
                        //{
                        //    GetlandneighborhasLandInfo(geoLand, landStation, landneighborhasland);
                        //}
                        //else
                        //{
                        //    exportLandParcelMainOperation.GetlandneighborhasLandInfo(geoLand, landStation, landneighborhasland);
                        //}
                        var getneiborhaslandinfo = neiborghinfodics.First(d => d.Key == geoLand.ID.ToString());
                        if (getneiborhaslandinfo.IsNull())
                        {
                            landneighborhasland.Clear();
                            landneighborhasland.Add("东至", false);
                            landneighborhasland.Add("南至", false);
                            landneighborhasland.Add("西至", false);
                            landneighborhasland.Add("北至", false);
                        }
                        else
                        {
                            landneighborhasland = getneiborhaslandinfo.Value;
                        }
                    }
                    else
                    {
                        exportLandParcelMainOperation.GetlandneighborhasLandInfo(geoLand, tempLands, landneighborhasland);
                    }
                    int layerCountIndex = 0;
                    if (SettingDefine.IsShowNeighborLandGeo)
                    {
                        exportLandParcelMainOperation.GetSetNeighborlandGeos(tempLands, geolandbuffershape, listFeature, map, layerCountIndex++);
                    }
                    listFeature.Clear();

                    //点线面状地物
                    var bufferLine = geoLand.Shape.Buffer(SettingDefine.Neighbordxmzdwbufferdistence);

                    var tempPoints = ListPointFeature.FindAll(c => c.Shape.Intersects(bufferLine));  //点状地物
                    var tempLines = ListLineFeature.FindAll(c => c.Shape.Intersects(bufferLine));  //线状地物
                    var tempPolygons = ListPolygonFeature.FindAll(c => c.Shape.Intersects(bufferLine));  //面状地物
                    exportLandParcelMainOperation.GetSetALLdxmzdwGeos(listFeature, tempPoints, tempLines, tempPolygons, map, layerCountIndex);
                    layerCountIndex = layerCountIndex + 3;

                    DynamicGeoSource selfGeods = null;  //本宗数据源
                    selfGeods = exportLandParcelMainOperation.GetSetNeighborlandGeo(geoLand, selfGeods, map, layerCountIndex++, false);

                    //自动过滤界址点
                    exportLandParcelMainOperation.FilterjzdNodes = exportLandParcelMainOperation.FilterNodeByValidDot(geoLand, ListValidDot);
                    //创建界址圈图层
                    if (SettingDefine.IsShowJZDGeo)
                    {
                        exportLandParcelMainOperation.AddNodeLayer(geoLand, tempLands, map, ListValidDot, layerCountIndex++);
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            map.MapControl.SpatialReference = spatialReference;
                            var geosFullExtend = selfGeods.FullExtend.ToGeometry().Buffer(10).GetEnvelope();
                            map.MapControl.Extend = geosFullExtend;
                            map.MapControl.NavigateTo(geosFullExtend);
                            map.MapControl.Extend = geosFullExtend;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString(), "错误");
                            return; ;
                        }
                    }));

                    for (int mi = layerCountIndex; mi < map.MapControl.Layers.Count; mi++)
                    {
                        var pointLyer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                        var geoSource = pointLyer.DataSource as DynamicGeoSource;
                        geoSource.Clear();
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        var env = map.MapControl.Extend;

                        var mElements = new MyElements(visibleBounds);
                        var index = 1;
                        var scaledgm = exportLandParcelMainOperation.AddScaleText(ViewOfNeighorParcels, diagram, env, index++);    //添加比例尺文本标注
                        mElements.AddElement(scaledgm);

                        var ce = exportLandParcelMainOperation.AddCenterLable(ViewOfNeighorParcels, diagram, selfGeods, env, geoLand, index++);   //添加中心文本标注
                        mElements.AddElement(ce);

                        exportLandParcelMainOperation.AddCompass(ViewOfNeighorParcels, diagram, index++);  //添加指北针

                        #region 调查四至

                        /*******************************************************************
                        * ShowAlllandneighborLabel——邻宗图标注直接使用调查四至；ShowlandneighborLabel——邻宗图无地块相邻显示调查四至
                        * NeighborlandLabeluseDLTGGQname——邻宗直接打印调查四至中田埂、道路、沟渠
                        *******************************************************************/

                        if (SettingDefine.ShowAlllandneighborLabel)//临宗标注直接使用调查四至
                        {
                            landneighborhasland.Clear();
                            landneighborhasland.Add("东至", false);
                            landneighborhasland.Add("南至", false);
                            landneighborhasland.Add("西至", false);
                            landneighborhasland.Add("北至", false);

                            var elements = exportLandParcelMainOperation.GetNeighborDiagramBases(ViewOfNeighorParcels, geoLand, index++, landneighborhasland); //添加调查四至文本标注
                            foreach (var element in elements)
                            {
                                exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                mElements.AddElement(element);
                            }
                            index = index + elements.Count - 1;
                        }
                        else
                        {
                            foreach (var tempitem in tempLands)
                            {
                                var landOwnerName = tempitem.OwnerName;
                                if (landOwnerName.IsNullOrEmpty()) continue;
                                var element = exportLandParcelMainOperation.AddNeiberLandlabel(ViewOfNeighorParcels, diagram, env, tempitem, geoLand, index++);  //添加临宗文本标注
                                if (element == null)
                                {
                                    index--;
                                    continue;
                                }
                                exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                mElements.AddElement(element);
                            }

                            foreach (var tempitem in tempPoints)//点状标注
                            {
                                var dzdwname = tempitem.DWMC;
                                if (dzdwname.IsNullOrEmpty()) continue;
                                var element = exportLandParcelMainOperation.AddNeiberDzdwlabel(ViewOfNeighorParcels, diagram, env, tempitem, geoLand, index++);
                                if (element == null)
                                {
                                    index--;
                                    continue;
                                }
                                exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                mElements.AddElement(element);
                            }
                            foreach (var tempitem in tempLines)//线状标注
                            {
                                var xzdwname = tempitem.DWMC;
                                if (xzdwname.IsNullOrEmpty()) continue;
                                var element = exportLandParcelMainOperation.AddNeiberXzdwlabel(ViewOfNeighorParcels, diagram, env, tempitem, geoLand, index++);
                                if (element == null)
                                {
                                    index--;
                                    continue;
                                }
                                exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                mElements.AddElement(element);
                            }
                            foreach (var tempitem in tempPolygons)//面状标注
                            {
                                var mzdwname = tempitem.DWMC;
                                if (mzdwname.IsNullOrEmpty()) continue;
                                var element = exportLandParcelMainOperation.AddNeiberMzdwlabel(ViewOfNeighorParcels, diagram, env, tempitem, geoLand, index++);
                                if (element == null)
                                {
                                    index--;
                                    continue;
                                }
                                exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                mElements.AddElement(element);
                            }
                        }

                        if (SettingDefine.ShowlandneighborLabel && SettingDefine.ShowAlllandneighborLabel == false)//如果没有临宗，就把对应的调查四至打到临宗四个地方
                        {
                            var elements = exportLandParcelMainOperation.GetNeighborDiagramBases(ViewOfNeighorParcels, geoLand, index++, landneighborhasland); //添加调查四至文本标注
                            foreach (var element in elements)
                            {
                                exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                mElements.AddElement(element);
                            }
                            index = index + elements.Count - 1;
                        }

                        if (SettingDefine.NeighborlandLabeluseDLTGGQname && SettingDefine.ShowAlllandneighborLabel == false)//获取四至临宗直接打印调查四至中田埂、道路、沟渠
                        {
                            var elements = exportLandParcelMainOperation.GetNeighborDiagramBaseUSETGDLGQs(ViewOfNeighorParcels, geoLand, index++);
                            foreach (var element in elements)
                            {
                                exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                mElements.AddElement(element);
                            }
                            index = index + elements.Count - 1;
                        }

                        #endregion 调查四至

                        if (SettingDefine.IsShowJZDNumber)
                        {
                            if (exportLandParcelMainOperation.FilterjzdNodes != null)
                            {
                                foreach (var tempitem in exportLandParcelMainOperation.FilterjzdNodes)
                                {
                                    var temitemdot = ListValidDot.Find(ld => ld.Shape.Equals(tempitem) && ld.LandNumber == geoLand.LandNumber);//坐标都一样，才算一样
                                    if (temitemdot == null) continue;
                                    //var temitemdot = temitemdots.Find(tp => tp.LandNumber == geoLand.LandNumber);//地块编码不一样反而有问题。
                                    //if (temitemdot == null) continue;
                                    var element = exportLandParcelMainOperation.AddNeiberJzdNumberlabel(ViewOfNeighorParcels, diagram, env, temitemdot, geoLand, index++, SettingDefine.IsUniteJZDNumber);  //添加界址点号标注
                                    if (element == null)
                                    {
                                        index--;
                                        continue;
                                    }
                                    exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                    mElements.AddElement(element);
                                }
                            }
                        }

                        for (int i = index; i < ViewOfNeighorParcels.Items.Count; i++)
                        {
                            (ViewOfNeighorParcels.Items[i].Model as TextShape).Text = null;
                        }
                        image = ViewOfNeighorParcels.SaveToImage(3);   //保存为图片
                        if (image != null)
                        {
                            var uselandnumber = Regex.Replace(geoLand.LandNumber, @"[^\d]", "_");
                            string fileName = SavePathOfImage + @"\" + uselandnumber + ".jpg";

                            image.SaveToJpgFile(fileName);
                        }
                    }));
                    selfGeods.Clear();
                    selfGeods = null;
                    listFeature.Clear();
                }
            }
            catch (Exception ex)
            {
                throw new YltException("获取邻地块示意图失败!" + ex.ToString());
            }
        }

        /// <summary>
        /// 获取临宗四至是否有临接地块
        /// </summary>
        private void GetlandneighborhasLandInfo(ContractLand targetland, YuLinTu.Library.WorkStation.IContractLandWorkStation landstation, Dictionary<string, bool> landneighborhasland)
        {
            var targetlandCenterPoint = targetland.Shape.Instance.PointOnSurface;
            landneighborhasland.Add("东至", true);
            landneighborhasland.Add("南至", true);
            landneighborhasland.Add("西至", true);
            landneighborhasland.Add("北至", true);
            if (targetland.Shape == null || targetlandCenterPoint.IsEmpty)
            {
                return;
            }
            int geoSrid = targetland.Shape.Srid;
            var extendGeo = targetland.Shape.GetEnvelope();

            List<YuLinTu.Spatial.Geometry> currentlandpolyline = targetland.Shape.ToPolylines().ToList();
            var currentlandtargetline = currentlandpolyline[0];//外环

            double bufferDistence = SettingDefine.Neighborlandbufferdistence;//缓冲距离
            //东北框点坐标
            Coordinate extendNorthEastCdt = new Coordinate(extendGeo.MaxX, extendGeo.MaxY);
            //西北框点坐标
            Coordinate extendNorthWestCdt = new Coordinate(extendGeo.MinX, extendGeo.MaxY);
            //东南框点坐标
            Coordinate extendSouthEastCdt = new Coordinate(extendGeo.MaxX, extendGeo.MinY);
            //西南框点坐标
            Coordinate extendSouthWestCdt = new Coordinate(extendGeo.MinX, extendGeo.MinY);

            //当前地块中心点坐标
            Coordinate currentLandExtendCenterCdt = new Coordinate(targetlandCenterPoint.X, targetlandCenterPoint.Y);

            //北至
            //缩小搜索框
            Coordinate newextendNorthinterCdt = new Coordinate(targetlandCenterPoint.X, extendGeo.MaxY + bufferDistence);
            YuLinTu.Spatial.Geometry northtrianglegeo = YuLinTu.Spatial.Geometry.CreatePoint(newextendNorthinterCdt, geoSrid);

            var northintersectlands = GetIntersectLands(targetland, northtrianglegeo);
            if (northintersectlands.Count == 0)
            {
                landneighborhasland["北至"] = false;
            }

            //东边
            Coordinate newextendEastinterCdt = new Coordinate(extendGeo.MaxX + bufferDistence, targetlandCenterPoint.Y);
            YuLinTu.Spatial.Geometry easttrianglegeo = YuLinTu.Spatial.Geometry.CreatePoint(newextendEastinterCdt, geoSrid);

            var eastintersectlands = GetIntersectLands(targetland, easttrianglegeo);
            if (eastintersectlands.Count == 0)
            {
                landneighborhasland["东至"] = false;
            }

            //西至
            Coordinate newextendWestinterCdt = new Coordinate(extendGeo.MinX - bufferDistence, targetlandCenterPoint.Y);
            YuLinTu.Spatial.Geometry westtrianglegeo = YuLinTu.Spatial.Geometry.CreatePoint(newextendWestinterCdt, geoSrid);
            var westintersectlands = GetIntersectLands(targetland, westtrianglegeo);
            if (westintersectlands.Count == 0)
            {
                landneighborhasland["西至"] = false;
            }

            //南至
            Coordinate newextendSouthinterCdt = new Coordinate(targetlandCenterPoint.X, extendGeo.MinY - bufferDistence);
            YuLinTu.Spatial.Geometry southtrianglegeo = YuLinTu.Spatial.Geometry.CreatePoint(newextendSouthinterCdt, geoSrid);

            var southintersectlands = GetIntersectLands(targetland, southtrianglegeo);
            if (southintersectlands.Count == 0)
            {
                landneighborhasland["南至"] = false;
            }
        }

        /// <summary>
        /// 根据目标地块，返回相交地块集合，不包括传入的地块
        /// </summary>
        /// <param name="tagetLand"></param>
        /// <returns></returns>
        private List<ContractLand> GetIntersectLands(ContractLand tagetLand, YuLinTu.Spatial.Geometry tagetLandShape)
        {
            List<ContractLand> intersectLands = new List<ContractLand>();
            if (tagetLand.Shape == null || tagetLandShape == null || tagetLandShape.IsValid() == false)
            {
                return intersectLands;
            }
            var results = VillageContractLands.Where(c => c.Shape.Intersects(tagetLandShape) && c.ID != tagetLand.ID).ToList();
            if (results != null) intersectLands = results;
            return intersectLands;
        }

        #endregion Parcel

        #region 全域所有信息获取与设置

        /// <summary>
        /// 是否全组只有一户人家
        /// </summary>
        /// <returns></returns>
        private bool isOnlyHaveOneFamily()
        {
            bool flag = true;
            if (ListGeoLand.Count == geoLandCollection.Count)
            {
                foreach (var land in ListGeoLand)
                {
                    if (geoLandCollection.All(t => t.ID != land.ID))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        ///  全域获取设置临宗地块
        /// </summary>
        private DynamicGeoSource GetSetALLOtherlands(List<FeatureObject> listAllFeature, MapShapeUI map, int layerCountIndex)
        {
            //首先创建邻宗图层
            foreach (var land in ListGeoLand)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = land;
                fo.Geometry = land.Shape;
                fo.GeometryPropertyName = "Shape";
                if (geoLandCollection.Contains(land) == false)
                    listAllFeature.Add(fo);
            }
            if (listAllFeature.Count == 0)
            {
                // 如果一组只有一家人，则临宗地块为自己
                if (isOnlyHaveOneFamily())
                {
                    foreach (var land in ListGeoLand)
                    {
                        FeatureObject fo = new FeatureObject();
                        fo.Object = land;
                        fo.Geometry = land.Shape;
                        fo.GeometryPropertyName = "Shape";
                        if (geoLandCollection.Contains(land) == true)
                            listAllFeature.Add(fo);
                    }
                }
                else
                {
                    return null;
                }
            }
            DynamicGeoSource allGeo = null;  //邻宗数据源
            VectorLayer lyer = null;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyer = new VectorLayer();  //创建邻宗矢量图层
                    lyer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 255, 255, 255),
                        BorderStrokeColor = Color.FromArgb(255, 0, 255, 64),
                        BorderThickness = 1
                    });

                    allGeo = new DynamicGeoSource();
                    allGeo.AddRange(listAllFeature.ToArray());
                    lyer.DataSource = allGeo;
                    map.MapControl.Layers.Add(lyer);
                }
                else
                {
                    lyer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    allGeo = lyer.DataSource as DynamicGeoSource;
                    allGeo.Clear();
                    allGeo.AddRange(listAllFeature.ToArray());
                }

                ((lyer.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = SettingDefine.ViewOfAllOthervpLandColor;
                ((lyer.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = SettingDefine.ViewOfAllOthervpLandBorderWidth;
            }));
            return allGeo;
        }

        /// <summary>
        /// 本宗大区获取设置本宗地块
        /// </summary>
        /// <param name="listAllFeature"></param>
        /// <param name="map"></param>
        private DynamicGeoSource GetSetALLOwnerlands(List<FeatureObject> listOwenrFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建本宗图层
            foreach (var child in geoLandCollection)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = child;
                fo.Geometry = child.Shape;
                fo.GeometryPropertyName = "Shape";
                listOwenrFeature.Add(fo);
            }

            DynamicGeoSource ownerGeo = null;  //本宗数据源
            VectorLayer lyerOwnerExtent = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerOwnerExtent = new VectorLayer();   //创建本宗矢量图层
                    lyerOwnerExtent.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerOwnerExtent.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 255, 62, 62),
                        BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                        BorderThickness = 1
                    });
                    lyerOwnerExtent.Labeler = new SimpleExpressionLabeler(new SimpleTextPolygonSymbolPerFeature()
                    {
                        FontSize = 13,
                        AllowOverlapping = true,
                        AllowTextOverflow = true,
                    })
                    {
                        LabelExpression = "LandNumber == null? LandNumber: (LandNumber.Length>5? Int32.Parse(LandNumber.Substring(LandNumber.Length-5,5)).ToString() : Int32.Parse(LandNumber).ToString() )",
                    };
                    ownerGeo = new DynamicGeoSource();
                    ownerGeo.AddRange(listOwenrFeature.ToArray());
                    lyerOwnerExtent.DataSource = ownerGeo;
                    map.MapControl.Layers.Add(lyerOwnerExtent);
                }
                else if (map.MapControl.Layers.Count > 0)
                {
                    lyerOwnerExtent = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    ownerGeo = lyerOwnerExtent.DataSource as DynamicGeoSource;
                    ownerGeo.Clear();
                    ownerGeo.AddRange(listOwenrFeature.ToArray());
                }
                if (lyerOwnerExtent != null)
                {
                    ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).FontFamily = SettingDefine.ViewOfAllLabelFontSet;
                    ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).ForegroundColor = SettingDefine.ViewOfAllLabelFontColor;
                    ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).FontSize = SettingDefine.ViewOfAllLabelFontSize;
                    var nowFontStyle = ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).FontStyle;

                    if (SettingDefine.ViewOfAllLabelBold)
                        nowFontStyle = nowFontStyle | eFontStyle.Bold;
                    if (SettingDefine.ViewOfAllLabelUnderLine)
                        nowFontStyle = nowFontStyle | eFontStyle.Underline;
                    if (SettingDefine.ViewOfAllLabeltiltLine)
                        nowFontStyle = nowFontStyle | eFontStyle.Italic;
                    if (SettingDefine.ViewOfAllLabelStrikeLine)
                        nowFontStyle = nowFontStyle | eFontStyle.Strikeout;

                    ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).FontStyle = nowFontStyle;
                    (lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Enabled = SettingDefine.IsShowViewOfAllLabel;
                    ((lyerOwnerExtent.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BackgroundColor = SettingDefine.ViewOfAllvpLandColor;
                    ((lyerOwnerExtent.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = SettingDefine.ViewOfAllvpLandStrokeColor;
                    ((lyerOwnerExtent.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = SettingDefine.ViewOfAllvpLandBorderWidth;
                }
            }));

            return ownerGeo;
        }

        /// <summary>
        /// 本宗大区获取设置点状要素
        /// </summary>
        /// <param name="listdzdwFeature"></param>
        /// <param name="map"></param>
        private void GetSetALLdzdwGeos(List<FeatureObject> listdzdwFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建点状地物图层
            foreach (var child in ListPointFeature)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = child;
                fo.Geometry = child.Shape;
                fo.GeometryPropertyName = "Shape";
                listdzdwFeature.Add(fo);
            }
            DynamicGeoSource dzdwGeo = null;  //点状地物数据源
            VectorLayer lyerdzdw = null;
            //点状地物
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerdzdw = new VectorLayer();  //创建邻宗矢量图层
                    lyerdzdw.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerdzdw.Renderer = new SimpleRenderer(new SimplePointSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 0.3,
                    });
                    lyerdzdw.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
                    {
                        FontSize = 10
                    })
                    {
                        Enabled = false,
                        LabelProperty = "OwnerName"
                    };
                    dzdwGeo = new DynamicGeoSource();
                    dzdwGeo.AddRange(listdzdwFeature.ToArray());
                    lyerdzdw.DataSource = dzdwGeo;
                    map.MapControl.Layers.Add(lyerdzdw);
                }
                else
                {
                    lyerdzdw = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    dzdwGeo = lyerdzdw.DataSource as DynamicGeoSource;
                    dzdwGeo.Clear();
                    dzdwGeo.AddRange(listdzdwFeature.ToArray());
                }
            }));
        }

        private void GetSetALLxzdwGeos(List<FeatureObject> listxzdwFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建线状地物图层
            foreach (var child in ListLineFeature)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = child;
                fo.Geometry = child.Shape;
                fo.GeometryPropertyName = "Shape";
                listxzdwFeature.Add(fo);
            }

            DynamicGeoSource xzdwGeo = null;  //线状地物数据源
            VectorLayer lyerxzdw = null;
            //线状地物
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerxzdw = new VectorLayer();  //创建邻宗矢量图层
                    lyerxzdw.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerxzdw.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                    {
                        StrokeColor = Color.FromArgb(255, 158, 148, 112),
                        StrokeThickness = 1,
                    });
                    lyerxzdw.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
                    {
                        FontSize = 10
                    })
                    {
                        Enabled = false,
                        LabelProperty = "OwnerName"
                    };
                    xzdwGeo = new DynamicGeoSource();
                    xzdwGeo.AddRange(listxzdwFeature.ToArray());
                    lyerxzdw.DataSource = xzdwGeo;
                    map.MapControl.Layers.Add(lyerxzdw);
                }
                else
                {
                    lyerxzdw = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    xzdwGeo = lyerxzdw.DataSource as DynamicGeoSource;
                    xzdwGeo.Clear();
                    xzdwGeo.AddRange(listxzdwFeature.ToArray());
                }
            }));
        }

        private void GetSetALLmzdwGeos(List<FeatureObject> listmzdwFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建面状地物图层
            foreach (var child in ListPolygonFeature)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = child;
                fo.Geometry = child.Shape;
                fo.GeometryPropertyName = "Shape";
                listmzdwFeature.Add(fo);
            }
            DynamicGeoSource mzdwGeo = null;  //面状地物数据源
            VectorLayer lyermzdw = null;
            //面状地物
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyermzdw = new VectorLayer();  //创建邻宗矢量图层
                    lyermzdw.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyermzdw.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 239, 235, 219),
                        BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                        BorderThickness = 1,
                    });
                    mzdwGeo = new DynamicGeoSource();
                    mzdwGeo.AddRange(listmzdwFeature.ToArray());
                    lyermzdw.DataSource = mzdwGeo;
                    map.MapControl.Layers.Add(lyermzdw);
                }
                else
                {
                    lyermzdw = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    mzdwGeo = lyermzdw.DataSource as DynamicGeoSource;
                    mzdwGeo.Clear();
                    mzdwGeo.AddRange(listmzdwFeature.ToArray());
                }
            }));
        }

        /// <summary>
        /// 本宗大区获取设置组级地域边界
        /// </summary>
        /// <param name="listdzdwFeature"></param>
        /// <param name="map"></param>
        private void GetSetGroupZoneGeos(List<FeatureObject> listgroupZoneFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建组级地物图层
            FeatureObject fo = new FeatureObject();
            fo.Object = CurrentZone;
            fo.Geometry = CurrentZone.Shape;
            fo.GeometryPropertyName = "Shape";
            listgroupZoneFeature.Add(fo);

            DynamicGeoSource zjdyGeo = null;  //组级地域数据源
            VectorLayer lyerzjdy = null;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerzjdy = new VectorLayer();  //创建邻宗矢量图层
                    lyerzjdy.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerzjdy.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Colors.Transparent,
                        //BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderStrokeColor = Colors.Red,
                        BorderThickness = 2,
                    });
                    zjdyGeo = new DynamicGeoSource();
                    zjdyGeo.AddRange(listgroupZoneFeature.ToArray());
                    lyerzjdy.DataSource = zjdyGeo;
                    map.MapControl.Layers.Add(lyerzjdy);
                }
                else if (map.MapControl.Layers.Count > 0)
                {
                    lyerzjdy = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    zjdyGeo = lyerzjdy.DataSource as DynamicGeoSource;
                    zjdyGeo.Clear();
                    zjdyGeo.AddRange(listgroupZoneFeature.ToArray());
                }
                if (lyerzjdy != null)
                {
                    ((lyerzjdy.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = SettingDefine.GroupBoundaryBorderColor;
                    ((lyerzjdy.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = SettingDefine.GroupZoneBoundaryBorderWidth;
                }
            }));
        }

        /// <summary>
        /// 本宗大区获取设置村级地域边界
        /// </summary>
        /// <param name="listdzdwFeature"></param>
        /// <param name="map"></param>
        private void GetSetVillageZoneGeos(List<FeatureObject> listvillageZoneFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建村级地物图层
            FeatureObject fo = new FeatureObject();
            fo.Object = VillageZone;
            fo.Geometry = VillageZone.Shape;
            fo.GeometryPropertyName = "Shape";
            listvillageZoneFeature.Add(fo);

            DynamicGeoSource cjdyGeo = null;  //村级地域数据源
            VectorLayer lyercjdy = null;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyercjdy = new VectorLayer();  //创建邻宗矢量图层
                    lyercjdy.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyercjdy.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Colors.Transparent,
                        //BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderStrokeColor = Colors.Blue,
                        BorderThickness = 2,
                    });
                    cjdyGeo = new DynamicGeoSource();
                    cjdyGeo.AddRange(listvillageZoneFeature.ToArray());
                    lyercjdy.DataSource = cjdyGeo;
                    map.MapControl.Layers.Add(lyercjdy);
                }
                else if (map.MapControl.Layers.Count > 0)
                {
                    lyercjdy = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    cjdyGeo = lyercjdy.DataSource as DynamicGeoSource;
                    cjdyGeo.Clear();
                    cjdyGeo.AddRange(listvillageZoneFeature.ToArray());
                }
                if (lyercjdy != null)
                {
                    ((lyercjdy.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = SettingDefine.VillageZoneBoundaryBorderColor;
                    ((lyercjdy.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = SettingDefine.VillageZoneBoundaryBorderWidth;
                }
            }));
        }

        #endregion 全域所有信息获取与设置

        #endregion Methods
    }
}