/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 接收消息
    /// </summary>
    public class ConcordMessageContext : WorkstationContextBase
    {
        #region Fields

        #endregion

        #region Methods

        #region 数据处理

        /// <summary>
        /// 清空合同(承包方和承包地块均被清空)
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_CLEARLANDANDPERSON_COMPLETE)]
        public void OnClearAllConcords(object sender, ModuleMsgArgs e)
        {
            try
            {
                string currentZoneCode = e.ZoneCode;
                IDbContext db = e.Datasource;
                var concordStation = db.CreateConcordStation();
                concordStation.DeleteOtherByZoneCode(currentZoneCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnClearAllConcords(清空合同)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清空合同(承包地块均被清空)
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_CLEAR_COMPLETE)]
        public void OnClearConcords(object sender, ModuleMsgArgs e)
        {
            try
            {
                string currentZoneCode = e.ZoneCode;
                IDbContext db = e.Datasource;
                var concordStation = db.CreateConcordStation();
                concordStation.DeleteByZoneCode(currentZoneCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnClearConcords(清空合同)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清空合同(承包方被清空)
        /// </summary>
        //[MessageHandler(Name = VirtualPersonMessage.CLEAR_COMPLATE)]
        //public void OnClearContractConcord(object sender, ModuleMsgArgs e)
        //{
        //    try
        //    {
        //        var dbContext = e.Datasource;
        //        var zoneCode = e.SenderCode;
        //        if (dbContext == null || string.IsNullOrEmpty(zoneCode))
        //            return;
        //        var concordStation = dbContext.CreateConcordStation();
        //        e.ReturnValue = concordStation.DeleteOtherByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearContractConcord(清空合同)", ex.Message + ex.StackTrace);
        //    }
        //}

        /// <summary>
        /// 更新合同(添加新的地块时接收消息)
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTLAND_ADD_COMPLETE)]
        public void OnModifyConcord(object sender, ModuleMsgArgs e)
        {
            ContractLand addLand = e.Parameter as ContractLand;
            if (addLand == null)
                return;
            SigningConcord(addLand, e.Datasource);
        }

        /// <summary>
        /// 更新合同及权证(合户时接收消息),删除以前的人的权证和合同，重新签订
        /// </summary>
        [MessageHandler(Name = VirtualPersonMessage.VIRTUALPERSON_COMBINE_COMPLATE)]
        public void OnCombineContractor(object sender, ModuleMsgArgs e)
        {
            try
            {
                string currentZoneCode = e.ZoneCode;
                IDbContext db = e.Datasource;
                var contractlandStation = db.CreateContractLandWorkstation();               
                MultiObjectArg personinfo = e.Parameter as MultiObjectArg;
                if (personinfo == null)
                    return;
                contractlandStation.UpdateConcordAndBookByCombineContractor(personinfo.ParameterA as VirtualPerson, personinfo.ParameterB as VirtualPerson);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnCombineContractor(合户更新合同及权证)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 更新合同(编辑地块时接收消息)
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE)]
        public void OnUpdateConcord(object sender, ModuleMsgArgs e)
        {
            ContractLand updateLand = e.Parameter as ContractLand;
            ContractLand previousLand = e.Tag as ContractLand;
            if (previousLand == null || updateLand == null)
                return;
            if (previousLand.OwnerId == updateLand.OwnerId)
            {
                //没有改变承包方
                SigningConcord(updateLand);
            }
            else
            {
                //改变承包方
                SigningConcord(previousLand);
                SigningConcord(updateLand);
            }
        }

        /// <summary>
        /// 删除/更新合同(删除承包地块时接收消息)
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTLAND_DELETE_COMPLETE)]
        public void OnDeleteConcord(object sender, ModuleMsgArgs e)
        {
            ContractLand delLand = e.Parameter as ContractLand;
            if (delLand == null)
                return;
            SigningConcord(delLand);
        }

        #endregion

        /// <summary>
        /// 签订合同
        /// </summary>
        public void SigningConcord(ContractLand land, IDbContext dbContext = null)
        {
            try
            {
                //先解除合同
                if (dbContext == null)
                    dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                    return;
                var landStation = dbContext.CreateContractLandWorkstation();
                List<string> fList = ConcordExtend.DeserializeContractInfo(true);
                List<string> ofList = ConcordExtend.DeserializeContractInfo(false);
                landStation.SignConcord(land, fList, ofList);

                //var dbContext = DataBaseSource.GetDataBaseSource();
                //var concordStation = dbContext.CreateConcordStation();
                //var landStation = dbContext.CreateContractLandWorkstation();
                //var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                //var person = personStation.Get(c => c.ID == (Guid)land.OwnerId).FirstOrDefault();
                //var listConcords = concordStation.GetContractsByFamilyID((Guid)land.OwnerId);
                //concordStation.Delete(c => c.ContracterId != null && c.ContracterId == land.OwnerId);
                ////再签订合同
                //var listLand = landStation.GetCollection((Guid)land.OwnerId);
                //string familyMode = ((int)eConstructMode.Family).ToString();
                //ContractConcord concordFamily = listConcords.Find(c => !string.IsNullOrEmpty(c.ArableLandType) && c.ArableLandType == familyMode);
                //ContractConcord concordOther = listConcords.Find(c => !string.IsNullOrEmpty(c.ArableLandType) && c.ArableLandType != familyMode);
                //InitiallizeArea(concordFamily);
                //InitiallizeArea(concordOther);
                //List<string> fList = ConcordExtend.DeserializeContractInfo(true);
                //List<string> ofList = ConcordExtend.DeserializeContractInfo(false);
                //if (listLand != null && listLand.Count > 0)
                //{
                //    bool isFamilyUpdate = false;
                //    bool isOtherUpdate = false;
                //    //承包方下有地块信息才考虑签订合同
                //    foreach (var ld in listLand)
                //    {
                //        bool isExsit = fList.Any(c => c.Equals(ld.LandCategory));
                //        bool oisExsit = ofList.Any(c => c.Equals(ld.LandCategory));
                //        bool isFamilyMode = ld.ConstructMode == familyMode ? true : false;
                //        if (concordFamily != null && isExsit && isFamilyMode)
                //        {
                //            //家庭承包方式合同
                //            concordFamily.CountActualArea += ld.ActualArea;
                //            concordFamily.CountAwareArea += ld.AwareArea;
                //            concordFamily.TotalTableArea += ld.TableArea;
                //            ld.ConcordId = concordFamily.ID;
                //            isFamilyUpdate = true;
                //        }
                //        if (concordOther != null && oisExsit && !isFamilyMode)
                //        {
                //            //其他承包方式合同
                //            concordOther.CountActualArea += ld.ActualArea;
                //            concordOther.CountAwareArea += ld.AwareArea;
                //            concordOther.TotalTableArea += ld.TableArea;
                //            ld.ConcordId = concordOther.ID;
                //            isOtherUpdate = true;
                //        }
                //        if (isFamilyUpdate || isOtherUpdate)
                //            UpdateContractLand(ld, landStation);
                //    }
                //    if (isFamilyUpdate)
                //        UpdateConcord(concordFamily, concordStation);
                //    if (isOtherUpdate)
                //        UpdateConcord(concordOther, concordStation);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "SigningConcord(重新签订合同)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清空合同中的面积
        /// </summary>
        private void InitiallizeArea(ContractConcord concordFamily)
        {
            if (concordFamily == null)
                return;
            concordFamily.CountActualArea = 0;
            concordFamily.CountAwareArea = 0;
            concordFamily.TotalTableArea = 0;
        }

        /// <summary>
        /// 更新合同
        /// </summary> 
        private void UpdateConcord(ContractConcord concordFamily, IConcordWorkStation concordStation)
        {
            if (concordFamily == null)
                return;
            concordStation.Add(concordFamily);
        }

        /// <summary>
        /// 更新地块
        /// </summary>
        private void UpdateContractLand(ContractLand land, IContractLandWorkStation landStation)
        {
            if (land == null)
                return;
            landStation.Update(land);
        }

        #endregion
    }
}
