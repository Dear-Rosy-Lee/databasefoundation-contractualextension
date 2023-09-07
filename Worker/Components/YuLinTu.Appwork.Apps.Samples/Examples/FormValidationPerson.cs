using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class FormValidationPerson : NotifyInfoCDObject
    {
        #region Properties

        [Required(AllowEmptyStrings = false, ErrorMessage = "名称不能为空")]
        [StringLength(10, ErrorMessage = "名称长度不能超过10")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("Name"); }
        }
        private string _Name = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "电话号码必填")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "电话号码格式错误")]
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; NotifyPropertyChanged("Phone"); }
        }
        private string _Phone;

        public string Address
        {
            get { return _Address; }
            set { _Address = value; NotifyPropertyChanged("Address"); }
        }
        private string _Address;

        [Required(AllowEmptyStrings = false, ErrorMessage = "身份证号码必填")]
        [IcnValidation(ErrorMessage = "身份证号码格式错误")]
        public string Icn
        {
            get { return _Icn; }
            set { _Icn = value; NotifyPropertyChanged("Icn"); }
        }
        private string _Icn = null;

        [Range(typeof(DateTime), "1900/1/1", "2016/1/1", ErrorMessage = "生日不能超出范围")]
        public DateTime Birthday
        {
            get { return _Birthday; }
            set { _Birthday = value; NotifyPropertyChanged("Birthday"); }
        }
        private DateTime _Birthday = new DateTime(1900, 1, 1);

        public eGender Gender
        {
            get { return _Gender; }
            set { _Gender = value; NotifyPropertyChanged("Gender"); }
        }
        private eGender _Gender = eGender.Male;

        [Range(0, 300, ErrorMessage = "高度不能超出范围(0 ~ 300)")]
        public double Height
        {
            get { return _Height; }
            set { _Height = value; NotifyPropertyChanged("Height"); }
        }
        private double _Height = -1;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("_Description"); }
        }
        private string _Description = null;

        protected override string OnValidate(string columnName)
        {
            switch (columnName)
            {
                case "Gender":
                    return ValidateGender();
                case "Birthday":
                    return ValidateBirthday();
                case "Icn":
                    return ValidateICN();
                default:
                    break;
            }
            return null;
        }

        private string ValidateBirthday()
        {
            if (Icn.TrimSafe().IsNullOrBlank())
                return null;

            try
            {
                IcnHelper.Check(Icn);
                var birthday = IcnHelper.GetBirthday(Icn);
                if (birthday.Date != Birthday.Date)
                    return "出生日期与身份证号码中的出生日期信息不匹配";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        private string ValidateICN()
        {
            if (Icn.TrimSafe().IsNullOrBlank())
                return "身份证号码必填";

            try
            {
                var gender = IcnHelper.GetGender(Icn);
                if (gender != Gender)
                    return "性别与身份证号码中的性别信息不匹配";

                var birthday = IcnHelper.GetBirthday(Icn);
                if (birthday.Date != Birthday.Date)
                    return "出生日期与身份证号码中的出生日期信息不匹配";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        private string ValidateGender()
        {
            if (Icn.TrimSafe().IsNullOrBlank())
                return null;

            try
            {
                IcnHelper.Check(Icn);
                var gender = IcnHelper.GetGender(Icn);
                if (gender != Gender)
                    return "性别与身份证号码中的性别信息不匹配";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }


        #endregion
    }
}
