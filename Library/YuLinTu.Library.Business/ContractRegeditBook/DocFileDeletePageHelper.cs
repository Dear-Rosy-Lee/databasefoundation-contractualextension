using System;
namespace YuLinTu.Library.Business
{
    public class DocFileDeletePageHelper
    {
        /// <summary>
        /// 设置第几页删除几次直到不能删除
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="deletePageIndex"></param>
        /// <param name="deletePageCnt"></param>
        /// <param name="wdUnit"></param>
        /// <returns></returns>
        public static bool DocFileDeletePage(string FilePath, int deletePageIndex, int deletePageCnt = 1, WdUnits wdUnit = WdUnits.wdCharacter)
        {
            bool ret = true;

            if (FilePath.IsNullOrEmpty())
            {
                ret = false;
                return ret;
            }
            try
            {

                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
                object oMissing = System.Reflection.Missing.Value;
                object filePath = FilePath;//文件路径

                //打开文件
                Document wordDoc = wordApp.Documents.Open(
                    ref filePath, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                     ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                      ref oMissing, ref oMissing, ref oMissing, ref oMissing);


                int pagerange = 0;
                for (int i = 0; i < deletePageCnt; i++)
                {
                    int pages = wordDoc.ComputeStatistics(WdStatistic.wdStatisticPages, ref oMissing);
                    if (deletePageIndex > pages)
                    {
                        ret = false;
                        return ret;
                    }

                    object objWhat = Microsoft.Office.Interop.Word.WdGoToItem.wdGoToPage;
                    object objWhich = Microsoft.Office.Interop.Word.WdGoToDirection.wdGoToAbsolute;
                    object objPage = deletePageIndex;

                    Microsoft.Office.Interop.Word.Range range1 = wordDoc.GoTo(ref objWhat, ref objWhich, ref objPage, ref oMissing);
                    Microsoft.Office.Interop.Word.Range range2 = range1.GoToNext(Microsoft.Office.Interop.Word.WdGoToItem.wdGoToPage);

                    object objStart = range1.Start;
                    object objEnd = range2.Start;
                    pagerange = range2.Start - range1.Start;

                    if (range1.Start == range2.Start)
                    {
                        objEnd = range1.End;
                    }                   

                    object Unit = (int)wdUnit;                   

                    object Count = 1;
                    wordDoc.Range(ref objStart, ref objEnd).Delete(ref Unit, ref Count);
                    
                }

                wordDoc.Save();

                wordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                wordApp.Quit(ref oMissing, ref oMissing, ref oMissing);

            }
            catch (Exception ex)
            {
                var error = ex.Message;
                Log.Log.WriteError(null,"导出权证删除空白页",error);
                ret = false;
                return ret;
            }
            return ret;
        }
    }
}
