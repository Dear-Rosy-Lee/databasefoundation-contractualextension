/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// 承包地块添加/编辑页面
    /// </summary>
    public partial class AllLandPage : InfoPageBase
    {
        #region Fields
        public List<ContractLand> LandList { get; set; }

        #endregion

        #region Property
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public AllLandPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Method

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInitializeGo()
        {
            if (LandList != null)
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var item in LandList)
                    {
                        ucLandGrid.Items.Add(item);
                    }
                });
            }
            //var datalist = new List<BelongRelationUI>();
            //foreach (var item in PersonbelongRelations)
            //{
            //    var br = item.ConvertTo<BelongRelationUI>();
            //    datalist.Add(br);
            //    var l = LandList.Find(t => t.ID == item.LandID);
            //    if (l != null)
            //    {
            //        br.SCMJM = l.ActualArea;
            //        br.DKBM = l.LandNumber;
            //        br.DKMC = l.LandName;
            //    }
            //}
            Dispatcher.Invoke(() =>
            {
                //dotView.Roots = datalist;
            });
        }

        #endregion

        #region Event

        /// <summary>
        /// 响应添加或者编辑承包台账地块信息确定按钮
        /// </summary>
        private void mbtnLandOK_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync(goCallback =>
            {
                //if (currentPerson == null && !IsStockRight)
                //    return false;
                return GoConfirm();
            }, completedCallback =>
            {
                Close(true);
            });
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <returns></returns>
        public bool GoConfirm()
        {
            return true;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        public void ShowBox(string title, string msg, eMessageGrade grade = eMessageGrade.Error)
        {
            if (Workpage == null)
            {
                return;
            }
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = grade,
                CancelButtonText = "取消",
            });
        }

        /// <summary>
        /// 发送消息(三种)
        /// </summary>
        private void SendMessasge(ModuleMsgArgs args)
        {
            Workpage.Message.Send(this, args);
            TheBns.Current.Message.Send(this, args);
            Workpage.Workspace.Message.Send(this, args);
        }

        #endregion  

    }
}
