// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using System.Text.RegularExpressions;
using YuLinTu;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 界址点
    /// </summary>    
    [Serializable]
    [DataTable("JZD")]
    public class BuildLandBoundaryAddressDot : NotifyCDObject, IComparable
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        static BuildLandBoundaryAddressDot()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandMarkType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryPointType);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BuildLandBoundaryAddressDot()
        {
            ID = Guid.NewGuid();
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            //LandmarkType = eLandMarkType.Other;
            //DotType = eBoundaryPointType.FigurePoint;
            //LandType = eLandPropertyRightType.AgricultureLand;
            LandMarkType = "9";
            DotType = "3";
            LandType = "4";
            IsValid = false;
        }

        #endregion

        #region Fields

        private Guid id;  //标识码
        private string dotCode;  //界址点标识码
        private string uniteDotNumber;  //统编界址点号
        private string dotNumber;   //界址点号
        private string landMarkType;  //界标类型
        private string dotType;  //界址点类型
        private Guid landID;    //集体建设用地使用权ID
        private string founder; //创建者
        private DateTime? creationTime;  //创建时间
        private string modifier;  //最后修改者
        private DateTime? modifiedTime;  //最后修改时间
        private string description;   //描述信息
        private string zoneCode;  //地域代码
        private string landNumber;  //地块编码
        private string landType;  //界址点所属权利类型
        private Geometry shape;  //空间字段
        private bool isValid; //界址点是否可用

        #endregion

        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public virtual Guid ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }

        /// <summary>
        /// 界址点标识码
        /// </summary>
        [DataColumn("BSM")]
        public string DotCode
        {
            get { return dotCode; }
            set
            {
                dotCode = value;
                NotifyPropertyChanged("DotCode");
            }
        }

        /// <summary>
        ///统编界址点号
        /// </summary>
        [DataColumn("TBJZDH")]
        public string UniteDotNumber
        {
            get { return uniteDotNumber; }
            set
            {
                uniteDotNumber = value;
                if (!string.IsNullOrEmpty(uniteDotNumber))
                    uniteDotNumber = uniteDotNumber.Trim();
                NotifyPropertyChanged("UniteDotNumber");
            }
        }

        /// <summary>
        ///界址点号
        /// </summary>
        [DataColumn("JZDH")]
        public string DotNumber
        {
            get { return dotNumber; }
            set
            {
                dotNumber = value;
                if (!string.IsNullOrEmpty(dotNumber))
                    dotNumber = dotNumber.Trim();
                NotifyPropertyChanged("DotNumber");
            }
        }

        /// <summary>
        ///界标类型（钢钉…… 见表36）
        /// </summary>
        [DataColumn("JBLX")]
        public string LandMarkType
        {
            get { return landMarkType; }
            set
            {
                landMarkType = value;
                NotifyPropertyChanged("LandMarkType");
            }
        }

        /// <summary>
        ///界址点类型（解析界址点…… 见表37）
        /// </summary>
        [DataColumn("JZDLX")]
        public string DotType
        {
            get { return dotType; }
            set
            {
                dotType = value;
                NotifyPropertyChanged("DotType");
            }
        }

        /// <summary>
        ///集体建设用地使用权ID
        /// </summary>
        [DataColumn("DKID")]
        public Guid LandID
        {
            get { return landID; }
            set
            {
                landID = value;
                NotifyPropertyChanged("LandID");
            }
        }

        /// <summary>
        ///创建者
        /// </summary>
        [DataColumn("CJZ")]
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
        ///创建时间
        /// </summary>
        [DataColumn("CJSJ")]
        public DateTime? CreationTime
        {
            get { return creationTime; }
            set
            {
                creationTime = value;
                NotifyPropertyChanged("CreationTime");
            }
        }

        /// <summary>
        ///最后修改者
        /// </summary>
        [DataColumn("XGZ")]
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
        ///最后修改时间
        /// </summary>
        [DataColumn("XGSJ")]
        public DateTime? ModifiedTime
        {
            get { return modifiedTime; }
            set
            {
                modifiedTime = value;
                NotifyPropertyChanged("ModifiedTime");
            }
        }

        /// <summary>
        ///描述信息
        /// </summary>
        [DataColumn("MSXX")]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }

        /// <summary>
        ///地域代码
        /// </summary>
        [DataColumn("DYBM")]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;
                NotifyPropertyChanged("ZoneCode");
            }
        }

        /// <summary>
        ///界址点所属权利类型
        /// </summary>
        [DataColumn("JZDSSQLLX")]
        public string LandType
        {
            get { return landType; }
            set
            {
                landType = value;
                NotifyPropertyChanged("LandType");
            }
        }

        /// <summary>
        /// 地块编码
        /// </summary>
        [DataColumn("DKBM")]
        public string LandNumber
        {
            get { return landNumber; }
            set
            {
                landNumber = value;
                NotifyPropertyChanged("LandNumber");
            }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        [DataColumn("SFKY")]
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                NotifyPropertyChanged("IsValid");
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

        #endregion

        #region Methods - Override

        /// <summary>
        /// 升序
        /// </summary>
        /// <param name="obj">需要比较的实体</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            BuildLandBoundaryAddressDot dot = obj as BuildLandBoundaryAddressDot;

            if (int.Parse(GetNumber(this.DotNumber)) > int.Parse(GetNumber(dot.DotNumber)))
                return 1;
            else if (int.Parse(GetNumber(this.DotNumber)) < int.Parse(GetNumber(dot.DotNumber)))
                return -1;

            return 0;
        }

        private string GetNumber(string propert)
        {
            int i = 0;
            char[] array = propert.ToCharArray();

            for (i = (array.Length - 1); i > -1; i--)
            {
                if (Regex.Replace(array[i].ToString(), @"[\d]", "").Length > 0)
                {
                    break;
                }
            }
            i++;

            return propert.Substring(i);
        }

        public int GetUniteDotNumber()
        {
            return GetDotNumber(UniteDotNumber);
        }

        public int GetDotNumber()
        {
            return GetDotNumber(DotNumber);
        }

        public static int GetDotNumberToInt(string number)
        {
            if (string.IsNullOrEmpty(number))
                return 0;

            int start = -1;
            int end = 0;
            char[] array = number.ToCharArray();
            foreach (char item in array)
            {
                if (start == -1 && Regex.IsMatch(item.ToString(), @"[\d]"))
                    start = end;

                if (start != -1 && !Regex.IsMatch(item.ToString(), @"[\d]"))
                    break;

                end++;
            }
            if (start < 0)
                return 0;
            return int.Parse(number.Substring(start, end - start));
        }

        private int GetDotNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
                return 0;

            int start = -1;
            int end = 0;
            char[] array = number.ToCharArray();
            foreach (char item in array)
            {
                if (start == -1 && Regex.IsMatch(item.ToString(), @"[\d]"))
                    start = end;

                if (start != -1 && !Regex.IsMatch(item.ToString(), @"[\d]"))
                    break;

                end++;
            }
            if (start < 0)
                return 0;
            return int.Parse(number.Substring(start, end - start));
        }

        #endregion
    }


    [Serializable]
    [DataTable("JZD")]
    public class TempBLBAD : BuildLandBoundaryAddressDot
    {
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public override Guid ID
        {
            get
            {
                return base.ID;
            }

            set
            {
                base.ID = value;
            }
        }
    }
}