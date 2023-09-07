using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Component.MapFoundation.Controls
{
    /// <summary>
    /// GenerateData.xaml 的交互逻辑
    /// </summary>
    public partial class GenerateData : InfoPageBase
    {
        #region Proerties
        /// <summary>
        /// 界线类型
        /// </summary>
        private KeyValueList<string, string> LineType;

        /// <summary>
        /// 界线性质
        /// </summary>
        private KeyValueList<string, string> LineNature;
        /// <summary>
        /// 界线类型
        /// </summary>
        public KeyValue<string, string> selectLineType;

        /// <summary>
        /// 界线性质
        /// </summary>
        public KeyValue<string, string> selectLineNature;
        #endregion
        public GenerateData(IWorkpage page)
        {
            InitializeComponent();
            this.Workpage = page;
            InitiallControl();
        }
        private void InitiallControl()
        {
            try
            {
                var dbContext = DataBaseSource.GetDataBaseSource();
                var dictStation = dbContext.CreateDictWorkStation();
                LineType = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JXLX);
                LineNature = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JXXZ);
            }
            catch (Exception)
            {
                LineType = new KeyValueList<string, string>();
                LineType = new KeyValueList<string, string>();
            }
            cbLineNature.DisplayMemberPath = "Value";
            cbLineNature.ItemsSource = LineNature;
            cbLineNature.SelectedIndex = 0;

            cbLineType.DisplayMemberPath = "Value";
            cbLineType.ItemsSource = LineType;
            cbLineType.SelectedIndex = 0;
        }
        private void btnExcuteGenerate_Click(object sender, RoutedEventArgs e)
        {
            selectLineType = cbLineType.SelectedValue as KeyValue<string,string>;
            selectLineNature = cbLineNature.SelectedValue as KeyValue<string, string>;
            Workpage.Page.CloseMessageBox(true);
        }
    }
}
