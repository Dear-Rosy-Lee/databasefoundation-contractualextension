using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
   public class BelongRelationRespository: RepositoryDbContext<BelongRelation>, IBelongRelationRespository
    {
        public BelongRelationRespository(IDataSource ds): base(ds)
        {

        }


    }
}
