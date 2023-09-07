// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 地域
    /// </summary>
    [Serializable]
    [DataTable("JCSJ_XZQY")]
    public class Zone : NotifyCDObject
    {
        #region Ctor

        static Zone()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eZoneLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_Zone);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 国家级代码86
        /// </summary>
        public const string ZONE_STATE_CODE = "86";

        /// <summary>
        /// 国家级地域长度2
        /// </summary>
        public const int ZONE_STATE_LENGTH = 2;

        /// <summary>
        /// 省级地域长度3
        /// </summary>
        public const int ZONE_PROVICE_LENGTH = 2;

        /// <summary>
        /// 市级地域长度4
        /// </summary>
        public const int ZONE_CITY_LENGTH = 4;

        /// <summary>
        /// 区县级地域长度6
        /// </summary>
        public const int ZONE_COUNTY_LENGTH = 6;

        /// <summary>
        /// 乡镇级地域长度9
        /// </summary>
        public const int ZONE_TOWN_LENGTH = 9;

        /// <summary>
        /// 村级地域长度12
        /// </summary>
        public const int ZONE_VILLAGE_LENGTH = 12;

        /// <summary>
        /// 组级地域长度14
        /// </summary>
        public const int ZONE_GROUP_LENGTH = 14;

        /// <summary>
        /// 新地域全长度
        /// </summary>
        public const int ZONE_FULL_LENGTH = 16;

        /// <summary>
        /// 地域加补位后长度
        /// </summary>
        public const int ZONE_FULLGROUP_LENGTH = 19;

        /// <summary>
        /// 行政区域
        /// </summary>
        public const string YULINTUZONESTRING = "YuLinTuZone";

        #endregion

        #region Properties

        private Guid id;
        private string name;
        private string fullName;
        private string upLevelName;
        private string code;
        private string fullCode;
        private string upLevelCode;
        private eZoneLevel level;
        private string comment;
        private DateTime? createTime;
        private DateTime? lastModifyTime;
        private string createUser;
        private string lastModifyUser;
        private Geometry shape;
        private string aliasCode;
        private string aliasName;

        /// <summary>
        /// 唯一标识
        /// </summary>
        [DataColumn("ID", PrimaryKey = true, Nullable = false)]
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
        /// 地域名称
        /// </summary>
        [DataColumn("DYMC")]
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
        /// 地域名称
        /// </summary>
        [DataColumn("DYQMC")]
        public string FullName
        {
            get { return fullName; }
            set
            {
                fullName = value;
                NotifyPropertyChanged("FullName");
            }
        }

        /// <summary>
        /// 地域别名称
        /// </summary>
        [DataColumn("DYBMC")]
        public string AliasName
        {
            get { return aliasName; }
            set
            {
                aliasName = value;
                NotifyPropertyChanged("AliasName");
            }
        }

        /// <summary>
        /// 地域别编码
        /// </summary>
        [DataColumn("DYBBM")]
        public string AliasCode
        {
            get { return aliasCode; }
            set
            {
                aliasCode = value;
                NotifyPropertyChanged("AliasCode");
            }
        }

        /// <summary>
        /// 上级地域名称
        /// </summary>
        [DataColumn("SJQMC")]
        public string UpLevelName
        {
            get { return upLevelName; }
            set
            {
                upLevelName = value;
                NotifyPropertyChanged("UpLevelName");
            }
        }

        /// <summary>
        /// 地域编码
        /// </summary>
        [DataColumn("DYBM")]
        public string Code
        {
            get { return code; }
            set
            {
                code = value;
                if (code != null)
                    code = code.Trim();
                NotifyPropertyChanged("Code");
            }
        }

        /// <summary>
        /// 地域全编码
        /// </summary>
        [DataColumn("DYQBM")]
        public string FullCode
        {
            get { return fullCode; }
            set
            {
                fullCode = value;
                if (fullCode != null)
                    fullCode = fullCode.Trim();
                NotifyPropertyChanged("FullCode");
            }
        }

        /// <summary>
        /// 上级地域代码
        /// </summary>
        [DataColumn("SJQBM")]
        public string UpLevelCode
        {
            get { return upLevelCode; }
            set
            {
                upLevelCode = value;
                if (upLevelCode != null)
                    upLevelCode = upLevelCode.Trim();
                NotifyPropertyChanged("UpLevelCode");
            }
        }

        /// <summary>
        /// 地域级别
        /// </summary>
        [DataColumn("DYJB")]
        public eZoneLevel Level
        {
            get { return level; }
            set
            {
                level = value;
                NotifyPropertyChanged("Level");
            }
        }

        /// <summary>
        /// 备注信息
        /// </summary>
        [DataColumn("BZXX")]
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
        /// 创建时间
        /// </summary>
        [DataColumn("CJSJ")]
        public DateTime? CreateTime
        {
            get { return createTime; }
            set
            {
                createTime = value;
                NotifyPropertyChanged("CreateTime");
            }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataColumn("XGSJ")]
        public DateTime? LastModifyTime
        {
            get { return lastModifyTime; }
            set
            {
                lastModifyTime = value;
                NotifyPropertyChanged("LastModifyTime");
            }
        }

        /// <summary>
        /// 创建者
        /// </summary>
        [DataColumn("CJZ")]
        public string CreateUser
        {
            get { return createUser; }
            set
            {
                createUser = value;
                NotifyPropertyChanged("CreateUser");
            }
        }

        /// <summary>
        /// 修改者
        /// </summary>
        [DataColumn("XGZ")]
        public string LastModifyUser
        {
            get { return lastModifyUser; }
            set
            {
                lastModifyUser = value;
                NotifyPropertyChanged("LastModifyUser");
            }
        }

        /// <summary>
        /// 空间字段
        /// </summary>
        [DataColumn("Shape")]
        public Geometry Shape
        {
            get { return shape; }
            set
            {
                shape = value;
                NotifyPropertyChanged("Geometry");
            }
        }

        #endregion

        #region Ctor

        public Zone()
        {
            CreateTime = DateTime.Now;
            LastModifyTime = DateTime.Now;
            Level = eZoneLevel.Group;
        }

        #endregion
    }
}
