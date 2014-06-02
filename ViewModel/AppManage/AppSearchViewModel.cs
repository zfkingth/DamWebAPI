using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DamServiceV3.Test.DamServiceRef;

namespace DamWebAPI.ViewModel.AppManage
{
    public class AppSearchViewModel : AllAppManageViewModel
    {
        public AppSearchViewModel()
            : base()
        {
            this.DisplayName = "搜索测点";
        }

        #region AppName

        private string _appName;
        public string AppName
        {
            get { return _appName; }
            set
            {
                if (_appName != value)
                {
                    _appName = value;
                    RaisePropertyChanged("AppName");
                }
            }
        }

        #endregion


        #region CalcName

        private string _calcName;
        public string CalcName
        {
            get { return _calcName; }
            set
            {
                if (_calcName != value)
                {
                    _calcName = value;
                    RaisePropertyChanged("CalcName");
                }
            }
        }

        #endregion

        private ICommand _cmdQueryApp;

        public ICommand CmdQueryApp
        {
            get
            {
                if (_cmdQueryApp == null)
                {
                    _cmdQueryApp = new RelayCommand<string>(param => this.HandleQueryApp(param));
                }
                return _cmdQueryApp;
            }
            protected set { _cmdQueryApp = value; }
        }


        private void HandleQueryApp(string param)
        {

            //has children
            CurrentApps.Clear();

            string match = AppName.Trim().Replace("*", "%");

            //get entity from cache
            IEnumerable<App> dataQuery = DbContext.SearcyAppByName(match);
            CurrentApps.Load(dataQuery);
        }



        private void HandleAppData(App a)
        {

            MainWindowViewModel.Instance.ShowAppData(_selectedApp);

        }


        private ICommand _cmdQueryAppByCalcName;

        public ICommand CmdQueryAppByCalcName
        {
            get
            {
                if (_cmdQueryAppByCalcName == null)
                {
                    _cmdQueryAppByCalcName = new RelayCommand<string>(param => this.HandleQueryAppByCalcName(param));
                }
                return _cmdQueryAppByCalcName;
            }
            protected set { _cmdQueryAppByCalcName = value; }
        }


        private void HandleQueryAppByCalcName(string param)
        {

            //has children
            CurrentApps.Clear();

            string match = CalcName.Trim().Replace("*", "%");

            //get entity from cache
            IEnumerable<App> dataQuery = DbContext.SearcyAppCalcName(match);
            CurrentApps.Load(dataQuery);
        }

    }
}
