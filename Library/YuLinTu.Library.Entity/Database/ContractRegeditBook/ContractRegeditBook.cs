// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 农村土地承包经营权登记薄
    /// </summary>
    [DataTable("CBJYQ_QZ")]
    [Serializable]
    public class ContractRegeditBook : NotifyCDObject
    {
        #region Fields

        private Guid id;
        private string year;
        private string number;
        private string serialNumber;
        private string regeditNumber;
        private string sendOrganization;
        private DateTime sendDate;
        private string writeOrganization;
        private DateTime writeDate;
        private DateTime printDate;
        private int count;
        private string founder;
        private DateTime? creationTime;
        private string modifier;
        private DateTime? modifiedTime;
        private string zoneCode;
        private string comment;
        private int status;
        private string regeditBookGetted;
        private DateTime regeditBookGettedDate;
        private string getterName;
        private string getterCardType;
        private string getterCardNumber;
        private string contractRegeditBookPerson;
        private DateTime? contractRegeditBookTime;
        private string contractRegeditBookExcursus;
        private bool oldBookReTake;
        private string printBookNumber;
        #endregion

        #region Properties

        /// <summary>
        /// 权证标识号
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public Guid ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }

        /// <summary>
        /// 权证年号
        /// </summary>
        [DataColumn("QZNH")]
        public string Year
        {
            get { return year; }
            set
            {
                year = value.TrimSafe();
                NotifyPropertyChanged("Year");
            }
        }

        /// <summary>
        /// 权证流水号
        /// </summary>
        [DataColumn("QZLSH")]
        public string SerialNumber
        {
            get { return serialNumber; }
            set
            {
                serialNumber = value.TrimSafe();
                NotifyPropertyChanged("serialNumber");
            }
        }

        /// <summary>
        /// 权证登簿人
        /// </summary>
        [DataColumn("QZDBR")]
        public string ContractRegeditBookPerson
        {
            get { return contractRegeditBookPerson; }
            set
            {
                contractRegeditBookPerson = value.TrimSafe();
                NotifyPropertyChanged("ContractRegeditBookPerson");
            }
        }

        /// <summary>
        ///权证登记日期
        /// </summary>
        [DataColumn("QZDJRQ")]
        public DateTime? ContractRegeditBookTime
        {
            get { return contractRegeditBookTime; }
            set
            {
                contractRegeditBookTime = value;
                NotifyPropertyChanged("ContractRegeditBookTime");
            }
        }
        /// <summary>
        /// 权证附记
        /// </summary>
        [DataColumn("QZFJ")]
        public string ContractRegeditBookExcursus
        {
            get { return contractRegeditBookExcursus; }
            set
            {
                contractRegeditBookExcursus = value.TrimSafe();
                NotifyPropertyChanged("ContractRegeditBookExcursus");
            }
        }

        /// <summary>
        /// 权证编码（组级地域+4位流水号）
        /// </summary>
        [DataColumn("QZBM")]
        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                if (string.IsNullOrEmpty(number))
                    return;
                number = number.Trim();
                NotifyPropertyChanged("Number");
            }
        }

        /// <summary>
        ///登记薄编号
        /// </summary>
        [DataColumn("CBJYQZBM")]
        public string RegeditNumber
        {
            get { return regeditNumber; }
            set
            {
                regeditNumber = value;
                if (string.IsNullOrEmpty(regeditNumber))
                    return;
                regeditNumber = regeditNumber.Trim();
                NotifyPropertyChanged("RegeditNumber");
            }
        }

        /// <summary>
        ///发证机关
        /// </summary>
        [DataColumn("FZJG")]
        public string SendOrganization
        {
            get { return sendOrganization; }
            set
            {
                sendOrganization = value.TrimSafe();
                NotifyPropertyChanged("SendOrganization");
            }
        }

        /// <summary>
        ///发证日期
        /// </summary>
        [DataColumn("FZRQ")]
        public DateTime SendDate
        {
            get { return sendDate; }
            set
            {
                sendDate = value;
                NotifyPropertyChanged("SendDate");
            }
        }

        /// <summary>
        ///填证机关
        /// </summary>
        [DataColumn("TZJG")]
        public string WriteOrganization
        {
            get { return writeOrganization; }
            set
            {
                writeOrganization = value.TrimSafe();
                NotifyPropertyChanged("WriteOrganization");
            }
        }

        /// <summary>
        ///填证日期
        /// </summary>
        [DataColumn("TZRQ")]
        public DateTime WriteDate
        {
            get { return writeDate; }
            set
            {
                writeDate = value;
                NotifyPropertyChanged("WriteDate");
            }
        }

        /// <summary>
        ///打证日期
        /// </summary>
        [DataColumn("DZRQ")]
        public DateTime PrintDate
        {
            get { return printDate; }
            set
            {
                printDate = value;
                NotifyPropertyChanged("PrintDate");
            }
        }

        /// <summary>
        ///打证次数
        /// </summary>
        [DataColumn("DZCS")]
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                NotifyPropertyChanged("Count");
            }
        }

        /// <summary>
        ///创建者
        /// </summary>
        [DataColumn("CJZ")]
        public string Founder
        {
            get { return founder; }
            set
            {
                founder = value.TrimSafe();
                NotifyPropertyChanged("Founder");
            }
        }

        /// <summary>
        ///创建时间
        /// </summary>
        [DataColumn("CJSJ")]
        public DateTime? CreationTime
        {
            get { return creationTime; }
            set
            {
                creationTime = value;
                NotifyPropertyChanged("CreationTime");
            }
        }

        /// <summary>
        ///最后修改者
        /// </summary>
        [DataColumn("XGZ")]
        public string Modifier
        {
            get { return modifier; }
            set
            {
                modifier = value.TrimSafe();
                NotifyPropertyChanged("Modifier");
            }
        }

        /// <summary>
        ///最后修改时间
        /// </summary>
        [DataColumn("XGSJ")]
        public DateTime? ModifiedTime
        {
            get { return modifiedTime; }
            set
            {
                modifiedTime = value;
                NotifyPropertyChanged("ModifiedTime");
            }
        }

        /// <summary>
        ///地域代码
        /// </summary>
        [DataColumn("DYDM")]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value.TrimSafe();
                NotifyPropertyChanged("ZoneCode");
            }
        }

        /// <summary>
        ///备注
        /// </summary>
        [DataColumn("BZ")]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value.TrimSafe();
                NotifyPropertyChanged("Comment");
            }
        }

        /// <summary>
        ///状态 申请权证10、审核发证20、30为变更申请、40为变更制证
        /// </summary>
        [DataColumn("ZT")]
        public int Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        /// <summary>
        /// 权证是否领取
        /// </summary>
        [DataColumn("QZSFLY")]
        public string RegeditBookGetted
        {
            get { return regeditBookGetted; }
            set
            {
                regeditBookGetted = value.TrimSafe();
                NotifyPropertyChanged("RegeditBookGetted");
            }
        }

        /// <summary>
        /// 权证领取日期
        /// </summary>
        [DataColumn("QZLQRQ")]
        public DateTime RegeditBookGettedDate
        {
            get { return regeditBookGettedDate; }
            set
            {
                regeditBookGettedDate = value;
                NotifyPropertyChanged("RegeditBookGettedDate");
            }
        }

        /// <summary>
        /// 权证领取人姓名
        /// </summary>
        [DataColumn("QZLQRXM")]
        public string GetterName
        {
            get { return getterName; }
            set
            {
                getterName = value.TrimSafe();
                NotifyPropertyChanged("GetterName");
            }
        }

        /// <summary>
        /// 权证领取人证件类型
        /// </summary>
        [DataColumn("QZLQRZJLX")]
        public string GetterCardType
        {
            get { return getterCardType; }
            set
            {
                getterCardType = value.TrimSafe();
                NotifyPropertyChanged("GetterCardType");
            }
        }

        /// <summary>
        /// 权证领取人证件号码
        /// </summary>
        [DataColumn("QZLQRZJHM")]
        public string GetterCardNumber
        {
            get { return getterCardNumber; }
            set
            {
                getterCardNumber = value.TrimSafe();
                NotifyPropertyChanged("GetterCardNumber");
            }
        }

        /// <summary>
        /// 旧证是否收回
        /// </summary>
        [DataColumn("JZSFSH")]
        public bool OldBookReTake
        {
            get { return oldBookReTake; }
            set
            {
                oldBookReTake = value;
                NotifyPropertyChanged("OldBookReTake");
            }
        }

        /// <summary>
        /// 印刷证书编号
        /// </summary>
        [DataColumn("YSZSBH")]
        public string PrintBookNumber
        {
            get { return printBookNumber; }
            set
            {
                printBookNumber = value.TrimSafe();
                NotifyPropertyChanged("PrintBookNumber");
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractRegeditBook()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            SendDate = DateTime.Now;
            WriteDate = DateTime.Now;
            PrintDate = DateTime.Now;
            regeditBookGettedDate = DateTime.Now;
            oldBookReTake = false;
        }

        #endregion
    }
}