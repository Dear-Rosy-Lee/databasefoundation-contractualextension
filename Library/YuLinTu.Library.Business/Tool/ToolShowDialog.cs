/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利 
 * 作用：用于消息提示
 */
using System;
using System.Windows;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 消息提示对话框
    /// </summary>
    public class ToolShowDialog
    {
        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="workpage">工作页</param>
        /// <param name="title">标题</param>
        /// <param name="msg">提示信息</param>
        /// <param name="type">提示类型</param>
        /// <param name="hasConfirm">是否有确定按钮</param>
        /// <param name="hasCancel">是否有取消按钮</param>
        /// <param name="action">业务处理委托方法</param>
        public static void ShowBox(IWorkpage workpage,
                                   string title,
                                   string msg,
                                   eMessageGrade type,
                                   bool hasConfirm,
                                   bool hasCancel,
                                   Action<bool?, eCloseReason> action = null)
        {
            if (workpage == null)
                return;
            workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
                ConfirmButtonVisibility = hasConfirm ? Visibility.Visible : Visibility.Collapsed,
                CancelButtonVisibility = hasCancel ? Visibility.Visible : Visibility.Collapsed,
            }, action);
        }
    }
}
