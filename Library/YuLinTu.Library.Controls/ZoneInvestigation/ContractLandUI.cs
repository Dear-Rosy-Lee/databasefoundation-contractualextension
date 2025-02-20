/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包地块界面实体
    /// </summary>
    public class ContractLandUI : ContractLand
    {
        #region Fields

        private string nameUI;
        private string surveyNumberUI;
        private string tabelAreaUI;
        private string actualAreaUI;
        private int img;

        #endregion

        #region Properties

        /// <summary>
        /// 承包地块缩略图像(界面显示)
        /// </summary>
        public int ImgUI
        {
            get { return img; }
            set
            {
                img = value;
                NotifyPropertyChanged("Img");
            }
        }

        /// <summary>
        /// 承包地块名称(界面显示)
        /// </summary>
        public string NameUI
        {
            get { return nameUI; }
            set
            {
                nameUI = value;
                NotifyPropertyChanged("NameUI");
            }
        }

        /// <summary>
        /// 承包地块调查编码(界面显示)
        /// </summary>
        public string SurveyNumberUI
        {
            get { return surveyNumberUI; }
            set
            {
                surveyNumberUI = value;
                NotifyPropertyChanged("SurveyNumberUI");
            }
        }

        /// <summary>
        /// 承包地块台账面积(界面显示)
        /// </summary>
        public string TabelAreaUI
        {
            get { return tabelAreaUI; }
            set
            {
                tabelAreaUI = value;
                NotifyPropertyChanged("TabelAreaUI");
            }
        }

        /// <summary>
        /// 承包地块实测面积(界面显示)
        /// </summary>
        public string ActualAreaUI
        {
            get { return actualAreaUI; }
            set
            {
                actualAreaUI = value;
                NotifyPropertyChanged("ActualAreaUI");
            }
        }

        #endregion

        #region Method - Static

        /// <summary>
        /// 承包地块界面实体类转换
        /// </summary>
        /// <param name="listLand"></param>
        public static ContractLandUI ContractLandUIConvert(ContractLand land)
        {
            if (land == null)
            {
                return null;
            }
            ContractLandUI landUI = land.ConvertTo<ContractLandUI>();
            landUI.NameUI = land.Name;
            if (land.Shape != null)
            {
                landUI.ImgUI = 1;
            }
            else
            {
                landUI.ImgUI = 2;
            }
            //landUI.SurveyNumberUI = land.LandNumber.Length >= 5 ? land.LandNumber.Substring(land.LandNumber.Length - 5) : land.LandNumber.PadLeft(5, '0');
            landUI.SurveyNumberUI = land.LandNumber;
            landUI.TabelAreaUI = land.TableArea.ToString() + "亩" + "(台)";
            landUI.ActualAreaUI = land.ActualArea.ToString() + "亩" + "(实)";
            return landUI;
        }

        #endregion
    }
}
