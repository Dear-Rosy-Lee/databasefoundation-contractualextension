/*
 * (C) 2018  鱼鳞图公司版权所有,保留所有权利 
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
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// ZoneSelectorPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ZoneSelectorPanel : InfoPageBase
    {
        #region Fields

        private ZoneDataItem rootZone;

        #endregion

        #region Properties

        /// <summary>
        /// 区域实例 包括地域名称 和 地域全编码
        /// </summary>
        public ZoneDataItem RootZone
        {
            get { return rootZone; }
            set
            {
                rootZone = value;
            }
        }

        public ZoneDataItem SelectorZone { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public IDbContext DbContext { get; set; }

        #endregion

        #region Ctor

        public ZoneSelectorPanel()
        {
            InitializeComponent();
            DataContext = this;

            Confirm += ZoneSelectorPanel_Confirm;
        }

        #endregion


        #region Methods

        #region Methods - Events

        /// <summary>
        /// 重载
        /// </summary>
        protected override void OnInitializeGo()
        {
            RootZone = new ZoneDataItem()
            {
                FullCode = "86",
                FullName = "中国",
                Name = "中国",
            };
            //System.Threading.Thread.Sleep(2000);
        }

        protected override void OnInitializeCompleted()
        {
            treeViewer.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<ZoneDataItem>() { RootZone };
            treeViewer.ExpandAsync(objs =>
            {
                foreach (var item in objs)
                {
                    var obj = item as ZoneDataItem;
                    if (obj == null)
                        continue;
                    if ((SelectorZone.FullCode != null && SelectorZone.FullCode != "86" && SelectorZone.FullCode.StartsWith(obj.FullCode)) || obj.FullCode == "86")
                        return obj;
                }
                return null;
            });
        }

        void ZoneSelectorPanel_Confirm(object sender, MsgEventArgs<bool> e)
        {
            //System.Threading.Thread.Sleep(2000);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync();
        }

        /// <summary>
        /// 展开某个节点时响应
        /// 当一个父节点展开时,动态读取数据库的下级区域,并添加到子节点中
        /// </summary>
        private void treeViewer_ItemsGetter(object sender, MetroViewItemItemsEventArgs e)
        {
            var obj = e.Object as ZoneDataItem;
            if (obj == null)         //对于传入的参数有个检查
                return;
            var db = GetDb();
            ContainerFactory factroy = new ContainerFactory(db);
            string rootCode = obj.FullCode;
            if (!string.IsNullOrEmpty(rootCode))
            {
                IZoneWorkStation station = factroy.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
                var list = station.Get(c => c.UpLevelCode == rootCode).Select(c => c.ConvertTo<ZoneDataItem>()).ToList();
                //陈泽林 20161209
                list.Sort((a, b) =>
                {
                    int an = int.Parse(a.Code);
                    int bn = int.Parse(b.Code);
                    return an.CompareTo(bn);
                });
                Dispatcher.Invoke(new Action(() => list.ForEach(c => obj.Children.Add(c))));
            }
        }

        /// <summary>
        /// 某个项是否有子节点
        /// 若没有, 则不现实展开折叠按钮
        /// 若有, 则显示展开折叠按钮
        /// </summary>
        private void treeViewer_HasItemsGetter(object sender, MetroViewItemHasItemsEventArgs e)
        {
            var obj = e.Object as ZoneDataItem;
            if (obj == null)         //对于传入的参数有个检查
                return;
            var db = GetDb();
            ContainerFactory factroy = new ContainerFactory(db);
            string rootCode = obj.FullCode;
            if (!string.IsNullOrEmpty(rootCode))
            {
                IZoneWorkStation station = factroy.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
                e.HasItems = station.Any(c => c.UpLevelCode == rootCode);
            }
        }

        /// <summary>
        /// 选中某个节点时调用
        /// </summary>
        private void treeViewer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var obj = e.NewValue as ZoneDataItem;
            if (obj == null)
            {
                return;
            }
            this.rootZone = obj;
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns></returns>
        private IDbContext GetDb()
        {
            if (DbContext == null)
                return DataBaseSourceWork.GetDataBaseSource();
            else
                return DbContext;
        }

        #endregion

        #endregion
    }
}
