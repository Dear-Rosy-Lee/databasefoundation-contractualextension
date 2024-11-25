/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
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
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方信息界面
    /// </summary>
    public partial class VirtualPersonPageContent : UserControl
    {
        #region Fields

        /// <summary>
        /// 错误图片
        /// </summary>
        private BitmapImage errorImg = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/错误16_2.png"));

        /// <summary>
        /// 正确图片
        /// </summary>
        private BitmapImage rightImg = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/是16.png"));

        /// <summary>
        /// 证件类型
        /// </summary>
        private KeyValueList<string, string> cardtypeList;

        /// <summary>
        /// 民族
        /// </summary>
        private List<EnumStore<eNation>> nationList;

        /// <summary>
        /// 变化情况
        /// </summary>
        private List<EnumStore<eBHQK>> bhqkList;

        /// <summary>
        /// 承包方类型
        /// </summary>
        private KeyValueList<string, string> contractorTypeList;  //eContractorType

        /// <summary>
        /// 土地承包方式
        /// </summary>
        private KeyValueList<string, string> constructModeList;  //eConstructMode

        /// <summary>
        /// 性别
        /// </summary>
        private KeyValueList<string, string> genderList;  //eGender

        /// <summary>
        /// 承包方
        /// </summary>
        private VirtualPerson contractor;

        /// <summary>
        /// 当前民族
        /// </summary>
        private eNation cNation;

        /// <summary>
        /// 当前承包方式
        /// </summary>
        private eConstructMode eMode;

        private eBHQK eBHQK;

        private string cMode;

        /// <summary>
        /// 当前性别
        /// </summary>
        private eGender eGender;
        private string cGender;

        /// <summary>
        /// 当前证件类型
        /// </summary>
        private eCredentialsType eCard;
        private string cCard;

        /// <summary>
        /// 当前承包方类型
        /// </summary>
        private eContractorType eType;
        private string cType;

        /// <summary>
        /// 承包方
        /// </summary>
        private VirtualPerson tempContracor;

        /// <summary>
        /// 是否添加
        /// </summary>
        private bool isAdd;

        private bool isLock;

        #endregion

        #region Properties

        /// <summary>
        /// 地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 扩展实体
        /// </summary>
        public VirtualPersonExpand FamilyExpand { get; set; }

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
                    cbType.IsEnabled = true;
                }
                else
                {
                    cbType.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson Contractor
        {
            private get { return contractor; }
            set
            {
                tempContracor = value;
                contractor = value.Clone() as VirtualPerson;
                FamilyExpand = (contractor.FamilyExpand.Clone() as VirtualPersonExpand);
                //this.DataContext = this;
                if (IsAdd)
                {
                    contractor.FamilyNumber = Business.GetFamilyNumber(contractor.ZoneCode);
                }
                IntiallFamilyToControl(contractor);
            }
        }

        /// <summary>
        /// 业务
        /// </summary>
        public VirtualPersonBusiness Business { get; set; }

        /// <summary>
        /// 承包方被锁定
        /// </summary>
        public bool IsLock
        {
            get { return isLock; }
            set
            {
                isLock = value;
                if (isLock)
                {
                    MetroListTabItem[] items = new MetroListTabItem[] { tbitemExpand, tbitemFoundation };
                    for (int i = 0; i < 3; i++)
                    {
                        if (i >= items.Length)
                            break;
                        var enmu = (items[i].Content as System.Windows.Controls.Grid).Children.GetEnumerator();
                        while (enmu.MoveNext())
                        {
                            var curTxtEle = enmu.Current as TextBox;
                            if (curTxtEle != null)
                            {
                                curTxtEle.IsReadOnly = true;
                                continue;
                            }
                            var curLbEle = enmu.Current as Label;
                            if (curLbEle != null)
                                continue;
                            var curEle = enmu.Current as UIElement;
                            curEle.IsEnabled = false;
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 构造方法
        /// </summary>
        public VirtualPersonPageContent()
        {
            InitializeComponent();
            DataContext = this;
            InlitiallCombox();

            cNation = eNation.Han;
            eMode = eConstructMode.Family;
            cMode = ((int)eConstructMode.Family).ToString();
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        public string CheckSubmit(FamilyOtherDefine otherDefine)
        {
            string errorMsg = string.Empty; ;
            if (contractor == null)
            {
                return errorMsg = VirtualPersonInfo.EntityNull + ",";
            }
            //if (string.IsNullOrEmpty(contractor.Name))
            //{
            //    errorMsg += VirtualPersonInfo.VpNameNull + ",";
            //}
            if (string.IsNullOrEmpty(contractor.FamilyNumber))
            {
                errorMsg += VirtualPersonInfo.VpCodeNull + ",";
            }
            if (otherDefine.HasTelephoneNumber && string.IsNullOrEmpty(contractor.Telephone))        //检查承包方电话号码是否填写
            {
                errorMsg += VirtualPersonInfo.VpTelNull + ",";
            }
            EnumStore<eCredentialsType> cardType = cbCardType.SelectedItem as EnumStore<eCredentialsType>;
            bool right = ToolICN.Check(contractor.Number);
            if (otherDefine.IsCheckCardNumber && cardType != null && cardType.Value == eCredentialsType.IdentifyCard && !right)
            {
                errorMsg += VirtualPersonInfo.CardCodeError + ",";           //检查身份证号码是否填写以及检验正确性
            }
            if (FamilyExpand.ConcordStartTime >= FamilyExpand.ConcordEndTime)
            {
                errorMsg += VirtualPersonInfo.StartBigEnd + ",";
            }
            if (!string.IsNullOrEmpty(contractor.Name) && contractor.Name.Contains("集体") && !isAdd && FamilyExpand.ContractorType != eContractorType.Unit)
            {
                errorMsg += VirtualPersonInfo.CollPersonMustAdd + ",";
            }
            if (!otherDefine.IsPromiseCardNumberNull && string.IsNullOrEmpty(contractor.Number) && !string.IsNullOrEmpty(contractor.Name) && !contractor.Name.Contains("集体"))
            {
                errorMsg += VirtualPersonInfo.PersonNumberMust + ",";     //不允许证件号码为空
            }
            //if (contractor.PostalNumber != null && contractor.PostalNumber.Length != 6)
            //{
            //    errorMsg += VirtualPersonInfo.PersonNumberError;     //邮政编码不正确
            //}

            int fn;
            Int32.TryParse(txtNum.Text, out fn);
            if (cType == ((int)eContractorType.Farmer).ToString())
            {
                if (fn < 1 || fn > 8000)
                {
                    errorMsg += "农户家庭户号应在0001~8000之间" + ",";
                }
            }
            else if (cType == ((int)eContractorType.Personal).ToString())
            {
                if (fn < 8001 || fn > 9000)
                {
                    errorMsg += "个人家庭户号应在8001~9000之间" + ",";
                }
            }
            else if (cType == ((int)eContractorType.Unit).ToString())
            {
                if (fn < 9001 || fn > 9999)
                {
                    errorMsg += "单位家庭户号应在9001~9999之间";
                }
            }

            return errorMsg.TrimEnd(',');
        }

        /// <summary>
        /// 获取承包方
        /// </summary>
        /// <returns></returns>
        public VirtualPerson GetVirtualPerson()
        {
            try
            {
                if (IsAdd)
                {
                    AddProcessPerson();
                }
                else
                {
                    EditProcessPerson();
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "获取承包方", ex.Message + ex.StackTrace);
            }
            return contractor;
        }

        #endregion

        #region Private

        /// <summary>
        /// 根据内容初始化控件
        /// </summary>
        private void IntiallFamilyToControl(VirtualPerson vp)
        {
            if (vp == null)
            {
                return;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                string familyNumber = vp.FamilyNumber;
                if (string.IsNullOrEmpty(familyNumber))
                {
                    return;
                }
                List<Person> plist = vp.SharePersonList;
                cbBHQK.SelectedItem = bhqkList.Find(t => t.Value == vp.ChangeSituation);
                Person p = (plist == null) ? null : (plist.Find(t => t.Name == vp.Name));
                if (p != null)
                {
                    cbNation.SelectedItem = nationList.Find(t => t.Value == p.Nation);
                    txt_Age.Text = p.Age;
                    txt_Birthday.Value = p.Birthday;
                    cbGender.SelectedItem = genderList.Find(t => t.Key == ((int)p.Gender).ToString());
                }
                var type = contractorTypeList.Find(t => t.Key == ((int)FamilyExpand.ContractorType).ToString());
                if (type != null)
                    cbType.SelectedItem = type;
                var cardType = cardtypeList.Find(t => t.Key == ((int)vp.CardType).ToString());
                if (cardType != null)
                    cbCardType.SelectedItem = cardType;
                var contractWay = constructModeList.Find(t => t.Key == ((int)FamilyExpand.ConstructMode).ToString());
                if (contractWay != null)
                    cbContractWay.SelectedItem = contractWay;
               

                txtNum.Text = familyNumber;
                string splitContent = (familyNumber.PadLeft(4, '0').Replace(familyNumber, ""));
                txtFamilyNum.Text = ZoneCodeExtend.ChangeToSenderCode(contractor.ZoneCode) + splitContent;
            }));
        }

        /// <summary>
        /// 初始化下拉控件
        /// </summary>
        private void InlitiallCombox()
        {
            try
            {
                var dbContext = DataBaseSource.GetDataBaseSource();
                var dictStation = dbContext.CreateDictWorkStation();
                cardtypeList = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.ZJLX);
                genderList = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.XB);
                contractorTypeList = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.CBFLX);
                constructModeList = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.CBJYQQDFS);
            }
            catch (Exception)
            {
                cardtypeList = new KeyValueList<string, string>();
                genderList = new KeyValueList<string, string>();
                contractorTypeList = new KeyValueList<string, string>();
                constructModeList = new KeyValueList<string, string>();
            }
            nationList = EnumStore<eNation>.GetListByType();
            cbNation.DisplayMemberPath = "DisplayName";
            cbNation.ItemsSource = nationList;
            cbNation.SelectedIndex = 0;

            cbGender.DisplayMemberPath = "Value";
            cbGender.ItemsSource = genderList;
            cbGender.SelectedIndex = 0;

            cbType.DisplayMemberPath = "Value";
            cbType.ItemsSource = contractorTypeList;
            cbType.SelectedIndex = 0;

            cbCardType.DisplayMemberPath = "Value";
            cbCardType.ItemsSource = cardtypeList;
            cbCardType.SelectedIndex = 0;

            cbContractWay.DisplayMemberPath = "Value";
            cbContractWay.ItemsSource = constructModeList;
            cbContractWay.SelectedIndex = 0;
            
            cbBHQK.DisplayMemberPath = "DisplayName";
            bhqkList = EnumStore<eBHQK>.GetListByType();
            cbBHQK.ItemsSource = bhqkList;
        }

        /// <summary>
        /// 添加承包方处理
        /// </summary>
        private void AddProcessPerson()
        {
            Person p = new Person();
            p.Name = contractor.Name;
            p.ICN = contractor.Number;
            p.ZoneCode = contractor.ZoneCode;
            p.ID = contractor.ID;
            p.PostNumber = contractor.PostalNumber;
            p.Gender = eGender;
            p.Address = contractor.Address;
            //p.Birthday = ToolICN.GetBirthday(p.ICN);
            p.Birthday = txt_Birthday.Value;
            p.FamilyNumber = contractor.FamilyNumber;
            p.Nation = cNation;
            p.FamilyID = contractor.ID;
            p.CardType = eCard;
            p.IsSharedLand = "是";
            int age = p.GetAge();
            p.Age = age > 0 ? age.ToString() : txt_Age.Text;
            if (!p.Name.Contains("集体"))
                p.Relationship = "户主";
            FamilyExpand.ConstructMode = eMode;
            contractor.CardType = eCard;
            contractor.SharePersonList = new List<Person>() { p };
            FamilyExpand.ContractorType = eType;
            FamilyExpand.BusinessStatus = eBusinessStatus.End;
            contractor.FamilyExpand = FamilyExpand;
        }

        /// <summary>
        /// 修改承包方处理
        /// </summary>
        private void EditProcessPerson()
        {
            List<Person> personList = contractor.SharePersonList;
            Person p = personList.Find(t => t.Name == tempContracor.Name);
            if (p != null)
            {
                p.ICN = contractor.Number;
                p.Name = contractor.Name;
                p.PostNumber = contractor.PostalNumber;
                int age = p.GetAge();
                p.Age = age > 0 ? age.ToString() : txt_Age.Text?.Trim();
                p.Gender = eGender;
                p.Address = contractor.Address;
                p.Birthday = ToolICN.GetBirthday(p.ICN);
                p.FamilyNumber = contractor.FamilyNumber;
                p.Nation = cNation;
                p.FamilyID = contractor.ID;
                p.CardType = eCard;
            }
            FamilyExpand.ConstructMode = eMode;
            contractor.CardType = eCard;
            contractor.ChangeSituation = eBHQK;
            contractor.SharePersonList = personList;
            FamilyExpand.ContractorType = eType;
            FamilyExpand.BusinessStatus = eBusinessStatus.End;
            contractor.FamilyExpand = this.FamilyExpand;
        }

        /// <summary>
        /// 解决数据获取不到的问题
        /// </summary>
        private void GetData()
        {
            Contractor.Number = txt_ICN.Text.Trim();
            Contractor.Telephone = txt_Phone.Text.Trim();
            Contractor.Comment = txt_Comment.Text.Trim();
            Contractor.Address = txtAddress.Text.Trim();
            FamilyExpand.SurveyPerson = txtSurvey.Text.Trim();
            FamilyExpand.SurveyDate = txtSurveyDate.Value;
            FamilyExpand.SurveyChronicle = txtSurveyInfo.Text.Trim();
            FamilyExpand.PublicityChroniclePerson = txtPub.Text;
            FamilyExpand.PublicityDate = txtPubDate.Value;
            FamilyExpand.PublicityChronicle = txtPubInfo.Text;
            FamilyExpand.PublicityCheckPerson = txtPubPerson.Text;
            FamilyExpand.ConcordNumber = txtConcordNum.Text;
            FamilyExpand.WarrantNumber = txtBookNum.Text;
            FamilyExpand.ConcordStartTime = txtStartTime.Value;
            FamilyExpand.CheckPerson = txtCheck.Text.Trim();
            FamilyExpand.CheckDate = txtCheckDate.Value;
            FamilyExpand.ConcordEndTime = txtEndTime.Value;
            FamilyExpand.CheckOpinion = txtCheckInfo.Text;
            Contractor.FamilyExpand = FamilyExpand;
        }

        #endregion

        #region Events

        /// <summary>
        /// 户编码改变
        /// </summary>
        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            string famliyNumer = txtNum.Text.Trim();
            contractor.FamilyNumber = famliyNumer;
            if (string.IsNullOrEmpty(famliyNumer))
            {
                return;
            }
            string splitContent = (famliyNumer.PadLeft(4, '0').Replace(famliyNumer, ""));
            txtFamilyNum.Text = ZoneCodeExtend.ChangeToSenderCode(CurrentZone.FullCode) + splitContent;
        }

        /// <summary>
        /// 身份证输入框值变化
        /// </summary>
        private void txt_ICN_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ICN = txt_ICN.Text.Trim();
            SetBirthInfo(ICN);
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        private void SetBirthInfo(string ICN)
        {
            try
            {
                var type = cbType.SelectedItem as KeyValue<string, string>;
                if (type == null || type.Key == ((int)eContractorType.Unit).ToString() || eCard != eCredentialsType.IdentifyCard)
                {
                    return;
                }
                DateTime birthday = ToolICN.GetBirthdayInNotCheck(ICN);
                txt_Birthday.Value = birthday;

                int age = ToolDateTime.GetAge(birthday);
                txt_Age.Text = age > 0 ? age.ToString() : "";

                eGender gender = ToolICN.GetAllGenderNoCheck(ICN);
                cbGender.SelectedItem = genderList.Find(t => t.Key == ((int)gender).ToString());
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteError(this, "SetBirthInfo(设置信息)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 类型变化
        /// </summary>
        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var type = cbType.SelectedItem as KeyValue<string, string>;
            if (type == null)
                return;
            cType = type.Key;
            Enum.TryParse(cType, out eType);  //转换成承包方类型枚举
            if (cType == ((int)eContractorType.Unit).ToString())
            {
                cbNation.SelectedItem = nationList.Find(t => t.DisplayName == "未知");
                cbNation.IsEnabled = false;
                cbCardType.SelectedIndex = 2;
                cbCardType.IsEnabled = false;
            }
            else
            {
                //cbNation.SelectedIndex = 0;
                cbNation.IsEnabled = true;
                //cbCardType.SelectedIndex = 0;
                cbCardType.IsEnabled = true;
            }
            GetNewFamilyNumber();
        }

        /// <summary>
        /// 获取新的家庭户号
        /// </summary>
        private void GetNewFamilyNumber()
        {
            if (CurrentZone == null)
                return;
            var dbContext = DataBaseSource.GetDataBaseSource();
            var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            string familyNumber = vpStation.CreateVirtualPersonNum(CurrentZone.FullCode, eType);
            if (string.IsNullOrEmpty(familyNumber))
                return;
            txtNum.Text = familyNumber;
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
        /// 承包方式改变
        /// </summary>
        private void cbContractWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = cbContractWay.SelectedItem;
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

        /// <summary>
        /// 证件类型改变
        /// </summary>
        private void cbCardType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = cbCardType.SelectedItem;
            if (selectItem == null)
            {
                return;
            }
            var esValue = selectItem as KeyValue<string, string>;
            if (esValue == null)
            {
                return;
            }
            cCard = esValue.Key;
            Enum.TryParse(cCard, out eCard);
            SetControlEnable(txt_Name.Text.Trim(), eCard);
        }

        /// <summary>
        /// 性别改变
        /// </summary>
        private void cbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = cbGender.SelectedItem;
            if (selectItem == null)
            {
                return;
            }
            var esValue = selectItem as KeyValue<string, string>;
            if (esValue == null)
            {
                return;
            }
            cGender = esValue.Key;
            Enum.TryParse(cGender, out eGender);
        }

        /// <summary>
        /// 名称
        /// </summary>
        private void txt_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            string nametxt = txt_Name.Text.Trim();
            SetControlEnable(nametxt, eCard);
        }

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        private void SetControlEnable(string name, eCredentialsType cardTyp)
        {
            if (isAdd)
            {
                if (name.Contains("集体"))
                {
                    cbType.SelectedItem = contractorTypeList.Find(t => t.Key == ((int)eContractorType.Unit).ToString());
                    cbCardType.SelectedItem = cardtypeList.Find(t => t.Key == ((int)eCredentialsType.AgentCard).ToString());
                    cbGender.SelectedItem = genderList.Find(t => t.Key == ((int)eGender.Unknow).ToString());
                    txt_Birthday.Value = null;
                    txt_Age.Text = "";
                    cbType.IsEnabled = false;
                    cbCardType.IsEnabled = false;
                    cbGender.IsEnabled = false;
                    cbNation.IsEnabled = false;
                }
                else
                {
                    //cbType.SelectedItem = contractorTypeList.Find(t => t.Value == eContractorType.Farmer);
                    cbType.IsEnabled = true;
                    cbCardType.IsEnabled = true;
                    //cbNation.IsEnabled = true;
                    txt_Birthday.IsEnabled = true;
                    txt_Age.IsEnabled = false;
                    cbGender.IsEnabled = true;
                    if (cardTyp == eCredentialsType.IdentifyCard)
                    {
                        txt_Birthday.IsEnabled = false;
                        cbGender.IsEnabled = false;
                        txt_Age.IsEnabled = false;
                    }
                }
            }
            else
            {
                //cbType.IsEnabled = true;
                cbCardType.IsEnabled = true;
                //cbNation.IsEnabled = true;
                txt_Birthday.IsEnabled = true;
                txt_Age.IsEnabled = false;
                cbGender.IsEnabled = true;
                if (cardTyp == eCredentialsType.IdentifyCard)
                {
                    txt_Birthday.IsEnabled = false;
                    cbGender.IsEnabled = false;
                    txt_Age.IsEnabled = false;
                }
            }
        }

        #endregion

        #endregion

        private void txt_Birthday_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (txt_Birthday == null || txt_Birthday.Value == null)
            {
                txt_Age.Text = "";
                return;
            }
            DateTime? ds = txt_Birthday.Value;
            DateTime now = DateTime.Now;
            int age = now.Year - ds.Value.Year;
            if (now.Month < ds.Value.Month || (now.Month == ds.Value.Month && now.Day < ds.Value.Day)) age--;
            txt_Age.Text = age.ToString();           
        }

        private void cbBHQK_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = cbBHQK.SelectedItem;
            if (selectItem == null)
            {
                return;
            }
            EnumStore<eBHQK> eValue = selectItem as EnumStore<eBHQK>;
            if (eValue == null)
            {
                return;
            }
            eBHQK = eValue.Value;
        }
    }
}
