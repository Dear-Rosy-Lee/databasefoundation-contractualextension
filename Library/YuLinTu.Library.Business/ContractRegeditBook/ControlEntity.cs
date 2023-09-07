using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
   public class ControlEntity
    {
        /// <summary>
        /// 控件开始x坐标
        /// </summary>
        public double startX { get; set; }
        /// <summary>
        /// 控件开始y坐标
        /// </summary>
        public double startY { get; set; }

        /// <summary>
        /// 控件输入文本框内容
        /// </summary>
        public string textValue { get; set; }

        /// <summary>
        /// 控件选择框下拉框内容
        /// </summary>
        public string ComboboxValue { get; set; }

        /// <summary>
        /// 控件所处的行
        /// </summary>
        public int LocationRowNum { get; set; }
    }
}
