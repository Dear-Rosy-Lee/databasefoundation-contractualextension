/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Configuration;
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
    /// OptionsConnectionStrings.xaml 的交互逻辑
    /// </summary>
    public partial class OptionsConnectionStrings : InfoPageBase
    {

        #region Fields

        private ObservableCollection<ConnectionStringDescriptor> list;

        #endregion

        #region Properties

        public Configuration Configuration
        {
            get { return _Configuration; }
            set { SetConfiguration(value); }
        }

        private Configuration _Configuration;

        #endregion

        #region Ctor
        public OptionsConnectionStrings()
        {
            InitializeComponent();

            Dispatcher.Invoke(new Action(() =>
            {
                list = new ObservableCollection<ConnectionStringDescriptor>();

                treeViewer.ItemsSource = list;
            }));

        }

        #endregion


        #region Methods

        #region Methods - override

        protected override void OnInitializeGo()
        {

            
        }

        #endregion

        #region Methods - Privates

        private void SetConfiguration(Configuration value)
        {
            list.Clear();

            _Configuration = value;
            if (value == null)
                return;

            foreach (ConnectionStringSettings item in value.ConnectionStrings.ConnectionStrings)
            {
                ConnectionStringDescriptor descriptor = null;

                var meta = ConnectionStringsManager.GetConnectionStringTypeMetadata(item.ProviderName);
                if (meta == null)
                    descriptor = new ConnectionStringDescriptor(item);
                else
                    descriptor = Activator.CreateInstance(meta.DescriptorType, item) as ConnectionStringDescriptor;

                list.Add(descriptor);
            }
        }

        #endregion

        #region Methods - Events
        private void treeViewer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #endregion
    }
}
