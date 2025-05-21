using Quality.Business.Entity;
using System;
using System.Collections.Generic;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;

namespace YuLinTu.Component.ImportResultDataBaseTask
{
    public class LocalComplexRightEntity
    {
        #region Fields

        private static List<string> ErrorInfomation = new List<string>();
        #endregion

        #region Properties

        public string BookCode { get; set; }
        public string VirtualPersonCode { get; set; }
        public string ZoneCode { get; set; }

        public VirtualPerson CBF { get; set; }
        public List<ContractRegeditBook> CBJYQZ { get; set; }
        public List<ContractRegeditBook> DJB { get; set; }
        public List<ContractLand> DKXXS { get; set; }
        public CollectivityTissue FBF { get; set; }
        public List<ContractConcord> HT { get; set; }
        //地块ID和land对应的空间坐标,当前人的
        public Dictionary<Guid, string> LandguidkjzbList { get; set; }

        //地块ID和land对应的合同编码,当前人的
        public Dictionary<Guid, string> LandguidHTBMList { get; set; }
        //确股地块数据
        public List<Library.Entity.BelongRelation> BRDKS { get; set; }

        #endregion

        #region Ctor

        public LocalComplexRightEntity()
        {

        }

        #endregion

        #region Methods

        #region Methods-Main

        /// <summary>
        /// 转换获取交换实体及属性
        /// </summary>
        /// <param name="importComplexRightEntity"></param>
        public static LocalComplexRightEntity From(ComplexRightEntity importComplexRightEntity,
            Action<string> errorAction = null)
        {
            try
            {

                if (importComplexRightEntity == null)
                {
                    return null;
                }

                ErrorInfomation.Clear();
                var obj = new LocalComplexRightEntity();

                Dictionary<Guid, string> landguidkjzbList = new Dictionary<Guid, string>();
                Dictionary<Guid, string> landguidHTBMList = new Dictionary<Guid, string>();
                List<Library.Entity.BelongRelation> BRDKlist = new List<Library.Entity.BelongRelation>();
                obj.BookCode = importComplexRightEntity.BookCode == null ? "" : importComplexRightEntity.BookCode;
                obj.VirtualPersonCode = importComplexRightEntity.VirtualPersonCode == null
                    ? ""
                    : importComplexRightEntity.VirtualPersonCode;
                obj.ZoneCode = importComplexRightEntity.ZoneCode == null ? "" : importComplexRightEntity.ZoneCode;
                obj.CBF = InitalizeContractorData(importComplexRightEntity.CBF, importComplexRightEntity.JTCY,
                    obj.ZoneCode);
                obj.CBJYQZ = InitalizeWarrantBook(importComplexRightEntity.CBJYQZ, obj.ZoneCode);
                obj.DJB = InitalizeRegeditBook(importComplexRightEntity.DJB, obj.ZoneCode);
                obj.DKXXS = InitalizeAgricultureLandData(importComplexRightEntity.DKXX, obj.ZoneCode, landguidkjzbList,
                    landguidHTBMList, BRDKlist);
                obj.LandguidkjzbList = landguidkjzbList;
                obj.LandguidHTBMList = landguidHTBMList;
                obj.BRDKS = BRDKlist;
                obj.FBF = InitalizeSenderData(importComplexRightEntity.FBF, obj.ZoneCode);
                obj.HT = InitalizeConcordData(importComplexRightEntity.HT, importComplexRightEntity.DJB, obj.ZoneCode);

                return obj;
            }
            catch (Exception)
            {
                if (ErrorInfomation != null)
                {
                    foreach (var value in ErrorInfomation)
                    {
                        errorAction(value);
                    }
                }
                throw;
            }
        }

        #endregion

        #region Methods-deteil

        /// <summary>
        /// 获得承包方实体
        /// </summary>
        /// <returns></returns>
        private static VirtualPerson InitalizeContractorData(ICBF ICBFEntity, List<CBF_JTCY> entityJTCY, String zoneCode)
        {
            if (ICBFEntity == null)
            {
                return null;
            }
            CBFSC entityCBF = null;
            if (ICBFEntity is CBF)
            {
                entityCBF = (ICBFEntity as CBF).ConvertTo<CBFSC>();
            }
            else if (ICBFEntity is CBFSC)
            {
                entityCBF = ICBFEntity as CBFSC;
            }

            VirtualPerson VirtualPersonCBF = new LandVirtualPerson();
            VirtualPersonCBF.ZoneCode = zoneCode;
            VirtualPersonCBF.FamilyNumber = SubVirtualPersonFamilyNum(entityCBF.CBFBM);
            VirtualPersonCBF.Name = entityCBF.CBFMC == null ? "" : entityCBF.CBFMC;
            VirtualPersonCBF.CardType = (eCredentialsType)int.Parse(entityCBF.CBFZJLX);
            VirtualPersonCBF.Number = entityCBF.CBFZJHM.IsNullOrEmpty() ? "" : entityCBF.CBFZJHM;
            VirtualPersonCBF.Address = entityCBF.CBFDZ;
            VirtualPersonCBF.PostalNumber = entityCBF.YZBM;
            VirtualPersonCBF.Telephone = entityCBF.LXDH;
            VirtualPersonCBF.OldVirtualCode = entityCBF.QQCBFBM;
            VirtualPersonCBF.SharePersonList = InitalizeSharePersonData(entityJTCY, VirtualPersonCBF);

            VirtualPersonExpand vpFamilyExpand = new VirtualPersonExpand();
            vpFamilyExpand.SurveyDate = entityCBF.CBFDCRQ;
            vpFamilyExpand.SurveyPerson = entityCBF.CBFDCY;
            vpFamilyExpand.SurveyChronicle = entityCBF.CBFDCJS;
            vpFamilyExpand.PublicityChroniclePerson = entityCBF.GSJSR;
            vpFamilyExpand.PublicityChronicle = entityCBF.GSJS;
            vpFamilyExpand.CheckDate = entityCBF.GSSHRQ;
            vpFamilyExpand.PublicityDate = entityCBF.GSSHRQ;
            vpFamilyExpand.PublicityCheckPerson = entityCBF.GSSHR;
            vpFamilyExpand.ContractorType = (eContractorType)(int.Parse(entityCBF.CBFLX));
            VirtualPersonCBF.FamilyExpand = vpFamilyExpand;
            return VirtualPersonCBF;
        }

        /// <summary>
        /// 初始化共有人信息
        /// </summary>
        /// <returns></returns>
        private static List<Person> InitalizeSharePersonData(List<CBF_JTCY> entityJTCY, VirtualPerson vp)
        {
            if (entityJTCY == null)
            {
                return null;
            }
            List<Person> fsps = new List<Person>();
            Person fsp;
            foreach (CBF_JTCY entityperson in entityJTCY)
            {
                try
                {
                    fsp = new Person();
                    fsp.FamilyID = vp.ID;
                    fsp.Relationship = CodeMapping(entityperson.YHZGX);
                    fsp.FamilyNumber = entityperson.CBFBM;
                    fsp.Name = entityperson.CYXM;
                    if (entityperson.CYXB == "1")
                    {
                        fsp.Gender = eGender.Male;
                    }
                    else if (entityperson.CYXB == "2")
                    {
                        fsp.Gender = eGender.Female;
                    }
                    else
                    {
                        fsp.Gender = eGender.Unknow;
                    }

                    fsp.CardType = entityperson.CYZJLX.IsNullOrEmpty()
                        ? eCredentialsType.IdentifyCard
                        : (eCredentialsType)int.Parse(entityperson.CYZJLX);
                    fsp.ICN = entityperson.CYZJHM.IsNullOrEmpty() ? "" : entityperson.CYZJHM;
                    //fsp.Comment = SharePersonCommentCodeMapping(entityperson.CYBZ);
                    //if (entityperson.CYBZ == "9")
                    //{
                    //    fsp.Comment = entityperson.CYBZSM;
                    //}修改导入 备注直接成员备注说明  导出的时候以关键字生成 成员备注代码

                    fsp.IsSharedLand = entityperson.SFGYR == "1" ? "是" : "否";
                    fsp.Comment = entityperson.CYBZSM;
                    fsps.Add(fsp);
                }
                catch (Exception e)
                {
                    ErrorInfomation.Add($"CBF_JTCY表中{entityperson.CYXM}的数据存在{e.Message}的问题，请检查");
                    throw;
                }
            }

            return fsps;
        }

        /// <summary>
        /// 初始化权证实体
        /// </summary>
        /// <returns></returns>
        private static List<ContractRegeditBook> InitalizeWarrantBook(List<CBJYQZ> entityCBJYQZs, String zoneCode)
        {
            if (entityCBJYQZs == null || entityCBJYQZs.Count == 0)
            {
                return null;
            }
            List<ContractRegeditBook> returndjbs = new List<ContractRegeditBook>();
            ContractRegeditBook CBJYQZ = null;
            foreach (var entityCBJYQZ in entityCBJYQZs)
            {
                CBJYQZ = new ContractRegeditBook();
                CBJYQZ.RegeditNumber = entityCBJYQZ.CBJYQZBM;
                CBJYQZ.ZoneCode = zoneCode;
                CBJYQZ.SendOrganization = entityCBJYQZ.FZJG;
                CBJYQZ.SendDate = entityCBJYQZ.FZRQ == null ? DateTime.Now : entityCBJYQZ.FZRQ.Value;
                CBJYQZ.PrintDate = DateTime.Now;//没有的话，只能设置为当前日期
                CBJYQZ.RegeditBookGetted = entityCBJYQZ.QZSFLQ;
                CBJYQZ.RegeditBookGettedDate = entityCBJYQZ.QZLQRQ == null ? DateTime.Now : entityCBJYQZ.QZLQRQ.Value;
                CBJYQZ.GetterName = entityCBJYQZ.QZLQRXM;
                CBJYQZ.GetterCardNumber = entityCBJYQZ.QZLQRZJHM;
                CBJYQZ.GetterCardType = entityCBJYQZ.QZLQRZJLX;
                returndjbs.Add(CBJYQZ);
            }

            return returndjbs;
        }

        /// <summary>
        /// 初始化登记簿实体
        /// </summary>
        /// <returns></returns>
        private static List<ContractRegeditBook> InitalizeRegeditBook(List<CBJYQZDJB> entityDJBs, String zoneCode)
        {
            if (entityDJBs == null || entityDJBs.Count == 0)
            {
                return null;
            }
            List<ContractRegeditBook> returndjbs = new List<ContractRegeditBook>();
            ContractRegeditBook DJB = null;
            foreach (var entityDJB in entityDJBs)
            {
                DJB = new ContractRegeditBook();
                DJB.Number = entityDJB.CBJYQZBM;
                DJB.ZoneCode = zoneCode;
                DJB.RegeditNumber = entityDJB.CBJYQZBM;

                DJB.SerialNumber = entityDJB.CBJYQZLSH;//新规范
                DJB.ContractRegeditBookExcursus = entityDJB.DJBFJ;
                DJB.ContractRegeditBookPerson = entityDJB.DBR;
                DJB.ContractRegeditBookTime = entityDJB.DJSJ;
                returndjbs.Add(DJB);
            }
            return returndjbs;
        }

        /// <summary>
        /// 初始化承包地块实体
        /// </summary>
        /// <returns></returns>
        private static List<ContractLand> InitalizeAgricultureLandData(List<CBDKXXEX> entityDKXX, String zoneCode, Dictionary<Guid, string> landguidkjzbList, Dictionary<Guid, string> landguidHTBMList, List<Library.Entity.BelongRelation> BRDKlist)
        {
            if (entityDKXX == null || entityDKXX.Count == 0)
            {
                return null;
            }
            List<ContractLand> dkxxs = new List<ContractLand>();
            BelongRelation brdk = null;
            ContractLand cbd = null;
            foreach (var edkxxitem in entityDKXX)
            {
                //HT.CountAwareArea = edkxxitem.HTMJ;
                cbd = InitalizeSpaceLandData(edkxxitem.KJDK);
                if (cbd == null)
                {
                    cbd = new ContractLand();
                }
                if (edkxxitem.SFQQQG == "1")
                {
                    cbd.IsStockLand = true;
                    brdk = new BelongRelation();
                    brdk.LandID = cbd.ID;
                    brdk.ZoneCode = zoneCode;
                    brdk.QuanficationArea = edkxxitem.HTMJM == null ? 0 : edkxxitem.HTMJM.Value;
                    brdk.TableArea = edkxxitem.YHTMJM == null ? 0 : edkxxitem.YHTMJM.Value;
                    BRDKlist.Add(brdk);
                }
                else
                {
                    cbd.IsStockLand = false;
                }
                cbd.ConstructMode = edkxxitem.CBJYQQDFS;
                cbd.ManagementType = "9";//因为导出的数据中没有此字段，导入回来添加默认。
                cbd.LandNumber = edkxxitem.DKBM;
                cbd.ZoneCode = zoneCode;
                cbd.AwareArea = edkxxitem.HTMJM != null ? edkxxitem.HTMJM.Value : 0;
                cbd.TableArea = edkxxitem.YHTMJM;
                cbd.CadastralNumber = $"{cbd.LandNumber}";
                landguidHTBMList.Add(cbd.ID, edkxxitem.CBHTBM);
                if (edkxxitem.KJDK != null)
                {
                    landguidkjzbList.Add(cbd.ID, (edkxxitem.KJDK as DKEX).KJZB);
                }
                dkxxs.Add(cbd);
            }
            return dkxxs;
        }

        /// <summary>
        /// 初始化空间地块
        /// </summary>
        /// <returns></returns>
        public static ContractLand InitalizeSpaceLandData(IDK IDk)
        {
            if (IDk == null)
            {
                return null;
            }

            DKEXP dk = null;
            if (IDk is DKEXP)
            {
                dk = (DKEXP)IDk;
            }
            else
            {
                dk = (IDk as DKEX).ConvertTo<DKEXP>();
            }
            ContractLand getcbd = new ContractLand();

            getcbd.LandNumber = dk.DKBM;
            getcbd.Name = dk.DKMC;
            getcbd.LandCategory = dk.DKLB;
            getcbd.LandCode = dk.TDLYLX;
            getcbd.LandLevel = dk.DLDJ.IsNullOrEmpty() ? "" : dk.DLDJ;
            getcbd.Purpose = dk.TDYT;

            if (dk.SFJBNT.IsNullOrEmpty() || dk.SFJBNT == "0")
            {
                getcbd.IsFarmerLand = null;
            }
            else if (dk.SFJBNT == "1")
            {
                getcbd.IsFarmerLand = true;
            }
            else if (dk.SFJBNT == "2")
            {
                getcbd.IsFarmerLand = false;
            }

            //getcbd.ActualArea = ToolMath.RoundNumericFormat(dk.SCMJ * 0.0015, 2);
            getcbd.ActualArea = ToolMath.RoundNumericFormat(dk.SCMJ * 0.0015, 2);
            getcbd.NeighborEast = dk.DKDZ;
            getcbd.NeighborSouth = dk.DKNZ;
            getcbd.NeighborWest = dk.DKXZ;
            getcbd.NeighborNorth = dk.DKBZ;
            getcbd.Comment = dk.DKBZXX;
            getcbd.OldLandNumber = dk.QQDKBM;
            AgricultureLandExpand landex = new AgricultureLandExpand();
            landex.ReferPerson = dk.ZJRXM;
            getcbd.LandExpand = landex;

            if (dk.Shape != null)
            {
                getcbd.Shape = dk.Shape as Geometry;
            }
            return getcbd;
        }

        /// <summary>
        /// 初始化承包合同实体
        /// </summary>
        /// <returns></returns>
        private static List<ContractConcord> InitalizeConcordData(List<ICBHT> entityHTs, List<CBJYQZDJB> entityDJBs, String zoneCode)
        {
            if (entityHTs == null || entityHTs.Count == 0 && (entityDJBs == null || entityDJBs.Count == 0))
            {
                return null;
            }
            List<ContractConcord> returnccds = new List<ContractConcord>();
            int count = 0;
            foreach (var entityHT in entityHTs)
            {
                ContractConcord HT = new ContractConcord();
                HT.ConcordNumber = entityHT.CBHTBM;
                HT.ArableLandType = entityHT.CBFS;
                HT.ArableLandStartTime = entityHT.CBQXQ;
                HT.ArableLandEndTime = entityHT.CBQXZ;
                //HT.CountActualArea = ToolMath.RoundNumericFormat(entityHT.HTZMJ * 0.0015, 2);
                HT.CountAwareArea = entityHT.HTZMJM != null ? entityHT.HTZMJM.Value : 0;
                HT.TotalTableArea = entityHT.YHTZMJM;
                HT.ContractDate = entityHT.QDSJ;
                HT.ZoneCode = zoneCode;
                HT.LandPurpose = "1";

                if (entityDJBs != null && entityDJBs.Count > count)
                {
                    var entityDJB = entityDJBs[count];
                    HT.ConcordNumber = entityDJB.CBJYQZBM;
                    HT.ArableLandType = entityDJB.CBFS;
                    HT.ArableLandStartTime = entityDJB.CBQXQ;
                    HT.ArableLandEndTime = entityDJB.CBQXZ;
                    HT.ManagementTime = entityDJB.CBQX;
                    HT.LandPurpose = "1";
                }
                if (HT.ManagementTime.IsNullOrEmpty())
                {
                    if (entityHT.CBQXQ != null && entityHT.CBQXZ != null)
                    {
                        HT.ManagementTime = (entityHT.CBQXZ.Value.Year - entityHT.CBQXQ.Value.Year).ToString();
                    }
                }

                count++;
                returnccds.Add(HT);
            }

            return returnccds;
        }

        /// <summary>
        /// 初始化发包方数据
        /// </summary>
        public static CollectivityTissue InitalizeSenderData(FBF entityFBF, String zoneCode)
        {
            if (entityFBF == null)
            {
                return null;
            }
            CollectivityTissue FBF = new CollectivityTissue();
            FBF.Code = entityFBF.FBFBM;
            FBF.Name = entityFBF.FBFMC;
            FBF.ZoneCode = zoneCode;
            FBF.LawyerName = entityFBF.FBFFZRXM;
            if (entityFBF.FZRZJLX != null)
                FBF.LawyerCredentType = (eCredentialsType)(int.Parse(entityFBF.FZRZJLX));
            FBF.LawyerCartNumber = entityFBF.FZRZJHM.IsNullOrEmpty() ? "" : entityFBF.FZRZJHM;
            FBF.LawyerTelephone = entityFBF.LXDH;
            FBF.LawyerAddress = entityFBF.FBFDZ;
            FBF.LawyerPosterNumber = entityFBF.YZBM;
            FBF.SurveyPerson = entityFBF.FBFDCY;
            FBF.SurveyDate = entityFBF.FBFDCRQ;
            FBF.SurveyChronicle = entityFBF.FBFDCJS;
            return FBF;
        }

        #endregion

        #region Methods-helper

        /// <summary>
        /// 截取户号
        /// </summary>
        /// <param name="num">输入户号</param>
        /// <returns></returns>
        private static string SubVirtualPersonFamilyNum(string num)
        {
            if (num == null) return null;
            string opnum = num;
            if (num.Length > 14)
            {
                opnum = num.Substring(14);
            }
            opnum = opnum.TrimStart('0');
            return opnum;
        }

        /// <summary>
        /// 翻转名称
        /// </summary>
        private static string CodeMapping(string nameCode)
        {
            string name = "其他";
            switch (nameCode)
            {
                case "01":
                    name = "本人";
                    break;
                case "34":
                    name = "三女";
                    break;
                case "02":
                    name = "户主";
                    break;
                case "35":
                    name = "四女";
                    break;
                case "36":
                    name = "五女";
                    break;
                case "10":
                    name = "配偶";
                    break;
                case "37":
                    name = "养女或继女";
                    break;
                case "11":
                    name = "夫";
                    break;
                case "38":
                    name = "儿媳";
                    break;
                case "12":
                    name = "妻";
                    break;
                case "39":
                    name = "其他女儿";
                    break;
                case "20":
                    name = "子";
                    break;
                case "40":
                    name = "孙子、孙女或外孙子、外孙女";
                    break;
                case "21":
                    name = "独生子";
                    break;
                case "41":
                    name = "孙子";
                    break;
                case "22":
                    name = "长子";
                    break;
                case "42":
                    name = "孙女";
                    break;
                case "23":
                    name = "次子";
                    break;
                case "43":
                    name = "外孙子";
                    break;
                case "24":
                    name = "三子";
                    break;
                case "44":
                    name = "外孙女";
                    break;
                case "25":
                    name = "四子";
                    break;
                case "45":
                    name = "孙媳妇或外孙媳妇";
                    break;
                case "26":
                    name = "五子";
                    break;
                case "46":
                    name = "孙女婿或外孙女婿";
                    break;
                case "27":
                    name = "养子或继子";
                    break;
                case "47":
                    name = "曾孙子或外曾孙子";
                    break;
                case "28":
                    name = "女婿";
                    break;
                case "48":
                    name = "曾孙女或外曾孙女";
                    break;
                case "29":
                    name = "其他儿子";
                    break;
                case "49":
                    name = "其他孙子、孙女或外孙子、外孙女";
                    break;
                case "30":
                    name = "女";
                    break;
                case "50":
                    name = "父母";
                    break;
                case "31":
                    name = "独生女";
                    break;
                case "51":
                    name = "父亲";
                    break;
                case "32":
                    name = "长女";
                    break;
                case "52":
                    name = "母亲";
                    break;
                case "33":
                    name = "次女";
                    break;
                case "53":
                    name = "公公";
                    break;
                case "54":
                    name = "婆婆";
                    break;
                case "76":
                    name = "姐夫";
                    break;
                case "55":
                    name = "岳父";
                    break;
                case "77":
                    name = "妹妹";
                    break;
                case "56":
                    name = "岳母";
                    break;
                case "78":
                    name = "妹夫";
                    break;
                case "57":
                    name = "继父或养父";
                    break;
                case "79":
                    name = "其他兄弟姐妹";
                    break;
                case "58":
                    name = "继母或养母";
                    break;
                case "59":
                    name = "其他父母关系";
                    break;
                case "80":
                    name = "其他";
                    break;
                case "81":
                    name = "伯父";
                    break;
                case "60":
                    name = "祖父母或外祖父母";
                    break;
                case "82":
                    name = "伯母";
                    break;
                case "61":
                    name = "祖父";
                    break;
                case "83":
                    name = "叔父";
                    break;
                case "62":
                    name = "祖母";
                    break;
                case "84":
                    name = "婶母";
                    break;
                case "63":
                    name = "外祖父";
                    break;
                case "85":
                    name = "舅父";
                    break;
                case "64":
                    name = "外祖母";
                    break;
                case "86":
                    name = "舅母";
                    break;
                case "65":
                    name = "配偶的祖父母或外祖父母";
                    break;
                case "87":
                    name = "姨父";
                    break;
                case "66":
                    name = "曾祖父";
                    break;
                case "88":
                    name = "姨母";
                    break;
                case "67":
                    name = "曾祖母";
                    break;
                case "89":
                    name = "姑父";
                    break;
                case "68":
                    name = "配偶的曾祖父母或外曾祖父母";
                    break;
                case "90":
                    name = "姑母";
                    break;
                case "69":
                    name = "其他祖父母或外祖父母关系";
                    break;
                case "91":
                    name = "堂兄弟、堂姐妹";
                    break;
                case "92":
                    name = "表兄弟、表姐妹";
                    break;
                case "70":
                    name = "兄弟姐妹";
                    break;
                case "93":
                    name = "侄子";
                    break;
                case "71":
                    name = "兄";
                    break;
                case "94":
                    name = "侄女";
                    break;
                case "72":
                    name = "嫂";
                    break;
                case "95":
                    name = "外甥";
                    break;
                case "73":
                    name = "弟";
                    break;
                case "96":
                    name = "外甥女";
                    break;
                case "74":
                    name = "弟媳";
                    break;
                case "97":
                    name = "其他亲属";
                    break;
                case "75":
                    name = "姐姐";
                    break;
                case "99":
                    name = "非亲属";
                    break;
            }
            return name;
        }

        /// <summary>
        /// 性别映射
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string SharePersonCommentCodeMapping(string code)
        {
            switch (code)
            {
                case "1":
                    return "外嫁女";
                case "2":
                    return "入赘男";
                case "3":
                    return "在校大学生";
                case "4":
                    return "国家公职人员";
                case "5":
                    return "军人(军官、士兵)";
                case "6":
                    return "新生儿";
                case "7":
                    return "去世";
                case "9":
                    return "其他备注";
                default:
                    return code;
            }
        }

        #endregion

        #endregion

    }
}
