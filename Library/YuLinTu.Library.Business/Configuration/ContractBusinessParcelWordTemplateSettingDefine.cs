using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    public partial class ContractBusinessParcelWordSettingDefine
    {
        /// <summary>
        /// 是否使用横板模板
        /// </summary>
        public bool HorizontalVersion
        {
            get { return _HorizontalVersion; }
            set { _HorizontalVersion = value; NotifyPropertyChanged(() => HorizontalVersion); }
        }

        private bool _HorizontalVersion;

        /// <summary>
        /// 左右置换
        /// </summary>
        public bool IsLeftToRight
        {
            get { return _IsLeftToRight; }
            set { _IsLeftToRight = value; NotifyPropertyChanged(() => IsLeftToRight); }
        }

        private bool _IsLeftToRight;

        /// <summary>
        /// 首页行数
        /// </summary>
        public int RowCount1
        {
            get { return _RowCount1; }
            set { _RowCount1 = value; NotifyPropertyChanged(() => RowCount1); }
        }

        private int _RowCount1;

        /// <summary>
        /// 首页列数
        /// </summary>
        public int ColCount1
        {
            get { return _ColCount1; }
            set { _ColCount1 = value; NotifyPropertyChanged(() => ColCount1); }
        }

        private int _ColCount1;

        /// <summary>
        /// 后续页行数
        /// </summary>
        public int RowCount2
        {
            get { return _RowCount2; }
            set { _RowCount2 = value; NotifyPropertyChanged(() => RowCount2); }
        }

        private int _RowCount2;

        /// <summary>
        /// 后续页列数
        /// </summary>
        public int ColCount2
        {
            get { return _ColCount2; }
            set { _ColCount2 = value; NotifyPropertyChanged(() => ColCount2); }
        }

        private int _ColCount2;

        /// <summary>
        /// 示意图最大地块数量
        /// </summary>
        public int MaxLandNum
        {
            get { return _MaxLandNum; }
            set { _MaxLandNum = value; NotifyPropertyChanged(() => MaxLandNum); }
        }

        private int _MaxLandNum;

        /// <summary>
        /// 示意图扩展页行数
        /// </summary>
        public int ExtendRowCount
        {
            get { return _ExtendRowCount; }
            set { _ExtendRowCount = value; NotifyPropertyChanged(() => ExtendRowCount); }
        }

        private int _ExtendRowCount;

        /// <summary>
        /// 示意图扩展页列数
        /// </summary>
        public int ExtendColCount
        {
            get { return _ExtendColCount; }
            set { _ExtendColCount = value; NotifyPropertyChanged(() => ExtendColCount); }
        }

        private int _ExtendColCount;

        /// <summary>
        /// 是否固定扩展页图片高/宽度
        /// </summary>
        public bool IsFixedExtendLandGeoWord
        {
            get { return _IsFixedExtendLandGeoWord; }
            set { _IsFixedExtendLandGeoWord = value; NotifyPropertyChanged(() => IsFixedExtendLandGeoWord); }
        }

        private bool _IsFixedExtendLandGeoWord;

        /// <summary>
        /// 扩展页图片宽度
        /// </summary>
        public double ExtendLandGeoWordWidth
        {
            get { return _ExtendLandGeoWordWidth; }
            set { _ExtendLandGeoWordWidth = value; NotifyPropertyChanged(() => ExtendLandGeoWordWidth); }
        }

        private double _ExtendLandGeoWordWidth;

        /// <summary>
        /// 扩展页图片高度
        /// </summary>
        public double ExtendLandGeoWordHeight
        {
            get { return _ExtendLandGeoWordHeight; }
            set { _ExtendLandGeoWordHeight = value; NotifyPropertyChanged(() => ExtendLandGeoWordHeight); }
        }

        private double _ExtendLandGeoWordHeight;
    }
}