/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
*/
using Quality.Business.TaskBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YuLinTu.Excel;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    /// <summary>
    /// 文件导出选择界面
    /// </summary>
    public partial class FileChoicePage : TabMessageBox
    {
        #region Fields

        /// <summary>
        /// 临时导出文件选择信息
        /// </summary>
        private ImportFileEntity exportConfig;

        private FileZoneEntity fileZoneConfig;

        //private bool isLoadComplate;
        private CheckBox cbshape;

        private CheckBox cbbase;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 导出文件配置
        /// </summary>
        public ImportFileEntity ExportConfig
        {
            get { return exportConfig; }
            set { exportConfig = value; }
        }

        /// <summary>
        /// 选择文件地址
        /// </summary>
        public string ImportFilePath { get; set; }

        /// <summary>
        /// 工作页面
        /// </summary>
        public IWorkpage Workpage { get; set; }

        /// <summary>
        /// 导出文件/地域配置
        /// </summary>
        public FileZoneEntity FileZonetConfig
        {
            get
            {
                fileZoneConfig.ImportFile = exportConfig;
                return fileZoneConfig;
            }
            set
            {
                //isLoadComplate = false;
                if (value == null)
                {
                    fileZoneConfig = new FileZoneEntity();
                }
                else
                {
                    fileZoneConfig = value;
                }
                SelectZone = fileZoneConfig.ZoneInfo;
                exportConfig = CloneEntity(fileZoneConfig.ImportFile);
                this.DataContext = this;
                //isLoadComplate = true;
            }
        }

        /// <summary>
        /// 选择地域
        /// </summary>
        public ZoneDataItem SelectZone { get; set; }

        /// <summary>
        /// 登录SessionCode
        /// </summary>
        public Guid? SessionCode { get; set; }

        #endregion Properties

        #region Ctor

        public FileChoicePage()
        {
            InitializeComponent();
            //isLoadComplate = false;
            InitiallControls();
        }

        #endregion Ctor

        #region Methods

        private void InitiallControls()
        {
        }

        /// <summary>
        /// 克隆实体
        /// </summary>
        private ImportFileEntity CloneEntity(ImportFileEntity entity)
        {
            ImportFileEntity efe = new ImportFileEntity();
            efe.VictorZone = entity.VictorZone.Clone() as FileEntity;
            efe.TableCBDKXX = entity.TableCBDKXX.Clone() as FileEntity;
            efe.TableCBF = entity.TableCBF.Clone() as FileEntity;
            efe.TableJTCY = entity.TableJTCY.Clone() as FileEntity;
            efe.TableCBHT = entity.TableCBHT.Clone() as FileEntity;
            efe.TableCBQZ = entity.TableCBQZ.Clone() as FileEntity;
            efe.TableQZBF = entity.TableQZBF.Clone() as FileEntity;
            efe.TableQZHF = entity.TableQZHF.Clone() as FileEntity;
            efe.TableQZZX = entity.TableQZZX.Clone() as FileEntity;
            efe.TableQZDJB = entity.TableQZDJB.Clone() as FileEntity;
            efe.TableFBF = entity.TableFBF.Clone() as FileEntity;
            efe.TableLZHT = entity.TableLZHT.Clone() as FileEntity;
            efe.TableZLFJ = entity.TableZLFJ.Clone() as FileEntity;
            efe.VictorDK = entity.VictorDK.Clone() as FileEntity;
            efe.VictorDZDW = entity.VictorDZDW.Clone() as FileEntity;
            efe.VictorJBNTBHQ = entity.VictorJBNTBHQ.Clone() as FileEntity;
            efe.VictorJZDX = entity.VictorJZDX.Clone() as FileEntity;
            efe.VictorKZD = entity.VictorKZD.Clone() as FileEntity;
            efe.VictorMZDW = entity.VictorMZDW.Clone() as FileEntity;
            efe.VictorXZDW = entity.VictorXZDW.Clone() as FileEntity;
            efe.VictorQYJX = entity.VictorQYJX.Clone() as FileEntity;
            return efe;
        }

        /// <summary>
        /// 提交
        /// </summary>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            fileZoneConfig.IsImportBusinessData = (bool)cbbase.IsChecked;
            string errorInfo = string.Empty;
            if (fileZoneConfig.IsSelectZone)
            {
                if (SelectZone == null)
                    errorInfo = "未选择导入业务数据的地域! ";

                if (fileZoneConfig.RootZoneInfo != null && (int)fileZoneConfig.RootZoneInfo.Level != (int)eZoneLevel.County)
                {
                    errorInfo = "根级地域不是区县级不能导入数据! ";
                }
            }
            bool hasFileExport = false;
            if (fileZoneConfig.IsImportBusinessData || fileZoneConfig.ImportFile.VictorZone.IsExport ||
              fileZoneConfig.ImportFile.VictorDZDW.IsExport || fileZoneConfig.ImportFile.VictorJBNTBHQ.IsExport ||
                fileZoneConfig.ImportFile.VictorKZD.IsExport || fileZoneConfig.ImportFile.VictorMZDW.IsExport ||
                fileZoneConfig.ImportFile.VictorQYJX.IsExport || fileZoneConfig.ImportFile.VictorXZDW.IsExport ||
                fileZoneConfig.ImportFile.VictorJZDX.IsExport)
            {
                hasFileExport = true;
            }
            if (!hasFileExport)
            {
                errorInfo += "请选择一种导入的数据!";
            }
            if (!string.IsNullOrEmpty(errorInfo))
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = "导入文件设置",
                    Message = errorInfo,
                    MessageGrade = eMessageGrade.Error
                });
                return;
            }
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        private void TabMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Border headborder = (Border)gbBaseData.Template.FindName("Header", gbBaseData);
                ContentPresenter headContentPresenter = (ContentPresenter)headborder.Child;
                DataTemplate template = gbBaseData.HeaderTemplate;
                cbbase = template.FindName("cbBaseData", headContentPresenter) as CheckBox;
                Border shapeheadborder = (Border)gbShapeData.Template.FindName("Header", gbShapeData);
                ContentPresenter shapeContentPresenter = (ContentPresenter)shapeheadborder.Child;
                DataTemplate shapetemplate = gbShapeData.HeaderTemplate;
                cbshape = shapetemplate.FindName("cbShapeData", shapeContentPresenter) as CheckBox;
                if (cbbase != null)
                {
                    cbbase.Click += cbbase_Click;
                    cbbase.IsChecked = fileZoneConfig.IsImportBusinessData;
                    SetCheckStatus(fileZoneConfig.IsImportBusinessData);
                }
                if (cbshape != null)
                {
                    bool isall = IsShapeAllCheck();
                    bool hascheck = IsShapeHasCheck();
                    if (isall)
                        cbshape.IsChecked = true;
                    else
                    {
                        if (hascheck)
                            cbshape.IsChecked = null;
                        else
                            cbshape.IsChecked = false;
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 按钮事件
        /// </summary>
        private void cbbase_Click(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)cbbase.IsChecked;
            SetCheckStatus(isCheck);
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        private void SetCheckStatus(bool setStatus)
        {
            foreach (var item in wpBaseData.Children)
            {
                CheckBox cb = item as CheckBox;
                if (cb == null)
                    continue;
                cb.IsChecked = setStatus;
            }
        }

        /// <summary>
        /// 获取子控件
        /// </summary>
        private DependencyObject GetChildName(DependencyObject control, string controlName)
        {
            if (control == null)
            {
                return null;
            }
            if ((control as FrameworkElement).Name == controlName)
            {
                return control;
            }

            DependencyObject obj = null;
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(control) - 1; i++)
            {
                obj = VisualTreeHelper.GetChild(control, i);
                obj = GetChildName(obj, controlName);
                if (obj != null)
                    return obj;
            }
            return null;
        }

        /// <summary>
        /// 数据库模块
        /// </summary>
        private void BaseData_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbHeader = sender as CheckBox;
            if (cbHeader == null)
            {
                return;
            }
            foreach (var item in wpBaseData.Children)
            {
                CheckBox cb = item as CheckBox;
                if (cb.IsEnabled)
                    cb.IsChecked = cbHeader.IsChecked;
            }
        }

        /// <summary>
        /// 空间表模块
        /// </summary>
        private void ShapeData_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbHeader = sender as CheckBox;
            if (cbHeader == null)
            {
                return;
            }
            if (cbHeader.IsChecked == null)
                cbHeader.IsChecked = false;
            foreach (var item in wpShapeData.Children)
            {
                CheckBox cb = item as CheckBox;
                if (cb.IsEnabled)
                    cb.IsChecked = cbHeader.IsChecked;
            }
        }

        /// <summary>
        /// 空间表模块
        /// </summary>
        private void ShapeItem_Click(object sender, RoutedEventArgs e)
        {
            bool isall = IsShapeAllCheck();
            bool hascheck = IsShapeHasCheck();
            if (isall)
                cbshape.IsChecked = true;
            else
            {
                if (hascheck)
                    cbshape.IsChecked = null;
                else
                    cbshape.IsChecked = false;
            }
        }

        /// <summary>
        /// 是否全部选择
        /// </summary>
        private bool IsShapeAllCheck()
        {
            bool isAllcheck = true;
            foreach (var item in wpShapeData.Children)
            {
                CheckBox cb = item as CheckBox;
                if (cb == null)
                    continue;
                if (!(bool)cb.IsChecked)
                {
                    isAllcheck = false;
                    break;
                }
            }
            return isAllcheck;
        }

        /// <summary>
        /// 是否有选择
        /// </summary>
        private bool IsShapeHasCheck()
        {
            bool isAllcheck = false;
            foreach (var item in wpShapeData.Children)
            {
                CheckBox cb = item as CheckBox;
                if (cb == null)
                    continue;
                if ((bool)cb.IsChecked)
                {
                    isAllcheck = true;
                    break;
                }
            }
            return isAllcheck;
        }

        /// <summary>
        /// 获取地域
        /// </summary>
        private void mbtnGetCode_Click(object sender, RoutedEventArgs e)
        {
            ZoneSelectorPanel zoneSelectorPanel = new ZoneSelectorPanel();
            zoneSelectorPanel.SelectorZone = fileZoneConfig.ZoneInfo;
            zoneSelectorPanel.GetZone += GetZoneList;
            zoneSelectorPanel.Workpage = Workpage;
            Workpage.Page.ShowDialog(zoneSelectorPanel, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                fileZoneConfig.ZoneInfo = zoneSelectorPanel.SelectorZone;
                txtCode.Text = fileZoneConfig.ZoneInfo == null ? "" : fileZoneConfig.ZoneInfo.Name;
                SelectZone = fileZoneConfig.ZoneInfo;
                fileZoneConfig.RootZoneInfo = zoneSelectorPanel.RootZone;
            });
        }

        /// <summary>
        /// 获取地域集合
        /// </summary>
        private List<ZoneSelectInfo> GetZoneList()
        {
            List<ZoneSelectInfo> list = new List<ZoneSelectInfo>();
            if (string.IsNullOrEmpty(ImportFilePath))
            {
                return list;
            }
            string cateloge = ImportFilePath + @"\权属数据";
            if (!Directory.Exists(cateloge))
                return list;
            string[] files = Directory.GetFiles(cateloge);
            string folder = System.IO.Path.GetFileName(ImportFilePath);
            if (files.Length == 0)
                return list;
            if (folder.Length >= 6)
                folder = folder.Substring(0, 6);
            string dbName = string.Empty;
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                string ext = System.IO.Path.GetExtension(path);
                if (name.StartsWith(folder) && name.Length == 10 && (ext.ToLower() == ".mdb"))
                {
                    dbName = name;
                    break;
                }
            }
            if (string.IsNullOrEmpty(dbName))
                return list;
            string excelName = dbName + "权属单位代码表.xls";
            string excelPath = string.Empty;
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                string name = System.IO.Path.GetFileName(path);
                if (name.ToLower() == excelName)
                {
                    excelPath = path;
                    break;
                }
            }
            if (string.IsNullOrEmpty(excelPath))
                return list;
            List<Zone> zoneList = GetFileZone(excelPath);
            if (zoneList != null && zoneList.Count > 0)
            {
                int lelvel = zoneList.Max(t => (int)t.Level);
                Zone z = zoneList.Find(t => (int)t.Level == lelvel);
                ZoneSelectInfo info = new ZoneSelectInfo() { Entity = z, FullCode = z.FullCode, Level = z.Level, Name = z.Name };
                List<Zone> children = zoneList.FindAll(t => t.UpLevelCode == z.FullCode);
                if (children != null && children.Count > 0)
                {
                    foreach (var item in children)
                    {
                        GetChildren(item, zoneList, info, z.FullName);
                    }
                }
                list.Add(info);
            }
            return list;
        }

        /// <summary>
        /// 组织树
        /// </summary>
        private void GetChildren(Zone z, List<Zone> zoneList, ZoneSelectInfo zsi, string name)
        {
            if (z == null)
            {
                return;
            }
            z.Name = z.Name.Replace(name, "");
            ZoneSelectInfo info = new ZoneSelectInfo()
            {
                Entity = z,
                FullCode = z.FullCode,
                Level = z.Level,
                Name = z.Name
            };
            zsi.Children.Add(info);
            List<Zone> children = zoneList.FindAll(t => t.UpLevelCode == z.FullCode);
            if (children != null && children.Count > 0)
            {
                foreach (var item in children)
                {
                    GetChildren(item, zoneList, info, z.FullName);
                }
            }
        }

        #endregion Methods

        #region EXCEL-READ

        /// <summary>
        /// 获取权属单位代码表中的地域
        /// </summary>
        /// <returns></returns>
        public List<Zone> GetFileZone(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    return new List<Zone>();
                }
                string errorMsg = string.Empty;
                ISheet sheet = GetSheetFromFile(filePath, ref errorMsg);
                if (sheet == null || !string.IsNullOrEmpty(errorMsg))
                {
                    return new List<Zone>();
                }
                bool result = true;
                List<NameCode> codeList = GetFileNameCode(sheet, ref result);
                return ChangNameCodeToEntity(codeList);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private List<NameCode> GetFileNameCode(ISheet sheet, ref bool result)
        {
            if (sheet == null)
            {
                return null;
            }
            List<NameCode> list = new List<NameCode>();
            for (int i = 1; i < sheet.Rows.Count; i++)
            {
                IRow row = sheet.Rows[i];
                string zoneCode = row.Cells[0].Value.ToString();
                string name = row.Cells[1].Value.ToString();
                if (string.IsNullOrEmpty(zoneCode) && string.IsNullOrEmpty(name))
                {
                    continue;
                }
                if (zoneCode.Length != 14)
                {
                    result = false;
                }
                NameCode nc = new NameCode() { CategoryCode = zoneCode, Name = name };
                nc.Code = zoneCode.TrimEnd('0');
                nc.Length = nc.Code.Length;
                if (nc.Length < 6)
                {
                    nc.Code = nc.Code.PadRight(6, '0');
                    nc.Length = 6;
                }
                if (nc.Length > 6 && nc.Length < 9)
                {
                    nc.Code = nc.Code.PadRight(9, '0');
                    nc.Length = 9;
                }
                if (nc.Length > 9 && nc.Length < 12)
                {
                    nc.Code = nc.Code.PadRight(12, '0');
                    nc.Length = 12;
                }
                if (nc.Length > 12)
                {
                    nc.Code = nc.Code.PadRight(14, '0');
                    nc.Length = 14;
                }
                list.Add(nc);
            }
            return list;
        }

        /// <summary>
        /// 转换编码为地域
        /// </summary>
        /// <returns></returns>
        private List<Zone> ChangNameCodeToEntity(List<NameCode> codeList)
        {
            List<Zone> list = new List<Zone>();
            if (codeList == null || codeList.Count == 0)
            {
                return list;
            }
            List<NameCode> countylist = new List<NameCode>();
            List<NameCode> townlist = new List<NameCode>();
            List<NameCode> villagelist = new List<NameCode>();
            List<NameCode> grouplist = new List<NameCode>();
            List<NameCode> otherlist = new List<NameCode>();
            foreach (var item in codeList)
            {
                switch (item.Length)
                {
                    case 6:
                        countylist.Add(item);
                        break;

                    case 9:
                        townlist.Add(item);
                        break;

                    case 12:
                        villagelist.Add(item);
                        break;

                    case 14:
                        grouplist.Add(item);
                        break;
                }
            }
            countylist.ForEach(t => list.Add(new Zone()
            {
                Name = t.Name,
                FullName = t.Name,
                FullCode = t.Code,
                Level = eZoneLevel.County
            }));
            townlist.ForEach(t =>
            {
                var zone = new Zone()
                {
                    Name = t.Name,
                    FullName = t.Name,
                    FullCode = t.Code,
                    Level = eZoneLevel.Town,
                    UpLevelCode = t.Code.Substring(0, 6)
                };
                list.Add(zone);
            });
            villagelist.ForEach(t =>
            {
                var zone = new Zone()
                {
                    Name = t.Name,
                    FullName = t.Name,
                    FullCode = t.Code,
                    Level = eZoneLevel.Village,
                    UpLevelCode = t.Code.Substring(0, 9)
                };
                list.Add(zone);
            });
            grouplist.ForEach(t =>
            {
                var zone = new Zone()
                {
                    Name = t.Name,
                    FullName = t.Name,
                    FullCode = t.Code,
                    Level = eZoneLevel.Group,
                    UpLevelCode = t.Code.Substring(0, 12)
                };
                list.Add(zone);
            });
            return list;
        }

        /// <summary>
        /// 获取汇总表格
        /// </summary>
        private ISheet GetSheetFromFile(string fileName, ref string errorMsg)
        {
            ISheet sheet = null;
            try
            {
                ExcelFileConnectionStringBuilder excelfielcs = new ExcelFileConnectionStringBuilder();
                excelfielcs.FileName = fileName;
                excelfielcs.FileMode = System.IO.FileMode.Open;
                excelfielcs.FileAccess = System.IO.FileAccess.Read;
                IProviderExcelFile provider = new ProviderExcelFile(excelfielcs.ConnectionString);
                sheet = provider.Sheets.Get(0);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return null;
            }
            return sheet;
        }

        #endregion EXCEL-READ

        #region Class

        /// <summary>
        /// 名称代码
        /// </summary>
        private class NameCode
        {
            /// <summary>
            /// 编码长度
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            /// 编码
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// 权属代码
            /// </summary>
            public string CategoryCode { get; set; }

            /// <summary>
            /// 权属单位
            /// </summary>
            public string Name { get; set; }
        }

        #endregion Class
    }
}