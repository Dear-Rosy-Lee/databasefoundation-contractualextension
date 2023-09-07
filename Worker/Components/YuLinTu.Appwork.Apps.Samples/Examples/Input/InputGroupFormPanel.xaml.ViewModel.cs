using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.Input
{
    [View(typeof(InputGroupFormPanel))]
    public partial class InputGroupFormPanelViewModel : ViewModelObject
    {
        #region Properties

        #region Properties - Common

        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(() => Description); }
        }
        private string _Description;

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

        #endregion

        #endregion

        #region Commands

        #region Commands - Confirm

        public DelegateCommand CommandConfirm { get { return _CommandConfirm ?? (_CommandConfirm = new DelegateCommand(args => OnConfirm(args), args => OnCanConfirm(args))); } }
        private DelegateCommand _CommandConfirm;

        private bool OnCanConfirm(object args)
        {
            return true;
        }

        private void OnConfirm(object args)
        {
            var root = args as Grid;
            if (root == null)
                throw new ArgumentNullException("args");

            var hasError = root.Validate();
            var errors = root.GetErrors();

            Description = hasError ? $"表单中有 {errors.Length} 个错误" : "验证通过";
        }

        #endregion

        #region Commands - Cancel

        public DelegateCommand CommandCancel { get { return _CommandCancel ?? (_CommandCancel = new DelegateCommand(args => OnCancel(args), args => OnCanCancel(args))); } }
        private DelegateCommand _CommandCancel;

        private bool OnCanCancel(object args)
        {
            return true;
        }

        private void OnCancel(object args)
        {
            Description = null;
            Person = new FormValidationPerson();
        }

        #endregion


        #endregion

        #region Ctor

        public InputGroupFormPanelViewModel()
        {
        }

        #endregion

        #region Methods

        #region Methods - Public

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

        #endregion

        #endregion
    }
}
