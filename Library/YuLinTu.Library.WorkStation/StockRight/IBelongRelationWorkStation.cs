using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.WorkStation
{
    public interface IBelongRelationWorkStation : IWorkstation<BelongRelation>
    {
        int DeleteRelations(string zoneCode);

        int Deleteland(string zoneCode);

        int DeleteLands(List<ContractLand> lands);
        /// <summary>
        /// 根据人获取当前人下的地，并且修改备注
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        List<ContractLand> GetLandByPerson(Guid personId, string zoneCode);
        /// <summary>
        /// 根据人获取当前人下的地，并且不修改备注
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        List<ContractLand> GetNotCommentLandByPerson(Guid personId, string zoneCode);

        List<PersonStockLand> GetPersonStockLand(string zoneCode);

        void DataBaseExchange(string zoneCode);

        int AddRang(List<BelongRelation> listRelation);

        int DeleteByZone(string zoneCode, eLevelOption searchOption);

        List<BelongRelation> GetdDataByZoneCode(string zoneCode, eLevelOption searchOption);
    }
}
