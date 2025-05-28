using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
namespace YuLinTu.Library.Business
{
    public static class FamailyRelationProcessor
    {
        /**
         * 比较两个身份证持有者的年龄大小
         * @param idCard1 第一个身份证号码
         * @param idCard2 第二个身份证号码
         * @return 比较结果：
         *          正数 - idCard1持有者更年长
         *          负数 - idCard2持有者更年长
         *          0    - 两人同龄
         * @throws ArgumentException 如果身份证号码格式无效
         */
        public static int CompareAge(string idCard1, string idCard2)
        {
            DateTime birth1 = GetBirthDateFromIdCard(idCard1);
            DateTime birth2 = GetBirthDateFromIdCard(idCard2);
            return DateTime.Compare(birth2, birth1); // 直接返回日期比较结果
        }

        /**
         * 从身份证号码提取出生日期
         * @param idCard 身份证号码（15位或18位）
         * @return 出生日期对象
         * @throws ArgumentException 如果身份证号码格式无效
         */
        private static DateTime GetBirthDateFromIdCard(string idCard)
        {
            // 参数校验
            if (string.IsNullOrWhiteSpace(idCard))
            {
                throw new ArgumentException("身份证号码不能为空");
            }

            string trimmedId = idCard.Trim();

            // 验证长度
            if (trimmedId.Length != 15 && trimmedId.Length != 18)
            {
                throw new ArgumentException("身份证号码长度不正确");
            }

            // 验证字符格式
            if (!Regex.IsMatch(trimmedId, @"^\d{15}$") && !Regex.IsMatch(trimmedId, @"^\d{17}[0-9Xx]$"))
            {
                throw new ArgumentException("身份证包含非法字符");
            }

            // 提取出生日期字符串
            string birthStr;
            if (trimmedId.Length == 15)
            {
                birthStr = "19" + trimmedId.Substring(6, 6); // 15位身份证补19前缀
            }
            else
            {
                birthStr = trimmedId.Substring(6, 8); // 18位身份证直接取8位
            }

            // 解析日期
            try
            {
                return DateTime.ParseExact(birthStr, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new ArgumentException("无效的出生日期");
            }
        }

        /**
         * 根据身份证号码提取性别
         * @param idCard 身份证号码（15位或18位）
         * @return 性别：0 - 女，1 - 男
         * @throws ArgumentException 如果身份证号码格式不正确
         */
        public static int GetGenderFromIdCard(string idCard)
        {
            if (string.IsNullOrWhiteSpace(idCard))
            {
                throw new ArgumentException("身份证号码不能为空");
            }

            idCard = idCard.Trim();

            if (idCard.Length != 15 && idCard.Length != 18)
            {
                throw new ArgumentException("身份证号码长度不正确（应为15位或18位）");
            }

            if (!Regex.IsMatch(idCard, @"^\d{15}$") && !Regex.IsMatch(idCard, @"^\d{17}[0-9Xx]$"))
            {
                throw new ArgumentException("身份证号码包含非法字符");
            }

            string genderDigit;
            if (idCard.Length == 15)
            {
                genderDigit = idCard.Substring(14, 1);
            }
            else
            {
                genderDigit = idCard.Substring(16, 1);
            }

            int genderNum = int.Parse(genderDigit);
            return genderNum % 2;
        }

        /// <summary>
        /// 初始化亲属关系编码映射表
        /// </summary>
        /// <param name="map">目标字典</param>
        /// <remarks>
        /// 定义各个亲属关系对应的编码：
        /// 2-配偶，11/12-养父母，21-29儿子，31-39女儿等
        /// </remarks>
        private static void SetRelationCodeToIntegerMap(Dictionary<RelationCode, int> map)
        {
            // 配偶
            map[new RelationCode(0, 0, 0, 0, 0)] = 2;
            map[new RelationCode(0, 0, 0, 0, 1)] = 2;
            map[new RelationCode(0, 0, 1, 1, 1)] = 11;
            map[new RelationCode(0, 0, 1, 1, 0)] = 12;
            // 儿子
            map[new RelationCode(-1, 0, 0, 0, 1)] = 21;
            map[new RelationCode(-1, 1, 0, 0, 1)] = 22;
            map[new RelationCode(-1, 2, 0, 0, 1)] = 23;
            map[new RelationCode(-1, 3, 0, 0, 1)] = 24;
            map[new RelationCode(-1, 4, 0, 0, 1)] = 25;
            map[new RelationCode(-1, 5, 0, 0, 1)] = 26;
            map[new RelationCode(-1, 6, 0, 1, 1)] = 27;
            map[new RelationCode(-1, 0, 1, 1, 1)] = 28;
            map[new RelationCode(-1, 0, 0, 0, 1)] = 29;
            // 女儿
            map[new RelationCode(-1, 0, 0, 0, 0)] = 31;
            map[new RelationCode(-1, 1, 0, 0, 0)] = 32;
            map[new RelationCode(-1, 2, 0, 0, 0)] = 33;
            map[new RelationCode(-1, 3, 0, 0, 0)] = 34;
            map[new RelationCode(-1, 4, 0, 0, 0)] = 35;
            map[new RelationCode(-1, 5, 0, 0, 0)] = 36;
            map[new RelationCode(-1, 0, 0, 1, 0)] = 37;
            map[new RelationCode(-1, 0, 1, 1, 0)] = 38;
            map[new RelationCode(-1, 0, 0, 0, 0)] = 39;
            // 孙子辈
            map[new RelationCode(-2, 0, 0, 0, 1)] = 41;
            map[new RelationCode(-2, 0, 0, 0, 0)] = 42;
            map[new RelationCode(-2, 0, 1, 0, 0)] = 43;
            map[new RelationCode(-2, 0, 1, 0, 1)] = 44;
            map[new RelationCode(-2, 0, 1, 1, 0)] = 45;
            map[new RelationCode(-2, 0, 1, 1, 1)] = 46;
            // 曾孙子辈
            map[new RelationCode(-3, 0, 0, 0, 1)] = 47;
            map[new RelationCode(-3, 0, 0, 0, 0)] = 48;
            // 父辈
            map[new RelationCode(1, 0, 0, 0, 1)] = 51;
            map[new RelationCode(1, 0, 1, 0, 0)] = 52;
            map[new RelationCode(1, 0, 1, 1, 1)] = 53;
            map[new RelationCode(1, 0, 1, 1, 0)] = 54;
            // 兄弟姐妹
            map[new RelationCode(0, 1, 0, 0, 1)] = 71;
            map[new RelationCode(0, -1, 0, 0, 1)] = 73;
            map[new RelationCode(0, 1, 1, 1, 0)] = 72;
            map[new RelationCode(0, -1, 1, 1, 0)] = 74;
            map[new RelationCode(0, 1, 0, 0, 0)] = 75;
            map[new RelationCode(0, 1, 1, 1, 1)] = 76;
            map[new RelationCode(0, -1, 0, 0, 0)] = 77;
            map[new RelationCode(0, -1, 0, 1, 1)] = 78;
            // 祖父或祖母
            map[new RelationCode(2, 0, 0, 0, 1)] = 61;
            map[new RelationCode(2, 0, 1, 0, 0)] = 62;
            map[new RelationCode(2, 0, 1, 0, 1)] = 63;
        }

        /// <summary>
        /// 创建逆向映射字典（编码->关系对象）
        /// </summary>
        /// <param name="map">原始关系字典</param>
        /// <returns>逆向映射字典</returns>
        private static Dictionary<int, RelationCode> GetReverseMap(Dictionary<RelationCode, int> map)
        {
            Dictionary<int, RelationCode> reverseMap = new Dictionary<int, RelationCode>();
            foreach (KeyValuePair<RelationCode, int> entry in map)
            {
                reverseMap[entry.Value] = entry.Key;
            }
            return reverseMap;
        }

        /// <summary>
        /// 自上而下的亲属关系调整
        /// </summary>
        /// <param name="relationCodeToIntegerMap">关系编码字典</param>
        /// <param name="integerToRelationMap">逆向映射字典</param>
        /// <param name="relationCode">目标关系对象</param>
        /// <param name="members">家庭成员列表</param>
        /// <param name="PersonUnit">当前处理人员</param>
        /// <remarks>
        /// 处理长辈对晚辈的关系调整，包括：
        /// 1. 辈分计算 2. 收养状态 3. 同辈排序
        /// </remarks
        private static void TopToBottom(Dictionary<RelationCode, int> relationCodeToIntegerMap, Dictionary<int, RelationCode> integerToRelationMap, RelationCode relationCode, List<PersonUnit> members, PersonUnit PersonUnit)
        {
            for (int i = 0; i < members.Count; i++)
            {
                int code = int.Parse(members[i].YfzGx);
                if (members[i].YfzGx != null && integerToRelationMap.TryGetValue(code, out RelationCode cur))
                {
                    if (code == 2)
                    {
                        cur.Gender = GetGenderFromIdCard(members[i].Sfz);
                    }
                    if (cur.Equals(relationCode))
                    {
                        members[i].YfzGx = "02";
                    }
                    else
                    {
                        cur.Seniority = cur.Seniority - relationCode.Seniority;
                        if (cur.Seniority == relationCode.Seniority)
                        {
                            cur.Peer = cur.Peer > relationCode.Peer ? 1 : 0;
                        }
                        if (members[i].YfzGx == "11" || members[i].YfzGx == "12")
                        {
                            cur.FosterCare = 0;
                        }
                        if (cur.Seniority == -1 && relationCode.Seniority != 0)
                        {
                            cur.Peer = 0;
                        }
                        members[i].YfzGx = relationCodeToIntegerMap[cur].ToString();
                    }
                }
            }
        }

        // <summary>
        /// 自下而上的亲属关系调整
        /// </summary>
        /// <param name="relationCodeToIntegerMap">关系编码字典</param>
        /// <param name="integerToRelationMap">逆向映射字典</param>
        /// <param name="relationCode">目标关系对象</param>
        /// <param name="members">家庭成员列表</param>
        /// <param name="PersonUnit">当前处理人员</param>
        /// <remarks>
        /// 处理晚辈对长辈的关系调整，包括：

        /// </remarks
        private static void BottomToTop(Dictionary<RelationCode, int> relationCodeToIntegerMap, Dictionary<int, RelationCode> integerToRelationMap, RelationCode relationCode, List<PersonUnit> members, PersonUnit PersonUnit)
        {
            List<PersonUnit> integerMembers = new List<PersonUnit>();
            for (int i = 0; i < members.Count; i++)
            {
                int code = int.Parse(members[i].YfzGx);
                if (members[i].YfzGx != null && integerToRelationMap.TryGetValue(code, out RelationCode cur))
                {
                    if (code == 2)
                    {
                        cur.Gender = GetGenderFromIdCard(members[i].Sfz);
                    }
                    if (cur.Equals(relationCode))
                    {
                        members[i].YfzGx = "02";
                    }
                    else
                    {
                        cur.Seniority = cur.Seniority - relationCode.Seniority;
                        if (cur.Seniority == 1)
                        {
                            cur.Spouse = 1;
                            cur.FosterCare = 1;
                        }
                        if (cur.Seniority == -2)
                        {
                            cur.Seniority = 0;
                        }
                        if (cur.Seniority == 0)
                        {
                            cur.FosterCare = 1;
                            cur.Spouse = 1;
                        }
                        if (cur.Seniority == -1)
                        {
                            integerMembers.Add(members[i]);
                        }
                        else
                        {
                            members[i].YfzGx = relationCodeToIntegerMap[cur].ToString();
                        }
                    }
                }
            }
            integerMembers.Sort((o1, o2) => -CompareAge(o1.Sfz, o2.Sfz));
            int index = 1;
            foreach (PersonUnit each in integerMembers)
            {
                int eachCode = int.Parse(each.YfzGx);
                if (integerToRelationMap.TryGetValue(eachCode, out RelationCode cur))
                {
                    cur.Peer = index;
                    index++;
                    each.YfzGx = relationCodeToIntegerMap[cur].ToString();
                }
            }
        }

        private static void ToSpouse(Dictionary<RelationCode, int> relationCodeToIntegerMap, Dictionary<int, RelationCode> integerToRelationMap, RelationCode relationCode, List<PersonUnit> members, PersonUnit PersonUnit)
        {
            for (int i = 0; i < members.Count; i++)
            {
                int code = int.Parse(members[i].YfzGx);
                if (members[i].YfzGx != null && integerToRelationMap.TryGetValue(code, out RelationCode cur))
                {
                    if (code == 2)
                    {
                        cur.Gender = GetGenderFromIdCard(members[i].Sfz);
                    }
                    if (cur.Equals(relationCode))
                    {
                        members[i].YfzGx = "02";
                    }
                    else
                    {
                        cur.Seniority = cur.Seniority - relationCode.Seniority;
                        if (cur.Seniority == relationCode.Seniority)
                        {
                            cur.Spouse = 1;
                            cur.FosterCare = 1;
                        }
                        if (members[i].YfzGx == "11" || members[i].YfzGx == "12")
                        {
                            cur.FosterCare = 1;
                            cur.Peer = 1;
                        }
                        if (cur.Seniority == 1)
                        {
                            cur.FosterCare = cur.FosterCare == 1 ? 0 : 1;
                            cur.Spouse = cur.FosterCare == 1 ? 0 : 1;
                        }
                        members[i].YfzGx = relationCodeToIntegerMap[cur].ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 计算家庭成员关系
        /// </summary>
        /// <param name="members">家庭成员列表</param>
        /// <param name="PersonUnit">需要计算关系的人员</param>
        /// <returns>
        /// 0-计算失败，1-计算成功
        /// </returns>
        /// <remarks>
        /// 主要处理流程：
        /// 1. 初始化映射表
        /// 2. 根据辈分选择处理方向
        /// 3. 执行具体关系调整
        /// </remarks>
        public static int CalMemberRelationship(List<PersonUnit> members, PersonUnit PersonUnit)
        {
            if (PersonUnit.YfzGx == "28" || PersonUnit.YfzGx == "38")
            {
                return 0;
            }

            Dictionary<RelationCode, int> relationCodeToIntegerMap = new Dictionary<RelationCode, int>();
            SetRelationCodeToIntegerMap(relationCodeToIntegerMap);
            Dictionary<int, RelationCode> integerToRelationMap = GetReverseMap(relationCodeToIntegerMap);

            if (!int.TryParse(PersonUnit.YfzGx, out int PersonUnitCode) || !integerToRelationMap.TryGetValue(PersonUnitCode, out RelationCode relationCode))
            {
                return 0;
            }

            if (relationCode.Seniority >= 2 || relationCode.Seniority <= -2)
            {
                return 0;
            }

            if (relationCode.Seniority > 0)
            {
                BottomToTop(relationCodeToIntegerMap, integerToRelationMap, relationCode, members, PersonUnit);
            }
            else
            {
                if (relationCode.Seniority == 0 && relationCode.Spouse == 1)
                {
                    ToSpouse(relationCodeToIntegerMap, integerToRelationMap, relationCode, members, PersonUnit);
                }
                else
                {
                    TopToBottom(relationCodeToIntegerMap, integerToRelationMap, relationCode, members, PersonUnit);
                }
            }

            return 1;
        }
    }

    public class RelationCode
    {
        /// <summary>辈分差（正-长辈，负-晚辈）</summary>
        public int Seniority { get; set; }
        /// <summary>同辈中的排序</summary>
        public int Peer { get; set; }
        /// <summary>配偶标识（0-否，1-是）</summary>
        public int Spouse { get; set; }
        /// <summary>是否有血缘关系（0-是，1-否）</summary>
        public int FosterCare { get; set; }
        /// <summary>性别标识（0-女，1-男）</summary>
        public int Gender { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="seniority">辈分差（正-长辈，负-晚辈）</param>
        /// <param name="peer">同辈中的排序</param>
        /// <param name="spouse">配偶标识（0-否，1-是）</param>
        /// <param name="fosterCare">是否有血缘关系（0-是，1-否）</param>
        /// <param name="gender">性别标识（0-女，1-男）</param>
        public RelationCode(int seniority, int peer, int spouse, int fosterCare, int gender)
        {
            Seniority = seniority;
            Peer = peer;
            Spouse = spouse;
            FosterCare = fosterCare;
            Gender = gender;
        }
        ///
        public override bool Equals(object obj)
        {
            if (obj is RelationCode other)
            {
                return Seniority == other.Seniority &&
                       Peer == other.Peer &&
                       Spouse == other.Spouse &&
                       FosterCare == other.FosterCare &&
                       Gender == other.Gender;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 19;
            hash = hash * 23 + Seniority.GetHashCode();
            hash = hash * 23 + Peer.GetHashCode();
            hash = hash * 23 + Spouse.GetHashCode();
            hash = hash * 23 + FosterCare.GetHashCode();
            hash = hash * 23 + Gender.GetHashCode();
            return hash;
        }
    }

    public class PersonUnit
    {
        public string Sfz { get; set; }
        public string YfzGx { get; set; }
    }
}