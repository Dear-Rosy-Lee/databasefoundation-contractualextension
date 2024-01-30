using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using Aspose.Cells;
using YuLinTu.Data;
using YuLinTu.NetAux.CglLib;
using YuLinTu.Diagrams;
using YuLinTu.tGIS;
using System.Windows.Media.Imaging;
using System.Windows;
using YuLinTu.tGIS.Data;
using YuLinTu.tGIS.Client;
using System.Windows.Media;
using YuLinTu.NetAux;
using YuLinTu.Spatial;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 农村土地承包经营权登记调查确认书
    /// </summary>
    public class ExportFamilySurveyBook : ExcelBase
    {
        #region Fields
        private bool useActualArea;//是否使用实测面积
        private string errorInformation = string.Empty;//错误信息   
        private string fileName = string.Empty;//文件名称    
        private GetDictionary dic;//字典
        private int currentIndex;
        private const double mapW = 136;
        private const double mapH = 120;
        #endregion

        #region Properties
        /// <summary>
        /// 承包合同
        /// </summary>
        public ContractConcord Concord { get; set; }
        /// <summary>
        /// 承包地集合
        /// </summary>
        public List<ContractLand> Lands { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson VirtualPerson { get; set; }
        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }
        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Tissue { get; set; }
        /// <summary>
        /// 线状地物(当前地域)
        /// </summary>
        public List<XZDW> ListLineFeature { get; set; }
        public DiagramsView ViewOfNeighorParcels { get; set; }
        public IDbContext DbContext { get; set; }
        #endregion

        #region Ctor

        public ExportFamilySurveyBook(string finame, string dictoryname)
        {
            fileName = finame;
            dic = new GetDictionary(dictoryname);
            dic.Read();
            currentIndex = 7;
        }

        #endregion

        #region Methods

        #region Override
        public override void Read()
        {

        }
        /// <summary>
        /// 写方法
        /// </summary>
        public override void Write()
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || !OpenFamilyFile(fileName) || !CheckDataInformation())
                {
                    return;
                }
                GetNeighorParcels();               //生成需要的地块图
                useActualArea = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "true");
                Boolean.TryParse(value, out useActualArea);//使用实测面积作为颁证面积               
                WriteTitleInformation();
                WriteConcordInformation();
                Disponse();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                errorInformation = ex.ToString();
                return;
            }
            return;
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        private bool OpenFamilyFile(string fname)
        {
            try
            {
                Open(fname, 0);//打开文件
                return true;
            }
            catch
            {
                errorInformation = "打开文件失败";
                return false;
            }
        }
        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CheckDataInformation()
        {
            //检查合同数据
            if (VirtualPerson == null)
            {
                errorInformation = "请选择承包方";
                return false;
            }
            if (Concord == null)
            {
                errorInformation = "合同数据为空";
                return false;
            }
            Lands = SortLandCollection(Lands);
            //检查地域数据
            if (CurrentZone == null)
            {
                errorInformation = "行政地域为空";
                return false;
            }
            if (Tissue == null)
                Tissue = new CollectivityTissue { };
            if (dic == null)
            {
                errorInformation = "请检查字典，字典为空";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            VirtualPerson = null;
            CurrentZone = null;
            Lands = null;
            GC.Collect();
        }

        #endregion

        #region Family

        /// <summary>
        /// 填写承包方信息
        /// </summary>
        private void WitePersonInformaion()
        {
            List<Person> spersons = VirtualPerson.SharePersonList;//得到户对应的共有人
            PersonCollection persons = SortSharePerson(spersons, VirtualPerson.Name);//排序共有人，并返回人口集合
            string name = "";
            foreach (var item in persons)
            {
                name += item.Name;
                name += "、";
            }
            WriteSharePersonValue();
            persons.Clear();
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        private void WriteSharePersonValue()
        {

            if (VirtualPerson.FamilyExpand != null &&  VirtualPerson.FamilyExpand.ConstructMode != eConstructMode.Family)
            {
                currentIndex += 7;
                return;

            }
            List<Person> spersons = VirtualPerson.SharePersonList;//得到户对应的共有人
            object obj = GetRangeToValue("A" + currentIndex, "F" + currentIndex);
            if (obj != null)
            {
                string[] percount = obj.ToString().Split('（');
                string value = "";
                if (percount.Count() < 2)
                {
                    percount = obj.ToString().Split('(');
                    value = percount[0] + "(" + spersons.Count + percount[1];
                }
                else
                    value = percount[0] + "（" + spersons.Count + percount[1];
                InitalizeRangeValue("A" + currentIndex, "F" + currentIndex, value);
            }
            currentIndex++;
            InitalizeRangeValue("A" + currentIndex, "F" + currentIndex, "共(" + spersons.Count + ")人");//共多少人

            PersonCollection sharePersons = SortSharePerson(spersons, VirtualPerson.Name);//排序共有人，并返回人口集合
            currentIndex += 2;
            if (sharePersons.Count > 1)
                InsertRowCell(currentIndex, sharePersons.Count - 1);
            foreach (Person person in sharePersons)
            {
                InitalizeRangeValue("A" + currentIndex, "A" + currentIndex, person.Name);//姓名
                InitalizeRangeValue("B" + currentIndex, "C" + currentIndex, person.Relationship);//家庭关系 
                InitalizeRangeValue("D" + currentIndex, "D" + currentIndex, person.ICN);//身份证号码 
                InitalizeRangeValue("E" + currentIndex, "F" + currentIndex, person.Comment);//备注 
                currentIndex++;
            }
            currentIndex = currentIndex + (sharePersons.Count < 1 ? 4 : 3);
        }

        /// <summary>
        /// 对共有人排序
        /// </summary>
        /// <param name="personCollection"></param>
        /// <param name="houseName"></param>
        /// <returns></returns>
        private PersonCollection SortSharePerson(List<Person> personCollection, string houseName)
        {
            PersonCollection sharePersonCollection = new PersonCollection();
            foreach (Person person in personCollection)
            {
                if (person.Name == houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            foreach (Person person in personCollection)
            {
                if (person.Name != houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            personCollection.Clear();
            return sharePersonCollection;
        }

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <returns></returns>
        private string GetGender()
        {
            List<Person> spersons = VirtualPerson.SharePersonList;//得到户对应的共有人
            PersonCollection sharePersons = SortSharePerson(spersons, VirtualPerson.Name);//排序共有人，并返回人口集合
            string value = EnumNameAttribute.GetDescription(sharePersons[0].Gender);
            string sex = value == EnumNameAttribute.GetDescription(eGender.Unknow) ? "" : value;
            return " " + sex + " ";
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <returns></returns>
        private string GetAge()
        {
            List<Person> spersons = VirtualPerson.SharePersonList;//得到户对应的共有人
            PersonCollection persons = SortSharePerson(spersons, VirtualPerson.Name);//排序共有人，并返回人口集合
            if (persons.Count == 0)
            {
                return "";
            }
            Person person = persons[0].Clone() as Person;
            if (person.Birthday != null && person.Birthday.HasValue && person.Birthday.Value.Date == DateTime.Today.Date)
            {
                person.Birthday = ToolICN.GetBirthday(person.ICN);
            }
            int age = person.GetAge();
            person = null;
            if (age < 1 || age > 200)
            {
                return "     ";
            }
            else
            {
                return age.ToString();
            }
        }

        #endregion

        #region Contractland

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        private void WriteLandInformation()
        {
            if (Lands == null || Lands.Count < 1)
            {
                return;
            }
            Range org = workSheet.Cells.CreateRange("A" + currentIndex, "F" + (currentIndex + 3));
            if (Lands.Count > 1)
            {
                int index = currentIndex;
                for (int i = 1; i < Lands.Count; i++)
                {
                    index += 4;
                    InsertRowCell(index - 1, 4);
                    Range tar = workSheet.Cells.CreateRange("A" + index, "F" + (index + 4));
                    tar.Copy(org);
                    tar.CopyStyle(org);
                }
            }
            #region wirte
            foreach (var item in Lands)
            {
                string landNumber = ContractLand.GetLandNumber(item.CadastralNumber);

                if (landNumber.Length > AgricultureSetting.AgricultureLandNumberMedian)
                {
                    landNumber = landNumber.Substring(AgricultureSetting.AgricultureLandNumberMedian);
                }
                InitalizeRangeValue("A" + currentIndex, "A" + (currentIndex + 3), item.Name + "\n" + landNumber);//地块编号名称
                InitalizeRangeValue("C" + currentIndex, "C" + currentIndex, item.NeighborEast);//东
                InitalizeRangeValue("F" + currentIndex, "F" + (currentIndex + 1), item.TableArea.Value.ToString("f2"));//合同面积
                currentIndex++;
                InitalizeRangeValue("C" + currentIndex, "C" + currentIndex, item.NeighborSouth);//南
                currentIndex++;
                InitalizeRangeValue("C" + currentIndex, "C" + currentIndex, item.NeighborWest);//西
                InitalizeRangeValue("F" + currentIndex, "F" + currentIndex, item.ActualArea.ToString("f2"));//实测面积
                currentIndex++;
                InitalizeRangeValue("C" + currentIndex, "C" + currentIndex, item.NeighborNorth);//北
                InitalizeRangeValue("F" + currentIndex, "F" + currentIndex, item.AwareArea.ToString("f2"));//确权面积

                string imagePath = FilePath + ContractLand.GetLandNumber(item.LandNumber) + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    InitalizeRangeImage("D" + (currentIndex - 3), "D" + currentIndex, imagePath, 120, 100);
                }
                System.IO.File.Delete(imagePath);
                currentIndex += 1;
            }
            currentIndex += 5;
            double tablearea = 0.0, aware = 0.0, actularea = 0.0;
            Lands.ForEach(c => tablearea += c.TableArea.Value);
            Lands.ForEach(c => aware += c.AwareArea);
            Lands.ForEach(c => actularea += c.ActualArea);

            InitalizeRangeValue("A" + currentIndex, "A" + currentIndex, "地块数量：" + Lands.Count);
            InitalizeRangeValue("B" + currentIndex, "C" + currentIndex, "合同面积总数：" + tablearea.ToString("f2"));
            InitalizeRangeValue("D" + currentIndex, "D" + currentIndex, "测量面积总数：" + actularea.ToString("f2"));
            InitalizeRangeValue("E" + currentIndex, "F" + currentIndex, "确权面积总数：" + aware.ToString("f2"));
            currentIndex += 3;
            DateTime now = DateTime.Now;
            InitalizeRangeValue("A" + currentIndex, "F" + currentIndex, now.Year + "年" + now.Month + "月" + now.Day + "日");
            #endregion
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<ContractLand> SortLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
                int index = landNumber.IndexOf("J");
                if (index < 0)
                {
                    index = landNumber.IndexOf("Q");
                }
                if (index > 0)
                {
                    landNumber = landNumber.Substring(index + 1);
                }
                Int32.TryParse(landNumber, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            List<ContractLand> landCollection = new List<ContractLand>();
            foreach (var land in orderdVps)
            {
                eConstructMode constructMode = new eConstructMode();
                Enum.TryParse<eConstructMode>(land.ConstructMode, out constructMode);
                if (constructMode != eConstructMode.Family)
                {
                    continue;
                }
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }

        /// <summary>
        /// 写开垦地信息
        /// </summary>
        private void WriteReclamationInformation()
        {
            List<ContractLand> landCollection = Lands.Clone() as List<ContractLand>;
            List<ContractLand> landArray = landCollection.FindAll(ld => (!string.IsNullOrEmpty(ld.Comment) && ld.Comment.IndexOf("开垦地") >= 0));
            double reclamationTableArea = 0.0;//开垦地台帐面积
            double reclamationActualArea = 0.0;//开垦地实测面积
            double reclamationAwareArea = 0.0;//开垦地确权面积
            foreach (ContractLand land in landArray)
            {
                reclamationTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                reclamationActualArea += land.ActualArea;
                reclamationAwareArea += land.AwareArea;
                landCollection.Remove(land);
            }
            double retainTableArea = 0.0;
            double retainActualArea = 0.0;
            double retainAwareArea = 0.0;
            foreach (ContractLand land in landCollection)
            {
                retainTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                retainActualArea += land.ActualArea;
                retainAwareArea += land.AwareArea;
            }
            landCollection.Clear();
            landArray.Clear();
        }

        #endregion

        #region Concord

        /// <summary>
        /// 填写合同信息
        /// </summary>
        private void WriteConcordInformation()
        {
            WriteSharePersonValue();

            WriteLandInformation();

            //WriteReclamationInformation();

            //WriteStartAndEnd();

            //WritePrintDate();
        }

        #endregion

        #region Parcel

        /// <summary>
        /// 获取全宗地示意图
        /// </summary>
        //public void GetAllMultiParcel()
        //{
        //    try
        //    {
        //        MapShapeUI map = null;
        //        BitmapSource image = null;
        //        Application.Current.Dispatcher.Invoke(new Action(() =>
        //        {
        //            if (ViewOfAllMultiParcel == null)
        //            {
        //                ViewOfAllMultiParcel = new DiagramsView();
        //                ViewOfAllMultiParcel.Paper.Model.Width = 326;
        //                ViewOfAllMultiParcel.Paper.Model.Height = 398;
        //                //ViewOfAllMultiParcel.Paper.Model.Width = 150;
        //                //ViewOfAllMultiParcel.Paper.Model.Height = 190;
        //            }
        //        }));
        //        List<FeatureObject> listAllFeature = new List<FeatureObject>();
        //        List<FeatureObject> listOwenrFeature = new List<FeatureObject>();
        //        //首先创建邻宗图层
        //        foreach (var land in ListGeoLand)
        //        {
        //            FeatureObject fo = new FeatureObject();
        //            fo.Object = land;
        //            fo.Geometry = land.Shape;
        //            fo.GeometryPropertyName = "Shape";
        //            listAllFeature.Add(fo);
        //        }
        //        //创建本宗图层
        //        foreach (var child in geoLandCollection)
        //        {
        //            FeatureObject fo = new FeatureObject();
        //            fo.Object = child;
        //            fo.Geometry = child.Shape;
        //            fo.GeometryPropertyName = "Shape";
        //            listOwenrFeature.Add(fo);
        //        }
        //        //当前地域下的所有空间地块图形导出
        //        DiagramBase diagram = null;
        //        Application.Current.Dispatcher.Invoke(new Action(() =>
        //        {
        //            if (ViewOfAllMultiParcel.Items.Count == 0)
        //            {
        //                diagram = new MapShape() { }.CreateDiagram();
        //                ViewOfAllMultiParcel.Items.Add(diagram);
        //                diagram.Model.Width = 326;
        //                diagram.Model.Height = 398;
        //                //diagram.Model.Width = 150;
        //                //diagram.Model.Height = 210;

        //                diagram.Model.BorderWidth = 0;
        //                diagram.Model.X = 0;
        //                diagram.Model.Y = 0;
        //            }
        //            else
        //            {
        //                diagram = ViewOfAllMultiParcel.Items[0];
        //            }
        //            map = diagram.Content as MapShapeUI;
        //            map.MapControl.DataProcessingMethod = eDataProcessingMethod.Synchronous;
        //        }));

        //        //Application.Current.Dispatcher.Invoke(new Action(() =>
        //        //{
        //        //    map = diagram.Content as MapShapeUI;
        //        //    map.MapControl.DataProcessingMethod = eDataProcessingMethod.Synchronous;
        //        //}));

        //        DynamicGeoSource allGeo = null;  //邻宗数据源 
        //        VectorLayer lyer = null;
        //        DynamicGeoSource ownerGeo = null;  //本宗数据源
        //        VectorLayer lyerOwnerExtent = null;

        //        Application.Current.Dispatcher.Invoke(new Action(() =>
        //        {
        //            if (map.MapControl.Layers.Count == 0)
        //            {
        //                lyer = new VectorLayer();  //创建邻宗矢量图层
        //                lyer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
        //                lyer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
        //                {
        //                    BackgroundColor = Color.FromArgb(255, 255, 255, 255),
        //                    BorderStrokeColor = Color.FromArgb(255, 0, 255, 64),
        //                    BorderThickness = 1
        //                });
        //                lyer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
        //                {
        //                    FontSize = 10
        //                })
        //                {
        //                    Enabled = false,
        //                    LabelProperty = "OwnerName"
        //                };
        //                allGeo = new DynamicGeoSource();
        //                allGeo.AddRange(listAllFeature.ToArray());
        //                lyer.DataSource = allGeo;
        //                map.MapControl.Extend = allGeo.FullExtend.ToGeometry().Buffer(0).GetEnvelope();
        //                map.MapControl.SpatialReference = new SpatialReference(0);
        //                map.MapControl.Layers.Add(lyer);
        //            }
        //            else
        //            {
        //                lyer = map.MapControl.Layers[0] as VectorLayer;
        //                allGeo = lyer.DataSource as DynamicGeoSource;
        //                allGeo.Clear();
        //                allGeo.AddRange(listAllFeature.ToArray());
        //                //map.MapControl.NavigateTo(allGeo.FullExtend.ToGeometry().Buffer(20).GetEnvelope());
        //            }
        //        }));
        //        Application.Current.Dispatcher.Invoke(new Action(() =>
        //        {
        //            if (map.MapControl.Layers.Count == 1)
        //            {
        //                lyerOwnerExtent = new VectorLayer();   //创建本宗矢量图层
        //                lyerOwnerExtent.DataProcessingMethod = eDataProcessingMethod.Synchronous;
        //                lyerOwnerExtent.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
        //                {
        //                    BackgroundColor = Color.FromArgb(255, 255, 62, 62),
        //                    BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
        //                    BorderThickness = 1
        //                });
        //                lyerOwnerExtent.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
        //                {
        //                    FontSize = 10
        //                })
        //                {
        //                    Enabled = false,
        //                    LabelProperty = "OwnerName"
        //                };
        //                ownerGeo = new DynamicGeoSource();
        //                ownerGeo.AddRange(listOwenrFeature.ToArray());
        //                lyerOwnerExtent.DataSource = ownerGeo;
        //                map.MapControl.Layers.Add(lyerOwnerExtent);
        //            }
        //            else
        //            {
        //                lyerOwnerExtent = map.MapControl.Layers[1] as VectorLayer;
        //                ownerGeo = lyerOwnerExtent.DataSource as DynamicGeoSource;
        //                ownerGeo.Clear();
        //                ownerGeo.AddRange(listOwenrFeature.ToArray());
        //                map.MapControl.NavigateTo(map.MapControl.Extend.Clone() as Envelope);
        //            }
        //        }));

        //        Application.Current.Dispatcher.Invoke(new Action(() =>
        //        {
        //            AddMainCompass(ViewOfAllMultiParcel, diagram);    //添加主视图指北针
        //            //保存为图片
        //            image = ViewOfAllMultiParcel.SaveToImage();
        //            if (image == null)
        //                return;
        //            string fileName = SavePathOfImage + @"\" + Contractor.ZoneCode + "-" + Contractor.Name + ".jpg";
        //            image.SaveToJpgFile(fileName);

        //        }));
        //        listAllFeature.Clear();
        //        listAllFeature = null;
        //        listOwenrFeature.Clear();
        //        listOwenrFeature = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new YltException("获取全宗地示意图失败!");
        //    }
        //}


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
                foreach (var geoLand in Lands)
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
                    var tempLands = Lands.FindAll(c => c.Shape.Intersects(geoLand.Shape) && c.ID != geoLand.ID);  //缓冲区(除去目标地块本身)
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
                            string fileName = FilePath + geoLand.LandNumber + ".jpg";
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

        #region OtherInformation

        /// <summary>
        /// 写表头信息
        /// </summary>
        private void WriteTitleInformation()
        {
            InitalizeRangeValue("B3", "D3", Concord.SenderName);//发包方名称
            InitalizeRangeValue("B2", "D2", dic.translante(Concord.SenderName));//发包方名称
            //InitalizeRangeValue("B3", "D3", Tissue.Name);//发包方名称
            //InitalizeRangeValue("B2", "D2", dic.translante(Tissue.Name));//发包方名称
            int familyNumber = 0;
            Int32.TryParse(VirtualPerson.FamilyNumber, out familyNumber);
            string number = string.Format("{0:D4}", familyNumber);
            InitalizeRangeValue("F2", "F3", number);//承包方编码
            InitalizeRangeValue("B5", "C5", VirtualPerson.Name);//承包方姓名
            string vpName = VirtualPerson.Name;
            if (vpName == null || vpName == "")
                vpName = dic.translante(VirtualPerson.Name);
            InitalizeRangeValue("B4", "C4", vpName);//承包方姓名
            InitalizeRangeValue("E4", "F5", VirtualPerson.Telephone);//联系电话
            InitalizeRangeValue("B6", "F6", VirtualPerson.Number);//身份证号码  
        }
        
        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetZoneNameByLevel(string zoneCode, eZoneLevel level)
        {
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            var zoneStation = dbContext.CreateZoneWorkStation();
            Zone temp = zoneStation.Get(zoneCode);
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            else
                return GetZoneNameByLevel(temp.UpLevelCode, level);
        }

        #endregion

        #endregion
    }
}
