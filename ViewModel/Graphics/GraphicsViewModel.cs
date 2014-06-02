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

    }
}
