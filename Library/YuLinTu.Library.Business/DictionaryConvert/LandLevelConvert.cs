using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{ 
    public  class GrassLevelConvert :DictionaryConvert
    {

        public static readonly GrassLevelConvert Instance = new GrassLevelConvert();//单例模式

        public GrassLevelConvert()
        {
            GroupCode = DictionaryTypeInfo.DLDJ;
        }
    }
}
