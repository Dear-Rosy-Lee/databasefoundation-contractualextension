using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskAttributeDataCheckerArgument : TaskArgument
    {

        #region Property
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        public MandatoryField MandatoryField { get; set; }
        public DataCorrectness DataCorrectness { get; set; }
        public RuleOfIDCheck RuleOfIDCheck { get; set; }
        public DataLogic DataLogic { get; set; }
        public DataRepeataBility DataRepeataBility { get; set; }
        public Uniqueness Uniqueness { get; set; }

        public Dictionary<string, VirtualPerson> AllVirtualPeople { get; set; }
        public Dictionary<string, Person> AllPeople { get; set; }
        public Dictionary<string, ContractLand> AllContractLand { get; set; }

        #endregion
        public TaskAttributeDataCheckerArgument()
        {
            
        }
    }
}
