/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 共有人信息页面
    /// </summary>
    public partial class PersonInfoPage : InfoPageBase
    {
        #region Fields

        private FamilyOtherDefine _config;

        /// <summary>
        /// 错误图片
        /// </summary>
        private BitmapImage errorImg = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/错误16_2.png"));

        /// <summary>
        /// 正确图片
        /// </summary>
        private BitmapImage rightImg = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/是16.png"));

        /// <summary>
        /// 人员信息
        /// </summary>
        private Person person;

        /// <summary>
        /// 是否添加
        /// </summary>
        private bool isAdd;

        /// <summary>
        /// 民族
        /// </summary>
        private List<EnumStore<eNation>> nationList;

        /// <summary>
        /// 证件类型
        /// </summary>
        private KeyValueList<string, string> cardtypeList;

        /// <summary>
        /// 性别
        /// </summary>
        private KeyValueList<string, string> genderList;

        /// <summary>
        /// 家庭关系
        /// </summary>
        private List<string> relationList;

        /// <summary>
        /// 当前选择项
        /// </summary>
        private VirtualPerson item = null;

        /// <summary>
        /// 民族
        /// </summary>
        private eNation cNation;

        /// <summary>
        /// 证件类型
        /// </summary>
        private string cCardType;

        /// <summary>
        /// 证件类型
        /// </summary>
        private eCredentialsType eCardType;

        private bool isLock;

        #endregion

        #region Propertys

        /// <summary>
        /// 人员信息
        /// </summary>
        public Person Person
        {
            get { return person; }
            set
            {
                person = (Person)(value).Clone();
                txt_Age.Text = person.Age;
                txt_Birthday.Value = person.Birthday;
                SetCombobox(person);
                this.DataContext = person;
                //cbRelationship.SelectedValue = Person.Relationship;
            }
        }

        /// <summary>
        /// 当前地域承包方集合
        /// </summary>
        public List<VirtualPerson> PersonItems { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// 业务
        /// </summary>
        public VirtualPersonBusiness Business { get; set; }

        /// <summary>
        /// 承包方其它设置
        /// </summary>
        public FamilyOtherDefine OtherDefine { get; set; }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson Virtualperson
        {
            get { return item; }
            set { item = value; }
        }

        /// <summary>
        /// 共有人被锁定
        /// </summary>
        public bool IsLock
        {
            get { return isLock; }
            set
            {
                isLock = value;
                if (isLock)
                {
                    var enmu = (svInfo.Content as System.Windows.Controls.Grid).Children.GetEnumerator();
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

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="zone">地域</param>
        /// <param name="editvp">是否为编辑承包方</param>
        /// <param name="per">实体</param>
        public PersonInfoPage(VirtualPerson currentVp = null, bool isAdd = false)
        {
            InitializeComponent();
            this.Virtualperson = currentVp;
            InitiallControl();
            this.isAdd = isAdd;
            Confirm += PersonInfoPage_Confirm;
            //this.DataContext = this;

            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<FamilyOtherDefine>();
            var section = profile.GetSection<FamilyOtherDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            _config = (section.Settings as FamilyOtherDefine);   //得到经反序列化后的对象
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
                cardtypeList = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.ZJLX);
                genderList = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.XB);
            }
            catch (Exception)
            {
                cardtypeList = new KeyValueList<string, string>();
                genderList = new KeyValueList<string, string>();
            }
            nationList = EnumStore<eNation>.GetListByType();
            relationList = FamilyRelationShip.AllRelation();
            cbGender.DisplayMemberPath = "Value";
            cbGender.ItemsSource = genderList;
            cbGender.SelectedIndex = 0;

            cbNation.DisplayMemberPath = "DisplayName";
            cbNation.ItemsSource = nationList;
            cbNation.SelectedIndex = 0;

            cbCardType.DisplayMemberPath = "Value";
            cbCardType.ItemsSource = cardtypeList;
            cbCardType.SelectedIndex = 0;


            cbRelationship.ItemsSource = relationList;
            cbRelationship.SelectedIndex = 0;

            cbShare.ItemsSource = new List<string> { "是", "否" };
            cbShare.SelectedIndex = 0;

            if (Virtualperson.Status == eVirtualPersonStatus.Lock)
                btnAdd.IsEnabled = false;
        }

        /// <summary>
        /// 设置下拉控件
        /// </summary>
        private void SetCombobox(Person person)
        {
            if (person == null)
            {
                return;
            }
            if (!isAdd)
            {
                cbGender.SelectedItem = genderList.Find(t => t.Key == ((int)person.Gender).ToString());
                cbCardType.SelectedItem = cardtypeList.Find(t => t.Key == ((int)person.CardType).ToString());

                cbNation.SelectedItem = nationList.Find(t => t.Value == person.Nation);
                cbShare.SelectedValue = person.IsSharedLand;
                cbRelationship.SelectedValue = Person.Relationship;
            }
            //陈泽林 20161019 如果是共有人是户主，则是否共有人不允许修改
            string vnumber = Virtualperson.Number == null ? "" : Virtualperson.Number.Trim();
            string pnumber = Person.ICN == null ? "" : person.ICN.Trim();
            string vname = Virtualperson.Name == null ? "" : Virtualperson.Name.Trim();
            string pname = Person.Name == null ? "" : Person.Name.Trim();
            if (vname == pname && pnumber == vnumber)
            {
                cbShare.IsEnabled = false;
                cbRelationship.IsEnabled = false;
                //如果是单位，则不允许修改证件类型 
                if (Virtualperson.FamilyExpand.ContractorType == eContractorType.Unit)
                    cbCardType.IsEnabled = false;
            }

        }

        /// <summary>
        /// 提交数据
        /// </summary>
        private void PersonInfoPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            Result = false;
            person.Nation = cNation;
            person.CardType = eCardType;
            if (isAdd)
            {
                item.AddSharePerson(person);
            }
            else
            {
                List<Person> list = item.SharePersonList;
                var ps = list.FindAll(t => t.ID == person.ID);
                int index = list.FindIndex(t => t.ID == person.ID);
                var p = ps.Find(t => t.Name == person.Name && t.Relationship == person.Relationship);
                p = p == null ? ps[0] : p;
                if (p.Relationship != "01" || p.Relationship != "02")
                {
                    p.ID = Guid.NewGuid();
                }
                list.Remove(p);
                list.Insert(index, person);
                item.SharePersonList = list;
                //if (person.ID == item.ID)
                //{
                //    item.Name = person.Name;
                //    item.Number = person.ICN;
                //    item.CardType = cCardType;
                //}
                //不应该用person,person是改过之后的共有人，有可能会更改名称或者证件号码
                if (p.ICN == item.Number && p.Name == item.Name)
                {
                    item.Name = person.Name;
                    item.Number = person.ICN;
                    item.CardType = eCardType;
                }
            }
            Business.Update(item);
            e.Parameter = true;
            Result = true;
        }

        #endregion

        #region Events

        /// <summary>
        /// 提交数据
        /// </summary> 
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            item = null;
            if (PersonItems == null || PersonItems.Count == 0 || Business == null)
            {
                return;
            }
            var cardType = cbCardType.SelectedItem as KeyValue<string, string>;
            bool right = ToolICN.Check(person.ICN);
            string errorMsg = string.Empty;
            if (string.IsNullOrEmpty(person.Name))
            {
                errorMsg += VirtualPersonInfo.NameEmpty;
                //ShowBox(VirtualPersonInfo.SharePersonProc, VirtualPersonInfo.NameEmpty);
                //return;
            }
            if (_config.IsCheckCardNumber)
            {
                if (string.IsNullOrEmpty(person.ICN))
                {
                    errorMsg += VirtualPersonInfo.CardCodeEmpty;
                }
                else if (cardType != null && cardType.Key == ((int)eCredentialsType.IdentifyCard).ToString() && !right)
                {
                    errorMsg += VirtualPersonInfo.CardCodeError;
                }
            }

            //if (!_config.IsPromiseCardNumberNull)
            //{
            //    if (string.IsNullOrEmpty(person.ICN))
            //    {
            //        errorMsg += VirtualPersonInfo.CardCodeEmpty;
            //    }
            //}

            if (OtherDefine.HasFamilyRelation && string.IsNullOrEmpty(person.Relationship))     //检查共有人家庭关系是否填写
            {
                errorMsg += VirtualPersonInfo.RelationShipMust;
                //ShowBox(VirtualPersonInfo.SharePersonProc, VirtualPersonInfo.RelationShipMust);
                //return;
            }
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ShowBox(VirtualPersonInfo.SharePersonProc, errorMsg);
                return;
            }
            bool isRepeat = false;
            item = PersonItems.FirstOrDefault(t => t.ID == person.FamilyID);
            if (item == null)
            {
                return;
            }
            bool exit = item.SharePersonList.Any(t => t.Name == person.Name && t.ID != person.ID);
            if (exit)
            {
                isRepeat = true;
            }
            foreach (var pi in PersonItems)
            {
                if (pi.ID == item.ID)
                {
                    continue;
                }
                bool result = pi.SharePersonList.Any(t => t.Name == person.Name && t.ICN == person.ICN);
                if (result)
                {
                    isRepeat = true;
                    break;
                }
            }
            if (isRepeat)
            {
                ShowBox(VirtualPersonInfo.SharePersonProc, VirtualPersonInfo.PersonInExist);
                return;
            }
            if (eCardType != eCredentialsType.IdentifyCard)
            {
                person.Age = txt_Age.Text;
                person.Birthday = txt_Birthday.Value;
            }
            if (cbGender.SelectedIndex == 0)
            {
                person.Gender = eGender.Male;
            }
            else if (cbGender.SelectedIndex == 1)
            {
                person.Gender = eGender.Female;
            }
            else
            {
                person.Gender = eGender.Unknow;
            }
            ConfirmAsync();
        }

        /// <summary>
        /// 消息框
        /// </summary>
        private void ShowBox(string title, string msg)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }

        /// <summary>
        /// 身份证输入框值变化
        /// </summary>
        private void txt_ICN_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ICN = txt_ICN.Text.Trim();
            person.ICN = ICN;
            var type = cbCardType.SelectedItem as KeyValue<string, string>;
            if (type.Key == ((int)eCredentialsType.IdentifyCard).ToString())
            {
                if (string.IsNullOrEmpty(ICN))
                {
                    ZjImg.Source = errorImg;
                    ZjImg.ToolTip = "证件号码为空!";
                    txt_Age.Text = string.Empty;
                    txt_Birthday.Text = string.Empty;
                }
                else
                {
                    if (ICN.Length != 15 && ICN.Length != 18)
                    {
                        ZjImg.Source = errorImg;
                        ZjImg.ToolTip = "证件号码长度不正确!";
                    }
                    else if (!ToolICN.Check(ICN))
                    {
                        ZjImg.Source = errorImg;
                        ZjImg.ToolTip = "不符合身份号码验证规则!";
                    }
                    else
                    {
                        ZjImg.Source = rightImg;
                        ZjImg.ToolTip = "身份证号码正确!";
                    }
                }
                SetBirthInfo();
            }
            else
            {
                person.Age = txt_Age.Text;
                person.Birthday = txt_Birthday.Value;
                if (cbGender.SelectedIndex == 0)
                {
                    person.Gender = eGender.Female;
                }
                else if (cbGender.SelectedIndex == 1)
                {
                    person.Gender = eGender.Male;
                }
                else
                {
                    person.Gender = eGender.Unknow;
                }
            }
        }

        /// <summary>
        /// 姓名输入框值变化
        /// </summary>
        //private void txt_Name_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(txt_Name.Text.Trim()))
        //    {
        //        XMImg.Source = errorImg;
        //        XMImg.ToolTip = "姓名不能为空!";
        //    }
        //    else
        //    {
        //        XMImg.Source = rightImg;
        //        XMImg.ToolTip = "姓名正确!";
        //    }
        //}

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="ICN"></param>
        private void SetBirthInfo()
        {
            if (person.ICN.Length != 15 && person.ICN.Length != 18)
            {
                return;
            }
            try
            {
                DateTime birthday = ToolICN.GetBirthdayInNotCheck(person.ICN);
                person.Birthday = birthday;
                int age = ToolDateTime.GetAge(person.Birthday);
                person.Age = age > 0 ? age.ToString() : "";
                txt_Age.Text = person.Age;
                txt_Birthday.Value = person.Birthday;
                person.Gender = ToolICN.GetAllGenderNoCheck(person.ICN);
                cbGender.SelectedItem = genderList.Find(t => t.Key == ((int)person.Gender).ToString());
            }
            catch { }
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
        /// 证件类型
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
            cCardType = esValue.Key;
            /* 修改于2016/8/30 获取证件类型枚举值 */
            Enum.TryParse<eCredentialsType>(cCardType, out eCardType);
            if (cCardType == ((int)eCredentialsType.IdentifyCard).ToString())
            {
                txt_ICN.MaxLength = 18;
            }
            else
            {
                txt_ICN.MaxLength = 30;
            }
            SetControlEnable();
        }

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        private void SetControlEnable()
        {
            cbNation.IsEnabled = true;
            txt_Birthday.IsEnabled = true;
            txt_Age.IsEnabled = false;
            cbGender.IsEnabled = true;
            if (eCardType == eCredentialsType.IdentifyCard)
            {
                txt_Birthday.IsEnabled = false;
                cbGender.IsEnabled = false;
                txt_Age.IsEnabled = false;
            }
        }

        #endregion

        #endregion
        /// <summary>
        /// 出身日期发生变化后，年龄跟着变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void txt_Birthday_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if (txt_Birthday == null || txt_Birthday.Value == null)
            //{
            //    txt_Age.Text = "";
            //    return;
            //}
            //DateTime? ds = txt_Birthday.Value;
            //DateTime now = DateTime.Now;
            //int age = now.Year - ds.Value.Year;
            //if (now.Month < ds.Value.Month || (now.Month == ds.Value.Month && now.Day < ds.Value.Day)) age--;
            //txt_Age.Text = age.ToString();
        }
    }
}
