/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 显示所有的承包方，并且可以选择承包方
    /// </summary>
    public partial class VirtualPesonSelectPanel : InfoPageBase
    {

        #region Fields

        private ObservableCollection<VirtualPerson> checkpersons;
        private VirtualPerson currentPerson;
        private List<NameToEntity> NameList = new List<NameToEntity>();

        #endregion

        #region Propertys

        /// <summary>
        /// 承包方集合
        /// </summary>
        public ObservableCollection<VirtualPerson> CheckPersons
        {
            get
            {
                return checkpersons;
            }
            set
            {
                checkpersons = value;
                //NameList.Clear();
                //foreach (VirtualPerson vp in CheckPersons)
                //{
                //    NameToEntity nte = new NameToEntity();
                //    nte.NamePY = YuLinTu.Library.WorkStation.ToolChinese.MakeSpellCode(vp.Name);
                //    nte.NameQPY = YuLinTu.Library.WorkStation.ToolChinese.MakeSpellCode(vp.Name, YuLinTu.Library.WorkStation.eSpellOptions.EnableUnicodeLetter);
                //    nte.VirtualPerson = vp;
                //    nte.Name = vp.Name;
                //    nte.Number = vp.Number;
                //    NameList.Add(nte);
                //}
                //BindToControl(NameList);
            }
        }

        /// <summary>
        /// 被选择承包方
        /// </summary>
        public VirtualPerson CurrentPerson
        {
            get { return currentPerson; }
            private set
            {
                currentPerson = value;
                if (currentPerson != null)
                {
                    btnComit.IsEnabled = true;
                }
                else
                {
                    btnComit.IsEnabled = false;
                }
            }
        }

        public IWorkpage theWorkpage { get; set; }

        public Zone CurrenZone { get; set; }

        #endregion

        #region Ctor

        public VirtualPesonSelectPanel()
        {
            InitializeComponent();
        }
        protected override void OnInitializeCompleted()
        {
            base.OnInitializeCompleted();
            NameList.Clear();
            foreach (VirtualPerson vp in CheckPersons)
            {
                NameToEntity nte = new NameToEntity();
                nte.NamePY = YuLinTu.Library.WorkStation.ToolChinese.MakeSpellCode(vp.Name);
                nte.NameQPY = YuLinTu.Library.WorkStation.ToolChinese.MakeSpellCode(vp.Name, YuLinTu.Library.WorkStation.eSpellOptions.EnableUnicodeLetter);
                nte.VirtualPerson = vp;
                nte.Name = vp.Name;
                nte.Number = vp.Number;
                nte.Gender = ToolICN.GenderImgeString(vp.Number);
                NameList.Add(nte);
            }
            BindToControl(NameList);
        }

        #endregion

        #region Methods

        #region Methods - Events

        /// <summary>
        /// 承包方选中
        /// </summary>
        private void boxHosts_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            NameToEntity vp = boxHosts.SelectedItem as NameToEntity;
            if (vp != null)
            {
                CurrentPerson = vp.VirtualPerson;
            }
        }
        //双击选择
        private void boxHosts_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            NameToEntity vp = boxHosts.SelectedItem as NameToEntity;
            if (vp != null)
            {
                CurrentPerson = vp.VirtualPerson;
            }
            if (theWorkpage != null)
            {
                theWorkpage.Page.CloseMessageBox(true);
            }
            else
            {
                Workpage.Page.CloseMessageBox(true);
            }
        }


        /// <summary>
        /// 提交选择的结果
        /// </summary>
        private void btnComit_Click(object sender, RoutedEventArgs e)
        {
            if (theWorkpage != null)
            {
                theWorkpage.Page.CloseMessageBox(true);
            }
            else
            {
                Workpage.Page.CloseMessageBox(true);
            }
        }

        /// <summary>
        /// 快速过滤
        /// </summary>
        private void txtwh_TextChanged(object sender, TextChangedEventArgs e)
        {
            string wh = txtwh.Text.Trim();
            bool checkd = cbAll.IsChecked.Value;
            ChangeVirtualPersons(wh, checkd);
        }

        /// <summary>
        /// 全匹配过滤
        /// </summary>
        private void cbAll_Checked(object sender, RoutedEventArgs e)
        {
            string wh = txtwh.Text.Trim();
            bool checkd = cbAll.IsChecked.Value;
            ChangeVirtualPersons(wh, checkd);
        }

        #endregion

        #region Methods - Privates

        /// <summary>
        /// 绑定到控件
        /// </summary>
        private void BindToControl(List<NameToEntity> entitys)
        {
            Dispatcher.Invoke(() =>
            {
                boxHosts.ItemsSource = null;
                boxHosts.ItemsSource = entitys;
            });
        }

        /// <summary>
        /// 过滤承包方
        /// </summary>
        private void ChangeVirtualPersons(string wh, bool ischeck)
        {
            if (string.IsNullOrEmpty(wh))
            {
                BindToControl(NameList);
                return;
            }
            List<NameToEntity> vpc = new List<NameToEntity>();
            if (ischeck == true)
            {
                vpc = NameList.FindAll(t =>
                (!string.IsNullOrEmpty(t.NamePY) && t.NamePY == wh.ToUpper()) ||
                (!string.IsNullOrEmpty(t.Name) && t.Name == wh) ||
                (!string.IsNullOrEmpty(t.NameQPY) && t.NameQPY == wh.ToUpper()) ||
                (!string.IsNullOrEmpty(t.Number) && t.Number == wh));
            }
            else
            {
                vpc = NameList.FindAll(t =>
                (!string.IsNullOrEmpty(t.NamePY) && t.NamePY.Contains(wh.ToUpper())) ||
                (!string.IsNullOrEmpty(t.Name) && t.Name.Contains(wh.ToUpper())) ||
                (!string.IsNullOrEmpty(t.NameQPY) && t.NameQPY.Contains(wh.ToUpper())) ||
                (!string.IsNullOrEmpty(t.Number) && t.Number.Contains(wh)));
            }
            BindToControl(vpc);
        }

        #endregion

        #endregion

        class NameToEntity
        {
            public string Name { get; set; }
            public string Number { get; set; }
            public VirtualPerson VirtualPerson { get; set; }
            public string NamePY { get; set; }
            public string NameQPY { get; set; }
            public string Gender { get; set; }
        }
    }
}
