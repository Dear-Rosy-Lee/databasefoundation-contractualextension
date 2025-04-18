using Microsoft.Scripting.Actions;
using NPOI.POIFS.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    public class TaskAttributeDataCheckerOperation : Task
    {
        #region Property

        public IDbContext dbContext { get; set; }
        

        #endregion Property

        #region Field

        private Zone currentZone; //当前地域
        private TaskAttributeDataCheckerArgument argument;
        private int index;
        private int cindex;

        #endregion Field

        public TaskAttributeDataCheckerOperation()
        {
            
        }
        #region Method
        #region Method — Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证参数...");
            this.ReportInfomation("开始验证参数...");
            System.Threading.Thread.Sleep(200);
            if (!ValidateArgs())
                return;
            this.ReportProgress(1, "开始检查...");
            this.ReportInfomation("开始检查...");
            System.Threading.Thread.Sleep(200);

            try
            {
                if (!Beginning())
                {
                    this.ReportError(string.Format("数据检查出错!"));
                    CanOpenResult = true;
                    return;
                }
                
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGo(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据检查出错!"));
                return;
            }
            CanOpenResult = true;
            this.ReportProgress(100);
            this.ReportInfomation("数据检查成功。");
        }

        #endregion Method — Override
        #region Method - Private 
        private bool Beginning()
        {
            index = 1;
            cindex = 1;
            currentZone = argument.CurrentZone;
            dbContext = argument.DbContext;
            ReadData(dbContext);
            if (currentZone.Level == eZoneLevel.Group)
            {
                var resfile = CreateLog(currentZone);
                this.ReportProgress(3 + 50, $"(1/1)进行{currentZone.FullName}属性数据质量检查");
                DataCheck(resfile,currentZone);
            }
            else
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                var groups = zoneStation.GetByChildLevel(currentZone.FullCode, eZoneLevel.Group);
                double Percent = 100 / (double)groups.Count;
                
                foreach (var group in groups)
                {
                    this.ReportProgress(3 + (int)(cindex * Percent), string.Format("({0}/{1})进行{2}属性数据质量检查", cindex, groups.Count, group.FullName));
                    var resfile = CreateLog(group);
                    DataCheck(resfile,group);
                    cindex++;
                }
                return true;
            }
            return true;
        }
        /// <summary>
        /// 数据检查
        /// </summary>
        /// <returns></returns>

        private bool DataCheck(string resfile, Zone group) 
        {
            try
            {
                //进行质检
                var dcp = new DataCheckProgress();
                dcp.DataArgument = argument;
                dcp.zone = group;
                var errorInfo = dcp.Check();
                if (errorInfo.IsNotNullOrEmpty())
                {
                    this.ReportError($"{group.Name},数据检查存在错误，详情请在安装软件安装目录中查看错误文档");
                    WriteLog(resfile, errorInfo);
                    return true;
                }
                else
                {
                    this.ReportInfomation($"{group.Name}数据检查通过。");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Library.Log.Log.WriteException(this, "OnGo(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据检查时出错!"));
                return false;
            }
        }
        
            private void WriteLog(string path, string errinfo)
            {
                File.AppendAllText(path, errinfo);
            }
        
        private string CreateLog(Zone group)
        {

            // 定义文件夹路径
            var folderPath = Path.Combine(Path.GetTempPath(),"检查结果");

            // 检查文件夹是否存在，如果不存在则创建
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
            string fileName = $"{group.FullName}质量检查结果{DateTime.Now.ToString("yyyy年M月d日HH时mm分")}.txt";
            // 合成完整文件路径
            folderPath = Path.Combine(folderPath, fileName);
            File.WriteAllText(folderPath, "检查结果记录");
            return folderPath;
        }

        private bool ValidateArgs()
        {
            var args = Argument as TaskAttributeDataCheckerArgument;
            
            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }

            argument = args;

            this.ReportInfomation(string.Format("数据检查参数正确。"));
            return true;
        }
        
        private void ReadData(IDbContext dbContext)
        {
            var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var vps = vpStation.Get();
            argument.AllVirtualPeople = vps;
            var landStation = dbContext.CreateContractLandWorkstation();
            var lands = landStation.Get();
            List<Person> persons = new List<Person>();
            foreach(var item in vps)
            {
                persons.AddRange(item.SharePersonList);
            }
            argument.AllPeople = persons;
            argument.AllContractLand = lands;

        }
        public override void OpenResult()
        {
            var folderPath = Path.Combine(Path.GetTempPath(), "检查结果");
            System.Diagnostics.Process.Start(folderPath);
            base.OpenResult();
        }
        #endregion Method - Private 

        #endregion Methods
    }
}
