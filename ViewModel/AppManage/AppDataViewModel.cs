using GalaSoft.MvvmLight.Messaging;
using Hammergo.GlobalConfig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DamServiceV3.Test.DamServiceRef;

namespace DamWebAPI.ViewModel.AppManage
{
    public class AppDataViewModel : WorkspaceViewModel
    {
        public AppDataViewModel()
        {
            this.PropertyChanged += AppDataViewModel_PropertyChanged;
            this.DbContext.MergeOption = System.Data.Services.Client.MergeOption.OverwriteChanges;



        }

        public AppDataViewModel(App currentApp)
            : this()
        {
            CurrentApp = currentApp;
        }




        private App _currentApp = null;
        public App CurrentApp
        {
            get { return _currentApp; }
            set
            {
                //if (_currentApp != value)
                //{
                //无论什么 时候都触发事件
                _currentApp = value;
                RaisePropertyChanged("CurrentApp");
                //}
            }
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

        #region RecordNum

        private int _recordNum = PubConstant.ConfigData.LastedRecordNum;
        /// <summary>
        /// 数据的条数
        /// </summary>
        public int RecordNum
        {
            get { return _recordNum; }
            set
            {
                if (_recordNum != value)
                {
                    _recordNum = value;
                }
            }
        }

        #endregion

        #region CmdGetAllData
        private ICommand _cmdGetAllData;

        public ICommand CmdGetAllData
        {
            get
            {
                if (_cmdGetAllData == null)
                {
                    _cmdGetAllData = new RelayCommand(s => this.HandleGetAllData(), CanGetAllData);
                }
                return _cmdGetAllData;
            }
            protected set { _cmdGetAllData = value; }
        }


        private bool CanGetAllData(object obj)
        {
            if (_appInfo != null)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleGetAllData()
        {
            try
            {
                //显示测点的全部数据
                _recordNum = 0;

                FetchData();

                // CurrentApp = app;
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }

        }

        #endregion


        #region CmdDeleteSelectedData
        private ICommand _cmdDeleteSelectedData;

        public ICommand CmdDeleteSelectedData
        {
            get
            {
                if (_cmdDeleteSelectedData == null)
                {
                    _cmdDeleteSelectedData = new RelayCommand(s => this.HandleDeleteSelectedData(), CanDeleteSelectedData);
                }
                return _cmdDeleteSelectedData;
            }
            protected set { _cmdDeleteSelectedData = value; }
        }

        private bool CanDeleteSelectedData(object obj)
        {
            if (_selection.Count > 0)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleDeleteSelectedData()
        {
            var msg = new DialogMessage(string.Format("确定要删选定时间的数据吗?"), result =>
            {
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        var delRows = (from i in Selection
                                       select i.Row).ToList();
                        Selection.Clear();


                        foreach (var row in delRows)
                        {
                            //从appInfo中删除数据
                            var date = (DateTime)(row[PubConstant.timeColumnName]);
                            _appInfo.DeleteValuesByDate(date);

                            //删除构造表中的行
                            AppDataTable.Rows.Remove(row);
                        }
                        AppDataTable.AcceptChanges();

                        _appInfo.Update();
                        //让当前时刻为第一个

                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send<Exception>(ex);
                    }

                }
            });

            msg.Caption = "确定要删除吗?";
            msg.Button = MessageBoxButton.YesNo;
            msg.Icon = MessageBoxImage.Question;

            Messenger.Default.Send<DialogMessage>(msg);

        }

        #endregion


        #region CmdAddData
        private ICommand _cmdAddData;

        public ICommand CmdAddData
        {
            get
            {
                if (_cmdAddData == null)
                {
                    _cmdAddData = new RelayCommand(s => this.HandleAddData(), CanAddData);
                }
                return _cmdAddData;
            }
            protected set { _cmdAddData = value; }
        }

        private bool CanAddData(object obj)
        {
            if (_appInfo != null)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleAddData()
        {

            var msg = new NotificationMessageAction<DateTime>("请输入新增数据的日期及时间", (result) =>
            {

                try
                {
                    //检查是否有数据存在


                    if (DbContext.CheckExistData(_appInfo.CurrentApp.Id, result))
                    {
                        throw new Exception("该时刻的数据已存在，无法添加");

                    }

                    DataRow newRow = AppDataTable.NewRow();
                    newRow[PubConstant.timeColumnName] = result;
                    AppDataTable.Rows.Add(newRow);
                    AppDataTable.AcceptChanges();

                }
                catch (Exception ex)
                {
                    Messenger.Default.Send<Exception>(ex);
                }

            });

            Messenger.Default.Send<NotificationMessageAction<DateTime>>(msg);

        }

        #endregion

        private readonly ObservableCollection<DataRowView> _selection = new ObservableCollection<DataRowView>();
        public ObservableCollection<DataRowView> Selection { get { return this._selection; } }


        private ICommand _cmdAppParams;
        public ICommand CmdAppParams
        {
            get
            {
                if (_cmdAppParams == null)
                {
                    _cmdAppParams = new RelayCommand<App>(param => HandleAppParams(param), CanAppParams);
                }
                return _cmdAppParams;
            }
            protected set { _cmdAppParams = value; }
        }


        private bool CanAppParams(object obj)
        {
            if (_currentApp != null)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleAppParams(App a)
        {

            MainWindowViewModel.Instance.ShowAppParams(_currentApp);

        }


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
            try
            {
                var app = (from i in DbContext.Apps
                           where i.AppName == AppName
                           select i).FirstOrDefault();
                if (app == null)
                {
                    throw new Exception("找不到测点：" + AppName);
                }

                CurrentApp = app;
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }

        }


        protected override void OnDispose()
        {
            this.PropertyChanged -= AppDataViewModel_PropertyChanged;
            base.OnDispose();
        }


        /// <summary>
        /// 参数的小数位数字典
        /// </summary>
        public Dictionary<string, byte> DotNumDic
        {
            get
            {
                return _appInfo.DotNumDic;
            }
        }

        AppIntegratedInfo _appInfo = null;
        void AppDataViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "CurrentApp")
                {
                    if (_currentApp == null)
                    {
                        this.DisplayName = "测点数据";
                    }
                    else
                    {
                        FetchData();
                        this.DisplayName = _currentApp.AppName;
                        this.AppName = _currentApp.AppName;

                    }
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }
        }

        private void FetchData()
        {
            _appInfo = new AppIntegratedInfo(_currentApp, _recordNum, null, null);
            //param已经排序了

            //获取相应的数据

            AppDataTable = _appInfo.ConstructTable();
        }




        private DataTable _appDataTable = null;
        public DataTable AppDataTable
        {
            get { return _appDataTable; }
            set
            {
                if (_appDataTable != value)
                {
                    _appDataTable = value;
                    RaisePropertyChanged("AppDataTable");
                }
            }
        }





        /// <summary>
        /// 处理数据更改后的后台操作
        /// </summary>
        /// <param name="row"></param>
        /// <param name="feildName"></param>
        public void HandleDataRowChanged(DataRow row, string feildName)
        {
            try
            {
                _appInfo.HandleDataRowChanged(row, feildName);

            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }
        }
    }
}
