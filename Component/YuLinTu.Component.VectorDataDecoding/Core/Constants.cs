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

        internal const string appKeyVaule = "2tlb6nii0VYncW6Gq/AKDWSO8JU9Kj39e64syJUfy3tMVVUmaYqjYA==";

    }
}
