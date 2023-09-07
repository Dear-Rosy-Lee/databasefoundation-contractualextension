using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows;
using YuLinTu.tGIS.Client;
using YuLinTu.Appwork;
using YuLinTu.Data;
using System.Windows.Data;
using System.Collections;
using YuLinTu.Spatial;
using Xceed.Wpf.Toolkit;
using Microsoft.Win32;
using YuLinTu.Diagrams;
using YuLinTu.tGIS.Data;
using YuLinTu.Data.Shapefile;
using YuLinTu.Data.Dynamic;
using YuLinTu.tGIS;
using YuLinTu.Components.Diagrams;
using YuLinTu.Components.tGIS;
using YuLinTu.Visio;
using YuLinTu.Components.Visio;
using YuLinTu.Visio.Designer;

namespace YuLinTu.Component.DiagramFoundation
{
    internal class VisioModePageContext : VisioPageContextBase
    {
        #region Fields

        private MetroButton btnAddContractLandLabel = null;
        private MetroButton btnClearContractLandLabel = null;
        private MetroButton btnAddContractorNameTable = null;
                    
        #endregion

        #region Ctor

        public VisioModePageContext(IWorkpage workpage)
            : base(workpage)
        {
        }

        #endregion

        #region Methods
        #region Methods - Message Handler

        protected override void OnUninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
            btnAddContractLandLabel.Click -= btnAddContractLandLabel_Click;
            btnClearContractLandLabel.Click -= btnClearContractLandLabel_Click;
            btnAddContractorNameTable.Click -= btnAddContractorNameTable_Click;
           
        }

        [MessageHandler(ID = IDDiagrams.InstallToolbarView)]
        private void InstallToolbarView(object sender, InstallUIElementsEventArgs e)
        {
            btnAddContractLandLabel = new MetroButton();
            btnAddContractLandLabel.MaxWidth = 45;
            btnAddContractLandLabel.ToolTip = "添加图面地块标注";
            btnAddContractLandLabel.Padding = new Thickness(4, 2, 4, 2);
            btnAddContractLandLabel.VerticalContentAlignment = VerticalAlignment.Stretch;
            btnAddContractLandLabel.Content = new ImageTextItem() { ImagePosition = eDirection.Top, Text = "地块标注", Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ContractLandLabelALL.png")) };
            btnAddContractLandLabel.Click += btnAddContractLandLabel_Click;
            e.Items.Add(btnAddContractLandLabel);

            btnClearContractLandLabel = new MetroButton();
            btnClearContractLandLabel.MaxWidth = 45;
            btnClearContractLandLabel.ToolTip = "清除图面地块标注";
            btnClearContractLandLabel.Padding = new Thickness(4, 2, 4, 2);
            btnClearContractLandLabel.VerticalContentAlignment = VerticalAlignment.Stretch;
            btnClearContractLandLabel.Content = new ImageTextItem() { ImagePosition = eDirection.Top, Text = "清除标注", Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ClearCBDAllLalbel.png")) };
            btnClearContractLandLabel.Click += btnClearContractLandLabel_Click;
            e.Items.Add(btnClearContractLandLabel);

            btnAddContractorNameTable = new MetroButton();
            btnAddContractorNameTable.MaxWidth = 45;
            btnAddContractorNameTable.ToolTip = "添加承包方签章表";
            btnAddContractorNameTable.Padding = new Thickness(4, 2, 4, 2);
            btnAddContractorNameTable.VerticalContentAlignment = VerticalAlignment.Stretch;
            btnAddContractorNameTable.Content = new ImageTextItem() { ImagePosition = eDirection.Top, Text = "添加签章表", Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ContractorNameTB.png")) };
            btnAddContractorNameTable.Click += btnAddContractorNameTable_Click;
            e.Items.Add(btnAddContractorNameTable);

        }

        /// <summary>
        /// 添加承包地标注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAddContractLandLabel_Click(object sender, RoutedEventArgs e)
        {
            var currentZoneCode = Workpage.Workspace.Properties.TryGetValue<string>("CurrentZoneCode", null);
            CBDlabelSetPage qzbtableSetting = new CBDlabelSetPage();
            qzbtableSetting.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(qzbtableSetting, (b, ed) =>
            {
                if (!(bool)b)
                {
                    Designer.MouseClick -= DiagramsView_MouseClick;
                    return;
                }
                //配置文件
                VisioMapLayoutSetInfo vmlsi = VisioMapLayoutSetInfoExtend.DeserializeSelectedSetInfo();
                var task = new VisioAddMapCurrentCBDLabelTask()
                {
                    Name = "进行图面标注",
                    Description = "将当前地域下承包地图面上进行标注处理",
                    Argument = new VisioAddMapCurrentCBDLabelTaskArgument()
                    {
                        VisiosView = Designer,
                        CurrentZoneCode = currentZoneCode,
                        VisioMapLayoutSetInfo = vmlsi,
                    }
                };

                Workpage.TaskCenter.Add(task);
                task.StartAsync();

                Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));

            });
        }

        /// <summary>
        /// 清除承包地及地域标注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClearContractLandLabel_Click(object sender, RoutedEventArgs e)
        {
            var view = Designer as ElementsDesigner;
            if (view == null) return;
            view.Dispatcher.Invoke(new Action(() =>
            {
                //去除承包地标注              
                var cbdMarkSheet = view.FindSheetByName("contractLandLabelSheet");
                if (cbdMarkSheet == null) return;

                foreach (var item in (cbdMarkSheet as ElementsSheet).Items)
                    if (item.IsSelected)
                        item.IsSelected = false;

                view.Sheets.Remove(cbdMarkSheet);
                cbdMarkSheet.Dispose();

                //去除组标注
                var czoneMarkSheet = view.FindSheetByName("groupZoneLabelSheet");
                if (czoneMarkSheet == null) return;

                foreach (var item in (czoneMarkSheet as ElementsSheet).Items)
                    if (item.IsSelected)
                        item.IsSelected = false;

                view.Sheets.Remove(czoneMarkSheet);
                czoneMarkSheet.Dispose();

            }));
        }

        /// <summary>
        /// 添加承包方表格，签章表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAddContractorNameTable_Click(object sender, RoutedEventArgs e)
        {
            Designer.MouseClick += DiagramsView_MouseClick;
            TableColumnCountSetPage tableColSetting = new TableColumnCountSetPage();
            Workpage.Page.ShowMessageBox(tableColSetting, (b, ed) =>
            {
                if (!(bool)b)
                {
                    Designer.MouseClick -= DiagramsView_MouseClick;
                    return;
                }
            });
        }

        /// <summary>
        /// 在制图模块上面单击返回坐标信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiagramsView_MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //配置文件
            VisioMapLayoutSetInfo vmlsi = VisioMapLayoutSetInfoExtend.DeserializeSelectedSetInfo();
            var localSP = e.GetPosition(Designer);//先获取屏幕坐标
            var localP = Designer.FromScreen(localSP);//再获取直角坐标
            var task = new VisioAddContractorNameTableTask()
            {
                Name = "添加签章表",
                Description = "在当前地域下添加签章表",
                Argument = new VisioAddContractorNameTableArgument()
                {
                    VisiosView = Designer,
                    locationX = localP.X,
                    locationY = localP.Y,
                    VisioMapLayoutSetInfo = vmlsi,
                }
            };

            Workpage.TaskCenter.Add(task);
            task.StartAsync();

            Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            Designer.MouseClick -= DiagramsView_MouseClick;
        }

        #endregion

        #endregion
    }
}
