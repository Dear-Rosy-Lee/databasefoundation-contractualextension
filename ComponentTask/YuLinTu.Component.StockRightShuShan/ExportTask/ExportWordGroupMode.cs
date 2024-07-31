using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan.ExportTask
{
    /// <summary>
    /// 一组导出一张汇总表模式导出,需要该内容继续填充，目前不需要
    /// </summary>
    public class ExportWordGroupMode  : ExportWordTask
    {
        public override bool ExportLandSurveyWordTable(List<Zone> zoneList, List<VirtualPerson> listPerson, List<ContractLand> listLand, List<Dictionary> listDict, CollectivityTissue tissue,
            List<ContractRegeditBook> books, List<BuildLandBoundaryAddressDot> dotList)
        {

            this.ReportProgress(1, "开始");
            Book.DictList = DbContext.CreateDictWorkStation().Get();
            Book.Tissue = DbContext.CreateSenderWorkStation().Get(CurrentZone.ID);
            Book.DbContext = DbContext;
            Book.CurrentZone = CurrentZone;

            string openFilePath = FileName + @"\" + CurrentZone.FullName + "-" + Name + ".doc";
            Book.OpenTemplate(TemplateHelper.WordTemplate(TempName));
            Book.SaveAs(new object(), openFilePath);
            this.ReportProgress(100, "完成");
            return true;
        }

    }
}
