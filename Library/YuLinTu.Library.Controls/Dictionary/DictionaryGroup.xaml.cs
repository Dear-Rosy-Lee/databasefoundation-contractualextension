/*
 * (C) 2025 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Library.Entity;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Repository;
using YuLinTu.Library.Business;
using System.Collections.ObjectModel;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 添加属性组窗体
    /// </summary>
    public partial class DictionaryGroup : InfoPageBase
    {
        #region Field

        /// <summary>
        /// 全局变量:判断是添加属性组还是修改属性组
        /// </summary>
        public bool isAdd;

        /// <summary>
        /// 存放左侧所有属性信息
        /// </summary>
        public ObservableCollection<Dictionary> groups;

        /// <summary>
        /// 私有变量，当前字典实体对象
        /// </summary>
        public Dictionary currentDictionary;

        /// <summary>
        /// 数据字典业务
        /// </summary>
        public DictionaryBusiness business;

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext dbContext;

        #endregion

        #region Property

        /// <summary>
        /// 实体属性
        /// </summary>
        public Dictionary CurrentDictionary
        {
            get { return currentDictionary; }
            set
            {
                currentDictionary = value;
                this.DataContext = CurrentDictionary;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 初始化窗体，有参构造方法—传入是否添加
        /// </summary>
        public DictionaryGroup(bool isAdd, ObservableCollection<Dictionary> groups)
        {
            InitializeComponent();
            this.isAdd = isAdd;
            this.groups = groups;
            business = new DictionaryBusiness();
            dbContext = DataBaseSourceWork.GetDataBaseSource();
            business.Station = dbContext == null ? null : dbContext.CreateDictWorkStation();
            business.DbContext = dbContext;
        }

        #endregion

        #region Method

        /// <summary>
        /// 分组码是否存在
        /// </summary>
        private bool IsExsit(string groupCode)
        {
            if (string.IsNullOrEmpty(groupCode))
            {
                return true;
            }
            foreach (var dictGroup in groups)
            {
                if (groupCode.Equals(dictGroup.GroupCode))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Events

        /// <summary>
        /// 响应添加或者编辑属性组确定按钮(数据库的操作)
        /// </summary>
        private void btnGroup_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync(goCallback =>
            {
                if (currentDictionary == null)
                    return false;
                return GoConfirm();
            }, completedCallback =>
            {
                Close(true);
            });
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <returns></returns>
        private bool GoConfirm()
        {
            bool result = true;
            try
            {
                if (dbContext == null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ShowBox("添加属性组", DataBaseSourceWork.ConnectionError);
                    }));
                    return false;
                }
                if (CheckCommitData())
                {
                    if (isAdd)
                    {
                        business.AddDict(currentDictionary);
                    }
                    else
                    {
                        business.ModifyDict(currentDictionary);
                    }
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Dispatcher.Invoke(new Action(() =>
                {
                    ShowBox("添加属性组", "添加属性组失败!");
                }));
                YuLinTu.Library.Log.Log.WriteException(this, "GoConfirm(添加属性组失败!)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 检查提交数据
        /// </summary>
        /// <returns></returns>
        private bool CheckCommitData()
        {
            bool right = true;
            if (business == null)
            {
                business = new DictionaryBusiness(dbContext);
            }
            bool repeat = business.IsGroupCodeReapet(currentDictionary.GroupCode, currentDictionary.ID);

            //此处做输入值判断   
            Dispatcher.Invoke(new Action(() =>
            {
                if (repeat)
                {
                    ShowBox("添加属性组", "已经存在该分组码!");
                    right = false;
                }
                else if (string.IsNullOrEmpty(currentDictionary.GroupCode))
                {
                    ShowBox("添加属性组", "分组码不能为空!");
                    right = false;
                }
            }));
            return right;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        public void ShowBox(string title, string msg)
        {
            if (Workpage == null)
            {
                return;
            }
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }

        #endregion
    }
}
