using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YuLinTu.Data;
using YuLinTu.Library.Aux;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity; 
using YuLinTu.Library.WorkStation;
using YuLinTu.NetAux;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 界址点属性面板
    /// </summary>
    public partial class DotPropertyPanel : UserControl
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DotPropertyPanel()
        {
            InitializeComponent();
            DotDataItems = new ObservableCollection<ConstructionLandDotItem>();
            tq = new TaskQueueDispatcher(Application.Current.Dispatcher);
            lstJZDLXContent = new KeyValueList<string, string>();
            lstJBLXContent = new KeyValueList<string, string>();
            graphic = new Graphic();
            DataContext = this;
        }

        #endregion

        #region Field
        public const string POSITIONOUT = "3";
        public const string POSITIONCEN = "2";
        public const string POSITIONIN = "1";
        private ContractLand land;
        private TaskQueue tq;
        private BuildLandBoundaryAddressDot currentDot;
        private KeyValueList<string, string> lstJZDLXContent;
        private KeyValueList<string, string> lstJBLXContent;
        private Graphic graphic;
        public ConstructionLandDotItem CurrentDotItem { get; set; }
        public CoilPropertyPanel coilPanel { get; set; }
        public GraphicsLayer layerHover
        {
            get; set;
        }
        public event EventHandler<MsgEventArgs<ContractLand>> OnDotSaved;

        #endregion

        #region Property
        public ContractLand currentLand
        {
            get
            {
                return land;
            }
            set
            {
                tq.Cancel();
                tq.Do(
                    go =>
                    {
                        currentDot = null;
                        //InitializeControl();
                        InitialDotControl(land);
                    },
                    completed => { },
                    terminated => { }, null,
                    started => { land = value; IsBusy = true; },
                    ended => { IsBusy = false; });
            }
        }
        /// <summary>
        /// 是否显示加载
        /// </summary>
        public bool IsBusy = true;
        /// <summary>
        /// 界址点界面显示
        /// </summary>
        public ObservableCollection<ConstructionLandDotItem> DotDataItems { get; set; }
        public IWorkpage Workpage { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// 异步获取界址点数据
        /// </summary>
        private void InitialDotControl(ContractLand land)
        {
            List<BuildLandBoundaryAddressDot> currentDots = null;
            List<BuildLandBoundaryAddressDot> currentDotsByCoil = null;
            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                IDictionaryWorkStation dictStation = null;
                IContractLandWorkStation landStation = null;
                IBuildLandBoundaryAddressDotWorkStation dotStation = null;
                IBuildLandBoundaryAddressCoilWorkStation coilStation = null;
                var dbContext = DataBaseSource.GetDataBaseSource();
                dictStation = dbContext.CreateDictWorkStation();

                try
                {
                    if (land == null)
                        return;
                    dictStation = dbContext.CreateDictWorkStation();
                    landStation = dbContext.CreateContractLandWorkstation();
                    dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                    coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                    currentDots = dotStation.GetByLandId(land.ID);
                    if (currentDots == null || currentDots.Count == 0)
                        return;
                    currentDots.Sort();
                    currentDotsByCoil = currentDots;
                    lstJZDLXContent = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZDLX);    //界址点类型
                    lstJBLXContent = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JBLX);    //界标类型
                }
                catch { }
            }, null, null, started =>
            {
                DotDataItems.Clear();
                currentDots = new List<BuildLandBoundaryAddressDot>(50);
                currentDotsByCoil = new List<BuildLandBoundaryAddressDot>(50);
                dotView.Roots = null;
            }, ended =>
            {
            }, completed =>
            {
                if (currentDots == null || currentDots.Count == 0)
                    return;
                if (lstJZDLXContent != null)
                {
                    cbDotType.ItemsSource = lstJZDLXContent;
                    cbDotType.DisplayMemberPath = "Value";
                    cbDotType.SelectedIndex = 0;
                }

                if (lstJBLXContent != null)
                {
                    cbDotMark.ItemsSource = lstJBLXContent;
                    cbDotMark.DisplayMemberPath = "Value";
                    cbDotMark.SelectedIndex = 0;
                }
                txtDotNumberPrefix.Text = "J";  //默认设置为J前缀
                txtDotDescriptioon.Text = "";
                if (currentDot != null)
                    SetControlValue();
                currentDotsByCoil.ForEach(c => DotDataItems.Add(c.ConvertToItem(lstJBLXContent, lstJZDLXContent)));
                if (DotDataItems != null)
                    DotDataItems.FirstOrDefault().IsEditable = false;
                dotView.Roots = DotDataItems;

            }, null, terminated =>
            {
                ShowBox("承包地块界址点", "获取承包地块界址点失败");
                return;
            }).StartAsync();
        }

        /// <summary>
        /// 设置控件值
        /// </summary>
        private void SetControlValue()
        {
            if (!string.IsNullOrEmpty(currentDot.DotNumber))
            {
                var dotNumberCh = currentDot.DotNumber.ToCharArray();
                txtDotNumberPrefix.Text = dotNumberCh[0].ToString();
            }

            if (!string.IsNullOrEmpty(currentDot.DotType) && lstJZDLXContent != null)
            {
                var dict = lstJZDLXContent.Find(c => c.Key == currentDot.DotType);
                if (dict != null)
                    cbDotType.SelectedItem = dict;
            }
            if (!string.IsNullOrEmpty(currentDot.Description))
                txtDotDescriptioon.Text = currentDot.Description;
            else
                txtDotDescriptioon.Text = "";
            if (!string.IsNullOrEmpty(currentDot.LandMarkType) && lstJBLXContent != null)
            {
                var dict = lstJBLXContent.Find(c => c.Key == currentDot.LandMarkType);
                if (dict != null)
                    cbDotMark.SelectedItem = dict;
            }
        }
        #region 辅助方法

        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        public void ShowBox(string title, string msg, eMessageGrade grade = eMessageGrade.Error)
        {
            if (Workpage == null)
            {
                return;
            }
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = grade,
                CancelButtonText = "取消",
            });
        }
        #endregion

        #endregion


        private void dotView_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void mbtnLandOK_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (currentDot == null)
                {
                    ShowBox("界址点", "请选择一个界址点进行编辑", eMessageGrade.Infomation);
                    return;
                }
                bool result = GoConfirm();
                if (result)
                {
                    OnDotSaved?.Invoke(this, new MsgEventArgs<ContractLand>() { Parameter = currentLand });
                    ShowBox("界址点", "保存成功", eMessageGrade.Infomation);
                }
                else
                    ShowBox("界址点", "保存失败");
            }));
        }
        /// <summary>
        /// 确定
        /// </summary>
        private bool GoConfirm()
        {
            bool result = false;
            try
            {
                GetControlValue();

                CurrentDotItem = dotView.SelectedItem as ConstructionLandDotItem;
                if (CurrentDotItem == null) return result;
                bool isvalid = CurrentDotItem.IsValidUI;
                CurrentDotItem = currentDot.ConvertToItem(lstJBLXContent, lstJZDLXContent);
                CurrentDotItem.IsValidUI = isvalid;
                currentDot.IsValid = isvalid;

                var dbContext = DataBaseSource.GetDataBaseSource();
                string expression = @"[a-z]|[A-Z]";
                System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(expression);
                string curdot = pattern.Replace(currentDot.DotNumber, "");

                for (int i = 0; i < DotDataItems.Count; i++)
                {
                    string itemdot = pattern.Replace(DotDataItems[i].Entity.DotNumber, "");
                    if (itemdot == curdot)
                    {
                        DotDataItems[i].DotNumberUI = CurrentDotItem.DotNumberUI;
                        DotDataItems[i].DotType = CurrentDotItem.DotType;
                        DotDataItems[i].DotMarkType = CurrentDotItem.DotMarkType;
                        DotDataItems[i].Entity.DotNumber = CurrentDotItem.Entity.DotNumber;
                        DotDataItems[i].Entity.DotType = CurrentDotItem.Entity.DotType;
                        DotDataItems[i].Entity.LandMarkType = CurrentDotItem.Entity.LandMarkType;
                        DotDataItems[i].Entity.Description = CurrentDotItem.Entity.Description;
                        //DotDataItems[i].Entity.IsValid = CurrentDotItem.Entity.IsValid;
                        break;
                    }
                }
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                int upCount = dotStation.Update(currentDot);
                string name = typeof(Zone).GetAttribute<DataTableAttribute>().TableName;
                var sr = ReferenceHelper.GetDbReference<Zone>(dbContext, name, "Shape");
                //result = ModifyDots(dbContext, sr.WKID);
                result = ModifyDots(dbContext);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "提交承包地块界址点编辑失败", ex.Message + ex.StackTrace);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 修改界址点信息（是否有效）
        /// </summary>
        private bool ModifyDots(IDbContext db)
        {
            if (DotDataItems == null || DotDataItems.Count == 0)
                return true;
            var dotStation = db.CreateBoundaryAddressDotWorkStation();
            var coilStation = db.CreateBoundaryAddressCoilWorkStation();
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(DotDataItems.Count);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(DotDataItems.Count);
            int isHandleValidUI = 0;//是否操作了界址点是否有效，默认是操作了的
            foreach (var item in DotDataItems)
            {
                if (item.Entity == null)
                    continue;
                if (item.Entity.IsValid == item.IsValidUI)
                {
                    ++isHandleValidUI;//没有变界址点有效性，所以不执行后面的
                }
                item.Entity.IsValid = item.IsValidUI;
                listDot.Add(item.Entity);
                if (item.IsValidUI)
                    listValidDot.Add(item.Entity);
            }
            if (isHandleValidUI == DotDataItems.Count) return true;

            if (listValidDot.Count < 3)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ShowBox("界址点", "有效界址点不能少于3个", eMessageGrade.Error);
                }));
                return false;
            }
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(DotDataItems.Count);
            BuildLandBoundaryAddressCoil curCoil = coilStation.GetByLandId(listDot[0].LandID).FirstOrDefault();

            var zoneStation = db.CreateZoneWorkStation();
            var landStation = db.CreateContractLandWorkstation();
            var curLand = landStation.Get(currentLand.ID);

            coilStation.DeleteByLandIds(new Guid[] { curLand.ID });
            var param = new InitLandDotCoilParam();
            var buffGeo = curLand.Shape.Buffer(param.AddressLinedbiDistance);
            var lands = landStation.Get(t => t.Shape != null && t.Shape.Intersects(buffGeo));
            if (lands == null || lands.Count == 0)
                lands = new List<ContractLand>();
            var coils = coilStation.Get(t => t.Shape != null && t.Shape.Intersects(buffGeo));
            listCoil = ProcessLine(curLand, lands, new InitLandDotCoilParam(), listDot, coils, true);//初始化界址线

            //if (curCoil == null)
            //    curCoil = new BuildLandBoundaryAddressCoil();
            //int j = 0;
            //for (int i = 0; i < listValidDot.Count; i++)
            //{
            //    BuildLandBoundaryAddressCoil cCoil = new BuildLandBoundaryAddressCoil();
            //    cCoil = curCoil.Clone() as BuildLandBoundaryAddressCoil;

            //    cCoil.ID = Guid.NewGuid();
            //    cCoil.OrderID = short.Parse((i + 1).ToString());
            //    cCoil.LandID = listValidDot[i].LandID;
            //    cCoil.LandNumber = listValidDot[i].LandNumber;
            //    cCoil.StartPointID = listValidDot[i].ID;
            //    cCoil.StartNumber = listValidDot[i].DotNumber;
            //    if (i + 1 == listValidDot.Count)
            //    {
            //        cCoil.EndPointID = listValidDot[0].ID;
            //        cCoil.EndNumber = listValidDot[0].DotNumber;
            //    }
            //    else
            //    {
            //        cCoil.EndPointID = listValidDot[i + 1].ID;
            //        cCoil.EndNumber = listValidDot[i + 1].DotNumber;
            //    }
            //    List<Coordinate> dots = new List<Coordinate>();
            //    double dis = 0;
            //    for (; j < DotDataItems.Count; j++)
            //    {
            //        dots.Add(DotDataItems[j].Entity.Shape.ToCoordinates().FirstOrDefault());
            //        ConstructionLandDotItem ed = DotDataItems[0];
            //        if (j + 1 != DotDataItems.Count)
            //            ed = DotDataItems[j + 1];
            //        dots.Add(ed.Entity.Shape.ToCoordinates().FirstOrDefault());
            //        dis += Distance(DotDataItems[j], ed);
            //        if (ed.Entity.ID == cCoil.EndPointID)
            //        {
            //            j++;
            //            break;
            //        }
            //    }
            //    //cCoil.CoilLength = Math.Round(dis, 2);
            //    //cCoil.Shape = YuLinTu.Spatial.Geometry.CreatePolyline(dots);
            //    //cCoil.Shape.Instance.SRID = SRID;
            //    //cCoil.Shape = YuLinTu.Spatial.Geometry.FromInstance(cCoil.Shape.Instance);
            //    //cCoil.Shape = CreatLine(listDot, SRID);

            //    //GetLineDescription(cCoil, true, false, listDot);

            //    listCoil.Add(cCoil);
            //}
            db.BeginTransaction();
            try
            {
                //先删除界址线，然后再添加更新
                if (curCoil != null)
                {
                    coilStation.Delete(c => c.LandID.Equals(curCoil.LandID));
                }
                coilStation.AddRange(listCoil);
                dotStation.UpdateRange(listDot);
                //DotDataItems.Clear();
                //foreach (var dot in listDot)
                //    DotDataItems.Add(dot.ConvertToItem(lstJBLXContent, lstJZDLXContent));

                db.CommitTransaction();
                return true;
            }
            catch
            {
                db.RollbackTransaction();
                return false;
            }
        }


        /// <summary>
        /// 获取没有绑定属性值
        /// </summary>
        private void GetControlValue()
        {
            var prefix = txtDotNumberPrefix.Text.Trim();
            if (!string.IsNullOrEmpty(currentDot.DotNumber))
            {
                string expression = @"[a-z]|[A-Z]";
                System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(expression);
                currentDot.DotNumber = pattern.Replace(currentDot.DotNumber, prefix);
                currentDot.UniteDotNumber = currentDot.DotNumber;
            }
            if (txtDotDescriptioon.Text.Trim() != "")
                currentDot.Description = txtDotDescriptioon.Text.Trim();
            var dotType = cbDotType.SelectedItem as KeyValue<string, string>;
            if (dotType != null)
                currentDot.DotType = dotType.Key;
            var dotMark = cbDotMark.SelectedItem as KeyValue<string, string>;
            if (dotMark != null)
                currentDot.LandMarkType = dotMark.Key;
        }

        /// <summary>
        /// 初始化界址线
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> ProcessLine(ContractLand land, List<ContractLand> lands, InitLandDotCoilParam param,
            List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> coils, bool IsSetAddressLinePosition)
        {
            var entityList = new List<BuildLandBoundaryAddressCoil>();
            if (dots == null || dots.Count == 0)
                return entityList;

            dots.Add(dots[0]);
            var createdots = new List<BuildLandBoundaryAddressDot>();
            short sxh = 1;
            bool hasStartKeyDot = false; //是否已经找到开始界址点
            bool hasEndKeyDot = false; //是否已经找到结束界址点

            foreach (var item in dots)
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

                    var line = CreateAddressCoil(createdots, land, sxh, param, lands);
                    entityList.Add(line);

                    createdots.Clear();
                    createdots.Add(item);
                    hasStartKeyDot = true;
                    hasEndKeyDot = false;
                    sxh++;
                }
            }

            LinePropertiesSet(entityList, coils, dots, IsSetAddressLinePosition, param.IsLineDescription);

            dots.Remove(dots[dots.Count - 1]);
            return entityList;
        }

        /// <summary>
        /// 创建界址线
        /// </summary>
        private BuildLandBoundaryAddressCoil CreateAddressCoil(List<BuildLandBoundaryAddressDot> list, ContractLand land,
            short sxh, InitLandDotCoilParam param, List<ContractLand> lands)
        {
            var linestring = CreatLine(list, land.Shape.Srid);
            var line = new BuildLandBoundaryAddressCoil()
            {
                ID = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                ZoneCode = land.ZoneCode,
                Shape = linestring,
                Modifier = "",
                ModifiedTime = DateTime.Now,
                Founder = "",
                LandID = land.ID,
                LandNumber = land.LandNumber,
                LandType = land.LandCode,
                StartPointID = list[0].ID,
                StartNumber = list[0].DotNumber,
                EndPointID = list[list.Count - 1].ID,
                EndNumber = list[list.Count - 1].DotNumber,
                OrderID = sxh,
                CoilLength = linestring.Length(),
                Position = param.IsAddressLinePosition ? param.AddressLinePosition : "3",
                LineType = param.AddressLineType,
                CoilType = param.AddressLineCatalog,
                Description = linestring.Length().ToString(),
            };
            var linebuffer = linestring.Buffer(param.AddressLinedbiDistance);
            var landList = lands.FindAll(t => t.Shape.Intersects(linebuffer));
            landList.RemoveAll(t => t.ID == land.ID);
            if (landList.Count > 0)
            {
                line.NeighborPerson = landList[0].OwnerName;
                line.NeighborFefer = line.NeighborPerson;
            }
            return line;
        }


        /// <summary>
        /// 界址线设置
        /// </summary> 
        private void LinePropertiesSet(List<BuildLandBoundaryAddressCoil> entityList, List<BuildLandBoundaryAddressCoil> coils,
          List<BuildLandBoundaryAddressDot> dots, bool IsSetAddressLinePosition, bool isUseLengthAndPosition)
        {
            var startIndex = 1;
            foreach (var line in entityList)
            {
                if (coils != null && coils.Count > 0)
                {
                    var coil = coils.Find(t => TestLineEqual(t.Shape, line.Shape));
                    if (coil != null)
                    {
                        line.Position = POSITIONCEN;
                    }
                }
                var charArray = line.StartNumber.Reverse();
                List<char> charlist = new List<char>();
                foreach (var item in charArray)
                {
                    if (item >= 48 && item <= 58)
                    {
                        charlist.Add(item);
                    }
                }
                charlist.Reverse();
                var num = "";
                foreach (var t in charlist)
                {
                    num += t;
                }
                if (num == "1")
                {
                    startIndex = line.OrderID;
                }
                //GetLineDescription(line, isUnit, isUseLengthAndPosition, dots);
            }
            if (IsSetAddressLinePosition)
            {
                foreach (var item in entityList)
                {
                    if (item.Position == POSITIONOUT)
                        item.Position = POSITIONIN;
                }
            }
            short orderid = 1;

            for (int i = startIndex; i <= entityList.Count; i++)
            {
                entityList[i - 1].OrderID = orderid;
                orderid++;
            }
            for (int i = 0; i < startIndex - 1; i++)
            {
                entityList[i].OrderID = orderid;
                orderid++;
            }
        }


        /// <summary>
        /// 线是否相等
        /// </summary>
        public bool TestLineEqual(Geometry geo1, Geometry geo2, double tolerance = 0.001)
        {
            var result = true;
            var geoArray1 = geo1.ToCoordinates();
            var geoArray2 = geo2.ToCoordinates();

            if (geoArray1.Length != geoArray2.Length)
                return false;

            if (!testIsEqual(geoArray1[0], geoArray2[0].X, geoArray2[0].Y))
                geoArray2 = geoArray2.Reverse().ToArray();
            if (!testIsEqual(geoArray1[0], geoArray2[0].X, geoArray2[0].Y))
                return false;

            for (int i = 0; i < geoArray1.Length; i++)
            {
                if (!testIsEqual(geoArray1[i], geoArray2[i].X, geoArray2[i].Y))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        public bool testIsEqual(Coordinate c, double x, double y, double tolerance = 0.001)
        {
            return CglHelper.equal(c.X, x, tolerance) && CglHelper.equal(c.Y, y, tolerance);
        }

        /// <summary>
        /// 创建线
        /// </summary>
        private Geometry CreatLine(List<BuildLandBoundaryAddressDot> dots, int srid)
        {
            Coordinate[] corrds = new Coordinate[dots.Count];
            for (int i = 0; i < dots.Count; i++)
            {
                corrds[i] = dots[i].Shape.ToCoordinates()[0];
            }
            var geo = YuLinTu.Spatial.Geometry.CreatePolyline(corrds.ToList(), srid);
            return geo;
        }


        /// <summary>
        /// 创建界址线说明
        /// </summary>
        /// <returns></returns>
        private void GetLineDescription(BuildLandBoundaryAddressCoil line, bool isUnit, bool isUseLengthAndPosition, List<BuildLandBoundaryAddressDot> dots)
        {
            Aspect a = new Aspect(0);
            if (line.Shape != null && !isUseLengthAndPosition)
            {
                var coords = line.Shape.ToCoordinates();
                var p0 = coords[0];
                var p1 = coords[coords.Count() - 1];
                a.Assign(p0.X, p0.Y, p1.X, p1.Y);
                string qjzdh = line.StartNumber;
                var zjzdh = line.EndNumber;
                if (isUnit)
                {
                    qjzdh = dots.Find(t => t.ID == line.StartPointID).UniteDotNumber;
                    zjzdh = dots.Find(t => t.ID == line.EndPointID).UniteDotNumber;
                }
                var jszsm = qjzdh + "沿" + a.toAzimuthString() + "方" + Math.Round(line.Shape.Length(), 2) + "米到" + zjzdh;
                line.Description = jszsm;
            }
        }

        /// <summary>
        /// 界址点号文本输入
        /// </summary>
        private void txtDotNumberPrefix_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string prefix = e.Text.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(prefix, "^[a-zA-Z]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                e.Handled = true;
            else
                e.Handled = false;
        }

        /// <summary>
        /// 界址点号文本框键盘按下
        /// </summary>
        private void txtDotNumberPrefix_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void dotView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            layerHover.Graphics.Clear();
            var currtDot = dotView.SelectedItem as ConstructionLandDotItem;
            if (currtDot == null)
            {
                //graphic = null;
                //layerHover.Graphics.Add(graphic);
                //ShowBox("编辑界址点", "请选择要编辑的界址点!");
                return;
            }

            currentDot = currtDot.Entity.Clone() as BuildLandBoundaryAddressDot;
            SetControlValue();

            graphic.Geometry = Spatial.Geometry.CreatePoint(new Coordinate(double.Parse(currtDot.XCoordinateUI), double.Parse(currtDot.YCoordinateUI)), 0);

            switch (graphic.Geometry.GeometryType)
            {
                case YuLinTu.Spatial.eGeometryType.Point:
                case YuLinTu.Spatial.eGeometryType.MultiPoint:
                    graphic.Symbol = Application.Current.TryFindResource("UISymbol_Mark_Measure") as UISymbol;
                    break;
                default:
                    return;
            }
            layerHover.Graphics.Add(graphic);
        }

        private void miStartDot_Click(object sender, RoutedEventArgs e)
        {
            var currtDot = dotView.SelectedItem as ConstructionLandDotItem;

            if (currtDot == null)
            {
                ShowBox("承包地块界址点", "请选择一个承包地块界址点");
                return;
            }
            currentDot = currtDot.Entity.Clone() as BuildLandBoundaryAddressDot;
            if (!currentDot.IsValid)
            {
                ShowBox("设置界址点", "请选择有效界址点设为起点!");
                return;
            }
            var db = DataBaseSource.GetDataBaseSource();
            var dotStation = db.CreateBoundaryAddressDotWorkStation();
            var dictStation = db.CreateDictWorkStation();
            var coilStation = db.CreateBoundaryAddressCoilWorkStation();
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>();
            List<BuildLandBoundaryAddressDot> currentDotsByCoil = new List<BuildLandBoundaryAddressDot>();
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>();

            KeyValueList<string, string> lstDictContentJXXZ = null;
            KeyValueList<string, string> lstDictContentJZXLB = null;
            KeyValueList<string, string> lstDictContentJZXWZ = null;

            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                if (listCoil == null || listCoil.Count == 0)
                    return;
                lstDictContentJXXZ = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JXXZ);
                lstDictContentJZXLB = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXLB);
                lstDictContentJZXWZ = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXWZ);

                string expression = @"[a-z]|[A-Z]";
                System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(expression);
                string dotNumber = pattern.Replace(currentDot.DotNumber, "");
                string strN = currentDot.DotNumber.Replace(dotNumber, "");
                int curNumber = int.Parse(dotNumber);
                foreach (var item in listDot)
                {
                    dotNumber = pattern.Replace(item.DotNumber, "");
                    int num = int.Parse(dotNumber);
                    if (num >= curNumber)
                        num = num - curNumber + 1;
                    else
                        num = listDot.Count - curNumber + num + 1;
                    strN = item.DotNumber.Replace(dotNumber, "");
                    item.DotNumber = strN + num.ToString();
                }
                BuildLandBoundaryAddressCoil cur = listCoil.Find(c => c.StartPointID.Equals(currentDot.ID));
                int orderNum = 0;
                if (cur != null)
                    orderNum = cur.OrderID;
                else
                    return;
                listCoil.ForEach(c =>
                {
                    c.StartNumber = listDot.Find(a => a.ID.Equals(c.StartPointID)).DotNumber;
                    c.EndNumber = listDot.Find(a => a.ID.Equals(c.EndPointID)).DotNumber;
                    dotNumber = pattern.Replace(c.StartNumber, "");
                    int ornum = c.OrderID;
                    if (ornum >= orderNum)
                        ornum = ornum - orderNum + 1;
                    else
                        ornum = listCoil.Count - orderNum + ornum + 1;
                    c.OrderID = short.Parse(ornum.ToString());
                });
            },
                null, null, started =>
                {
                    DotDataItems.Clear();
                    listDot = dotStation.GetByLandID(currentDot.LandID);
                    listCoil = coilStation.GetByLandID(currentDot.LandID);
                    dotView.Roots = null;

                }, ended => { },
                comleted =>
                {
                    listDot.Sort();
                    currentDotsByCoil = listDot;
                    currentDotsByCoil.ForEach(c => DotDataItems.Add(c.ConvertToItem(lstJBLXContent, lstJZDLXContent)));
                    if (DotDataItems != null)
                        DotDataItems.FirstOrDefault().IsEditable = true;
                    dotView.Roots = DotDataItems;
                    if (DotDataItems != null && DotDataItems.Count > 0)
                    {
                        dotView.BringIntoView(DotDataItems[0], true);
                    }
                    dotStation.UpdateRange(currentDotsByCoil);
                    listCoil.ForEach(c => coilStation.Update(c));
                    coilPanel.currentLand = currentLand;
                }, terminated =>
                {
                }).StartAsync();
        }

        private void miSaveDot_Click(object sender, RoutedEventArgs e)
        {
            if (DotDataItems == null || DotDataItems.Count == 0)
                return;
            var db = DataBaseSource.GetDataBaseSource();
            var dotStation = db.CreateBoundaryAddressDotWorkStation();
            var coilStation = db.CreateBoundaryAddressCoilWorkStation();
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(DotDataItems.Count);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(DotDataItems.Count);
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>();

            int SRID = 0;
            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                foreach (var item in DotDataItems)
                {
                    if (item.Entity == null)
                        continue;
                    item.Entity.IsValid = item.IsValidUI;
                    listDot.Add(item.Entity);
                    if (item.IsValidUI)
                        listValidDot.Add(item.Entity);
                }
                if (listValidDot.Count < 3)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ShowBox("承包地块界址点", "有效界址点不能少于3个", eMessageGrade.Error);
                    }));
                    return;
                }
                BuildLandBoundaryAddressCoil curCoil = coilStation.GetByLandId(listDot[0].LandID).FirstOrDefault();

                if (curCoil == null)
                    curCoil = new BuildLandBoundaryAddressCoil();
                int j = 0;
                for (int i = 0; i < listValidDot.Count; i++)
                {
                    BuildLandBoundaryAddressCoil cCoil = new BuildLandBoundaryAddressCoil();
                    cCoil = curCoil.Clone() as BuildLandBoundaryAddressCoil;

                    cCoil.ID = Guid.NewGuid();
                    cCoil.OrderID = short.Parse((i + 1).ToString());
                    cCoil.LandID = listValidDot[i].LandID;
                    cCoil.LandNumber = listValidDot[i].LandNumber;
                    cCoil.StartPointID = listValidDot[i].ID;
                    cCoil.StartNumber = listValidDot[i].DotNumber;
                    if (i + 1 == listValidDot.Count)
                    {
                        cCoil.EndPointID = listValidDot[0].ID;
                        cCoil.EndNumber = listValidDot[0].DotNumber;
                    }
                    else
                    {
                        cCoil.EndPointID = listValidDot[i + 1].ID;
                        cCoil.EndNumber = listValidDot[i + 1].DotNumber;
                    }
                    List<Coordinate> dots = new List<Coordinate>();
                    double dis = 0;
                    for (; j < DotDataItems.Count; j++)
                    {
                        dots.Add(DotDataItems[j].Entity.Shape.ToCoordinates().FirstOrDefault());
                        ConstructionLandDotItem ed = DotDataItems[0];
                        if (j + 1 != DotDataItems.Count)
                            ed = DotDataItems[j + 1];
                        dots.Add(ed.Entity.Shape.ToCoordinates().FirstOrDefault());
                        dis += Distance(DotDataItems[j], ed);
                        if (ed.Entity.ID == cCoil.EndPointID)
                        {
                            j++;
                            break;
                        }
                    }
                    cCoil.CoilLength = Math.Round(dis, 2);
                    cCoil.Shape = YuLinTu.Spatial.Geometry.CreatePolyline(dots);
                    cCoil.Shape.Instance.SRID = SRID;
                    cCoil.Shape = YuLinTu.Spatial.Geometry.FromInstance(cCoil.Shape.Instance);
                    listCoil.Add(cCoil);
                }
            }, null, null,
                started =>
                {
                    string name = typeof(Zone).GetAttribute<DataTableAttribute>().TableName;
                    var sr = ReferenceHelper.GetDbReference<Zone>(db, name, "Shape");
                    SRID = sr.WKID;
                },
                ended => { },
                comleted =>
                {
                    db.BeginTransaction();
                    try
                    {
                        //先删除界址线，然后再添加更新
                        coilStation.Delete(c => c.LandID.Equals(DotDataItems[0].Entity.LandID));
                        coilStation.AddRange(listCoil);
                        dotStation.UpdateRange(listDot);
                        db.CommitTransaction();
                        coilPanel.currentLand = currentLand;
                        ShowBox("承包地块界址点", "承包地块界址点保存成功", eMessageGrade.Infomation);
                        return;
                    }
                    catch
                    {
                        db.RollbackTransaction();
                        return;
                    }
                },
                null,
                terminated =>
                {
                    ShowBox("承包地块界址点", "承包地块界址点保存失败");
                    return;
                }).StartAsync();

        }

        private double Distance(ConstructionLandDotItem sd, ConstructionLandDotItem ed)
        {
            double x1 = double.Parse(sd.XCoordinateUI);
            double x2 = double.Parse(ed.XCoordinateUI);
            double y1 = double.Parse(sd.YCoordinateUI);
            double y2 = double.Parse(ed.YCoordinateUI);
            double val = Math.Sqrt(Math.Abs(x1 - x2) * Math.Abs(x1 - x2) + Math.Abs(y1 - y2) * Math.Abs(y1 - y2));
            return val;
        }
    }
}
