/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// ClipPolygonSetting.xaml 的交互逻辑-裁剪设置选择框
    /// </summary>
    public partial class ClipPolygonSetting : InfoPageBase
    {
        #region Fields
        private double selectTargetArea;//被选中要素的面积
        private bool selectTargetAreaBigger = false;
       

        /// <summary>
        /// 目标总面积
        /// </summary>
        private double inputTargetArea = 0.0;
        #endregion

        #region Ctor
        public ClipPolygonSetting(double selectTargetArea)
        {
            InitializeComponent();
            TargetAreaList = new List<double>();            
            DataContext = this;
            ClipLandNum.Value = 2;
            this.selectTargetArea = selectTargetArea;
            txt_file.IsEnabled = false;
            btnFileSelect.IsEnabled = false;
            btnExcuteImport.IsEnabled = true;
            eimportlandcliptype = eImportLandClipType.ClipByDistence;//默认按照距离裁剪，按宗裁剪
            lbAreainfo.Visibility = Visibility.Collapsed;
            cbClipByTextAndProportion.Visibility = Visibility.Collapsed;
            proportionInfoGrid.Visibility = Visibility.Collapsed;
            chkClipByDistence.IsChecked = true;
        }
        #endregion

        #region Properties

        /// <summary>
        /// 按何种类型裁剪
        /// </summary> 
        public eImportLandClipType eimportlandcliptype { set; get; }

        /// <summary>
        /// 裁剪的个数
        /// </summary>
        public int StepCount { set; get; }

        /// </summary>
        /// 目标面积列表
        /// </summary>
        public List<double> TargetAreaList { set; get; }
                      
        #endregion

        #region Event

        private void chkClipByDistence_Check(object sender, RoutedEventArgs e)
        {
            ClipLandNum.IsEnabled = true;
            txt_file.IsEnabled = false;
            btnFileSelect.IsEnabled = false;
            ClipLandNum.Value = 2;
            btnExcuteImport.IsEnabled = true;
            eimportlandcliptype = eImportLandClipType.ClipByDistence;
            lbAreainfo.Visibility = Visibility.Collapsed;
            cbClipByTextAndProportion.Visibility = Visibility.Collapsed;
            proportionInfoGrid.Visibility = Visibility.Collapsed;
        }

        private void chkClipByArea_Checked(object sender, RoutedEventArgs e)
        {
            ClipLandNum.IsEnabled = true;
            txt_file.IsEnabled = false;
            btnFileSelect.IsEnabled = false;
            ClipLandNum.Value = 2;
            btnExcuteImport.IsEnabled = true;
            eimportlandcliptype = eImportLandClipType.ClipByAverageArea;
            lbAreainfo.Visibility = Visibility.Collapsed;
            cbClipByTextAndProportion.Visibility = Visibility.Collapsed;
            proportionInfoGrid.Visibility = Visibility.Collapsed;
        }

        private void chkClipByImportSeting_Checked(object sender, RoutedEventArgs e)
        {
            ClipLandNum.IsEnabled = false;
            ClipLandNum.Value = 0;
            txt_file.IsEnabled = true;
            btnFileSelect.IsEnabled = true;
            eimportlandcliptype = eImportLandClipType.ClipByText;
            btnExcuteImport.IsEnabled = false;
        }

        private void txt_file_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string name = txt_file.Text.Trim();
            if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && !System.IO.File.Exists(name)))
            {
                btnExcuteImport.IsEnabled = false;
            }
            else
            {
                btnExcuteImport.IsEnabled = true;
            }
        }

        private void btnFileSelect_Click(object sender, RoutedEventArgs e)
        {
            cbClipByTextAndProportion.IsChecked = false;
            TargetAreaList.Clear();          
            inputTargetArea = 0.0;
            btnExcuteImport.IsEnabled = false;
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "文件类型(*.txt)|*.txt";
            var val = ofd.ShowDialog();
            if (val != null && val.Value)
            {
                txt_file.Text = ofd.FileName;
            }
            if (txt_file.Text == "") return;
            string filepath = System.IO.Path.GetDirectoryName(txt_file.Text);
            string filename = System.IO.Path.GetFileNameWithoutExtension(txt_file.Text);

            StreamReader sr = new StreamReader(txt_file.Text, Encoding.Default);
            string line;

            while ((line = sr.ReadLine()) != null && line != "")
            {
                try
                {                 
                    TargetAreaList.Add(double.Parse(line) / 0.0015);
                    inputTargetArea = inputTargetArea + double.Parse(line) / 0.0015;
                }
                catch
                {
                    TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                    if (Workpage == null) return;
                    messagebox.Message = "数据导入的格式不正确，面积应该为**(亩)";
                    messagebox.Header = "提示";
                    messagebox.MessageGrade = eMessageGrade.Error;
                    messagebox.CancelButtonText = "取消";
                    Workpage.Page.ShowMessageBox(messagebox);
                    btnExcuteImport.IsEnabled = false;
                    return;
                }
            }
            proportionInfoGrid.Visibility = Visibility.Visible;
            lbAreainfo.Visibility = Visibility.Visible;
            cbClipByTextAndProportion.Visibility = Visibility.Visible;
            if (inputTargetArea > selectTargetArea)
            {
                btnExcuteImport.IsEnabled = false;
                selectTargetAreaBigger = false;
                this.lbAreainfo.Content = "导入面积大于图形面积，是否按比例分割";
            }
            else
            {
                selectTargetAreaBigger = true;
                this.lbAreainfo.Content = "导入面积小于图形面积，是否按比例分割";
                eimportlandcliptype = eImportLandClipType.ClipByText;
            }
        }

        private void btnExcuteImport_Click_1(object sender, RoutedEventArgs e)
        {
            if (eimportlandcliptype != eImportLandClipType.ClipByText && eimportlandcliptype != eImportLandClipType.ClipByTextAndProportion)
            {
                StepCount = ClipLandNum.Value.Value;
                if (StepCount < 2) return;
            }
            Workpage.Page.CloseMessageBox(true);
        }

        private void cbClipByTextAndProportion_Click(object sender, RoutedEventArgs e)
        {
            if (cbClipByTextAndProportion.IsChecked.Value)
            {
                List<double> targetAreaProportionList = new List<double>();
                double areaProportion = 0.0;
                if (selectTargetAreaBigger)
                {
                    areaProportion = inputTargetArea / selectTargetArea;
                    TargetAreaList.ForEach(i => targetAreaProportionList.Add(i / areaProportion));
                    TargetAreaList = targetAreaProportionList;
                }
                else
                {
                    //用户导入的面积大
                    //定义新的按比例的面积集合                 
                    areaProportion = selectTargetArea / inputTargetArea;
                    TargetAreaList.ForEach(i => targetAreaProportionList.Add(i * areaProportion));
                    TargetAreaList = targetAreaProportionList;
                }
                eimportlandcliptype = eImportLandClipType.ClipByTextAndProportion;
                btnExcuteImport.IsEnabled = true;
            }
            else
            {
                if (selectTargetAreaBigger)
                {
                    btnExcuteImport.IsEnabled = true;
                    eimportlandcliptype = eImportLandClipType.ClipByText;
                }
                else
                {
                    btnExcuteImport.IsEnabled = false;
                }
            }
        }
        #endregion
    }
}
