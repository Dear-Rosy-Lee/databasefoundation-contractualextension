/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Spatial;
using YuLinTu.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    ///导入行政地域图斑
    /// </summary>
    public partial class ImportZoneShip : InfoPageBase
    {
        #region Property

        /// <summary>
        /// 导入文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 导入类型
        /// </summary>
        public eImportTypes importType { get; private set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Db { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ImportZoneShip()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Event

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButton_Click_1(object sender, RoutedEventArgs e)
        {
            lbnowDBSRInfo.Content = "";
            lbImportShpSRInfo.Content = "";
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "文件类型(*.shp)|*.shp";
            var val = ofd.ShowDialog();
            if (val != null && val.Value)
            {
                txt_file.Text = ofd.FileName;
            }
        }

        /// <summary>
        /// Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_file_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string name = txt_file.Text.Trim();
            if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && !System.IO.File.Exists(name)))
            {
                mbset.IsEnabled = false;
                return;
            }
            else
            {
                FileName = name;
                mbset.IsEnabled = true;
            }
            ConfigFile();
            CheckReference(FileName, Db);
        }

        /// <summary>
        /// 导入
        /// </summary>
        private void btnExcuteImport_Click_1(object sender, RoutedEventArgs e)
        {
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 配置数据
        /// </summary>
        private void btnSConfig_Click(object sender, RoutedEventArgs e)
        {
            if (ppConfig.IsOpen == true)
            {
                ppConfig.IsOpen = false;
            }
            else
            {
                ppConfig.IsOpen = true;
            }
            ConfigFile();
            dpPanel.IsEnabled = !ppConfig.IsOpen;
            //if()
            //cbfullcode.Text
        }

        /// <summary>
        /// 配置文件
        /// </summary>
        private void ConfigFile()
        {
            tbField.Clear();
            string filename = txt_file.Text.Trim();
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            ShapefileDataReader dataReader = new ShapefileDataReader(filename, GeometryFactory.Default);
            List<KeyValue<int, string>> kvList = new List<KeyValue<int, string>>();
            for (int index = 0; index < dataReader.DbaseHeader.Fields.Count(); index++)
            {
                kvList.Add(new KeyValue<int, string>(index, dataReader.DbaseHeader.Fields[index].Name));
            }
            cbfullcode.ItemsSource = kvList;
            cbfullcode.DisplayMemberPath = "Value";
            YuLinTuDataCommentCollection dataCollection = YuLinTuDataComment.DeserializeXml();
            YuLinTuDataComment dataComment = dataCollection.Find(t => t.AliseName == Zone.YULINTUZONESTRING);
            if (dataComment != null)
            {
                string filed = dataComment.Mapping["全编码"];
                if (!string.IsNullOrEmpty(filed))
                {
                    string rowName = string.Empty;
                    if (kvList.Count > dataComment.RowValue)
                    {
                        rowName = kvList[dataComment.RowValue].Value;
                    }
                    KeyValue<int, string> selectInfo = kvList.Find(t => t.Value == filed);
                    if (selectInfo != null && rowName == filed)
                    {
                        cbfullcode.SelectedItem = selectInfo;
                        tbField.Text = rowName;
                    }
                }
            }
        }

        /// <summary>
        /// 确定选择
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object fullcode = cbfullcode.SelectedValue;
            KeyValue<int, string> info = cbfullcode.SelectedItem as KeyValue<int, string>;
            if (info != null)
            {
                btnExcuteImport.IsEnabled = true;
            }
            else
            {
                btnExcuteImport.IsEnabled = false;
                return;
            }
            Dictionary<string, string> liststr = new Dictionary<string, string>();
            liststr.Add("全编码", fullcode == null ? "None" : info.Value);
            YuLinTuDataComment dataComment = new YuLinTuDataComment();
            dataComment.Name = "行政区域";
            dataComment.AliseName = Zone.YULINTUZONESTRING;
            dataComment.Checked = true;
            dataComment.LayerName = "行政区域";
            dataComment.Mapping = liststr;
            dataComment.RowValue = info.Key;
            YuLinTuDataComment.SerializeXml(new YuLinTuDataCommentCollection() { dataComment });
            tbField.Text = info.Value;
            ppConfig.IsOpen = false;
            dpPanel.IsEnabled = !ppConfig.IsOpen;
        }

        /// <summary>
        /// 取消选择
        /// </summary>
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            ppConfig.IsOpen = false;
            dpPanel.IsEnabled = !ppConfig.IsOpen;
        }

        /// <summary>
        /// 选择匹配项
        /// </summary> 
        private void cbfullcode_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            KeyValue<int, string> info = cbfullcode.SelectedItem as KeyValue<int, string>;
            if (info != null)
            {
                btnSetResult.IsEnabled = true;
                btnExcuteImport.IsEnabled = true;
            }
            else
            {
                btnSetResult.IsEnabled = false;
                btnExcuteImport.IsEnabled = false;
            }
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        private void CheckReference(string fileName, IDbContext db)
        {
            lbInfo.Content = "";
            try
            {
                SpatialReference shpReference = ReferenceHelper.GetShapeReference(fileName);
                string name = typeof(Zone).GetAttribute<DataTableAttribute>().TableName;
                SpatialReference dbReference = ReferenceHelper.GetDbReference<Zone>(db, name, "Shape");
                if (shpReference == null)
                {
                    lbInfo.Content = "Shape文件不存在坐标信息!";
                }
                if (!shpReference.Equals(dbReference))
                {
                    lbInfo.Content = "当前Shape文件坐标信息与数据库不一致!";

                    //坐标提示信息
                    if (shpReference == null || shpReference.WKID == 0)
                    {
                        lbImportShpSRInfo.Content = "当前Shape文件坐标为: Unknown";
                    }
                    var shppi = YuLinTu.Spatial.SpatialReferences.CreateProjectionInfo(shpReference);
                    if (shpReference.IsPROJCS())
                    {
                        lbImportShpSRInfo.Content = "当前Shape文件坐标为: " + shppi.Name + "(" + shpReference.WKID + ")";
                    }
                    if (shpReference.IsGEOGCS() || !shpReference.IsValid())
                    {
                        lbImportShpSRInfo.Content = "当前Shape文件坐标为: " + shppi.GeographicInfo.Name + "(" + shpReference.WKID + ")";
                    }

                    if (dbReference == null || dbReference.WKID == 0)
                    {
                        lbnowDBSRInfo.Content = "当前数据库坐标为: Unknown";
                    }
                    var dbpi = YuLinTu.Spatial.SpatialReferences.CreateProjectionInfo(dbReference);
                    if (dbReference.IsPROJCS())
                    {
                        lbnowDBSRInfo.Content = "当前数据库坐标为: " + dbpi.Name + "(" + dbReference.WKID + ")";
                    }
                    if (dbReference.IsGEOGCS() || !dbReference.IsValid())
                    {
                        lbnowDBSRInfo.Content = "当前数据库坐标为: " + dbpi.GeographicInfo.Name + "(" + dbReference.WKID + ")";
                    }
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "CheckReference", ex.Message + ex.StackTrace);
            }
        }

        #endregion

    }
}
