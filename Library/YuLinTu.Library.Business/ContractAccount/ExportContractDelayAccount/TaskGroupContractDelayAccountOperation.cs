using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskGroupContractDelayAccountOperation : TaskGroup
    {
        private string openFilePath;  //打开文件路径

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupAccountFiveTableArgument groupMeta = Argument as TaskGroupAccountFiveTableArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.SelfAndSubsZones;

            var nationList = EnumStore<eContractAccountType>.GetListByType();
            foreach (var zone in selfAndSubsZones)
            {
                if (groupMeta.TableType == 5)
                {
                    openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone));
                }
                else
                {
                    openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectoryByVilliage(groupMeta.AllZones, zone));
                }
                string savePath = CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone);
                var argtypeName = nationList.Find(t => t.Value == groupMeta.ArgType).DisplayName;
                var meta = new TaskAccountFiveTableArgument();
                meta.IsClear = false;
                meta.FileName = fileName + @"\" + savePath;
                meta.ArgType = groupMeta.ArgType;
                meta.Database = dbContext;
                meta.CurrentZone = zone;
                meta.VirtualType = groupMeta.VirtualType;
                meta.UserName = "";
                meta.Date = groupMeta.Date;
                meta.TableType = groupMeta.TableType;
                meta.AllZones = groupMeta.AllZones;
                meta.SelfAndSubsZones = groupMeta.SelfAndSubsZones;
                meta.IsBatch = groupMeta.IsBatch;
                meta.DictList = groupMeta.DictList;
                meta.DelcTime = groupMeta.DelcTime;
                meta.PubTime = groupMeta.PubTime;
                var import = new TaskContractDelayAccountOperation();
                import.Argument = meta;
                import.Description = "导出" + zone.FullName;
                import.Name = $"导出台账报表-{argtypeName}";
                Add(import);   //添加子任务到任务组中
            }
            CanOpenResult = true;
            base.OnGo();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            TaskGroupAccountFiveTableArgument groupMeta = Argument as TaskGroupAccountFiveTableArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }
    }
}
