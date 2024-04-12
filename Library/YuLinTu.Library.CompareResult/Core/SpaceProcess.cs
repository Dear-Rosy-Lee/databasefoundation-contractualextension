using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YuLinTu.Library.Entity;
using YuLinTu.tGISCNet;

namespace YuLinTu.Library.CompareResult
{
    /// <summary>
    /// 空间数据操作
    /// </summary>
    public class SpaceProcess
    {
        #region Fields

        public int srid;
  
        private bool _containsShapes = true;
        private List<string> jzdFile = new List<string>();
        private List<string> jzxFile = new List<string>();
        #endregion

        #region Properties

        /// <summary>
        /// 矢量文件集合
        /// </summary>
        public List<FileCondition> ShapeFileList { get; set; }

        #endregion

        #region Ctor

        public SpaceProcess(int sr = 0)
        {
            srid = sr;
        }

        public SpaceProcess(int sr, List<FileCondition> shpFileList, bool containsShapes = true)
        {
            srid = sr;
            ShapeFileList = shpFileList;
            _containsShapes = containsShapes;
        }

        #endregion

        #region 获取编码中的上级地域编码

        /// <summary>
        /// 获取上级地域编码
        /// </summary>
        /// <returns></returns>
        private static string GetUpLevelCode(eZoneLevel level, string fullcode)
        {
            string code = string.Empty;
            int length = Zone.ZONE_CITY_LENGTH;
            switch (level)
            {
                case eZoneLevel.Group:
                    length = Zone.ZONE_VILLAGE_LENGTH;
                    break;
                case eZoneLevel.Village:
                    length = Zone.ZONE_TOWN_LENGTH;
                    break;
                case eZoneLevel.Town:
                    length = Zone.ZONE_COUNTY_LENGTH;
                    break;
            }
            if (fullcode.Length > length)
            {
                code = fullcode.Substring(0, length);
            }
            return code;
        }

        #endregion

        #region 获取Shape数据



        public void DeleteFile()
        {
            try
            {
                foreach (var fnp in jzdFile)
                {
                    File.Delete(fnp);
                }
                foreach (var fnp in jzxFile)
                {
                    File.Delete(fnp);
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// 循环获取记录
        /// </summary>
        private IEnumerable<Tuple<string, int>> EnumReadFile(FileCondition fnp, string keyFiledName, string zoneCode, bool andRow)
        {
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(fnp.FilePath);
                if (!string.IsNullOrEmpty(err))
                {
                    throw new Exception(string.Format("矢量文件{0}存在错误,无法读取", fnp.ExtFileName));
                }

                var index = shp.FindField(keyFiledName);
                if (index < 0)
                    yield break;
                for (int i = 0; i < shp.GetRecordCount(); i++)
                {
                    var strValue = shp.GetFieldString(i, index);
                    if (andRow)
                        yield return new Tuple<string, int>(strValue + "#" + i, i);
                    else
                        yield return new Tuple<string, int>(strValue, i);
                }
            }
        }


        #endregion
    }

    public class KeyCodeIndex
    {
        public string Code { get; set; }
        public int Index { get; set; }
    }
}
