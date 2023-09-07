using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 新建图层组对话框
    /// </summary>
    public partial class GroupLayerNewDialog : Form
    {
        #region Fields

        private string layerGroupName = "";

        private double maxScale = 0;

        private double minScale = 0;

        private bool layerGroupVisiable = true;

        #endregion

        #region Properties

        /// <summary>
        /// 最大的比例尺
        /// </summary>
        public Double MaxScale 
        {
            get 
            {
                return maxScale; 
            }
            set
            { 
                maxScale = value;
                this.TxtBoxLGMaxScale.Text = maxScale.ToString();
            }
        }

        /// <summary>
        /// 最小比例尺
        /// </summary>
        public Double MinScale 
        {
            get
            {
                return minScale;
            }
            set
            {
                minScale = value;
                this.TxtBoxLGMinScale.Text = minScale.ToString();
            }
        }

        /// <summary>
        /// 组图层的可见性
        /// </summary>
        public bool LayerGroupVisiable 
        {
            get { return layerGroupVisiable; }
        }

        /// <summary>
        /// 组图层名称
        /// </summary>
        public string LayerGroupName 
        {
            get { return layerGroupName; }
        }

        #endregion

        public GroupLayerNewDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtBoxLGName.Text)) 
            {
                MessageBox.Show("图层组名称不能为空！", "参数错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            layerGroupName=TxtBoxLGName.Text.Trim();

            Regex doubleRegex = new Regex(@"^[-+]?\d+(\.\d+)?$");
            if (!(doubleRegex.IsMatch(TxtBoxLGMaxScale.Text.Trim()) && doubleRegex.IsMatch(TxtBoxLGMinScale.Text.Trim())))
            {
                MessageBox.Show("请检查输入是否为数值类型！", "参数错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            maxScale = double.Parse(TxtBoxLGMaxScale.Text.Trim());
            minScale = double.Parse(TxtBoxLGMinScale.Text.Trim());
            if (maxScale <= minScale) 
            {
                MessageBox.Show("最大比例尺不能小于最小比例尺！", "参数错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
