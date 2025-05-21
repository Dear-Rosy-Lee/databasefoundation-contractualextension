using Aspose.Cells;
using GeoAPI.CoordinateSystems;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Utils;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.PropertyRight;
using YuLinTu.PropertyRight.ContractLand;
using eCredentialsType = YuLinTu.Library.Entity.eCredentialsType;
using eGender = YuLinTu.Library.Entity.eGender;

namespace YuLinTu.Library.Business
{
    public class DataCheckProgress
    {
        #region Property

        public TaskAttributeDataCheckerArgument DataArgument { get; set; }

        public IDbContext dbContext { get; set; }

        public Zone zone { get; set; }

        public CollectivityTissue collectivityTissue { get; set; }

        public List<VirtualPerson> virtualPeople { get; set; }
        
        public List<ContractLand> contractLand { get; set; }

        public List<ContractConcord> contractConcord { get; set; }

        public List<Person> persons { get; set; }

        public List<string> ListError { get; private set; }

        public List<Dictionary> DictList { get; set; }

        #endregion Property

        public List<string> Check()
        {
            dbContext = DataArgument.DbContext;
            var dictStation = dbContext.CreateDictWorkStation();
            DictList = dictStation.Get();
            var senderStation = dbContext.CreateSenderWorkStation();
            var sender = senderStation.GetByCode(zone.FullCode);
            collectivityTissue = sender;
            var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var vp = vpStation.GetByZoneCode(zone.FullCode);
            virtualPeople = vp;
            var landStation = dbContext.CreateContractLandWorkstation();
            var lands = landStation.GetCollection(zone.FullCode);
            contractLand = lands;
            var concordStation = dbContext.CreateConcordStation();
            var concords = concordStation.GetByZoneCode(zone.FullCode);
            contractConcord = concords;
            persons = new List<Person>();
            virtualPeople.ForEach(t => { persons.AddRange(t.SharePersonList); });
            ListError = new List<string>();
            MandatoryFieldCheck();
            DataCorrectnessCheck();
            RuleOfIDCheck();
            DataLogicCheck();
            DataRepeataBilityCheck();
            UniquenessCheck();
            return ListError;
        }

        private void MandatoryFieldCheck()
        {
            if (DataArgument.MandatoryField.MandatoryFieldSender == true)
            {
                var ErrorInfo = new List<string>();
                //发包方名称
                if (collectivityTissue.Name.IsNullOrEmpty())
                {
                    ErrorInfo.Add("\n发包方名称未填写;");
                }
                //发包方编码
                if (collectivityTissue.Code.IsNullOrEmpty())
                {
                    
                    ErrorInfo.Add("\n发包方编码未填写;");
                }
                //发包方负责人
                if (collectivityTissue.LawyerName.IsNullOrEmpty())
                {
                    ErrorInfo.Add("\n发包方负责人未填写;");
                }
                //发包方负责人证件类型
                if (collectivityTissue.LawyerCredentType.ToString().IsNullOrEmpty())
                {
                    ErrorInfo.Add("\n发包方负责人证件类型未填写;");
                }
                //发包方负责人证件号
                if (collectivityTissue.LawyerCartNumber.IsNullOrEmpty())
                {
                    ErrorInfo.Add("\n发包方负责人证件号未填写;");
                }
                //发包方地址
                if (collectivityTissue.LawyerAddress.IsNullOrEmpty())
                {
                    ErrorInfo.Add("\n发包方地址未填写;");
                }
                //调查员
                if (collectivityTissue.SurveyPerson.IsNullOrEmpty())
                {
                    ErrorInfo.Add("\n发包方调查员未填写;");
                }
                //调查日期
                if (collectivityTissue.SurveyDate.ToString().IsNullOrEmpty())
                {
                    ErrorInfo.Add("\n发包方调查日期未填写;");
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n发包方:{collectivityTissue.Name},必填字段检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            
            if (DataArgument.MandatoryField.MandatoryFieldVP == true)
            {
                var ErrorInfo = new List<string>();
                if (virtualPeople.Where(x => x.FamilyNumber.IsNullOrEmpty()).ToList() != null)
                {
                    var vpList = virtualPeople.Where(x => x.FamilyNumber.IsNullOrEmpty()).ToList();
                    foreach (var vp in vpList) 
                    {
                        ErrorInfo.Add($"\n承包方{vp.Name},承包方户编号未填写;");
                    }
                }
                if (virtualPeople.Where(x => x.Name.IsNullOrEmpty()).ToList() != null)
                {
                    var vpList = virtualPeople.Where(x => x.Name.IsNullOrEmpty()).ToList();
                    foreach (var vp in vpList)
                    {
                        ErrorInfo.Add($"\n承包方户编号{vp.FamilyNumber},承包方姓名未填写;");
                    }
                }
                if (virtualPeople.Where(x => x.VirtualType.ToString().IsNullOrEmpty()).ToList() != null)
                {
                    var vpList = virtualPeople.Where(x => x.VirtualType.ToString().IsNullOrEmpty()).ToList();
                    foreach (var vp in vpList)
                    {
                        ErrorInfo.Add($"\n承包方户编号{vp.FamilyNumber},承包方类型未填写;");
                    }
                }
                if (virtualPeople.Where(x => x.Address.IsNullOrEmpty()).ToList() != null)
                {
                    var vpList = virtualPeople.Where(x => x.Address.IsNullOrEmpty()).ToList();
                    foreach (var vp in vpList)
                    {
                        ErrorInfo.Add($"\n承包方户编号{vp.FamilyNumber},承包方地址未填写;");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n承包方必填字段检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            if (DataArgument.MandatoryField.MandatoryFieldMember == true)
            {
                var ErrorInfo = new List<string>();
                if (persons.Where(x => x.Name.IsNullOrEmpty()).ToList() != null)
                {
                    var sharePersonList = persons.Where(x => x.Name.IsNullOrEmpty()).ToList();
                    foreach (var sharePerson in sharePersonList)
                    {
                        var vp = virtualPeople.Where(x => x.ID == sharePerson.FamilyID).FirstOrDefault();
                        ErrorInfo.Add($"\n承包方户编码{vp.FamilyNumber},存在成员姓名为空的数据;");
                    }
                }
                if (persons.Where(x => x.Gender.ToString().IsNullOrEmpty()).ToList() != null)
                {
                    var sharePersonList = persons.Where(x => x.Gender.ToString().IsNullOrEmpty()).ToList();
                    foreach (var sharePerson in sharePersonList)
                    {
                        var vp = virtualPeople.Where(x => x.ID == sharePerson.FamilyID).FirstOrDefault();
                        ErrorInfo.Add($"\n承包方户编码{vp.FamilyNumber},存在成员性别为空的数据;");
                    }
                }
                if (persons.Where(x => x.CardType.ToString().IsNullOrEmpty()).ToList() != null)
                {
                    var sharePersonList = persons.Where(x => x.CardType.ToString().IsNullOrEmpty()).ToList();
                    foreach (var sharePerson in sharePersonList)
                    {
                        var vp = virtualPeople.Where(x => x.ID == sharePerson.FamilyID).FirstOrDefault();
                        ErrorInfo.Add($"\n承包方户编码{vp.FamilyNumber},存在成员证件类型为空的数据;");
                    }
                }
                if (persons.Where(x => x.ICN.IsNullOrEmpty()).ToList() != null)
                {
                    var sharePersonList = persons.Where(x => x.ICN.IsNullOrEmpty()).ToList();
                    foreach (var sharePerson in sharePersonList)
                    {
                        var vp = virtualPeople.Where(x => x.ID == sharePerson.FamilyID).FirstOrDefault();
                        ErrorInfo.Add($"\n承包方户编码{vp.FamilyNumber},存在成员证件号码为空的数据;");
                    }
                }
                if (persons.Where(x => x.Relationship.IsNullOrEmpty()).ToList() != null)
                {
                    var sharePersonList = persons.Where(x => x.Relationship.IsNullOrEmpty()).ToList();
                    foreach (var sharePerson in sharePersonList)
                    {
                        var vp = virtualPeople.Where(x => x.ID == sharePerson.FamilyID).FirstOrDefault();
                        ErrorInfo.Add($"\n承包方户编码{vp.FamilyNumber},存在成员家庭关系为空的数据;");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n承包方家庭成员必填字段检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            if (DataArgument.MandatoryField.MandatoryFieldLand == true)
            {
                var ErrorInfo = new List<string>();
                if (contractLand.Where(x => x.Name.IsNullOrEmpty()).ToList() != null)
                {
                    var landList = contractLand.Where(x => x.Name.IsNullOrEmpty()).ToList();
                    foreach (var land in landList)
                    {
                        ErrorInfo.Add($"\n地块编码：{land.LandNumber},地块名称未填写;");
                    }
                }
                if (contractLand.Where(x => x.LandCategory.IsNullOrEmpty()).ToList() != null)
                {
                    var landList = contractLand.Where(x => x.LandCategory.IsNullOrEmpty()).ToList();
                    foreach (var land in landList)
                    {
                        ErrorInfo.Add($"\n地块编码：{land.LandNumber},地块类别未填写;");
                    }
                }
                if (contractLand.Where(x => x.LandCode.IsNullOrEmpty()).ToList() != null)
                {
                    var landList = contractLand.Where(x => x.LandCode.IsNullOrEmpty()).ToList();
                    foreach (var land in landList)
                    {
                        ErrorInfo.Add($"\n地块编码：{land.LandNumber},土地利用类型未填写;");
                    }
                }
                if (contractLand.Where(x => x.Purpose.IsNullOrEmpty()).ToList() != null)
                {
                    var landList = contractLand.Where(x => x.Purpose.IsNullOrEmpty()).ToList();
                    foreach (var land in landList)
                    {
                        ErrorInfo.Add($"\n地块编码：{land.LandNumber},土地用途未填写;");
                    }
                }
                if (contractLand.Where(x => x.ActualArea == 0).ToList() != null)
                {
                    var landList = contractLand.Where(x => x.ActualArea == 0).ToList();
                    foreach (var land in landList)
                    {
                        ErrorInfo.Add($"\n地块编码：{land.LandNumber},实测面积未填写;");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n承包地块必填字段检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            if (DataArgument.MandatoryField.MandatoryFieldContract == true)
            {
                var ErrorInfo = new List<string>();
                foreach (var item in virtualPeople) 
                {
                    var contract = contractConcord.Where(x => x.ContracterId == item.ID);
                    if (contract.IsNullOrEmpty())
                    {
                        ErrorInfo.Add($"\n承包方:{item.Name},无承包合同信息，无法检查;");
                    }
                }


                if (contractConcord.Where(x => x.ConcordNumber.IsNullOrEmpty()).ToList() != null)
                {
                    var contractList = contractConcord.Where(x => x.ConcordNumber.IsNullOrEmpty()).ToList();
                    foreach (var contract in contractList)
                    {
                        ErrorInfo.Add($"\n承包方:{contract.ContracterName},承包合同编码未填写;");
                    }
                }
                if (contractConcord.Where(x => x.ContractDate.ToString().IsNullOrEmpty()).ToList() != null)
                {
                    var contractList = contractConcord.Where(x => x.ContractDate.ToString().IsNullOrEmpty()).ToList();
                    foreach (var contract in contractList)
                    {
                        ErrorInfo.Add($"\n承包合同:{contract.ConcordNumber},承包合同签订日期未填写;");
                    }
                }
                
                if (contractConcord.Where(x => x.ArableLandStartTime.ToString().IsNullOrEmpty()).ToList() != null)
                {
                    var contractList = contractConcord.Where(x => x.ArableLandStartTime.ToString().IsNullOrEmpty()).ToList();
                    foreach (var contract in contractList)
                    {
                        ErrorInfo.Add($"\n承包合同:{contract.ConcordNumber},承包合同签订起始时间未填写;");
                    }
                }
                if (contractConcord.Where(x => x.ArableLandEndTime.ToString().IsNullOrEmpty()).ToList() != null)
                {
                    var contractList = contractConcord.Where(x => x.ArableLandEndTime.ToString().IsNullOrEmpty()).ToList();
                    foreach (var contract in contractList)
                    {
                        ErrorInfo.Add($"\n承包合同:{contract.ConcordNumber},承包合同签订终止时间未填写;");
                    }
                }
                if (contractConcord.Where(x => x.ManagementTime.ToString().IsNullOrEmpty()).ToList() != null)
                {
                    var contractList = contractConcord.Where(x => x.ManagementTime.ToString().IsNullOrEmpty()).ToList();
                    foreach (var contract in contractList)
                    {
                        ErrorInfo.Add($"\n承包合同:{contract.ConcordNumber},承包合同期限未填写;");
                    }
                }
                if (contractConcord.Where(x => x.ArableLandType.IsNullOrEmpty()).ToList() != null)
                {
                    var contractList = contractConcord.Where(x => x.ArableLandType.IsNullOrEmpty()).ToList();
                    foreach (var contract in contractList)
                    {
                        ErrorInfo.Add($"\n承包合同:{contract.ConcordNumber},合同承包方式未填写;");
                    }
                }
                if (contractConcord.Where(x => x.CountAwareArea == 0).ToList() != null)
                {
                    var contractList = contractConcord.Where(x => x.CountAwareArea == 0).ToList();
                    foreach (var contract in contractList)
                    {
                        ErrorInfo.Add($"\n承包合同:{contract.ConcordNumber},合同面积亩未填写;");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n承包合同必填字段检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            
        }
        private void DataCorrectnessCheck()
        {
            if (DataArgument.DataCorrectness.DataCorrectnessField == true)
            {
                var ErrorInfo = new List<string>();
                foreach (var vp in virtualPeople)
                {
                    if (!Enum.IsDefined(typeof(eVirtualPersonType), vp.VirtualType))
                    {
                        ErrorInfo.Add($"\n承包方户编号:{vp.FamilyNumber},承包方类型字段填写不满足值域检查要求;");
                    }
                    if (!Enum.IsDefined(typeof(eCredentialsType), vp.CardType))
                    {
                        ErrorInfo.Add($"\n承包方户编号:{vp.FamilyNumber},证件类型字段填写不满足值域检查要求;");
                    }
                    if (!Enum.IsDefined(typeof(eBHQK), vp.ChangeSituation))
                    {
                        ErrorInfo.Add($"\n承包方户编号:{vp.FamilyNumber},变化情况字段填写不满足值域检查要求;");
                    }
                }
               
                foreach(var person in persons) 
                {
                    if (!Enum.IsDefined(typeof(eCredentialsType), person.CardType))
                    {
                        ErrorInfo.Add($"\n成员身份证:{person.ICN},证件类型字段填写不满足值域检查要求;");
                    }
                    if (!Enum.IsDefined(typeof(eGender), person.Gender))
                    {
                        ErrorInfo.Add($"\n成员身份证:{person.ICN},性别字段填写不满足值域检查要求;");
                    }
                    var relationShipList = FamilyRelationShip.AllRelation();

                    if (!relationShipList.Contains(person.Relationship))
                    {
                        ErrorInfo.Add($"\n成员身份证:{person.ICN},家庭关系填写不满足值域检查要求;");
                    }
                }
                foreach (var land in contractLand)
                {
                    var dictDKLB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DKLB);
                    if (!dictDKLB.Select(x=>x.Code).Contains(land.LandCategory))
                    {
                        ErrorInfo.Add($"\n地块编码:{land.LandNumber},地块类别字段填写不满足值域检查要求;");
                    }
                    var dictTDYT = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
                    if (!dictTDYT.Select(x => x.Code).Contains(land.Purpose))
                    {
                        ErrorInfo.Add($"\n地块编码:{land.LandNumber},土地用途字段填写不满足值域检查要求;");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n字段填写值域检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            if (DataArgument.DataCorrectness.DataCorrectnessVPName == true)
            {
                var ErrorInfo = new List<string>();
                // 检查 承包方姓名 是否合法（仅中文和·）
                var invalidvpName = virtualPeople.Where(p => !IsValidName(p.Name)).ToList();
                if (invalidvpName.Any())
                {
                    foreach (var vp in invalidvpName)
                    {
                        ErrorInfo.Add($"\n承包方：{vp.Name},身份证：{vp.Number},承包方姓名不能包含数字、空格，除了 · 以外的的特殊字符");
                    }
                }
                // 检查 成员姓名 是否合法（仅中文和·）
                var invalidpersonName = persons.Where(p => !IsValidName(p.Name)).ToList();
                if (invalidpersonName.Any())
                {
                    foreach (var person in invalidpersonName)
                    {
                        ErrorInfo.Add($"\n成员姓名：{person.Name},身份证：{person.ICN},成员姓名不能包含数字、空格，除了 · 以外的的特殊字符");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n承包方姓名、成员姓名检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            if (DataArgument.DataCorrectness.DataCorrectnessSurveyName == true)
            {
                var ErrorInfo = new List<string>();
                ErrorInfo.AddRange(SenderNameCheck());
                ErrorInfo.AddRange(VPNameCheck()); 
                ErrorInfo.AddRange(LandNameCheck());
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n调查人员、审核人员姓名检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            if (DataArgument.DataCorrectness.DataCorrectnessEvent == true)
            {
                var ErrorInfo = new List<string>();
                if (collectivityTissue.SurveyChronicle!=null && collectivityTissue.CheckOpinion != null)
                {
                    if (collectivityTissue.SurveyChronicle.Length > 500)
                    {
                        ErrorInfo.Add($"\n发包方：{collectivityTissue.Name},调查记事内容的长度不能超过500个字符");
                    }
                    if (collectivityTissue.CheckOpinion.Length > 500)
                    {
                        ErrorInfo.Add($"\n发包方：{collectivityTissue.Name},审核记事内容的长度不能超过500个字符");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n调查记事、审核记事检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            
        }
        private void RuleOfIDCheck()
        {
            
            if (DataArgument.RuleOfIDCheck.RuleOfID == true)
            {
                var ErrorInfo = new List<string>();
                if (!IsValidIdCard(collectivityTissue.LawyerCartNumber))
                {
                    ErrorInfo.Add($"\n发包方负责人:{collectivityTissue.LawyerName},错误身份证号：{collectivityTissue.LawyerCartNumber}, 证件号码不符合身份证规则");
                }
                foreach (var vp in virtualPeople)
                {
                    if (!IsValidIdCard(vp.Number))
                    {
                        ErrorInfo.Add($"\n承包方:{vp.Name},错误身份证号：{vp.Number}, 证件号码不符合身份证规则");
                    }
                }
                foreach (var person in persons)
                {
                    if (!IsValidIdCard(person.ICN))
                    {
                        ErrorInfo.Add($"\n成员姓名:{person.Name}，错误身份证号：{person.ICN}, 证件号码不符合身份证规则");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n证件号码检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            
        }
        private void DataLogicCheck()
        {
            
            if (DataArgument.DataLogic.DataLogicLandType == true)
            {
                var ErrorInfo = new List<string>();
                foreach (var land in contractLand)
                {
                    if (land.LandCategory == "10")
                    {
                        if (!(land.AwareArea > 0))
                        {
                            ErrorInfo.Add($"\n地块编码：{land.LandNumber},地块类型为“承包地块”时，确权面积必须大于0；");
                        }

                        if (land.ConstructMode != "110")
                        {
                            ErrorInfo.Add($"\n地块编码：{land.LandNumber},地块类型为“承包地块”时，地块的承包方式必须为家庭承包方式;");
                        }
                    }
                    
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n地块类型逻辑检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            if (DataArgument.DataLogic.DataLogicLandLimit == true)
            {
                var ErrorInfo = new List<string>();
                foreach (var land in contractLand)
                {

                    var invalidNeighbors = contractLand.Where(p =>
                                        new[] { p.NeighborEast, p.NeighborWest, p.NeighborSouth, p.NeighborNorth }
                                        .Count(x => !string.IsNullOrEmpty(x)) < 3).ToList();
                    if (invalidNeighbors.Any())
                    {
                        ErrorInfo.Add($"\n地块编码：{land.LandNumber},地块四至的东、南、西、北至，至少有3个方向不能为空;");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n地块四至逻辑检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            
        }
        private void DataRepeataBilityCheck()
        {
            
            if (DataArgument.DataRepeataBility.DataRepeataBilityCheck == true)
            {
                var ErrorInfo = new List<string>();
                var duplicateHH = virtualPeople
                                    .Where(p => !string.IsNullOrEmpty(p.FamilyNumber)) 
                                    .GroupBy(p => p.FamilyNumber)                      
                                    .Where(g => g.Count() > 1)                
                                    .Select(g => new { HH = g.Key, Name = g.Select(p => p.Name).ToList() }) 
                                    .ToList();

                if (duplicateHH.Any())
                {
                    foreach (var dup in duplicateHH)
                    {
                        ErrorInfo.Add($"\n姓名：{string.Join("、", dup.Name)},重复的承包方户编号:{dup.HH}");
                    }
                }
                var duplicateVPID = virtualPeople
                                    .Where(p => !string.IsNullOrEmpty(p.Number))
                                    .GroupBy(p => p.Number)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => new { ID = g.Key, Name = g.Select(p => p.Name).ToList() })
                                    .ToList();

                if (duplicateVPID.Any())
                {
                    foreach (var dup in duplicateVPID)
                    {
                        ErrorInfo.Add($"\n姓名：{string.Join("、", dup.Name)},重复的承包方证件号码:{dup.ID}");
                    }
                }
                var duplicatePersonID = persons
                                        .Where(p => !string.IsNullOrEmpty(p.ICN))
                                        .GroupBy(p => p.ICN)
                                        .Where(g => g.Count() > 1)
                                        .Select(g => new { ID = g.Key, Name = g.Select(p => p.Name).ToList() })
                                        .ToList();

                if (duplicatePersonID.Any())
                {
                    foreach (var dup in duplicatePersonID)
                    {
                        ErrorInfo.Add($"\n姓名：{string.Join("、", dup.Name)},重复的成员身份证号码:{dup.ID}");
                    }
                }
                var duplicateLands = contractLand
                                     .Where(p => !string.IsNullOrEmpty(p.LandNumber))
                                     .GroupBy(p => p.LandNumber)
                                     .Where(g => g.Count() > 1)
                                     .Select(g => new { ID = g.Key, Name = g.Select(p => p.Name).ToList() })
                                     .ToList();

                if (duplicateLands.Any())
                {
                    foreach (var dup in duplicateLands)
                    {
                        ErrorInfo.Add($"\n地块名称：{string.Join("、", dup.Name)},重复的地块编码:{dup.ID}");
                    }
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n集体唯一性检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            if (DataArgument.DataRepeataBility.DataRepeataBilityCheckCode == true)
            {
                var ErrorInfo = new List<string>();
                List<string> CollectivelyName = new List<string>() { "村集体", "社集体", "集体", "集体地", "组集体" };
                foreach (var name in CollectivelyName) 
                {
                    if (virtualPeople.Select(x => x.Name).Contains(name))
                    {
                        
                        if(virtualPeople.Where(x => x.Name == name).ToList().Count != 1)
                        {
                            ErrorInfo.Add("\n单个发包方下不能有多个集体");
                        }
                    }
                
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n单个发包方内编码唯一性检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            
        }
        private void UniquenessCheck()
        {
            
            List<LandVirtualPerson> AllVirtualPeople = DataArgument.AllVirtualPeople;
            List<Person> AllPeople = DataArgument.AllPeople;
            List<ContractLand> AllContractLand = DataArgument.AllContractLand;
            if (DataArgument.Uniqueness.UniquenessCheck == true)
            {
                var ErrorInfo = new List<string>();
                var vpNumberCounts = new Dictionary<string, int>();
                foreach (var vp in AllVirtualPeople)
                {
                    if (vp.Number.IsNotNullOrEmpty())
                    {
                        if (vpNumberCounts.ContainsKey(vp.Number))
                        {
                            vpNumberCounts[vp.Number]++;
                        }
                        else
                        {
                            vpNumberCounts[vp.Number] = 1;
                        }
                    }
                }

                foreach (var vp in virtualPeople)
                {
                    if (vp.Number.IsNotNullOrEmpty())
                    {
                        if (vpNumberCounts.TryGetValue(vp.Number, out int count) && count > 1)
                        {
                            ErrorInfo.Add($"\n承包方户编号：{vp.FamilyNumber},承包方身份证：{vp.Number}整库必须唯一");
                        }
                    }
                    
                }
                var personNumberCounts = new Dictionary<string, int>();
                foreach (var person in AllPeople)
                {
                    if (person.ICN.IsNotNullOrEmpty())
                    {
                        if (personNumberCounts.ContainsKey(person.ICN))
                        {
                            personNumberCounts[person.ICN]++;
                        }
                        else
                        {
                            personNumberCounts[person.ICN] = 1;
                        }
                    }
                    
                }
                foreach (var person in persons)
                {
                    if (person.ICN.IsNotNullOrEmpty())
                    {
                        if (personNumberCounts.TryGetValue(person.ICN, out int count) && count > 1)
                        {
                            ErrorInfo.Add($"\n成员身份证：{person.ICN}整库必须唯一");
                        }
                    }
                    
                }
                var landNumberCounts = new Dictionary<string, int>();
                foreach (var contractLand in AllContractLand)
                {
                    if (contractLand.LandNumber.IsNotNullOrEmpty())
                    {
                        if (landNumberCounts.ContainsKey(contractLand.LandNumber))
                        {
                            landNumberCounts[contractLand.LandNumber]++;
                        }
                        else
                        {
                            landNumberCounts[contractLand.LandNumber] = 1;
                        }
                    }
                }
                foreach (var land in contractLand)
                {
                    if (land.LandNumber.IsNotNullOrEmpty())
                    {
                        if (landNumberCounts.TryGetValue(land.LandNumber, out int count) && count > 1)
                        {
                            ErrorInfo.Add($"\n地块编码：{land.LandNumber}整库必须唯一");
                        }
                    }
                        
                }
                if (ErrorInfo.Count == 0)
                {
                    ErrorInfo.Add($"\n整库编码唯一性检查通过。");
                }
                ListError.AddRange(ErrorInfo);
            }
            
        }
        private static bool IsValidName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false; // 如果允许空值，可以改成 return true;

            return Regex.IsMatch(input, @"^[\u4e00-\u9fa5·]+$");
        }
        // 检查 cbfxm 中的每个名字是否合法，返回非法名字列表
        private static List<string> GetInvalidNames(string xm)
        {
            if (string.IsNullOrWhiteSpace(xm))
                return new List<string>();

            // 支持的拆分符号：中文顿号、逗号、分号 + 英文逗号、分号
            char[] separators = { '、', '，', '；', ',', ';' };
            string[] names = xm.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length > 1)
            {
                var invalidNames = names
                .Where(name => !Regex.IsMatch(name.Trim(), @"^[\u4e00-\u9fa5·]+$"))
                .ToList();
                return invalidNames;
            }
            // 校验每个名字是否仅含中文和·

            return null;
        }
        private bool IsNames(string xm)
        {
            if (string.IsNullOrWhiteSpace(xm))
                return false;
            // 支持的拆分符号：中文顿号、逗号、分号 + 英文逗号、分号
            char[] separators = { '、', '，', '；', ',', ';' };
            string[] names = xm.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool IsValidIdCard(string idCard)
        {
            if (string.IsNullOrEmpty(idCard))
                return false;

            // 18位身份证（严格模式）
            if (Regex.IsMatch(idCard, @"^[1-9]\d{5}(19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dXx]$"))
                return true;

            return false;
        }
        private List<string> SenderNameCheck()
        {
            var ErrorInfo = new List<string>();
            var SurveyNameflag = IsNames(collectivityTissue.SurveyPerson);//判断发包方调查人员是否存在多个人名
            if (SurveyNameflag)
            {
                if (!GetInvalidNames(collectivityTissue.SurveyPerson).IsNullOrEmpty())
                {
                    ErrorInfo.Add($"\n发包方：{collectivityTissue.Name},调查人员存在多个人名:{collectivityTissue.SurveyPerson}，不可使用除、，；,;的分拆字符串。");
                }
            }
            else
            {
                if (!IsValidName(collectivityTissue.SurveyPerson))
                {
                    ErrorInfo.Add($"\n发包方：{collectivityTissue.Name},调查人员:{collectivityTissue.SurveyPerson},不能包含数字、空格，除了 · 以外的的特殊字符");
                }
            }
            var CheckNameflag = IsNames(collectivityTissue.CheckPerson);//判断发包方审核人员是否存在多个人名
            if (CheckNameflag)
            {
                if (!GetInvalidNames(collectivityTissue.CheckPerson).IsNullOrEmpty())
                {
                    ErrorInfo.Add($"\n发包方：{collectivityTissue.Name},审核人员存在多个人名:{collectivityTissue.CheckPerson}，不可使用除、，；,;的分拆字符串。");
                }
            }
            else
            {
                if (!IsValidName(collectivityTissue.CheckPerson))
                {
                    ErrorInfo.Add($"\n发包方：{collectivityTissue.Name},审核人员:{collectivityTissue.CheckPerson},不能包含数字、空格，除了 · 以外的的特殊字符");
                }
            }
            return ErrorInfo;
        }

        private List<string> VPNameCheck()
        {
            var ErrorInfo = new List<string>();
            foreach (var item in virtualPeople)
            {
                var SurveyNameflag = IsNames(item.FamilyExpand.SurveyPerson);//判断承包方调查人员是否存在多个人名
                if (SurveyNameflag)
                {
                    if (!GetInvalidNames(item.FamilyExpand.SurveyPerson).IsNullOrEmpty())
                    {
                        ErrorInfo.Add($"\n承包方：{item.Name},调查人员存在多个人名:{item.FamilyExpand.SurveyPerson}，不可使用除、，；,;的分拆字符串。") ;
                    }
                }
                else
                {
                    if (!IsValidName(item.FamilyExpand.SurveyPerson))
                    {
                        ErrorInfo.Add($"\n承包方：{item.Name},调查人员:{item.FamilyExpand.SurveyPerson},不能包含数字、空格，除了 · 以外的的特殊字符");
                    }
                }
                var CheckNameflag = IsNames(item.FamilyExpand.CheckPerson);//判断承包方审核人员是否存在多个人名
                if (CheckNameflag)
                {
                    if (!GetInvalidNames(item.FamilyExpand.CheckPerson).IsNullOrEmpty())
                    {
                        ErrorInfo.Add($"\n承包方：{item.Name},审核人员存在多个人名:{item.FamilyExpand.CheckPerson}，不可使用除、，；,;的分拆字符串。");
                    }
                }
                else
                {
                    if (!IsValidName(item.FamilyExpand.CheckPerson))
                    {
                        ErrorInfo.Add($"\n承包方：{item.Name},审核人员:{item.FamilyExpand.CheckPerson},不能包含数字、空格，除了 · 以外的的特殊字符");
                    }
                }
            }
            return ErrorInfo;
        }

        private List<string> LandNameCheck()
        {
            var ErrorInfo = new List<string>();
            foreach (var item in contractLand)
            {
                var SurveyNameflag = IsNames(item.LandExpand.SurveyPerson);//判断地块调查人员是否存在多个人名
                if (SurveyNameflag)
                {
                    if (!GetInvalidNames(item.LandExpand.SurveyPerson).IsNullOrEmpty())
                    {
                        ErrorInfo.Add($"\n地块编码：{item.LandNumber},调查人员存在多个人名:{item.LandExpand.SurveyPerson}，不可使用除、，；,;的分拆字符串。") ;
                    }
                }
                else
                {
                    if (!IsValidName(item.LandExpand.SurveyPerson))
                    {
                        ErrorInfo.Add($"\n地块编码：{item.LandNumber},调查人员:{item.LandExpand.SurveyPerson},不能包含数字、空格，除了 · 以外的的特殊字符");
                    }
                }
                var CheckNameflag = IsNames(item.LandExpand.CheckPerson);//判断地块审核人员是否存在多个人名
                if (CheckNameflag)
                {
                    if (!GetInvalidNames(item.LandExpand.CheckPerson).IsNullOrEmpty())
                    {
                        ErrorInfo.Add($"\n地块编码：{item.LandNumber},审核人员存在多个人名:{item.LandExpand.CheckPerson}，不可使用除、，；,;的分拆字符串。");
                    }
                }
                else
                {
                    if (!IsValidName(item.LandExpand.CheckPerson))
                    {
                        ErrorInfo.Add($"\n地块编码：{item.LandNumber},审核人员:{item.LandExpand.CheckPerson},不能包含数字、空格，除了 · 以外的的特殊字符");
                    }
                }
            }
            return ErrorInfo;
        }
        
    }
}
