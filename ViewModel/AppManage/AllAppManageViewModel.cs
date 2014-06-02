using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using GalaSoft.MvvmLight.Ioc;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Collections;
using DamServiceV3.Test.DamServiceRef;

namespace DamWebAPI.ViewModel.AppManage
{
    public class AllAppManageViewModel : WorkspaceViewModel
    {

        public AllAppManageViewModel()
        {
            DisplayName = "测点管理";
            DbContext.MergeOption = System.Data.Services.Client.MergeOption.OverwriteChanges;
        }



        /// <summary>
        /// 测点类型数据
        /// </summary>
        DataServiceCollection<ApparatusType> _appTypes = null;
        public ICollection<ApparatusType> AppTypes
        {
            get
            {
                if (_appTypes == null)
                {
                    var query = from i in DbContext.ApparatusTypes
                                select i;
                    DataServiceCollection<ApparatusType> types = new DataServiceCollection<ApparatusType>();
                    types.Load(query);

                    _appTypes = types;
                }
                return _appTypes;
            }
        }

        #region Commands

        private ICommand _refreshAppsCmd;

        public ICommand CmdRefreshAppsCmd
        {
            get
            {
                if (_refreshAppsCmd == null)
                {
                    _refreshAppsCmd = new RelayCommand<ProjectPartViewModel>(model => this.HandleRefreshApps(model));
                }
                return _refreshAppsCmd;
            }
            protected set { _refreshAppsCmd = value; }
        }

        //测试命令


        /// <summary>
        /// 移动部位的命令
        /// </summary>
        private ICommand _cmdMovePart;
        public ICommand CmdMovePart
        {
            get
            {
                if (_cmdMovePart == null)
                {
                    _cmdMovePart = new RelayCommand(param => HandleMovePart(param), CanMovePart);
                }
                return _cmdMovePart;
            }
            protected set { _cmdMovePart = value; }
        }


        private bool CanMovePart(object obj)
        {
            return true;
        }

        private void HandleMovePart(object a)
        {


            var args = a as object[];
            var sourceModel = args[0] as ProjectPartViewModel;
            var targetModel = args[1] as ProjectPartViewModel;

            try
            {
                if (sourceModel.ParentViewModel == null)
                {
                    throw new Exception("根结点无法移动!");

                }

                if (sourceModel.ParentViewModel == targetModel)
                {
                    throw new Exception("已在该节点下");
                }

                var cnt = (from i in DbContext.Apps
                           where i.ProjectPartID == targetModel.ProjectPartID
                           select i).Count();
                if (cnt != 0)
                {
                    throw new Exception("目标节点已有测点与之关联，无法在其下添加新的节点");
                }
                else
                {
                    //在数据库中保存

                    sourceModel.Entity.ParentPart = targetModel.Entity.Id;

                    DbContext.UpdateObject(sourceModel.Entity);

                    DbContext.SaveChanges();

                    //sourceModel.ParentViewModel.Children = GetChildren(sourceModel.ParentViewModel);
                    //targetModel.Children = GetChildren(targetModel);


                    //直接刷新


                    //从源节点中移除
                    sourceModel.ParentViewModel.Children.Remove(sourceModel);

                    //从新节点中添加
                    sourceModel.ParentViewModel = targetModel;
                    targetModel.Children.Add(sourceModel);



                }
            }
            catch (Exception ex)
            {




                Messenger.Default.Send(ex);
            }
            // targetModel
            // Messenger.Default.Send<Exception>(new Exception("Hello world!"));
        }


        /// <summary>
        /// 行更新命令
        /// </summary>
        private ICommand _cmdRowUpdated;
        public ICommand CmdRowUpdated
        {
            get
            {
                if (_cmdRowUpdated == null)
                {
                    _cmdRowUpdated = new RelayCommand<App>(param => HandleUpdate(param));
                }
                return _cmdRowUpdated;
            }
            protected set { _cmdRowUpdated = value; }
        }

        private void HandleUpdate(App app)
        {
            try
            {
                DbContext.SaveChanges(SaveChangesOptions.Batch);
            }
            catch (Exception ex)
            {
                int index = CurrentApps.IndexOf(app);
                CurrentApps.RemoveAt(index);
                DbContext.Detach(app);

                var query = from i in DbContext.Apps
                            where i.AppName == app.AppName
                            select i;
                //DataServiceCollection<App> col = new DataServiceCollection<App>();
                //col.Load(query);
                CurrentApps.Insert(index, query.First());
                //App appIndb= DbContext.Apps.Single(item => item.AppName == app.AppName);
                //CurrentApps.Insert(index,appIndb);
                RaisePropertyChanged("CurrentApps");
                Messenger.Default.Send<Exception>(ex);
            }
        }

        /// <summary>
        /// 重命名部位
        /// </summary>
        private ICommand _cmdRenamePart;
        public ICommand CmdRenamePart
        {
            get
            {
                if (_cmdRenamePart == null)
                {
                    _cmdRenamePart = new RelayCommand<ProjectPartViewModel>(param => HandleRenamePart(param), CanRenamePart);
                }
                return _cmdRenamePart;
            }
            protected set { _cmdRenamePart = value; }
        }

        private bool CanRenamePart(ProjectPartViewModel obj)
        {
            if (_currentModel == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void HandleRenamePart(ProjectPartViewModel model)
        {
            model = _currentModel;

            var msg = new NotificationMessageAction<string>("请输入新的名称", (result) =>
            {
                if (result != null && result.Trim().Length != 0)
                {
                    try
                    {
                        model.PartName = result.Trim();
                        DbContext.UpdateObject(model.Entity);
                        DbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send<Exception>(ex);
                    }

                }
            });

            Messenger.Default.Send<NotificationMessageAction<string>>(msg);


        }

        /// <summary>
        /// 删除部位命令
        /// </summary>
        private ICommand _cmdDeletePart;
        public ICommand CmdDeletePart
        {
            get
            {
                if (_cmdDeletePart == null)
                {
                    _cmdDeletePart = new RelayCommand<ProjectPartViewModel>(param => HandleDeletePart(param), CanDeletePart);
                }
                return _cmdDeletePart;
            }
            protected set { _cmdDeletePart = value; }
        }

        private bool CanDeletePart(ProjectPartViewModel obj)
        {
            //不能是空结点
            //不能为根结点
            //不能有子结点
            //不能有测点与之关联
            if (_currentModel != null && _currentModel.ParentViewModel != null && _currentModel.Children.Count == 0 && CurrentApps.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void HandleDeletePart(ProjectPartViewModel model)
        {
            model = _currentModel;

            var msg = new DialogMessage(string.Format("确定要删除部位：{0}吗?", model.PartName), result =>
            {
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {

                        DbContext.DeleteObject(model.Entity);
                        DbContext.SaveChanges();

                        //从视图中删除
                        model.ParentViewModel.Children.Remove(model);
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


        private ICommand _cmdAttachApp;
        public ICommand CmdAttachApp
        {
            get
            {
                if (_cmdAttachApp == null)
                {
                    _cmdAttachApp = new RelayCommand<ProjectPartViewModel>(param => HandleAttachApp(param), CanAttachApp);
                }
                return _cmdAttachApp;
            }
            protected set { _cmdAttachApp = value; }
        }

        private bool CanAttachApp(ProjectPartViewModel obj)
        {

            if (_currentModel != null && _currentModel.Children.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void HandleAttachApp(ProjectPartViewModel model)
        {
            var msg = new NotificationMessageAction<List<string>>("请将需要关联的测点名称粘贴到下面的列表中", (result) =>
            {

                try
                {
                    DbContext.UpdateAppsProjectByNames(_currentModel.ProjectPartID, result);

                    //refresh current 
                    HandleRefreshApps(_currentModel, true);
                }
                catch (Exception ex)
                {
                    Messenger.Default.Send<Exception>(ex);
                }

            });

            Messenger.Default.Send<NotificationMessageAction<List<string>>>(msg);





        }


        private ICommand _cmdCreateApp;
        public ICommand CmdCreateApp
        {
            get
            {
                if (_cmdCreateApp == null)
                {
                    _cmdCreateApp = new RelayCommand<ProjectPartViewModel>(param => HandleCreateApp(param), CanCreateApp);
                }
                return _cmdCreateApp;
            }
            protected set { _cmdCreateApp = value; }
        }

        private bool CanCreateApp(ProjectPartViewModel obj)
        {

            if (_currentModel != null && _currentModel.Children.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void HandleCreateApp(ProjectPartViewModel model)
        {
#if  !Hide
            MainWindowViewModel.Instance.ShowCreateApp(DbContext, _currentModel.Entity, _currentApps);
#endif
        }

        /// <summary>
        /// 添加部位命令
        /// </summary>
        private ICommand _cmdAddPart;
        public ICommand CmdAddPart
        {
            get
            {
                if (_cmdAddPart == null)
                {
                    _cmdAddPart = new RelayCommand<ProjectPartViewModel>(param => HandleAddPart(param), CanAddPart);
                }
                return _cmdAddPart;
            }
            protected set { _cmdAddPart = value; }
        }

        private bool CanAddPart(ProjectPartViewModel obj)
        {
            if (_currentModel != null && CurrentApps.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void HandleAddPart(ProjectPartViewModel model)
        {
            model = _currentModel;



            try
            {
                ProjectPart newPart = new ProjectPart();
                newPart.Id = Guid.NewGuid();
                newPart.PartName = "新部位";
                newPart.ParentPart = model.ProjectPartID;


                DbContext.AddToProjectParts(newPart);
                DbContext.SaveChanges();

                ProjectPartViewModel newModel = new ProjectPartViewModel(model, newPart, GetChildren);
                model.Children.Add(newModel);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }






        }


        /// <summary>
        /// 关联部位的命令
        /// </summary>
        private ICommand _cmdAttachPart;
        public ICommand CmdAttachPart
        {
            get
            {
                if (_cmdAttachPart == null)
                {
                    _cmdAttachPart = new RelayCommand(param => HandleAttachPart(param), CanAttachPart);
                }
                return _cmdAttachPart;
            }
            protected set { _cmdAttachPart = value; }
        }


        private bool CanAttachPart(object obj)
        {
            return true;
        }

        private void HandleAttachPart(object a)
        {


            var args = a as object[];
            ICollection sourceCollection = args[0] as ICollection;
            var targetModel = args[1] as ProjectPartViewModel;

            try
            {


                if (targetModel.Children.Count != 0)
                {
                    throw new Exception("该节点下还有结点，无法关联");
                }


                //在数据库中保存
                foreach (App item in sourceCollection)
                {

                    item.ProjectPartID = targetModel.Entity.Id;

                    DbContext.UpdateObject(item);
                }

                DbContext.SaveChanges(SaveChangesOptions.Batch);

                //sourceModel.ParentViewModel.Children = GetChildren(sourceModel.ParentViewModel);
                //targetModel.Children = GetChildren(targetModel);


                //直接刷新


                HandleRefreshApps(_currentModel);




            }
            catch (Exception ex)
            {




                Messenger.Default.Send(ex);
            }
            // targetModel
            // Messenger.Default.Send<Exception>(new Exception("Hello world!"));
        }

        /// <summary>
        /// 焦点改变的命令
        /// </summary>
        private ICommand _cmdSelectedItemChanged;
        public ICommand CmdSelectedItemChanged
        {
            get
            {
                if (_cmdSelectedItemChanged == null)
                {
                    _cmdSelectedItemChanged = new RelayCommand<App>(param => HandleSelectedItemChanged(param), CanSelectedItemChanged);
                }
                return _cmdSelectedItemChanged;
            }
            protected set { _cmdSelectedItemChanged = value; }
        }


        private bool CanSelectedItemChanged(object obj)
        {
            return true;
        }

        /// <summary>
        /// 被选中的App
        /// </summary>
        protected App _selectedApp = null;
        private void HandleSelectedItemChanged(App a)
        {

            _selectedApp = a;

        }

        /// <summary>
        /// 显示参数信息的命令
        /// </summary>
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
            if (_selectedApp != null)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleAppParams(App a)
        {

#if  !Hide
            MainWindowViewModel.Instance.ShowAppParams(_selectedApp);

#endif
        }


        private ICommand _cmdAppData;
        public ICommand CmdAppData
        {
            get
            {
                if (_cmdAppData == null)
                {
                    _cmdAppData = new RelayCommand<App>(param => HandleAppData(param), CanAppData);
                }
                return _cmdAppData;
            }
            protected set { _cmdAppData = value; }
        }


        private bool CanAppData(object obj)
        {
            if (_selectedApp != null)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleAppData(App a)
        {

#if  !Hide
            MainWindowViewModel.Instance.ShowAppData(_selectedApp);
#endif
        }




        private ICommand _cmdRenameApp;
        public ICommand CmdRenameApp
        {
            get
            {
                if (_cmdRenameApp == null)
                {
                    _cmdRenameApp = new RelayCommand(param => HandleRenameApp(param), CanRenameApp);
                }
                return _cmdRenameApp;
            }
            protected set { _cmdRenameApp = value; }
        }

        private bool CanRenameApp(object obj)
        {
            if (_selectedApp == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void HandleRenameApp(object model)
        {
            var handelModel = _selectedApp;

            var msg = new NotificationMessageAction<string>("请输入新的名称", (result) =>
            {
                if (result != null && result.Trim().Length != 0)
                {
                    try
                    {
                        handelModel.AppName = result.Trim();
                        //  DbContext.UpdateObject(handelModel);
                        DbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send<Exception>(ex);
                        //  DbContext.Detach(handelModel);
                        _selectedApp = (from i in DbContext.Apps
                                        where i.Id == _selectedApp.Id
                                        select i).First();
                        //HandleRefreshApps(_currentModel, true);
                    }

                }
            });

            Messenger.Default.Send<NotificationMessageAction<string>>(msg);


        }



        #endregion

        ProjectPartViewModel _currentModel = null;
        private void HandleRefreshApps(ProjectPartViewModel model)
        {
            HandleRefreshApps(model, false);
        }
        private void HandleRefreshApps(ProjectPartViewModel model, bool forceFromDB)
        {
            _currentModel = model;
            if (model != null)
            {
                if (model.Children != null && model.Children.Count > 0)
                {
                    //has children
                    CurrentApps.Clear();
                }
                else
                {
                    //get entity from cache
                    IEnumerable<App> dataQuery = null;

                    if (forceFromDB)
                    {
                        //clear cache
                        foreach (var item in CurrentApps)
                        {
                            DbContext.Detach(item);
                        }


                    }
                    else
                    {
                        var q1 = from i in DbContext.Entities
                                 where i.Entity is App
                                 select i.Entity;

                        var q2 = from i in q1.OfType<App>()
                                 where i.ProjectPartID == model.ProjectPartID
                                 select i;
                        if (q2.Count() != 0)
                        {
                            dataQuery = q2;
                            //CurrentApps.Clear();
                            //CurrentApps.Load(q2);
                        }

                    }

                    if (dataQuery == null)
                    {
                        dataQuery = from i in DbContext.Apps
                                    where i.ProjectPartID == model.ProjectPartID
                                    select i;
                    }


                    CurrentApps.Clear();
                    CurrentApps.Load(dataQuery);


                }
            }
        }


        private DataServiceCollection<App> _currentApps = new DataServiceCollection<App>();
        /// <summary>
        /// get the corresponding apps for current projectPart,not null
        /// </summary>
        public DataServiceCollection<App> CurrentApps
        {
            get { return _currentApps; }
            set
            {
                if (_currentApps != value)
                {
                    _currentApps = value;
                    RaisePropertyChanged("CurrentApps");
                }
            }
        }




        //[Conditional("DEBUG")]
        //private void InitialPart()
        //{
        //    ProjectPartViewModel root = new ProjectPartViewModel();
        //    root.PartName = "根节点";
        //    List<ProjectPartViewModel> list1 = new List<ProjectPartViewModel>();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        ProjectPartViewModel levelone = new ProjectPartViewModel();
        //        levelone.PartName = "第一层" + i.ToString();
        //        List<ProjectPartViewModel> list2 = new List<ProjectPartViewModel>();
        //        for (int j = 0; j < 5; j++)
        //        {
        //            ProjectPartViewModel leveltwo = new ProjectPartViewModel();
        //            leveltwo.PartName = "第二层" + j.ToString();
        //            list2.Add(leveltwo);
        //        }
        //        levelone.Children=new ObservableCollection<ProjectPartViewModel>(list2);
        //        list1.Add(levelone);
        //    }
        //    root.Children = new ObservableCollection<ProjectPartViewModel>(list1);
        //    _fristLevel = new List<ProjectPartViewModel>() { root };

        //}

        public ObservableCollection<ProjectPartViewModel> GetChildren(ProjectPartViewModel parent)
        {

            Guid? parentKey = null;
            if (parent != null)
            {
                //if entity is null,get the first level nodes
                parentKey = parent.Entity.Id;
            }

            var query = from i in DbContext.ProjectParts
                        where i.ParentPart == parentKey
                        select new ProjectPartViewModel(parent, i, GetChildren);

            var obc = new ObservableCollection<ProjectPartViewModel>(query);
            return obc;
        }

        private ObservableCollection<ProjectPartViewModel> _fristLevel;
        public ObservableCollection<ProjectPartViewModel> FirstLevel
        {
            get
            {
                if (_fristLevel == null)
                {
                    _fristLevel = GetChildren(null);
                }
                return _fristLevel;
            }
            set
            {
                if (_fristLevel != value)
                {
                    _fristLevel = value;
                    RaisePropertyChanged("RootPart");
                }
            }
        }


        /// <summary>
        /// 删除测点
        /// </summary>
        private ICommand _cmdDeleteApp;
        public ICommand CmdDeleteApp
        {
            get
            {
                if (_cmdDeleteApp == null)
                {
                    _cmdDeleteApp = new RelayCommand<App>(param => HandleDeleteApp(param), CanDeleteApp);
                }
                return _cmdDeleteApp;
            }
            protected set { _cmdDeleteApp = value; }
        }

        private bool CanDeleteApp(App obj)
        {

            if (_selectedApp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void HandleDeleteApp(App model)
        {
            model = _selectedApp;
            var msg = new DialogMessage(string.Format("确定要删除测点：{0}吗?", model.AppName), result =>
            {
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {

                        _currentApps.Remove(model);
                        DbContext.SaveChanges();

                        //从视图中删除
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


    }
}

