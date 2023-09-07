using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 视图导航地图命令类
    /// </summary>
    public class MapComandExtendNavigation : DependencyObject
    {
        #region Field


        /// <summary>
        /// 地图控件
        /// </summary>
        private MapControl mapControl;

        /// <summary>
        /// 当前视图索引
        /// </summary>
        private int currentExtendIndex = -1;

        /// <summary>
        /// 是否为命令导航视图
        /// 如果此视图范围是由视图导航命令产生的就为true
        /// </summary>
        private bool isComandExtend = false;

        /// <summary>
        /// 最近的一次视图范围
        /// 屏蔽tGIS执行导航到某个范围（ZoomTo(Envelope ev)）时会产生的两次视图刷新
        /// </summary>
        private Envelope nearEnvelope;

        /// <summary>
        /// 是否继续往视图列表添加视图范围
        /// </summary>
        private bool isContinue = true;

        /// <summary>
        /// 下一视图的可执行性的依赖属性
        /// </summary>
        public static readonly DependencyProperty NextExtendEnableProperty;


        /// <summary>
        /// 上一视图的可执行性的依赖属性
        /// </summary>
        public static readonly DependencyProperty PreviousExtendEnableProperty;

        /// <summary>
        /// 视图列表
        /// </summary>
        private List<Envelope> ExtendList = new List<Spatial.Envelope>() { };

        /// <summary>
        /// 上一视图导航是否可用
        /// </summary>
        public Binding previousExtendEnableBinding;

        /// <summary>
        /// 下一视图导航是否可用
        /// </summary>
        public Binding nextExtendEnableBinding;

        #endregion

        #region Ctor

        static MapComandExtendNavigation()
        {
            NextExtendEnableProperty = DependencyProperty.Register("isNextExtendEnable", typeof(bool), typeof(MapComandExtendNavigation), new PropertyMetadata(false, OnEnableChanged));
            PreviousExtendEnableProperty = DependencyProperty.Register("isPreviousExtendEnable", typeof(bool), typeof(MapComandExtendNavigation), new PropertyMetadata(false, OnEnableChanged));
        }


        public MapComandExtendNavigation(MapControl mc)
        {
            mapControl = mc;
            mapControl.InitializeEnd += mapControl_InitializeEnd;

        }

        #endregion

        #region Properties

        private bool isNextExtendEnable
        {
            get
            {
                return (bool)GetValue(NextExtendEnableProperty);
            }
            set
            {
                SetValue(NextExtendEnableProperty, value);
            }
        }

        private bool isPreviousExtendEnable
        {
            get
            {
                return (bool)GetValue(PreviousExtendEnableProperty);
            }
            set
            {
                SetValue(PreviousExtendEnableProperty, value);
            }
        }

        /// <summary>
        /// 下一视图可用性的绑定属性
        /// </summary>
        public Binding NextExtendEnableBinding
        {
            get
            {
                if (nextExtendEnableBinding == null)
                {
                    nextExtendEnableBinding = new Binding("isNextExtendEnable") { Source = this, Mode = BindingMode.TwoWay };
                }
                return nextExtendEnableBinding;
            }
        }

        /// <summary>
        /// 上一视图可用性的绑定属性
        /// </summary>
        public Binding PreviousExtendEnableBinding
        {
            get
            {
                if (previousExtendEnableBinding == null)
                {
                    previousExtendEnableBinding = new Binding("isPreviousExtendEnable") { Source = this, Mode = BindingMode.TwoWay };
                }
                return previousExtendEnableBinding;
            }
        }



        #endregion

        #region Method--Public

        /// <summary>
        /// 返回上一视图
        /// </summary>
        public void PreviousExtend(object sender, RoutedEventArgs e)
        {
            if (currentExtendIndex == -1)
            {
                currentExtendIndex = ExtendList.Count - 1;
            }
            if (ExtendList.Count <= 1 || currentExtendIndex == 0)
            {
                return;
            }

            mapControl.ZoomTo(ExtendList[currentExtendIndex - 1]);
            currentExtendIndex--;

            if (currentExtendIndex == 0)
            {
                isPreviousExtendEnable = false;
            }

            isNextExtendEnable = true;
            isComandExtend = true;
            isContinue = false;
        }

        /// <summary>
        /// 返回下一视图
        /// </summary>
        public void NextExtend(object sender, RoutedEventArgs e)
        {
            if (currentExtendIndex == -1)
            {
                currentExtendIndex = ExtendList.Count - 1;
            }
            if (ExtendList.Count <= 1 || currentExtendIndex == ExtendList.Count - 1)
            {
                return;
            }
            mapControl.ZoomTo(ExtendList[currentExtendIndex + 1]);
            currentExtendIndex++;
            if (currentExtendIndex == ExtendList.Count - 1)
            {
                isNextExtendEnable = false;
            }
            isPreviousExtendEnable = true;
            isComandExtend = true;
            isContinue = false;
        }

        #endregion

        #region Method--Private


        /// <summary>
        /// 比较两个范围值
        /// </summary>
        private bool ComparableExtend(Envelope ev1, Envelope ev2)
        {
            if (ev1.MinX != ev2.MinX || ev1.MinY != ev2.MinY || ev1.MaxX != ev2.MaxX || ev1.MaxY != ev2.MaxY)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 地图发生改变
        /// </summary>
        private void MapControl_ExtendChanged(object sender, ExtendChangedEventArgs e)
        {
            // 不添加相邻视图范围相同的对象
            if (nearEnvelope == null)
            {
                nearEnvelope = (sender as MapControl).Extend;
            }
            else if (ComparableExtend(nearEnvelope, (sender as MapControl).Extend))
            {
                return;
            }
            nearEnvelope = (sender as MapControl).Extend;

            if (isComandExtend)
            {
                isComandExtend = false;
                return;
            }
            else if (isContinue)
            {
                ExtendList.Add((sender as MapControl).Extend);
                currentExtendIndex = ExtendList.Count - 1;
                if (ExtendList.Count > 1)
                {
                    isPreviousExtendEnable = true;
                }
            }
            else
            {
                Envelope ev = ExtendList[currentExtendIndex];
                ExtendList.Clear();
                ExtendList.Add(ev);
                ExtendList.Add((sender as MapControl).Extend);
                currentExtendIndex = 1;

                isContinue = true;
                isPreviousExtendEnable = true;
                isNextExtendEnable = false;
            }
        }

        /// <summary>
        /// 地图初始化完成
        /// </summary>
        private void mapControl_InitializeEnd(object sender, EventArgs e)
        {
            mapControl.ExtendChanged += MapControl_ExtendChanged;
        }

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion
    }
}
