using System;
using System.Collections.Generic;
using System.Configuration;
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
using WpfApplication1.Properties;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //string _string5x = Strings.ConnectionString5x;
        //public string ConnectionString5x
        //{
        //    get { return _string5x; }
        //    set { _string5x = value; }
        //}

        //string _string6x = Strings.ConnectionString6x;
        //public string ConnectionString6x
        //{
        //    get { return _string6x; }
        //    set { _string6x = value; }
        //}


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //string connStr = ConfigurationManager.ConnectionStrings["Dam5xEntities"].ConnectionString;
            this.DataContext = new Import.ImportBase();
          
        }

 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as Import.ImportBase).TestConnection();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //(this.DataContext as Import.ImportBase).Percentage = 50;
            var item = new Import.ImportAppType();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }



        public void SetButtonEnable(bool enable)
        {
            foreach (var item in ButtonList)
            {
                item.IsEnabled = enable;
            }
        }


        List<Button> _buttonList = null;
        public List<Button> ButtonList
        {
            get
            {
                if (_buttonList == null)
                {
                    _buttonList = GetChildObjects<Button>(this);
                }
                return _buttonList;
            }
        }

        private List<T> GetChildObjects<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T)
                {
                    childList.Add(child as T);
                }
                List<T> temp = GetChildObjects<T>(child);
                childList.AddRange(temp);
            }
            return childList;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportProjectPart();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportApp();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }

        void item_WorkCompeleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            SetButtonEnable(true);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportConstParam();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportMessureParam();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportMessureValue();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportCalcParam();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportCalcValue();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportRemark();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            var item = new Import.ImportTask();
            SetButtonEnable(false);
            this.DataContext = item;
            item.WorkCompeleted += item_WorkCompeleted;
            item.startWork();
        }


    }
}
