// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using System.Text.RegularExpressions;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    ///
    ///集体建设用地使用权
    ///
    [Serializable]
    public class BuildLandPropertyCollection : CDObjectList<BuildLandProperty> 
    {
        #region Methods

        #region Methods - SortToCadastralNumber

        public void SortToCadastralNumber()
        {
            //if (this.Count < 2)
            //    return;
            //SortToCadastralNumber(1);
            //SortToCadastralNumber(2);
            throw new NotImplementedException();
        }

        private void SortToCadastralNumber(int sortType)
        {
            //BuildLandProperty temp = new BuildLandProperty();

            //for (int i = 0; i < this.Count; i++)
            //{
            //    for (int j = i + 1; j < this.Count; j++)
            //    {
            //        switch (sortType)
            //        {
            //            case 1:
            //                if (CompareToCadastralNumberJieFang(this[i].CadastralNumber, this[j].CadastralNumber))
            //                {
            //                    temp = this[i];
            //                    this[i] = this[j];
            //                    this[j] = temp;
            //                }
            //                break;
            //            case 2:
            //                if (CompareToCadastralNumberOrderNumber(this[i].CadastralNumber, this[j].CadastralNumber))
            //                {
            //                    temp = this[i];
            //                    this[i] = this[j];
            //                    this[j] = temp;
            //                }
            //                break;
            //        }
            //    }
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据地籍号中的街坊号进行比较
        /// </summary>
        /// <param name="number"></param>
        /// <param name="number2"></param>
        /// <returns></returns>
        public bool CompareToCadastralNumberJieFang(string number, string number2)
        {
            if (!CheckCadastralNumber(number, number2))
                return false;

            if (int.Parse(SubCadastralNumber(number2, 2)) < int.Parse(SubCadastralNumber(number, 2)))
                return true;

            return false;
        }

        /// <summary>
        /// 根据地籍号的后三位顺序号进行排序
        /// </summary>
        /// <param name="number"></param>
        /// <param name="number2"></param>
        /// <returns></returns>
        public bool CompareToCadastralNumberOrderNumber(string number, string number2)
        {
            if (!CheckCadastralNumber(number, number2))
                return false;

            if (ReturnOrderInInt(SubCadastralNumber(number2, 3)) < ReturnOrderInInt(SubCadastralNumber(number, 3)))
                return true;

            return false;
        }

        /// <summary>
        /// 截取地籍号中的值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strArrayIndex"></param>
        /// <returns></returns>
        private string SubCadastralNumber(string str, int strArrayIndex)
        {
            return str.Split(new string[] { "-" }, StringSplitOptions.None)[strArrayIndex];
        }

        /// <summary>
        /// 验证地籍号是否正确
        /// </summary>
        /// <param name="number"></param>
        /// <param name="number2"></param>
        /// <returns></returns>
        private bool CheckCadastralNumber(string number, string number2)
        {
            if (string.IsNullOrEmpty(number) || string.IsNullOrEmpty(number2))
            {
                return false;
            }
            string pattern = @"(\w|\d]+)-([\w|\d]+)-(\d+)-([\w|\d]+)"; //varchar-varchar-int-varchar格式
            if (!Regex.IsMatch(number, pattern) || !Regex.IsMatch(number2, pattern))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 返回顺序号中的前几位Int值
        /// </summary>
        /// <returns></returns>
        private int ReturnOrderInInt(string number)
        {
            char[] array = number.ToCharArray();
            if (array.Length < 1)
                return -1;
            int index = 0;
            foreach (char item in array)
            {
                if (Regex.Replace(item.ToString(), @"[\d]", "").Length > 0)
                    break;
                index++;
            }
            return int.Parse(number.Substring(0, index));
        }

        #endregion

        #endregion
    }
}