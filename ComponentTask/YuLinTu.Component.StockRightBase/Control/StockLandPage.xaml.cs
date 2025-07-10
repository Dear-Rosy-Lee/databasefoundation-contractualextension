/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// 承包地块添加/编辑页面
    /// </summary>
    public partial class StockLandPage : InfoPageBase
    {
        #region Fields
      
        //private List<VirtualPerson> virtualPersonList;
        //private VirtualPersonBusiness personBusiness;
        //private AccountLandBusiness landBusiness;
        //private List<Dictionary> list;
        //private TaskQueue queueGet;//获取数据
        //private bool isLock;
        //private Graphic pointgraphic;
        //private string newLandNumber;//新地块编码
        private IDbContext dbContext;//数据库
        private Bussiness.BussinessData _bussiness;
        public const string POSITIONOUT = "3";
        public const string POSITIONCEN = "2";
        public const string POSITIONIN = "1";
        private BelongRelationUI SelectLand;
        private bool isAdd;
        private VirtualPerson currentPerson;

        #endregion

        #region Property

        public VirtualPerson VirtualItem { get; set; }

        public PersonGridModel SelectedItem { get; set; }
        public List<BelongRelation> PersonbelongRelations { get; set; }
        public List<ContractLand> LandList { get; set; }
        public Zone CurrentZone { get; set; }

        public IDbContext DataSource
        {
            get { return dbContext; }
            set
            {
                dbContext = value;
                _bussiness = new Bussiness.BussinessData(dbContext, CurrentZone);
            }
        }

        /// <summary>
        /// 界址点界面显示
        /// </summary>
        public ObservableCollection<ConstructionLandDotItem> DotDataItems { get; set; }


        /// <summary>
        /// 地图属性编辑框委托
        /// </summary>
        public delegate bool ShowMapPropertyDelegate();

        /// <summary>
        /// 在地图中显示地块属性编辑框
        /// </summary>
        public ShowMapPropertyDelegate ShowMapProperty { get; set; }
        public MapControl MapControl { get; set; }
        public GraphicsLayer layerHover { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public StockLandPage()
        {
            InitializeComponent();
            SelectLand = new BelongRelationUI();
            dbContext = DataBaseSource.GetDataBaseSource();
            //queueGet = new TaskQueueDispatcher(Dispatcher);
            //list = new List<Dictionary>();
            DotDataItems = new ObservableCollection<ConstructionLandDotItem>();
            if (isAdd)
            {
                //dotItem.Visibility = Visibility.Collapsed;
                //coilItem.Visibility = Visibility.Collapsed;
                //spInitialDotCoil.Visibility = Visibility.Collapsed;
            }
            this.DataContext = this;
            //pointgraphic = new Graphic();
            //IsStockRight = false;
        }

        #endregion

        #region Method

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInitializeGo()
        {
            var datalist = new List<BelongRelationUI>();
            foreach (var item in PersonbelongRelations)
            {
                var br = item.ConvertTo<BelongRelationUI>();
                datalist.Add(br);
                var l = LandList.Find(t => t.ID == item.LandID);
                if (l != null)
                {
                    br.SCMJM = l.ActualArea;
                    br.DKBM = l.LandNumber;
                    br.DKMC = l.LandName;
                }
            }
            Dispatcher.Invoke(() =>
            {
                dotView.Roots = datalist;
            });
        }

        #endregion

        #region Event

        /// <summary>
        /// 响应添加或者编辑承包台账地块信息确定按钮
        /// </summary>
        private void mbtnLandOK_Click(object sender, RoutedEventArgs e)
        {
            GetLandValue();
            ConfirmAsync(goCallback =>
            {
                return GoConfirm();
            }, completedCallback =>
            {
                Close(true);
            });
        }

        /// <summary>
        /// 获取承包地块值
        /// </summary>
        private void GetLandValue()
        {
            if (currentPerson == null)
            {
                ////没有指明添加地块的承包方
                //if (!IsStockRight)
                //{
                //    ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.LandAddNoSelectedPerson);
                //    return;
                //}
            }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <returns></returns>
        private bool GoConfirm()
        {
            bool result = true;
            var db = dbContext;
            List<BelongRelationUI> belongRelations = null;
            try
            {
                Dispatcher.Invoke(() =>
                {
                    belongRelations = dotView.Roots as List<BelongRelationUI>;
                });
                var landStation = db.CreateContractLandWorkstation();
                if (CheckCommitData(belongRelations))
                {
                    var brstation = dbContext.CreateBelongRelationWorkStation();
                    foreach (var item in belongRelations)
                    {
                        var pr = PersonbelongRelations.Find(t => t.LandID == item.LandID && t.VirtualPersonID == item.VirtualPersonID);
                        if (pr != null)
                        {
                            PersonbelongRelations.Remove(pr);
                            if (pr.QuanficationArea != item.QuanficationArea)
                            {
                                brstation.Update(item.ConvertTo<BelongRelation>(), t =>
                                     t.LandID == item.LandID &&
                                     t.VirtualPersonID == item.VirtualPersonID);
                            }
                        }
                        else
                        {
                            brstation.Add(item.ConvertTo<BelongRelation>());
                        }
                    }
                    foreach (var item in PersonbelongRelations)
                    {
                        brstation.Delete(t => t.LandID == item.LandID &&
                                     t.VirtualPersonID == item.VirtualPersonID);
                    }
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Dispatcher.Invoke(new Action(() =>
                {
                    ShowBox(ContractAccountInfo.ContractLandPro, ContractAccountInfo.ContractLandProFail);
                }));
                YuLinTu.Library.Log.Log.WriteException(this, "GoConfirm(承包地块数据处理失败!)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 检查提交数据
        /// </summary>
        /// <returns></returns>
        private bool CheckCommitData(List<BelongRelationUI> belongRelations)
        {
            bool right = true;
            foreach (var br in belongRelations)
            {
                if (br.QuanficationArea <= 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ShowBox(ContractAccountInfo.LandInfo, $"地块{br.DKBM}的确股面积必须大于0");
                    }); right = false;
                    break;
                }
            }

            return right;
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

        #region Method

        private void dotView_MouseDoubleClick(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectLand = dotView.SelectedItem as BelongRelationUI;
            if (SelectLand != null)
            {
                txtName.Text = SelectLand.DKMC;
                txtCode.Text = SelectLand.DKBM;
                cmbActualArea.Value = SelectLand.SCMJM;
                cmbAwareArea.Value = SelectLand.QuanficationArea;
                btnNeedSave.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 面积修改
        /// </summary>
        private void cmbAwareArea_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectLand == null)
                return;
            if (cmbAwareArea.Value != SelectLand.QuanficationArea)
                btnNeedSave.Visibility = Visibility.Visible;
            else
                btnNeedSave.Visibility = Visibility.Hidden;
        }

        private void btnNeedSave_Click(object sender, RoutedEventArgs e)
        {
            SelectLand.QuanficationArea = (double)cmbAwareArea.Value;
            btnNeedSave.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectLand == null)
                return;
            var brlist = dotView.Roots as List<BelongRelationUI>;
            var br = brlist.Find(t => t.LandID == SelectLand.LandID && t.VirtualPersonID == SelectLand.VirtualPersonID);
            if (br != null)
                brlist.Remove(br);
            dotView.Roots = null;
            dotView.Roots = brlist;
        }

        /// <summary>
        /// 添加地块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addland_Click(object sender, RoutedEventArgs e)
        {
            var bussnessObject = _bussiness.GetBussinessObject(CurrentZone);
            if (bussnessObject == null || bussnessObject.ContractLands.Count == 0)
            {
                ShowBox("提示", "暂无可划拨的确股地块！", eMessageGrade.Warn);
                return;
            }
            var alllands = bussnessObject.ContractLands.ToList();
            var cbrlist = dotView.Roots as List<BelongRelationUI>;
            foreach (var item in cbrlist)
            {
                alllands.RemoveAll(t => t.ID == item.LandID);
            }
            if (alllands.Count == 0)
            {
                ShowBox("提示", "暂无可划拨的确股地块！", eMessageGrade.Warn);
                return;
            }
            AllLandPage landGrid = new AllLandPage();
            landGrid.LandList = alllands;
            Workpage.Page.ShowDialog(landGrid, (b, ac) =>
            {
                if (!(bool)b)
                    return;
                var land = landGrid.ucLandGrid.SelectedItem;
                if (land == null)
                    return;
                var brlist = dotView.Roots as List<BelongRelationUI>;
                BelongRelationUI belong = new BelongRelationUI();
                belong.DKBM = land.LandNumber;
                belong.DKMC = land.Name;
                belong.SCMJM = land.ActualArea;
                belong.LandID = land.ID;
                if (VirtualItem != null)
                    belong.VirtualPersonID = VirtualItem.ID;
                belong.ZoneCode = land.ZoneCode;
                //belong. = brlist[0].ZoneCode;
                brlist.Add(belong);
                dotView.Roots = null;
                dotView.Roots = brlist;
            });
        }
        #endregion

    }

    public class BelongRelationUI : BelongRelation
    {
        private string dkmc;
        private string dkbm;
        private double scmjm;

        public int Img
        {
            get { return 1; }
        }

        public string DKMC
        {
            get { return dkmc; }
            set
            {
                dkmc = value;
                NotifyPropertyChanged(nameof(DKMC));
            }
        }

        public string DKBM
        {
            get { return dkbm; }
            set
            {
                dkbm = value;
                NotifyPropertyChanged(nameof(DKBM));
            }
        }

        public double SCMJM
        {
            get { return scmjm; }
            set
            {
                scmjm = value;
                NotifyPropertyChanged(nameof(SCMJM));
            }
        }
    }
}
