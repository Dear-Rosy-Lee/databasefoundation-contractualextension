/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包地信息处理
    /// </summary>
    public partial class InitalizeLandSurveyInformation
    {
        #region Methods-Land

        /// <summary>
        /// 获取Excel表中信息
        /// </summary>
        /// <param name="allItem">所有项目</param>
        private void InitalizeLandInformation(LandFamily landFamily, bool isNotLand, bool isCheckLandNumberRepeat)
        {
            //得到承包地
            GetContractLand(landFamily, isNotLand, isCheckLandNumberRepeat);
            landFamily.CountArea = 0.0;
        }

        /// <summary>
        /// 获取承包地块信息
        /// </summary>
        /// <param name="landFamily"></param>
        /// <returns></returns>
        private bool GetContractLand(LandFamily landFamily, bool isNotLand, bool isCheckLandNumberRepeat)
        {
            string value = string.Empty;
            string objValue = string.Empty;
            ContractLand land = new ContractLand();
            land.ConcordId = landFamily.Concord.ID;
            land.OwnerId = landFamily.CurrentFamily.ID;
            land.OwnerName = landFamily.CurrentFamily.Name;
            //地块编码
            value = ContractLandImportSurveyDefine.CadastralNumberIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.CadastralNumberIndex]) : "";
            //如果面积与编号同时为空  这行则省去
            if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(ContractLandImportSurveyDefine.ActualAreaIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ActualAreaIndex]) : "")
                && string.IsNullOrEmpty(ContractLandImportSurveyDefine.AwareAreaIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.AwareAreaIndex]) : ""))
            {
                return true;
            }
            if (string.IsNullOrEmpty(value))
            {
                ReportErrorInfo(this.ExcelName + string.Format("序号为{0}的地块编码为空!", currentIndex + 1));
            }

            land.CadastralNumber = value;
            //验证编号是否存在
            if (!CheckCadastralNumber(land.CadastralNumber, isCheckLandNumberRepeat) || !CheckCadastralNumber(landFamily, land.CadastralNumber, isCheckLandNumberRepeat))
                return false;

            InitializeInnerLand(land, landFamily);
            land.CadastralNumber = currentZone.FullCode.PadRight(17,'0') + value;//地籍编码为 发包方14位加3个0加地块编码

            //小地名
            value = ContractLandImportSurveyDefine.LandNameIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandNameIndex]) : "";
            if (string.IsNullOrEmpty(value))
                land.Name = "";
            else
                land.Name = value;
            //畦数
            value = ContractLandImportSurveyDefine.PlotNumberIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.PlotNumberIndex]) : "";
            if (!string.IsNullOrEmpty(value) && (!ToolMath.MatchAllNumber(value) || value.Length > 5))
            {
                AddErrorMessage(this.ExcelName + string.Format("序号为{0}畦数{1}不正确", currentIndex + 1, value));
            }
            else
            {
                if (string.IsNullOrEmpty(value))
                    land.PlotNumber = "";
                else
                    land.PlotNumber = ToolString.GetLeftNumberWithInString(value);
            }
            //实测面积
            string actualAreaString = ContractLandImportSurveyDefine.ActualAreaIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ActualAreaIndex]) : "";
            land.ActualArea = ContractLandImportSurveyDefine.ActualAreaIndex > 0 ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.ActualAreaIndex]) : 0.0;
            if (!allowNoWriteActualArea && string.IsNullOrEmpty(actualAreaString))
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块编码为" + (ContractLandImportSurveyDefine.CadastralNumberIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.CadastralNumberIndex]) : "") + " 的承包地实测面积数据填写不完整!");
            }
            //确权面积
            string awareAreaString = ContractLandImportSurveyDefine.AwareAreaIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.AwareAreaIndex]) : "";
            land.AwareArea = ContractLandImportSurveyDefine.AwareAreaIndex > 0 ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.AwareAreaIndex]) : 0.0;

            if (!allowNoWriteAwareArea && string.IsNullOrEmpty(awareAreaString))
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块编码为" + (ContractLandImportSurveyDefine.CadastralNumberIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.CadastralNumberIndex]) : "") + " 的承包地确权面积数据填写不完整!");
            }
            if (land.AwareArea > land.ActualArea && !allowAwareAreaBigActualArea)
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块编码为" + (ContractLandImportSurveyDefine.CadastralNumberIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.CadastralNumberIndex]) : "") + " 的承包地确权面积大于实测面积!");
            }
            //机动地面积
            land.MotorizeLandArea = ContractLandImportSurveyDefine.MotorizeAreaIndex > 0 ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.MotorizeAreaIndex]) : 0.0;
            land.ContractDelayArea = ContractLandImportSurveyDefine.ContractDelayAreaIndex > 0 ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.ContractDelayAreaIndex]) : 0.0;
            //二轮承包地台账
            land.TableArea = ContractLandImportSurveyDefine.TableAreaIndex > 0 ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.TableAreaIndex]) : 0.0;
            if (allowNoWriteActualArea && land.ActualArea < 0.0)
                land.ActualArea = 0.0;
            if (!allowNoWriteActualArea && land.ActualArea <= 0.0 && ContractLandImportSurveyDefine.ActualAreaIndex > 0)
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块实测面积填写错误!");
            if (allowNoWriteAwareArea && land.AwareArea < 0.0)
                land.AwareArea = 0.0;
            if (!allowNoWriteAwareArea && land.AwareArea <= 0.0 && ContractLandImportSurveyDefine.AwareAreaIndex > 0)
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块确权面积填写错误!");
            if (land.MotorizeLandArea < 0.0 && ContractLandImportSurveyDefine.MotorizeAreaIndex > 0)
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块机动地面积填写错误!");
            if (land.TableArea < 0.0 && ContractLandImportSurveyDefine.TableAreaIndex > 0)
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块二轮承包地台账面积填写错误!");
            //地类
            value = ContractLandImportSurveyDefine.LandTypeIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandTypeIndex]) : "";

            if (!string.IsNullOrEmpty(value))
            {
                value = value == "田" ? "水田" : (value == "土" ? "旱地" : value);
                var landType = this.listTDLYLX.Find(t => t.Name == value);
                land.LandCode = landType != null ? landType.Code : "01";
                land.LandName = value;
            }
            /* 修改与2016/8/27 如果土地利用类型列未填或未匹配 则为空 */
            //else
            //{
            //    land.LandCode = "01";
            //    land.LandName = "耕地";
            //}
            if (ContractLandImportSurveyDefine.LandPlantIndex > 0)
            {
                land.PlantType = LandProtectionConvert.Instance.ConvertBack(GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandPlantIndex])).ToString();
            }
            //经营方式
            land.ManagementType = GetManageType();
            //四至信息
            bool landNeighbor = GetLandNeighbor(land);
            //原户主姓名（曾经耕种）
            land.FormerPerson = ContractLandImportSurveyDefine.SourceNameIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SourceNameIndex]) : "";
            //宗地座落方位描述
            land.ExtendA = ContractLandImportSurveyDefine.LandLocationIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandLocationIndex]) : "";
            string isTranster = ContractLandImportSurveyDefine.IsTransterIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsTransterIndex]) : "";
            if (!string.IsNullOrEmpty(isTranster) && isTranster != "是" && isTranster != "否")
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块是否流转" + isTranster + "填写错误,内容应是(是、否)其中一种!");
            }
            land.IsTransfer = isTranster == "是" ? true : false;
            string constructMode = ContractLandImportSurveyDefine.ConstructModeIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ConstructModeIndex]) : "";
            var dictList = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            if (dictList != null && dictList.Count > 0)
            {
                var dictMode = dictList.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name.Equals(constructMode));
                if (dictMode != null)
                {
                    objValue = dictMode.Code;
                }
            }
            if (!string.IsNullOrEmpty(constructMode) && string.IsNullOrEmpty(objValue))
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块承包方式" + constructMode + "填写错误!应是(" + InitalizeEnumDescription(typeof(eConstructMode), 3) + ")其中一种!");
            }
            int cMode = (int)eConstructMode.Family;
            land.ConstructMode = string.IsNullOrEmpty(objValue) ? cMode.ToString() : objValue;
            string farmerLand = ContractLandImportSurveyDefine.IsFarmerLandIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsFarmerLandIndex]) : "";
            if (!string.IsNullOrEmpty(farmerLand) && farmerLand != "是" && farmerLand != "否")
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块是否基本农田" + farmerLand + "填写错误,内容应是(是、否)其中一种!");
            }
            if (farmerLand == "是")
            {
                land.IsFarmerLand = true;
            }
            else if (farmerLand == "否")
            {
                land.IsFarmerLand = false;
            }
            else
            {
                land.IsFarmerLand = null;
            }
            string transferMode = ContractLandImportSurveyDefine.TransterModeIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.TransterModeIndex]) : "";
            objValue = string.IsNullOrEmpty(transferMode) ? land.TransferType :
               (listLZLX.Find(c => c.Name == transferMode) == null ? null : listLZLX.Find(c => c.Name == transferMode).Code);   // EnumNameAttribute.GetValue(typeof(eTransferType), transferMode);
            if (!string.IsNullOrEmpty(transferMode) && objValue == null)
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块流转方式" + transferMode + "填写错误!应是(" + InitalizeEnumDescription(typeof(eTransferType), 3) + ")其中一种!");
            }
            land.TransferType = objValue != null ? objValue.ToString() :
                (listLZLX.Find(c => c.Name == "未知") == null ? ((int)eTransferType.Other2).ToString() : listLZLX.Find(c => c.Name == "未知").Code);   //eTransferType.Other2
            land.TransferTime = ContractLandImportSurveyDefine.TransterTermIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.TransterTermIndex]) : "";
            land.PertainToArea = ContractLandImportSurveyDefine.TransterAreaIndex > 0 ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.TransterAreaIndex]) : 0.0;
            string platType = ContractLandImportSurveyDefine.PlatTypeIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.PlatTypeIndex]) : "";
            objValue = string.IsNullOrEmpty(platType) ? land.PlatType :
               (listZZLX.Find(c => c.Name == platType) == null ? null : listZZLX.Find(c => c.Name == platType).Code);   // EnumNameAttribute.GetValue(typeof(ePlantingType), platType);
            if (!string.IsNullOrEmpty(platType) && objValue == null)
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块种植类型" + platType + "填写错误!应是(" + InitalizeEnumDescription(typeof(ePlantingType), 3) + ")其中一种!");
                land.PlatType = listZZLX.Find(c => c.Name == "其它") == null ? ((int)ePlantingType.Other).ToString() : listZZLX.Find(c => c.Name == "其它").Code;   // ePlantingType.Other;
            }
            else
            {
                land.PlatType = objValue != null ? objValue.ToString() :
                    (listZZLX.Find(c => c.Name == "其它") == null ? ((int)ePlantingType.Other).ToString() : listZZLX.Find(c => c.Name == "其它").Code);  //ePlantingType.Other
            }
            string landLevel = ContractLandImportSurveyDefine.LandLevelIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandLevelIndex]) : "";
            objValue = string.IsNullOrEmpty(landLevel) ? land.LandLevel :
               (listDLDJ.Find(c => c.Name == landLevel) == null ? null : listDLDJ.Find(c => c.Name == landLevel).Code);   // EnumNameAttribute.GetValue(typeof(ePlantingType), platType);
            if (!string.IsNullOrEmpty(landLevel) && objValue == null)
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地力等级" + landLevel + "填写错误!应是(" + InitalizeEnumDescription(typeof(eContractLandLevel), 3) + ")其中一种!");
            }
            land.LandLevel = objValue != null ? objValue.ToString() : string.Empty;
            //(listDLDJ.Find(c => c.Name == "未知") == null ? ((int)eContractLandLevel.UnKnow).ToString() : listDLDJ.Find(c => c.Name == "未知").Code);   //eContractLandLevel.UnKnow
            string landPurpose = ContractLandImportSurveyDefine.LandPurposeIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandPurposeIndex]) : "";
            objValue = string.IsNullOrEmpty(landPurpose) ? land.Purpose :
               (listTDYT.Find(c => c.Name == landPurpose) == null ? null : listTDYT.Find(c => c.Name == landPurpose).Code);
            if (!string.IsNullOrEmpty(landPurpose) && objValue == null)
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下土地用途" + landPurpose + "填写错误!应是(" + InitalizeEnumDescription(typeof(eLandPurposeType), 3) + ")其中一种!");
            }
            land.Purpose = objValue != null ? objValue.ToString() :
                (listTDYT.Find(c => c.Name == "种植业") == null ? ((int)eLandPurposeType.Planting).ToString() : listTDYT.Find(c => c.Name == "种植业").Code);
            //备注
            land.Comment = ContractLandImportSurveyDefine.CommentIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.CommentIndex]) : "";
            land.Opinion = ContractLandImportSurveyDefine.OpinionIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.OpinionIndex]) : "";
            SetLandContractType(land);
            //SetFirmStock(land, isNotLand);
            if (TableType == 10)
            {
                SetLandExtendInformation(land);
            }
            InitalizeLandExpandInformation(land);
            landFamily.LandCollection.Add(land);
            return true;
        }

        ///// <summary>
        ///// 设置确股信息
        ///// </summary>
        ///// <param name="land"></param>
        //private void SetFirmStock(ContractLand land, bool isNotLand)
        //{
        //    try
        //    {
        //        var index = 41;
        //        if (isNotLand)
        //        {
        //            land.ShareArea = GetString(allItem[currentIndex, index++]);
        //            land.ConcordArea = GetString(allItem[currentIndex, index++]);
        //            land.StockQuantity = Convert.ToInt32(GetString(allItem[currentIndex, index++]));
        //            land.StockQuantityAdv = GetString(allItem[currentIndex, index++]);
        //            //land.Modulus = GetString(allItem[currentIndex, index++]);
        //            //land.QuantificatAreaByLand = GetString(allItem[currentIndex, index++]);
        //            //land.QuantificatAreaByStock = GetString(allItem[currentIndex, index]);
        //        }
        //        else
        //        {
        //            land.ShareArea = GetString(allItem[currentIndex, index++]);
        //            land.ConcordArea = GetString(allItem[currentIndex, index++]);
        //            //land.StockQuantity = Convert.ToInt32(GetString(allItem[currentIndex, index++]));
        //            //and.StockQuantityAdv = GetString(allItem[currentIndex, index++]);
        //            land.Modulus = GetString(allItem[currentIndex, index++]);
        //            //land.QuantificatAreaByLand = GetString(allItem[currentIndex, index++]);
        //            //land.QuantificatAreaByStock = GetString(allItem[currentIndex, index]);
        //        }
        //        //if (!string.IsNullOrWhiteSpace(land.QuantificatAreaByStock))
        //        //{
        //        //    land.IsStockLand = true;
        //        //}
        //    }
        //    catch (Exception)
        //    {
        //    }

        //}

        /// <summary>
        /// 初始化枚举类型字符串
        /// </summary>
        /// <returns></returns>
        private string InitalizeEnumDescription(Type enumType, int lenght = 0)
        {
            EnumNameAttribute[] values = EnumNameAttribute.GetAttributes(enumType);
            string description = "";
            if (lenght == 0 || lenght > values.Length)
                lenght = values.Length;
            for (int i = 0; i < lenght; i++)
            {
                description += values[i].Description;
                description += "、";
            }
            description = description.TrimEnd('、');
            if (lenght < values.Length)
            {
                description += "等等";
            }
            values = null;
            return description;
        }

        /// <summary>
        /// 设置地块扩展信息
        /// </summary>
        private void SetLandExtendInformation(ContractLand land)
        {
            switch (TableType)
            {
                case 10://表格中含有摸底现状地块名称、地类、现状面积
                    land.ExtendA = string.IsNullOrEmpty(land.ExtendA) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 7]) : land.ExtendA;//现状地块名称
                    land.ExtendC = string.IsNullOrEmpty(land.ExtendC) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 8]) : land.ExtendC;//现状地类
                    land.PertainToArea = GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex + 9]);//现状面积
                    break;

                case 20:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 设置耕地类型
        /// </summary>
        private void SetLandContractType(ContractLand land)
        {
            string arableType = ContractLandImportSurveyDefine.ArableTypeIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ArableTypeIndex]) : "";
            string cModeDec = EnumNameAttribute.GetDescription(eLandCategoryType.ContractLand);
            int cModeCode = (int)eLandCategoryType.ContractLand;
            if (string.IsNullOrEmpty(arableType))
            {
                var cDict = this.listDKLB.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == cModeDec);
                land.LandCategory = cDict == null ? cModeCode.ToString() : cDict.Code;
                return;
            }
            string objValue = this.listDKLB.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == arableType) == null ? null :
                this.listDKLB.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == arableType).Code;  //EnumNameAttribute.GetValue(typeof(eConstructType), arableType);
            if (objValue == null)
            {
                AddErrorMessage(this.ExcelName + land.OwnerName + "下地块类别" + arableType + "填写错误!应是(" + InitalizeEnumDescription(typeof(eLandCategoryType), 3) + ")其中一种!");
                return;
            }
            land.LandCategory = objValue;

            //bool isFind = false;
            //EnumNameAttribute[] values = EnumNameAttribute.GetAttributes(typeof(eLandCategoryType));
            //for (int i = 0; i < values.Length; i++)//增加一个土地类型的判别2012-1-4
            //{
            //    if (values[i].Description == arableType)
            //    {
            //        objValue = this.listDKLB.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == values[i].Description.ToString()).Code;   // EnumNameAttribute.GetValue(typeof(eConstructType), values[i].Value.ToString());
            //        if (objValue != null)
            //        {
            //            land.LandCategory = objValue.ToString();
            //            isFind = true;
            //        }
            //        break;
            //    }
            //}
            //if (!isFind)
            //{
            //    land.LandCategory = this.listDKLB.Find(c => c.Name == "其他集体土地").Code;   //eConstructType.CollectiveLand;
            //}
        }

        /// <summary>
        /// 获取经营方式
        /// </summary>
        /// <returns></returns>
        private string GetManageType()
        {
            string value = ContractLandImportSurveyDefine.ManagementTypeIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ManagementTypeIndex]) : "";
            if (string.IsNullOrEmpty(value))
            {
                return "9";
            }
            object objValue = EnumNameAttribute.GetValue(typeof(eManageType), value);
            if (objValue != null)
            {
                return objValue as string;
            }
            switch (value.Trim())
            {
                case "自营":
                    return "1";

                case "租赁":
                    return "2";

                case "转让":
                    return "3";

                case "互换":
                    return "4";
            }
            return "9";
        }

        /// <summary>
        /// 初始化内部承包地块
        /// </summary>
        /// <param name="land"></param>
        /// <param name="landFamily"></param>
        private void InitializeInnerLand(ContractLand land, LandFamily landFamily)
        {
            land.LandNumber = land.CadastralNumber;
            land.SurveyNumber = land.CadastralNumber;
            //land.LandLevel = this.listDLDJ.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "未知") == null ? ((int)eContractLandLevel.UnKnow).ToString() : this.listDLDJ.Find(c => c.Name == "未知").Code;
            land.TransferType = this.listLZLX.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "未知") == null ? ((int)eTransferType.Other2).ToString() :
                this.listLZLX.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "未知").Code;     //eTransferType.Other;
            land.OwnerName = landFamily.CurrentFamily.Name;
            land.OwnerId = landFamily.CurrentFamily.ID;
            land.LandCategory = this.listDKLB.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "承包地块") == null ? ((int)eLandCategoryType.ContractLand).ToString() : this.listDKLB.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "承包地块").Code;   //eConstructType.ContractLand;
            land.LandScopeLevel = this.listGDPDJ.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "未知") == null ? ((int)eLandSlopeLevel.UnKnown).ToString() :
                this.listGDPDJ.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "未知").Code;  //eLandSlopeLevel.UnKnown;
            land.OwnRightType = this.listSYQXZ.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "集体土地所有权") == null ? ((int)eLandPropertyType.Collectived).ToString() : this.listSYQXZ.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "集体土地所有权").Code;//eLandPropertyType.Collectived;
            land.ConstructMode = this.listCBJYQQDFS.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "家庭承包") == null ? ((int)eConstructMode.Family).ToString() :
                this.listCBJYQQDFS.Find(c => !string.IsNullOrEmpty(c.Name) && c.Name == "家庭承包").Code;
            land.SenderCode = currentZone.FullCode;
            land.SenderName = currentZone.FullName;
            land.ZoneCode = currentZone.FullCode;
            land.ZoneName = currentZone.FullName;
            land.AwareArea = land.ActualArea;  //发证面积为了打证而用。
            land.IsFlyLand = false;
            land.IsTransfer = false;
        }

        /// <summary>
        /// 获取地块四至
        /// </summary>
        /// <param name="allItem"></param>
        /// <returns></returns>
        private bool GetLandNeighbor(ContractLand land)
        {
            try
            {
                string value = ContractLandImportSurveyDefine.EastIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.EastIndex]) : "";
                land.NeighborEast = value;

                value = ContractLandImportSurveyDefine.SourthIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SourthIndex]) : "";
                land.NeighborSouth = value;

                value = ContractLandImportSurveyDefine.WestIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.WestIndex]) : "";
                land.NeighborWest = value;

                value = ContractLandImportSurveyDefine.NorthIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.NorthIndex]) : "";
                land.NeighborNorth = value;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取四至
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetNeighbor(string value)
        {
            string neighbor = "";
            if (string.IsNullOrEmpty(value))
            {
                neighbor += "\r";
            }
            else
            {
                neighbor += value + "\r";
            }
            return neighbor;
        }

        /// <summary>
        /// 获取耕保类型
        /// </summary>
        /// <param name="plantType"></param>
        /// <returns></returns>
        public string GetPlantProtectType(string plantType)
        {
            if (plantType.IsNullOrEmpty())
            {
                return "3";
            }
            if (plantType == "一" || plantType == "一类")
            {
                return "1";
            }
            if (plantType == "二" || plantType == "二类")
            {
                return "2";
            }
            string objValue = this.listGBZL.Find(c => c.Name == plantType).Code; //EnumNameAttribute.GetValue(typeof(ePlantProtectType), plantType);
            if (objValue != null)
            {
                return objValue;
            }
            return "3";
        }

        #endregion Methods-Land

        #region Methods-Extend

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        private void InitalizeLandExpandInformation(ContractLand land)
        {
            AgricultureLandExpand landExpand = new AgricultureLandExpand();
            landExpand.ID = land.ID;
            landExpand.Name = land.Name;
            landExpand.HouseHolderName = land.OwnerName;
            string value = ContractLandImportSurveyDefine.UseSituationIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.UseSituationIndex]) : "";
            landExpand.UseSituation = value;
            value = ContractLandImportSurveyDefine.YieldIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.YieldIndex]) : "";
            landExpand.Yield = value;
            value = ContractLandImportSurveyDefine.OutputValueIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.OutputValueIndex]) : "";
            landExpand.OutputValue = value;
            value = ContractLandImportSurveyDefine.IncomeSituationIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IncomeSituationIndex]) : "";
            landExpand.IncomeSituation = value;
            value = ContractLandImportSurveyDefine.ManagementTypeIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ManagementTypeIndex]) : "";
            landExpand.ManagerMode = value;
            landExpand.SurveyPerson = ContractLandImportSurveyDefine.LandSurveyPersonIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandSurveyPersonIndex]) : "";
            string cellValue = ContractLandImportSurveyDefine.LandSurveyDateIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandSurveyDateIndex]) : "";
            landExpand.SurveyDate = ContractLandImportSurveyDefine.LandSurveyDateIndex > 0 ? GetDateTime(allItem[currentIndex, ContractLandImportSurveyDefine.LandSurveyDateIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (landExpand.SurveyDate == null || !landExpand.SurveyDate.HasValue))
            {
                string information = this.ExcelName + string.Format("表中地块编码为{0}的地块调查日期{1}不符合日期填写要求!", ContractLand.GetLandNumber(land.CadastralNumber), cellValue);
                AddErrorMessage(information);
            }
            landExpand.SurveyChronicle = ContractLandImportSurveyDefine.LandSurveyChronicleIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandSurveyChronicleIndex]) : "";
            landExpand.CheckPerson = ContractLandImportSurveyDefine.LandCheckPersonIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandCheckPersonIndex]) : "";
            cellValue = ContractLandImportSurveyDefine.LandCheckDateIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandCheckDateIndex]) : "";
            landExpand.CheckDate = ContractLandImportSurveyDefine.LandCheckDateIndex > 0 ? GetDateTime(allItem[currentIndex, ContractLandImportSurveyDefine.LandCheckDateIndex]) : null;
            if (!string.IsNullOrEmpty(cellValue) && (landExpand.CheckDate == null || !landExpand.CheckDate.HasValue))
            {
                string information = this.ExcelName + string.Format("表中地块编码为{0}的地块审核日期{1}不符合日期填写要求!", ContractLand.GetLandNumber(land.CadastralNumber), cellValue);
                AddErrorMessage(information);
            }
            landExpand.CheckOpinion = ContractLandImportSurveyDefine.LandCheckOpinionIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LandCheckOpinionIndex]) : "";
            landExpand.ReferPerson = ContractLandImportSurveyDefine.ReferPersonIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ReferPersonIndex]) : "";
            landExpand.ImageNumber = ContractLandImportSurveyDefine.ImageNumberIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ImageNumberIndex]) : "";
            land.LandExpand = landExpand;
            if (ContractLandImportSurveyDefine.LandSurveyDateIndex < 0 || ContractLandImportSurveyDefine.LandCheckDateIndex < 0)
            {
                return;
            }
            if (landExpand.SurveyDate == null || !landExpand.SurveyDate.HasValue || landExpand.CheckDate == null || !landExpand.CheckDate.HasValue)
            {
                return;
            }
            string landNumber = ContractLandImportSurveyDefine.CadastralNumberIndex > 0 ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.CadastralNumberIndex]) : "";
            if (landExpand.SurveyDate.Value > landExpand.CheckDate.Value && !string.IsNullOrEmpty(landNumber))
            {
                string errorInformation = this.ExcelName + string.Format("表中地块编码为{0}的地块调查日期{1}大于审核日期{2}!", landNumber, ToolDateTime.GetLongDateString(landExpand.SurveyDate.Value), ToolDateTime.GetLongDateString(landExpand.CheckDate.Value));
                AddErrorMessage(errorInformation);
            }
            landExpand = null;
        }

        #endregion Methods-Extend

        #region Methods - Concord

        /// <summary>
        /// 统计合同地块面积
        /// </summary>
        /// <param name="landFamily"></param>
        /// <returns></returns>
        private LandFamily CountLandAreaToConcord(LandFamily landFamily)
        {
            if (landFamily.LandCollection == null || landFamily.LandCollection.Count < 1)
            {
                AddWarnMessage(this.ExcelName + "表中" + landFamily.CurrentFamily.Name + "下没有承包地数据，已略过！");
                landFamily.Concord = null;
                landFamily.RegeditBook = null;
                return landFamily;
            }
            landFamily.Concord.TotalTableArea = 0.0;
            foreach (ContractLand land in landFamily.LandCollection)
            {
                if (land.TableArea.HasValue)
                    landFamily.Concord.TotalTableArea += land.TableArea;

                if (land.MotorizeLandArea.HasValue)
                    landFamily.Concord.CountMotorizeLandArea += land.MotorizeLandArea.Value;

                landFamily.Concord.CountActualArea += land.ActualArea;
                landFamily.Concord.CountAwareArea += land.AwareArea;
            }

            CheckAreaValue(landFamily.CurrentFamily.Name);

            if (countTotalTableArea > 0.0)
            {
                landFamily.Concord.TotalTableArea = countTotalTableArea;
            }
            if (countActualArea > 0.0)
            {
                landFamily.Concord.CountActualArea = countActualArea;
            }
            if (countAwareArea > 0.0)
            {
                landFamily.Concord.CountAwareArea = countAwareArea;
            }
            if (countMotorizeLandArea > 0.0)
            {
                landFamily.Concord.CountMotorizeLandArea = countMotorizeLandArea;
            }
            else
            {
                landFamily.Concord.CountMotorizeLandArea = landFamily.Concord.CountActualArea - landFamily.Concord.CountAwareArea;
            }
            CheckConcordAreaValue(landFamily);

            CheckConcordDataValue(landFamily);

            return landFamily;
        }

        /// <summary>
        /// 检查合同面积
        /// </summary>
        /// <param name="landFamily"></param>
        private void CheckConcordAreaValue(LandFamily landFamily)
        {
            landFamily.Concord.CountActualArea = double.Parse(landFamily.Concord.CountActualArea.ToString());

            landFamily.Concord.CountAwareArea = double.Parse(landFamily.Concord.CountAwareArea.ToString());

            landFamily.Concord.CountMotorizeLandArea = double.Parse(landFamily.Concord.CountMotorizeLandArea.ToString());

            landFamily.Concord.TotalTableArea = double.Parse(landFamily.Concord.TotalTableArea.ToString());

            if (CheckAreaLength(landFamily.Concord.CountActualArea))
            {
                landFamily.Concord.CountActualArea += 0.0000001;
                landFamily.Concord.CountActualArea = Math.Round(landFamily.Concord.CountActualArea, 4, MidpointRounding.AwayFromZero);
            }

            if (CheckAreaLength(landFamily.Concord.CountAwareArea))
            {
                landFamily.Concord.CountAwareArea += 0.0000001;
                landFamily.Concord.CountAwareArea = Math.Round(landFamily.Concord.CountAwareArea, 4, MidpointRounding.AwayFromZero);
            }

            if (CheckAreaLength(landFamily.Concord.CountMotorizeLandArea))
            {
                landFamily.Concord.CountMotorizeLandArea += 0.0000001;
                landFamily.Concord.CountMotorizeLandArea = Math.Round(landFamily.Concord.CountMotorizeLandArea, 4, MidpointRounding.AwayFromZero);
            }

            if (CheckAreaLength(landFamily.Concord.TotalTableArea.Value))
            {
                landFamily.Concord.TotalTableArea += 0.0000001;
                landFamily.Concord.TotalTableArea = Math.Round(landFamily.Concord.TotalTableArea.Value, 4, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// 检查面积
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        private bool CheckAreaValue(string familyName)
        {
            countTotalTableArea = double.Parse(countTotalTableArea.ToString());
            countMotorizeLandArea = double.Parse(countMotorizeLandArea.ToString());
            countActualArea = double.Parse(countActualArea.ToString());
            countAwareArea = double.Parse(countAwareArea.ToString());
            string errorStr = "{0}下{1}总面积错误，请检查Excel是否所有单元格都是文本格式\r\n" +
                "并且每条承包地记录的{1}面积或总{1}面积小数位数是否正确！";
            if (CheckAreaLength(countActualArea))
            {
                countActualArea += 0.0000001;
                countActualArea = Math.Round(countActualArea, 4, MidpointRounding.AwayFromZero);
                AddErrorMessage(this.ExcelName + string.Format(errorStr, familyName, "实测"));
            }
            if (CheckAreaLength(countAwareArea))
            {
                countAwareArea += 0.0000001;
                countAwareArea = Math.Round(countAwareArea, 4, MidpointRounding.AwayFromZero);
                AddErrorMessage(this.ExcelName + string.Format(errorStr, familyName, "确权"));
            }
            if (CheckAreaLength(countTotalTableArea))
            {
                countTotalTableArea += 0.0000001;
                countTotalTableArea = Math.Round(countTotalTableArea, 4, MidpointRounding.AwayFromZero);
                if (CheckAreaLength(countTotalTableArea))
                {
                    AddErrorMessage(this.ExcelName + string.Format(errorStr, familyName, "二轮台账"));
                }
            }

            if (countMotorizeLandArea > 0.0)
            {
                if (CheckAreaLength(countMotorizeLandArea))
                {
                    countMotorizeLandArea += 0.0000001;
                    countMotorizeLandArea = Math.Round(countMotorizeLandArea,
                        GetDoubleLength(countActualArea) > GetDoubleLength(countAwareArea) ?
                        GetDoubleLength(countActualArea) : GetDoubleLength(countAwareArea), MidpointRounding.AwayFromZero);
                    if (CheckAreaLength(countMotorizeLandArea))
                    {
                        AddErrorMessage(this.ExcelName + string.Format(errorStr, familyName, "机动地"));
                    }
                }
            }
            else
            {
                if (countActualArea > 0.0 && countAwareArea > 0.0)
                    countMotorizeLandArea = countActualArea - countAwareArea;
            }

            return true;
        }

        /// <summary>
        /// 获取双精度值长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int GetDoubleLength(double value)
        {
            if (value == 0.0)
                return 1;
            string[] numberList = value.ToString().Split(new string[] { "." }, StringSplitOptions.None);
            if (numberList == null || numberList.Count() < 2)
                return 1;

            return numberList[1].Length;
        }

        /// <summary>
        /// 检查面积长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CheckAreaLength(double value)
        {
            int length = GetDoubleLength(value);
            if (length == 1)
                return false;

            if (length > 6)
                return true;

            return false;
        }

        /// <summary>
        /// 设置精度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double SetDecimalFraction(double value)
        {
            string[] numberList = value.ToString().Split(new string[] { "." }, StringSplitOptions.None);
            if (numberList == null || numberList.Count() < 2)
                return value;
            if (numberList[1].Length > 2)
            {
                if (int.Parse(numberList[1][2] + "") > 4)
                {
                    int i = int.Parse(numberList[1].Substring(0, 2)) + 1; //这里没有考虑到小数点后一位是0的情况，2011-5-7李昌松修改
                    if (i == 100)//针对于129.999 应是130.0 而不是129.1
                        return double.Parse(string.Format("{0}{1}{2}", int.Parse(numberList[0]) + 1, ".", "00"));
                    if (numberList[1].Substring(0, 1) == "0" && i < 10)// && i < 10  针对类似 123.099 应是123.1 而不是123.01
                    {
                        return double.Parse(string.Format("{0}{1}{2}", numberList[0], ".0", i));
                    }
                    else
                    {
                        return double.Parse(string.Format("{0}{1}{2}", numberList[0], ".", i));
                    }
                }
                return double.Parse(string.Format("{0}{1}{2}", numberList[0], ".", numberList[1].Substring(0, 2)));
            }
            return value;
        }

        /// <summary>
        /// 添加扩展属性
        /// </summary>
        private LandFamily InitalizeConcord(LandFamily landFamily)
        {
            if (!string.IsNullOrEmpty(landFamily.Concord.ConcordNumber) &&
           !string.IsNullOrEmpty(landFamily.RegeditBook.RegeditNumber) &&
           !string.IsNullOrEmpty(landFamily.RegeditBook.Number))
            {
                landFamily.Concord.ContractCredentialNumber = GetRegeditNumber(landFamily.Concord.ConcordNumber);//经营权证号
                landFamily.Concord.IsValid = true;
            }
            else
            {
                landFamily.Concord.IsValid = false;
            }

            landFamily = AddRegeditBook(landFamily);//添加登记薄
            landFamily.Concord.ArableLandEndTime = DateTime.Parse("2029-09-30");//承包结束时间
            landFamily.Concord.ArableLandStartTime = DateTime.Parse("2010-10-01"); //承包开始时间
            landFamily.Concord.ArableLandType = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "家庭承包").Code; //eConstructMode.Family 承包方式
            landFamily.Concord.ContracerType = landFamily.CurrentFamily.FamilyExpand.ContractorType.ToString(); //承包方类型
            landFamily.Concord.ContracterId = landFamily.CurrentFamily.ID;//承包方Id
            landFamily.Concord.ContracterIdentifyNumber = landFamily.CurrentFamily.Number;//承包方证件号
            landFamily.Concord.ContracterName = landFamily.CurrentFamily.Name;//承包方姓名
            landFamily.Concord.Flag = false;//长久标志
            if (landFamily.Concord.Flag)
            {
                landFamily.Concord.ManagementTime = "长久";
            }
            else
            {
                landFamily.Concord.ManagementTime = ToolDateTime.CalcateTerm(landFamily.Concord.ArableLandStartTime, landFamily.Concord.ArableLandEndTime); ;
            }
            landFamily.Concord.SenderId = Guid.NewGuid();//发包方Id
            landFamily.Concord.SenderName = "";//发包方名称
            landFamily.Concord.SenderDate = DateTime.Now;//颁证日期
            landFamily.Concord.CheckAgencyDate = DateTime.Now;//填证日期
            landFamily.Concord.Status = eStatus.Checked;//状态
            landFamily.Concord.ZoneCode = currentZone.FullCode;//地域代码
            landFamily.Concord.Founder = "Admin";//创建者
            landFamily.Concord.Modifier = "Admin";//修改者
            landFamily.Concord.ArableLandType = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "家庭承包").Code; //eConstructMode.Family;
            landFamily.Concord.LandPurpose = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT).Find(c => c.Name == "种植业").Code; // eLandPurposeType.Planting;
            landFamily = CountLandAreaToConcord(landFamily);//计算总面积
            return landFamily;
        }

        /// <summary>
        /// 获取合同编号
        /// </summary>
        /// <returns></returns>
        private string GetRegeditNumber(string number)
        {
            string regeditNumber = number;
            int index = 1;
            var dbContext = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            var bookStation = dbContext.CreateRegeditBookStation();
            while (bookStation.Exists(regeditNumber))
            {
                index++;
                regeditNumber = currentZone.FullCode + string.Format("{0:D4}", index);
            }
            return regeditNumber;
        }

        /// <summary>
        /// 添加登记薄
        /// </summary>
        private LandFamily AddRegeditBook(LandFamily landFamily)
        {
            landFamily.RegeditBook.ID = landFamily.Concord.ID;
            landFamily.RegeditBook.Count = 0;
            landFamily.RegeditBook.Founder = "Admin";
            landFamily.RegeditBook.Modifier = "Admin";
            landFamily.RegeditBook.PrintDate = DateTime.Now;
            return landFamily;
        }

        #endregion Methods - Concord
    }
}