/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;

namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    /// InvestigationVisioMapPanel.xaml 的交互逻辑-出图导航栏
    /// </summary>
    public partial class InvestigationVisioMapPanel : UserControl
    {
        #region Fields

        private ITheWorkpage theWorkPage;
        private DiagramFoundationLabelCommonSetting dflcsetting = DiagramFoundationLabelCommonSetting.GetIntence();

        #endregion

        #region Properties

        /// <summary>
        /// 地图控件
        /// </summary>
        public MapControl CurrentMapControl { get; set; }

        /// <summary>
        /// 工作页
        /// </summary>
        public ITheWorkpage TheWorkPage
        {
            get { return theWorkPage; }
            set
            {
                theWorkPage = value;
            }
        }

        #endregion

        #region Ctor

        public InvestigationVisioMapPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods-Override   


        #endregion


        #region Events

        /// <summary>
        /// 按照地域导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbZoneSelectExport_Click(object sender, RoutedEventArgs e)
        {
            CurrentMapControl.Action = null;
            var currentZoneCode = theWorkPage.Properties.TryGetValue<string>("CurrentZoneCode", null);
            string templateFileName = "Visio.A0.H.yltvd"; ;//模板名称，是那种模式的模板

            switch (dflcsetting.UseLayoutModel)
            {
                case "A0_横版":
                    templateFileName = "Visio.A0.H.yltvd";
                    break;
                case "A0_竖版":
                    templateFileName = "Visio.A0.V.yltvd";
                    break;
                case "A1_横版":
                    templateFileName = "Visio.A1.H.yltvd";
                    break;
                case "A1_竖版":
                    templateFileName = "Visio.A1.V.yltvd";
                    break;
                case "A3_横版":
                    templateFileName = "Visio.A3.H.yltvd";
                    break;
                case "A3_竖版":
                    templateFileName = "Visio.A3.V.yltvd";
                    break;
                case "A4_横版":
                    templateFileName = "Visio.A4.H.yltvd";
                    break;
                case "A4_竖版":
                    templateFileName = "Visio.A4.V.yltvd";
                    break;
            }

            var task = new MapToVisiosViewTask()
            {
                Name = "导出到公示图",
                Description = "将地图中的数据做适当处理并导出到制图工具中",
                Argument = new MapToVisiosViewTaskArgument()
                {
                    currentZoneCode = currentZoneCode,
                    Workspace = theWorkPage.Workspace,
                    MapControl = CurrentMapControl,
                    ExportGeometryOfExtend = null,
                    DestinationDatabaseFileName = System.IO.Path.Combine(TheApp.Current.GetDataPath(), "Diagrams", Guid.NewGuid().ToString(), "db.sqlite"),
                    TemplateFileName = System.IO.Path.Combine(TheApp.GetApplicationPath(), "Template", templateFileName)
                }
            };

            theWorkPage.TaskCenter.Add(task);
            task.StartAsync();

            theWorkPage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
        }

        #endregion
    }

}
