using System;
using System.IO;
using System.Linq;
using YuLinTu.Windows;

namespace YuLinTu.Component.VectorDataTreatTask
{
    /// <summary>
    /// 应用程序上下文
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>(); 

        }
         
        #endregion Methods
    }
}