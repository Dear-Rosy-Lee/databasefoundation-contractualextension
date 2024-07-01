/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Library.Aux;
using YuLinTu.NetAux;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化界址点线信息任务
    /// </summary>
    public class InitializeLandNeighborInfo : Task
    {
        #region Field
      
        #endregion

        #region Methods
        /// <summary>
        /// 初始化承包地块界址点线
        /// </summary>
        public void ContractLandInitialTool()
        {
            var meta = Argument as TaskInitializeLandNeighborInfoArgument;
            if (meta.CurrentZone == null)
            {
                this.ReportError("未选择初始化数据的地域!");
                return;
            }                  
           
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                            
                InitialDotCoils(meta);                
               
            }
            finally
            {
                sw.Stop();
                this.ReportInfomation(string.Format("共耗时：{0}", sw.Elapsed));
            }
            return;

        }

        /// <summary>
        /// 初始化数据
        /// </summary> 
        public virtual void InitialDotCoils(TaskInitializeLandNeighborInfoArgument meta)
        {
            var tolerance = ContractBusinessParcelWordSettingDefine.GetIntence().Neighborlandbufferdistence;
            using (var db = new DBSpatialite())
            {
                var dbFile = meta.Database.DataSource.ConnectionString;
                dbFile = dbFile.Substring(dbFile.IndexOf('=') + 1);
               
                db.Open(dbFile);//@"C:\myprojects\工单\20160816建库工具\通川区安云乡.sqlite");
                var prms = new InitLandDotCoilParam();
                           
                prms.Tolerance = tolerance;
             
                var t = new InitLandNeighborInfo(db, prms);
                t.ReportProgress += (msg, i) =>
                {
                    this.ReportProgress(i, msg);
                };
                t.ReportInfomation += msg =>
                {
                    this.ReportInfomation(msg);
                };
                
                string wh = null;
                wh = string.Format(Zd_cbdFields.ZLDM + " like '{0}%'", meta.TownZoneCode);// zldm like '511702208200%'";
                                                                                                  //}
                t.DoInit(wh);
            }
        }

        

        #endregion

    

        #region Methods—任务辅助方法

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        public void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }


        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        public void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

       

        #endregion
    }


}
