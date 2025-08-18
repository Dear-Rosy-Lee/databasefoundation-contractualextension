using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;

namespace VectorDataDecodingTest
{
    [TestClass]
    public class Core_Test
    {
        [TestMethod]
        public void TestMethod1()
        {
            Registerdll();
            Gdal.SetConfigOption("SHAPE_IGNORE_INVALID_ACCESS", "YES");
            Gdal.SetConfigOption("SHAPE_RESTORE_SHX", "YES");  // 尝试重建索引
            string inputPath = "F:\\项目资料\\2.承包地业务\\2.承包地建库工具\\1.在线脱密插件\\按村组下载\\51202220250718000003_宝林镇承包地矢量图斑_20250818.shp";
            string outputDir = "F:\\项目资料\\2.承包地业务\\2.承包地建库工具\\1.在线脱密插件\\按村组下载\\矢量分离";
            string splitField = "DKBM";
            VectorSplitter vectorSplitter = new VectorSplitter();
            vectorSplitter.SplitVectorFile(inputPath, outputDir, splitField, (splitFieldVaule =>
           {
               
               if (splitFieldVaule?.Length > 14)
               {
                   return splitFieldVaule?.Substring(0, 14);
               }
               else if(splitFieldVaule?.Length > 12&& splitFieldVaule?.Length<=14)
               {
                   return splitFieldVaule?.Substring(0, 12);
               }
               return splitFieldVaule.IsNullOrEmpty()? "IsNullOrEmpty": splitFieldVaule.Length>=6?splitFieldVaule?.Substring(0, 6):"lengthTwoLow";
           }),50);
           
        }
        /// <summary>
        /// 注册Ogr驱动
        /// </summary>
        public static bool Registerdll()
        {
            try
            {

                Ogr.RegisterAll();
                //OSGeo.GDAL.Gdal.AllRegister(); 支持栅格数据
                // 为了支持中文路径，请添加下面这句代码
                OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
                // 为了使属性表字段支持中文，请添加下面这句
                OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "CP936");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

    }
}
