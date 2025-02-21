using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using YuLinTu;
using System.Configuration;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 配置管理中心
    /// </summary>
    public class ConfigManagerCentre
    {
        /// <summary>
        /// 创建界面
        /// </summary>
        /// <param name="en">检查配置项</param>
        /// <param name="tc">控件</param>
        public void CreatControlUi(HashSet<TermParamCondition> tpcList, TabControl tc)
        {
            var categoryList = tpcList.Select(t => t.Category).Distinct().ToList();
            for (int i = 0; i < categoryList.Count; i++)
            {
                var itemName = categoryList[i];

                var list = tpcList.Where(t => t.Category == itemName).ToList();

                TabItem ti = new TabItem() { Header = itemName };
                ti.Content = CreateItem(list);
                tc.Items.Insert(i, ti);
            }
        }
        /// <summary>
        /// 创建TableControl的子项
        /// </summary>
        private TabItemControl CreateItem(List<TermParamCondition> aiList)
        {
            Dictionary<TermParamCondition, List<TermParamCondition>> tcList = new Dictionary<TermParamCondition, List<TermParamCondition>>();
            var aiGroup = aiList.GroupBy(t => t.Group).ToList();
            foreach (var item in aiGroup)
            {
                List<TermParamCondition> attList = item.ToList();
                TermParamCondition aiEn = null;
                foreach (var att in attList)
                {
                    if (aiEn == null && !string.IsNullOrEmpty(att.Img))
                        aiEn = att;
                }
                if (aiEn != null)
                {
                    tcList.Add(aiEn, attList);
                }
            }

            TabItemControl tc = new TabItemControl();
            tc.InistallControl(tcList);
            return tc;
        }
        
    }
}
