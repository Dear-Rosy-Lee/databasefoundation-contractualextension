using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Spatial;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    /// <summary>
    /// 提供空间要素的定义及其相关的功能。
    /// </summary>
    public sealed class FeatureObject : CDObject
    {
        #region Properties

        /// <summary>
        /// 获取或设置空间要素的实体。
        /// </summary>
        public object Object { get; set; }

        /// <summary>
        /// 获取或设置空间要素中空间字段的名称。
        /// </summary>
        /// 
        public string GeometryPropertyName { get; set; }

        /// <summary>
        /// 获取或设置空间要素中的空间数据。
        /// </summary>
        public Geometry Geometry { get; set; }

        #endregion

        #region Fields

        #endregion

        #region Ctor

        /// <summary>
        /// 创建 FeatureObject 的新实例。
        /// </summary>
        public FeatureObject()
        {
        }

        #endregion

        #region Methods

        #region Methods - System

        public override string ToString()
        {
            return string.Format("{0}, {1}", Object, Geometry);
        }

        #endregion

        #endregion
    }
}
