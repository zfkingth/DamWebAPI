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
using System.IO;
using System.ComponentModel;

namespace DamWebAPI.ViewModel.DataImport
{
    public class ImportExcelDataViewModel : WorkspaceViewModel,IDisposable
    {
        public ImportExcelDataViewModel()
        {
            DisplayName = "测点管理";

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;

        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HandleInfo = "导入完成";
            Handling = false;
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           HandleInfo = e.UserState.ToString();
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            try
            {

                if (e.Argument is string)
                {
                    handleSingleDir((string)e.Argument);
                }
                else
                {
                    handleTreeDir((DirectoryInfo)e.Argument);
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }
        }

        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
           ExcelImporter importer = new ExcelImporter();
        private void handleTreeDir(DirectoryInfo directoryInfo)
        {
            foreach (DirectoryInfo cdir in directoryInfo.GetDirectories())
            {
                handleTreeDir(cdir);
            }

            handleSingleDir(directoryInfo.FullName);
        }

        private void handleSingleDir(string directory)
        {

            List<FileInfo> xlsFiles = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(directory);
            xlsFiles.AddRange(dir.GetFiles("*.xls"));

            xlsFiles.AddRange(dir.GetFiles("*.xlsx"));

            if (xlsFiles.Count == 0)
            {

                return;
            }

            FileInfo exInfo = null;

            try
            {

                foreach (FileInfo info in xlsFiles)
                {
                    exInfo = info;
                    backgroundWorker1.ReportProgress(0, "从" + exInfo.FullName + "导入数据: ");

                    importer.import(info.FullName);

                }

            }
            catch (Exception ex)
            {
                if (exInfo != null)
                    throw new Exception(exInfo.FullName + "\n" + ex.Message);
                else
                    throw new Exception(ex.Message);

            }
        }
        

        /// <summary>
        /// 选择路径
        /// </summary>
        private ICommand _cmdChoosePath;
        public ICommand CmdChoosePath
        {
            get
            {
                if (_cmdChoosePath == null)
                {
                    _cmdChoosePath = new RelayCommand(param => HandleChoosePath(param), CanChoosePath);
                }
                return _cmdChoosePath;
            }
            protected set { _cmdChoosePath = value; }
        }

        private bool CanChoosePath(object obj)
        {

            return true;
        }

        private void HandleChoosePath(object obj)
        {

            var msg = new NotificationMessageAction<System.IO.DirectoryInfo>("请选择要导入文件的路径", (result) =>
            {

                try
                {
                    Dir = result;
                }
                catch (Exception ex)
                {
                    Messenger.Default.Send<Exception>(ex);
                }

            });

            Messenger.Default.Send<NotificationMessageAction<System.IO.DirectoryInfo>>(msg);
 
        }



        private ICommand _cmdHandleImportData;
        public ICommand CmdHandleImportData
        {
            get
            {
                if (_cmdHandleImportData == null)
                {
                    _cmdHandleImportData = new RelayCommand(param => HandleHandleImportData(param), CanHandleImportData);
                }
                return _cmdHandleImportData;
            }
            protected set { _cmdHandleImportData = value; }
        }

        private bool CanHandleImportData(object obj)
        {

            return !Handling;
        }

        private void HandleHandleImportData(object obj)
        {

            try
            {
                Handling = true;
                if (SingleFolder)
                {

                    backgroundWorker1.RunWorkerAsync(Dir.FullName);

                }
                else
                {

                    backgroundWorker1.RunWorkerAsync(Dir);


                }
                HandleInfo = "导入完成!";
            }
            catch (Exception ex)
            {

                Messenger.Default.Send(ex);
                HandleInfo = ex.Message;
                Handling = false;
            }

        }

        //是否为单个文件夹
        private bool _singleFolder=true;
        public bool SingleFolder
        {
            get
            {
                return _singleFolder;
            }
            set
            {
                if (_singleFolder != value)
                {
                    _singleFolder = value;
                    RaisePropertyChanged("SingleFolder");
                }
            }
        }

        //选择的路径
        private DirectoryInfo _dir;
        public DirectoryInfo Dir
        {
            get
            {
                return _dir;
            }
            set
            {
                if(_dir!=value)
                {
                    _dir = value;
                    RaisePropertyChanged("Dir");
                }
            }
        }

        private bool _handling = false;
        public bool Handling
        {
            get
            {
                return _handling;
            }
            set
            {
                if(_handling!=value)
                {
                    _handling = value;
                    RaisePropertyChanged("Handling");
                }
            }
        }


        private string _handleInfo;
        public string HandleInfo
        {
            get
            {
                return _handleInfo;
            }
            set
            {
                if(_handleInfo!=value)
                {
                    _handleInfo = value;
                    RaisePropertyChanged("HandleInfo");
                }
            }
        }
    }
}
