using System;
using System.ComponentModel;
using YuLinTu.Spatial;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 模型接口
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IModel : INotifyPropertyChanged, ICD, ICloneable
    {
    }

    /// <summary>
    /// 带主键的模型接口
    /// </summary>
    /// <typeparam name="TId">The type of the key.</typeparam>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IModel<TId> : IModel
    {
        TId ID { get; }

        /// <summary>
        /// 设置 ID。
        /// 若 ID 类型为 Guid，请使用 <see cref="Guids.IGuidGenerator"/> 创建
        /// </summary>
        void SetID(TId id);
    }

    /// <summary>
    /// 空间模型
    /// </summary>
    public interface IGeoModel : IModel
    {
        int BSM { get; }

        Geometry SHAPE { get; set; }
    }

    /// <summary>
    /// 带主键的空间模型
    /// </summary>
    public interface IGeoModel<TId> : IGeoModel, IModel<TId>
    {
    }
}