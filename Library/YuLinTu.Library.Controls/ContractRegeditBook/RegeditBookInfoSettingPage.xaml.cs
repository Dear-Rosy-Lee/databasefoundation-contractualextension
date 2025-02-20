/*
 * (C) 2025 - 2017 鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// RegeditBookInfoSettingPage.xaml 的交互逻辑-权证登记后台
    /// </summary>
    public partial class RegeditBookInfoSettingPage : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public RegeditBookInfoSettingPage(bool isBatch = false, IDbContext db = null)
        {
            InitializeComponent();
            this.Header = isBatch ? ContractRegeditBookInfo.InitialWarrantInfoVolumn : ContractRegeditBookInfo.InitialWarrantInfo;
            this.DbContext = db;

            this.DataContext = this;  //绑定数据源(任意属性)
            LandsOfInitialConcord = new List<ContractLand>();

            WarrantsModified = new List<ContractRegeditBook>();
            ListConcord = new List<ContractConcord>();

            ListLand = new List<ContractLand>();
            Confirm += ContractRegeditBookSetPage_Confirm;
        }

        #endregion

        #region Fields

        ///// <summary>
        ///// 承包方类型
        ///// </summary>
        //private List<Dictionary> contracterTypeList;

        ///// <summary>
        ///// 地块类别
        ///// </summary>
        //private List<Dictionary> dklbList;

        ///// <summary>
        ///// 土地用途
        ///// </summary>
        //private List<Dictionary> tdytList;

        ///// <summary>
        ///// 承包方式
        ///// </summary>
        //private List<Dictionary> cbfsList;

        ///// <summary>
        ///// 发包方列表
        ///// </summary>
        //private List<CollectivityTissue> tissueList;

        /// <summary>
        /// 只签承包方
        /// </summary>
        private bool boolFamilyContract = true;

        /// <summary>
        /// 当前选择的承包方模式
        /// </summary>
        private string FamilyContractMode = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 承包方数据业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

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
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 待签订权证的承包方集合-排除了被锁定的
        /// </summary>
        public List<VirtualPerson> SelectPersonCollection { get; set; }

        /// <summary>
        /// 承包合同集合-排除了被锁定的
        /// </summary>
        public List<ContractConcord> ListConcord { get; set; }

        /// <summary>
        /// 承包地集合
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        /// <summary>
        /// 初始化合同信息的地块集合
        /// </summary>
        public List<ContractLand> LandsOfInitialConcord { get; set; }

        /// <summary>
        /// 承包权证集合(初始化之前)
        /// </summary>
        public List<ContractRegeditBook> RegeditBooksBefore { get; set; }

        /// <summary>
        /// 承包权证集合(初始化之后)
        /// </summary>
        public List<ContractRegeditBook> WarrantsModified { get; set; }

        /// <summary>
        /// 所有的地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }
        //private int maxSerialIndex;
        #endregion

        #region Events

        /// <summary>
        /// 使用简称
        /// </summary>
        private void useSimpleName_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
                   {
                       txtAwareUnit.Text = (bool)chkUseAbbreviation.IsChecked ? CreateAwareAbbreviationUnit() : CreateAwareUnit();
                   }));
        }

        /// <summary>
        /// 只签家庭承包
        /// </summary>
        private void chkuseFamilyContract_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
                   {
                       boolFamilyContract = chkuseFamilyContract.IsChecked.Value ? true : false;
                       FamilyContractMode = chkuseFamilyContract.IsChecked.Value ? ((int)eConstructMode.Family).ToString() : "";
                   }));
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (txtYearNumber.Value == null)
            {
                ShowBox("权证年号不能为空！");
                return;
            }
            if (txtContractRegeditBookTime.Value == null)
            {
                ShowBox("登簿日期不能为空！");
                return;
            }
            if (txtWriteTime.Value == null)
            {
                ShowBox("填证日期不能为空！");
                return;
            }
            if (txtStartTime.Value == null)
            {
                ShowBox("颁证日期不能为空！");
                return;
            }
            if (txtWriteTime.Value != null && txtStartTime.Value != null && ((DateTime)txtWriteTime.Value).Date > ((DateTime)txtStartTime.Value).Date)
            {
                ShowBox(ContractRegeditBookInfo.WarrantDateError);
                return;
            }
            ConfirmAsync();
        }

        private void ContractRegeditBookSetPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            try
            {
                if (boolFamilyContract)
                {
                    foreach (var zoneitem in AllZones)
                    {
                        //当前地域下以前的权证和家庭承包合同
                        var zoneBooksBefore = RegeditBooksBefore.FindAll(r => r.ZoneCode == zoneitem.FullCode);
                        List<ContractConcord> familyConcords = ListConcord.FindAll(l => l.ArableLandType == ((int)eConstructMode.Family).ToString() && l.ZoneCode == zoneitem.FullCode);
                        if (familyConcords != null && familyConcords.Count > 0)
                        {
                            familyConcords.Sort((a, b) =>
                            {
                                long aNumber = 0;
                                long bNumber = 0;
                                long.TryParse(a.ConcordNumber.Replace("J", "").Substring(a.ConcordNumber.Length - 6, 5), out aNumber);
                                long.TryParse(b.ConcordNumber.Replace("J", "").Substring(b.ConcordNumber.Length - 6, 5), out bNumber);
                                return aNumber.CompareTo(bNumber);
                            });
                        }
                        foreach (var familyConcord in familyConcords)
                        {
                            var RegeditBooksPersonsBefore = zoneBooksBefore.Find(r => r.ID == familyConcord.ID);
                            ContractRegeditBook addFamilyContractWarrant = new ContractRegeditBook();//修改后的权证    
                            if (RegeditBooksPersonsBefore == null)
                            {
                                ContractRegeditBook tempFamilyContractWarrant = new ContractRegeditBook() { ZoneCode = zoneitem.FullCode };
                                addFamilyContractWarrant = SetWarrantValue(tempFamilyContractWarrant, familyConcord);
                                WarrantsModified.Add(addFamilyContractWarrant);
                            }
                            else
                            {
                                addFamilyContractWarrant = SetWarrantValue(RegeditBooksPersonsBefore, familyConcord);
                                WarrantsModified.Add(addFamilyContractWarrant);
                            }
                        }
                    }
                }
                if (!boolFamilyContract)
                {
                    foreach (var zoneitem in AllZones)
                    {
                        //当前地域下以前的权证和家庭承包合同
                        var zoneBooksBefore = RegeditBooksBefore.FindAll(r => r.ZoneCode == zoneitem.FullCode);
                        List<ContractConcord> zoneConcords = ListConcord.FindAll(l => l.ZoneCode == zoneitem.FullCode);
                        foreach (var concord in zoneConcords)
                        {
                            var RegeditBooksPersonsBefore = RegeditBooksBefore.Find(r => r.ID == concord.ID);
                            ContractRegeditBook addContractWarrant = new ContractRegeditBook();//修改后的权证    
                            if (RegeditBooksPersonsBefore == null)
                            {
                                ContractRegeditBook tempFamilyContractWarrant = new ContractRegeditBook() { ZoneCode = zoneitem.FullCode };
                                addContractWarrant = SetWarrantValue(tempFamilyContractWarrant, concord);
                                WarrantsModified.Add(addContractWarrant);
                            }
                            else
                            {
                                addContractWarrant = SetWarrantValue(RegeditBooksPersonsBefore, concord);
                                WarrantsModified.Add(addContractWarrant);
                            }
                        }
                    }
                }
                e.Parameter = true;
            }
            catch
            {
                e.Parameter = false;
            }
        }

        #endregion

        #region Methods - Private

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
            arg.Datasource = DbContext;
            arg.Parameter = zoneCode;
            arg.Name = ZoneMessage.ZONE_GET;
            TheBns.Current.Message.Send(this, arg);
            Zone county = arg.ReturnValue as Zone;
            return county;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        private bool CheckSubmit()
        {
            bool error = false;
            //Dispatcher.Invoke(new Action(() =>
            //{
            if (txtYearNumber.Value == null)
            {
                ShowBox("权证年号不能为空！");
                error = true;
            }
            if (txtContractRegeditBookTime.Value == null)
            {
                ShowBox("登簿日期不能为空！");
                error = true;
            }
            if (txtWriteTime.Value == null)
            {
                ShowBox("填证日期设置！");
                error = true;
            }
            if (txtStartTime.Value == null)
            {
                ShowBox("颁证日期设置！");
                error = true;
            }
            if (txtWriteTime.Value != null && txtStartTime.Value != null && ((DateTime)txtWriteTime.Value).Date > ((DateTime)txtStartTime.Value).Date)
            {
                ShowBox(ContractRegeditBookInfo.WarrantDateError);
                error = true;
            }
            //}));
            return error;
        }

        /// <summary>
        /// 设置权证的值
        /// </summary>
        /// <param name="concord">待签订的权证</param>       
        /// <returns>签订后的权证</returns>
        private ContractRegeditBook SetWarrantValue(ContractRegeditBook book, ContractConcord concord)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                book.ID = concord.ID;
                book.Count = 0;
                book.Founder = "Admin";
                book.Modifier = "Admin";

                /* 将耗时的生成流水号业务放在任务里面（是为了防止界面卡死） */
                //book.SerialNumber = InitalizeBookSerialNumber(book);

                book.Number = concord.ConcordNumber;
                book.RegeditNumber = concord.ConcordNumber;
                book.Year = this.txtYearNumber.Value.ToString();
                book.WriteDate = (DateTime)txtWriteTime.Value;//填证日期
                book.SendDate = (DateTime)txtStartTime.Value;//颁证日期
                book.SendOrganization = string.IsNullOrEmpty(txtAwareUnit.Text) ? "" : txtAwareUnit.Text.Trim();//颁证机关
                book.WriteOrganization = txtWriteUnit.Text;//填证单位 
                book.ContractRegeditBookTime = (DateTime)txtContractRegeditBookTime.Value;//登薄日期
                book.ContractRegeditBookPerson = txtContractRegeditBookPerson.Text;
                book.ContractRegeditBookExcursus = txtContractRegeditBookExcursus.Text;

                book.OldBookReTake = chkOldbookTakeBack.IsChecked ?? false;
                book.RegeditBookGettedDate = (DateTime)txtBookTakeTime.Value;
                book.GetterName = txtBookTakePerson.Text;
                book.Comment = txtBookComment.Text;
                book.PrintBookNumber = txtPrintBookNumber.Text;

            }));
            return book;
        }

        /// <summary>
        /// 提示
        /// </summary>
        private void ShowBox(string msg)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = ContractRegeditBookInfo.InitialWarrantInfo,
                Message = msg,
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 初始化页面--用于获取初始化值
        /// </summary>
        protected override void OnInitializeGo()
        {

        }

        /// <summary>
        /// 初始化完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            InitiallPageControl();
        }

        /// <summary>
        /// 初始化页面控件值
        /// </summary>
        private void InitiallPageControl()
        {
            Dispatcher.Invoke(new Action(() =>
                  {
                      chkUseAbbreviation.IsChecked = true;
                      chkuseFamilyContract.IsChecked = true;
                      txtAwareUnit.Text = CreateAwareAbbreviationUnit();
                      txtWriteUnit.Text = CreateWriteUnit();
                      txtStartTime.Value = DateTime.Now;
                      txtWriteTime.Value = DateTime.Now;
                      txtYearNumber.Value = DateTime.Now.Year;
                      txtContractRegeditBookTime.Value = DateTime.Now;

                      chkOldbookTakeBack.IsChecked = false;
                      txtBookTakeTime.Value = DateTime.Now;

                  }));
        }

        #endregion
    }
}
