/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Appwork;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 插件入口
    /// </summary>
    internal class ApplicationContext : TheApplicationContext
    {
        public ApplicationContext()
        {

        }
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
            
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);

            //Application.Current.Resources.MergedDictionaries.Add(
            //    new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Component.MapFoundation;component/Resources/Symbols.xaml") });
          
        }
    }
}
