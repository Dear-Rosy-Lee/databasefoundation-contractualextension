/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Unity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using System.IO;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    /// 公示图标注扩展类
    /// </summary>
   public class VisioMapLayoutSetInfoExtend
    {       

       /// <summary>
       /// 序列化到文件
       /// </summary>
       public static bool SerializeSelectedSetInfo(VisioMapLayoutSetInfo info)
       {
           try
           {
               string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\VisioMapLayoutSetInfo.xml";
               if (!Directory.Exists(Path.GetDirectoryName(fileName)))
               {
                   Directory.CreateDirectory(Path.GetDirectoryName(fileName));
               }
               YuLinTu.Library.WorkStation.ToolSerialization.SerializeXml(fileName, info);
               return true;
           }
           catch
           {
               return false;
           }
       }

       /// <summary>
       /// 反序列化
       /// </summary>
       public static VisioMapLayoutSetInfo DeserializeSelectedSetInfo()
       {
           try
           {
               string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\VisioMapLayoutSetInfo.xml";
               if (!File.Exists(fileName))
               {
                   return new VisioMapLayoutSetInfo()
                   {
                       CbdxzqySetInfo = new VisioMapLayoutCBDXZQYSetInfo()
                       {
                           CbdLabelFontColor = System.Windows.Media.Colors.Black.ToString(),
                           CbdLabelSize = 5,
                           CbdLabelSeparatorHeight = 0.5,
                           GroupZoneLabelColor = System.Windows.Media.Colors.Black.ToString(),
                           GroupZoneLabelSize = 20,
                       },
                       QzbSetInfo = new VisioMapLayoutQZBSetInfo() 
                       {
                           QZBTitleLabelSize = 13,
                           QZBTitleFontColor = System.Windows.Media.Colors.Black.ToString(),
                           QZBHNumBox = 2,
                           QZBTableBorderColor = System.Windows.Media.Colors.Black.ToString(),
                           QZBTableCellHeightSize = 20,
                           QZBTableCellWidthSize = 50,
                           QZBTableLabelColor = System.Windows.Media.Colors.Black.ToString(),
                           QZBTableLabelSize = 5,                       
                       },
                       DxmzdwSetInfo = new VisioMapLayoutDXMZDWSetInfo()
                       {
                           DZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString(),
                           DZDWLabelSize = 15,
                           XZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString(),
                           XZDWLabelSize = 15,
                           MZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString(),
                           MZDWLabelSize = 15,
                       }
                   };
               }
               VisioMapLayoutSetInfo info = YuLinTu.Library.WorkStation.ToolSerialization.DeserializeXml(fileName, typeof(VisioMapLayoutSetInfo)) as VisioMapLayoutSetInfo;
               if (info == null)
                   return new VisioMapLayoutSetInfo()
                   {
                       CbdxzqySetInfo = new VisioMapLayoutCBDXZQYSetInfo()
                       {
                       CbdLabelFontColor = System.Windows.Media.Colors.Black.ToString(),
                       CbdLabelSize = 5,
                       CbdLabelSeparatorHeight = 0.5,
                       GroupZoneLabelColor = System.Windows.Media.Colors.Black.ToString(),
                       GroupZoneLabelSize = 20,
                       },
                       QzbSetInfo = new VisioMapLayoutQZBSetInfo()
                       {
                           QZBTitleLabelSize = 13,
                           QZBTitleFontColor = System.Windows.Media.Colors.Black.ToString(),
                           QZBHNumBox = 2,
                           QZBTableBorderColor = System.Windows.Media.Colors.Black.ToString(),
                           QZBTableCellHeightSize = 20,
                           QZBTableCellWidthSize = 50,
                           QZBTableLabelColor = System.Windows.Media.Colors.Black.ToString(),
                           QZBTableLabelSize = 5,
                       },
                       DxmzdwSetInfo = new VisioMapLayoutDXMZDWSetInfo()
                       {
                           DZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString(),
                           DZDWLabelSize = 15,
                           XZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString(),
                           XZDWLabelSize = 15,
                           MZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString(),
                           MZDWLabelSize = 15,
                       }
                   };
               else
               {
                   return info;
               }
           }
           catch
           {
               return new VisioMapLayoutSetInfo();
           }
       }

    }
}
