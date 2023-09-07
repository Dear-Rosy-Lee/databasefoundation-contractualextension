using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;


///
///
///
namespace YuLinTu.Library.Entity
{
    [DataTable("LandScapeEntity")]
    [Serializable]
    public class LandScapeEntity : NameableObject  //(修改前)YltEntityIDName
    {
        #region Propertys

        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// 地域要素代码
        /// </summary>
        public string FeatureCode { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public new string Name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string AliseName { get; set; }

        /// <summary>
        /// 地域编码
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 扩展A
        /// </summary>
        public string ExtendA { get; set; }

        /// <summary>
        /// 扩展B
        /// </summary>
        public string ExtendB { get; set; }

        /// <summary>
        /// 扩展C
        /// </summary>
        public string ExtendC { get; set; }

        /// <summary>
        /// 实际面积
        /// </summary>
        public double ActualArea { get; set; }

        /// <summary>
        /// 控制面积
        /// </summary>
        public double ControlArea { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creater { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public string Editor { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? EditionTime { get; set; }

        /// <summary>
        /// 地籍区域
        /// </summary>
        public string CadastralZone { get; set; }

        /// <summary>
        /// 参考资料
        /// </summary>
        public string IntroductionReference { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        #endregion

        #region Ctor

        public LandScapeEntity()
        {
        }

        #endregion

    }
    [Serializable]
    public class LandScapeEntityCollection //(修改前): YltEntityCollection<LandScapeEntity>
    {
    }
}