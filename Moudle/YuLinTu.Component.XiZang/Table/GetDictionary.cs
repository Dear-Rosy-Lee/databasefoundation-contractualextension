using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;

using YuLinTu.Library.Entity;

using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 字典
    /// </summary>
    [Serializable]
    public class GetDictionary : ExcelBase
    {
        #region 字段

        private string fileName = string.Empty;//文件名称
        private string errorInformation = string.Empty;//错误信息       

        #endregion

        #region 属性
        public object[,] DallItem { get; set; }//字典   
        public int rowCount { get; set; }//行数
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInformation
        {
            get { return errorInformation; }
        }


        #endregion

        #region Ctor
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public GetDictionary(string fileName)
        {
            this.fileName = fileName;
        }
       
        #endregion

        #region 方法
        public override void Read()
        {
            try
            {
                DallItem = getDictory();
                rowCount = GetRangeRowCount();
                if (DallItem == null)
                    return;
                
            }
            catch (Exception ex)
            {
                errorInformation = ex.Message;
                return;
            }
        }
        /// <summary>
        /// 写方法
        /// </summary>
        public override void Write()
        {
           
            
        }
        /// <summary>
        /// 读取字典文件信息
        /// </summary>
        /// <returns></returns>
        private object[,] getDictory()
        {
            if (string.IsNullOrEmpty(fileName) || !OpenFamilyFile(fileName))
            {
                errorInformation = "打开字典文件失败";
                return null;
            }
            object[,] DallItem = GetAllRangeValue();//使用域

            if (DallItem == null)
            {
                errorInformation = "文件错误";
                return null;
            }
            return DallItem;
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        private bool OpenFamilyFile(string fname)
        {
            try
            {
                Open(fname,0);//打开文件
                return true;
            }
            catch
            {
                errorInformation = "打开文件失败";
                return false;
            }
        }
        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="chinaName"></param>
        /// <returns></returns>
        public string translante(string chinaName)
        {
            if (chinaName == null)
                return "";
            string zangwen = "";
            for (int i = 0; i < rowCount; i++)
            {
                string name = DallItem[i, 0] == null ? "" : DallItem[i, 0].ToString();
                if (name.Trim() == chinaName.Trim())
                {
                    zangwen = DallItem[i, 1] == null ? "" : DallItem[i, 1].ToString();
                    break;
                }
            }
            return zangwen;
        }
        #endregion
    }
}
