using Aspose.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.CombineWordTask
{
    public class WordTool
    {
        public static void CombineWord(string sourceDoc, string appendDoc, string outDoc, bool keepSourceFormat)
        {
            Document doc1 = new Document(sourceDoc);
            //需要合并的文档
            Document doc2 = new Document(appendDoc);

            //将doc2文档内容追加到doc1文档结尾
            doc1.AppendDocument(doc2, keepSourceFormat ? ImportFormatMode.KeepSourceFormatting : ImportFormatMode.UseDestinationStyles);

            doc1.Save(outDoc);
        }
    
    }
}
