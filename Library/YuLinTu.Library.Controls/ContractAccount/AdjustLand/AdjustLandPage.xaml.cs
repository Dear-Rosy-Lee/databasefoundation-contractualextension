using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YuLinTu.Data;
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
        private TaskQueueDispatcher queueQuery;
        private IDbContext dbContext;
        private string zoneCode;
        private List<ContractLand> lands;
        private List<VirtualPersonItem> virtualPersons;

        public List<AdjustLand> LandData { get; set; }

        public List<Tuple<AdjustLand, TextBlock>> SelectLandData { get; set; }

        public List<ContractLand> UpDateLands { get; private set; }

        public string NewVPName { get; set; }

        public string OldVPName { get; set; }

        public AdjustLandPage(IDbContext dbContext, string zoneCode, List<Dictionary> dic)
        {
            InitializeComponent();
            DictList = dic;
            this.dbContext = dbContext;
            this.zoneCode = zoneCode;
            lands = new List<ContractLand>();
            //virtualPersons = virtualPersons;
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueQuery.Cancel();
            queueQuery.DoWithInterruptCurrent(
                go =>
                {
                    var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                    var virtualPeoples = vpStation.GetByZoneCode(zoneCode);
                    go.Instance.ReportProgress(50, virtualPeoples);

                    var landStation = dbContext.CreateContractLandWorkstation();
                    var lands = landStation.GetCollection(zoneCode);
                    go.Instance.ReportProgress(100, lands);
                },
                completed =>
                {
                    GenerateDataItems();
                    //GetLandData(contractLands);  // 加载数据
                },
                terminated =>
                {
                    //ShowBox("提示", "获取数据时发生错误,可以尝试升级数据库解决问题。");
                },
                progressChanged =>
                {

                    if (progressChanged.Percent == 50)
                    {
                        virtualPersons = new List<VirtualPersonItem>();
                        var tvps = progressChanged.UserState as List<VirtualPerson>;
                        if (tvps.Count > 0)
                        {
                            foreach (var item in tvps)
                            {
                                virtualPersons.Add(new VirtualPersonItem()
                                {
                                    Tag = item,
                                    ID = item.ID,
                                    FamilyNumber = item.FamilyNumber,
                                    Name = $"{item.Name}({item.FamilyNumber})"
                                });
                            }
                        }
                        //virtualPersons = progressChanged.UserState as List<VirtualPerson>;
                    }
                    else if (progressChanged.Percent == 100)
                    {
                        var tlands = progressChanged.UserState as List<ContractLand>;
                        HashSet<Guid> ids = new HashSet<Guid>();
                        Dictionary<Guid, string> idCodeDic = new Dictionary<Guid, string>();
                        foreach (var item in virtualPersons)
                        {
                            if (!idCodeDic.ContainsKey(item.ID))
                                idCodeDic.Add(item.ID, item.Tag.FamilyNumber);

                            if (item.Tag.Name.Contains("集体") && int.Parse(item.Tag.FamilyNumber) > 9000)
                                continue;
                            if (!ids.Contains(item.ID))
                                ids.Add(item.ID);
                        }
                        lands.Clear();

                        foreach (var item in tlands)
                        {
                            if (item.OwnerId == null || (item.OwnerId != null && !ids.Contains(item.OwnerId.Value)))
                            {
                                lands.Add(item);
                            }
                        }
                        GetLandData(lands, idCodeDic);
                    }
                },
                start =>
                {
                    //TheWorkPage.Page.IsBusy = true;
                }, ended =>
                {
                    //TheWorkPage.Page.IsBusy = false;
                }, null, null, null);
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
            SenderComboBox.ItemsSource = virtualPersons;//.Select(t => $"{t.Name}({t.FamilyNumber})");
            //SenderComboBox.SelectedItem = $"{dataItem.CBFMC}({virtualPersons.Where(t => t.ID == dataItem.CBFId).FirstOrDefault().FamilyNumber})";
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
                NewVPName = (comboBox.SelectedItem as VirtualPersonItem)?.Name;

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

        private void GetLandData(List<ContractLand> lands, Dictionary<Guid, string> idCodeDic)
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

                if (item.OwnerId != null && idCodeDic.ContainsKey(item.OwnerId.Value))
                {
                    adjustLand.CBFMC = $"{item.OwnerName}({idCodeDic[item.OwnerId.Value]})";
                }
                else
                {
                    adjustLand.CBFMC = item.OwnerName;
                }
                adjustLand.YCBFMC = item.OwnerName;
                adjustLand.CBFId = (Guid)item.OwnerId;
                LandData.Add(adjustLand);
            }
        }

        /// <summary>
        /// 提交保存
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            UpDateLands = new List<ContractLand>();
            var vp = SenderComboBox.SelectedItem as VirtualPersonItem;
            if (vp == null)
            {
                CanClose = false;
                return;
            }
            else
            {
                CanClose = true;
            }
            foreach (var item in SelectLandData.Select(tuple => tuple.Item1).ToList())
            {
                var nland = lands.Find(q => q.ID == item.Id);
                if (nland != null)
                {
                    nland.OwnerName = vp.Tag.Name;
                    nland.OwnerId = vp.ID;
                    UpDateLands.Add(nland);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CanClose = true;
            this.Close();
        }
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateNewFBFMC(NewVPName/*.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]*/);
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
