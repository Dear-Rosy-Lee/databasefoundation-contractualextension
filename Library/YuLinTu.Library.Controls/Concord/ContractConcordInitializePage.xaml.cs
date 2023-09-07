/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包合同初始化界面
    /// </summary>
    public partial class ContractConcordInitializePage : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractConcordInitializePage(bool isBatch = false, IDbContext db = null)
        {
            InitializeComponent();
            this.Header = isBatch ? ContractConcordInfo.InitialConcordInfoVolumn : ContractConcordInfo.InitialConcordInfo;
            //if (isBatch)
            //    cbTissue.IsEnabled = false;
            this.DbContext = db;
            //cbReArea.IsChecked = true;
            this.DataContext = this;  //绑定数据源(任意属性)
            LandsOfInitialConcord = new List<ContractLand>();
            ConcordsModified = new List<ContractConcord>();
            ListLand = new List<ContractLand>();
            SelectPersonCollection = new List<VirtualPerson>();

            Confirm += ContractConcordInitializePage_Confirm;
        }

        #endregion

        #region Fields


        /// <summary>
        /// 承包方类型
        /// </summary>
        private List<Dictionary> contracterTypeList;

        /// <summary>
        /// 地块类别
        /// </summary>
        private List<Dictionary> dklbList;

        /// <summary>
        /// 土地用途
        /// </summary>
        private List<Dictionary> tdytList;

        /// <summary>
        /// 承包方式
        /// </summary>
        private List<Dictionary> cbfsList;

        /// <summary>
        /// 发包方列表
        /// </summary>
        private List<CollectivityTissue> tissueList;

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
        /// 当前地域下的承包地块集合
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 待签订合同的承包方集合
        /// </summary>
        public List<VirtualPerson> SelectPersonCollection { get; set; }

        /// <summary>
        /// 承包合同集合(初始化之前)
        /// </summary>
        public List<ContractConcord> ListConcord { get; set; }

        /// <summary>
        /// 承包合同集合(初始化之后)
        /// </summary>
        public List<ContractConcord> ConcordsModified { get; set; }

        /// <summary>
        /// 初始化合同信息的地块集合
        /// </summary>
        public List<ContractLand> LandsOfInitialConcord { get; set; }

        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Sender { get; set; }
        /// <summary>
        /// 当前选择行政区域下的所有发包方
        /// </summary>
        public List<CollectivityTissue> Senders { get; set; }

        /// <summary>
        /// 承包合同常规设置
        /// </summary>
        public ContractConcordSettingDefine ConcordSettingDefine { get; set; }

        /// <summary>
        /// 是否对承包地块面积进行处理
        /// </summary>
        public bool IsCalculateArea { get; set; }

        /// <summary>
        /// 所有的地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 异步执行
        /// </summary>
        private void ContractConcordInitializePage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            try
            {
                string error = this.CheckSubmit();

                if (!string.IsNullOrEmpty(error))
                {
                    Dispatcher.Invoke(new Action(() =>
                   {
                       ShowBox(error);
                   }));
                    e.Parameter = false;
                    return;
                }
                List<VirtualPerson> persons = CreateVirtualPersonCollection();
                if (this.SelectPersonCollection == null || this.SelectPersonCollection.Count == 0)
                {
                    //此时没有进行承包方选择操作,使用序列化文件
                    List<PersonSelectedInfo> infos = ConcordExtend.DeserializeSelectedInfo();
                    foreach (var person in persons)
                    {
                        List<PersonSelectedInfo> selectedInfo = new List<PersonSelectedInfo>();
                        var info = infos.Find(c => c.ID == person.ID);
                        if (info == null)
                        {
                            info = new PersonSelectedInfo() { ID = person.ID, Name = person.Name, Status = eVirtualPersonStatus.Lock };
                            selectedInfo.Add(info);
                            ConcordExtend.SerializeSelectedInfo(selectedInfo);
                        }
                        if (info.Status == eVirtualPersonStatus.Lock)
                        {
                            SelectPersonCollection.Add(person);
                        }
                    }
                }
                SetDataToConcord();
                List<string> flist = ConcordExtend.DeserializeContractInfo(true);
                List<string> olist = ConcordExtend.DeserializeContractInfo();
                List<string> selectList = new List<string>();
                ConcordContractTimeInfo userconcordtimeinfo = new ConcordContractTimeInfo();
                Dispatcher.Invoke(new Action(() =>
                {
                    foreach (var item in lbItem.Items)
                    {
                        YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind clb = item as YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind;
                        if (!clb.Status)
                            continue;
                        selectList.Add(clb.Entity.Code);
                    }
                    if ((cbContractWay.SelectedItem as Dictionary).Code == ((int)eConstructMode.Family).ToString())
                        flist = selectList;
                    else
                        olist = selectList;

                    userconcordtimeinfo.ConcordStartTime = txtStartTime.Value == null ? DateTime.Now : txtStartTime.Value.Value;
                    userconcordtimeinfo.ConcordEndTime = txt_EndTime.Value == null ? DateTime.Now : txt_EndTime.Value.Value;
                    userconcordtimeinfo.ContractDate = ContractDateTime.Value == null ? DateTime.Now : ContractDateTime.Value.Value;
                    userconcordtimeinfo.SenderDate = SenderDateTime.Value == null ? DateTime.Now : SenderDateTime.Value.Value;
                }));
                ConcordExtend.SerializeContractInfo(flist, olist);
                ConcordExtend.SerializeConcordContractTimeInfo(userconcordtimeinfo);
                e.Parameter = true;
            }
            catch
            {
                e.Parameter = false;
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>  
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //IsCalculateArea = (bool)cbPointSet.IsChecked;
            IsCalculateArea = false;
            ConfirmAsync();
        }

        /// <summary>
        /// 选择承包方
        /// </summary>
        private void btnSelectPerson_Click(object sender, RoutedEventArgs e)
        {
            InitializeConcordPersonPage selectPersonPage = new InitializeConcordPersonPage();
            selectPersonPage.Workpage = this.Workpage;
            selectPersonPage.FamilyCollection = CreateVirtualPersonCollection();
            Workpage.Page.ShowMessageBox(selectPersonPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                //此处将选中的承包方传回来
                this.SelectPersonCollection = selectPersonPage.SelectPersonCollection;
            });
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 创建承包方集合
        /// </summary>  
        private List<VirtualPerson> CreateVirtualPersonCollection()
        {
            var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            List<VirtualPerson> persons = personStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
            List<VirtualPerson> vps = new List<VirtualPerson>();
            var orderdVps = persons.OrderBy(vp =>
            {
                //排序
                int num = 0;
                Int32.TryParse(vp.FamilyNumber, out num);
                return num;
            });
            foreach (VirtualPerson vp in orderdVps)
            {
                vps.Add(vp);
            }
            //vps.RemoveAll(c => c.Name.Contains("集体"));  //排除集体户昌松说加入集体的
            return vps;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        private string CheckSubmit()
        {
            string errorMsg = string.Empty;
            Dispatcher.Invoke(new Action(() =>
            {
                bool hasSelect = false;
                foreach (var item in lbItem.Items)
                {
                    YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind clb = item as YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind;
                    if (clb.Status)
                        hasSelect = true;
                }
                Sender = cbTissue.SelectedItem as CollectivityTissue;
                if (Sender == null)
                {
                    errorMsg += string.Format("{0}未获取到发包方信息", CurrentZone.FullName) + ",";
                }
                if (!hasSelect)
                {
                    errorMsg += ContractConcordInfo.NoLandTypeSelect + ",";
                }
                if (txt_EndTime.Value < txtStartTime.Value)
                {
                    errorMsg += ContractConcordInfo.ContractDateError + ",";
                }
                if (txtResultDate.Value < txtSurveyDate.Value)
                {
                    errorMsg += ContractConcordInfo.ResultAndSurveyDateError + ",";
                }
                if (txtCheckDate.Value < txtResultDate.Value)
                {
                    errorMsg += ContractConcordInfo.CheckAndResultDateError + ",";
                }
            }));
            return errorMsg.TrimEnd(',');
        }

        /// <summary>
        /// 设置数据到合同及其它信息
        /// </summary>
        private void SetDataToConcord()
        {
            try
            {
                List<string> typeList = new List<string>();
                string currentConstructMode = string.Empty;
                Dispatcher.Invoke(new Action(() =>
                {
                    foreach (var item in lbItem.Items)
                    {
                        YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind bind = item as YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind;
                        if (bind != null && bind.Status)
                        {
                            typeList.Add(bind.Entity.Code);
                        }
                    }
                    currentConstructMode = (cbContractWay.SelectedItem as Dictionary).Code;
                }));
                var cordStation = DbContext.CreateConcordStation();
                List<ContractLand> landCollectioin = new List<ContractLand>();  //当前承包方下待初始化合同的地块集合
                foreach (var currentPerson in this.SelectPersonCollection)
                {
                    ContractConcord tempConcord = new ContractConcord() { ZoneCode = currentPerson.ZoneCode };
                    var concords = ListConcord.FindAll(c => c.ContracterId == currentPerson.ID);
                    List<ContractLand> lands = ListLand.FindAll(t => t.OwnerId == currentPerson.ID);
                    var landhasCBFSCount = lands.Count(lf => lf.ConstructMode.IsNullOrEmpty());
                    if (landhasCBFSCount > 0)
                    {
                        Log.Log.WriteException(currentPerson, "错误", CurrentZone.FullName + currentPerson.Name + "在库中的地承包方式为空。");
                        continue;
                    }
                    foreach (ContractLand land in lands)
                    {
                        if (!typeList.Contains(land.LandCategory))
                        {
                            continue;
                        }
                        if (currentConstructMode.Equals(((int)eConstructMode.Family).ToString()) && land.ConstructMode == currentConstructMode)
                        {
                            landCollectioin.Add(land);
                        }
                        else if (!currentConstructMode.Equals(((int)eConstructMode.Family).ToString()) && land.ConstructMode != ((int)eConstructMode.Family).ToString())
                        {
                            landCollectioin.Add(land);
                        }
                    }
                    if (landCollectioin.Count == 0)
                    {
                        continue;
                    }
                    if (concords == null || concords.Count == 0)
                    {
                        //当前承包下没有签订合同
                        ContractConcord addConcord = SetConcordValue(tempConcord, currentPerson, currentConstructMode, landCollectioin);
                        ConcordsModified.Add(addConcord);
                        continue;
                    }
                    if (concords != null && concords.Count > 0)
                    {
                        bool isExsitConcord = concords.Any(c => c.ArableLandType == currentConstructMode);
                        if (!isExsitConcord && landCollectioin != null && landCollectioin.Count > 0)
                        {
                            //当前承包方下已有签订合同,但是不存在与所选承包方式相同的合同,并且存在与所选承包方式相同的地块(集合)
                            ContractConcord addConcord = SetConcordValue(tempConcord, currentPerson, currentConstructMode, landCollectioin);
                            ConcordsModified.Add(addConcord);
                            continue;
                        }
                    }
                    foreach (var concord in concords)
                    {
                        //此处仅做初始化修改操作
                        if ((currentConstructMode == ((int)eConstructMode.Family).ToString() && concord.ArableLandType == ((int)eConstructMode.Family).ToString())
                            || (currentConstructMode != ((int)eConstructMode.Family).ToString() && concord.ArableLandType != ((int)eConstructMode.Family).ToString()))
                        {
                            ContractConcord modifiedConcord = SetConcordValue(concord, currentPerson, currentConstructMode, landCollectioin);
                            ConcordsModified.Add(modifiedConcord);
                        }
                    }
                    landCollectioin.Clear();
                    concords = null;
                    lands.Clear();
                    lands = null;
                }
                landCollectioin = null;
            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// 设置合同的值
        /// </summary>
        /// <param name="concord">待签订的合同</param>
        /// <param name="currentPerson">当前签订合同的承包方</param>
        /// <param name="currentConstructMode">承包方式</param>
        /// <returns>签订后的合同</returns>
        private ContractConcord SetConcordValue(ContractConcord concord, VirtualPerson currentPerson, string currentConstructMode, List<ContractLand> landCollectioin)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                CalcateConcordArea(concord, landCollectioin);//计算合同面积
                Dictionary contracterTypeDiction = contracterTypeList.Find(t => t.Code == ((int)currentPerson.FamilyExpand.ContractorType).ToString());
                concord.ArableLandEndTime = txt_EndTime.Value;//承包结束时间
                concord.ArableLandStartTime = txtStartTime.Value;//承包开始时间
                concord.ContractDate = ContractDateTime.Value;//承包方签订时间
                concord.SenderDate = SenderDateTime.Value;//发包方签订时间    
                concord.ContracerType = contracterTypeDiction == null ? "1" : contracterTypeDiction.Code;//承包方类型
                concord.ContracterId = currentPerson.ID;//承包方Id
                concord.ContracterIdentifyNumber = currentPerson.Number;//承包方证件号
                concord.ContracterName = currentPerson.Name;//承包方姓名
                concord.Flag = (bool)cbLongtime.IsChecked;//长久标志
                concord.LandPurpose = (cbLandPurpose.SelectedItem as Dictionary).Code;//土地用途
                if (concord.Flag)
                {
                    concord.ManagementTime = "长久";
                }
                else
                {
                    concord.ManagementTime = YuLinTu.Library.WorkStation.ToolDateTime.CalcateTerm(concord.ArableLandStartTime.Value, concord.ArableLandEndTime.Value);
                }


                concord.Status = eStatus.Checked;//状态
                concord.ZoneCode = currentPerson.ZoneCode;//地域代码
                concord.Founder = "Admin";//创建者
                concord.Modifier = "Admin";//修改者
                concord.IsValid = true;
                concord.ArableLandType = currentConstructMode;//承包方式
                concord.LandPurpose = (cbLandPurpose.SelectedItem as Dictionary).Code;

                //陈泽林 20161216 根据设置进行
                if (Senders == null)//表示按组进行导时，根据选择的发包方来进行
                {
                    concord.SenderId = Sender.ID;//发包方Id
                    concord.SenderName = Sender.Name;//发包方名称
                    if (Sender == null)
                    {
                        concord.ConcordNumber = CurrentZone.FullCode.PadRight(14, '0') + currentPerson.FamilyNumber.PadLeft(4, '0') + (currentConstructMode == ((int)eConstructMode.Family).ToString() ? "J" : "Q");
                    }
                    else
                    {
                        concord.ConcordNumber = Sender.Code + currentPerson.FamilyNumber.PadLeft(4, '0') + (currentConstructMode == ((int)eConstructMode.Family).ToString() ? "J" : "Q");
                    }
                }
                else
                {
                    if (ConcordSettingDefine.ChooseBatch == 0)//批量签时
                    {
                        CollectivityTissue ssender = Senders.Find(c => c.ZoneCode.Equals(currentPerson.ZoneCode));
                        concord.ConcordNumber = currentPerson.ZoneCode.PadRight(14, '0') + currentPerson.FamilyNumber.PadLeft(4, '0') + (currentConstructMode == ((int)eConstructMode.Family).ToString() ? "J" : "Q");
                        if (ssender != null)
                        {
                            concord.SenderId = ssender.ID;//发包方Id
                            concord.SenderName = ssender.Name;//发包方名称
                        }
                    }
                    else
                    {
                        concord.SenderId = Sender.ID;//发包方Id
                        concord.SenderName = Sender.Name;//发包方名称
                        if (Sender == null)
                        {
                            concord.ConcordNumber = CurrentZone.FullCode.PadRight(14, '0') + currentPerson.FamilyNumber.PadLeft(4, '0') + (currentConstructMode == ((int)eConstructMode.Family).ToString() ? "J" : "Q");
                        }
                        else
                        {
                            concord.ConcordNumber = Sender.Code + currentPerson.FamilyNumber.PadLeft(4, '0') + (currentConstructMode == ((int)eConstructMode.Family).ToString() ? "J" : "Q");
                        }
                    }
                }


                concord.ContractCredentialNumber = concord.ConcordNumber;//经营权证号

                if (cbSurveyPersonName.IsChecked.Value)
                {
                    concord.PublicityChroniclePerson = string.IsNullOrEmpty(txtSurvey.Text) ? "" : txtSurvey.Text.Trim();
                }
                if (cbSurveyDate.IsChecked.Value)
                {
                    concord.PublicityDate = (txtSurveyDate.Value.HasValue && txtSurveyDate.Value != null) ? txtSurveyDate.Value : null;
                }
                if (cbSurveyInfo.IsChecked.Value)
                {
                    concord.PublicityChronicle = string.IsNullOrEmpty(txtSurveyInfo.Text) ? "" : txtSurveyInfo.Text.Trim();
                }
                if (cbCheck.IsChecked.Value)
                {
                    concord.PublicityContractor = currentPerson.Name; //string.IsNullOrEmpty(txtCheck.Text) ? "" : txtCheck.Text.Trim();
                }
                if (cbCheckDate.IsChecked.Value)
                {
                    concord.PublicityResultDate = (txtResultDate.Value.HasValue && txtResultDate.Value != null) ? txtResultDate.Value : null;
                }
                if (cbCheckInfo.IsChecked.Value)
                {
                    concord.PublicityResult = string.IsNullOrEmpty(txtCheckInfo.Text) ? "" : txtCheckInfo.Text.Trim();
                }
                if (cbPub.IsChecked.Value)
                {
                    concord.PublicityCheckPerson = string.IsNullOrEmpty(txtPub.Text) ? "" : txtPub.Text.Trim();
                }
                if (cbPubDate.IsChecked.Value)
                {
                    concord.PublicityCheckDate = (txtCheckDate.Value.HasValue && txtCheckDate.Value != null) ? txtCheckDate.Value : null;
                }
                if (cbPubInfo.IsChecked.Value)
                {
                    concord.PublicityCheckOpinion = string.IsNullOrEmpty(txtPubInfo.Text) ? "" : txtPubInfo.Text.Trim();
                }
                if (cbComment.IsChecked.Value)
                {
                    concord.Comment = string.IsNullOrEmpty(txtComment.Text) ? "" : txtComment.Text.Trim();
                }
            }));

            return concord;
        }

        /// <summary>
        /// 计算合同面积
        /// </summary>
        /// <param name="concord">合同 </param>      
        /// <param name="landCollection">未签订合同的地块集合</param>
        private void CalcateConcordArea(ContractConcord concord, List<ContractLand> landCollection)
        {
            concord.CountActualArea = 0.0;
            concord.CountAwareArea = 0.0;
            concord.TotalTableArea = 0.0;
            concord.CountMotorizeLandArea = 0.0;
            foreach (ContractLand land in landCollection)
            {
                land.ConcordId = concord.ID;
                concord.CountActualArea += land.ActualArea;
                concord.CountAwareArea += land.AwareArea;   //界面显示的合同面积
                concord.TotalTableArea += land.TableArea.HasValue ? land.TableArea.Value : 0.0;
                concord.CountMotorizeLandArea += land.MotorizeLandArea.HasValue ? land.MotorizeLandArea.Value : 0.0;
                LandsOfInitialConcord.Add(land);
            }
            landCollection.Clear();
        }

        /// <summary>
        /// 提示
        /// </summary>
        private void ShowBox(string msg)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = ContractConcordInfo.InitialConcordInfo,
                Message = msg,
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 初始化页面
        /// </summary>
        protected override void OnInitializeGo()
        {
            DictionaryBusiness dicBusiness = new DictionaryBusiness(DbContext);
            List<Dictionary> dicList = dicBusiness.GetAll();
            dklbList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DKLB);
            tdytList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
            cbfsList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            contracterTypeList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.CBFLX);
            ModuleMsgArgs arg = MessageExtend.SenderMsg(DbContext, SenderMessage.SENDER_GETCHILDRENDATA, CurrentZone.FullCode);
            TheBns.Current.Message.Send(this, arg);
            var senderStation = DbContext.CreateSenderWorkStation();
            var tissues = senderStation.GetTissues(CurrentZone.UpLevelCode);
            tissueList = arg.ReturnValue as List<CollectivityTissue>;
            tissueList.Sort((a, b) => { return a.Code.CompareTo(b.Code); });
            if (tissues != null)
                tissueList.AddRange(tissues);
        }

        /// <summary>
        /// 初始化完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            InitiallPageControl();

            Dispatcher.Invoke(new Action(() =>
            {
                List<string> list = null;
                if ((cbContractWay.SelectedItem as Dictionary).Code == ((int)eConstructMode.Family).ToString())
                {
                    list = ConcordExtend.DeserializeContractInfo(true);
                }
                else
                {
                    list = ConcordExtend.DeserializeContractInfo();
                }
                if (list == null || list.Count == 0)
                    return;
                foreach (var item in lbItem.Items)
                {
                    YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind clb = item as YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind;
                    if (list.Any(t => t == clb.Entity.Code))
                        clb.Status = true;
                    else
                        clb.Status = false;
                }

            }));

        }

        /// <summary>
        /// 初始化页面控件值
        /// </summary>
        private void InitiallPageControl()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ConcordContractTimeInfo ConcordContractTimeInfos = ConcordExtend.DeserializeSerializeConcordContractTimeInfo();
                if (ConcordContractTimeInfos == null || (ConcordContractTimeInfos.ConcordStartTime == DateTime.MinValue && ConcordContractTimeInfos.ConcordEndTime == DateTime.MinValue))
                {
                    txtStartTime.Value = DateTime.Now;
                    txt_EndTime.Value = new DateTime(2027, 12, 31);
                    SenderDateTime.Value = DateTime.Now;
                    ContractDateTime.Value = DateTime.Now;
                }
                else
                {
                    txtStartTime.Value = ConcordContractTimeInfos.ConcordStartTime;
                    txt_EndTime.Value = ConcordContractTimeInfos.ConcordEndTime;
                    SenderDateTime.Value = ConcordContractTimeInfos.SenderDate;
                    ContractDateTime.Value = ConcordContractTimeInfos.ContractDate;
                    if (ConcordContractTimeInfos.SenderDate == DateTime.MinValue)
                    {
                        SenderDateTime.Value = DateTime.Now;
                    }
                    if (ConcordContractTimeInfos.ContractDate == DateTime.MinValue)
                    {
                        ContractDateTime.Value = DateTime.Now;
                    }
                }

                if (tissueList != null)
                {
                    cbTissue.ItemsSource = tissueList;
                    cbTissue.DisplayMemberPath = "Name";
                    cbTissue.SelectedIndex = 0;
                }
                if (dklbList != null)
                {
                    dklbList.RemoveAll(t => t.Code == "");
                    List<YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind> clbList = new List<YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind>();
                    dklbList.ForEach(t =>
                    {
                        YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind en = new YuLinTu.Library.Controls.ConcordInfoPage.ConcordLandBind();
                        en.Entity = t;
                        en.Name = t.Name;
                        en.Status = (t.Code == "10" ? true : false);
                        clbList.Add(en);
                    });
                    lbItem.ItemsSource = clbList;
                }
                if (tdytList != null)
                {
                    tdytList.RemoveAll(t => t.Code == "");
                    cbLandPurpose.ItemsSource = tdytList;
                    cbLandPurpose.DisplayMemberPath = "Name";
                    cbLandPurpose.SelectedIndex = 0;
                }
                if (cbfsList != null)
                {
                    if (ConcordSettingDefine.AgricultureConcordAllowMultiType)
                    {
                        cbfsList.RemoveAll(t => t.Code != ((int)eConstructMode.Family).ToString() && t.Code != ((int)eConstructMode.OtherContract).ToString());
                    }
                    else
                    {
                        cbfsList.RemoveAll(t => t.Code != ((int)eConstructMode.Family).ToString());
                    }
                    cbContractWay.ItemsSource = cbfsList;
                    cbContractWay.DisplayMemberPath = "Name";
                    cbContractWay.SelectedIndex = 0;
                }
            }));
        }


        #endregion
    }
}
