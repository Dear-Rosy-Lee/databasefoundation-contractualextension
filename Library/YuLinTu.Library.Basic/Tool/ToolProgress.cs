using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public class ToolProgress
    {
        #region Fields

        private int entityCount;
        private int convertIndex;
        private int convertNumber;
        private int percentCount;
        private double percent;
        private double convertCount;

        public delegate void PostProgressDelegate(int progress);
        public event PostProgressDelegate OnPostProgress;

        #endregion

        #region Methods

        /// <summary>
        /// 初始数据
        /// </summary>
        /// <param name="listcount">数据总条数</param>
        /// <param name="count">剩余需要完成的条数</param>
        public void InitializationPercent(int listcount, int count)
        {
            if (listcount < 1)
            {
                if (OnPostProgress != null)
                    OnPostProgress(100);
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
        /// 计算百分比
        /// </summary>
        public void DynamicProgress()
        {
            convertCount += percent;
            convertNumber++;

            if (convertNumber >= entityCount)
            {
                if (OnPostProgress != null)
                    OnPostProgress(100);
                return;
            }

            if (entityCount >= percentCount)
            {
                if (convertCount > (double)convertIndex && convertIndex < 91)
                {
                    if (OnPostProgress != null)
                        OnPostProgress(100 - percentCount + convertIndex);
                    convertIndex++;
                }
            }
            else
            {
                if (OnPostProgress != null)
                    OnPostProgress(100 - percentCount + (int)convertCount);
            }

        }

        #endregion
    }
}
