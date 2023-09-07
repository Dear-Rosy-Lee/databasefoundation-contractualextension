/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 公示无异议声明书
    /// </summary>
    public class ExportIdeaBook : AgricultureWordBook
    {
        #region Fields

        private VirtualPerson currentFamily;
        private List<Person> persons;
        private SystemSetDefine sysset = SystemSetDefine.GetIntence();
        #endregion

        #region Propert

        /// <summary>
        /// 打印模式（不输出承包方，让现实承包方手填）
        /// </summary>
        public bool IsPrint { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 公示日期
        /// </summary>
        public DateTime? PubDate { get; set; }

        /// <summary>
        /// 权属名称
        /// </summary>
        public string RightName { get; set; }

        /// <summary>
        /// 地块名称
        /// </summary>
        public string LandAliseName { get; set; }

        /// <summary>
        /// 地域名称
        /// </summary>
        public string ZoneName { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ExportIdeaBook(VirtualPerson family)
        {
            if (family == null || family == currentFamily)
            {
                return;
            }
            currentFamily = family;
            persons = SortSharePerson(family.SharePersonList, currentFamily.Name);//排序共有人，并返回人口集合
        }

        #endregion

        #region Methods
        
        #region Override

        protected override bool OnSetParamValue(object data)
        {
            DateValue = Date;
            base.OnSetParamValue(data);
            SetBookmarkValue("ZoneName", sysset.ExportAddressToTown?AddressExporthHelper.GetNewAddressToTown(ZoneName): ZoneName);
            SetBookmarkValue("ZoneName1", sysset.ExportAddressToTown ? AddressExporthHelper.GetNewAddressToTown(ZoneName) : ZoneName);
            if (PubDate != null)
            {
                SetBookmarkValue("PublicYear", ((DateTime)PubDate).Year.ToString());
                SetBookmarkValue("PublicMonth", ((DateTime)PubDate).Month.ToString());
                SetBookmarkValue("PublicDay", ((DateTime)PubDate).Day.ToString());
            }
            base.Destroyed();
            Disponse();
            return true;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            persons.Clear();
            GC.Collect();
        }

        #endregion

        #endregion
    }
}
