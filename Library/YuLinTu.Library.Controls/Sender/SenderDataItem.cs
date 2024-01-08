/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 发包方绑定实体
    /// </summary>
    public class SenderDataItem : CollectivityTissue
    {
        #region Fields

        private static BitmapImage imgSender = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/企业16.png"));

        #endregion

        #region Property

        /// <summary>
        /// 图片
        /// </summary>
        public BitmapImage Img { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public Visibility Visibility { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public SenderDataItem()
        {
            Img = imgSender;
            Visibility = Visibility.Visible;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 转换发包方为绑定实体
        /// </summary>
        public static SenderDataItem ConvertToItem(CollectivityTissue tissue)
        {
            SenderDataItem item = new SenderDataItem();
            item.CheckDate = tissue.CheckDate;
            item.CheckOpinion = tissue.CheckOpinion;
            item.CheckPerson = tissue.CheckPerson;
            item.Code = tissue.Code;
            item.Comment = tissue.Comment;
            item.CreationTime = tissue.CreationTime;
            item.SocialCode = tissue.SocialCode;
            item.Founder = tissue.Founder;
            item.ID = tissue.ID;
            item.LawyerAddress = tissue.LawyerAddress;
            item.SocialCode = tissue.SocialCode;
            item.LawyerCartNumber = tissue.LawyerCartNumber;
            item.LawyerCredentType = tissue.LawyerCredentType;
            item.LawyerName = tissue.LawyerName;
            item.LawyerPosterNumber = tissue.LawyerPosterNumber;
            item.LawyerTelephone = tissue.LawyerTelephone;
            item.ModifiedTime = tissue.ModifiedTime;
            item.Modifier = tissue.Modifier;
            item.Name = tissue.Name;
            item.Status = tissue.Status;
            item.SurveyChronicle = tissue.SurveyChronicle;
            item.SurveyDate = tissue.SurveyDate;
            item.SurveyPerson = tissue.SurveyPerson;
            item.Type = tissue.Type;
            item.ZoneCode = tissue.ZoneCode;
            return item;
        }

        /// <summary>
        /// 设置发包方数据到绑定实体
        /// </summary>
        public static void SetItemValue(SenderDataItem item, CollectivityTissue tissue)
        {
            item.CheckDate = tissue.CheckDate;
            item.CheckOpinion = tissue.CheckOpinion;
            item.CheckPerson = tissue.CheckPerson;
            item.Code = tissue.Code;
            item.Comment = tissue.Comment;
            item.CreationTime = tissue.CreationTime;
            item.Founder = tissue.Founder;
            item.ID = tissue.ID;
            item.LawyerAddress = tissue.LawyerAddress;
            item.LawyerCartNumber = tissue.LawyerCartNumber;
            item.LawyerCredentType = tissue.LawyerCredentType;
            item.LawyerName = tissue.LawyerName;
            item.SocialCode = tissue.SocialCode;
            item.LawyerPosterNumber = tissue.LawyerPosterNumber;
            item.LawyerTelephone = tissue.LawyerTelephone;
            item.ModifiedTime = tissue.ModifiedTime;
            item.Modifier = tissue.Modifier;
            item.Name = tissue.Name;
            item.Status = tissue.Status;
            item.SurveyChronicle = tissue.SurveyChronicle;
            item.SurveyDate = tissue.SurveyDate;
            item.SurveyPerson = tissue.SurveyPerson;
            item.Type = tissue.Type;
            item.ZoneCode = tissue.ZoneCode;
        }

        #endregion
    }
}
