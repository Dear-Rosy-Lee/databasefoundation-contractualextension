/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 户地实体
    /// </summary>
    [Serializable]
    public class LandFamily
    {
        #region Fields

        private int number;//编号
        private int personCount;//家庭成员数
        private bool isContrac;//是否签订合同
        private bool issueCertificate;//是否发证
        private VirtualPerson currentFamily;//当前承包方
        private PersonCollection persons;
        private ContractConcord concord;
        private ContractRegeditBook regeditBook;
        private List<ContractLand> landCollection = new List<ContractLand>();//地块集合
        private List<ContractLand_Del> delLandCollection = new List<ContractLand_Del>();//删除地块集合

        #endregion

        #region Properties

        /// <summary>
        /// 证书
        /// </summary>
        public ContractRegeditBook RegeditBook
        {
            get { return regeditBook; }
            set { regeditBook = value; }
        }

        /// <summary>
        /// 合同
        /// </summary>
        public ContractConcord Concord
        {
            get { return concord; }
            set { concord = value; }
        }

        /// <summary>
        /// 家庭成员集合（含户主）
        /// </summary>
        public PersonCollection Persons
        {
            get { return persons; }
            set { persons = value; }
        }

        /// <summary>
        /// 二轮延包人口集合
        /// </summary>
        public PersonCollection TablePersons { get; set; }

        /// <summary>
        /// 临时承包人口集合
        /// </summary>
        public List<Person> TemporaryPersons { get; set; }

        /// <summary>
        /// 家庭成员个数
        /// </summary>
        public int PersonCount
        {
            get { return personCount; }
            set { personCount = value; }
        }

        /// <summary>
        /// 二轮家庭成员个数
        /// </summary>
        public int TablePersonCount { get; set; }

        /// <summary>
        /// 地块数
        /// </summary>
        public int LandCount { get; set; }

        /// <summary>
        /// 二轮地块数
        /// </summary>
        public int TableLandCount { get; set; }

        /// <summary>
        /// 总面积
        /// </summary>
        public double CountArea { get; set; }

        /// <summary>
        /// 当前索引号
        /// </summary>
        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        /// <summary>
        /// 是否签订合同
        /// </summary>
        public bool IsContrac
        {
            get { return isContrac; }
            set { isContrac = value; }
        }

        /// <summary>
        /// 是否发证
        /// </summary>
        public bool IssueCertificate
        {
            get { return issueCertificate; }
            set { issueCertificate = value; }
        }

        /// <summary>
        /// 当前户
        /// </summary>
        public VirtualPerson CurrentFamily
        {
            get { return currentFamily; }
            set { currentFamily = value; }
        }

        /// <summary>
        /// 二轮家庭情况
        /// </summary>
        public VirtualPerson TableFamily { get; set; }

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> LandCollection
        {
            get { return landCollection; }
            set { landCollection = value; }
        }

        /// <summary>
        /// 二轮台账地块信息
        /// </summary>
        public SecondTableLandCollection TableLandCollection { get; set; }

        /// <summary>
        /// 删除地块集合
        /// </summary>
        public List<ContractLand_Del> DelLandCollection
        {
            get { return delLandCollection; }
            set { delLandCollection = value; }
        }
        /// <summary>
        /// 删除人口集合
        /// </summary>
        public List<Person> DelPersons { get; set; }
        #endregion

        #region Ctor

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public LandFamily()
        {
            currentFamily = new VirtualPerson();
            persons = new PersonCollection();
            TablePersons = new PersonCollection();
            concord = new ContractConcord();
            regeditBook = new ContractRegeditBook();
            landCollection = new List<ContractLand>();
            TableFamily = new VirtualPerson();
            TableLandCollection = new SecondTableLandCollection();
            TemporaryPersons = new List<Person>();
        }

        #endregion
    }
}
