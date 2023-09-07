using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 界址线位置
    /// </summary>
    public enum eBoundaryLinePosition
    {
        /// <summary>
        /// 内
        /// </summary>
        [EntityEnumName("key41101", IsLanguageName = true)]
        Left = 1,

        /// <summary>
        /// 中
        /// </summary>
        [EntityEnumName("key41102", IsLanguageName = true)]
        Middle,

        /// <summary>
        /// 外
        /// </summary>
        [EntityEnumName("key41103", IsLanguageName = true)]
        Right,

        ///// <summary>
        ///// 未知
        ///// </summary>
        //[EntityEnumName("key41104", IsLanguageName = true)]
        //UnKnown
    }
}
