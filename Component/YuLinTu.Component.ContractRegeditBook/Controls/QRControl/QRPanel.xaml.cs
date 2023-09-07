using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThoughtWorks.QRCode.Codec;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.ContractRegeditBook.Controls.QRControl
{
    /// <summary>
    /// QRPanel.xaml 的交互逻辑
    /// </summary>
    public partial class QRPanel : UserControl
    {
        private bool isCreateControl = false;
        private double rowHeight = 36;
        private double ctry;

        bool IsMouseDown = false;
        Point mousePoint;
        object mouseCtrl = null;
        private bool isMoveEWMControl = false;

        List<QRContentControl> panelEWMList = new List<QRContentControl>();
        List<QRContentControl> contentEWMList = new List<QRContentControl>();
        List<ControlEntity> entityControlList = new List<ControlEntity>();
        List<Line> lineList = new List<Line>();
        Dictionary<string, string> QRContentDic = new Dictionary<string, string>();
        List<QRValueSettingEntity> qrContentValueList = new List<QRValueSettingEntity>();
        SettingEntity entityList = new SettingEntity();
        public int canvasRowNum = 6;
        private string settingPath = System.IO.Directory.GetCurrentDirectory() + "\\config\\QrSetting.xml";


        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(name));
        //    }
        //}

        public int CanvasRowNumber
        {
            get { return canvasRowNum; }
            set
            {
                canvasRowNum = value;

                //if (mainCanvas != null)
                //{
                //    DrawLine();
                //}
            }
        }
        public QRPanel()
        {
            InitializeComponent();
            ItemSource = GetComboboxItemSource();
            mainCanvas.Loaded += MainCanvas_Loaded;
            
        }

        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }
        private readonly static DependencyProperty ItemSourceProperty = DependencyProperty.Register(
            nameof(ItemSource),
            typeof(IEnumerable),
            typeof(QRPanel),
            new PropertyMetadata(null, ItemSourceChange));



        private static void ItemSourceChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as QRPanel;
            control.RefreshItemSource();
        }

        public void RefreshItemSource()
        {
            foreach (var item in panelEWMList)
            {
                if (ItemSource != null)
                {
                    item.comboBox.ItemsSource = ItemSource;
                }
            }
        }


        /// <summary>
        /// canvas加载完成时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            if (mainCanvas != null)
            {
                SetCanvasHeight();
                DrawLine();
                AddControl();
            }
        }


        #region 方法

        public List<string> GetComboboxItemSource()
        {
            List<string> comboboxList = new List<string>();
            var dic = entityList.QRContentValueList;
            if(dic==null)
            {
                return null;
            }
            foreach (var key in dic)
            {
                if (!comboboxList.Contains(key.Name))
                {
                    comboboxList.Add(key.Name);
                }
            }

            return comboboxList;
        }

        /// <summary>
        /// 设置主画板的高度
        /// </summary>
        public void SetCanvasHeight()
        {
            int canvasHeight = (int)(canvasRowNum * rowHeight);
            if (mainCanvas.IsLoaded)
            {
                mainCanvas.Height = (double)canvasHeight;
            }
        }

        /// <summary>
        /// 绘制面板的水平线线
        /// </summary>
        private void DrawLine()
        {
            var width = 600;
            for (int i = 1; i <= canvasRowNum; i++)
            {
                Line titleLine = new Line();
                titleLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                titleLine.X1 = 1;
                titleLine.Y1 = i * rowHeight;
                titleLine.X2 = width;
                titleLine.Y2 = i * rowHeight;
                titleLine.HorizontalAlignment = HorizontalAlignment.Left;
                titleLine.VerticalAlignment = VerticalAlignment.Center;
                lineList.Add(titleLine);
                this.mainCanvas.Children.Add(titleLine);
            }

        }

        /// <summary>
        /// 通过读取配置文件添加二维码内容控件
        /// </summary>
        private void AddControl()
        {
            //读取二维码设置xml字符串
            //var entityXml = XmlHelper<ControlEntity>.ReadXml(System.IO.Directory.GetCurrentDirectory() + "\\wem.xml");
            //将字符串转化为实体
            //var entityList = XmlHelper<ControlEntity>.XmlToEntityList(entityXml);
            if (!File.Exists(settingPath))
            {
                SettingEntity settingEntity = new SettingEntity();
                settingEntity.QRSize = 100;
                settingEntity.TotalRow = 6;
                var valueList = new QRValueSettingEntity();
                valueList.Name = "承包方姓名";
                valueList.Value = "ContractorName";
                settingEntity.QRContentValueList = new List<QRValueSettingEntity>() { valueList };
                var ewmSavePath = settingPath;
                ToolSerialization.SerializeXml(ewmSavePath, settingEntity);
            }
            entityList =(SettingEntity)ToolSerialization.DeserializeXml(settingPath, typeof(SettingEntity));
            if (entityControlList == null)
            {
                return;
            }
            qrContentValueList = entityList.QRContentValueList;
            ewmSize.Value = entityList.QRSize;
            rowNum.Value = entityList.TotalRow;
            foreach (var item in entityList.QRContentValueList)
            {
                if (!QRContentDic.Keys.Contains(item.Name))
                {
                    QRContentDic.Add(item.Name, item.Value);
                }
            }
            //将实体添加到主画板中
            foreach (var item in entityList.ControlList)
            {
                QRContentControl ewm = new QRContentControl(GetComboboxItemSource());
                ewm.comboBox.Text =QRContentDic.FirstOrDefault(i=>i.Value == item.ComboboxValue).Key ;
                ewm.textcontent.Text = item.textValue;
                ewm.StartPoint = new Point(item.startX, item.startY);
                ewm.LocationRow = item.LocationRowNum;
                ewm.MouseLeftButtonDown += EWMControl_MouseLeftButtonDown;
                ewm.MouseMove += Window_MouseMove;
                ewm.MouseUp += Window_MouseUp;
                panelEWMList.Add(ewm);
                mainCanvas.Children.Add(ewm);
                Canvas.SetLeft(ewm, ewm.StartPoint.X);
                Canvas.SetTop(ewm, ewm.StartPoint.Y);
            }

        }

        #endregion

        #region 添加控件

        private void controlBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.SizeAll;
        }
        private void controlBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isCreateControl)
            {
                this.Cursor = Cursors.Arrow;
            }

        }

        /// <summary>
        /// 鼠标左键按下时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controlBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isCreateControl = true;
            this.Cursor = Cursors.Cross;
        }

        /// <summary>
        /// 鼠标左键弹起时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isCreateControl)
            {
                //获取鼠标相当于主画板的 位置
                var controlX = e.GetPosition(mainCanvas).X;
                var controlY = e.GetPosition(mainCanvas).Y;
                QRContentControl cont = new QRContentControl(GetComboboxItemSource());
                //计算该坐标所在的行数
                var row = Math.Ceiling(controlY / rowHeight);
                cont.LocationRow = ((int)row) >= 0 ? ((int)row) : 0;
                controlY = (row - 1) * rowHeight + (rowHeight - 32) / 2;//设置控件的纵坐标
               // cont.comboBox.ItemsSource = ItemSource;
                //给控件添加点击事件
                cont.MouseLeftButtonDown += EWMControl_MouseLeftButtonDown;
                cont.MouseMove += Window_MouseMove;
                cont.MouseUp += Window_MouseUp;
                if (!ControlIsMovedBorder(new Point(controlX, controlY)))
                {
                    double overX = controlX + 180 - mainCanvas.ActualWidth;
                    double overY = controlY + 32 - mainCanvas.ActualHeight;
                    if (overX >= 0)
                    {
                        controlX = controlX - overX;
                    }
                    if (overY >= 0)
                    {
                        controlY = controlY - overY;
                    }
                }
                //设置控件的开始坐标
                cont.StartPoint = new Point(controlX, controlY);
                //将控件加入在主画板
                mainCanvas.Children.Add(cont);

                panelEWMList.Add(cont);
                //设置画板相对于主画板的位置
                Canvas.SetLeft(cont, controlX);
                Canvas.SetTop(cont, controlY);
            }
            //创建控件结束
            isCreateControl = false;
            this.Cursor = Cursors.Arrow;
        }
        #endregion


        #region 移动控件

        /// <summary>
        /// 鼠标左键按下时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EWMControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IsMouseDown = true;
                //获取鼠标的坐标
                mousePoint = e.GetPosition(this.mainCanvas);
                mouseCtrl = sender;
                this.Cursor = Cursors.Cross;
                //将其他的设置为不选中状态
                foreach (var item in panelEWMList)
                {
                    if (item.IsSelected)
                    {
                        item.IsSelected = false;
                    }
                }
                (mouseCtrl as QRContentControl).IsSelected = true;
                (mouseCtrl as QRContentControl).IsMoveing = true;
            }
        }

        /// <summary>
        /// 鼠标移动时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //如果左键处于按下状态
            if (IsMouseDown)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    //获取鼠标坐标
                    Point theMousePoint = e.GetPosition(this.mainCanvas);
                    double x = theMousePoint.X - (mousePoint.X - Canvas.GetLeft(((UIElement)mouseCtrl)));
                    double y = theMousePoint.Y - (mousePoint.Y - Canvas.GetTop(((UIElement)mouseCtrl)));

                    //判断控件是否到达边界
                    if (ControlIsMovedBorder(new Point(x, y)))
                    {
                        (sender as QRContentControl).StartPoint = new Point(x, y);
                        Canvas.SetLeft((UIElement)mouseCtrl, x);
                        Canvas.SetTop((UIElement)mouseCtrl, y);
                        ctry = e.GetPosition(mainCanvas).Y;
                        mousePoint = theMousePoint;
                    }
                    isMoveEWMControl = true;
                }
            }
        }

        /// <summary>
        /// 鼠标左键弹起时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in panelEWMList)
            {
                if (item.IsSelected)
                {
                    item.IsMoveing = false;
                    break;
                }
            }
            if (IsMouseDown && isMoveEWMControl)
            {
                //获取当前的行
                var row = Math.Ceiling(ctry / rowHeight);
                ctry = (row - 1) * rowHeight + (rowHeight - (((QRContentControl)mouseCtrl).ActualHeight)) / 2;
                (sender as QRContentControl).LocationRow = (int)row >= 0 ? (int)row : 0;//设置当前行数
                (sender as QRContentControl).StartPoint = new Point((sender as QRContentControl).StartPoint.X, ctry);//设置开始坐标
                Canvas.SetTop((UIElement)mouseCtrl, ctry);//设置控件相对主画板的相对位置
                this.Cursor = Cursors.Arrow;
            }
            IsMouseDown = false;
            isMoveEWMControl = false;
        }

        /// <summary>
        /// 清楚操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in panelEWMList)
            {
                if (item.IsSelected)
                {
                    mainCanvas.Children.Remove(item);//主画板移除控件
                    panelEWMList.Remove(item);//集合移除控件
                    break;
                }
            }
        }

        public void SaveTemplate()
        {
            //清空实体集合
            entityControlList.Clear();
            foreach (var item in panelEWMList)
            {
                ControlEntity entity = new ControlEntity();
                entity.startX = item.StartPoint.X;
                entity.startY = item.StartPoint.Y;
                entity.textValue = (item.textcontent.Text).Trim();
                if (!string.IsNullOrEmpty(item.comboBox.Text))
                {
                    entity.ComboboxValue = QRContentDic[(item.comboBox.Text)];
                }
                else
                {
                    entity.ComboboxValue = "";
                }
                entity.LocationRowNum = item.LocationRow;
                entityControlList.Add(entity);
            }
            SettingEntity settingEntity = new SettingEntity();
            settingEntity.ControlList = entityControlList;
            settingEntity.QRSize = (int)ewmSize.Value;
            settingEntity.TotalRow = (int)rowNum.Value;
            settingEntity.QRContentValueList = qrContentValueList;
            var ewmSavePath = settingPath;
            ToolSerialization.SerializeXml(ewmSavePath, settingEntity);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //清空实体集合
            entityControlList.Clear();
            foreach (var item in panelEWMList)
            {
                ControlEntity entity = new ControlEntity();
                entity.startX = item.StartPoint.X;
                entity.startY = item.StartPoint.Y;
                entity.textValue = (item.textcontent.Text).Trim();
                if (!string.IsNullOrEmpty(item.comboBox.Text))
                {
                    entity.ComboboxValue = QRContentDic[(item.comboBox.Text)];
                }
                else
                {
                    entity.ComboboxValue = "";
                }
                entity.LocationRowNum = item.LocationRow;
                entityControlList.Add(entity);
            }
            SettingEntity settingEntity = new SettingEntity();
            settingEntity.ControlList = entityControlList;
            settingEntity.QRSize = (int)ewmSize.Value;
            settingEntity.TotalRow = (int)rowNum.Value;
            settingEntity.QRContentValueList= qrContentValueList; 
            var ewmSavePath = settingPath;
            ToolSerialization.SerializeXml(ewmSavePath, settingEntity);
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateEWMBtn_Click(object sender, RoutedEventArgs e)
        {
            dispalyTextPop.IsOpen = true;
            displayImg.Visibility = Visibility.Visible;
            //displayImg.Visibility = Visibility.Collapsed;
            BitmapImage bt;
            string enCodeString = PickText();
            //CreatePicture(enCodeString);
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeVersion = 0;
            System.Drawing.Bitmap btmap = qrCodeEncoder.Encode(enCodeString,Encoding.UTF8);
            bt = BitmapToBitmapImage(btmap);
            var top = (200 - Convert.ToDouble(ewmSize.Value)) / 2;
            displayImg.Margin = new Thickness(0, top, 0, 0);
            displayImg.Width = Convert.ToDouble(ewmSize.Value);
            displayImg.Height = Convert.ToDouble(ewmSize.Value);
            displayImg.Source = bt;
            // Write(ewmContent);
            //创建二维码图片
            //CreatePicture(PickText());
        }
        #endregion

        public void Write(string content)
        {
            FileStream fs = new FileStream("E:\\ak.txt", FileMode.Create);
            //获得字节数组
            byte[] data = System.Text.Encoding.Default.GetBytes(content);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// 判断控件是否到了主画板的边缘
        /// </summary>
        /// <param name="p">控件的起始坐标</param>
        /// <returns></returns>
        private bool ControlIsMovedBorder(Point p)
        {
            if (p.X < 0 || p.X + 180 >= mainCanvas.ActualWidth || p.Y < 0 || p.Y + 32 >= mainCanvas.ActualHeight)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="nr">二维码字符串</param>
        private void CreatePicture(string nr)
        {
            try
            {
                System.Drawing.Bitmap bt;
                string enCodeString = nr;
                BitmapImage btmap;
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                bt = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);
                string fileName = DateTime.Now.ToString("yyyyMMdd");
                fileName = "E:" + @"\" + fileName + ".jpg";
                //bt.Save(fileName);
                
                btmap = BitmapToBitmapImage(bt);
                var top = (200 - Convert.ToDouble(ewmSize.Value)) / 2;
                displayImg.Margin = new Thickness(0, top, 0, 0);
                displayImg.Width = Convert.ToDouble(ewmSize.Value);
                displayImg.Height = Convert.ToDouble(ewmSize.Value);
                displayImg.Source = btmap;
                //if (System.IO.File.Exists(fileName))
                //{
                //    InsertImageCellWithoutPading("EnCode", fileName, 50, 50);
                //}
                // System.IO.File.Delete(fileName);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 提取文字内容
        /// </summary>
        /// <returns></returns>
        private string PickText()
        {
            StringBuilder sb = new StringBuilder();
            var controlOrderList = panelEWMList.OrderBy(i => i.LocationRow).ThenBy(j=>j.StartPoint.X).ToList();
            int currentRow = 1;
            QRContentControl lastEWMc = new QRContentControl(GetComboboxItemSource());

            for (int i = 0; i < controlOrderList.Count(); i++)
            {
                string content = "";
                //如果当前处于同一行
                if (controlOrderList[i].LocationRow == currentRow)
                {
                    //上一个控件和当前控件处于同一行
                    if (lastEWMc.LocationRow == controlOrderList[i].LocationRow)
                    {
                        //计算2个控件之间的距离，以12个像素为一个空格
                        int distance = (int)((controlOrderList[i].StartPoint.X - lastEWMc.StartPoint.X - 180) / 12);
                        //添加空格
                        for (int k = 0; k < distance; k++)
                        {
                            content += " ";
                        }
                    }
                    //获取内容
                    content += controlOrderList[i].textcontent.Text + controlOrderList[i].comboBox.Text;
                }
                else
                {
                    //获取距离上一个控件几行，并添加换行符
                    for (int j = 0; j < controlOrderList[i].LocationRow - currentRow; j++)
                    {
                        //content += "\n";
                        content += Environment.NewLine;
                        //content += "&Chr(10)&";
                    }
                    content += controlOrderList[i].textcontent.Text + controlOrderList[i].comboBox.Text;
                    currentRow = controlOrderList[i].LocationRow;
                }
                lastEWMc = controlOrderList[i];
                sb.Append(content);
            }
            return sb.ToString();

        }


        /// <summary>
        /// bitmap格式转换BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                //bitmap.Save(ms, bitmap.RawFormat);
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CanvasRowNumber = Convert.ToInt32(e.NewValue);
            if (mainCanvas != null)
            {
                //mainCanvas.Children.Clear();
                foreach (var item in lineList)
                {
                    mainCanvas.Children.Remove(item);
                }
                SetCanvasHeight();
                lineList.Clear();
                DrawLine();
                //AddControl();
            }
        }
    }
}
