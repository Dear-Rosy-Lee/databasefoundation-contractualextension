using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using System.IO;
using System.Windows.Forms;
using YuLinTu.Library.Business;
using YuLinTu.Data;

namespace YuLinTu.Component.StockRightShuShan
{
    public class StockLandExpressProgress : WordBase
    {
        #region Fields

        // private LandTypeCollection landTypes;//地类集合
        private bool printLandNeighbor = false;//打印四至信息
        private bool showBirthday;//是否打印出生日期

        private bool showLandNeighborDirection;//显示地块四至方向
        private bool showLandNumberPerfix;//显示地块编码地域前缀
        private bool showLandNumberFamilyNumber;//显示地块编码户号
        private bool showLandNumberContractMode;//显示地块编码承包方式

        #endregion

        #region Propertys

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 承包合同
        /// </summary>
        public ContractConcord Concord { get; set; }

        /// <summary>
        /// 地块信息
        /// </summary>
        public List<ContractLand> LandCollection { get; set; }

        /// <summary>
        /// 共有人信息
        /// </summary>
        public List<Person> SharePersonCollection { get; set; }


        /// <summary>
        /// 是否承包方汇总导出
        /// </summary>
        public bool IsDataSummaryExport { get; set; }

        public Zone CurrentZone { get; set; }

        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 数据字典集合
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 批量导出
        /// </summary>
        public bool BatchExport { get; set; }

        /// <summary>
        /// 证书共有人数设置-证书数据处理分页设置
        /// </summary>
        public int? BookPersonNum;

        /// <summary>
        /// 证书地块数设置-证书数据处理分页设置
        /// </summary>
        public int? BookLandNum;

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        /// <summary>
        /// 是否是家庭承包
        /// </summary>
        public bool IsFamilyMode { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public StockLandExpressProgress()
        {
            //printLandNeighbor = AgriculturePrintSetting.IsShowLandNeighbor;
            //showBirthday = AgriculturePrintSetting.IsShowBirthday;//是否打印出生日期
            //landTypes = ToolSerialization.DeserializeXml(System.Windows.Forms.Application.StartupPath + "\\Config\\LandType.xml", typeof(LandTypeCollection)) as LandTypeCollection;
            //showLandNeighborDirection = AgriculturePrintSetting.IsShowLandNeighborDirection;
            //showLandNumberPerfix = AgriculturePrintSetting.IsShowLandNumberPerfix;
            //showLandNumberFamilyNumber = AgriculturePrintSetting.IsShowLandNumberFamilyNumber;
            //showLandNumberContractMode = AgriculturePrintSetting.IsShowLandNumberContractMode;
        }

        #endregion

        /// <summary>
        /// 设置书签值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            try
            {
                if (SharePersonCollection != null && SharePersonCollection.Count > 0)
                {
                    WriteSharePersonValue();
                }
                if (LandCollection != null && LandCollection.Count > 0)
                {
                    SetContractLandValue(LandCollection);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Method - Land

        /// <summary>
        /// 初始化共有人信息
        /// </summary>
        public void InitalizeAgriLandExpress(bool isPrint, string filePath)
        {
            if (!InitalizeAgriLandTemplateFilePath())//获取模板文件路径
            {
                MessageBox.Show("打印模板不存在共有人模版信息或模板错误!", "获取打印模板", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    int familyNumber = 0;
                    if (!string.IsNullOrEmpty(Contractor.FamilyNumber))
                    {
                        Int32.TryParse(Contractor.FamilyNumber, out familyNumber);
                    }
                    else
                    {
                        ContractLand land = (LandCollection != null && LandCollection.Count > 0) ? LandCollection[0] : null;
                        string landNumber = land != null ? ContractLand.GetLandNumber(land.CadastralNumber) : "";
                        landNumber = (!string.IsNullOrEmpty(landNumber) && landNumber.Length >= 18) ? landNumber.Substring(14, 3) : "";
                        if (!string.IsNullOrEmpty(landNumber))
                        {
                            Int32.TryParse(landNumber, out familyNumber);
                        }
                    }
                    string familyName = (familyNumber == 0 ? Contractor.Name : (familyNumber + "-" + Contractor.Name));
                    string file = filePath;
                    if (BatchExport)
                    {
                        file += @"\农村土地承包经营权证地块扩展";
                    }
                    if (!Directory.Exists(file))
                    {
                        Directory.CreateDirectory(file);
                    }
                    file += @"\";
                    file += familyName;
                    file += "-农村土地承包经营权证地块扩展";
                    if (IsDataSummaryExport) file = filePath + @"\权证地块扩展";
                    this.SaveAs(this, file, true);
                    return;
                }
                if (!isPrint)
                {
                    this.PrintPreview(this, SystemSet.DefaultPath + @"\" + Contractor.Name + " -农村土地承包经营权证地块扩展");
                }
                else
                {
                    this.Print(this);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        private bool InitalizeAgriLandTemplateFilePath()
        {
            //string filePath = System.Windows.Forms.Application.StartupPath + @"\Template\农村土地承包经营权证地块扩展.dot";
            string filePath = TemplateHelper.WordTemplate("安徽蜀山权证模板扩展");
            if (!File.Exists(filePath))//判断文件是否存在
            {
                return false;
            }
            if (!OpenTemplate(filePath))//打开文件
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        /// <param name="dt"></param>
        private void SetContractLandValue(List<ContractLand> landCollection)
        {
            List<Dictionary> listDKLB = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB);
            int index = 1;
            int tabIndex = 0;
            //AddTable(tabIndex);
            //if (IsFamilyMode)
            //{
            //    InsertTableRow(0, 1, landCollection.Count);//插入行

            //    //InsertTableRowClone(0, 1, landCollection.Count);
            //    tabIndex = 0;
            //}
            //else
            //{
            //    InsertTableRow(1, 1, landCollection.Count);
            //    tabIndex = 1;
            //}
            int flag = 1;
            int tableCount = landCollection.Count / 12;
            if (landCollection.Count % 12 > 0)
                tableCount++;
            if (tableCount > 1)
            {
                for (int i = 1; i < tableCount; i++)
                {
                    AddTable(0);
                }
            }
            foreach (ContractLand land in landCollection)
            {
                if (flag == 13 * (tabIndex + 1))
                {
                    tabIndex++;
                    index = 1;
                }

                SetTableCellValue(tabIndex, index, 0, land.Name.IsNullOrEmpty() ? "" : land.Name);
                SetTableCellValue(tabIndex, index, 1, (land.LandNumber.Length > 5 ? land.LandNumber.Substring(land.LandNumber.Length - 5) : land.LandNumber));//取最后5位
                SetTableCellValue(tabIndex, index, 2, land.ShareArea);
                SetTableCellValue(tabIndex, index, 3, land.IsFarmerLand == null ? "" : (land.IsFarmerLand.Value ? "是" : "否"));
                SetTableCellValue(tabIndex, index, 4, DataHelper.GetNeighbor(land));
                var quaArea = DataHelper.GetQuantificationArea(Contractor, land, CurrentZone, DbContext);
                SetTableCellValue(tabIndex, index, 5, quaArea.AreaFormat(2));

                index ++;
                flag++;
            }
        }

        /// <summary>
        /// 设置地块编码值
        /// </summary>
        /// <returns></returns>
        private void SetLandNumberValue(int index, ContractLand land)
        {
            string landNumber = ExAgricultureString.BOOKLANDNUMBER + index.ToString();
            //string landNumberValue = ContractLand.GetLandNumber(land.CadastralNumber);
            //string landValue = string.Empty;
            //if (showLandNumberPerfix && landNumberValue.Length > 14)
            //{
            //    landValue += landNumberValue.Substring(0, 14);
            //}
            //if (showLandNumberFamilyNumber && landNumberValue.Length > 17)
            //{
            //    landValue += landNumberValue.Substring(14, 3);
            //}
            //if (showLandNumberContractMode && landNumberValue.Length > 18)
            //{
            //    landValue += landNumberValue.Substring(17, 1);
            //}
            //if (landNumberValue.Length > 18)
            //{
            //    landValue += landNumberValue.Substring(18);
            //}
            //if (string.IsNullOrEmpty(landValue))
            //{
            //    landValue = landNumberValue;
            //}
            SetBookmarkValue(landNumber, land.LandNumber);//地块编号
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<ContractLand> SortLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
                int index = landNumber.IndexOf("J");
                if (index < 0)
                {
                    index = landNumber.IndexOf("Q");
                }
                if (index > 0)
                {
                    landNumber = landNumber.Substring(index + 1);
                }
                Int32.TryParse(landNumber, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            List<ContractLand> landCollection = new List<ContractLand>();
            foreach (var land in orderdVps)
            {
                landCollection.Add(land);
            }
            return landCollection;
        }

        /// <summary>
        /// 设置角码
        /// </summary>
        private string SetSmallNumber(string smallNumber, string landNumber)
        {
            string landName = landNumber;
            int index = landNumber.IndexOf("-");
            if (index < 0)
            {
                index = landNumber.IndexOf("－");
            }
            if (index < 0)
            {
                return landName;
            }
            string surfix = landNumber.Substring(0, index);
            string number = ToolString.GetAllNumberWithInString(surfix);
            if (string.IsNullOrEmpty(number))
            {
                return landNumber;
            }
            SetBookmarkValue(smallNumber, number);
            landName = surfix.Replace(number, "") + "　" + landNumber.Substring(index, 3) + "\n" + landNumber.Substring(index + 3);
            return landName;
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        /// <param name="printSetting"></param>
        /// <param name="index"></param>
        private void SetLandInformation(ContractLand land, int index)
        {
            //if (printSetting.ShowLevel)
            //{
            //string landLevel = ExAgricultureString.BOOKLANDLEVEL + index.ToString();
            //string levelString = ToolMath.MatchEntiretyNumber(land.LandLevel.ToString()) ? "" : EnumNameAttribute.GetDescription(land.LandLevel);
            //levelString = levelString == "未知" ? "" : levelString;

            //levelString = AgricultureSetting.UseSystemLandLevelDescription ? levelString : land.LandLevel;
            //SetBookmarkValue(landLevel, levelString);//耕保等级
            //}
            //if (printSetting.ShowLandType)
            //{
            //string landType = ExAgricultureString.BOOKLANDTYPE + index.ToString();
            //SetBookmarkValue(landType, GetLandType(land.LandCode));//地类

            if (!SystemSet.NergionbourSet)
            {
                string landFigure = ExAgricultureString.BOOKLANDNEIGHBOR + index.ToString();
                SetBookmarkValue(landFigure, "见附图");//附图
            }
            else
            {
                string landNeighbor = ExAgricultureString.BOOKLANDNEIGHBOR + index.ToString();
                string[] landNeighborList = new string[] { land.NeighborEast, land.NeighborWest, land.NeighborSouth, land.NeighborNorth };
                WriteLandNeighbor(landNeighborList, landNeighbor);
            }
            //}
            //}
            //if (printSetting.ShowIsFarmerLand)
            //{
            // string isFarmerLand = ExAgricultureString.BOOKISFARMERLAN + index.ToString();
            // SetBookmarkValue(isFarmerLand, ContractLand.GetFarmerLand(land.IsFarmerLand));
            // }
        }

        /// <summary>
        /// 打印土地四至
        /// </summary>
        private void WriteLandNeighbor(string[] array, string neighborIndex)
        {
            //if (string.IsNullOrEmpty(landNeighbor.Trim()))
            //{
            //    return;
            //}
            //string[] array = landNeighbor.Split('\r');
            //if (array.Length == 1)
            //{
            //    array = landNeighbor.Split('\n');
            //}
            string column = neighborIndex + "1";
            if (array.Length >= 1)
            {
                SetBookmarkValue(column, "东:" + array[0]);//四至东
            }
            column = neighborIndex + "2";
            if (array.Length >= 2)
            {
                SetBookmarkValue(column, "西:" + array[1]);//四至西
            }
            column = neighborIndex + "3";
            if (array.Length >= 3)
            {
                SetBookmarkValue(column, "南:" + array[2]);//四至南
            }
            column = neighborIndex + "4";
            if (array.Length >= 4)
            {
                SetBookmarkValue(column, "北:" + array[3]);//四至北
            }
        }

        /// <summary>
        /// 获取地类
        /// </summary>
        /// <param name="landCode"></param>
        /// <returns></returns>
        private string GetLandType(string landCode)
        {
            List<Dictionary> landTypeDicts = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDLYLX);
            Dictionary landtypedy = landTypeDicts.Find(m => m.Code == landCode);
            return landtypedy.Name;
        }

        #endregion

        #region Method - SharePerson

        /// <summary>
        /// 初始化共有人信息
        /// </summary>
        public void InitalizeSharePersonExpress(bool isPrint, string filePath)
        {
            if (!InitalizeSharePersonTemplateFilePath())//获取模板文件路径
            {
                MessageBox.Show("打印模板不存在共有人模版信息或模板错误!", "获取打印模板", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    int familyNumber = 0;
                    if (!string.IsNullOrEmpty(Contractor.FamilyNumber))
                    {
                        Int32.TryParse(Contractor.FamilyNumber, out familyNumber);
                    }
                    else
                    {
                        string landNumber = Concord != null ? Concord.ConcordNumber : "";
                        landNumber = (!string.IsNullOrEmpty(landNumber) && landNumber.Length >= 18) ? landNumber.Substring(14, 3) : "";
                        if (!string.IsNullOrEmpty(landNumber))
                        {
                            Int32.TryParse(landNumber, out familyNumber);
                        }
                    }
                    string familyName = (familyNumber == 0 ? Contractor.Name : (familyNumber + "-" + Contractor.Name));
                    string file = filePath;
                    if (BatchExport)
                    {
                        file += @"\农村土地承包经营权证共有人扩展";
                    }
                    if (!Directory.Exists(file))
                    {
                        Directory.CreateDirectory(file);
                    }
                    file += @"\";
                    file += familyName;
                    file += "-农村土地承包经营权证共有人扩展";
                    if (IsDataSummaryExport) file = filePath + @"\权证共有人扩展";
                    this.SaveAs(this, file, true);
                    return;
                }
                if (!isPrint)
                {
                    this.PrintPreview(this, SystemSet.DefaultPath + @"\" + Contractor.Name + "-农村土地承包经营权证共有人扩展");
                }
                else
                {
                    this.Print(this);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        private bool InitalizeSharePersonTemplateFilePath()
        {
            //string filePath = System.Windows.Forms.Application.StartupPath + @"\Template\农村土地承包经营权证共有人扩展.dot";
            string filePath = TemplateHelper.WordTemplate(TemplateFile.RegeditBookSharePersonExtendWord);
            if (!File.Exists(filePath))//判断文件是否存在
            {
                return false;
            }
            if (!OpenTemplate(filePath))//打开文件
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        public void WriteSharePersonValue()
        {
            if (SharePersonCollection == null || SharePersonCollection.Count == 0)
            {
                return;
            }
            int index = 1;
            InsertTableRow(0, 1, SharePersonCollection.Count);//插入行    

            foreach (Person person in SharePersonCollection)
            {
                SetTableCellValue(0, index, 0, person.Name.IsNullOrEmpty() ? "" : person.Name);
                SetTableCellValue(0, index, 1, person.Relationship.IsNullOrEmpty() ? "" : person.Relationship);
                //SetTableCellValue(0, index, 2, person.ICN.IsNullOrEmpty() ? "" : person.ICN);
                SetTableCellValue(0, index, 2, person.Comment.IsNullOrEmpty() ? "" : person.Comment);
                index++;
            }
        }

        /// <summary>
        /// 获取人的年龄
        /// </summary>
        /// <returns></returns>
        private string GetPersonBirthday(Person person, bool showBirthday)
        {
            if (!person.Birthday.HasValue || string.IsNullOrEmpty(person.ICN))
            {
                return string.Empty;
            }
            if (!showBirthday && person.Birthday.HasValue)
            {
                int age = DateTime.Now.Year - person.Birthday.Value.Year;
                return age.ToString();
            }
            if (ICNHelper.Check(person.ICN))
            {
                DateTime birthDay = ICNHelper.GetBirthday(person.ICN);
                return birthDay.Year.ToString() + "." + birthDay.Month.ToString();
            }
            return string.Empty;
        }

        #endregion
    }
}
