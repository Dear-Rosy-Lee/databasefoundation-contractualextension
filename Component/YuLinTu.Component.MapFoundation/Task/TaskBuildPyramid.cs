using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace YuLinTu.Component.MapFoundation
{
    [TaskDescriptor(IsLanguageName = false, Name = "影像切片到 SQLite 数据库", Gallery = "影像处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class TaskBuildPyramid : Task
    {
        #region Fields

        private static readonly String VERSION = "0.1";
        private SQLiteHelper _db = null;
        private int _nImportPicCount = 0;
        private SQLiteTransaction _trans = null;
        private int _currentTilesID;
        private int _currentFileIndex = 0;
        private int nPicCount = 0;
        private int fileCount = 0;

        #endregion

        #region Ctor

        public TaskBuildPyramid()
        {
            Name = "影像切片到 SQLite 数据库";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证切片参数...");
            this.ReportInfomation("开始验证切片参数...");
            System.Threading.Thread.Sleep(200);
            if (!ValidateArgs())
                return;

            this.ReportProgress(0, "开始切片...");
            this.ReportInfomation("开始切片...");
            System.Threading.Thread.Sleep(200);
            if (!BuildPyramidProc())
                return;

            this.ReportProgress(100);
            this.ReportInfomation("切片完成。");
        }

        protected override void OnValidate()
        {
            this.ReportProgress(0);

            //System.Threading.Thread.Sleep(1000);

            if (!ValidateArgs())
                return;
        }

        private bool BuildPyramidProc()
        {
            var args = Argument as TaskBuildPyramidArgument;

            List<string> lst = args.RasterFiles.Split(';').ToList();
            if (lst.Count == 0)
            {
                this.ReportAlert(eMessageGrade.Warn, null, "未选择任何影像文件。切片过程已中止。");
                return false;
            }

            string dstFileName = args.SQLiteFileName;
            //if (!dstFileName.ToLower().EndsWith(".rdb"))
            //    dstFileName += ".rdb";

            try
            {
                _db = null;
                if (File.Exists(dstFileName))
                {
                    _db = MySqlHelper.OpenDatabase(dstFileName);
                    if (!MySqlHelper.IsTableExists(_db, "tileInfo"))
                        MySqlHelper.CreateTableTileInfo(_db);
                }

                if (_db == null)
                    _db = MySqlHelper.CreateDatabase(dstFileName);

                fileCount = lst.Count;
                this.ReportAlert(eMessageGrade.Infomation, null, string.Format("共 {0} 个文件需要处理。", lst.Count));


                _currentFileIndex = 0;
                foreach (string fileName in lst)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, string.Format("开始处理第 {0} 个文件: {1}", ++_currentFileIndex, fileName));
                    this.ReportProgress((int)((double)(_currentFileIndex - 1) / lst.Count * 100), string.Format("第 {0} 个文件...", _currentFileIndex));
                    _nImportPicCount = 0;


                    nPicCount = BuildPyramid.CalcSplitCount(fileName);
                    if (nPicCount <= 0)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, string.Format("文件无效。"));
                        continue;
                    }

                    this._currentTilesID = MySqlHelper.GetNextObjectOID(_db, "tileInfo");

                    _trans = _db.BeginTransaction();


                    var val = BuildPyramid.Convert(fileName, (f) =>
                    {
                        this.OnDescribleFileCreated(f);
                    }, (z, r, c, f) =>
                    {
                        this.OnFileCreated(z, r, c, f);
                    });

                    _trans.Commit();

                    if (val < 0)
                        this.ReportAlert(eMessageGrade.Warn, null, string.Format("文件无效。"));
                    else
                        this.ReportAlert(eMessageGrade.Infomation, null, string.Format("共创建了 {0} 幅切片。", _nImportPicCount));
                }

            }
            catch (Exception ex)
            {
                this.ReportAlert(eMessageGrade.Exception, null, string.Format("切片过程发生异常。{0}", ex));
                return false;
            }
            finally
            {
                try
                {
                    if (_db != null)
                    {
                        _db.Close();
                        _db = null;
                    }
                }
                catch
                {

                }
                _nImportPicCount = 0;
            }

            return true;
        }

        void OnDescribleFileCreated(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName, Encoding.Default))
            {
                string str = VERSION;
                while (true)
                {
                    var s = r.ReadLine();
                    if (string.IsNullOrEmpty(s))
                        break;
                    //if (str == null)
                    //    str = s;
                    //else
                    str += "_" + s;
                }

                String currentLayerName = "tiles" + this._currentTilesID;
                MySqlHelper.CreateTableTiles(_db, currentLayerName);

                string sql = "insert into tileInfo(id,info) values(" + this._currentTilesID + ",'" + str + "')";
                //string sql = "update tileInfo set info="+ "'" + str + "' where name='"+this._currentLayerName+"'";
                _db.ExecuteNonQuery(sql, null);
            }
        }

        void OnFileCreated(int zoom, int row, int col, string fileName)
        {
            byte[] picData = null;
            using (FileStream fsm = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    fsm.CopyTo(memory);
                    picData = memory.ToArray();
                }
            }
            string sql = "insert into tiles" + _currentTilesID + "(zoom,row,col,img) values(" + zoom + "," + row + "," + col + ",?)";
            List<SQLiteParam> lst = new List<SQLiteParam>()
            {
                new SQLiteParam() { ParamName="img",ParamValue=picData }
            };
            _db.ExecuteNonQuery(sql, lst);

            ++_nImportPicCount;
            this.ReportProgress((int)(
                (double)(_nImportPicCount) / nPicCount * 100 / fileCount +
                (double)(_currentFileIndex - 1) / fileCount * 100),
                string.Format("第 {0} 个文件，第 {1} 幅切片...", _currentFileIndex, _nImportPicCount));
            //UIHelper.DoEvents();

            //if ((++_nImportPicCount % 10) == 0)
            //{
            //    _trans.Commit();
            //}
        }

        #endregion

        #region Methods - Validate

        private bool ValidateArgs()
        {
            var args = Argument as TaskBuildPyramidArgument;

            if (args.RasterFiles.IsNullOrBlank())
            {
                this.ReportError(string.Format("影像不能为空，请选择一个或多个有效的影像文件。"));
                return false;
            }

            if (args.SQLiteFileName.IsNullOrBlank())
            {
                this.ReportError(string.Format("SQLite 数据库文件为空，请选择一个有效的数据库文件。"));
                return false;
            }

            this.ReportInfomation(string.Format("切片参数正确。"));

            return true;
        }

        #endregion

        #endregion
    }
}
