using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data.Dynamic;

namespace YuLinTu.Component.VectorDataDecoding.Repository
{
    internal class ShapeFileRepostiory
    {
       
        
        
        
        
        private const int BATCH_COUNT = 500;
      
        


        
        public static void PagingTrans<TTarget>(
           DynamicQuery dq,
           string schemaName,
           string tableName,
           string keyName,
           ConditionSection where,
           Action<List<TTarget>,int> save,
           Func<int, int, object, TTarget> convert,
           int pageSize = BATCH_COUNT)
        {
            var index = 0;
            //ConditionSection where = whereExpression.IsNullOrEmpty()
            //    ? null : QuerySection.DLinq(whereExpression);
          //  QuerySection.Column("BM").Equal(QuerySection.Parameter(rootZoneCode)),
            var count = DynamicCount(dq, schemaName, tableName, where);

            var pageCount = (count + pageSize - 1) / pageSize;
            var list = new List<TTarget>();

            for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                QueryResult page = null;
                if (where is null)
                    page = dq.Paging(schemaName, tableName, pageIndex, pageSize, eOrder.Ascending, keyName);
                else
                    page = dq.Paging(schemaName, tableName, pageIndex, pageSize, eOrder.Ascending, keyName, where);

                var result = page.Result as IList;
                foreach (var item in result)
                {
                    index++;

                    var en = convert(index, count, item);

                    if (en == null)
                    {
                        continue;
                    }

                    list.Add(en);
                }

                if (list.Count > 0)
                {
                    save?.Invoke(list, count);
                    list.Clear();
                }
            }
        }

        private static int DynamicCount(DynamicQuery dq, string schema, string elementName, ConditionSection predicate)
        {
            if (predicate is null)
                return dq.Count(schema, elementName);
            else
                return dq.Count(schema, elementName, predicate);
        }

    }
}
