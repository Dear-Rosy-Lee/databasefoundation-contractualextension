using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal  class Constants
    {
       
        internal static string baseUrl = ConfigurationManager.AppSettings.TryGetValue<string>("DefaultBusinessAPIAdress", "https://api.yizhangtu.com");
        internal const string appidName = "t-open-api-app-id";

        internal const string appidVaule = "2d954f1f39d04ea5be07d7b67d6c1ad7";


        internal const string appKeyName = "t-open-api-app-key";
        //以下配置应登录账号后获取，目前先硬编码写死

        internal const string appKeyVaule = "2tlb6nii0VYncW6Gq/AKDWSO8JU9Kj39e64syJUfy3tMVVUmaYqjYA==";

        internal const string Sm4Key = "efd4a17e7c2a89ea5a9d9a283fe03ae1";

    }
}
