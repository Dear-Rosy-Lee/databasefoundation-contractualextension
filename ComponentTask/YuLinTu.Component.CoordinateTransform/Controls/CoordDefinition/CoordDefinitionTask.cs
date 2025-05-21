using System.IO;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 定义坐标系
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "定义坐标系", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/globe.png")]
    public class CoordDefinitionTask : Task
    {
        #region Fields
        private DataSave dataSave;

        private CoordDefinitionArgument args;
        #endregion

        #region Ctor
        public CoordDefinitionTask()
        {
            Name = "定义坐标系";
            Description = "将未定义坐标系的矢量、栅格文件定义一个新的坐标系";
        }
        #endregion

        #region Methods - Override
        protected override void OnGo()
        {
            args = Argument as CoordDefinitionArgument;
            if (!ValidateArgs())
                return;
            DefinitionCoord();
        }
        #endregion

        #region Methods - Validate

        //参数检查
        private bool ValidateArgs()
        {
            if (args.DefinitionShape == null || args.DefinitionShape.Count == 0)
            {
                this.ReportError("请选择源Shape文件路径!");
                return false;
            }
            if (args.TargetPrjPath == null || !File.Exists(args.TargetPrjPath))
            {
                this.ReportError("请选择定义的坐标系文件路径!");
                return false;
            }
            this.ReportInfomation(string.Format("导出参数设置正确。"));
            return true;
        }

        #endregion

        #region Methods - Private

        private void DefinitionCoord()
        {
            this.ReportProgress(0, "开始");
            this.ReportInfomation(string.Format("开始定义Shape坐标系。"));
            this.ReportProgress(30, "开始定义");

            foreach (var shp in args.DefinitionShape)
            {
                string SourceShapePrjPath = Path.ChangeExtension(shp, ".prj");
                if (File.Exists(SourceShapePrjPath))
                {
                    this.ReportError(string.Format("{0}已经定义了坐标系，若需改变坐标系请使用更改坐标系功能!", Path.GetFileName(shp)));
                    continue;
                }
                string newDefinitionPrj = Path.ChangeExtension(shp, ".prj");
                File.Copy(args.TargetPrjPath, newDefinitionPrj, true);
            }


            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("Shape坐标系定义完成。"));
        }
        #endregion
    }
}
