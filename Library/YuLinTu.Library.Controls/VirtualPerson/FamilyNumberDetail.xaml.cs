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

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 户号编码详情对话框
    /// </summary>
    public partial class FamilyNumberDetail : InfoPageBase
    {
        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public FamilyNumberDetail(int maxNum, int minNum, List<int> missNums)
        {
            InitializeComponent();
            DataContext = this;
            maxNumber.Text = maxNum.ToString();
            minNumber.Text = minNum.ToString();
            int count = 1;   //计数器
            if (missNums == null || missNums.Count() == 0)
            {
                missNumbers.Text = VirtualPersonInfo.MissNumnberNo;
            }
            else
            {
                foreach (var num in missNums)
                {
                    if (count < missNums.Count())
                    {
                        missNumbers.Text += num.ToString() + "、";
                    }
                    else
                    {
                        missNumbers.Text += num.ToString();
                    }
                    count++;
                }
            }
        }

        #endregion

        #region Method

        #endregion

        #region Event

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion

    }
}
