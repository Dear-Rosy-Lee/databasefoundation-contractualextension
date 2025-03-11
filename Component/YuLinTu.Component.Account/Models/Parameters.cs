using System;

namespace YuLinTu.Component.Account.Models
{
    public class Parameters
    {
        public static string TokeName = "LoginToken";
        public static string UserCodName = "LoginUserCode";
        public static string RegionName = "LoginRegion";

        public Guid Token { get; set; }

        public string UserCode { get; set; }

        public string Region { get; set; }

    }
}