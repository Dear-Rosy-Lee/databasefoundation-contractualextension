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
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// InitializeVirtualPersonRelationship.xaml 的交互逻辑
    /// </summary>
    public partial class InitializeVirtualPersonRelationship : InfoPageBase
    {
        #region Fields

        //private int familyIndex;//承包方索引值
        public string contents { get; set; }//查找内容
        public string result { get; set; }//替换为

        #endregion

        #region Propertys
        ///// <summary>
        ///// 工作空间
        ///// </summary>
        //public IWorkpage ThePage { get; set; }
        ///// <summary>
        ///// 定义委托
        ///// </summary>
        //public delegate void TaskViewerShowDelegate();
        ///// <summary>
        ///// 显示任务
        ///// </summary>
        //public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        /// <summary>
        /// 数据实例
        /// </summary>
        public IDbContext DataSource { get; set; }

        /// <summary>
        /// 查找地域
        /// </summary>
        public Zone CurrentZone { get; set; }
        //public List<VirtualPerson> vps { get; set; }


        /// <summary>
        /// 是否成功处理
        /// </summary>
        public bool Success { get; set; }

        #endregion
        public InitializeVirtualPersonRelationship()
        {
            InitializeComponent();
            cbRelationship.ItemsSource = InitalizeAllRelation();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync(goCallback =>
            {
                return SetAndCheckValue();
            }, completedCallback =>
            {
                Close(true);
            });


            //TaskInitialVPRelationShipArgument argument = new TaskInitialVPRelationShipArgument();
            //argument.VPS = vps;
            //argument.Database = DataSource;
            //argument.CurrentZone = CurrentZone;
            //argument.ReplaceName = txt_content.Text.Trim();
            //argument.ChooseName = cbRelationship.Text;
            //TaskInitialVPRelationShipOperation task = new TaskInitialVPRelationShipOperation();
            //task.Argument = argument;
            //task.Name = "家庭关系替换";
            //task.Description = "家庭关系替换";
            //task.Completed += new TaskCompletedEventHandler((o, t) =>
            //{
            //});
            //ThePage.TaskCenter.Add(task);
            //if (ShowTaskViewer != null)
            //{
            //    ShowTaskViewer();
            //}
            //task.StartAsync();
        }

        /// <summary>
        /// 设置和检查值
        /// </summary>
        private bool SetAndCheckValue()
        {
            bool canContinue = true;

            Dispatcher.Invoke(new Action(() =>
            {
                contents = txt_content.Text.Trim();
                result = cbRelationship.Text.Trim();
            }));

            if (string.IsNullOrEmpty(contents))
            {
                MessageBox.Show("查找内容不允许为空!", "数据替换");
                return false;
            }
            if (contents == result)
            {
                MessageBox.Show("查找内容与替换内容不能相同!", "数据替换");
                return false;
            }
            //vps = null;
            //var vpstation = DataSource.CreateVirtualPersonStation<LandVirtualPerson>();
            ////vps = vpstation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            //if (vps == null || vps.Count == 0)
            //{
            //    MessageBox.Show("当前地域下没有数据需要替换!", "数据替换");
            //    return false;
            //}
            return canContinue;
        }
        #region Methods

        /// <summary>
        /// 初始化家庭关系
        /// </summary>
        private List<string> InitalizeAllRelation()
        {
            List<string> list = new List<string>();
            list.Add("本人");
            list.Add("户主");
            list.Add("配偶");
            list.Add("夫");
            list.Add("妻");
            list.Add("父亲");
            list.Add("母亲");
            list.Add("子");
            list.Add("女");
            list.Add("独生子");
            list.Add("长子");
            list.Add("次子");
            list.Add("三子");
            list.Add("四子");
            list.Add("五子");
            list.Add("养子或继子");
            list.Add("其他儿子");
            list.Add("女婿");
            list.Add("独生女");
            list.Add("长女");
            list.Add("次女");
            list.Add("三女");
            list.Add("四女");
            list.Add("五女");
            list.Add("养女或继女");
            list.Add("儿媳");
            list.Add("其他女儿");
            list.Add("孙子");
            list.Add("孙女");
            list.Add("外孙子");
            list.Add("外孙女");
            list.Add("孙媳妇或外孙媳妇");
            list.Add("孙女婿或外孙女婿");
            list.Add("曾孙子或外曾孙子");
            list.Add("曾孙女或外曾孙女");
            list.Add("其他孙子、孙女或外孙子、外孙女");
            list.Add("公公");
            list.Add("婆婆");
            list.Add("岳父");
            list.Add("岳母");
            list.Add("继父或养父");
            list.Add("继母或养母");
            list.Add("其他父母关系");
            list.Add("祖父");
            list.Add("祖母");
            list.Add("外祖父");
            list.Add("外祖母");
            list.Add("配偶的祖父母或外祖父母");
            list.Add("曾祖父");
            list.Add("曾祖母");
            list.Add("配偶的曾祖父母或外曾祖父母");
            list.Add("其他祖父母或外祖父母关系");
            list.Add("兄");
            list.Add("嫂");
            list.Add("弟");
            list.Add("弟媳");
            list.Add("姐姐");
            list.Add("姐夫");
            list.Add("妹妹");
            list.Add("妹夫");
            list.Add("其他兄弟姐妹");
            list.Add("伯父");
            list.Add("伯母");
            list.Add("叔父");
            list.Add("婶母");
            list.Add("舅父");
            list.Add("舅母");
            list.Add("姨父");
            list.Add("姨母");
            list.Add("姑父");
            list.Add("姑母");
            list.Add("堂兄弟、堂姐妹");
            list.Add("表兄弟、表姐妹");
            list.Add("侄子");
            list.Add("侄女");
            list.Add("外甥");
            list.Add("外甥女");
            list.Add("其他亲属");
            list.Add("非亲属");
            return list;
        }

        #endregion

        #region Event


        #endregion
    }
}
