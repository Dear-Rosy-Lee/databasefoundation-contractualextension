
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.IO;
using Feature = OSGeo.OGR.Feature;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    public class VectorSplitter
    {
        const int forgivingTag = 0;
        public void SplitVectorFile(
            string inputPath,
            string outputDir,
            string splitField,
            Func<string, string> splitFieldFunc,
            int maxOpenFiles = 50,string ExtendShpName="")
        {

            ExtendShpName = ExtendShpName.IsNullOrEmpty() ? "" : "_" + ExtendShpName;
            // 打开输入数据源
            using (DataSource inputData = Ogr.Open(inputPath, 0))
            {
                if (inputData == null)
                    throw new Exception("无法打开输入文件：" + inputPath);

                Layer inputLayer = inputData.GetLayerByIndex(0);
                if (inputLayer == null)
                    throw new Exception("无法获取输入图层");

                // 获取分割字段索引
                int fieldIndex = inputLayer.GetLayerDefn().GetFieldIndex(splitField);
                if (fieldIndex == -1)
                    throw new Exception($"字段 '{splitField}' 不存在");

                // 第一阶段：收集所有唯一值
                HashSet<string> uniqueValues = new HashSet<string>();
                inputLayer.ResetReading();
                OSGeo.OGR.Feature feature;
                while ((feature = inputLayer.GetNextFeature()) != null)
                {
                    
                        string fieldValue = feature.GetFieldAsString(fieldIndex);
                        fieldValue = splitFieldFunc?.Invoke(fieldValue);
                        uniqueValues.Add(fieldValue);
                        feature.Dispose();
                  
                }

                // 根据唯一值数量选择处理策略
                if (uniqueValues.Count <= maxOpenFiles)
                {
                    ProcessWithMultipleFiles(inputData, outputDir, splitField, splitFieldFunc, ExtendShpName);
                }
                else
                {
                    ProcessWithSinglePass(inputData, outputDir, uniqueValues,splitField, maxOpenFiles, splitFieldFunc);
                }
            }
        }

        private void ProcessWithMultipleFiles(
            DataSource inputData,
            string outputDir,
            string splitField,
             Func<string, string> splitFieldFunc, string ExtendShpName = "")
        {
            Layer inputLayer = inputData.GetLayerByIndex(0);
            int fieldIndex = inputLayer.GetLayerDefn().GetFieldIndex(splitField);
            Dictionary<string, DataSource> outputDataSources = new Dictionary<string, DataSource>();

            try
            {
                inputLayer.ResetReading();
                OSGeo.OGR.Feature feature;
                while ((feature = inputLayer.GetNextFeature()) != null)
                {
                    string fieldValue = feature.GetFieldAsString(fieldIndex);
                    fieldValue = splitFieldFunc?.Invoke(fieldValue);
                    string safeValue = MakeValidFileName(fieldValue);

                    // 为新的唯一值创建数据源
                    if (!outputDataSources.ContainsKey(fieldValue))
                    {
                        string outputPath = Path.Combine(outputDir, $"{safeValue}{ExtendShpName}.shp");
                        Driver shpDriver = Ogr.GetDriverByName("ESRI Shapefile");
                        if (shpDriver == null) throw new Exception("Shapefile驱动不可用");

                        // 删除已存在的文件
                        if (File.Exists(outputPath)) shpDriver.DeleteDataSource(outputPath);

                        DataSource newData = shpDriver.CreateDataSource(outputPath, null);
                        if (newData == null) throw new Exception("无法创建输出文件：" + outputPath);

                        Layer newLayer = newData.CreateLayer(
                            Path.GetFileNameWithoutExtension(outputPath),
                            inputLayer.GetSpatialRef(),
                            inputLayer.GetGeomType(),
                            null);

                        // 复制字段定义
                        FeatureDefn srcDefn = inputLayer.GetLayerDefn();
                        for (int i = 0; i < srcDefn.GetFieldCount(); i++)
                        {
                            FieldDefn srcField = srcDefn.GetFieldDefn(i);
                            newLayer.CreateField(srcField, 1);
                        }

                        outputDataSources.Add(fieldValue, newData);
                    }

                    // 写入要素
                    Layer targetLayer = outputDataSources[fieldValue].GetLayerByIndex(0);
                    OSGeo.OGR.Feature newFeature = new Feature(targetLayer.GetLayerDefn());
                    newFeature.SetFrom(feature, forgivingTag);
                    targetLayer.CreateFeature(newFeature);
                    newFeature.Dispose();
                    feature.Dispose();
                }
            }
            finally
            {
                // 清理所有打开的数据源
                foreach (var ds in outputDataSources.Values)
                {
                    ds.Dispose();
                }
            }
        }

        private void ProcessWithSinglePass(
            DataSource inputData,
            string outputDir,
              HashSet<string> uniqueValues,
            string splitField,
            int maxOpenFiles, Func<string, string> splitFieldFunc, string ExtendShpName = "")
        {
            Layer inputLayer = inputData.GetLayerByIndex(0);
            int fieldIndex = inputLayer.GetLayerDefn().GetFieldIndex(splitField);

         

        
            Feature feature;
            foreach (string value in uniqueValues)
            {
                string safeValue = MakeValidFileName(value);
                string outputPath = Path.Combine(outputDir, $"{safeValue}{ExtendShpName}.shp");

                // 创建输出数据源
                Driver shpDriver = Ogr.GetDriverByName("ESRI Shapefile");
                if (shpDriver == null) throw new Exception("Shapefile驱动不可用");
                if (File.Exists(outputPath)) shpDriver.DeleteDataSource(outputPath);

                using (DataSource outputData = shpDriver.CreateDataSource(outputPath, null))
                {
                    if (outputData == null) throw new Exception("无法创建输出文件：" + outputPath);

                    Layer outputLayer = outputData.CreateLayer(
                        Path.GetFileNameWithoutExtension(outputPath),
                        inputLayer.GetSpatialRef(),
                        inputLayer.GetGeomType(),
                        null);

                    // 复制字段定义
                    FeatureDefn srcDefn = inputLayer.GetLayerDefn();
                    for (int i = 0; i < srcDefn.GetFieldCount(); i++)
                    {
                        FieldDefn srcField = srcDefn.GetFieldDefn(i);
                        outputLayer.CreateField(srcField, 1);
                    }

                    // 遍历源要素并写入匹配项
                    inputLayer.ResetReading();
                    while ((feature = inputLayer.GetNextFeature()) != null)
                    {
                        var tagVaule = splitFieldFunc?.Invoke(feature.GetFieldAsString(fieldIndex));
                        if (tagVaule == value)
                        {
                            Feature newFeature = new Feature(outputLayer.GetLayerDefn());
                            newFeature.SetFrom(feature, forgivingTag);
                            outputLayer.CreateFeature(newFeature);
                            newFeature.Dispose();
                        }
                        feature.Dispose();
                    }
                }
            }
        }

        private string MakeValidFileName(string name)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
    }
}