/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账承包方信息
    /// </summary>
    [Serializable]
    public class ContractAccountLandFamily
    {
        #region Fields

        private VirtualPerson currentFamily;//承包方
        private List<Person> persons;// 共有人集合
        private List<ContractLand> landCollection; //地块集合
        private int personCount; //共有人个数
        private int landCount;//地块数目

        #endregion

        #region Properties

        public List<Person> Persons
        {
            get { return persons; }
            set { persons = value; }
        }

        public List<ContractLand> LandCollection
        {
            get { return landCollection ;} 
            set { landCollection = value; }
        }

        public int PersonCount
        {
            get { return personCount; }
            set { personCount = value; }
        }

        public int LandCount
        {
            get { return landCount;}
            set {landCount =value;}
        }

        public VirtualPerson CurrentFamily
        {
            get { return currentFamily; }
            set { currentFamily = value; }
        }

        #endregion

        #region Ctor

        public ContractAccountLandFamily()
        {
            currentFamily = new VirtualPerson();
            persons = new List<Person>();
            landCollection = new List<ContractLand>();

        }
        #endregion 
    }
}
