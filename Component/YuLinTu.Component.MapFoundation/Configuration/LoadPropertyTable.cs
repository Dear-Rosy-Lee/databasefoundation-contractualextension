using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.MapFoundation.Configuration
{
    /// <summary>
    /// 获取属性表信息
    /// </summary>
     public class LoadPropertyTable : AgricultureWordBook
    {
        #region Ctor

        public LoadPropertyTable()
        {
        }

        #endregion

        #region Override

        public List<ProTable> Read()
        {
            List<ProTable> protable = new List<ProTable>();
            int tablecount = GetTableCount();
            for (int i = 0; i < tablecount; i++)
            {
                ProTable temp = new ProTable();
                temp.TableName = GetTableCellValue(i, 0, 0).Replace("", "");
                List<SetField> fields = new List<SetField>();
                string fieldName = GetTableCellValue(i, 1, 0).Replace("", "");
                int j = 1;
                while(!fieldName.IsNullOrEmpty())
                {
                    SetField field = new SetField();
                    field.FieldName = fieldName;
                    field.AliseName = GetTableCellValue(i, j, 1).Replace("", "");
                    field.IsEdit= BoolValue(GetTableCellValue(i, j, 2));
                    field.IsVisible= BoolValue(GetTableCellValue(i, j, 3));                    
                    fields.Add(field);
                    j++;
                    fieldName = GetTableCellValue(i, j, 0).Replace("", "");
                }
                temp.FieldList = fields;
                protable.Add(temp);
            }
            return protable;
        }
        private bool BoolValue(string text)
        {
            string txt = text.ToLower();
            if (txt.Contains("true"))
                return true;
            return false;
        }       
        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            Contractor = null;
            CurrentZone = null;
            LandCollection = null;
            GC.Collect();
        }

        #endregion
    }
}
