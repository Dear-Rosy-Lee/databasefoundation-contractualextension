using DotSpatial.Projections.Transforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;


namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// AdjustSender.xaml 的交互逻辑
    /// </summary>
    public partial class AdjustLandPage : TabMessageBox
    {
        protected List<Dictionary> DictList;
        public List<AdjustLand> LandData { get; set; }

        public List<Tuple<AdjustLand, TextBlock>> SelectLandData { get; set; }

        public List<ContractLand> Lands { get; set; }

        List<VirtualPerson> VirtualPersons { get; set; }

        public string NewVPName { get; set; }

        public string OldVPName { get; set; }

        public AdjustLandPage(List<VirtualPerson> virtualPersons, List<ContractLand> contractLands, List<Dictionary> dic)
        {
            InitializeComponent();
            DictList = dic;
            Lands = contractLands;
            VirtualPersons = virtualPersons;
            GetLandData(contractLands);  // 加载数据
            GenerateDataItems();
        }


        private void GenerateDataItems()
        {
            DataItemsControl.ItemsSource = LandData;
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

            if (LandData is IList<AdjustLand> dataList)
            {
                foreach (var item in dataList)
                {
                    item.IsSelected = isChecked; 
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
            var checkBox = (CheckBox)sender;
            var dataItem = (AdjustLand)checkBox.Tag;
            SenderComboBox.Visibility = Visibility.Visible;
            SenderComboBox.ItemsSource = VirtualPersons.Select(t => $"{t.Name} （{t.FamilyNumber}）");
            SenderComboBox.SelectedItem = $"{dataItem.CBFMC} （{VirtualPersons.Where(t=>t.ID == dataItem.CBFId).FirstOrDefault().FamilyNumber}）";
            OldVPName = dataItem.CBFMC;
            AddData(checkBox.Parent as Grid, checkBox);
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var dataItem = (AdjustLand)checkBox.Tag;
            DelData(checkBox.Parent as Grid, checkBox);
        }

        private void AddData(Grid parent, CheckBox checkBox)
        {
            SenderComboBox.Visibility = Visibility.Visible;
            var item = (AdjustLand)checkBox.Tag;
            var tuple = new Tuple<AdjustLand, TextBlock>(item, FindCBFMCTextBlock(parent));
            SelectLandData.Add(tuple);
        }
        private void DelData(Grid parent, CheckBox checkBox)
        {
            var item = (AdjustLand)checkBox.Tag;
            var tuple = new Tuple<AdjustLand, TextBlock>(item, FindCBFMCTextBlock(parent));
            SelectLandData.Remove(tuple);
        }

        private void SenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // 显示操作按钮
                btnConfirm.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;
                
                // 更新选中值
                NewVPName = comboBox.SelectedItem?.ToString();
                
            }
        }


        // 更新关联的发包方名称
        private void UpdateOldFBFMC()
        {
            if (SelectLandData != null)
            {
                foreach (var item in SelectLandData)
                {
                    item.Item2.Text = item.Item1.YCBFMC;
                }
            }

        }
        private void UpdateNewFBFMC(string CBFMC)
        {
            if (SelectLandData != null)
            {
                foreach (var item in SelectLandData)
                {
                    item.Item2.Text = CBFMC;
                }
            }

        }
        private TextBlock FindCBFMCTextBlock(Grid parent)
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
        private void GetLandData(List<ContractLand> lands)
        {
            var dictDKLB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DKLB);
            SelectLandData = new List<Tuple<AdjustLand, TextBlock>>();
            LandData = new List<AdjustLand>();
            foreach (var item in lands)
            {
                Dictionary dklb = dictDKLB.Find(c => c.Name.Equals(item.LandCategory) || c.Code.Equals(item.LandCategory));
                AdjustLand adjustLand = new AdjustLand();
                adjustLand.Id = item.ID;
                adjustLand.IsSelected = false;
                adjustLand.DKBM = item.LandNumber.Substring(14);
                adjustLand.DKMC = item.Name;
                adjustLand.DKLB = dklb.Name;
                adjustLand.HTMJ = item.AwareArea;
                adjustLand.SCMJ = item.ActualArea;
                adjustLand.CBFMC = item.OwnerName;
                adjustLand.YCBFMC = item.OwnerName;
                adjustLand.CBFId = (Guid)item.OwnerId;
                LandData.Add(adjustLand);
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
            UpdateNewFBFMC(NewVPName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
            e.Handled = true; // 阻止事件继续处理
        }
        private void celButton_Click(object sender, RoutedEventArgs e)
        {
            SenderComboBox.SelectedItem = OldVPName;
            UpdateOldFBFMC();
            e.Handled = true; // 阻止事件继续处理
        }

    }

}
