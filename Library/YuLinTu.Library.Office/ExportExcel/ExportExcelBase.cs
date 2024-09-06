// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Office
{
    /// <summary>
    /// 导出Excel基类
    /// </summary>
    [Serializable]
    abstract public class ExportExcelBase : ExcelBase
    {
        #region Fields

        protected double collWidth;
        public delegate void PostProgressDelegate(int progress, object userState = null);
        public event PostProgressDelegate PostProgressEvent;

        public delegate void PostErrorInfoDelegate(string message);
        public event PostErrorInfoDelegate PostErrorInfoEvent;

        public delegate void PostExceptionInfoDelegate(string message);
        public event PostExceptionInfoDelegate PostExceptionInfoEvent;


        private int entityCount;
        private int convertIndex;
        private int convertNumber;
        private int percentCount;
        private double percent;
        private double convertCount;

        #endregion

        #region Methods

        /// <summary>
        /// 获取替换字符
        /// </summary>
        virtual public void GetReplaceMent()
        {
            EmptyReplacement = "";
        }

        #region SetRange

        /// <summary>
        /// 设置Range信息
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        public void SetRangeInfo(string start, string end, string value, double height, double size, bool bold)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, height);
            SetRangeFont(start, end, (int)size, 266, bold, false);
        }

        /// <summary>
        /// 设置Range与宽度
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        public void SetWidthAndRange(string start, string end, string value, double height, double size, bool bold)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, height);
            SetRangeFont(start, end, (int)size, 266, bold, false);
        }

        /// <summary>
        /// 设置Range与宽度
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        /// <param name="width"></param>
        public void SetWidthAndRange(string start, string end, string value, double height, double size, bool bold, double width)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, height);
            int rowStartIndex = InitalizeLetter(start);
            int rowEndIndex = InitalizeLetter(end);
            SetRangeWidth(rowStartIndex, rowEndIndex, width);
            SetRangeFont(start, end, (int)size, 266, bold, false);
        }

        /// <summary>
        /// 设置范围
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="rowHeight"></param>
        /// <param name="fontSize"></param>
        /// <param name="value"></param>
        /// <param name="bold"></param>
        public void SetRange(string start, string end, double rowHeight, double fontSize, string value, bool bold)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, rowHeight);
            SetRangeFont(start, end, (int)fontSize, 266, bold, false);
        }

        /// <summary>
        /// 设置Range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        public void SetRangRightTop(string start, string end, string value, double height, double size, bool bold)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, height);
            SetRangeFont(start, end, (int)size, 266, bold, false);
        }

        /// <summary>
        /// 设置Range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        public void SetRangRight(string start, string end, string value, double height, double size, bool bold)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, height);
            SetRangeFont(start, end, (int)size, 266, bold, false);
        }

        /// <summary>
        /// 设置左侧Range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="rowHeight"></param>
        /// <param name="fontSize"></param>
        /// <param name="value"></param>
        /// <param name="bold"></param>
        public void SetRangLeft(string start, string end, double rowHeight, double fontSize, string value, bool bold)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, rowHeight);
            SetRangeFont(start, end, (int)fontSize, 266, bold, false);
        }

        /// <summary>
        /// 设置Range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="fontSize"></param>
        /// <param name="value"></param>
        /// <param name="bold"></param>
        public void SetRange(string start, string end, double fontSize, string value, bool bold)
        {
            InitalizeRangeValue(start, end, value);
            SetRangeFont(start, end, (int)fontSize, 266, bold, false);
        }

        /// <summary>
        /// 供导出人口Excel使用
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        public void SetRange(string start, string end, double rowHeight, string value)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, rowHeight);
        }

        /// <summary>
        /// 供导出公示结果用
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        public void SetRangPublishExcelUse(string start, string end, double rowHeight, string value, int setUnderLineStartIndex1, int setUnderLineLenth1, int setUnderLineStartIndex2, int setUnderLineLenth2)
        {
            InitalizeRangeValuePublishExcelUse(start, end, value, setUnderLineStartIndex1, setUnderLineLenth1, setUnderLineStartIndex2, setUnderLineLenth2);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, rowHeight);
        }

        /// <summary>
        /// 供导出公示结果用
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="rowHeight"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        /// <param name="halign"> -1 0 1 代表左中右</param>
        /// <param name="aalign">-1 0 1 代表上中下</param>
        /// <param name="value"></param>
        public void SetRange(string start, string end, double rowHeight, double size, bool bold, int halign, int aalign, string value)
        {
            InitalizeRangeValue(start, end, value);
            int startIndex = InitalizeIndex(start);
            int endIndex = InitalizeIndex(end);
            SetRangeHeight(startIndex, endIndex, rowHeight);
            SetRangeFont(start, end, 0, 266, bold, false);
            SetRangeAlignment(start, end, halign, aalign);
        }

        #endregion

        #region protected

        /// <summary>
        /// 报告进度
        /// </summary>
        /// <param name="progress"></param>
        protected void PostProgress(int progress, object userState = null)
        {
            if (PostProgressEvent != null)
                PostProgressEvent(progress, userState);
        }

        /// <summary>
        /// 报告错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool PostErrorInfo(string message)
        {
            if (PostErrorInfoEvent != null)
                PostErrorInfoEvent(message);
            else
                throw new Exception(message);
            return false;
        }

        /// <summary>
        /// 报告异常信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool PostExceptionInfo(string message)
        {
            if (PostExceptionInfoEvent != null)
                PostExceptionInfoEvent(message);

            return false;
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        public void SetLineType(string end)
        {
            SetRangeLineStyle("A1", end, 0, true);
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        public void SetLineType(string end, bool alignment = true)
        {
            SetRangeLineStyle("A1", end, 0, alignment);
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        public void SetLineTypes(string start, string end)
        {
            SetRangeLineStyle(start, end, 0, true);
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        public void SetLineType(string start, string end, bool alignment = true)
        {
            SetRangeLineStyle(start, end, 0, alignment);
        }

        /// <summary>
        /// 清空区域
        /// </summary>
        public void ClearRang()
        {
            CleasAll();
        }

        /// <summary>
        /// 设置当前sheet
        /// </summary>
        /// <param name="index"></param>
        public void SetSheet(int index)
        {
            SetSheetIndex(index);
        }

        /// <summary>
        /// 写信息
        /// </summary>
        public override void Write()
        {
        }

        /// <summary>
        /// 读信息
        /// </summary>
        public override void Read()
        {
        }

        #endregion

        #region Percent

        /// <summary>
        /// 初始化百分比
        /// </summary>
        /// <param name="listcount"></param>
        /// <param name="count"></param>
        protected void InitializationPercent(int listcount, int count)
        {
            if (listcount < 1)
            {
                PostProgress(100);
                return;
            }
            percentCount = count;
            entityCount = listcount;
            convertIndex = 1;
            percent = (double)percentCount / (double)entityCount;
            convertCount = 0.0;
            convertNumber = 0;
        }

        /// <summary>
        /// 动态报告进程
        /// </summary>
        protected void DynamicProgress()
        {
            convertCount += percent;
            convertNumber++;

            if (convertNumber == entityCount)
            {
                PostProgress(100);
                return;
            }

            if (entityCount >= percentCount)
            {
                if (convertCount > (double)convertIndex && convertIndex < 91)
                {
                    PostProgress(100 - percentCount + convertIndex);
                    convertIndex++;
                }
            }
            else
            {
                PostProgress(100 - percentCount + (int)convertCount);
            }
        }

        /// <summary>
        /// 初始化户主名称
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public string InitalizeFamilyName(string familyName, bool except = true)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                return "";
            }
            if (!except)
            {
                return familyName;
            }
            string number = GetAllNumberWithInString(familyName);
            if (!string.IsNullOrEmpty(number))
            {
                return familyName.Replace(number, "");
            }
            int index = familyName.IndexOf("(");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            index = familyName.IndexOf("（");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            return familyName;
        }

        /// <summary>
        /// 获取字符串中所有数字字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public string GetAllNumberWithInString(string value)
        {
            string number = string.Empty;
            foreach (char item in value)
            {
                if (item >= 48 && item <= 58)
                {
                    number += item;
                }
            }
            return number;
        }

        public string GetDate()
        {
            return string.Format("{0: yyyy 年 MM 月 dd 日}", Date == null ? DateTime.Now : Date.Value);
        }

        #endregion

        #endregion
    }
}
