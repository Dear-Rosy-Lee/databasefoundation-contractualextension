using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    public class ExoprtCBGYRXZJJSTJBTable: ExportExcelBase
    {
        #region Fields

        protected int index;

        #endregion Fields

        #region Properties

        public string TemplatePath { get; set; }

        public string ZoneDesc { get; set; }

        public string TownNme { get; set; }

        public List<VirtualPerson> VirtualPeoples { get; set; }

        public List<VirtualPerson_Del> VirtualPerson_Dels { get; set; }

        public CollectivityTissue CollectivityTissue { get; set; }

        public string saveFilePath { get; set; }

        public Zone Zone { get; set; }

        #endregion Properties

        #region Ctor

        public ExoprtCBGYRXZJJSTJBTable()
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
            
            WriteTempLate();
            
            foreach (var item in VirtualPeoples)
            {
                var res = item.SharePersonList.Where(t => t.Opinion!="").ToList();
                item.SharePerson = string.Empty;
                item.SharePersonList = res;
            }
            List<VirtualPerson> vps = new List<VirtualPerson>(VirtualPeoples.Where(t=>t.SharePersonList.Count != 0));

            
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
                    
                    vps.Add(VirtualPerson.ChangeDataEntity(Zone.FullCode,delPerson));
                }
            }
            foreach (var vp in vps)
            {
                WriteInformation(vp);
            }
            SetLineType("A4", "J" + (index - 1), false);
            return true;
        }
        public virtual void WriteInformation(VirtualPerson vp)
        {
            int aindex = index;
            int height = vp.SharePersonList.Count();
            foreach (var person in vp.SharePersonList)
            {
                WritePersonInformation(person, aindex);
                aindex++;
            }
            InitalizeRangeValue("A" + index, "A" + (index + height - 1), vp.FamilyNumber);
            InitalizeRangeValue("B" + index, "B" + (index + height - 1), vp.Name);
            InitalizeRangeValue("J" + index, "J" + (index + height - 1), vp.FamilyExpand.Description);
       
            index += height;
        }

        public virtual void WritePersonInformation(Person person, int index)
        {
            if(person.Opinion == "迁入" || person.Opinion == "误登到他户" || person.Opinion == "漏登")
            {
                if (person.Opinion == "漏登" && person.Comment == "新生儿")
                {
                    InitalizeRangeValue("C" + index, "C" + index, person.Name);
                    InitalizeRangeValue("D" + index, "D" + index, person.ICN);
                    InitalizeRangeValue("E" + index, "E" + index, person.Relationship);
                    InitalizeRangeValue("F" + index, "F" + index, person.Comment);
                }
                else
                {
                    InitalizeRangeValue("C" + index, "C" + index, person.Name);
                    InitalizeRangeValue("D" + index, "D" + index, person.ICN);
                    InitalizeRangeValue("E" + index, "E" + index, person.Relationship);
                    InitalizeRangeValue("F" + index, "F" + index, person.Opinion);
                }
            }
            if(person.Opinion == "删除")
            {
                if (person.Opinion == "删除" && person.Comment == "去世")
                {
                    InitalizeRangeValue("G" + index, "G" + index, person.Name);
                    InitalizeRangeValue("H" + index, "H" + index, person.Relationship);
                    InitalizeRangeValue("I" + index, "I" + index, person.Comment);
                }
                else
                {
                    InitalizeRangeValue("G" + index, "G" + index, person.Name);
                    InitalizeRangeValue("H" + index, "H" + index, person.Relationship);
                    InitalizeRangeValue("I" + index, "I" + index, "自愿放弃");
                }
            }
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        public virtual void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "J1").ToString();
            title = $"{ZoneDesc}{title}";
            InitalizeRangeValue("A" + 1, "J" + 1, title);
            string unit = GetRangeToValue("A2", "C2").ToString();
            unit = $"{unit}{TownNme}";
            InitalizeRangeValue("A" + 2, "C" + 2, unit);
            string sendedName = GetRangeToValue("D2", "D2").ToString();
            InitalizeRangeValue("D" + 2, "D" + 2, unit);
            sendedName = $"{sendedName}{CollectivityTissue.LawyerName}";
            string Date = GetRangeToValue("H2", "J2").ToString();
            Date = $"{Date}{DateTime.Now:yyyy年MM月dd日}";
            InitalizeRangeValue("H" + 2, "J" + 2, Date);
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
            index = 4;
            return true;
        }
        #endregion Method
    }
}
