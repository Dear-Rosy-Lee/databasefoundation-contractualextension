/*
 * (C) 2015 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Windows;
using YuLinTu.Library.Business;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 数据字典 数据显示控件界面
    /// </summary>
    public partial class DictManagerPanel : UserControl
    {
        #region Field

        /// <summary>
        /// 私有变量,用于存储属性数据集合
        /// </summary>
        private ObservableCollection<Dictionary> listDictionaries;

        /// <summary>
        /// 私有变量,用于存储所选分组项信息
        /// </summary>
        private Dictionary groupDict;

        /// <summary>
        /// 私有变量,用于响应TaskQueue的一些机制
        /// </summary>
        private TaskQueue taskQueueForDictionaryLoading;

        /// <summary>
        /// 数据字典业务
        /// </summary>
        private DictionaryBusiness business;

        #endregion

        #region Property

        /// <summary>
        /// 工作页属性
        /// </summary>
        public IWorkpage ThePage { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 初始化数据显示控件窗体
        /// </summary>
        public DictManagerPanel()
        {
            InitializeComponent();
            taskQueueForDictionaryLoading = new TaskQueueDispatcher(Dispatcher);
            //IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            // business.Station = dbContext == null ? null : dbContext.CreateDictWorkStation();
            //business.DbContext = dbContext;
        }

        #endregion

        #region Method 自定义

        /// <summary>
        /// 自定义方法:将数据集合绑定到MetroDataGrid控件上
        /// </summary>
        /// <param name="groupname">分组名称</param>
        public void SetDataGridSource(Dictionary group)
        {
            try
            {
                this.groupDict = group;
                if (groupDict == null)
                {
                    dictGrid.Roots = null;
                    return;
                }
                business = new DictionaryBusiness(DataBaseSourceWork.GetDataBaseSource());
                var listDict = business.GetByGroupCodeDict(groupDict.GroupCode);
                listDict.Remove(listDict.Where(c => c.Code == null || c.Code == "").FirstOrDefault());
                if (listDict != null)
                {
                    listDictionaries = new ObservableCollection<Dictionary>();
                    foreach (var dictionary in listDict)
                    {
                        listDictionaries.Add(dictionary);
                    }
                    dictGrid.Roots = listDictionaries;
                }

            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ShowBox("数据字典", string.Format("获取此组下的属性失败,具体原因为:{0}", ex), eMessageGrade.Error);

                    //ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                    //{
                    //    Header = "数据字典",
                    //    Message = string.Format("获取此组下的属性失败,具体原因为:{0}", ex),
                    //    MessageGrade = eMessageGrade.Error,
                    //});
                }));
            }
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <param name="header">消息对话框头标题</param>
        /// <param name="message">显示的消息</param>
        private bool CanContinue(string header, string message)
        {
            if (groupDict == null)
            {
                ShowBox(header, message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <param name="header">消息对话框头标题</param>
        /// <param name="message">显示的消息</param>
        /// <param name="dict">当前选择的属性信息</param>
        private bool CanContinue(string header, string message, Dictionary dict)
        {
            if (dict == null)
            {
                ShowBox(header, message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 消息提示框
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Infomation, Action<bool?, eCloseReason> action = null)
        {
            ThePage.Page.ShowDialog(new TabMessageBoxDialog
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            }, action);
        }

        /// <summary>
        /// 属性编码是否为数字
        /// </summary>
        private bool IsNumberic(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }
            foreach (char c in code)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 属性编码是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool IsExsit(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return true;
            }
            business = new DictionaryBusiness(DataBaseSourceWork.GetDataBaseSource());
            var list = business.GetByGroupCodeDict(groupDict.GroupCode);
            foreach (var item in list)
            {
                if (code.Equals(item.Code))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region method 按钮响应

        /// <summary>
        /// 添加属性
        /// </summary>
        public void Add()
        {
            if (!CanContinue(DictionaryInfo.DictAdd, DictionaryInfo.AddNoSelectedGroup))
            {
                return;
            }
            var dlg = new PropertyGridDialog();
            dlg.Header = "添加属性项";
            dlg.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/plus.png"));
            dlg.Object = new Dictionary();
            dlg.PropertyGrid.InitializeProperty += (s, a) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    business = new DictionaryBusiness(DataBaseSourceWork.GetDataBaseSource());
                    if (a.PropertyDescriptor.Name == "GroupName")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/ReadOnly.png"));
                        a.PropertyDescriptor.Value = groupDict.GroupName;
                        var b = new Binding("Value");
                        b.Source = a.PropertyDescriptor;
                        var txtGropName = new MetroTextBox();
                        txtGropName.IsReadOnly = true;
                        txtGropName.PaddingWatermask = new Thickness(5, 0, 0, 0);
                        txtGropName.VerticalContentAlignment = VerticalAlignment.Center;
                        txtGropName.SetBinding(TextBox.TextProperty, b);
                        txtGropName.SetBinding(TextBox.ToolTipProperty, b);
                        a.PropertyDescriptor.Designer = txtGropName;
                    }
                    else if (a.PropertyDescriptor.Name == "GroupCode")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/ReadOnly.png"));
                        a.PropertyDescriptor.Value = groupDict.GroupCode;
                        var b = new Binding("Value");
                        b.Source = a.PropertyDescriptor;
                        var txtGropCode = new MetroTextBox();
                        txtGropCode.IsReadOnly = true;
                        txtGropCode.PaddingWatermask = new Thickness(5, 0, 0, 0);
                        txtGropCode.VerticalContentAlignment = VerticalAlignment.Center;
                        txtGropCode.SetBinding(TextBox.TextProperty, b);
                        txtGropCode.SetBinding(TextBox.ToolTipProperty, b);
                        a.PropertyDescriptor.Designer = txtGropCode;
                    }
                    else if (a.PropertyDescriptor.Name == "Code")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Resources/ConstrainNumeric.png"));
                        var dict = business.GetByGroupCodeDict(groupDict.GroupCode).LastOrDefault();
                        if (dict == null || string.IsNullOrEmpty(dict.Code))
                        {
                            return;
                        }
                        int i = -1;
                        int.TryParse(dict.Code, out i);
                        if (i > 0)
                        {
                            i += 1;
                            a.PropertyDescriptor.Value = i.ToString();
                        }
                    }
                    else if (a.PropertyDescriptor.Name == "Name")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Resources/属性16.png"));
                    }
                    else if (a.PropertyDescriptor.Name == "AliseName")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Resources/属性16.png"));
                    }
                    else if (a.PropertyDescriptor.Name == "Comment")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Resources/属性16.png"));
                        var b = new Binding("Value");
                        b.Source = a.PropertyDescriptor;
                        var txtComment = new MetroTextBox();
                        txtComment.MaxLength = 200;
                        txtComment.Height = 100;
                        txtComment.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        txtComment.PaddingWatermask = new Thickness(5, 0, 0, 0);
                        txtComment.VerticalContentAlignment = VerticalAlignment.Top;
                        txtComment.AcceptsReturn = true;
                        txtComment.AcceptsTab = true;
                        txtComment.SetBinding(TextBox.TextProperty, b);
                        txtComment.SetBinding(TextBox.ToolTipProperty, b);
                        a.PropertyDescriptor.Designer = txtComment;
                    }
                }));
            };
            dlg.Confirm += (s, a) =>
            {
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        business = new DictionaryBusiness(DataBaseSourceWork.GetDataBaseSource());
                        var dictionary = dlg.Object as Dictionary;
                        if (IsExsit(dictionary.Code))
                        {
                            ShowBox("添加属性", string.Format(string.IsNullOrEmpty(dictionary.Code) ? "属性编码不能为空!" : "已经存在该属性编码!"), eMessageGrade.Infomation);

                            //ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                            //{
                            //    Header = "添加属性",
                            //    Message = string.Format(string.IsNullOrEmpty(dictionary.Code) ? "属性编码不能为空!" : "已经存在该属性编码!"),
                            //    MessageGrade = eMessageGrade.Infomation,
                            //});
                            return;
                        }
                        else if (!IsNumberic(dictionary.Code))
                        {
                            ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                            {
                                Header = "添加属性",
                                Message = string.Format(string.IsNullOrEmpty(dictionary.Code) ? "属性编码不能为空!" : "输入的属性编码不合法!"),
                                MessageGrade = eMessageGrade.Infomation,
                            });
                            return;
                        }
                        business.AddDict(dictionary);
                        listDictionaries.Add(dictionary);
                    }));
                    a.Parameter = true;
                }
                catch (Exception ex)
                {
                    a.Parameter = false;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ShowBox("数据字典", string.Format("添加属性项失败,具体原因为:{0}", ex), eMessageGrade.Error);

                        //ThePage.Page.ShowDialog(new TabMessageBoxDialog
                        //{
                        //    Header = "数据字典",
                        //    Message = string.Format("添加属性项失败,具体原因为:{0}", ex),
                        //    MessageGrade = eMessageGrade.Error,
                        //});
                    }));
                }
            };
            ThePage.Page.ShowDialog(dlg);
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        public void Edit()
        {
            var dictionary = dictGrid.SelectedItem as Dictionary;
            if (!CanContinue(DictionaryInfo.DictEdit, DictionaryInfo.EditNoSelected, dictionary))
            {
                return;
            }
            var dictTemp = dictionary.Clone() as Dictionary;
            var dlg = new PropertyGridDialog()
            {
                Header = "编辑属性",
                Icon = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/application--pencil.png")),
                Object = dictTemp,
            };
            dlg.PropertyGrid.InitializeProperty += (s, a) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (a.PropertyDescriptor.Name == "GroupCode")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/ReadOnly.png"));
                        var b = new Binding("Value");
                        b.Source = a.PropertyDescriptor;
                        var txtGroupCode = new MetroTextBox();
                        txtGroupCode.IsReadOnly = true;
                        txtGroupCode.PaddingWatermask = new Thickness(5, 0, 0, 0);
                        txtGroupCode.VerticalContentAlignment = VerticalAlignment.Center;
                        txtGroupCode.SetBinding(TextBox.TextProperty, b);
                        txtGroupCode.SetBinding(TextBox.ToolTipProperty, b);
                        a.PropertyDescriptor.Designer = txtGroupCode;
                    }
                    else if (a.PropertyDescriptor.Name == "GroupName")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/ReadOnly.png"));
                        var b = new Binding("Value");
                        b.Source = a.PropertyDescriptor;
                        var txtGroupName = new MetroTextBox();
                        txtGroupName.IsReadOnly = true;
                        txtGroupName.PaddingWatermask = new Thickness(5, 0, 0, 0);
                        txtGroupName.VerticalContentAlignment = VerticalAlignment.Center;
                        txtGroupName.SetBinding(TextBox.TextProperty, b);
                        txtGroupName.SetBinding(TextBox.ToolTipProperty, b);
                        a.PropertyDescriptor.Designer = txtGroupName;
                    }
                    else if (a.PropertyDescriptor.Name == "Code")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Resources/ConstrainNumeric.png"));
                        var b = new Binding("Value");
                        b.Source = a.PropertyDescriptor;
                        var txtCode = new MetroTextBox();
                        txtCode.IsReadOnly = false;
                        txtCode.PaddingWatermask = new Thickness(5, 0, 0, 0);
                        txtCode.VerticalContentAlignment = VerticalAlignment.Center;
                        txtCode.SetBinding(TextBox.TextProperty, b);
                        txtCode.SetBinding(TextBox.ToolTipProperty, b);
                        a.PropertyDescriptor.Designer = txtCode;
                    }
                    else if (a.PropertyDescriptor.Name == "Name")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Resources/属性16.png"));
                    }
                    else if (a.PropertyDescriptor.Name == "AliseName")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Resources/属性16.png"));
                    }
                    else if (a.PropertyDescriptor.Name == "Comment")
                    {
                        a.PropertyDescriptor.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Resources/属性16.png"));
                        var b = new Binding("Value");
                        b.Source = a.PropertyDescriptor;
                        var txtComment = new MetroTextBox();
                        txtComment.MaxLength = 200;
                        txtComment.Height = 100;
                        txtComment.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        txtComment.PaddingWatermask = new Thickness(5, 0, 0, 0);
                        txtComment.VerticalContentAlignment = VerticalAlignment.Top;
                        txtComment.AcceptsReturn = true;
                        txtComment.AcceptsTab = true;
                        txtComment.SetBinding(TextBox.TextProperty, b);
                        txtComment.SetBinding(TextBox.ToolTipProperty, b);
                        a.PropertyDescriptor.Designer = txtComment;
                    }
                }));
            };
            dlg.Confirm += (s, a) =>
            {
                try
                {
                    if (!IsNumberic(dictTemp.Code))
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            ShowBox("添加属性", string.Format(string.IsNullOrEmpty(dictTemp.Code) ? "属性编码不能为空!" : "输入的属性编码不合法!"), eMessageGrade.Infomation);

                            //ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                            //{
                            //    Header = "添加属性",
                            //    Message = string.Format(string.IsNullOrEmpty(dictTemp.Code) ? "属性编码不能为空!" : "输入的属性编码不合法!"),
                            //    MessageGrade = eMessageGrade.Infomation,
                            //});
                        }));
                        return;
                    }
                    business = new DictionaryBusiness(DataBaseSourceWork.GetDataBaseSource());
                    dictTemp.Modifier = "**";
                    dictTemp.ModifierTime = DateTime.Today;
                    business.ModifyDict(dictTemp);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dictionary.CopyPropertiesFrom(dictTemp);
                    }));
                    a.Parameter = true;
                }
                catch (Exception ex)
                {
                    a.Parameter = false;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ShowBox("数据字典", string.Format("编辑属性项失败,具体原因为:{0}", ex), eMessageGrade.Error);

                        //ThePage.Page.ShowDialog(new TabMessageBoxDialog
                        //{
                        //    Header = "数据字典",
                        //    Message = string.Format("编辑属性项失败,具体原因为:{0}", ex),
                        //    MessageGrade = eMessageGrade.Error,
                        //});
                    }));
                }
            };
            ThePage.Page.ShowDialog(dlg);
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        public void Delete()
        {
            var dictionary = dictGrid.SelectedItem as Dictionary;
            if (!CanContinue(DictionaryInfo.DictDel, DictionaryInfo.DelNoSelected, dictionary))
            {
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, a) =>
            {
                if (b == false)
                {
                    return;
                }
                business = new DictionaryBusiness(DataBaseSourceWork.GetDataBaseSource());
                business.DelDict(dictionary);
                Dispatcher.Invoke(new Action(() =>
                {
                    listDictionaries.Remove(dictionary);
                }));
            });
            ShowBox("删除属性", "是否删除选中的属性信息?", eMessageGrade.Infomation, action);

            //var dlg = new TabMessageBoxDialog
            //{
            //    Header = "删除属性",
            //    Message = "是否删除选中的属性信息?",
            //    MessageGrade = eMessageGrade.Infomation,
            //};
            //ThePage.Page.ShowMessageBox(dlg, (b, a) =>
            //{
            //    if (b == false)
            //    {
            //        return;
            //    }
            //    business = new DictionaryBusiness(DataBaseSource.GetDataBaseSource());
            //    business.DelDict(dictionary);
            //    Dispatcher.Invoke(new Action(() =>
            //    {
            //        listDictionaries.Remove(dictionary);
            //    }));
            //});
        }

        /// <summary>
        /// 清空属性显示
        /// </summary>
        public void Clear()
        {
            if (!CanContinue(DictionaryInfo.DictClear, DictionaryInfo.ClearNoSelectedGroup))
            {
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (b == false)
                {
                    return;
                }
                business = new DictionaryBusiness(DataBaseSourceWork.GetDataBaseSource());
                var listDict = business.GetByGroupCodeDict(groupDict.GroupCode);
                listDict.Remove(listDict.Where(c => c.Code == null || c.Code == "").FirstOrDefault());
                business.DelRangeFromDict(listDict);
                Dispatcher.Invoke(new Action(() =>
                {
                    listDictionaries.Clear();
                }));
            });
            ShowBox("清空属性", "是否清空所选属性组下的所有属性信息?", eMessageGrade.Infomation, action);

            //var dlg = new TabMessageBoxDialog()
            //{
            //    Header = "清空属性",
            //    Message = "是否清空所选属性组下的所有属性信息?",
            //    MessageGrade = eMessageGrade.Infomation,
            //};
            //ThePage.Page.ShowDialog(dlg, (b, r) =>
            //{
            //    if (b == false)
            //    {
            //        return;
            //    }
            //    business = new DictionaryBusiness(DataBaseSource.GetDataBaseSource());
            //    var listDict = business.GetByGroupCodeDict(groupDict.GroupCode);
            //    listDict.Remove(listDict.Where(c => c.Code == null || c.Code == "").FirstOrDefault());
            //    business.DelRangeFromDict(listDict);
            //    Dispatcher.Invoke(new Action(() =>
            //    {
            //        listDictionaries.Clear();
            //    }));
            //});
        }

        /// <summary>
        /// 刷新属性显示
        /// </summary>
        public void Refresh()
        {
            if (groupDict == null || listDictionaries.Count == 0)
            {
                return;
            }
            taskQueueForDictionaryLoading.Cancel();
            taskQueueForDictionaryLoading.DoWithInterruptCurrent(go =>
            {
                var ds = DataSource.Create(TheBns.Current.GetDataSourceName());
                var workstation = new ContainerFactory(ds).CreateWorkstation<IDictionaryWorkStation, IDictionaryRepository>();
                var list = workstation.Get(c => c.GroupCode == groupDict.GroupCode);
                list.Remove(list.Where(c => c.Code == null || c.Code == "").FirstOrDefault());
                go.Instance.Argument.UserState = list;
            }, completed =>
            {
                var listDict = completed.Instance.Argument.UserState as List<Dictionary>;
                if (listDict == null)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    listDict.ForEach(c => listDictionaries.Add(c));
                }));
            }, terminated =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ShowBox("数据字典", string.Format("加载属性失败，错误详细信息为 {0}", terminated.Exception), eMessageGrade.Error);

                    //ThePage.Page.ShowDialog(new TabMessageBoxDialog
                    //{
                    //    Header = "数据字典",
                    //    Message = string.Format("加载属性失败，错误详细信息为 {0}", terminated.Exception),
                    //    MessageGrade = eMessageGrade.Error,
                    //});
                }));
            }, null, started =>
            {
                listDictionaries.Clear();
            }, null, null, null, groupDict);
        }

        #endregion

        #region Event

        /// <summary>
        /// 响应双击事件
        /// </summary>
        private void dictGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit();
        }

        #endregion

        #region Events 右键菜单

        /// <summary>
        /// 添加属性
        /// </summary>
        private void muAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        private void muEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        private void muDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        /// <summary>
        /// 清空属性
        /// </summary>
        private void muClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// 刷新属性
        /// </summary>
        private void muRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        #endregion
    }
}
