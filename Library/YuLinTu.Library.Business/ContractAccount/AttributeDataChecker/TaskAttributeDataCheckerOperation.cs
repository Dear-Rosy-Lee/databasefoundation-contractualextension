using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business.ContractAccount.AttributeDataChecker
{
    public class TaskAttributeDataCheckerOperation : Task
    {
        #region Property

        public bool IsGroup { get; set; }

        #endregion Property

        #region Field

        private string openFilePath;  //打开文件路径
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        public IDbContext dbContext { get; set; }
        public Zone zone { get; set; }


        #endregion Field

        public TaskAttributeDataCheckerOperation()
        {
            
        }

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskAttributeDataCheckerArgument argument = Argument as TaskAttributeDataCheckerArgument;
            if (argument == null)
            {
                return;
            }
           //todo 实现检查项 
        }

        

        #endregion Method—Helper
    }
}
