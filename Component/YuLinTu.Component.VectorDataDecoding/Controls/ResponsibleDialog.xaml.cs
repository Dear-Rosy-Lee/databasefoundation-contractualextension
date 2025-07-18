using System;
using System.IO;
using YuLinTu.Appwork;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Controls
{
    public partial class ResponsibleDialog : MetroDialog
    {
        #region Properties

        #endregion

        #region Ctor

        public ResponsibleDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void OnInitializeCompleted()
        {
            base.OnInitializeCompleted();
            try
            {
                var pdfpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template\\免责申明.pdf");
                moonPdfPanel.OpenFile(pdfpath);
                moonPdfPanel.Zoom(1.7);
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
