using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 系统风格设置
    /// </summary>
    public class SystemStyleSetting
    {
        #region 注册模版类型

        public const string REGEDITTEMPLATESTYLE = "RegeditTemplate";//注册模版样式

        /// <summary>
        /// 注册模版样式
        /// </summary>
        public static bool RegeditTemplateStyle
        {
            get
            {
                //bool isShow = true;
                //string value = ToolConfiguration.GetSpecialAppSettingValue(REGEDITTEMPLATESTYLE, "false");
                //Boolean.TryParse(value, out isShow);
                //return isShow;
                throw new NotImplementedException();
            }
            set
            {
                //ToolConfiguration.SetSpecialAppSettingValue(REGEDITTEMPLATESTYLE, value.ToString());
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
