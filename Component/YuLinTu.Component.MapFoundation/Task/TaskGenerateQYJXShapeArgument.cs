/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.MapFoundation
{
   public class TaskGenerateQYJXShapeArgument: TaskArgument
    {

        /// <summary>
        /// 界线类型
        /// </summary>
        public KeyValue<string, string> selectLineType { get; set; }

        /// <summary>
        /// 界线性质
        /// </summary>
        public KeyValue<string, string> selectLineNature { get; set; }

        public TaskGenerateQYJXShapeArgument()
        {
                
        }
    }
}
