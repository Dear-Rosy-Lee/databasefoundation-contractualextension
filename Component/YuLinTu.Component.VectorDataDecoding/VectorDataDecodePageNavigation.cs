using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.DF.Controls.Messages;
using YuLinTu.DF.Data;
using YuLinTu.DF.Zones;
using YuLinTu.DF.Component.Navigation;



namespace YuLinTu.Component.VectorDataDecoding
{
    [Workpage(typeof(VectorDataDecodePage))]
    public class VectorDataDecodePageNavigation : ZoneNavigationContextbase
    {
        #region Properties

        public new VectorDataDecodePage PageContent
        { get { return base.PageContent as VectorDataDecodePage; } }

        // private ILandInfoRepository LandInfoRep { get => GetRepository<ILandInfoRepository>(); }
        public override bool IsConnectDB { get => base.IsConnectDB; set => base.IsConnectDB = false; }
        #endregion Properties

        #region Fields

        #endregion Fields

        #region Ctor

        public VectorDataDecodePageNavigation(IWorkpage workpage)
            : base(workpage)
        {
            IsConnectDB = false;
        }

        #endregion Ctor

        #region Methods

        #region Methods - Override

        public override void NavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            base.NavigateTo(sender, e);

            //var args = new RefreshEventArgs();
            //Workpage.Message.Send(this, args);
        }

        protected override int ShowContentCountByZoneCode(string zoneCode)
        {
            return 100;//LandInfoRep.CountByZoneCode(zoneCode);
        }

        #endregion Methods - Override

        #endregion Methods
    }
}