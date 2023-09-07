/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using YuLinTu.Library.Office;
using System.Threading;
using YuLinTu.Library.Entity;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;
using ThoughtWorks.QRCode.Codec;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 打印证书
    /// </summary>
    public partial class ContractWarrantPrinter : AgricultureWordBook
    {
        #region Fields

        //private IDatabase db;//数据库
        //private ContractConcord Concord = null;//承包合同
        protected ContractRegeditBook regBook = null;//承包权证

        protected ContractConcord otherConcord;
        protected ContractRegeditBook otherBook;
        protected Zone currentZone = null;//当前地域

        // private VirtualPerson Contractor;//承包方
        private string filePath;//保存文件路径

        private string proviceName;//省名称
        private string cityName;//市名称
        private string countryName;//区县名称
        private string countyCode;//区县编码
        private string townName = string.Empty;//镇名称
        private string villageName = string.Empty;//村名称
        private string groupName = string.Empty;//组名称
        private string fullName = string.Empty;//地域全称
        private bool isPrint;

        private List<ControlEntity> entityList = new List<ControlEntity>();
        private int qrSize;//二维码大小
        private string settingPath = System.IO.Directory.GetCurrentDirectory() + "\\config\\QrSetting.xml";
        private int currentVirLandCount = 0;

        #endregion Fields

        #region Propertys

        /// <summary>
        /// 其他方式承包合同
        /// </summary>
        public ContractConcord OtherConcord
        {
            get
            {
                return otherConcord;
            }
            set
            {
                otherConcord = value;
            }
        }

        /// <summary>
        /// 其他方式承包权证
        /// </summary>
        public ContractRegeditBook OtherBook
        {
            get
            {
                return otherBook;
            }
            set
            {
                otherBook = value;
            }
        }

        ///// <summary>
        ///// 承包合同
        ///// </summary>
        //public new ContractConcord Concord
        //{
        //    set
        //    {
        //        Concord = value;
        //    }
        //}

        /// <summary>
        /// 承包权证
        /// </summary>
        public ContractRegeditBook RegeditBook
        {
            get
            {
                return regBook;
            }
            set
            {
                regBook = value;
            }
        }

        ///// <summary>
        ///// 承包方
        ///// </summary>
        //public new VirtualPerson Contractor
        //{
        //    set { Contractor = value; }
        //}

        /// <summary>
        /// 数据源
        /// </summary>
        public IDbContext dbContext
        { get; set; }

        /// <summary>
        /// 批量导出
        /// </summary>
        public bool BatchExport { get; set; }

        /// <summary>
        /// 是否承包方汇总导出
        /// </summary>
        public bool IsDataSummaryExport { get; set; }

        /// <summary>
        /// 扩展人和地使用Excel文件
        /// </summary>
        public bool UseExcel { get; set; }

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string TempleFilePath { get; set; }

        /// <summary>
        /// 证书共有人数设置-证书数据处理分页设置
        /// </summary>
        public int? BookPersonNum;

        /// <summary>
        /// 证书地块数设置-证书数据处理分页设置
        /// </summary>
        public int? BookLandNum;

        /// <summary>
        /// 证书编码设置-证书编码样式设置
        /// </summary>
        public string BookNumSetting;

        /// <summary>
        /// 直到省的父级地域
        /// </summary>
        public List<Zone> ParentsToProvince { get; set; }

        #endregion Propertys

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="Concord">合同</param>
        public ContractWarrantPrinter()
        {
            base.TemplateName = "权证";
        }

        #endregion Ctor

        #region Method

        /// <summary>
        /// 打印承包经营权证
        /// </summary>
        public void PrintContractLand(bool isPrint)
        {
            this.isPrint = isPrint;
            currentZone = CurrentZone;

            if (!CanContinue())
            {
                MessageBox.Show("当前数据不能打印!", "承包经营权证打印", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!GetTemplateFilePath())//获取模板文件路径
            {
                MessageBox.Show("打印模板不存在或模板错误!", "获取打印模板", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            InitalizeData();
            try
            {
                if (!isPrint)
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

                    this.PrintPreview(this, SystemSet.DefaultPath + @"\" + familyName + " - 农村土地承包经营权证书");
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
        /// 导出承包经营权证
        /// </summary>
        public void ExportContractLand(string fileName)
        {
            filePath = fileName;
            currentZone = CurrentZone;
            if (!CanContinue())
            {
                MessageBox.Show("当前数据不能打印!", "承包经营权证打印", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!GetTemplateFilePath())//获取模板文件路径
            {
                MessageBox.Show("打印模板不存在或模板错误!", "获取打印模板", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            InitalizeData();
            try
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
                string file = fileName;
                if (BatchExport)
                {
                    file += @"\农村土地承包经营权证书";
                }
                if (!Directory.Exists(file))
                {
                    Directory.CreateDirectory(file);
                }
                file += @"\";
                file += familyName;
                file += "-农村土地承包经营权证书";
                if (IsDataSummaryExport) file = filePath + @"\承包权证";
                if (Concord != null && Concord.ArableLandType != ((int)eConstructMode.Family).ToString())
                {
                    file += "(其它承包方式)";
                }

                SaveAs(this, file, true);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 是否能继续
        /// </summary>
        /// <returns></returns>
        private bool CanContinue()
        {
            if (Concord == null && regBook == null && otherConcord == null && otherBook == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        private bool GetTemplateFilePath()
        {
            //string filePath = System.Windows.Forms.Application.StartupPath + @"\Template\农村土地承包经营权证.dot";
            string filePath = TempleFilePath;
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
        /// 初始化数据
        /// </summary>
        private void InitalizeData()
        {
            if (ParentsToProvince == null || ParentsToProvince.Count == 0)
                return;
            var province = ParentsToProvince.Find(c => c.Level == eZoneLevel.Province);
            var city = ParentsToProvince.Find(c => c.Level == eZoneLevel.City);
            var county = ParentsToProvince.Find(c => c.Level == eZoneLevel.County);
            var town = ParentsToProvince.Find(c => c.Level == eZoneLevel.Town);
            var village = ParentsToProvince.Find(c => c.Level == eZoneLevel.Village);
            var group = ParentsToProvince.Find(c => c.Level == eZoneLevel.Group);

            proviceName = province == null ? null : province.Name;
            cityName = city == null ? null : city.Name;
            countryName = county == null ? null : county.Name;
            countyCode = county == null ? string.Empty : county.FullCode;
            townName = town == null ? null : town.Name;
            villageName = village == null ? null : village.Name;
            groupName = group == null ? null : group.Name;
            fullName = currentZone.FullName == null ? null : currentZone.FullName.Replace(city.FullName, "");
        }

        /// <summary>
        /// 设置书签值
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            GetQRContentSetting();
            base.OnSetParamValue(data);
            return WriteBookInformation(data);
        }

        /// <summary>
        /// 填写信息
        /// </summary>
        /// <param name="data"></param>
        protected virtual bool WriteBookInformation(object data)
        {
            try
            {
                WriteZoneExpressBookMark();//设置其它书签
                List<ContractLand> landCollection = SortLandCollection(LandCollection);//获取地块集合
                WriteSharePersonValue();//设置共有人信息
                SetContractLandValue(landCollection);//设置地块值
                DisposeQRContent();
                CreatePicture(PickContentString(entityList));
                landCollection = null;
                Disponse();//注销
                return true;
            }
            catch
            {
                this.Close();
                return false;
            }
        }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="nr">二维码字符串</param>
        private void CreatePicture(string nr)
        {
            try
            {
                System.Drawing.Bitmap bt;
                string enCodeString = nr;
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeVersion = 0;
                bt = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);
                string fileName = DateTime.Now.ToString("yyyyMMdd");
                fileName = System.IO.Directory.GetCurrentDirectory() + @"\" + fileName + ".jpg";
                bt.Save(fileName);
                if (System.IO.File.Exists(fileName))
                {
                    InsertImageCellWithoutPading(AgricultureBookMark.EnCode, fileName, qrSize, qrSize);
                }
                System.IO.File.Delete(fileName);
            }
            catch (SystemException ex)

            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 处理实际数据
        /// </summary>
        public void DisposeQRContent()
        {
            foreach (var item in QrDic)
            {
                foreach (var entity in entityList)
                {
                    if (item.Key == entity.ComboboxValue)
                    {
                        entity.ComboboxValue = item.Value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取二维码设置
        /// </summary>
        public void GetQRContentSetting()
        {
            //var entityXml = XmlHelper<ControlEntity>.ReadXml(System.IO.Directory.GetCurrentDirectory() + "\\wem.xml");
            //将字符串转化为实体
            //entityList = XmlHelper<ControlEntity>.XmlToEntityList(entityXml).ToList();
            if (!File.Exists(settingPath))
            {
                SettingEntity settingEntitys = new SettingEntity();
                settingEntitys.QRSize = 100;
                settingEntitys.TotalRow = 6;
                var valueList = new QRValueSettingEntity();
                valueList.Name = "承包方姓名";
                valueList.Value = "ContractorName";
                settingEntitys.QRContentValueList = new List<QRValueSettingEntity>() { valueList };
                var ewmSavePath = settingPath;
                ToolSerialization.SerializeXml(ewmSavePath, settingEntitys);
            }
            var landCollection = dbContext.CreateContractLandWorkstation();
            currentVirLandCount = landCollection.GetByConcordId(Concord.ID).Count;
            SettingEntity settingEntity = (SettingEntity)ToolSerialization.DeserializeXml(settingPath, typeof(SettingEntity));
            Dictionary<string, string> qrContentDic = new Dictionary<string, string>();
            entityList = settingEntity.ControlList;
            qrSize = settingEntity.QRSize;
            if (entityList == null)
            {
                return;
            }
            else
            {
                foreach (var item in settingEntity.ControlList)
                {
                    string landInfo = item.ComboboxValue;
                    if (DisposeLandInfo(ref landInfo))
                    {
                        for (int i = 1; i <= currentVirLandCount; i++)
                        {
                            if (!qrContentDic.Keys.Contains(landInfo + i))
                            {
                                qrContentDic.Add(landInfo + i, "");
                            }
                        }
                    }
                    else
                    {
                        if (!qrContentDic.Keys.Contains(item.ComboboxValue))
                        {
                            qrContentDic.Add(item.ComboboxValue, "");
                        }
                    }
                }
                QrDic = qrContentDic;
                IsUseQRState = true;
            }
        }

        /// <summary>
        /// 处理地块标识的部分
        /// </summary>
        /// <param name="markerString"></param>
        /// <returns></returns>
        public bool DisposeLandInfo(ref string markerString)
        {
            if (markerString.Contains("(s)") || markerString.Contains("（s）"))
            {
                if (markerString.Contains("(s)"))
                {
                    markerString = markerString.Replace("(s)", "");
                }
                else
                {
                    markerString = markerString.Replace("（s）", "");
                }
                return true;
            }
            return false;
        }

        private StringBuilder sbLandInfo = new StringBuilder();

        /// <summary>
        /// 提取所有文字
        /// </summary>
        /// <param name="controlList"></param>
        /// <returns></returns>
        public string PickContentString(List<ControlEntity> controlList)
        {
            DisposeText(controlList);
            return PickText(controlList, true, 1);
        }

        private string DisposeText(List<ControlEntity> controlList)
        {
            List<ControlEntity> landInfoControlList = new List<ControlEntity>();
            List<string> landInfoMarker = new List<string>();
            //StringBuilder sb = new StringBuilder();
            var controlOrderList = controlList.OrderBy(i => i.LocationRowNum).ThenBy(j => j.startX).ToList();
            foreach (var item in controlOrderList)
            {
                string comboboxValue = item.ComboboxValue;
                if (DisposeLandInfo(ref comboboxValue))
                {
                    landInfoMarker.Add(item.ComboboxValue);
                    landInfoControlList.Add(item);
                }
            }
            if (landInfoControlList.Count <= 0)
            {
                return "";
            }
            var content = PickText(landInfoControlList, false, landInfoControlList.OrderBy(i => i.LocationRowNum).ToList()[0].LocationRowNum) + Environment.NewLine;
            for (int i = 1; i <= currentVirLandCount; i++)
            {
                string landInfo = content;
                foreach (var item in landInfoMarker)
                {
                    if (landInfo.Contains(item))
                    {
                        string marker = item;
                        DisposeLandInfo(ref marker);
                        landInfo = landInfo.Replace(item, QrDic[marker + i]);
                    }
                }
                sbLandInfo.Append(landInfo);
            }
            return sbLandInfo.ToString();
        }

        //string Landcontent = "";
        /// <summary>
        /// 提取文字内容
        /// </summary>
        /// <returns></returns>
        private string PickText(List<ControlEntity> controlList, bool isDisposeLandinfo, int startRow)
        {
            List<ControlEntity> landInfoControlList = new List<ControlEntity>();
            List<string> landInfoMarker = new List<string>();
            var controlOrderList = controlList.OrderBy(i => i.LocationRowNum).ThenBy(j => j.startX).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in controlOrderList)
            {
                string comboboxValue = item.ComboboxValue;
                if (DisposeLandInfo(ref comboboxValue))
                {
                    landInfoMarker.Add(item.ComboboxValue);
                    landInfoControlList.Add(item);
                }
            }

            int currentRow = startRow;
            //QRContentControl lastEWMc = new QRContentControl(GetComboboxItemSource());
            ControlEntity lastEWMc = new ControlEntity();
            for (int i = 0; i < controlOrderList.Count(); i++)
            {
                string content = "";
                string currentCombobocValue = controlOrderList[i].ComboboxValue;
                //此时找到第一个地块信息集
                if (isDisposeLandinfo)
                {
                    if (DisposeLandInfo(ref currentCombobocValue))
                    {
                        sb.Append(Environment.NewLine + sbLandInfo.ToString());
                        i = i + landInfoMarker.Count;
                        currentRow = landInfoControlList[landInfoMarker.Count - 1].LocationRowNum;
                        continue;
                    }
                }

                //如果当前处于同一行
                if (controlOrderList[i].LocationRowNum == currentRow)
                {
                    //上一个控件和当前控件处于同一行
                    if (lastEWMc.LocationRowNum == controlOrderList[i].LocationRowNum)
                    {
                        //计算2个控件之间的距离，以12个像素为一个空格
                        int distance = (int)((controlOrderList[i].startX - lastEWMc.startX - 180) / 12);
                        //添加空格
                        for (int k = 0; k < distance; k++)
                        {
                            content += " ";
                        }
                    }
                    //获取内容
                    content += controlOrderList[i].textValue + controlOrderList[i].ComboboxValue;
                }
                else
                {
                    //获取距离上一个控件几行，并添加换行符
                    for (int j = 0; j < Math.Abs(controlOrderList[i].LocationRowNum - currentRow); j++)
                    {
                        var iii = Math.Abs(controlOrderList[i].LocationRowNum - currentRow);
                        //content += "\n";
                        content += Environment.NewLine;
                        //content += "&Chr(10)&";
                    }
                    content += controlOrderList[i].textValue + controlOrderList[i].ComboboxValue;
                    currentRow = controlOrderList[i].LocationRowNum;
                }
                lastEWMc = controlOrderList[i];
                sb.Append(content);
            }
            var s = sb.ToString();
            return sb.ToString();
        }

        /// <summary>
        /// 写地域扩展书签
        /// </summary>
        protected virtual void WriteZoneExpressBookMark()
        {
            var simpleProvinceNamesDics = InitalizeSimpleProvice();
            var simplenamedic = simpleProvinceNamesDics.Where(s => s.Key.Contains(proviceName)).FirstOrDefault();
            var simplename = simplenamedic.Value != null ? simplenamedic.Value : "";
            SetBookmarkValue(AgricultureBookMark.SimpleProviceName, simplename);
            string bookYear = regBook == null ? (otherBook == null ? "" : otherBook.Year) : regBook.Year;
            SetBookmarkValue(AgricultureBookMark.BookYear, " " + bookYear + " ");
            SetBookmarkValue("County", countryName);
            string serialNumber = string.Empty;
            if (regBook != null)
                serialNumber = string.IsNullOrEmpty(regBook.SerialNumber) ? "" : regBook.SerialNumber.PadLeft(6, '0');
            else if (otherBook != null)
                serialNumber = string.IsNullOrEmpty(otherBook.SerialNumber) ? "" : otherBook.SerialNumber.PadLeft(6, '0');
            SetBookmarkValue(AgricultureBookMark.BookSerialNumber, serialNumber);

            string year = string.Empty;
            string month = string.Empty;
            string day = string.Empty;
            if (regBook != null)
            {
                year = regBook.PrintDate != null ? regBook.PrintDate.Year.ToString() : "";
                month = regBook.PrintDate != null ? regBook.PrintDate.Month.ToString() : "";
                day = regBook.PrintDate != null ? regBook.PrintDate.Day.ToString() : "";
            }
            else if (otherBook != null)
            {
                year = otherBook.PrintDate != null ? otherBook.PrintDate.Year.ToString() : "";
                month = otherBook.PrintDate != null ? otherBook.PrintDate.Month.ToString() : "";
                day = otherBook.PrintDate != null ? otherBook.PrintDate.Day.ToString() : "";
            }
            string allAwareString = year + "  " + month + "  " + day + "  ";
            SetBookmarkValue(AgricultureBookMark.BookAllAwareDate, allAwareString);
            SetBookmarkValue("Year", year);
            SetBookmarkValue("Month", month);
            SetBookmarkValue("Day", day);
            string number = string.Empty;
            if (BookNumSetting.IsNullOrEmpty())
            {
                BookNumSetting = "NO.J";
            }
            if (regBook != null)
                number = BookNumSetting + countyCode + (string.IsNullOrEmpty(regBook.SerialNumber) ? "" : regBook.SerialNumber.PadLeft(7, '0'));
            else if (otherBook != null)
                number = BookNumSetting + countyCode + (string.IsNullOrEmpty(otherBook.SerialNumber) ? "" : otherBook.SerialNumber.PadLeft(7, '0'));
            SetBookmarkValue(AgricultureBookMark.BookWarrantNumber, number);//权证编号
        }

        /// <summary>
        /// 初始化省市简写
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> InitalizeSimpleProvice()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("北京市", "京");
            dic.Add("天津市", "津");
            dic.Add("河北省", "冀");
            dic.Add("山西省", "晋");
            dic.Add("内蒙古自治区", "内蒙古");
            dic.Add("辽宁省", "辽");
            dic.Add("吉林省", "吉");
            dic.Add("黑龙江省", "黑");
            dic.Add("上海市", "沪");
            dic.Add("江苏省", "苏");
            dic.Add("浙江省", "浙");
            dic.Add("安徽省", "皖");
            dic.Add("福建省", "闽");
            dic.Add("江西省", "赣");
            dic.Add("山东省", "鲁");
            dic.Add("河南省", "豫");
            dic.Add("湖北省", "鄂");
            dic.Add("湖南省", "湘");
            dic.Add("广东省", "粤");
            dic.Add("广西壮族自治区", "桂");
            dic.Add("海南省", "琼");
            dic.Add("重庆市", "渝");
            dic.Add("四川省", "川");
            dic.Add("贵州省", "黔");
            dic.Add("云南省", "云");
            dic.Add("西藏自治区", "藏");
            dic.Add("陕西省", "陕");
            dic.Add("甘肃省", "甘");
            dic.Add("青海省", "青");
            dic.Add("宁夏回族自治区", "宁");
            dic.Add("新疆维吾尔自治区", "新");
            dic.Add("香港特别行政区", "港");
            dic.Add("澳门特别行政区", "澳");
            dic.Add("台湾省", "台");
            return dic;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            regBook = null;//承包权证
            Contractor = null;//承包方
        }

        #endregion Method
    }
}