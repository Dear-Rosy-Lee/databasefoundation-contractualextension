/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 地块编码顺序号详情界面
    /// </summary>
    public partial class LandNumberDetail : InfoPageBase
    {
        #region Ctro

        /// <summary>
        /// 构造函数
        /// </summary>
        public LandNumberDetail(int minNum, int maxNum, List<int> missNums)
        {
            InitializeComponent();
            DataContext = this;
            txtMinNumber.Text = FillNumber( minNum.ToString());
            txtMaxNumber.Text = FillNumber( maxNum.ToString());
            if (missNums == null || missNums.Count == 0)
            {
                //没有缺失的地块编码顺序号
                txtMissNumbers.Text = ContractAccountInfo.LandNumberNoMissing;
            }
            else
            {
                int index = 1;  //缺失编号遍历索引
                int counts = missNums.Count();
                foreach (var num in missNums)
                {
                    if (index < counts)
                    {
                        txtMissNumbers.Text += FillNumber(num.ToString())+ "、";
                    }
                    else
                    {
                        txtMissNumbers.Text += FillNumber(num.ToString());
                    }
                    index++;
                }
            }
        }


        /// <summary>
        /// 填充满5位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string FillNumber(string value)
        {
            if (value.Length == 1)
            {
                return "0000" + value;
            }
            if (value.Length == 2)
            {
                return "000" + value;
            }
            if (value.Length == 3)
            {
                return "00" + value;
            }
            if (value.Length == 4)
            {
                return "0" + value;
            }
            return value;
        }

        #endregion

    }
}
