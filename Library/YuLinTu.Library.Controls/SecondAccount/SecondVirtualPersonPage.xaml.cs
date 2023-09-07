/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// SecondVirtualPersonPage.xaml 的交互逻辑
    /// </summary>
    public partial class SecondVirtualPersonPage : InfoPageBase
    {
        #region Fields

        private Zone currentZone;
        private IWorkpage thePage;
        #endregion

        #region Property

        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                this.personPanel.CurrentZone = value;
            }
        }

        /// <summary>
        /// ThePage
        /// </summary>
        public IWorkpage ThePage
        {
            get { return thePage; }
            set { thePage = value; this.personPanel.ThePage = thePage; }
        }

        #endregion

        public SecondVirtualPersonPage()
        {
            InitializeComponent();
            IDbContext db = DataBaseSource.GetDataBaseSource(); 
            VirtualPersonBusiness business = new VirtualPersonBusiness(db);  //绑定业务数据
            this.personPanel.Business = business;
            this.personPanel.IsShowBatch = false;
            this.personPanel.DataContext = db;
            this.personPanel.ShowEqualColum = false;
            this.personPanel.ShowRelationColum = false;
            this.personPanel.ShowTableColum = true;
            this.personPanel.VirtualType = eVirtualType.SecondTable;   //根据承包方类型操作不同的表;
        }

        /// <summary>
        /// 增添承包人
        /// </summary>
        private void AddVirtualPerson_Click(object sender, RoutedEventArgs e)
        {
            personPanel.AddVirtualPerson();
        }

        /// <summary>
        /// 添加共有人  必须选中某个承包方
        /// </summary>
        private void AddSharePersonBtn_Click(object sender, RoutedEventArgs e)
        {
            personPanel.AddSharePerson();
        }

        /// <summary>
        /// 编辑  
        /// 如果选中承包方编辑承包方界面
        /// 如果选中共有人编辑共有人界面
        /// </summary>
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            personPanel.EditPerson();
        }

        /// <summary>
        /// 删除 
        /// 如果选中承包方，则删除承包方下所有的共有人
        /// 如果选中共有人，则直接删除共有人，在共有人下选择承包方是不能被删除的
        /// </summary>
        private void DeltBtn_Click(object sender, RoutedEventArgs e)
        {
            personPanel.DelPerson(); // 
        }

        /// <summary>
        /// 设置承包方
        /// 将共有人与承包方调换
        /// </summary>
        private void SetFamilyBtn_Click(object sender, RoutedEventArgs e)
        {
            personPanel.VirtualPersonSet();
        }

        /// <summary>
        /// 退出承包方管理界面
        /// </summary>
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close(true);
        }
    }
}
