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
    public class DataInputViewModel : WorkspaceViewModel
    {
        public AppDataViewModel AppDataVM { get; set; }

        public Graphics.GraphicsViewModel GraphicsVM { get; set; }

        public DataInputViewModel()
        {
            AppDataVM = new AppDataViewModel();
            GraphicsVM = new Graphics.GraphicsViewModel();

            AppDataVM.DbContext = this.DbContext;

            GraphicsVM.DbContext = this.DbContext;
        }
         



    }
}
