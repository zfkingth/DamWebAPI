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

namespace DamWebAPI.ViewModel.DataImport
{
    public class ImportExcelDataViewModel : WorkspaceViewModel
    {
        public ImportExcelDataViewModel()
        {
            DisplayName = "测点管理";
            DbContext.MergeOption = System.Data.Services.Client.MergeOption.OverwriteChanges;
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

                }
                catch (Exception ex)
                {
                    Messenger.Default.Send<Exception>(ex);
                }

            });

            Messenger.Default.Send<NotificationMessageAction<System.IO.DirectoryInfo>>(msg);
 
        }

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
    }
}
