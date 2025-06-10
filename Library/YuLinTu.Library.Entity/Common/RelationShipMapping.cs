/*
 * (C) 2014-2016 鱼鳞图公司版权所有，保留所有权利
 * http://www.yulintu.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 家庭成员关系
    /// </summary>
    public class RelationShipMapping
    {
        #region public

        /// <summary>
        /// 关系转换
        /// </summary>
        public static string NameMapping(string relationName)
        {
            if (string.IsNullOrEmpty(relationName) || string.IsNullOrEmpty(relationName.Trim()))
            {
                return string.Empty;
            }
            string value = "80";
            if (relationName.Contains("子") && !relationName.Contains("孙"))
            {
                value = ConvertSon(relationName);
            }
            else if (relationName.Contains("女") && !relationName.Contains("孙"))
            {
                value = ConvertDaughter(relationName);
            }
            else if (relationName.Contains("孙") || relationName.Contains("孙"))
            {
                value = ConvertGrandDaughter(relationName);
            }
            else if (relationName.Contains("父") || relationName.Contains("母") || relationName.Contains("公") || relationName.Contains("婆"))
            {
                value = ConvertParent(relationName);
            }
            else if (relationName.Contains("兄") || relationName.Contains("弟") || relationName.Contains("姐") || relationName.Contains("妹"))
            {
                value = ConvertBrothter(relationName);
            }
            else
            {
                value = ConvertOthter(relationName);
            }
            return value;
        }

        /// <summary>
        /// 翻转名称
        /// </summary>
        public static string CodeMapping(string nameCode)
        {
            string name = "其他";
            switch (nameCode)
            {
                case "10":
                    name = "配偶";
                    break;
                case "01":
                    name = "本人";
                    break;

                case "02":
                    name = "户主";
                    break;
                case "11":
                    name = "夫";
                    break;
                case "12":
                    name = "妻";
                    break;

                case "20":
                    name = "子";
                    break;
                case "21":
                    name = "独生子";
                    break;
                case "22":
                    name = "长子";
                    break;
                case "23":
                    name = "次子";
                    break;
                case "24":
                    name = "三子";
                    break;
                case "25":
                    name = "四子";
                    break;
                case "26":
                    name = "五子";
                    break;
                case "29":
                    name = "其他儿子";
                    break;
                case "27":
                    name = "养子或继子";
                    break;
                case "28":
                    name = "女婿";
                    break;
                case "30":
                    name = "女";
                    break;

                case "31":
                    name = "独生女";
                    break;

                case "32":
                    name = "长女";
                    break;

                case "33":
                    name = "次女";
                    break;
                case "34":
                    name = "三女";
                    break;
                case "35":
                    name = "四女";
                    break;
                case "36":
                    name = "五女";
                    break;
  
                case "37":
                    name = "养女或继女";
                    break;

                
                case "38":
                    name = "儿媳";
                    break;
         
                case "39":
                    name = "其他女儿";
                    break;
      
                case "40":
                    name = "孙子、孙女或外孙子、外孙女";
                    break;
                case "41":
                    name = "孙子";
                    break;
                
                case "42":
                    name = "孙女";
                    break;
        
                case "43":
                    name = "外孙子";
                    break;
  
                case "44":
                    name = "外孙女";
                    break;
         
                case "45":
                    name = "孙媳妇或外孙媳妇";
                    break;
     
                case "46":
                    name = "孙女婿或外孙女婿";
                    break;
          
                case "47":
                    name = "曾孙子或外曾孙子";
                    break;
         
                case "48":
                    name = "曾孙女或外曾孙女";
                    break;
       
                case "49":
                    name = "其他孙子、孙女或外孙子、外孙女";
                    break;
    
                case "50":
                    name = "父母";
                    break;
                case "51":
                    name = "父亲";
                    break;
                case "52":
                    name = "母亲";
                    break;
                case "53":
                    name = "公公";
                    break;
                case "54":
                    name = "婆婆";
                    break;
                case "55":
                    name = "岳父";
                    break;
                case "56":
                    name = "岳母";
                    break;

                case "57":
                    name = "继父或养父";
                    break;

                case "58":
                    name = "继母或养母";
                    break;
                case "59":
                    name = "其他父母关系";
                    break;
                case "60":
                    name = "祖父母或外祖父母";
                    break;

                case "61":
                    name = "祖父";
                    break;

                case "62":
                    name = "祖母";
                    break;

                case "63":
                    name = "外祖父";
                    break;

                case "64":
                    name = "外祖母";
                    break;

                case "65":
                    name = "配偶的祖父母或外祖父母";
                    break;
                case "87":
                    name = "姨父";
                    break;
                case "66":
                    name = "曾祖父";
                    break;

                case "67":
                    name = "曾祖母";
                    break;
                case "68":
                    name = "配偶的曾祖父母或外曾祖父母";
                    break;
                case "69":
                    name = "其他祖父母或外祖父母关系";
                    break;
                case "70":
                    name = "兄弟姐妹";
                    break;
                case "7l":
                    name = "兄";
                    break;
                case "71":
                    name = "兄";
                    break;
                case "72":
                    name = "嫂";
                    break;
                case "73":
                    name = "弟";
                    break;
                case "74":
                    name = "弟媳";
                    break;
                case "75":
                    name = "姐姐";
                    break;
                case "76":
                    name = "姐夫";
                    break;
                case "77":
                    name = "妹妹";
                    break;
                case "78":
                    name = "妹夫";
                    break;
                case "79":
                    name = "其他兄弟姐妹";
                    break;
                case "80":
                    name = "其他";
                    break;
                case "81":
                    name = "伯父";
                    break;
                case "82":
                    name = "伯母";
                    break;
                case "83":
                    name = "叔父";
                    break;
                case "84":
                    name = "婶母";
                    break;
                case "85":
                    name = "舅父";
                    break;
                case "86":
                    name = "舅母";
                    break;
                case "88":
                    name = "姨母";
                    break;

                case "89":
                    name = "姑父";
                    break;
           
                case "90":
                    name = "姑母";
                    break;
      
                case "91":
                    name = "堂兄弟、堂姐妹";
                    break;
                case "92":
                    name = "表兄弟、表姐妹";
                    break;
     
                case "93":
                    name = "侄子";
                    break;
             
                case "94":
                    name = "侄女";
                    break;
           
        
                case "95":
                    name = "外甥";
                    break;
             ;
         
                case "96":
                    name = "外甥女";
                    break;
           
                case "97":
                    name = "其他亲属";
                    break;
  
                case "99":
                    name = "非亲属";
                    break;
            }
            return name;
        }

        #endregion

        #region Private

        /// <summary>
        /// 转换子
        /// </summary>
        private static string ConvertSon(string relationName)
        {
            string value = "20";
            switch (relationName)
            {
                case "独生子":
                    value = "21";
                    break;
                case "长子":
                    value = "22";
                    break;
                case "次子":
                    value = "23";
                    break;
                case "三子":
                    value = "24";
                    break;
                case "四子":
                    value = "25";
                    break;
                case "五子":
                    value = "26";
                    break;
                case "养子":
                    value = "27";
                    break;
                case "继子":
                    value = "27";
                    break;
                case "养子或继子":
                    value = "27";
                    break;
                case "其他儿子":
                    value = "29";
                    break;
                case "侄子":
                    value = "93";
                    break;
                case "妻子":
                    value = "12";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 转换女
        /// </summary>
        private static string ConvertDaughter(string relationName)
        {
            string value = "30";
            switch (relationName)
            {
                case "独生女":
                    value = "31";
                    break;
                case "长女":
                    value = "32";
                    break;
                case "次女":
                    value = "33";
                    break;
                case "三女":
                    value = "34";
                    break;
                case "四女":
                    value = "35";
                    break;
                case "五女":
                    value = "36";
                    break;
                case "养女":
                    value = "37";
                    break;
                case "继女":
                    value = "37";
                    break;
                case "养女或继女":
                    value = "37";
                    break;
                case "女婿":
                    value = "28";
                    break;
                case "其他女儿":
                    value = "39";
                    break;
                case "侄女":
                    value = "94";
                    break;
                case "外甥女":
                    value = "96";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 转换孙子女
        /// </summary>
        private static string ConvertGrandDaughter(string relationName)
        {
            string value = "40";
            switch (relationName)
            {
                case "孙子、孙女或外孙子、外孙女":
                    value = "40";
                    break;
                case "孙子":
                    value = "41";
                    break;
                case "孙女":
                    value = "42";
                    break;
                case "外孙子":
                    value = "43";
                    break;
                case "外孙女":
                    value = "44";
                    break;
                case "孙媳妇":
                    value = "45";
                    break;
                case "外孙媳妇":
                    value = "45";
                    break;
                case "孙媳妇或外孙媳妇":
                    value = "45";
                    break;
                case "孙女婿":
                    value = "46";
                    break;
                case "外孙女婿":
                    value = "46";
                    break;
                case "孙女婿或外孙女婿":
                    value = "46";
                    break;
                case "曾孙子":
                    value = "47";
                    break;
                case "外曾孙子":
                    value = "47";
                    break;
                case "曾孙子或外曾孙子":
                    value = "47";
                    break;
                case "曾孙女或外曾孙女":
                    value = "48";
                    break;
                case "其他孙子、孙女或外孙子、外孙女":
                    value = "49";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 转换兄弟姐妹
        /// </summary>
        private static string ConvertBrothter(string relationName)
        {
            string value = "70";
            switch (relationName)
            {
                case "兄":
                    value = "71";
                    break;
                case "弟":
                    value = "73";
                    break;
                case "弟媳":
                    value = "74";
                    break;
                case "姐姐":
                    value = "75";
                    break;
                case "姐夫":
                    value = "76";
                    break;
                case "妹妹":
                    value = "77";
                    break;
                case "妹夫":
                    value = "78";
                    break;
                case "其他兄弟姐妹":
                    value = "79";
                    break;
                case "堂兄弟":
                    value = "91";
                    break;
                case "堂姐妹":
                    value = "91";
                    break;
                case "堂兄弟或堂姐妹":
                    value = "91";
                    break;
                case "堂兄弟、堂姐妹":
                    value = "91";
                    break;
                case "表兄弟":
                    value = "92";
                    break;
                case "表姐妹":
                    value = "92";
                    break;
                case "表兄弟或表姐妹":
                    value = "92";
                    break;
                case "表兄弟、表姐妹":
                    value = "92";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 转换父母
        /// </summary>
        private static string ConvertParent(string relationName)
        {
            string value = "50";
            switch (relationName)
            {
                case "父亲":
                    value = "51";
                    break;
                case "母亲":
                    value = "52";
                    break;
                case "公公":
                    value = "53";
                    break;
                case "婆婆":
                    value = "54";
                    break;
                case "曾祖母":
                    value = "67";
                    break;
                case "伯父":
                    value = "81";
                    break;
                case "伯母":
                    value = "82";
                    break;
                case "叔父":
                    value = "83";
                    break;
                case "婶母":
                    value = "84";
                    break;
                case "舅父":
                    value = "85";
                    break;
                case "舅母":
                    value = "86";
                    break;
                case "父母":
                    value = "50";
                    break;
                case "岳父":
                    value = "55";
                    break;
                case "岳母":
                    value = "56";
                    break;
                case "继父或养父":
                    value = "57";
                    break;
                case "继母或养母":
                    value = "58";
                    break;
                case "其他父母关系":
                    value = "59";
                    break;
                case "祖父母或外祖父母":
                    value = "60";
                    break;
                case "祖父":
                    value = "61";
                    break;
                case "祖母":
                    value = "62";
                    break;
                case "外祖父":
                    value = "63";
                    break;
                case "外祖母":
                    value = "64";
                    break;
                case "配偶的祖父母或外祖父母":
                    value = "65";
                    break;
                case "姨父":
                    value = "87";
                    break;
                case "曾祖父":
                    value = "66";
                    break;
                case "姨母":
                    value = "88";
                    break;
                case "姑父":
                    value = "89";
                    break;
                case "配偶的曾祖父母或外曾祖父母":
                    value = "68";
                    break;
                case "姑母":
                    value = "90";
                    break;
                case "其他祖父母或外祖父母关系":
                    value = "69";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 转换其他
        /// </summary>
        private static string ConvertOthter(string relationName)
        {
            string value = "80";
            switch (relationName)
            {
                case "本人":
                    value = "01";
                    break;
                case "户主":
                    value = "02";
                    break;
                case "配偶":
                    value = "10";
                    break;
                case "夫":
                    value = "11";
                    break;
                case "丈夫":
                    value = "11";
                    break;
                case "妻子":
                    value = "12";
                    break;
                case "妻":
                    value = "12";
                    break;
                case "儿媳":
                    value = "38";
                    break;
                case "嫂":
                    value = "72";
                    break;
                case "伯父":
                    value = "81";
                    break;
                case "伯母":
                    value = "82";
                    break;
                case "叔父":
                    value = "83";
                    break;
                case "婶母":
                    value = "84";
                    break;
                case "舅父":
                    value = "85";
                    break;
                case "舅母":
                    value = "86";
                    break;
                case "其他":
                    value = "80";
                    break;
                case "其他亲属":
                    value = "97";
                    break;
                case "非亲属":
                    value = "99";
                    break;
                case "外甥":
                    value = "95";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 根据家庭成员集合的家庭关系和规范排列家庭成员
        /// </summary>
        /// <param name="persons"></param>
        /// <returns></returns>
        public static List<Person> SortPersonListByRelation(List<Person> persons)
        {
            List<Person> newPersons = new List<Person>();
            List<KeyValue<string, Person>> allPersonDics = new List<KeyValue<string, Person>>();
            foreach (var person in persons)
            {
                var persondic = ConvertToDic(person);
                allPersonDics.Add(persondic);
            }

            allPersonDics.Sort((a, b) =>
            {
                var aindex = Convert.ToInt32(a.Key);
                var bindex = Convert.ToInt32(b.Key);

                return aindex.CompareTo(bindex);
            });

            foreach (var item in allPersonDics)
            {
                newPersons.Add(item.Value);
            }

            return newPersons;
        }
        
        private static KeyValue<string, Person> ConvertToDic(Person person)
        {
            var relationName = person.Relationship;
            KeyValue<string, Person> retdic = new KeyValue<string, Person>();

            var index=  NameMapping(relationName);
            if (index.IsNullOrEmpty()) index = "80";

            retdic.Key = index;
            retdic.Value = person;

            return retdic;
        }

        #endregion
    }
}
