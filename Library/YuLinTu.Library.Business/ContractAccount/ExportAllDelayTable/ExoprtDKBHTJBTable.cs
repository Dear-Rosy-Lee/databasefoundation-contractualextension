using Microsoft.Scripting.Utils;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    public class ExoprtDKBHTJBTable : ExportExcelBase
    {
        #region Fields
        protected int index;
        protected int bindex;

        private double qqld;
        private double cddth;
        private double hhzj;
        private double xzcb;
        private double gjzy;
        private double jtgysyzy;
        private double wddwh;
        private double hhjs;
        private double qt;
        private int secondLandCount;
        private double secondLandArea;
        private int landCount;
        private double landArea;
        private Dictionary<string, double> cdkeyValuePairs;
        private Dictionary<Guid?, double> hhkeyValuePairs;
        private List<string> oldLandNumbers;
        private List<string> newLandNumbers;
        private List<ContractLand> contractLands;
        #endregion Fields

        #region Properties

        public string TemplatePath { get; set; }

        public string ZoneDesc { get; set; }

        public string TownNme { get; set; }

        public List<SurveyForm> SurveyForms { get; set; }

        public List<ContractLand_Del> ContractLand_Dels { get; set; }

        public string saveFilePath { get; set; }

        public Zone Zone { get; set; }

        public CollectivityTissue Tissue { get; set; }

        #endregion Properties

        #region Ctor

        public ExoprtDKBHTJBTable()
        {

        }

        #endregion Ctor

        #region Method

        public virtual bool BeginToZone(string templatePath)
        {
            //RePostProgress(1);
            if (!File.Exists(templatePath))
            {
                PostErrorInfo("模板路径不存在！");
                return false;
            }
            TemplatePath = templatePath;
            Write();//写数据
            return true;
        }
        public override void Write()
        {
            try
            {
                //RePostProgress(5);
                OpenExcelFile();
                //RePostProgress(15);
                if (!SetValue())
                    return;
                BeginWrite();
                //RePostProgress(100);
            }
            catch (System.Exception e)
            {
                PostErrorInfo(e.Message.ToString());
                Dispose();
            }
        }
        public virtual bool BeginWrite()
        {
            int aindex = 1;
            cdkeyValuePairs = new Dictionary<string, double>();
            hhkeyValuePairs = new Dictionary<Guid?, double>();
            contractLands = new List<ContractLand>();
            foreach (var item in SurveyForms)
            {
                cdkeyValuePairs[item.FamilyNumber] = 0;
                hhkeyValuePairs[item.ID] = 0;
                contractLands.AddRange(item.ContractLandList);
            }



            WriteTempLate();


            foreach (var surveyForm in SurveyForms)
            {
                GetExchangeLandArea(SurveyForms, contractLands);
                WriteInformation(surveyForm, aindex);
                
                aindex++;
            }
            
            SetLineType("A6", "I" + (index-1), false);
            return true;
        }
        public virtual void WriteInformation(SurveyForm surveyForm, int aindex)
        {
            qqld = 0;
            cddth = 0;
            hhzj = 0;
            xzcb = 0;
            gjzy = 0;
            jtgysyzy = 0;
            hhjs = 0;
            qt = 0;
            landCount = surveyForm.ContractLandList.Count;
            landArea = 0;
            surveyForm.ContractLandList.ForEach(t => { landArea += t.AwareArea; });
            var contractLands = surveyForm.ContractLandList;
            if (ContractLand_Dels != null)
            {
                foreach (var item in ContractLand_Dels.Where(t => t.CBFID == surveyForm.OwnerId).ToList())
                {
                    contractLands.Add(ContractLand.ChangeDataDelEntity(Zone.FullCode, item));
                }
            }

            bindex = 0;

            

            foreach (var land in contractLands)
            {
                WriteLandInformation(land);
                
            }
            //if (surveyForm.ExchangeLandArea > 0)
            //{
            //    InitalizeRangeValue("C" + index, "C" + index, land.Name);
            //    InitalizeRangeValue("D" + index, "D" + index, landArea);
            //    InitalizeRangeValue("E" + index, "E" + index, "互换新增");
            //    bindex++;
            //}
            //if (surveyForm.ExchangeLandArea < 0)
            //{
            //    hhjs = surveyForm.ExchangeLandArea;
            //    bindex++;
            //}

            if (bindex != 0)
            {
                InitalizeRangeValue("A" + (index - bindex), "A" + (index - 1), aindex);
                InitalizeRangeValue("B" + (index - bindex), "B" + (index - 1), surveyForm.OwnerName);
                InitalizeRangeValue("I" + (index - bindex), "I" + (index - 1), surveyForm.OldZoneName);
            }
            
        }



        public virtual void WriteLandInformation(ContractLand land)
        {
            double landArea = 0;
            if(land.Comment == "漏登" || land.Comment == "错登"|| land.Comment == "新增")
            { 
                landArea = land.AwareArea;
                InitalizeRangeValue("C" + index, "C" + index, land.Name);
                InitalizeRangeValue("D" + index, "D" + index, landArea);
                InitalizeRangeValue("E" + index, "E" + index, land.Comment);
                index++;
                bindex++;
            }
            
            if (land.LandChange == "国家征用" || land.LandChange == "集体公益事业占用")
            {
                landArea = land.AwareArea;
                InitalizeRangeValue("F" + index, "F" + index, land.Name);
                InitalizeRangeValue("G" + index, "G" + index, landArea);
                InitalizeRangeValue("H" + index, "H" + index, land.LandChange);
                bindex++;
                index++;
            }
            if (land.Comment == "其他")
            {
                landArea = double.Parse(land.LandChange);
                InitalizeRangeValue("F" + index, "F" + index, land.Name);
                InitalizeRangeValue("G" + index, "G" + index, landArea);
                InitalizeRangeValue("H" + index, "H" + index, land.Comment+"减少");
                bindex++;
                index++;
            }
            
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        public virtual void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "I1").ToString();
            title = $"{ZoneDesc}{title}";
            InitalizeRangeValue("A" + 1, "I" + 1, title);

            string unit = GetRangeToValue("A2", "D2").ToString();
            unit = $"{unit}{TownNme}";
            InitalizeRangeValue("A" + 2, "D" + 2, unit);

            string senderName = GetRangeToValue("E2", "E2").ToString();
            senderName = $"{senderName}{Tissue.LawyerName}";
            InitalizeRangeValue("E" + 2, "E" + 2, senderName);

            string date = GetRangeToValue("H2", "I2").ToString();
            date = $"{date}{DateTime.Now:yyyy年MM月dd日}";
            InitalizeRangeValue("H" + 2, "I" + 2, date);
        }
        private void GetExchangeLandArea(List<SurveyForm> surveyForms, List<ContractLand> contractLands)
        {

            foreach (var survey in surveyForms)
            {
                var oldLandNumbers = new List<string>();
                var newLandNumbers = new List<string>();
                survey.ContractLandList.ForEach(land =>
                {
                    if (land.Comment == "互换")
                    {
                        var lands = land.LandChange.Split('换');
                        oldLandNumbers.Add(lands[0]);
                        newLandNumbers.Add(lands[1]);
                    }
                    if (land.Comment == "错登")
                    {
                        cdkeyValuePairs[land.LandChange.Substring(14)] += land.AwareArea;
                    }

                });
                GetExchangeLand(survey, surveyForms, oldLandNumbers, newLandNumbers, contractLands);
            }
        }

        private void GetExchangeLand(SurveyForm surveyForm, List<SurveyForm> surveyForms, List<string> oldLandNumbers, List<string> newLandNumbers, List<ContractLand> contractLands)
        {
            var oldFamily = new SurveyForm();
            Dictionary<string, string> result = new Dictionary<string, string>();

            var groupedPairs = oldLandNumbers
                .Select((oldNum, index) => new { Old = oldNum, New = newLandNumbers[index] })
                .GroupBy(x => x.Old);

            foreach (var group in groupedPairs)
            {

                string combinedNewNumbers = string.Join(",", group.Select(x => x.New));
                result.Add(group.Key, combinedNewNumbers);
            }
            double oldLandArea = 0;
            double newLandArea = 0;
            foreach (var item in result)
            {


                var oldLand = contractLands.FirstOrDefault(x => x.LandNumber.Contains(item.Key));
                oldFamily = surveyForms.FirstOrDefault(x => x.OwnerId.Equals(oldLand.OwnerId));
                oldLandArea += oldLand.AwareArea;



                var newres = item.Value.Split(',');
                newres.ForEach(t => { var newLand = contractLands.FirstOrDefault(x => x.LandNumber.Contains(t)); newLandArea += newLand.AwareArea; });
            }

            surveyForm.ExchangeLandArea += Math.Round(newLandArea - oldLandArea, 2);
            oldFamily.ExchangeLandArea += Math.Round(oldLandArea - newLandArea, 2);

        }

        private int GetSecondLandCout()
        {
            return secondLandCount;
        }

        private double GetSecondLandArea()
        {
            return secondLandArea;
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(TemplatePath);
        }
        /// <summary>
        /// 初始值
        /// </summary>
        private bool SetValue()
        {
            //RePostProgress(5);
            index = 6;
            return true;
        }
        #endregion Method
    }
}
