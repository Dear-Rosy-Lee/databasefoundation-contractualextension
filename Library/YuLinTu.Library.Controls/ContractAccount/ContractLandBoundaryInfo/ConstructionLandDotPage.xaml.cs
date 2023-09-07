/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利 
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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Data;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包地快界址点编辑页面
    /// </summary>
    public partial class ConstructionLandDotPage : InfoPageBase
    {
        #region Field

        private IDictionaryWorkStation dictStation;
        private KeyValueList<string, string> lstJZDLXContent;
        private KeyValueList<string, string> lstJBLXContent;
        private BuildLandBoundaryAddressDot currentDot;
        private IDbContext dbContext;
        private bool isLock;

        #endregion

        #region Property

        /// <summary>
        /// 当前待编辑界址点
        /// </summary>
        public BuildLandBoundaryAddressDot CurrentDot
        {
            get { return currentDot; }
            set { currentDot = value; }
        }

        /// <summary>
        /// 当前界址点界址点界面实体
        /// </summary>
        public ConstructionLandDotItem CurrentDotItem { get; set; }

        /// <summary>
        /// 承包地被锁定
        /// </summary>
        public bool IsLock
        {
            get { return isLock; }
            set
            {
                isLock = value;
                if (isLock)
                {
                    var enmu = (this.Content as System.Windows.Controls.Grid).Children.GetEnumerator();
                    while (enmu.MoveNext())
                    {
                        var curLbEle = enmu.Current as Label;
                        if (curLbEle != null)
                            continue;
                        var curBtnEle = enmu.Current as System.Windows.Controls.Border;
                        if (curBtnEle != null)
                            continue;
                        var curEle = enmu.Current as UIElement;
                        curEle.IsEnabled = false;
                    }
                }
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConstructionLandDotPage()
        {
            InitializeComponent();
            DataContext = this;
            InitializeControl();
        }

        #endregion

        #region Method - Initialize

        /// <summary>
        /// 初始化界面控件值
        /// </summary>
        private void InitializeControl()
        {
            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                dbContext = DataBaseSource.GetDataBaseSource();
                dictStation = dbContext.CreateDictWorkStation();
                lstJZDLXContent = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZDLX);    //界址点类型
                lstJBLXContent = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JBLX);    //界标类型
            }, null, null, started =>
            {
                Workpage.Page.IsBusy = true;
            }, null, comleted =>
            {
                if (lstJZDLXContent != null)
                {
                    cbDotType.ItemsSource = lstJZDLXContent;
                    cbDotType.DisplayMemberPath = "Value";
                    cbDotType.SelectedIndex = 0;
                }

                if (lstJBLXContent != null)
                {
                    cbDotMark.ItemsSource = lstJBLXContent;
                    cbDotMark.DisplayMemberPath = "Value";
                    cbDotMark.SelectedIndex = 0;
                }
                txtDotNumberPrefix.Text = "J";  //默认设置为J前缀

                if (currentDot != null)
                    SetControlValue();

                Workpage.Page.IsBusy = false;
            }, null, terminated =>
            {
                ShowBox("编辑界址点", "获取数据字典失败!");
                return;
            }).StartAsync();
        }

        #endregion

        #region Method - Event

        /// <summary>
        /// 确定
        /// </summary>
        private void mbtnLandOK_Click(object sender, RoutedEventArgs e)
        {
            GetControlValue();
            ConfirmAsync(goCallback =>
            {
                if (currentDot == null)
                    return false;
                return GoConfirm();
            }, completedCallback =>
            {
                Close(true);
            });
        }

        /// <summary>
        /// 界址点号文本输入
        /// </summary>
        private void txtDotNumberPrefix_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string prefix = e.Text.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(prefix, "^[a-zA-Z]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                e.Handled = true;
            else
                e.Handled = false;
        }

        /// <summary>
        /// 界址点号文本框键盘按下
        /// </summary>
        private void txtDotNumberPrefix_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        #endregion

        #region Method - Private

        /// <summary>
        /// 确定
        /// </summary>
        private bool GoConfirm()
        {
            bool result = false;
            try
            {
                CurrentDotItem = currentDot.ConvertToItem(lstJBLXContent, lstJZDLXContent);
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                int upCount = dotStation.Update(currentDot);
                result = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "提交承包地块界址点编辑失败", ex.Message + ex.StackTrace);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 设置控件值
        /// </summary>
        private void SetControlValue()
        {
            if (!string.IsNullOrEmpty(currentDot.DotNumber))
            {
                var dotNumberCh = currentDot.DotNumber.ToCharArray();
                txtDotNumberPrefix.Text = dotNumberCh[0].ToString();
            }

            if (!string.IsNullOrEmpty(currentDot.DotType) && lstJZDLXContent != null)
            {
                var dict = lstJZDLXContent.Find(c => c.Key == currentDot.DotType);
                if (dict != null)
                    cbDotType.SelectedItem = dict;
            }

            if (!string.IsNullOrEmpty(currentDot.LandMarkType) && lstJBLXContent != null)
            {
                var dict = lstJBLXContent.Find(c => c.Key == currentDot.LandMarkType);
                if (dict != null)
                    cbDotMark.SelectedItem = dict;
            }
        }

        /// <summary>
        /// 获取没有绑定属性值
        /// </summary>
        private void GetControlValue()
        {
            var prefix = txtDotNumberPrefix.Text.Trim();
            if (!string.IsNullOrEmpty(currentDot.DotNumber))
            {
                string expression = @"[a-z]|[A-Z]";
                System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(expression);
                currentDot.DotNumber = pattern.Replace(currentDot.DotNumber, prefix);
                currentDot.UniteDotNumber = pattern.Replace(currentDot.UniteDotNumber, prefix);
                //currentDot.UniteDotNumber = currentDot.DotNumber;
            }
            var dotType = cbDotType.SelectedItem as KeyValue<string, string>;
            if (dotType != null)
                currentDot.DotType = dotType.Key;
            var dotMark = cbDotMark.SelectedItem as KeyValue<string, string>;
            if (dotMark != null)
                currentDot.LandMarkType = dotMark.Key;
        }

        /// <summary>
        /// 消息提示
        /// </summary>
        private void ShowBox(string title, string message, eMessageGrade type = eMessageGrade.Error)
        {
            var dlg = new TabMessageBoxDialog()
            {
                Header = title,
                Message = message,
                MessageGrade = type
            };
            Workpage.Page.ShowMessageBox(dlg);
        }

        /// <summary>
        /// 判断是否是字母
        /// </summary>
        private bool IsCharacter(string prefix)
        {
            if (prefix.IsNullOrBlank())
                return false;
            foreach (char item in prefix)
            {
                if (!Char.IsLetter(item))
                    return false;
            }
            return true;
        }

        #endregion
    }
}
