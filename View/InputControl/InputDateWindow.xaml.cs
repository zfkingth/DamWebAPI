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
    public partial class InputDateWindow : Window
    {

        object[] _vals;

        public InputDateWindow(object[] vals)
        {
            InitializeComponent();
            _vals = vals;
        }

        private void DXWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.textbolckTip.Text = _vals[0].ToString();

            this.deInput.EditValue = DateTime.Today;
            this.deInput.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var date = this.deInput.DateTime;
            _vals[1] = date.AddMilliseconds(-date.Millisecond);//去掉毫秒部分
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _vals[1] = null;
            this.Close();
        }
    }
}
