using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Component.MapFoundation
{
    [MessageHandler(ID = EditGISClient.tGIS_LandCodeEdit_Geometry_Begin)]
    public class MessageSplitLandInstallEventArgs : MapMessageEventArgs
    {
        #region Properties

        public IDbContext DbContext { get; set; }

        public Layer Layer { get; set; }

        public List<Graphic> Graphics { get; private set; }

        public int TargetIndex { get; set; }

        public AutoResetEvent AutoResetEvent { get; private set; }

        public bool IsCancel { get; set; }

        #endregion Properties

        #region Ctor

        public MessageSplitLandInstallEventArgs(Layer layer, List<Graphic> gs, IDbContext dbContext)
            : base(EditGISClient.tGIS_LandCodeEdit_Geometry_Begin)
        {
            DbContext = dbContext;
            Layer = layer;
            AutoResetEvent = new AutoResetEvent(false);
            Graphics = new List<Graphic>();
            Graphics.AddRange(gs);
        }

        #endregion Ctor

        #region Methods

        public override string ToString()
        {
            return string.Format(base.ToString(), Layer);
        }

        #endregion Methods
    }
}