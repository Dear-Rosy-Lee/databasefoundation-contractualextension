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
   public class TaskInitializeLandArgument : TaskArgument
    {

        /// <summary>
        /// 地块信息
        /// </summary>
        public ContractLand land { get; set; }
        

        public TaskInitializeLandArgument()
        {
                
        }
    }
}
