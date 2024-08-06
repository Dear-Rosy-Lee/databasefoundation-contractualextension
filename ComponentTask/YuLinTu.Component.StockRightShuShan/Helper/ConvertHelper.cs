using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightShuShan.Model;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.StockRightShuShan.Helper
{
    /// <summary>
    /// 枚举转string通用方法，转前请在枚举前加EnumNameAttribute
    /// </summary>
    public static class Helper
    {
        public static T Convert<T>(object sourceValue)
        {
            return default(T);
        }

        /// <summary>
        /// 获取枚举的特性描述信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static T ConvertEnum<T>(object sourceValue)
        {
            try
            {
                foreach (var f in typeof(T).GetFields())
                {
                    if ((f.GetCustomAttributes(typeof(EnumNameAttribute),false)?.FirstOrDefault() as EnumNameAttribute)?.Description == sourceValue as string)
                        return (T)f.GetValue(null);
                }
                return default(T);
            }
            catch (Exception e)
            {
                throw  new Exception("表格数据不正确！");
            }
        }


        /// <summary>
        /// 计算两个时间年份差,不满一年按一整年算；
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static int CalculateYear(DateTime endTime, DateTime startTime)
        {
            var year = endTime.Year - startTime.Year;
            if (endTime.Month > startTime.Month)
                year++;
            if (endTime.Month == startTime.Month)
                if (endTime.Day > startTime.Day)
                    year++;
            return year;
        }

        /// <summary>
        /// 接入合同设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="vp"></param>
        /// <param name="concordSet"></param>
        /// <param name="currentZone"></param>
        /// <returns></returns>
        public static StockConcord CreateContractConcord(CollectivityTissue sender, VirtualPerson vp, ConcordSettingModel concordSet,Zone currentZone)
        {
            var concord = new StockConcord();
            concord.ContracterId = vp.ID;
            var concordNumber = GetConcordNumber(vp, currentZone);
            concord.ConcordNumber = concordNumber;
            concord.Flag = concordSet.IsEver;
            concord.ArableLandStartTime = concordSet.StartTime;
            concord.ArableLandType ="家庭承包";
            concord.ArableLandEndTime = concordSet.EndTime;
            concord.ContractDate = concordSet.ContractTime;
            concord.SenderDate = concordSet.SenderTime;
            concord.PublicityChroniclePerson = concordSet.RecordPerson;
            concord.PublicityDate = concordSet.RecordTime;
            concord.PublicityChronicle = concordSet.RecordThings;
            concord.PublicityContractor = concordSet.Contractor;
            concord.PublicityResultDate = concordSet.PublicResultTime;
            concord.PublicityResult = concordSet.ContractorOpinion;
            concord.PublicityCheckPerson = concordSet.CheckPerson;
            concord.PublicityCheckDate = concordSet.CheckTime;
            concord.PublicityCheckOpinion = concordSet.CheckOpinion;
            concord.Comment = concordSet.Comment;
            concord.ZoneCode = currentZone.FullCode;
            concord.ManagementTime = CalculateYear((DateTime)concord.ArableLandEndTime, (DateTime)concord.ArableLandStartTime).ToString();
            concord.LandPurpose = concordSet.LandPurpose;

            concord.ContracterName = vp.Name;
            concord.SenderId = sender.ID;
            concord.SenderName = sender.Name;
            return concord;
        }

        /// <summary>
        /// 获取合同编号
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="currentZone"></param>
        /// <returns></returns>
        private static string GetConcordNumber(VirtualPerson vp, Zone currentZone)
        {
            string concordNumber = string.Empty;
            var dbContext = DataBaseSource.GetDataBaseSource();
            var zoneStation = dbContext.CreateZoneWorkStation();

            if (vp != null&&currentZone!=null)
            {
                if (currentZone.Level == eZoneLevel.Village && zoneStation.Count(currentZone.FullCode, eLevelOption.Subs) == 0)
                {
                    concordNumber = vp.ZoneCode + vp.FamilyNumber.PadLeft(6, '0')+"J";//村发包
                }
                else
                {
                    concordNumber = vp.ZoneCode + vp.FamilyNumber.PadLeft(4, '0')+"J";//组发包
                }
            }

            return concordNumber;
        }

        /// <summary>
        /// 接入权证设置
        /// </summary>
        /// <returns></returns>
        public static StockWarrant CreateContractRegeditBook(CollectivityTissue sender, VirtualPerson vp, StockConcord concord ,WarantSettingModel warantSet,Zone currentZone)
        {
            var book = new StockWarrant();
            book.ID = concord.ID;
            book.Number = concord.ConcordNumber;
            book.SerialNumber = sender.Code + vp.Number.GetLastString(4);
            book.RegeditNumber = book.Number;
            book.Year = warantSet.Year.ToString();
            book.WriteDate = warantSet.WriteTime;//填证日期
            book.SendDate = warantSet.SendTime;//颁证日期
            book.SendOrganization = warantSet.SendOffice;//颁证机关
            book.WriteOrganization = warantSet.WriteOffice;//填证单位
            book.ContractRegeditBookExcursus = warantSet.RegisterComment;
            book.ContractRegeditBookPerson = warantSet.RegisterPerson;
            book.ContractRegeditBookTime = warantSet.RegisterTime;
            book.ZoneCode = currentZone.FullCode;
            return book;
        }

        ///// <summary>
        ///// 获取当前枚举项的描述, 获取前请在枚举前加EnumNameAttribute （主程序已有本方法）
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="sourceValue"></param>
        ///// <returns></returns>
        //public static string ConvertEnum(this object sourceValue)
        //{
        //    try
        //    {
        //        foreach (var f in sourceValue.GetType().GetFields())
        //        {
        //            if (f.Name == sourceValue.ToString())
        //                if(f.GetAttribute<EnumNameAttribute>()!=null)
        //                    return f.GetAttribute<EnumNameAttribute>().Description;
        //        }
        //        return string.Empty;
        //    }
        //    catch(Exception e)
        //    {
        //        throw new Exception("枚举转换失败！"+e.Message+e.StackTrace);
        //    }
        //}


    }
}
