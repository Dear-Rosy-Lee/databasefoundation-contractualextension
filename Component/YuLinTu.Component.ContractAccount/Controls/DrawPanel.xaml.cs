using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using YuLinTu.Diagrams;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractAccount
{
    public partial class DrawPanel : UserControl, IDisposable
    {
        #region Properties


        public DiagramsView DiagramsView { get; set; }

        #endregion

        #region Events

        public static Type[] ShapeTypes = new Type[] {
            //typeof(ShapeBase),
            //typeof(ContainerShape),
            //typeof(TextShapeBase),
            typeof(PolylineShape),
            //typeof(PolygonShape),
            //typeof(RectangleShape),
            //typeof(RectanglePathShape),
            //typeof(EllipseShape),
            //typeof(TringleShape),
            //typeof(TrapezoidShape),
            typeof(TextShape),
            //typeof(ListTextShape),
            //typeof(DataGridShape),
            //typeof(ImageShape),
            //typeof(MapShape),
            //typeof(CompassShape),
            };
        #endregion

        #region Ctor

        public DrawPanel()
        {
            InitializeComponent();

            DataContext = this;

        }

        #endregion

        #region Methods

        public void Install(DiagramsView view)
        {
            DiagramsView = view;

            List<DrawItemContext> list = new List<DrawItemContext>();
            list.AddRange(ShapeTypes.
                Where(c =>
                    c != typeof(RectanglePathShape) &&
                    c != typeof(ShapeBase) &&
                    c != typeof(ContainerShape) &&
                    c != typeof(TextShapeBase) &&
                    c != typeof(PaperShape)).
                Select(c => new DrawItemContextShape(DiagramsView, (
                    Activator.CreateInstance(c) as ShapeBase),
                    c.GetAttribute<DisplayLanguageAttribute>() != null ?
                    c.GetAttribute<DisplayLanguageAttribute>().Name : c.FullName)));

            list.ForEach(c =>
            {
                c.Model.Width = 32;
                c.Model.Height = 32;

                if (c.Name == "地图")
                    c.ModelUI = new ImageTextItem() { Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/ViewWebLayoutView.png")) };
                else if (c.Name == "图片")
                    c.ModelUI = new ImageTextItem() { Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/PictureInsertFromFile.png")) };
                else if (c.Name == "文本")
                    c.ModelUI = new ImageTextItem() { Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/TextBoxDrawMenu.png")) };
                else if (c.Name == "列表")
                    c.ModelUI = new ImageTextItem() { Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/TextAlignCenter.png")) };
                else if (c.Name == "表格")
                    c.ModelUI = new ImageTextItem() { Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/CustomTablesGallery.png")) };
                else if (c.Name == "线")
                    c.ModelUI = new ImageTextItem()
                    {
                        Image = SymbolImageBuilder.GetImage(
                            new SimplePolylineSymbol()
                            {
                                StrokeColor = c.Model.BorderColor,
                                StrokeThickness = c.Model.BorderWidth,
                            }, 32, 32)
                    };
                else if (c.Name == "面")
                    c.ModelUI = new ImageTextItem()
                    {
                        Image = SymbolImageBuilder.GetImage(
                            new SimplePolygonSymbol()
                            {
                                BorderStrokeColor = c.Model.BorderColor,
                                BorderThickness = c.Model.BorderWidth,
                                BackgroundColor = c.Model.BackgroundColor,
                            }, 32, 32)
                    };
            });

            itemsControl.ItemsSource = list;

        }

        public void Dispose()
        {
            var list = itemsControl.ItemsSource as List<DrawItemContext>;
            if (list != null)
                foreach (var item in list)
                    if(item!=null) item.Dispose();
            
            DiagramsView = null;
        }

        #endregion
    }
}
