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
using System.Windows.Shapes;
using DevExpress.Xpf.Core;


namespace DamWebAPI.View
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {

        string[] _vals;
        
        public InputWindow( string[] vals)
        {
            InitializeComponent();
            _vals = vals;
        }

        private void DXWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.textbolckTip.Text = _vals[0];
            this.textBoxContent.Text = _vals[1];

            this.textBoxContent.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vals[1] = this.textBoxContent.Text;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _vals[1] = null;
            this.Close();
        }
    }
}
