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
    /// 初始化二轮承包台账信息
    /// </summary>
    public partial class InitalizeLandSurveyInformation
    {
        #region Methods - Check

        /// <summary>
        /// 检查导入数据
        /// </summary>
        /// <returns></returns>
        public bool CheckImportSecondTableData(List<LandFamily> landFamilyCollection)
        {
            bool check = true;
            for (int i = 0; i < landFamilyCollection.Count; i++)
            {
                LandFamily onefamily = landFamilyCollection[i];
                if (string.IsNullOrEmpty(onefamily.TableFamily.Name))
                {
                    continue;
                }
                for (int j = i + 1; j < landFamilyCollection.Count; j++)
                {
                    LandFamily twofamily = landFamilyCollection[j];
                    if (onefamily.TableFamily.Name == twofamily.TableFamily.Name && ContractLandImportSurveyDefine.SecondNameIndex > 0)
                    {
                        string warnInfo = string.Format("二轮承包方{0}在表中重复存在!", onefamily.TableFamily.Name);
                        AddErrorMessage(warnInfo);
                    }
                    if (onefamily.TableFamily.Name == twofamily.TableFamily.Name && onefamily.TableFamily.Number != twofamily.TableFamily.Number && ContractLandImportSurveyDefine.SecondNameIndex > 0)
                    {
                        string warnInfo = string.Format("二轮承包方{0}在表中重复存在!", onefamily.TableFamily.Name);
                        AddErrorMessage(warnInfo);
                    }
                    if (onefamily.TableFamily.Name == twofamily.TableFamily.Name && onefamily.TableFamily.Number == twofamily.TableFamily.Number && !string.IsNullOrEmpty(onefamily.TableFamily.Number) && ContractLandImportSurveyDefine.SecondNameIndex > 0)
                    {
                        string errorInfo = string.Format("二轮承包方{0}在表中重复存在，并且身份证号码{1}相同!", onefamily.TableFamily.Name, onefamily.TableFamily.Number);
                        AddErrorMessage(errorInfo);
                        check = false;
                    }
                    for (int l = 0; l < onefamily.TablePersons.Count; l++)
                    {
                        Person per = onefamily.TablePersons[l];
                        if (string.IsNullOrEmpty(per.ICN))
                        {
                            continue;
                        }
                        for (int k = l + 1; k < onefamily.TablePersons.Count; k++)
                        {
                            Person pe = onefamily.TablePersons[k];
                            if (string.IsNullOrEmpty(pe.ICN))
                            {
                                continue;
                            }
                            if (pe.ICN == per.ICN && ContractLandImportSurveyDefine.SecondNumberIcnIndex > 0)
                            {
                                string errorInfo = string.Format("二轮家庭成员{0}身份证号码{1}与二轮家庭成员{2}身份证号码{3}重复!", per.Name, per.ICN, pe.Name, pe.ICN);
                                AddErrorMessage(errorInfo);
                                check = false;
                            }
                        }
                    }
                    foreach (Person p1 in onefamily.TablePersons)
                    {
                        if (string.IsNullOrEmpty(p1.ICN))
                        {
                            continue;
                        }
                        foreach (Person p2 in twofamily.TablePersons)
                        {
                            if (string.IsNullOrEmpty(p2.ICN))
                            {
                                continue;
                            }
                            if (p1.ICN == p2.ICN && ContractLandImportSurveyDefine.SecondNumberIcnIndex > 0)
                            {
                                string errorInfo = string.Format("二轮家庭成员{0}身份证号码{1}与二轮家庭成员{2}身份证号码{3}重复!", p1.Name, p1.ICN, p2.Name, p2.ICN);
                                AddErrorMessage(errorInfo);
                                check = false;
                            }
                        }
                    }

                }
            }
            return check;
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 获取Excel表中信息
        /// </summary>
        /// <param name="allItem">所有项目</param>
        private void InitalizeTableFamilyInformation(LandFamily landFamily)
        {
            string familyName = (ContractLandImportSurveyDefine.SecondNameIndex > 0 && ContractLandImportSurveyDefine.SecondNameIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNameIndex]) : landFamily.CurrentFamily.Name;
            string value = string.Empty;
            int count = 0;
            if (!string.IsNullOrEmpty(familyName))
            {
                landFamily.TableFamily.Name = familyName;
                landFamily.TableFamily.ID = landFamily.CurrentFamily.ID;
                if (string.IsNullOrEmpty(landFamily.TableFamily.FamilyNumber))
                {
                    landFamily.TableFamily.FamilyNumber = ToolString.ExceptSpaceString(GetString(allItem[currentIndex, 0]));
                }
                InitializeInnerTableFamily(landFamily);
                value = (ContractLandImportSurveyDefine.SecondNumberIndex > 0 && ContractLandImportSurveyDefine.SecondNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNumberIndex]) : landFamily.CurrentFamily.PersonCount;
                if (!string.IsNullOrEmpty(value))
                {
                    int.TryParse(value, out count);
                }
                if (count > 0)
                {
                    landFamily.TablePersonCount = count;
                    landFamily.TableFamily.PersonCount = count.ToString();
                }
                double temTotalArea = 0.0;
                string totalArea = (ContractLandImportSurveyDefine.SecondTotalTableAreaIndex > 0 && ContractLandImportSurveyDefine.SecondTotalTableAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondTotalTableAreaIndex]) : "";
                double.TryParse(totalArea, out temTotalArea);
                landFamily.TableFamily.TotalTableArea = temTotalArea;
                landFamily.TableFamily.TotalArea = landFamily.TableFamily.TotalTableArea > 0 ? "" : landFamily.TableFamily.TotalTableArea.ToString();
                InitalizeTableFamilyExtendInformation(landFamily.TableFamily);
            }
            else
            {
                if (string.IsNullOrEmpty(landFamily.TableFamily.Name) && ContractLandImportSurveyDefine.SecondNameIndex > 0)
                {
                    string errorInfo = string.Format("序号为{0}的二轮承包方姓名为空!", currentIndex + 1);
                    ReportErrorInfo(errorInfo);//记录错误信息
                }
            }
            InitalizeTablePerson(landFamily);
        }

        /// <summary>
        /// 初始化承包方信息
        /// </summary>
        /// <param name="landFamily"></param>
        /// <returns></returns>
        private void InitializeInnerTableFamily(LandFamily landFamily)
        {
            landFamily.TableFamily.ZoneCode = currentZone.FullCode;
            landFamily.TableFamily.ModifiedTime = DateTime.Now;
            landFamily.TableFamily.CreationTime = DateTime.Now;
        }

        /// <summary>
        /// 添加共有人
        /// </summary>
        /// <param name="landFamily"></param>
        /// <returns></returns>
        private bool InitalizeTablePerson(LandFamily landFamily)
        {
            string value = string.Empty;
            Person person = new Person();
            person.FamilyID = landFamily.TableFamily.ID;
            person.CreateTime = DateTime.Now;
            person.LastModifyTime = DateTime.Now;
            person.Nation = eNation.UnKnown;
            person.ZoneCode = currentZone.FullCode;
            //性别
            value = (ContractLandImportSurveyDefine.SecondNumberGenderIndex > 0 && ContractLandImportSurveyDefine.SecondNumberGenderIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNumberGenderIndex]) : "";
            person.Gender = string.IsNullOrEmpty(value) ? eGender.Unknow : GetGender(value);
            //2011年1月18日 0:12:50 Roc 增加从 Excel 中获取填写的年龄
            //年龄
            int age = 0;
            string strAge = (ContractLandImportSurveyDefine.SecondNumberAgeIndex > 0 && ContractLandImportSurveyDefine.SecondNumberAgeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNumberAgeIndex]) : "";
            Int32.TryParse(strAge, out age);
            //身份证号
            string icn = (ContractLandImportSurveyDefine.SecondNumberIcnIndex > 0 && ContractLandImportSurveyDefine.SecondNumberIcnIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNumberIcnIndex]) : "";
            //家庭关系
            person.Relationship = (ContractLandImportSurveyDefine.SecondNumberRelatioinIndex > 0 && ContractLandImportSurveyDefine.SecondNumberRelatioinIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNumberRelatioinIndex]) : "";
            //备注
            person.Comment = (ContractLandImportSurveyDefine.SecondFamilyCommentIndex > 0 && ContractLandImportSurveyDefine.SecondFamilyCommentIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondFamilyCommentIndex]) : "";
            //名称
            string name = (ContractLandImportSurveyDefine.SecondNumberNameIndex > 0 && ContractLandImportSurveyDefine.SecondNumberNameIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNumberNameIndex]) : "";
            if (string.IsNullOrEmpty(name))
            {
                name = (ContractLandImportSurveyDefine.ExPackageNameIndex > 0 && ContractLandImportSurveyDefine.ExPackageNameIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ExPackageNameIndex]) : "";//家庭成员名
            }
            if (string.IsNullOrEmpty(name) && (!string.IsNullOrEmpty(value) || age != 0 || !string.IsNullOrEmpty(icn)) && (ContractLandImportSurveyDefine.SecondNumberNameIndex > 0 || ContractLandImportSurveyDefine.ExPackageNameIndex > 0))
            {
                AddErrorMessage(string.Format("序号为{0}的二轮家庭成员姓名为空!", currentIndex + 1));
            }
            person.Name = name;
            InitalizeTablePersonInformation(landFamily, person, age, icn);
            InitalizeTablePersonExtendInformation(person);
            if (string.IsNullOrEmpty(landFamily.TableFamily.TotalArea))
            {
                landFamily.TableFamily.TotalArea = (ContractLandImportSurveyDefine.SecondTotalTableAreaIndex > 0 && ContractLandImportSurveyDefine.SecondTotalTableAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondTotalTableAreaIndex]) : "";//二轮台账总面积
            }
            if (!string.IsNullOrEmpty(person.Name))
            {
                landFamily.TablePersons.Add(person);
            }
            if (person.Name == landFamily.TableFamily.Name)
            {
                landFamily.TableFamily.SourceID = person.ID;
                landFamily.TableFamily.Number = person.ICN;
            }
            return true;
        }

        /// <summary>
        /// 初始化家庭成员信息
        /// </summary>
        private void InitalizeTablePersonInformation(LandFamily landFamily, Person person, int age, string icn)
        {
            if (age > 0)
            {
                person.Birthday = DateTime.Now.Date.AddYears(-age);
            }
            if (string.IsNullOrEmpty(icn))
            {
                person.ICN = "";
                if (!string.IsNullOrEmpty(person.Name) && (ContractLandImportSurveyDefine.SecondNumberNameIndex > 0 || ContractLandImportSurveyDefine.ExPackageNameIndex > 0))
                {
                    AddWarnMessage("二轮共有人" + person.Name + "的身份证号码为空");
                }
            }
            else
            {
                person.ICN = icn;
                VerifyTablePersonNumber(person);
                if (!string.IsNullOrEmpty(icn) && icn.Length != 15 && icn.Length != 18 && ContractLandImportSurveyDefine.SecondNumberIcnIndex > 0)
                {
                    ReportErrorInfo(string.Format("表中二轮共有人{0}的身份证号码{1}共{2}位,不满足身份证号码15位或18位要求!", person.Name, icn, icn.Length));
                }
            }
            if (person.Gender == eGender.Unknow && !string.IsNullOrEmpty(person.Name) && ContractLandImportSurveyDefine.SecondNumberGenderIndex > 0)
            {
                AddWarnMessage("二轮共有人" + person.Name + "的性别填写不正确!");
            }
            //2011-2-18 weiwei  检查同户中存在同名成员的情况
            if (!string.IsNullOrEmpty(person.Name) && (ContractLandImportSurveyDefine.SecondNumberNameIndex > 0 || ContractLandImportSurveyDefine.ExPackageNameIndex > 0))
            {
                if (existTablePersons.ContainsKey(person.Name))
                {
                    AddWarnMessage("二轮承包方" + landFamily.TableFamily.Name + "下存在同名的成员：" + person.Name);
                }
                else
                {
                    existTablePersons.Add(person.Name, person.Name);
                }
            }
        }

        /// <summary>
        /// 初始化二轮承包方扩展信息
        /// </summary>
        private void InitalizeTableFamilyExtendInformation(VirtualPerson family)
        {
            VirtualPersonExpand familyExpand = new VirtualPersonExpand();
            familyExpand.ID = family.ID;
            familyExpand.Name = family.Name;
            familyExpand.HouseHolderName = family.Name;
            familyExpand.FirstContractorPersonNumber = (ContractLandImportSurveyDefine.FirstContractorPersonNumberIndex > 0 && ContractLandImportSurveyDefine.FirstContractorPersonNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FirstContractorPersonNumberIndex]) : "";
            familyExpand.FirstContractArea = (ContractLandImportSurveyDefine.FirstContractAreaIndex > 0 && ContractLandImportSurveyDefine.FirstContractAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FirstContractAreaIndex]) : "";
            familyExpand.SecondContractorPersonNumber = (ContractLandImportSurveyDefine.SecondContractorPersonNumberIndex > 0 && ContractLandImportSurveyDefine.SecondContractorPersonNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondContractorPersonNumberIndex]) : "";
            familyExpand.SecondExtensionPackArea = (ContractLandImportSurveyDefine.SecondExtensionPackAreaIndex > 0 && ContractLandImportSurveyDefine.SecondExtensionPackAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondExtensionPackAreaIndex]) : "";
            familyExpand.FoodCropArea = (ContractLandImportSurveyDefine.FoodCropAreaIndex > 0 && ContractLandImportSurveyDefine.FoodCropAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.FoodCropAreaIndex]) : "";
            family.OtherInfomation = familyExpand.ToString();//扩展信息
        }

        /// <summary>
        /// 设置家庭信息
        /// </summary>
        /// <param name="family">户对象</param>
        /// <param name="familyName">户主名</param>
        /// <param name="allItem">所有值</param>
        private void InitalizeTablePersonExtendInformation(Person person)
        {
            person.ExtensionPackageNumber = (ContractLandImportSurveyDefine.ExPackageNumberIndex > 0 && ContractLandImportSurveyDefine.ExPackageNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ExPackageNumberIndex]) : "";//延包土地份数
            person.IsDeaded = (ContractLandImportSurveyDefine.IsDeadedIndex > 0 && ContractLandImportSurveyDefine.IsDeadedIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsDeadedIndex]) : "";//已死亡人员
            person.IsSharedLand = (ContractLandImportSurveyDefine.IsSharedLandIndex > 0 && ContractLandImportSurveyDefine.IsSharedLandIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.IsSharedLandIndex]) : "";//是否享有承包地
            person.LocalMarriedRetreatLand = (ContractLandImportSurveyDefine.LocalMarriedRetreatLandIndex > 0 && ContractLandImportSurveyDefine.LocalMarriedRetreatLandIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.LocalMarriedRetreatLandIndex]) : "";//出嫁后未退承包地人员
            person.PeasantsRetreatLand = (ContractLandImportSurveyDefine.PeasantsRetreatLandIndex > 0 && ContractLandImportSurveyDefine.PeasantsRetreatLandIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.PeasantsRetreatLandIndex]) : "";//农转非后未退承包地人员
            person.ForeignMarriedRetreatLand = (ContractLandImportSurveyDefine.ForeignMarriedRetreatLandIndex > 0 && ContractLandImportSurveyDefine.ForeignMarriedRetreatLandIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.ForeignMarriedRetreatLandIndex]) : "";//婚进在婚出地未退承包地
            string value = (ContractLandImportSurveyDefine.SecondNationIndex > 0 && ContractLandImportSurveyDefine.SecondNationIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNationIndex]) : "";
            if (!string.IsNullOrEmpty(value) && value.Length == 1)
            {
                value += "族";
            }
            object obj = EnumNameAttribute.GetValue(typeof(eNation), value);
            if (obj != null)
            {
                person.Nation = (eNation)EnumNameAttribute.GetValue(typeof(eNation), value);
            }
            value = string.Empty;
            obj = null;
            string birthDay = (ContractLandImportSurveyDefine.SecondAgeIndex > 0 && ContractLandImportSurveyDefine.FoodCropAreaIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondAgeIndex]) : "";
            if (string.IsNullOrEmpty(birthDay))
            {
                return;
            }
            string[] values = birthDay.Split(new char[] { '.' });
            if (values.Length != 3)
            {
                return;
            }
            int year, month, day;
            year = month = day = 0;
            Int32.TryParse(values[0], out year);
            Int32.TryParse(values[1], out month);
            Int32.TryParse(values[2], out day);
            try
            {
                person.Birthday = new DateTime(year, month, day);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 设置共有人值
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private void VerifyTablePersonNumber(Person person)
        {
            if (string.IsNullOrEmpty(person.ICN))
            {
                return;
            }
            if (ToolICN.Check(person.ICN))
            {
                switch (ToolICN.GetGender(person.ICN))
                {
                    case 1:
                        person.Gender = eGender.Male;
                        break;
                    case 0:
                        person.Gender = eGender.Female;
                        break;
                    default:
                        person.Gender = eGender.Unknow;
                        break;
                }
                person.Birthday = ToolICN.GetBirthday(person.ICN);
            }
            else
            {
                person.ICN = person.ICN.Trim();
                if (person.ICN.Length == 18 || person.ICN.Length == 15)
                {
                    if (person.Gender == eGender.Unknow)
                    {
                        switch (ToolICN.GetGenderInNotCheck(person.ICN))
                        {
                            case 1:
                                person.Gender = eGender.Male;
                                break;
                            case 0:
                                person.Gender = eGender.Female;
                                break;
                            default:
                                person.Gender = eGender.Unknow;
                                break;
                        }
                    }
                    person.Birthday = ToolICN.GetBirthdayInNotCheck(person.ICN);
                    if (ContractLandImportSurveyDefine.SecondNumberIcnIndex > 0)
                    {
                        AddWarnMessage("二轮共有人" + person.Name + "的身份证号码：" + person.ICN + "不符合身份证验证规则!");
                    }
                }
            }
        }

        /// <summary>
        /// 获取Excel表中信息
        /// </summary>
        /// <param name="allItem">所有项目</param>
        private void InitzlizeTableLandInformation(LandFamily landFamily)
        {
            SecondTableLand land = new SecondTableLand();
            //地块编号
            land.CadastralNumber = (ContractLandImportSurveyDefine.CadastralNumberIndex > 0 && ContractLandImportSurveyDefine.CadastralNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.CadastralNumberIndex]) : "";
            InitializeInnerTableLand(land, landFamily);
            //小地名
            land.Name = (ContractLandImportSurveyDefine.SecondLandNameIndex > 0 && ContractLandImportSurveyDefine.SecondLandNameIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondLandNameIndex]) : "";
            //二轮承包地台账面积
            land.TableArea = (ContractLandImportSurveyDefine.SecondTableAreaIndex > 0 && ContractLandImportSurveyDefine.SecondTableAreaIndex < columnCount) ? GetDouble(allItem[currentIndex, ContractLandImportSurveyDefine.SecondTableAreaIndex]) : 0.0;
            if (land.TableArea < 0.0 && ContractLandImportSurveyDefine.SecondTableAreaIndex > 0)
            {
                AddErrorMessage(land.OwnerName + "下地块二轮承包地台账面积填写错误!");
            }
            //地类
            land.LandName = (ContractLandImportSurveyDefine.SecondLandTypeIndex > 0 && ContractLandImportSurveyDefine.SecondLandTypeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondLandTypeIndex]) : "";

            //四至信息 
            bool landNeighbor = IntializeTalbeLandNeighbor(land);

            //备注
            land.Comment = (ContractLandImportSurveyDefine.SecondCommentIndex > 0 && ContractLandImportSurveyDefine.SecondCommentIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondCommentIndex]) : "";
            if (string.IsNullOrEmpty(land.LandName) && string.IsNullOrEmpty(land.Name) && string.IsNullOrEmpty(land.Comment)
                && (land.TableArea == null || !land.TableArea.HasValue || land.TableArea.Value == 0.0))
            {
                return;
            }
            InitalizeTableLandExpandInformation(land);
            landFamily.TableLandCollection.Add(land);
        }

        /// <summary>
        /// 初始化内部承包地块
        /// </summary>
        /// <param name="land"></param>
        /// <param name="landFamily"></param>
        private void InitializeInnerTableLand(SecondTableLand land, LandFamily landFamily)
        {
            land.CadastralNumber = string.IsNullOrEmpty(land.CadastralNumber) ? currentIndex.ToString() : land.CadastralNumber;
            land.LandNumber = land.CadastralNumber;
            land.LandLevel = DictionaryTypeInfo.K; //this.listDLDJ.Find(c => c.Name == "未知").Code;
            land.TransferType = this.listLZLX.Find(c => c.Name == "其他").Code;
            land.OwnerName = landFamily.TableFamily.Name;
            land.OwnerId = landFamily.TableFamily.ID;
            land.LandCategory = DictionaryTypeInfo.CBDK; //this.listDKLB.Find(c => c.Name == "承包地块").Code;
            land.LandScopeLevel = this.listGDPDJ.Find(c => c.Name == "未知") == null ? "" : listGDPDJ.Find(c => c.Name == "未知").Code;
            land.CadastralZoneCode = currentZone.FullCode;
            land.LocationCode = currentZone.FullCode;
            land.LocationName = currentZone.FullName;
            land.AwareArea = land.ActualArea;
            land.IsFlyLand = false;
            land.IsTransfer = false;
        }

        /// <summary>
        /// 获取地块四至
        /// </summary>
        /// <param name="allItem"></param>
        /// <returns></returns>
        private bool IntializeTalbeLandNeighbor(SecondTableLand land)
        {
            bool flag = false;
            try
            {
                string value = (ContractLandImportSurveyDefine.SecondEastIndex > 0 && ContractLandImportSurveyDefine.SecondEastIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondEastIndex]) : "";
                land.NeighborEast = value;
                value = (ContractLandImportSurveyDefine.SecondSourthIndex > 0 && ContractLandImportSurveyDefine.SecondSourthIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondSourthIndex]) : "";
                land.NeighborSouth = value;
                value = (ContractLandImportSurveyDefine.SecondWestIndex > 0 && ContractLandImportSurveyDefine.SecondWestIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondWestIndex]) : "";
                land.NeighborWest = value;
                value = (ContractLandImportSurveyDefine.SecondNorthIndex > 0 && ContractLandImportSurveyDefine.SecondNorthIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondNorthIndex]) : "";
                land.NeighborNorth = value;
                flag = true;
                return flag;
            }
            catch
            {
                return flag;
            }
        }

        /// <summary>
        /// 初始化地块扩展信息
        /// </summary>
        private void InitalizeTableLandExpandInformation(SecondTableLand land)
        {
            AgricultureLandExpand landExpand = new AgricultureLandExpand();
            landExpand.ID = land.ID;
            landExpand.Name = land.Name;
            landExpand.HouseHolderName = land.OwnerName;
            string value = (ContractLandImportSurveyDefine.SecondLandNumberIndex > 0 && ContractLandImportSurveyDefine.SecondLandNumberIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondLandNumberIndex]) : "";
            landExpand.SecondLandNumber = value;
            value = (ContractLandImportSurveyDefine.SecondLandLevelIndex > 0 && ContractLandImportSurveyDefine.SecondLandLevelIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondLandLevelIndex]) : "";
            landExpand.SecondLandLevel = value;
            value = (ContractLandImportSurveyDefine.SecondLandPurposeIndex > 0 && ContractLandImportSurveyDefine.SecondLandPurposeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondLandPurposeIndex]) : "";
            landExpand.SecondLandPurpose = value;
            value = (ContractLandImportSurveyDefine.SecondArableTypeIndex > 0 && ContractLandImportSurveyDefine.SecondArableTypeIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondArableTypeIndex]) : "";
            landExpand.SecondLandType = value;
            value = (ContractLandImportSurveyDefine.SecondIsFarmerLandIndex > 0 && ContractLandImportSurveyDefine.SecondIsFarmerLandIndex < columnCount) ? GetString(allItem[currentIndex, ContractLandImportSurveyDefine.SecondIsFarmerLandIndex]) : "";
            landExpand.SecondIsFarmerLand = value;
        }

        #endregion
    }
}
