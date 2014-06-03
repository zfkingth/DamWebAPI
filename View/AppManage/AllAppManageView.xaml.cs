using DamWebAPI.ViewModel;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DamWebAPI.View
{
    /// <summary>
    /// Interaction logic for AllAppManageView.xaml
    /// </summary>
    public partial class AllAppManageView : UserControl
    {

        public AllAppManageView()
        {
            InitializeComponent();
        }

        private void gridControl_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {

            Hammergo.Utility.Helper.InvodeCmd(this.DataContext, "CmdSelectedItemChanged", e.NewItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gridControl.CopyToClipboard();
        }


    }
}
