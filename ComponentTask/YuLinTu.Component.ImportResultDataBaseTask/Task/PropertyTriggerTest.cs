/*
 * (C) 2014  鱼鳞图公司版权所有,保留所有权利
 * http://www.yulintu.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ImportResultDataBaseTask
{
    public class PropertyTriggerTest : PropertyTrigger
    {
        #region Methods

        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "DataServicePath")
            {
                object servicePath = ObjectExtension.GetPropertyValue(pd.Object, propertyName);
                ChangeServiceAddres(servicePath.ToString());
            }
            pd.Designer.Dispatcher.Invoke(new Action(() =>
            {
               
            }));
        }

        public override void OnPropertyValueInstalled(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "DataServicePath")
            {
                string servicePath = ChangeServiceAddres("", true);
                object oldServiceObj = ObjectExtension.GetPropertyValue(pd.Object, propertyName);
                string oldServicePath = oldServiceObj == null ? "" : oldServiceObj.ToString();
                if (oldServicePath != servicePath && servicePath != "")
                {
                    ObjectExtension.SetPropertyValue(pd.Object, propertyName, servicePath);
                }
            }
        }

        /// <summary>
        /// 修改服务地址
        /// </summary>
        private string ChangeServiceAddres(string servicePath, bool GetValue = false)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "YuLinTuQualityTool.exe.Config";
            if (!File.Exists(fileName))
                return "";
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            string cntString = "/configuration/system.serviceModel/client/endpoint[@name='MinistryOfAgriDataExchangeService']";
            XmlNode cntStringKey = doc.SelectSingleNode(cntString);
            if (GetValue)
            {
                return cntStringKey.Attributes["address"].Value;
            }
            cntStringKey.Attributes["address"].Value = servicePath;
            doc.Save(fileName);
            return servicePath;
        }
        #endregion
    }

    /// <summary>
    /// 地域设置
    /// </summary>
    public class PropertyTriggerZone : PropertyTrigger
    {
        #region Methods

        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
          
        }

        public override void OnPropertyValueInstalled(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "ImportFilePath")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    ITheWorkpage workpage = pd.PropertyGrid.Properties.TryGetValue<ITheWorkpage>("Workpage", null);
                    var dentify = pd.Designer as FileChoiceControl;
                    if (dentify != null)
                    {
                        dentify.Workpage = pd.PropertyGrid.Properties.TryGetValue<IWorkpage>("Workpage", null);
                        dentify.ImportFilePath = pd.Object.GetPropertyValue<string>("ImportFilePath");
                    }
                }));
            }
        }

        #endregion
    }
}
