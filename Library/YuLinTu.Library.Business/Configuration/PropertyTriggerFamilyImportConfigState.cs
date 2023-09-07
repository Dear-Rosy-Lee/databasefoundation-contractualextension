/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml;
using YuLinTu.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 定义触发器，以便触发些方法
    /// </summary>
    public class PropertyTriggerFamilyImportConfigState : PropertyTrigger
    {
        #region Methods

        /// <summary>
        /// 选择值时，此方法会被触发
        /// </summary>
        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
            RefreshState(pd);
        }

        /// <summary>
        /// pd初始时此方法被触发
        /// </summary>
        public override void OnPropertyValueInstalled(PropertyDescriptor pd, string propertyName)
        {
            RefreshState(pd);
        }

        /// <summary>
        /// 刷新状态 pd
        /// pd.Object是取所用属性
        /// pd.Object.TraversalPropertiesInfo() 根据当前控件更新值遍历整个对象属性值
        /// </summary>
        private void RefreshState(PropertyDescriptor pd)
        {
            bool hasEquals = false;
            //导入shp图斑
            var importShpByTypeObj = pd.PropertyGrid.Properties["importShpByType"];
            var imporCommonByTypeObj = pd.PropertyGrid.Properties["importComByType"];                       

            if (importShpByTypeObj != null)
            {
                if (bool.Parse(ObjectExtensions.GetPropertyValue(importShpByTypeObj, "UseLandCodeBindImport").ToString()))
                {
                    if ((pd.Object as ImportAccountLandShapeSettingDefine).CadastralNumberIndex == "None" && pd.Name == "CadastralNumberIndex")
                    {
                        RaiseAlert(pd, eMessageGrade.Error, "地块编码字段为空");  //提示信息  
                        return;
                    }
                }
                else if (bool.Parse(ObjectExtensions.GetPropertyValue(importShpByTypeObj, "UseContractorInfoImport").ToString()))
                {
                    if ((pd.Object as ImportAccountLandShapeSettingDefine).NameIndex == "None" && pd.Name == "NameIndex")
                    {
                        RaiseAlert(pd, eMessageGrade.Error, "承包方名称为空");  //提示信息  
                        return;
                    }
                }
                else if (bool.Parse(ObjectExtensions.GetPropertyValue(importShpByTypeObj, "UseContractorNumberImport").ToString()))
                {
                    if ((pd.Object as ImportAccountLandShapeSettingDefine).VpFamilyNumberIndex == "None" && pd.Name == "VpFamilyNumberIndex")
                    {
                        RaiseAlert(pd, eMessageGrade.Error, "承包方户号为空");  //提示信息  
                        return;
                    }
                }
            }          

            pd.Object.TraversalPropertiesInfo((name, value) =>
            {
                if (pd.Name == name)
                    return true;
                if (importShpByTypeObj != null || imporCommonByTypeObj!=null)                
                {
                    if (object.Equals(pd.Value, value) && ("None" != pd.Value.ToString()))
                    {
                        RaiseAlert(pd, eMessageGrade.Error, "选择重复列");  //提示信息
                        hasEquals = true;
                        return false;
                    }
                }
                else
                {
                    if (object.Equals(pd.Value, value) && (pd.Value as string != "None" && (-1 != (int)pd.Value)))
                    {
                        RaiseAlert(pd, eMessageGrade.Error, "选择重复列");  //提示信息
                        hasEquals = true;
                        return false;
                    }
                }

                return true;
            });
            if (hasEquals)
                return;
            RaiseAlert(pd, eMessageGrade.Infomation, "");  //提示信息
            ////将状态图标置空， 可以换其他的图标
            pd.Designer.Dispatcher.Invoke(new Action(() =>
            {
                pd.ImageState = null;

            }));
        }

        #endregion
    }
}