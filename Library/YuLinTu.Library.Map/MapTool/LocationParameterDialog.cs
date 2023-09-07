using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 获取定位坐标参数对话框
    /// 
    /// </summary>
    public partial class LocationParameterDialog : Form
    {
        public LocationParameterDialog()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
        }

        #region Fields

        private double x = 0;

        private double y = 0;

        #endregion

        #region Properties

        /// <summary>
        /// X坐标
        /// 暂时只考虑单位米
        /// </summary>
        public double X { get { return x; } }

        /// <summary>
        /// Y坐标
        /// 暂时只考虑单位米
        /// </summary>
        public double Y { get { return y; } }

        /// <summary>
        /// 坐标应处于的范围
        /// x、y坐标超出此范围 将会给与超出提示
        /// </summary>
        public Envelope FullExtend { set; get; }

        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            Regex doubleRegex = new Regex(@"^[-+]?\d+(\.\d+)?$");
            if (!(doubleRegex.IsMatch(textBox1.Text.Trim()) && doubleRegex.IsMatch(textBox2.Text.Trim()))) 
            {
                MessageBox.Show("请检查输入是否为数值类型！","参数错误",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            x = double.Parse(textBox1.Text.Trim());
            y = double.Parse(textBox2.Text.Trim());

            if (!XYExtendCheck(x, y, FullExtend)) 
            {
                if (MessageBox.Show("输入坐标不在当前地图范围内！忽略继续？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) 
                {
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 判断x、y是否在指定范围内
        /// </summary>
        /// <returns>True（在）/False(不在)</returns>
        private bool XYExtendCheck(double x,double y,Envelope extend)
        {
            bool result = true;
            if (x < extend.MinX || x > extend.MaxX || y < extend.MinY || y > extend.MaxY) 
            {
                result = false;
            }
            return result;
        }
    }
}
