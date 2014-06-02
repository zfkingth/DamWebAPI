using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DamServiceV3.Test.DamServiceRef;

namespace DamWebAPI.ViewModel.AppManage
{
    public class CreateAppViewModel : WorkspaceViewModel
    {
        ProjectPart _part = null;
        DataServiceCollection<App> _currentApps;

        App _newApp = null;

        public App NewApp
        {
            get
            {
                if (_newApp == null)
                {
                    _newApp = new App();
                }
                return _newApp;
            }
        }

        public CreateAppViewModel(string displayName, Container dbcontext, ProjectPart part, DataServiceCollection<App> currentApps)
        {
            this.DisplayName = displayName;
            this._dbContext = dbcontext;
            this._part = part;
            this._currentApps = currentApps;


        }

        public ProjectPart Part
        {
            get
            {
                return _part;
            }
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


        private bool _allowClone = true;

        public bool AllowClone
        {
            get { return _allowClone; }
            set
            {
                if (_allowClone != value)
                {
                    _allowClone = value;
                    RaisePropertyChanged("AllowClone");
                }
            }
        }

        private string _cloneAppName = "";

        public string CloneAppName
        {
            get { return _cloneAppName; }
            set
            {
                if (_cloneAppName != value)
                {
                    _cloneAppName = value;
                    RaisePropertyChanged("CloneAppName");
                }
            }
        }


        private ICommand _cmdCloneAppProperties;
        public ICommand CmdCloneAppProperties
        {
            get
            {
                if (_cmdCloneAppProperties == null)
                {
                    _cmdCloneAppProperties = new RelayCommand(param => HandleCloneAppProperties(param), CanCloneAppProperties);
                }
                return _cmdCloneAppProperties;
            }
            protected set { _cmdCloneAppProperties = value; }
        }


        private bool CanCloneAppProperties(object param)
        {
            return AllowClone;

        }

        private App cloneApp = null;
        private void HandleCloneAppProperties(object param)
        {
            try
            {
                if (AllowClone && CloneAppName.Trim().Length != 0)
                {
                    cloneApp = (from s in DbContext.Apps
                                where s.AppName == CloneAppName
                                select s).FirstOrDefault();
                    if (cloneApp == null)
                    {
                        throw new Exception(string.Format("找不到测点编号为{0}的测点", CloneAppName));
                    }

                    ApparatusType type = AppTypes.FirstOrDefault(s => s.Id == cloneApp.AppTypeID);

                    NewApp.ApparatusType = type;
                    NewApp.X = cloneApp.X;
                    NewApp.Y = cloneApp.Y;
                    NewApp.Z = cloneApp.Z;
                    NewApp.OtherInfo = cloneApp.OtherInfo;
                    NewApp.BuriedTime = cloneApp.BuriedTime;
                }

            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }



        }



        private ICommand _cmdCreateApp;
        public ICommand CmdCreateApp
        {
            get
            {
                if (_cmdCreateApp == null)
                {
                    _cmdCreateApp = new RelayCommand(param => HandleCreateApp(param), CanCreateApp);
                }
                return _cmdCreateApp;
            }
            protected set { _cmdCreateApp = value; }
        }


        private bool CanCreateApp(object param)
        {
            return true;

        }

        private void HandleCreateApp(object param)
        {
            List<object> addedList = new List<object>();

            try
            {
                //
                int childrenCnt = (from i in DbContext.ProjectParts
                                   where i.ParentPart == Part.Id
                                   select i).Count();
                if (childrenCnt > 0)
                {
                    throw new Exception("只能在没有子结点的部位创建测点");
                }

                int appCnt = (from s in DbContext.Apps
                              where s.AppName == NewApp.AppName || s.CalculateName == NewApp.CalculateName
                              select s).Count();
                if (appCnt > 0)
                {
                    throw new Exception("新测点的名称或计算名称已存在，无法创建");
                }

                App needAddApp = new App();


                needAddApp.Id = Guid.NewGuid();
                needAddApp.ProjectPartID = Part.Id;

                needAddApp.AppName = NewApp.AppName;
                needAddApp.CalculateName = NewApp.CalculateName;
                needAddApp.BuriedTime = NewApp.BuriedTime;
                needAddApp.X = NewApp.X;
                needAddApp.Y = NewApp.Y;
                needAddApp.Z = NewApp.Z;
                needAddApp.OtherInfo = NewApp.OtherInfo;



                DbContext.AddToApps(needAddApp);
                addedList.Add(needAddApp);

                if (AllowClone)
                {
                    //clone app params

                    if (CloneAppName.Trim().Length == 0)
                    {
                        throw new Exception("当选择克隆时，模板测点不能为空");
                    }

                    if (cloneApp == null)
                    {

                        cloneApp = (from s in DbContext.Apps
                                    where s.AppName == CloneAppName
                                    select s).FirstOrDefault();
                        if (cloneApp == null)
                        {
                            throw new Exception(string.Format("找不到测点编号为{0}的测点", CloneAppName));
                        }
                    }

                    DbContext.LoadProperty(cloneApp, "AppParams");

                    foreach (var item in cloneApp.AppParams)
                    {
                        AppParam newParam = null;
                        if (item is ConstantParam)
                        {
                            newParam = new ConstantParam();
                            (newParam as ConstantParam).Val = (item as ConstantParam).Val;
                        }
                        else if (item is MessureParam)
                        {
                            newParam = new MessureParam();
                        }
                        else if (item is CalculateParam)
                        {
                            newParam = new CalculateParam();
                        }

                        //set values
                        newParam.Id = Guid.NewGuid();
                        newParam.AppId = needAddApp.Id;
                        newParam.ParamName = item.ParamName;
                        newParam.ParamSymbol = item.ParamSymbol;
                        newParam.PrecisionNum = item.PrecisionNum;
                        newParam.UnitSymbol = item.UnitSymbol;
                        newParam.Order = item.Order;
                        newParam.Description = item.Description;

                        DbContext.AddToAppParams(newParam);
                        addedList.Add(newParam);

                        //clone formules
                        if (item is CalculateParam)
                        {
                            CalculateParam cp = item as CalculateParam;
                            var formulae = (from i in DbContext.Formulae
                                            where i.ParamId == cp.Id
                                            select i).ToList();
                            foreach (var fl in formulae)
                            {
                                Formula newfl = new Formula();
                                newfl.Id = Guid.NewGuid();
                                newfl.ParamId = newParam.Id;
                                newfl.FormulaExpression = fl.FormulaExpression;
                                newfl.StartDate = fl.StartDate;
                                newfl.EndDate = fl.EndDate;
                                newfl.CalculateOrder = fl.CalculateOrder;
                                DbContext.AddToFormulae(newfl);
                                addedList.Add(newfl);
                            }
                        }

                    }

                }

                DbContext.SaveChanges(SaveChangesOptions.Batch);
                _currentApps.Add(needAddApp);


                var msg = new DialogMessage("测点创建成功并已添加到相关的工程部位中。", result =>
                {

                });

                msg.Caption = "创建成功";
                msg.Button = MessageBoxButton.OK;
                msg.Icon = MessageBoxImage.Information;

                Messenger.Default.Send<DialogMessage>(msg);



            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
                //reject the changes
                foreach (object item in addedList)
                {
                    DbContext.Detach(item);
                }

            }
            finally
            {
                // DbContext.Detach(NewApp);

            }



        }

    }
}
