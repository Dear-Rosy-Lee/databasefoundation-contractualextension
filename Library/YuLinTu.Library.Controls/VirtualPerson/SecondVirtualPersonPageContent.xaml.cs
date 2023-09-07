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

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// SecondVirtualPersonPageContent.xaml 的交互逻辑
    /// </summary>
    public partial class SecondVirtualPersonPageContent : UserControl
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
        private List<EnumStore<eCredentialsType>> cardtypeList;

        /// <summary>
        /// 民族
        /// </summary>
        private List<EnumStore<eNation>> nationList;

        /// <summary>
        /// 承包方类型
        /// </summary>
        private List<EnumStore<eContractorType>> contractorTypeList;

        /// <summary>
        /// 土地承包方式
        /// </summary>
        private List<EnumStore<eConstructMode>> constructModeList;

        /// <summary>
        /// 性别
        /// </summary>
        private List<EnumStore<eGender>> genderList;

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
        private eConstructMode cMode;

        /// <summary>
        /// 当前证件类型
        /// </summary>
        private eGender cGender;

        /// <summary>
        /// 当前证件类型
        /// </summary>
        private eCredentialsType cCard;

        /// <summary>
        /// 承包方
        /// </summary>
        private VirtualPerson tempContracor;

        /// <summary>
        /// 是否添加
        /// </summary>
        private bool isAdd;

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
            set { isAdd = value; }
        }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson Contractor
        {
            get { return contractor; }
            set
            {
                tempContracor = value;
                contractor = value.Clone() as VirtualPerson;
                if (contractor.FamilyExpand != null)
                {
                    FamilyExpand = (contractor.FamilyExpand.Clone() as VirtualPersonExpand);
                }
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

        #endregion

        #region Methods

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public SecondVirtualPersonPageContent()
        {
            InitializeComponent();
            InlitiallCombox();
            cNation = eNation.Han;
            cMode = eConstructMode.Family;
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
                this.DataContext = this;
                string familyNumber = vp.FamilyNumber;
                if (string.IsNullOrEmpty(familyNumber))
                {
                    return;
                }
                List<Person> plist = vp.SharePersonList;
                Person p = (plist == null) ? null : (plist.Find(t => t.Name == vp.Name));
                // cbType.SelectedItem = contractorTypeList.Find(t => t.Value == FamilyExpand.ContractorType);
                cbCardType.SelectedItem = cardtypeList.Find(t => t.Value == vp.CardType);
                if (p != null)
                {
                    cbNation.SelectedItem = nationList.Find(t => t.Value == p.Nation);
                    int age = 0;
                    int.TryParse(txt_Age.Text, out age);
                    txt_Age.Value = age;
                    txt_Birthday.Value = p.Birthday;
                    cbGender.SelectedItem = genderList.Find(t => t.Value == p.Gender);
                }

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
            genderList = EnumStore<eGender>.GetListByType();
            cardtypeList = EnumStore<eCredentialsType>.GetListByType();
            nationList = EnumStore<eNation>.GetListByType();
            contractorTypeList = EnumStore<eContractorType>.GetListByType();
            constructModeList = EnumStore<eConstructMode>.GetListByType();

            cbGender.DisplayMemberPath = "DisplayName";
            cbGender.ItemsSource = genderList;
            cbGender.SelectedIndex = 2;

            cbNation.DisplayMemberPath = "DisplayName";
            cbNation.ItemsSource = nationList;
            cbNation.SelectedIndex = 0;

            cbCardType.DisplayMemberPath = "DisplayName";
            cbCardType.ItemsSource = cardtypeList;
            cbCardType.SelectedIndex = 0;
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

                    cbCardType.SelectedItem = cardtypeList.Find(t => t.Value == eCredentialsType.AgentCard);
                    cbGender.SelectedItem = genderList.Find(t => t.Value == eGender.Unknow);
                    txt_Birthday.Value = null;
                    txt_Age.Text = "";

                    cbCardType.IsEnabled = false;
                    cbGender.IsEnabled = false;
                    cbNation.IsEnabled = false;
                }
                else
                {
                    cbCardType.IsEnabled = true;
                    cbNation.IsEnabled = true;
                    txt_Birthday.IsEnabled = true;
                    txt_Age.IsEnabled = true;
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
                cbCardType.IsEnabled = true;
                cbNation.IsEnabled = true;
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
            if (cCard == eCredentialsType.IdentifyCard)
            {
                int age = p.GetAge();
                p.Age = age > 0 ? age.ToString() : "";
                p.Gender = cGender;
                p.Birthday = ToolICN.GetBirthday(p.ICN);
            }
            else
            {
                p.Age = txt_Age.Text;
                p.Birthday = txt_Birthday.Value;
                if (cbGender.SelectedIndex == 0)
                {
                    p.Gender = eGender.Female;
                }
                else if (cbGender.SelectedIndex == 1)
                {
                    p.Gender = eGender.Male;
                }
                else
                {
                    p.Gender = eGender.Unknow;
                }
            }
            p.Address = contractor.Address;
            p.FamilyNumber = contractor.FamilyNumber;
            p.Nation = cNation;
            p.FamilyID = contractor.ID;
            p.CardType = cCard;
            if (!p.Name.Contains("集体"))
                p.Relationship = "户主";
            FamilyExpand.ConstructMode = cMode;
            contractor.CardType = cCard;
            contractor.SharePersonList = new List<Person>() { p };
            contractor.FamilyExpand = this.FamilyExpand;
            contractor.FamilyExpand.BusinessStatus = eBusinessStatus.End;
        }

        /// <summary>
        /// 修改承包方处理
        /// </summary>
        private void EditProcessPerson()
        {
            List<Person> personList = contractor.SharePersonList;
            Person p = personList.Find(t => t.Name == tempContracor.Name);
            p.ICN = contractor.Number;
            p.Name = contractor.Name;
            p.PostNumber = contractor.PostalNumber;
            p.Comment = contractor.Comment;
            if (cCard == eCredentialsType.IdentifyCard)
            {
                int age = p.GetAge();
                p.Age = age > 0 ? age.ToString() : "";
                p.Gender = cGender;
                p.Birthday = ToolICN.GetBirthday(p.ICN);
            }
            else
            {
                p.Age = txt_Age.Value.ToString();
                p.Birthday = txt_Birthday.Value;
                if (cbGender.SelectedIndex == 0)
                {
                    p.Gender = eGender.Male;
                }
                else if (cbGender.SelectedIndex == 1)
                {
                    p.Gender = eGender.Female;
                }
                else
                {
                    p.Gender = eGender.Unknow;
                }
            }
            p.Address = contractor.Address;
            p.FamilyNumber = contractor.FamilyNumber;
            p.Nation = cNation;
            p.FamilyID = contractor.ID;
            p.CardType = cCard;
            FamilyExpand.ConstructMode = cMode;
            contractor.CardType = cCard;
            contractor.SharePersonList = personList;
            contractor.FamilyExpand = this.FamilyExpand;
            contractor.FamilyExpand.BusinessStatus = eBusinessStatus.End;
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        private void SetBirthInfo(string ICN)
        {
            if (cCard != eCredentialsType.IdentifyCard)
            {
                return;
            }
            if (ICN.Length != 15 && ICN.Length != 18)
            {
                return;
            }
            try
            {
                DateTime birthday = ToolICN.GetBirthdayInNotCheck(ICN);
                txt_Birthday.Value = birthday;

                int age = ToolDateTime.GetAge(birthday);
                txt_Age.Text = age > 0 ? age.ToString() : "";

                eGender gender = ToolICN.GetAllGenderNoCheck(ICN);
                cbGender.SelectedItem = genderList.Find(t => t.Value == gender);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteError(this, "SetBirthInfo(设置信息)", ex.Message + ex.StackTrace);
            }
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



        #endregion

        #region Public
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


        /// <summary>
        /// 检查数据
        /// </summary>
        public string CheckSubmit()
        {
            string errorMsg = string.Empty; ;
            if (contractor == null)
            {
                return errorMsg = VirtualPersonInfo.EntityNull + ",";
            }
            if (string.IsNullOrEmpty(contractor.Name))
            {
                errorMsg += VirtualPersonInfo.VpNameNull + ",";
            }
            if (string.IsNullOrEmpty(contractor.FamilyNumber))
            {
                errorMsg += VirtualPersonInfo.VpCodeNull + ",";
            }
            EnumStore<eCredentialsType> cardType = cbCardType.SelectedItem as EnumStore<eCredentialsType>;
            bool right = ToolICN.Check(contractor.Number);
            if (cardType != null && cardType.Value == eCredentialsType.IdentifyCard && !right)
            {
                errorMsg += VirtualPersonInfo.CardCodeError + ",";
            }
            if (FamilyExpand.ConcordStartTime >= FamilyExpand.ConcordEndTime)
            {
                errorMsg += VirtualPersonInfo.StartBigEnd + ",";
            }
            if (!string.IsNullOrEmpty(contractor.Name) && contractor.Name.Contains("集体") && !isAdd && FamilyExpand.ContractorType != eContractorType.Unit)
            {
                errorMsg += VirtualPersonInfo.CollPersonMustAdd;
            }
            if (string.IsNullOrEmpty(contractor.Number) && !string.IsNullOrEmpty(contractor.Name) && !contractor.Name.Contains("集体"))
            {
                errorMsg += VirtualPersonInfo.PersonNumberMust;
            }
            bool zipCheck = isNumberic(contractor.PostalNumber);
            if (!string.IsNullOrEmpty(contractor.PostalNumber) && !zipCheck)
            {
                errorMsg += SecondAccountInfo.ZipCodeError;
            }
            return errorMsg.TrimEnd(',');
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
        /// 邮编输入框变化
        /// </summary>
        private void txt_ZIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ZIP = txtZip.Text.Trim();
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
        /// 证件类型改变
        /// </summary>
        private void cbCardType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = cbCardType.SelectedItem;
            if (selectItem == null)
            {
                return;
            }
            EnumStore<eCredentialsType> esValue = selectItem as EnumStore<eCredentialsType>;
            if (esValue == null)
            {
                return;
            }
            cCard = esValue.Value;
            if (cCard == eCredentialsType.IdentifyCard)
            {
                txt_ICN.MaxLength = 18;
            }
            else
            {
                txt_ICN.MaxLength = 30;
            }
            SetControlEnable(txt_Name.Text.Trim(), cCard);
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
            EnumStore<eGender> esValue = selectItem as EnumStore<eGender>;
            if (esValue == null)
            {
                return;
            }
            cGender = esValue.Value;
        }

        /// <summary>
        /// 名称
        /// </summary>
        private void txt_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            string nametxt = txt_Name.Text.Trim();
            SetControlEnable(nametxt, cCard);
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
        
    }
}
