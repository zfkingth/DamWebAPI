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
using DamMVVM.ViewModel.DamService;
using DamMVVM.ViewModel;
using C1.WPF.C1Chart;
using hammergo.GlobalConfig;
using DevExpress.Xpf.Grid;

namespace DamMVVM.View.Graphics
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

        DamMVVM.ViewModel.Entity.Graphics graphicDS = new ViewModel.Entity.Graphics();





        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                App selApp = e.AddedItems[0] as App;
                AppIntegratedInfo appInfo = new AppIntegratedInfo(selApp, 0, null, null);
                //DamMVVM.ViewModel.Entity.Graphics graInfo = new ViewModel.Entity.Graphics();
                var results = from i in appInfo.CalcParams
                              group i by i.UnitSymbol;//根据物理量的符号判断是不是同一类量

                c1Chart.BeginUpdate();
                // Clear current chart c1Chart.Reset(true);
                c1Chart.Reset(true);
                tby1.Text = tby2.Text = "";
                // Set chart type c1Chart.ChartType = ChartType.XYPlot; 
                c1Chart.ChartType = ChartType.Line;
                // get axes
                var xAxis = c1Chart.View.AxisX;

                const double dashPixels = 3;

                xAxis.IsTime = true;
                xAxis.MajorGridStrokeDashes = new DoubleCollection(new double[] { dashPixels, dashPixels });



                var y1Axis = c1Chart.View.AxisY;
                y1Axis.Name = "y1";
                // configure Y axis

                //set legend position

                int lineIndex = 0;
                for (int i = 0; i < results.Count(); i++)
                {
                    //有N个Y轴
                    Axis yAxis = null;
                    TextBlock tbTitle = null;
                    if (i == 0)
                    {
                        yAxis = y1Axis;
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



                    foreach (var item in results.ElementAt(i))
                    {
                        // tbTitle.Text += " 单位:" + results.ElementAt(i).Key;
                        tbTitle.Text = string.Format("{0} {1}: {2}", tbTitle.Text, item.ParamName, results.ElementAt(i).Key);
                        XYDataSeries ds = new XYDataSeries();
                        ds.Label = item.ParamName;
                        var valCollection = (from val in appInfo.CalcValues
                                             where val.ParamId == item.ParamId
                                             select val).ToList();
                        //消除异常值

                        foreach (var valItem in valCollection)
                        {
                            if (valItem.Val != null && valItem.Val.HasValue)
                            {
                                if (Utility.Helper.isErrorValue(valItem.Val.Value))
                                {
                                    valItem.Val = double.NaN;
                                }
                            }
                        }


                        ds.XValuesSource = (from val in valCollection
                                            select val.Date).ToArray();
                        ds.ValuesSource = (from val in valCollection
                                           select val.Val).ToArray();

                        ds.AxisY = yAxis.Name;
                        //set line style
                        if (lineIndex < PubConstant.ConfigData.LineStyleInfoList.Count)
                        {
                            var lineInfo = hammergo.GlobalConfig.PubConstant.ConfigData.LineStyleInfoList[i];
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
                        c1Chart.Data.Children.Add(ds);
                    }

                }

                tbcaption.Text = appInfo.CurrentApp.AppName;

                c1Chart.EndUpdate();


                propertyGrid.SelectedObject = new GraphicProperty(c1Chart, tby1, tby2, tbcaption);

            }
        }


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

                var xArray = ds.XValuesSource as System.DateTime[];
                var yArray = ds.ValuesSource as double?[];
                if (xArray != null && yArray != null)
                {
                    string info = string.Format("{0}:{1} 日期:{2}", ds.Label, yArray[index], (xArray[index]).ToString("yyyy-MM-dd"));

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            propertyGrid.SelectedObject = new GraphicProperty(c1Chart, tby1, tby2, tbcaption);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            lineGrid.SetBinding(GridControl.ItemsSourceProperty, new Binding("Lines") { Source = graphicDS });

        }
    }
}
