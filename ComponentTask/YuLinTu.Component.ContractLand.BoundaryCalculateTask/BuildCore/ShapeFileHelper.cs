/*
 * (C) 2016 鱼鳞图公司版权所有，保留所有权利
 * http://www.yulintu.com
 *
 * CLR 版本：   4.0.30319.34014            最低的 Framework 版本：4.5
 * 文 件 名：   ShapeFileHelper
 * 创 建 人：   颜学铭
 * 创建时间：   2016/6/1 11:17:05
 * 版    本：   1.0.0
 * 备注描述：
 * 修订历史：
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.NetAux;
using YuLinTu.tGISCNet;

namespace YuLinTu.Library.BuildJzdx
{
    public static class ShapeFileHelper
    {
        public static bool IsFileEmpty(string shpFile)
        {
            using (var shp = new ShapeFile())
            {
                var err=shp.Open(shpFile);
                if (err != null)
                    throw new Exception(err);
                return shp.GetRecordCount() == 0;
            }
        }
        //public static void ClearShapeFile(string shpFile)
        //{
        //    //try
        //    //{
        //    //    if (IsFileEmpty(shpFile))
        //    //        return;
        //    //}
        //    //catch
        //    //{

        //    //}
        //    var s = shpFile.Replace('\\', '/');
        //    int n = s.LastIndexOf('/');
        //    var path = s.Substring(0, n + 1);
        //    var name = s.Substring(n + 1);
        //    n = name.LastIndexOf('.');
        //    name = name.Substring(0, n);


        //    var outName=System.Guid.NewGuid().ToString();
        //    var outPath=Path.GetTempPath();
        //    using (var srcShp = new ShapeFile())
        //    {
        //        srcShp.Open(shpFile);
        //        srcShp.CopyStruct(outPath + outName);
        //    }
        //    var sa = new string[]{
        //        ".shp",".dbf",".shx"
        //    };
        //    foreach (var ext in sa)
        //    {
        //        var fileName=path + name + ext;
        //        File.Delete(fileName);
        //        var tmpFileName=outPath + outName + ext;
        //        File.Copy(tmpFileName, fileName);
        //        File.Delete(tmpFileName);
        //    }
        //    s = outPath + outName + ".prj";
        //    if (File.Exists(s))
        //    {
        //        File.Delete(s);
        //    }
        //}

        public static void DeleteShapeFile(string shpFile)
        {
            //try
            //{
            //    if (IsFileEmpty(shpFile))
            //        return;
            //}
            //catch
            //{

            //}
            var s = shpFile.Replace('\\', '/');
            int n = s.LastIndexOf('/');
            var path = s.Substring(0, n + 1);
            var name = s.Substring(n + 1);
            n = name.LastIndexOf('.');
            name = name.Substring(0, n);


            //var outName = System.Guid.NewGuid().ToString();
            //var outPath = Path.GetTempPath();
            //using (var srcShp = new ShapeFile())
            //{
            //    srcShp.Open(shpFile);
            //    srcShp.CopyStruct(outPath + outName);
            //}
            var sa = new string[]{
                ".shp",".dbf",".shx",".prj"
            };
            foreach (var ext in sa)
            {
                var fileName = path + name + ext;
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                //var tmpFileName = outPath + outName + ext;
                //File.Copy(tmpFileName, fileName);
                //File.Delete(tmpFileName);
            }
            //s = outPath + outName + ".prj";
            //if (File.Exists(s))
            //{
            //    File.Delete(s);
            //}
        }
        public static ShapeFile createJzdShapeFile(string shpFileName, string sPrjStr)
        {
            var shp = new ShapeFile();
            var err = shp.Create(shpFileName, EShapeType.SHPT_POINT, sPrjStr);
            if (err != null)
                throw new Exception(err);
            int n = shp.AddField(JzdFields.BSM, DBFFieldType.FTInteger, 10);
            shp.AddField(JzdFields.YSDM, DBFFieldType.FTString, 6);
            shp.AddField(JzdFields.JZDH, DBFFieldType.FTString, 10);
            shp.AddField(JzdFields.JZDLX, DBFFieldType.FTString, 1);
            shp.AddField(JzdFields.JBLX, DBFFieldType.FTString, 1);
            shp.AddField(JzdFields.DKBM, DBFFieldType.FTString, 254);
            shp.AddField(JzdFields.XZBZ, DBFFieldType.FTDouble, 11, 3);
            shp.AddField(JzdFields.YZBZ, DBFFieldType.FTDouble, 11, 3);
            return shp;
        }

        public static ShapeFile createJzxShapeFile(string shpFileName, string sPrjStr)
        {
            var shp = new ShapeFile();
            shp.Create(shpFileName, EShapeType.SHPT_ARC, sPrjStr);
            int n = shp.AddField(JzxFields.BSM, DBFFieldType.FTInteger, 10);
            shp.AddField(JzxFields.YSDM, DBFFieldType.FTString, 6);
            shp.AddField(JzxFields.JXXZ, DBFFieldType.FTString, 6);
            shp.AddField(JzxFields.JZXLB, DBFFieldType.FTString, 2);
            shp.AddField(JzxFields.JZXWZ, DBFFieldType.FTString, 1);
            shp.AddField(JzxFields.JZXSM, DBFFieldType.FTString, 254);
            shp.AddField(JzxFields.PLDWQLR, DBFFieldType.FTString, 100);
            shp.AddField(JzxFields.PLDWZJR, DBFFieldType.FTString, 100);
            shp.AddField(JzxFields.JZXH, DBFFieldType.FTString, 10);
            shp.AddField(JzxFields.QJZDH, DBFFieldType.FTString, 10);
            shp.AddField(JzxFields.ZJZDH, DBFFieldType.FTString, 10);
            shp.AddField(JzxFields.DKBM, DBFFieldType.FTString, 254);
            return shp;
        }

        public static string GetPrjString(string shpFileName)
        {
            string prjFile;
            var ext = FileHelper.GetFileExtension(shpFileName).ToLower();
            if (ext == ".shp")
            {
                prjFile =shpFileName.Substring(0,shpFileName.Length-ext.Length)+".prj";
            }
            else
            {
                prjFile = Path.Combine(shpFileName, ".prj");
            }
            if (File.Exists(prjFile))
            {
               return File.ReadAllText(prjFile);
            }
            return null;
        }
    }
}
