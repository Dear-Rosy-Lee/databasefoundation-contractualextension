using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal enum ClientEum
    {
        UploadRowDataClient = 0,
        UploaDeclassifyDataClient=1

    }

    internal enum BatchsStausCode
    {
         未送审=0,
         已送审=1,
         待处理=2,
         处理中 = 3,
         处理完成 = 4,
         处理失败 = 5,
    }
}
