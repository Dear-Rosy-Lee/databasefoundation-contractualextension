using System;
using System.Windows.Forms; 

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// 导出行政区划成果数据库
    /// </summary>
   public class PanelArcDataExport : Task
    {
        //#region Ctor

        //public PanelArcDataExport()
        //{     
        //    InitalizeInnerData();
        //}

        ///// <summary>
        ///// 初始化内部数据
        ///// </summary>
        //private void InitalizeInnerData()
        //{
        //    txtTargetZone.ReadOnly = true;
        //    txtSavePath.ReadOnly = true;
        //    txtSavePath.ButtonCustom.Image = YuLinTu.WinForm.Resources.Properties.Resources.打开16;
        //    txtSavePath.ButtonCustomClick += txtSavePath_ButtonCustomClick;
        //}

        //#endregion

        //#region Methods

        //#region Methods - Override

        ///// <summary>
        ///// 应用配置
        ///// </summary>
        //protected override void OnApplyConfig()
        //{
        //    base.OnApplyConfig();
        //}

        ///// <summary>
        ///// 创建任务元数据
        ///// </summary>
        ///// <returns></returns>
        //protected override TaskMetadata OnCreateTaskMetadata()
        //{
        //    bool isAuthention = IsAuthenticated();
        //    if (!isAuthention)
        //    {
        //        AuthenticateForm authForm = new AuthenticateForm();
        //        if (authForm.ShowDialog() != DialogResult.OK)
        //        {
        //            return null;
        //        }
        //    }
        //    errorInfo.Clear();
        //    InitalizeByZoneMetadata zoneMeta = new InitalizeByZoneMetadata();
        //    zoneMeta.PerpertyDatabase = YuLinTu.Component.BusinessContext.TheBns.Current.CommonDataSource;
        //    zoneMeta.ZoneCode = txtTargetZone.Text.Trim();
        //    zoneMeta.SpaceDatabase = YuLinTu.Component.BusinessContext.TheBns.Current.CommonDataSource2;
        //    Zone zone = zoneMeta.DbInstance.Zone.Get(zoneMeta.ZoneCode);
        //    bool canContinue = true;
        //    if (zone == null)
        //    {
        //        errorInfo.SetError(txtTargetZone, "行政区域无效!");
        //        canContinue = false;
        //    }
        //    if (zone != null && zone.Level > eZoneLevel.County)
        //    {
        //        errorInfo.SetError(txtTargetZone, "行政区域不能超过县级行政区域!");
        //        canContinue = false;
        //    }
        //    string authCode = ToolAuthenticate.InitalizeAuthZoneCode();
        //    if (zone != null && authCode != "86" && !zone.FullCode.Contains(authCode))
        //    {
        //        DialogResult result = MessageBox.Show("选择行政区域未授权,是否重新授权?", "数据检查", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //        if (result == DialogResult.Yes)
        //        {
        //            AuthenticateForm authForm = new AuthenticateForm();
        //            if (authForm.ShowDialog() != DialogResult.OK)
        //            {
        //                return null;
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    if (zone != null)
        //    {
        //        zoneMeta.Information = string.Format("导出{0}下数据", ZoneOperator.InitalizeZoneName(YuLinTu.Component.BusinessContext.TheBns.Current.GetDatabase(), zone));
        //    }
        //    else
        //    {
        //        zoneMeta.Information = "导出系统数据库中数据";
        //    }
        //    if (string.IsNullOrEmpty(txtSavePath.Text))
        //    {
        //        errorInfo.SetError(txtSavePath, "请选择数据保存路径!");
        //        canContinue = false;
        //    }
        //    if (!System.IO.Directory.Exists(txtSavePath.Text))
        //    {
        //        errorInfo.SetError(txtSavePath, "数据保存路径无效!");
        //        canContinue = false;
        //    }
        //    if (string.IsNullOrEmpty(txtUnitName.Text))
        //    {
        //        errorInfo.SetError(txtUnitName, "单位名称不允许为空!");
        //        canContinue = false;
        //    }
        //    if (string.IsNullOrEmpty(txtLinkMan.Text))
        //    {
        //        errorInfo.SetError(txtLinkMan, "联系人不允许为空!");
        //        canContinue = false;
        //    }
        //    if (!string.IsNullOrEmpty(txtTelephone.Text))
        //    {
        //        bool isRight = ToolMath.MatchAllNumber(txtTelephone.Text.Replace("+", "").Replace("-", ""));
        //        if (!isRight)
        //        {
        //            errorInfo.SetError(txtTelephone, "联系电话不符合数字要求！");
        //            canContinue = false;
        //        }
        //    }
        //    else
        //    {
        //        errorInfo.SetError(txtTelephone, "联系电话不允许为空!");
        //        canContinue = false;
        //    }
        //    if (string.IsNullOrEmpty(txtAddress.Text))
        //    {
        //        errorInfo.SetError(txtAddress, "通信地址不允许为空!");
        //        canContinue = false;
        //    }
        //    if (!string.IsNullOrEmpty(txtPostNumber.Text))
        //    {
        //        bool isRight = txtPostNumber.Text.Length == 6 && ToolMath.MatchAllNumber(txtPostNumber.Text);
        //        if (!isRight)
        //        {
        //            errorInfo.SetError(txtPostNumber, "邮政编码不符合数字要求！");
        //            canContinue = false;
        //        }
        //    }
        //    else
        //    {
        //        errorInfo.SetError(txtPostNumber, "邮政编码不允许为空!");
        //        canContinue = false;
        //    }
        //    return canContinue ? zoneMeta : null;
        //}

        ///// <summary>
        ///// 创建任务
        ///// </summary>
        ///// <param name="meta"></param>
        ///// <returns></returns>
        //protected override TaskNode OnCreateTaskNode(TaskMetadata meta)
        //{
        //    ArcDataExportProgress dataProgress = new ArcDataExportProgress();
        //    dataProgress.Argument = meta as InitalizeByZoneMetadata;
        //    dataProgress.Folder = txtSavePath.Text;
        //    dataProgress.ContainMatrical = chkMartical.Checked;
        //    dataProgress.IsReportErrorICN = chkMartICN.Checked;
        //    dataProgress.UnitName = txtUnitName.Text;
        //    dataProgress.LinkMan = txtLinkMan.Text;
        //    dataProgress.Telephone = txtTelephone.Text;
        //    dataProgress.Address = txtAddress.Text;
        //    dataProgress.PosterNumber = txtPostNumber.Text;
        //    dataProgress.ContainDot = chkDot.Checked;
        //    dataProgress.ContainLine = chkLine.Checked;
        //    dataProgress.CanChecker = chkCheckData.Checked;
        //    dataProgress.CheckCardNumber = chkCheckCardNumber.Checked;
        //    TaskNode root = new TaskNode(new TaskInitalizeZoneItemProvider(dataProgress));
        //    return root;
        //}

        ///// <summary>
        ///// 是否已授权
        ///// </summary>
        ///// <returns></returns>
        //private bool IsAuthenticated()
        //{
        //    ToolAuthenticate.KeyFileName = Application.StartupPath + @"\YuLinTu Files\" + "Agriculture.key";
        //    bool isAuthention = ToolAuthenticate.AuthenticateKey(null) == ToolAuthenticate.MappingKeyInformation();
        //    Authenticate.KeyFileName = System.IO.Path.Combine(TheApp.GetProfileDirectoryName(), ConfigurationManager.AppSettings["KeyFileName"]);
        //    string systemCode = Authenticate.InitalizeAuthZoneCode();
        //    string authCode = ToolAuthenticate.InitalizeAuthZoneCode();
        //    if (systemCode == "86" && !string.IsNullOrEmpty(authCode))
        //    {
        //        return isAuthention;
        //    }
        //    if (!authCode.Contains(systemCode))
        //    {
        //        return false;
        //    }
        //    return isAuthention;
        //}

        //#endregion

        //#endregion

        //#region Events

        ///// <summary>
        ///// 地域改变
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void txtTargetZone_TextChanged(object sender, EventArgs e)
        //{
        //    Config.SetValue(txtTargetZone.Name, txtTargetZone.Text.Trim());
        //    EnableCommand();
        //}

        ///// <summary>
        ///// 选择文件保存路径
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void txtSavePath_ButtonCustomClick(object sender, EventArgs e)
        //{
        //    FolderBrowserDialog dialog = new FolderBrowserDialog();
        //    dialog.Description = "请选择数据保存路径:";
        //    if (dialog.ShowDialog() != DialogResult.OK)
        //    {
        //        return;
        //    }
        //    txtSavePath.Text = dialog.SelectedPath;
        //}

        ///// <summary>
        ///// 检查数据改变
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void chkCheckData_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    chkCheckCardNumber.Visible = chkCheckData.Checked;
        //    if (!chkCheckData.Checked)
        //    {
        //        chkCheckCardNumber.Checked = false;
        //    }
        //}

        //#endregion
    }
}
