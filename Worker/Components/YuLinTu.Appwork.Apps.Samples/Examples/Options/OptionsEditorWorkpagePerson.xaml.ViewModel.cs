using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [View(typeof(OptionsEditorWorkpagePerson))]
    public partial class OptionsEditorWorkpagePersonViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description = "查看和编辑负责人信息";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        public double TitleWidth
        {
            get { return _TitleWidth; }
            set { _TitleWidth = value; NotifyPropertyChanged(() => TitleWidth); }
        }
        private double _TitleWidth = 150;


        public FormValidationPerson Person
        {
            get { return _Person; }
            set { _Person = value; NotifyPropertyChanged(() => Person); }
        }
        private FormValidationPerson _Person = new FormValidationPerson();

        public EnumNameAttribute[] GenderSource
        {
            get { return _GenderSource; }
            set { _GenderSource = value; NotifyPropertyChanged(() => GenderSource); }
        }
        private EnumNameAttribute[] _GenderSource = EnumNameAttribute.GetAttributes(typeof(eGender));

        #endregion

        #region Properties - System

        public IWorkpage Workpage { get; private set; }

        #endregion

        #endregion

        #region Commands

        #endregion

        #region Ctor

        public OptionsEditorWorkpagePersonViewModel(IWorkpage workpage)
        {
            Workpage = workpage;
        }

        #endregion

        #region Methods

        #region Methods - Public

        internal void Load()
        {
            var center = Workpage.Workspace.GetUserSettingsProfileCenter();
            var profile = center.GetProfile<FormValidationPerson>();
            var section = profile.GetSection<FormValidationPerson>();
            Person = section.Settings.Clone() as FormValidationPerson;
        }

        internal void Save()
        {
            var center = Workpage.Workspace.GetUserSettingsProfileCenter();
            var profile = center.GetProfile<FormValidationPerson>();
            var section = profile.GetSection<FormValidationPerson>();
            section.Settings = Person;
            center.Save<FormValidationPerson>();
        }

        #endregion

        #region Methods - Override

        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        #endregion

        #region Methods - Private

        #endregion

        #region Methods - System

        public void Dispose()
        {
            Workpage = null;
        }

        #endregion

        #endregion
    }
}
