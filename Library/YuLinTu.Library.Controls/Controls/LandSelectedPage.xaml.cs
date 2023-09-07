/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 选择地块界面
    /// </summary>
    public partial class LandSelectedPage : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public LandSelectedPage(IWorkpage page)
        {
            InitializeComponent();
            DataContext = this;
            this.Workpage = page;
            ListObligee = new List<VirtualPerson>();
            ObligeeItems = new ObservableCollection<SelectObligee>();
            filePath = AppDomain.CurrentDomain.BaseDirectory + @"Config\SelectObligees.xml";
        }

        #endregion

        #region Field

        private List<VirtualPerson> listObligee;
        private ObservableCollection<SelectObligee> obligeeItems;
        private readonly string filePath;

        #endregion

        #region Property

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> ListObligee
        {
            get { return listObligee; }
            set
            {
                listObligee = value;
                if (listObligee != null && listObligee.Count > 0)
                {
                    var obligeeClone = CDObject.TryClone(value) as List<VirtualPerson>;
                    var items = DeserializeSelectObligee(filePath);
                    ObligeeItems = GetObligeeItems(obligeeClone, items);
                }
            }
        }

        /// <summary>
        /// 界面显示承包方集合
        /// </summary>
        public ObservableCollection<SelectObligee> ObligeeItems
        {
            get { return obligeeItems; }
            set
            {
                obligeeItems = value;
                lbObligeeItem.ItemsSource = obligeeItems;
                SetAllSelectCheck();
            }
        }

        #endregion

        #region Event

        /// <summary>
        /// 选中
        /// </summary>
        private void cbObligeeItem_Click(object sender, RoutedEventArgs e)
        {
            SetAllSelectCheck();
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void cbAllSelect_Click(object sender, RoutedEventArgs e)
        {
            bool check = (bool)cbAllSelect.IsChecked;
            foreach (var item in ObligeeItems)
            {
                item.Status = check;
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SerializeSelectObligee();
            var stObligees = GetSelectedObligee();
            if (stObligees == null || stObligees.Count == 0)
            {
                ShowBox("地块选择", "请至少选择一个地块所属承包方信息");
                return;
            }
            Workpage.Page.CloseMessageBox(true);
        }

        #endregion

        #region Method

        /// <summary>
        /// 获取界面显示承包方
        /// </summary>
        private ObservableCollection<SelectObligee> GetObligeeItems(List<VirtualPerson> listObligee, ObservableCollection<SelectObligee> configObligees)
        {
            if (listObligee == null || listObligee.Count == 0)
                return null;
            if (configObligees == null)
                configObligees = new ObservableCollection<SelectObligee>();
            ObservableCollection<SelectObligee> items = new ObservableCollection<SelectObligee>();
            listObligee.ForEach(c =>
            {
                var find = configObligees.FirstOrDefault(t => t.ID == c.ID);
                if (find == null)
                {
                    items.Add(new SelectObligee { Name = c.Name.Trim(), ID = c.ID, Status = false });
                    return;
                }
                items.Add(find);
            });
            return items;
        }

        /// <summary>
        /// 获取选中的目标承包方
        /// </summary>
        private List<VirtualPerson> GetSelectedObligee()
        {
            var items = ObligeeItems.Where(c => c.Status == true);
            if (items == null || items.Count() == 0)
                return null;
            List<VirtualPerson> obligeesTemp = new List<VirtualPerson>(items.Count());
            var lstItem = items.ToList();
            lstItem.ForEach(c =>
            {
                var obligee = ListObligee.Find(f => f.ID == c.ID);
                if (obligee == null)
                    return;
                obligeesTemp.Add(obligee);
            });
            listObligee.Clear();
            listObligee.AddRange(obligeesTemp);

            obligeesTemp.Clear();
            obligeesTemp = null;

            return listObligee;
        }

        /// <summary>
        /// 设置全选按钮
        /// </summary>
        private void SetAllSelectCheck()
        {
            if (ObligeeItems == null || ObligeeItems.Count == 0)
            {
                cbAllSelect.IsChecked = false;
                return;
            }
            bool isExsit = ObligeeItems.Any(c => c.Status == false);
            if (isExsit)
                cbAllSelect.IsChecked = false;
            else
                cbAllSelect.IsChecked = true;
        }

        /// <summary>
        /// 序列化文件
        /// </summary>
        private void SerializeSelectObligee()
        {
            if (ObligeeItems == null || ObligeeItems.Count == 0)
                return;
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            ToolSerialization.SerializeXml(filePath, ObligeeItems);
        }

        /// <summary>
        /// 反序列化文件
        /// </summary>
        private ObservableCollection<SelectObligee> DeserializeSelectObligee(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return null;
            var obj = ToolSerialization.DeserializeXml(fileName, typeof(ObservableCollection<SelectObligee>));
            if (obj == null)
                return null;
            return obj as ObservableCollection<SelectObligee>;
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            var dlg = new TabMessageBoxDialog()
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            };
            Workpage.Page.ShowMessageBox(dlg);
        }

        #endregion
    }

    /// <summary>
    /// 选中承包方
    /// </summary>
    public class SelectObligee : NotifyCDObject
    {
        /// <summary>
        /// 承包方名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 承包方唯一标识
        /// </summary>d
        public Guid ID { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }
        private bool status;
    }
}
