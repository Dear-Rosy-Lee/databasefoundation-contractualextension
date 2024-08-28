using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 界址线类型D代表地理
    /// </summary>
    public sealed class DLineTypeConvert:DictionaryConvert
    {
        public static readonly DLineTypeConvert Instance = new DLineTypeConvert();//单例模式

        public DLineTypeConvert()
        {
            GroupCode = DictionaryTypeInfo.JZXLB;
        }
    }
}
