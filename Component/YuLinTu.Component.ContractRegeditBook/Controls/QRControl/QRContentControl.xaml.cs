using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace YuLinTu.Component.ContractRegeditBook.Controls.QRControl
{
    /// <summary>
    /// QRContentControl.xaml 的交互逻辑
    /// </summary>
    public partial class QRContentControl : UserControl
    {
        List<string> list = new List<string>();

        private bool isSelected;
        private Point startPoint;
        private int locationRow;
        private bool isMoveing;
        Rectangle rec = new Rectangle();
        Line lineV = new Line();
        Line lineH = new Line();

        public QRContentControl(List<string> itemsource)
        {
            InitializeComponent();
            list = itemsource;
            comboBox.ItemsSource = list;
        }
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (value)
                {
                    DrawRectangle();
                }
                else
                {
                    canvas.Children.Remove(rec);
                }
            }
        }

        /// <summary>
        /// 是否正在移动
        /// </summary>
        public bool IsMoveing
        {
            get { return isMoveing; }
            set
            {
                isMoveing = value;
                if (value)
                {
                    DrawCrossLine(startPoint);
                }
                else
                {
                    canvas.Children.Remove(lineH);
                    canvas.Children.Remove(lineV);
                }
            }
        }

        /// <summary>
        /// 右上角开始的坐标
        /// </summary>
        public Point StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }

        /// <summary>
        /// 处在的行数
        /// </summary>
        public int LocationRow
        {
            get { return locationRow; }
            set { locationRow = value; }
        }
        /// <summary>
        /// 化标准线
        /// </summary>
        /// <param name="p"></param>
        public void DrawCrossLine(Point p)
        {
            lineH.X1 = -50;
            lineH.X2 = 250;
            lineH.Y1 = 0;
            lineH.Y2 = 0;
            lineH.StrokeThickness = 2;
            lineH.Stroke = Brushes.Black;
            lineH.StrokeDashArray = new DoubleCollection() { 2, 3 }; ;

            lineV.X1 = 0;
            lineV.Y1 = -50;
            lineV.X2 = 0;
            lineV.Y2 = 100;
            lineV.StrokeThickness = 2;
            lineV.StrokeDashArray = new DoubleCollection() { 2, 3 };
            lineV.Stroke = Brushes.Black;
            if (canvas.Children.Contains(lineV) || canvas.Children.Contains(lineH))
            {
                canvas.Children.Remove(lineV);
                canvas.Children.Remove(lineH);
            }
            canvas.Children.Add(lineH);
            canvas.Children.Add(lineV);

        }

        /// <summary>
        /// 绘制选中框
        /// </summary>
        private void DrawRectangle()
        {

            //rec.Stroke = System.Windows.Media.Brushes.Red;
            Color color = Color.FromRgb(54, 107, 231);
            Brush brush = new SolidColorBrush(color);
            rec.Stroke = brush;
            rec.Width = 202;
            rec.Height = 28;
            rec.RadiusX = 5;
            rec.RadiusY = 5;
            canvas.Children.Add(rec);
        }
        /// <summary>
        /// combobox搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            List<string> mylist = new List<string>();
            if (string.IsNullOrEmpty(comboBox.Text))
            {
                comboBox.ItemsSource = list;
                return;
            }
            mylist = list.FindAll(delegate (string s)
            {
                return s.Contains(comboBox.Text.Trim());
            });
            comboBox.ItemsSource = mylist;
            comboBox.IsDropDownOpen = true;
        }

    }
}
