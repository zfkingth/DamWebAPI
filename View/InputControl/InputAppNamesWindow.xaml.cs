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


namespace DamMVVM.View
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputAppNamesWindow : Window
    {

        object[] _vals;

        public InputAppNamesWindow(object[] vals)
        {
            InitializeComponent();
            _vals = vals;
        }

        private void DXWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.textbolckTip.Text = _vals[0].ToString();

            this.deInput.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string clipString = (string)System.Windows.Forms.Clipboard.GetDataObject().GetData(typeof(string));

            if (clipString == null || clipString.Length == 0) return;


            string[] sns = clipString.Split(new char[] { '\n', '\r', '\t' });

          

            for (int i = 0; i < sns.Length; i++)
            {
                string name = sns[i].Trim();
                if (name.Length!=0&&deInput.Items.Contains(name)==false)
                {
                    deInput.Items.Add(name);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _vals[1] = null;
            this.Close();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            List<string> nameList = new List<string>(deInput.Items.Count);
            foreach (var item in deInput.Items)
            {
                nameList.Add(item.ToString());
            }

            _vals[1] = nameList;
            this.Close();
        }
    }
}
