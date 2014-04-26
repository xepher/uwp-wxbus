using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Host.Commands
{
    public class DelegateCommand<T> : ICommand where T : class
    {
        Action<T> m_excuteAction;
        Func<T, bool> m_canExcunteFunc;

        public DelegateCommand(Action<T> excuteAction)
        {
            m_excuteAction = excuteAction;
        }
        public DelegateCommand(Action<T> excuteAction, Func<T, bool> canExcuteAction)
        {
            m_canExcunteFunc = canExcuteAction;
            m_excuteAction = excuteAction;
        }

        private bool m_isCanExecute = true;
        public bool IsCanExecute
        {
            get { return m_isCanExecute; }
            set
            {
                if (value != m_isCanExecute)
                {
                    m_isCanExecute = value;
                    FireCanExecuteChanged();
                }

            }
        }

        private void FireCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (m_canExcunteFunc == null)
            {
                return IsCanExecute;
            }
            else
            {
                T args;

                args = parameter as T;

                IsCanExecute = m_canExcunteFunc(args);
            }
            return IsCanExecute;
        }

        public void Execute(object parameter)
        {
            if (m_excuteAction != null)
            {
                T args;

                args = parameter as T;

                m_excuteAction(args);
            }
        }

        #endregion
    }
}
