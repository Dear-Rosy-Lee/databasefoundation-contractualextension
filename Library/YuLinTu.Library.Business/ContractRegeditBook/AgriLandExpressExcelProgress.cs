using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 填写地块相关信息
    /// </summary>
    public class AgriLandExpressExcelProgress : ExportExcelBase
    {
        #region Fields

        // private LandTypeCollection landTypes;//地类集合
        //private bool printLandNeighbor = false;//打印四至信息
        //private bool showBirthday;//是否打印出生日期
        private bool useActualArea;
        //private bool showLandNeighborDirection;//显示地块四至方向
        //private bool showLandNumberPerfix;//显示地块编码地域前缀
        //private bool showLandNumberFamilyNumber;//显示地块编码户号
        //private bool showLandNumberContractMode;//显示地块编码承包方式

        #endregion

        #region Propertys

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 地块信息
        /// </summary>
        public List<ContractLand> LandCollection { get; set; }

        /// <summary>
        /// 共有人信息
        /// </summary>
        public List<Person> SharePersonCollection { get; set; }

        /// <summary>
        /// 承包合同
        /// </summary>
        public ContractConcord Concord { get; set; }

        /// <summary>
        /// 是否使用实测面积
        /// </summary>
        public bool UseActualArea
        {
            set
            {
                useActualArea = value;
            }
        }

        /// <summary>
        /// 是否承包方汇总导出
        /// </summary>
        public bool IsDataSummaryExport { get; set; }

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

        #endregion

        #region Ctor

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public AgriLandExpressExcelProgress()
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
                if (LandCollection != null && LandCollection.Count > 0)
                {
                    SetContractLandValue(LandCollection);
                }
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
                    file += "-农村土地承包经营权证地块扩展.xls";
                    if (IsDataSummaryExport) file = filePath + @"\权证地块扩展.xls";
                    if (File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                    SaveAs(file);
                    Dispose();
                    return;
                }
                if (!isPrint)
                {
                    Show();
                }
                else
                {
                    Print();
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
            //string filePath = System.Windows.Forms.Application.StartupPath + @"\Template\农村土地承包经营权证地块扩展.xlt";
            string filePath = TemplateHelper.ExcelTemplate(TemplateFile.RegeditBookLandExtendExcel);
            if (!File.Exists(filePath))//判断文件是否存在
            {
                return false;
            }
            if (!Open(filePath))//打开文件
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
            //PrintSetting printSetting = new PrintSetting();
            landCollection = SortLandCollection(landCollection);
            int increment = landCollection.Count / 17;
            increment = landCollection.Count % 17 == 0 ? increment : ++increment;
            int index = 1;
            for (int i = 0; i < increment; i++)
            {
                int endIndex = SetAgriLandHeadText(index, landCollection, i + 1);
                SetRange("A" + index, "A" + endIndex, 19.50, 16, false, "承包地块情况");
                index = endIndex;
            }
            SetLineType("A1", "F" + (index - 1));
        }

        /// <summary>
        /// 设置标头
        /// </summary>
        /// <param name="index"></param>
        private int SetAgriLandHeadText(int index, List<ContractLand> landCollection, int increMent)
        {
            int startIndex = index;
            //陈泽林 20161229
            //SetRange("B" + startIndex, "B" + (startIndex + 1), 19.50, 14, false, "地块名称");
            //SetRange("C" + startIndex, "C" + (startIndex + 1), 19.50, 14, false, "地块编号");
            //SetRange("D" + startIndex, "D" + (startIndex + 1), 19.50, 14, false, "实测面积(亩)");
            //SetRange("E" + startIndex, "F" + (startIndex + 1), 19.50, 14, false, "四  至");
            startIndex += 2;
            for (int i = (increMent - 1) * 17; i < increMent * 17; i++)
            {
                if (i >= landCollection.Count)
                {
                    break;
                }
                ContractLand land = landCollection[i];
                SetRange("B" + startIndex, "B" + startIndex, land.LandNumber);
                SetRange("C" + startIndex, "C" + startIndex,  "见附图");
                //SetLandNumberValue(startIndex, land);
                SetRange("D" + startIndex, "D" + startIndex,  land.AwareArea > 0 ? ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2) : "");
                string sfjbno = "";
                if (land.IsFarmerLand != null)
                {
                    if (land.IsFarmerLand.Value == false)
                        sfjbno = "否";
                    if (land.IsFarmerLand.Value == true)
                        sfjbno = "是";
                }
                SetRange("E" + startIndex, "E" + startIndex, sfjbno);
                SetRange("F" + startIndex, "F" + startIndex, land.Comment);
                //WriteLandNeighbor(land.LandNeighbor, startIndex);
                //SetRange("E" + startIndex, "E" + startIndex, 19.50, 10, false, "东:" + land.NeighborEast);//四至东

                //SetRange("E" + (startIndex + 1), "E" + (startIndex + 1), 19.50, 10, false, "西:" + land.NeighborWest);//四至西

                //SetRange("F" + startIndex, "F" + startIndex, 19.50, 10, false, "南:" + land.NeighborSouth);//四至南

                //SetRange("F" + (startIndex + 1), "F" + (startIndex + 1), 19.50, 10, false, "北:" + land.NeighborNorth);//四至北  

                startIndex += 1;
            }
            while (startIndex < increMent * 36)

            {
                SetRange("B" + startIndex, "B" + startIndex,  "");
                SetRange("C" + startIndex, "C" + startIndex, "");
                SetRange("D" + startIndex, "D" + startIndex,  "");
                startIndex += 1;
            }
            return startIndex;
        }

        /// <summary>
        /// 设置地块编码值
        /// </summary>
        /// <returns></returns>
        private void SetLandNumberValue(int index, ContractLand land)
        {
            //string landNumber = ExAgricultureString.BOOKLANDNUMBER + index.ToString();
            //string landNumberValue = ContractLand.GetLandNumber(land.LandNumber);
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
            //landValue = landNumberValue;
            //}
            SetRange("C" + index, "C" + (index + 1), 19.50, 10, false, land.LandNumber);//地块编号
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
                if (SharePersonCollection != null && SharePersonCollection.Count > 0)
                {
                    WriteSharePersonValue();
                }
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
                    file += "-农村土地承包经营权证共有人扩展.xls";
                    if (IsDataSummaryExport) file = filePath + @"\权证共有人扩展.xls";
                    if (File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                    SaveAs(file);
                    Dispose();
                    return;
                }
                if (!isPrint)
                {
                    Show();
                }
                else
                {
                    Print();
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
            //string filePath = System.Windows.Forms.Application.StartupPath + @"\Template\农村土地承包经营权证共有人扩展.xlt";
            string filePath = TemplateHelper.ExcelTemplate(TemplateFile.RegeditBookSharePersonExtendExcel);
            if (!File.Exists(filePath))//判断文件是否存在
            {
                return false;
            }
            if (!Open(filePath))//打开文件
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
            int increment = SharePersonCollection.Count / 17 + 1;
            if (SharePersonCollection.Count == 17)
            {
                increment = 1;
            }
            int index = 1;
            for (int i = 0; i < increment; i++)
            {
                index = SetSharePersonHeadText(index, SharePersonCollection, i + 1);
            }
            SetLineType("A1", "C" + (index - 1));
        }

        /// <summary>
        /// 设置标头
        /// </summary>
        /// <param name="index"></param>
        private int SetSharePersonHeadText(int index, List<Person> personCollection, int increMent)
        {
            int startIndex = index;
            SetRange("A" + startIndex, "A" + (startIndex + 1), 19.50, 14, false, "姓  名");
            SetRange("B" + startIndex, "B" + (startIndex + 1), 19.50, 14, false, "与承包方代表关系");
            //SetRange("C" + startIndex, "C" + (startIndex + 1), 19.50, 14, false, "身份证号码");
            SetRange("C" + startIndex, "C" + (startIndex + 1), 19.50, 14, false, "备  注");
            startIndex += 2;
            for (int i = (increMent - 1) * 17; i < increMent * 17; i++)
            {
                if (i >= personCollection.Count)
                {
                    break;
                }
                Person person = personCollection[i];
                SetRange("A" + startIndex, "A" + (startIndex + 1), 19.50, 14, false, string.IsNullOrEmpty(person.Name) ? "" : InitializeNameBySet.InitalizeFamilyName(person.Name));
                SetRange("B" + startIndex, "B" + (startIndex + 1), 19.50, 14, false, string.IsNullOrEmpty(person.Relationship) ? "" : person.Relationship);
                //SetRange("C" + startIndex, "C" + (startIndex + 1), 19.50, 14, false, person.ICN);
                SetRange("C" + startIndex, "C" + (startIndex + 1), 19.50, 14, false, string.IsNullOrEmpty(person.Comment) ? "" : person.Comment);
                startIndex += 2;
            }
            while (startIndex < increMent * 36)
            {
                SetRange("A" + startIndex, "A" + (startIndex + 1), 19.50, 14, false, "");
                SetRange("B" + startIndex, "B" + (startIndex + 1), 19.50, 14, false, "");
                //SetRange("C" + startIndex, "C" + (startIndex + 1), 19.50, 14, false, "");
                SetRange("C" + startIndex, "C" + (startIndex + 1), 19.50, 14, false, "");
                startIndex += 2;
            }
            return startIndex;
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

        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }

        #endregion
    }
}
