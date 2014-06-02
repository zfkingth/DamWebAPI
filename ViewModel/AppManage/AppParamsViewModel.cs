using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DamServiceV3.Test.DamServiceRef;

namespace DamWebAPI.ViewModel.AppManage
{
    public class AppParamsViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">用于显示的ViewModel名称</param>
        /// <param name="app">测点信息</param>
        /// <param name="dbContext">数据上下文</param>
        public AppParamsViewModel(string displayName, App app)
        {

            this.DisplayName = displayName;
            this._app = app;

            //查询所有的数据
            //  Func<string, string> selector 
            var qp = DbContext.AppParams.Where(i => i.AppId == App.Id);



            _allParams.Load(qp);
            _allFormulae.Load(DbContext.GetAllFormulaeByAppID(App.Id));
        }



        private DataServiceCollection<AppParam> _allParams = new DataServiceCollection<AppParam>();
        private DataServiceCollection<Formula> _allFormulae = new DataServiceCollection<Formula>();

        private App _app;
        public App App
        {
            get
            {
                return _app;
            }
        }


        /// <summary>
        /// 测点类型数据
        /// </summary>
        ObservableCollection<ConstantParam> _constantParams = null;
        public ObservableCollection<ConstantParam> ConstantParams
        {
            get
            {
                if (_constantParams == null)
                {
                    var query = _allParams.OfType<ConstantParam>();
                    ObservableCollection<ConstantParam> collection = new ObservableCollection<ConstantParam>(query);


                    _constantParams = collection;
                }
                return _constantParams;
            }
        }

        ObservableCollection<MessureParam> _messureParams = null;
        public ObservableCollection<MessureParam> MessureParams
        {
            get
            {
                if (_messureParams == null)
                {
                    var query = _allParams.OfType<MessureParam>();
                    ObservableCollection<MessureParam> collection = new ObservableCollection<MessureParam>(query);


                    _messureParams = collection;
                }
                return _messureParams;
            }
        }


        ObservableCollection<CalculateParam> _calculateParams = null;
        public ObservableCollection<CalculateParam> CalculateParams
        {
            get
            {
                if (_calculateParams == null)
                {
                    var query = _allParams.OfType<CalculateParam>();
                    ObservableCollection<CalculateParam> collection = new ObservableCollection<CalculateParam>(query);


                    _calculateParams = collection;
                }
                return _calculateParams;
            }
        }


        #region CurrentDate

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    RaisePropertyChanged("CurrentDate");
                }
            }
        }

        #endregion // CurrentDate

        ObservableCollection<DateTimeOffset> _dates = null;
        public ObservableCollection<DateTimeOffset> Dates
        {
            get
            {
                if (_dates == null)
                {
                    //dbcontext 只保存当前测点的信息
                    var query = (from i in _allFormulae
                                 select i.StartDate).Distinct();
                    _dates = new ObservableCollection<DateTimeOffset>(query);

                    if (_dates.Count == 0)
                    {
                        _dates.Add(hammergo.GlobalConfig.PubConstant.InitialTime);
                    }
                }
                return _dates;
            }
        }

        public string GetFormulaString(Guid calcParamId, DateTime startDate)
        {

            var f = (from i in _allFormulae
                     where i.ParamId == calcParamId && i.StartDate == startDate
                     select i).FirstOrDefault();
            if (f != null)
            {
                return f.FormulaExpression;
            }
            else
            {
                return "未设置公式";
            }
        }

        public void SetFormulaString(Guid calcParamId, DateTime startDate, string formula)
        {

            var formulaEntity = (from i in _allFormulae
                                 where i.ParamId == calcParamId && i.StartDate == startDate
                                 select i).FirstOrDefault();
            if (formulaEntity == null)
            {
                //数据库中不存在
                formulaEntity = new Formula();
                formulaEntity.Id = Guid.NewGuid();
                formulaEntity.ParamId = calcParamId;
                formulaEntity.StartDate = startDate;
                formulaEntity.FormulaExpression = "";
                formulaEntity.CalculateOrder = 1;

                //查询下一个的startDate
                var nextEntity = (from i in _allFormulae
                                  where i.ParamId == calcParamId && i.StartDate > startDate
                                  orderby i.StartDate ascending
                                  select i).FirstOrDefault();
                if (nextEntity == null)
                {
                    //没有下一个时间段的公式，
                    formulaEntity.EndDate = hammergo.GlobalConfig.PubConstant.OverTime;//默认终止时间
                }
                else
                {
                    formulaEntity.EndDate = nextEntity.StartDate;
                }

                // DbContext.AddToFormulae(formulaEntity);
                _allFormulae.Add(formulaEntity);
            }

            formulaEntity.FormulaExpression = formula;

            //var entityDescriptor= DbContext.GetEntityDescriptor(formulaEntity);

            //if (entityDescriptor.State != EntityStates.Added)
            //{
            //    DbContext.UpdateObject(formulaEntity);
            //}
        }



        /// <summary>
        /// 保存修改
        /// </summary>
        private ICommand _cmdSave;
        public ICommand CmdSave
        {
            get
            {
                if (_cmdSave == null)
                {
                    _cmdSave = new RelayCommand(s => HandleSave(s));
                }
                return _cmdSave;
            }
            protected set { _cmdSave = value; }
        }



        private void HandleSave(object s)
        {
            try
            {
                DbContext.SaveChanges(SaveChangesOptions.Batch);

                var msg = new DialogMessage("保存成功!", null);

                msg.Caption = "提示";
                msg.Button = MessageBoxButton.OK;
                msg.Icon = MessageBoxImage.Information;

                Messenger.Default.Send<DialogMessage>(msg);

            }
            catch (Exception ex)
            {
                Messenger.Default.Send(ex);
            }

        }

        List<hammergo.GlobalConfig.ParamInfo> _configConstParamsList = null;
        public List<hammergo.GlobalConfig.ParamInfo> ConfigConstParamsList
        {
            get
            {
                if (_configConstParamsList == null)
                {
                    _configConstParamsList = new List<hammergo.GlobalConfig.ParamInfo>(hammergo.GlobalConfig.PubConstant.ConfigData.ConstParamsList);

                }
                return _configConstParamsList;
            }
        }


        List<hammergo.GlobalConfig.ParamInfo> _configDefaultParamsList = null;
        public List<hammergo.GlobalConfig.ParamInfo> ConfigDefaultParamsList
        {
            get
            {
                if (_configDefaultParamsList == null)
                {
                    _configDefaultParamsList = new List<hammergo.GlobalConfig.ParamInfo>(hammergo.GlobalConfig.PubConstant.ConfigData.DefaultParamsList);

                }
                return _configDefaultParamsList;
            }
        }


        /// <summary>
        /// 选择改变的命令
        /// </summary>
        private ICommand _cmdSelectedItemChangedConst;
        public ICommand CmdSelectedItemChangedConst
        {
            get
            {
                if (_cmdSelectedItemChangedConst == null)
                {
                    _cmdSelectedItemChangedConst = new RelayCommand<ConstantParam>(param => HandleSelectedItemChangedConst(param), CanSelectedItemChangedConst);
                }
                return _cmdSelectedItemChangedConst;
            }
            protected set { _cmdSelectedItemChangedConst = value; }
        }


        private bool CanSelectedItemChangedConst(object obj)
        {
            return true;
        }

        /// <summary>
        /// 被选中的ConstantParam
        /// </summary>
        ConstantParam _selectedConstantParam = null;
        private void HandleSelectedItemChangedConst(ConstantParam a)
        {

            _selectedConstantParam = a;

        }

        private ICommand _cmdDeleteConstParam;
        public ICommand CmdDeleteConstParam
        {
            get
            {
                if (_cmdDeleteConstParam == null)
                {
                    _cmdDeleteConstParam = new RelayCommand<ConstantParam>(param => HandleDeleteConstParam(param), CanDeleteConstParam);
                }
                return _cmdDeleteConstParam;
            }
            protected set { _cmdDeleteConstParam = value; }
        }


        private bool CanDeleteConstParam(object obj)
        {
            if (_selectedConstantParam != null)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleDeleteConstParam(ConstantParam a)
        {
            var delmodel = _selectedConstantParam;
            var msg = new DialogMessage(string.Format("确定要删除参数：{0}吗?", delmodel.ParamName), result =>
            {
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {

                        _constantParams.Remove(delmodel);
                        _allParams.Remove(delmodel);
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


        private ICommand _cmdAddConstParam;
        public ICommand CmdAddConstParam
        {
            get
            {
                if (_cmdAddConstParam == null)
                {
                    _cmdAddConstParam = new RelayCommand<ConstantParam>(param => HandleAddConstParam(param), CanAddConstParam);
                }
                return _cmdAddConstParam;
            }
            protected set { _cmdAddConstParam = value; }
        }


        private bool CanAddConstParam(object obj)
        {

            return true;


        }

        private void HandleAddConstParam(ConstantParam a)
        {
            int num = _constantParams.Count();
            ConstantParam cp = new ConstantParam();
            cp.Id = Guid.NewGuid();
            cp.ParamName = "常量参数" + num.ToString();
            cp.PrecisionNum = 5;
            cp.ParamSymbol = "cn" + num.ToString();
            cp.Order = (byte)num;
            cp.UnitSymbol = "no symbol";
            cp.Val = 1;
            cp.AppId = App.Id;

            _constantParams.Add(cp);
            _allParams.Add(cp);


        }


        /// <summary>
        /// 选择改变的命令
        /// </summary>
        private ICommand _cmdSelectedItemChangedMes;
        public ICommand CmdSelectedItemChangedMes
        {
            get
            {
                if (_cmdSelectedItemChangedMes == null)
                {
                    _cmdSelectedItemChangedMes = new RelayCommand<MessureParam>(param => HandleSelectedItemChangedMes(param), CanSelectedItemChangedMes);
                }
                return _cmdSelectedItemChangedMes;
            }
            protected set { _cmdSelectedItemChangedMes = value; }
        }


        private bool CanSelectedItemChangedMes(object obj)
        {
            return true;
        }

        /// <summary>
        /// 被选中的ConstantParam
        /// </summary>
        MessureParam _selectedMessureParam = null;
        private void HandleSelectedItemChangedMes(MessureParam a)
        {

            _selectedMessureParam = a;

        }


        private ICommand _cmdDeleteMessureParam;
        public ICommand CmdDeleteMessureParam
        {
            get
            {
                if (_cmdDeleteMessureParam == null)
                {
                    _cmdDeleteMessureParam = new RelayCommand<MessureParam>(param => HandleDeleteMessureParam(param), CanDeleteMessureParam);
                }
                return _cmdDeleteMessureParam;
            }
            protected set { _cmdDeleteMessureParam = value; }
        }


        private bool CanDeleteMessureParam(object obj)
        {
            if (_selectedMessureParam != null)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleDeleteMessureParam(MessureParam a)
        {
            var delmodel = _selectedMessureParam;
            var msg = new DialogMessage(string.Format("确定要删除参数：{0}吗?", delmodel.ParamName), result =>
            {
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {

                        _messureParams.Remove(delmodel);
                        _allParams.Remove(delmodel);
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

        private ICommand _cmdDeleteCalculateParam;
        public ICommand CmdDeleteCalculateParam
        {
            get
            {
                if (_cmdDeleteCalculateParam == null)
                {
                    _cmdDeleteCalculateParam = new RelayCommand<CalculateParam>(param => HandleDeleteCalculateParam(param), CanDeleteCalculateParam);
                }
                return _cmdDeleteCalculateParam;
            }
            protected set { _cmdDeleteCalculateParam = value; }
        }


        private bool CanDeleteCalculateParam(object obj)
        {
            if (_selectedCalculateParam != null)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleDeleteCalculateParam(CalculateParam a)
        {
            var delmodel = _selectedCalculateParam;
            var msg = new DialogMessage(string.Format("确定要删除参数：{0}吗?", delmodel.ParamName), result =>
            {
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {

                        _calculateParams.Remove(delmodel);
                        _allParams.Remove(delmodel);
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


        /// <summary>
        /// 选择改变的命令
        /// </summary>
        private ICommand _cmdSelectedItemChangedCalc;
        public ICommand CmdSelectedItemChangedCalc
        {
            get
            {
                if (_cmdSelectedItemChangedCalc == null)
                {
                    _cmdSelectedItemChangedCalc = new RelayCommand<CalculateParam>(param => HandleSelectedItemChangedCalc(param), CanSelectedItemChangedCalc);
                }
                return _cmdSelectedItemChangedCalc;
            }
            protected set { _cmdSelectedItemChangedCalc = value; }
        }


        private bool CanSelectedItemChangedCalc(object obj)
        {
            return true;
        }

        /// <summary>
        /// 被选中
        /// </summary>
        CalculateParam _selectedCalculateParam = null;
        private void HandleSelectedItemChangedCalc(CalculateParam a)
        {

            _selectedCalculateParam = a;

        }


        private ICommand _cmdAddMessureParam;
        public ICommand CmdAddMessureParam
        {
            get
            {
                if (_cmdAddMessureParam == null)
                {
                    _cmdAddMessureParam = new RelayCommand<MessureParam>(param => HandleAddMessureParam(param), CanAddMessureParam);
                }
                return _cmdAddMessureParam;
            }
            protected set { _cmdAddMessureParam = value; }
        }


        private bool CanAddMessureParam(object obj)
        {

            return true;


        }

        private void HandleAddMessureParam(MessureParam a)
        {
            int num = _messureParams.Count() + 1;
            MessureParam cp = new MessureParam();
            cp.Id = Guid.NewGuid();
            cp.ParamName = "计算参数" + num.ToString();
            cp.PrecisionNum = 2;
            cp.ParamSymbol = "m" + num.ToString();
            cp.Order = (byte)num;
            cp.UnitSymbol = "no symbol";
            cp.AppId = App.Id;

            _messureParams.Add(cp);
            _allParams.Add(cp);


        }

        private ICommand _cmdAddCalculateParam;
        public ICommand CmdAddCalculateParam
        {
            get
            {
                if (_cmdAddCalculateParam == null)
                {
                    _cmdAddCalculateParam = new RelayCommand<CalculateParam>(param => HandleAddCalculateParam(param), CanAddCalculateParam);
                }
                return _cmdAddCalculateParam;
            }
            protected set { _cmdAddCalculateParam = value; }
        }


        private bool CanAddCalculateParam(object obj)
        {

            return true;


        }

        private void HandleAddCalculateParam(CalculateParam a)
        {
            int num = _calculateParams.Count() + 1;
            CalculateParam cp = new CalculateParam();
            cp.Id = Guid.NewGuid();
            cp.ParamName = "计算参数" + num.ToString();
            cp.PrecisionNum = 2;
            cp.ParamSymbol = "c" + num.ToString();
            cp.Order = (byte)num;
            cp.UnitSymbol = "no symbol";
            cp.AppId = App.Id;

            _calculateParams.Add(cp);
            _allParams.Add(cp);


        }


        //当前的分段公式的时刻点
        private ICommand _cmdSelectedItemChangedDate;
        public ICommand CmdSelectedItemChangedDate
        {
            get
            {
                if (_cmdSelectedItemChangedDate == null)
                {
                    _cmdSelectedItemChangedDate = new RelayCommand<DateTime?>(param => HandleSelectedItemChangedDate(param), CanSelectedItemChangedDate);
                }
                return _cmdSelectedItemChangedDate;
            }
            protected set { _cmdSelectedItemChangedDate = value; }
        }


        private bool CanSelectedItemChangedDate(DateTime? obj)
        {
            return true;
        }

        /// <summary>
        /// 被选中
        /// </summary>
        DateTime? _selectedDate = null;
        private void HandleSelectedItemChangedDate(DateTime? a)
        {

            _selectedDate = a;

        }

        private ICommand _cmdAddFormulaDate;
        public ICommand CmdAddFormulaDate
        {
            get
            {
                if (_cmdAddFormulaDate == null)
                {
                    _cmdAddFormulaDate = new RelayCommand(s => HandleAddFormulaDate(s));
                }
                return _cmdAddFormulaDate;
            }
            protected set { _cmdAddFormulaDate = value; }
        }

        private void HandleAddFormulaDate(object arg)
        {



            var msg = new NotificationMessageAction<DateTime>("请输入新的分段公式起始时刻", (result) =>
            {

                try
                {
                    if (result < hammergo.GlobalConfig.PubConstant.InitialTime)
                    {
                        throw new Exception("要插入的时刻不能小于" + hammergo.GlobalConfig.PubConstant.InitialTime.ToString("u"));

                    }

                    //首先获取当前的时刻列表，以进行时刻划分
                    var dateList = (from i in _allFormulae
                                    orderby i.StartDate ascending
                                    select i.StartDate).Distinct().ToList();

                    if (dateList.Contains(result))
                    {
                        throw new Exception("已存在时刻点" + result.ToString("u"));
                    }

                    //寻找大于result的最小值
                    DateTimeOffset greaterDate = (from i in dateList
                                            where i > result
                                            orderby i ascending
                                            select i).FirstOrDefault();


                    //寻找小于result在最大值
                    DateTimeOffset lessDate = (from i in dateList
                                         where i < result
                                         orderby i descending
                                         select i).FirstOrDefault();

                    DateTimeOffset startDate, endDate;

                    if (greaterDate == DateTime.MinValue && lessDate == DateTime.MinValue)
                    {
                        //没有任何计算公式
                        startDate = result;
                        endDate = hammergo.GlobalConfig.PubConstant.OverTime;
                    }
                    else if (lessDate == DateTime.MinValue)
                    {
                        //要插入的时刻在所有时刻之前
                        startDate = result;
                        endDate = greaterDate;
                    }
                    else if (greaterDate == DateTime.MinValue)
                    {
                        //要插入的时刻在所有时刻之后
                        startDate = result;
                        endDate = hammergo.GlobalConfig.PubConstant.OverTime;
                    }
                    else
                    {
                        //要插入的时刻在两个开始时刻之间
                        startDate = result;
                        endDate = greaterDate;
                    }

                    foreach (var param in _calculateParams)
                    {
                        var formulaEntity = new Formula();
                        formulaEntity.Id = Guid.NewGuid();
                        formulaEntity.ParamId = param.Id;
                        formulaEntity.StartDate = startDate;
                        formulaEntity.EndDate = endDate;
                        formulaEntity.FormulaExpression = "未设置公式";

                        _allFormulae.Add(formulaEntity);

                        //修改lessDate时刻公式的结束时刻
                        var mf = (from i in _allFormulae
                                  where i.ParamId == param.Id && i.StartDate == lessDate
                                  select i).FirstOrDefault();
                        if (mf != null)
                        {
                            //更新结束时刻
                            mf.EndDate = result;
                        }

                    }

                    _dates.Add(result);
                    CurrentDate = result;

                }
                catch (Exception ex)
                {
                    Messenger.Default.Send<Exception>(ex);
                }

            });

            Messenger.Default.Send<NotificationMessageAction<DateTime>>(msg);



        }


        private ICommand _cmdDeleteCalcDate;
        public ICommand CmdDeleteCalcDate
        {
            get
            {
                if (_cmdDeleteCalcDate == null)
                {
                    _cmdDeleteCalcDate = new RelayCommand(param => HandleDeleteCalcDate(param), CanDeleteCalcDate);
                }
                return _cmdDeleteCalcDate;
            }
            protected set { _cmdDeleteCalcDate = value; }
        }


        private bool CanDeleteCalcDate(object obj)
        {
            if (_dates.Count > 1)
            {
                return true;
            }
            else
                return false;

        }

        private void HandleDeleteCalcDate(object a)
        {
            var delDate = CurrentDate;
            var msg = new DialogMessage(string.Format("确定要删该时间点{0}的公式吗?", delDate.ToString("u")), result =>
            {
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        DateTimeOffset prefixDate = (from i in _dates
                                               where i < CurrentDate
                                               orderby i descending
                                               select i).FirstOrDefault();
                        //当没有之前的时间时，preDate为DateTime.MinDate
                        DateTimeOffset suffixDate = (from i in _dates
                                               where i > CurrentDate
                                               orderby i ascending
                                               select i).FirstOrDefault();

                        if (suffixDate == DateTime.MinValue)
                        {
                            //没有后缀的时间，要删除的项为最为一个时刻
                            suffixDate = hammergo.GlobalConfig.PubConstant.OverTime;
                        }

                        //删除
                        var delList = (from i in _allFormulae
                                       where i.StartDate == CurrentDate
                                       select i).ToList();

                        foreach (var delItem in delList)
                        {
                            _allFormulae.Remove(delItem);
                        }

                        //修正前一个时刻点的公式结束时刻
                        var modifyList = (from i in _allFormulae
                                          where i.StartDate == prefixDate
                                          select i).ToList();

                        foreach (var modifyItem in modifyList)
                        {
                            modifyItem.EndDate = suffixDate;
                        }

                        Dates.Remove(CurrentDate);
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



    }
}
