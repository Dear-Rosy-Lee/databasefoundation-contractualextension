using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.StockRightBase.Helper
{
    public static class DictionaryHelper
    {

        public static List<Dictionary> DicListAll { get; set; } = new List<Dictionary>();


        public static  List<Dictionary> GetDKLB()
        {
            return DicListAll?.FindAll(o => o.GroupCode == DictionaryTypeInfo.DKLB);
        }

        public static  List<Dictionary> GetTDYT()
        {
            return DicListAll?.FindAll(o => o.GroupCode == DictionaryTypeInfo.TDYT);
        }

        public static List<Dictionary> GetCBJYQQDFS()
        {
            return DicListAll?.FindAll(o => o.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
        }


        public static List<Dictionary> GetTDLYLX()
        {
            return DicListAll?.FindAll(o => o.GroupCode == DictionaryTypeInfo.TDLYLX);
        }

        //dklbList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DKLB);
        //    tdytList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
        //    cbfsList = dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.CBJYQQDFS);




    }
}
