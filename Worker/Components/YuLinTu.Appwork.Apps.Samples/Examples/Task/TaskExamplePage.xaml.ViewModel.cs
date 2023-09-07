using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [View(typeof(TaskExamplePage))]
    public partial class TaskExamplePageViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description = "TaskExamplePage Content";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        #endregion

        #region Properties - System

        public ITheWorkpage Workpage { get; private set; }

        #endregion

        #endregion

        #region Commands

        #region Commands - Loaded

        public DelegateCommand CommandLoaded { get { return _CommandLoaded ?? (_CommandLoaded = new DelegateCommand(obj => OnLoaded(obj))); } }
        private DelegateCommand _CommandLoaded;

        #endregion

        #region Commands - StartSingleTask

        public DelegateCommand CommandStartSingleTask { get { return _CommandStartSingleTask ?? (_CommandStartSingleTask = new DelegateCommand(args => OnStartSingleTask(args), args => OnCanStartSingleTask(args))); } }
        private DelegateCommand _CommandStartSingleTask;

        private bool OnCanStartSingleTask(object args)
        {
            return true;
        }

        private void OnStartSingleTask(object args)
        {
            var task = new SingleTask();
            task.Argument = new SingleTaskArgument()
            {
                // 使用随机数来模拟用户的输入参数，在任务中将根据参数值的不同执行不同的逻辑
                Parameter2 = new Random((int)DateTime.Now.Ticks).Next(400) - 100,
            };

            Workpage.TaskCenter.Add(task);
            task.StartAsync();

            Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
        }

        #endregion

        #region Commands - StartGroupTask

        public DelegateCommand CommandStartGroupTask { get { return _CommandStartGroupTask ?? (_CommandStartGroupTask = new DelegateCommand(args => OnStartGroupTask(args), args => OnCanStartGroupTask(args))); } }
        private DelegateCommand _CommandStartGroupTask;

        private bool OnCanStartGroupTask(object args)
        {
            return true;
        }

        private void OnStartGroupTask(object args)
        {
            var task = new GroupTask();
            task.Argument = new GroupTaskArgument()
            {
                // 使用随机数来模拟需要创建的子任务的个数
                Parameter2 = new Random((int)DateTime.Now.Ticks).Next(9) + 1,
            };

            Workpage.TaskCenter.Add(task);
            task.StartAsync();

            Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
        }

        #endregion

        #region Commands - StartParallelGroupTask

        public DelegateCommand CommandStartParallelGroupTask { get { return _CommandStartParallelGroupTask ?? (_CommandStartParallelGroupTask = new DelegateCommand(args => OnStartParallelGroupTask(args), args => OnCanStartParallelGroupTask(args))); } }
        private DelegateCommand _CommandStartParallelGroupTask;

        private bool OnCanStartParallelGroupTask(object args)
        {
            return true;
        }

        private void OnStartParallelGroupTask(object args)
        {
            var task = new ParallelGroupTask();
            task.Argument = new ParallelGroupTaskArgument()
            {
                // 使用随机数来模拟需要创建的子任务的个数
                Parameter2 = new Random((int)DateTime.Now.Ticks).Next(9) + 1,
            };

            Workpage.TaskCenter.Add(task);
            task.StartAsync();

            Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
        }

        #endregion


        #endregion

        #region Ctor

        public TaskExamplePageViewModel(ITheWorkpage workpage) : base(workpage.Message)
        {
            Workpage = workpage;
        }

        #endregion

        #region Methods

        #region Methods - Public

        #endregion

        #region Methods - Override

        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        #endregion

        #region Methods - Private

        private void OnLoaded(object obj)
        {
            // 一般不使用该方法处理页面的加载过程，请使用 Message 文件中的
            // InstallWorkpageContent 初始化界面组件，
            // InstallAccountData 初始化用户数据。
        }

        #endregion

        #region Methods - System

        public void Dispose()
        {
            Workpage = null;
        }

        #endregion

        #endregion
    }
}
