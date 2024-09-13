namespace YuLinTu.Component.Account.Models
{
    public class AccountResult
    {
        public string Code { get; set; }
        public string HttpCode { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
        public object Errors { get; set; }
        public bool Success { get; set; }
    }

    public class AccountResult<T>
    {
        public string Code { get; set; }
        public string HttpCode { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
        public object Errors { get; set; }
        public bool Success { get; set; }
    }
}