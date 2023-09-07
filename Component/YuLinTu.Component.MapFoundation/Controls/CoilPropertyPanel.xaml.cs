using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 界址线属性面板
    /// </summary>
    public partial class CoilPropertyPanel : UserControl, INotifyPropertyChanged
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public CoilPropertyPanel()
        {
            InitializeComponent();
            CoilDataItems = new ObservableCollection<ConstructionLandCoilItem>();
            tq = new TaskQueueDispatcher(Application.Current.Dispatcher);
            lstJXXZContent = new KeyValueList<string, string>();
            lstJZXLBContent = new KeyValueList<string, string>();
            lstJZXWZContent = new KeyValueList<string, string>();
            graphic = new Graphic();
            this.DataContext = this;
            //dotInfoGrid.DataContext = CurrentCoil;

        }

        #endregion

        #region Field
        private ContractLand land;
        private TaskQueue tq;
        private BuildLandBoundaryAddressCoil currentCoil;
        private KeyValueList<string, string> lstJXXZContent;
        private KeyValueList<string, string> lstJZXLBContent;
        private KeyValueList<string, string> lstJZXWZContent;
        private List<BuildLandBoundaryAddressDot> currentDots = null;  //当前地块的所有界址点
        private Graphic graphic;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
                        currentDots = null;
                        InitialCoilControl(land);
                    },
                    completed => { },
                    terminated => { }, null,
                    started => { land = value; BusyIs = true; },
                    ended =>
                    {
                        BusyIs = false;
                    });
            }
        }
        /// <summary>
        /// 是否显示加载
        /// </summary>
        public bool BusyIs { get; set; }
        /// <summary>
        /// 当前待编辑界址线
        /// </summary>
        public BuildLandBoundaryAddressCoil CurrentCoil
        {
            get { return currentCoil; }
            set
            {
                currentCoil = value;
                NotifyPropertyChanged("CurrentCoil");
            }
        }

        /// <summary>
        /// 界址点界面显示
        /// </summary>
        /// <summary>
        /// 界址线界面显示
        /// </summary>
        public ObservableCollection<ConstructionLandCoilItem> CoilDataItems { get; set; }
        public IWorkpage Workpage { get; set; }
        /// <summary>
        /// 当前界址线界面实体
        /// </summary>
        public ConstructionLandCoilItem CurrentCoilItem { get; set; }
        /// <summary>
        /// 当前地块的所有界址点
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListDot { get; set; }
        public GraphicsLayer layerHover
        {
            get; set;
        }
        #endregion

        #region Method
        /// <summary>
        /// 异步获取界址线数据
        /// </summary>
        private void InitialCoilControl(ContractLand land)
        {
            //if (land == null)
            //    return;
            List<BuildLandBoundaryAddressCoil> currentCoils = null;
            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                IDictionaryWorkStation dictStation = null;
                IContractLandWorkStation landStation = null;
                IBuildLandBoundaryAddressDotWorkStation dotStation = null;
                IBuildLandBoundaryAddressCoilWorkStation coilStation = null;

                try
                {
                    if (land == null)
                        return;
                    var dbContext = DataBaseSource.GetDataBaseSource();
                    dictStation = dbContext.CreateDictWorkStation();
                    landStation = dbContext.CreateContractLandWorkstation();
                    dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                    coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                    currentCoils = coilStation.GetByLandId(land.ID);
                    currentDots = dotStation.GetByLandId(land.ID);
                    if (currentDots == null || currentDots.Count == 0 || currentCoils == null || currentCoils.Count == 0)
                        return;
                    lstJXXZContent = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JXXZ);    //界线性质
                    lstJZXLBContent = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXLB);    //界址线类别
                    lstJZXWZContent = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXWZ);    //界址线位置 
                }
                catch { }
            }, null, null, started =>
            {
                CoilDataItems.Clear();
                currentCoils = new List<BuildLandBoundaryAddressCoil>(50);
                currentDots = new List<BuildLandBoundaryAddressDot>(50);
                coilView.Roots = null;
                //dotInfoGrid.DataContext = null;
            }, ended =>
            {
            }, completed =>
            {
                if (currentCoils == null || currentCoils.Count == 0)
                    return;
                if (lstJXXZContent != null)
                {
                    cbCoilProperty.ItemsSource = lstJXXZContent;
                    cbCoilProperty.DisplayMemberPath = "Value";
                    cbCoilProperty.SelectedIndex = 0;
                }

                if (lstJZXLBContent != null)
                {
                    cbCoilType.ItemsSource = lstJZXLBContent;
                    cbCoilType.DisplayMemberPath = "Value";
                    cbCoilType.SelectedIndex = 0;
                }

                if (lstJZXWZContent != null)
                {
                    cbCoilPosition.ItemsSource = lstJZXWZContent;
                    cbCoilPosition.DisplayMemberPath = "Value";
                    cbCoilPosition.SelectedIndex = 0;
                }

                if (currentCoil != null)
                    SetControlValue();
                var orderCoils = currentCoils.OrderBy(t => t.OrderID).ToList();
                orderCoils.ForEach(c => CoilDataItems.Add(c.ConvertToItem(currentDots,
                    lstJXXZContent, lstJZXLBContent, lstJZXWZContent)));
                coilView.Roots = CoilDataItems;
            }, null, terminated =>
            {
                ShowBox("承包地块界址线", "获取承包地块界址线失败");
                return;
            }).StartAsync();
        }

        /// <summary>
        /// 设置控件值
        /// </summary>
        private void SetControlValue()
        {
            if (!string.IsNullOrEmpty(currentCoil.LineType) && lstJXXZContent != null)
            {
                var dict = lstJXXZContent.Find(c => c.Key == currentCoil.LineType);
                if (dict != null)
                    cbCoilProperty.SelectedItem = dict;
            }

            if (!string.IsNullOrEmpty(currentCoil.CoilType) && lstJZXLBContent != null)
            {
                var dict = lstJZXLBContent.Find(c => c.Key == currentCoil.CoilType);
                if (dict != null)
                    cbCoilType.SelectedItem = dict;
            }

            if (!string.IsNullOrEmpty(currentCoil.Position) && lstJZXWZContent != null)
            {
                var dict = lstJZXWZContent.Find(c => c.Key == currentCoil.Position);
                if (dict != null)
                    cbCoilPosition.SelectedItem = dict;
            }
        }
        /// <summary>
        /// 获取没有绑定属性值
        /// </summary>
        private void GetControlValue()
        {
            var coilProperty = cbCoilProperty.SelectedItem as KeyValue<string, string>;
            if (coilProperty != null)
                currentCoil.LineType = coilProperty.Key;
            var coilType = cbCoilType.SelectedItem as KeyValue<string, string>;
            if (coilType != null)
                currentCoil.CoilType = coilType.Key;
            var coilPosition = cbCoilPosition.SelectedItem as KeyValue<string, string>;
            if (coilPosition != null)
                currentCoil.Position = coilPosition.Key;
        }


        #endregion

        #region Events
        private void mbtnLandOK_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (currentCoil == null)
                {
                    ShowBox("界址线", "请选择一个界址线进行编辑", eMessageGrade.Infomation);
                    return;
                }
                bool result = GoConfirm();
                if (result)
                    ShowBox("界址线", "保存成功", eMessageGrade.Infomation);
                else
                    ShowBox("界址线", "保存失败");
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
                Application.Current.Dispatcher.Invoke(new Action(()=> {
                    GetControlValue();
                }));

                CurrentCoilItem = currentCoil.ConvertToItem(ListDot, lstJXXZContent, lstJZXLBContent, lstJZXWZContent);
                for (int i = 0; i < CoilDataItems.Count; i++)
                {
                    if (CoilDataItems[i].CoilNumberUI == CurrentCoilItem.CoilNumberUI)
                    {
                        CoilDataItems[i].Entity.AgreementBook = CurrentCoilItem.Entity.AgreementBook;
                        CoilDataItems[i].Entity.AgreementNumber = CurrentCoilItem.Entity.AgreementNumber;
                        CoilDataItems[i].Entity.Comment = CurrentCoilItem.Entity.Comment;
                        CoilDataItems[i].Entity.ControversyBook = CurrentCoilItem.Entity.ControversyBook;
                        CoilDataItems[i].Entity.ControversyNumber = CurrentCoilItem.Entity.ControversyNumber;
                        CoilDataItems[i].Entity.Description = CurrentCoilItem.Entity.Description;
                        CoilDataItems[i].Entity.NeighborFefer = CurrentCoilItem.Entity.NeighborFefer;
                        CoilDataItems[i].Entity.NeighborPerson = CurrentCoilItem.Entity.NeighborPerson;

                        CoilDataItems[i].Entity.CoilType = CurrentCoilItem.Entity.CoilType;
                        CoilDataItems[i].Entity.LandType = CurrentCoilItem.Entity.LandType;
                        CoilDataItems[i].Entity.LineType = CurrentCoilItem.Entity.LineType;
                        break;
                    }
                }
                var dbContext = DataBaseSource.GetDataBaseSource();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                int upCount = coilStation.Update(currentCoil);
                result = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "提交承包地块界址线编辑失败", ex.Message + ex.StackTrace);
                result = false;
            }
            return result;
        }
        private void coilView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ConstructionLandCoilItem curCoil = null;
            curCoil = coilView.SelectedItem as ConstructionLandCoilItem;
            if (curCoil == null)
            {
                //ShowBox("编辑界址线", "请选择要编辑的界址线!");
                return;
            }
            CurrentCoil = curCoil.Entity.Clone() as BuildLandBoundaryAddressCoil;
            SetControlValue();
            if (layerHover == null)
                return;
            layerHover.Graphics.Clear();
            graphic.Geometry = currentCoil.Shape;
            switch (graphic.Geometry.GeometryType)
            {
                case YuLinTu.Spatial.eGeometryType.Polyline:
                case YuLinTu.Spatial.eGeometryType.MultiPolyline:
                    graphic.Symbol = Application.Current.TryFindResource("UISymbol_Line_Measure") as LineSymbol;
                    break;
                default:
                    break;
            }
            layerHover.Graphics.Add(graphic);
        }

        #endregion

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


    }
}
