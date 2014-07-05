using GalaSoft.MvvmLight.Messaging;
using hammergo.GlobalConfig;
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
using DamWebAPI.ViewModel.AppManage;

namespace DamWebAPI.ViewModel.Graphics
{
    public class GraphicsViewModel : WorkspaceViewModel
    {
        private AllAppManageViewModel _appManageViewModel;

        public AllAppManageViewModel AppManageViewModel
        {
            get
            {
                if (_appManageViewModel == null)
                {
                    _appManageViewModel = new AllAppManageViewModel();
                }
                return _appManageViewModel;
            }
        }

        private DamWebAPI.ViewModel.Entity.Graphics graphicDS = new DamWebAPI.ViewModel.Entity.Graphics();

        public DamWebAPI.ViewModel.Entity.Graphics GraphicDS
        {
            get
            {
                return graphicDS;
            }
        }

  

        #region AddAppInDS
        /// <summary>
        /// 将选中的测点添加到表中
        /// </summary>
        /// <param name="selapp"></param>
        public void AddAppInDS(App selapp)
        {
            try
            {
                string appName = selapp.AppName;

                AppIntegratedInfo appInfo = new AppIntegratedInfo(selapp, 0, null, null);


                // appInfo.CalcParams默认已排序
                foreach (CalculateParam cp in appInfo.CalcParams)
                {

                    var line = GraphicDS.Lines.NewLinesRow();

                    line.AppName = appName;


                    line.UnitSymbol = cp.UnitSymbol;

                    line.AppId = cp.AppId;
                    line.ParamId = cp.Id;

                    line.ParamName = cp.ParamName;


                    line.LegendName = appName + "." + cp.ParamName;


                    line.EndEdit();


                    GraphicDS.Lines.AddLinesRow(line);

                }

                GraphicDS.AcceptChanges();


            }
            catch (Exception ex)
            {

                Messenger.Default.Send(ex);
                GraphicDS.RejectChanges();
            }
        }

        #endregion

        #region StartDate

        private DateTime? _startDate;
        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    RaisePropertyChanged("StartDate");
                }
            }
        }

        #endregion

        #region OffsetStart
        public DateTimeOffset? OffsetStart
        {
            get
            {
                DateTimeOffset? date = null;
                if (StartDate != null)
                {
                    date = new DateTimeOffset(StartDate.Value);
                }

                return date;
            }
        }
        #endregion


        #region OffsetEnd
        public DateTimeOffset? OffsetEnd
        {
            get
            {
                DateTimeOffset? date = null;
                if (EndDate != null)
                {
                    date = new DateTimeOffset(EndDate.Value);
                }

                return date;
            }
        }
        #endregion


        #region EndDate

        private DateTime? _endDate;
        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    RaisePropertyChanged("EndDate");
                }
            }
        }

        #endregion


        #region FeildAppName

        private string _feildAppName;
        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public string FeildAppName
        {
            get { return _feildAppName; }
            set
            {
                if (_feildAppName != value)
                {
                    _feildAppName = value;
                    RaisePropertyChanged("FeildAppName");
                }
            }
        }

        #endregion


        #region CmdAddApp
        private ICommand _cmdAddApp;
        public ICommand CmdAddApp
        {
            get
            {
                if (_cmdAddApp == null)
                {
                    _cmdAddApp = new RelayCommand(param => HandleAddApp(param));
                }
                return _cmdAddApp;
            }
            protected set { _cmdAddApp = value; }
        }


        private void HandleAddApp(object param)
        {

            try
            {
                var fapp = DbContext.Apps.Where(s => s.AppName == FeildAppName).FirstOrDefault();
                if (fapp == null)
                {
                    throw new Exception("测点不存在");
                }
                if (AppManageViewModel.CurrentApps.Contains(fapp))
                {
                    throw new Exception("已存在");
                }

                AppManageViewModel.CurrentApps.Add(fapp);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }

        }

        #endregion


        public  IEnumerable<CalculateValue>   GetAllCalcValues(Entity.Graphics graDS)
        {
            var appids = (from i in graDS.Lines
                          where i.IsShow == true
                          select i.AppId).Distinct().ToList();

            var values = DbContext.GetCalcValues(appids, 0, OffsetStart, OffsetEnd);

            return values;
        }

        public Entity.Graphics CreateNewGraphicDS(App selApp)
        {
            string appName = selApp.AppName;

            AppIntegratedInfo appInfo = new AppIntegratedInfo(selApp, 0, null, null);
            var graDS = new Entity.Graphics();
            // appInfo.CalcParams默认已排序
            for (int i = 0; i < appInfo.CalcParams.Count; i++)
            {
                CalculateParam cp = appInfo.CalcParams[i];
                var line = graDS.Lines.NewLinesRow();
                line.AppName = appName;
                line.UnitSymbol = cp.UnitSymbol;
                line.AppId = cp.AppId;
                line.ParamName = cp.ParamName;
                line.LegendName = cp.ParamName;
                line.IsShow = true;
                line.ParamId = cp.Id;
                line.EndEdit();
                graDS.Lines.AddLinesRow(line);
            }
            graDS.AcceptChanges();

            return graDS;
        }
    }
}
