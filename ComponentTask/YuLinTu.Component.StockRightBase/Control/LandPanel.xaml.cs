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
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Component.StockRightBase.Bussiness;
using YuLinTu.Component.StockRightBase.Entity;
using YuLinTu.Windows;
using System.Collections.ObjectModel;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// LandPanel.xaml 的交互逻辑
    /// </summary>
    public partial class LandPanel : UserControl
    {
        private readonly TaskQueue _queueQuery;
        private Zone _currentZone;
        private IDbContext _dbContext;
        private Bussiness.BussinessData _bussiness;
        

        public IDbContext DbContext
        {
            get { return _dbContext; }
            set
            {
                _dbContext = value;
                _bussiness= new Bussiness.BussinessData(_dbContext,CurrentZone);
            }
        }


        public ContractLand SelectedItem
        {
            get { return LandGrid.SelectedItem; }
        }

        public ObservableCollection<ContractLand> Items
        {
            get { return LandGrid.Items; }
        } 
        public Bussiness.BussinessData Bussiness
        {
            get { return _bussiness; }
            set { _bussiness = value; }
        }

        public Action SelectChangedAction { get; set; }

        public Action ItemDoubleClick { get; set; }

        public IWorkpage Workpage { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return _currentZone; }
            set
            {
                _currentZone = value;
                InlitialControl(_currentZone == null ? "" : _currentZone.FullCode);
            }
        }

        public LandPanel()
        {
            InitializeComponent();
            _queueQuery = new TaskQueueDispatcher(Dispatcher);
        }

        /// <summary>
        /// 主界面初始化完成后执行
        /// </summary>
        public void Initlized()
        {
            InlitialControl(_currentZone == null ? "" : _currentZone.FullCode);
            LandGrid.SelectChangedAction += SelectChangedAction;
            LandGrid.ItemDoubleClick += ItemDoubleClick;
        }


        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InlitialControl(string zoneCode)
        {
            _queueQuery.Cancel();
            _queueQuery.DoWithInterruptCurrent(
                go =>
                {
                    DoWork(go);
                },
                completed =>
                {
                },
                terminated =>
                {
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                },
                start =>
                {
                    LandGrid.Items.Clear();
                    if(Workpage!=null)
                        Workpage.Page.IsBusy = true;
                }, ended =>
                {
                    if(Workpage!=null)
                        Workpage.Page.IsBusy = false;
                }, null, null, zoneCode);

        }

        private void DoWork(TaskGoEventArgs arg)
        {
            if (_currentZone != null)
            {
                var bussnessObject = _bussiness.GetBussinessObject(_currentZone);
                arg.Instance.ReportProgress(1, bussnessObject);
            }
        }

        /// <summary>
        /// 获取数据完成
        /// </summary>
        private void Changed(TaskProgressChangedEventArgs arg)
        {
            var value = arg.UserState as StockRightBussinessObject;
            if (value == null) return;
            double actualAreaTotal=0;
            double stockAreaTotal=0;
            foreach (var item in value.ContractLands)
            {
                actualAreaTotal += item.ActualArea;
                double d;
                double.TryParse(item.ShareArea,out d);
                stockAreaTotal += d;
            }

            LandGrid.view.Roots = value.ContractLands;

            LandStatistics.GetData(value.ContractLands.Count, actualAreaTotal, stockAreaTotal,value.CurrentZone.Name);

        }

        public void Refresh()
        {
            InlitialControl(_currentZone == null ? "" : _currentZone.FullCode);
        }


    }
}
