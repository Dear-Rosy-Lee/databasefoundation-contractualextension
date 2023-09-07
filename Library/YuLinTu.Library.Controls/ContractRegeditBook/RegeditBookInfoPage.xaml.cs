/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 权证信息界面
    /// </summary>
    public partial class RegeditBookInfoPage : InfoPageBase
    {
        #region Fields

        /// <summary>
        /// 发包方列表
        /// </summary>
        private List<CollectivityTissue> tissueList;
        private bool isAdd; //是否添加
        private ContractRegeditBookSettingDefine otherDefine = ContractRegeditBookSettingDefine.GetIntence();

        #endregion

        #region Properties

        /// <summary>
        /// 已签证权证
        /// </summary>
        public ObservableCollection<ContractRegeditBookItem> Items { get; set; }

        public List<ContractRegeditBook> ContractRegeditBookAll { get; set; }

        public ContractRegeditBookSettingDefine ExtendUseExcelDefine { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 权证数据访问层
        /// </summary>
        public IContractRegeditBookWorkStation BookStation
        {
            get;
            set;
        }

        /// <summary>
        /// 数据业务
        /// </summary>
        public IVirtualPersonWorkStation<LandVirtualPerson> personStation { get; set; }

        /// <summary>
        /// 数据字典处理业务
        /// </summary>
        public DictionaryBusiness DictBusiness { get; set; }

        /// <summary>
        /// 土地业务
        /// </summary>
        public AccountLandBusiness AccountLandBusiness { get; set; }

        /// <summary>
        /// 合同业务类
        /// </summary>       
        public ConcordBusiness ConcordBusiness { get; set; }

        /// <summary>
        /// 权证业务类
        /// </summary>
        public ContractRegeditBookBusiness ContractRegeditBookBusiness { get; set; }

        /// <summary>
        /// 承包方列表
        /// </summary>
        public List<VirtualPerson> ContracterList { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext dbContext { get; set; }

        /// <summary>
        /// 承包合同
        /// </summary>
        public ContractConcord Concord { get; set; }

        /// <summary>
        /// 当前签订权证的承包方
        /// </summary>
        public VirtualPerson CurrentFamily { get; set; }

        /// <summary>
        ///当前权证
        /// </summary>
        public ContractRegeditBook CurrentRegeditBook { get; set; }

        /// <summary>
        /// 是否添加
        /// </summary>
        public bool IsAdd
        {
            get { return isAdd; }
            set
            {
                isAdd = value;
                if (isAdd)
                {
                    //chkUseAbbreviation.Visibility = Visibility.Visible;
                }
                else
                {
                    //chkUseAbbreviation.Visibility = Visibility.Collapsed;
                    if (CurrentVp != null && CurrentVp.Status == eVirtualPersonStatus.Lock)
                        btnAdd.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson CurrentVp { get; set; }

        #endregion

        #region Ctor

        public RegeditBookInfoPage()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Confirm += RegeditBookPage_Confirm;
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        protected override void OnInitializeGo()
        {
            ModuleMsgArgs arg = MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_GETDATA, CurrentZone.FullCode);
            TheBns.Current.Message.Send(this, arg);
            tissueList = arg.ReturnValue as List<CollectivityTissue>;
        }

        /// <summary>
        /// 初始化完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            InitiallPageControl();
            if (isAdd)
            {
                txtStartTime.Value = DateTime.Now;
                txtWriteTime.Value = DateTime.Now;
                txtYearNumber.Value = DateTime.Now.Year;
                txtBookTakeTime.Value = DateTime.Now;

                txtContractRegeditBookTime.Value = DateTime.Now;
                chkOldbookTakeBack.IsChecked = false;
                

                txtAwareUnit.Text = CreateAwareAbbreviationUnit();
                txtWriteUnit.Text = CreateWriteUnit();
                txtSerialNumber.Text = "";
                string number = CreateNewSerialNumber();
                if (!string.IsNullOrEmpty(number))
                    txtSerialNumber.Text = number.PadLeft(6, '0');
            }
            else
            {
                EditWarrantValue();
            }
        }

        /// <summary>
        /// 初始化页面控件值
        /// </summary>
        private void InitiallPageControl()
        {
            if (ContracterList != null)
            {
                ContracterList.Sort((a, b) => { return a.Name.CompareTo(b.Name); });//对承包首字母排序
                cbVirtualPerson.ItemsSource = ContracterList;
                cbVirtualPerson.DisplayMemberPath = "Name";
                cbVirtualPerson.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 编辑证书值
        /// </summary>
        private void EditWarrantValue()
        {
            if (Concord == null)
            {
                return;
            }
            if (Concord.ContracterId.HasValue)
            {

                VirtualPerson vp = personStation.Get(Concord.ContracterId.Value);
                if (vp == null) return;
                if (vp.Status == eVirtualPersonStatus.Lock)
                {
                    btnAdd.IsEnabled = false;
                    txtWriteTime.IsEnabled = false;
                    txtYearNumber.IsEnabled = false;
                    txtStartTime.IsEnabled = false;
                    cbVirtualPerson.SelectedValue = vp.Name;
                    cbVirtualPerson.IsEnabled = false;

                    chkOldbookTakeBack.IsEnabled = false;
                    txtBookTakeTime.IsEnabled = false;
                    txtBookTakePerson.IsEnabled = false;
                    txtBookComment.IsEnabled = false;
                    txtPrintBookNumber.IsEnabled = false;
                    //MessageBox.Show(string.Format("户：{0}已锁定，不能再进行修改", vp.Name), "承包权证");
                }
                else
                {
                    btnAdd.IsEnabled = true;
                }
            }
            else
            {
                btnAdd.IsEnabled = true;
            }
            ContractRegeditBook book = ContractRegeditBookBusiness.Get(Concord.ID);
            if (book != null)
            {
                txtYearNumber.Value = Convert.ToInt32(book.Year);
                txtAwareUnit.Text = book.SendOrganization;
                txtWriteUnit.Text = book.WriteOrganization;
                txtStartTime.Value = book.SendDate;
                txtWriteTime.Value = book.WriteDate;
                txtContractRegeditBookExcursus.Text = book.ContractRegeditBookExcursus;
                txtContractRegeditBookPerson.Text = book.ContractRegeditBookPerson;
                txtContractRegeditBookTime.Value = book.ContractRegeditBookTime;

                chkOldbookTakeBack.IsChecked = book.OldBookReTake;
                txtBookTakeTime.Value = book.RegeditBookGettedDate;
                txtBookTakePerson.Text = book.GetterName;
                txtBookComment.Text = book.Comment;               
                txtPrintBookNumber.Text = book.PrintBookNumber;

                string number = string.IsNullOrEmpty(book.SerialNumber) ? "" : book.SerialNumber;
                if (number.Trim().Equals(""))
                    txtSerialNumber.Text = "无";
                else if (number.Trim().Equals("/"))
                    txtSerialNumber.Text = "/";
                else
                    txtSerialNumber.Text = number.PadLeft(6, '0');
                txtContractRegeditBookExcursus.Text = book.ContractRegeditBookExcursus;
            }
            cbVirtualPerson.IsEnabled = false;
            cbVirtualPerson.Text = Concord.ContracterName;
        }

        #endregion

        #region Methods

        public bool InitalizeContractor(bool isAdd)
        {
            this.IsAdd = isAdd;
            List<VirtualPerson> vps = null;
            if (CurrentZone.Level == eZoneLevel.Group)
                vps = personStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
            else
                vps = personStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            List<VirtualPerson> familys = new List<VirtualPerson>();
            if (isAdd)
            {
                List<ContractConcord> concords = ConcordBusiness.GetCollection(CurrentZone.FullCode);
                List<ContractRegeditBook> regeditBookList = this.ContractRegeditBookBusiness.GetByZoneCode(CurrentZone.FullCode, eSearchOption.Fuzzy);
                foreach (ContractConcord concord in concords)
                {
                    if (string.IsNullOrEmpty(concord.ConcordNumber))
                    {
                        continue;
                    }
                    ContractRegeditBook book = null;
                    if (regeditBookList != null)
                        book = regeditBookList.Find(c => c.ID == concord.ID);
                    if (book != null && !string.IsNullOrEmpty(book.RegeditNumber))
                    {
                        continue;
                    }
                    VirtualPerson vp = vps.Find(fam => fam.ID == concord.ContracterId.Value);
                    if (vp != null && vp.Status != eVirtualPersonStatus.Lock)
                    {
                        familys.Add(vp);
                    }
                }
                regeditBookList.Clear();
                concords.Clear();
                if (familys != null && familys.Count == 0)
                {
                    vps.Clear();
                    familys.Clear();
                    return false;
                }
            }
            else
            {
                familys = vps as List<VirtualPerson>;
                vps = null;
            }
            this.ContracterList = familys as List<VirtualPerson>;
            return true;
        }

        #endregion

        #region Events

        /// <summary>
        /// 使用简称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void useSimpleName_Checked(object sender, RoutedEventArgs e)
        {
            txtAwareUnit.Text = (bool)chkUseAbbreviation.IsChecked ? CreateAwareAbbreviationUnit() : CreateAwareUnit();
        }
                
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string errorMsg = string.Empty;
            if (txtWriteTime.Value == null)
            {
                ShowBox("填证日期不能为空!");
                return;
            }
            if (txtStartTime.Value == null)
            {
                ShowBox("颁证日期不能为空!");
                return;
            }
            if (((DateTime)txtWriteTime.Value).Date > ((DateTime)txtStartTime.Value).Date)
            {
                ShowBox("证书颁证日期必须大于等于填证日期!");
                return;
            }
            if (txtYearNumber.Value == null)
            {
                ShowBox("权证年号不能为空！");
                return;
            }
            ConfirmAsync();
        }

        /// <summary>
        /// 页面提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegeditBookPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            InitalizeConcord(e);
        }

        /// <summary>
        /// 提示
        /// </summary>
        private void ShowBox(string msg)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = isAdd ? "添加权证信息" : "编辑权证信息",
                Message = msg,
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }


        #endregion

        #region Privates

        object lockObject = new object();
        /// <summary>
        /// 初始化合同相关数据
        /// </summary>
        /// <param name="family"></param>
        /// <param name="concords"></param>
        /// <param name="landCollection"></param>
        /// <param name="index"></param>
        private void InitalizeConcord(MsgEventArgs<bool> e)
        {
            //lock (lockObject)
            //{
            //bool addRb = false;
            VirtualPerson vp = null;
            if (this.isAdd)
            {
                Dispatcher.Invoke(new Action(() =>
                       {
                           if (Concord == null)
                           {
                               vp = cbVirtualPerson.SelectedItem as VirtualPerson;
                           }
                       }));
                if (vp == null)
                {
                    return;
                }
                CurrentFamily = vp;
            }
            else
            {
                CurrentFamily = CurrentVp;
            }

            List<ContractConcord> concordCollection = ConcordBusiness.GetCollection(CurrentFamily.ID);
            if (concordCollection != null && concordCollection.Count == 1)
            {
                Concord = concordCollection[0];
            }
            else
            {
                foreach (ContractConcord concord in concordCollection)
                {
                    if (ContractRegeditBookBusiness.Get(concord.ID) != null)
                    {
                        continue;
                    }
                    Concord = concord;
                    break;
                }
            }
            bool addsuccuss = false;
            Dispatcher.Invoke(new Action(() =>
            {
                UpdateLandCertificate(Concord);
                addsuccuss = AddRegeditBook(Concord);//添加登记簿
            }));
            if (!addsuccuss)
                e.Parameter = false;
            //}
        }

        /// <summary>
        /// 更新合同
        /// </summary>
        private void UpdateLandCertificate(ContractConcord concord)
        {
            var serialNumber = txtSerialNumber.Text;
            if (CurrentRegeditBook != null)
            {
                CurrentRegeditBook.SerialNumber = serialNumber == null ? "" : serialNumber.TrimStart('0');
                BookStation.Update(CurrentRegeditBook);
            }
            concord.SenderDate = this.txtStartTime.Value;
            concord.CheckAgencyDate = this.txtWriteTime.Value;
            ConcordBusiness.Update(concord);
        }

        /// <summary>
        /// 添加登记簿
        /// </summary>
        /// <param name="concord">合同对象</param>
        private bool AddRegeditBook(ContractConcord concord)
        {
            ContractRegeditBook regBook = ContractRegeditBookBusiness.Get(concord.ID);
            if (regBook != null)
            {
                regBook.Number = concord.ConcordNumber;
                //regBook.SerialNumber = InitalizeBookSerialNumber(regBook); //流水号
                if (!string.IsNullOrEmpty(regBook.SerialNumber))
                {
                    int number = int.Parse(regBook.SerialNumber);
                    if (number < otherDefine.MinNumber || number > otherDefine.MaxNumber)
                    {
                        ShowBox("流水号超出配置区间");
                        return false;
                    }
                }

                regBook.RegeditNumber = concord.ConcordNumber;
                regBook.Year = this.txtYearNumber.Value.ToString();
                regBook.WriteDate = (DateTime)txtWriteTime.Value;//填证日期
                regBook.SendDate = (DateTime)txtStartTime.Value;//颁证日期
                regBook.SendOrganization = txtAwareUnit.Text;//颁证机关
                regBook.WriteOrganization = txtWriteUnit.Text;//填证单位

                regBook.ContractRegeditBookExcursus = txtContractRegeditBookExcursus.Text;
                regBook.ContractRegeditBookPerson = txtContractRegeditBookPerson.Text;
                regBook.ContractRegeditBookTime = txtContractRegeditBookTime.Value;

                regBook.OldBookReTake= chkOldbookTakeBack.IsChecked??false;
                regBook.RegeditBookGettedDate = (DateTime)txtBookTakeTime.Value;
                regBook.GetterName = txtBookTakePerson.Text;
                regBook.Comment = txtBookComment.Text;
                regBook.PrintBookNumber = txtPrintBookNumber.Text;
                CurrentRegeditBook = regBook;
                ContractRegeditBookBusiness.Update(regBook);
                return true;
            }
            ContractRegeditBook book = new ContractRegeditBook();
            book.ID = concord.ID;
            book.Count = 0;
            book.Founder = "Admin";
            book.Modifier = "Admin";
            book.SerialNumber = string.IsNullOrEmpty(txtSerialNumber.Text) ? "" : txtSerialNumber.Text.TrimStart('0');
            book.Number = concord.ConcordNumber;
            if (!string.IsNullOrEmpty(book.SerialNumber))
            {
                int number = int.Parse(book.SerialNumber);
                if (number < otherDefine.MinNumber || number > otherDefine.MaxNumber)
                {
                    ShowBox("流水号超出配置区间");
                    return false;
                }

            }

            book.RegeditNumber = concord.ConcordNumber;
            book.Year = this.txtYearNumber.Value.ToString();
            book.WriteDate = (DateTime)txtWriteTime.Value;//填证日期
            book.SendDate = (DateTime)txtStartTime.Value;//颁证日期
            book.SendOrganization = txtAwareUnit.Text;//颁证机关
            book.WriteOrganization = txtWriteUnit.Text;//填证单位
            book.ContractRegeditBookExcursus = txtContractRegeditBookExcursus.Text;
            book.ContractRegeditBookPerson = txtContractRegeditBookPerson.Text;
            book.ContractRegeditBookTime = txtContractRegeditBookTime.Value;

            book.OldBookReTake = chkOldbookTakeBack.IsChecked ?? false;
            book.RegeditBookGettedDate = (DateTime)txtBookTakeTime.Value;
            book.GetterName = txtBookTakePerson.Text;
            book.Comment = txtBookComment.Text;
            book.PrintBookNumber = txtPrintBookNumber.Text;

            book.ZoneCode = CurrentZone.FullCode;
            CurrentRegeditBook = book;
            ContractRegeditBookBusiness.AddRegeditBook(book);
            return true;
        }

        /// <summary>
        /// 生成流水号
        /// </summary>
        private string CreateNewSerialNumber()
        {
            //var maxSerialNum = dbContext.CreateRegeditBookStation().Get().Max(o => Convert.ToInt32(o.SerialNumber)) + 1;
            var maxSerialNum = BookStation.GetMaxSerialNumber();
            maxSerialNum++;
            if (maxSerialNum >= otherDefine.MinNumber && maxSerialNum <= otherDefine.MaxNumber)
                return maxSerialNum.ToString();
            return string.Empty;
        }

        /// <summary>
        /// 创建颁证单位
        /// </summary>
        /// <returns></returns>
        private string CreateAwareUnit()
        {
            string zoneCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            Zone county = GetZoneByCode(zoneCode);
            if (county != null)
            {
                return county.Name + "人民政府";
            }
            return "";
        }

        /// <summary>
        /// 创建颁证单位
        /// </summary>
        /// <returns></returns>
        private string CreateAwareAbbreviationUnit()
        {
            string zoneCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            Zone county = GetZoneByCode(zoneCode);
            if (county != null)
            {
                return county.Name.Substring(0, 1) + "府";
            }
            return "";
        }

        /// <summary>
        /// 创建填证单位
        /// </summary>
        /// <returns></returns>
        private string CreateWriteUnit()
        {
            string zoneCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            Zone county = GetZoneByCode(zoneCode);
            if (county != null)
            {
                return county.Name + "农业局";
            }
            return "";
        }

        /// <summary>
        /// 获取地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private Zone GetZoneByCode(string zoneCode)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zoneCode;
            arg.Name = ZoneMessage.ZONE_GET;
            TheBns.Current.Message.Send(this, arg);
            Zone county = arg.ReturnValue as Zone;
            return county;
        }

        #endregion
    }
}
