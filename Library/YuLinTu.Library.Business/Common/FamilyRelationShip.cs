/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 家庭关系
    /// </summary>
    public class FamilyRelationShip
    {
        public string Code { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 初始化家庭关系
        /// </summary>
        public static List<string> AllRelation()
        {
            List<string> list = new List<string>()
            {
               "本人",
               "户主",
               "父母",
               "父亲",
               "母亲",
               "配偶",
                "夫",
                "妻",
                "兄弟姐妹",
                "兄",
                "嫂",
                "弟",
                "弟媳",
                "姐姐",
                "姐夫",
                "妹妹",
                "妹夫",
                "其他兄弟姐妹",
                "子",
                "独生子",
                "长子",
                "次子",
                "三子",
                "四子",
                "五子",
                "养子或继子",
                "女婿",
                "其他儿子",
                "女",
                "独生女",
                "长女",
                "次女",
                "三女",
                "四女",
                "五女",
                "养女或继女",
                "儿媳",
                "其他女儿",
                "孙子、孙女或外孙子、外孙女",
                "孙子",
                "孙女",
                "外孙子",
                "外孙女",
                "孙媳妇或外孙媳妇",
                "孙女婿或外孙女婿",
                "曾孙子或外曾孙子",
                "曾孙女或外曾孙女",
                "其他孙子、孙女或外孙子、外孙女",
                "公公",
                "婆婆",
                "岳父",
                "岳母",
                "继父或养父",
                "继母或养母",
                "其他父母关系",
                "祖父母或外祖父母",
                "祖父",
                "祖母",
                "外祖父",
                "外祖母",
                "配偶的祖父母或外祖父母",
                "曾祖父",
                "曾祖母",
                "配偶的曾祖父母或外曾祖父母",
                "其他祖父母或外祖父母关系",
                "其他",
                "伯父",
                "伯母",
                "叔父",
                "婶母",
                "舅父",
                "舅母",
                "姨父",
                "姨母",
                "姑父",
                "姑母",
                "堂兄弟、堂姐妹",
                "表兄弟、表姐妹",
                "侄子",
                "侄女",
                "外甥",
                "外甥女",
                "其他亲属",
                "非亲属"
            };
            return list;
        }

        /// <summary>
        /// 初始化家庭关系
        /// </summary>
        public static List<FamilyRelationShip> AllRelationEn()
        {
            List<FamilyRelationShip> list = new List<FamilyRelationShip>();
            list.Add(new FamilyRelationShip() { Name = "01", Code = "本人" });
            list.Add(new FamilyRelationShip() { Name = "02", Code = "户主" });
            list.Add(new FamilyRelationShip() { Name = "50", Code = "父母" });
            list.Add(new FamilyRelationShip() { Name = "51", Code = "父亲" });
            list.Add(new FamilyRelationShip() { Name = "52", Code = "母亲" });
            list.Add(new FamilyRelationShip() { Name = "10", Code = "配偶" });
            list.Add(new FamilyRelationShip() { Name = "11", Code = "夫" });
            list.Add(new FamilyRelationShip() { Name = "12", Code = "妻" });
            list.Add(new FamilyRelationShip() { Name = "70", Code = "兄弟姐妹" });
            list.Add(new FamilyRelationShip() { Name = "71", Code = "兄" });
            list.Add(new FamilyRelationShip() { Name = "72", Code = "嫂" });
            list.Add(new FamilyRelationShip() { Name = "73", Code = "弟" });
            list.Add(new FamilyRelationShip() { Name = "74", Code = "弟媳" });
            list.Add(new FamilyRelationShip() { Name = "75", Code = "姐姐" });
            list.Add(new FamilyRelationShip() { Name = "76", Code = "姐夫" });
            list.Add(new FamilyRelationShip() { Name = "77", Code = "妹妹" });
            list.Add(new FamilyRelationShip() { Name = "78", Code = "妹夫" });
            list.Add(new FamilyRelationShip() { Name = "79", Code = "其他兄弟姐妹" });
            list.Add(new FamilyRelationShip() { Name = "20", Code = "子" });
            list.Add(new FamilyRelationShip() { Name = "21", Code = "独生子" });
            list.Add(new FamilyRelationShip() { Name = "22", Code = "长子" });
            list.Add(new FamilyRelationShip() { Name = "23", Code = "次子" });
            list.Add(new FamilyRelationShip() { Name = "24", Code = "三子" });
            list.Add(new FamilyRelationShip() { Name = "25", Code = "四子" });
            list.Add(new FamilyRelationShip() { Name = "26", Code = "五子" });
            list.Add(new FamilyRelationShip() { Name = "27", Code = "养子或继子" });
            list.Add(new FamilyRelationShip() { Name = "28", Code = "女婿" });
            list.Add(new FamilyRelationShip() { Name = "29", Code = "其他儿子" });
            list.Add(new FamilyRelationShip() { Name = "30", Code = "女" });
            list.Add(new FamilyRelationShip() { Name = "31", Code = "独生女" });
            list.Add(new FamilyRelationShip() { Name = "32", Code = "长女" });
            list.Add(new FamilyRelationShip() { Name = "33", Code = "次女" });
            list.Add(new FamilyRelationShip() { Name = "34", Code = "三女" });
            list.Add(new FamilyRelationShip() { Name = "35", Code = "四女" });
            list.Add(new FamilyRelationShip() { Name = "36", Code = "五女" });
            list.Add(new FamilyRelationShip() { Name = "37", Code = "养女或继女" });
            list.Add(new FamilyRelationShip() { Name = "38", Code = "儿媳" });
            list.Add(new FamilyRelationShip() { Name = "39", Code = "其他女儿" });
            list.Add(new FamilyRelationShip() { Name = "40", Code = "孙子、孙女或外孙子、外孙女" });
            list.Add(new FamilyRelationShip() { Name = "41", Code = "孙子" });
            list.Add(new FamilyRelationShip() { Name = "42", Code = "孙女" });
            list.Add(new FamilyRelationShip() { Name = "43", Code = "外孙子" });
            list.Add(new FamilyRelationShip() { Name = "44", Code = "外孙女" });
            list.Add(new FamilyRelationShip() { Name = "45", Code = "孙媳妇或外孙媳妇" });
            list.Add(new FamilyRelationShip() { Name = "46", Code = "孙女婿或外孙女婿" });
            list.Add(new FamilyRelationShip() { Name = "47", Code = "曾孙子或外曾孙子" });
            list.Add(new FamilyRelationShip() { Name = "48", Code = "曾孙女或外曾孙女" });
            list.Add(new FamilyRelationShip() { Name = "49", Code = "其他孙子、孙女或外孙子、外孙女" });
            list.Add(new FamilyRelationShip() { Name = "53", Code = "公公" });
            list.Add(new FamilyRelationShip() { Name = "54", Code = "婆婆" });
            list.Add(new FamilyRelationShip() { Name = "55", Code = "岳父" });
            list.Add(new FamilyRelationShip() { Name = "56", Code = "岳母" });
            list.Add(new FamilyRelationShip() { Name = "57", Code = "继父或养父" });
            list.Add(new FamilyRelationShip() { Name = "58", Code = "继母或养母" });
            list.Add(new FamilyRelationShip() { Name = "59", Code = "其他父母关系" });
            list.Add(new FamilyRelationShip() { Name = "60", Code = "祖父母或外祖父母" });
            list.Add(new FamilyRelationShip() { Name = "61", Code = "祖父" });
            list.Add(new FamilyRelationShip() { Name = "62", Code = "祖母" });
            list.Add(new FamilyRelationShip() { Name = "63", Code = "外祖父" });
            list.Add(new FamilyRelationShip() { Name = "64", Code = "外祖母" });
            list.Add(new FamilyRelationShip() { Name = "65", Code = "配偶的祖父母或外祖父母" });
            list.Add(new FamilyRelationShip() { Name = "66", Code = "曾祖父" });
            list.Add(new FamilyRelationShip() { Name = "67", Code = "曾祖母" });
            list.Add(new FamilyRelationShip() { Name = "68", Code = "配偶的曾祖父母或外曾祖父母" });
            list.Add(new FamilyRelationShip() { Name = "69", Code = "其他祖父母或外祖父母关系" });
            list.Add(new FamilyRelationShip() { Name = "80", Code = "其他" });
            list.Add(new FamilyRelationShip() { Name = "81", Code = "伯父" });
            list.Add(new FamilyRelationShip() { Name = "82", Code = "伯母" });
            list.Add(new FamilyRelationShip() { Name = "83", Code = "叔父" });
            list.Add(new FamilyRelationShip() { Name = "84", Code = "婶母" });
            list.Add(new FamilyRelationShip() { Name = "85", Code = "舅父" });
            list.Add(new FamilyRelationShip() { Name = "86", Code = "舅母" });
            list.Add(new FamilyRelationShip() { Name = "87", Code = "姨父" });
            list.Add(new FamilyRelationShip() { Name = "88", Code = "姨母" });
            list.Add(new FamilyRelationShip() { Name = "89", Code = "姑父" });
            list.Add(new FamilyRelationShip() { Name = "90", Code = "姑母" });
            list.Add(new FamilyRelationShip() { Name = "91", Code = "堂兄弟、堂姐妹" });
            list.Add(new FamilyRelationShip() { Name = "92", Code = "表兄弟、表姐妹" });
            list.Add(new FamilyRelationShip() { Name = "93", Code = "侄子" });
            list.Add(new FamilyRelationShip() { Name = "94", Code = "侄女" });
            list.Add(new FamilyRelationShip() { Name = "95", Code = "外甥" });
            list.Add(new FamilyRelationShip() { Name = "96", Code = "外甥女" });
            list.Add(new FamilyRelationShip() { Name = "97", Code = "其他亲属" });
            list.Add(new FamilyRelationShip() { Name = "99", Code = "非亲属" });
            return list;
        }


        /// <summary>
        /// 将已有家庭关系进行排序
        /// </summary>
        /// <param name="targetlist"></param>
        /// <returns></returns>
        public static List<string> AllRelationSort(List<string> targetlist)
        {
            List<string> retlist = new List<string>();

            List<string> list = AllRelation();

            foreach (var item in list)
            {
                var hasitem = targetlist.Find(d => d.Equals(item));
                if (hasitem == null) continue;

                retlist.Add(hasitem);
            }

            return retlist;
        }

        /// <summary>
        /// 将已有家庭关系进行排序
        /// </summary>
        /// <param name="targetlist"></param>
        /// <returns></returns>
        public static List<Person> AllRelationSort(List<Person> targetlist)
        {
            List<Person> retlist = new List<Person>();

            List<string> list = AllRelation();

            foreach (var item in list)
            {
                var hasitem = targetlist.Find(d => d.Relationship.Equals(item));
                if (hasitem == null) continue;

                retlist.Add(hasitem);
            }

            return retlist;
        }
    }
}