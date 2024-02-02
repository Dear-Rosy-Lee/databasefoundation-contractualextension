using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.MapFoundation
{
    public class EditGISClient : Ed
    {
        [Language("lang3055512")]
        public const int tGIS_LandCodeEdit_Geometry_Begin = 3055512;

        static EditGISClient()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
        }
    }
}