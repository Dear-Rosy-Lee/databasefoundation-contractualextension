/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 二轮台账调查表
    /// </summary>
    [Serializable]
    public class SecondTableLandFamily
    {

        #region Fields

        private int number;//编号
        private int personCount;//家庭成员数
        private VirtualPerson currentFamily;//当前承包方
        private List<Person> persons;//共有人

        #endregion

        #region Propertys

        /// <summary>
        /// 二轮台账地块
        /// </summary>
        public List<SecondTableLand> LandCollection
        {
            get;
            set;
        }

        /// <summary>
        /// 家庭成员集合（共有人）
        /// </summary>
        public List<Person> Persons
        {
            get { return persons; }
            set { persons = value; }
        }

        /// <summary>
        /// 家庭成员个数
        /// </summary>
        public int PersonCount
        {
            get { return personCount; }
            set { personCount = value; }
        }

        /// <summary>
        /// 地块数
        /// </summary>
        public int LandCount { get; set; }

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
        /// 当前户
        /// </summary>
        public VirtualPerson CurrentFamily
        {
            get { return currentFamily; }
            set { currentFamily = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public SecondTableLandFamily()
        {
            currentFamily = new VirtualPerson();
            persons = new List<Person>();
        }

        #endregion
    }
}
