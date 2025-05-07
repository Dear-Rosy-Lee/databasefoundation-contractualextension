/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 合同信息界面
    /// </summary>
    public partial class ConcordInfoPage : InfoPageBase
    {
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

        private bool isAdd; //是否添加     

        private List<ContractLand> familyLandList;//当前待签定合同的地块

        #endregion

        #region Properties

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson CurrentVp { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 合同
        /// </summary>
        public ContractConcord Concord { get; set; }

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
                    cbVirtualPerson.IsEnabled = true;
                    cbContractWay.IsEnabled = true;
                }
                else
                {
                    cbVirtualPerson.IsEnabled = false;
                    cbContractWay.IsEnabled = false;
                    if (CurrentVp != null && CurrentVp.Status == eVirtualPersonStatus.Lock)
                        btnAdd.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 业务
        /// </summary>
        public ConcordBusiness Business { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext dbContext { get; set; }

        /// <summary>
        /// 已签订合同
        /// </summary>
        public ObservableCollection<ConcordItem> Items { get; set; }

        /// <summary>
        /// 承包方列表
        /// </summary>
        public List<VirtualPerson> ContracterList { get; set; }

        /// <summary>
        /// 当前签订合同的承包方
        /// </summary>
        public VirtualPerson CurrentFamily { get; set; }

        /// <summary>
        /// 地域下地块
        /// </summary>
        public List<ContractLand> LandList { get; set; }

        /// <summary>
        /// 当前待签定合同的地块
        /// </summary>
        public List<ContractLand> FamilyLandList
        {
            get { return familyLandList; }
            set { familyLandList = value; }
        }

        /// <summary>
        /// 承包合同常规设置
        /// </summary>
        public ContractConcordSettingDefine ConcordSettingDefine { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConcordInfoPage()
        {
            InitializeComponent();
            //cbReArea.IsChecked = true;
            this.DataContext = this;
            this.Confirm += ConcordInfoPage_Confirm;
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        protected override void OnInitializeGo()
        {
            DictionaryBusiness dicBusiness = new DictionaryBusiness(dbContext);
            List<Dictionary> dicList = dicBusiness.GetAll();
            dklbList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DKLB);
            tdytList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
            cbfsList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            contracterTypeList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.CBFLX);
            ModuleMsgArgs arg = MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_GETCHILDRENDATA, CurrentZone.FullCode);
            TheBns.Current.Message.Send(this, arg);
            var senderStation = dbContext.CreateSenderWorkStation();
            var tissues = senderStation.GetTissues(CurrentZone.UpLevelCode);
            tissueList = arg.ReturnValue as List<CollectivityTissue>;
            if (tissues != null)
                tissueList.AddRange(tissues);
        }

        /// <summary>
        /// 初始化完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            InitiallPageControl();
            if (!isAdd)
            {
                txtStartTime.Value = Concord.ArableLandStartTime;
                txt_EndTime.Value = Concord.ArableLandEndTime;
                SenderDateTime.Value = Concord.SenderDate;
                ContractDateTime.Value = Concord.ContractDate;
                //cbAddress
                if (Concord.Flag)
                {
                    cbLongtime.IsChecked = true;
                }
                foreach (var item in cbTissue.Items)
                {
                    CollectivityTissue tissue = item as CollectivityTissue;
                    if (tissue.ID == Concord.SenderId)
                    {
                        cbTissue.SelectedItem = item;
                        break;
                    }
                }
                foreach (var item in cbVirtualPerson.Items)
                {
                    VirtualPerson vp = item as VirtualPerson;
                    if (vp.ID == Concord.ContracterId)
                    {
                        cbVirtualPerson.SelectedItem = item;
                        break;
                    }
                }
                foreach (var item in cbContractWay.Items)
                {
                    Dictionary dic = item as Dictionary;
                    if (dic.Code == Concord.ArableLandType)
                    {
                        cbContractWay.SelectedItem = item;
                        break;
                    }
                }
                foreach (var item in cbLandPurpose.Items)
                {
                    Dictionary dic = item as Dictionary;
                    if (dic.Code == Concord.LandPurpose)
                    {
                        cbLandPurpose.SelectedItem = item;
                        break;
                    }
                }
            }
            List<string> list = new List<string> ();
            if (!IsAdd)
            {
                //陈泽林 20161021 地块类别从数据库地块中取
                List<ContractLand> lands = Business.GetLandByConcordId(Concord.ID);
                if (lands == null || lands.Count <= 0)
                {
                    ShowBox("该合同没有签订地块，请检查");
                    return;
                }
                lands.ForEach(c =>
                {
                    if (!list.Any(t => t == c.LandCategory))
                        list.Add(c.LandCategory);
                });
            }
            else
            {
                if ((cbContractWay.SelectedItem as Dictionary).Code == "110")
                {
                    list = ConcordExtend.DeserializeContractInfo(true);
                }
                else
                {
                    list = ConcordExtend.DeserializeContractInfo();
                }
            }
            foreach (var item in lbItem.Items)
            {
                ConcordLandBind clb = item as ConcordLandBind;
                if (list.Any(t => t == clb.Entity.Code))
                    clb.Status = true;
                else
                    clb.Status = false;
                clb.Enable = IsAdd;
            }
        }

        /// <summary>
        /// 初始化页面控件值
        /// </summary>
        private void InitiallPageControl()
        {
            txtStartTime.Value = DateTime.Now;
            txt_EndTime.Value = new DateTime(2027, 12, 31);
            SenderDateTime.Value = DateTime.Now;
            ContractDateTime.Value = DateTime.Now;
            if (tissueList != null)
            {
                cbTissue.ItemsSource = tissueList;
                cbTissue.DisplayMemberPath = "Name";
                cbTissue.SelectedIndex = 0;
            }
            if (dklbList != null)
            {
                dklbList.RemoveAll(t => t.Code == "");
                List<ConcordLandBind> clbList = new List<ConcordLandBind>();
                dklbList.ForEach(t =>
                {
                    ConcordLandBind en = new ConcordLandBind();
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
            if (ContracterList != null)
            {
                cbVirtualPerson.ItemsSource = ContracterList;
                cbVirtualPerson.DisplayMemberPath = "Name";
                cbVirtualPerson.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        public string CheckSubmit()
        {
            string errorMsg = string.Empty;
            bool hasSelect = false;
            foreach (var item in lbItem.Items)
            {
                ConcordLandBind clb = item as ConcordLandBind;
                if (clb.Status)
                    hasSelect = true;
            }
            if (!hasSelect)
            {
                errorMsg += ContractConcordInfo.NoLandTypeSelect + ",";
            }
            if (txt_EndTime.Value < txtStartTime.Value)
            {
                errorMsg += ContractConcordInfo.ContractDateError + ",";
            }
            if (txtPubResultDate.Value.HasValue && txtSurveyDate.Value.HasValue && txtPubResultDate.Value < txtSurveyDate.Value)
            {
                errorMsg += ContractConcordInfo.ResultAndSurveyDateError + ",";
            }
            if (txtPubCheckDate.Value.HasValue && txtPubResultDate.Value.HasValue && txtPubCheckDate.Value < txtPubResultDate.Value)
            {
                errorMsg += ContractConcordInfo.CheckAndResultDateError;
            }
            if (txtPubCheckDate.Value.HasValue && txtSurveyDate.Value.HasValue && txtSurveyDate.Value > txtPubCheckDate.Value)
            {
                errorMsg += ContractConcordInfo.CheckAndSurveyDateError;
            }
            return errorMsg.TrimEnd(',');
        }

        #endregion

        #region Private

        /// <summary>
        /// 设置合同数据
        /// </summary>
        private void SetDataToConcord()
        {
            familyLandList = null;
            CollectivityTissue tissue = cbTissue.SelectedItem as CollectivityTissue;
            if (tissue == null)
            {
                return;
            }
            List<string> typeList = new List<string>();
            foreach (var item in lbItem.Items)
            {
                ConcordLandBind bind = item as ConcordLandBind;
                if (bind != null && bind.Status)
                {
                    typeList.Add(bind.Entity.Code);
                }
            }
            CurrentFamily = cbVirtualPerson.SelectedItem as VirtualPerson;
            if (CurrentFamily == null)
            {
                return;
            }
            string currentConstructMode = (cbContractWay.SelectedItem as Dictionary).Code;
            List<ContractLand> lands = LandList.FindAll(t => t.OwnerId == CurrentFamily.ID);
            List<ContractConcord> concords = Business.GetCollection(CurrentFamily.ID);  //获取当前承包方下的已有合同集合
            List<ContractLand> landCollectioin = new List<ContractLand>();
            foreach (ContractLand land in lands)
            {
                if (!typeList.Contains(land.LandCategory))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(currentConstructMode) && currentConstructMode == "110" && land.ConstructMode == currentConstructMode)
                    landCollectioin.Add(land);
                else if (!string.IsNullOrEmpty(currentConstructMode) && currentConstructMode != "110" && land.ConstructMode != "110")
                    landCollectioin.Add(land);
            }
            if (landCollectioin.Count == 0 && isAdd)
            {
                string msg = string.Format("承包方{0}下没有此种承包方式的地块或与所选地块类别相匹配的地块,无法签订合同!", CurrentFamily.Name);
                ShowBox(msg);
                return;
            }
            var cord = concords.FirstOrDefault(c => c.ArableLandType == currentConstructMode);
            if (cord != null && cord.IsValid == true && isAdd)
            {
                //已经存在所选承包方式的合同
                string mode = (cbContractWay.SelectedItem as Dictionary).Name;
                string msg = string.Format("承包方{0}下已经存在承包方式采用{1}的合同", CurrentFamily.Name, mode);
                ShowBox(msg);
                return;
            }
            if (cord != null && cord.IsValid == false && isAdd)
            {
                isAdd = false;
                Concord = cord;
            }
            Dictionary contracterTypeDiction = contracterTypeList.Find(t => t.Code == ((int)CurrentFamily.FamilyExpand.ContractorType).ToString());
            CalcateConcordArea(Concord, landCollectioin);//计算合同面积
            familyLandList = landCollectioin;
            Concord.ArableLandEndTime = txt_EndTime.Value;//承包结束时间
            Concord.ArableLandStartTime = txtStartTime.Value;//承包开始时间
            Concord.ContractDate = ContractDateTime.Value;//承包方签订时间
            Concord.SenderDate = SenderDateTime.Value;//发包方签订时间
            Concord.ContracerType = contracterTypeDiction == null ? "1" : contracterTypeDiction.Code;//承包方类型
            Concord.ContracterId = CurrentFamily.ID;//承包方Id
            Concord.ContracterIdentifyNumber = CurrentFamily.Number;//承包方证件号
            Concord.ContracterName = CurrentFamily.Name;//承包方姓名
            Concord.Flag = (bool)cbLongtime.IsChecked;//长久标志
            Concord.LandPurpose = (cbLandPurpose.SelectedItem as Dictionary).Code;//土地用途
            if (Concord.Flag)
            {
                Concord.ManagementTime = "长久";
            }
            else
            {
                Concord.ManagementTime = ToolDateTime.CalcateTerm(Concord.ArableLandStartTime, Concord.ArableLandEndTime);
            }
            Concord.SenderId = tissue.ID;//发包方Id
            Concord.SenderName = tissue.Name;//发包方名称
            Concord.Status = eStatus.Checked;//状态
            Concord.ZoneCode = CurrentZone.FullCode;//地域代码
            Concord.Founder = "Admin";//创建者
            Concord.Modifier = "Admin";//修改者
            Concord.IsValid = true;
            Concord.ArableLandType = currentConstructMode;//承包方式
            Concord.LandPurpose = (cbLandPurpose.SelectedItem as Dictionary).Code;
            //Concord.SecondContracterLocated = txtAddress.Text;//承包方地址
            // Concord.ConcordNumber = CurrentZone.FullCode.PadRight(14, '0') + CurrentFamily.FamilyNumber.PadLeft(4, '0') + (currentConstructMode == "110" ? "J" : "Q");
            if (tissue == null)
            {
                Concord.ConcordNumber = CurrentZone.FullCode.PadRight(14, '0') + CurrentFamily.FamilyNumber.PadLeft(4, '0') + (currentConstructMode == ((int)eConstructMode.Family).ToString() ? "J" : "Q");
            }
            else
            {
                Concord.ConcordNumber = tissue.Code + CurrentFamily.FamilyNumber.PadLeft(4, '0') + (currentConstructMode == ((int)eConstructMode.Family).ToString() ? "J" : "Q");
            }
            Concord.ContractCredentialNumber = Concord.ConcordNumber;//经营权证号
        }

        /// <summary>
        /// 计算合同面积
        /// </summary>
        /// <param name="concord">合同 </param>
        /// <param name="cbPointSet">是否截取小数</param>
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
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string error = CheckSubmit();
            if (!string.IsNullOrEmpty(error))
            {
                ShowBox(error);
                return;
            }
            SetDataToConcord();
            List<string> flist = ConcordExtend.DeserializeContractInfo(true);
            List<string> olist = ConcordExtend.DeserializeContractInfo();
            List<string> selectList = new List<string>();
            foreach (var item in lbItem.Items)
            {
                ConcordLandBind clb = item as ConcordLandBind;
                if (!clb.Status)
                    continue;
                selectList.Add(clb.Entity.Code);
            }
            if ((cbContractWay.SelectedItem as Dictionary).Code == "110")
                flist = selectList;
            else
                olist = selectList;
            ConcordExtend.SerializeContractInfo(flist, olist);
            ConfirmAsync();
        }

        /// <summary>
        /// 页面提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConcordInfoPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            if (familyLandList != null)
            {
                AccountLandBusiness landbus = new AccountLandBusiness(dbContext);
                foreach (var item in familyLandList)
                {
                    landbus.ModifyLand(item);
                }
            }
            if (isAdd && familyLandList != null)
            {
                Business.Add(Concord);
            }
            else if (!isAdd)
            {
                Business.Update(Concord);
            }
            e.Parameter = true;
            Result = true;
        }

        /// <summary>
        /// 提示
        /// </summary>
        private void ShowBox(string msg)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = isAdd ? ContractConcordInfo.AddConcrod : ContractConcordInfo.EditConcord,
                Message = msg,
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }

        #endregion

        #region Class

        public class ConcordLandBind : NotifyCDObject
        {
            private bool status;

            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 是否可选择
            /// </summary>
            public bool Enable { get; set; }

            /// <summary>
            /// 状态
            /// </summary>
            public bool Status
            {
                get { return status; }
                set { status = value; NotifyPropertyChanged("Status"); }
            }

            /// <summary>
            /// 数据
            /// </summary>
            public Dictionary Entity { get; set; }
        }

        #endregion
    }
}
