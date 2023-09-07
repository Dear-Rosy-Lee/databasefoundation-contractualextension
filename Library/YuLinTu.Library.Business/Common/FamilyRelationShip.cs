/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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