
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DamWebAPI.ViewModel
{
    public class WorkspaceViewModel : ViewModelBase
    {

        #region DisplayName

        private string _displayName;
        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    RaisePropertyChanged("DisplayName");
                }
            }
        }

        #endregion // DisplayName

        #region Fields

        RelayCommand _closeCommand;

        #endregion // Fields

        #region Constructor

        protected WorkspaceViewModel()
        {
        }

        #endregion // Constructor

        #region CloseCommand

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(param => this.OnRequestClose());

                return _closeCommand;
            }
        }

        #endregion // CloseCommand

        #region RequestClose [event]

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region DbContex
        protected DamServiceV3.Test.DamServiceRef.Container _dbContext = null;
        public DamServiceV3.Test.DamServiceRef.Container DbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    _dbContext = new DamServiceV3.Test.DamServiceRef.Container(SimpleIoc.Default.GetInstance<Uri>());
                }
                return _dbContext;
            }
            set
            {
                if(_dbContext!=value)
                {
                    _dbContext = value;
                    RaisePropertyChanged("DbContex");
                }
            }
        }
        #endregion

    }
}
