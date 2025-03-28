namespace YuLinTu.Component.ImportResultDataBaseTask
{
    public class BSST
    {
        /// <summary>
        /// 地块编码
        /// </summary>
        public string DKBM { get; set; }

        /// <summary>
        /// 数据行好
        /// </summary>
        public int SJHH { get; set; }

        /// <summary>
        /// 图层标识
        /// </summary>
        public string TCBS { get; set; }
    }

    public class DKST : BSST
    {
    }


    public class JZDST : BSST
    {
    }


    public class JZXST : BSST
    {
    }
}
