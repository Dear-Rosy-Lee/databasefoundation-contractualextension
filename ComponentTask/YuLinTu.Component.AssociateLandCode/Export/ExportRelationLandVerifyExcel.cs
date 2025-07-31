using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 摸底调查核实表
    /// </summary>
    [Serializable]
    public class ExportRelationLandVerifyExcel : ExportLandVerifyExcelTable
    {
        List<CheckVpEntity> checkVpEntities = new List<CheckVpEntity>();

        /// <summary>
        /// 是否应确未确
        /// </summary>
        public bool SFYQWQ { get; set; }
        private string contractlandtype;

        #region Ctor

        public ExportRelationLandVerifyExcel()
        {
            SaveFilePath = string.Empty;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
            base.TemplateName = "摸底调查核实表";
            checkVpEntities.Clear();
            contractlandtype = ((int)eLandCategoryType.ContractLand) + "";
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

        public override bool BeginWrite()
        {
            var dictStation = DbContext.CreateDictWorkStation();
            var DictList = dictStation.Get();
            if (DictList != null && DictList.Count > 0)
            {
                dicSF = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.SF);
                dictCBFLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBFLX);
                dictXB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.XB);
                dictZJLX = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.ZJLX);
                dictTDYT = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
                dictDLDJ = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DLDJ);
                dictSYQXZ = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.SYQXZ);
                dictDKLB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DKLB);
                dictTDLYLX = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDLYLX);
            }
            if (CurrentZone == null)
            {
                return false;
            }

            if (AccountLandFamily == null || AccountLandFamily.Count == 0)
                return false;

            AccountLandFamily.Sort((a, b) =>
            {
                long aNumber = Convert.ToInt64(a.CurrentFamily.FamilyNumber);
                long bNumber = Convert.ToInt64(b.CurrentFamily.FamilyNumber);
                return aNumber.CompareTo(bNumber);
            });

            if (!string.IsNullOrEmpty(AccountLandFamily[0].CurrentFamily.OldVirtualCode))
            {
                //AccountLandFamily.GroupBy(g=>g.CurrentFamily.)
                //var query = from d in AccountLandFamily
                //            orderby d.CurrentFamily.oldVirtualCode, d.CurrentFamily.FamilyNumber
                //            select d;
                //AccountLandFamily = query.ToList();
            }
            familyCount = AccountLandFamily.Count;
            toolProgress.InitializationPercent(AccountLandFamily.Count, 99, 1);
            Worksheet2 = workbook.Worksheets.Add("ysb");
            Worksheet2.Cells.SetColumnWidth(0, 21);
            Worksheet2.Cells.SetColumnWidth(1, 21);
            Worksheet2.Cells.SetColumnWidth(2, 25);
            Worksheet2.Cells.SetColumnWidth(3, 25);
            bhqkList = EnumStore<eBHQK>.GetListByType();
            //var delvps = AccountLandFamily.FindAll(t => t.CurrentFamily.Status == eVirtualPersonStatus.Bad);

            //HashSet<Guid> delset = new HashSet<Guid>();
            //foreach (var landFamily in AccountLandFamily)
            //{
            //    if (delset.Contains(landFamily.CurrentFamily.ID))
            //        continue;
            //    var delvp = delvps.Find(t => (t.CurrentFamily.ZoneCode.PadRight(14, '0') + t.CurrentFamily.FamilyNumber.PadLeft(4, '0')) == landFamily.CurrentFamily.OldVirtualCode);
            //    if (delvp != null)
            //    {
            //        if (delvp.CurrentFamily.ID.ToString() == "abbcb9db-41bd-4572-b58e-79e5454b87c1")
            //        {
            //        }
            //        delset.Add(delvp.CurrentFamily.ID);
            //        foreach (var item in delvp.LandDelCollection)
            //        {
            //            if (landFamily.LandCollection.Any(t => t.LandNumber == item.DKBM))
            //                continue;
            //            landFamily.LandDelCollection.Add(item);
            //        }
            //    }
            //}
            //AccountLandFamily.RemoveAll(t => delset.Contains(t.CurrentFamily.ID));
            foreach (var landFamily in AccountLandFamily)
            {
                cindex++;
                toolProgress.DynamicProgress(ZoneDesc + landFamily.CurrentFamily.Name);
                WriteInformation(landFamily);
            }
            WriteTempLate();
            SetLineType("A1", "D" + index, false, Worksheet2);
            SetLineType("A7", "AI" + index, false);
            Worksheet2.IsVisible = false;
            this.Information = string.Format("{0}导出{1}条承包台账数据!", ZoneDesc, AccountLandFamily.Count);
            AccountLandFamily = null;
            return true;
        }

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

            if (landFamily.CurrentFamily.Name == "集体" &&
                lands.Count == 0 &&
                peoples.Count == 0 &&
                landDels.Count == 0)
            {
                return;
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
            var vpcode = landFamily.CurrentFamily.ZoneCode.PadRight(14, '0') + landFamily.CurrentFamily.FamilyNumber.PadLeft(4, '0');
            var cvp = checkVpEntities.Find(t => t.NewVpCode == vpcode);
            if (cvp == null && !jtmcs.Contains(landFamily.CurrentFamily.Name))
            {
                cvp = new CheckVpEntity() { NewVpCode = vpcode, OldVpCode = landFamily.CurrentFamily.OldVirtualCode };
                checkVpEntities.Add(cvp);
            }

            HashSet<string> setlandnumber = new HashSet<string>();//手动设置关联的地块编码
            var familystr = ((int)eLandCategoryType.ContractLand).ToString();
            foreach (ContractLand land in lands)
            {
                double tableArea = land.TableArea ?? 0;
                TotalLandTable += tableArea;
                if (land.LandCategory == familystr)
                {
                    TotalLandAware += land.AwareArea;
                }
                TotalLandActual += land.ActualArea;
                WriteLandInformation(landFamily.CurrentFamily, land, aindex, land.LandCategory == familystr);
                SetRowHeight(aindex, 27.75);
                if (!string.IsNullOrEmpty(land.OldLandNumber) && !setlandnumber.Contains(land.OldLandNumber))
                {
                    setlandnumber.Add(land.OldLandNumber);
                }
                if (cvp != null)
                {
                    cvp.LandList.Add(new CheckLandEntity()
                    {
                        NewLandCode = land.LandNumber.IsNullOrEmpty() ? "" : land.LandNumber,
                        OldLandCode = land.OldLandNumber.IsNullOrEmpty() ? "" : land.OldLandNumber
                    });
                }
                aindex++;
            }
            landDels.RemoveAll(t => setlandnumber.Contains(t.DKBM));
            foreach (ContractLand_Del landDel in landDels)
            {
                if (cvp != null)
                {
                    cvp.DelLandList.Add(landDel);
                }
                SetRowHeight(aindex, 27.75);
                WriteDelLandInformation(landFamily.CurrentFamily, landDel, aindex);
                aindex++;
            }
            int height = 0;
            if ((landFamily.LandCollection.Count + landFamily.LandDelCollection.Count) > landFamily.Persons.Count)
            {
                height = landFamily.LandCollection.Count + landFamily.LandDelCollection.Count;
            }
            else
            {
                height = landFamily.Persons.Count;
            }
            landCount += lands.Count + landDels.Count;
            height = height == 0 ? 1 : height;
            AwareArea += TotalLandAware;
            ActualArea += TotalLandActual;
            TableArea += TotalLandTable;
            int getcode = GetCBFLXNumber(landFamily.CurrentFamily.FamilyExpand.ContractorType);
            Dictionary cardtype = dictCBFLX.Find(c => c.Code.Equals(getcode.ToString()));
            string result = landFamily.CurrentFamily.FamilyNumber.PadLeft(4, '0');
            string virtualpersonCode = $"{landFamily.CurrentFamily.ZoneCode.PadRight(14, '0')}{result}";
            if (landFamily.CurrentFamily.Status == eVirtualPersonStatus.Bad)
            {
                virtualpersonCode = landFamily.CurrentFamily.OldVirtualCode;
            }
            bool sfjt = jtmcs.Contains(landFamily.CurrentFamily.Name);
            string oldvpcode = string.IsNullOrEmpty(landFamily.CurrentFamily.OldVirtualCode) && landFamily.CurrentFamily.Status == eVirtualPersonStatus.Bad ? virtualpersonCode : landFamily.CurrentFamily.OldVirtualCode;
            InitalizeRangeValue("A" + index, "A" + (index + height - 1), cindex);
            InitalizeRangeValue("B" + index, "B" + (index + height - 1), landFamily.CurrentFamily.Name);
            InitalizeRangeValue("C" + index, "C" + (index + height - 1), sfjt ? "" : cardtype.Name);
            InitalizeRangeValue("D" + index, "D" + (index + height - 1), sfjt ? "" : virtualpersonCode);
            InitalizeSheet2RangeValue("A" + 1, "A" + 1, "c1", Worksheet2);
            InitalizeSheet2RangeValue("A" + (index - 5), "A" + (index + height - 1 - 5), sfjt ? "" : virtualpersonCode, Worksheet2);
            InitalizeSheet2RangeValue("B" + 1, "B" + 1, "c2", Worksheet2);
            if (!SFYQWQ)
                InitalizeSheet2RangeValue("B" + (index - 5), "B" + (index + height - 1 - 5), sfjt ? "" : oldvpcode, Worksheet2);
            InitalizeRangeValue("E" + index, "E" + (index + height - 1), landFamily.CurrentFamily.Telephone);
            InitalizeRangeValue("F" + index, "F" + (index + height - 1), landFamily.CurrentFamily.Address);
            InitalizeRangeValue("G" + index, "G" + (index + height - 1), sfjt ? 0 : landFamily.Persons.Count);
            InitalizeRangeValue("X" + index, "X" + (index + height - 1), TotalLandTable);
            InitalizeRangeValue("Z" + index, "Z" + (index + height - 1), TotalLandAware);
            InitalizeRangeValue("AB" + index, "AB" + (index + height - 1), TotalLandActual);

            //if (!sfjt)
            //    checkVpEntities.Add(new CheckVpEntity() { NewVpCode = virtualpersonCode, OldVpCode = oldvpcode });

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
        /// 书写地块信息
        /// </summary>
        public override void WriteLandInformation(VirtualPerson vp, ContractLand land, int index, bool familycontract, int datanum = 2)
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
            if (!SFYQWQ)
                InitalizeSheet2RangeValue("D" + (index - 5), "D" + (index - 5), land.OldLandNumber.IsNullOrEmpty() ? "" : land.OldLandNumber, Worksheet2);
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
            if (land.LandCategory == contractlandtype && land.AwareArea == 0)
                throw new Exception($"延包地块{land.LandNumber}的合同面积必须大于0");
            InitalizeRangeValue("Y" + index, "Y" + index, familycontract ? ToolMath.RoundNumericFormat(land.AwareArea, 2) : 0);
            InitalizeRangeValue("AA" + index, "AA" + index, ToolMath.RoundNumericFormat(land.ActualArea, 2));
            InitalizeRangeValue("AC" + index, "AC" + index, land.NeighborEast != null ? land.NeighborEast : "");
            InitalizeRangeValue("AD" + index, "AD" + index, land.NeighborSouth != null ? land.NeighborSouth : "");
            InitalizeRangeValue("AE" + index, "AE" + index, land.NeighborWest != null ? land.NeighborWest : "");
            InitalizeRangeValue("AF" + index, "AF" + index, land.NeighborNorth != null ? land.NeighborNorth : "");
            InitalizeRangeValue("AG" + index, "AH" + index, land.Comment);
        }

        /// <summary>
        /// 书写删除地块信息
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="landDel"></param>
        /// <param name="index"></param>
        public override void WriteDelLandInformation(VirtualPerson vp, ContractLand_Del landDel, int index)
        {
            InitalizeRangeValue("P" + index, "P" + index, landDel.DKMC.IsNullOrEmpty() ? "" : landDel.DKMC);
            InitalizeRangeValue("Q" + index, "Q" + index, landDel.DKBM.IsNullOrEmpty() ? "" : landDel.DKBM);
            InitalizeSheet2RangeValue("C" + 1, "C" + 1, "d1", Worksheet2);
            InitalizeSheet2RangeValue("C" + (index - 5), "C" + (index - 5), landDel.DKBM.IsNullOrEmpty() ? "" : landDel.DKBM, Worksheet2);
            InitalizeSheet2RangeValue("D" + 1, "D" + 1, "d2", Worksheet2);
            if (!SFYQWQ)
                InitalizeSheet2RangeValue("D" + (index - 5), "D" + (index - 5), landDel.QQDKBM.IsNullOrEmpty() ? landDel.DKBM : landDel.QQDKBM, Worksheet2);
            InitalizeRangeValue("Y" + index, "Y" + index, (landDel.QQMJ > 0.0) ? ToolMath.SetNumbericFormat(landDel.QQMJ.ToString(), 2) : SystemDefine.InitalizeAreaString());
            InitalizeRangeValue("AA" + index, "AA" + index, (landDel.SCMJ > 0.0) ? ToolMath.SetNumbericFormat(landDel.SCMJ.ToString(), 2) : SystemDefine.InitalizeAreaString());
            InitalizeRangeValue("AC" + index, "AC" + index, landDel.DKDZ != null ? landDel.DKDZ : "");
            InitalizeRangeValue("AD" + index, "AD" + index, landDel.DKNZ != null ? landDel.DKNZ : "");
            InitalizeRangeValue("AE" + index, "AE" + index, landDel.DKXZ != null ? landDel.DKXZ : "");
            InitalizeRangeValue("AF" + index, "AF" + index, landDel.DKBZ != null ? landDel.DKBZ : "");
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
                if (SF != null)
                    InitalizeRangeValue("W" + index, "W" + index, SF.Name);
            }

        }

        public override void WriteTempLate()
        {
            base.WriteTempLate();
            var title = $"{ZoneDesc}-农村土地承包经营权调查成果表";
            InitalizeRangeValue("A" + 1, "AI" + 2, title);
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
                sheet.Protect(ProtectionType.All, "Ylt@123456", ""); // 设置密码
                                                                     // 创建一个范围，例如A1:C10，然后解锁这个范围
                Cells cells = sheet.Cells;
                Range range = cells.CreateRange("AI3:AI" + (index - 1));
                range.ApplyStyle(unlockStyle, styleFlag); // 应用样式但不锁定范围 


                Worksheet sheet2 = workbook.Worksheets[2];
                // 保护工作表
                sheet2.Protect(ProtectionType.All, "Ylt@ysb", ""); // 设置密码
                                                                   // 创建一个范围，例如A1:C10，然后解锁这个范围
                Cells cells2 = sheet2.Cells;
                Range range2 = cells.CreateRange("A1:E" + (index - 1));
                range2.ApplyStyle(unlockStyle, styleFlag); // 应用样式但不锁定范围 

                workbook.Save(fileName);
            }
            catch (Exception ex)
            {
                Log.Log.WriteError(this, "SaveEnd", ex.Message);
            }
            CheckData();
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        private void CheckData()
        {
            //新承包方编码重复，就承包方编码重复   新地块编码重复，就地块编码重复
            if (checkVpEntities.Count == 0)
                return;
            var oldvpkeys = new List<string>();
            var newVpkeys = new List<string>();
            var oldLandkeys = new List<string>();
            var newLandkeys = new List<string>();
            var delLandkeys = new List<string>();

            foreach (var cvpe in checkVpEntities)
            {
                oldvpkeys.Add(cvpe.OldVpCode == null ? "" : cvpe.OldVpCode);
                newVpkeys.Add(cvpe.NewVpCode);

                foreach (var item in cvpe.LandList)
                {
                    oldLandkeys.Add(item.OldLandCode == null ? "" : item.OldLandCode);
                    newLandkeys.Add(item.NewLandCode);
                }
                foreach (var item in cvpe.DelLandList)
                {
                    delLandkeys.Add(item.DKBM);
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            var nglist = newVpkeys.GroupBy(g => g).Where(t => t.Key != "" && t.Count() > 1).ToList();
            var oglist = oldvpkeys.GroupBy(g => g).Where(t => t.Key != "" && t.Count() > 1).ToList();
            var ollist = oldLandkeys.GroupBy(g => g).Where(t => t.Key != "" && t.Count() > 1).ToList();
            var nllist = newLandkeys.GroupBy(g => g).Where(t => t.Key != "" && t.Count() > 1).ToList();
            oglist.ForEach(t => stringBuilder.AppendLine($"原承包方编码：{t.Key}重复挂接"));
            nglist.ForEach(t => stringBuilder.AppendLine($"承包方编码：{t.Key}存在重复"));

            ollist.ForEach(t => stringBuilder.AppendLine($"原地块编码：{t.Key}重复挂接"));
            nllist.ForEach(t => stringBuilder.AppendLine($"地块编码：{t.Key}存在重复"));

            delLandkeys.ForEach(f =>
            {
                if (newLandkeys.Contains(f))
                {
                    stringBuilder.AppendLine($"地块编码：{f}存在重复处理的情况，请核实数据");
                }
            });

            var nexitvps = newVpkeys.FindAll(t => oldvpkeys.Contains(t));//新编码在旧编码中存在的情况
            var nexitlands = newLandkeys.FindAll(t => oldLandkeys.Contains(t));//新编码在旧编码中存在的情况

            var kvlist = new List<Tuple<string, string>>();
            var landkvlist = new List<Tuple<string, string>>();
            foreach (var cvpe in checkVpEntities)
            {
                if (cvpe.NewVpCode == cvpe.OldVpCode)
                    continue;
                if (!string.IsNullOrEmpty(cvpe.OldVpCode))
                {
                    var fkv = kvlist.Find(t => t.Item1 == cvpe.OldVpCode && t.Item2 == cvpe.NewVpCode);
                    if (fkv != null)
                    {
                        stringBuilder.AppendLine($"承包方挂接存在交叉：承包方编码{cvpe.NewVpCode}，原承包方编码{cvpe.OldVpCode};");
                    }
                    else
                    {
                        if (nexitvps.Contains(cvpe.NewVpCode) && nexitvps.Contains(cvpe.OldVpCode))
                        {
                            stringBuilder.AppendLine($"承包方挂接存在交叉：承包方编码{cvpe.NewVpCode}，原承包方编码{cvpe.OldVpCode};");
                        }
                        kvlist.Add(new Tuple<string, string>(cvpe.NewVpCode, cvpe.OldVpCode));
                    }
                }
                foreach (var item in cvpe.LandList)
                {
                    if (item.NewLandCode == item.OldLandCode)
                        continue;
                    if (!string.IsNullOrEmpty(item.OldLandCode))
                    {
                        var landkv = landkvlist.Find(t => t.Item1 == item.OldLandCode && t.Item2 == item.NewLandCode);
                        if (landkv != null)
                        {
                            stringBuilder.AppendLine($"地块挂接存在交叉：地块编码{item.NewLandCode}，原地块编码{item.OldLandCode};");
                        }
                        else
                        {
                            if (nexitlands.Contains(item.NewLandCode) && nexitlands.Contains(item.OldLandCode))
                            {
                                stringBuilder.AppendLine($"地块挂接存在交叉：地块编码{item.NewLandCode}，原地块编码{item.OldLandCode};");
                            }
                            landkvlist.Add(new Tuple<string, string>(item.NewLandCode, item.OldLandCode));
                        }
                    }
                }
            }
            if (stringBuilder.Length > 0)
            {
                throw new Exception(" 挂接数据存在问题：\n" + stringBuilder.ToString());
            }
        }

        class CheckVpEntity
        {
            public string NewVpCode { get; set; }

            public string OldVpCode { get; set; }

            public List<CheckLandEntity> LandList { get; set; }

            public List<ContractLand_Del> DelLandList { get; set; }

            public CheckVpEntity()
            {
                LandList = new List<CheckLandEntity>();
                DelLandList = new List<ContractLand_Del>();
            }
        }

        class CheckLandEntity
        {
            public string NewLandCode { get; set; }

            public string OldLandCode { get; set; }
        }
    }
}