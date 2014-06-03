using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DamMVVM.View.AppManage
{
    /// <summary>
    /// AppSearch.xaml 的交互逻辑
    /// </summary>
    public partial class AppSearchView : UserControl
    {
        public AppSearchView()
        {
            InitializeComponent();
        }

        private void gridControl_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {

            Utility.Helper.InvodeCmd(this.DataContext, "CmdSelectedItemChanged", e.NewItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gridControl.CopyToClipboard();
        }

        private void appNameTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {

                // execute the command, if it exists
                ICommand cmd = btnNameSearch.Command;

                (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
               
                if (cmd != null) cmd.Execute(null);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                appNameTxt.Focus();
            }
        }

        private void calcNameTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {

                // execute the command, if it exists
                ICommand cmd = btnCalcNameSearch.Command;

                (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();

                if (cmd != null) cmd.Execute(null);
            }
        }
    }
}
