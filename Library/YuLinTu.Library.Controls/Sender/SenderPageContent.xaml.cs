/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Input;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 发包方编辑页内容
    /// </summary>
    public partial class SenderPageContent : UserControl
    {
        #region Ctor

        public SenderPageContent()
        {
            InitializeComponent();
            InitiallData();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 证件类型集合
        /// </summary>
        public List<EnumStore<eCredentialsType>> CredtypeList { get; set; }

        /// <summary>
        /// 发包方状态集合
        /// </summary>
        public List<EnumStore<eStatus>> SatusList { get; set; }

        /// <summary>
        /// 证件类型集合
        /// </summary>
        public List<EnumStore<eTissueType>> TissuetypeList { get; set; }

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        public Zone CurrentZone { get; set; }

        public string selectZoneCode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitiallData()
        {
            CredtypeList = EnumStore<eCredentialsType>.GetListByType();
            cbCredtype.DisplayMemberPath = "DisplayName";
            cbCredtype.ItemsSource = CredtypeList;
            cbCredtype.SelectedIndex = 0;

            //SatusList = EnumStore<eStatus>.GetListByType();
            //cbStatus.DisplayMemberPath = "DisplayName";
            //cbStatus.ItemsSource = SatusList;
            //cbStatus.SelectedIndex = 1;

            //TissuetypeList = EnumStore<eTissueType>.GetListByType();
            //cbSenderKind.DisplayMemberPath = "DisplayName";
            //cbSenderKind.ItemsSource = TissuetypeList;
            //cbSenderKind.SelectedIndex = 0;
        }

        /// <summary>
        /// 地域编码变化
        /// </summary>
        private void MetroTextBox_CodeChanged(object sender, TextChangedEventArgs e)
        {
            //tempTissue.Code = mtbCode.Text.Trim();
            //tempTissue.FullCode = (currentTissue.Level == eZoneLevel.Province) ? tempTissue.Code : currentTissue.UpLevelCode + tempTissue.Code;
            //if (!isAdd)
            //{
            //    btnSubmit.IsEnabled = EditChanged();
            //}
            //else
            //{
            //    btnSubmit.IsEnabled = CanSubmit();
            //}
        }

        /// <summary>
        /// 备注变化
        /// </summary>
        private void MetroTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //tempTissue.Comment = mtbComment.Text.Trim();
            //if (!isAdd)
            //{
            //    btnSubmit.IsEnabled = EditChanged();
            //}
            //else
            //{
            //    btnSubmit.IsEnabled = CanSubmit();
            //}
        }


        /// <summary>
        /// 检测输入
        /// </summary>
        private void mtbCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        /// <summary>
        /// 按键盘判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        /// <summary>
        /// isDigit是否是数字 
        /// </summary>
        public bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 证件类型选择改变
        /// </summary>
        private void cbCredtype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnumStore<eCredentialsType> store = cbCredtype.SelectedValue as EnumStore<eCredentialsType>;
            eCredentialsType type = store.Value;
        }

        /// <summary>
        /// 发包方类型选择改变
        /// </summary>
        //private void cbSenderKind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    EnumStore<eTissueType> store = cbSenderKind.SelectedValue as EnumStore<eTissueType>;
        //    eTissueType type = store.Value;
        //}

        /// <summary>
        /// 发包方状态
        /// </summary>
        //private void cbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    EnumStore<eStatus> store = cbStatus.SelectedValue as EnumStore<eStatus>;
        //    eStatus type = store.Value;
        //}

        /// <summary>
        /// 检查实体
        /// </summary>
        public bool CheckEntity(CollectivityTissue tempTissue, bool isAdd = true)
        {
            bool canContinue = true;
            if (ToolString.ExceptSpaceString(tempTissue.Name).Length < 1)
            {
                ShowBox(isAdd, SenderInfo.SenderNameError);
                canContinue = false;
                return canContinue;
            }
            if (!string.IsNullOrEmpty(tempTissue.LawyerPosterNumber) && (!ToolMath.MatchEntiretyNumber(tempTissue.LawyerPosterNumber) || tempTissue.LawyerPosterNumber.Length != 6))
            {
                ShowBox(isAdd, SenderInfo.SenderPosterNumberError);
                canContinue = false;
                return canContinue;
            }
            if (!string.IsNullOrEmpty(tempTissue.LawyerTelephone) && !ToolMath.MatchEntiretyNumber(tempTissue.LawyerTelephone.Replace("+", "").Replace("-", "")))
            {
                ShowBox(isAdd, SenderInfo.SenderPoneNumberError);
                canContinue = false;
                return canContinue;
            }
            if (!string.IsNullOrEmpty(tempTissue.LawyerCartNumber))
            {
                if (!ToolMath.MatchEntiretyNumber(tempTissue.LawyerCartNumber.Replace("x", "").Replace("X", "")))
                {
                    ShowBox(isAdd, SenderInfo.SenderCardNumberError);
                    canContinue = false;
                    return canContinue;
                }
                else
                {
                    if (tempTissue.LawyerCredentType == eCredentialsType.IdentifyCard && tempTissue.LawyerCartNumber.Length != 15
                        && tempTissue.LawyerCartNumber.Length != 18 && !ToolICN.Check(tempTissue.LawyerCartNumber))
                    {
                        ShowBox(isAdd, SenderInfo.SenderIdentityCardNumberError);
                        canContinue = false;
                        return canContinue;
                    }
                }
            }
            if (tempTissue.SurveyDate != null && tempTissue.CheckDate != null && tempTissue.SurveyDate > tempTissue.CheckDate)
            {
                ShowBox(isAdd, SenderInfo.SenderDateError);
                canContinue = false;
                return canContinue;
            }
            return canContinue;
        }

        /// <summary>
        /// 消息显示框
        /// </summary>   
        private void ShowBox(bool isAdd, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            var showDlg = new TabMessageBoxDialog()
            {
                Header = isAdd ? SenderInfo.SenderAdd : SenderInfo.SenderEdit,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            };
            ThePage.Page.ShowMessageBox(showDlg);
        }

        #endregion

        private void MetroButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ZoneSelectorPanel zoneSelectorPanel = new ZoneSelectorPanel();
            zoneSelectorPanel.SelectorZone = new ZoneDataItem() { FullCode = CurrentZone.FullCode };
            zoneSelectorPanel.Workpage = this.ThePage;
            ThePage.Page.ShowDialog(zoneSelectorPanel, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                var selectzone = zoneSelectorPanel.RootZone.ConvertTo<Zone>();
                senderzonetxt.Text = selectZoneCode = selectzone.FullCode;
            });
        }
    }
}
