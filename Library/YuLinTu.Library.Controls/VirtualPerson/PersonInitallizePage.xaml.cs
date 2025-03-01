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
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 初始化承包方数据
    /// </summary>
    public partial class PersonInitallizePage : InfoPageBase
    {
        #region Fields

        //private int index = 0;//序号  

        /// <summary>
        /// 民族
        /// </summary>
        private List<EnumStore<eNation>> nationList;
        /// <summary>
        /// 土地承包方式
        /// </summary>
        private KeyValueList<string, string> constructModeList;

        private string address = "";
        private bool initiallNumber = false;//初始化户编号

        private bool initiallZip = false;//初始化邮编
        private string zipCode = "";

        private bool initiallNation = false;//初始化民族
        private eNation cNation;
        private int chkNum = 0;
        /// <summary>
        /// 当前承包方式
        /// </summary>
        private eConstructMode eMode;
        private string cMode;

        /// <summary>
        /// 家庭成员备注
        /// </summary>
        private string personComment = string.Empty;

        private VirtualPersonExpand expand = null;

        #endregion

        #region Propertys

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                txtAddress.Text = address;
            }
        }

        /// <summary>
        /// 初始化户编号
        /// </summary>
        public bool InitiallNumber
        {
            get { return initiallNumber; }
            set { initiallNumber = value; }
        }

        /// <summary>
        /// 初始化邮编
        /// </summary>
        public bool InitiallZip
        {
            get { return initiallZip; }
            set { initiallZip = value; }
        }

        /// <summary>
        /// 初始化民族
        /// </summary>
        public bool InitiallNation
        {
            get { return initiallNation; }
            set { initiallNation = value; }
        }

        #region 初始化调查信息

        /// <summary>
        /// 初始化承包方地址
        /// </summary>
        public bool InitiallVpAddress { get; set; }

        /// <summary>
        /// 初始化调查员
        /// </summary>
        public bool InitiallSurveyPerson { get; set; }

        /// <summary>
        /// 初始化调查日期
        /// </summary>
        public bool InitiallSurveyDate { get; set; }

        /// <summary>
        /// 初始化调查记事
        /// </summary>
        public bool InitiallSurveyAccount { get; set; }

        /// <summary>
        /// 初始化审核人
        /// </summary>
        public bool InitiallCheckPerson { get; set; }

        /// <summary>
        /// 初始化审核日期
        /// </summary>
        public bool InitiallCheckDate { get; set; }

        /// <summary>
        /// 初始化审核意见
        /// </summary>
        public bool InitiallCheckOpinion { get; set; }

        /// <summary>
        /// 初始化公示记事人
        /// </summary>
        public bool InitiallPublishAccountPerson { get; set; }

        /// <summary>
        /// 初始化公示日期
        /// </summary>
        public bool InitiallPublishDate { get; set; }

        /// <summary>
        /// 初始化公示审核人
        /// </summary>
        public bool InitiallPublishCheckPerson { get; set; }

        /// <summary>
        /// 初始化公示记事
        /// </summary>
        public bool InitiallcbPublishAccount { get; set; }
        /// <summary>
        /// 只初始化空项
        /// </summary>
        public bool InitialNull { get; set; }
        /// <summary>
        /// 承包方式
        /// </summary>
        public bool InitialContractWay { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        public bool InitConcordNumber { get; set; }
        /// <summary>
        /// 权证编码
        /// </summary>
        public bool InitWarrentNumber { get; set; }
        /// <summary>
        /// 承包开始时间
        /// </summary>
        public bool InitStartTime { get; set; }
        /// <summary>
        /// 承包结束时间
        /// </summary>
        public bool InitEndTime { get; set; }

        /// <summary>
        /// 家庭成员备注
        /// </summary>
        public bool InitPersonComment { get; set; }

        /// <summary>
        /// 初始化共有人备注
        /// </summary>
        public bool InitSharePersonComment { get; set; }

        public bool InitPersonSex { get; set; }
        #endregion

        /// <summary>
        /// 承包方扩展信息
        /// </summary>
        public VirtualPersonExpand Expand
        {
            get { return expand; }
            set { expand = value; }
        }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        /// <summary>
        /// 民族
        /// </summary>
        public eNation CNation
        {
            get { return cNation; }
            set { cNation = value; }
        }

        /// <summary>
        /// 家庭成员备注
        /// </summary>
        public string PersonComment
        {
            get { return personComment; }
            set { personComment = value; }
        }

        public bool InstallerAllNumber { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="zone">地域</param>
        /// <param name="editvp">是否为编辑承包方</param>
        /// <param name="per">实体</param>
        public PersonInitallizePage()
        {
            InitializeComponent();
            InitiallControl();
            btnAdd.IsEnabled = false;
            eMode = eConstructMode.Family;
        }

        #endregion

        #region Private

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitiallControl()
        {
            try
            {
                var dbContext = DataBaseSource.GetDataBaseSource();
                var dictStation = dbContext.CreateDictWorkStation();
                constructModeList = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.CBJYQQDFS);
            }
            catch (Exception)
            {
                constructModeList = new KeyValueList<string, string>();
            }
            nationList = EnumStore<eNation>.GetListByType();
            cbNation.DisplayMemberPath = "DisplayName";
            cbNation.ItemsSource = nationList;
            cbNation.SelectedIndex = 0;

            mcbContractWay.DisplayMemberPath = "Value";
            mcbContractWay.ItemsSource = constructModeList;
            mcbContractWay.SelectedIndex = 0;
        }

        #endregion

        #region Events

        /// <summary>
        /// 提交数据
        /// </summary> 
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //initiallNumber = (bool)cbNumber.IsChecked;
            //initiallNation = (bool)chbNation.IsChecked;
            //initiallZip = (bool)cbZip.IsChecked;
            //initiallSurvey = (bool)cbSuevery.IsChecked;
            //if (!initiallNumber && !initiallNation && !initiallZip && !initiallSurvey)
            //{
            //    return;
            //}
            ConfirmAsync(goCallback =>
            {
                return SetAndCheckValue();
            }, completedCallback =>
             {
                 Close(true);
             });
            //Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 设置和检查值
        /// </summary>
        private bool SetAndCheckValue()
        {
            bool canContinue = true;

            Dispatcher.Invoke(new Action(() =>
            {
                initiallNumber = (bool)cbNumber.IsChecked;
                initiallNation = (bool)chbNation.IsChecked;
                initiallZip = (bool)cbZip.IsChecked;
                InitiallVpAddress = (bool)cbVpAddress.IsChecked;
                InitiallSurveyPerson = (bool)cbSurveyPerson.IsChecked;
                InitiallSurveyDate = (bool)cbSurveyDate.IsChecked;
                InitiallSurveyAccount = (bool)cbSurveyAccount.IsChecked;
                InitiallCheckPerson = (bool)cbCheckPerson.IsChecked;
                InitiallCheckDate = (bool)cbCheckDate.IsChecked;
                InitiallCheckOpinion = (bool)cbCheckOpinion.IsChecked;
                InitiallPublishAccountPerson = (bool)cbPublishAccountPerson.IsChecked;
                InitiallPublishDate = (bool)cbPublishDate.IsChecked;
                InitiallPublishCheckPerson = (bool)cbPublishCheckPerson.IsChecked;
                InitiallcbPublishAccount = (bool)cbPublishAccount.IsChecked;
                InitialNull = (bool)cbInitializeNull.IsChecked;
                InitialContractWay = (bool)cbContractWay.IsChecked;
                InitConcordNumber = (bool)cbConcordNumber.IsChecked;
                InitWarrentNumber = (bool)cbWarrantNumber.IsChecked;
                InitStartTime = (bool)cbStartTime.IsChecked;
                InitEndTime = (bool)cbEndTime.IsChecked;
                InitPersonComment = (bool)cbPersonComment.IsChecked;
                InitSharePersonComment = (bool)cbSharePersonComment.IsChecked;
                InstallerAllNumber = (bool)cbNumberAll.IsChecked;
            }));

            if (!InitialContractWay && !InitConcordNumber && !InitWarrentNumber && !initiallNumber && !initiallNation && !initiallZip && !InitiallVpAddress && !InitiallSurveyPerson
                && !InitStartTime && !InitEndTime && !InitiallSurveyDate && !InitiallSurveyAccount && !InitiallCheckPerson && !InitiallCheckDate
                && !InitiallCheckOpinion && !InitiallPublishAccountPerson && !InitiallPublishDate && !InitiallPublishCheckPerson && !InitiallcbPublishAccount &&
                !InitPersonComment && !InitSharePersonComment && !InitPersonSex)
            {
                return false;
            }

            Dispatcher.Invoke(new Action(() =>
            {
                expand = new VirtualPersonExpand();
                expand.SurveyPerson = txtSurvey.Text.Trim();
                expand.SurveyDate = txtSurveyDate.Value;
                expand.SurveyChronicle = txtSurveyInfo.Text.Trim();
                expand.PublicityCheckPerson = txtPubPerson.Text.Trim();
                expand.PublicityChronicle = txtPubInfo.Text.Trim();
                expand.PublicityChroniclePerson = txtPub.Text.Trim();
                expand.PublicityDate = txtPubDate.Value;
                expand.CheckDate = txtCheckDate.Value;
                expand.CheckPerson = txtCheck.Text.Trim();
                expand.CheckOpinion = txtCheckInfo.Text.Trim();
                expand.ConstructMode = eMode;
                expand.ConcordStartTime = txtStartTime.Value;
                expand.ConcordEndTime = txtEndTime.Value;
                address = txtAddress.Text.Trim();
                zipCode = txtZip.Text.Trim();
                personComment = txtPersonComment.Text.Trim();
            }));

            if (initiallZip && (zipCode.IsNullOrBlank() || (!string.IsNullOrEmpty(zipCode) && zipCode.Length != 6)))
            {
                ShowBox(VirtualPersonInfo.PersonInitiall, "邮政编码不符合要求!");
                return false;
            }

            if (expand.SurveyDate > expand.CheckDate)
            {
                ShowBox(VirtualPersonInfo.PersonInitiall, "调查日期不能大于审核日期!");
                return false;
            }
            if (expand.CheckDate > expand.PublicityDate)
            {
                ShowBox(VirtualPersonInfo.PersonInitiall, "审核日期不能大于公示日期!");
                return false;
            }
            if (expand.ConcordStartTime >= expand.ConcordEndTime)
            {
                ShowBox(VirtualPersonInfo.PersonInitiall, "承包起始日期不能大于承包结束日期");
                return false;
            }
            return canContinue;
        }

        /// <summary>
        /// 消息提示
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = header,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                });
            }));
        }

        /// <summary>
        /// 民族改变
        /// </summary>
        private void cbNation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = cbNation.SelectedItem;
            if (selectItem == null)
            {
                return;
            }
            EnumStore<eNation> esValue = selectItem as EnumStore<eNation>;
            if (esValue == null)
            {
                return;
            }
            cNation = esValue.Value;
        }

        /// <summary>
        /// 邮政编码输入
        /// </summary>
        private void mtbCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        /// <summary>
        /// 邮政编码输入
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

        #endregion

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

        #region 设置是否可提交
        private void cbCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ck = sender as CheckBox;
            if (ck == null)
                return;
            if ((bool)ck.IsChecked)
                chkNum++;
            else
                chkNum--;
            if (chkNum > 0)
                btnAdd.IsEnabled = true;
            else
                btnAdd.IsEnabled = false;

        }
        #endregion




        #endregion

        private void mcbContractWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = mcbContractWay.SelectedItem;
            if (selectItem == null)
            {
                return;
            }
            var esValue = selectItem as KeyValue<string, string>;
            if (esValue == null)
            {
                return;
            }
            cMode = esValue.Key;
            Enum.TryParse(cMode, out eMode);
        }

        private void cbSex_Click(object sender, RoutedEventArgs e)
        {
            InitPersonSex = cbSex.IsChecked == null ? false : cbSex.IsChecked.Value;
            if ((bool)cbSex.IsChecked)
                chkNum++;
            else
                chkNum--;
            if (chkNum > 0)
                btnAdd.IsEnabled = true;
            else
                btnAdd.IsEnabled = false;
        }
    }
}
