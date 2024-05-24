using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.CombineWordTask
{
    public interface ICombineWord
    {
        Dictionary<int, FileInfo> GetSourceDictionary(FileInfo[] fileInfos);

        Dictionary<int, FileInfo> GetAppendDictionayr(FileInfo[] fileInfos);
    }
}
