/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 进度工具
    /// </summary>
    public class ToolProgress
    {
        #region Fields

        private int entityCount;
        private int convertIndex;
        private int convertNumber;
        private double percentCount;
        private double percent;
        private double convertCount;
        private double currentCount;  //当前百分比

        public delegate void PostProgressDelegate(int progress, string info = "");
        public event PostProgressDelegate OnPostProgress;

        #endregion

        #region Methods

        /// <summary>
        /// 初始数据
        /// </summary>
        /// <param name="listcount">数据总条数</param>
        /// <param name="count">百分比跨度</param>
        /// <param name="currentPercent">当前进度百分比</param>
        public void InitializationPercent(int listcount, double count, double currentPercent = 0.0)
        {
            if (listcount < 1)
            {
                if (OnPostProgress != null)
                    OnPostProgress(100);
                return;
            }
            percentCount = count;
            currentCount = currentPercent;
            entityCount = listcount;
            convertIndex = 1;
            percent = percentCount / (double)entityCount;
            convertCount = 0.0;
            convertNumber = 0;
        }

        /// <summary>
        /// 计算百分比
        /// </summary>
        public void DynamicProgress(string info = "")
        {
            convertCount += percent;
            convertNumber++;

            if (convertNumber >= entityCount)
            {
                if (OnPostProgress != null)
                    OnPostProgress((int)(currentCount + percentCount), info);
                return;
            }

            if (entityCount >= percentCount)
            {
                if (convertCount > (double)convertIndex && convertIndex < 91)
                {
                    if (OnPostProgress != null)
                        OnPostProgress((int)(currentCount + convertIndex), info);
                    convertIndex++;
                }
            }
            else
            {
                if (OnPostProgress != null)
                    OnPostProgress((int)(currentCount + (int)convertCount), info);
            }
        }

        #endregion
    }
}
