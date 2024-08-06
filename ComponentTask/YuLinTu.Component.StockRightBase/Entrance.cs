using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;


namespace YuLinTu.Component.StockRightBase
{

    public class Entrance : EntranceBase
    {
        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
        }
    }
}
