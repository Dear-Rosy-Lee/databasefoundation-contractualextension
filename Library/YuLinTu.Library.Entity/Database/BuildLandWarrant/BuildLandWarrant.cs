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
    /// 集体建设用地第字号表
    /// </summary>
    [DataTable("BuildLandWarrant")]
    [Serializable]
    public class BuildLandWarrant : NameableObject
    {
        #region Field

        private string _ZoneCode = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        ///
        /// </summary>
        [DataColumn("ID", ColumnType = eDataType.Guid)]
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        ///前标示
        /// </summary>
        [DataColumn("Prefix", ColumnType = eDataType.String)]
        public string Prefix { get; set; }

        /// <summary>
        ///年号
        /// </summary>
        [DataColumn("Year", ColumnType = eDataType.Int32)]
        public int? Year { get; set; }

        /// <summary>
        ///权证编号
        /// </summary>
        [DataColumn("WarrantNumber", ColumnType = eDataType.String)]
        public string WarrantNumber { get; set; }

        /// <summary>
        /// 打印次数
        /// </summary>
        [DataColumn("PrintCount", ColumnType = eDataType.Int32)]
        public int PrintCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataColumn("Founder", ColumnType = eDataType.String)]
        public string Founder { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataColumn("CreationTime", ColumnType = eDataType.DateTime)]
        public DateTime? CreationTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataColumn("Modifier", ColumnType = eDataType.String)]
        public string Modifier { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataColumn("ModifiedTime", ColumnType = eDataType.DateTime)]
        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataColumn("Comment", ColumnType = eDataType.String)]
        public string Comment { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataColumn("ZoneCode", ColumnType = eDataType.String)]
        public string ZoneCode
        {
            get { return _ZoneCode; }
            set
            {
                _ZoneCode = value;
                if (!string.IsNullOrEmpty(_ZoneCode))
                    _ZoneCode = _ZoneCode.Trim();
            }
        }

        #endregion

        #region Ctor

        public BuildLandWarrant()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
        }

        #endregion

        #region Methods

        public string GetWarrantNumber()
        {
            if (string.IsNullOrEmpty(WarrantNumber))
                return "";

            if (WarrantNumber.Length < 13)
                return "";

            return WarrantNumber.Substring(WarrantNumber.Length - 13, 13);
        }

        public string GetWarrantNumber(string zoneCode)
        {
            if (string.IsNullOrEmpty(WarrantNumber))
                return "";
            int oldLength = WarrantNumber.Length;

            if (WarrantNumber.Replace(zoneCode, "").Length != oldLength)
                return WarrantNumber.Replace(zoneCode, "");
            else
            {
                if (WarrantNumber.Length > Zone.ZONE_GROUP_LENGTH)
                    return WarrantNumber.Substring(Zone.ZONE_GROUP_LENGTH);
                else
                    return "";
            }
        }

        #endregion
    }
}