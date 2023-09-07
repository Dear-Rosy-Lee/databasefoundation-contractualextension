/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using System.Collections;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 读取发包方信息
    /// </summary>
    [Serializable]
    public class ExcelReaderSender : ExcelBase
    {
        #region Fileds

        private int currentIndex;//当前索引值

        #endregion

        #region Propertys

        /// <summary>
        /// 错误集合
        /// </summary>
        public List<string> ErrorCollection { get; set; }

        /// <summary>
        /// 警告集合
        /// </summary>
        public List<string> WarnCollection { get; set; }

        /// <summary>
        /// 发包方集合
        /// </summary>
        public List<CollectivityTissue> TissueCollection { get; set; }

        #endregion

        #region Ctor

        public ExcelReaderSender()
        {
            TissueCollection = new List<CollectivityTissue>();
            ErrorCollection = new List<string>();
            WarnCollection = new List<string>();
        }

        #endregion

        #region

        /// <summary>
        /// 读取表格信息
        /// </summary>
        /// <returns></returns>
        public bool ReadInformation()
        {
            object[,] allItem = GetAllRangeValue();//获取所有使用域值
            if (allItem == null)
            {
                return false;
            }
            string information = "";
            int colCount = GetRangeColumnCount();
            if (colCount < 14)
            {
                information = "表中数据调查格式与调查表格式不符,列数不足!";
                AddErrorInformation(information);
                return false;
            }
            int rowCount = GetRangeRowCount();
            int calIndex = InitalizeStartRow(rowCount, allItem);//获取数据开始行数
            string landNumber = string.Empty;
            for (int index = calIndex; index < rowCount; index++)
            {
                currentIndex = index;//当前行数
                string rowValue = GetString(allItem[currentIndex, 0]);
                if (rowValue == "合计" || rowValue == "总计")
                {
                    break;
                }
                InitalizeTissueInformation(allItem);
            }
            allItem = null;
            rowCount = 0;
            currentIndex = 0;
            GC.Collect();
            return ErrorCollection.Count == 0;
        }

        /// <summary>
        /// 初始化发包方信息
        /// </summary>
        private void InitalizeTissueInformation(object[,] allItem)
        {
            CollectivityTissue tissue = InitalizeTissue(allItem);
            bool canContinue = AddTissueErrorInformation(tissue, allItem);
            if (!canContinue)
            {
                return;
            }
            if (TissueCollection.Find(te => te.Code == tissue.Code) == null)
            {
                TissueCollection.Add(tissue);
            }
            else
            {
                AddErrorInformation(string.Format("表中第{0}行发包方编码重复存在!", currentIndex + 1));
            }
        }

        /// <summary>
        /// 初始化集体经济组织
        /// </summary>
        /// <param name="allItem"></param>
        /// <returns></returns>
        private CollectivityTissue InitalizeTissue(object[,] allItem)
        {
            CollectivityTissue tissue = new CollectivityTissue();
            try
            {
                tissue.Code = GetString(allItem[currentIndex, 0]);
                tissue.Name = GetString(allItem[currentIndex, 1]);
                tissue.LawyerName = GetString(allItem[currentIndex, 2]);
                tissue.LawyerTelephone = GetString(allItem[currentIndex, 3]);
                tissue.LawyerAddress = GetString(allItem[currentIndex, 4]);
                tissue.LawyerPosterNumber = GetString(allItem[currentIndex, 5]);
                tissue.LawyerCartNumber = GetString(allItem[currentIndex, 7]);
                tissue.SurveyPerson = GetString(allItem[currentIndex, 8]);
                tissue.SurveyDate = GetDateTime(allItem[currentIndex, 9]);
                tissue.SurveyChronicle = GetString(allItem[currentIndex, 10]);
                tissue.CheckPerson = GetString(allItem[currentIndex, 11]);
                tissue.CheckDate = GetDateTime(allItem[currentIndex, 12]);
                tissue.CheckOpinion = GetString(allItem[currentIndex, 13]);
                tissue.Comment = GetString(allItem[currentIndex, 14]);
                tissue.Status = eStatus.Register;//状态
                tissue.Type = eTissueType.General;//集体经济组织类型
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return tissue;
        }


        /// <summary>
        /// 添加发包方错误信息
        /// </summary>
        /// <param name="tissue"></param>
        private bool AddTissueErrorInformation(CollectivityTissue tissue, object[,] allItem)
        {
            if (tissue == null)
                return false;
            bool canContinue = true;
            if (string.IsNullOrEmpty(tissue.Code))
            {
                AddWarnInformation(string.Format("表中第{0}行发包方编码为空!", currentIndex + 1));
            }
            else
            {
                if (!ToolMath.MatchEntiretyNumber(tissue.Code) || tissue.Code.Length != 14)
                {
                    AddWarnInformation(string.Format("表中第{0}行发包方编码应该为14位的数字类型!", currentIndex + 1));
                }
            }
            if (string.IsNullOrEmpty(tissue.LawyerName))
            {
                AddWarnInformation(string.Format("表中第{0}行发包方负责人姓名为空!", currentIndex + 1));
            }
            if (string.IsNullOrEmpty(tissue.LawyerTelephone))
            {
                AddWarnInformation(string.Format("表中第{0}行发包方负责人联系电话为空!", currentIndex + 1));
            }
            else
            {
                if (!ToolMath.MatchEntiretyNumber(tissue.LawyerTelephone))
                {
                    AddWarnInformation(string.Format("表中第{0}行发包方负责人联系电话只能填写数字类型!", currentIndex + 1));
                }
            }
            if (string.IsNullOrEmpty(tissue.LawyerAddress))
            {
                AddWarnInformation(string.Format("表中第{0}行发包方负责人地址为空!", currentIndex + 1));
            }
            if (string.IsNullOrEmpty(tissue.LawyerPosterNumber))
            {
                AddWarnInformation(string.Format("表中第{0}行邮政编码为空!", currentIndex + 1));
            }
            else
            {
                if (!ToolMath.MatchEntiretyNumber(tissue.LawyerPosterNumber) || tissue.LawyerPosterNumber.Length != 6)
                {
                    AddWarnInformation(string.Format("表中第{0}行邮政编码只能是6位的数字类型!", currentIndex + 1));
                }
            }
            string value = GetString(allItem[currentIndex, 6]);
            if (string.IsNullOrEmpty(value))
            {
                AddWarnInformation(string.Format("表中第{0}行发包方负责人证件类型为空!", currentIndex + 1));
            }
            else
            {
                try
                {
                    tissue.LawyerCredentType = (eCredentialsType)EnumNameAttribute.GetValue(typeof(eCredentialsType), value);
                }
                catch
                {
                    AddWarnInformation(string.Format("表中第{0}行发包方负责人证件类型不符合证件类型填写要求!", currentIndex + 1));
                }
            }
            if (string.IsNullOrEmpty(tissue.LawyerCartNumber))
            {
                AddWarnInformation(string.Format("表中第{0}行发包方负责人证件号码为空!", currentIndex + 1));
            }
            else
            {
                if (tissue.LawyerCredentType == eCredentialsType.IdentifyCard && (!ToolMath.MatchEntiretyNumber(tissue.LawyerCartNumber.Replace("X", "").Replace("x", "")) || (tissue.LawyerCartNumber.Length != 15
                    && tissue.LawyerCartNumber.Length != 18)))
                {
                    AddWarnInformation(string.Format("表中第{0}行证件号码只能是15位或18位的数字类型!", currentIndex + 1));
                }
            }
            if (string.IsNullOrEmpty(tissue.SurveyPerson))
            {
                AddWarnInformation(string.Format("表中第{0}行调查员为空!", currentIndex + 1));
            }
            if (string.IsNullOrEmpty(tissue.SurveyChronicle))
            {
                AddWarnInformation(string.Format("表中第{0}行调查记事为空!", currentIndex + 1));
            }
            if (tissue.SurveyDate == null || !tissue.SurveyDate.HasValue)
            {
                AddWarnInformation(string.Format("表中第{0}行调查日期不符合日期填写要求!", currentIndex + 1));
            }
            if (string.IsNullOrEmpty(tissue.CheckPerson))
            {
                AddWarnInformation(string.Format("表中第{0}行审核人为空!", currentIndex + 1));
            }
            if (string.IsNullOrEmpty(tissue.CheckOpinion))
            {
                AddWarnInformation(string.Format("表中第{0}行审核意见为空!", currentIndex + 1));
            }
            if (tissue.CheckDate == null || !tissue.CheckDate.HasValue)
            {
                AddWarnInformation(string.Format("表中第{0}行审核日期不符合日期填写要求!", currentIndex + 1));
            }
            if (tissue.SurveyDate != null && tissue.SurveyDate.HasValue && tissue.CheckDate != null && tissue.CheckDate.HasValue
                && tissue.SurveyDate.Value > tissue.CheckDate.Value)
            {
                AddErrorInformation(string.Format("表中第{0}行审核日期不能小于调查日期!", currentIndex + 1));
            }
            if (string.IsNullOrEmpty(tissue.Name))
            {
                AddWarnInformation(string.Format("表中第{0}行发包方名称为空!", currentIndex + 1));
                canContinue = false;
            }
            return canContinue;
        }

        /// <summary>
        /// 获取开始行数据
        /// </summary>
        /// <param name="range">值范围</param>
        /// <param name="allItem">所有值</param>
        /// <returns></returns>
        private int InitalizeStartRow(int rowCount, object[,] allItem)
        {
            int startIndex = 0;
            for (int i = 0; i < rowCount; i++)
            {
                string rowValue = GetString(allItem[i, 0]);//编号栏数据
                if (!string.IsNullOrEmpty(rowValue) && ToolMath.MatchEntiretyNumber(rowValue))
                {
                    startIndex = i;
                    break;
                }
            }
            return startIndex;
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="information"></param>
        private void AddErrorInformation(string information)
        {
            if (ErrorCollection.Contains(information))
                return;
            ErrorCollection.Add(information);
        }

        /// <summary>
        /// 添加警告信息
        /// </summary>
        private void AddWarnInformation(string information)
        {
            if (string.IsNullOrEmpty(information))
                return;
            WarnCollection.Add(information);
        }

        /// <summary>
        /// 获取单元格数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public string GetValue(string start, string end)
        {
            object[,] value = GetRangeToValue(start, end) as object[,];
            if (value == null || value[1, 1] == null)
            {
                return "";
            }
            string cellValue = value[1, 1].ToString();
            value = null;
            return cellValue;
        }

        #endregion

        #region Override

        /// <summary>
        /// 读取数据
        /// </summary>
        public override void Read()
        {

        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {

        }

        #endregion
    }
}
