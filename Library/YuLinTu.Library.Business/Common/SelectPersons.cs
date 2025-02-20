/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 绑定选中承包方类
    /// </summary>
    public class SelectPerson : NotifyCDObject
    {
        #region Field

        private Guid id;
        private string name;
        private eVirtualPersonStatus status;

        #endregion

        #region Properties

        /// <summary>
        /// 标识
        /// </summary>
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
        /// 承包方名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 承包方状态
        /// </summary>
        public eVirtualPersonStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        #endregion
    }
}
