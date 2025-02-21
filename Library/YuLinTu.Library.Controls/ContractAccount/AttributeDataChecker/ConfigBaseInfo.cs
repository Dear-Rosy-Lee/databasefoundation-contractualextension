using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 检查配置项信息
    /// </summary>
    [Serializable]
    public class ConfigBaseInfo : CDObject, INotifyPropertyChanged
    {

        #region Property

        /// <summary>
        /// 检查配置项
        /// </summary>
        public HashSet<TermParamCondition> TermSet { get; set; }

        /// <summary>
        /// 检查参数
        /// </summary>
        public TermCheckArgument TermArg { get; set; }

        #endregion

        #region Ctor

        public ConfigBaseInfo()
        {
            TermArg = new TermCheckArgument();
            TermArg.InitaillArg();
        }

        #endregion

        #region Method

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override object Clone()
        {
            var info = new ConfigBaseInfo();
            if (this.TermArg != null)
            {
                info.TermArg = this.TermArg.Clone() as TermCheckArgument;
            }
            if (this.TermSet != null)
            {
                var hs = new HashSet<TermParamCondition>();
                foreach (var item in this.TermSet)
                {
                    hs.Add(item.Clone() as TermParamCondition);
                }
                info.TermSet = hs;
            }
            return info;
        }

        public override bool Equals(object obj)
        {
            var arg = obj as ConfigBaseInfo;
            if (arg == null)
                return false;
            if ((arg.TermSet == null && this.TermSet != null) ||
                (arg.TermSet != null && this.TermSet == null))
                return false;
            if (arg.TermSet != null && this.TermSet != null)
            {
                
            }
            if (arg.TermSet != null && this.TermSet != null)
            {
                foreach (var item in arg.TermSet)
                {
                    var cTs = this.TermSet.FirstOrDefault(t => t.Name == item.Name);
                    if (cTs == null)
                        return false;
                    if (cTs.Value != item.Value)
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
