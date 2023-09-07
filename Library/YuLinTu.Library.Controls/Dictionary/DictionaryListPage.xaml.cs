/*
 * (C) 2015 鱼鳞图公司版权所有，保留所有权利
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
using System.IO;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Collections.ObjectModel;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// MetroListBox控件界面
    /// </summary>
    public partial class DictionaryListPage : UserControl
    {
        #region Fields

        /// <summary>
        /// 定义委托方法
        /// </summary>
        /// <param name="groupName">分组名称</param>
        public delegate void SetDataGridSource(Dictionary groupArray);

        /// <summary>
        /// 全局变量,用于本地存储分组码和分组名称
        /// </summary>
        public ObservableCollection<Dictionary> listGroup;

        /// <summary>
        /// 定义变量，实现TaskQueue的机制
        /// </summary>
        private TaskQueue taskQueueForGroupLoading;

        /// <summary>
        /// 数据字典业务
        /// </summary>
        private DictionaryBusiness business;

        /// <summary>
        /// 当前选择的实体对象(属性组)
        /// </summary>
        private Dictionary currentDict;

        #endregion

        #region Property

        /// <summary>
        /// 委托属性
        /// </summary>
        public SetDataGridSource SetDataGrid { get; set; }

        /// <summary>
        /// 工作页属性
        /// </summary>
        public IWorkpage ListPageWorkPage { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数:初始化左边框选项卡控件中放置的MetroListBox控件
        /// </summary>
        public DictionaryListPage()
        {
            InitializeComponent();
            listPropertyGroup.ItemsSource = listGroup = new ObservableCollection<Dictionary>();
            taskQueueForGroupLoading = new TaskQueueDispatcher(Dispatcher);
            business = new DictionaryBusiness();
            IDbContext dbContext = YuLinTu.Library.Business.DataBaseSource.GetDataBaseSource();
            business.Station = dbContext == null ? null : dbContext.CreateDictWorkStation();
            business.DbContext = dbContext;
        }

        #endregion

        #region Events

        /// <summary>
        /// 响应ListBox控件的选中项事件
        /// </summary>
        public void listPropertyGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshListDictionary();
        }

        /// <summary>
        /// 装载左侧列表
        /// </summary>
        private void listPropertyGroup_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGroupDictionary();
        }

        #endregion

        #region Method 自定义

        /// <summary>
        /// 刷新右侧列表,字典属性显示
        /// </summary>
        public void RefreshListDictionary()
        {
            currentDict = listPropertyGroup.SelectedItem as Dictionary;
            if (SetDataGrid != null)
            {
                SetDataGrid(currentDict);
            }
        }

        /// <summary>
        /// 刷新左侧列表,字典属性组显示
        /// </summary>
        public void RefreshGroupDictionary()
        {
            taskQueueForGroupLoading.Cancel();
            taskQueueForGroupLoading.DoWithInterruptCurrent(go =>
            {
                var ds =DataBaseSourceWork.GetDataBaseSource();
                var workstation = new ContainerFactory(ds).CreateWorkstation<IDictionaryWorkStation, IDictionaryRepository>();
                //var dictionaries = workstation.Get(c => c.Code == null || c.Code == "");
                var dictionaries = workstation.GetAllFZM();
                go.Instance.Argument.UserState = dictionaries;
            }, completed =>
            {
                var dicts = completed.Instance.Argument.UserState as List<Dictionary>;
                dicts.ForEach(c => listGroup.Add(c));
            }, terminated =>
            {
                YuLinTu.Library.Log.Log.WriteException(this, "RefreshGroupDictionary(获取数据字典失败)", terminated.Exception.ToString());
                ShowBox("数据字典", string.Format("加载数据字典失败.连接数据库失败,请检查数据库连接路径"), eMessageGrade.Error);

                //ListPageWorkPage.Page.ShowDialog(new YuLinTu.Windows.Wpf.Metro.Components.TabMessageBoxDialog()
                //{
                //    Header = "数据字典",
                //    Message = string.Format("加载数据字典失败.连接数据库失败,请检查数据库连接路径"),
                //    MessageGrade = eMessageGrade.Error,
                //});
            }, null, started =>
            {
                listGroup.Clear();
            }, null, null, null, null);

            //每次刷新完毕即清空选中实体
            currentDict = null;
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool CanContiune(string header, string message, Dictionary dict)
        {
            if (dict == null)
            {
                ShowBox(header, message);
                return false;
            }
            return true;
        }

        /// <summary>
        ///  消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Infomation, Action<bool?, eCloseReason> action = null)
        {
            ListPageWorkPage.Page.ShowMessageBox(new TabMessageBoxDialog
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            }, action);
        }

        /// <summary>
        /// 分组码是否存在
        /// </summary>
        private bool IsExsit(string groupCode)
        {
            if (string.IsNullOrEmpty(groupCode))
            {
                return true;
            }
            foreach (var dictGroup in listGroup)
            {
                if (groupCode.Equals(dictGroup.GroupCode))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Method 响应按钮命令

        /// <summary>
        /// 添加属性组
        /// </summary>
        public void AddTool()
        {
            var dictGroupDlg = new DictionaryGroup(true, listGroup);
            dictGroupDlg.Workpage = ListPageWorkPage;
            dictGroupDlg.CurrentDictionary = new Dictionary();
            ListPageWorkPage.Page.ShowDialog(dictGroupDlg, (b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                var dict = dictGroupDlg.CurrentDictionary;
                //if (IsExsit(dict.GroupCode))
                //{
                //    Dispatcher.Invoke(new Action(() =>
                //    {
                //        ShowBox("添加属性组", string.Format(string.IsNullOrEmpty(dict.GroupCode) ? "分组码不能为空!" : "已经存在该分组码!"), eMessageGrade.Infomation);

                //        //ListPageWorkPage.Page.ShowMessageBox(new TabMessageBoxDialog
                //        //{
                //        //    Header = "添加属性组",
                //        //    Message = string.Format(string.IsNullOrEmpty(dict.GroupCode) ? "分组码不能为空!" : "已经存在该分组码!"),
                //        //    MessageGrade = eMessageGrade.Infomation,
                //        //});
                //    }));
                //    return;
                //}
                listGroup.Add(dict);
            });
        }

        /// <summary>
        /// 编辑属性组
        /// </summary>
        public void EditTool()
        {
            currentDict = listPropertyGroup.SelectedItem as Dictionary;
            if (!CanContiune(DictionaryInfo.DictEditGroup, DictionaryInfo.EditNoSelectedGroup, currentDict))
            {
                return;
            }
            var dictTemp = currentDict.Clone() as Dictionary;
            var dictGroupDlg = new DictionaryGroup(false, listGroup);
            dictGroupDlg.Workpage = ListPageWorkPage;
            dictGroupDlg.CurrentDictionary = dictTemp;
            ListPageWorkPage.Page.ShowMessageBox(dictGroupDlg, (b, r) =>
            {
                if (b == false)
                {
                    return;
                }
                dictTemp.Modifier = "Admin";
                dictTemp.ModifierTime = DateTime.Today;
                Dispatcher.Invoke(new Action(() =>
                {
                    currentDict.CopyPropertiesFrom(dictTemp);
                }));
            });
        }

        /// <summary>
        /// 删除属性组
        /// </summary>
        public void DelTool()
        {
            currentDict = listPropertyGroup.SelectedItem as Dictionary;
            if (!CanContiune(DictionaryInfo.DictDelGroup, DictionaryInfo.DelNoSelectedGroup, currentDict))
            {
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                try
                {
                    var list = business.GetByGroupCodeDict(currentDict.GroupCode);
                    int dictCounts = business.DelRangeFromDict(list);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        listGroup.Remove(currentDict);
                    }));
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ListPageWorkPage.Message.Send(this, new ModuleMsgArgs() { ID = 111111, DictionaryEntity = currentDict, Count = dictCounts });
                    }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ShowBox("数据字典", string.Format("删除属性组失败,错误信息为:{1}", ex), eMessageGrade.Error);

                        //ListPageWorkPage.Page.ShowDialog(new TabMessageBoxDialog
                        //{
                        //    Header = "数据字典",
                        //    Message = string.Format("删除属性组失败,错误信息为:{1}", ex),
                        //    MessageGrade = eMessageGrade.Error,
                        //});
                    }));
                }
            });
            ShowBox("删除属性组", "是否删除选择的属性组信息?", eMessageGrade.Infomation, action);

            //var delDictGroupDlg = new TabMessageBoxDialog()
            //{
            //    Header = "删除属性组",
            //    Message = "是否删除选择的属性组信息?",
            //    MessageGrade = eMessageGrade.Infomation,
            //};
            //ListPageWorkPage.Page.ShowMessageBox(delDictGroupDlg, (b, r) =>
            //{
            //    if (!(bool)b)
            //    {
            //        return;
            //    }
            //    try
            //    {
            //        var list = business.GetByGroupCodeDict(currentDict.GroupCode);
            //        int dictCounts = business.DelRangeFromDict(list);
            //        Dispatcher.Invoke(new Action(() =>
            //        {
            //            listGroup.Remove(currentDict);
            //        }));
            //        Dispatcher.Invoke(new Action(() =>
            //        {
            //            ListPageWorkPage.Message.Send(this, new ModuleMsgArgs() { ID = 111111, DictionaryEntity = currentDict, Count = dictCounts });
            //        }));
            //    }
            //    catch (Exception ex)
            //    {
            //        Dispatcher.Invoke(new Action(() =>
            //        {
            //            ListPageWorkPage.Page.ShowDialog(new TabMessageBoxDialog
            //            {
            //                Header = "数据字典",
            //                Message = string.Format("删除属性组失败,错误信息为:{1}", ex),
            //                MessageGrade = eMessageGrade.Error,
            //            });
            //        }));
            //    }
            //});
        }

        /// <summary>
        /// 刷新属性组
        /// </summary>
        public void RefreshTool()
        {
            RefreshGroupDictionary();
        }

        #endregion

        #region Events 右键菜单

        /// <summary>
        /// 添加属性组
        /// </summary>
        private void muAddTool_Click(object sender, RoutedEventArgs e)
        {
            AddTool();
        }

        /// <summary>
        /// 编辑属性组
        /// </summary>
        private void muEditTool_Click(object sender, RoutedEventArgs e)
        {
            EditTool();
        }

        /// <summary>
        /// 删除属性组
        /// </summary>
        private void muDelTool_Click(object sender, RoutedEventArgs e)
        {
            DelTool();
        }

        /// <summary>
        /// 刷新属性组
        /// </summary>
        private void muRefreshTool_Click(object sender, RoutedEventArgs e)
        {
            RefreshTool();
        }

        #endregion

    }
}
