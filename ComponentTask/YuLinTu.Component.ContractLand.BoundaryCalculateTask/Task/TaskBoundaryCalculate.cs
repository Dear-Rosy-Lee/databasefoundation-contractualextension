/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Data;
using YuLinTu.Library.BuildJzdx;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;
using YuLinTu.NetAux;

namespace YuLinTu.Component.ContractedLand.BoundaryCalculateTask
{
    /// <summary>
    /// 生成界址点线
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "界址点线生成",
        Gallery = "汇交数据库成果",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/table-import.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/table-import.png")]
    public class TaskBoundaryCalculate : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskBoundaryCalculate()
        {
            Name = "界址点线生成";
            Description = "通过汇交成果数据进行界址点线生成";
        }

        #endregion Ctor

        #region Fields

        private readonly DKQlr _qlrMgr = new DKQlr();

        [DllImport("shell32.dll")]
        public static extern int ShellExecute(IntPtr hwnd, string lpszOp, string lpszFile, string lpszParams, string lpszDir, int fsShowCmd);

        #endregion Fields

        #region  Methods

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证参数...");
            this.ReportInfomation("验证参数...");
            if (!ValidateArgs())
                return;
            try
            {
                var args = Argument as TaskBoundaryCalculateArgument;
                InitLandDotCoilParam param = Createparams();
                ProductData(args.ShapeFilePath, args.DatabaseSavePath, args.DatabaseFilePath, param);
            }
            catch (Exception ex)
            {
                Log.WriteException(this, "界址点线生成失败!)", ex.Message + ex.StackTrace);
                this.ReportError($"界址点线生成失败:{ex.Message}");
                return;
            }
            this.ReportProgress(100);
            this.ReportInfomation("生成界址点线数据完成。");
        }

        /// <summary>
        /// 参数合法性检查
        /// </summary>
        private bool ValidateArgs()
        {
            var args = Argument as TaskBoundaryCalculateArgument;
            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            if (args.DatabaseFilePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择待权属数据库文件。"));
                return false;
            }
            if (args.ShapeFilePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择地块矢量文件。"));
                return false;
            }
            if (args.DatabaseSavePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择生成文件保存路径。"));
                return false;
            }
            if (args.ShapeFilePath == args.DatabaseSavePath)
            {
                this.ReportError(string.Format("保存路径不能与地块路径相同！"));
                return false;
            }
            this.ReportInfomation(string.Format("数据参数正确。"));
            return true;
        }

        /// <summary>
        /// 生成界址点线
        /// </summary> >
        private void ProductData(string sShpCBDFile, string outfolder, string mdbFile, InitLandDotCoilParam param)
        {
            var pathList = new List<string>();
            var sp = new ShapeProcess(sShpCBDFile, outfolder);
            sp.Info += (msg) => { this.ReportInfomation(msg); };
            sp.Process += (i) => { this.ReportProgress(2, $"初始化数据 {i}%"); };
            sShpCBDFile = sp.ProcessDkPath();

            pathList.Add(sShpCBDFile);

            var exeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "Libraries\\libs\\", "BuildCbfMc.exe");// 
            if (!File.Exists(exeFile))
            {
                throw new Exception("未找到执行程序" + exeFile);
            }
            if (!File.Exists(mdbFile))
            {
                throw new Exception("文件" + mdbFile + "不存在！");
            }
            int SW_HIDE = 0;
            ShellExecute(IntPtr.Zero, "open", exeFile, "\"" + mdbFile + "\"", null, SW_HIDE);

            var mdbPath = FileHelper.GetFilePath(mdbFile);
            this.ReportProgress(3, $"初始化权利人名称字典...");
            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            var fOK = false;
            while (!fOK)
            {
                if (IsStopPending)
                    return;
                fOK = _qlrMgr.Init(mdbPath, sShpCBDFile);
                if (fOK)
                {
                    sw1.Stop();
                    this.ReportInfomation($"权利人名称字典生成完成：{sw1.Elapsed}");
                    break;
                }
                System.Threading.Thread.Sleep(1000);
            }
            this.ReportProgress(5, $"生成界址点线数据...");
            var s = sShpCBDFile.Replace('\\', '/');
            int n = s.LastIndexOf('/');
            var path = s.Substring(0, n + 1);
            var name = s.Substring(n + 1);
            if (!name.Contains("DK"))
            {
                MessageBox.Show("请选择有效的地块shape文件（以DK开头的.shp文件）！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var sShpJzdFile = path + name.Replace("DK", "JZD");
            var sShpJzxFile = path + name.Replace("DK", "JZX");

            pathList.Add(sShpJzdFile);
            pathList.Add(sShpJzxFile);


            var SplitLineString = System.Configuration.ConfigurationManager.AppSettings["SplitNumber"].ToString();
            var onlykey = "true";
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("OnlyExportKeyJzd"))
                onlykey = System.Configuration.ConfigurationManager.AppSettings["OnlyExportKeyJzd"].ToString();

            var t = new InitLandDotCoil(param);
            t.OnQueryCbdQlr += en =>
            {
                var sQlr = _qlrMgr.GetQlr(en.rowid);
                if (sQlr == null)
                {
                    sQlr = en.rowid.ToString();
                }
                return sQlr;
            };
            t.ReportProgress += (msg, i) =>
            {
                this.ReportProgress((int)i);
            };
            t.ReportInfomation += msg =>
            {
                this.ReportInfomation(msg);
            };
            try
            {
                t.DoInit(sShpCBDFile, sShpJzdFile, sShpJzxFile);
            }
            catch (Exception ex)
            {
            }
        }

        private InitLandDotCoilParam Createparams()
        {
            var param = new InitLandDotCoilParam();
            param.fOnlyExportKeyJzd = true;
            param.fSplitLine = true;
            param.AddressPointPrefix = "J";

            param.nJzdBSMStartVal = SafeConvertAux.SafeConvertToInt32("20000000");
            param.sJzdYSDMVal = "211021";
            param.sJBLXVal = "6";
            param.sJZDLXVal = "3";
            param.nJzxBSMStartVal = SafeConvertAux.SafeConvertToInt32("50000000");
            param.sJzxYSDMVal = "211031";
            param.JXXZ = "600001";
            //param.JZXSM = tbJZXSM.Text.Trim();
            //param.PLDWZJR = tbPLDWZJR.Text.Trim();
            param.JZXLB = "01";
            //param.JZXWZ = tbJZXWZ.Text.Trim();
            param.AddressLinedbiDistance = 1.5;
            //param.LineDescription = eLineDescription.LengthDirectrion;
            return param;
        }

        #endregion 
    }
}