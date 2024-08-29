/*
 * YuLinTu Entity Library （鱼鳞图实体类库）
 * (C) 2010-2012 鱼鳞图信息技术股份有限公司版权所有，保留所有权利
 * http://www.yulintu.com
 * 
 * 创建人：     郑建（Roc Zheng）
 * 创建时间：   2011年1月17日 14:25:15
 * 版本：       1.0.0
 * 修订历史：
*/
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 包含基本信息的“人”，在鱼鳞图系统中作为与地相关承包方和基类。
    /// </summary>
    [Serializable]
    public abstract class CommonPerson : NameableObject  //(修改前)YltEntityIDName
    {
    }
}
