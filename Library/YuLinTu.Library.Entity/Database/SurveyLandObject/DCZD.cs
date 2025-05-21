// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 调查宗地
    /// </summary>
    [Serializable]
    [DataTable("DC_ZD")]
    public class DCZD : NotifyCDObject
    {
        /// <summary>
        /// 空间字段
        /// </summary>
        private Geometry shape;

        /// <summary>
        /// 备注
        /// </summary>
        private string comment;

        /// <summary>
        /// 标识
        /// </summary>
        private Guid id;

        /// <summary>
        /// 标识码
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = true)]
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
        /// 空间字段
        /// </summary>
        public Geometry Shape
        {
            get { return shape; }
            set
            {
                shape = value;
                NotifyPropertyChanged("Shape");
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        [DataColumn("notes")]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                NotifyPropertyChanged("Comment");
            }
        }

        public DCZD()
        {
            ID = Guid.NewGuid();
        }


    }
}
