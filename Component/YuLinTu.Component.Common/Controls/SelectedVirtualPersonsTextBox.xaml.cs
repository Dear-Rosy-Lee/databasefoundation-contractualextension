/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

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
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.Common
{
    /// <summary>
    /// SelectedVirtualPersonsTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedVirtualPersonsTextBox : MetroTextBox
    {
        //#region Ctor

        ///// <summary>
        ///// 构造函数
        ///// </summary>
        //public SelectedVirtualPersonsTextBox()
        //{
        //    InitializeComponent();
        //    this.DataContext = this;
        //}

        //#endregion

        //#region Properties

        ///// <summary>
        ///// 依赖属性地域及人信息
        ///// </summary>
        //public SelectedZoneAndPersonInfo SelectZoneAndPersonInfo
        //{
        //    get { return (SelectedZoneAndPersonInfo)GetValue(SelectZoneAndPersonInfoProperty); }
        //    set { SetValue(SelectZoneAndPersonInfoProperty, value); }
        //}

        //public static readonly DependencyProperty SelectZoneAndPersonInfoProperty = DependencyProperty.Register("SelectZoneAndPersonInfo", typeof(SelectedZoneAndPersonInfo), typeof(SelectedSummaryExportZoneTB), new PropertyMetadata());

        ///// <summary>
        ///// 工作空间属性
        ///// </summary>
        //public IWorkpage WorkPage { get; set; }

        ///// <summary>
        ///// 数据源
        ///// </summary>
        //public IDbContext DbContext { get; set; }

        //#endregion

        //#region Event

        ///// <summary>
        ///// 地域选择按钮选择
        ///// </summary>
        //private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        //{
        //
        //    string[] nameAndCode = this.SelectZoneAndPersonInfo.ZoneNameAndCode.IsNullOrBlank() ? null : this.SelectZoneAndPersonInfo.ZoneNameAndCode.Split('#');
        //    var dbcontext = DataBaseSource.GetDataBaseSource();
        //    InitializeConcordPersonPage selectPersonPage = new InitializeConcordPersonPage();
        //    selectPersonPage.Workpage = WorkPage;
        //    selectPersonPage.FamilyCollection = CreateVirtualPersonCollection(dbcontext, nameAndCode == null ? string.Empty : nameAndCode[1]);
        //    WorkPage.Workspace.Window.ShowDialog(selectPersonPage, (b, r) =>
        //    {
        //        if (!(bool)b)
        //        {
        //            return;
        //        }
        //        //此处将选中的承包方传回来
        //        var spcs = selectPersonPage.SelectPersonCollection;
        //        if (spcs.Count == 0) return;
        //        PersonSelectedInfo ps;
        //        foreach (var item in spcs)
        //        {
        //            ps = new PersonSelectedInfo();
        //            ps.ID = item.ID;
        //            ps.Name = item.Name;
        //            ps.Status = item.Status;
        //            SelectZoneAndPersonInfo.SelectedPersons.Add(ps);
        //        }
        //    });
        //}

        ///// <summary>
        ///// 创建承包方集合
        ///// </summary>
        //private List<VirtualPerson> CreateVirtualPersonCollection(IDbContext DbContext, string ZoneCode)
        //{
        //    var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
        //    List<VirtualPerson> persons = personStation.GetByZoneCode(ZoneCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);
        //    List<VirtualPerson> vps = new List<VirtualPerson>();
        //    var orderdVps = persons.OrderBy(vp =>
        //    {
        //        //排序
        //        int num = 0;
        //        Int32.TryParse(vp.FamilyNumber, out num);
        //        return num;
        //    });
        //    foreach (VirtualPerson vp in orderdVps)
        //    {
        //        vps.Add(vp);
        //    }
        //    vps.RemoveAll(c => c.Name.Contains("集体"));  //排除集体户
        //    return vps;
        //}

        //#endregion
    }
}