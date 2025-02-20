/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Entity;
using System.Collections.ObjectModel;
using YuLinTu.tGIS.Client;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Business;
using YuLinTu.tGIS.Data;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包地块显示
    /// </summary>
    public partial class InvestigationLandPanel : UserControl
    {
        #region Fields       

        /// <summary>
        /// 承包地块面板
        /// </summary>
        private InvestigationLandPanel landPanelControl;

        /// <summary>
        /// 添加地块
        /// </summary>
        public ClickEvent AddLandClick;

        /// <summary>
        /// 编辑地块
        /// </summary>
        public ClickEvent EditLandClick;

        /// <summary>
        /// 删除地块
        /// </summary>
        public ClickEvent DelLandClick;

        /// <summary>
        /// 当前承包地块
        /// </summary>
        private ContractLandUI currentContractLand;

        private VectorLayer landLayer;//承包地图层
        #endregion

        #region Properties

        /// <summary>
        /// 地图控件
        /// </summary>
        public MapControl CurrentMapControl { get; set; }

        /// <summary>
        /// 当前土地地块图层
        /// </summary>
        //public VectorLayer LandLayer
        //{
        //    get { return landLayer; }
        //    set { landLayer = value; }
        //}              

        /// <summary>
        /// 工作页属性
        /// </summary>
        public IWorkpage theworkpage { get; set; }

        /// <summary>
        /// 承包地块面板属性
        /// </summary>
        public InvestigationLandPanel LandPanelControl
        {
            get { return landPanelControl; }
            set { landPanelControl = value; }
        }

        /// <summary>
        /// 承包地块集合
        /// </summary>
        public ObservableCollection<ContractLandUI> ContractLandUIItems { get; set; }

        /// <summary>
        /// 当前承包地块
        /// </summary>
        public ContractLandUI CurrentContractLand
        {
            get { return currentContractLand; }
            set
            {
                currentContractLand = value;
            }
        }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson CurrentContractor { get; set; }

        /// <summary>
        /// 添加地块委托属性
        /// </summary>
        public AddContractLand AddCtLand { get; set; }

        /// <summary>
        /// 编辑地块委托属性
        /// </summary>
        public EditContractLand EditCtLand { get; set; }

        /// <summary>
        /// 删除地块委托属性
        /// </summary>
        public DelContractLand DelCtLand { get; set; }

        /// <summary>
        /// 地位地块委托属性
        /// </summary>
        public SearchLand SearchLandSet { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public InvestigationLandPanel()
        {
            InitializeComponent();           
            ContractLandUIItems = new ObservableCollection<ContractLandUI>();
            landListBox.ItemsSource = ContractLandUIItems;
        }

        #endregion

        #region Method - Delegate

        /// <summary>
        /// 按钮委托方法定义
        /// </summary>
        public delegate void ClickEvent(object sender, RoutedEventArgs e);

        /// <summary>
        /// 右键添加承包地委托方法定义
        /// </summary>
        public delegate void AddContractLand();

        /// <summary>
        /// 右键编辑承包地委托方法定义
        /// </summary>
        public delegate void EditContractLand();

        /// <summary>
        /// 右键删除承包地委托方法定义
        /// </summary>
        public delegate void DelContractLand();

        /// <summary>
        /// 定位地块委托方法定义
        /// </summary>
        /// <param name="code"></param>
        public delegate void SearchLand(string code);

        #endregion

        #region Method - Events

        /// <summary>
        /// 添加地块事件
        /// </summary>
        private void landAddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddLandClick(sender, e);
        }

        /// <summary>
        /// 编辑地块事件
        /// </summary>
        private void landUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            EditLandClick(sender, e);
        }

        /// <summary>
        /// 删除地块事件
        /// </summary>
        private void landDelBtn_Click(object sender, RoutedEventArgs e)
        {
            DelLandClick(sender, e);
        }

        /// <summary>
        /// 承包地块选择改变
        /// </summary> 
        private void landListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetLandItem();
            CurrentMapControl.SelectedItems.Clear();                       
            if (CurrentContractLand == null) return;
            switch (CurrentContractLand.LandCategory)
            {
                case "10":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_cbd_Layer") as VectorLayer;
                    break;
                case "21":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_zld_Layer") as VectorLayer;
                    break;
                case "22":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_jdd_Layer") as VectorLayer;
                    break;
                case "23":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_khd_Layer") as VectorLayer;
                    break;
                case "99":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_qtjttd_Layer") as VectorLayer;
                    break;
            }
            if (landLayer == null) return;
            var fo = landLayer.DataSource.FirstOrDefault(string.Format("{0} = \"{1}\"", typeof(ContractLand).GetProperty("ID").GetAttribute<DataColumnAttribute>().ColumnName, CurrentContractLand.ID));
            if (fo != null)
            {
                var g = new Graphic();
                g.Object = fo;
                g.Layer = landLayer;
                CurrentMapControl.SelectedItems.Add(g);
            }
        }

        /// <summary>
        /// 鼠标双击事件
        /// </summary>
        private void landListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GetLandItem();
            if (CurrentContractLand == null) 
                return;
            CurrentMapControl.ZoomTo(CurrentContractLand.Shape as YuLinTu.Spatial.Geometry); //定位
        }

        /// <summary>
        /// 右键菜单弹出时触发该事件
        /// </summary>
        private void landListBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            GetLandItem();
            if (CurrentContractLand == null || CurrentContractor == null || (CurrentContractLand != null && CurrentContractor.Status == eVirtualPersonStatus.Lock))
            {
                btnLandAdd.IsEnabled = false;
                btnLandEdit.IsEnabled = false;
                btnLandDel.IsEnabled = false;
            }
            else if (CurrentContractLand != null && CurrentContractor.Status != eVirtualPersonStatus.Lock)
            {
                btnLandAdd.IsEnabled = true;
                btnLandEdit.IsEnabled = true;
                btnLandDel.IsEnabled = true;
            }
        }

        #endregion

        #region Method - 右键菜单

        /// <summary>
        /// 添加承包地块
        /// </summary>
        private void btnLandAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCtLand();
        }

        /// <summary>
        /// 编辑承包地块
        /// </summary>
        private void btnLandEdit_Click(object sender, RoutedEventArgs e)
        {
            EditCtLand();
        }

        /// <summary>
        /// 删除承包地块
        /// </summary>
        private void btnLandDel_Click(object sender, RoutedEventArgs e)
        {
            DelCtLand();
        }

        /// <summary>
        /// 定位地块
        /// </summary>
        private void btnLandZoomTo_Click(object sender, RoutedEventArgs e)
        {
            GetLandItem();
            if (SearchLandSet != null)
            {
                SearchLandSet(CurrentContractLand.LandNumber);
            }
        }

        #endregion

        #region Methods - public

        /// <summary>
        /// 添加承包地块界面显示集合
        /// </summary> 
        public void Set(List<ContractLand> lands)
        {
            if (CurrentContractor != null && CurrentContractor.Status == eVirtualPersonStatus.Lock)
            {
                landAddBtn.IsEnabled = false;
                landUpdateBtn.IsEnabled = false;
                landDelBtn.IsEnabled = false;
            }
            else if (CurrentContractor != null && CurrentContractor.Status != eVirtualPersonStatus.Lock)
            {
                landAddBtn.IsEnabled = true;
                landUpdateBtn.IsEnabled = true;
                landDelBtn.IsEnabled = true;
            }

            ContractLandUIItems.Clear();
            if (lands == null || lands.Count == 0)
            {
                return;
            }
            foreach (var land in lands)
            {
                var landUI = ContractLandUI.ContractLandUIConvert(land);
                ContractLandUIItems.Add(landUI);
            }
        }

        #endregion

        #region Method - private

        /// <summary>
        /// 获取当前选择地块
        /// </summary>
        private void GetLandItem()
        {
            CurrentContractLand = null;
            var land = landListBox.SelectedItem as ContractLandUI;
            if (land == null)
            {
                return;
            }
            CurrentContractLand = land;
        }

        #endregion

    }
}
