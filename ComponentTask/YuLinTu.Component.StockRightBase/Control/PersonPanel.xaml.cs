using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// VirtualPersonPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPanel : UserControl
    {

        private readonly TaskQueue _queueQuery;
        public IWorkpage Workpage { get; set; }

        private Zone _currentZone ;
        private IDbContext _dbContext;

        private Bussiness.BussinessData _bussiness;

        public Action SelectChangedAction { get; set; }

        public IDbContext DbContext
        {
            get { return _dbContext; }
            set
            {
                _dbContext = value;
                _bussiness = new Bussiness.BussinessData(_dbContext,CurrentZone);
            }
        }

        public ObservableCollection<PersonGridModel> Items
        {
            get
            {
                return personGrid.Items;
            }
        }


        public PersonGridModel SelectedItem
        {
            get { return personGrid.SelectedItem; }
        }


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

        public PersonPanel()
        {
            InitializeComponent();
            DataContext = this;
            _queueQuery = new TaskQueueDispatcher(Dispatcher);
        }


        public void Initlized()
        {
            InlitialControl(_currentZone == null ? "" : _currentZone.FullCode);
            personGrid.SelectChangedAction += SelectChangedAction; 
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
                    personGrid.Items.Clear();
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
            if(value == null)
                return;
            var sharePersonCount = 0;
            foreach (var item in value.VirtualPersons)
            {
                if (item.IsStockFarmer)
                {
                    var personModel = new PersonGridModel();
                    personModel.VirtualPerson = item;
                    personGrid.Items.Add(personModel);
                    //personGrid.view.SelectedIndex = 0;//不默认选中
                    sharePersonCount += item.SharePersonList.Count;
                }
            }
            personStatistics.GetData(sharePersonCount,value.VirtualPersons.Count);
        }



        public void Refresh()
        {
            InlitialControl(_currentZone == null ? "" : _currentZone.FullCode);
        }

    }
}
