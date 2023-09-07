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
    /// 承包地块界址线编辑页面
    /// </summary>
    public partial class ConstructionLandCoilPage : InfoPageBase
    {
        #region Field

        private IDictionaryWorkStation dictStaion;
        private KeyValueList<string, string> lstJXXZContent;
        private KeyValueList<string, string> lstJZXLBContent;
        private KeyValueList<string, string> lstJZXWZContent;
        private BuildLandBoundaryAddressCoil currentCoil;
        private IDbContext dbContext;
        private bool isLock;

        #endregion

        #region Property

        /// <summary>
        /// 当前地块的所有界址点
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListDot { get; set; }

        /// <summary>
        /// 当前待编辑界址线
        /// </summary>
        public BuildLandBoundaryAddressCoil CurrentCoil
        {
            get { return currentCoil; }
            set { currentCoil = value; }
        }

        /// <summary>
        /// 当前界址线界面实体
        /// </summary>
        public ConstructionLandCoilItem CurrentCoilItem { get; set; }

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
        public ConstructionLandCoilPage()
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
                dictStaion = dbContext.CreateDictWorkStation();
                lstJXXZContent = dictStaion.GetCodeNameByGroupCode(DictionaryTypeInfo.JXXZ);    //界线性质
                lstJZXLBContent = dictStaion.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXLB);    //界址线类别
                lstJZXWZContent = dictStaion.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXWZ);    //界址线位置 
            }, null, null, started =>
            {
                Workpage.Page.IsBusy = true;
            }, null, comleted =>
            {
                if (lstJXXZContent != null)
                {
                    cbCoilProperty.ItemsSource = lstJXXZContent;
                    cbCoilProperty.DisplayMemberPath = "Value";
                    cbCoilProperty.SelectedIndex = 0;
                }

                if (lstJZXLBContent != null)
                {
                    cbCoilType.ItemsSource = lstJZXLBContent;
                    cbCoilType.DisplayMemberPath = "Value";
                    cbCoilType.SelectedIndex = 0;
                }

                if (lstJZXWZContent != null)
                {
                    cbCoilPosition.ItemsSource = lstJZXWZContent;
                    cbCoilPosition.DisplayMemberPath = "Value";
                    cbCoilPosition.SelectedIndex = 0;
                }

                if (currentCoil != null)
                    SetControlValue();

                Workpage.Page.IsBusy = false;
            }, null, terminated =>
            {
                ShowBox("编辑界址线", "获取数据字典失败!");
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
                if (currentCoil == null)
                    return false;
                return GoConfirm();
            }, completedCallback =>
            {
                Close(true);
            });
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
                CurrentCoilItem = currentCoil.ConvertToItem(ListDot, lstJXXZContent, lstJZXLBContent, lstJZXWZContent);
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                int upCount = coilStation.Update(currentCoil);
                result = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "提交承包地块界址线编辑失败", ex.Message + ex.StackTrace);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 设置控件值
        /// </summary>
        private void SetControlValue()
        {
            if (!string.IsNullOrEmpty(currentCoil.LineType) && lstJXXZContent != null)
            {
                var dict = lstJXXZContent.Find(c => c.Key == currentCoil.LineType);
                if (dict != null)
                    cbCoilProperty.SelectedItem = dict;
            }

            if (!string.IsNullOrEmpty(currentCoil.CoilType) && lstJZXLBContent != null)
            {
                var dict = lstJZXLBContent.Find(c => c.Key == currentCoil.CoilType);
                if (dict != null)
                    cbCoilType.SelectedItem = dict;
            }

            if (!string.IsNullOrEmpty(currentCoil.Position) && lstJZXWZContent != null)
            {
                var dict = lstJZXWZContent.Find(c => c.Key == currentCoil.Position);
                if (dict != null)
                    cbCoilPosition.SelectedItem = dict;
            }
        }

        /// <summary>
        /// 获取没有绑定属性值
        /// </summary>
        private void GetControlValue()
        {
            var coilProperty = cbCoilProperty.SelectedItem as KeyValue<string, string>;
            if (coilProperty != null)
                currentCoil.LineType = coilProperty.Key;
            var coilType = cbCoilType.SelectedItem as KeyValue<string, string>;
            if (coilType != null)
                currentCoil.CoilType = coilType.Key;
            var coilPosition = cbCoilPosition.SelectedItem as KeyValue<string, string>;
            if (coilPosition != null)
                currentCoil.Position = coilPosition.Key;
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

        #endregion
    }
}
