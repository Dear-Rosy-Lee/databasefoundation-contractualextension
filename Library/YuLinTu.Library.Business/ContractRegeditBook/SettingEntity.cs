using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    [Serializable]
    public  class SettingEntity
    {
      /// <summary>
      /// 总行数
      /// </summary>
      public int TotalRow { get; set; }
      /// <summary>
      /// 二维码大小
      /// </summary>
      public int QRSize { get; set; }
      
       /// <summary>
       /// 显示内容
       /// </summary>
      public List<QRValueSettingEntity> QRContentValueList { get; set; }
       /// <summary>
       /// 控件集合
       /// </summary>
      public  List<ControlEntity> ControlList { get; set; }
    }
}
