using System;
using System.Windows;
using System.Xml.Serialization;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 模型
    /// </summary>
    /// <seealso cref="YuLinTu.NotifyCDObject" />
    /// <seealso cref="YuLinTu.DF.IModel" />
    [Serializable]
    public class Model : NotifyCDObject, IModel
    {
        [XmlIgnore]
        [Enabled(false)]
        [DataColumn(Enabled = false)]
        public Visibility Visibility { get; set; } = Visibility.Visible;
    }

    /// <summary>
    /// 带主键的模型
    /// </summary>
    /// <typeparam name="TId">The type of the key.</typeparam>
    /// <seealso cref="YuLinTu.NotifyCDObject" />
    /// <seealso cref="YuLinTu.DF.IModel" />
    [Serializable]
    public abstract class Model<TId> : Model, IModel<TId>
    {
        [XmlIgnore]
        [Enabled(false)]
        public virtual TId ID { get; protected set; }

        /// <summary>
        /// 设置 ID。
        /// 若 ID 类型为 Guid，请使用 <see cref="Guids.IGuidGenerator"/> 创建
        /// </summary>
        /// <param name="id"></param>
        public void SetID(TId id)
        {
            ID = id;
        }
    }

    /// <summary>
    /// 空间模型
    /// </summary>
    public abstract class GeoModel : Model, IGeoModel
    {
        #region Properties

        /// <summary>
        /// 标识码
        /// </summary>
        [Enabled(false)]
        public virtual int BSM { get; protected set; }

        /// <summary>
        /// 图斑
        /// </summary>
        [Enabled(false)]
        public virtual Geometry SHAPE { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// 带主键的空间模型
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class GeoModel<TId> : Model<TId>, IGeoModel<TId>
    {
        #region Properties

        /// <summary>
        /// 标识码
        /// </summary>
        [Enabled(false)]
        public virtual int BSM { get; protected set; }

        /// <summary>
        /// 图斑
        /// </summary>
        [Enabled(false)]
        public virtual Geometry SHAPE { get; set; }

        #endregion Properties
    }
}