using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskExportSelectedLandShapeArgument : TaskArgument
    {
        public Zone CurrentZone { get; set; }

        public IDbContext DbContext { get; set; }

        public List<ContractLand> Lands { get; set; }

        public List<VirtualPerson> VPS { get; set; }

        public List<Dictionary> DictList { get; set; }

        public string SaveFilePath { get; set; }

        public TaskExportSelectedLandShapeArgument()
        {
        }
    }
}