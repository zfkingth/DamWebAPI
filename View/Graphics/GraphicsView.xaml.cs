using System;
using System.Collections.Generic;
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
using DamWebAPI.ViewModel;
using DamWebAPI.ViewModel.Graphics;
using C1.WPF.C1Chart;
using hammergo.GlobalConfig;
using DevExpress.Xpf.Grid;
using DamServiceV3.Test.DamServiceRef;
using System.Collections.ObjectModel;



namespace DamWebAPI.View.Graphics
{
    /// <summary>
    /// GraphicsView.xaml 的交互逻辑
    /// </summary>
    public partial class GraphicsView : UserControl
    {
        public GraphicsView()
        {
            InitializeComponent();

            defRow.Height = new GridLength(PubConstant.ConfigData.GraphicHeight);
            defCol.Width = new GridLength(PubConstant.ConfigData.GraphicWidth);
        }


        private DamWebAPI.ViewModel.Graphics.GraphicsViewModel ViewModel
        {
            get
            {
                return this.DataContext as DamWebAPI.ViewModel.Graphics.GraphicsViewModel;
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            //only for test

            var selapp = listBoxApps.SelectedItem as App;




            ViewModel.AddAppInDS(selapp);


        }



        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                App selApp = e.AddedItems[0] as App;
                DamWebAPI.ViewModel.Entity.Graphics graInfo = ViewModel.CreateNewGraphicDS(selApp);

                CreateGhpics(graInfo);

                //只有显示一支仪器时才显示这条信息

                tbcaption.Text = string.Format("{0} {1} {2} {3}", selApp.AppName, selApp.X, selApp.Y, selApp.Z);




                propertyGrid.SelectedObject = new GraphicProperty(c1Chart, tby1, tby2, tbcaption);

            }
        }

        private void CreateGhpics(DamWebAPI.ViewModel.Entity.Graphics graInfo)
        {
            c1Chart.BeginUpdate();

            double dashPixels = ResetChart();

            //一次性查询所有的数据

            var results = from i in graInfo.Lines
                          where i.IsShow == true
                          group i by i.UnitSymbol;  //根据 物理量单位 判断是不是同一类量

            List<CalculateValue> allCalcValues = null;

            tby1.Text = tby2.Text = tbcaption.Text = "";

            int lineIndex = 0;
            //根据多少个Y轴进行循环
            for (int i = 0; i < results.Count(); i++)
            {
                if (allCalcValues == null)
                {
                    //避免多个枚举
                    allCalcValues = ViewModel.GetAllCalcValues(graInfo).ToList();
                }

                //有N个Y轴
                Axis yAxis = null;
                TextBlock tbTitle = null;
                if (i == 0)
                {
                    yAxis = c1Chart.View.AxisY;
                    yAxis.Name = "y1";

                    //set axis title
                    tbTitle = tby1;

                    yAxis.MajorGridStrokeDashes = new DoubleCollection(new double[] { dashPixels, dashPixels });


                }
                else
                {
                    //创建新的axis
                    yAxis = new Axis
                    {
                        AxisType = AxisType.Y,
                        Position = AxisPosition.Far,
                        Name = "y" + (i + 1).ToString(),
                    };

                    c1Chart.View.Axes.Add(yAxis);
                    tbTitle = tby2;

                    //hid grid lines
                    yAxis.MajorGridStroke = Brushes.White;
                    yAxis.MinorGridStroke = Brushes.White;

                }

                string unitSymbol = results.ElementAt(i).Key;

                var groupItems = results.ElementAt(i).ToList();

                //物理量名称
                string calcName = groupItems.First().ParamName;

                if (groupItems.Count > 1)
                {
                    //有多个相同单位的物理量
                    calcName = "单位";
                }

                tbTitle.Text = string.Format("{0} {1}: {2}", tbTitle.Text, calcName, unitSymbol);


                foreach (var item in groupItems)
                {
                    XYDataSeries ds = new XYDataSeries();
                    ds.Label = item.LegendName;
                    var valCollection = (from val in allCalcValues
                                         where val.ParamId == item.ParamId
                                         orderby val.Date
                                         select val);

                    ObservableCollection<CalculateValue> collection = new ObservableCollection<CalculateValue>(valCollection);
                    //消除异常值

                    HandleErrorValue(collection);

                    ds.ItemsSource = collection;

                    ds.XValueBinding = new Binding("Date.DateTime");
                    ds.ValueBinding = new Binding("Val");

                    //ds.XValuesSource = (from val in valCollection
                    //                    select val.Date.DateTime).ToArray();
                    //ds.ValuesSource = (from val in valCollection
                    //                   select val.Val).ToArray();

                    ds.AxisY = yAxis.Name;
                    //set line style
                    lineIndex = setLineStyle(lineIndex, ds);
                    c1Chart.Data.Children.Add(ds);
                }

            }
            c1Chart.EndUpdate();
        }

        private double ResetChart()
        {

            // Clear current chart c1Chart.Reset(true);
            c1Chart.Reset(true);

            c1Chart.ChartType = ChartType.Line;

            var xAxis = c1Chart.View.AxisX;
            double dashPixels = 3;

            xAxis.IsTime = true;
            xAxis.MajorGridStrokeDashes = new DoubleCollection(new double[] { dashPixels, dashPixels });
            return dashPixels;
        }

        private static void HandleErrorValue(IEnumerable<CalculateValue> valCollection)
        {
            foreach (var valItem in valCollection)
            {
                if (valItem.Val != null && valItem.Val.HasValue)
                {
                    if (Hammergo.Utility.Helper.isErrorValue(valItem.Val.Value))
                    {
                        valItem.Val = double.NaN;
                    }
                }
            }
        }

        private static int setLineStyle(int lineIndex, XYDataSeries ds)
        {
            if (lineIndex < PubConstant.ConfigData.LineStyleInfoList.Count)
            {
                var lineInfo = hammergo.GlobalConfig.PubConstant.ConfigData.LineStyleInfoList[lineIndex];
                ds.ConnectionStroke = lineInfo.ConnectionStroke;
                ds.ConnectionStrokeThickness = lineInfo.LineThickness;
                ds.ConnectionStrokeDashes = lineInfo.ConnectionStrokeDashes;
                ds.SymbolMarker = lineInfo.SymbolMarker;
                ds.SymbolSize = lineInfo.SymbolSize;
                ds.SymbolFill = lineInfo.SymbolFill;
                ds.SymbolStroke = lineInfo.SymbolStroke;
                ds.SymbolStrokeThickness = lineInfo.SymbolStrokeThickness;
                lineIndex++;
            }
            return lineIndex;
        }


        #region handle mouse on chart
        ToolTip m_toolTip = null;
        private void c1Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_toolTip == null)
            {
                m_toolTip = new ToolTip();


            }


            bool find = false;

            double dist;
            int si = 0;
            int index = -1;
            Point p = e.GetPosition(sender as IInputElement);
            for (; si < c1Chart.Data.Children.Count; si++)
            {

                index = c1Chart.View.DataIndexFromPoint(p, si, MeasureOption.XY, out dist);
                if (index >= 0 && dist < 3)
                {
                    //找到了附近的点
                    find = true;
                    break;
                }
            }

            if (find)
            {
                Cursor = Cursors.Cross;
                var ds = c1Chart.Data.Children[si] as XYDataSeries;

                //var xArray = ds.XValuesSource as System.DateTime[];
                //var yArray = ds.ValuesSource as double?[];

                IEnumerable<CalculateValue> source = ds.ItemsSource as IEnumerable<CalculateValue>;

                if (source != null && source.Count() > 0)
                {
                    var val = source.ElementAt(index);
                    string info = string.Format("{0}:{1} 日期:{2}", ds.Label, val.Val, (val.Date.DateTime).ToString("yyyy-MM-dd"));

                    m_toolTip.Content = new TextBlock { Text = info, TextWrapping = TextWrapping.Wrap };
                    m_toolTip.PlacementTarget = c1Chart;
                    //Point p = AvalonEdit.TextArea.TextView.GetVisualPosition(AvalonEdit.TextArea.Caret.Position, VisualYPosition.LineBottom);
                    m_toolTip.PlacementRectangle = new Rect(p.X + 10, p.Y + 10, 0, 0);
                    m_toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    m_toolTip.IsOpen = true;
                }

            }
            else
            {
                Cursor = Cursors.Arrow;
                m_toolTip.IsOpen = false;
            }

        }



        private void c1Chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                bool find = false;

                double dist;
                int si = 0;
                int index = -1;
                Point p = e.GetPosition(sender as IInputElement);
                for (; si < c1Chart.Data.Children.Count; si++)
                {

                    index = c1Chart.View.DataIndexFromPoint(p, si, MeasureOption.XY, out dist);
                    if (index >= 0 && dist < 3)
                    {
                        //找到了附近的点
                        find = true;
                        break;
                    }
                }

                if (find)
                {
                    propertyGrid.SelectedObject = new CustomProperty(c1Chart.Data.Children[si]);
                }

            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int cha = c1Chart.Data.Children.Count - PubConstant.ConfigData.LineStyleInfoList.Count;

            if (cha > 0)
            {

                for (int i = 0; i < cha; i++)
                {
                    PubConstant.ConfigData.LineStyleInfoList.Add(new LineStyleInfo());
                }
            }

            for (int i = 0; i < c1Chart.Data.Children.Count; i++)
            {
                DataSeries ds = c1Chart.Data.Children[i];
                LineStyleInfo lineStyle = PubConstant.ConfigData.LineStyleInfoList[i];

                lineStyle.ID = i;
                lineStyle.ConnectionStroke = ds.ConnectionStroke;
                lineStyle.LineThickness = ds.ConnectionStrokeThickness;
                lineStyle.ConnectionStrokeDashes = ds.ConnectionStrokeDashes;

                lineStyle.SymbolMarker = ds.SymbolMarker;

                if (double.IsInfinity(ds.SymbolSize.Width))
                {
                    ds.SymbolSize = new Size(0, 0);
                }

                lineStyle.SymbolSize = ds.SymbolSize;
                lineStyle.SymbolFill = ds.SymbolFill;
                lineStyle.SymbolStroke = ds.SymbolStroke;
                lineStyle.SymbolStrokeThickness = ds.SymbolStrokeThickness;




            }
            //保存图形的大小
            PubConstant.ConfigData.GraphicHeight = defRow.ActualHeight;
            PubConstant.ConfigData.GraphicWidth = defCol.ActualWidth;


            PubConstant.updateConfigData();
        }

        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            propertyGrid.SelectedObject = new GraphicProperty(c1Chart, tby1, tby2, tbcaption);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //var context = this.DataContext as DamWebAPI.ViewModel.Graphics.GraphicsViewModel;

            lineGrid.SetBinding(GridControl.ItemsSourceProperty, new Binding("GraphicDS.Lines") { Source = this.DataContext });

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CreateGhpics(ViewModel.GraphicDS);


            propertyGrid.SelectedObject = new GraphicProperty(c1Chart, tby1, tby2, tbcaption);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }



    }
}
