using System;
using System.Text.RegularExpressions;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
{
    public class ExportParcelExtend: ExportContractLandParcelWord
    {
        private int fromthirdPageTableCount;//从第三页开始的表个数，包括第三页

        public ExportParcelExtend(IDbContext db):base(db)
        {
        }

        protected override void WriteLandInfo()
        {
            InitalizeLandRowInformation();
            InitalizeLandBookMarkInformation();
        }

        /// <summary>
        /// 初始化地块行信息
        /// </summary>
        private void InitalizeLandRowInformation()
        {
            if (geoLandCollection == null || geoLandCollection.Count == 0)
            {
                return;
            }
            if (geoLandCollection != null && geoLandCollection.Count <= 9)
            {
                SetTableCellValue(0, 7, 1, "1/1");
                DeleteTable(1);
                DeleteParagraph();
                return;
            }
            int landCount = geoLandCollection.Count - 9;
            int pageCount = landCount / 9;
            pageCount = landCount % 9 == 0 ? pageCount : (pageCount + 1);

            fromthirdPageTableCount = pageCount;
            int totalPage = fromthirdPageTableCount + 1;//总页数

            //从第三页后添加28的表格
            if (totalPage > 1)
            {
                for (int i = 0; i < fromthirdPageTableCount; i++)
                {
                    AddTable(0);
                }
            }

            SetTableCellValue(0, 7, 1, "1/" + totalPage.ToString());
            for (int index = 1; index < fromthirdPageTableCount + 1; index++)
            {
                SetTableCellValue(index, 7, 1, (index + 1).ToString() + "/" + totalPage.ToString());
            }
        }

        /// <summary>
        /// 插入地块影像信息
        /// </summary> 
        private void InitalizeLandBookMarkInformation()
        {
            if (geoLandCollection == null || geoLandCollection.Count == 0)
            {
                return;
            }
            int landIndex = geoLandCollection.Count;
            ContractLand land = new ContractLand();

            for (int i = 0; i < landIndex && i < 9; i++)
            {
                switch (i)
                {
                    case 0:
                        land = geoLandCollection[0];
                        InsertImageShapeFirstPage(land, 1, 1, 0);
                        break;
                    case 1:
                        land = geoLandCollection[1];
                        InsertImageShapeFirstPage(land, 1, 2, 0);
                        break;
                    case 2:
                        land = geoLandCollection[2];
                        InsertImageShapeFirstPage(land, 1, 3, 0);
                        break;
                    case 3:
                        land = geoLandCollection[3];
                        InsertImageShapeFirstPage(land, 2, 1, 0);
                        break;
                    case 4:
                        land = geoLandCollection[4];
                        InsertImageShapeFirstPage(land, 2, 2, 0);
                        break;
                    case 5:
                        land = geoLandCollection[5];
                        InsertImageShapeFirstPage(land, 2, 3, 0);
                        break;
                    case 6:
                        land = geoLandCollection[6];
                        InsertImageShapeFirstPage(land, 4, 2, 0);
                        break;
                    case 7:
                        land = geoLandCollection[7];
                        InsertImageShapeFirstPage(land, 4, 3, 0);
                        break;
                    case 8:
                        land = geoLandCollection[8];
                        InsertImageShapeFirstPage(land, 4, 4, 0);
                        break;
                    default:
                        break;
                }
            }
            int pageCount = landIndex / 9;
            pageCount = landIndex % 9 == 0 ? pageCount : (pageCount + 1);

            fromthirdPageTableCount = pageCount + 1;
            int totalPage = fromthirdPageTableCount + 1;//总页数

            for (int i = 1; i < totalPage; i++)
            {
                InitalizeLandBookMarkInformationOther(i * 9, i);
            }
        }

        /// <summary>
        /// 插入其他地块影像信息
        /// </summary> 
        private void InitalizeLandBookMarkInformationOther(int startIndex, int tableIndex)
        {
            if (geoLandCollection == null || geoLandCollection.Count == 0)
            {
                return;
            }
            int landIndex = geoLandCollection.Count;
            ContractLand land = new ContractLand();
            for (int i = 0; i + (tableIndex * 9) < landIndex && i < 9; i++)
            {
                switch (i)
                {
                    case 0:
                        land = geoLandCollection[startIndex + 0];
                        InsertImageShapeFirstPage(land, 1, 1, tableIndex);
                        break;
                    case 1:
                        land = geoLandCollection[startIndex + 01];
                        InsertImageShapeFirstPage(land, 1, 2, tableIndex);
                        break;
                    case 2:
                        land = geoLandCollection[startIndex + 02];
                        InsertImageShapeFirstPage(land, 1, 3, tableIndex);
                        break;
                    case 3:
                        land = geoLandCollection[startIndex + 03];
                        InsertImageShapeFirstPage(land, 2, 1, tableIndex);
                        break;
                    case 4:
                        land = geoLandCollection[startIndex + 04];
                        InsertImageShapeFirstPage(land, 2, 2, tableIndex);
                        break;
                    case 5:
                        land = geoLandCollection[startIndex + 05];
                        InsertImageShapeFirstPage(land, 2, 3, tableIndex);
                        break;
                    case 6:
                        land = geoLandCollection[startIndex + 06];
                        InsertImageShapeFirstPage(land, 4, 2, tableIndex);
                        break;
                    case 7:
                        land = geoLandCollection[startIndex + 07];
                        InsertImageShapeFirstPage(land, 4, 3, tableIndex);
                        break;
                    case 8:
                        land = geoLandCollection[startIndex + 08];
                        InsertImageShapeFirstPage(land, 4, 4, tableIndex);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 插入文件
        /// </summary>
        private void InsertImageShapeFirstPage(ContractLand land, int rowIndex, int columnIndex, int tableIndex = 0)
        {
            try
            {
                var uselandnumber = Regex.Replace(land.LandNumber, @"[^\d]", "_");
                string imagePath = SavePathOfImage + @"\" + uselandnumber + ".jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    if (SettingDefine.IsFixedLandGeoWordExtend)
                    {
                        SetTableCellValue(tableIndex, rowIndex, columnIndex, imagePath, 100, 100, false);
                    }
                    else
                    {
                        SetTableCellValue(tableIndex, rowIndex, columnIndex, imagePath, SettingDefine.LandGeoWordWidth + 30, SettingDefine.LandGeoWordHeight + 30, false);
                    }
                }
                System.IO.File.Delete(imagePath);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

        }
    }
}
