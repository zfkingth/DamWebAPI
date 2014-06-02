using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Ioc;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using System.Data.Services.Client;
using DamWebAPI.ViewModel.AppManage;
using Odata=DamServiceV3.Test.DamServiceRef;
using DamServiceV3.Test.DamServiceRef;


namespace DamWebAPI.ViewModel
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : WorkspaceViewModel
    {
        #region Fields

        ObservableCollection<WorkspaceViewModel> _workspaces;



        #endregion // Fields

        //single instance mode

        private static MainWindowViewModel _instance;

        public static MainWindowViewModel Instance
        {
            get { return _instance; }
        }

        static MainWindowViewModel()
        {
            _instance = new MainWindowViewModel();
        }

        #region Constructor

        private MainWindowViewModel()
        {
            // base.DisplayName = Strings.MainWindowViewModel_DisplayName;
            //全局程序入口

        }



        #endregion // Constructor

        #region Commands

        private ICommand _cmdShowProjectPart;

        public ICommand CmdShowProjectPart
        {
            get
            {
                if (_cmdShowProjectPart == null)
                {
                    _cmdShowProjectPart = new RelayCommand(param => this.ShowProjectPart());
                }
                return _cmdShowProjectPart;
            }
        }

        private ICommand _cmdSearchApp;

        public ICommand CmdSearchApp
        {
            get
            {
                if (_cmdSearchApp == null)
                {
                    _cmdSearchApp = new RelayCommand(param => this.HandleSearchApp());
                }
                return _cmdSearchApp;
            }
        }

        void HandleSearchApp()
        {
            var displayName = "搜索测点";
            var workspace =
            this.Workspaces.FirstOrDefault(vm =>
            {
                var item = vm as AppSearchViewModel;
                if (item != null && item.DisplayName == displayName)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            ) as AppSearchViewModel;


            if (workspace == null)
            {
                workspace = new AppSearchViewModel();
                this.Workspaces.Add(workspace);
            }



            this.SetActiveWorkspace(workspace);
        }




        private ICommand _cmdAppData;

        public ICommand CmdAppData
        {
            get
            {
                if (_cmdAppData == null)
                {
                    _cmdAppData = new RelayCommand(param => this.HandleAppData());
                }
                return _cmdAppData;
            }
        }

        void HandleAppData()
        {
            var workspace = new AppDataViewModel();
            workspace.DisplayName = "测点数据";
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //测点过程线命令
        private ICommand _cmdAppGraphic;

        public ICommand CmdAppGraphic
        {
            get
            {
                if (_cmdAppGraphic == null)
                {
                    _cmdAppGraphic = new RelayCommand(param => this.HandleAppGraphic());
                }
                return _cmdAppGraphic;
            }
        }

        void HandleAppGraphic()
        {
            var workspace = new Graphics.GraphicsViewModel();
            workspace.DisplayName = "过程线";
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        #endregion // Commands

        #region Workspaces

        /// <summary>
        /// Returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }


        /// <summary>
        /// Raised when this workspace added to workspaces.
        /// </summary>
        public event EventHandler<WorkspaceViewModel> EventWorkspaceAdded;


        /// <summary>
        /// Raised before this workspace removed from workspaces.
        /// </summary>
        public event EventHandler<WorkspaceViewModel> EventBeforeWorkspaceRemove;


        /// <summary>
        /// Raised when this workspace selected.
        /// </summary>
        public event EventHandler<WorkspaceViewModel> EventWorkspaceActived;

        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                {
                    workspace.RequestClose += this.OnWorkspaceRequestClose;
                    if (EventWorkspaceAdded != null)
                    {
                        EventWorkspaceAdded(this, workspace);
                    }
                }

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            if (EventBeforeWorkspaceRemove != null)
            {
                EventBeforeWorkspaceRemove(this, workspace);
            }
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }


        #endregion // Workspaces

        #region Private Helpers

        void CreateNewCustomer()
        {
            //Customer newCustomer = Customer.CreateNewCustomer();
            //CustomerViewModel workspace = new CustomerViewModel(newCustomer, _customerRepository);
            //this.Workspaces.Add(workspace);
            //this.SetActiveWorkspace(workspace);
        }

        void ShowAllCustomers()
        {
            //AllCustomersViewModel workspace =
            //    this.Workspaces.FirstOrDefault(vm => vm is AllCustomersViewModel)
            //    as AllCustomersViewModel;

            //if (workspace == null)
            //{
            //    workspace = new AllCustomersViewModel(_customerRepository);
            //    this.Workspaces.Add(workspace);
            //}

            //this.SetActiveWorkspace(workspace);
        }




        void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));

            //ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            //if (collectionView != null)
            //    collectionView.MoveCurrentTo(workspace);

            if (EventWorkspaceActived != null)
            {
                EventWorkspaceActived(this, workspace);
            }
        }

        #endregion // Private Helpers

        /// <summary>
        /// 显示工程部位管理
        /// </summary>
        void ShowProjectPart()
        {
            var displayName = "测点管理";
            var workspace =
            this.Workspaces.FirstOrDefault(vm =>
            {
                var item = vm as AllAppManageViewModel;
                if (item != null && item.DisplayName == displayName)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            ) as AllAppManageViewModel;


            if (workspace == null)
            {
                workspace = new AllAppManageViewModel();
                this.Workspaces.Add(workspace);
            }

            this.SetActiveWorkspace(workspace);


        }


        internal void ShowCreateApp(Odata.Container dbcontext, Odata.ProjectPart part, DataServiceCollection<Odata.App> currentApps)
        {
            var displayName = "创建新的测点";
            var workspace =
            this.Workspaces.FirstOrDefault(vm =>
            {
                var item = vm as AppManage.CreateAppViewModel;
                if (item != null && item.DisplayName == displayName)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            ) as AppManage.CreateAppViewModel;


            if (workspace == null)
            {
                workspace = new AppManage.CreateAppViewModel(displayName, dbcontext, part, currentApps);
                this.Workspaces.Add(workspace);
            }

            this.SetActiveWorkspace(workspace);
        }

        internal void ShowAppParams(App app)
        {
            var displayName = app.AppName + "参数信息";
            var workspace =
              this.Workspaces.FirstOrDefault(vm =>
              {
                  var item = vm as AppParamsViewModel;
                  if (item != null && item.DisplayName == displayName)
                  {
                      return true;
                  }
                  else
                  {
                      return false;
                  }

              }
              ) as AppParamsViewModel;


            if (workspace == null)
            {
                workspace = new AppParamsViewModel(displayName, app);
                this.Workspaces.Add(workspace);
            }

            this.SetActiveWorkspace(workspace);

        }


        internal void ShowAppData(App app)
        {
            var displayName = app.AppName;
            var workspace =
              this.Workspaces.FirstOrDefault(vm =>
              {
                  var item = vm as AppDataViewModel;
                  if (item != null && item.DisplayName == displayName)
                  {
                      return true;
                  }
                  else
                  {
                      return false;
                  }

              }
              ) as AppDataViewModel;


            if (workspace == null)
            {
                workspace = new AppDataViewModel(app);
                this.Workspaces.Add(workspace);
            }

            this.SetActiveWorkspace(workspace);

        }
    }
}
