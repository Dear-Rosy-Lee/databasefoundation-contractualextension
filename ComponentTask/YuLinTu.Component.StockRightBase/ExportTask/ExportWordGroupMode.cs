using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase.ExportTask
{
    /// <summary>
    /// 一组导出一张汇总表模式导出,需要该内容继续填充，目前不需要
    /// </summary>
    class ExportWordGroupMode  :ExportWordTask
    {
        public override bool ExportLandSurveyWordTable(List<Zone> zoneList, List<VirtualPerson> listPerson, List<ContractLand> listLand, List<Dictionary> listDict, CollectivityTissue tissue,
            List<ContractRegeditBook> books, List<BuildLandBoundaryAddressDot> dotList)
        {
            return true;
        }
    }
}
