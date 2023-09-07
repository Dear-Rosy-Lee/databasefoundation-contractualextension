/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 此类提供实现字符与数值的转换
    /// </summary>
    public class LetterToNumber
    {
        #region Methods

        /// <summary>
        /// 数值转换字符
        /// </summary>
        /// <param name="number">要转换的数值</param>
        /// <param name="loop">要求字符循环,默认26，即A-Z</param>
        /// <returns> 返回对应的字符，如果返回-1，说明输入的参数值不符合要求 </returns>
        public static string GetCharacterByNumber(int number, int loop = 26)
        {
            string character = null;    //用于记录转换得到的字符            
            if (number <= 0)
            {
                return "-1";
            }
            if (number <= loop)
            {
                character = string.Format("{0}", (char)(64 + number)); //64表示从字符A十进制前一个数值，主要生成字符
            }
            else
            {
                character = PreNumberToCharacter(number, loop);
            }
            return character;
        }
        
        /// <summary>
        /// 求解子问题对应的数值到字符串的转换
        /// 每次通过除数递归得到字符串的前缀字符串
        /// </summary>
        /// <param name="dividend">除数 </param>
        /// <param name="loop">要求字符循环,默认26，即A-Z</param>
        /// <returns>返回</returns>
        private static string PreNumberToCharacter(int dividend, int loop)
        {
            if (dividend <= 0)
            {
                return "-1";
            }
            int temp = (dividend - 1) / loop;
            if (temp < 1)
            {
                string preCharacter = string.Format("{0}", (char)(64 + dividend)); //64表示从字符A十进制前一个数值，主要生成字符
                return preCharacter;
            }
            else
            {
                int mod = dividend - temp * loop; //余数
                string preCharacter = PreNumberToCharacter(temp, loop) + string.Format("{0}", (char)(64 + mod));  //64表示从字符A十进制前一个数值，这里主要得到字符串的最后一个字符串
                return preCharacter;
            }
        }

        /// <summary>
        /// 字符到数值的转换
        /// </summary>
        /// <param name="character">要转的字符串</param>
        /// <param name="loop">要求字符循环,默认26，即A-Z</param>
        /// <returns>返回对应的数值，如果返回的数值是-1，则表明输入的字符串不符合要求</returns>
        public static int GetNumberByCharacter(string character, int loop = 26)
        {
            //先判断字符串的正确性
            if (!IsFormalCharacter(character))  
            {
                return -1;
            }
            character = character.ToUpper(); //字符串小写转换大写
            int number = PreCharacterToNumber(character, loop);
            return number;
        }

        /// <summary>
        /// 子问题 求解字符的前缀的字符对应的数
        /// </summary>
        /// <param name="preCharacter">字符串的前缀，比如AABC，则前缀为AAB；这里没有对preCharacter进行判断，因为之前已经对字符串判断</param>
        /// <param name="loop">要求字符循环,默认26，即A-Z</param>
        /// <returns>返回前缀字符串的对应的数值</returns>
        private static int PreCharacterToNumber(string preCharacter, int loop)
        {
            if (!IsFormalCharacter(preCharacter))
            {
                return -1;
            }
            char[] temp = preCharacter.ToCharArray();   
            string subString = preCharacter.Substring(0, preCharacter.Length - 1);
            if (1 == preCharacter.Length)
            {
                int preIndex = (int)preCharacter[0];
                preIndex = (preIndex - 64) % (loop + 1); //64表示从字符A十进制前一个数值，减64 表示将字符转换成对应的数值
                return preIndex;
            }
            else
            {
                int lastLetter = (int)temp[temp.Length - 1];
                int preIndex = PreCharacterToNumber(subString, loop) * loop + (lastLetter - 64) % (loop + 1);
                return preIndex;
            }
        }

        /// <summary>
        /// 判断输入的字符串是否符合要求
        /// 字符若为空，或者字符串中含有除A-Z或者a-z以外的字符都不符合要求
        /// </summary>
        /// <param name="character">输入的字符串</param>
        /// <returns>bool型值</returns>
        private static bool IsFormalCharacter(string character)
        {
            bool flag = true;
            if (character != null)
            {
                char[] temp = character.ToCharArray();
                for (int i = 0; i < temp.Length; i++)
                {
                    if (!((temp[i] <= 'Z' && temp[i] >= 'A') || (temp[i] <= 'z' && temp[i] >= 'a')))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    flag = true ;
                }
            }
            else
            {
                flag = false;
            }

            return flag;
        }

        #endregion
    }
}
