﻿using GalaSoft.MvvmLight.Messaging;
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

        private List<string> yAxis = new List<string>() { "y1", "y2", "y3" };

        public List<string> YAxis
        {
            get
            {
                return yAxis;
            }
        }

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
                    //Graphics.图形Row row = graphics.图形.New图形Row();

                    var line = GraphicDS.Lines.NewLinesRow();

                    //row.测点编号 = appName;
                    line.AppName = appName;

                    //row.刻度轴 = "主轴";

                    line.AxisName = "y1";

                    //row.线条名称 = cp.ParamName;
                    line.ParamName = cp.ParamName;

                    //row.图例名称 = appName + "." + cp.ParamName;

                    line.LegendName = appName + "." + cp.ParamName;

                    //row.EndEdit();

                    line.EndEdit();

                    //graphics.图形.Add图形Row(row);

                    GraphicDS.Lines.AddLinesRow(line);

                }

                GraphicDS.AcceptChanges();

                //graphics.AcceptChanges()

            }
            catch (Exception ex)
            {

                Messenger.Default.Send(ex);
                GraphicDS.RejectChanges();
            }
        }


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


        #region AddedAppName

        private string _addedAppName;
        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public string AddedAppName
        {
            get { return _addedAppName; }
            set
            {
                if (_addedAppName != value)
                {
                    _addedAppName = value;
                    RaisePropertyChanged("AddedAppName");
                }
            }
        }

        #endregion


        #region AddedAppName
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

            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }

        }

        #endregion

    }
}
