using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Data;
using Microsoft.Office.Interop.Excel;
using YuLinTu.Library.YltDatabase;
using YuLinTu.Library.Entity;
using System.IO;

namespace YuLinTu.Library.Office
{
    public class PublicFrom : ExportExcelBase
    {
        #region Ctor

        public PublicFrom(IDatabase DataBase, Zone tempZone)
        {
            dataBase = DataBase;
            zone = tempZone;
        }

        #endregion

        #region Fildes

        IDatabase dataBase;
        BuildLandPropertyCollection landPropertys;
        Zone zone;
        string message;
        string cityName;
        string townName;
        string villageName;
        int index = 10;
        bool isChecked = true;
        string templatePath;

        #endregion

        #region Methods

        public string BeginPrint(string templatePath)
        {
            this.templatePath = templatePath;

            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                return "目标模板文件不存在！";

            if (zone == null)
                return "对应地域信息错误";

            OpenExcelFile();
            GetData();
            if (isChecked)
            {
                Write();
            }
            return message;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile()
        {
            string filePaths = templatePath;

            Open(filePaths);
        }

        #region  得到数据、设置Range 、设置标题 

        /// <summary>
        /// 得到数据
        /// </summary>
        private void GetData()
        {
            landPropertys = dataBase.BuildLandProperty.GetByOwnUnitCode(zone.FullCode);
            if (landPropertys == null)
            {
                isChecked = false;
                message = "错误  没有集体建设用地信息!";
            }
            GetCity(zone);
            GetTown(zone);
            GetVillage(zone);
        }

        /// <summary>
        /// 得到市级名称
        /// </summary>
        /// <param name="Temp"></param>
        private void GetCity(Zone Temp)
        {
            if (Temp.Level == eZoneLevel.City)
                cityName = Temp.Name;
            else
                GetCity(dataBase.Zone.Get(Temp.UpLevelCode));
        }

        /// <summary>
        /// 得到镇级名称
        /// </summary>
        /// <param name="tmepZone"></param>
        /// <returns></returns>
        private void GetTown(Zone tempZone)
        {
            if (tempZone.Level == eZoneLevel.Town)
                townName = tempZone.Name;
            else
                GetTown(dataBase.Zone.Get(tempZone.UpLevelCode));
        }

        /// <summary>
        /// 得到村级名称
        /// </summary>
        /// <param name="tmepZone"></param>
        /// <returns></returns>
        private void GetVillage(Zone tempZone)
        {
            if (tempZone.Level == eZoneLevel.Village)
                villageName = tempZone.Name;
            else
                GetVillage(dataBase.Zone.Get(tempZone.UpLevelCode));
        }

        /// <summary>
        /// 设置Range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        private void SetRangeInfo(string start, string end, string value)
        {
            Range range = workSheet.get_Range(start, end);
            range.Merge(0);
            range.NumberFormatLocal = "@";
            range.WrapText = true;
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range.Value2 = value;
            range.RowHeight = 36.25;
            range.Font.Size = 12;
            
        }

        /// <summary>
        /// 设置Title
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        private void SetTitleInfo()
        {
            int count = 0;
            foreach (BuildLandProperty landProperty in landPropertys)
            {
                HouseLandPropertyCollection housePropertys = dataBase.HouseLandProperty.GetByLandID(landProperty.ID);
                if (housePropertys != null)
                    count += housePropertys.Count;
            }
            string pathName = cityName+townName+villageName+zone.Name+count.ToString();
            SetTitleRange("B1", "Q2", pathName + "户农村房屋所有权证");
            SetTitleInfoTwo();
        }

        /// <summary>
        /// 设置Title
        /// </summary>
        private void SetTitleRange(string start, string end, string value)
        {
            Range range1 = workSheet.get_Range(start, end);
            range1.Merge(0);
            range1.NumberFormatLocal = "@";
            range1.WrapText = true;
            range1.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range1.Value2 = value;
            range1.Font.Size = 22;
            range1.Font.Bold = true;
        }

        /// <summary>
        /// 设置Title2信息
        /// </summary>
        private void SetTitleInfoTwo()
        {
            SetTitleRange("G3", "K4", "确权登记情况公示表");
            SetTitleTwoRange("N6", "O6", "单位：平方米、层");
            SetTitleTwoRange("A7", "A9", "门牌号");
            SetTitleTwoRange("B7", "B9", "产权人");
            SetTitleTwoRange("C7", "D9", "房屋坐落");
            SetTitleTwoRange("E7", "F9", "建筑总面积");
            SetTitleTwoRange("G7", "K7", "结构");
            SetTitleTwoRange("G8", "G9", "混合");
            SetTitleTwoRange("H8", "H9", "砖木");
            SetTitleTwoRange("I8", "I9", "简易");
            SetTitleTwoRange("J8", "J9", "木");
            SetTitleTwoRange("K8", "K9", "其他");
            SetTitleTwoRange("L7", "L9", "楼层");
            SetTitleTwoRange("M7", "M9", "建成年份");
            SetTitleTwoRange("N7", "N9", "  家庭人  口数");
            SetTitleTwoRange("O7", "R9", "共有人");
        }

        /// <summary>
        /// 设置标题2信息
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        private void SetTitleTwoRange(string start, string end, string value)
        {
            Range range1 = workSheet.get_Range(start, end);
            range1.Merge(0);
            range1.NumberFormatLocal = "@";
            range1.WrapText = true;
            range1.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range1.Value2 = value;
            range1.Font.Size = 11;
        }

        #endregion

        #region 设置内容

        /// <summary>
        /// 设置房屋内容
        /// </summary>
        private void SetHouseInfo(HouseLandProperty houseProperty)
        {
            #region Fildes

            HouseCollection houses = dataBase.House.GetByPropertyID(houseProperty.ID);
            if (houses == null)
            {
                houses = new HouseCollection();
            }
            double sum = 0.0;
            double sumMix = 0.0;
            double sumBrickWood = 0.0;
            double sumSimple = 0.0;
            double sumWood = 0.0;
            double sumOther = 0.0;
            int floorNumber = 0;
            int buidTime = 0;

            #endregion

            #region 循环读取每间房屋 得到面积

            foreach (House house in houses)
            {
                sum += house.BilidingArea.Value;
                if (floorNumber < house.FloorNumber)
                    floorNumber = house.FloorNumber;
                if (buidTime < int.Parse(house.BuidTime.ToString().Substring(0, 4)))
                    buidTime = int.Parse(house.BuidTime.ToString().Substring(0, 4));
                switch (house.HousesStructure)
                {
                    case eHouseStructure.Mix:
                        sumMix += house.BilidingArea.Value;
                        break;
                    case eHouseStructure.BrickWood :
                        sumBrickWood += house.BilidingArea.Value;
                        break;
                    case eHouseStructure.Simple:
                        sumSimple += house.BilidingArea.Value;
                        break;
                    case eHouseStructure.Wood:
                        sumWood += house.BilidingArea.Value;
                        break;
                    case eHouseStructure.Other:
                        sumOther += house.BilidingArea.Value;
                        break;
                    default:
                        sumOther += house.BilidingArea.Value;
                        break;
                }
            }

            #endregion

            #region 设置Excel显示

            SetRangeInfo("E" + index.ToString(), "F" + index.ToString(), sum.ToString());
            if (sumMix > 0.0)
                SetRangeInfo("G" + index.ToString(), "G" + index.ToString(), sumMix.ToString());
            else
                SetRangeInfo("G" + index.ToString(), "G" + index.ToString(), "");

            if (sumBrickWood > 0.0)
                SetRangeInfo("H" + index.ToString(), "H" + index.ToString(), sumBrickWood.ToString());
            else
                SetRangeInfo("H" + index.ToString(), "H" + index.ToString(), "");

            if (sumSimple > 0.0)
                SetRangeInfo("I" + index.ToString(), "I" + index.ToString(), sumSimple.ToString());
            else
                SetRangeInfo("I" + index.ToString(), "I" + index.ToString(), "");

            if (sumWood > 0.0)
                SetRangeInfo("J" + index.ToString(), "J" + index.ToString(), sumWood.ToString());
            else
                SetRangeInfo("J" + index.ToString(), "J" + index.ToString(), "");

            if (sumOther > 0.0)
                SetRangeInfo("K" + index.ToString(), "K" + index.ToString(), sumOther.ToString());
            else
                SetRangeInfo("K" + index.ToString(), "K" + index.ToString(), "");
            SetRangeInfo("L" + index.ToString(), "L" + index.ToString(), floorNumber.ToString());
            SetRangeInfo("M" + index.ToString(), "M" + index.ToString(), buidTime.ToString());

            #endregion
        }

        /// <summary>
        /// 设置房屋所有权信息
        /// </summary>
        private void SetHousePoperty(BuildLandProperty landProperty)
        { 
            HouseLandPropertyCollection housePropertys = dataBase.HouseLandProperty.GetByLandID(landProperty.ID);
            if (housePropertys == null)
            {
                isChecked = false;
                message = "错误  没有找到房屋所有权信息！";
                return;
            }
            foreach (HouseLandProperty property in housePropertys)
	        {
                SetRangeInfo("A" + index.ToString(), "A" + index.ToString(), property.DoorNumber);
                Family family = dataBase.Family.Get(property.Owner);
                if (family == null)
                    family = new Family();
                SetRangeInfo("B" + index.ToString(), "B" + index.ToString(), family.HouseholderName);
                SetRangeInfo("N" + index.ToString(), "N" + index.ToString(), property.Persons.ToString());
                SetRangeInfo("C" + index.ToString(), "D" + index.ToString(), townName + villageName + zone.Name);

                PersonCollection persons = dataBase.Person.GetPersons(family.ID);
                if (persons != null)
                {
                    string personsName = null;
                    foreach (Person person in persons)
                    {
                        personsName += person.Name + "、";
                    }
                    SetRangeInfo("O" + index.ToString(), "R" + index.ToString(), personsName.Substring(0, personsName.Length - 1));
                }
                else
                    SetRangeInfo("O" + index.ToString(), "R" + index.ToString(), "");
                SetHouseInfo(property);
                index++;
	        }
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        private void SetLineType(string end)
        {
            Range range = workSheet.get_Range("A10", end);

            SetRangeLineType(range, XlLineStyle.xlContinuous);
        }

        #endregion

        #endregion

        #region Override

        public override void Write()
        {
            try
            {
                //设置标题
                SetTitleInfo();
                index = 10;
                foreach (BuildLandProperty landProperty in landPropertys)
                {
                    SetHousePoperty(landProperty);
                    if (!isChecked)
                        break;
                }
                if (isChecked)
                {
                    SetLineType("R" + (index - 1).ToString());
                }
                if (string.IsNullOrEmpty(message))
                    Show();//显示数据
            }
            catch (Exception e)
            {
                message += e.Message;
                Dispose();
                return;
            }

        }

        public override void Read()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
