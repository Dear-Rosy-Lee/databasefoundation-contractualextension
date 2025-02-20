/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 检查权证数据任务类
    /// </summary>
    public class TaskWarrantCheckOperation : Task
    {
        #region Ctor


        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskWarrantCheckOperation()
        { }

        #endregion

        #region Field

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskWarrantCheckArgument argument = Argument as TaskWarrantCheckArgument;
            if (argument == null)
            {
                return;
            }
            var dbContext = argument.DbContext;
            try
            {
                var query = dbContext.CreateQuery<ContractRegeditBook>();
                var count = query.Count();
                List<string> numberList = new List<string>();
                query.OrderBy(t => t.SerialNumber).ForEach((i, p, en) =>
                {
                    CheckNumerMember(en.SerialNumber);
                    if (numberList.Count == 0 || (numberList.Count > 0 && numberList[0] == en.SerialNumber))
                    {
                        numberList.Add(en.SerialNumber);
                    }
                    else
                    {
                        if (numberList.Count == 1)
                            numberList.Clear();
                        else if (numberList.Count > 1)
                        {
                            var numstring = numberList[0];
                            this.ReportError(string.Format("承包经营权证流水号{0}存在重复{1}条", (numstring != "/" && !string.IsNullOrEmpty(numstring)) ? numstring.PadLeft(6, '0') :
                                (numstring == "" ? "(空)" : numstring), numberList.Count));
                            numberList.Clear();
                        }
                        numberList.Add(en.SerialNumber);
                    }
                    int num = (int)(i / (double)count) * 100;
                    this.ReportProgress(new TaskProgressChangedEventArgs(num, null));
                    return true;
                });
                this.ReportProgress(100);
            }
            catch (Exception ex)
            {
                this.ReportException(ex, "检查数据发生异常!");
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            base.OpenResult();
        }

        #endregion

        #region Method—Helper

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="number"></param>
        private void CheckNumerMember(string number)
        {
            var charArray = number.ToArray();
            foreach (var item in charArray)
            {
                if (item < 48 || item > 57)
                {
                    this.ReportError(string.Format("承包经营权证流水号{0}存在非数字字符", number));
                    break;
                }
            }
        }
        #endregion
    }
}
