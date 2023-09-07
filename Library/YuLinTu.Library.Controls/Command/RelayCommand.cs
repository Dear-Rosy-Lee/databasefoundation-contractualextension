using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace YuLinTu.Library.Controls
{ 
    /// <summary>
    /// 不带参数通用命令类。
    /// </summary>
    public class RelayCommand : System.Windows.Input.ICommand
    {
        //public RelayCommand(Action execute)
        //    : this(execute, null)
        //{
        //}

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            //if (execute == null)
            //    throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        //[DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            if(_execute!=null)
                _execute();
        }

        readonly Action _execute;
        readonly Func<bool> _canExecute;



        //public void Execute(object parameter)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool CanExecute(object parameter)
        //{
        //    throw new NotImplementedException();
        //}

        //public event EventHandler CanExecuteChanged;
    }
}
