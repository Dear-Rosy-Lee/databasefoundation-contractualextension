using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Business.ContractLand.Exchange2;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    public class ExportCBMJQRBTable : ExportExcelBase
    {
        #region Fields

        protected int index;
        protected int cindex;
        public List<Dictionary> dictXB;
        #endregion Fields

        #region Properties

        public string TemplatePath { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        public string TownNme { get; set; }

        public List<SurveyForm> SurveyForms { get; set; }

        public string saveFilePath { get; set; }

        #endregion Properties

        #region Ctor
        public ExportCBMJQRBTable()
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
            cindex = 1;
            WriteTempLate();
            foreach (var surveyForm in SurveyForms)
            {
                WriteInformation(surveyForm);
                cindex++;
            }
            return true;
        }
        public virtual void WriteInformation(SurveyForm surveyForm)
        {
            int aindex = index;
            foreach(var person in surveyForm.SharePersonList)
            {
                WritePersonInformation(person,aindex);
                aindex++;
            }
            int height = surveyForm.SharePersonList.Count;
            var res = surveyForm.ContractLand;
            double TotalLandAware = 0;
            double TotalLandActual = 0;
            surveyForm.ContractLandList.ForEach(t => { TotalLandAware += t.AwareArea; TotalLandActual += t.ActualArea; });
            InitalizeRangeValue("A" + index, "A" + (index + height - 1), cindex.ToString());
            InitalizeRangeValue("B" + index, "B" + (index + height - 1), surveyForm.OwnerName);
            InitalizeRangeValue("C" + index, "C" + (index + height - 1), surveyForm.PersonCount);
            InitalizeRangeValue("J" + index, "J" + (index + height - 1), surveyForm.ContractLandList.Count.ToString());
            InitalizeRangeValue("K" + index, "K" + (index + height - 1), "");
            InitalizeRangeValue("L" + index, "L" + (index + height - 1), TotalLandAware.ToString());
            InitalizeRangeValue("M" + index, "M" + (index + height - 1), TotalLandActual.ToString());
            index += height;
        }

        public virtual void WritePersonInformation(Person person, int index)
        {
            Dictionary gender = dictXB.Find(c => c.Code.Equals(person.Gender == eGender.Male ? "1" : "2"));
            InitalizeRangeValue("D" + index, "D" + index, person.Name);
            InitalizeRangeValue("E" + index, "E" + index, gender.Name + "性");
            InitalizeRangeValue("F" + index, "F" + index, person.Age);
            InitalizeRangeValue("G" + index, "G" + index, person.ICN);
            InitalizeRangeValue("H" + index, "H" + index, person.Relationship);
            InitalizeRangeValue("I" + index, "I" + index, person.Comment);
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        public virtual void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "M1").ToString();
            title = $"{ZoneDesc}{title}";
            InitalizeRangeValue("A" + 1, "M" + 1, title);
            string unit = GetRangeToValue("A2", "D2").ToString();
            unit = $"{unit}{TownNme}";
            InitalizeRangeValue("A" + 2, "D" + 2, title);
            string Date = GetRangeToValue("E2", "M2").ToString();
            Date = $"{unit}{DateTime.Now:yyyy.MM.dd}";
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
