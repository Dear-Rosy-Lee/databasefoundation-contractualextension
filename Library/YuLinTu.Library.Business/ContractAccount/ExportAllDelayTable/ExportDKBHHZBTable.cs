using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Scripting.Utils;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    public class ExportDKBHHZBTable : ExportExcelBase
    {
        #region Fields
        protected double index;
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
        private List<ContractLand> contractLands;
        #endregion Fields

        #region Properties

        //public string TemplatePath { get; set; }

        public string ZoneDesc { get; set; }

        public string TownNme { get; set; }

        public List<SurveyForm> SurveyForms { get; set; }

        public List<ContractLand_Del> ContractLand_Dels { get; set; } 

        public string saveFilePath { get; set; }

        public Zone Zone { get; set; }

        public CollectivityTissue Tissue { get; set; }

        #endregion Properties

        #region Ctor

        public ExportDKBHHZBTable()
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
            double totalqqld = 0;
            double totalcddth = 0;
            double totalhhzj = 0;
            double totalxzcb = 0;
            double totalgjzy = 0;
            double totaljtgysyzy = 0;
            double totalwddwh = 0;
            double totalhhjs = 0;
            double totalqt = 0;
            int totalSecondLandCount = 0;
            double totalSecondLandArea = 0;
            int totalLandCount = 0;
            double totalLandArea = 0;
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
                totalqqld += qqld;
                totalcddth += cddth;
                totalhhzj += hhzj;
                totalxzcb += xzcb;
                totalgjzy += gjzy;
                totaljtgysyzy += jtgysyzy;
                totalwddwh += wddwh;
                totalhhjs += hhjs;
                totalqt += qt;
                totalSecondLandCount += secondLandCount;
                totalSecondLandArea += secondLandArea;
                totalLandCount += landCount;
                totalLandArea += landArea;
                aindex++;
            }
            InitalizeRangeValue("A" + index, "A" + index, "合计");
            InitalizeRangeValue("B" + index, "B" + index, "/");
            InitalizeRangeValue("C" + index, "C" + index, "/");
            InitalizeRangeValue("D" + index, "D" + index, totalSecondLandCount);
            InitalizeRangeValue("E" + index, "E" + index, totalSecondLandArea);
            InitalizeRangeValue("F" + index, "F" + index, 0);
            InitalizeRangeValue("G" + index, "G" + index, totalqqld + totalcddth + totalhhzj + totalxzcb);
            InitalizeRangeValue("H" + index, "H" + index, totalqqld);
            InitalizeRangeValue("I" + index, "I" + index, totalcddth);
            InitalizeRangeValue("J" + index, "J" + index, totalhhzj);
            InitalizeRangeValue("K" + index, "K" + index, totalxzcb);
            InitalizeRangeValue("L" + index, "L" + index, totalgjzy + totaljtgysyzy + totalwddwh + totalhhjs + totalqt);
            InitalizeRangeValue("M" + index, "M" + index, totalgjzy);
            InitalizeRangeValue("N" + index, "N" + index, totaljtgysyzy);
            InitalizeRangeValue("O" + index, "O" + index, totalwddwh);
            InitalizeRangeValue("P" + index, "P" + index, totalhhjs);
            InitalizeRangeValue("Q" + index, "Q" + index, totalqt);
            InitalizeRangeValue("R" + index, "R" + index, totalLandCount);
            InitalizeRangeValue("S" + index, "S" + index, totalLandArea);
            InitalizeRangeValue("T" + index, "T" + index, "/");
            SetLineType("A4", "T" + index, false);
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
            
            
            foreach (var land in contractLands)
            {
                CountLandInformation(land);
            }

            if (surveyForm.ExchangeLandArea > 0)
            {
                hhzj = surveyForm.ExchangeLandArea;
            }
            else
            {
                hhjs = surveyForm.ExchangeLandArea;
            }
            InitalizeRangeValue("A" + index, "A" + index, aindex);
            InitalizeRangeValue("B" + index, "B" + index, surveyForm.FamilyNumber);
            InitalizeRangeValue("C" + index, "C" + index, surveyForm.OwnerName);
            InitalizeRangeValue("D" + index, "D" + index, GetSecondLandCout());
            InitalizeRangeValue("E" + index, "E" + index, GetSecondLandArea());
            InitalizeRangeValue("F" + index, "F" + index, 0);
            InitalizeRangeValue("G" + index, "G" + index, qqld + cddth + hhzj + xzcb);
            InitalizeRangeValue("H" + index, "H" + index, qqld);
            InitalizeRangeValue("I" + index, "I" + index, cddth);
            InitalizeRangeValue("J" + index, "J" + index, hhzj);
            InitalizeRangeValue("K" + index, "K" + index, xzcb);
            InitalizeRangeValue("L" + index, "L" + index, gjzy + jtgysyzy + wddwh + hhjs + qt);
            InitalizeRangeValue("M" + index, "M" + index, gjzy);
            InitalizeRangeValue("N" + index, "N" + index, jtgysyzy);
            InitalizeRangeValue("O" + index, "O" + index, cdkeyValuePairs[surveyForm.FamilyNumber]);
            InitalizeRangeValue("P" + index, "P" + index, hhjs);
            InitalizeRangeValue("Q" + index, "Q" + index, qt);
            InitalizeRangeValue("R" + index, "R" + index, landCount);
            InitalizeRangeValue("S" + index, "S" + index, landArea);
            InitalizeRangeValue("T" + index, "T" + index, "/");
            index++;
        }



        public virtual void CountLandInformation(ContractLand land)
        {
            if (land.Comment == "漏登")
            {
                qqld += land.AwareArea;
            }
            if (land.Comment == "错登")
            {
                cddth += land.AwareArea;
            }
            if (land.Comment == "新增")
            {
                xzcb += land.AwareArea;
            }
            if (land.LandChange == "国家征用")
            {
                gjzy += land.AwareArea;
            }
            if (land.LandChange == "集体公益事业占用")
            {
                jtgysyzy += land.AwareArea;
            }
            if (land.Comment == "其他")
            {
                qt += double.Parse(land.LandChange);
            }
            
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        public virtual void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "T1").ToString();
            title = $"{ZoneDesc}{title}";
            InitalizeRangeValue("A" + 1, "T" + 1, title);

            string unit = GetRangeToValue("A2", "E2").ToString();
            unit = $"{unit}{TownNme}";
            InitalizeRangeValue("A" + 2, "E" + 2, unit);

            string senderName = GetRangeToValue("F2", "I2").ToString();
            senderName = $"{senderName}{Tissue.LawyerName}";
            InitalizeRangeValue("F" + 2, "I" + 2, senderName);
            
            string date = GetRangeToValue("O2", "T2").ToString();
            date = $"{date}{DateTime.Now:yyyy年MM月dd日}";
            InitalizeRangeValue("O" + 2, "T" + 2, date);
        }
        private void GetExchangeLandArea(List<SurveyForm> surveyForms,List<ContractLand> contractLands)
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
            index = 7;
            return true;
        }
        #endregion Method
    }
}
