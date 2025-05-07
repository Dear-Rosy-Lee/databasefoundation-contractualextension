/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.DataCheckerTask
{
    /// <summary>
    /// 检查农村土地调查数据库数据任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "检查数据库数据",
        Gallery = "调查数据库处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class TaskDataChecker : Task
    {
        #region Ctor

        public TaskDataChecker()
        {
            Name = "检查农村土地调查数据库数据";
            Description = "检查农村土地调查数据库数据";
            checkCardNumber = true;
        }

        #endregion

        #region Fields

        private Zone currentZone; //当前地域
        private List<string> relationList;//家庭关系列表
        private bool showInformation;//是否显示信息
        private bool canExport;//是否检查没有问题
        private IDbContext dbContext;  //数据库
        private string zoneName;//地域名称
        private bool checkCardNumber;  //是否检查证件号码
        private bool checkCBF;//是否检查承包方
        private bool checkLand;//是否检查地块
        private bool checkFBF;//是否检查发包方
        private bool checkHT;//是否检查合同
        double averagePercent;  //平均百分比
        double currentPercent;  //当前百分比

        /*当前地域下的数据集合*/
        private List<VirtualPerson> familyCollection = new List<VirtualPerson>();
        private List<ContractLand> landCollection = new List<ContractLand>();
        private List<ContractConcord> concordCollection = new List<ContractConcord>();
        private List<ContractRegeditBook> bookCollection = new List<ContractRegeditBook>();

        /*所有数据集合(包括子级地域)*/
        private List<VirtualPerson> listVp = new List<VirtualPerson>();
        private List<ContractLand> listLand = new List<ContractLand>();
        private List<ContractConcord> listConcord = new List<ContractConcord>();
        private List<ContractRegeditBook> listBook = new List<ContractRegeditBook>();

        /*业务逻辑层接口*/
        private Library.WorkStation.IVirtualPersonWorkStation<LandVirtualPerson> personStation;
        private Library.WorkStation.IContractLandWorkStation landStation;
        private Library.WorkStation.IConcordWorkStation concordStation;
        private Library.WorkStation.IContractRegeditBookWorkStation bookStation;

        /*创建信息编辑界面*/
        private TaskAlterEditView<CollectivityTissue> senderAlterView;
        private TaskAlterEditView<VirtualPerson> vpAlterView;
        private TaskAlterEditView<ContractLand> landAlterView;
        //private TaskAlterEditView<ContractConcord> concordAlterView;
        //private TaskAlterEditView<ContractRegeditBook> regeditBookAlterView;

        #endregion

        #region Methods

        #region Method - Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证检查数据参数...");
            this.ReportInfomation("开始验证检查数据参数...");
            System.Threading.Thread.Sleep(200);
            if (!ValidateArgs())
            {
                this.ReportProgress(100);
                return;
            }

            this.ReportProgress(1, "开始检查...");
            this.ReportInfomation("开始检查...");
            System.Threading.Thread.Sleep(200);
            try
            {
                if (!BuildCheckDataPro())
                {
                    this.ReportError(string.Format("数据检查时出错!"));
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGo(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据检查时出错!"));
                return;
            }

            this.ReportProgress(100);
            this.ReportInfomation("检查完成。");
        }

        #endregion

        #region Method - Private - Validate

        /// <summary>
        /// 参数合法性检查
        /// </summary>
        private bool ValidateArgs()
        {
            var args = Argument as TaskDataCheckerArgument;

            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            if (args.ZoneNameAndCode.IsNullOrBlank())
            {
                this.ReportError(string.Format("地域不能为空，请选择地域信息。"));
                return false;
            }
            try
            {
                dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return false;
                }
                var zoneStation = dbContext.CreateZoneWorkStation();
                string fullCode = (args.ZoneNameAndCode.Split('#'))[1];
                currentZone = zoneStation.Get(c => c.FullCode == fullCode).FirstOrDefault();
                checkCardNumber = args.IsCheckCardNumber;
                checkCBF = args.IsCheckCBF;
                checkFBF = args.IsCheckFBF;
                checkLand = args.IsCheckLand;
                checkHT = args.IsCheckHT;
                if (currentZone == null)
                {
                    this.ReportError(string.Format("地域参数错误!"));
                    return false;
                }

                personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                listVp = personStation.GetByZoneCode(fullCode, eLevelOption.SelfAndSubs);
                landStation = dbContext.CreateContractLandWorkstation();
                listLand = landStation.GetCollection(fullCode, eLevelOption.SelfAndSubs);
                concordStation = dbContext.CreateConcordStation();
                listConcord = concordStation.GetContractsByZoneCode(fullCode, eLevelOption.SelfAndSubs);
                bookStation = dbContext.CreateRegeditBookStation();
                listBook = bookStation.GetContractsByZoneCode(fullCode, eLevelOption.SelfAndSubs);

                relationList = InitalizeAllRelation();
                /// 移植到确权工具8.0新框架 此处修改showInformation值恒为true 目的是不做记录检查日志工作  
                /// 这样修改可以减少修改工作量
                showInformation = true;
                if (!showInformation)
                {
                    InitalizeDirectory();
                }
                canExport = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ValidateArgs(参数合法性检查失败!)", ex.Message + ex.StackTrace);
                return false;
            }
            this.ReportInfomation(string.Format("检查数据参数正确。"));
            return true;
        }

        /// <summary>
        /// 初始化家庭关系
        /// </summary>
        private List<string> InitalizeAllRelation()
        {
            var list = FamilyRelationShip.AllRelation();

            return list;
        }

        /// <summary>
        /// 初始化错误记录文件目录
        /// </summary>
        private void InitalizeDirectory()
        {
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\Error"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + @"\Error");
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\Error\DataChecker.txt";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
            {
                writer.WriteLine(System.DateTime.Now.ToString());
            }
        }

        #endregion

        #region Method - Private - Pro

        /// <summary>
        /// 处理数据检查业务
        /// </summary>
        private bool BuildCheckDataPro()
        {
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                List<Zone> zones = currentZone.Level == eZoneLevel.State ? zoneStation.GetAll() : zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                if (zones == null || zones.Count == 0)
                {
                    return false;
                }
                int count = 0;
                int index = -1;  //索引
                averagePercent = 99.0 / (double)zones.Count;
                foreach (Zone zone in zones)
                {
                    index++;
                    string zoneName = InitalizeZoneName(dbContext, zone);
                    var landVirtualPersonStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                    count = landVirtualPersonStation.CountByZone(zone.FullCode);
                    if (count <= 0 && zone.Level <= eZoneLevel.Town)
                    {
                        count = zones.Count(ze => ze.FullCode.Contains(zone.FullCode));
                        if (count <= 0)
                        {
                            this.ReportAlert(eMessageGrade.Warn, null, zoneName + "下没有数据可供检查!");
                        }
                        //continue;
                    }
                    if (zone.Level > eZoneLevel.Town)
                    {
                        continue;
                    }
                    currentPercent = 1.0 + averagePercent * (index);
                    DataCheckProgress(zone);
                }
                zones = null;
                GC.Collect();
                if (checkCardNumber)
                {
                    ContractorNubmerProgress();   //承包方证件号码检查
                }
                if (!showInformation && !canExport)
                {
                    string dataRecord = Application.StartupPath + @"\Error\DataChecker.txt";
                    this.ReportAlert(eMessageGrade.Error, null, "检查" + currentZone.FullName + "下数据存在问题,请在" + dataRecord + "中查看详细信息...");
                    if (System.IO.File.Exists(dataRecord))
                    {
                        System.Diagnostics.Process.Start(dataRecord);
                    }
                }
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "BuildCheckDataPro(处理数据检查业务失败!)", ex.Message + ex.StackTrace);
                this.ReportError("处理数据检查业务失败!");
                return false;
            }
            finally
            {
                try
                {
                    if (dbContext != null)
                    {
                        dbContext.CloseConnection();
                        dbContext = null;
                    }
                }
                catch
                { }
            }
            return true;
        }

        /// <summary>
        /// 初始化地域名称
        /// </summary>
        public static string InitalizeZoneName(IDbContext db, Zone zone)
        {
            if (db == null || zone == null)
            {
                return string.Empty;
            }
            var zoneStation = db.CreateZoneWorkStation();
            Zone upZone = zoneStation.Get(c => c.FullCode == zone.UpLevelCode).FirstOrDefault();
            if (upZone == null)
            {
                return zone.Name;
            }
            string zoneName = upZone.Name + zone.Name;
            upZone = null;
            return zoneName;
        }

        /// <summary>
        /// 数据检查
        /// </summary>
        private void DataCheckProgress(Zone zone)
        {
            try
            {
                zoneName = InitalizeZoneName(dbContext, zone);

                familyCollection = listVp.FindAll(c => c.ZoneCode == zone.FullCode);
                landCollection = listLand.FindAll(c => c.ZoneCode == zone.FullCode);
                concordCollection = listConcord.FindAll(c => c.ZoneCode == zone.FullCode);
                bookCollection = listBook.FindAll(c => c.ZoneCode == zone.FullCode);
                var landSpaceCollection = (landCollection ?? new List<ContractLand>()).FindAll(c => c.Shape != null);
                if (checkFBF)
                {
                    var senderStation = dbContext.CreateSenderWorkStation();
                    var tissue = senderStation.Get(zone.ID);
                    if (tissue == null)
                        tissue = senderStation.FirstOrDefault(c => c.ZoneCode == zone.FullCode);
                    SenderProgress(tissue);
                    tissue = null;
                }
                this.ReportProgress((int)currentPercent, zoneName + "数据检查");
                //int per = 90 / familyCollection.Count;
                int i = 0;
                foreach (VirtualPerson vp in familyCollection)
                {
                    i++;
                    if ((i * 90 / familyCollection.Count) != currentPercent)
                    {
                        currentPercent = i * 90 / familyCollection.Count;
                        this.ReportProgress((int)currentPercent, vp.Name + "数据检查");
                    }
                    if (checkCBF)
                    {
                        vpAlterView = new TaskAlterEditView<VirtualPerson>();
                        vpAlterView.DbContext = dbContext;
                        vpAlterView.CurrentZone = zone;
                        vpAlterView.CurListVirtualPerson = familyCollection;
                        vpAlterView.CurrentVirtualPerson = vp;
                        ContractorProgress(vp, vpAlterView);
                        PersonChecker(vp, dbContext, zone, familyCollection);
                        //vpAlterView = null;
                        //GC.Collect();
                    }
                    string description = string.Format("{0}下承包方:{1}", zoneName, vp.Name);
                    if (checkLand)
                    {
                        List<ContractLand> lands = (landCollection ?? new List<ContractLand>()).FindAll(c => c.OwnerId != null && c.OwnerId.HasValue && c.OwnerId.Value == vp.ID);
                        foreach (ContractLand land in lands)
                        {
                            landAlterView = new TaskAlterEditView<ContractLand>();
                            landAlterView.DbContext = dbContext;
                            landAlterView.CurrentZone = zone;
                            landAlterView.CurListVirtualPerson = familyCollection;
                            landAlterView.CurListContractLand = landCollection;
                            landAlterView.CurrentVirtualPerson = vp;
                            landAlterView.CurrentLand = land;
                            ContractLandProgress(land, landAlterView);

                            bool exist = landSpaceCollection.Exists(ld => ld.ID == land.ID || ld.LandNumber == land.LandNumber);
                            if (!exist)
                            {
                                if (showInformation)
                                {
                                    //this.ReportAlert(eMessageGrade.Warn, null, description + "中地块编码为:" + ContractLand.GetLandNumber(land.LandNumber) + "的地块无空间信息!");
                                    this.ReportError(description + "中地块编码为:" + ContractLand.GetLandNumber(land.LandNumber) + "的地块无空间信息!", null, landAlterView);
                                }
                                else
                                {
                                    WriteDataInformation("警告:" + description + "中地块编码为:" + ContractLand.GetLandNumber(land.LandNumber) + "的地块无空间信息!");
                                }
                            }
                        }
                    }
                    if (checkHT)
                    {
                        bool concordExist = concordCollection.Exists(cd => cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == vp.ID);
                        if (!concordExist)
                        {
                            if (showInformation)
                            {
                                this.ReportAlert(eMessageGrade.Warn, null, description + "未签订承包合同!");
                                this.ReportAlert(eMessageGrade.Warn, null, description + "未签订承包权证!");
                            }
                            else
                            {
                                WriteDataInformation("警告:" + description + "未签订承包合同!");
                                WriteDataInformation("警告:" + description + "未签订承包权证");
                            }
                            canExport = false;
                            continue;
                        }
                        List<ContractConcord> concords = concordCollection.FindAll(cd => cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == vp.ID);
                        bool warrantExist = false;
                        foreach (ContractConcord concord in concords)
                        {
                            warrantExist = bookCollection.Exists(bk => bk.ID == concord.ID);
                            if (bookCollection.Count == 0 && !warrantExist)
                            {
                                warrantExist = bookStation.Get(concord.ID) != null;
                            }
                            if (warrantExist)
                            {
                                break;
                            }
                        }
                        if (!warrantExist)
                        {
                            if (showInformation)
                            {
                                this.ReportAlert(eMessageGrade.Infomation, null, description + "已签订承包合同!");
                                this.ReportAlert(eMessageGrade.Warn, null, description + "未签订承包权证!");
                            }
                            else
                            {
                                WriteDataInformation("提示:" + description + "已签订承包合同!");
                                WriteDataInformation("警告:" + description + "未签订承包权证");
                            }
                            canExport = false;
                        }
                    }
                }

                bookCollection = null;
                concordCollection = null;
                landCollection = null;
                familyCollection = null;
                this.ReportProgress((int)(currentPercent + averagePercent), zoneName + "数据检查完毕!");
                //ReportProgress(new TaskProgressChangedEventArgs(1, zoneName + "数据检查完毕!"));
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DataCheckProgress(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError("数据检查失败!");
            }
            finally
            { }
        }

        #region 证件号码检查

        /// <summary>
        /// 承包方证件号码检查
        /// </summary>
        private void ContractorNubmerProgress()
        {
            try
            {
                //var landVirtualPersonStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                //List<VirtualPerson> familyCollection = currentZone.Level == eZoneLevel.State ? landVirtualPersonStation.Get<VirtualPerson>() : landVirtualPersonStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
                var rePNums = from f in listVp //familyCollection
                              from p in f.SharePersonList.Select(c => new { Address = f.Address+f.Name, Person = c })
                              group p by p.Person.ICN == null ? String.Empty : p.Person.ICN.Trim() into g
                              where g.Count() > 1
                              select new
                              {
                                  Number = g.Key,
                                  People = g.ToList()
                              };
                foreach (var person in rePNums)
                {
                    if (string.IsNullOrEmpty(person.Number))
                    {
                        continue;
                    }
                    string information = String.Join("、", person.People.Select(p => p.Address).Distinct().ToArray());
                    string description = string.Format("证件号码:{0}重复,存在于{1}!", person.Number, information);
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, description);
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description);
                    }
                }
                rePNums = null;
                familyCollection = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ContractorNubmerProgress(承包方证件号码检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError("承包方证件号码检查失败!");
            }
        }

        #endregion

        /// <summary>
        /// 发包方处理
        /// </summary>
        private void SenderProgress(CollectivityTissue tissue)
        {
            if (tissue == null)
            {
                return;
            }
            senderAlterView = new TaskAlterEditView<CollectivityTissue>();
            senderAlterView.DbContext = dbContext;
            senderAlterView.CurrentTissue = tissue;
            string description = string.Format("{0}下发包方:{1}中", zoneName, tissue.Name);
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(tissue.LawyerName)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人姓名未填写!");
                    this.ReportError(description + "发包方负责人姓名未填写!", null, senderAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方负责人姓名未填写!");
                }
                canExport = false;
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(tissue.LawyerCartNumber)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人证件号码未填写!");
                    this.ReportError(description + "发包方负责人证件号码未填写!", null, senderAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方负责人证件号码未填写!");
                }
                canExport = false;
            }
            if (tissue.LawyerCredentType == eCredentialsType.IdentifyCard && !string.IsNullOrEmpty(tissue.LawyerCartNumber))
            {
                bool isRight = ToolICN.Check(tissue.LawyerCartNumber);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Warn, null, description + "发包方负责人证件号码" + tissue.LawyerCartNumber + "不符合身份证算法验证规范!");
                        this.ReportWarn(description + "发包方负责人证件号码" + tissue.LawyerCartNumber + "不符合身份证算法验证规范!", null, senderAlterView);
                    }
                    else
                    {
                        WriteDataInformation("警告:" + description + "发包方负责人证件号码" + tissue.LawyerCartNumber + "不符合身份证算法验证规范!");
                    }
                }
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(tissue.LawyerTelephone)))
            {
                /* 修改于20160829 对于农业规程上要求的非必填字段，不做检查提示。 */

                //if (showInformation)
                //{
                //    this.ReportAlert(eMessageGrade.Infomation, null, description + "发包方负责人联系电话未填写!");
                //}
                //else
                //{
                //    WriteDataInformation("提示:" + description + "发包方负责人联系电话未填写!");
                //}
            }
            else
            {
                bool isRight = ToolMath.MatchAllNumber(tissue.LawyerTelephone.Replace("+", "").Replace("-", ""));
                if (!isRight)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人联系电话" + tissue.LawyerTelephone + "不符合数字要求!");
                        this.ReportError(description + "发包方负责人联系电话" + tissue.LawyerTelephone + "不符合数字要求!", null, senderAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "发包方负责人联系电话" + tissue.LawyerTelephone + "不符合数字要求!");
                    }
                    canExport = false;
                }
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(tissue.LawyerAddress)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "发包方地址未填写!");
                    this.ReportError(description + "发包方地址未填写!", null, senderAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方地址未填写!");
                }
                canExport = false;
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(tissue.LawyerPosterNumber)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "邮政编码未填写!");
                    this.ReportError(description + "邮政编码未填写!", null, senderAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "邮政编码未填写!");
                }
                canExport = false;
            }
            else
            {
                bool isRight = tissue.LawyerPosterNumber.Length == 6 && ToolMath.MatchAllNumber(tissue.LawyerPosterNumber);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人邮政编码" + tissue.LawyerTelephone + "不符合数字要求!");
                        this.ReportError(description + "发包方负责人邮政编码" + tissue.LawyerTelephone + "不符合数字要求!", null, senderAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "发包方负责人邮政编码" + tissue.LawyerTelephone + "不符合数字要求!");
                    }
                    canExport = false;
                }
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(tissue.SurveyPerson)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "发包方调查员未填写!");
                    this.ReportError(description + "发包方调查员未填写!", null, senderAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方调查员未填写!");
                }
                canExport = false;
            }
            if (tissue.SurveyDate == null || !tissue.SurveyDate.HasValue)
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "发包方调查日期未填写!");
                    this.ReportError(description + "发包方调查日期未填写!", null, senderAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方调查日期未填写!");
                }
                canExport = false;
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(tissue.SurveyChronicle)))
            {
                /* 修改于20160829 对于农业规程上要求的非必填字段，不做检查提示。 */

                //if (showInformation)
                //{
                //    this.ReportAlert(eMessageGrade.Infomation, null, description + "发包方调查记事未填写!");
                //}
                //else
                //{
                //    WriteDataInformation("提示:" + description + "发包方调查记事未填写!");
                //}
            }
        }

        /// <summary>
        /// 承包方处理
        /// </summary>
        private void ContractorProgress(VirtualPerson vp, TaskAlterEditView<VirtualPerson> vpAlterView)
        {
            VirtualPersonExpand expand = vp.FamilyExpand;
            string description = string.Format("{0}下承包方:{1}中", zoneName, vp.Name);
            if (expand != null)
            {
                int number = 0;
                Int32.TryParse(vp.FamilyNumber, out number);
                //TODO 考虑数据字典
                string familyType = EnumNameAttribute.GetDescription(expand.ContractorType);
                switch (expand.ContractorType)
                {
                    case eContractorType.Farmer:
                        if (number < 0 || number > 8000)
                        {
                            if (showInformation)
                            {
                                //this.ReportAlert(eMessageGrade.Error, null, description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应大于0小于等于8000!");
                                this.ReportError(description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应大于0小于等于8000!", null, vpAlterView);
                            }
                            else
                            {
                                WriteDataInformation("错误:" + description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应大于0小于等于8000!");
                            }
                        }
                        break;
                    case eContractorType.Personal:
                        if (number <= 8000 || number > 9000)
                        {
                            if (showInformation)
                            {
                                //this.ReportAlert(eMessageGrade.Error, null, description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在8001-9000间!");
                                this.ReportError(description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在8001-9000间!", null, vpAlterView);
                            }
                            else
                            {
                                WriteDataInformation("错误:" + description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在8001-9000间!");
                            }
                        }
                        break;
                    case eContractorType.Unit:
                        if (number <= 9000 || number > 9999)
                        {
                            if (showInformation)
                            {
                                //this.ReportAlert(eMessageGrade.Error, null, description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在9001-9999间!");
                                this.ReportError(description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在9001-9999间!", null, vpAlterView);
                            }
                            else
                            {
                                WriteDataInformation("错误:" + description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在9001-9999间!");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(vp.Number)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方证件号码未填写!");
                    this.ReportError(description + "承包方证件号码未填写!", null, vpAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方证件号码未填写!");
                }
                canExport = false;
            }
            //if (vp.CardType == eCredentialsType.IdentifyCard && !string.IsNullOrEmpty(vp.Number))
            //{
            //    bool isRight = ToolICN.Check(vp.Number);
            //    if (!isRight)
            //    {
            //        if (showInformation)
            //        {
            //            //this.ReportAlert(eMessageGrade.Warn, null, description + "承包方证件号码" + vp.Number + "不符合身份证算法验证规范!");
            //            this.ReportWarn(description + "承包方证件号码" + vp.Number + "不符合身份证算法验证规范!", null, vpAlterView);
            //        }
            //        else
            //        {
            //            WriteDataInformation("提示:" + description + "承包方证件号码" + vp.Number + "不符合身份证算法验证规范!");
            //        }
            //    }
            //}
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(vp.Address)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方地址未填写!");
                    this.ReportError(description + "承包方地址未填写!", null, vpAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方地址未填写!");
                }
                canExport = false;
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(vp.PostalNumber)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方邮政编码未填写!");
                    this.ReportError(description + "承包方邮政编码未填写!", null, vpAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方邮政编码未填写!");
                }
                canExport = false;
            }
            else
            {
                bool isRight = vp.PostalNumber.Length == 6 && ToolMath.MatchAllNumber(vp.PostalNumber);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方邮政编码" + vp.PostalNumber + "不符合数字要求!");
                        this.ReportError(description + "承包方邮政编码" + vp.PostalNumber + "不符合数字要求!", null, vpAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方邮政编码" + vp.PostalNumber + "不符合数字要求!");
                    }
                    canExport = false;
                }
            }
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(vp.Telephone)))
            {
                /* 修改于20160829 对于农业规程上要求的非必填字段，不做检查提示。 */

                //if (showInformation)
                //{
                //    this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方联系电话未填写!");
                //}
                //else
                //{
                //    WriteDataInformation("提示:" + description + "承包方联系电话未填写!");
                //}
            }
            else
            {
                bool isRight = ToolMath.MatchAllNumber(vp.Telephone.Replace("+", "").Replace("-", ""));
                if (!isRight)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方联系电话" + vp.Telephone + "不符合数字要求!");
                        this.ReportError(description + "承包方联系电话" + vp.Telephone + "不符合数字要求!", null, vpAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方联系电话" + vp.Telephone + "不符合数字要求!");
                    }
                    canExport = false;
                }
            }
            if (expand != null)
            {
                if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(expand.SurveyPerson)))
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查员未填写!");
                        this.ReportError(description + "承包方调查员未填写!", null, vpAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方调查员未填写!");
                    }
                    canExport = false;
                }
                if (expand.SurveyDate == null || !expand.SurveyDate.HasValue)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查日期未填写!");
                        this.ReportError(description + "承包方调查日期未填写!", null, vpAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方调查日期未填写!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(expand.SurveyChronicle)))
                {
                    /* 修改于20160829 对于农业规程上要求的非必填字段，不做检查提示。 */

                    //if (showInformation)
                    //{
                    //    this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方调查记事未填写!");
                    //}
                    //else
                    //{
                    //    WriteDataInformation("提示:" + description + "承包方调查记事未填写!");
                    //}
                }
                if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(expand.PublicityChroniclePerson)))
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示记事人未填写!");
                        this.ReportError(description + "承包方公示记事人未填写!", null, vpAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示记事人未填写!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(expand.PublicityChronicle)))
                {
                    /* 修改于20160829 对于农业规程上要求的非必填字段，不做检查提示。 */

                    //if (showInformation)
                    //{
                    //    this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方公示记事未填写!");
                    //}
                    //else
                    //{
                    //    WriteDataInformation("提示:" + description + "承包方公示记事未填写!");
                    //}
                }
                if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(expand.PublicityCheckPerson)))
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核人未填写!");
                        this.ReportError(description + "承包方公示审核人未填写!", null, vpAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示审核人未填写!");
                    }
                    canExport = false;
                }
                if (expand.PublicityDate == null || !expand.PublicityDate.HasValue)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核日期未填写!");
                        this.ReportError(description + "承包方公示审核日期未填写!", null, vpAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示审核日期未填写!");
                    }
                    canExport = false;
                }
                expand = null;
                GC.Collect();
            }
            else
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查员未填写!");
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查日期未填写!");
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示记事人未填写!");
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核人未填写!");
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核日期未填写!");

                    this.ReportError(description + "承包方调查员未填写!", null, vpAlterView);
                    this.ReportError(description + "承包方调查日期未填写!", null, vpAlterView);
                    this.ReportError(description + "承包方公示记事人未填写!", null, vpAlterView);
                    this.ReportError(description + "承包方公示审核人未填写!", null, vpAlterView);
                    this.ReportError(description + "承包方公示审核日期未填写!", null, vpAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方调查员未填写!");
                    WriteDataInformation("错误:" + description + "承包方调查日期未填写!");
                    WriteDataInformation("提示:" + description + "承包方调查记事未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示记事人未填写!");
                    WriteDataInformation("提示:" + description + "承包方公示记事未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示审核人未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示审核日期未填写!");
                }
                canExport = false;
            }
            GC.Collect();
        }

        /// <summary>
        /// 检查家庭成员
        /// </summary>
        private void PersonChecker(VirtualPerson vp, IDbContext dbContext, Zone zone, List<VirtualPerson> familyCollection)
        {
            string description = string.Format("{0}下承包方:{1}中", zoneName, vp.Name);
            List<Person> persons = SortSharePerson(vp);

            foreach (Person person in persons)
            {
                var personAlterView = new TaskAlterEditView<VirtualPerson>();
                personAlterView.CurrentPerson = person;
                personAlterView.CurListVirtualPerson = familyCollection;
                personAlterView.DbContext = dbContext;
                personAlterView.CurrentZone = zone;
                personAlterView.CurrentVirtualPerson = vp;
                if (person.Name == vp.Name && person.ICN == vp.Number && string.IsNullOrEmpty(ToolString.ExceptSpaceString(person.Relationship)))
                {
                    person.Relationship = "户主";
                }
                if (person.Gender != eGender.Male && person.Gender != eGender.Female)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name + "性别未知!");
                        this.ReportError(description + "家庭成员:" + person.Name + "性别未知!", null, personAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "性别未知!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(person.ICN)))
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name + "证件号码未填写!");
                        this.ReportError(description + "家庭成员:" + person.Name + "证件号码未填写!", null, personAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "证件号码未填写!");
                    }
                    canExport = false;
                }
                if (person.CardType == eCredentialsType.IdentifyCard && !string.IsNullOrEmpty(person.ICN))
                {
                    bool isRight = ToolICN.Check(person.ICN);
                    if (!isRight)
                    {
                        if (showInformation)
                        {
                            //this.ReportAlert(eMessageGrade.Warn, null, description + "家庭成员:" + person.Name + "证件号码" + person.ICN + "不符合身份证算法验证规范!");
                            this.ReportWarn(description + "家庭成员:" + person.Name + "证件号码" + person.ICN + "不符合身份证算法验证规范!", null, personAlterView);
                        }
                        else
                        {
                            WriteDataInformation("提示:" + description + "家庭成员:" + person.Name + "证件号码" + person.ICN + "不符合身份证算法验证规范!");
                        }
                    }
                }
                if (person.Name != vp.Name)
                {
                    if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(person.Relationship)))
                    {
                        if (showInformation)
                        {
                            //this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name + "家庭关系未填写!");
                            this.ReportError(description + "家庭成员:" + person.Name + "家庭关系未填写!", null, personAlterView);
                        }
                        else
                        {
                            WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "家庭关系未填写!");
                        }
                        canExport = false;
                    }
                    else
                    {
                        if (!relationList.Contains(person.Relationship))
                        {
                            if (showInformation)
                            {
                                //this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name + "家庭关系" + person.Relationship + "不符合农业部家庭关系填写要求!");
                                this.ReportError(description + "家庭成员:" + person.Name + "家庭关系" + person.Relationship + "不符合农业部家庭关系填写要求!", null, personAlterView);
                            }
                            else
                            {
                                WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "家庭关系" + person.Relationship + "不符合农业部家庭关系填写要求!");
                            }
                            canExport = false;
                        }
                    }
                }
            }
            persons = null;
            GC.Collect();
        }

        /// <summary>
        /// 承包地块处理
        /// </summary>
        private void ContractLandProgress(ContractLand land, TaskAlterEditView<ContractLand> landAlterView)
        {
            string landNumber = land.LandNumber;
            string description = string.Format("{0}下承包方:{1}下地块编码为:{2}的地块", zoneName, land.OwnerName, landNumber);
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(land.Name)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "地块名称未填写!");
                    this.ReportWarn(description + "地块名称未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "地块名称未填写!");
                }
                canExport = false;
            }
            if (landNumber.Length != 19)
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Warn, null, description + "地块编码不符合农业部19位数字要求!");
                    this.ReportWarn(description + "地块编码不符合农业部19位数字要求!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("警告:" + description + "地块编码不符合农业部19位数字要求!");
                }
                canExport = false;
            }
            if (!string.IsNullOrEmpty(land.LandCode) && land.LandCode.Length != 3)
            {
                if (land.LandCode.IndexOf("XX") >= 0)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "土地利用类型未知不符合农业部二级类型要求!");
                        this.ReportError(description + "土地利用类型未知不符合农业部二级类型要求!", null, landAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "土地利用类型未知不符合农业部二级类型要求!");
                    }
                }
                else
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "土地利用类型填写不符合农业部二级类型要求!");
                        this.ReportError(description + "土地利用类型填写不符合农业部二级类型要求!", null, landAlterView);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "土地利用类型填写不符合农业部二级类型要求!");
                    }
                }
                canExport = false;
            }
            //TODO 考虑数据字典
            string landLevel = ((int)eContractLandLevel.UnKnow).ToString();
            if (land.LandLevel == landLevel)
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "地力等级未填写!");
                    this.ReportError(description + "地力等级未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "地力等级未填写!");
                }
                canExport = false;
            }
            if (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue)
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "是否基本农田未填写!");
                    this.ReportError(description + "是否基本农田未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "是否基本农田未填写!");
                }
                canExport = false;
            }
            if (land.ActualArea <= 0.00)
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "实测面积未填写!");
                    this.ReportError(description + "实测面积未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "实测面积未填写!");
                }
                canExport = false;
            }
            string[] neighbors = { land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth };
            if (neighbors != null && neighbors.Length > 0 && string.IsNullOrEmpty(ToolString.ExceptSpaceString(neighbors[0])))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "四至东未填写!");
                    this.ReportError(description + "四至东未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "四至东未填写!");
                }
                canExport = false;
            }
            if (neighbors != null && neighbors.Length > 1 && string.IsNullOrEmpty(ToolString.ExceptSpaceString(neighbors[1])))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "四至南未填写!");
                    this.ReportError(description + "四至南未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "四至南未填写!");
                }
                canExport = false;
            }
            if (neighbors != null && neighbors.Length > 2 && string.IsNullOrEmpty(ToolString.ExceptSpaceString(neighbors[2])))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "四至西未填写!");
                    this.ReportError(description + "四至西未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "四至西未填写!");
                }
                canExport = false;
            }
            if (neighbors != null && neighbors.Length > 3 && string.IsNullOrEmpty(ToolString.ExceptSpaceString(neighbors[3])))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "四至北未填写!");
                    this.ReportError(description + "四至北未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "四至北未填写!");
                }
                canExport = false;
            }
            if (land == null || string.IsNullOrEmpty(ToolString.ExceptSpaceString(land.LandExpand.ReferPerson)))
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "指界人未填写!");
                    this.ReportError(description + "指界人未填写!", null, landAlterView);
                }
                else
                {
                    WriteDataInformation("错误:" + description + "指界人未填写!");
                }
            }
        }

        /// <summary>
        /// 撰写数据记录信息
        /// </summary>
        private void WriteDataInformation(string message)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\Error\DataChecker.txt";
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, true))
            {
                writer.WriteLine(message);
            }
        }

        /// <summary>
        /// 共有人排序
        /// </summary>
        private List<Person> SortSharePerson(VirtualPerson vp)
        {
            var fsp = vp.SharePersonList;
            if (fsp == null || fsp.Count == 0)
            {
                return new List<Person>();
            }
            List<Person> sharePersonCollection = new List<Person>();
            foreach (Person person in fsp)
            {
                if (person.Name == vp.Name)
                {
                    sharePersonCollection.Add(person);
                    break;
                }
            }
            foreach (Person person in fsp)
            {
                if (person.Name != vp.Name)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }

        #endregion

        #endregion
    }
}
