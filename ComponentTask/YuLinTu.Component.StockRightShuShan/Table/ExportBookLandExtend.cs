using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.IO;
using YuLinTu.Library.Business;
using YuLinTu.Data;

namespace YuLinTu.Component.StockRightShuShan
{
    public class ExportBookLandExtend : ExportExcelBase
    {
        public VirtualPerson VirtualPerson { get; set; }
        private int _startIndex = 3;

        public List<ContractLand> LandCollection { get; set; }

        public Zone CurrentZone { get; set; }

        public IDbContext DbContext { get; set; }


        /// <summary>
        /// 处理地块扩展数据
        /// </summary>
        /// <param name="filePath"></param>
        public void ProcessLandExtend(string filePath)
        {
            if (!InitalizeAgriLandTemplateFilePath())//获取模板文件路径
            {
                MessageBox.Show("打印模板不存在共有人模版信息或模板错误!", "获取打印模板", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {

                SetContractLandValue();
                if (!string.IsNullOrEmpty(filePath))
                {
                    string file = filePath;
                    if (!Directory.Exists(file))
                    {
                        Directory.CreateDirectory(file);
                    }
                    file += @"\";
                    file += VirtualPerson.Name;
                    file += "-农村土地承包经营权证地块扩展.xls";
                    if (File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                    SaveAs(file);
                    Dispose();
                    return;
                }
                Show();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// 设置地块信息
        /// </summary>
        /// <param name="dt"></param>
        private void SetContractLandValue()
        {
            SetAgriLandHeadText();
            SetLineType("A1", "F" + (LandCollection.Count * 4 + 2));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void SetAgriLandHeadText()
        {
            foreach (var item in LandCollection)
            {
                var quaArea = DataHelper.GetQuantificationArea(VirtualPerson, item, CurrentZone, DbContext);
                SetRange("A" + _startIndex, "A" + (_startIndex + 3), 19.50, 10, false, item.Name);
                SetRange("B" + _startIndex, "B" + (_startIndex + 3), 19.50, 10, false, item.LandNumber);
                SetRange("C" + _startIndex, "C" + (_startIndex + 3), 19.50, 10, false, item.ShareArea);
                if (item.IsFarmerLand.HasValue)
                    SetRange("D" + _startIndex, "D" + (_startIndex + 3), 19.50, 10, false, (bool)item.IsFarmerLand ? "是" : "否");
                SetRange("E" + _startIndex, "E" + (_startIndex), 19.50, 10, false, 1, 2, "东:" + item.NeighborEast);//四至东
                SetRange("E" + (_startIndex + 1), "E" + (_startIndex + 1), 19.50, 10, false, 1, 2, "西:" + item.NeighborWest);//四至西
                SetRange("E" + (_startIndex + 2), "E" + (_startIndex + 2), 19.50, 10, false, 1, 2, "南:" + item.NeighborSouth);//四至南
                SetRange("E" + (_startIndex + 3), "E" + (_startIndex + 3), 19.50, 10, false, 1, 2, "北:" + item.NeighborNorth);//四至北
                SetRange("F" + _startIndex, "F" + (_startIndex + 3), 19.50, 10, false, quaArea.AreaFormat(2));
                _startIndex += 4;
            }
        }




        /// <summary>
        /// 获取文件路径
        /// </summary>
        private bool InitalizeAgriLandTemplateFilePath()
        {
            string filePath = TemplateHelper.ExcelTemplate("安徽蜀山权证地块扩展");
            if (!File.Exists(filePath))//判断文件是否存在
            {
                return false;
            }
            if (!Open(filePath))//打开文件
            {
                return false;
            }
            return true;
        }
    }
}
