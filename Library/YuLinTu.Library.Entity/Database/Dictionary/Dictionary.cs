/*
 * (C) 2015 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 创建实体类——字典类
    /// </summary>
    [DataTable("JCSJ_SJZD")]
    [Serializable]
    public class Dictionary : NotifyCDObject
    {
        #region Fields

        /// <summary>
        /// 私有变量—字典标识号
        /// </summary>
        private Guid id;

        /// <summary>
        ///  私有变量—分组名称
        /// </summary>
        private string groupName;

        /// <summary>
        ///  私有变量—分组码
        /// </summary>
        private string groupCode;

        /// <summary>
        ///  私有变量—名称
        /// </summary>
        private string name;

        /// <summary>
        ///  私有变量—编码
        /// </summary>
        private string code;

        /// <summary>
        ///  私有变量—别名
        /// </summary>
        private string aliseName;

        /// <summary>
        ///  私有变量—说明
        /// </summary>
        private string comment;

        /// <summary>
        ///  私有变量—创建者
        /// </summary>
        private string founder;

        /// <summary>
        ///  私有变量—修改者
        /// </summary>
        private string modifier;

        /// <summary>
        ///  私有变量—创建时间
        /// </summary>
        private DateTime creationTime;

        /// <summary>
        ///  私有变量—修改时间
        /// </summary>
        private DateTime modifierTime;

        #endregion

        #region Ctor

        /// <summary>
        /// 初始化数据字典ID编号
        /// </summary>
        public Dictionary()
        {
            ID = Guid.NewGuid();
            Founder = "**";
            CreationTime = DateTime.Today;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 字典标识号
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = true)]
        [Enabled(false)]
        [DisplayLanguage("标识")]
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
        /// 分组码
        /// </summary>
        [DataColumn("FZM", Size = 10, Nullable = false)]
        [DisplayLanguage("分组码")]
        public string GroupCode
        {
            get { return groupCode; }
            set
            {
                groupCode = value;
                NotifyPropertyChanged("GroupCode");
            }
        }

        /// <summary>
        /// 分组名称
        /// </summary>
        [DataColumn("FZMC", Size = 20, Nullable = false)]
        [DisplayLanguage("分组名称")]
        public string GroupName
        {
            get { return groupName; }
            set
            {
                groupName = value;
                NotifyPropertyChanged("GroupName");
            }
        }

        /// <summary>
        /// 具体类型代码
        /// </summary>
        [DataColumn("LXBM", Size = 14)]
        [DisplayLanguage("编码")]
        public string Code
        {
            get { return code; }
            set
            {
                code = value;
                NotifyPropertyChanged("Code");
            }
        }

        /// <summary>
        /// 具体类型名称
        /// </summary>
        [DataColumn("MC", Size = 50)]
        [DisplayLanguage("名称")]
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
        /// 具体类型别名
        /// </summary>
        [DataColumn("BM", Size = 50)]
        [DisplayLanguage("别名")]
        public string AliseName
        {
            get { return aliseName; }
            set
            {
                aliseName = value;
                NotifyPropertyChanged("AliseName");
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        [DataColumn("BZXX", Size = 100)]
        [DisplayLanguage("说明")]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                NotifyPropertyChanged("Comment");
            }
        }

        /// <summary>
        /// 创建者
        /// </summary>
        [DataColumn("CJZ")]
        [Enabled(false)]
        [DisplayLanguage("创建者")]
        public string Founder
        {
            get { return founder; }
            set
            {
                founder = value;
                NotifyPropertyChanged("Founder");
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataColumn("CJSJ")]
        [Enabled(false)]
        [DisplayLanguage("创建时间")]
        public DateTime CreationTime
        {
            get { return creationTime; }
            set
            {
                creationTime = value;
                NotifyPropertyChanged("CreationTime");
            }
        }

        /// <summary>
        /// 修改者
        /// </summary>
        [DataColumn("XGZ")]
        [Enabled(false)]
        [DisplayLanguage("修改者")]
        public string Modifier
        {
            get { return modifier; }
            set
            {
                modifier = value;
                NotifyPropertyChanged("Modifier");
            }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataColumn("XGSJ")]
        [Enabled(false)]
        [DisplayLanguage("修改时间")]
        public DateTime ModifierTime
        {
            get { return modifierTime; }
            set
            {
                modifierTime = value;
                NotifyPropertyChanged("ModifierTime");
            }
        }

        #endregion
    }
}