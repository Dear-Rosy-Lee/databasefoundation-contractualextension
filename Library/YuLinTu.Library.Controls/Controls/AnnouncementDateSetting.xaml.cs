/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Data;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 公示公告信息设置界面
    /// </summary>
    public partial class AnnouncementDateSetting : InfoPageBase
    {
        #region Ctro

        /// <summary>
        /// 构造函数
        /// </summary>
        public AnnouncementDateSetting()
        {
            InitializeComponent();
            cbEndDate.IsChecked = true;
            cbAnnouncementDate.IsChecked = true;
            dtEndDate.Value = DateTime.Now;
            dtAnnouncementDate.Value = DateTime.Now;
            DateSettingForAnnouncementWord = new DateSetting();
        }

        #endregion

        #region Field

        private Zone currentZone;

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                if (currentZone.Level == eZoneLevel.Group || currentZone.Level == eZoneLevel.Village)
                {
                    txtStampUnit.Text = GetStampName();   //获得盖章单位名称
                    txtUnitAndAddress.Text = GetAddress();  //获取申请单位与地址名称
                }
                else
                {
                    txtStampUnit.Text = "默认值";
                    txtStampUnit.IsReadOnly = true;
                    txtStampUnit.IsEnabled = false;
                    txtUnitAndAddress.Text = "默认值";
                    txtUnitAndAddress.IsReadOnly = true;
                    txtUnitAndAddress.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext dbContext { get; set; }

        /// <summary>
        /// 承包地块业务
        /// </summary>
        public AccountLandBusiness LandBusiness { get; set; }

        /// <summary>
        /// 设置村组公示公告表的日期和相关单位名称
        /// </summary>
        public DateSetting DateSettingForAnnouncementWord { get; set; }

        #endregion

        #region Event

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //设置日期
            bool endDate = (bool)cbEndDate.IsChecked;
            bool announceDate = (bool)cbAnnouncementDate.IsChecked;
            if (endDate)
            {
                DateSettingForAnnouncementWord.PublishEndDate = dtEndDate.Value;
            }
            else
            {
                DateSettingForAnnouncementWord.PublishEndDate = null;
            }
            if (announceDate)
            {
                DateSettingForAnnouncementWord.PublishStartDate = dtAnnouncementDate.Value;
            }
            else
            {
                DateSettingForAnnouncementWord.PublishStartDate = null;
            }

            //设置名称
            DateSettingForAnnouncementWord.StampUnit = txtStampUnit.Text.Trim();
            DateSettingForAnnouncementWord.Address = txtUnitAndAddress.Text.Trim();

            Workpage.Page.CloseMessageBox(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 得到盖章单位名称
        /// </summary>
        private string GetStampName()
        {
            string name = string.Empty;
            if (currentZone.Level == eZoneLevel.Village)
            {
                //地域为村级
                name = currentZone.FullName.Replace(currentZone.UpLevelName, "");
            }
            else if (currentZone.Level == eZoneLevel.Group)
            {
                //地域为组级
                Zone parent = LandBusiness.GetParent(currentZone);
                name = parent.FullName.Replace(parent.UpLevelName, "");
            }
            return name + "村委会";
        }

        /// <summary>
        /// 获得申请单位与地址名称
        /// </summary>
        private string GetAddress()
        {
            string levelName = string.Empty;
            if (currentZone.Level == eZoneLevel.Village)
            {
                //地域为村级
                levelName = currentZone.FullName + "村委会)";
            }
            else if (currentZone.Level == eZoneLevel.Group)
            {
                //地域为组级
                levelName = currentZone.UpLevelName + "村委会)";
            }
            string address = GetStampName() + "(地址:" + levelName;
            return address;
        }

        #endregion

    }
}
