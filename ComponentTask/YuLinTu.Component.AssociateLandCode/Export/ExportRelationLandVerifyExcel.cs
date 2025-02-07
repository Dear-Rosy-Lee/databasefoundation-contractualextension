using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Cells;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 摸底调查核实表
    /// </summary>
    [Serializable]
    public class ExportRelationLandVerifyExcel : ExportLandVerifyExcelTable
    {
        #region Ctor

        public ExportRelationLandVerifyExcel()
        {
            SaveFilePath = string.Empty;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
            base.TemplateName = "摸底调查核实表";
        }

        /// <summary>
        /// 进度提示
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

        #endregion Ctor

        #region 开始生成Excel之前的一系列操作

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

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(templatePath);
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

        #endregion 开始生成Excel之前的一系列操作  

        /// <summary>
        /// 书写每个承包户信息
        /// </summary>
        /// <param name="virtualPerson"></param>
        public override void WriteInformation(ContractAccountLandFamily landFamily)
        {
            if (landFamily == null)
                return;

            double TotalLandAware = 0;
            double TotalLandActual = 0;
            double TotalLandTable = 0;
            int aindex = index;
            int bindex = index;

            List<ContractLand> lands = landFamily.LandCollection;
            List<ContractLand_Del> landDels = landFamily.LandDelCollection;
            List<Person> peoples = landFamily.Persons;

            var hz = peoples.FirstOrDefault(f => f.Relationship == "01" || f.Relationship == "02"
            || f.Relationship == "户主" || f.Relationship == "本人");
            if (hz != null)
            {
                peoples.Remove(hz);
                peoples.Insert(0, hz);
            }

            peopleCount += peoples.Count;
            lands.Sort("IsStockLand", eOrder.Ascending);
            bool flag = false;
            if (peoples.Count > lands.Count)
                flag = true;
            foreach (Person person in peoples)
            {
                SetRowHeight(bindex - 1, 27.75);
                WritePersonInformation(person, bindex, flag);
                SetRowHeight(bindex, 27.75);
                bindex++;
            }
            HashSet<string> setlandnumber = new HashSet<string>();//手动设置关联的地块编码
            foreach (ContractLand land in lands)
            {
                double tableArea = land.TableArea ?? 0;
                TotalLandTable += tableArea;
                TotalLandAware += land.AwareArea;
                TotalLandActual += land.ActualArea;
                WriteLandInformation(landFamily.CurrentFamily, land, aindex);
                SetRowHeight(aindex, 27.75);
                if (!string.IsNullOrEmpty(land.OldLandNumber) && !setlandnumber.Contains(land.OldLandNumber))
                {
                    setlandnumber.Add(land.OldLandNumber);
                }
                aindex++;
            }
            landDels.RemoveAll(t => setlandnumber.Contains(t.DKBM));
            foreach (ContractLand_Del landDel in landDels)
            {
                TotalLandTable += 0;
                TotalLandAware += landDel.QQMJ;
                TotalLandActual += landDel.SCMJ;
                SetRowHeight(aindex, 27.75);
                WriteDelLandInformation(landFamily.CurrentFamily, landDel, aindex);
                aindex++;
            }
            int height = 0;
            if ((landFamily.LandCollection.Count > landFamily.Persons.Count) || landFamily.LandDelCollection.Count > landFamily.Persons.Count)
            {
                height = landFamily.LandCollection.Count + landFamily.LandDelCollection.Count;
            }
            else
            {
                height = landFamily.Persons.Count;
            }
            landCount += lands.Count + landDels.Count;
            if (lands.Count == 0)
            {
                //index++;
            }
            AwareArea += TotalLandAware;
            ActualArea += TotalLandActual;
            TableArea += TotalLandTable;
            int getcode = GetCBFLXNumber(landFamily.CurrentFamily.FamilyExpand.ContractorType);
            Dictionary cardtype = dictCBFLX.Find(c => c.Code.Equals(getcode.ToString()));
            string result = landFamily.CurrentFamily.FamilyNumber.PadLeft(4, '0');
            string virtualpersonCode = $"{landFamily.CurrentFamily.ZoneCode.PadRight(14, '0')}{result}";
            string oldvpcode = string.IsNullOrEmpty(landFamily.CurrentFamily.OldVirtualCode) ? virtualpersonCode : landFamily.CurrentFamily.OldVirtualCode;
            InitalizeRangeValue("A" + index, "A" + (index + height - 1), cindex);
            InitalizeRangeValue("B" + index, "B" + (index + height - 1), landFamily.CurrentFamily.Name);
            InitalizeRangeValue("C" + index, "C" + (index + height - 1), cardtype.Name);
            InitalizeRangeValue("D" + index, "D" + (index + height - 1), $"{landFamily.CurrentFamily.ZoneCode.PadRight(14, '0')}{result}");
            InitalizeSheet2RangeValue("A" + 1, "A" + 1, "c1", Worksheet2);
            InitalizeSheet2RangeValue("A" + (index - 5), "A" + (index + height - 1 - 5), virtualpersonCode, Worksheet2);
            InitalizeSheet2RangeValue("B" + 1, "B" + 1, "c2", Worksheet2);
            InitalizeSheet2RangeValue("B" + (index - 5), "B" + (index + height - 1 - 5), oldvpcode, Worksheet2);
            InitalizeRangeValue("E" + index, "E" + (index + height - 1), landFamily.CurrentFamily.Telephone);
            InitalizeRangeValue("F" + index, "F" + (index + height - 1), landFamily.CurrentFamily.Address);
            InitalizeRangeValue("G" + index, "G" + (index + height - 1), landFamily.Persons.Count);
            InitalizeRangeValue("X" + index, "X" + (index + height - 1), TotalLandTable);
            InitalizeRangeValue("Z" + index, "Z" + (index + height - 1), TotalLandAware);
            InitalizeRangeValue("AB" + index, "AB" + (index + height - 1), TotalLandActual);
            if (landFamily.CurrentFamily.Status == eVirtualPersonStatus.Bad)
            {
                InitalizeRangeValue("AI" + index, "AI" + (index + height - 1), "合同注销");
            }
            else
            {
                var bhqk = bhqkList.Find(t => t.Value == landFamily.CurrentFamily.ChangeSituation);
                if (bhqk != null)
                    InitalizeRangeValue("AI" + index, "AI" + (index + height - 1), bhqk.DisplayName);
                else
                {
                    InitalizeRangeValue("AI" + index, "AI" + (index + height - 1), "其他直接顺延");
                }
            }
            index += height;
            //workbook.Worksheets[0].HorizontalPageBreaks.Add("A" + index);
            //workbook.Worksheets[0].VerticalPageBreaks.Add("A" + index);
            lands.Clear();
        }

        /// <summary>
        /// 书写删除地块信息
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="landDel"></param>
        /// <param name="index"></param>
        public void WriteDelLandInformation(VirtualPerson vp, ContractLand_Del landDel, int index)
        {
            InitalizeRangeValue("P" + index, "P" + index, landDel.DKMC.IsNullOrEmpty() ? "/" : landDel.DKMC);
            InitalizeRangeValue("Q" + index, "Q" + index, landDel.DKBM.IsNullOrEmpty() ? "/" : landDel.DKBM);
            InitalizeSheet2RangeValue("C" + 1, "C" + 1, "d1", Worksheet2);
            InitalizeSheet2RangeValue("C" + (index - 5), "C" + (index - 5), landDel.DKBM.IsNullOrEmpty() ? "" : landDel.DKBM, Worksheet2);
            InitalizeSheet2RangeValue("D" + 1, "D" + 1, "d2", Worksheet2);
            InitalizeSheet2RangeValue("D" + (index - 5), "D" + (index - 5), landDel.QQDKBM.IsNullOrEmpty() ? landDel.DKBM : landDel.QQDKBM, Worksheet2);
            InitalizeRangeValue("Y" + index, "Y" + index, (landDel.QQMJ > 0.0) ? ToolMath.SetNumbericFormat(landDel.QQMJ.ToString(), 2) : SystemDefine.InitalizeAreaString());
            InitalizeRangeValue("AA" + index, "AA" + index, (landDel.SCMJ > 0.0) ? ToolMath.SetNumbericFormat(landDel.SCMJ.ToString(), 2) : SystemDefine.InitalizeAreaString());
            InitalizeRangeValue("AC" + index, "AC" + index, landDel.DKDZ != null ? landDel.DKDZ : "/");
            InitalizeRangeValue("AD" + index, "AD" + index, landDel.DKNZ != null ? landDel.DKNZ : "/");
            InitalizeRangeValue("AE" + index, "AE" + index, landDel.DKXZ != null ? landDel.DKXZ : "/");
            InitalizeRangeValue("AF" + index, "AF" + index, landDel.DKBZ != null ? landDel.DKBZ : "/");
            InitalizeRangeValue("AG" + index, "AH" + index, "删除");

            Dictionary syqxz = dictSYQXZ.Find(c => c.Name.Equals(landDel.SYQXZ) || c.Code.Equals(landDel.SYQXZ));
            Dictionary dklb = dictDKLB.Find(c => c.Name.Equals(landDel.DKLB) || c.Code.Equals(landDel.DKLB));
            Dictionary tdlylx = dictTDLYLX.Find(c => c.Name.Equals(landDel.TDLYLX) || c.Code.Equals(landDel.TDLYLX));
            Dictionary tdyt = dictTDYT.Find(c => c.Name.Equals(landDel.TDYT) || c.Code.Equals(landDel.TDYT));
            Dictionary dldj = dictDLDJ.Find(c => c.Name.Equals(landDel.DLDJ) || c.Code.Equals(landDel.DLDJ));


            if (syqxz != null)
                InitalizeRangeValue("R" + index, "R" + index, syqxz.Name);
            if (dklb != null)
                InitalizeRangeValue("S" + index, "S" + index, dklb.Name);
            if (tdlylx != null)
                InitalizeRangeValue("T" + index, "T" + index, tdlylx.Name);
            if (dldj != null)
                InitalizeRangeValue("U" + index, "U" + index, dldj.Name);
            if (tdyt != null)
                InitalizeRangeValue("V" + index, "V" + index, tdyt.Name);

            if (!string.IsNullOrEmpty(landDel.SFJBNT))
            {
                Dictionary SF = dicSF.Find(c => c.Code.Equals(landDel.SFJBNT));
                InitalizeRangeValue("W" + index, "W" + index, SF.Name);
            }

        }

        /// <summary>
        /// 书写地块信息
        /// </summary>
        public void WriteLandInformation(VirtualPerson vp, ContractLand land, int index)
        {
            Dictionary syqxz = dictSYQXZ.Find(c => c.Name.Equals(land.OwnRightType) || c.Code.Equals(land.OwnRightType));
            Dictionary dklb = dictDKLB.Find(c => c.Name.Equals(land.LandCategory) || c.Code.Equals(land.LandCategory));
            Dictionary tdlylx = dictTDLYLX.Find(c => c.Name.Equals(land.LandCode) || c.Code.Equals(land.LandCode));
            Dictionary tdyt = dictTDYT.Find(c => c.Name.Equals(land.Purpose) || c.Code.Equals(land.Purpose));
            Dictionary dldj = dictDLDJ.Find(c => c.Name.Equals(land.LandLevel) || c.Code.Equals(land.LandLevel));
            InitalizeRangeValue("P" + index, "P" + index, land.Name.IsNullOrEmpty() ? "" : land.Name);
            InitalizeRangeValue("Q" + index, "Q" + index, land.LandNumber.IsNullOrEmpty() ? "" : land.LandNumber);
            InitalizeSheet2RangeValue("C" + 1, "C" + 1, "d1", Worksheet2);
            InitalizeSheet2RangeValue("C" + (index - 5), "C" + (index - 5), land.LandNumber.IsNullOrEmpty() ? "" : land.LandNumber, Worksheet2);
            InitalizeSheet2RangeValue("D" + 1, "D" + 1, "d2", Worksheet2);
            InitalizeSheet2RangeValue("D" + (index - 5), "D" + (index - 5), land.OldLandNumber.IsNullOrEmpty() ? land.LandNumber : land.OldLandNumber, Worksheet2);
            if (syqxz != null)
                InitalizeRangeValue("R" + index, "R" + index, syqxz.Name);
            if (dklb != null)
                InitalizeRangeValue("S" + index, "S" + index, dklb.Name);
            if (tdlylx != null)
                InitalizeRangeValue("T" + index, "T" + index, tdlylx.Name);
            if (dldj != null)
                InitalizeRangeValue("U" + index, "U" + index, dldj.Name);
            if (tdyt != null)
                InitalizeRangeValue("V" + index, "V" + index, tdyt.Name);

            if (land.IsFarmerLand != null)
            {
                Dictionary SF = dicSF.Find(c => c.Code.Equals(land.IsFarmerLand == true ? "1" : "2"));
                InitalizeRangeValue("W" + index, "W" + index, SF.Name);
            }

            InitalizeRangeValue("Y" + index, "Y" + index, Math.Round(land.AwareArea, 2));
            InitalizeRangeValue("AA" + index, "AA" + index, Math.Round(land.ActualArea, 2));
            InitalizeRangeValue("AC" + index, "AC" + index, land.NeighborEast != null ? land.NeighborEast : "/");
            InitalizeRangeValue("AD" + index, "AD" + index, land.NeighborSouth != null ? land.NeighborSouth : "/");
            InitalizeRangeValue("AE" + index, "AE" + index, land.NeighborWest != null ? land.NeighborWest : "/");
            InitalizeRangeValue("AF" + index, "AF" + index, land.NeighborNorth != null ? land.NeighborNorth : "/");
            InitalizeRangeValue("AG" + index, "AH" + index, land.Comment);
        }

        /// <summary>
        /// 保存完成后
        /// </summary>
        protected override void SaveEnd(string fileName)
        {
            try
            {
                Workbook workbook = new Workbook(fileName);
                Worksheet sheet = workbook.Worksheets[0];
                Style unlockStyle = workbook.CreateStyle();
                unlockStyle.IsLocked = false;
                StyleFlag styleFlag = new StyleFlag();
                styleFlag.Locked = true;
                // 保护工作表
                sheet.Protect(ProtectionType.All, "ylt", ""); // 设置密码
                // 创建一个范围，例如A1:C10，然后解锁这个范围
                Cells cells = sheet.Cells;
                Range range = cells.CreateRange("AI3:AI" + (index - 1));
                range.ApplyStyle(unlockStyle, styleFlag); // 应用样式但不锁定范围 
                workbook.Save(fileName);
            }
            catch (Exception ex)
            {
                Log.Log.WriteError(this, "SaveEnd", ex.Message);
            }
        }
    }
}