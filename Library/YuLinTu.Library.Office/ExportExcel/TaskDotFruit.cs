using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Basic;
using YuLinTu.Library.YltDatabase;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Office
{
    public class TaskDotFruit : TaskExportExcelBase
    {
        #region Methods

        #region Override

        protected override void OnDo(object arg)
        {
            ReportProgressMessage(2);
            base.OnDo(arg);
        }

        protected override void BeginExport()
        {
            DotFruit dotFruit = new DotFruit(DB);
            dotFruit.PostProgressEvent += new ExportExcelBase.PostProgressDelegate(dotFruit_PostProgressEvent);
            BuildLandProperty temp = meata.Parameters[0] as BuildLandProperty;

            string message = dotFruit.BeginPrint(temp, meata.TemplatePath);
            if(!string.IsNullOrEmpty(message))
                ReportAlertInfo(message);
        }

        #endregion

        #region Priavte - Check

        protected override bool CheckParameters()
        {
            if (meata.Parameters[0] == null || !(meata.Parameters[0] is BuildLandProperty))
                return ReportAlertInfo("集体建设用地信息为空，无法导出界址点成果表");

            return true;
        }

        #endregion

        #region Private - Event

        void dotFruit_PostProgressEvent(int progress)
        {
            ReportProgressMessage(progress);
        }

        #endregion

        #endregion
    }
}
