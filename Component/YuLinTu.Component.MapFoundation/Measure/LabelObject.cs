/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 * 添加长度标注实体
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Component.MapFoundation
{
    public class LabelObject : NotifyCDObject
    {
        public string LabelText
        {
            get { return _LabelText; }
            set { _LabelText = value; NotifyPropertyChanged("LabelText"); }
        }
        private string _LabelText;

    }
}
