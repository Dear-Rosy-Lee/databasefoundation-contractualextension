using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business.ContractAccount.ExportAllDelayTable
{
    public class ExoprtRKBHHZBTable : ExportExcelBase
    {
        #region Fields
        protected int index;
        private int xsr;
        private int qr;
        private int ld;
        private int wddth;
        private int sw;
        private int wddwh;
        private int zyfq;
        private int secondperson;
        private int person;
        private Dictionary<Guid?,int> keyValuePairs;
        #endregion Fields

        #region Properties

        //public string TemplatePath { get; set; }

        public string ZoneDesc { get; set; }

        public string TownNme { get; set; }

        public List<VirtualPerson> VirtualPeoples { get; set; }

        public List<VirtualPerson_Del> VirtualPerson_Dels { get; set; }

        public CollectivityTissue CollectivityTissue { get; set; }

        public string saveFilePath { get; set; }

        public Zone Zone { get; set; }

        #endregion Properties

        #region Ctor

        public ExoprtRKBHHZBTable()
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
            int totalxsr = 0;
            int totalqr = 0;
            int totalld = 0;
            int totalwddth = 0;
            int totalsw = 0;
            int totalwddwh = 0;
            int totalzyfq = 0;
            int totalsecondperson = 0;
            int totalperson = 0;
            int aindex = 1;
            keyValuePairs = new Dictionary<Guid?, int>();

            foreach (var item in VirtualPeoples)
            {
                keyValuePairs[item.ID] = 0; 
            }

            WriteTempLate();

            List<VirtualPerson> vps = new List<VirtualPerson>(VirtualPeoples);


            foreach (var delPerson in VirtualPerson_Dels)
            {

                var existingPerson = vps.FirstOrDefault(p => p.ID == delPerson.ID);

                if (existingPerson != null)
                {

                    var res = existingPerson.SharePersonList.ToList();
                    res.AddRange(delPerson.SharePersonList);
                    existingPerson.SharePerson = string.Empty;
                    existingPerson.SharePersonList = res;
                }
                else
                {

                    vps.Add(VirtualPerson.ChangeDataEntity(Zone.FullCode, delPerson));
                }
            }
            foreach (var vp in vps)
            {
                WriteInformation(vp,aindex);
                totalxsr += xsr;
                totalqr += qr;
                totalld += ld;
                totalwddth += wddth;
                totalsw += sw;
                totalwddwh += wddwh;
                totalzyfq += zyfq;
                totalsecondperson += secondperson;
                totalperson += person;
                aindex++;
            }
            InitalizeRangeValue("A" + index, "A" + index, "合计");
            InitalizeRangeValue("B" + index, "B" + index, "/");
            InitalizeRangeValue("C" + index, "C" + index, "/");
            InitalizeRangeValue("D" + index, "D" + index, totalsecondperson);
            InitalizeRangeValue("E" + index, "E" + index, totalxsr + totalqr + totalld + totalwddth);
            InitalizeRangeValue("F" + index, "F" + index, totalxsr);
            InitalizeRangeValue("G" + index, "G" + index, totalqr);
            InitalizeRangeValue("H" + index, "H" + index, totalld);
            InitalizeRangeValue("I" + index, "I" + index, totalwddth);
            InitalizeRangeValue("J" + index, "J" + index, totalsw + totalwddwh + totalzyfq);
            InitalizeRangeValue("K" + index, "K" + index, totalsw);
            InitalizeRangeValue("L" + index, "L" + index, totalwddwh);
            InitalizeRangeValue("M" + index, "M" + index, totalzyfq);
            InitalizeRangeValue("N" + index, "N" + index, totalperson);
            InitalizeRangeValue("O" + index, "O" + index, "/");
            SetLineType("A4", "O" + index, false);
            return true;
        }
        public virtual void WriteInformation(VirtualPerson vp,int aindex)
        {
            xsr = 0;
            qr = 0;
            ld = 0;
            wddth = 0;
            sw = 0;
            wddwh = keyValuePairs.Where(t=>t.Key == vp.ID).FirstOrDefault().Value;
            zyfq = 0;
            secondperson = vp.SharePersonList.Count;
            person = int.Parse(vp.PersonCount);
            foreach (var person in vp.SharePersonList)
            {
                CountPersonInformation(person);
               
            }
            InitalizeRangeValue("A" + index, "A" + index, aindex);
            InitalizeRangeValue("B" + index, "B" + index, vp.FamilyNumber);
            InitalizeRangeValue("C" + index, "C" + index, vp.Name);
            InitalizeRangeValue("D" + index, "D" + index, vp.SharePersonList.Count);
            InitalizeRangeValue("E" + index, "E" + index, xsr+qr+ld+wddth);
            InitalizeRangeValue("F" + index, "F" + index, xsr);
            InitalizeRangeValue("G" + index, "G" + index, qr);
            InitalizeRangeValue("H" + index, "H" + index, ld);
            InitalizeRangeValue("I" + index, "I" + index, wddth);
            InitalizeRangeValue("J" + index, "J" + index, sw + wddwh + zyfq);
            InitalizeRangeValue("K" + index, "K" + index, sw);
            InitalizeRangeValue("L" + index, "L" + index, wddwh);
            InitalizeRangeValue("M" + index, "M" + index, zyfq);
            InitalizeRangeValue("N" + index, "N" + index, int.Parse(vp.PersonCount));
            InitalizeRangeValue("O" + index, "O" + index, vp.FamilyExpand.Description);
            index++;
        }

        public virtual void CountPersonInformation(Person person)
        {
            if(person.Comment == "新生儿")
            {
                xsr++;
            }
            if(person.Opinion == "迁入")
            {
                qr++;
            }
            if (person.Opinion == "误登到他户")
            {
                wddth++;
                keyValuePairs[person.FamilyID] ++;
            }
            if (person.Opinion == "漏登" && person.Comment == "其他备注")
            {
                ld++;
            }
            if(person.Comment == "去世")
            {
                sw++;
            }
            if(person.Comment == "其他备注" && person.Opinion == "删除")
            {
                zyfq++;
            }
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        public virtual void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "O1").ToString();
            title = $"{ZoneDesc}{title}";
            InitalizeRangeValue("A" + 1, "O" + 1, title);
            string unit = GetRangeToValue("A2", "C2").ToString();
            unit = $"{unit}{TownNme}";
            InitalizeRangeValue("A" + 2, "C" + 2, unit);
            string sendedName = GetRangeToValue("D2", "F2").ToString();
            InitalizeRangeValue("D" + 2, "F" + 2, unit);
            sendedName = $"{sendedName}{CollectivityTissue.LawyerName}";
            string Date = GetRangeToValue("K2", "O2").ToString();
            Date = $"{Date}{DateTime.Now:yyyy年MM月dd日}";
            InitalizeRangeValue("K" + 2, "O" + 2, Date);
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
