using DotSpatial.Projections.ProjectedCategories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.VectorDataDecoding.Core
{

    public class ShpFolderDescription
    {

      
    
        public static ShpFileDescription GetShpFileDescription(string FilePath)
        {
            ShpFileDescription en = new ShpFileDescription();
            en.FileName = Path.GetFileName(FilePath);
            en.FullPath = FilePath;
            var info = CheckShpFile(en.FullPath, out bool sucess, out int? landCount);
            en.DataCount = landCount.Value;
            
            en.Description = (info!=null&&info.Count>0) ?string.Join(";", info):"";
            return en;
        }


        public static IEnumerable<string> GetFilesByExtensionLegacy(string rootPath, string extension=".shp")
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;

            return Directory.EnumerateFiles(
                rootPath,
                "*" + extension,
                SearchOption.AllDirectories)
                .Where(file =>
                    string.Equals(Path.GetExtension(file), extension, StringComparison.OrdinalIgnoreCase));
        }

        public static List<string> CheckShpFile(string fileName, out bool sucess, out int? landCount)
        {
            List<string> error = new List<string>();
            sucess = false; landCount = null;

            var prjFile = Path.ChangeExtension(fileName, ".prj");
            if (!File.Exists(prjFile))
            {
                error.Add("获取坐标系文件失败！");
            }
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(fileName);
                if (!string.IsNullOrEmpty(err))
                {
                    error.Add("读取Shape文件发生错误" + err);
                    return error;
                }
                landCount = shp.GetRecordCount();           
            }
            sucess = error.Count == 0 ? true : false;
            return error;
        }

    
    }
    public class ShpFileDescription
    { 
    
        public string FileName { get; set; }
        public string FullPath { get; set; }

        public int DataCount { get; set; }

        public string DataType { get; set; }
        public bool IsDecoded { get; set; }
        public string Description { get; set; }
       

      
    }
}
