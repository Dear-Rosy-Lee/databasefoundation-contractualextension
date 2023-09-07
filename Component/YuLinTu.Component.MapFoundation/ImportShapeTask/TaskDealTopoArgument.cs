/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskDealTopoArgument : TaskArgument
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public string Database { get; set; }
        public bool SmallArea
        {
            get; set;
        }//碎面
        public bool AreaRepeat
        {
            get; set;
        }//面节点自重叠
        public bool AreaSelfOverlap { get; set; }// 面要素边界自重叠
        public bool SharePoint
        {
            get; set;
        }// 共用边节点打断
      
        public string TableName { get; set; }
        public TaskDealTopoArgument()
        {
                
        }
    }
}
