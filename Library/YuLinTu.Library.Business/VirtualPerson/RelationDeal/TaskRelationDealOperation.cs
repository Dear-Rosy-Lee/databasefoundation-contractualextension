/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方数据操作任务类
    /// </summary>
    public class TaskRelationDealOperation : Task
    {
        #region Fields

        //private int currentIndex = 1;//当前索引号
        private List<string> shipList;//家庭关系集合

        private List<string> upList;//上级家庭关系集合
        private List<string> downList;//下级家庭关系集合
        private List<string> granddownList;//孙级家庭关系集合

        #endregion Fields

        #region Property

        #endregion Property

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskRelationDealOperation()
        {
        }

        #endregion Ctor

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary
        protected override void OnGo()
        {
            TaskRelationDealArgument metadata = Argument as TaskRelationDealArgument;
            if (metadata == null)
            {
                this.ReportError("");
                return;
            }
            if (metadata.Database == null)
            {
                this.ReportError("数据源连接异常");
                this.ReportProgress(100);
                return;
            }
            if (metadata.Type == 1)
            {
                RelationCheck();
            }
        }

        #endregion Override

        #region 检查家庭关系

        private void RelationCheck()
        {
            TaskRelationDealArgument metadata = Argument as TaskRelationDealArgument;
            InitalizeData();
            this.ReportProgress(1, "开始获取数据...");
            try
            {
                //var zonestation = metadata.Database.CreateZoneWorkStation();
                //List<Zone> zones = zonestation.GetChildren(metadata.CurrentZone.FullCode, eLevelOption.SelfAndSubs);//.GetSubZones(CurrentZone.FullCode, eLevelOption.AllSubLevel);
                //foreach (Zone zone in zones)
                //{
                ContractorDataProgress(metadata.CurrentZone);
                //}
                //zones = null;
            }
            catch (System.Exception ex)
            {
                if (!(ex is TaskStopException))
                {
                    this.ReportError(ex.Message + "数据检查时出错!");
                }
            }
            this.ReportProgress(100, null);
            shipList = null;
            upList = null;
            downList = null;
            granddownList = null;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private void InitalizeData()
        {
            //currentIndex = 1;
            shipList = InitalizeAllRelation();
            upList = InitalzieUpRelation();
            downList = InitalzieDownRelation();
            granddownList = InitalzieGrandDownRelation();
        }

        /// <summary>
        /// 承包方数据处理
        /// </summary>
        private void ContractorDataProgress(Zone zone)
        {
            TaskRelationDealArgument metadata = Argument as TaskRelationDealArgument;
            var vpstation = metadata.Database.CreateVirtualPersonStation<LandVirtualPerson>();
            int familyCount = vpstation.Count(c => c.ZoneCode.Equals(zone.FullCode));// "SenderCode", zone.FullCode, Library.Data.ConditionOption.Equal);
            if (familyCount == 0)
            {
                this.ReportInfomation(string.Format("{0}下没有承包方数据", zone.Name));
                this.ReportInfomation(string.Format("检查{0}下家庭成员关系结束", zone.Name));
                return;
            }
            this.ReportProgress(1, string.Format("开始获取{0}承包方数据...", metadata.CurrentZone.Name));
            List<VirtualPerson> vps = vpstation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
            if (vps == null || vps.Count == 0)
            {
                this.ReportWarn(string.Format("{0}下无承包方数据!", zone.Name));
                return;
            }
            string description = vps.Count > 0 ? ("(" + vps.Count + ")") : "";
            int present = 1;
            string status = string.Format("正在检查{0}下家庭成员关系...", zone.Name);
            this.ReportProgress(2, status);
            this.ReportProgress(3, string.Format("开始检查{0}下家庭成员关系...", zone.Name));
            //ReportAlert(eLogGrade.Infomation, null, string.Format("开始检查{0}下家庭成员关系...", ZoneOperator.InitalizeZoneName(DataInstance, zone)));
            foreach (VirtualPerson vp in vps)
            {
                List<Person> fsp = vp.SharePersonList;
                //FamilySharePerson fsp = new FamilySharePerson(vp);

                foreach (Person person in fsp)
                {
                    string relationShip = ToolString.ExceptSpaceString(person.Relationship);
                    if (vp.Name == person.Name && vp.Number == person.ICN)
                    {
                        if (relationShip != "本人" && relationShip != "户主")
                        {
                            this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}家庭关系有误!", vp.Name, person.Name));
                            continue;
                        }
                    }
                    if (vp.Name != person.Name && relationShip == "户主")
                    {
                        this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}家庭关系有误!存在多个户主", vp.Name, person.Name));
                        continue;
                    }

                    if (string.IsNullOrEmpty(relationShip))
                    {
                        this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}未填写家庭关系!", vp.Name, person.Name));
                        continue;
                    }
                    if (!shipList.Contains(relationShip))
                    {
                        this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}家庭关系:{2}不在家庭关系代码表中!", vp.Name, person.Name, relationShip));
                        //ReportAlert(eLogGrade.Infomation, null, string.Format("承包方:{0}下家庭成员:{1}家庭关系:{2}不在家庭关系代码表中!", vp.Name, person.Name, relationShip));
                    }
                    if (vp.CardType != eCredentialsType.IdentifyCard || person.CardType != eCredentialsType.IdentifyCard)
                    {
                        continue;
                    }
                    DateTime? localDate = ToolICN.GetBirthdayInNotCheck(vp.Number);
                    if (localDate == null || !localDate.HasValue)
                    {
                        this.ReportWarn(string.Format("承包方:{0}身份证号码:{1}中日期无效!", vp.Name, vp.Number));
                        continue;
                    }
                    RelationShipCheck(vp, person, localDate);
                }

                fsp = null;
                present++;
                this.ReportProgress(present, description);
            }
            vps = null;
            GC.Collect();
            this.ReportAlert(null, string.Format("检查{0}下家庭成员关系结束", zone.Name));
            //ReportAlert(eLogGrade.Infomation, null, string.Format("检查{0}下家庭成员关系结束", ZoneOperator.InitalizeZoneName(DataInstance, zone)));
            this.ReportProgress(100, "完成");
        }

        /// <summary>
        /// 检查家庭关系
        /// </summary>
        private void RelationShipCheck(VirtualPerson vp, Person person, DateTime? localDate)
        {
            if (vp == null || person == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(vp.Number) || string.IsNullOrEmpty(person.ICN))
            {
                return;
            }
            string ship = ToolString.ExceptSpaceString(person.Relationship);
            int age = 1;
            bool canContinue = false;
            if (upList.Contains(ship))
            {
                canContinue = true;
            }
            if (downList.Contains(ship))
            {
                canContinue = true;
            }
            if (granddownList.Contains(ship))
            {
                canContinue = true;
            }
            if (!canContinue)
            {
                return;
            }
            DateTime? date = ToolICN.GetBirthdayInNotCheck(person.ICN);
            if (date == null || !date.HasValue)
            {
                this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}身份证号码:{2}中日期无效!", vp.Name, person.Name, person.ICN));
                //ReportAlert(eLogGrade.Infomation, null, string.Format("承包方:{0}下家庭成员:{1}身份证号码:{2}中日期无效!", vp.Name, person.Name, person.ICN));
                return;
            }
            if (downList.Contains(ship))
            {
                string y = ToolDateTime.CalcateTerm(localDate.Value, date.Value);
                Int32.TryParse(y, out age);
                if (age <= 0)
                    this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}的家庭关系为:{2},但年龄比" + vp.Name + "大!", vp.Name, person.Name, ship, age));
            }
            if (upList.Contains(ship))
            {
                string y = ToolDateTime.CalcateTerm(localDate.Value, date.Value);
                Int32.TryParse(y, out age);
                if (age >= 0)
                    this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}的家庭关系为:{2},但年龄比" + vp.Name + "小!", vp.Name, person.Name, ship, age));
            }
            if (granddownList.Contains(ship))
            {
                string yr = ToolDateTime.CalcateTerm(localDate.Value, date.Value);
                Int32.TryParse(yr, out age);
                age = Math.Abs(age);
                if (age <= 35)
                {
                    this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}的家庭关系为:{2},但年龄只相差{3}岁!", vp.Name, person.Name, ship, age));
                }
            }
            else
            {
                string year = ToolDateTime.CalcateTerm(localDate.Value, date.Value);
                Int32.TryParse(year, out age);
                age = Math.Abs(age);
                if (age <= 15)
                {
                    this.ReportWarn(string.Format("承包方:{0}下家庭成员:{1}的家庭关系为:{2},但年龄只相差{3}岁!", vp.Name, person.Name, ship, age));
                }
            }
        }

        #region 初始化

        /// <summary>
        /// 初始化家庭关系
        /// </summary>
        private List<string> InitalizeAllRelation()
        {
            List<string> list = new List<string>();
            list.Add("本人");
            list.Add("户主");
            list.Add("配偶");
            list.Add("夫");
            list.Add("妻");
            list.Add("父母");
            list.Add("父亲");
            list.Add("母亲");
            list.Add("子");
            list.Add("女");
            list.Add("独生子");
            list.Add("长子");
            list.Add("次子");
            list.Add("三子");
            list.Add("四子");
            list.Add("五子");
            list.Add("养子或继子");
            list.Add("其他儿子");
            list.Add("女婿");
            list.Add("独生女");
            list.Add("长女");
            list.Add("次女");
            list.Add("三女");
            list.Add("四女");
            list.Add("五女");
            list.Add("养女或继女");
            list.Add("儿媳");
            list.Add("其他女儿");
            list.Add("孙子");
            list.Add("孙女");
            list.Add("外孙子");
            list.Add("外孙女");
            list.Add("孙媳妇或外孙媳妇");
            list.Add("孙女婿或外孙女婿");
            list.Add("曾孙子或外曾孙子");
            list.Add("曾孙女或外曾孙女");
            list.Add("孙子、孙女或外孙子、外孙女");
            list.Add("其他孙子、孙女或外孙子、外孙女");
            list.Add("公公");
            list.Add("婆婆");
            list.Add("岳父");
            list.Add("岳母");
            list.Add("继父或养父");
            list.Add("继母或养母");
            list.Add("其他父母关系");
            list.Add("祖父");
            list.Add("祖母");
            list.Add("外祖父");
            list.Add("外祖母");
            list.Add("祖父母或外祖父母");
            list.Add("配偶的祖父母或外祖父母");
            list.Add("曾祖父");
            list.Add("曾祖母");
            list.Add("配偶的曾祖父母或外曾祖父母");
            list.Add("其他祖父母或外祖父母关系");
            list.Add("兄");
            list.Add("嫂");
            list.Add("弟");
            list.Add("弟媳");
            list.Add("姐姐");
            list.Add("姐夫");
            list.Add("妹妹");
            list.Add("妹夫");
            list.Add("兄弟姐妹");
            list.Add("其他兄弟姐妹");
            list.Add("伯父");
            list.Add("伯母");
            list.Add("叔父");
            list.Add("婶母");
            list.Add("舅父");
            list.Add("舅母");
            list.Add("姨父");
            list.Add("姨母");
            list.Add("姑父");
            list.Add("姑母");
            list.Add("堂兄弟、堂姐妹");
            list.Add("表兄弟、表姐妹");
            list.Add("侄子");
            list.Add("侄女");
            list.Add("外甥");
            list.Add("外甥女");
            list.Add("其他亲属");
            list.Add("非亲属");
            list.Add("其他");
            return list;
        }

        /// <summary>
        /// 初始化子女
        /// </summary>
        /// <returns></returns>
        private List<string> InitalzieDownRelation()
        {
            List<string> list = new List<string>();
            list.Add("子");
            list.Add("女");
            list.Add("独生子");
            list.Add("长子");
            list.Add("次子");
            list.Add("三子");
            list.Add("四子");
            list.Add("五子");
            list.Add("其他儿子");
            list.Add("独生女");
            list.Add("长女");
            list.Add("次女");
            list.Add("三女");
            list.Add("四女");
            list.Add("五女");
            list.Add("其他女儿");
            return list;
        }

        /// <summary>
        /// 初始化孙子
        /// </summary>
        /// <returns></returns>
        private List<string> InitalzieGrandDownRelation()
        {
            List<string> list = new List<string>();
            list.Add("孙子");
            list.Add("孙女");
            list.Add("外孙子");
            list.Add("外孙女");
            list.Add("孙媳妇或外孙媳妇");
            list.Add("孙女婿或外孙女婿");
            list.Add("曾孙子或外曾孙子");
            list.Add("曾孙女或外曾孙女");
            list.Add("其他孙子、孙女或外孙子、外孙女");
            return list;
        }

        /// <summary>
        /// 初始化父母
        /// </summary>
        /// <returns></returns>
        private List<string> InitalzieUpRelation()
        {
            List<string> list = new List<string>();
            list.Add("父母");
            list.Add("父亲");
            list.Add("母亲");
            return list;
        }

        #endregion 初始化

        #endregion 检查家庭关系

        #region 辅助方法

        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        #endregion 辅助方法
    }
}