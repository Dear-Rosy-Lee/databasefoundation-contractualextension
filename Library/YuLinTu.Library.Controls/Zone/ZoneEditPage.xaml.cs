/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 地域编辑页面
    /// </summary>
    public partial class ZoneEditPage : InfoPageBase
    {
        #region Fields

        /// <summary>
        /// 当前选择地域
        /// </summary>
        private Zone currentZone;

        /// <summary>
        /// 临时地域
        /// </summary>
        private Zone tempZone;

        /// <summary>
        /// 是否添加
        /// </summary>
        private bool isAdd;

        /// <summary>
        /// 序号
        /// </summary>
        private int index;

        private int upindex;

        /// <summary>
        /// 业务处理
        /// </summary>
        private ZoneDataBusiness business;

        #endregion Fields

        #region Propertys

        /// <summary>
        /// 数据库
        /// </summary>
        public IZoneWorkStation Station { get; set; }

        /// <summary>
        /// 编辑地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                if (currentZone == null)
                {
                    currentZone = new Zone();
                }
                if (currentZone.ID == Guid.Empty)
                {
                    currentZone.ID = Guid.NewGuid();
                }
                tempZone = value.Clone() as Zone;
                SetByLevel(currentZone.Level);
            }
        }

        /// <summary>
        /// 数据项
        /// </summary>
        public ZoneDataItem CurrentItem { get; private set; }

        public ZoneDefine ZoneDefine { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result { get; private set; }

        #endregion Propertys

        #region ctor

        public ZoneEditPage(bool isAdd = false)
        {
            InitializeComponent();
            this.DataContext = this;
            Confirm += ZoneEditPage_Confirm;
            spInfo.Visibility = Visibility.Hidden;
            loadIcon.Visibility = Visibility.Visible;
            business = new ZoneDataBusiness();

            this.isAdd = isAdd;
            if (isAdd)
            {
                btnSubmit.IsEnabled = true;
                cb_recode.Visibility = Visibility.Visible;
            }
            else
            {
                btnSubmit.IsEnabled = false;
                cb_recode.Visibility = Visibility.Collapsed;
                cb_recode.IsChecked = false;
            }
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoneEditPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            business.Station = Station;
            try
            {
                if (business.IsZoneCodeExist(currentZone))
                {
                    Dispatcher.Invoke(new Action(() => Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                    {
                        Header = isAdd ? ZoneInfo.ZoneAdd : ZoneInfo.ZoneEdit,
                        Message = ZoneInfo.ZoneCodeExist,
                        MessageGrade = eMessageGrade.Error,
                        CancelButtonText = "取消",
                    })));
                    e.Parameter = false;
                    return;
                }
                if (currentZone.FullCode.IsNullOrBlank())
                {
                    Dispatcher.Invoke(new Action(() => Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                    {
                        Header = isAdd ? ZoneInfo.ZoneAdd : ZoneInfo.ZoneEdit,
                        Message = ZoneInfo.ZoneCodeNull,
                        MessageGrade = eMessageGrade.Error,
                        CancelButtonText = "取消",
                    })));
                    e.Parameter = false;
                    return;
                }
                if (isAdd)
                {
                    Result = business.Add(currentZone);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (cb_recode.IsChecked == true)
                            currentZone.CreateUser = "CreateZoneAndSender";
                    }));
                }
                else if (CurrentZone is ZoneDataItem)
                {
                    EditData((ZoneDataItem)CurrentZone);
                }
                e.Parameter = true;
            }
            catch (Exception ex)
            {
                e.Parameter = false;
                YuLinTu.Library.Log.Log.WriteException(this, "ZoneEditPage_Confirm(更新/添加发包方数据)", ex.Message + ex.StackTrace);
                throw new Exception("更新数据出错" + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion ctor

        #region Methods

        #region Override

        protected override void OnInitializeGo()
        {
        }

        protected override void OnInitializeCompleted()
        {
            spInfo.Visibility = Visibility.Visible;
            loadIcon.Visibility = Visibility.Collapsed;
        }

        #endregion Override

        #region Private

        /// <summary>
        /// 设置编码长度
        /// </summary>
        /// <param name="level"></param>
        private void SetByLevel(eZoneLevel level)
        {
            mtbCode.ToolTip = "2位编码";
            List<string> list = new List<string>();
            switch (level)
            {
                case eZoneLevel.Group:
                    list.Add(LanguageAttribute.GetLanguage("key41411"));
                    mtbCode.MaxLength = 2;
                    break;

                case eZoneLevel.Village:
                    list.Add(LanguageAttribute.GetLanguage("key41412"));
                    mtbCode.MaxLength = 3;
                    mtbCode.ToolTip = "3位编码";
                    break;

                case eZoneLevel.Town:
                    list.Add(LanguageAttribute.GetLanguage("key41413"));
                    mtbCode.MaxLength = 3;
                    mtbCode.ToolTip = "3位编码";
                    break;

                case eZoneLevel.County:
                    list.Add(LanguageAttribute.GetLanguage("key41414"));
                    mtbCode.MaxLength = 2;
                    break;

                case eZoneLevel.City:
                    list.Add(LanguageAttribute.GetLanguage("key41415"));
                    mtbCode.MaxLength = 2;
                    break;

                case eZoneLevel.Province:
                    list.Add(LanguageAttribute.GetLanguage("key41416"));
                    mtbCode.MaxLength = 2;
                    break;

                case eZoneLevel.State:
                    list.Add(LanguageAttribute.GetLanguage("key41417"));
                    mtbCode.MaxLength = 2;
                    break;

                default:
                    list.Add(LanguageAttribute.GetLanguage("key41411"));
                    mtbCode.MaxLength = 4;
                    mtbCode.ToolTip = "4位编码";
                    break;
            }
            cbLevel.ItemsSource = list;
            cbLevel.SelectedIndex = 0;
        }

        #endregion Private

        #region Events

        /// <summary>
        /// 提交按钮
        /// </summary>
        private void MetroButton_Click_1(object sender, RoutedEventArgs e)
        {
            bool isNameExist = Station.ExistName(currentZone.UpLevelCode, currentZone);
            if (isNameExist)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    var repeatDlg = new TabMessageBoxDialog()
                    {
                        Header = isAdd ? ZoneInfo.ZoneAdd : ZoneInfo.ZoneEdit,
                        Message = isAdd ? ZoneInfo.ZoneNameExist : ZoneInfo.ZoneNameExistEdit,
                        MessageGrade = eMessageGrade.Warn,
                        CancelButtonText = "取消",
                    };
                    Workpage.Page.ShowMessageBox(repeatDlg, (b, r) =>
                    {
                        if (!(bool)b)
                            return;
                        ConfirmAsync();
                    });
                }));
            }
            else
            {
                ConfirmAsync();
            }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        private void EditData(ZoneDataItem itemZone)
        {
            SetState();
            Zone curZone = itemZone.ConvertTo<Zone>();
            currentZone = curZone;
            //if (itemZone.Children == null || itemZone.Children.Count == 0)
            //{
            //    Result = business.UpdateZone(curZone);
            //    SetState(true);
            //}
            //else
            {
                UpdateZoneEntryInformation(itemZone);
            }
            CurrentItem = itemZone;
            SetState(true);
            SetLableInvoke("");
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        private void SetState(bool state = false)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                this.CanClose = state;
                gridContent.IsEnabled = state;
                Result = state;
                btnSubmit.IsEnabled = state;
                btnCancel.IsEnabled = state;
            }));
        }

        /// <summary>
        /// 更新地域信息
        /// </summary>
        private void UpdateZoneEntryInformation(ZoneDataItem item)
        {
            index = 0;
            int itemCount = CountItemChildren(item);
            Zone sourceZone = Station.Get(item.ID);
            Zone curZone = item.ConvertTo<Zone>();
            bool alterCode = sourceZone.Code != curZone.Code;
            bool alterName = sourceZone.Name != curZone.Name;
            string sourceCode = string.Empty;
            foreach (ZoneDataItem zoneItem in item.Children)
            {
                index++;
                var oldzone = zoneItem.Clone();
                SetLableInvoke(string.Format("({0}/{1})", index, itemCount));
                sourceCode = zoneItem.FullCode;
                if (zoneItem.FullCode == curZone.FullCode && zoneItem.Name == curZone.Name)
                {
                    continue;
                }
                if (alterCode)
                {
                    zoneItem.FullCode = curZone.FullCode + zoneItem.FullCode.Substring(sourceZone.FullCode.Length);
                    zoneItem.UpLevelCode = zoneItem.FullCode.Substring(0, zoneItem.FullCode.Length - zoneItem.Code.Length);
                    zoneItem.UpLevelCode = zoneItem.Level == eZoneLevel.State ? "86" : zoneItem.UpLevelCode;
                }
                if (alterName && item.Level != eZoneLevel.Province)
                {
                    zoneItem.UpLevelName = item != null ? item.FullName : zoneItem.UpLevelName;
                    zoneItem.FullName = zoneItem.UpLevelName + zoneItem.Name;
                    zoneItem.UpLevelName = zoneItem.Level == eZoneLevel.Province ? "中国" : zoneItem.UpLevelName;
                }
                if (alterCode || alterName)
                {
                    Zone zone = zoneItem.ConvertTo<Zone>();
                    if (business.UpdateZoneCodeName(zone))
                    {
                        if (zoneItem.Children.Count == 0)
                        {
                            ModuleMsgArgs arg = MessageExtend.ZoneMsg(business.DbContext, ZoneMessage.ZONE_UPDATE_COMPLETE,
                                new MultiObjectArg() { ParameterA = oldzone, ParameterB = zoneItem });
                            Workpage.Workspace.Message.Send(this, arg);
                            TheApp.Current.Message.Send(Workpage, arg);
                        }
                    }
                    else
                    {
                        Result = false;
                    }
                }
                if (zoneItem.Level < eZoneLevel.Province)//如果当前选择的是国家，只需修改省级地域上级编码
                {
                    UpdateChildren(zoneItem, alterCode, alterName, itemCount);
                }
            }
            if (business.UpdateZoneCodeName(curZone))
            {
                ModuleMsgArgs arg = MessageExtend.ZoneMsg(business.DbContext, ZoneMessage.ZONE_UPDATE_COMPLETE,
                    new MultiObjectArg() { ParameterA = item, ParameterB = sourceZone.ConvertTo<ZoneDataItem>() });
                Workpage.Workspace.Message.Send(this, arg);
                TheApp.Current.Message.Send(Workpage, arg);
            }
            else
            {
                Result = false;
            }
            SetLableInvoke(string.Format("({0}/{1})", index, itemCount));
        }

        /// <summary>
        /// 异步设置label信息
        /// </summary>
        private void SetLableInvoke(string info)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                upindex++;
                lbTip.Text = $"正在更新数据，请等待" + (upindex % 2 == 0 ? ".   " : "... ") + info;
            }));
        }

        /// <summary>
        /// 统计数据项下数据量
        /// </summary>
        private int CountItemChildren(ZoneDataItem item)
        {
            int count = 0;
            ZoneDataItem pItem = item;
            if (pItem.Level == eZoneLevel.State)
            {
                return pItem.Children.Count;
            }
            if (pItem.Children.Count > 0)
            {
                foreach (ZoneDataItem di in item.Children)
                {
                    count += CountItemChildren(di);
                }
            }
            count += 1;
            return count;
        }

        /// <summary>
        /// 更新子地域信息
        /// </summary>
        private void UpdateChildren(ZoneDataItem item, bool alterCode, bool alterName, int itemCount)
        {
            if (item.Children == null || item.Children.Count == 0)
            {
                return;
            }
            ZoneDataItem parentItem = item;
            string sourceCode = string.Empty;
            foreach (ZoneDataItem di in parentItem.Children)
            {
                index++;
                var oldezone = di.Clone();
                SetLableInvoke(string.Format("({0}/{1})", index, itemCount));
                sourceCode = di.FullCode;
                if (alterCode)
                {
                    di.FullCode = item.FullCode + di.FullCode.Substring(item.FullCode.Length);
                    di.UpLevelCode = di.FullCode.Substring(0, di.FullCode.Length - di.Code.Length);
                    di.UpLevelCode = di.Level == eZoneLevel.State ? "86" : di.UpLevelCode;
                }
                if (alterName)
                {
                    di.UpLevelName = parentItem != null ? parentItem.FullName : di.UpLevelName;
                    di.FullName = di.UpLevelName + di.Name;
                    di.UpLevelName = di.Level == eZoneLevel.Province ? "中国" : di.UpLevelName;
                }
                if (alterCode || alterName)
                {
                    Zone zone = di.ConvertTo<Zone>();
                    if (business.UpdateZone(zone))
                    {
                        if (di.Children.Count == 0)
                        {
                            ModuleMsgArgs arg = MessageExtend.ZoneMsg(business.DbContext, ZoneMessage.ZONE_UPDATE_COMPLETE,
                            new MultiObjectArg() { ParameterA = di, ParameterB = oldezone });
                            Workpage.Workspace.Message.Send(this, arg);
                            TheApp.Current.Message.Send(Workpage, arg);
                        }
                    }
                }
                UpdateChildren(di, alterCode, alterName, itemCount);
            }
        }

        /// <summary>
        /// 地域名称变化
        /// </summary>
        private void MetroTextBox_NameChanged(object sender, TextChangedEventArgs e)
        {
            string zoneName = mtbName.Text.Trim();
            currentZone.Name = zoneName;
            currentZone.FullName = (currentZone.Level == eZoneLevel.Province) ? zoneName : currentZone.UpLevelName + currentZone.Name;
            if (!isAdd)
            {
                btnSubmit.IsEnabled = EditChanged();
            }
            else
            {
                btnSubmit.IsEnabled = CanSubmit();
            }
        }

        /// <summary>
        /// 地域编码变化
        /// </summary>
        private void MetroTextBox_CodeChanged(object sender, TextChangedEventArgs e)
        {
            currentZone.Code = mtbCode.Text.Trim();
            currentZone.FullCode = (currentZone.Level == eZoneLevel.Province) ? currentZone.Code : currentZone.UpLevelCode + currentZone.Code;
            if (!isAdd)
            {
                btnSubmit.IsEnabled = EditChanged();
            }
            else
            {
                btnSubmit.IsEnabled = CanSubmit();
            }
        }


        private void MetroTextBox_UpCodeChanged(object sender, TextChangedEventArgs e)
        {
            currentZone.UpLevelCode = mtbUpCode.Text.Trim();
            currentZone.FullCode = (currentZone.Level == eZoneLevel.Province) ? currentZone.Code : currentZone.UpLevelCode + currentZone.Code;
            if (!isAdd)
            {
                btnSubmit.IsEnabled = EditChanged();
            }
            else
            {
                btnSubmit.IsEnabled = CanSubmit();
            }
        }

        /// <summary>
        /// 备注变化
        /// </summary>
        private void MetroTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentZone.Comment = mtbComment.Text.Trim();
            if (!isAdd)
            {
                btnSubmit.IsEnabled = EditChanged();
            }
            else
            {
                btnSubmit.IsEnabled = CanSubmit();
            }
        }

        /// <summary>
        /// 是否变更数据
        /// </summary>
        private bool EditChanged()
        {
            bool change = false;
            if (tempZone.Name != currentZone.Name)
            {
                change = true;
            }
            if (tempZone.UpLevelCode != currentZone.UpLevelCode)
            {
                change = true;
            }
            if (tempZone.Code != currentZone.Code)
            {
                change = true;
            }
            if (tempZone.Comment != currentZone.Comment)
            {
                change = true;
            }
            bool canSubmit = CanSubmit();
            return change && canSubmit;
        }

        /// <summary>
        /// 是否变更数据
        /// </summary>
        private bool CanSubmit()
        {
            bool change = true;
            int zeroNum = 0;
            if (!string.IsNullOrEmpty(currentZone.Code))
            {
                zeroNum = (currentZone.Code.Split('0').Length - 1);
            }
            if (string.IsNullOrEmpty(currentZone.Name) || (currentZone.Code != null && (currentZone.Code.Length != mtbCode.MaxLength ||
                currentZone.Code.Length == zeroNum)))
            {
                change = false;
            }
            return change;
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

        private void mtbUpCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
        private void mtbCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void mtbUpCode_PreviewKeyDown(object sender, KeyEventArgs e)
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

        #endregion Events

        #endregion Methods
    }
}