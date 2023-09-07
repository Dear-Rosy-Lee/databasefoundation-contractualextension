// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 农村土地承包经营权申请书
    /// </summary>
    [DataTable("CBJYQ_SQS")]
    [Serializable]
    public class ContractRequireTable : NotifyCDObject
    {
        #region Fields

        private Guid id;
        private string path;
        private string person;
        private DateTime? date;
        private string reciveTable;
        private string number;
        private string year;
        private string founder;
        private DateTime? creationTime;
        private string modifier;
        private DateTime? modifiedTime;
        private string comment;
        private string zoneCode;


        #endregion

        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public Guid ID
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("ID"); }
        }

        /// <summary>
        ///农村土地承包经营经申请书路径
        /// </summary>
        [DataColumn("SQBLJ")]
        public string Path
        {
            get { return path; }
            set { path = value; NotifyPropertyChanged("Path"); }
        }

        /// <summary>
        ///收件人
        /// </summary>
        [DataColumn("SJR")]
        public string Person
        {
            get { return person; }
            set { person = value; NotifyPropertyChanged("Person"); }
        }

        /// <summary>
        ///收件日期
        /// </summary>
        [DataColumn("SJRQ")]
        public DateTime? Date
        {
            get { return date; }
            set { date = value; NotifyPropertyChanged("Date"); }
        }

        /// <summary>
        ///收件单
        /// </summary>
        [DataColumn("SJD")]
        public string ReciveTable
        {
            get { return reciveTable; }
            set { reciveTable = value; NotifyPropertyChanged("ReciveTable"); }
        }

        /// <summary>
        ///申请书编号
        /// </summary>
        [DataColumn("SQSBH")]
        public string Number
        {
            get { return number; }
            set { number = value; NotifyPropertyChanged("Number"); }
        }

        /// <summary>
        ///申请年号
        /// </summary>
        [DataColumn("SQSNH")]
        public string Year
        {
            get { return year; }
            set { year = value; NotifyPropertyChanged("Year"); }
        }

        /// <summary>
        ///创建者
        /// </summary>
        [DataColumn("CJZ")]
        public string Founder
        {
            get { return founder; }
            set { founder = value; NotifyPropertyChanged("Founder"); }
        }

        /// <summary>
        ///创建时间
        /// </summary>
        [DataColumn("CJSJ")]
        public DateTime? CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; NotifyPropertyChanged("CreationTime"); }
        }

        /// <summary>
        ///最后修改者
        /// </summary>
        [DataColumn("ZHXGSZ")]
        public string Modifier
        {
            get { return modifier; }
            set { modifier = value; NotifyPropertyChanged("Modifier"); }
        }

        /// <summary>
        ///
        /// </summary>
        [DataColumn("ZHXGSJ")]
        public DateTime? ModifiedTime
        {
            get { return modifiedTime; }
            set { modifiedTime = value; NotifyPropertyChanged("ModifiedTime"); }
        }

        /// <summary>
        ///备注
        /// </summary>
        [DataColumn("BZ")]
        public string Comment
        {
            get { return comment; }
            set { comment = value; NotifyPropertyChanged("Comment"); }
        }

        /// <summary>
        ///地域代码
        /// </summary>
        [DataColumn("DYBM")]
        public string ZoneCode
        {
            get { return zoneCode; }
            set { zoneCode = value; NotifyPropertyChanged("ZoneCode"); }
        }

        #endregion

        #region Ctor

        public ContractRequireTable()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            Date = DateTime.Now;
        }

        #endregion
    }
}