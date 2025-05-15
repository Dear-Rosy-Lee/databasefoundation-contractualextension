using DotSpatial.Projections.Transforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows.Wpf.Metro.Components;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using Separator = YuLinTu.Windows.Wpf.Metro.Components.Separator;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// AdjustSender.xaml 的交互逻辑
    /// </summary>
    public partial class AdjustSenderPage : TabMessageBox
    {

        public List<AdjustSender> SenderData { get; set; }

        public List<Tuple<AdjustSender, TextBlock>> SelectSenderData { get; set; } 

        public List<CollectivityTissue> Senders { get; set; }

        public List<ContractLand> Lands { get; set; }

        public List<string> SendersName { get; set; }

        public string NewSenderName { get; set; }

        public string OldSenderName { get; set; }

        public AdjustSenderPage(List<VirtualPerson> selectedPersons,List<CollectivityTissue> collectivityTissues,List<ContractLand> contractLands)
        {
            InitializeComponent();
            Senders = collectivityTissues;
            Lands = contractLands;
            SendersName = new List<string>();
            SendersName = Senders.Select(t => t.Name).ToList();
            GetSenderData(selectedPersons);  // 加载数据
            GenerateDataItems();
        }
        

        private void GenerateDataItems()
        {
            DataItemsControl.ItemsSource = SenderData;
        }
        private void CheckBox_AllChecked(object sender, RoutedEventArgs e)
        {
            SetAllItemsCheckState(true);
        }
        private void CheckBox_AllUnchecked(object sender, RoutedEventArgs e)
        {
            SetAllItemsCheckState(false);
        }
        private void SetAllItemsCheckState(bool isChecked)
        {

            if (  SenderData is IList<AdjustSender> dataList)
            {
                foreach (var item in dataList)
                {
                    item.IsSelected = isChecked; // 假设数据类有IsSelected属性
                }
                return;
            }

        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T result)
                    return result;
                var childResult = FindVisualChild<T>(child);
                if (childResult != null)
                    return childResult;
            }
            return null;
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SenderComboBox.Visibility = Visibility.Visible;
            SenderComboBox.ItemsSource = Senders.Select(t => t.Name);
            SenderComboBox.SelectedItem = OldSenderName;
            var checkBox = (CheckBox)sender;
            var dataItem = (AdjustSender)checkBox.Tag;
            AddData(checkBox.Parent as Grid, checkBox);
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var dataItem = (AdjustSender)checkBox.Tag;
            DelData(checkBox.Parent as Grid, checkBox);
        }

        private void AddData(Grid parent,CheckBox checkBox)
        {
            SenderComboBox.Visibility = Visibility.Visible;
            var item = (AdjustSender)checkBox.Tag;
            var tuple = new Tuple<AdjustSender, TextBlock>(item, FindFBFMCTextBlock(parent));
            SelectSenderData.Add(tuple);
        }
        private void DelData(Grid parent, CheckBox checkBox)
        {
            var item = (AdjustSender)checkBox.Tag;
            var tuple = new Tuple<AdjustSender, TextBlock>(item, FindFBFMCTextBlock(parent));
            SelectSenderData.Remove(tuple);
        }
        
        private void SenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // 显示操作按钮
                btnConfirm.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;

                // 更新选中值
                NewSenderName = comboBox.SelectedItem?.ToString();
                
            }
        }
       

        // 更新关联的发包方名称
        private void UpdateFBFMC(string FBFMC)
        {
            if (SelectSenderData != null)
            {
                foreach (var item in SelectSenderData)
                {
                    item.Item2.Text = FBFMC;
                }
            }
            
        }
        private TextBlock FindFBFMCTextBlock(Grid parent)
        {
            foreach (var child in parent.Children)
            {
                if (child is TextBlock textBlock && Grid.GetColumn(textBlock) == 6)
                {
                    return textBlock;
                }
            }
            return null;
        }
        private void GetSenderData(List<VirtualPerson> ListVirtualPeople)
        {
            SelectSenderData = new List<Tuple<AdjustSender, TextBlock>>();
            SenderData = new List<AdjustSender>();
            var zoneCode = ListVirtualPeople.FirstOrDefault().ZoneCode;
            var sender = Senders.Where(x => x.Code == zoneCode).FirstOrDefault();
            OldSenderName = sender.Name;
            foreach (var item in ListVirtualPeople)
            {
                var landCount = Lands.Where(t => t.OwnerId == item.ID).Count();
                AdjustSender adjustSender = new AdjustSender();
                adjustSender.Id = item.ID;
                adjustSender.IsSelected = false;
                adjustSender.HH = item.FamilyNumber;
                adjustSender.MC = item.Name;
                adjustSender.ZJHM = item.Number;
                adjustSender.CYSL = item.PersonCount;
                adjustSender.DKSL = landCount;
                adjustSender.FBFMC = sender.Name;
                SenderData.Add(adjustSender);
            }

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateFBFMC(NewSenderName);
            e.Handled = true; // 阻止事件继续处理
        }
        private void celButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateFBFMC(OldSenderName);
            e.Handled = true; // 阻止事件继续处理
        }

    }

}
