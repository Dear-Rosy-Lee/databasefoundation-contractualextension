/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 申请表设置
    /// </summary>
    public partial class ConcordApplicationTableSetting : InfoPageBase
    {
        #region Properties

        public ConcordApplicationSet DateTimeSetting { get; set; }

        public CollectivityTissue Tissue { get; set; }

        public ConcordBusiness ContractConcordBusiess { get; set; }

        public string FileDir { get; set; }

        public bool PrintView { get; set; }
        public int MaxBookNumber { get; set; }

        /// <summary>
        /// 是否导出成功
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }


        #endregion

        public ConcordApplicationTableSetting()
        {
            InitializeComponent();
            DataContext = this;
            DateTimeSetting = new ConcordApplicationSet();
            Confirm += ContractConcordSetPage_Confirm;
        }

        protected override void OnInitializeGo()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MaxBookNumber = ContractConcordBusiess.GetMaxNumber() + 1;
                //DateTimeSetting.ApplicationBookNumber = GenerateNumber();
                DateTimeSetting.ApplicationBookNumber = MaxBookNumber.ToString();
                DateTimeSetting.YearNumber = DateTime.Now.Year.ToString();
            }));
        }

        protected override void OnInitializeCompleted()
        {
            this.DataContext = this;
        }
        //public ConcordApplicationTableSetting(ConcordBusiness concordBusiess,CollectivityTissue tissue)
        //{
        //    InitializeComponent();           
        //    
        //}

        #region Methods

        private string GenerateNumber()
        {
            List<ContractRequireTable> tabs = ContractConcordBusiess.GetTissueRequireTable(Tissue.Code);
            int num = 1;
            string numString = string.Empty;
            if (tabs == null || tabs.Count == 0)
            { }
            else
            {
                do
                {
                    numString = num.ToString();
                    bool found = false;
                    for (int i = 0; i < tabs.Count; i++)
                        if (tabs[i].Number == numString)
                        {
                            found = true;
                            break;
                        }

                    if (!found)
                        break;

                    num++;
                } while (num < 1000);
            }
            string Number = numString;
            while (ContractConcordBusiess.GetRequireTable(num.ToString()) != null)
            {
                num++;
            }
            Number = num.ToString();
            return Number;
        }

        /// <summary>
        /// 申请日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caEndDateBox_Click(object sender, RoutedEventArgs e)
        {
            caEndDate.IsEnabled = (bool)caEndDateBox.IsChecked;
        }

        /// <summary>
        /// 确定
        /// </summary>
        private void mbtnDateOK_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync();
        }

        private void ContractConcordSetPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    //mbtnDateOK.IsEnabled = false;

                    if (txtYearNumber.Text == string.Empty)
                    {
                        ShowBox("设置", "请输入年号!", eMessageGrade.Warn);
                        e.Parameter = false;
                        return;
                    }
                    if (txtBookNumber.Text == string.Empty)
                    {
                        ShowBox("设置", "请输入申请书编号!", eMessageGrade.Warn);
                        e.Parameter = false;
                        return;
                    }
                    DateTimeSetting.ApplicationBookNumber = txtBookNumber.Text;
                    DateTimeSetting.YearNumber = txtYearNumber.Text;
                    DateTimeSetting.CheckDate = caEndDate.Value;
                    if (PrintView)
                    {
                        Flag = ContractConcordBusiess.PrintApplicationFirst(Tissue, CurrentZone, PrintView, applicationSet: DateTimeSetting);
                    }
                    else
                    {
                        Flag = ContractConcordBusiess.PrintApplicationFirst(Tissue, CurrentZone, false, fileName: FileDir, applicationSet: DateTimeSetting);
                    }
                    e.Parameter = true;
                }));

                //if (PrintView)
                //{
                //    Flag = ContractConcordBusiess.PrintApplicationFirst(Tissue, CurrentZone, PrintView, applicationSet: DateTimeSetting);
                //}
                //else 
                //{
                //    Flag = ContractConcordBusiess.PrintApplicationFirst(Tissue, CurrentZone, false, fileName: FileDir, applicationSet: DateTimeSetting);
                //}
                //e.Parameter = true;
            }
            catch
            {
                e.Parameter = false;
            }
        }
        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }

        #endregion
    }
}
