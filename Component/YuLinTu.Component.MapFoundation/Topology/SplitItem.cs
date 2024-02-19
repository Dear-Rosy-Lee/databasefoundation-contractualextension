using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Component.MapFoundation
{
    public class SplitItem
    {
        #region Properties

        public Graphic Graphic { get; set; }

        public string Text { get; set; }

        public string SurveyNumber { get; set; }

        public Visibility Flag { get; set; }

        #endregion Properties
    }
}