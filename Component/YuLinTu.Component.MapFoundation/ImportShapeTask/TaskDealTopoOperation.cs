/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using YuLinTu.NetAux;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskDealTopoOperation : Task
    {
        public TaskDealTopoOperation()
        {

        }
        private string fileName;
        private string tableName;

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskDealTopoArgument metadata = Argument as TaskDealTopoArgument;
            if (metadata == null)
            {
                return;
            }
            if (metadata.Database.IsNullOrEmpty())
            {
                this.ReportError("数据库连接失败");
                this.ReportProgress(100);
                return;
            }
            if (metadata.TableName.IsNullOrEmpty())
            {
                this.ReportError("请选择需要处理的图层");
                this.ReportProgress(100);
                return;
            }
            fileName = metadata.Database;
            tableName = metadata.TableName;
            int count = 0;
            if (metadata.SharePoint)
                count++;
            if (metadata.SmallArea)
                count++;
            if (metadata.AreaRepeat)
                count++;
            if (metadata.AreaSelfOverlap)
                count++;
            int per = 90 / (count == 0 ? 1 : count);
            int start = 1;
            if (metadata.SharePoint)
            {
                InsertSharePointOnEdge(start, start + per);
                start += per;
            }
            if (metadata.SmallArea)
            {
                DeleteSmallAreaRecords(start, start + per);
                start += per;
            }
            if (metadata.AreaRepeat)
            {
                RemoveAreaRepeatPoints(start, start + per);
                start += per;
            }
            if (metadata.AreaSelfOverlap)
            {
                RemoveAreaSelfOverlapEdge(start, start + per);
                start += per;
            }
            this.ReportProgress(100);
        }

        #region privateMethod
        /// <summary>
        /// 删除碎面
        /// </summary>
        private void DeleteSmallAreaRecords(int sp, int ep)
        {
            try
            {
                using (var dp = new SpatialiteDataProcess())
                {
                    //var fileName = @"C:\myprojects\问题\20161212建库工具问题描述\27557\河北冀州4525.sqlite";
                    //var tableName = "ZD_CBD";
                    dp.Open(fileName, tableName);
                    int nDelCount = dp.DeleteSmallAreaRecords(0.5, null, i =>
                    {
                        if (sp <= ep)
                            sp += i;
                        this.ReportProgress(sp, "处理第" + i + "碎面");
                        //Console.WriteLine("进度：" + i + "%");
                    });
                    if (nDelCount == 0)
                    {
                        this.ReportInfomation("未发现碎面。");
                    }
                    else
                    {
                        this.ReportInfomation("共删除碎面" + nDelCount + "个。");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError("碎面数据处理失败" + ex.Message);
                this.ReportProgress(100);
                //Console.WriteLine("err:" + ex.Message);
            }
        }

        /// <summary>
        /// 面节点自重叠；
        /// </summary>
        private void RemoveAreaRepeatPoints(int sp, int ep)
        {
            try
            {
                using (var dp = new SpatialiteDataProcess())
                {
                    //var fileName = @"C:\myprojects\问题\20161212建库工具问题描述\27557\河北冀州4525.sqlite";
                    //var tableName = "ZD_CBD";
                    dp.Open(fileName, tableName);
                    int nCount = dp.RemoveRepeatPoints(0.5, null, i =>
                    {
                        if (sp <= ep)
                            sp += i;
                        this.ReportProgress(sp, "处理第" + i + "重复点");
                    });
                    if (nCount == 0)
                    {
                        this.ReportInfomation("未发现有重复点的记录。");
                    }
                    else
                    {
                        this.ReportInfomation("共修改面节点自重叠" + nCount + "条。");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError("面节点自重叠数据处理失败" + ex.Message);
                this.ReportProgress(100);
            }
        }

        /// <summary>
        /// 面要素边界自重叠
        /// </summary>
        private void RemoveAreaSelfOverlapEdge(int sp, int ep)
        {
            try
            {
                using (var dp = new SpatialiteDataProcess())
                {
                    //var fileName = @"C:\myprojects\问题\20161212建库工具问题描述\27557\河北冀州4525.sqlite";
                    //var tableName = "ZD_CBD";
                    dp.Open(fileName, tableName);
                    int nCount = dp.RemoveRepeatPoints(0.5, null, i =>
                    {
                        if (sp <= ep)
                            sp += i;
                        this.ReportProgress(sp, "处理第" + i + "面要素边界自重叠");
                    });
                    if (nCount == 0)
                    {
                        this.ReportInfomation("未发现有面要素边界自重叠的记录。");
                    }
                    else
                    {
                        this.ReportInfomation("共修改面要素边界自重叠" + nCount + "条。");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError("面要素边界自重叠数据处理失败" + ex.Message);
                this.ReportProgress(100);
            }
        }

        /// <summary>
        /// 共用边节点打断测试代码；
        /// </summary>
        private void InsertSharePointOnEdge(int sp, int ep)
        {
            try
            {
                using (var dp = new SpatialiteDataProcess())
                {
                    //var fileName = @"C:\myprojects\问题\20161212建库工具问题描述\27557\河北冀州4525.sqlite";
                    //var tableName = "ZD_CBD";
                    dp.Open(fileName, tableName);
                    int nCount = dp.InsertSharePointOnEdge(0.5, null, i =>
                    {
                        if (sp <= ep)
                            sp += i;
                        this.ReportProgress(sp, "处理第" + i + "共用边节点");
                    });
                    if (nCount == 0)
                    {
                        this.ReportInfomation("未发现有共用边节点的记录。");
                    }
                    else
                    {
                        this.ReportInfomation("共修改共用边节点" + nCount + "条。");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError("共用边节点数据处理失败" + ex.Message);
                this.ReportProgress(100);
            }
        }


        #endregion




        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

    }
}
