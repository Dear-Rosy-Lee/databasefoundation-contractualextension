/*
 * YuLinTu Entity Library （鱼鳞图实体类库）
 * (C) 2010-2012 鱼鳞图信息技术股份有限公司版权所有，保留所有权利
 * http://www.yulintu.com
 * 
 * 创建人：     郑建（Roc Zheng）
 * 创建时间：   2011年1月17日 14:34:28
 * 版本：       1.0.0
 * 修订历史：
*/
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

namespace YuLinTu.Library.Entity {
    /// <summary>
    /// 鱼鳞图“承包方”相关的共有人。
    /// </summary>
    public abstract class CommonSharePerson : CollectionBase{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CommonPerson this[int index] {
            get { return InnerList[index] as CommonPerson; }
        }

        /// <summary>
        /// 添加一个“共有人”。
        /// </summary>
        /// <param name="person">一个 <see cref="CommonPerson"/> 实例。</param>
        public void Add(CommonPerson person) {
            InnerList.Add(person);
        }

        /// <summary>
        /// 从当前“承包方”中删除指定的“共有人”。
        /// </summary>
        /// <param name="person">一个 <see cref="CommonPerson"/> 实例。</param>
        public void Remove(CommonPerson person) {
            InnerList.Remove(person);
        }

        /// <summary>
        /// 返回描述“承包方”中所有“共有人”信息的 <see cref="XElement"/>。
        /// </summary>
        /// <returns>包含当前“承包方”中所有“共有人”信息的 <see cref="XElement"/> 实例。如果不包含任何“共有人”则返回 <see cref="null"/>。</returns>
        public virtual XElement ToXElement() {
            if (Count < 1)
                return null;

            return new XElement("SharePerson", GenderateSharePersonElements());
        }

        /// <summary>
        /// 从一个 <see cref="XElement"/> 中反序列化“共有人”。
        /// </summary>
        /// <param name="xmlElement">包含“共有人”信息的 <see cref="XElement"/>。</param>
        /// <returns>包含共有人信息的 <see cref="CommonSharePerson"/> 的实例。如果 <paramref name="xmlElement"/> 中不包含信息“共有人”则返回 <see cref="null"/>。</returns>
        protected virtual CommonSharePerson Deserialize(XElement xmlElement) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回共有人信息 XML 格式的字符串。
        /// </summary>
        /// <returns>当前共有人信息的 XML 格式字符串。</returns>
        public override string ToString() {
            XElement el = ToXElement();
            if (el == null)
                return String.Empty;

            return el.ToString();
        }

        protected virtual void InitializeFromXML(string xml) { 
        }

        protected virtual XElement GenerateSharePersonElement(CommonPerson person) {
            if (person == null)
                return null;

            return new XElement("Person", new XAttribute("Name", person.Name));
        }

        private XElement[] GenderateSharePersonElements() {
            if (Count < 1)
                return null;

            List<XElement> elements = new List<XElement>(Count);
            foreach (CommonPerson per in InnerList) {
                if (per == null)
                    continue;

                XElement el = GenerateSharePersonElement(per);
                elements.Add(el);
            }

            return elements.ToArray();
        }
    }
}